using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Yuyushiki
{
    public class Play
    {
        readonly static int SECTION_TRAILER_FROM_S = 5;
        readonly static int POWER_RECEIVE_ERROR_S = 5;

        readonly MainForm mainForm;
        readonly System.Timers.Timer timer;
        readonly Plan plan;

        VfdWriter vfdWriter = null;
        VfdPanel vfdPanel = null;
        VfdFormat vfdFormat = null;
        int currentSectionIdx;
        List<PlanSection> sections = null;
        RollingAverage rollingAverage;
        DateTime lastPowerReceivedAt;
        DateTime pausedAt;

        public enum ConnectionStatus { Searching, Connected, SeachFailed, SeachFailedProcessing };
        volatile ConnectionStatus pmConnectionStatus;

        private int _deviceNumber = Properties.Settings.Default.DeviceNumber;
        public int DeviceNumber
        {
            get { return _deviceNumber; }
            set
            {
                if (_deviceNumber == value)
                    return;

                _deviceNumber = value;
                Properties.Settings.Default.DeviceNumber = value;
                Properties.Settings.Default.Save();
                UpdateStatusAsync();
            }
        }

        private string _comPort = Properties.Settings.Default.COMPort;
        public string ComPort
        {
            get { return _comPort; }
            set
            {
                if (_comPort == value)
                    return;

                _comPort = value;
                Properties.Settings.Default.COMPort = value;
                Properties.Settings.Default.Save();
                mainForm.UpdateStatus();
            }
        }

        private int _brightness = Properties.Settings.Default.Brightness;
        public int Brightness
        {
            get { return _brightness; }
            set
            {
                if (_brightness == value)
                    return;

                _brightness = value;
                Properties.Settings.Default.Brightness = value;
                Properties.Settings.Default.Save();
                if (vfdPanel != null)
                    vfdPanel.Brightness = value;
            }
        }

        private bool _isPausing = false;
        public bool IsPausing
        {
            get { return _isPausing; }
            set
            {
                if (_isPausing == value || !_isPlaying)
                    return;
                _isPausing = value;
                mainForm.UpdateStatus();
            }
        }


        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying == value)
                    return;

                if (value)
                {
                    plan.Parser.Text = mainForm.planTextBox.Text;
                    var error = plan.Parser.GetFirstError();
                    if (error != null)
                    {
                        mainForm.OverwritePlanStatusOther(error.Item3);
                        mainForm.planTextBox.Select(error.Item1, error.Item2);
                        mainForm.planTextBox.Select();
                        return;
                    }
                    sections = plan.Parser.Parse();
                }

                _isPlaying = value;
                _isPausing = false;

                mainForm.UpdateStatus();

                Network network;
                try
                {
                    network = Network.GetInstance();
                }
                catch (Exception)
                {
                    MessageBox.Show("ANT+ stick is not ready.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    _isPlaying = false;
                    mainForm.UpdateStatus();
                    return;
                }
                
                if (_isPlaying)
                {
                    currentSectionIdx = 0;
                    sections[0].PlayStartedAt = DateTime.MinValue;
                    plan.Reset();
                    rollingAverage = new RollingAverage();
                    lastPowerReceivedAt = DateTime.Now;
                    pausedAt = DateTime.MinValue;

                    network.OnSearchEnded += Network_OnSearchEnded;
                    network.OnAccumPowerReceived += Network_OnAccumPowerReceived;
                    vfdWriter = new VfdWriterImpl(ComPort);
                    vfdPanel = new VfdPanel(vfdWriter, Brightness);
                    vfdFormat = new VfdFormat(vfdPanel);
                    pmConnectionStatus = ConnectionStatus.Searching;
                    network.OpenChannel(0, (ushort)DeviceNumber);
                    mainForm.OverwritePlanStatus(MainForm.PlanStatus.Connecting);

                    timer.Start();
                }
                else
                {
                    timer.Stop();

                    network.OnSearchEnded -= Network_OnSearchEnded;
                    network.OnAccumPowerReceived -= Network_OnAccumPowerReceived;
                    if (network.CloseAllChannel().WaitOne(1000))
                    {
                        vfdWriter?.Dispose();
                        vfdWriter = null;
                        vfdFormat = null;
                        vfdPanel = null;
                    }
                    else
                    {
                        MessageBox.Show("Error", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public bool HasNextSection
        {
            get { return currentSectionIdx + 1 < sections.Count; }
        }

        public bool HasPrevSection
        {
            get { return currentSectionIdx > 0; }
        }

        public Play(MainForm mainForm, Plan plan)
        {
            this.plan = plan;
            this.mainForm = mainForm;
            this.timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;
        }

        public void ChangePlayingSection(int diff)
        {
            var now = DateTime.Now;
            plan.AccumJ(plan.RecentPower, 1, now, sections[currentSectionIdx]);
            currentSectionIdx += diff;
            vfdFormat.SectionPower = -1;
            PlayingSectionChanged(now);
        }

        private void PlayingSectionChangedUIThread()
        {
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new Action(PlayingSectionChangedUIThread));
                return;
            }
            if (currentSectionIdx == sections.Count)
            {
                mainForm.OverwritePlanStatus(MainForm.PlanStatus.Complete);
                IsPlaying = false;
            }
            mainForm.UpdateStatus();
        }

        private void PlayingSectionChanged(DateTime now)
        {
            if (currentSectionIdx < sections.Count)
            {
                var s = sections[currentSectionIdx];
                s.AccumMilliJ = 0;
                s.PlayStartedAt = now;
                vfdFormat.TargetPower = s.TargetPower;
                vfdFormat.Desc = s.Desc;
                vfdFormat.LoopIndex = s.LoopIndex;
                vfdFormat.DurationString = Plan.BuildDurationString(s.Duration);
            }
            else
            {
                vfdPanel.Overwrite("   Plan Complete    ", 0, 0);
                vfdPanel.Overwrite("     Yuyushiki      ", 1, 0);
            }
            vfdPanel.Update();
            PlayingSectionChangedUIThread();
        }

        private void ShowVfdAveW(DateTime now)
        {
            vfdFormat.SectionPower = plan.AveJ(now, sections[currentSectionIdx]);
        }

        private void PmSeachFailed()
        {
            pmConnectionStatus = ConnectionStatus.SeachFailedProcessing;
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new Action(PmSeachFailed));
                return;
            }
            mainForm.OverwritePlanStatus(MainForm.PlanStatus.SearchFailed);
            IsPlaying = false;
            MessageBox.Show("Cannot connect to the power meter.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            DeviceNumber = -1;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (pmConnectionStatus != ConnectionStatus.Connected)
            {
                if (pmConnectionStatus == ConnectionStatus.SeachFailed)
                    PmSeachFailed();
                return;
            }

            var now = DateTime.Now;
            var s = sections[currentSectionIdx];

            if ((now - lastPowerReceivedAt).TotalSeconds > POWER_RECEIVE_ERROR_S)
                vfdFormat.RecentPower = -1;

            if (s.PlayStartedAt == DateTime.MinValue)
            {
                // first event of first section
                plan.AccumJ(plan.RecentPower, 1, now, s);
                PlayingSectionChanged(now);
            }

            if (IsPausing)
            {
                if (pausedAt == DateTime.MinValue)
                    pausedAt = now;
                vfdPanel.Update();
                OverwritePlanStatusAsync(MainForm.PlanStatus.Pausing);
                return;
            }
            else if (pausedAt != DateTime.MinValue)
            {
                OverwritePlanStatusAsync(MainForm.PlanStatus.Running);
                var pausedSpan = now - pausedAt;
                plan.AddPausedSpan(pausedSpan, s);
                pausedAt = DateTime.MinValue;
            }

            if (s.Duration == 0)
            {
                var past = now - s.PlayStartedAt;
                vfdFormat.DurationString = Plan.BuildDurationString((int)Math.Round(past.TotalSeconds));
                ShowVfdAveW(now);
            }
            else
            {
                var durationEndsAt = s.PlayStartedAt + new TimeSpan(0, 0, s.Duration);
                var last = durationEndsAt - now;
                int lastSeconds = (int)Math.Round(last.TotalSeconds);
                if (lastSeconds <= 0)
                {
                    plan.AccumJ(plan.RecentPower, 1, now, s);
                    currentSectionIdx++;
                    vfdFormat.SectionPower = -1;
                    PlayingSectionChanged(now);
                    return;
                }
                else
                {
                    vfdFormat.DurationString = Plan.BuildDurationString(lastSeconds);
                    ShowVfdAveW(now);
                    if (lastSeconds <= SECTION_TRAILER_FROM_S && currentSectionIdx + 1 < sections.Count)
                    {
                        vfdPanel.Overwrite("Next:", 1, 0);
                        vfdFormat.Desc = sections[currentSectionIdx + 1].Desc;
                        vfdFormat.LoopIndex = sections[currentSectionIdx + 1].LoopIndex;
                    }
                }
            }
            vfdPanel.Update();
        }

        private void Network_OnAccumPowerReceived(Network sender, byte antChannel, int accumPower, int count)
        {
            if (!IsPlaying)
                return;

            var now = DateTime.Now;
            lastPowerReceivedAt = now;
            rollingAverage.Append(now, accumPower, count);
            vfdFormat.RecentPower = rollingAverage.Average(now);
            vfdPanel.Update();
            if (pausedAt == DateTime.MinValue)
                plan.AccumJ(accumPower, count, now, sections[currentSectionIdx]);
        }

        private void Network_OnSearchEnded(Network sender, byte antChannel, bool isConnected, int deviceNumber)
        {
            if (!IsPlaying)
                return;

            OverwritePlanStatusAsync(isConnected ? MainForm.PlanStatus.Running : MainForm.PlanStatus.SearchFailed);
            pmConnectionStatus = isConnected ? ConnectionStatus.Connected : ConnectionStatus.SeachFailed;
        }

        void UpdateStatusAsync()
        {
            if (mainForm.InvokeRequired)
                mainForm.Invoke(new Action(UpdateStatusAsync));
            else
                mainForm.UpdateStatus();
        }

        private void OverwritePlanStatusAsync(MainForm.PlanStatus s)
        {
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(new Action<MainForm.PlanStatus>(OverwritePlanStatusAsync), s);
                return;
            }
            mainForm.OverwritePlanStatus(s);
        }

        public void TestVfd()
        {
            var pl = VfdWriter.EnumerateComPortName();
            if (pl.Contains(ComPort))
            {
                try
                {
                    using (var w = new VfdWriterImpl(ComPort))
                    {
                        var panel = new VfdPanel(w, Brightness);
                        panel.Overwrite("Connected to", 0, 0);
                        panel.Overwrite("Yuyushiki", 1, 11);
                        panel.Update();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Selected COM port is not ready.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                mainForm.ResetComPortComboBoxItems();
                MessageBox.Show(ComPort + " is missing.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            mainForm.UpdateStatus();
        }
    }

    class VfdFormat
    {
        readonly VfdPanel vfdPanel;

        public VfdFormat(VfdPanel panel)
        {
            this.vfdPanel = panel;
        }

        public int SectionPower
        {
            set
            {
                if (value == -1)
                {
                    vfdPanel.Overwrite("---W", 0, 0);
                    return;
                }

                var sectionAvePowerStr = value.ToString();
                if (sectionAvePowerStr.Length > 3)
                    sectionAvePowerStr = sectionAvePowerStr.Substring(sectionAvePowerStr.Length - 3);
                vfdPanel.Overwrite(String.Format("{0,3}W", sectionAvePowerStr), 0, 0);
            }
        }

        public int TargetPower
        {
            set { vfdPanel.Overwrite(String.Format("{0,3}W ", value), 1, 0); }
        }

        public string Desc
        {
            set { vfdPanel.Overwrite(String.Format("{0, -15}", value), 1, 5); }
        }

        public string DurationString
        {
            set { vfdPanel.Overwrite(value, 0, 5); }
        }

        public int RecentPower
        {
            set
            {
                if (value == -1)
                {
                    vfdPanel.Overwrite("  N/A", 0, 15);
                    return;
                }
                var aveStr = value.ToString();
                if (aveStr.Length > 4)
                    aveStr = aveStr.Substring(aveStr.Length - 4);
                vfdPanel.Overwrite(String.Format("{0,4}W", aveStr), 0, 15);
            }
        }

        public List<int> LoopIndex
        {
            set
            {
                if (value.Count == 0)
                    return;
                var s = "";
                var buf = new List<int>();
                buf.AddRange(value);
                buf.Reverse();
                foreach (var i in buf)
                {
                    if (s.Length == 0)
                        s = " #" + (i + 1).ToString();
                    else
                        s += "-" + (i + 1).ToString();
                }
                vfdPanel.Overwrite(s, 1, 20 - s.Length);
            }
        }
    }

    class RollingAverage
    {
        static readonly TimeSpan AVERAGE_SPAN = new TimeSpan(0, 0, 0, 5, 500); // 5.5 sec

        Dictionary<DateTime, Tuple<int, int>> samples = new Dictionary<DateTime, Tuple<int, int>>();

        public void Append(DateTime now, int accumPower, int count)
        {
            samples[now] = new Tuple<int, int>(accumPower, count);
        }

        public int Average(DateTime now)
        {
            Debug.WriteLine("RollingAverage.Average: samples count before filtering:" + samples.Count.ToString());
            var sl = new List<KeyValuePair<DateTime, Tuple<int, int>>>();
            KeyValuePair<DateTime, Tuple<int, int>>? lastKvp = null;
            foreach (var kvp in samples.OrderBy(e => e.Key))
            {
                if (lastKvp == null)
                {
                    lastKvp = kvp;
                    continue;
                }
                if (kvp.Key >= (now - AVERAGE_SPAN))
                {
                    if (lastKvp != null)
                    {
                        sl.Add((KeyValuePair<DateTime, Tuple<int, int>>)lastKvp);
                        lastKvp = null;
                    }
                    sl.Add(kvp);
                }
            }
            if (lastKvp != null)
                sl.Add((KeyValuePair<DateTime, Tuple<int, int>>)lastKvp);
            samples = sl.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Debug.WriteLine("RollingAverage.Average: samples count after filtering:" + samples.Count.ToString());
            if (samples.Count == 0)
                return -1;
            else if (samples.Count == 1)
            {
                var v = samples.Values.First();
                return (int)Math.Round(((double)v.Item1) / v.Item2);
            }

            var tb = DateTime.MinValue;
            double accumMilliJ = 0.0;
            foreach (var v in sl)
            {
                if (tb == DateTime.MinValue)
                {
                    tb = v.Key;
                    continue;
                }
                accumMilliJ += (v.Value.Item1 * (v.Key - tb).TotalMilliseconds) / v.Value.Item2;
                tb = v.Key;
            }
            var spanMs = (sl.Last().Key - sl.First().Key).TotalMilliseconds;
            return (int)Math.Round(accumMilliJ / spanMs);
        }
    }
}

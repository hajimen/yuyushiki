using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Yuyushiki
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public enum PlanStatus { Running, Complete, Connecting, SearchFailed, Pausing, Stopped, ComPortNotReady, Empty, Other };
        PlanStatus planStatus;
        public static readonly IDictionary<PlanStatus, string> PlanStatusDic = new Dictionary<PlanStatus, string>();
        static MainForm()
        {
            PlanStatusDic[PlanStatus.Running] = "Running...";
            PlanStatusDic[PlanStatus.Complete] = "Plan Complete.";
            PlanStatusDic[PlanStatus.Connecting] = "Connecting to the power meter...";
            PlanStatusDic[PlanStatus.SearchFailed] = "Cannot connect to the power meter.";
            PlanStatusDic[PlanStatus.Pausing] = "Pausing.";
            PlanStatusDic[PlanStatus.Stopped] = "Stopped.";
            PlanStatusDic[PlanStatus.ComPortNotReady] = "COM port is not ready.";
            PlanStatusDic[PlanStatus.Empty] = "";
        }

        SearchPmForm searchPmForm = new SearchPmForm();
        Play play;
        Plan plan = new Plan();
        RadioButton[] brightnessButtons;
        bool _isPlanTextModified;
        bool IsPlanTextModified
        {
            get { return _isPlanTextModified; }
            set
            {
                if (_isPlanTextModified == value)
                    return;
                _isPlanTextModified = value;
                UpdateTitle();
                saveToolStripMenuItem.Enabled = IsPlanTextModified;
                toolStripSaveButton.Enabled = IsPlanTextModified;
            }
        }
        bool isNewPlan;

        private void MainForm_Shown(object sender, EventArgs e)
        {
            play = new Play(this, plan);

            brightnessButtons = new RadioButton[]{ vfdBrightnessRadioButton1,
                vfdBrightnessRadioButton2, vfdBrightnessRadioButton3, vfdBrightnessRadioButton4};
            foreach (var b in brightnessButtons)
                b.CheckedChanged += VfdBrightnessRadioButton_CheckedChanged;
            for (int i = 0; i < brightnessButtons.Length; i++)
                brightnessButtons[i].Checked = (play.Brightness == i);

            try
            {
                var lpf = Properties.Settings.Default.LastPlanFilePath;
                if (lpf.Length > 0 && Directory.Exists(Path.GetDirectoryName(lpf)))
                    ReadFile(lpf);
                else
                    NewFile();
            }
            catch (Exception)
            {
                NewFile();
            }

            OverwritePlanStatus(PlanStatus.Empty);
            ResetComPortComboBoxItems();
            UpdateStatus();
        }

        private void VfdBrightnessRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < brightnessButtons.Length; i++)
            {
                if (sender == brightnessButtons[i])
                    play.Brightness = i;
            }
        }

        private void UpdateTitle()
        {
            if (IsPlanTextModified)
                Text = Path.GetFileNameWithoutExtension(plan.FilePath) + " * - Yuyushiki";
            else
                Text = Path.GetFileNameWithoutExtension(plan.FilePath) + " - Yuyushiki";
        }

        public void OverwritePlanStatusOther(string s)
        {
            planStatus = PlanStatus.Other;
            planStatusLabel.Text = s;
        }

        public void OverwritePlanStatus(PlanStatus s)
        {
            if (s == planStatus)
                return;
            planStatus = s;
            planStatusLabel.Text = PlanStatusDic[s];
        }

        public void ResetComPortComboBoxItems()
        {
            var pl = VfdWriter.EnumerateComPortName();
            var items = comPortComboBox.Items;
            items.Clear();
            foreach (var s in pl)
                items.Add(s);
            comPortComboBox.SelectedIndex = pl.Contains(play.ComPort) ? items.IndexOf(play.ComPort) : -1;
            comPortComboBox.Enabled = (!play.IsPlaying) && (pl.Count > 0);
            if (planStatus == PlanStatus.ComPortNotReady)
                OverwritePlanStatus(PlanStatus.Empty);
        }

        public void UpdateStatus()
        {
            vfdTestButton.Enabled = (!play.IsPlaying) && (comPortComboBox.SelectedIndex != -1);

            if (comPortComboBox.SelectedIndex == -1)
                OverwritePlanStatus(PlanStatus.ComPortNotReady);

            planTextBox.Enabled = !play.IsPlaying;

            playButton.Enabled = play.DeviceNumber != -1 && (comPortComboBox.SelectedIndex != -1);
            playButton.Image = ((!play.IsPlaying) || play.IsPausing) ? Properties.Resources.Run_16x : Properties.Resources.Pause_16x;
            stopButton.Enabled = play.IsPlaying;
            nextButton.Enabled = play.IsPlaying && (!play.IsPausing) && play.HasNextSection;
            prevButton.Enabled = play.IsPlaying && (!play.IsPausing) && play.HasPrevSection;
            searchPmButton.Enabled = !play.IsPlaying;

            menuStrip.Enabled = !play.IsPlaying;
            toolStrip.Enabled = !play.IsPlaying;

            UpdateTitle();
        }

        private void searchPmButton_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            searchPmForm.DeviceNumber = play.DeviceNumber;
            if (searchPmForm.ShowDialog() == DialogResult.OK)
            {
                play.DeviceNumber = searchPmForm.DeviceNumber;
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (play.IsPlaying)
            {
                play.IsPausing = !play.IsPausing;
            }
            else
            {
                try
                {
                    play.IsPlaying = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    MessageBox.Show("Error", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    play.IsPlaying = false;
                }
            }
        }

        private void comPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pl = VfdWriter.EnumerateComPortName();
            if (pl.Contains(comPortComboBox.SelectedItem))
            {
                play.ComPort = (string)comPortComboBox.SelectedItem;
            }
            UpdateStatus();
        }

        private void vfdTestButton_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            play.TestVfd();
        }

        private void ReadFile(string filePath)
        {
            plan.ReadFile(filePath);
            planTextBox.Text = plan.Parser.Text;
            IsPlanTextModified = false;
            isNewPlan = false;
            UpdateTitle();
        }

        private void OpenFile()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ReadFile(dlg.FileName);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Cannot read this file.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private void SaveFile()
        {
            if (isNewPlan)
            {
                SaveAsFile();
                return;
            }
            try
            {
                plan.Parser.Text = planTextBox.Text;
                plan.WriteFile(false);
                IsPlanTextModified = false;
                UpdateTitle();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot save this file.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void SaveAsFile()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.FileName = Path.GetFileName(plan.FilePath);
                if (Directory.Exists(Path.GetDirectoryName(plan.FilePath)))
                    dlg.InitialDirectory = Path.GetDirectoryName(plan.FilePath);
                dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FilterIndex = 1;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        plan.FilePath = dlg.FileName;
                        plan.Parser.Text = planTextBox.Text;
                        plan.WriteFile(true);
                        IsPlanTextModified = false;
                        isNewPlan = false;
                        UpdateTitle();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Cannot save this file.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private void NewFile()
        {
            var l = new List<string>();
            l.Add("# Plan Example. Remove '#' to enable each section.");
            l.Add("#160W\t20m\tWarming Up");
            l.Add("#260W\t4m\tInterval 1-1");
            l.Add("#160W\t3m\tRest 1-1");
            l.Add("#260W\t4m\tInterval 1-2");
            l.Add("#160W\t3m\tRest 1-2");
            l.Add("#260W\t4m\tInterval 1-3");
            l.Add("#160W\t1h30m\tLSD");
            l.Add("#260W\t4m\tInterval 2-1");
            l.Add("#160W\t3m\tRest 2-1");
            l.Add("#260W\t4m\tInterval 2-2");
            l.Add("#160W\t3m\tRest 2-2");
            l.Add("#260W\t4m\tInterval 2-3");
            l.Add("#160W\t0\tLSD");
            l.Add("# Duration 0 is unlimited count up mode.");
            var t = String.Join("\r\n", l);
            planTextBox.Text = t;
            plan.FilePath = "New Plan.txt";
            IsPlanTextModified = false;
            isNewPlan = true;
            UpdateTitle();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            MessageBox.Show("Yuyushiki version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            play.IsPlaying = false;
            OverwritePlanStatus(PlanStatus.Stopped);
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            play.ChangePlayingSection(-1);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            play.ChangePlayingSection(1);
        }

        private void newFile_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            NewFile();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            OpenFile();
        }

        private void save_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            SaveFile();
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            OverwritePlanStatus(PlanStatus.Empty);
            SaveAsFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void planTextBox_TextChanged(object sender, EventArgs e)
        {
            IsPlanTextModified = true;
            UpdateTitle();
            OverwritePlanStatus(PlanStatus.Empty);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsPlanTextModified)
            {
                var ret = MessageBox.Show("Save this plan?", "Yuyushiki", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (ret == DialogResult.Yes)
                    SaveFile();
                else if (ret == DialogResult.No)
                { }
                else if (ret == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (!isNewPlan && Directory.Exists(Path.GetDirectoryName(plan.FilePath)))
                Properties.Settings.Default.LastPlanFilePath = plan.FilePath;
            else
                Properties.Settings.Default.LastPlanFilePath = "";
            Properties.Settings.Default.Save();
        }

        private void comPortComboBox_Click(object sender, EventArgs e)
        {
            ResetComPortComboBoxItems();
            UpdateStatus();
        }
    }
}

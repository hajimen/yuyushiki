using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yuyushiki
{
    public partial class SearchPmForm : Form
    {
        public SearchPmForm()
        {
            InitializeComponent();
        }

        public int DeviceNumber;

        int antChannelIdx;
        Dictionary<byte, ComboBoxItem> items = new Dictionary<byte, ComboBoxItem>();
        Network network = null;

        private void SearchPmForm_Shown(object sender, EventArgs e)
        {
            pmComboBox.Items.Clear();
            pmComboBox.Enabled = false;
            okButton.Enabled = false;
            antChannelIdx = 0;
            try
            {
                network = Network.GetInstance();
                network.OnSearchEnded += Network_OnSearchEnded;
                network.OnAccumPowerReceived += Network_OnAccumPowerReceived;
                network.OpenSearchChannel(0);
            }
            catch (Exception)
            {
                network = null;
                MessageBox.Show("ANT+ stick is not ready.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }
        }

        private void PowerReceived(byte antChannel, int power)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<byte, int>(PowerReceived), antChannel, power);
            }
            else
            {
                if (items.ContainsKey(antChannel))
                {
                    var cbi = items[antChannel];
                    cbi.Power = power;
                    var i = pmComboBox.Items.IndexOf(cbi);
                    pmComboBox.Items[i] = cbi;
                }
            }
        }

        private void Network_OnAccumPowerReceived(Network sender, byte antChannel, int accumPower, int count)
        {
            PowerReceived(antChannel, accumPower / count);
        }

        private void Connected(byte antChannel, int deviceNumber)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<byte, int>(Connected), antChannel, deviceNumber);
            }
            else
            {
                okButton.Enabled = true;
                var cbi = new ComboBoxItem(deviceNumber);
                lock (this)
                {
                    items[antChannel] = cbi;
                }
                pmComboBox.Items.Add(cbi);
                if (!pmComboBox.Enabled)
                {
                    pmComboBox.Enabled = true;
                    pmComboBox.SelectedIndex = 0;
                }
            }
        }

        private void SearchFinished()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(SearchFinished));
            }
            else
            {
                statusLabel.Text = "Seach finished.";
            }
        }

        private void Network_OnSearchEnded(Network sender, byte antChannel, bool isConnected, int deviceNumber)
        {
            if (isConnected)
            {
                lock (this)
                {
                    antChannelIdx++;
                    sender.OpenSearchChannel(antChannelIdx);
                }
                Connected(antChannel, deviceNumber);
            }
            else
            {
                SearchFinished();
            }
        }

        private void SearchPmForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (network != null)
            {
                network.OnAccumPowerReceived -= Network_OnAccumPowerReceived;
                network.OnSearchEnded -= Network_OnSearchEnded;
                if (!network.CloseAllChannel().WaitOne(1000))
                {
                    MessageBox.Show("ANT+ stick has some trouble.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                if (pmComboBox.SelectedIndex == -1)
                {
                    DeviceNumber = -1;
                }
                else
                {
                    var si = (ComboBoxItem)pmComboBox.SelectedItem;
                    DeviceNumber = si.DeviceNumber;
                }
            }
        }
    }

    class ComboBoxItem
    {
        public readonly int DeviceNumber;
        public int Power = -1;

        public ComboBoxItem(int deviceNumber)
        {
            this.DeviceNumber = deviceNumber;
        }

        public override string ToString()
        {
            if (Power == -1)
            {
                return String.Format("ID:{0:D5} ----W", DeviceNumber);
            }
            else
            {
                return String.Format("ID:{0:D5} {1, 4}W", DeviceNumber, Power);
            }
        }
    }
}

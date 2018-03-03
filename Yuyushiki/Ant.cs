using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ANT_Managed_Library;
using ANTEventID = ANT_Managed_Library.ANT_ReferenceLibrary.ANTEventID;
using ANTMessageID = ANT_Managed_Library.ANT_ReferenceLibrary.ANTMessageID;
using System.Threading;

namespace Yuyushiki
{
    class PowerChannel
    {
        static readonly byte HRM_DEVICE_TYPE = 120;
        static readonly byte POWER_DEVICE_TYPE = 11;
        // static readonly byte POWER_DEVICE_TYPE = HRM_DEVICE_TYPE; // to debug
        static readonly byte STANDARD_POWER_ONLY_DATA_PAGE = 16;
        static readonly int STANDARD_POWER_ONLY_MSG_UPDATE_EVENT_COUNT_IDX = 1;
        static readonly int STANDARD_POWER_ONLY_MSG_ACCUMLATED_POWER_LSB_IDX = 4;
        static readonly int STANDARD_POWER_ONLY_MSG_ACCUMLATED_POWER_MSB_IDX = 5;
        static readonly byte USER_TRANSTYPE = 0;           // Transmission type = 2 way
        static readonly byte USER_RADIOFREQ = 0x39;          // RF Frequency + 2400 MHz
        static readonly ushort USER_DEVICE_NUM = 0;
        static readonly ushort USER_CH_PERIOD = 8192;

        readonly ANT_Channel channel;

        volatile bool isBroadcasting = false;
        volatile bool isClosed = false;
        volatile bool onceConnected = false;
        volatile byte[] txBuffer = { 0, 0, 0, 0, 0, 0, 0, 0 };

        byte lastRawUpdateEventCount;
        ushort lastRawAccumPower;
        ushort debug_rawAccumHeartRate = 0;

        public delegate void SearchEventHandler(PowerChannel sender, bool isConnected, int deviceNumber);
        public event SearchEventHandler OnSearchEnded;

        public delegate void AccumPowerReceivedHandler(PowerChannel sender, int accumPower, int count);
        public event AccumPowerReceivedHandler OnAccumPowerReceived;

        public delegate void ClosedHandler(PowerChannel sender);
        public event ClosedHandler OnClosed;

        void Closed()
        {
            Debug.WriteLine("unassign channel");
            if (channel.unassignChannel(500))
            {
                isClosed = true;
                OnClosed?.Invoke(this);
                channel.channelResponse -= ChannelResponse;
                OnSearchEnded = null;
                OnAccumPowerReceived = null;
                OnClosed = null;
            }
        }

        void ChannelEventResponse(ANT_Response response)
        {
            switch (response.getChannelEventCode())
            {
                // This event indicates that a message has just been
                // sent over the air. We take advantage of this event to set
                // up the data for the next message period.   
                case ANTEventID.EVENT_TX_0x03:
                    {
                        txBuffer[0]++;  // Increment the first byte of the buffer

                        // Broadcast data will be sent over the air on
                        // the next message period
                        if (isBroadcasting)
                        {
                            channel.sendBroadcastData(txBuffer);
                        }
                        break;
                    }
                case ANTEventID.EVENT_RX_SEARCH_TIMEOUT_0x01:
                    {
                        Debug.WriteLine("Search Timeout");
                        OnSearchEnded(this, false, 0);
                        Closed();
                        break;
                    }
                case ANTEventID.EVENT_RX_FAIL_0x02:
                    {
                        // Rx Fail
                        break;
                    }
                case ANTEventID.EVENT_TRANSFER_RX_FAILED_0x04:
                    {
                        // Burst receive has failed
                        break;
                    }
                case ANTEventID.EVENT_TRANSFER_TX_COMPLETED_0x05:
                    {
                        // Transfer Completed
                        break;
                    }
                case ANTEventID.EVENT_TRANSFER_TX_FAILED_0x06:
                    {
                        // Transfer Failed
                        break;
                    }
                case ANTEventID.EVENT_CHANNEL_CLOSED_0x07:
                    {
                        Debug.WriteLine("Channel closed");
                        // This event should be used to determine that the channel is closed.
                        Closed();
                        break;
                    }
                case ANTEventID.EVENT_RX_FAIL_GO_TO_SEARCH_0x08:
                    {
                        // Go to Search
                        break;
                    }
                case ANTEventID.EVENT_CHANNEL_COLLISION_0x09:
                    {
                        Debug.WriteLine("Channel Collision");
                        break;
                    }
                case ANTEventID.EVENT_TRANSFER_TX_START_0x0A:
                    {
                        // Burst Started
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Unhandled Channel Event " + response.getChannelEventCode());
                        break;
                    }
            }
        }

        Tuple<int, int> ProcessRollOver(ushort rawAccumPower, byte rawUpdateEventCount)
        {
            int accumPower;
            if (lastRawAccumPower > rawAccumPower)
            {
                // rollover occured
                int iRawAccumPower = rawAccumPower + (1 << 16);
                accumPower = iRawAccumPower - lastRawAccumPower;
            }
            else
                accumPower = rawAccumPower - lastRawAccumPower;
            lastRawAccumPower = rawAccumPower;

            int count;
            if (lastRawUpdateEventCount > rawUpdateEventCount)
            {
                // rollover occured
                int iRawUpdateEventCount = rawUpdateEventCount + (1 << 8);
                count = iRawUpdateEventCount - lastRawUpdateEventCount;
            }
            else
                count = rawUpdateEventCount - lastRawUpdateEventCount;
            lastRawUpdateEventCount = rawUpdateEventCount;

            return new Tuple<int, int>(accumPower, count);
        }

        void ChannelDataResponse(ANT_Response response)
        {
            if (response.isExtended()) // Check if we are dealing with an extended message
            {
                ANT_ChannelID chID = response.getDeviceIDfromExt();    // Channel ID of the device we just received a message from
                var pl = response.getDataPayload();

                if (POWER_DEVICE_TYPE == HRM_DEVICE_TYPE && chID.deviceTypeID == HRM_DEVICE_TYPE) // to debug
                {
                    byte hr = pl[7];

                    // making mock data
                    debug_rawAccumHeartRate += hr;
                    ushort rawAccumPower = debug_rawAccumHeartRate;
                    byte rawUpdateEventCount = lastRawUpdateEventCount;
                    rawUpdateEventCount++;

                    var t = ProcessRollOver(rawAccumPower, rawUpdateEventCount);

                    OnAccumPowerReceived(this, t.Item1, t.Item2);
                }
                else if (chID.deviceTypeID == POWER_DEVICE_TYPE)
                {
                    if (pl[0] == STANDARD_POWER_ONLY_DATA_PAGE)
                    {
                        byte rawUpdateEventCount = pl[STANDARD_POWER_ONLY_MSG_UPDATE_EVENT_COUNT_IDX];

                        if (rawUpdateEventCount != lastRawUpdateEventCount)
                        {
                            ushort rawAccumPower = pl[STANDARD_POWER_ONLY_MSG_ACCUMLATED_POWER_MSB_IDX];
                            rawAccumPower = (ushort)((rawAccumPower << 8) + pl[STANDARD_POWER_ONLY_MSG_ACCUMLATED_POWER_LSB_IDX]);
                            var t = ProcessRollOver(rawAccumPower, rawUpdateEventCount);
                            OnAccumPowerReceived(this, t.Item1, t.Item2);
                        }
                    }
                }
                if (!onceConnected)
                {
                    onceConnected = true;
                    channel.setChannelSearchTimeout(255); // infinite
                    OnSearchEnded(this, true, chID.deviceNumber);
                }
                Debug.Write("Ch ID(" + chID.deviceNumber.ToString() + "," + chID.deviceTypeID.ToString() + "," + chID.transmissionTypeID.ToString() + ") - ");
            }

            switch ((ANTMessageID)response.responseID)
            {
                case ANTMessageID.BROADCAST_DATA_0x4E:
                case ANTMessageID.EXT_BROADCAST_DATA_0x5D:
                    Debug.Write("Rx:(" + response.antChannel.ToString() + "): ");
                    break;
                case ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
                case ANTMessageID.EXT_ACKNOWLEDGED_DATA_0x5E:
                    Debug.Write("Acked Rx:(" + response.antChannel.ToString() + "): ");
                    break;
                default:
                    Debug.Write("Burst(" + response.getBurstSequenceNumber().ToString("X2") + ") Rx:(" + response.antChannel.ToString() + "): ");
                    break;
            }

            Debug.WriteLine(BitConverter.ToString(response.getDataPayload()));
        }

        void ChannelResponse(ANT_Response response)
        {
            try
            {
                switch ((ANTMessageID)response.responseID)
                {
                    case ANTMessageID.RESPONSE_EVENT_0x40:
                        ChannelEventResponse(response);
                        break;
                    case ANTMessageID.BROADCAST_DATA_0x4E:
                    case ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
                    case ANTMessageID.BURST_DATA_0x50:
                    case ANTMessageID.EXT_BROADCAST_DATA_0x5D:
                    case ANTMessageID.EXT_ACKNOWLEDGED_DATA_0x5E:
                    case ANTMessageID.EXT_BURST_DATA_0x5F:
                        ChannelDataResponse(response);
                        break;
                    default:
                        Debug.WriteLine("Unknown Message " + response.responseID);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Channel response processing failed with exception: " + ex.Message);
            }
        }

        public void Open()
        {
            // Opening channel...
            isBroadcasting = true;
            isClosed = false;
            if (channel.openChannel(500))
            {
                Debug.WriteLine("Channel opened");
            }
            else
            {
                isBroadcasting = false;
                throw new Exception("Error opening channel");
            }
        }

        public void Close()
        {
            if (!isClosed)
            {
                isBroadcasting = false;
                channel.closeChannel();
            }
        }

        public PowerChannel(ANT_Device device, int ant_channel, byte network_num, ushort deviceNumber)
        {
            channel = device.getChannel(ant_channel);    // Get channel from ANT device
            channel.channelResponse += ChannelResponse;  // Add channel response function to receive channel event messages

            if (!channel.assignChannel(ANT_ReferenceLibrary.ChannelType.BASE_Slave_Receive_0x00, network_num, 500))
                throw new Exception("Error assigning channel");

            if (!channel.setChannelID(deviceNumber, false, POWER_DEVICE_TYPE, USER_TRANSTYPE, 500))
                throw new Exception("Error configuring Channel ID");

            if (!channel.setChannelFreq(USER_RADIOFREQ, 500))
                throw new Exception("Error configuring Radio Frequency");

            if (!channel.setChannelPeriod(USER_CH_PERIOD, 500))
                throw new Exception("Error configuring Channel Period");
        }

        public PowerChannel(ANT_Device device, int ant_channel, byte network_num) : this(device, ant_channel, network_num, USER_DEVICE_NUM)
        {
        }
    }

    public class Network
    {
        static readonly byte[] USER_NETWORK_KEY = null; // ANT+ network key is embedded in ANT_NET.dll
        static readonly byte USER_NETWORK_NUM = 0;         // The network key is assigned to this network number

        ANT_Device device;
        readonly IDictionary<byte, PowerChannel> channels = new Dictionary<byte, PowerChannel>();

        public delegate void SearchEventHandler(Network sender, byte antChannel, bool isConnected, int deviceNumber);
        public event SearchEventHandler OnSearchEnded;

        public delegate void AccumPowerReceivedHandler(Network sender, byte antChannel, int accumPower, int count);
        public event AccumPowerReceivedHandler OnAccumPowerReceived;

        void PrintErrorMessage(ANT_Response response, string header)
        {
            Debug.WriteLine(String.Format(header + " {0} configuring {1}", response.getChannelEventCode(), response.getMessageID()));
        }

        void DeviceEventResponse(ANT_Response response)
        {
            var ec = response.getChannelEventCode();
            switch (response.getMessageID())
            {
                case ANTMessageID.CHANNEL_SEARCH_TIMEOUT_0x44:
                    Debug.WriteLine("DeviceEventResponse CHANNEL_SEARCH_TIMEOUT_0x44");
                    break;
                case ANTMessageID.CLOSE_CHANNEL_0x4C:
                    if (ec == ANTEventID.CHANNEL_IN_WRONG_STATE_0x15)
                    {
                        Debug.WriteLine("CHANNEL_IN_WRONG_STATE_0x15: Channel is already closed");
                    }
                    break;
                case ANTMessageID.NETWORK_KEY_0x46:
                case ANTMessageID.ASSIGN_CHANNEL_0x42:
                case ANTMessageID.CHANNEL_ID_0x51:
                case ANTMessageID.CHANNEL_RADIO_FREQ_0x45:
                case ANTMessageID.CHANNEL_MESG_PERIOD_0x43:
                case ANTMessageID.OPEN_CHANNEL_0x4B:
                case ANTMessageID.UNASSIGN_CHANNEL_0x41:
                    if (ec != ANTEventID.RESPONSE_NO_ERROR_0x00)
                    {
                        PrintErrorMessage(response, "UNASSIGN_CHANNEL_0x41 RESPONSE_NO_ERROR_0x00");
                    }
                    break;
                case ANTMessageID.RX_EXT_MESGS_ENABLE_0x66:
                    if (ec == ANTEventID.INVALID_MESSAGE_0x28)
                    {
                        Debug.WriteLine("Extended messages not supported in this ANT product");
                        break;
                    }
                    else if (ec != ANTEventID.RESPONSE_NO_ERROR_0x00)
                    {
                        PrintErrorMessage(response, "RX_EXT_MESGS_ENABLE_0x66 RESPONSE_NO_ERROR_0x00");
                        break;
                    }
                    // Extended messages enabled
                    break;
                case ANTMessageID.REQUEST_0x4D:
                    if (ec == ANTEventID.INVALID_MESSAGE_0x28)
                    {
                        Debug.WriteLine("Requested message not supported in this ANT product");
                        break;
                    }
                    break;
                default:
                    PrintErrorMessage(response, "Unhandled response");
                    break;
            }
        }

        void DeviceResponse(ANT_Response response)
        {
            switch ((ANTMessageID)response.responseID)
            {
                case ANTMessageID.STARTUP_MESG_0x6F:
                    // RESET Complete
                    break;
                case ANTMessageID.VERSION_0x3E:
                    Debug.WriteLine("VERSION: " + new ASCIIEncoding().GetString(response.messageContents));
                    break;
                case ANTMessageID.RESPONSE_EVENT_0x40:
                    DeviceEventResponse(response);
                    break;
            }
        }

        void ChannelSearchEnded(PowerChannel sender, bool isConnected, int deviceNumber)
        {
            foreach (var kvp in channels)
            {
                if (kvp.Value == sender)
                {
                    OnSearchEnded?.Invoke(this, kvp.Key, isConnected, deviceNumber);
                    return;
                }
            }
            throw new Exception("channels inconsistency");
        }

        void ChannelAccumPowerReceived(PowerChannel sender, int accumPower, int count)
        {
            foreach (var kvp in channels)
            {
                if (kvp.Value == sender)
                {
                    OnAccumPowerReceived?.Invoke(this, kvp.Key, accumPower, count);
                    return;
                }
            }
            throw new Exception("channels inconsistency");
        }

        void ChannelClosed(PowerChannel sender)
        {
            lock (this)
            {
                byte i = 255;
                foreach (var kvp in channels)
                {
                    if (kvp.Value == sender)
                    {
                        i = kvp.Key;
                        break;
                    }
                }
                if (i == 255)
                    throw new Exception("channels inconsistency");
                channels.Remove(i);
            }
        }

        void RegisterChannel(int antChannel, PowerChannel c)
        {
            c.OnSearchEnded += ChannelSearchEnded;
            c.OnAccumPowerReceived += ChannelAccumPowerReceived;
            c.OnClosed += ChannelClosed;
            lock (this)
            {
                channels.Add((byte)antChannel, c);
                c.Open();
            }
        }

        public void OpenSearchChannel(int antChannel)
        {
            RegisterChannel(antChannel, new PowerChannel(device, antChannel, USER_NETWORK_NUM));
        }

        public void OpenChannel(int antChannel, ushort deviceNumber)
        {
            RegisterChannel(antChannel, new PowerChannel(device, antChannel, USER_NETWORK_NUM, deviceNumber));
        }

        public Network()
        {
            device = new ANT_Device();   // Create a device instance using the automatic constructor (automatic detection of USB device number and baud rate)
            device.deviceResponse += new ANT_Device.dDeviceResponseHandler(DeviceResponse);    // Add device response function to receive protocol event messages

            device.ResetSystem();     // Soft reset
            System.Threading.Thread.Sleep(500);    // Delay 500ms after a reset

            // If you call the setup functions specifying a wait time, you can check the return value for success or failure of the command
            // This function is blocking - the thread will be blocked while waiting for a response.
            // 500ms is usually a safe value to ensure you wait long enough for any response
            // If you do not specify a wait time, the command is simply sent, and you have to monitor the protocol events for the response,
            if (!device.setNetworkKey(USER_NETWORK_NUM, USER_NETWORK_KEY, 500))
                throw new Exception("Error configuring network key");

            device.enableRxExtendedMessages(true);
        }

        public void Shutdown()
        {
            // Clean up ANT
            Debug.WriteLine("Disconnecting module...");
            ANT_Device.shutdownDeviceInstance(ref device);  // Close down the device completely and completely shut down all communication
            Debug.WriteLine("ANT device shutdowned successfully!");
        }

        public ManualResetEvent CloseAllChannel()
        {
            ManualResetEvent ev = new ManualResetEvent(false);

            lock (this)
            {
                if (channels.Count == 0)
                    ev.Set();
                else
                {
                    foreach (var kvp in channels)
                    {
                        kvp.Value.OnClosed += (sender) => {
                            if (channels.Count == 0)
                            {
                                ev.Set();
                                Debug.WriteLine("channel closed");
                            }
                        };
                        kvp.Value.Close();
                    }
                }
            }
            return ev;
        }

        public bool IsAllChannelClosed()
        {
            return channels.Count == 0;
        }

        static Network instance = null;
        static object lockInstance = new object();
        public static Network GetInstance()
        {
            lock (lockInstance)
            {
                if (instance == null)
                    instance = new Network();
            }
            return instance;
        }
    }
}

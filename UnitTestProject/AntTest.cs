using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yuyushiki;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    [Ignore]
    public class AntTest
    {
        ManualResetEvent waitHandle = new ManualResetEvent(false);

        [TestMethod]
        public void TestNetwork()
        {
            var network = new Network();
            network.OnSearchEnded += Network_OnSearchEnded;
            network.OpenSearchChannel(0);
            waitHandle.WaitOne(Timeout.Infinite);
        }

        private void Network_OnSearchEnded(Network sender, byte antChannel, bool isConnected, int deviceNumber)
        {
            Assert.IsTrue(isConnected);
            waitHandle.Set();
        }

        [TestMethod]
        public void TestPowerReceive()
        {
            var network = new Network();
            network.OnAccumPowerReceived += Network_OnAccumPowerReceived;
            network.OpenSearchChannel(0);
            waitHandle.WaitOne(Timeout.Infinite);
        }

        private void Network_OnAccumPowerReceived(Network sender, byte antChannel, int accumPower, int count)
        {
            Trace.WriteLine("accumPower : " + accumPower.ToString());
            waitHandle.Set();
        }
    }
}

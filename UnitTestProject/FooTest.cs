using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yuyushiki;
using System.IO.Ports;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    [Ignore]
    public class FooTest
    {
        [TestMethod]
        public void TestVfdAscii()
        {
            SerialPort myPort = new SerialPort("COM3");
            myPort.Open();

            byte[] buffer2 = System.Text.Encoding.ASCII.GetBytes("abc");
            myPort.Write(buffer2, 0, buffer2.Length);
            myPort.Close();
        }

        [TestMethod]
        public void TestVfdEsc()
        {
            SerialPort myPort = new SerialPort("COM3");
            myPort.Open();

            // byte[] buffer = { 0x02, 0x05, 0x43, 0x37, 0x03 }; // set to CD5220
            // byte[] buffer = { 0x1b, 0x2a, 0x04 }; // set brightness in CD5220
            // byte[] buffer = { 0x02, 0x05, 0x43, 0x31, 0x03 }; // set to ESC/POS
            // byte[] buffer = { 0x1f, 0x58, 0x01 }; // set brightness in ESC/POS
            byte[] buffer = { 0x1b, 0x40, // init
                0x1b, 0x26, 1, 126, 126, 5, 1, 1, 1, 1, 1, // define PCG in ESC/POS
                // 0x0c, // clear
                // 0x1b, 0x25, 0x01, // select PCG
                // 126, // show it (126)
                // 0x1b, 0x25, 0x00, // cancel PCG
            }; 
            myPort.Write(buffer, 0, buffer.Length);
            myPort.Close();
        }

        [TestMethod]
        public void TestVfdDelay()
        {
            SerialPort myPort = new SerialPort("COM3");
            myPort.Open();
            byte[] b = { 0x0c };
            myPort.Write(b, 0, b.Length);

            for (int i = 0; i < 10; i++)
            {
                byte[] buffer = { 0x0c, (byte)(48 + i) };
                myPort.Write(buffer, 0, buffer.Length);
                Trace.WriteLine(i.ToString());
                System.Threading.Thread.Sleep(1000);
            }
            myPort.Close();
        }

        [TestMethod]
        public void TestEnumerateComPortName()
        {
            foreach (var s in VfdWriter.EnumerateComPortName())
                Trace.WriteLine(s);
        }
    }
}

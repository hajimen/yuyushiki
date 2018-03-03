using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using Yuyushiki;

namespace UnitTestProject
{
    [TestClass]
    public class VfdTest
    {
        [TestMethod]
        public void TestVfdUpdate()
        {
            VfdWriter w = new TestVfdWriter(new string[] { "0C", "1F-58-04", "1F-24-06-01-61", "1F-24-01-02-41-42-43" });
            var p = new VfdPanel(w, 3);
            p.Rows[0][5] = 'a';
            p.Rows[1][0] = 'A';
            p.Rows[1][1] = 'B';
            p.Rows[1][2] = 'C';
            p.Update();
            w.Dispose();
        }
    }

    class TestVfdWriter : VfdWriter
    {
        string[] shouldSend;
        int shouldSendIdx = 0;

        public TestVfdWriter(string[] shouldSend)
        {
            this.shouldSend = shouldSend;
        }

        public override void Dispose()
        {
            Trace.WriteLine("writer disposed");
        }

        public override void Send(byte[] b)
        {
            var s = BitConverter.ToString(b);
            Trace.WriteLine("send: " + s);
            StringAssert.Equals(s, shouldSend[shouldSendIdx]);
            shouldSendIdx++;
        }
    }
}

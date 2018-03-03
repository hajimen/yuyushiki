using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yuyushiki;

namespace UnitTestProject
{
    [TestClass]
    public class PlanTest
    {
        [TestMethod]
        public void TestIndexOfNextWhiteSpace()
        {
            string s = "abc def  gh ";
            var p = new PlanParser();
            Assert.AreEqual(3, p.AsDynamic().IndexOfNextWhiteSpace(s, 0));
            Assert.AreEqual(3, p.AsDynamic().IndexOfNextWhiteSpace(s, 1));
            Assert.AreEqual(3, p.AsDynamic().IndexOfNextWhiteSpace(s, 2));
            Assert.AreEqual(3, p.AsDynamic().IndexOfNextWhiteSpace(s, 3));
            Assert.AreEqual(7, p.AsDynamic().IndexOfNextWhiteSpace(s, 4));
            Assert.AreEqual(8, p.AsDynamic().IndexOfNextWhiteSpace(s, 8));
            Assert.AreEqual(11, p.AsDynamic().IndexOfNextWhiteSpace(s, 9));
            Assert.AreEqual(12, p.AsDynamic().IndexOfNextWhiteSpace(s, 12));
        }

        [TestMethod]
        public void TestIndexOfNextLetter()
        {
            string s = "abc def  gh ";
            var p = new PlanParser();
            Assert.AreEqual(0, p.AsDynamic().IndexOfNextNonWhiteSpace(s, 0));
            Assert.AreEqual(1, p.AsDynamic().IndexOfNextNonWhiteSpace(s, 1));
            Assert.AreEqual(4, p.AsDynamic().IndexOfNextNonWhiteSpace(s, 3));
            Assert.AreEqual(9, p.AsDynamic().IndexOfNextNonWhiteSpace(s, 7));
            Assert.AreEqual(12, p.AsDynamic().IndexOfNextNonWhiteSpace(s, 12));
        }

        int parseDuration(string s)
        {
            var p = new PlanParser();
            var o = new PrivateObject(p.AsDynamic().ParseDuration(s));
            if (!((bool)o.GetField("IsError")))
                return (int)o.GetField("Duration");
            return -1;
        }

        [TestMethod]
        public void TestParseDuration()
        {
            Assert.AreEqual(270, parseDuration("4m30s"));
            Assert.AreEqual(1200, parseDuration("20m"));
            Assert.AreEqual(3600, parseDuration("1h"));
            Assert.AreEqual(4200, parseDuration("1h10m"));
            Assert.AreEqual(100, parseDuration("100"));
            Assert.AreEqual(4210, parseDuration("1h10m10s"));
            Assert.AreEqual(0, parseDuration("0"));
        }

        void assertParseLineComment(int offset, string line)
        {
            var p = new PlanParser();
            var o = p.AsDynamic().ParseLine(offset, line);
            Assert.IsNull(o);
        }

        void assertParseLineSection(int offset, string line, int shouldTargetPower, int shouldDuration, string shouldDesc)
        {
            var p = new PlanParser();
            var o = new PrivateObject(p.AsDynamic().ParseLine(offset, line));
            Assert.IsFalse((bool)o.GetField("IsError"));
            Assert.AreEqual(shouldTargetPower, (int)o.GetField("TargetPower"));
            Assert.AreEqual(shouldDuration, (int)o.GetField("Duration"));
            Assert.AreEqual(shouldDesc, (string)o.GetField("Desc"));
        }

        void assertParseLineError(int offset, string line, int shouldStart, int shouldLength)
        {
            var p = new PlanParser();
            var o = new PrivateObject(p.AsDynamic().ParseLine(offset, line));
            Assert.IsTrue((bool)o.GetField("IsError"));
            Assert.AreEqual(shouldStart, (int)o.GetField("Start"));
            Assert.AreEqual(shouldLength, (int)o.GetField("Length"));
        }

        [TestMethod]
        public void TestParseLine()
        {
            assertParseLineError(0, "a", 0, 1);
            assertParseLineComment(0, "# comment");
            assertParseLineError(100, "a", 100, 1);
            assertParseLineError(0, "100    a", 7, 1);
            assertParseLineError(0, "100    aa", 7, 2);
            assertParseLineError(0, "100 100", 4, 3);
            assertParseLineSection(0, "100 100 test", 100, 100, "test");
            assertParseLineSection(0, "100 100 test #2", 100, 100, "test #2");
        }

        [TestMethod]
        public void TestBuildDurationString()
        {
            Assert.AreEqual("   10s", Plan.BuildDurationString(10));
            Assert.AreEqual(" 1m00s", Plan.BuildDurationString(60));
            Assert.AreEqual(" 1h00m", Plan.BuildDurationString(3600));
        }
    }
}
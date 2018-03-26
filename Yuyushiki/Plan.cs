using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Yuyushiki
{
    public class Plan
    {
        public readonly PlanParser Parser = new PlanParser();
        DateTime lastPowerAccumedAt;
        public int RecentPower
        {
            get;
            private set;
        }

        public Plan()
        {
            Reset();
        }

        public string FilePath;

        public void ReadFile(string filePath)
        {
            Parser.Text = String.Join("\r\n", File.ReadAllLines(filePath));
            FilePath = filePath;
        }

        public void WriteFile(bool isSaveAs)
        {
            File.WriteAllText(FilePath, Parser.Text, Encoding.UTF8);
        }

        public static string BuildDurationString(int duration)
        {
            int s = duration % 60;
            int rm = (duration - s) / 60;
            int m = rm % 60;
            int h = (rm - m) / 60;
            string ret = "";
            if (h > 0)
            {
                ret += h.ToString() + "h";
                ret += String.Format("{0:D2}m", m);
            }
            else
            {
                if (m > 0)
                {
                    ret += m.ToString() + "m";
                    ret += String.Format("{0:D2}s", s);
                }
                else
                    ret += s.ToString() + "s";
            }

            return String.Format("{0,6}", ret);
        }

        public void Reset()
        {
            lastPowerAccumedAt = DateTime.MinValue;
            RecentPower = 0;
        }

        public void AccumJ(int accumPower, int count, DateTime now, PlanSection s)
        {
            lock (this)
            {
                if (lastPowerAccumedAt == DateTime.MinValue)
                {
                    // first sample
                }
                else
                {
                    if (now <= lastPowerAccumedAt)
                        return;
                    var span = now - lastPowerAccumedAt;
                    int milliJ = (int)(accumPower * span.TotalMilliseconds / count);
                    s.AccumMilliJ += milliJ;
                }
                lastPowerAccumedAt = now;
                RecentPower = accumPower / count;
                Debug.WriteLine("RecentPower: " + RecentPower.ToString());
            }
        }

        public int AveJ(DateTime now, PlanSection s)
        {
            var end = lastPowerAccumedAt;
            var addMilliJ = 0;
            if (now > lastPowerAccumedAt)
            {
                var span = now - lastPowerAccumedAt;
                addMilliJ = (int)(RecentPower * span.TotalMilliseconds);
                end = now;
            }
            var powerAccumedSpan = end - s.PlayStartedAt;
            if (Math.Round(powerAccumedSpan.TotalSeconds) > 0.8)
            {
                return (int)Math.Round(((double)(s.AccumMilliJ + addMilliJ)) / powerAccumedSpan.TotalMilliseconds);
            }
            else
                return -1;
        }

        public void AddPausedSpan(TimeSpan span, PlanSection s)
        {
            s.PlayStartedAt += span;
            lastPowerAccumedAt += span;
        }
    }

    public class PlanSection
    {
        public readonly int TargetPower;
        public readonly int Duration;
        public readonly string Desc;
        public readonly List<int> LoopIndex = new List<int>();

        public long AccumMilliJ = 0;
        public DateTime PlayStartedAt;

        public PlanSection(int targetPower, int duration, string desc)
        {
            TargetPower = targetPower;
            Duration = duration;
            Desc = desc;
        }
    }

    class SectionOrError : PlanSection
    {
        public readonly bool IsError;
        public readonly int Start;
        public readonly int Length;
        public readonly string Message;
        public readonly bool IsLoop;
        public readonly int LoopCount;
        public readonly SectionOrError LoopSection;

        SectionOrError(int targetPower, int duration, string desc, bool isError, int start, int length, string message, bool isLoop, int loopCount, SectionOrError loopSection) : base(targetPower, duration, desc)
        {
            IsError = isError;
            Start = start;
            Length = length;
            Message = message;
            IsLoop = isLoop;
            LoopCount = loopCount;
            LoopSection = loopSection;
        }

        public static SectionOrError GetSection(int targetPower, int duration, string desc)
        {
            return new SectionOrError(targetPower, duration, desc, false, 0, 0, null, false, 0, null);
        }

        public static SectionOrError GetSection(int targetPower, int duration, string desc, SectionOrError loopSection)
        {
            return new SectionOrError(targetPower, duration, desc, false, 0, 0, null, false, 0, loopSection);
        }

        public static SectionOrError GetError(int start, int length, string message)
        {
            return new SectionOrError(0, 0, null, true, start, length, message, false, 0, null);
        }

        public static SectionOrError GetLoop(int count)
        {
            return new SectionOrError(0, 0, null, false, 0, 0, null, true, count, null);
        }
    }

    public class PlanParser
    {
        public string Text;

        readonly Encoding asciiEncoder = ASCIIEncoding.GetEncoding("us-ascii",
            EncoderFallback.ExceptionFallback,
            DecoderFallback.ReplacementFallback);

        List<SectionOrError> loopSectionList = new List<SectionOrError>();

        List<SectionOrError> ParseText()
        {
            int offset = 0;
            var leftText = Text;
            var ret = new List<SectionOrError>();
            loopSectionList = new List<SectionOrError>();
            while (leftText.Length > 0)
            {
                bool isLastLine = false;
                int tailIdx = leftText.IndexOf("\r\n");
                if (tailIdx == -1)
                {
                    tailIdx = leftText.Length;
                    isLastLine = true;
                }
                var line = leftText.Substring(0, tailIdx);
                leftText = isLastLine ? "" : leftText.Substring(tailIdx + 2);
                var offsetBefore = offset;
                offset += tailIdx + 2;
                if (line.Length == 0)
                    continue;
                var soe = ParseLine(offsetBefore, line);
                if (soe != null)
                    ret.Add(soe);
            }
            return ret;
        }

        int IndexOfNextWhiteSpace(string s, int start)
        {
            return s.Substring(start).TakeWhile(c => !Char.IsWhiteSpace(c)).Count() + start;
        }

        int IndexOfNextNonWhiteSpace(string s, int start)
        {
            return s.Substring(start).TakeWhile(c => Char.IsWhiteSpace(c)).Count() + start;
        }

        class DurationOrError
        {
            public readonly int Duration;
            public readonly bool IsError;
            public readonly string Message;

            DurationOrError(int duration, bool isError, string message)
            {
                Duration = duration;
                IsError = isError;
                Message = message;
            }
            public static DurationOrError GetDuration(int duration)
            {
                return new DurationOrError(duration, false, null);
            }

            public static DurationOrError GetError(string message)
            {
                return new DurationOrError(0, true, message);
            }
        }

        DurationOrError ParseDuration(string s)
        {
            int d = 0;
            int idx = 0;
            int rank = 0;
            while (idx < s.Length)
            {
                var digitTailIdx = s.Substring(idx).TakeWhile(c => Char.IsDigit(c)).Count() + idx;
                if (digitTailIdx == idx)
                {
                    return DurationOrError.GetError("Duration column is malformed. It should be like '1h30m', '3m30s', '180' or just '0'.");
                }
                var n = Int32.Parse(s.Substring(idx, digitTailIdx - idx));
                if (digitTailIdx == s.Length)
                {
                    if (rank == 0)
                    {
                        d += n;
                        return DurationOrError.GetDuration(d);
                    }
                    else
                    {
                        return DurationOrError.GetError("'m' is missing in duration column.");
                    }
                }
                else
                {
                    switch (s[digitTailIdx])
                    {
                        case 'h':
                            rank = 1;
                            d += 60 * 60 * n;
                            break;
                        case 'm':
                            rank = 0;
                            d += 60 * n;
                            break;
                        case 's':
                            d += n;
                            return DurationOrError.GetDuration(d);
                        default:
                            return DurationOrError.GetError("Duration column is malformed. It should be like '1h30m', '3m30s' or just '0'.");
                    }
                    idx = digitTailIdx + 1;
                }
            }
            return DurationOrError.GetDuration(d);
        }

        bool CheckDesc(string desc)
        {
            try
            {
                asciiEncoder.GetBytes(desc);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        SectionOrError ParseLine(int offset, string line, int depth = 0)
        {
            if (loopSectionList.Count < depth)
                return SectionOrError.GetError(0, line.Length, "Loop mark is too many.");

            if (line[0] == '#')
                return null;
            int sectionHeadIdx = 0;
            while (line[sectionHeadIdx] == '%')
            {
                sectionHeadIdx = IndexOfNextNonWhiteSpace(line, sectionHeadIdx + 1);
                depth++;
            }
            if (loopSectionList.Count < depth)
                return SectionOrError.GetError(offset, line.Length, "Bad loop mark is found.");
            while (loopSectionList.Count > depth)
            {
                loopSectionList.RemoveAt(loopSectionList.Count - 1);
            }

            if (line.Substring(sectionHeadIdx).ToLower().IndexOf("loop") == 0)
            {
                int countIdx = IndexOfNextNonWhiteSpace(line, sectionHeadIdx + 4);
                if (countIdx == line.Length)
                    return SectionOrError.GetError(offset, line.Length, "Loop header 'LOOP n' lacks 'n'.");
                try
                {
                    var n = Int32.Parse(line.Substring(countIdx));
                    var section = SectionOrError.GetLoop(n);
                    loopSectionList.Add(section);
                    return section;
                }
                catch (Exception)
                {
                    return SectionOrError.GetError(offset, line.Length, "Loop header 'LOOP n' don't have digits. 'n' must be digits.");
                }
            }

            int targetPowerTailIdx = IndexOfNextWhiteSpace(line, sectionHeadIdx);
            if (targetPowerTailIdx == sectionHeadIdx)
                return SectionOrError.GetError(offset, 1, "Each lines of plan should start with # or number.");
            if (targetPowerTailIdx == line.Length)
                return SectionOrError.GetError(offset, line.Length, "No duration column and description.");
            char mayW = line[targetPowerTailIdx - 1];
            int targetPowerTailAdj = 0;
            if (mayW == 'w' || mayW == 'W')
                targetPowerTailAdj = -1;
            int targetPower;
            var s = line.Substring(sectionHeadIdx, targetPowerTailIdx + targetPowerTailAdj - sectionHeadIdx);
            if (!Int32.TryParse(line.Substring(sectionHeadIdx, targetPowerTailIdx + targetPowerTailAdj - sectionHeadIdx), out targetPower) || targetPower < 0)
                return SectionOrError.GetError(offset, targetPowerTailIdx - sectionHeadIdx, "Target power column should be a natural number or zero.");

            int durationHeadIdx = IndexOfNextNonWhiteSpace(line, targetPowerTailIdx);
            if (durationHeadIdx == line.Length)
                return SectionOrError.GetError(offset + targetPowerTailIdx, durationHeadIdx - targetPowerTailIdx, "No duration column and description.");
            int durationTailIdx = IndexOfNextWhiteSpace(line, durationHeadIdx);
            if (durationTailIdx == line.Length)
                return SectionOrError.GetError(offset + durationHeadIdx, durationTailIdx - durationHeadIdx, "No description.");
            var durationOrError = ParseDuration(line.Substring(durationHeadIdx, durationTailIdx - durationHeadIdx));
            if (durationOrError.IsError)
                return SectionOrError.GetError(offset + durationHeadIdx, durationTailIdx - durationHeadIdx, durationOrError.Message);
            int duration = durationOrError.Duration;

            int descHeadIdx = IndexOfNextNonWhiteSpace(line, durationTailIdx);
            if (descHeadIdx == line.Length)
                return SectionOrError.GetError(offset + durationTailIdx, descHeadIdx - durationTailIdx, "No description.");
            string desc = line.Substring(descHeadIdx).Trim();
            if (desc.Length > 15)
                return SectionOrError.GetError(offset + descHeadIdx, line.Length - descHeadIdx, "Description should be under 15 characters.");
            if (!CheckDesc(desc))
                return SectionOrError.GetError(offset + descHeadIdx, line.Length - descHeadIdx, "Description should be ASCII characters.");

            while (loopSectionList.Count > depth)
            {
                loopSectionList.RemoveAt(loopSectionList.Count - 1);
            }
            if (depth == 0)
                return SectionOrError.GetSection(targetPower, duration, desc);
            else
            {
                return SectionOrError.GetSection(targetPower, duration, desc, loopSectionList[depth - 1]);
            }
        }

        public Tuple<int, int, string> GetFirstError()
        {
            if (Text.Trim().Length == 0)
                return new Tuple<int, int, string>(0, 0, "Plan is empty.");
            var soes = ParseText();
            if (soes.Count == 0)
                return new Tuple<int, int, string>(0, 0, "There's no section.");
            foreach (var soe in soes)
            {
                if (soe.IsError)
                {
                    return new Tuple<int, int, string>(soe.Start, soe.Length, soe.Message);
                }
            }
            return null;
        }

        Tuple<int, List<PlanSection>> UnrollLoop(List<SectionOrError> soes, int loopIdx)
        {
            var loopSection = soes[loopIdx];
            var inner = new List<PlanSection>();
            int i = loopIdx + 1;
            for (; i < soes.Count; i++)
            {
                var soe = soes[i];
                if (soe.IsLoop)
                {
                    var r = UnrollLoop(soes, i);
                    i = r.Item1 - 1;
                    inner.AddRange(r.Item2);
                    continue;
                }
                if (soe.LoopSection == loopSection)
                    inner.Add(soe);
                else
                    break;
            }
            var outer = new List<PlanSection>();
            for (int k = 0; k < loopSection.LoopCount; k ++)
            {
                var innerIdxed = new List<PlanSection>();
                foreach (var ips in inner)
                {
                    var nps = new PlanSection(ips.TargetPower, ips.Duration, ips.Desc);
                    nps.LoopIndex.AddRange(ips.LoopIndex);
                    nps.LoopIndex.Add(k);
                    innerIdxed.Add(nps);
                }
                outer.AddRange(innerIdxed);
            }
            return new Tuple<int, List<PlanSection>>(i, outer);
        }

        public List<PlanSection> Parse()
        {
            var ret = new List<PlanSection>();
            var pt = ParseText();
            for (int i = 0; i < pt.Count; i ++)
            {
                var soe = pt[i];
                if (soe.IsError)
                    throw new Exception("Plan is malformed.");
                if (soe.IsLoop)
                {
                    var r = UnrollLoop(pt, i);
                    i = r.Item1 - 1;
                    ret.AddRange(r.Item2);
                    continue;
                }
                ret.Add(soe);
            }
            return ret;
        }
    }
}

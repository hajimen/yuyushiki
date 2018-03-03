using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace Yuyushiki
{
    public abstract class VfdWriter : IDisposable
    {
        public abstract void Dispose();

        public abstract void Send(byte[] b);

        public static List<string> EnumerateComPortName()
        {
            return new List<string>(SerialPort.GetPortNames());
        }
    }

    public class VfdWriterImpl : VfdWriter
    {
        SerialPort port;
        bool isPortOpen = false;

        public VfdWriterImpl(string comPortName)
        {
            port = new SerialPort(comPortName);
            port.Open();
            isPortOpen = true;
        }

        public override void Dispose()
        {
            lock (this)
            {
                port.Close();
                isPortOpen = false;
            }
        }

        public override void Send(byte[] b)
        {
            lock (this)
            {
                if (!isPortOpen)
                    throw new IOException("The port has been closed.");
                port.Write(b, 0, b.Length);
            }
        }
    }

    public class VfdPanel
    {
        static readonly int NUM_ROW = 2;
        List<List<char>> _Rows = new List<List<char>>();
        List<List<char>> _RowsBefore = new List<List<char>>();
        int _Brightness;
        int _BrightnessBefore;

        VfdWriter writer;

        public List<List<char>> Rows
        {
            get
            {
                return _Rows;
            }
        }

        public int Brightness // 0, 1, 2, or 3
        {
            get
            {
                return _Brightness;
            }
            set
            {
                if (value < 0 || value >= 4)
                    throw new ArgumentException("Brightness sould be 0, 1, 2, or 3");
                _Brightness = value;
            }
        }

        public VfdPanel(VfdWriter w, int brightness)
        {
            writer = w;
            _Brightness = brightness;
            _BrightnessBefore = brightness;
            w.Send(new byte[] { 0x0c /* clear */});
            for (int i = 0; i < NUM_ROW; i ++)
                _Rows.Add(new List<char>("                    "));
            foreach (var r in _Rows)
                _RowsBefore.Add(new List<char>(r));
            w.Send(new byte[] { 0x1f, 0x58, (byte)(Brightness + 1) }); // brightness
        }

        struct Range
        {
            public int Start, End;
        }

        void OverwriteRange(int rowIdx, Range r, List<char> row)
        {
            byte[] b = new byte[r.End - r.Start + 4];
            Array.Copy(new byte[] { 0x1f, 0x24, (byte)(r.Start + 1), (byte)(rowIdx + 1) }, b, 4);
            for (int i = r.Start; i < r.End; i ++)
            {
                b[4 + i - r.Start] = (byte)row[i];
            }
            writer.Send(b);
        }

        List<Range> FindInvalidatedRanges(List<char> row, List<char> rowBefore)
        {
            var ir = new List<Range>();
            var r = new Range();
            bool hitting = false;
            for (int i = 0; i < row.Count; i++)
            {
                if (row[i] != rowBefore[i] && !hitting)
                {
                    r.Start = i;
                    hitting = true;
                }
                else if (hitting && row[i] == rowBefore[i])
                {
                    hitting = false;
                    r.End = i;
                    ir.Add(r);
                    r = new Range();
                }
            }
            if (hitting)
            {
                r.End = row.Count;
                ir.Add(r);
                hitting = false;
            }
            return ir;
        }

        void UpdateRow(int rowIdx, List<char> row, List<char> rowBefore)
        {
            foreach (var r in FindInvalidatedRanges(row, rowBefore))
            {
                OverwriteRange(rowIdx, r, row);
            }
        }

        public void Update()
        {
            for (int i = 0; i < NUM_ROW; i++)
            {
                UpdateRow(i, _Rows[i], _RowsBefore[i]);
                _RowsBefore[i] = new List<char>(_Rows[i]);
            }
            if (_BrightnessBefore != Brightness)
            {
                writer.Send(new byte[] { 0x1f, 0x58, (byte)(Brightness + 1) });
                _BrightnessBefore = Brightness;
            }
        }

        public void Overwrite(string s, int row, int start)
        {
            if (start + s.Length > Rows[row].Count)
                throw new ArgumentException("too long string");
            for (int i = 0; i < s.Length; i ++)
            {
                Rows[row][i + start] = s[i];
            }
        }
    }
}

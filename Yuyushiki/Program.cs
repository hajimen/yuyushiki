using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yuyushiki
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string mutexName = "Yuyushiki_b6f63n66694KqWK";
            bool createdNew;
            using (var mutex = new System.Threading.Mutex(true, mutexName, out createdNew))
            {
                if (createdNew == false)
                {
                    MessageBox.Show("Yuyushiki is already running.", "Yuyushiki", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}

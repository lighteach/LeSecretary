using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LeSecretary
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(int hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(int hwnd, int id);

        private int WinHandleToNum
        {
            get
            {
                return (int)this.Handle;
            }
        }

        private Dictionary<string, int> ModKeyDefines = new Dictionary<string, int>
        {
            { "none", 0 }
            , { "alt", 1 }
            , { "control", 2 }
            , { "shift", 4 }
            , { "windows", 8 }
        };

        public Form1()
        {
            InitializeComponent();
        }

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowIcon = true;

            ShortCutModel scmData = (new ShortCutManager()).GetShortCutData();

            foreach (ShortCutItem sci in scmData.ShortCutList)
            {
                Tuple<int, int, int> tplMods = Tuple.Create(0, 0, 0);
                int modKey1 = ModKeyDefines.ContainsKey(sci.modKey1.ToLower()) ? ModKeyDefines[sci.modKey1.ToLower()] : 0;
                int modKey2 = ModKeyDefines.ContainsKey(sci.modKey2.ToLower()) ? ModKeyDefines[sci.modKey2.ToLower()] : 0;
                int modKey3 = ModKeyDefines.ContainsKey(sci.modKey3.ToLower()) ? ModKeyDefines[sci.modKey3.ToLower()] : 0;
                Keys key = Keys.Escape;
                Keys.TryParse(sci.key, false, out key);

                RegisterHotKey(WinHandleToNum, sci.id, modKey1 | modKey2 | modKey3, (int)key);
            }
        } 
        #endregion

        #region Form1_FormClosed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShortCutModel scmData = (new ShortCutManager()).GetShortCutData();
            foreach (ShortCutItem sci in scmData.ShortCutList)
            {
                UnregisterHotKey((int)WinHandleToNum, sci.id);
            }
        }
        #endregion

        #region Form1_FormClosing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult confirm = MessageBox.Show("Are you sure?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //e.Cancel = (confirm == DialogResult.Cancel);
        }
        #endregion

        #region Form1_Resize
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.ShowIcon = false;
                notifyIcon1.Visible = true;
            }
        }
        #endregion

        #region Form1_KeyDown
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult confirm = MessageBox.Show("Are you sure?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (confirm == DialogResult.OK)
                    this.Close();
            }
        } 
        #endregion

        #region notifyIcon1_DoubleClick
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowIcon = true;
            notifyIcon1.Visible = false;

            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.TopLevel = true;
            this.Focus();
        }
        #endregion

        #region WndProc
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == (int)0x312)
            {
                IntPtr msgParam = m.WParam;
                ShortCutModel scmData = (new ShortCutManager()).GetShortCutData();
                List<ShortCutItem> listSci = scmData.ShortCutList.ToList();

                if (listSci.Exists(s => (IntPtr)s.id == msgParam))
                {
                    ShortCutItem sci = listSci.First(s => (IntPtr)s.id == msgParam);
                    //MessageBox.Show($"{sci.cmd}, {sci.parameters}");
                    try
                    {
                        Process.Start(sci.cmd, sci.parameters);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "프로그램을 실행하지 못했습니다.");
                    }
                }

                // if (m.WParam == (IntPtr) 0x0)
                // {
                //     Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                // }
                // else if (m.WParam == (IntPtr) 0x1)
                // {
                //     Process.Start("explorer", @"d:\Downloads");
                // }
            }
        }

        #endregion

        #region button1_Click : shortCut.json 열기
        private void button1_Click(object sender, EventArgs e)
        {
            string execPath = Path.GetDirectoryName(Application.ExecutablePath);
            string shortCutPath = Path.Combine(execPath, "shortCuts.json");
            Process.Start(@"C:\Program Files\EditPlus\editplus.exe", shortCutPath);
        } 
        #endregion
    }
}

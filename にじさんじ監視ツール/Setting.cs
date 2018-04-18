using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace にじさんじ監視ツール
{
    public partial class Setting : Form
    {
        [DllImport("KERNEL32.DLL")]
        public static extern uint WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);

        public Setting()
        {
            InitializeComponent();

            textBox1.Text = Start.Api_key;
            checkBox1.Checked = Start.HasOpenLive;
            checkBox2.Checked = Start.HasOpenCommentViewer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("API キーは必ず入力してください。", "にじさんじ監視ツール", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Start.Api_key = textBox1.Text;
            Start.HasOpenLive = checkBox1.Checked;
            Start.HasOpenCommentViewer = checkBox2.Checked;

            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "config.ini";

            WritePrivateProfileString("section", "api_key", Start.Api_key, iniFileName);
            WritePrivateProfileString("section", "hasOpenLive", Start.HasOpenLive.ToString(), iniFileName);
            WritePrivateProfileString("section", "hasOpenCommentViewer", Start.HasOpenCommentViewer.ToString(), iniFileName);

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace にじさんじ監視ツール
{
    public partial class Whitelist_Form : Form
    {
        private static List<string> whitelist = new List<string>();

        public static List<string> Whitelist { get => whitelist; set => whitelist = value; }

        private BindingSource bindingSource1 = null;

        public Whitelist_Form()
        {
            InitializeComponent();

            bindingSource1 = new BindingSource(whitelist, "");
            listBox1.DataSource = bindingSource1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int sel = listBox1.SelectedIndex;
            whitelist.RemoveAt(sel);
            bindingSource1.ResetBindings(false);
            save_whitelist();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            whitelist.Add(textBox4.Text);
            bindingSource1.ResetBindings(false);
            save_whitelist();
        }

        private void save_whitelist()
        {
            try
            {
                // 保存用のファイルを開く
                using (StreamWriter sw = new StreamWriter(@"whitelist.csv", false, Encoding.GetEncoding("shift_jis")))
                {

                    int rowCount = whitelist.Count;

                    foreach (string row in whitelist)
                    {
                        sw.WriteLine(row);
                    }
                }
            }
            catch (Exception ex)
            {
                // ファイルを開くのに失敗したとき
                MessageBox.Show("データが不正です.\n" + ex.ToString(),
                                "エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

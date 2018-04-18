using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace にじさんじ監視ツール
{
    public partial class Start : Form
    {
        // Win32APIの GetPrivateProfileString を使う宣言
        [DllImport("KERNEL32.DLL")]
        public static extern uint
        GetPrivateProfileString(string lpAppName,
            string lpKeyName, string lpDefault,
            StringBuilder lpReturnedString, uint nSize,
            string lpFileName);

        Form1 form1 = null;
        Whitelist_Form whitelist_form = null;
        Setting setting = null;

        static string api_key = null;
        string video_id = null;
        string video_title = null;
        string channel_title = null;

        Dictionary<String, String> old_video_id = new Dictionary<String, String>()
        {
            { "UCD-miitqNY3nyukJ4Fnf4_A", "" }, //月ノ美兎
            { "UCsg-YqdqQ-KFF0LNk23BY4A", "" }, //樋口楓
            { "UC6oDys1BGgBsIC3WhG1BovQ", "" }, //静凛
            { "UCt9qik4Z-_J-rj3bKKQCeHg", "" }, //鈴谷アキ
            { "UCYKP16oMX9KKPbrNgo_Kgag", "" }, //エルフのえる
            { "UCvmppcdYf4HOv-tFQhHHJMA", "" }, //モイラ
            { "UCLO9QDxVL4bnvRRsz6K4bsQ", "" }, //勇気ちひろ
            { "UCeK9HFcRZoTrvqcUCtccMoQ", "" }, //渋谷ハジメ
            { "UC48jH1ul-6HOrcSSfoR02fQ", "" }, //夕陽リリ
            { "UC_GCs6GARLxEHxy1w40d6VQ", "" }, //家長むぎ
            { "UCv1fFr156jc65EMiLbaLImw", "" }, //剣持刀也
            { "UCt0clH12Xk1-Ej5PXKGfdPA", "" }, //物述有栖
            { "UCmUjjW5zF1MMOhYUwwwQv9Q", "" }, //宇志海いちご
            { "UCwokZsOK_uEre70XayaFnzA", "" }, //鈴鹿詩子
            { "UCBiqkFJljoxAj10SoP2w2Cg", "" }, //野良猫
            { "UCXU7YYxy_iQd3ulXyO-zC2w", "" }, //伏見ガク
            { "UCtpB6Bvhs1Um93ziEDACQ8g", "" }, //森中花咲
            { "UCUzJ90o1EjqUbk2pBAy0_aw", "" }  //ギルザレンIII世
        };

        static bool hasOpenLive = false;
        static bool hasOpenCommentViewer = true;

        string[] member_list = new string[] { "月ノ美兎", "樋口楓", "静凛", "鈴谷アキ", "エルフのえる", "モイラ", "勇気ちひろ", "渋谷ハジメ", "夕陽リリ", "家長むぎ", "剣持刀也", "物述有栖", "宇志海いちご", "鈴鹿詩子", "野良猫", "伏見ガク", "森中花咲", "ギルザレンIII世" };

        Dictionary<String, String> channel_list = new Dictionary<String, String>()
        {
            { "月ノ美兎", "UCD-miitqNY3nyukJ4Fnf4_A" },
            { "樋口楓", "UCsg-YqdqQ-KFF0LNk23BY4A" },
            { "静凛", "UC6oDys1BGgBsIC3WhG1BovQ" },
            { "鈴谷アキ", "UCt9qik4Z-_J-rj3bKKQCeHg" },
            { "エルフのえる", "UCYKP16oMX9KKPbrNgo_Kgag" },
            { "モイラ", "UCvmppcdYf4HOv-tFQhHHJMA" },
            { "勇気ちひろ", "UCLO9QDxVL4bnvRRsz6K4bsQ" },
            { "渋谷ハジメ", "UCeK9HFcRZoTrvqcUCtccMoQ" },
            { "夕陽リリ", "UC48jH1ul-6HOrcSSfoR02fQ" },
            { "家長むぎ", "UC_GCs6GARLxEHxy1w40d6VQ" },
            { "剣持刀也", "UCv1fFr156jc65EMiLbaLImw" },
            { "物述有栖", "UCt0clH12Xk1-Ej5PXKGfdPA" },
            { "宇志海いちご", "UCmUjjW5zF1MMOhYUwwwQv9Q" },
            { "鈴鹿詩子", "UCwokZsOK_uEre70XayaFnzA" },
            { "野良猫", "UCBiqkFJljoxAj10SoP2w2Cg" },
            { "伏見ガク", "UCXU7YYxy_iQd3ulXyO-zC2w" },
            { "森中花咲", "UCtpB6Bvhs1Um93ziEDACQ8g" },
            { "ギルザレンIII世", "UCUzJ90o1EjqUbk2pBAy0_aw" }
        };

        private static Dictionary<String, Boolean> channel_is_onair = new Dictionary<String, Boolean>()
        {
            { "UCD-miitqNY3nyukJ4Fnf4_A", false }, //月ノ美兎
            { "UCsg-YqdqQ-KFF0LNk23BY4A", false }, //樋口楓
            { "UC6oDys1BGgBsIC3WhG1BovQ", false }, //静凛
            { "UCt9qik4Z-_J-rj3bKKQCeHg", false }, //鈴谷アキ
            { "UCYKP16oMX9KKPbrNgo_Kgag", false }, //エルフのえる
            { "UCvmppcdYf4HOv-tFQhHHJMA", false }, //モイラ
            { "UCLO9QDxVL4bnvRRsz6K4bsQ", false }, //勇気ちひろ
            { "UCeK9HFcRZoTrvqcUCtccMoQ", false }, //渋谷ハジメ
            { "UC48jH1ul-6HOrcSSfoR02fQ", false }, //夕陽リリ
            { "UC_GCs6GARLxEHxy1w40d6VQ", false }, //家長むぎ
            { "UCv1fFr156jc65EMiLbaLImw", false }, //剣持刀也
            { "UCt0clH12Xk1-Ej5PXKGfdPA", false }, //物述有栖
            { "UCmUjjW5zF1MMOhYUwwwQv9Q", false }, //宇志海いちご
            { "UCwokZsOK_uEre70XayaFnzA", false }, //鈴鹿詩子
            { "UCBiqkFJljoxAj10SoP2w2Cg", false }, //野良猫
            { "UCXU7YYxy_iQd3ulXyO-zC2w", false }, //伏見ガク
            { "UCtpB6Bvhs1Um93ziEDACQ8g", false }, //森中花咲
            { "UCUzJ90o1EjqUbk2pBAy0_aw", false }  //ギルザレンIII世
        };

        public static Dictionary<String, Boolean> Channel_is_onair { get => channel_is_onair; set => channel_is_onair = value; }
        public static string Api_key { get => api_key; set => api_key = value; }
        public static bool HasOpenLive { get => hasOpenLive; set => hasOpenLive = value; }
        public static bool HasOpenCommentViewer { get => hasOpenCommentViewer; set => hasOpenCommentViewer = value; }

        public Start()
        {
            InitializeComponent();

            LoadIniFile();

            LoadCsvFile();

            SetComponents();

            var a = new EventHandler(OpenLive);

            timer1.Tick += new EventHandler(OpenLive);
            timer1.Enabled = true;
        }

        private void LoadIniFile()
        {
            Api_key = GetIniStringValue("section", "api_key");
            HasOpenLive = GetIniBooleanValue("section", "hasOpenLive");
            HasOpenCommentViewer = GetIniBooleanValue("section", "hasOpenCommentViewer");

            if (Api_key == "")
            {
                setting = new Setting();
                AddOwnedForm(setting);
                setting.Show();
            }
        }

        public string GetIniStringValue(string section, string key)
        {
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "config.ini";

            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, string.Empty, sb, Convert.ToUInt32(sb.Capacity), iniFileName);
            return sb.ToString();
        }

        public bool GetIniBooleanValue(string section, string key)
        {
            string iniFileName = AppDomain.CurrentDomain.BaseDirectory + "config.ini";

            StringBuilder sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, "false", sb, Convert.ToUInt32(sb.Capacity), iniFileName);
            return Convert.ToBoolean(sb.ToString());
        }

        private void LoadCsvFile()
        {
            try
            {
                // csvファイルを開く
                using (var sr = new StreamReader(@"whitelist.csv", System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    // ストリームの末尾まで繰り返す
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        // ファイルから一行読み込む
                        string line = sr.ReadLine();

                        Whitelist_Form.Whitelist.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                // ファイルを開くのに失敗したとき
                MessageBox.Show(ex.ToString(),
                                "エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void OpenLive(object sender, EventArgs e)
        {
            foreach (string channel_id in channel_list.Values)
            {
                CheckOnAir(channel_id, "Auto");
            }
        }

        private void CheckOnAir(string channel_id, string manualElement)
        {
            var video_id_request = WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=" + channel_id + "&type=video&eventType=live&key=" + Api_key);

            try
            {
                using (var video_id_response = video_id_request.GetResponse())
                {
                    using (var video_id_stream = new StreamReader(video_id_response.GetResponseStream(), Encoding.UTF8))
                    {
                        

                        var video_id_object = Codeplex.Data.DynamicJson.Parse(video_id_stream.ReadToEnd());

                        string live = video_id_object.items[0].snippet.liveBroadcastContent;

                        if (live == "live")
                        {
                            video_id = video_id_object.items[0].id.videoId;

                            video_title = video_id_object.items[0].snippet.title;

                            channel_title = video_id_object.items[0].snippet.channelTitle;

                            if ((video_id != old_video_id[channel_id]) && manualElement == "Auto")
                            {
                                if (HasOpenLive)
                                {
                                    System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=" + video_id);
                                }

                                if (HasOpenCommentViewer)
                                {
                                    form1 = new Form1(video_id, video_title, channel_id, channel_title);
                                    AddOwnedForm(form1);
                                    form1.Show();
                                }
                            }
                            else
                            {
                                if(manualElement == "Live")
                                {
                                    System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=" + video_id);
                                }
                                if(manualElement == "CommentViewer")
                                {
                                    form1 = new Form1(video_id, video_title, channel_id, channel_title);
                                    AddOwnedForm(form1);
                                    form1.Show();
                                }
                            } 

                            old_video_id[channel_id] = video_id;
                        }
                    }
                }
            }
            catch
            {
                if (manualElement != "Auto")
                    MessageBox.Show("ただいま配信は行われておりません。", "にじさんじ監視ツール", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        

        

        private void SetComponents()
        {
            ContextMenuStrip mainMenu = new ContextMenuStrip();

            ToolStripMenuItem liveMenuItem = new ToolStripMenuItem();
            ContextMenuStrip liveMenu = new ContextMenuStrip();
            liveMenuItem.Text = "配信ページを開く";
            liveMenuItem.DropDown = liveMenu;
            #region liveMenu
            ToolStripMenuItem[] liveItem = new ToolStripMenuItem[18];

            for (int i = 0; i < liveItem.Length; i++)
            {
                liveItem[i] = new ToolStripMenuItem();
                liveItem[i].Text = member_list[i];
                liveItem[i].Click += new EventHandler(OpenLiveManually);
            }
            liveMenu.Items.AddRange(liveItem);
            #endregion
            mainMenu.Items.Add(liveMenuItem);

            ToolStripMenuItem commentViewerMenuItem = new ToolStripMenuItem();
            ContextMenuStrip commentViewerMenu = new ContextMenuStrip();
            commentViewerMenuItem.Text = "コメントビューワを開く";
            commentViewerMenuItem.DropDown = commentViewerMenu;
            #region commentViewerMenu
            ToolStripMenuItem[] commentViewerItem = new ToolStripMenuItem[18];

            for(int i = 0; i< commentViewerItem.Length;i++)
            {
                commentViewerItem[i] = new ToolStripMenuItem();
                commentViewerItem[i].Text = member_list[i];
                commentViewerItem[i].Click += new EventHandler(OpenCommentViewerManually);
            }
            commentViewerMenu.Items.AddRange(commentViewerItem);
            #endregion
            mainMenu.Items.Add(commentViewerMenuItem);

            ToolStripMenuItem whitelistMenuItem = new ToolStripMenuItem();
            whitelistMenuItem.Text = "Whitelistの編集";
            whitelistMenuItem.Click += new EventHandler(Open_Whitelist);
            mainMenu.Items.Add(whitelistMenuItem);

            ToolStripMenuItem settingMenuItem = new ToolStripMenuItem();
            settingMenuItem.Text = "設定";
            settingMenuItem.Click += new EventHandler(Setting_Click);
            mainMenu.Items.Add(settingMenuItem);

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem();
            exitMenuItem.Text = "終了";
            exitMenuItem.Click += new EventHandler(Close_Click);
            mainMenu.Items.Add(exitMenuItem);

            notifyIcon1.ContextMenuStrip = mainMenu;
        }

        private void OpenLiveManually(object sender, EventArgs e)
        {
            CheckOnAir(channel_list[((ToolStripMenuItem)sender).Text], "Live");
        }

        private void OpenCommentViewerManually(object sender, EventArgs e)
        {
            CheckOnAir(channel_list[((ToolStripMenuItem)sender).Text], "CommentViewer");
        }

        private void Open_Whitelist(object sender, EventArgs e)
        {
            whitelist_form = new Whitelist_Form();
            AddOwnedForm(whitelist_form);
            whitelist_form.Show();
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            setting = new Setting();
            AddOwnedForm(setting);
            setting.Show();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

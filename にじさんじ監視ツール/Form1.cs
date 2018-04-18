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
using System.Text.RegularExpressions;
using Codeplex.Data;
using System.Threading;

namespace にじさんじ監視ツール
{
    public partial class Form1 : Form
    {
        string video_id = null;
        string video_title = null;
        string channel_id = null;
        string channel_title = null;
        string api_key = "AIzaSyBbdediKFpbc6KcBKxMdrK-Gq-JL_Duz1s";
        string live_chat_id = null;

        bool show_owners_message = true;



        dynamic messages_object = null;
        Dictionary<string, object[]> messages_dictionary = new Dictionary<string, object[]>();
        List<string> messages_ids = new List<string>();
        List<string> messages_ids_old = new List<string>();
        List<string> messages_ids_diff = new List<string>();

        public Form1(string vi, string vt, string ci, string ct)
        {
            InitializeComponent();

            video_id = vi;
            video_title = vt;
            channel_id = ci;
            channel_title = ct;


            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = video_title;



            var live_chat_id_request = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + video_id + "&key=" + api_key);

            try
            {
                using (var live_chat_id_response = live_chat_id_request.GetResponse())
                {
                    using (var live_chat_id_stream = new StreamReader(live_chat_id_response.GetResponseStream(), Encoding.UTF8))
                    {
                        var live_chat_id_object = DynamicJson.Parse(live_chat_id_stream.ReadToEnd());

                        live_chat_id = live_chat_id_object.items[0].liveStreamingDetails.activeLiveChatId;

                        if (live_chat_id == null)
                        {
                            textBox1.Text = "Error: Live Chat IDの取得に失敗しました";
                            return;
                        }

                    }
                }
            }
            catch
            {
                textBox1.Text = "Error: Live Chat IDの取得に失敗しました";
                return;
            }

            textBox1.Text = "「" + channel_title + "」の「" + video_title + "」に接続しました\r\n";

            timer1.Tick += new EventHandler(get_Comment);
            timer1.Enabled = true;
        }

        private void get_Comment(object sender, EventArgs e)
        {
            {
                var messages_request = WebRequest.Create("https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&liveChatId=" + live_chat_id + "&key=" + api_key);

                messages_object = null;

                try
                {
                    using (var messages_response = messages_request.GetResponse())
                    {
                        using (var messages_stream = new StreamReader(messages_response.GetResponseStream()))
                        {
                            messages_object = DynamicJson.Parse(messages_stream.ReadToEnd());
                        }
                    }
                }
                catch
                {
                    textBox1.Text = "Error: コメントの取得に失敗しました";
                }

                messages_ids.Clear();
                messages_dictionary.Clear();

                foreach (var value in messages_object.items)
                {
                    messages_ids.Add(value.id);

                    messages_dictionary.Add(value.id, new object[]
                    {
                    value.authorDetails.displayName,
                    value.snippet.textMessageDetails.messageText,
                    value.authorDetails.isChatOwner
                    });
                }

                messages_ids_diff = new List<string>(messages_ids);
                messages_ids_diff.RemoveAll(messages_ids_old.Contains);

                foreach (var value in messages_ids_diff)
                {
                    if (show_owners_message || !Convert.ToBoolean(messages_dictionary[value][2]))
                    {
                        var message_sender = messages_dictionary[value][0];
                        var message_text = messages_dictionary[value][1];

                        if ((Whitelist_Form.Whitelist.Contains(message_sender + "")))
                            textBox1.AppendText(message_sender + " : " + message_text + "\r\n");
                    }
                }

                messages_ids_old.Clear();
                messages_ids_old = new List<string>(messages_ids);

                messages_ids_diff.Clear();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Start.Channel_is_onair[channel_id] = false;
        }

        private void URL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=" + video_id);
        }
    }
}

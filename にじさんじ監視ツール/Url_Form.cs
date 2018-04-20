using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace にじさんじ監視ツール
{
    public partial class Url_Form : Form
    {
        public Url_Form()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var video_id = textBox1.Text.Substring(textBox1.Text.Length - 11);

            var channel_id_request = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=snippet&id=" + video_id + "&key=" + Start.Api_key);

            try
            {
                using (var channel_id_response = channel_id_request.GetResponse())
                {
                    using (var channel_id_stream = new StreamReader(channel_id_response.GetResponseStream(), Encoding.UTF8))
                    {


                        var channel_id_object = Codeplex.Data.DynamicJson.Parse(channel_id_stream.ReadToEnd());

                        string live = channel_id_object.items[0].snippet.liveBroadcastContent;

                        if (live == "live")
                        {
                            var channel_id = channel_id_object.items[0].snippet.channelId;

                            var video_title = channel_id_object.items[0].snippet.title;

                            var channel_title = channel_id_object.items[0].snippet.channelTitle;

                            //old_video_id[channel_id] = video_id;

                            Form1 form1 = new Form1(video_id, video_title, channel_id, channel_title);
                            AddOwnedForm(form1);
                            form1.Show();

                            this.Visible = false;
                                
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("ただいま配信は行われておりません。", "にじさんじ監視ツール", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

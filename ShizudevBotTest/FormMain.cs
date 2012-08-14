using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Twitterizer;
using Twitterizer.Core;
using Twitterizer.Entities;
using Twitterizer.Streaming;

namespace ShizudevBotTest
{
    public partial class FormMain : Form
    {
        private OAuthTokens tokens;

        private List<string> Messages;

        private int messageCount;
        
        public FormMain() {
            InitializeComponent();
            tokens = new OAuthTokens();
            tokens.AccessToken = "6167462-AmIvUlWCIGGWJgmZZXbLy5wAhC2rQ0YKeSPAbZo";
            tokens.AccessTokenSecret = "ydNacQC1dzdyiLd99y5sev0869jLMATScNFg4iCRM";
            tokens.ConsumerKey = "ohYQEUAGLnWA5NPWA1ag";
            tokens.ConsumerSecret = "BBnxoaBuhlgdsO4VeCYQonhrhMapCr3B8hDDJv989s";

            messageCount = 0;
        }

        private void button1_Click(object sender, EventArgs e) {
            SendTwitterMessage();
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e) {

            SendTwitterMessage();
        }

        private void SendTwitterMessage() {

            if (messageCount >= Messages.Count) {
                messageCount = 0;
            }


            //80文字以上の文字列は80文字で切る。80文字のカウントはサロゲートペアを考慮する。
            var si = new System.Globalization.StringInfo(Messages[messageCount]);
            string tweetMessage;
            const int MaxLength = 80;
            if (si.LengthInTextElements > MaxLength) {
                tweetMessage = si.SubstringByTextElements(0, MaxLength);
            }
            else {
                tweetMessage = si.String;
            }
            messageCount++;
            label1.Text = "Now Tweeting!";

            //同期でのやり方
            //TwitterResponse<TwitterStatus> tweetRespose = TwitterStatus.Update(tokens, tweetMessage);
            //if (tweetRespose.Result == RequestResult.Success) {
            //    label1.Text = "StatusUpdate Success!! " + DateTime.Now.ToString();
            //}
            //else {
            //    label1.Text = "StatusUpdate Abnormal!!" + res.Result.ToString() + " " + DateTime.Now.ToString();
            //}

            //非同期でのやり方
            var option = new StatusUpdateOptions();
            option.UseSSL = false;

            var result = TwitterStatusAsync.Update(tokens, tweetMessage, option, new TimeSpan(0, 1, 0), res => {
                if (res.Result == RequestResult.Success) {
                    BeginInvoke(new Action(() => {
                        label1.Text = "StatusUpdate Success!! " + DateTime.Now.ToString();
                    }));
                }
                else {
                    BeginInvoke(new Action(() => {
                        label1.Text = "StatusUpdate Abnormal!!" + res.Result.ToString() + " " + DateTime.Now.ToString();
                    }));
                }
            });
        }

        private void button2_Click(object sender, EventArgs e) {
            timer1.Stop();
            label1.Text = "";
        }

        private void FormMain_Load(object sender, EventArgs e) {
            Messages = new List<string>();
            var reader = new System.IO.StreamReader(@"Message.txt", Encoding.GetEncoding("utf-8"));
            while (reader.Peek() > -1) {
                // メッセージの先頭が#だったらコメントと見なして読み飛ばす。
                var commentChar = "#";
                var line = reader.ReadLine();
                if (line.IndexOf(commentChar) != 0) {
                    Messages.Add(line);
                }
            }

        }

        
    }
}

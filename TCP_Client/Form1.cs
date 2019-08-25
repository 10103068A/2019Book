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
using System.Net.Sockets;
using System.Threading;

namespace TCP_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket T;
        Thread Th;
        string User;
        bool isListen;

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox4.Text = "請輸入您的名字！";
                return;
            }
            CheckForIllegalCrossThreadCalls = false;
            string IP = textBox1.Text;
            int Port = int.Parse(textBox2.Text);
            User = textBox3.Text;
            if (button1.Text == "登入伺服器")
            {
                try
                {
                    IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);
                    T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    T.Connect(EP);
                    Th = new Thread(Listen);
                    Th.IsBackground = true;
                    Th.Start();
                    textBox4.Text = "已經連上伺服器\r\n";
                    Send("0" + User);
                    button1.Text = "登出伺服器";
                    button2.Enabled = true;

                }
                catch (Exception)
                {
                    textBox4.Text = "無法連上伺服器\r\n";
                }
            }
            else if (button1.Text == "登出伺服器")
            {
                isListen = false;
                Send("9" + User);
                stopConnect();
                textBox4.Text += "已經離開\r\n";
            }
        }
        
        private void stopConnect()
        {
            T.Close();
            listBox1.Items.Clear();
            button1.Text = "登入伺服器";
            Th.Abort();
        }

        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);
            T.Send(B, 0, B.Length, SocketFlags.None);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button1.Text == "登出伺服器")
            {
                Send("9" + User);
                T.Close();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "") return;
            if (listBox1.SelectedIndex < 0)
            {
                Send($"1{User}公告：{textBox5.Text}");
            }
            else
            {
                Send($"2來自{User}：{textBox5.Text}|{listBox1.SelectedItem}");
                textBox4.Text += $"告訴{listBox1.SelectedItem}：{textBox5.Text}\r\n";
            }
            textBox5.Text = "";
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            listBox1.ClearSelected();
        }

        private void Listen()
        {
            EndPoint ServerEP = (EndPoint)T.RemoteEndPoint;
            byte[] B = new byte[1023];
            int inLen = 0;
            string Msg;
            string St;
            string Str;
            isListen = true;
            while (isListen)
            {
                try
                {
                    inLen = T.ReceiveFrom(B, ref ServerEP);
                }
                catch (Exception)
                {
                    if (isListen)
                    {
                        MessageBox.Show("伺服器斷了！");
                        stopConnect();
                    }
                }

                Msg = Encoding.Default.GetString(B, 0, inLen);
                St = Msg.Substring(0, 1);
                Str = Msg.Substring(1);
                switch (St)
                {
                    case "L":
                        listBox1.Items.Clear();
                        string[] M = Str.Split(',');
                        foreach (var m in M)
                        {
                            listBox1.Items.Add(m);
                        }
                        break;
                    case "1":
                        textBox4.Text += $"(公開){Str}\r\n";
                        textBox4.SelectionStart = textBox4.Text.Length;
                        textBox4.ScrollToCaret();
                        break;
                    case "2":
                        textBox4.Text += $"(私密){Str}\r\n";
                        textBox4.SelectionStart = textBox4.Text.Length;
                        textBox4.ScrollToCaret();
                        break;
                }
            }
        }

        private void TextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCP_Gobang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ShapeContainer CVS;
        byte[,] S;
        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap bg = new Bitmap(570, 570);
            Graphics g = Graphics.FromImage(bg);
            g.Clear(Color.White);
            for (int i = 15; i <= 555; i += 30)
            {
                g.DrawLine(Pens.Black, i, 15, i, 555);
            }
            for (int j = 15; j <= 555; j += 30)
            {
                g.DrawLine(Pens.Black, 15, j, 555, j);
            }
            panel1.BackgroundImage = bg;
            CVS = new ShapeContainer();
            panel1.Controls.Add(CVS);
            S = new byte[19, 19];
        }

        private void Chess(int i, int j, Color BW)
        {
            OvalShape C = new OvalShape();
            C.Width = 26;
            C.Height = 26;
            C.Left = i * 30 + 2;
            C.Top = j * 30 + 2;
            C.FillStyle = FillStyle.Solid;
            C.FillColor = BW;
            C.Parent = CVS;
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int i = e.X / 30;
            int j = e.Y / 30;
            if (S[i, j] == 0)
            {
                Chess(i, j, Color.Black);
                S[i, j] = 1;
            }
            if (listBox1.SelectedIndex >= 0)
            {
                Send($"6{i.ToString()},{j.ToString()}|{listBox1.SelectedItem}");
                panel1.Enabled = false;
            }
            if (chk5(i, j, 1)) MessageBox.Show("你贏了！");
        }

        private bool chk5(int i, int j, byte tg)
        {
            int ii, jj;
            //水平
            int n = 0;
            for (int k = -4; k <= 4; k++)
            {
                ii = i + k;
                if (ii >= 0 && ii < 19)
                {
                    if (S[ii, j] == tg)
                    {
                        n += 1;
                        if (n == 5) return true;
                    }
                    else
                    {
                        n = 0;
                    }
                }
            }
            //垂直
            n = 0;
            for (int k = -4; k <= 4; k++)
            {
                jj = j + k;
                if (jj >= 0 && jj < 19)
                {
                    if (S[i, jj] == tg)
                    {
                        n += 1;
                        if (n == 5) return true;
                    }
                    else
                    {
                        n = 0;
                    }
                }
            }
            //左上右下
            n = 0;
            for (int k = -4; k <= 4; k++)
            {
                ii = i + k;
                jj = j + k;
                if (ii >= 0 && ii < 19 && jj >= 0 && jj < 19)
                {
                    if (S[ii, jj] == tg)
                    {
                        n += 1;
                        if (n == 5) return true;
                    }
                    else
                    {
                        n = 0;
                    }
                }
            }
            //右上左下
            n = 0;
            for (int k = -4; k <= 4; k++)
            {
                ii = i - k;
                jj = j + k;
                if (ii >= 0 && ii < 19 && jj >= 0 && jj < 19)
                {
                    if (S[ii, jj] == tg)
                    {
                        n += 1;
                        if (n == 5) return true;
                    }
                    else
                    {
                        n = 0;
                    }
                }
            }
            return false;
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
            Control.CheckForIllegalCrossThreadCalls = false;
            User = textBox3.Text;
            string IP = textBox1.Text;
            int Port = int.Parse(textBox2.Text);
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
                    textBox4.Text = "已連接伺服器\r\n";
                    textBox3.Enabled = false;
                    Send("0" + User);
                    button1.Text = "登出伺服器";
                }
                catch (Exception)
                {
                    textBox4.Text = "無法連上伺服器\r\n";
                }
            }
            else if (button1.Text == "登出伺服器")
            {
                textBox3.Enabled = true;
                isListen = false;
                Send("9" + User);
                stopConnect();
                textBox4.Text = "已經離開\r\n";
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
                    stopConnect();
                    MessageBox.Show("伺服器斷了！");
                }
                Msg = Encoding.Default.GetString(B, 0, inLen);
                St = Msg.Substring(0, 1);
                Str = Msg.Substring(1);
                switch (St)
                {
                    case "L":
                        listBox1.Items.Clear();
                        string[] M = Str.Split(',');
                        listBox1.Items.AddRange(M);
                        break;
                    case "5":
                        CVS.Shapes.Clear();
                        S = new byte[19, 19];
                        panel1.Enabled = true;
                        break;
                    case "6":
                        string[] D = Str.Split(',');
                        int x = int.Parse(D[0]);
                        int y = int.Parse(D[1]);
                        Chess(x, y, Color.White);
                        S[x, y] = 2;
                        panel1.Enabled = true;
                        if (chk5(x, y, 2))
                        {
                            MessageBox.Show("你輸了！！");
                        }
                        break;
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                Send($"5C|{listBox1.SelectedItem}");
            }
            CVS.Shapes.Clear();
            S = new byte[19, 19];
            panel1.Enabled = true;
        }
    }
}

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

namespace TCP_ShootGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Select();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Z:
                    P.Left -= 5;
                    break;
                case Keys.X:
                    P.Left += 5;
                    break;
                case Keys.Space:
                    myShot();
                    break;
            }
            if (listBox1.SelectedIndex >= 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                    case Keys.X:
                        Send("3" + P.Left.ToString() + "|" + listBox1.SelectedItem);
                        break;
                    case Keys.Space:
                        Send("4S|" + listBox1.SelectedItem);
                        break;
                }
            }
            button2.Select();
        }

        private void myShot()
        {
            Label B = new Label();
            B.Tag = "B";
            B.Width = 3;
            B.Height = 6;
            B.BackColor = Color.Red;
            B.Left = P.Left + P.Width / 2 - B.Width / 2;
            B.Top = P.Top - B.Height;
            panel1.Controls.Add(B);
        }
        private void XShot()
        {
            Label B = new Label();
            B.Tag = "X";
            B.Width = 3;
            B.Height = 6;
            B.BackColor = Color.Gray;
            B.Left = Q.Left + Q.Width / 2 - B.Width / 2;
            B.Top = Q.Bottom;
            panel1.Controls.Add(B);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            foreach (Control c in panel1.Controls)
            {
                string s = c.Tag.ToString();
                switch (s)
                {
                    case "B":
                        c.Top -= 5;
                        if (c.Bottom < 0) c.Dispose();
                        if (chkHit((Label)c, Q))
                        {
                            c.Dispose();
                            Score.Text = (int.Parse(Score.Text) + 1).ToString();
                        }
                        break;
                    case "X":
                        c.Top += 5;
                        if (c.Top >panel1.Height) c.Dispose();
                        break;
                }
            }
        }
        private bool chkHit(Label B, PictureBox C)
        {
            if (B.Right < C.Left) return false;
            if (B.Left > C.Right) return false;
            if (B.Bottom < C.Top) return false;
            if (B.Top > C.Bottom) return false;
            return true;
        }

        Socket T;
        Thread Th;
        string User;
        private void Button1_Click(object sender, EventArgs e)
        {
            button2.Select();
            Control.CheckForIllegalCrossThreadCalls = false;
            User = textBox3.Text;
            string IP = textBox1.Text;
            int Port = int.Parse(textBox2.Text);

            try
            {
                IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);
                T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                T.Connect(EP);
                Th = new Thread(Listen);
                Th.IsBackground = true;
                Th.Start();
                textBox4.Text = "已連接伺服器\r\n";
                Send("0" + User);
                button1.Enabled = false;
            }
            catch (Exception)
            {
                textBox4.Text = "無法連上伺服器\r\n";
            }
        }

        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);
            T.Send(B, 0, B.Length, SocketFlags.None);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button1.Enabled == false)
            {
                Send("9" + User);
                T.Close();
            }
        }

        bool Xbang;
        private void Listen()
        {
            EndPoint ServerEP = (EndPoint)T.RemoteEndPoint;
            byte[] B = new byte[1023];
            int inLen = 0;
            string Msg;
            string St;
            string Str;
            while (true)
            {
                try
                {
                    inLen = T.ReceiveFrom(B, ref ServerEP);
                }
                catch (Exception)
                {
                    T.Close();
                    listBox1.Items.Clear();
                    MessageBox.Show("伺服器斷了！");
                    button1.Enabled = true;
                    Th.Abort();
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
                    case "3":
                        Q.Left = panel1.Width - int.Parse(Str) - Q.Width;
                        break;
                    case "4":
                        Xbang = true;
                        break;
                }
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Select();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (Xbang)
            {
                XShot();
                Xbang = false;
            }
        }
    }
}

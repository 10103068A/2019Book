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

namespace TCP_Tennis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GO_Click(object sender, EventArgs e)
        {
            Q.Tag = new Point(5, -5);
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Point V = (Point)Q.Tag;
            Q.Left += V.X;
            Q.Top += V.Y;
            chkHit(Q, G, true);
            chkHit(Q, H1, false);
            chkHit(Q, H2, false);
            if (listBox1.SelectedIndex >= 0)
            {
                Send("8" + Q.Left.ToString() + "," + Q.Top.ToString() + "|" + listBox1.SelectedItem);
            }
        }

        private bool chkHit(Label B, object C, bool inside)
        {
            Point V = (Point)B.Tag;
            if (inside)
            {
                Panel p = (Panel)C;
                if (B.Right > p.Width)//右牆碰撞
                {
                    V.X = -Math.Abs(V.X);
                    B.Tag = V;
                    return true;
                }
                if (B.Left < 0)//左牆碰撞
                {
                    V.X = Math.Abs(V.X);
                    B.Tag = V;
                    return true;
                }
                if (B.Bottom > p.Height)//地板碰撞
                {
                    V.Y = -Math.Abs(V.Y);
                    B.Tag = V;
                    return true;
                }
                if (B.Top < 0)//屋頂碰撞
                {
                    V.Y = Math.Abs(V.Y);
                    B.Tag = V;
                    return true;
                }
                return false;//未發生碰撞
            }
            else //羽拍碰撞
            {
                Label k = (Label)C;
                if (B.Right < k.Left) return false; //球在物件之左 確定未碰撞
                if (B.Left > k.Right) return false; //球在物件之右 確定未碰撞
                if (B.Bottom < k.Top) return false; //球在物件之上 確定未碰撞
                if (B.Top > k.Bottom) return false; //球在物件之下 確定未碰撞

                //目標左側碰撞
                if (B.Right >= k.Left && (B.Right - k.Left) <= Math.Abs(V.X)) V.X = -Math.Abs(V.X);
                //目標右側碰撞
                if (B.Left <= k.Right && (k.Right - B.Left) <= Math.Abs(V.X)) V.X = Math.Abs(V.X);
                //目標底部碰撞
                if (B.Top <= k.Bottom && (k.Bottom - B.Top) <= Math.Abs(V.Y)) V.Y = Math.Abs(V.Y);
                //目標頂部碰撞
                if (B.Bottom >= k.Top && (B.Bottom - k.Top) <= Math.Abs(V.Y)) V.Y = -Math.Abs(V.Y);
                B.Tag = V;
                return true;
            }
        }

        int mdx;
        private void H1_MouseDown(object sender, MouseEventArgs e)
        {
            mdx = e.X;
        }

        int oX;
        private void H1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int X = H1.Left + e.X - mdx;//試算拖曳終點位置
                if (X < 0) X = 0;//不能超出左邊界
                if (X > G.Width - H1.Width) X = G.Width - H1.Width;//不能超出右邊界
                H1.Left = X;
                if (listBox1.SelectedIndex >= 0)
                {
                    if (oX != H1.Left)
                    {
                        Send("7" + H1.Left.ToString() + "|" + listBox1.SelectedItem);
                        oX = H1.Left;
                    }
                }
            }
        }

        Socket T;
        Thread Th;
        string User;
        private void Button1_Click(object sender, EventArgs e)
        {
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
                    case "7":
                        H2.Left = G.Width - int.Parse(Str) - H2.Width; //鏡射之後的位置
                        break;
                    case "8":
                        string[] C = Str.Split(',');
                        Q.Left = G.Width - int.Parse(C[0]) - Q.Width;//左右鏡射位置
                        Q.Top = G.Height - Q.Height - int.Parse(C[1]);//上下鏡射位置
                        break;
                }
            }
        }
    }
}

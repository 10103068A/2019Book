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

namespace UDP_BullsNCows
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        UdpClient U;
        Thread Th;

        private void Listen()
        {
            var port = int.Parse(textBox1.Text);
            U = new UdpClient(port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            while (true)
            {
                byte[] B = U.Receive(ref EP);
                string G = Encoding.Default.GetString(B);
                if (G.Substring(1, 1) == "A")
                {
                    listBox2.Items.Add($"{textBox5.Text} >> {G}");
                    textBox5.Text = "";
                    button2.Enabled = false;
                    if (G == "4A0B")
                    {
                        gameEnd("你贏了");
                    }
                }
                else
                {
                    string k = chk(G);
                    Send(k);
                    listBox1.Items.Add($"{G} >> {k}");
                    button2.Enabled = true;
                    if (k == "4A0B")
                    {
                        gameEnd("你輸了");
                    }
                }
            }
        }

        private void gameEnd(string msg)
        {
            MessageBox.Show(msg);
            textBox4.Enabled = true;
            button3.Enabled = true;
            button4.BackColor = Color.Empty;
            button5.BackColor = Color.Empty;
            button6.BackColor = Color.Empty;
            button7.BackColor = Color.Empty;
            button8.BackColor = Color.Empty;
            button9.BackColor = Color.Empty;
            button10.BackColor = Color.Empty;
            button11.BackColor = Color.Empty;
            button12.BackColor = Color.Empty;
            button13.BackColor = Color.Empty;
            listBox1.Items.Add("~~~~~~~~~~");
            listBox2.Items.Add("~~~~~~~~~~");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            (sender as Button).Enabled = false;
            textBox1.Enabled = false;
        }

        private string MyIP()
        {
            string hn = Dns.GetHostName();
            IPAddress[] ip = Dns.GetHostEntry(hn).AddressList;
            foreach (var it in ip)
            {
                if (it.AddressFamily == AddressFamily.InterNetwork)
                {
                    return it.ToString();
                }
            }
            return "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += "" + MyIP();
        }

        private void TextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort();
                U.Close();
            }
            catch
            {

            }
        }

        private void Send(string post)
        {
            int Port = int.Parse(textBox3.Text);
            UdpClient S = new UdpClient(textBox2.Text, Port);
            byte[] B = Encoding.Default.GetBytes(post);
            S.Send(B, B.Length);
            S.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (licit(textBox5.Text))
            {
                Send(textBox5.Text);
            }
        }

        private string chk(string G)
        {
            int A = 0, B = 0;
            char[] my = textBox4.Text.ToCharArray();
            char[] other = G.ToCharArray();

            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    if (my[i] == other[j])
                    {
                        if (i == j)
                        {
                            A += 1;
                        }
                        else
                        {
                            B += 1;
                        }
                    }
                }
            }
            return $"{A}A{B}B";
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (licit(textBox4.Text))
            {
                textBox4.Enabled = false;
                button3.Enabled = false;
            }
        }
        private bool licit(string S)
        {
            bool ret = true;
            bool isnum = int.TryParse(S, out int input);
            if (S.Length != 4 || !isnum)
            {
                MessageBox.Show("必須是4個數字！");
                ret=false;
            }
            var c = S.ToCharArray();
            for (int i = 0; i < 4; i++)
            {
                for (int j = i + 1; j < 4; j++)
                {
                    if (c[i] == c[j])
                    {
                        MessageBox.Show("數字不能重複");
                        ret =false;
                    }
                }
            }
            return ret;
        }

        private void numButton_Click(object sender, EventArgs e)
        {
            if((sender as Button).BackColor == Color.DarkGreen)
            {
                (sender as Button).BackColor = Color.DarkRed;
            }
            else if ((sender as Button).BackColor == Color.DarkRed)
            {
                (sender as Button).BackColor = Color.Empty;
            }
            else
            {
                (sender as Button).BackColor = Color.DarkGreen;
            }
        }
    }
}

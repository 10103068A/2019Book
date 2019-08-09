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

namespace UDP_XO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        UdpClient U;
        Thread Th;
        bool isclear = false;
        private void Listen()
        {
            var port = int.Parse(textBox1.Text);
            U = new UdpClient(port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            while (true)
            {
                byte[] B = U.Receive(ref EP);
                string A = Encoding.Default.GetString(B);
                if (chk(A))
                {
                    label4.Text = "你輸了";
                }
                char[] C = A.ToCharArray();
                for (int i = 0; i < 9; i++)
                {
                    switch (C[i])
                    {
                        case 'O':
                            C[i] = 'X';
                            break;
                        case 'X':
                            C[i] = 'O';
                            break;
                    }
                    Button D = (Button)this.Controls[$"B{i}"];
                    D.Tag = C[i];

                    switch (C[i])
                    {
                        case '_':
                            D.Image = Properties.Resources.E;
                            break;
                        case 'O':
                            D.Image = Properties.Resources.O;
                            break;
                        case 'X':
                            D.Image = Properties.Resources.X;
                            break;
                    }
                }
                T = true;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            button1.Enabled = false;
            textBox1.Enabled = false;
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

        private bool chk(string A)
        {
            var C = A.ToCharArray();

            if (C[0] == 'O' && C[1] == 'O' && C[2] == 'O') { return true; }
            if (C[3] == 'O' && C[4] == 'O' && C[5] == 'O') { return true; }
            if (C[6] == 'O' && C[7] == 'O' && C[8] == 'O') { return true; }

            if (C[0] == 'O' && C[3] == 'O' && C[6] == 'O') { return true; }
            if (C[1] == 'O' && C[4] == 'O' && C[7] == 'O') { return true; }
            if (C[2] == 'O' && C[5] == 'O' && C[8] == 'O') { return true; }

            if (C[0] == 'O' && C[4] == 'O' && C[8] == 'O') { return true; }
            if (C[2] == 'O' && C[4] == 'O' && C[6] == 'O') { return true; }
            return false;
        }

        bool T = true;
        private void B_Click(object sender, EventArgs e)
        {
            if (!T) return;

            Button B = (Button)sender;
            if (B.Tag.ToString() != "_") return;
            B.Image = Properties.Resources.O;
            B.Tag = "O";

            string A = "";
            for (int i = 0; i < 9; i++)
            {
                A += this.Controls[$"B{i}"].Tag;
            }
            label4.Text = "";

            if (chk(A))
            {
                label4.Text = "你贏了！";
            }

            int Port = int.Parse(textBox3.Text);
            UdpClient S = new UdpClient(textBox2.Text, Port);
            byte[] K = Encoding.Default.GetBytes(A);
            S.Send(K, K.Length);
            S.Close();

            T = false;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                Button D = (Button)this.Controls[$"B{i}"];
                D.Tag = "_";
                D.Image = Properties.Resources.E;
                T = true;
            }
        }
    }
}

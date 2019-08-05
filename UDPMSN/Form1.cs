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

namespace UDPMSN
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
                textBox2.Text = Encoding.Default.GetString(B);
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

        private void Button2_Click(object sender, EventArgs e)
        {
            var IP = textBox3.Text;
            try
            {
                int Port = int.Parse(textBox4.Text);
                byte[] B = Encoding.Default.GetBytes(textBox5.Text);
                UdpClient S = new UdpClient();
                S.Send(B, B.Length, IP, Port);
                S.Close();
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

        private void TextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2.PerformClick();
                e.SuppressKeyPress = true;
                textBox5.Text = "";
            }
        }
    }
}

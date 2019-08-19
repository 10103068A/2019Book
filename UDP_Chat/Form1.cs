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
using System.Collections;

namespace UDP_Chat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        UdpClient U;
        Thread Th;
        String MyName;
        ArrayList ips = new ArrayList();
        const short Port = 2019;
        string BC = IPAddress.Broadcast.ToString();
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            label4.Text = "我的IP：" + MyIP();
        }

        private void Send(string ToIP, string msg, string toWhom)
        {
            //我的名字:我的IP:訊息:傳給誰
            string A = $"{MyName}:{MyIP()}:{msg}:{toWhom}";
            byte[] B = Encoding.Default.GetBytes(A);
            UdpClient V = new UdpClient(ToIP, Port);
            V.Send(B, B.Length);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("請輸入姓名！");
                return;
            }
            MyName = textBox1.Text;
            listBox1.Items.Clear();
            ips.Clear();
            if (button1.Text == "上線")
            {
                Th = new Thread(Listen);
                Th.Start();
                Send(BC, "online", "");
                button1.Text = "離線";
            }
            else
            {
                Send(BC, "offline", "");
                Th.Abort();
                U.Close();
                button1.Text = "上線";
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            listBox1.ClearSelected();
        }

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //e.SuppressKeyPress = true;
                if (listBox1.SelectedIndex < 0)
                {
                    Send(BC, textBox2.Text, "");
                }
                else
                {
                    Send(ips[listBox1.SelectedIndex].ToString(), textBox2.Text, listBox1.SelectedItem.ToString());
                    listBox2.Items.Add($"{MyName}(私密)：{textBox2.Text}");
                }
                textBox2.Text = "";
            }
        }

        private void Listen()
        {
            U = new UdpClient(Port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(MyIP()), Port);
            while (true)
            {
                byte[] B = U.Receive(ref EP);
                string A = Encoding.Default.GetString(B);
                string[] C = A.Split(':');
                switch (C[2])
                {
                    case "online":
                        listBox1.Items.Add(C[0]);
                        ips.Add(C[1]);
                        if (C[0] != MyName) Send(C[1], "addMe", C[0]);
                        break;
                    case "addMe":
                        listBox1.Items.Add(C[0]);
                        ips.Add(C[1]);
                        break;
                    case "offline":
                        listBox1.Items.Remove(C[0]);
                        ips.Remove(C[1]);
                        break;
                    default:
                        if (C[3] == "")
                        {
                            listBox2.Items.Add($"{C[0]}(公開)：{C[2]}");
                        }
                        else
                        {
                            listBox2.Items.Add($"{C[0]}(私密)：{C[2]}");
                        }
                        break;
                }
            }
        }
    }
}

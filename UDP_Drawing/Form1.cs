﻿using System;
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
using Microsoft.VisualBasic.PowerPacks;

namespace UDP_Drawing
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
                string[] Z = A.Split('_');
                if (Z[0] == "Clear")
                {
                    isclear = true;
                }
                else
                {
                    string[] Q = Z[1].Split('/');
                    Point[] R = new Point[Q.Length];
                    for (int i = 0; i < Q.Length; i++)
                    {
                        string[] K = Q[i].Split(',');
                        R[i].X = int.Parse(K[0]);
                        R[i].Y = int.Parse(K[1]);
                    }
                    for (int i = 0; i < Q.Length - 1; i++)
                    {
                        LineShape L = new LineShape();
                        L.StartPoint = R[i];
                        L.EndPoint = R[i + 1];
                        switch (Z[0])
                        {
                            case "1":
                                L.BorderColor = Color.Red;
                                break;
                            case "2":
                                L.BorderColor = Color.Lime;
                                break;
                            case "3":
                                L.BorderColor = Color.Blue;
                                break;
                            case "4":
                                L.BorderColor = Color.Black;
                                break;
                        }
                        L.Parent = otherPerson;
                    }
                }
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

        ShapeContainer myself;
        ShapeContainer otherPerson;
        Point stP;
        string p;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += "" + MyIP();

            myself = new ShapeContainer();
            this.Controls.Add(myself);
            otherPerson = new ShapeContainer();
            this.Controls.Add(otherPerson);
            radioButton4.Checked = true;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stP = e.Location;
            p = $"{stP.X},{stP.Y}";
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isclear)
            {
                Clear();
                p = "";
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();
                L.StartPoint = stP;
                L.EndPoint = e.Location;
                if (radioButton1.Checked) { L.BorderColor = Color.Red; }
                if (radioButton2.Checked) { L.BorderColor = Color.Lime; }
                if (radioButton3.Checked) { L.BorderColor = Color.Blue; }
                if (radioButton4.Checked) { L.BorderColor = Color.Black; }
                L.Parent = myself;
                stP = e.Location;
                p += $"/{stP.X},{stP.Y}";
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int Port = int.Parse(textBox3.Text);
            UdpClient S = new UdpClient(textBox2.Text, Port);
            if (radioButton1.Checked) { p = "1_" + p; }
            if (radioButton2.Checked) { p = "2_" + p; }
            if (radioButton3.Checked) { p = "3_" + p; }
            if (radioButton4.Checked) { p = "4_" + p; }
            byte[] B = Encoding.Default.GetBytes(p);
            S.Send(B, B.Length);
            S.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Clear();
            int Port = int.Parse(textBox3.Text);
            UdpClient S = new UdpClient(textBox2.Text, Port);
            byte[] B = Encoding.Default.GetBytes("Clear");
            S.Send(B, B.Length);
            S.Close();
        }

        private void Clear()
        {
            myself.Shapes.Clear();
            otherPerson.Shapes.Clear();
            isclear = false;
        }
    }
}

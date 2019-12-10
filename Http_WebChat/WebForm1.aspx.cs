using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Http_WebChat
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Button1.Text == "上線")
            {
                if (TextBox2.Text != "")
                {
                    Session["Me"] = TextBox2.Text;
                    Application.Lock();
                    Hashtable L = (Hashtable)Application["L"];
                    L.Add(Session["Me"], DateTime.Now);
                    Application.UnLock();
                    Button1.Text = "登出";
                }
            }
            else
            {
                Application.Lock();
                Hashtable L = (Hashtable)Application["L"];
                L.Remove(Session["Me"]);
                Application.UnLock();
                Session["Me"] = null;
                Button1.Text = "上線";
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (!(Session["Me"] == null))
            {
                Application.Lock();
                Queue Q = (Queue)Application["Q"];
                Q.Enqueue(TextBox2.Text + ":" + TextBox3.Text);
                while (Q.Count > 5)
                {
                    Q.Dequeue();
                }
                Application.UnLock();
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Queue Q = (Queue)Application["Q"];
            TextBox1.Text = "";
            foreach(var i in Q)
            {
                TextBox1.Text += i + "\r\n";
            }

            Application.Lock();
            Hashtable L = (Hashtable)Application["L"];
            if (!(Session["Me"]== null))
            {
                if (L[Session["Me"]] == null)
                {
                    L.Add(Session["Me"], DateTime.Now);
                }
                else
                {
                    L[Session["Me"]] = DateTime.Now;
                }
            }
            Application.UnLock();
            ListBox1.Items.Clear();
            foreach(var i in L.Keys)
            {
                ListBox1.Items.Add(i.ToString());
            }
        }
    }
}
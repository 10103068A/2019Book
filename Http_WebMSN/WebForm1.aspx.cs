using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _11_WebMSN
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string A = TextBox2.Text + ":" + TextBox4.Text;
            TextBox1.Text += A + "\r\n";
            Application[TextBox3.Text] = A;
            TextBox4.Text = "";
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (Application[TextBox2.Text] != null)
            {
                TextBox1.Text += Application[TextBox2.Text] + "\r\n";
                Application[TextBox2.Text] = null;
            }
        }
    }
}
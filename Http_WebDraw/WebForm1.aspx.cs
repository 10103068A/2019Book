using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Http_WebDraw
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string Q = Request.QueryString["A"];
            if (Q != null && Session["to"] != null)
            {
                Application[Session["to"].ToString()] = Q;
            }
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Session["me"] = TextBox1.Text;
        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {
            Session["to"] = TextBox2.Text;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (Session["me"] == null) return;
            if (Application[Session["me"].ToString()] == null) return;
            H.Value = Application[Session["me"].ToString()].ToString();
            Application[Session["me"].ToString()] = null;
        }
    }
}
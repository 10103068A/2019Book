using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Http_Gobang
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*Bitmap bmp = new Bitmap(570, 570);
            Graphics g = Graphics.FromImage(bmp);
            for(int i = 0; i < 19; i++)
            {
                g.DrawLine(Pens.Black, i * 30 + 15, 15, i * 30 + 15, 30 * 18 + 15);
            }
            for(int j = 0; j < 19; j++)
            {
                g.DrawLine(Pens.Black, 15, j * 30 + 15, 30 * 18 + 15, j * 30 + 15);
            }
            bmp.Save(Server.MapPath("bg.gif"));*/
            string A = Request.QueryString["A"];
            if (A != null && Session["to"] != null)
            {
                Application[Session["to"].ToString()] = A;
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            H.Value = "X";
            if (Session["to"] != null)
            {
                Application[Session["to"].ToString()] = "X";
            }
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
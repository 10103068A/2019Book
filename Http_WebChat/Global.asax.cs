using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;

namespace Http_WebChat
{
    public class Global : System.Web.HttpApplication
    {
        System.Threading.Thread Th;
        protected void Application_Start(object sender, EventArgs e)
        {
            Hashtable L = new Hashtable();
            Application["L"] = L;
            Queue Q = new Queue();
            Application["Q"] = Q;

            Th = new System.Threading.Thread(chkList);
            Th.IsBackground = true;
            Th.Start();
        }

        private void chkList()
        {
            while (true)
            {
                Hashtable L = (Hashtable)Application["L"];
                if (L.Count > 0)
                {
                    Application.Lock();
                    Hashtable G = new Hashtable();
                    foreach(var h in L.Keys)
                    {
                        if (h != "")
                        {
                            DateTime t = (DateTime)L[h];
                            double s = new TimeSpan(DateTime.Now.Ticks - t.Ticks).TotalSeconds;
                            if ((int)s <= 2)
                            {
                                G.Add(h, t);
                            }
                        }
                    }
                    Application["L"] = G;
                    Application.UnLock();
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            Th.Abort();
        }
    }
}
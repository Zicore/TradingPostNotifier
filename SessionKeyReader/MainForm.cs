using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SessionKeyReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        KeyHelper key = new KeyHelper();

        private void btGetSession_Click(object sender, EventArgs e)
        {
            btGetSession.Enabled = false;
            
            key.UriFound += new EventHandler<UriFoundEventArgs>(key_UriFound);
            key.FindKey();
        }

        void key_UriFound(object sender, UriFoundEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                tbSessionKey.Text = e.Key;
                btGetSession.Enabled = true;
                btLogin.Enabled = true;
            });
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(String.Format("https://tradingpost-live.ncplatform.net/authenticate?session_key={0}&source=", key.Key));
            }
            catch
            {

            }
        }
        
    }
}

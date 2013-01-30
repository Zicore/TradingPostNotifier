namespace SessionKeyReader
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btGetSession = new System.Windows.Forms.Button();
            this.tbSessionKey = new System.Windows.Forms.TextBox();
            this.btLogin = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btGetSession
            // 
            this.btGetSession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btGetSession.Location = new System.Drawing.Point(12, 12);
            this.btGetSession.Name = "btGetSession";
            this.btGetSession.Size = new System.Drawing.Size(464, 23);
            this.btGetSession.TabIndex = 0;
            this.btGetSession.Text = "GW2 Session";
            this.btGetSession.UseVisualStyleBackColor = true;
            this.btGetSession.Click += new System.EventHandler(this.btGetSession_Click);
            // 
            // tbSessionKey
            // 
            this.tbSessionKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSessionKey.Location = new System.Drawing.Point(12, 41);
            this.tbSessionKey.Name = "tbSessionKey";
            this.tbSessionKey.Size = new System.Drawing.Size(464, 20);
            this.tbSessionKey.TabIndex = 1;
            // 
            // btLogin
            // 
            this.btLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btLogin.Enabled = false;
            this.btLogin.Location = new System.Drawing.Point(12, 67);
            this.btLogin.Name = "btLogin";
            this.btLogin.Size = new System.Drawing.Size(464, 23);
            this.btLogin.TabIndex = 2;
            this.btLogin.Text = "Browser - Login";
            this.btLogin.UseVisualStyleBackColor = true;
            this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 112);
            this.Controls.Add(this.btLogin);
            this.Controls.Add(this.tbSessionKey);
            this.Controls.Add(this.btGetSession);
            this.Name = "MainForm";
            this.Text = "GW2 Session";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGetSession;
        private System.Windows.Forms.TextBox tbSessionKey;
        private System.Windows.Forms.Button btLogin;
    }
}
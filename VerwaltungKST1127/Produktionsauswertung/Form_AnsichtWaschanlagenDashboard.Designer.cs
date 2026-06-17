namespace VerwaltungKST1127.Produktionsauswertung
{
    partial class Form_AnsichtWaschanlagenDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.DateTimePicker dtpDatum;
        private System.Windows.Forms.Button btnHeute;
        private System.Windows.Forms.Button btnNeuLaden;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Label lblAutoRefreshTimer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer timerAutoRefresh;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAutoRefreshTimer = new System.Windows.Forms.Label();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.btnNeuLaden = new System.Windows.Forms.Button();
            this.btnHeute = new System.Windows.Forms.Button();
            this.dtpDatum = new System.Windows.Forms.DateTimePicker();
            this.lblDatum = new System.Windows.Forms.Label();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.timerAutoRefresh = new System.Windows.Forms.Timer(this.components);
            this.panelToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            //
            // panelToolbar
            //
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(22, 29, 58);
            this.panelToolbar.Controls.Add(this.lblStatus);
            this.panelToolbar.Controls.Add(this.lblAutoRefreshTimer);
            this.panelToolbar.Controls.Add(this.chkAutoRefresh);
            this.panelToolbar.Controls.Add(this.btnNeuLaden);
            this.panelToolbar.Controls.Add(this.btnHeute);
            this.panelToolbar.Controls.Add(this.dtpDatum);
            this.panelToolbar.Controls.Add(this.lblDatum);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(1280, 48);
            this.panelToolbar.TabIndex = 0;
            //
            // lblDatum
            //
            this.lblDatum.AutoSize = true;
            this.lblDatum.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblDatum.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDatum.Location = new System.Drawing.Point(14, 14);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(50, 19);
            this.lblDatum.TabIndex = 0;
            this.lblDatum.Text = "Datum:";
            //
            // dtpDatum
            //
            this.dtpDatum.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpDatum.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDatum.CustomFormat = "ddd, dd.MM.yyyy";
            this.dtpDatum.Location = new System.Drawing.Point(70, 11);
            this.dtpDatum.Name = "dtpDatum";
            this.dtpDatum.Size = new System.Drawing.Size(150, 25);
            this.dtpDatum.TabIndex = 1;
            this.dtpDatum.ValueChanged += new System.EventHandler(this.dtpDatum_ValueChanged);
            //
            // btnHeute
            //
            this.btnHeute.BackColor = System.Drawing.Color.FromArgb(42, 51, 90);
            this.btnHeute.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnHeute.FlatAppearance.BorderSize = 1;
            this.btnHeute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeute.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnHeute.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.btnHeute.Location = new System.Drawing.Point(230, 10);
            this.btnHeute.Name = "btnHeute";
            this.btnHeute.Size = new System.Drawing.Size(70, 27);
            this.btnHeute.TabIndex = 2;
            this.btnHeute.Text = "Heute";
            this.btnHeute.UseVisualStyleBackColor = false;
            this.btnHeute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHeute.Click += new System.EventHandler(this.btnHeute_Click);
            //
            // btnNeuLaden
            //
            this.btnNeuLaden.BackColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnNeuLaden.FlatAppearance.BorderSize = 0;
            this.btnNeuLaden.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNeuLaden.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnNeuLaden.ForeColor = System.Drawing.Color.FromArgb(11, 16, 32);
            this.btnNeuLaden.Location = new System.Drawing.Point(312, 10);
            this.btnNeuLaden.Name = "btnNeuLaden";
            this.btnNeuLaden.Size = new System.Drawing.Size(100, 27);
            this.btnNeuLaden.TabIndex = 3;
            this.btnNeuLaden.Text = "Neu laden";
            this.btnNeuLaden.UseVisualStyleBackColor = false;
            this.btnNeuLaden.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNeuLaden.Click += new System.EventHandler(this.btnNeuLaden_Click);
            //
            // chkAutoRefresh
            //
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.chkAutoRefresh.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkAutoRefresh.Location = new System.Drawing.Point(428, 14);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(140, 21);
            this.chkAutoRefresh.TabIndex = 4;
            this.chkAutoRefresh.Text = "Auto-Aktualisierung";
            this.chkAutoRefresh.UseVisualStyleBackColor = false;
            this.chkAutoRefresh.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoRefresh.CheckedChanged += new System.EventHandler(this.chkAutoRefresh_CheckedChanged);
            //
            // lblAutoRefreshTimer
            //
            this.lblAutoRefreshTimer.AutoSize = true;
            this.lblAutoRefreshTimer.ForeColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.lblAutoRefreshTimer.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblAutoRefreshTimer.Location = new System.Drawing.Point(584, 15);
            this.lblAutoRefreshTimer.Name = "lblAutoRefreshTimer";
            this.lblAutoRefreshTimer.Size = new System.Drawing.Size(50, 19);
            this.lblAutoRefreshTimer.TabIndex = 5;
            this.lblAutoRefreshTimer.Text = "";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(660, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 17);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Bereit";
            //
            // webView
            //
            this.webView.AllowExternalDrop = false;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(17, 23, 49);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 48);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1280, 752);
            this.webView.TabIndex = 1;
            this.webView.ZoomFactor = 1D;
            //
            // timerAutoRefresh
            //
            this.timerAutoRefresh.Interval = 1000;
            this.timerAutoRefresh.Tick += new System.EventHandler(this.timerAutoRefresh_Tick);
            //
            // Form_AnsichtWaschanlagenDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(17, 23, 49);
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.panelToolbar);
            this.Name = "Form_AnsichtWaschanlagenDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Waschanlagen-Dashboard";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form_AnsichtWaschanlagenDashboard_Load);
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}

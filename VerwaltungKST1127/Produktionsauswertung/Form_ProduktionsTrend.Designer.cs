namespace VerwaltungKST1127.Produktionsauswertung
{
    partial class Form_ProduktionsTrend
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Label lblVon;
        private System.Windows.Forms.DateTimePicker dtpVon;
        private System.Windows.Forms.Label lblBis;
        private System.Windows.Forms.DateTimePicker dtpBis;
        private System.Windows.Forms.Button btnGestern;
        private System.Windows.Forms.Button btnHeute;
        private System.Windows.Forms.Button btnMinus2Wochen;
        private System.Windows.Forms.Button btnMinus30Tage;
        private System.Windows.Forms.Label lblAnlagen;
        private System.Windows.Forms.FlowLayoutPanel flowAnlagen;
        private System.Windows.Forms.CheckBox chkSamstag;
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
            this.btnMinus30Tage = new System.Windows.Forms.Button();
            this.btnMinus2Wochen = new System.Windows.Forms.Button();
            this.btnHeute = new System.Windows.Forms.Button();
            this.btnGestern = new System.Windows.Forms.Button();
            this.lblAnlagen = new System.Windows.Forms.Label();
            this.flowAnlagen = new System.Windows.Forms.FlowLayoutPanel();
            this.chkSamstag = new System.Windows.Forms.CheckBox();
            this.dtpBis = new System.Windows.Forms.DateTimePicker();
            this.lblBis = new System.Windows.Forms.Label();
            this.dtpVon = new System.Windows.Forms.DateTimePicker();
            this.lblVon = new System.Windows.Forms.Label();
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
            this.panelToolbar.Controls.Add(this.chkSamstag);
            this.panelToolbar.Controls.Add(this.flowAnlagen);
            this.panelToolbar.Controls.Add(this.lblAnlagen);
            this.panelToolbar.Controls.Add(this.btnNeuLaden);
            this.panelToolbar.Controls.Add(this.btnMinus30Tage);
            this.panelToolbar.Controls.Add(this.btnMinus2Wochen);
            this.panelToolbar.Controls.Add(this.btnHeute);
            this.panelToolbar.Controls.Add(this.btnGestern);
            this.panelToolbar.Controls.Add(this.dtpBis);
            this.panelToolbar.Controls.Add(this.lblBis);
            this.panelToolbar.Controls.Add(this.dtpVon);
            this.panelToolbar.Controls.Add(this.lblVon);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(1280, 84);
            this.panelToolbar.TabIndex = 0;
            //
            // lblVon
            //
            this.lblVon.AutoSize = true;
            this.lblVon.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblVon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblVon.Location = new System.Drawing.Point(14, 14);
            this.lblVon.Name = "lblVon";
            this.lblVon.Size = new System.Drawing.Size(38, 19);
            this.lblVon.TabIndex = 0;
            this.lblVon.Text = "Von:";
            //
            // dtpVon
            //
            this.dtpVon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpVon.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVon.CustomFormat = "ddd, dd.MM.yyyy";
            this.dtpVon.Location = new System.Drawing.Point(54, 11);
            this.dtpVon.Name = "dtpVon";
            this.dtpVon.Size = new System.Drawing.Size(150, 25);
            this.dtpVon.TabIndex = 1;
            this.dtpVon.ValueChanged += new System.EventHandler(this.dtpVon_ValueChanged);
            //
            // lblBis
            //
            this.lblBis.AutoSize = true;
            this.lblBis.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblBis.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBis.Location = new System.Drawing.Point(214, 14);
            this.lblBis.Name = "lblBis";
            this.lblBis.Size = new System.Drawing.Size(30, 19);
            this.lblBis.TabIndex = 2;
            this.lblBis.Text = "Bis:";
            //
            // dtpBis
            //
            this.dtpBis.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpBis.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBis.CustomFormat = "ddd, dd.MM.yyyy";
            this.dtpBis.Location = new System.Drawing.Point(246, 11);
            this.dtpBis.Name = "dtpBis";
            this.dtpBis.Size = new System.Drawing.Size(150, 25);
            this.dtpBis.TabIndex = 3;
            this.dtpBis.ValueChanged += new System.EventHandler(this.dtpBis_ValueChanged);
            //
            // btnGestern
            //
            this.btnGestern.BackColor = System.Drawing.Color.FromArgb(42, 51, 90);
            this.btnGestern.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnGestern.FlatAppearance.BorderSize = 1;
            this.btnGestern.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGestern.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnGestern.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.btnGestern.Location = new System.Drawing.Point(408, 10);
            this.btnGestern.Name = "btnGestern";
            this.btnGestern.Size = new System.Drawing.Size(74, 27);
            this.btnGestern.TabIndex = 4;
            this.btnGestern.Text = "Gestern";
            this.btnGestern.UseVisualStyleBackColor = false;
            this.btnGestern.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGestern.Click += new System.EventHandler(this.btnGestern_Click);
            //
            // btnHeute
            //
            this.btnHeute.BackColor = System.Drawing.Color.FromArgb(42, 51, 90);
            this.btnHeute.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnHeute.FlatAppearance.BorderSize = 1;
            this.btnHeute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeute.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnHeute.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.btnHeute.Location = new System.Drawing.Point(486, 10);
            this.btnHeute.Name = "btnHeute";
            this.btnHeute.Size = new System.Drawing.Size(70, 27);
            this.btnHeute.TabIndex = 5;
            this.btnHeute.Text = "Heute";
            this.btnHeute.UseVisualStyleBackColor = false;
            this.btnHeute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHeute.Click += new System.EventHandler(this.btnHeute_Click);
            //
            // btnMinus2Wochen
            //
            this.btnMinus2Wochen.BackColor = System.Drawing.Color.FromArgb(42, 51, 90);
            this.btnMinus2Wochen.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnMinus2Wochen.FlatAppearance.BorderSize = 1;
            this.btnMinus2Wochen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinus2Wochen.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnMinus2Wochen.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.btnMinus2Wochen.Location = new System.Drawing.Point(560, 10);
            this.btnMinus2Wochen.Name = "btnMinus2Wochen";
            this.btnMinus2Wochen.Size = new System.Drawing.Size(104, 27);
            this.btnMinus2Wochen.TabIndex = 6;
            this.btnMinus2Wochen.Text = "letzte 2 Wochen";
            this.btnMinus2Wochen.UseVisualStyleBackColor = false;
            this.btnMinus2Wochen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinus2Wochen.Click += new System.EventHandler(this.btnMinus2Wochen_Click);
            //
            // btnMinus30Tage
            //
            this.btnMinus30Tage.BackColor = System.Drawing.Color.FromArgb(42, 51, 90);
            this.btnMinus30Tage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnMinus30Tage.FlatAppearance.BorderSize = 1;
            this.btnMinus30Tage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinus30Tage.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnMinus30Tage.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.btnMinus30Tage.Location = new System.Drawing.Point(676, 10);
            this.btnMinus30Tage.Name = "btnMinus30Tage";
            this.btnMinus30Tage.Size = new System.Drawing.Size(104, 27);
            this.btnMinus30Tage.TabIndex = 7;
            this.btnMinus30Tage.Text = "letzte 30 Tage";
            this.btnMinus30Tage.UseVisualStyleBackColor = false;
            this.btnMinus30Tage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinus30Tage.Click += new System.EventHandler(this.btnMinus30Tage_Click);
            //
            // btnNeuLaden
            //
            this.btnNeuLaden.BackColor = System.Drawing.Color.FromArgb(122, 215, 255);
            this.btnNeuLaden.FlatAppearance.BorderSize = 0;
            this.btnNeuLaden.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNeuLaden.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnNeuLaden.ForeColor = System.Drawing.Color.FromArgb(11, 16, 32);
            this.btnNeuLaden.Location = new System.Drawing.Point(800, 10);
            this.btnNeuLaden.Name = "btnNeuLaden";
            this.btnNeuLaden.Size = new System.Drawing.Size(100, 27);
            this.btnNeuLaden.TabIndex = 8;
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
            this.chkAutoRefresh.Location = new System.Drawing.Point(916, 14);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(140, 21);
            this.chkAutoRefresh.TabIndex = 9;
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
            this.lblAutoRefreshTimer.Location = new System.Drawing.Point(1072, 15);
            this.lblAutoRefreshTimer.Name = "lblAutoRefreshTimer";
            this.lblAutoRefreshTimer.Size = new System.Drawing.Size(50, 19);
            this.lblAutoRefreshTimer.TabIndex = 10;
            this.lblAutoRefreshTimer.Text = "";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(1156, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 17);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Bereit";
            //
            // lblAnlagen
            //
            this.lblAnlagen.AutoSize = true;
            this.lblAnlagen.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblAnlagen.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAnlagen.Location = new System.Drawing.Point(14, 52);
            this.lblAnlagen.Name = "lblAnlagen";
            this.lblAnlagen.Size = new System.Drawing.Size(120, 19);
            this.lblAnlagen.TabIndex = 12;
            this.lblAnlagen.Text = "Anlagen einbeziehen:";
            //
            // flowAnlagen
            //
            this.flowAnlagen.BackColor = System.Drawing.Color.Transparent;
            this.flowAnlagen.Location = new System.Drawing.Point(168, 48);
            this.flowAnlagen.Name = "flowAnlagen";
            this.flowAnlagen.Size = new System.Drawing.Size(940, 30);
            this.flowAnlagen.TabIndex = 13;
            this.flowAnlagen.WrapContents = false;
            this.flowAnlagen.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            //
            // chkSamstag
            //
            this.chkSamstag.AutoSize = true;
            this.chkSamstag.ForeColor = System.Drawing.Color.FromArgb(230, 235, 250);
            this.chkSamstag.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.chkSamstag.Location = new System.Drawing.Point(1120, 52);
            this.chkSamstag.Name = "chkSamstag";
            this.chkSamstag.Size = new System.Drawing.Size(140, 21);
            this.chkSamstag.TabIndex = 14;
            this.chkSamstag.Text = "Samstag einbeziehen";
            this.chkSamstag.UseVisualStyleBackColor = false;
            this.chkSamstag.BackColor = System.Drawing.Color.Transparent;
            this.chkSamstag.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSamstag.CheckedChanged += new System.EventHandler(this.chkSamstag_CheckedChanged);
            //
            // timerAutoRefresh
            //
            this.timerAutoRefresh.Interval = 1000;
            this.timerAutoRefresh.Tick += new System.EventHandler(this.timerAutoRefresh_Tick);
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
            // Form_ProduktionsTrend
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(17, 23, 49);
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.panelToolbar);
            this.Name = "Form_ProduktionsTrend";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Produktions-Trend (Zeitraum)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form_ProduktionsTrend_Load);
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}

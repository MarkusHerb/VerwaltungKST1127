namespace VerwaltungKST1127.Produktionsauswertung
{
    partial class Form_ProduktionsuebersichtGestern
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Label lblTag;
        private System.Windows.Forms.DateTimePicker dateTimePickerTag;
        private System.Windows.Forms.Button btnNeuLaden;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Label lblAutoRefreshTimer;
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
            this.dateTimePickerTag = new System.Windows.Forms.DateTimePicker();
            this.lblTag = new System.Windows.Forms.Label();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.timerAutoRefresh = new System.Windows.Forms.Timer(this.components);
            this.panelToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            //
            // panelToolbar
            //
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(17, 23, 46);
            this.panelToolbar.Controls.Add(this.lblStatus);
            this.panelToolbar.Controls.Add(this.lblAutoRefreshTimer);
            this.panelToolbar.Controls.Add(this.chkAutoRefresh);
            this.panelToolbar.Controls.Add(this.btnNeuLaden);
            this.panelToolbar.Controls.Add(this.dateTimePickerTag);
            this.panelToolbar.Controls.Add(this.lblTag);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(1280, 48);
            this.panelToolbar.TabIndex = 0;
            //
            // lblTag
            //
            this.lblTag.AutoSize = true;
            this.lblTag.ForeColor = System.Drawing.Color.FromArgb(170, 177, 200);
            this.lblTag.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTag.Location = new System.Drawing.Point(16, 14);
            this.lblTag.Name = "lblTag";
            this.lblTag.Size = new System.Drawing.Size(38, 19);
            this.lblTag.TabIndex = 0;
            this.lblTag.Text = "Tag:";
            //
            // dateTimePickerTag
            //
            this.dateTimePickerTag.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dateTimePickerTag.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerTag.CustomFormat = "dddd, dd.MM.yyyy";
            this.dateTimePickerTag.Location = new System.Drawing.Point(60, 11);
            this.dateTimePickerTag.Name = "dateTimePickerTag";
            this.dateTimePickerTag.Size = new System.Drawing.Size(240, 25);
            this.dateTimePickerTag.TabIndex = 1;
            this.dateTimePickerTag.ValueChanged += new System.EventHandler(this.dateTimePickerTag_ValueChanged);
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
            this.btnNeuLaden.Size = new System.Drawing.Size(110, 27);
            this.btnNeuLaden.TabIndex = 2;
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
            this.chkAutoRefresh.Location = new System.Drawing.Point(440, 14);
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
            this.lblAutoRefreshTimer.Location = new System.Drawing.Point(596, 15);
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
            this.lblStatus.Location = new System.Drawing.Point(680, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 17);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Bereit";
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
            this.webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(11, 16, 32);
            this.webView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView.Location = new System.Drawing.Point(0, 48);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(1280, 752);
            this.webView.TabIndex = 1;
            this.webView.ZoomFactor = 1D;
            //
            // Form_ProduktionsuebersichtGestern
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(11, 16, 32);
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.panelToolbar);
            this.Name = "Form_ProduktionsuebersichtGestern";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Produktion gestern";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form_ProduktionsuebersichtGestern_Load);
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
        }
    }
}

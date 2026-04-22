namespace VerwaltungKST1127.RFID
{
    partial class Form_RFID_Test
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
            this.lblTestRfid = new System.Windows.Forms.Label();
            this.btnTestRfid = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTestRfid
            // 
            this.lblTestRfid.AutoSize = true;
            this.lblTestRfid.Location = new System.Drawing.Point(183, 167);
            this.lblTestRfid.Name = "lblTestRfid";
            this.lblTestRfid.Size = new System.Drawing.Size(35, 13);
            this.lblTestRfid.TabIndex = 0;
            this.lblTestRfid.Text = "label1";
            // 
            // btnTestRfid
            // 
            this.btnTestRfid.Location = new System.Drawing.Point(91, 72);
            this.btnTestRfid.Name = "btnTestRfid";
            this.btnTestRfid.Size = new System.Drawing.Size(75, 23);
            this.btnTestRfid.TabIndex = 1;
            this.btnTestRfid.Text = "rfid";
            this.btnTestRfid.UseVisualStyleBackColor = true;
            this.btnTestRfid.Click += new System.EventHandler(this.btnTestRfid_Click);
            // 
            // Form_RFID_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTestRfid);
            this.Controls.Add(this.lblTestRfid);
            this.Name = "Form_RFID_Test";
            this.Text = "Form_RFID_Test";
            this.Load += new System.EventHandler(this.Form_RFID_Test_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTestRfid;
        private System.Windows.Forms.Button btnTestRfid;
    }
}
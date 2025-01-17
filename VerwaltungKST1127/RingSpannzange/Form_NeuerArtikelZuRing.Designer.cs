namespace VerwaltungKST1127.RingSpannzange
{
    partial class Form_NeuerArtikelZuRing
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBoxArtnr = new System.Windows.Forms.TextBox();
            this.txtBoxSeite = new System.Windows.Forms.TextBox();
            this.txtBoxBelag = new System.Windows.Forms.TextBox();
            this.txtBoxBBM = new System.Windows.Forms.TextBox();
            this.txtBoxBemerkung = new System.Windows.Forms.TextBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ring_ID";
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(112, 13);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(38, 13);
            this.lblID.TabIndex = 1;
            this.lblID.Text = "RingId";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "ArtikelNr.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Seite";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Belag";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "BBM";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Bemerkung";
            // 
            // txtBoxArtnr
            // 
            this.txtBoxArtnr.Location = new System.Drawing.Point(115, 28);
            this.txtBoxArtnr.Name = "txtBoxArtnr";
            this.txtBoxArtnr.Size = new System.Drawing.Size(100, 20);
            this.txtBoxArtnr.TabIndex = 7;
            // 
            // txtBoxSeite
            // 
            this.txtBoxSeite.Location = new System.Drawing.Point(115, 52);
            this.txtBoxSeite.Name = "txtBoxSeite";
            this.txtBoxSeite.Size = new System.Drawing.Size(100, 20);
            this.txtBoxSeite.TabIndex = 8;
            // 
            // txtBoxBelag
            // 
            this.txtBoxBelag.Location = new System.Drawing.Point(115, 75);
            this.txtBoxBelag.Name = "txtBoxBelag";
            this.txtBoxBelag.Size = new System.Drawing.Size(100, 20);
            this.txtBoxBelag.TabIndex = 9;
            // 
            // txtBoxBBM
            // 
            this.txtBoxBBM.Location = new System.Drawing.Point(115, 98);
            this.txtBoxBBM.Name = "txtBoxBBM";
            this.txtBoxBBM.Size = new System.Drawing.Size(100, 20);
            this.txtBoxBBM.TabIndex = 10;
            // 
            // txtBoxBemerkung
            // 
            this.txtBoxBemerkung.Location = new System.Drawing.Point(115, 121);
            this.txtBoxBemerkung.Name = "txtBoxBemerkung";
            this.txtBoxBemerkung.Size = new System.Drawing.Size(223, 20);
            this.txtBoxBemerkung.TabIndex = 11;
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(262, 13);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 12;
            this.BtnSave.Text = "Speichern";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // Form_NeuerArtikelZuRing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(350, 152);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.txtBoxBemerkung);
            this.Controls.Add(this.txtBoxBBM);
            this.Controls.Add(this.txtBoxBelag);
            this.Controls.Add(this.txtBoxSeite);
            this.Controls.Add(this.txtBoxArtnr);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblID);
            this.Controls.Add(this.label1);
            this.Name = "Form_NeuerArtikelZuRing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Neuen Artikel zuordnen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBoxArtnr;
        private System.Windows.Forms.TextBox txtBoxSeite;
        private System.Windows.Forms.TextBox txtBoxBelag;
        private System.Windows.Forms.TextBox txtBoxBBM;
        private System.Windows.Forms.TextBox txtBoxBemerkung;
        private System.Windows.Forms.Button BtnSave;
    }
}
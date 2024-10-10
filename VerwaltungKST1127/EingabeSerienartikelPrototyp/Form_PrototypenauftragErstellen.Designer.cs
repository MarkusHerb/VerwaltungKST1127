namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    partial class Form_PrototypenauftragErstellen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_PrototypenauftragErstellen));
            this.PrintDocument = new System.Drawing.Printing.PrintDocument();
            this.PrintDialog = new System.Windows.Forms.PrintDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.LblErstelltAm = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnDrucken = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.ComboboxArtikel = new System.Windows.Forms.ComboBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.PictureboxAuflegenLinsenPrismen = new System.Windows.Forms.PictureBox();
            this.PictureboxZusatzinfo = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxAuflegenLinsenPrismen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxZusatzinfo)).BeginInit();
            this.SuspendLayout();
            // 
            // PrintDocument
            // 
            this.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.PrintDocument_PrintPage);
            // 
            // PrintDialog
            // 
            this.PrintDialog.UseEXDialog = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 687);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "Dokument: LF-00263";
            // 
            // LblErstelltAm
            // 
            this.LblErstelltAm.AutoSize = true;
            this.LblErstelltAm.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblErstelltAm.Location = new System.Drawing.Point(20, 717);
            this.LblErstelltAm.Name = "LblErstelltAm";
            this.LblErstelltAm.Size = new System.Drawing.Size(127, 17);
            this.LblErstelltAm.TabIndex = 14;
            this.LblErstelltAm.Text = "Auftrag erstellt am:";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel5.Location = new System.Drawing.Point(13, 742);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1082, 1);
            this.panel5.TabIndex = 13;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel4.Location = new System.Drawing.Point(13, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1082, 1);
            this.panel4.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel3.Location = new System.Drawing.Point(12, 707);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1082, 1);
            this.panel3.TabIndex = 11;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 731);
            this.panel2.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(1094, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 731);
            this.panel1.TabIndex = 9;
            // 
            // BtnDrucken
            // 
            this.BtnDrucken.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BtnDrucken.Location = new System.Drawing.Point(869, 713);
            this.BtnDrucken.Name = "BtnDrucken";
            this.BtnDrucken.Size = new System.Drawing.Size(190, 23);
            this.BtnDrucken.TabIndex = 17;
            this.BtnDrucken.Text = "Auftrag Drucken";
            this.BtnDrucken.UseVisualStyleBackColor = false;
            this.BtnDrucken.Click += new System.EventHandler(this.BtnDrucken_Click_1);
            // 
            // BtnClose
            // 
            this.BtnClose.BackColor = System.Drawing.Color.Red;
            this.BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClose.ForeColor = System.Drawing.Color.White;
            this.BtnClose.Location = new System.Drawing.Point(1065, 713);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(23, 23);
            this.BtnClose.TabIndex = 18;
            this.BtnClose.Text = "X";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // ComboboxArtikel
            // 
            this.ComboboxArtikel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboboxArtikel.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxArtikel.FormattingEnabled = true;
            this.ComboboxArtikel.Location = new System.Drawing.Point(23, 20);
            this.ComboboxArtikel.Name = "ComboboxArtikel";
            this.ComboboxArtikel.Size = new System.Drawing.Size(364, 46);
            this.ComboboxArtikel.TabIndex = 19;
            this.ComboboxArtikel.SelectedIndexChanged += new System.EventHandler(this.ComboboxArtikel_SelectedIndexChanged);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel6.Location = new System.Drawing.Point(635, 12);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1, 695);
            this.panel6.TabIndex = 20;
            // 
            // PictureboxAuflegenLinsenPrismen
            // 
            this.PictureboxAuflegenLinsenPrismen.BackColor = System.Drawing.Color.White;
            this.PictureboxAuflegenLinsenPrismen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureboxAuflegenLinsenPrismen.Image = ((System.Drawing.Image)(resources.GetObject("PictureboxAuflegenLinsenPrismen.Image")));
            this.PictureboxAuflegenLinsenPrismen.Location = new System.Drawing.Point(642, 52);
            this.PictureboxAuflegenLinsenPrismen.Name = "PictureboxAuflegenLinsenPrismen";
            this.PictureboxAuflegenLinsenPrismen.Size = new System.Drawing.Size(446, 287);
            this.PictureboxAuflegenLinsenPrismen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureboxAuflegenLinsenPrismen.TabIndex = 66;
            this.PictureboxAuflegenLinsenPrismen.TabStop = false;
            // 
            // PictureboxZusatzinfo
            // 
            this.PictureboxZusatzinfo.BackColor = System.Drawing.Color.White;
            this.PictureboxZusatzinfo.Image = ((System.Drawing.Image)(resources.GetObject("PictureboxZusatzinfo.Image")));
            this.PictureboxZusatzinfo.Location = new System.Drawing.Point(642, 414);
            this.PictureboxZusatzinfo.Name = "PictureboxZusatzinfo";
            this.PictureboxZusatzinfo.Size = new System.Drawing.Size(446, 287);
            this.PictureboxZusatzinfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureboxZusatzinfo.TabIndex = 67;
            this.PictureboxZusatzinfo.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(642, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(233, 29);
            this.label2.TabIndex = 68;
            this.label2.Text = "Information Auflegen";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(642, 382);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(201, 29);
            this.label3.TabIndex = 69;
            this.label3.Text = "Zusatzinformation";
            // 
            // Form_PrototypenauftragErstellen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1107, 755);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PictureboxZusatzinfo);
            this.Controls.Add(this.PictureboxAuflegenLinsenPrismen);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.ComboboxArtikel);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnDrucken);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblErstelltAm);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form_PrototypenauftragErstellen";
            this.Text = "Prototypen Auftrag";
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxAuflegenLinsenPrismen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxZusatzinfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Drawing.Printing.PrintDocument PrintDocument;
        private System.Windows.Forms.PrintDialog PrintDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LblErstelltAm;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnDrucken;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.ComboBox ComboboxArtikel;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.PictureBox PictureboxAuflegenLinsenPrismen;
        private System.Windows.Forms.PictureBox PictureboxZusatzinfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
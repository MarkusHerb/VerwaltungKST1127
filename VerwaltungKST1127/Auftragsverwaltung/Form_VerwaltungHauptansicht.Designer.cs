namespace VerwaltungKST1127.Auftragsverwaltung
{
    partial class Form_VerwaltungHauptansicht
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
            this.lblGestartet = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPfad = new System.Windows.Forms.Label();
            this.BtnZukauf = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PictureBoxZeichnung = new System.Windows.Forms.PictureBox();
            this.DgvInformationZuAuftrag = new System.Windows.Forms.DataGridView();
            this.DgvAnsichtAuftraege = new System.Windows.Forms.DataGridView();
            this.DgvLadeBelaege = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxZeichnung)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvInformationZuAuftrag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvAnsichtAuftraege)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLadeBelaege)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGestartet
            // 
            this.lblGestartet.AutoSize = true;
            this.lblGestartet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGestartet.Location = new System.Drawing.Point(1066, 9);
            this.lblGestartet.Name = "lblGestartet";
            this.lblGestartet.Size = new System.Drawing.Size(18, 20);
            this.lblGestartet.TabIndex = 24;
            this.lblGestartet.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(937, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 20);
            this.label5.TabIndex = 23;
            this.label5.Text = "Gestartet AVOs:";
            // 
            // lblPfad
            // 
            this.lblPfad.AutoSize = true;
            this.lblPfad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPfad.Location = new System.Drawing.Point(1508, 831);
            this.lblPfad.Name = "lblPfad";
            this.lblPfad.Size = new System.Drawing.Size(58, 9);
            this.lblPfad.TabIndex = 22;
            this.lblPfad.Text = "Zeichnungspfad";
            // 
            // BtnZukauf
            // 
            this.BtnZukauf.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BtnZukauf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnZukauf.Location = new System.Drawing.Point(11, 828);
            this.BtnZukauf.Name = "BtnZukauf";
            this.BtnZukauf.Size = new System.Drawing.Size(277, 44);
            this.BtnZukauf.TabIndex = 21;
            this.BtnZukauf.Text = "Zukauf hinzufügen / löschen";
            this.BtnZukauf.UseVisualStyleBackColor = false;
            this.BtnZukauf.Click += new System.EventHandler(this.BtnZukauf_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1504, 580);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 20);
            this.label4.TabIndex = 20;
            this.label4.Text = "Ausgewählter Artikel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(290, 580);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(284, 20);
            this.label3.TabIndex = 19;
            this.label3.Text = "Information zum ausgewählten Auftrag";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(290, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 20);
            this.label2.TabIndex = 18;
            this.label2.Text = "Aufträge nach Belag";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Filterung";
            // 
            // PictureBoxZeichnung
            // 
            this.PictureBoxZeichnung.Location = new System.Drawing.Point(1508, 608);
            this.PictureBoxZeichnung.Name = "PictureBoxZeichnung";
            this.PictureBoxZeichnung.Size = new System.Drawing.Size(317, 214);
            this.PictureBoxZeichnung.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxZeichnung.TabIndex = 16;
            this.PictureBoxZeichnung.TabStop = false;
            // 
            // DgvInformationZuAuftrag
            // 
            this.DgvInformationZuAuftrag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvInformationZuAuftrag.Location = new System.Drawing.Point(294, 608);
            this.DgvInformationZuAuftrag.Name = "DgvInformationZuAuftrag";
            this.DgvInformationZuAuftrag.RowHeadersVisible = false;
            this.DgvInformationZuAuftrag.Size = new System.Drawing.Size(1208, 264);
            this.DgvInformationZuAuftrag.TabIndex = 15;
            // 
            // DgvAnsichtAuftraege
            // 
            this.DgvAnsichtAuftraege.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvAnsichtAuftraege.Location = new System.Drawing.Point(294, 37);
            this.DgvAnsichtAuftraege.Name = "DgvAnsichtAuftraege";
            this.DgvAnsichtAuftraege.RowHeadersVisible = false;
            this.DgvAnsichtAuftraege.Size = new System.Drawing.Size(1531, 532);
            this.DgvAnsichtAuftraege.TabIndex = 14;
            this.DgvAnsichtAuftraege.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAnsichtAuftraege_CellClick);
            this.DgvAnsichtAuftraege.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAnsichtAuftraege_CellDoubleClick);
            this.DgvAnsichtAuftraege.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvAnsichtAuftraege_CellFormatting);
            this.DgvAnsichtAuftraege.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgvAnsichtAuftraege_MouseDown);
            // 
            // DgvLadeBelaege
            // 
            this.DgvLadeBelaege.AllowUserToAddRows = false;
            this.DgvLadeBelaege.AllowUserToDeleteRows = false;
            this.DgvLadeBelaege.AllowUserToResizeColumns = false;
            this.DgvLadeBelaege.AllowUserToResizeRows = false;
            this.DgvLadeBelaege.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvLadeBelaege.Location = new System.Drawing.Point(12, 37);
            this.DgvLadeBelaege.Name = "DgvLadeBelaege";
            this.DgvLadeBelaege.RowHeadersVisible = false;
            this.DgvLadeBelaege.Size = new System.Drawing.Size(276, 785);
            this.DgvLadeBelaege.TabIndex = 13;
            this.DgvLadeBelaege.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvLadeBelaege_CellClick);
            // 
            // Form_VerwaltungHauptansicht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1863, 889);
            this.Controls.Add(this.lblGestartet);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblPfad);
            this.Controls.Add(this.BtnZukauf);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PictureBoxZeichnung);
            this.Controls.Add(this.DgvInformationZuAuftrag);
            this.Controls.Add(this.DgvAnsichtAuftraege);
            this.Controls.Add(this.DgvLadeBelaege);
            this.Name = "Form_VerwaltungHauptansicht";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auftragsverwaltung Kst1127";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxZeichnung)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvInformationZuAuftrag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvAnsichtAuftraege)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLadeBelaege)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblGestartet;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPfad;
        private System.Windows.Forms.Button BtnZukauf;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox PictureBoxZeichnung;
        private System.Windows.Forms.DataGridView DgvInformationZuAuftrag;
        private System.Windows.Forms.DataGridView DgvAnsichtAuftraege;
        private System.Windows.Forms.DataGridView DgvLadeBelaege;

    }
}
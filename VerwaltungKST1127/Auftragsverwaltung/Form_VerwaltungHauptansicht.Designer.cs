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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.checkBoxShowZukauf = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblGestarteAuftraege = new System.Windows.Forms.Label();
            this.btnShowStkOffen = new System.Windows.Forms.Button();
            this.btnRueckstand = new System.Windows.Forms.Button();
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
            this.lblGestartet.Location = new System.Drawing.Point(1304, 11);
            this.lblGestartet.Name = "lblGestartet";
            this.lblGestartet.Size = new System.Drawing.Size(18, 20);
            this.lblGestartet.TabIndex = 24;
            this.lblGestartet.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1207, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 20);
            this.label5.TabIndex = 23;
            this.label5.Text = "Aktive AVOs:";
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
            this.BtnZukauf.Location = new System.Drawing.Point(11, 755);
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
            this.label1.Size = new System.Drawing.Size(63, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Suche: ";
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvInformationZuAuftrag.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvInformationZuAuftrag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvInformationZuAuftrag.Location = new System.Drawing.Point(294, 608);
            this.DgvInformationZuAuftrag.Name = "DgvInformationZuAuftrag";
            this.DgvInformationZuAuftrag.ReadOnly = true;
            this.DgvInformationZuAuftrag.RowHeadersVisible = false;
            this.DgvInformationZuAuftrag.Size = new System.Drawing.Size(1208, 285);
            this.DgvInformationZuAuftrag.TabIndex = 15;
            // 
            // DgvAnsichtAuftraege
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvAnsichtAuftraege.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DgvAnsichtAuftraege.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvAnsichtAuftraege.DefaultCellStyle = dataGridViewCellStyle3;
            this.DgvAnsichtAuftraege.Location = new System.Drawing.Point(294, 37);
            this.DgvAnsichtAuftraege.Name = "DgvAnsichtAuftraege";
            this.DgvAnsichtAuftraege.ReadOnly = true;
            this.DgvAnsichtAuftraege.RowHeadersVisible = false;
            this.DgvAnsichtAuftraege.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
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
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvLadeBelaege.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.DgvLadeBelaege.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvLadeBelaege.Location = new System.Drawing.Point(12, 37);
            this.DgvLadeBelaege.Name = "DgvLadeBelaege";
            this.DgvLadeBelaege.ReadOnly = true;
            this.DgvLadeBelaege.RowHeadersVisible = false;
            this.DgvLadeBelaege.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvLadeBelaege.Size = new System.Drawing.Size(276, 712);
            this.DgvLadeBelaege.TabIndex = 13;
            this.DgvLadeBelaege.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvLadeBelaege_CellClick);
            // 
            // checkBoxShowZukauf
            // 
            this.checkBoxShowZukauf.AutoSize = true;
            this.checkBoxShowZukauf.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxShowZukauf.Location = new System.Drawing.Point(1677, 9);
            this.checkBoxShowZukauf.Name = "checkBoxShowZukauf";
            this.checkBoxShowZukauf.Size = new System.Drawing.Size(149, 24);
            this.checkBoxShowZukauf.TabIndex = 25;
            this.checkBoxShowZukauf.Text = "Zeige nur Zukauf";
            this.checkBoxShowZukauf.UseVisualStyleBackColor = true;
            this.checkBoxShowZukauf.CheckedChanged += new System.EventHandler(this.checkBoxShowZukauf_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(1403, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(213, 20);
            this.label6.TabIndex = 27;
            this.label6.Text = "Gestartete Aufträge gesamt:";
            // 
            // lblGestarteAuftraege
            // 
            this.lblGestarteAuftraege.AutoSize = true;
            this.lblGestarteAuftraege.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGestarteAuftraege.Location = new System.Drawing.Point(1613, 9);
            this.lblGestarteAuftraege.Name = "lblGestarteAuftraege";
            this.lblGestarteAuftraege.Size = new System.Drawing.Size(18, 20);
            this.lblGestarteAuftraege.TabIndex = 28;
            this.lblGestarteAuftraege.Text = "0";
            // 
            // btnShowStkOffen
            // 
            this.btnShowStkOffen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnShowStkOffen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowStkOffen.Location = new System.Drawing.Point(12, 805);
            this.btnShowStkOffen.Name = "btnShowStkOffen";
            this.btnShowStkOffen.Size = new System.Drawing.Size(276, 41);
            this.btnShowStkOffen.TabIndex = 29;
            this.btnShowStkOffen.Text = "Unvergütete Stück";
            this.btnShowStkOffen.UseVisualStyleBackColor = false;
            this.btnShowStkOffen.Click += new System.EventHandler(this.btnShowStkOffen_Click);
            // 
            // btnRueckstand
            // 
            this.btnRueckstand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnRueckstand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRueckstand.Location = new System.Drawing.Point(12, 852);
            this.btnRueckstand.Name = "btnRueckstand";
            this.btnRueckstand.Size = new System.Drawing.Size(276, 41);
            this.btnRueckstand.TabIndex = 30;
            this.btnRueckstand.Text = "Rückstand Vorbereiten/Vergüten";
            this.btnRueckstand.UseVisualStyleBackColor = false;
            this.btnRueckstand.Click += new System.EventHandler(this.btnRueckstand_Click);
            // 
            // Form_VerwaltungHauptansicht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1838, 906);
            this.Controls.Add(this.btnRueckstand);
            this.Controls.Add(this.btnShowStkOffen);
            this.Controls.Add(this.lblGestarteAuftraege);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxShowZukauf);
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
        private System.Windows.Forms.CheckBox checkBoxShowZukauf;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblGestarteAuftraege;
        private System.Windows.Forms.Button btnShowStkOffen;
        private System.Windows.Forms.Button btnRueckstand;
    }
}
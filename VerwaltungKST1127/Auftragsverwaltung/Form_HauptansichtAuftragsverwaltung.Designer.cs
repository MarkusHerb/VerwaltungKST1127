namespace VerwaltungKST1127.Auftragsverwaltung
{
    partial class Form_HauptansichtAuftragsverwaltung
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
            this.dGvAuftragZuBelag = new System.Windows.Forms.DataGridView();
            this.dGvAnsichtAuswahlAuftrag = new System.Windows.Forms.DataGridView();
            this.dGvInfoZuAuswahlAuftrag = new System.Windows.Forms.DataGridView();
            this.pictureBoxAnsichtArtikel = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpenTLTLForm = new System.Windows.Forms.Button();
            this.lblPfad = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dGvAuftragZuBelag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGvAnsichtAuswahlAuftrag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGvInfoZuAuswahlAuftrag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnsichtArtikel)).BeginInit();
            this.SuspendLayout();
            // 
            // dGvAuftragZuBelag
            // 
            this.dGvAuftragZuBelag.AllowUserToAddRows = false;
            this.dGvAuftragZuBelag.AllowUserToDeleteRows = false;
            this.dGvAuftragZuBelag.AllowUserToResizeColumns = false;
            this.dGvAuftragZuBelag.AllowUserToResizeRows = false;
            this.dGvAuftragZuBelag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGvAuftragZuBelag.Location = new System.Drawing.Point(13, 36);
            this.dGvAuftragZuBelag.Name = "dGvAuftragZuBelag";
            this.dGvAuftragZuBelag.RowHeadersVisible = false;
            this.dGvAuftragZuBelag.Size = new System.Drawing.Size(276, 785);
            this.dGvAuftragZuBelag.TabIndex = 0;
            this.dGvAuftragZuBelag.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGvAuftragZuBelag_CellClick);
            // 
            // dGvAnsichtAuswahlAuftrag
            // 
            this.dGvAnsichtAuswahlAuftrag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGvAnsichtAuswahlAuftrag.Location = new System.Drawing.Point(295, 36);
            this.dGvAnsichtAuswahlAuftrag.Name = "dGvAnsichtAuswahlAuftrag";
            this.dGvAnsichtAuswahlAuftrag.RowHeadersVisible = false;
            this.dGvAnsichtAuswahlAuftrag.Size = new System.Drawing.Size(1531, 532);
            this.dGvAnsichtAuswahlAuftrag.TabIndex = 1;
            this.dGvAnsichtAuswahlAuftrag.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGvAnsichtAuswahlAuftrag_CellClick);
            this.dGvAnsichtAuswahlAuftrag.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dGvAnsichtAuswahlAuftrag_CellFormatting);
            this.dGvAnsichtAuswahlAuftrag.Click += new System.EventHandler(this.dGvAnsichtAuswahlAuftrag_Click);
            // 
            // dGvInfoZuAuswahlAuftrag
            // 
            this.dGvInfoZuAuswahlAuftrag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGvInfoZuAuswahlAuftrag.Location = new System.Drawing.Point(295, 607);
            this.dGvInfoZuAuswahlAuftrag.Name = "dGvInfoZuAuswahlAuftrag";
            this.dGvInfoZuAuswahlAuftrag.RowHeadersVisible = false;
            this.dGvInfoZuAuswahlAuftrag.Size = new System.Drawing.Size(1208, 214);
            this.dGvInfoZuAuswahlAuftrag.TabIndex = 2;
            // 
            // pictureBoxAnsichtArtikel
            // 
            this.pictureBoxAnsichtArtikel.Location = new System.Drawing.Point(1509, 607);
            this.pictureBoxAnsichtArtikel.Name = "pictureBoxAnsichtArtikel";
            this.pictureBoxAnsichtArtikel.Size = new System.Drawing.Size(317, 214);
            this.pictureBoxAnsichtArtikel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAnsichtArtikel.TabIndex = 3;
            this.pictureBoxAnsichtArtikel.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filterung";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(291, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Aufträge nach Belag";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(291, 579);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(284, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Information zum ausgewählten Auftrag";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1505, 579);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Ausgewählter Artikel";
            // 
            // btnOpenTLTLForm
            // 
            this.btnOpenTLTLForm.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnOpenTLTLForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenTLTLForm.Location = new System.Drawing.Point(12, 827);
            this.btnOpenTLTLForm.Name = "btnOpenTLTLForm";
            this.btnOpenTLTLForm.Size = new System.Drawing.Size(277, 44);
            this.btnOpenTLTLForm.TabIndex = 8;
            this.btnOpenTLTLForm.Text = "Zukauf hinzufügen / löschen";
            this.btnOpenTLTLForm.UseVisualStyleBackColor = false;
            this.btnOpenTLTLForm.Click += new System.EventHandler(this.btnOpenTLTLForm_Click);
            // 
            // lblPfad
            // 
            this.lblPfad.AutoSize = true;
            this.lblPfad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPfad.Location = new System.Drawing.Point(1509, 830);
            this.lblPfad.Name = "lblPfad";
            this.lblPfad.Size = new System.Drawing.Size(58, 9);
            this.lblPfad.TabIndex = 9;
            this.lblPfad.Text = "Zeichnungspfad";
            // 
            // Form_HauptansichtAuftragsverwaltung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1838, 883);
            this.Controls.Add(this.lblPfad);
            this.Controls.Add(this.btnOpenTLTLForm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxAnsichtArtikel);
            this.Controls.Add(this.dGvInfoZuAuswahlAuftrag);
            this.Controls.Add(this.dGvAnsichtAuswahlAuftrag);
            this.Controls.Add(this.dGvAuftragZuBelag);
            this.Name = "Form_HauptansichtAuftragsverwaltung";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auftragsverwaltung";
            ((System.ComponentModel.ISupportInitialize)(this.dGvAuftragZuBelag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGvAnsichtAuswahlAuftrag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGvInfoZuAuswahlAuftrag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnsichtArtikel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dGvAuftragZuBelag;
        private System.Windows.Forms.DataGridView dGvAnsichtAuswahlAuftrag;
        private System.Windows.Forms.DataGridView dGvInfoZuAuswahlAuftrag;
        private System.Windows.Forms.PictureBox pictureBoxAnsichtArtikel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOpenTLTLForm;
        private System.Windows.Forms.Label lblPfad;
    }
}
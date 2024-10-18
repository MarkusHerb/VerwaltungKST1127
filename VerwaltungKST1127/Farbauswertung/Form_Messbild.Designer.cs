namespace VerwaltungKST1127.Farbauswertung
{
    partial class Form_Messbild
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
            this.BtnDrucken = new System.Windows.Forms.Button();
            this.LblAnlage = new System.Windows.Forms.Label();
            this.LblBelagProcess = new System.Windows.Forms.Label();
            this.Lbl_Z_A = new System.Windows.Forms.Label();
            this.Lbl_Y_A = new System.Windows.Forms.Label();
            this.Lbl_X_A = new System.Windows.Forms.Label();
            this.Lbl_Z_I = new System.Windows.Forms.Label();
            this.Lbl_Y_I = new System.Windows.Forms.Label();
            this.Lbl_X_I = new System.Windows.Forms.Label();
            this.LblAnzeigeMessbild = new System.Windows.Forms.Label();
            this.pictureBoxMessung = new System.Windows.Forms.PictureBox();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMessung)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnDrucken
            // 
            this.BtnDrucken.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.BtnDrucken.Location = new System.Drawing.Point(1198, 478);
            this.BtnDrucken.Name = "BtnDrucken";
            this.BtnDrucken.Size = new System.Drawing.Size(220, 62);
            this.BtnDrucken.TabIndex = 21;
            this.BtnDrucken.Text = "Messkurve Drucken";
            this.BtnDrucken.UseVisualStyleBackColor = false;
            this.BtnDrucken.Click += new System.EventHandler(this.BtnDrucken_Click);
            // 
            // LblAnlage
            // 
            this.LblAnlage.AutoSize = true;
            this.LblAnlage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnlage.Location = new System.Drawing.Point(1300, 21);
            this.LblAnlage.Name = "LblAnlage";
            this.LblAnlage.Size = new System.Drawing.Size(76, 24);
            this.LblAnlage.TabIndex = 20;
            this.LblAnlage.Text = "Anlage";
            // 
            // LblBelagProcess
            // 
            this.LblBelagProcess.AutoSize = true;
            this.LblBelagProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblBelagProcess.Location = new System.Drawing.Point(1149, 21);
            this.LblBelagProcess.Name = "LblBelagProcess";
            this.LblBelagProcess.Size = new System.Drawing.Size(145, 24);
            this.LblBelagProcess.TabIndex = 19;
            this.LblBelagProcess.Text = "Belag-Process";
            // 
            // Lbl_Z_A
            // 
            this.Lbl_Z_A.AutoSize = true;
            this.Lbl_Z_A.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Z_A.Location = new System.Drawing.Point(355, 495);
            this.Lbl_Z_A.Name = "Lbl_Z_A";
            this.Lbl_Z_A.Size = new System.Drawing.Size(45, 24);
            this.Lbl_Z_A.TabIndex = 18;
            this.Lbl_Z_A.Text = "Z_A";
            // 
            // Lbl_Y_A
            // 
            this.Lbl_Y_A.AutoSize = true;
            this.Lbl_Y_A.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Y_A.Location = new System.Drawing.Point(183, 495);
            this.Lbl_Y_A.Name = "Lbl_Y_A";
            this.Lbl_Y_A.Size = new System.Drawing.Size(45, 24);
            this.Lbl_Y_A.TabIndex = 17;
            this.Lbl_Y_A.Text = "Y_A";
            // 
            // Lbl_X_A
            // 
            this.Lbl_X_A.AutoSize = true;
            this.Lbl_X_A.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_X_A.Location = new System.Drawing.Point(11, 495);
            this.Lbl_X_A.Name = "Lbl_X_A";
            this.Lbl_X_A.Size = new System.Drawing.Size(47, 24);
            this.Lbl_X_A.TabIndex = 16;
            this.Lbl_X_A.Text = "X_A";
            // 
            // Lbl_Z_I
            // 
            this.Lbl_Z_I.AutoSize = true;
            this.Lbl_Z_I.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Z_I.Location = new System.Drawing.Point(355, 471);
            this.Lbl_Z_I.Name = "Lbl_Z_I";
            this.Lbl_Z_I.Size = new System.Drawing.Size(36, 24);
            this.Lbl_Z_I.TabIndex = 15;
            this.Lbl_Z_I.Text = "Z_I";
            // 
            // Lbl_Y_I
            // 
            this.Lbl_Y_I.AutoSize = true;
            this.Lbl_Y_I.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_Y_I.Location = new System.Drawing.Point(183, 471);
            this.Lbl_Y_I.Name = "Lbl_Y_I";
            this.Lbl_Y_I.Size = new System.Drawing.Size(36, 24);
            this.Lbl_Y_I.TabIndex = 14;
            this.Lbl_Y_I.Text = "Y_I";
            // 
            // Lbl_X_I
            // 
            this.Lbl_X_I.AutoSize = true;
            this.Lbl_X_I.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lbl_X_I.Location = new System.Drawing.Point(11, 471);
            this.Lbl_X_I.Name = "Lbl_X_I";
            this.Lbl_X_I.Size = new System.Drawing.Size(38, 24);
            this.Lbl_X_I.TabIndex = 13;
            this.Lbl_X_I.Text = "X_I";
            // 
            // LblAnzeigeMessbild
            // 
            this.LblAnzeigeMessbild.AutoSize = true;
            this.LblAnzeigeMessbild.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnzeigeMessbild.Location = new System.Drawing.Point(15, 21);
            this.LblAnzeigeMessbild.Name = "LblAnzeigeMessbild";
            this.LblAnzeigeMessbild.Size = new System.Drawing.Size(166, 24);
            this.LblAnzeigeMessbild.TabIndex = 12;
            this.LblAnzeigeMessbild.Text = "Messbezeichnung";
            // 
            // pictureBoxMessung
            // 
            this.pictureBoxMessung.Location = new System.Drawing.Point(15, 65);
            this.pictureBoxMessung.Name = "pictureBoxMessung";
            this.pictureBoxMessung.Size = new System.Drawing.Size(1403, 403);
            this.pictureBoxMessung.TabIndex = 11;
            this.pictureBoxMessung.TabStop = false;
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
            // 
            // Form_Messbild
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1428, 561);
            this.Controls.Add(this.BtnDrucken);
            this.Controls.Add(this.LblAnlage);
            this.Controls.Add(this.LblBelagProcess);
            this.Controls.Add(this.Lbl_Z_A);
            this.Controls.Add(this.Lbl_Y_A);
            this.Controls.Add(this.Lbl_X_A);
            this.Controls.Add(this.Lbl_Z_I);
            this.Controls.Add(this.Lbl_Y_I);
            this.Controls.Add(this.Lbl_X_I);
            this.Controls.Add(this.LblAnzeigeMessbild);
            this.Controls.Add(this.pictureBoxMessung);
            this.MaximumSize = new System.Drawing.Size(1444, 600);
            this.MinimumSize = new System.Drawing.Size(1444, 600);
            this.Name = "Form_Messbild";
            this.Text = "Form_Messbild";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMessung)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnDrucken;
        private System.Windows.Forms.Label LblAnlage;
        private System.Windows.Forms.Label LblBelagProcess;
        private System.Windows.Forms.Label Lbl_Z_A;
        private System.Windows.Forms.Label Lbl_Y_A;
        private System.Windows.Forms.Label Lbl_X_A;
        private System.Windows.Forms.Label Lbl_Z_I;
        private System.Windows.Forms.Label Lbl_Y_I;
        private System.Windows.Forms.Label Lbl_X_I;
        private System.Windows.Forms.Label LblAnzeigeMessbild;
        private System.Windows.Forms.PictureBox pictureBoxMessung;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}
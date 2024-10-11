namespace VerwaltungKST1127
{
    partial class Form_Start
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Start));
            this.pictureBoxLinsenPrismen = new System.Windows.Forms.PictureBox();
            this.TimerDatumUhrzeit = new System.Windows.Forms.Timer(this.components);
            this.LblUeberschrift = new System.Windows.Forms.Label();
            this.LblUhrzeitDatum = new System.Windows.Forms.Label();
            this.BtnSerienartikelPrototyp = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BtnPrototypenAuftragErstellen = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLinsenPrismen)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLinsenPrismen
            // 
            this.pictureBoxLinsenPrismen.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLinsenPrismen.Image")));
            this.pictureBoxLinsenPrismen.Location = new System.Drawing.Point(906, 12);
            this.pictureBoxLinsenPrismen.Name = "pictureBoxLinsenPrismen";
            this.pictureBoxLinsenPrismen.Size = new System.Drawing.Size(150, 146);
            this.pictureBoxLinsenPrismen.TabIndex = 2;
            this.pictureBoxLinsenPrismen.TabStop = false;
            // 
            // TimerDatumUhrzeit
            // 
            this.TimerDatumUhrzeit.Interval = 1000;
            this.TimerDatumUhrzeit.Tick += new System.EventHandler(this.TimerDatumUhrzeit_Tick);
            // 
            // LblUeberschrift
            // 
            this.LblUeberschrift.AutoSize = true;
            this.LblUeberschrift.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblUeberschrift.Location = new System.Drawing.Point(12, 28);
            this.LblUeberschrift.Name = "LblUeberschrift";
            this.LblUeberschrift.Size = new System.Drawing.Size(784, 46);
            this.LblUeberschrift.TabIndex = 3;
            this.LblUeberschrift.Text = "Verwaltung Kst1127 US-Reinigen/Vergüten";
            // 
            // LblUhrzeitDatum
            // 
            this.LblUhrzeitDatum.AutoSize = true;
            this.LblUhrzeitDatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblUhrzeitDatum.Location = new System.Drawing.Point(16, 84);
            this.LblUhrzeitDatum.Name = "LblUhrzeitDatum";
            this.LblUhrzeitDatum.Size = new System.Drawing.Size(211, 31);
            this.LblUhrzeitDatum.TabIndex = 4;
            this.LblUhrzeitDatum.Text = "Datum + Uhrzeit";
            // 
            // BtnSerienartikelPrototyp
            // 
            this.BtnSerienartikelPrototyp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnSerienartikelPrototyp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSerienartikelPrototyp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSerienartikelPrototyp.Location = new System.Drawing.Point(661, 164);
            this.BtnSerienartikelPrototyp.Name = "BtnSerienartikelPrototyp";
            this.BtnSerienartikelPrototyp.Size = new System.Drawing.Size(239, 66);
            this.BtnSerienartikelPrototyp.TabIndex = 5;
            this.BtnSerienartikelPrototyp.Text = "Neuen Artikel anlegen/ändern";
            this.BtnSerienartikelPrototyp.UseVisualStyleBackColor = false;
            this.BtnSerienartikelPrototyp.Click += new System.EventHandler(this.BtnSerienartikelPrototyp_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.panel1.Location = new System.Drawing.Point(19, 148);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(887, 10);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.panel2.Location = new System.Drawing.Point(906, 157);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(10, 482);
            this.panel2.TabIndex = 7;
            // 
            // BtnPrototypenAuftragErstellen
            // 
            this.BtnPrototypenAuftragErstellen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnPrototypenAuftragErstellen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPrototypenAuftragErstellen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnPrototypenAuftragErstellen.Location = new System.Drawing.Point(661, 236);
            this.BtnPrototypenAuftragErstellen.Name = "BtnPrototypenAuftragErstellen";
            this.BtnPrototypenAuftragErstellen.Size = new System.Drawing.Size(239, 66);
            this.BtnPrototypenAuftragErstellen.TabIndex = 8;
            this.BtnPrototypenAuftragErstellen.Text = "Prototypenauftrag erstellen";
            this.BtnPrototypenAuftragErstellen.UseVisualStyleBackColor = false;
            this.BtnPrototypenAuftragErstellen.Click += new System.EventHandler(this.BtnPrototypenAuftragErstellen_Click);
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1068, 651);
            this.Controls.Add(this.BtnPrototypenAuftragErstellen);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BtnSerienartikelPrototyp);
            this.Controls.Add(this.LblUhrzeitDatum);
            this.Controls.Add(this.LblUeberschrift);
            this.Controls.Add(this.pictureBoxLinsenPrismen);
            this.MaximumSize = new System.Drawing.Size(1084, 690);
            this.MinimumSize = new System.Drawing.Size(1084, 690);
            this.Name = "Form_Start";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hauptmenü";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLinsenPrismen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLinsenPrismen;
        private System.Windows.Forms.Timer TimerDatumUhrzeit;
        private System.Windows.Forms.Label LblUeberschrift;
        private System.Windows.Forms.Label LblUhrzeitDatum;
        private System.Windows.Forms.Button BtnSerienartikelPrototyp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button BtnPrototypenAuftragErstellen;
    }
}


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
            this.LblInfo1 = new System.Windows.Forms.Label();
            this.BtnFarbwerte = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.LblQualitaet = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.Organisation = new System.Windows.Forms.Label();
            this.BtnMateriallager = new System.Windows.Forms.Button();
            this.BtnInformation = new System.Windows.Forms.Button();
            this.BtnLupe = new System.Windows.Forms.Button();
            this.BtnHomepage = new System.Windows.Forms.Button();
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
            this.BtnSerienartikelPrototyp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BtnSerienartikelPrototyp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSerienartikelPrototyp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSerienartikelPrototyp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSerienartikelPrototyp.Location = new System.Drawing.Point(13, 211);
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
            this.BtnPrototypenAuftragErstellen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BtnPrototypenAuftragErstellen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPrototypenAuftragErstellen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnPrototypenAuftragErstellen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnPrototypenAuftragErstellen.Location = new System.Drawing.Point(13, 283);
            this.BtnPrototypenAuftragErstellen.Name = "BtnPrototypenAuftragErstellen";
            this.BtnPrototypenAuftragErstellen.Size = new System.Drawing.Size(239, 66);
            this.BtnPrototypenAuftragErstellen.TabIndex = 8;
            this.BtnPrototypenAuftragErstellen.Text = "Prototypenauftrag erstellen";
            this.BtnPrototypenAuftragErstellen.UseVisualStyleBackColor = false;
            this.BtnPrototypenAuftragErstellen.Click += new System.EventHandler(this.BtnPrototypenAuftragErstellen_Click);
            // 
            // LblInfo1
            // 
            this.LblInfo1.AutoSize = true;
            this.LblInfo1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblInfo1.Location = new System.Drawing.Point(73, 166);
            this.LblInfo1.Name = "LblInfo1";
            this.LblInfo1.Size = new System.Drawing.Size(126, 26);
            this.LblInfo1.TabIndex = 9;
            this.LblInfo1.Text = "Produktion";
            // 
            // BtnFarbwerte
            // 
            this.BtnFarbwerte.BackColor = System.Drawing.Color.LightSteelBlue;
            this.BtnFarbwerte.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnFarbwerte.BackgroundImage")));
            this.BtnFarbwerte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnFarbwerte.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnFarbwerte.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnFarbwerte.Location = new System.Drawing.Point(257, 211);
            this.BtnFarbwerte.Name = "BtnFarbwerte";
            this.BtnFarbwerte.Size = new System.Drawing.Size(239, 66);
            this.BtnFarbwerte.TabIndex = 10;
            this.BtnFarbwerte.Text = "Farbauswertung";
            this.BtnFarbwerte.UseVisualStyleBackColor = false;
            this.BtnFarbwerte.Click += new System.EventHandler(this.BtnFarbwerte_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.panel3.Location = new System.Drawing.Point(254, 157);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1, 483);
            this.panel3.TabIndex = 11;
            // 
            // LblQualitaet
            // 
            this.LblQualitaet.AutoSize = true;
            this.LblQualitaet.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblQualitaet.Location = new System.Drawing.Point(329, 166);
            this.LblQualitaet.Name = "LblQualitaet";
            this.LblQualitaet.Size = new System.Drawing.Size(95, 26);
            this.LblQualitaet.TabIndex = 12;
            this.LblQualitaet.Text = "Qualität";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.panel4.Location = new System.Drawing.Point(498, 157);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1, 483);
            this.panel4.TabIndex = 13;
            // 
            // Organisation
            // 
            this.Organisation.AutoSize = true;
            this.Organisation.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Organisation.Location = new System.Drawing.Point(552, 166);
            this.Organisation.Name = "Organisation";
            this.Organisation.Size = new System.Drawing.Size(112, 26);
            this.Organisation.TabIndex = 15;
            this.Organisation.Text = "Abteilung";
            // 
            // BtnMateriallager
            // 
            this.BtnMateriallager.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.BtnMateriallager.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMateriallager.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnMateriallager.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnMateriallager.Location = new System.Drawing.Point(501, 211);
            this.BtnMateriallager.Name = "BtnMateriallager";
            this.BtnMateriallager.Size = new System.Drawing.Size(239, 66);
            this.BtnMateriallager.TabIndex = 14;
            this.BtnMateriallager.Text = "Materiallager";
            this.BtnMateriallager.UseVisualStyleBackColor = false;
            this.BtnMateriallager.Click += new System.EventHandler(this.BtnMateriallager_Click);
            // 
            // BtnInformation
            // 
            this.BtnInformation.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.BtnInformation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnInformation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnInformation.Location = new System.Drawing.Point(922, 573);
            this.BtnInformation.Name = "BtnInformation";
            this.BtnInformation.Size = new System.Drawing.Size(134, 66);
            this.BtnInformation.TabIndex = 16;
            this.BtnInformation.Text = "Information";
            this.BtnInformation.UseVisualStyleBackColor = false;
            this.BtnInformation.Click += new System.EventHandler(this.BtnInformation_Click);
            // 
            // BtnLupe
            // 
            this.BtnLupe.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.BtnLupe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnLupe.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnLupe.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnLupe.Location = new System.Drawing.Point(922, 501);
            this.BtnLupe.Name = "BtnLupe";
            this.BtnLupe.Size = new System.Drawing.Size(134, 66);
            this.BtnLupe.TabIndex = 17;
            this.BtnLupe.Text = "OptikLupe";
            this.BtnLupe.UseVisualStyleBackColor = false;
            this.BtnLupe.Click += new System.EventHandler(this.BtnLupe_Click);
            // 
            // BtnHomepage
            // 
            this.BtnHomepage.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.BtnHomepage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnHomepage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnHomepage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnHomepage.Location = new System.Drawing.Point(922, 429);
            this.BtnHomepage.Name = "BtnHomepage";
            this.BtnHomepage.Size = new System.Drawing.Size(134, 66);
            this.BtnHomepage.TabIndex = 18;
            this.BtnHomepage.Text = "Homepage";
            this.BtnHomepage.UseVisualStyleBackColor = false;
            this.BtnHomepage.Click += new System.EventHandler(this.BtnHomepage_Click);
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1068, 651);
            this.Controls.Add(this.BtnHomepage);
            this.Controls.Add(this.BtnLupe);
            this.Controls.Add(this.BtnInformation);
            this.Controls.Add(this.Organisation);
            this.Controls.Add(this.BtnMateriallager);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.LblQualitaet);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.BtnFarbwerte);
            this.Controls.Add(this.LblInfo1);
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
        private System.Windows.Forms.Label LblInfo1;
        private System.Windows.Forms.Button BtnFarbwerte;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label LblQualitaet;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label Organisation;
        private System.Windows.Forms.Button BtnMateriallager;
        private System.Windows.Forms.Button BtnInformation;
        private System.Windows.Forms.Button BtnLupe;
        private System.Windows.Forms.Button BtnHomepage;
    }
}


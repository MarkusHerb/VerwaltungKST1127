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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            this.ListBoxDocuments = new System.Windows.Forms.ListBox();
            this.BtnPersonalliste = new System.Windows.Forms.Button();
            this.PictureBoxBestellung = new System.Windows.Forms.PictureBox();
            this.BtnAuftragsverwaltung = new System.Windows.Forms.Button();
            this.process1 = new System.Diagnostics.Process();
            this.TimerCpu = new System.Windows.Forms.Timer(this.components);
            this.TimerRam = new System.Windows.Forms.Timer(this.components);
            this.LblCpu = new System.Windows.Forms.Label();
            this.LblRam = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pictureboxBild = new System.Windows.Forms.PictureBox();
            this.TimerBild = new System.Windows.Forms.Timer(this.components);
            this.chartPerformance = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLinsenPrismen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxBestellung)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureboxBild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPerformance)).BeginInit();
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
            this.LblUeberschrift.Font = new System.Drawing.Font("Rockwell", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblUeberschrift.ForeColor = System.Drawing.Color.Black;
            this.LblUeberschrift.Location = new System.Drawing.Point(12, 28);
            this.LblUeberschrift.Name = "LblUeberschrift";
            this.LblUeberschrift.Size = new System.Drawing.Size(817, 46);
            this.LblUeberschrift.TabIndex = 3;
            this.LblUeberschrift.Text = "Verwaltung Kst1127 US-Reinigen/Vergüten";
            // 
            // LblUhrzeitDatum
            // 
            this.LblUhrzeitDatum.AutoSize = true;
            this.LblUhrzeitDatum.Font = new System.Drawing.Font("Rockwell", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblUhrzeitDatum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.LblUhrzeitDatum.Location = new System.Drawing.Point(16, 84);
            this.LblUhrzeitDatum.Name = "LblUhrzeitDatum";
            this.LblUhrzeitDatum.Size = new System.Drawing.Size(213, 31);
            this.LblUhrzeitDatum.TabIndex = 4;
            this.LblUhrzeitDatum.Text = "Datum + Uhrzeit";
            // 
            // BtnSerienartikelPrototyp
            // 
            this.BtnSerienartikelPrototyp.BackColor = System.Drawing.Color.Silver;
            this.BtnSerienartikelPrototyp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSerienartikelPrototyp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSerienartikelPrototyp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSerienartikelPrototyp.Location = new System.Drawing.Point(44, 283);
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
            this.BtnPrototypenAuftragErstellen.BackColor = System.Drawing.Color.Silver;
            this.BtnPrototypenAuftragErstellen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPrototypenAuftragErstellen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnPrototypenAuftragErstellen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnPrototypenAuftragErstellen.Location = new System.Drawing.Point(44, 573);
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
            this.LblInfo1.Location = new System.Drawing.Point(103, 166);
            this.LblInfo1.Name = "LblInfo1";
            this.LblInfo1.Size = new System.Drawing.Size(126, 26);
            this.LblInfo1.TabIndex = 9;
            this.LblInfo1.Text = "Produktion";
            // 
            // BtnFarbwerte
            // 
            this.BtnFarbwerte.BackColor = System.Drawing.Color.Silver;
            this.BtnFarbwerte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnFarbwerte.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnFarbwerte.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnFarbwerte.Location = new System.Drawing.Point(332, 211);
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
            this.panel3.Location = new System.Drawing.Point(307, 157);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1, 483);
            this.panel3.TabIndex = 11;
            // 
            // LblQualitaet
            // 
            this.LblQualitaet.AutoSize = true;
            this.LblQualitaet.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblQualitaet.Location = new System.Drawing.Point(401, 166);
            this.LblQualitaet.Name = "LblQualitaet";
            this.LblQualitaet.Size = new System.Drawing.Size(95, 26);
            this.LblQualitaet.TabIndex = 12;
            this.LblQualitaet.Text = "Qualität";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.panel4.Location = new System.Drawing.Point(594, 157);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1, 483);
            this.panel4.TabIndex = 13;
            // 
            // Organisation
            // 
            this.Organisation.AutoSize = true;
            this.Organisation.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Organisation.Location = new System.Drawing.Point(682, 166);
            this.Organisation.Name = "Organisation";
            this.Organisation.Size = new System.Drawing.Size(112, 26);
            this.Organisation.TabIndex = 15;
            this.Organisation.Text = "Abteilung";
            // 
            // BtnMateriallager
            // 
            this.BtnMateriallager.BackColor = System.Drawing.Color.Silver;
            this.BtnMateriallager.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMateriallager.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnMateriallager.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnMateriallager.Location = new System.Drawing.Point(619, 211);
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
            // ListBoxDocuments
            // 
            this.ListBoxDocuments.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ListBoxDocuments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxDocuments.ForeColor = System.Drawing.Color.Black;
            this.ListBoxDocuments.FormattingEnabled = true;
            this.ListBoxDocuments.Location = new System.Drawing.Point(922, 166);
            this.ListBoxDocuments.Name = "ListBoxDocuments";
            this.ListBoxDocuments.Size = new System.Drawing.Size(134, 251);
            this.ListBoxDocuments.TabIndex = 19;
            this.ListBoxDocuments.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ListBoxDocuments_DrawItem);
            this.ListBoxDocuments.DoubleClick += new System.EventHandler(this.ListBoxDocuments_DoubleClick);
            this.ListBoxDocuments.MouseLeave += new System.EventHandler(this.ListBoxDocuments_MouseLeave);
            this.ListBoxDocuments.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListBoxDocuments_MouseMove);
            // 
            // BtnPersonalliste
            // 
            this.BtnPersonalliste.BackColor = System.Drawing.Color.Silver;
            this.BtnPersonalliste.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPersonalliste.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnPersonalliste.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnPersonalliste.Location = new System.Drawing.Point(619, 283);
            this.BtnPersonalliste.Name = "BtnPersonalliste";
            this.BtnPersonalliste.Size = new System.Drawing.Size(239, 66);
            this.BtnPersonalliste.TabIndex = 20;
            this.BtnPersonalliste.Text = "Personalliste";
            this.BtnPersonalliste.UseVisualStyleBackColor = false;
            this.BtnPersonalliste.Click += new System.EventHandler(this.BtnPersonalliste_Click);
            // 
            // PictureBoxBestellung
            // 
            this.PictureBoxBestellung.BackColor = System.Drawing.Color.Silver;
            this.PictureBoxBestellung.Location = new System.Drawing.Point(809, 225);
            this.PictureBoxBestellung.Name = "PictureBoxBestellung";
            this.PictureBoxBestellung.Size = new System.Drawing.Size(29, 40);
            this.PictureBoxBestellung.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxBestellung.TabIndex = 21;
            this.PictureBoxBestellung.TabStop = false;
            // 
            // BtnAuftragsverwaltung
            // 
            this.BtnAuftragsverwaltung.BackColor = System.Drawing.Color.Silver;
            this.BtnAuftragsverwaltung.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnAuftragsverwaltung.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnAuftragsverwaltung.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAuftragsverwaltung.Location = new System.Drawing.Point(44, 211);
            this.BtnAuftragsverwaltung.Name = "BtnAuftragsverwaltung";
            this.BtnAuftragsverwaltung.Size = new System.Drawing.Size(239, 66);
            this.BtnAuftragsverwaltung.TabIndex = 22;
            this.BtnAuftragsverwaltung.Text = "Auftragsverwaltung";
            this.BtnAuftragsverwaltung.UseVisualStyleBackColor = false;
            this.BtnAuftragsverwaltung.Click += new System.EventHandler(this.BtnAuftragsverwaltung_Click);
            // 
            // process1
            // 
            this.process1.StartInfo.Domain = "";
            this.process1.StartInfo.LoadUserProfile = false;
            this.process1.StartInfo.Password = null;
            this.process1.StartInfo.StandardErrorEncoding = null;
            this.process1.StartInfo.StandardOutputEncoding = null;
            this.process1.StartInfo.UserName = "";
            this.process1.SynchronizingObject = this;
            // 
            // TimerCpu
            // 
            this.TimerCpu.Interval = 1000;
            this.TimerCpu.Tick += new System.EventHandler(this.TimerCpu_Tick);
            // 
            // TimerRam
            // 
            this.TimerRam.Interval = 1000;
            this.TimerRam.Tick += new System.EventHandler(this.TimerRam_Tick);
            // 
            // LblCpu
            // 
            this.LblCpu.AutoSize = true;
            this.LblCpu.Location = new System.Drawing.Point(22, 123);
            this.LblCpu.Name = "LblCpu";
            this.LblCpu.Size = new System.Drawing.Size(26, 13);
            this.LblCpu.TabIndex = 23;
            this.LblCpu.Text = "Cpu";
            // 
            // LblRam
            // 
            this.LblRam.AutoSize = true;
            this.LblRam.Location = new System.Drawing.Point(166, 122);
            this.LblRam.Name = "LblRam";
            this.LblRam.Size = new System.Drawing.Size(31, 13);
            this.LblRam.TabIndex = 24;
            this.LblRam.Text = "RAM";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 651);
            this.splitter1.TabIndex = 25;
            this.splitter1.TabStop = false;
            // 
            // pictureboxBild
            // 
            this.pictureboxBild.Image = ((System.Drawing.Image)(resources.GetObject("pictureboxBild.Image")));
            this.pictureboxBild.Location = new System.Drawing.Point(332, 313);
            this.pictureboxBild.Name = "pictureboxBild";
            this.pictureboxBild.Size = new System.Drawing.Size(41, 36);
            this.pictureboxBild.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureboxBild.TabIndex = 26;
            this.pictureboxBild.TabStop = false;
            // 
            // TimerBild
            // 
            this.TimerBild.Tick += new System.EventHandler(this.TimerBild_Tick);
            // 
            // chartPerformance
            // 
            this.chartPerformance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.chartPerformance.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            chartArea1.Name = "ChartArea1";
            this.chartPerformance.ChartAreas.Add(chartArea1);
            legend1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            legend1.Name = "Legend1";
            this.chartPerformance.Legends.Add(legend1);
            this.chartPerformance.Location = new System.Drawing.Point(454, 77);
            this.chartPerformance.Name = "chartPerformance";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartPerformance.Series.Add(series1);
            this.chartPerformance.Size = new System.Drawing.Size(366, 65);
            this.chartPerformance.TabIndex = 27;
            this.chartPerformance.Text = "chart1";
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1068, 651);
            this.Controls.Add(this.chartPerformance);
            this.Controls.Add(this.pictureboxBild);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.LblRam);
            this.Controls.Add(this.LblCpu);
            this.Controls.Add(this.BtnAuftragsverwaltung);
            this.Controls.Add(this.PictureBoxBestellung);
            this.Controls.Add(this.BtnPersonalliste);
            this.Controls.Add(this.ListBoxDocuments);
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
            this.Load += new System.EventHandler(this.Form_Start_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLinsenPrismen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxBestellung)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureboxBild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPerformance)).EndInit();
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
        private System.Windows.Forms.ListBox ListBoxDocuments;
        private System.Windows.Forms.Button BtnPersonalliste;
        private System.Windows.Forms.PictureBox PictureBoxBestellung;
        private System.Windows.Forms.Button BtnAuftragsverwaltung;
        private System.Diagnostics.Process process1;
        private System.Windows.Forms.Timer TimerCpu;
        private System.Windows.Forms.Timer TimerRam;
        private System.Windows.Forms.Label LblRam;
        private System.Windows.Forms.Label LblCpu;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.PictureBox pictureboxBild;
        private System.Windows.Forms.Timer TimerBild;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPerformance;
    }
}


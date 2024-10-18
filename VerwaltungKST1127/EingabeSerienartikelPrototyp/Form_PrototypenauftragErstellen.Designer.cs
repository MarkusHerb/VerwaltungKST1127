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
            this.lblDokument = new System.Windows.Forms.Label();
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
            this.lblInfoAuflegen = new System.Windows.Forms.Label();
            this.lblZusatzinfo = new System.Windows.Forms.Label();
            this.lblNrProjekt = new System.Windows.Forms.Label();
            this.lblMenge = new System.Windows.Forms.Label();
            this.txtboxAuftragsnummer = new System.Windows.Forms.TextBox();
            this.TxtboxMenge = new System.Windows.Forms.TextBox();
            this.lblArtikelnummer = new System.Windows.Forms.Label();
            this.lblBelag = new System.Windows.Forms.Label();
            this.txtboxBelag = new System.Windows.Forms.TextBox();
            this.txtboxProzess = new System.Windows.Forms.TextBox();
            this.lblProzess = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lblDatenArtikel = new System.Windows.Forms.Label();
            this.txtboxDurchmesser = new System.Windows.Forms.TextBox();
            this.lblDurchmesser = new System.Windows.Forms.Label();
            this.txtboxBrechwert = new System.Windows.Forms.TextBox();
            this.lblBrechwert = new System.Windows.Forms.Label();
            this.txtboxRadiusRueckseite = new System.Windows.Forms.TextBox();
            this.lblRadiusRueckseite = new System.Windows.Forms.Label();
            this.txtboxRadiusVerguetung = new System.Windows.Forms.TextBox();
            this.lblRadiusverguetung = new System.Windows.Forms.Label();
            this.txtboxGlassorte = new System.Windows.Forms.TextBox();
            this.lblGlassorte = new System.Windows.Forms.Label();
            this.txtboxGnummer = new System.Windows.Forms.TextBox();
            this.lblGnummer = new System.Windows.Forms.Label();
            this.txtboxBezeichnung = new System.Windows.Forms.TextBox();
            this.lblBezeichnung = new System.Windows.Forms.Label();
            this.richtxtboxInforamationAuflegen = new System.Windows.Forms.RichTextBox();
            this.richtxtboxZusatzinfo = new System.Windows.Forms.RichTextBox();
            this.txtboxVorreinigung = new System.Windows.Forms.TextBox();
            this.lblVorreinigung = new System.Windows.Forms.Label();
            this.txtboxHandreinigung = new System.Windows.Forms.TextBox();
            this.lblHandreinigung = new System.Windows.Forms.Label();
            this.lblVorbehandlung = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.lblWerAufgelegt = new System.Windows.Forms.Label();
            this.lblZuzerstAuswaehlen = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.panel18 = new System.Windows.Forms.Panel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.panel20 = new System.Windows.Forms.Panel();
            this.panel21 = new System.Windows.Forms.Panel();
            this.lblDatum = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStueck = new System.Windows.Forms.Label();
            this.lblStueckGes = new System.Windows.Forms.Label();
            this.lblZeit = new System.Windows.Forms.Label();
            this.panel22 = new System.Windows.Forms.Panel();
            this.lblDicke = new System.Windows.Forms.Label();
            this.txtboxDicke = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxAuflegenLinsenPrismen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxZusatzinfo)).BeginInit();
            this.panel19.SuspendLayout();
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
            // lblDokument
            // 
            this.lblDokument.AutoSize = true;
            this.lblDokument.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDokument.Location = new System.Drawing.Point(713, 717);
            this.lblDokument.Name = "lblDokument";
            this.lblDokument.Size = new System.Drawing.Size(135, 17);
            this.lblDokument.TabIndex = 16;
            this.lblDokument.Text = "Dokument: LF-00xxx";
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
            this.panel5.Size = new System.Drawing.Size(1082, 2);
            this.panel5.TabIndex = 13;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel4.Location = new System.Drawing.Point(13, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1082, 2);
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
            this.panel2.Size = new System.Drawing.Size(2, 731);
            this.panel2.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(1094, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 731);
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
            this.ComboboxArtikel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboboxArtikel.FormattingEnabled = true;
            this.ComboboxArtikel.Location = new System.Drawing.Point(162, 67);
            this.ComboboxArtikel.Name = "ComboboxArtikel";
            this.ComboboxArtikel.Size = new System.Drawing.Size(308, 33);
            this.ComboboxArtikel.TabIndex = 19;
            this.ComboboxArtikel.SelectedIndexChanged += new System.EventHandler(this.ComboboxArtikel_SelectedIndexChanged);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel6.Location = new System.Drawing.Point(691, 12);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1, 695);
            this.panel6.TabIndex = 20;
            // 
            // PictureboxAuflegenLinsenPrismen
            // 
            this.PictureboxAuflegenLinsenPrismen.BackColor = System.Drawing.Color.White;
            this.PictureboxAuflegenLinsenPrismen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureboxAuflegenLinsenPrismen.Image = ((System.Drawing.Image)(resources.GetObject("PictureboxAuflegenLinsenPrismen.Image")));
            this.PictureboxAuflegenLinsenPrismen.Location = new System.Drawing.Point(703, 54);
            this.PictureboxAuflegenLinsenPrismen.Name = "PictureboxAuflegenLinsenPrismen";
            this.PictureboxAuflegenLinsenPrismen.Size = new System.Drawing.Size(380, 200);
            this.PictureboxAuflegenLinsenPrismen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureboxAuflegenLinsenPrismen.TabIndex = 66;
            this.PictureboxAuflegenLinsenPrismen.TabStop = false;
            // 
            // PictureboxZusatzinfo
            // 
            this.PictureboxZusatzinfo.BackColor = System.Drawing.Color.White;
            this.PictureboxZusatzinfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureboxZusatzinfo.Image = ((System.Drawing.Image)(resources.GetObject("PictureboxZusatzinfo.Image")));
            this.PictureboxZusatzinfo.Location = new System.Drawing.Point(703, 423);
            this.PictureboxZusatzinfo.Name = "PictureboxZusatzinfo";
            this.PictureboxZusatzinfo.Size = new System.Drawing.Size(380, 200);
            this.PictureboxZusatzinfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureboxZusatzinfo.TabIndex = 67;
            this.PictureboxZusatzinfo.TabStop = false;
            // 
            // lblInfoAuflegen
            // 
            this.lblInfoAuflegen.AutoSize = true;
            this.lblInfoAuflegen.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfoAuflegen.Location = new System.Drawing.Point(698, 20);
            this.lblInfoAuflegen.Name = "lblInfoAuflegen";
            this.lblInfoAuflegen.Size = new System.Drawing.Size(233, 29);
            this.lblInfoAuflegen.TabIndex = 68;
            this.lblInfoAuflegen.Text = "Information Auflegen";
            // 
            // lblZusatzinfo
            // 
            this.lblZusatzinfo.AutoSize = true;
            this.lblZusatzinfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZusatzinfo.Location = new System.Drawing.Point(698, 380);
            this.lblZusatzinfo.Name = "lblZusatzinfo";
            this.lblZusatzinfo.Size = new System.Drawing.Size(201, 29);
            this.lblZusatzinfo.TabIndex = 69;
            this.lblZusatzinfo.Text = "Zusatzinformation";
            // 
            // lblNrProjekt
            // 
            this.lblNrProjekt.AutoSize = true;
            this.lblNrProjekt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNrProjekt.Location = new System.Drawing.Point(19, 24);
            this.lblNrProjekt.Name = "lblNrProjekt";
            this.lblNrProjekt.Size = new System.Drawing.Size(251, 25);
            this.lblNrProjekt.TabIndex = 70;
            this.lblNrProjekt.Text = "Projekt/Auftragsnummer:";
            // 
            // lblMenge
            // 
            this.lblMenge.AutoSize = true;
            this.lblMenge.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMenge.Location = new System.Drawing.Point(19, 172);
            this.lblMenge.Name = "lblMenge";
            this.lblMenge.Size = new System.Drawing.Size(73, 25);
            this.lblMenge.TabIndex = 71;
            this.lblMenge.Text = "Menge";
            // 
            // txtboxAuftragsnummer
            // 
            this.txtboxAuftragsnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxAuftragsnummer.Location = new System.Drawing.Point(280, 19);
            this.txtboxAuftragsnummer.Name = "txtboxAuftragsnummer";
            this.txtboxAuftragsnummer.Size = new System.Drawing.Size(308, 38);
            this.txtboxAuftragsnummer.TabIndex = 72;
            // 
            // TxtboxMenge
            // 
            this.TxtboxMenge.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtboxMenge.Location = new System.Drawing.Point(162, 167);
            this.TxtboxMenge.Name = "TxtboxMenge";
            this.TxtboxMenge.Size = new System.Drawing.Size(124, 30);
            this.TxtboxMenge.TabIndex = 73;
            // 
            // lblArtikelnummer
            // 
            this.lblArtikelnummer.AutoSize = true;
            this.lblArtikelnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtikelnummer.Location = new System.Drawing.Point(19, 75);
            this.lblArtikelnummer.Name = "lblArtikelnummer";
            this.lblArtikelnummer.Size = new System.Drawing.Size(66, 25);
            this.lblArtikelnummer.TabIndex = 74;
            this.lblArtikelnummer.Text = "Artikel";
            // 
            // lblBelag
            // 
            this.lblBelag.AutoSize = true;
            this.lblBelag.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBelag.Location = new System.Drawing.Point(19, 208);
            this.lblBelag.Name = "lblBelag";
            this.lblBelag.Size = new System.Drawing.Size(62, 25);
            this.lblBelag.TabIndex = 76;
            this.lblBelag.Text = "Belag";
            // 
            // txtboxBelag
            // 
            this.txtboxBelag.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxBelag.Location = new System.Drawing.Point(162, 203);
            this.txtboxBelag.Name = "txtboxBelag";
            this.txtboxBelag.Size = new System.Drawing.Size(124, 30);
            this.txtboxBelag.TabIndex = 77;
            // 
            // txtboxProzess
            // 
            this.txtboxProzess.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxProzess.Location = new System.Drawing.Point(464, 203);
            this.txtboxProzess.Name = "txtboxProzess";
            this.txtboxProzess.Size = new System.Drawing.Size(124, 30);
            this.txtboxProzess.TabIndex = 79;
            // 
            // lblProzess
            // 
            this.lblProzess.AutoSize = true;
            this.lblProzess.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProzess.Location = new System.Drawing.Point(321, 208);
            this.lblProzess.Name = "lblProzess";
            this.lblProzess.Size = new System.Drawing.Size(83, 25);
            this.lblProzess.TabIndex = 78;
            this.lblProzess.Text = "Prozess";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel7.Location = new System.Drawing.Point(13, 253);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(678, 1);
            this.panel7.TabIndex = 80;
            // 
            // lblDatenArtikel
            // 
            this.lblDatenArtikel.AutoSize = true;
            this.lblDatenArtikel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDatenArtikel.Location = new System.Drawing.Point(19, 266);
            this.lblDatenArtikel.Name = "lblDatenArtikel";
            this.lblDatenArtikel.Size = new System.Drawing.Size(186, 25);
            this.lblDatenArtikel.TabIndex = 81;
            this.lblDatenArtikel.Text = "Information Artikel";
            // 
            // txtboxDurchmesser
            // 
            this.txtboxDurchmesser.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxDurchmesser.Location = new System.Drawing.Point(162, 306);
            this.txtboxDurchmesser.Name = "txtboxDurchmesser";
            this.txtboxDurchmesser.Size = new System.Drawing.Size(124, 30);
            this.txtboxDurchmesser.TabIndex = 83;
            // 
            // lblDurchmesser
            // 
            this.lblDurchmesser.AutoSize = true;
            this.lblDurchmesser.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDurchmesser.Location = new System.Drawing.Point(19, 311);
            this.lblDurchmesser.Name = "lblDurchmesser";
            this.lblDurchmesser.Size = new System.Drawing.Size(128, 25);
            this.lblDurchmesser.TabIndex = 82;
            this.lblDurchmesser.Text = "Durchmesser";
            // 
            // txtboxBrechwert
            // 
            this.txtboxBrechwert.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxBrechwert.Location = new System.Drawing.Point(464, 306);
            this.txtboxBrechwert.Name = "txtboxBrechwert";
            this.txtboxBrechwert.Size = new System.Drawing.Size(124, 30);
            this.txtboxBrechwert.TabIndex = 85;
            // 
            // lblBrechwert
            // 
            this.lblBrechwert.AutoSize = true;
            this.lblBrechwert.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrechwert.Location = new System.Drawing.Point(321, 311);
            this.lblBrechwert.Name = "lblBrechwert";
            this.lblBrechwert.Size = new System.Drawing.Size(99, 25);
            this.lblBrechwert.TabIndex = 84;
            this.lblBrechwert.Text = "Brechwert";
            // 
            // txtboxRadiusRueckseite
            // 
            this.txtboxRadiusRueckseite.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxRadiusRueckseite.Location = new System.Drawing.Point(464, 342);
            this.txtboxRadiusRueckseite.Name = "txtboxRadiusRueckseite";
            this.txtboxRadiusRueckseite.Size = new System.Drawing.Size(124, 30);
            this.txtboxRadiusRueckseite.TabIndex = 89;
            // 
            // lblRadiusRueckseite
            // 
            this.lblRadiusRueckseite.AutoSize = true;
            this.lblRadiusRueckseite.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRadiusRueckseite.Location = new System.Drawing.Point(321, 347);
            this.lblRadiusRueckseite.Name = "lblRadiusRueckseite";
            this.lblRadiusRueckseite.Size = new System.Drawing.Size(126, 25);
            this.lblRadiusRueckseite.TabIndex = 88;
            this.lblRadiusRueckseite.Text = "Radius Rück.";
            // 
            // txtboxRadiusVerguetung
            // 
            this.txtboxRadiusVerguetung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxRadiusVerguetung.Location = new System.Drawing.Point(162, 342);
            this.txtboxRadiusVerguetung.Name = "txtboxRadiusVerguetung";
            this.txtboxRadiusVerguetung.Size = new System.Drawing.Size(124, 30);
            this.txtboxRadiusVerguetung.TabIndex = 87;
            // 
            // lblRadiusverguetung
            // 
            this.lblRadiusverguetung.AutoSize = true;
            this.lblRadiusverguetung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRadiusverguetung.Location = new System.Drawing.Point(19, 347);
            this.lblRadiusverguetung.Name = "lblRadiusverguetung";
            this.lblRadiusverguetung.Size = new System.Drawing.Size(135, 25);
            this.lblRadiusverguetung.TabIndex = 86;
            this.lblRadiusverguetung.Text = "Radius Vergü.";
            // 
            // txtboxGlassorte
            // 
            this.txtboxGlassorte.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxGlassorte.Location = new System.Drawing.Point(464, 378);
            this.txtboxGlassorte.Name = "txtboxGlassorte";
            this.txtboxGlassorte.Size = new System.Drawing.Size(124, 30);
            this.txtboxGlassorte.TabIndex = 93;
            // 
            // lblGlassorte
            // 
            this.lblGlassorte.AutoSize = true;
            this.lblGlassorte.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGlassorte.Location = new System.Drawing.Point(321, 383);
            this.lblGlassorte.Name = "lblGlassorte";
            this.lblGlassorte.Size = new System.Drawing.Size(95, 25);
            this.lblGlassorte.TabIndex = 92;
            this.lblGlassorte.Text = "Glassorte";
            // 
            // txtboxGnummer
            // 
            this.txtboxGnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxGnummer.Location = new System.Drawing.Point(162, 378);
            this.txtboxGnummer.Name = "txtboxGnummer";
            this.txtboxGnummer.Size = new System.Drawing.Size(124, 30);
            this.txtboxGnummer.TabIndex = 91;
            // 
            // lblGnummer
            // 
            this.lblGnummer.AutoSize = true;
            this.lblGnummer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGnummer.Location = new System.Drawing.Point(19, 383);
            this.lblGnummer.Name = "lblGnummer";
            this.lblGnummer.Size = new System.Drawing.Size(108, 25);
            this.lblGnummer.TabIndex = 90;
            this.lblGnummer.Text = "G-Nummer";
            // 
            // txtboxBezeichnung
            // 
            this.txtboxBezeichnung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxBezeichnung.Location = new System.Drawing.Point(162, 130);
            this.txtboxBezeichnung.Name = "txtboxBezeichnung";
            this.txtboxBezeichnung.Size = new System.Drawing.Size(308, 30);
            this.txtboxBezeichnung.TabIndex = 95;
            // 
            // lblBezeichnung
            // 
            this.lblBezeichnung.AutoSize = true;
            this.lblBezeichnung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBezeichnung.Location = new System.Drawing.Point(19, 135);
            this.lblBezeichnung.Name = "lblBezeichnung";
            this.lblBezeichnung.Size = new System.Drawing.Size(126, 25);
            this.lblBezeichnung.TabIndex = 94;
            this.lblBezeichnung.Text = "Bezeichnung";
            // 
            // richtxtboxInforamationAuflegen
            // 
            this.richtxtboxInforamationAuflegen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richtxtboxInforamationAuflegen.Location = new System.Drawing.Point(703, 260);
            this.richtxtboxInforamationAuflegen.Name = "richtxtboxInforamationAuflegen";
            this.richtxtboxInforamationAuflegen.Size = new System.Drawing.Size(380, 72);
            this.richtxtboxInforamationAuflegen.TabIndex = 96;
            this.richtxtboxInforamationAuflegen.Text = "";
            // 
            // richtxtboxZusatzinfo
            // 
            this.richtxtboxZusatzinfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richtxtboxZusatzinfo.Location = new System.Drawing.Point(703, 629);
            this.richtxtboxZusatzinfo.Name = "richtxtboxZusatzinfo";
            this.richtxtboxZusatzinfo.Size = new System.Drawing.Size(380, 72);
            this.richtxtboxZusatzinfo.TabIndex = 97;
            this.richtxtboxZusatzinfo.Text = "";
            // 
            // txtboxVorreinigung
            // 
            this.txtboxVorreinigung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxVorreinigung.Location = new System.Drawing.Point(162, 480);
            this.txtboxVorreinigung.Name = "txtboxVorreinigung";
            this.txtboxVorreinigung.Size = new System.Drawing.Size(158, 30);
            this.txtboxVorreinigung.TabIndex = 99;
            // 
            // lblVorreinigung
            // 
            this.lblVorreinigung.AutoSize = true;
            this.lblVorreinigung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVorreinigung.Location = new System.Drawing.Point(19, 485);
            this.lblVorreinigung.Name = "lblVorreinigung";
            this.lblVorreinigung.Size = new System.Drawing.Size(123, 25);
            this.lblVorreinigung.TabIndex = 98;
            this.lblVorreinigung.Text = "Vorreinigung";
            // 
            // txtboxHandreinigung
            // 
            this.txtboxHandreinigung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxHandreinigung.Location = new System.Drawing.Point(517, 480);
            this.txtboxHandreinigung.Name = "txtboxHandreinigung";
            this.txtboxHandreinigung.Size = new System.Drawing.Size(158, 30);
            this.txtboxHandreinigung.TabIndex = 101;
            // 
            // lblHandreinigung
            // 
            this.lblHandreinigung.AutoSize = true;
            this.lblHandreinigung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHandreinigung.Location = new System.Drawing.Point(359, 485);
            this.lblHandreinigung.Name = "lblHandreinigung";
            this.lblHandreinigung.Size = new System.Drawing.Size(90, 25);
            this.lblHandreinigung.TabIndex = 100;
            this.lblHandreinigung.Text = "Auflegen";
            // 
            // lblVorbehandlung
            // 
            this.lblVorbehandlung.AutoSize = true;
            this.lblVorbehandlung.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVorbehandlung.Location = new System.Drawing.Point(19, 440);
            this.lblVorbehandlung.Name = "lblVorbehandlung";
            this.lblVorbehandlung.Size = new System.Drawing.Size(159, 25);
            this.lblVorbehandlung.TabIndex = 102;
            this.lblVorbehandlung.Text = "Vorbehandlung";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel8.Location = new System.Drawing.Point(13, 527);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(678, 1);
            this.panel8.TabIndex = 103;
            // 
            // lblWerAufgelegt
            // 
            this.lblWerAufgelegt.AutoSize = true;
            this.lblWerAufgelegt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWerAufgelegt.Location = new System.Drawing.Point(18, 538);
            this.lblWerAufgelegt.Name = "lblWerAufgelegt";
            this.lblWerAufgelegt.Size = new System.Drawing.Size(128, 25);
            this.lblWerAufgelegt.TabIndex = 104;
            this.lblWerAufgelegt.Text = "Bearbeitung";
            // 
            // lblZuzerstAuswaehlen
            // 
            this.lblZuzerstAuswaehlen.AutoSize = true;
            this.lblZuzerstAuswaehlen.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZuzerstAuswaehlen.ForeColor = System.Drawing.Color.Red;
            this.lblZuzerstAuswaehlen.Location = new System.Drawing.Point(476, 70);
            this.lblZuzerstAuswaehlen.Name = "lblZuzerstAuswaehlen";
            this.lblZuzerstAuswaehlen.Size = new System.Drawing.Size(197, 25);
            this.lblZuzerstAuswaehlen.TabIndex = 105;
            this.lblZuzerstAuswaehlen.Text = "<-- Zuerst auswählen";
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel9.Location = new System.Drawing.Point(51, 576);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(600, 1);
            this.panel9.TabIndex = 107;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel10.Location = new System.Drawing.Point(51, 601);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(600, 1);
            this.panel10.TabIndex = 108;
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel11.Location = new System.Drawing.Point(51, 626);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(600, 1);
            this.panel11.TabIndex = 109;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel12.Location = new System.Drawing.Point(51, 651);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(600, 1);
            this.panel12.TabIndex = 107;
            // 
            // panel13
            // 
            this.panel13.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel13.Location = new System.Drawing.Point(51, 676);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(600, 1);
            this.panel13.TabIndex = 110;
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel14.Location = new System.Drawing.Point(51, 701);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(600, 1);
            this.panel14.TabIndex = 111;
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel15.Location = new System.Drawing.Point(650, 576);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(1, 126);
            this.panel15.TabIndex = 112;
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel16.Location = new System.Drawing.Point(51, 576);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(1, 126);
            this.panel16.TabIndex = 113;
            // 
            // panel17
            // 
            this.panel17.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel17.Location = new System.Drawing.Point(196, 576);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(1, 126);
            this.panel17.TabIndex = 114;
            // 
            // panel18
            // 
            this.panel18.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel18.Location = new System.Drawing.Point(340, 576);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(1, 126);
            this.panel18.TabIndex = 115;
            // 
            // panel19
            // 
            this.panel19.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel19.Controls.Add(this.panel20);
            this.panel19.Location = new System.Drawing.Point(449, 576);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(1, 126);
            this.panel19.TabIndex = 116;
            // 
            // panel20
            // 
            this.panel20.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel20.Location = new System.Drawing.Point(0, 0);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(1, 126);
            this.panel20.TabIndex = 113;
            // 
            // panel21
            // 
            this.panel21.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel21.Location = new System.Drawing.Point(548, 576);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(1, 126);
            this.panel21.TabIndex = 117;
            // 
            // lblDatum
            // 
            this.lblDatum.AutoSize = true;
            this.lblDatum.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDatum.Location = new System.Drawing.Point(96, 581);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(49, 17);
            this.lblDatum.TabIndex = 118;
            this.lblDatum.Text = "Datum";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(245, 581);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 17);
            this.lblName.TabIndex = 119;
            this.lblName.Text = "Name";
            // 
            // lblStueck
            // 
            this.lblStueck.AutoSize = true;
            this.lblStueck.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStueck.Location = new System.Drawing.Point(371, 581);
            this.lblStueck.Name = "lblStueck";
            this.lblStueck.Size = new System.Drawing.Size(43, 17);
            this.lblStueck.TabIndex = 120;
            this.lblStueck.Text = "Stück";
            // 
            // lblStueckGes
            // 
            this.lblStueckGes.AutoSize = true;
            this.lblStueckGes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStueckGes.Location = new System.Drawing.Point(459, 581);
            this.lblStueckGes.Name = "lblStueckGes";
            this.lblStueckGes.Size = new System.Drawing.Size(85, 17);
            this.lblStueckGes.TabIndex = 121;
            this.lblStueckGes.Text = "Gesamt Stk.";
            // 
            // lblZeit
            // 
            this.lblZeit.AutoSize = true;
            this.lblZeit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZeit.Location = new System.Drawing.Point(582, 581);
            this.lblZeit.Name = "lblZeit";
            this.lblZeit.Size = new System.Drawing.Size(32, 17);
            this.lblZeit.TabIndex = 122;
            this.lblZeit.Text = "Zeit";
            // 
            // panel22
            // 
            this.panel22.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel22.Location = new System.Drawing.Point(13, 423);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(678, 1);
            this.panel22.TabIndex = 123;
            // 
            // lblDicke
            // 
            this.lblDicke.AutoSize = true;
            this.lblDicke.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDicke.Location = new System.Drawing.Point(321, 273);
            this.lblDicke.Name = "lblDicke";
            this.lblDicke.Size = new System.Drawing.Size(61, 25);
            this.lblDicke.TabIndex = 124;
            this.lblDicke.Text = "Dicke";
            // 
            // txtboxDicke
            // 
            this.txtboxDicke.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtboxDicke.Location = new System.Drawing.Point(464, 270);
            this.txtboxDicke.Name = "txtboxDicke";
            this.txtboxDicke.Size = new System.Drawing.Size(124, 30);
            this.txtboxDicke.TabIndex = 125;
            // 
            // Form_PrototypenauftragErstellen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1107, 755);
            this.Controls.Add(this.txtboxDicke);
            this.Controls.Add(this.lblDicke);
            this.Controls.Add(this.panel22);
            this.Controls.Add(this.lblZeit);
            this.Controls.Add(this.lblStueckGes);
            this.Controls.Add(this.lblStueck);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblDatum);
            this.Controls.Add(this.panel21);
            this.Controls.Add(this.panel19);
            this.Controls.Add(this.panel18);
            this.Controls.Add(this.panel17);
            this.Controls.Add(this.panel16);
            this.Controls.Add(this.panel15);
            this.Controls.Add(this.panel14);
            this.Controls.Add(this.panel13);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.panel11);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.lblZuzerstAuswaehlen);
            this.Controls.Add(this.lblWerAufgelegt);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.lblVorbehandlung);
            this.Controls.Add(this.txtboxHandreinigung);
            this.Controls.Add(this.lblHandreinigung);
            this.Controls.Add(this.txtboxVorreinigung);
            this.Controls.Add(this.lblVorreinigung);
            this.Controls.Add(this.richtxtboxZusatzinfo);
            this.Controls.Add(this.richtxtboxInforamationAuflegen);
            this.Controls.Add(this.txtboxBezeichnung);
            this.Controls.Add(this.lblBezeichnung);
            this.Controls.Add(this.txtboxGlassorte);
            this.Controls.Add(this.lblGlassorte);
            this.Controls.Add(this.txtboxGnummer);
            this.Controls.Add(this.lblGnummer);
            this.Controls.Add(this.txtboxRadiusRueckseite);
            this.Controls.Add(this.lblRadiusRueckseite);
            this.Controls.Add(this.txtboxRadiusVerguetung);
            this.Controls.Add(this.lblRadiusverguetung);
            this.Controls.Add(this.txtboxBrechwert);
            this.Controls.Add(this.lblBrechwert);
            this.Controls.Add(this.txtboxDurchmesser);
            this.Controls.Add(this.lblDurchmesser);
            this.Controls.Add(this.lblDatenArtikel);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.txtboxProzess);
            this.Controls.Add(this.lblProzess);
            this.Controls.Add(this.txtboxBelag);
            this.Controls.Add(this.lblBelag);
            this.Controls.Add(this.lblArtikelnummer);
            this.Controls.Add(this.TxtboxMenge);
            this.Controls.Add(this.txtboxAuftragsnummer);
            this.Controls.Add(this.lblMenge);
            this.Controls.Add(this.lblNrProjekt);
            this.Controls.Add(this.lblZusatzinfo);
            this.Controls.Add(this.lblInfoAuflegen);
            this.Controls.Add(this.PictureboxZusatzinfo);
            this.Controls.Add(this.PictureboxAuflegenLinsenPrismen);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.ComboboxArtikel);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnDrucken);
            this.Controls.Add(this.lblDokument);
            this.Controls.Add(this.LblErstelltAm);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(1123, 794);
            this.MinimumSize = new System.Drawing.Size(1123, 794);
            this.Name = "Form_PrototypenauftragErstellen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Prototypen Auftrag";
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxAuflegenLinsenPrismen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureboxZusatzinfo)).EndInit();
            this.panel19.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Drawing.Printing.PrintDocument PrintDocument;
        private System.Windows.Forms.PrintDialog PrintDialog;
        private System.Windows.Forms.Label lblDokument;
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
        private System.Windows.Forms.Label lblInfoAuflegen;
        private System.Windows.Forms.Label lblZusatzinfo;
        private System.Windows.Forms.Label lblNrProjekt;
        private System.Windows.Forms.Label lblMenge;
        private System.Windows.Forms.TextBox txtboxAuftragsnummer;
        private System.Windows.Forms.TextBox TxtboxMenge;
        private System.Windows.Forms.Label lblArtikelnummer;
        private System.Windows.Forms.Label lblBelag;
        private System.Windows.Forms.TextBox txtboxBelag;
        private System.Windows.Forms.TextBox txtboxProzess;
        private System.Windows.Forms.Label lblProzess;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lblDatenArtikel;
        private System.Windows.Forms.TextBox txtboxDurchmesser;
        private System.Windows.Forms.Label lblDurchmesser;
        private System.Windows.Forms.TextBox txtboxBrechwert;
        private System.Windows.Forms.Label lblBrechwert;
        private System.Windows.Forms.TextBox txtboxRadiusRueckseite;
        private System.Windows.Forms.Label lblRadiusRueckseite;
        private System.Windows.Forms.TextBox txtboxRadiusVerguetung;
        private System.Windows.Forms.Label lblRadiusverguetung;
        private System.Windows.Forms.TextBox txtboxGlassorte;
        private System.Windows.Forms.Label lblGlassorte;
        private System.Windows.Forms.TextBox txtboxGnummer;
        private System.Windows.Forms.Label lblGnummer;
        private System.Windows.Forms.TextBox txtboxBezeichnung;
        private System.Windows.Forms.Label lblBezeichnung;
        private System.Windows.Forms.RichTextBox richtxtboxInforamationAuflegen;
        private System.Windows.Forms.RichTextBox richtxtboxZusatzinfo;
        private System.Windows.Forms.TextBox txtboxVorreinigung;
        private System.Windows.Forms.Label lblVorreinigung;
        private System.Windows.Forms.TextBox txtboxHandreinigung;
        private System.Windows.Forms.Label lblHandreinigung;
        private System.Windows.Forms.Label lblVorbehandlung;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label lblWerAufgelegt;
        private System.Windows.Forms.Label lblZuzerstAuswaehlen;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Panel panel17;
        private System.Windows.Forms.Panel panel18;
        private System.Windows.Forms.Panel panel19;
        private System.Windows.Forms.Panel panel20;
        private System.Windows.Forms.Panel panel21;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStueck;
        private System.Windows.Forms.Label lblStueckGes;
        private System.Windows.Forms.Label lblZeit;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.Label lblDicke;
        private System.Windows.Forms.TextBox txtboxDicke;
    }
}
namespace VerwaltungKST1127.Personal
{
    partial class Form_Personalliste
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
            this.DgvPersonalliste = new System.Windows.Forms.DataGridView();
            this.lblUeberschrift = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtboxPersonalnummer = new System.Windows.Forms.TextBox();
            this.txtboxVorname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtboxNachname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Eintritt = new System.Windows.Forms.Label();
            this.datetimepickerEintritt = new System.Windows.Forms.DateTimePicker();
            this.datetimepickerGeburtstag = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtboxWochenstunden = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtboxTelefonnummer = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboboxTeam = new System.Windows.Forms.ComboBox();
            this.comboboxLohngruppe = new System.Windows.Forms.ComboBox();
            this.comboboxDirekterVorgesetzter = new System.Windows.Forms.ComboBox();
            this.BtnNeuerMitarbeiter = new System.Windows.Forms.Button();
            this.BtnMitarbeiterLoeschen = new System.Windows.Forms.Button();
            this.BtnMitarbeiterAnpassen = new System.Windows.Forms.Button();
            this.comboBoxPosition = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblMitarbeiterGesamt = new System.Windows.Forms.Label();
            this.lblVollzeit = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTeilzeit = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblAlgemein = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblVollzeitAequivalent = new System.Windows.Forms.Label();
            this.lblProduktivstunden = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DgvPersonalliste)).BeginInit();
            this.SuspendLayout();
            // 
            // DgvPersonalliste
            // 
            this.DgvPersonalliste.AllowUserToAddRows = false;
            this.DgvPersonalliste.AllowUserToDeleteRows = false;
            this.DgvPersonalliste.AllowUserToOrderColumns = true;
            this.DgvPersonalliste.AllowUserToResizeColumns = false;
            this.DgvPersonalliste.AllowUserToResizeRows = false;
            this.DgvPersonalliste.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.DgvPersonalliste.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvPersonalliste.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvPersonalliste.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvPersonalliste.Location = new System.Drawing.Point(13, 210);
            this.DgvPersonalliste.MultiSelect = false;
            this.DgvPersonalliste.Name = "DgvPersonalliste";
            this.DgvPersonalliste.ReadOnly = true;
            this.DgvPersonalliste.RowHeadersVisible = false;
            this.DgvPersonalliste.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DgvPersonalliste.Size = new System.Drawing.Size(1465, 683);
            this.DgvPersonalliste.TabIndex = 0;
            this.DgvPersonalliste.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvPersonalliste_CellClick);
            this.DgvPersonalliste.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvPersonalliste_CellFormatting);
            // 
            // lblUeberschrift
            // 
            this.lblUeberschrift.AutoSize = true;
            this.lblUeberschrift.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUeberschrift.Location = new System.Drawing.Point(13, 13);
            this.lblUeberschrift.Name = "lblUeberschrift";
            this.lblUeberschrift.Size = new System.Drawing.Size(647, 36);
            this.lblUeberschrift.TabIndex = 1;
            this.lblUeberschrift.Text = "Personalliste KST1127 - US-Reinigen/Vergüten";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Personalnummer";
            // 
            // txtboxPersonalnummer
            // 
            this.txtboxPersonalnummer.Location = new System.Drawing.Point(134, 75);
            this.txtboxPersonalnummer.Name = "txtboxPersonalnummer";
            this.txtboxPersonalnummer.Size = new System.Drawing.Size(132, 20);
            this.txtboxPersonalnummer.TabIndex = 3;
            // 
            // txtboxVorname
            // 
            this.txtboxVorname.Location = new System.Drawing.Point(134, 100);
            this.txtboxVorname.Name = "txtboxVorname";
            this.txtboxVorname.Size = new System.Drawing.Size(132, 20);
            this.txtboxVorname.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Vorname";
            // 
            // txtboxNachname
            // 
            this.txtboxNachname.Location = new System.Drawing.Point(134, 126);
            this.txtboxNachname.Name = "txtboxNachname";
            this.txtboxNachname.Size = new System.Drawing.Size(132, 20);
            this.txtboxNachname.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Nachname";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Position";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(289, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Geburtstag";
            // 
            // Eintritt
            // 
            this.Eintritt.AutoSize = true;
            this.Eintritt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Eintritt.Location = new System.Drawing.Point(289, 74);
            this.Eintritt.Name = "Eintritt";
            this.Eintritt.Size = new System.Drawing.Size(48, 17);
            this.Eintritt.TabIndex = 10;
            this.Eintritt.Text = "Eintritt";
            // 
            // datetimepickerEintritt
            // 
            this.datetimepickerEintritt.Location = new System.Drawing.Point(411, 73);
            this.datetimepickerEintritt.Name = "datetimepickerEintritt";
            this.datetimepickerEintritt.Size = new System.Drawing.Size(200, 20);
            this.datetimepickerEintritt.TabIndex = 18;
            // 
            // datetimepickerGeburtstag
            // 
            this.datetimepickerGeburtstag.Location = new System.Drawing.Point(411, 100);
            this.datetimepickerGeburtstag.Name = "datetimepickerGeburtstag";
            this.datetimepickerGeburtstag.Size = new System.Drawing.Size(200, 20);
            this.datetimepickerGeburtstag.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(640, 127);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 17);
            this.label8.TabIndex = 26;
            this.label8.Text = "Lohngruppe";
            // 
            // txtboxWochenstunden
            // 
            this.txtboxWochenstunden.Location = new System.Drawing.Point(762, 100);
            this.txtboxWochenstunden.Name = "txtboxWochenstunden";
            this.txtboxWochenstunden.Size = new System.Drawing.Size(132, 20);
            this.txtboxWochenstunden.TabIndex = 34;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(640, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 17);
            this.label9.TabIndex = 24;
            this.label9.Text = "Wochenstunden";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(289, 127);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 22;
            this.label10.Text = "Team";
            // 
            // txtboxTelefonnummer
            // 
            this.txtboxTelefonnummer.Location = new System.Drawing.Point(762, 74);
            this.txtboxTelefonnummer.Name = "txtboxTelefonnummer";
            this.txtboxTelefonnummer.Size = new System.Drawing.Size(132, 20);
            this.txtboxTelefonnummer.TabIndex = 33;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(640, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 17);
            this.label11.TabIndex = 20;
            this.label11.Text = "Telefonnummer";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(289, 153);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 17);
            this.label12.TabIndex = 28;
            this.label12.Text = "Direkter Vorg.";
            // 
            // comboboxTeam
            // 
            this.comboboxTeam.FormattingEnabled = true;
            this.comboboxTeam.Items.AddRange(new object[] {
            "136",
            "158",
            "159",
            "160"});
            this.comboboxTeam.Location = new System.Drawing.Point(411, 126);
            this.comboboxTeam.Name = "comboboxTeam";
            this.comboboxTeam.Size = new System.Drawing.Size(132, 21);
            this.comboboxTeam.TabIndex = 30;
            // 
            // comboboxLohngruppe
            // 
            this.comboboxLohngruppe.FormattingEnabled = true;
            this.comboboxLohngruppe.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "Geheim"});
            this.comboboxLohngruppe.Location = new System.Drawing.Point(762, 126);
            this.comboboxLohngruppe.Name = "comboboxLohngruppe";
            this.comboboxLohngruppe.Size = new System.Drawing.Size(132, 21);
            this.comboboxLohngruppe.TabIndex = 35;
            // 
            // comboboxDirekterVorgesetzter
            // 
            this.comboboxDirekterVorgesetzter.FormattingEnabled = true;
            this.comboboxDirekterVorgesetzter.Items.AddRange(new object[] {
            "Schreiner S.",
            "Plunser S.",
            "Herburger M.",
            "Jenewein T.",
            "Ranalter H."});
            this.comboboxDirekterVorgesetzter.Location = new System.Drawing.Point(411, 153);
            this.comboboxDirekterVorgesetzter.Name = "comboboxDirekterVorgesetzter";
            this.comboboxDirekterVorgesetzter.Size = new System.Drawing.Size(132, 21);
            this.comboboxDirekterVorgesetzter.TabIndex = 32;
            // 
            // BtnNeuerMitarbeiter
            // 
            this.BtnNeuerMitarbeiter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BtnNeuerMitarbeiter.Location = new System.Drawing.Point(1018, 141);
            this.BtnNeuerMitarbeiter.Name = "BtnNeuerMitarbeiter";
            this.BtnNeuerMitarbeiter.Size = new System.Drawing.Size(139, 48);
            this.BtnNeuerMitarbeiter.TabIndex = 50;
            this.BtnNeuerMitarbeiter.Text = "Neuer Mitarbeiter";
            this.BtnNeuerMitarbeiter.UseVisualStyleBackColor = false;
            this.BtnNeuerMitarbeiter.Click += new System.EventHandler(this.BtnNeuerMitarbeiter_Click);
            // 
            // BtnMitarbeiterLoeschen
            // 
            this.BtnMitarbeiterLoeschen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.BtnMitarbeiterLoeschen.Location = new System.Drawing.Point(1163, 142);
            this.BtnMitarbeiterLoeschen.Name = "BtnMitarbeiterLoeschen";
            this.BtnMitarbeiterLoeschen.Size = new System.Drawing.Size(139, 47);
            this.BtnMitarbeiterLoeschen.TabIndex = 60;
            this.BtnMitarbeiterLoeschen.Text = "Mitarbeiter Löschen";
            this.BtnMitarbeiterLoeschen.UseVisualStyleBackColor = false;
            this.BtnMitarbeiterLoeschen.Click += new System.EventHandler(this.BtnMitarbeiterLoeschen_Click);
            // 
            // BtnMitarbeiterAnpassen
            // 
            this.BtnMitarbeiterAnpassen.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BtnMitarbeiterAnpassen.Location = new System.Drawing.Point(1308, 142);
            this.BtnMitarbeiterAnpassen.Name = "BtnMitarbeiterAnpassen";
            this.BtnMitarbeiterAnpassen.Size = new System.Drawing.Size(139, 48);
            this.BtnMitarbeiterAnpassen.TabIndex = 70;
            this.BtnMitarbeiterAnpassen.Text = "Mitarbeiter Anpassen";
            this.BtnMitarbeiterAnpassen.UseVisualStyleBackColor = false;
            this.BtnMitarbeiterAnpassen.Click += new System.EventHandler(this.BtnMitarbeiterAnpassen_Click);
            // 
            // comboBoxPosition
            // 
            this.comboBoxPosition.FormattingEnabled = true;
            this.comboBoxPosition.Items.AddRange(new object[] {
            "Kostenstellenleiter:in",
            "Stv. Kostenstellenleiter:in",
            "Prozesstechniker:in",
            "Gruppenleiter:in",
            "Stv. Gruppenleiter:in",
            "Aufleger:in",
            "Anlagentechniker:in",
            "Werkzeugwart:in",
            "US-Bediener:in"});
            this.comboBoxPosition.Location = new System.Drawing.Point(134, 152);
            this.comboBoxPosition.Name = "comboBoxPosition";
            this.comboBoxPosition.Size = new System.Drawing.Size(132, 21);
            this.comboBoxPosition.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(1069, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 17);
            this.label6.TabIndex = 38;
            this.label6.Text = "Vollzeit:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.SystemColors.Control;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(997, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(129, 17);
            this.label13.TabIndex = 37;
            this.label13.Text = "Mitarbeiter gesamt:";
            // 
            // lblMitarbeiterGesamt
            // 
            this.lblMitarbeiterGesamt.AutoSize = true;
            this.lblMitarbeiterGesamt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMitarbeiterGesamt.Location = new System.Drawing.Point(1132, 20);
            this.lblMitarbeiterGesamt.Name = "lblMitarbeiterGesamt";
            this.lblMitarbeiterGesamt.Size = new System.Drawing.Size(16, 17);
            this.lblMitarbeiterGesamt.TabIndex = 71;
            this.lblMitarbeiterGesamt.Text = "0";
            // 
            // lblVollzeit
            // 
            this.lblVollzeit.AutoSize = true;
            this.lblVollzeit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVollzeit.Location = new System.Drawing.Point(1132, 46);
            this.lblVollzeit.Name = "lblVollzeit";
            this.lblVollzeit.Size = new System.Drawing.Size(16, 17);
            this.lblVollzeit.TabIndex = 72;
            this.lblVollzeit.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1069, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 17);
            this.label5.TabIndex = 73;
            this.label5.Text = "Teilzeit:";
            // 
            // lblTeilzeit
            // 
            this.lblTeilzeit.AutoSize = true;
            this.lblTeilzeit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTeilzeit.Location = new System.Drawing.Point(1132, 73);
            this.lblTeilzeit.Name = "lblTeilzeit";
            this.lblTeilzeit.Size = new System.Drawing.Size(16, 17);
            this.lblTeilzeit.TabIndex = 74;
            this.lblTeilzeit.Text = "0";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(971, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 179);
            this.panel1.TabIndex = 75;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(972, 121);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(500, 1);
            this.panel2.TabIndex = 76;
            // 
            // lblAlgemein
            // 
            this.lblAlgemein.AutoSize = true;
            this.lblAlgemein.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlgemein.Location = new System.Drawing.Point(1377, 73);
            this.lblAlgemein.Name = "lblAlgemein";
            this.lblAlgemein.Size = new System.Drawing.Size(16, 17);
            this.lblAlgemein.TabIndex = 82;
            this.lblAlgemein.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(1301, 73);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 17);
            this.label15.TabIndex = 81;
            this.label15.Text = "Algemein:";
            // 
            // lblVollzeitAequivalent
            // 
            this.lblVollzeitAequivalent.AutoSize = true;
            this.lblVollzeitAequivalent.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVollzeitAequivalent.Location = new System.Drawing.Point(1377, 20);
            this.lblVollzeitAequivalent.Name = "lblVollzeitAequivalent";
            this.lblVollzeitAequivalent.Size = new System.Drawing.Size(16, 17);
            this.lblVollzeitAequivalent.TabIndex = 80;
            this.lblVollzeitAequivalent.Text = "0";
            // 
            // lblProduktivstunden
            // 
            this.lblProduktivstunden.AutoSize = true;
            this.lblProduktivstunden.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProduktivstunden.Location = new System.Drawing.Point(1377, 46);
            this.lblProduktivstunden.Name = "lblProduktivstunden";
            this.lblProduktivstunden.Size = new System.Drawing.Size(16, 17);
            this.lblProduktivstunden.TabIndex = 79;
            this.lblProduktivstunden.Text = "0";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(1249, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(122, 17);
            this.label18.TabIndex = 78;
            this.label18.Text = "Vollzeitäquivalent:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.SystemColors.Control;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(1255, 46);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(116, 17);
            this.label19.TabIndex = 77;
            this.label19.Text = "Davon Produktiv:";
            // 
            // Form_Personalliste
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1490, 905);
            this.Controls.Add(this.lblAlgemein);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.lblVollzeitAequivalent);
            this.Controls.Add(this.lblProduktivstunden);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTeilzeit);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblVollzeit);
            this.Controls.Add(this.lblMitarbeiterGesamt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.comboBoxPosition);
            this.Controls.Add(this.BtnMitarbeiterAnpassen);
            this.Controls.Add(this.BtnMitarbeiterLoeschen);
            this.Controls.Add(this.BtnNeuerMitarbeiter);
            this.Controls.Add(this.comboboxDirekterVorgesetzter);
            this.Controls.Add(this.comboboxLohngruppe);
            this.Controls.Add(this.comboboxTeam);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtboxWochenstunden);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtboxTelefonnummer);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.datetimepickerGeburtstag);
            this.Controls.Add(this.datetimepickerEintritt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Eintritt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtboxNachname);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtboxVorname);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtboxPersonalnummer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblUeberschrift);
            this.Controls.Add(this.DgvPersonalliste);
            this.MaximumSize = new System.Drawing.Size(1506, 944);
            this.MinimumSize = new System.Drawing.Size(1506, 944);
            this.Name = "Form_Personalliste";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Personalliste Kst1127";
            ((System.ComponentModel.ISupportInitialize)(this.DgvPersonalliste)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DgvPersonalliste;
        private System.Windows.Forms.Label lblUeberschrift;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtboxPersonalnummer;
        private System.Windows.Forms.TextBox txtboxVorname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtboxNachname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label Eintritt;
        private System.Windows.Forms.DateTimePicker datetimepickerEintritt;
        private System.Windows.Forms.DateTimePicker datetimepickerGeburtstag;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtboxWochenstunden;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtboxTelefonnummer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboboxTeam;
        private System.Windows.Forms.ComboBox comboboxLohngruppe;
        private System.Windows.Forms.ComboBox comboboxDirekterVorgesetzter;
        private System.Windows.Forms.Button BtnNeuerMitarbeiter;
        private System.Windows.Forms.Button BtnMitarbeiterLoeschen;
        private System.Windows.Forms.Button BtnMitarbeiterAnpassen;
        private System.Windows.Forms.ComboBox comboBoxPosition;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblMitarbeiterGesamt;
        private System.Windows.Forms.Label lblVollzeit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTeilzeit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblAlgemein;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblVollzeitAequivalent;
        private System.Windows.Forms.Label lblProduktivstunden;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
    }
}
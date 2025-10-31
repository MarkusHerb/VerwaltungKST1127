namespace VerwaltungKST1127.Produktionsauswertung
{
    partial class Form_RFIDAnsichtWaschanlagen
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.TimerDatumUhrzeit = new System.Windows.Forms.Timer(this.components);
            this.lblDateTime = new System.Windows.Forms.Label();
            this.dGvRFID = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePickerDatumAb = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerDatumBis = new System.Windows.Forms.DateTimePicker();
            this.txtBoxAuftragsnummer = new System.Windows.Forms.TextBox();
            this.txtBoxArtikelnummer = new System.Windows.Forms.TextBox();
            this.txtBoxUID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBoxWaschprogramm = new System.Windows.Forms.TextBox();
            this.btnLoescheFilter = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblEingeleseneWaschkoerbe = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dGvRFID)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "RFID - Auswertung";
            // 
            // TimerDatumUhrzeit
            // 
            this.TimerDatumUhrzeit.Interval = 1000;
            this.TimerDatumUhrzeit.Tick += new System.EventHandler(this.TimerDatumUhrzeit_Tick);
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.Location = new System.Drawing.Point(15, 46);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(125, 17);
            this.lblDateTime.TabIndex = 1;
            this.lblDateTime.Text = "Datum + Uhrzeit";
            // 
            // dGvRFID
            // 
            this.dGvRFID.AllowUserToAddRows = false;
            this.dGvRFID.AllowUserToDeleteRows = false;
            this.dGvRFID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGvRFID.Location = new System.Drawing.Point(12, 146);
            this.dGvRFID.MultiSelect = false;
            this.dGvRFID.Name = "dGvRFID";
            this.dGvRFID.ReadOnly = true;
            this.dGvRFID.Size = new System.Drawing.Size(1508, 636);
            this.dGvRFID.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(732, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(182, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Suche Auftragsnummer:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(331, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Datum ab:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(331, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Datum bis:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(732, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Suche Artikelnummer:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(730, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 20);
            this.label6.TabIndex = 7;
            this.label6.Text = "Suche UID:";
            // 
            // dateTimePickerDatumAb
            // 
            this.dateTimePickerDatumAb.Location = new System.Drawing.Point(431, 19);
            this.dateTimePickerDatumAb.Name = "dateTimePickerDatumAb";
            this.dateTimePickerDatumAb.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerDatumAb.TabIndex = 8;
            // 
            // dateTimePickerDatumBis
            // 
            this.dateTimePickerDatumBis.Location = new System.Drawing.Point(431, 45);
            this.dateTimePickerDatumBis.Name = "dateTimePickerDatumBis";
            this.dateTimePickerDatumBis.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerDatumBis.TabIndex = 9;
            // 
            // txtBoxAuftragsnummer
            // 
            this.txtBoxAuftragsnummer.Location = new System.Drawing.Point(921, 22);
            this.txtBoxAuftragsnummer.Name = "txtBoxAuftragsnummer";
            this.txtBoxAuftragsnummer.Size = new System.Drawing.Size(189, 20);
            this.txtBoxAuftragsnummer.TabIndex = 10;
            // 
            // txtBoxArtikelnummer
            // 
            this.txtBoxArtikelnummer.Location = new System.Drawing.Point(921, 46);
            this.txtBoxArtikelnummer.Name = "txtBoxArtikelnummer";
            this.txtBoxArtikelnummer.Size = new System.Drawing.Size(189, 20);
            this.txtBoxArtikelnummer.TabIndex = 11;
            // 
            // txtBoxUID
            // 
            this.txtBoxUID.Location = new System.Drawing.Point(921, 94);
            this.txtBoxUID.Name = "txtBoxUID";
            this.txtBoxUID.Size = new System.Drawing.Size(189, 20);
            this.txtBoxUID.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(730, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(184, 20);
            this.label7.TabIndex = 13;
            this.label7.Text = "Suche Waschprogramm:";
            // 
            // txtBoxWaschprogramm
            // 
            this.txtBoxWaschprogramm.Location = new System.Drawing.Point(921, 70);
            this.txtBoxWaschprogramm.Name = "txtBoxWaschprogramm";
            this.txtBoxWaschprogramm.Size = new System.Drawing.Size(189, 20);
            this.txtBoxWaschprogramm.TabIndex = 14;
            // 
            // btnLoescheFilter
            // 
            this.btnLoescheFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnLoescheFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoescheFilter.Location = new System.Drawing.Point(12, 77);
            this.btnLoescheFilter.Name = "btnLoescheFilter";
            this.btnLoescheFilter.Size = new System.Drawing.Size(232, 63);
            this.btnLoescheFilter.TabIndex = 15;
            this.btnLoescheFilter.Text = "Filter löschen";
            this.btnLoescheFilter.UseVisualStyleBackColor = false;
            this.btnLoescheFilter.Click += new System.EventHandler(this.btnLoescheFilter_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(921, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(189, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Eingaben mit ENTER bestätigen";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(332, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(312, 39);
            this.label9.TabIndex = 17;
            this.label9.Text = "Es werden immer die vergangenen 30 Tag eingelesen\r\nWenn Datum Ab über mehrere Mon" +
    "ate eingestellt wird\r\n--> Aktualisierung dauert etwas!";
            // 
            // lblEingeleseneWaschkoerbe
            // 
            this.lblEingeleseneWaschkoerbe.AutoSize = true;
            this.lblEingeleseneWaschkoerbe.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEingeleseneWaschkoerbe.Location = new System.Drawing.Point(1190, 22);
            this.lblEingeleseneWaschkoerbe.Name = "lblEingeleseneWaschkoerbe";
            this.lblEingeleseneWaschkoerbe.Size = new System.Drawing.Size(222, 20);
            this.lblEingeleseneWaschkoerbe.TabIndex = 18;
            this.lblEingeleseneWaschkoerbe.Text = "Eingelesene Waschkörbe: ";
            // 
            // Form_RFIDAnsichtWaschanlagen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1532, 794);
            this.Controls.Add(this.lblEingeleseneWaschkoerbe);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnLoescheFilter);
            this.Controls.Add(this.txtBoxWaschprogramm);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtBoxUID);
            this.Controls.Add(this.txtBoxArtikelnummer);
            this.Controls.Add(this.txtBoxAuftragsnummer);
            this.Controls.Add(this.dateTimePickerDatumBis);
            this.Controls.Add(this.dateTimePickerDatumAb);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dGvRFID);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.label1);
            this.Name = "Form_RFIDAnsichtWaschanlagen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ausgelesene RFID-Chips";
            ((System.ComponentModel.ISupportInitialize)(this.dGvRFID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer TimerDatumUhrzeit;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.DataGridView dGvRFID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePickerDatumAb;
        private System.Windows.Forms.DateTimePicker dateTimePickerDatumBis;
        private System.Windows.Forms.TextBox txtBoxAuftragsnummer;
        private System.Windows.Forms.TextBox txtBoxArtikelnummer;
        private System.Windows.Forms.TextBox txtBoxUID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBoxWaschprogramm;
        private System.Windows.Forms.Button btnLoescheFilter;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblEingeleseneWaschkoerbe;
    }
}
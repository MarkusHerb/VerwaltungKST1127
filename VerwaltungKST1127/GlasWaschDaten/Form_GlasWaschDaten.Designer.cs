namespace VerwaltungKST1127.GlasWaschDaten
{
    partial class Form_GlasWaschDaten
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
            this.ComboBoxArtikel = new System.Windows.Forms.ComboBox();
            this.ComboBoxGNummer = new System.Windows.Forms.ComboBox();
            this.ComboBoxGlassorte = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DgvAuswahlGlasdaten = new System.Windows.Forms.DataGridView();
            this.BtnResett = new System.Windows.Forms.Button();
            this.LblAnzahlGNummern = new System.Windows.Forms.Label();
            this.LblAnzahlGlassorten = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DgvAuswahlGlasdaten)).BeginInit();
            this.SuspendLayout();
            // 
            // ComboBoxArtikel
            // 
            this.ComboBoxArtikel.FormattingEnabled = true;
            this.ComboBoxArtikel.Location = new System.Drawing.Point(13, 63);
            this.ComboBoxArtikel.Name = "ComboBoxArtikel";
            this.ComboBoxArtikel.Size = new System.Drawing.Size(167, 21);
            this.ComboBoxArtikel.TabIndex = 0;
            this.ComboBoxArtikel.SelectedIndexChanged += new System.EventHandler(this.ComboBoxArtikel_SelectedIndexChanged);
            // 
            // ComboBoxGNummer
            // 
            this.ComboBoxGNummer.FormattingEnabled = true;
            this.ComboBoxGNummer.Location = new System.Drawing.Point(206, 63);
            this.ComboBoxGNummer.Name = "ComboBoxGNummer";
            this.ComboBoxGNummer.Size = new System.Drawing.Size(168, 21);
            this.ComboBoxGNummer.TabIndex = 1;
            this.ComboBoxGNummer.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGNummer_SelectedIndexChanged);
            // 
            // ComboBoxGlassorte
            // 
            this.ComboBoxGlassorte.FormattingEnabled = true;
            this.ComboBoxGlassorte.Location = new System.Drawing.Point(400, 63);
            this.ComboBoxGlassorte.Name = "ComboBoxGlassorte";
            this.ComboBoxGlassorte.Size = new System.Drawing.Size(166, 21);
            this.ComboBoxGlassorte.TabIndex = 2;
            this.ComboBoxGlassorte.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGlassorte_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Artikel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(202, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "G-Nummer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(396, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Glassorte";
            // 
            // DgvAuswahlGlasdaten
            // 
            this.DgvAuswahlGlasdaten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvAuswahlGlasdaten.Location = new System.Drawing.Point(13, 123);
            this.DgvAuswahlGlasdaten.Name = "DgvAuswahlGlasdaten";
            this.DgvAuswahlGlasdaten.Size = new System.Drawing.Size(1074, 335);
            this.DgvAuswahlGlasdaten.TabIndex = 6;
            // 
            // BtnResett
            // 
            this.BtnResett.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BtnResett.Location = new System.Drawing.Point(592, 44);
            this.BtnResett.Name = "BtnResett";
            this.BtnResett.Size = new System.Drawing.Size(96, 40);
            this.BtnResett.TabIndex = 8;
            this.BtnResett.Text = "Resett Auswahl";
            this.BtnResett.UseVisualStyleBackColor = false;
            this.BtnResett.Click += new System.EventHandler(this.BtnResett_Click);
            // 
            // LblAnzahlGNummern
            // 
            this.LblAnzahlGNummern.AutoSize = true;
            this.LblAnzahlGNummern.Location = new System.Drawing.Point(969, 22);
            this.LblAnzahlGNummern.Name = "LblAnzahlGNummern";
            this.LblAnzahlGNummern.Size = new System.Drawing.Size(95, 13);
            this.LblAnzahlGNummern.TabIndex = 11;
            this.LblAnzahlGNummern.Text = "Anzahl GNummern";
            // 
            // LblAnzahlGlassorten
            // 
            this.LblAnzahlGlassorten.AutoSize = true;
            this.LblAnzahlGlassorten.Location = new System.Drawing.Point(969, 44);
            this.LblAnzahlGlassorten.Name = "LblAnzahlGlassorten";
            this.LblAnzahlGlassorten.Size = new System.Drawing.Size(92, 13);
            this.LblAnzahlGlassorten.TabIndex = 12;
            this.LblAnzahlGlassorten.Text = "Anzahl Glassorten";
            // 
            // Form_GlasWaschDaten
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1099, 470);
            this.Controls.Add(this.LblAnzahlGlassorten);
            this.Controls.Add(this.LblAnzahlGNummern);
            this.Controls.Add(this.BtnResett);
            this.Controls.Add(this.DgvAuswahlGlasdaten);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboBoxGlassorte);
            this.Controls.Add(this.ComboBoxGNummer);
            this.Controls.Add(this.ComboBoxArtikel);
            this.Name = "Form_GlasWaschDaten";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Glas/Waschdaten";
            ((System.ComponentModel.ISupportInitialize)(this.DgvAuswahlGlasdaten)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboBoxArtikel;
        private System.Windows.Forms.ComboBox ComboBoxGNummer;
        private System.Windows.Forms.ComboBox ComboBoxGlassorte;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView DgvAuswahlGlasdaten;
        private System.Windows.Forms.Button BtnResett;
        private System.Windows.Forms.Label LblAnzahlGNummern;
        private System.Windows.Forms.Label LblAnzahlGlassorten;
    }
}
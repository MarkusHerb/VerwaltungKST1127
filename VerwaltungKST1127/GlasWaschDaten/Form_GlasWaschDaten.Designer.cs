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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.ComboBoxArtikel.BackColor = System.Drawing.Color.White;
            this.ComboBoxArtikel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxArtikel.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.ComboBoxArtikel.FormattingEnabled = true;
            this.ComboBoxArtikel.Location = new System.Drawing.Point(13, 63);
            this.ComboBoxArtikel.Name = "ComboBoxArtikel";
            this.ComboBoxArtikel.Size = new System.Drawing.Size(167, 23);
            this.ComboBoxArtikel.TabIndex = 0;
            this.ComboBoxArtikel.SelectedIndexChanged += new System.EventHandler(this.ComboBoxArtikel_SelectedIndexChanged);
            // 
            // ComboBoxGNummer
            // 
            this.ComboBoxGNummer.BackColor = System.Drawing.Color.White;
            this.ComboBoxGNummer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxGNummer.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.ComboBoxGNummer.FormattingEnabled = true;
            this.ComboBoxGNummer.Location = new System.Drawing.Point(206, 63);
            this.ComboBoxGNummer.Name = "ComboBoxGNummer";
            this.ComboBoxGNummer.Size = new System.Drawing.Size(168, 23);
            this.ComboBoxGNummer.TabIndex = 1;
            this.ComboBoxGNummer.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGNummer_SelectedIndexChanged);
            // 
            // ComboBoxGlassorte
            // 
            this.ComboBoxGlassorte.BackColor = System.Drawing.Color.White;
            this.ComboBoxGlassorte.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxGlassorte.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.ComboBoxGlassorte.FormattingEnabled = true;
            this.ComboBoxGlassorte.Location = new System.Drawing.Point(400, 63);
            this.ComboBoxGlassorte.Name = "ComboBoxGlassorte";
            this.ComboBoxGlassorte.Size = new System.Drawing.Size(166, 23);
            this.ComboBoxGlassorte.TabIndex = 2;
            this.ComboBoxGlassorte.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGlassorte_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "Artikel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.label2.Location = new System.Drawing.Point(202, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "G-Nummer";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.label3.Location = new System.Drawing.Point(396, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Glassorte";
            // 
            // DgvAuswahlGlasdaten
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(217, 231, 249);
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.DgvAuswahlGlasdaten.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.DgvAuswahlGlasdaten.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvAuswahlGlasdaten.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DgvAuswahlGlasdaten.BackgroundColor = System.Drawing.Color.White;
            this.DgvAuswahlGlasdaten.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DgvAuswahlGlasdaten.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(31, 55, 100);
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(31, 55, 100);
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvAuswahlGlasdaten.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvAuswahlGlasdaten.ColumnHeadersHeight = 30;
            this.DgvAuswahlGlasdaten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(217, 231, 249);
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvAuswahlGlasdaten.DefaultCellStyle = dataGridViewCellStyle2;
            this.DgvAuswahlGlasdaten.EnableHeadersVisualStyles = false;
            this.DgvAuswahlGlasdaten.GridColor = System.Drawing.Color.FromArgb(222, 227, 233);
            this.DgvAuswahlGlasdaten.Location = new System.Drawing.Point(13, 123);
            this.DgvAuswahlGlasdaten.Name = "DgvAuswahlGlasdaten";
            this.DgvAuswahlGlasdaten.RowHeadersVisible = false;
            this.DgvAuswahlGlasdaten.RowTemplate.Height = 25;
            this.DgvAuswahlGlasdaten.Size = new System.Drawing.Size(1074, 335);
            this.DgvAuswahlGlasdaten.TabIndex = 6;
            // 
            // BtnResett
            // 
            this.BtnResett.BackColor = System.Drawing.Color.FromArgb(255, 190, 0);
            this.BtnResett.FlatAppearance.BorderSize = 0;
            this.BtnResett.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnResett.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnResett.ForeColor = System.Drawing.Color.Black;
            this.BtnResett.Location = new System.Drawing.Point(592, 44);
            this.BtnResett.Name = "BtnResett";
            this.BtnResett.Size = new System.Drawing.Size(120, 40);
            this.BtnResett.TabIndex = 8;
            this.BtnResett.Text = "Resett Auswahl";
            this.BtnResett.UseVisualStyleBackColor = false;
            this.BtnResett.Click += new System.EventHandler(this.BtnResett_Click);
            // 
            // LblAnzahlGNummern
            // 
            this.LblAnzahlGNummern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblAnzahlGNummern.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnzahlGNummern.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.LblAnzahlGNummern.Location = new System.Drawing.Point(807, 24);
            this.LblAnzahlGNummern.Name = "LblAnzahlGNummern";
            this.LblAnzahlGNummern.Size = new System.Drawing.Size(280, 22);
            this.LblAnzahlGNummern.TabIndex = 11;
            this.LblAnzahlGNummern.Text = "Anzahl GNummern";
            this.LblAnzahlGNummern.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblAnzahlGlassorten
            // 
            this.LblAnzahlGlassorten.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblAnzahlGlassorten.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnzahlGlassorten.ForeColor = System.Drawing.Color.FromArgb(31, 55, 100);
            this.LblAnzahlGlassorten.Location = new System.Drawing.Point(807, 46);
            this.LblAnzahlGlassorten.Name = "LblAnzahlGlassorten";
            this.LblAnzahlGlassorten.Size = new System.Drawing.Size(280, 22);
            this.LblAnzahlGlassorten.TabIndex = 12;
            this.LblAnzahlGlassorten.Text = "Anzahl Glassorten";
            this.LblAnzahlGlassorten.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form_GlasWaschDaten
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
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

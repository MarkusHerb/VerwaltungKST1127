namespace VerwaltungKST1127.Auftragsverwaltung
{
    partial class Form_TestansichtInforEinlesen
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
            this.dgvEingleseneInforDaten = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEingleseneInforDaten)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvEingleseneInforDaten
            // 
            this.dgvEingleseneInforDaten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEingleseneInforDaten.Location = new System.Drawing.Point(13, 38);
            this.dgvEingleseneInforDaten.Name = "dgvEingleseneInforDaten";
            this.dgvEingleseneInforDaten.Size = new System.Drawing.Size(1899, 908);
            this.dgvEingleseneInforDaten.TabIndex = 0;
            // 
            // Form_TestansichtInforEinlesen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 958);
            this.Controls.Add(this.dgvEingleseneInforDaten);
            this.Name = "Form_TestansichtInforEinlesen";
            this.Text = "Form_TestansichtInforEinlesen";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEingleseneInforDaten)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvEingleseneInforDaten;
    }
}
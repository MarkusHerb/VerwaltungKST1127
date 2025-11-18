namespace VerwaltungKST1127.Produktionsauswertung
{
    partial class Form_StkVorAvo
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
            this.dGvStkVorKst = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dGvStkVorKst)).BeginInit();
            this.SuspendLayout();
            // 
            // dGvStkVorKst
            // 
            this.dGvStkVorKst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGvStkVorKst.Location = new System.Drawing.Point(13, 59);
            this.dGvStkVorKst.Name = "dGvStkVorKst";
            this.dGvStkVorKst.Size = new System.Drawing.Size(939, 661);
            this.dGvStkVorKst.TabIndex = 0;
            // 
            // Form_StkVorAvo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 732);
            this.Controls.Add(this.dGvStkVorKst);
            this.Name = "Form_StkVorAvo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_StkVorAvo";
            ((System.ComponentModel.ISupportInitialize)(this.dGvStkVorKst)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dGvStkVorKst;
    }
}
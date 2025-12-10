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
            this.lblOffenStk = new System.Windows.Forms.Label();
            this.lblOffen1 = new System.Windows.Forms.Label();
            this.lblOffen2 = new System.Windows.Forms.Label();
            this.lblOffen0 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dGvStkVorKst)).BeginInit();
            this.SuspendLayout();
            // 
            // dGvStkVorKst
            // 
            this.dGvStkVorKst.AllowUserToAddRows = false;
            this.dGvStkVorKst.AllowUserToDeleteRows = false;
            this.dGvStkVorKst.AllowUserToResizeColumns = false;
            this.dGvStkVorKst.AllowUserToResizeRows = false;
            this.dGvStkVorKst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGvStkVorKst.Location = new System.Drawing.Point(12, 59);
            this.dGvStkVorKst.Name = "dGvStkVorKst";
            this.dGvStkVorKst.ReadOnly = true;
            this.dGvStkVorKst.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dGvStkVorKst.RowHeadersVisible = false;
            this.dGvStkVorKst.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dGvStkVorKst.Size = new System.Drawing.Size(1020, 661);
            this.dGvStkVorKst.TabIndex = 0;
            // 
            // lblOffenStk
            // 
            this.lblOffenStk.AutoSize = true;
            this.lblOffenStk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffenStk.Location = new System.Drawing.Point(206, 34);
            this.lblOffenStk.Name = "lblOffenStk";
            this.lblOffenStk.Size = new System.Drawing.Size(86, 13);
            this.lblOffenStk.TabIndex = 1;
            this.lblOffenStk.Text = "Offen gesamt:";
            // 
            // lblOffen1
            // 
            this.lblOffen1.AutoSize = true;
            this.lblOffen1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffen1.Location = new System.Drawing.Point(12, 9);
            this.lblOffen1.Name = "lblOffen1";
            this.lblOffen1.Size = new System.Drawing.Size(86, 13);
            this.lblOffen1.TabIndex = 2;
            this.lblOffen1.Text = "Offen 1-Seite:";
            // 
            // lblOffen2
            // 
            this.lblOffen2.AutoSize = true;
            this.lblOffen2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffen2.Location = new System.Drawing.Point(12, 34);
            this.lblOffen2.Name = "lblOffen2";
            this.lblOffen2.Size = new System.Drawing.Size(82, 13);
            this.lblOffen2.TabIndex = 3;
            this.lblOffen2.Text = "Offen 2-Seite";
            // 
            // lblOffen0
            // 
            this.lblOffen0.AutoSize = true;
            this.lblOffen0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOffen0.Location = new System.Drawing.Point(206, 9);
            this.lblOffen0.Name = "lblOffen0";
            this.lblOffen0.Size = new System.Drawing.Size(86, 13);
            this.lblOffen0.TabIndex = 4;
            this.lblOffen0.Text = "Offen 0-Seite:";
            // 
            // Form_StkVorAvo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 732);
            this.Controls.Add(this.lblOffen0);
            this.Controls.Add(this.lblOffen2);
            this.Controls.Add(this.lblOffen1);
            this.Controls.Add(this.lblOffenStk);
            this.Controls.Add(this.dGvStkVorKst);
            this.Name = "Form_StkVorAvo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stück vor AVO";
            ((System.ComponentModel.ISupportInitialize)(this.dGvStkVorKst)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dGvStkVorKst;
        private System.Windows.Forms.Label lblOffenStk;
        private System.Windows.Forms.Label lblOffen1;
        private System.Windows.Forms.Label lblOffen2;
        private System.Windows.Forms.Label lblOffen0;
    }
}
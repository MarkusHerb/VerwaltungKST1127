namespace VerwaltungKST1127
{
    partial class Form_InputMenge
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.LblEinheit = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnConfirm = new System.Windows.Forms.Button();
            this.TextBoxInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // LblEinheit
            //
            this.LblEinheit.AutoSize = true;
            this.LblEinheit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblEinheit.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.LblEinheit.Location = new System.Drawing.Point(121, 45);
            this.LblEinheit.Name = "LblEinheit";
            this.LblEinheit.Size = new System.Drawing.Size(58, 17);
            this.LblEinheit.TabIndex = 10;
            this.LblEinheit.Text = "Einheit";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Wie viel soll abgezogen werden:";
            //
            // BtnConfirm
            //
            this.BtnConfirm.BackColor = System.Drawing.Color.FromArgb(31, 55, 100);
            this.BtnConfirm.FlatAppearance.BorderSize = 0;
            this.BtnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnConfirm.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnConfirm.ForeColor = System.Drawing.Color.White;
            this.BtnConfirm.Location = new System.Drawing.Point(15, 68);
            this.BtnConfirm.Name = "BtnConfirm";
            this.BtnConfirm.Size = new System.Drawing.Size(92, 27);
            this.BtnConfirm.TabIndex = 8;
            this.BtnConfirm.Text = "Speichern";
            this.BtnConfirm.UseVisualStyleBackColor = false;
            this.BtnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            //
            // TextBoxInput
            //
            this.TextBoxInput.BackColor = System.Drawing.Color.White;
            this.TextBoxInput.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxInput.ForeColor = System.Drawing.Color.FromArgb(27, 42, 66);
            this.TextBoxInput.Location = new System.Drawing.Point(15, 39);
            this.TextBoxInput.Name = "TextBoxInput";
            this.TextBoxInput.Size = new System.Drawing.Size(100, 23);
            this.TextBoxInput.TabIndex = 7;
            this.TextBoxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxInput_KeyDown);
            //
            // Form_InputMenge
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.ClientSize = new System.Drawing.Size(268, 104);
            this.Controls.Add(this.LblEinheit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnConfirm);
            this.Controls.Add(this.TextBoxInput);
            this.MaximumSize = new System.Drawing.Size(284, 143);
            this.MinimumSize = new System.Drawing.Size(284, 143);
            this.Name = "Form_InputMenge";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_InputMenge";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Label LblEinheit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnConfirm;
        private System.Windows.Forms.TextBox TextBoxInput;
    }
}

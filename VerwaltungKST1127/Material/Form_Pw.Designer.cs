namespace VerwaltungKST1127.Material
{
    partial class Form_Pw
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
            this.textBoxPw = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnOk = new System.Windows.Forms.Button();
            this.BtnCancle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxPw
            // 
            this.textBoxPw.Location = new System.Drawing.Point(143, 12);
            this.textBoxPw.Name = "textBoxPw";
            this.textBoxPw.Size = new System.Drawing.Size(156, 20);
            this.textBoxPw.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Passwort eingeben:";
            // 
            // BtnOk
            // 
            this.BtnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.BtnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnOk.Location = new System.Drawing.Point(143, 35);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(156, 23);
            this.BtnOk.TabIndex = 2;
            this.BtnOk.Text = "Bestätigen";
            this.BtnOk.UseVisualStyleBackColor = false;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // BtnCancle
            // 
            this.BtnCancle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.BtnCancle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnCancle.Location = new System.Drawing.Point(143, 60);
            this.BtnCancle.Name = "BtnCancle";
            this.BtnCancle.Size = new System.Drawing.Size(156, 23);
            this.BtnCancle.TabIndex = 3;
            this.BtnCancle.Text = "Kenne das Pw nicht :(";
            this.BtnCancle.UseVisualStyleBackColor = false;
            this.BtnCancle.Click += new System.EventHandler(this.BtnCancle_Click);
            // 
            // Form_Pw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(311, 95);
            this.Controls.Add(this.BtnCancle);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPw);
            this.Name = "Form_Pw";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Passwort";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.Button BtnCancle;
    }
}
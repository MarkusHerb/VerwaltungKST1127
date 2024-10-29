using System.Windows.Forms;

namespace VerwaltungKST1127.Auftragsverwaltung
{
    partial class Form_RLTL
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAddRL = new System.Windows.Forms.Button();
            this.btnRemoveRL = new System.Windows.Forms.Button();
            this.btnRemoveTL = new System.Windows.Forms.Button();
            this.btnAddTL = new System.Windows.Forms.Button();
            this.txtRL = new System.Windows.Forms.TextBox();
            this.txtTL = new System.Windows.Forms.TextBox();
            this.lstRL = new System.Windows.Forms.ListBox();
            this.lstTL = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(57, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rohteillager";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(327, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Teilelager";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(245, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 350);
            this.panel1.TabIndex = 2;
            // 
            // btnAddRL
            // 
            this.btnAddRL.Location = new System.Drawing.Point(12, 60);
            this.btnAddRL.Name = "btnAddRL";
            this.btnAddRL.Size = new System.Drawing.Size(102, 39);
            this.btnAddRL.TabIndex = 3;
            this.btnAddRL.Text = "RL hinzufügen";
            this.btnAddRL.UseVisualStyleBackColor = true;
            this.btnAddRL.Click += new System.EventHandler(this.btnAddRL_Click);
            // 
            // btnRemoveRL
            // 
            this.btnRemoveRL.Location = new System.Drawing.Point(120, 60);
            this.btnRemoveRL.Name = "btnRemoveRL";
            this.btnRemoveRL.Size = new System.Drawing.Size(102, 39);
            this.btnRemoveRL.TabIndex = 4;
            this.btnRemoveRL.Text = "RL löschen";
            this.btnRemoveRL.UseVisualStyleBackColor = true;
            this.btnRemoveRL.Click += new System.EventHandler(this.btnRemoveRL_Click);
            // 
            // btnRemoveTL
            // 
            this.btnRemoveTL.Location = new System.Drawing.Point(375, 60);
            this.btnRemoveTL.Name = "btnRemoveTL";
            this.btnRemoveTL.Size = new System.Drawing.Size(102, 39);
            this.btnRemoveTL.TabIndex = 6;
            this.btnRemoveTL.Text = "TL löschen";
            this.btnRemoveTL.UseVisualStyleBackColor = true;
            this.btnRemoveTL.Click += new System.EventHandler(this.btnRemoveTL_Click);
            // 
            // btnAddTL
            // 
            this.btnAddTL.Location = new System.Drawing.Point(267, 60);
            this.btnAddTL.Name = "btnAddTL";
            this.btnAddTL.Size = new System.Drawing.Size(102, 39);
            this.btnAddTL.TabIndex = 5;
            this.btnAddTL.Text = "TL hinzufügen";
            this.btnAddTL.UseVisualStyleBackColor = true;
            this.btnAddTL.Click += new System.EventHandler(this.btnAddTL_Click);
            // 
            // txtRL
            // 
            this.txtRL.Location = new System.Drawing.Point(13, 106);
            this.txtRL.Name = "txtRL";
            this.txtRL.Size = new System.Drawing.Size(101, 20);
            this.txtRL.TabIndex = 7;
            // 
            // txtTL
            // 
            this.txtTL.Location = new System.Drawing.Point(268, 106);
            this.txtTL.Name = "txtTL";
            this.txtTL.Size = new System.Drawing.Size(101, 20);
            this.txtTL.TabIndex = 8;
            // 
            // lstRL
            // 
            this.lstRL.FormattingEnabled = true;
            this.lstRL.Location = new System.Drawing.Point(13, 133);
            this.lstRL.Name = "lstRL";
            this.lstRL.Size = new System.Drawing.Size(209, 303);
            this.lstRL.TabIndex = 9;
            // 
            // lstTL
            // 
            this.lstTL.FormattingEnabled = true;
            this.lstTL.Location = new System.Drawing.Point(267, 133);
            this.lstTL.Name = "lstTL";
            this.lstTL.Size = new System.Drawing.Size(210, 303);
            this.lstTL.TabIndex = 10;
            // 
            // Form_RLTL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 450);
            this.Controls.Add(this.lstTL);
            this.Controls.Add(this.lstRL);
            this.Controls.Add(this.txtTL);
            this.Controls.Add(this.txtRL);
            this.Controls.Add(this.btnRemoveTL);
            this.Controls.Add(this.btnAddTL);
            this.Controls.Add(this.btnRemoveRL);
            this.Controls.Add(this.btnAddRL);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form_RLTL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form_RLTL";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_RLTL_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddRL;
        private System.Windows.Forms.Button btnRemoveRL;
        private System.Windows.Forms.Button btnRemoveTL;
        private System.Windows.Forms.Button btnAddTL;
        private System.Windows.Forms.TextBox txtRL;
        private System.Windows.Forms.TextBox txtTL;
        private System.Windows.Forms.ListBox lstRL;
        private System.Windows.Forms.ListBox lstTL;
    }
}
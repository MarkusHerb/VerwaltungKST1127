namespace VerwaltungKST1127.Farbauswertung
{
    partial class Form_Monatsuebersicht
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.BtnDrucken = new System.Windows.Forms.Button();
            this.ChartMonatsuebersicht = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ChartBelagsuebersicht = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.LblAverage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ChartMonatsuebersicht)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartBelagsuebersicht)).BeginInit();
            this.SuspendLayout();
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // BtnDrucken
            // 
            this.BtnDrucken.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BtnDrucken.Location = new System.Drawing.Point(1123, 12);
            this.BtnDrucken.Name = "BtnDrucken";
            this.BtnDrucken.Size = new System.Drawing.Size(127, 41);
            this.BtnDrucken.TabIndex = 10;
            this.BtnDrucken.Text = "Drucken";
            this.BtnDrucken.UseVisualStyleBackColor = false;
            this.BtnDrucken.Click += new System.EventHandler(this.BtnDrucken_Click);
            // 
            // ChartMonatsuebersicht
            // 
            chartArea1.Name = "ChartArea1";
            this.ChartMonatsuebersicht.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ChartMonatsuebersicht.Legends.Add(legend1);
            this.ChartMonatsuebersicht.Location = new System.Drawing.Point(17, 59);
            this.ChartMonatsuebersicht.Name = "ChartMonatsuebersicht";
            this.ChartMonatsuebersicht.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.ChartMonatsuebersicht.Series.Add(series1);
            this.ChartMonatsuebersicht.Size = new System.Drawing.Size(1233, 382);
            this.ChartMonatsuebersicht.TabIndex = 11;
            this.ChartMonatsuebersicht.Text = "chart1";
            // 
            // ChartBelagsuebersicht
            // 
            chartArea2.Name = "ChartArea1";
            this.ChartBelagsuebersicht.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.ChartBelagsuebersicht.Legends.Add(legend2);
            this.ChartBelagsuebersicht.Location = new System.Drawing.Point(13, 499);
            this.ChartBelagsuebersicht.Name = "ChartBelagsuebersicht";
            this.ChartBelagsuebersicht.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.ChartBelagsuebersicht.Series.Add(series2);
            this.ChartBelagsuebersicht.Size = new System.Drawing.Size(1237, 346);
            this.ChartBelagsuebersicht.TabIndex = 12;
            this.ChartBelagsuebersicht.Text = "chart2";
            // 
            // LblAverage
            // 
            this.LblAverage.AutoSize = true;
            this.LblAverage.BackColor = System.Drawing.Color.White;
            this.LblAverage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAverage.ForeColor = System.Drawing.Color.Black;
            this.LblAverage.Location = new System.Drawing.Point(515, 12);
            this.LblAverage.Name = "LblAverage";
            this.LblAverage.Size = new System.Drawing.Size(301, 25);
            this.LblAverage.TabIndex = 13;
            this.LblAverage.Text = "Übersicht der Chargen pro Monat";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 25);
            this.label1.TabIndex = 14;
            this.label1.Text = "Übersicht der Chargen pro Monat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(8, 471);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "Anzahl, wie oft ein Belag gemessen wurde";
            // 
            // Form_Monatsuebersicht
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1262, 857);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblAverage);
            this.Controls.Add(this.ChartBelagsuebersicht);
            this.Controls.Add(this.ChartMonatsuebersicht);
            this.Controls.Add(this.BtnDrucken);
            this.Name = "Form_Monatsuebersicht";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_Monatsuebersicht";
            ((System.ComponentModel.ISupportInitialize)(this.ChartMonatsuebersicht)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartBelagsuebersicht)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.Button BtnDrucken;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartMonatsuebersicht;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartBelagsuebersicht;
        private System.Windows.Forms.Label LblAverage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
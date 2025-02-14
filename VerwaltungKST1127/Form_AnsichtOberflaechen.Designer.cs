namespace VerwaltungKST1127
{
    partial class Form_AnsichtOberflaechen
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
            this.ChartOberflaechen = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ChartBelaegeAnsicht = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ChartOberflaechen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartBelaegeAnsicht)).BeginInit();
            this.SuspendLayout();
            // 
            // ChartOberflaechen
            // 
            chartArea1.Name = "ChartArea1";
            this.ChartOberflaechen.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ChartOberflaechen.Legends.Add(legend1);
            this.ChartOberflaechen.Location = new System.Drawing.Point(12, 90);
            this.ChartOberflaechen.Name = "ChartOberflaechen";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.ChartOberflaechen.Series.Add(series1);
            this.ChartOberflaechen.Size = new System.Drawing.Size(1574, 430);
            this.ChartOberflaechen.TabIndex = 0;
            this.ChartOberflaechen.Text = "chart1";
            // 
            // ChartBelaegeAnsicht
            // 
            chartArea2.Name = "ChartArea1";
            this.ChartBelaegeAnsicht.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.ChartBelaegeAnsicht.Legends.Add(legend2);
            this.ChartBelaegeAnsicht.Location = new System.Drawing.Point(11, 526);
            this.ChartBelaegeAnsicht.Name = "ChartBelaegeAnsicht";
            this.ChartBelaegeAnsicht.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.ChartBelaegeAnsicht.Series.Add(series2);
            this.ChartBelaegeAnsicht.Size = new System.Drawing.Size(1575, 430);
            this.ChartBelaegeAnsicht.TabIndex = 1;
            this.ChartBelaegeAnsicht.Text = "chart1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(326, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ansicht der Produzierten Oberflächen";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Es werden nur Serienartikel berücksichtig!";
            // 
            // Form_AnsichtOberflaechen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1599, 966);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ChartBelaegeAnsicht);
            this.Controls.Add(this.ChartOberflaechen);
            this.MaximumSize = new System.Drawing.Size(1615, 1005);
            this.MinimumSize = new System.Drawing.Size(1615, 1005);
            this.Name = "Form_AnsichtOberflaechen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gesamtansicht Vergüteter Flächen";
            ((System.ComponentModel.ISupportInitialize)(this.ChartOberflaechen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartBelaegeAnsicht)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart ChartOberflaechen;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartBelaegeAnsicht;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
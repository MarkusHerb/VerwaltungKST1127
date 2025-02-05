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
            this.ChartOberflaechen = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.ChartOberflaechen)).BeginInit();
            this.SuspendLayout();
            // 
            // ChartOberflaechen
            // 
            chartArea1.Name = "ChartArea1";
            this.ChartOberflaechen.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.ChartOberflaechen.Legends.Add(legend1);
            this.ChartOberflaechen.Location = new System.Drawing.Point(13, 12);
            this.ChartOberflaechen.Name = "ChartOberflaechen";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.ChartOberflaechen.Series.Add(series1);
            this.ChartOberflaechen.Size = new System.Drawing.Size(1346, 430);
            this.ChartOberflaechen.TabIndex = 0;
            this.ChartOberflaechen.Text = "chart1";
            // 
            // Form_AnsichtOberflaechen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1371, 454);
            this.Controls.Add(this.ChartOberflaechen);
            this.MaximumSize = new System.Drawing.Size(1387, 493);
            this.MinimumSize = new System.Drawing.Size(1387, 493);
            this.Name = "Form_AnsichtOberflaechen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gesamtansicht Vergüteter Flächen";
            ((System.ComponentModel.ISupportInitialize)(this.ChartOberflaechen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart ChartOberflaechen;
    }
}
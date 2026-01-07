namespace VerwaltungKST1127.Auftragsverwaltung
{
    partial class Form_Rueckstand
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
            this.dGv_AuswahlBelag = new System.Windows.Forms.DataGridView();
            this.dateTimePickerRueckstandAb = new System.Windows.Forms.DateTimePicker();
            this.chartVorbereiten = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartVergueten = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lbl1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVorbereiten = new System.Windows.Forms.Label();
            this.lblVergueten = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblGesamt = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dGv_AuswahlBelag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVorbereiten)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVergueten)).BeginInit();
            this.SuspendLayout();
            // 
            // dGv_AuswahlBelag
            // 
            this.dGv_AuswahlBelag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGv_AuswahlBelag.Location = new System.Drawing.Point(13, 42);
            this.dGv_AuswahlBelag.Name = "dGv_AuswahlBelag";
            this.dGv_AuswahlBelag.Size = new System.Drawing.Size(161, 628);
            this.dGv_AuswahlBelag.TabIndex = 0;
            // 
            // dateTimePickerRueckstandAb
            // 
            this.dateTimePickerRueckstandAb.Location = new System.Drawing.Point(181, 42);
            this.dateTimePickerRueckstandAb.Name = "dateTimePickerRueckstandAb";
            this.dateTimePickerRueckstandAb.Size = new System.Drawing.Size(195, 20);
            this.dateTimePickerRueckstandAb.TabIndex = 1;
            // 
            // chartVorbereiten
            // 
            chartArea1.Name = "ChartArea1";
            this.chartVorbereiten.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartVorbereiten.Legends.Add(legend1);
            this.chartVorbereiten.Location = new System.Drawing.Point(180, 112);
            this.chartVorbereiten.Name = "chartVorbereiten";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartVorbereiten.Series.Add(series1);
            this.chartVorbereiten.Size = new System.Drawing.Size(911, 248);
            this.chartVorbereiten.TabIndex = 2;
            this.chartVorbereiten.Text = "chart1";
            // 
            // chartVergueten
            // 
            chartArea2.Name = "ChartArea1";
            this.chartVergueten.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartVergueten.Legends.Add(legend2);
            this.chartVergueten.Location = new System.Drawing.Point(180, 422);
            this.chartVergueten.Name = "chartVergueten";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartVergueten.Series.Add(series2);
            this.chartVergueten.Size = new System.Drawing.Size(911, 248);
            this.chartVergueten.TabIndex = 3;
            this.chartVergueten.Text = "chart2";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1.Location = new System.Drawing.Point(181, 85);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(207, 24);
            this.lbl1.TabIndex = 4;
            this.lbl1.Text = "Rückstand Vorbereiten:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(181, 395);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "Rückstand Vergüten:";
            // 
            // lblVorbereiten
            // 
            this.lblVorbereiten.AutoSize = true;
            this.lblVorbereiten.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVorbereiten.Location = new System.Drawing.Point(394, 85);
            this.lblVorbereiten.Name = "lblVorbereiten";
            this.lblVorbereiten.Size = new System.Drawing.Size(56, 24);
            this.lblVorbereiten.TabIndex = 6;
            this.lblVorbereiten.Text = "0.00h";
            // 
            // lblVergueten
            // 
            this.lblVergueten.AutoSize = true;
            this.lblVergueten.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVergueten.Location = new System.Drawing.Point(394, 395);
            this.lblVergueten.Name = "lblVergueten";
            this.lblVergueten.Size = new System.Drawing.Size(56, 24);
            this.lblVergueten.TabIndex = 7;
            this.lblVergueten.Text = "0.00h";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(593, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 24);
            this.label2.TabIndex = 8;
            this.label2.Text = "Rückstand gesamt:";
            // 
            // lblGesamt
            // 
            this.lblGesamt.AutoSize = true;
            this.lblGesamt.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGesamt.Location = new System.Drawing.Point(768, 42);
            this.lblGesamt.Name = "lblGesamt";
            this.lblGesamt.Size = new System.Drawing.Size(56, 24);
            this.lblGesamt.TabIndex = 9;
            this.lblGesamt.Text = "0.00h";
            // 
            // Form_Rueckstand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 684);
            this.Controls.Add(this.lblGesamt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblVergueten);
            this.Controls.Add(this.lblVorbereiten);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.chartVergueten);
            this.Controls.Add(this.chartVorbereiten);
            this.Controls.Add(this.dateTimePickerRueckstandAb);
            this.Controls.Add(this.dGv_AuswahlBelag);
            this.Name = "Form_Rueckstand";
            this.Text = "Form_Rueckstand";
            ((System.ComponentModel.ISupportInitialize)(this.dGv_AuswahlBelag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVorbereiten)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVergueten)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dGv_AuswahlBelag;
        private System.Windows.Forms.DateTimePicker dateTimePickerRueckstandAb;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVorbereiten;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVergueten;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVorbereiten;
        private System.Windows.Forms.Label lblVergueten;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblGesamt;
    }
}
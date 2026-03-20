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
            this.chartVergueten = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVergueten = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblGesamt = new System.Windows.Forms.Label();
            this.chartPlusMinusTwoMonths = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRueckstandPlusMinusFourtyDays = new System.Windows.Forms.Label();
            this.lblGesamtRueckstandAbteilung = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnEineWocheZurueck = new System.Windows.Forms.Button();
            this.btnEineWocheVor = new System.Windows.Forms.Button();
            this.btnZweiWochenZurueck = new System.Windows.Forms.Button();
            this.btnZweiWochenVor = new System.Windows.Forms.Button();
            this.btnResett = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dGv_AuswahlBelag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVergueten)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPlusMinusTwoMonths)).BeginInit();
            this.SuspendLayout();
            // 
            // dGv_AuswahlBelag
            // 
            this.dGv_AuswahlBelag.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGv_AuswahlBelag.Location = new System.Drawing.Point(12, 12);
            this.dGv_AuswahlBelag.Name = "dGv_AuswahlBelag";
            this.dGv_AuswahlBelag.Size = new System.Drawing.Size(271, 628);
            this.dGv_AuswahlBelag.TabIndex = 0;
            // 
            // dateTimePickerRueckstandAb
            // 
            this.dateTimePickerRueckstandAb.Location = new System.Drawing.Point(294, 44);
            this.dateTimePickerRueckstandAb.Name = "dateTimePickerRueckstandAb";
            this.dateTimePickerRueckstandAb.Size = new System.Drawing.Size(195, 20);
            this.dateTimePickerRueckstandAb.TabIndex = 1;
            // 
            // chartVergueten
            // 
            chartArea1.Name = "ChartArea1";
            this.chartVergueten.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartVergueten.Legends.Add(legend1);
            this.chartVergueten.Location = new System.Drawing.Point(289, 147);
            this.chartVergueten.Name = "chartVergueten";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartVergueten.Series.Add(series1);
            this.chartVergueten.Size = new System.Drawing.Size(1180, 248);
            this.chartVergueten.TabIndex = 3;
            this.chartVergueten.Text = "chart2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(289, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(273, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "Rückstand ausgewählter Belag:";
            // 
            // lblVergueten
            // 
            this.lblVergueten.AutoSize = true;
            this.lblVergueten.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVergueten.Location = new System.Drawing.Point(568, 120);
            this.lblVergueten.Name = "lblVergueten";
            this.lblVergueten.Size = new System.Drawing.Size(56, 24);
            this.lblVergueten.TabIndex = 7;
            this.lblVergueten.Text = "0.00h";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(289, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 26);
            this.label2.TabIndex = 8;
            this.label2.Text = "Rückstand ausgewählt:";
            // 
            // lblGesamt
            // 
            this.lblGesamt.AutoSize = true;
            this.lblGesamt.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGesamt.Location = new System.Drawing.Point(532, 67);
            this.lblGesamt.Name = "lblGesamt";
            this.lblGesamt.Size = new System.Drawing.Size(66, 26);
            this.lblGesamt.TabIndex = 9;
            this.lblGesamt.Text = "0.00h";
            // 
            // chartPlusMinusTwoMonths
            // 
            chartArea2.Name = "ChartArea1";
            this.chartPlusMinusTwoMonths.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartPlusMinusTwoMonths.Legends.Add(legend2);
            this.chartPlusMinusTwoMonths.Location = new System.Drawing.Point(289, 428);
            this.chartPlusMinusTwoMonths.Name = "chartPlusMinusTwoMonths";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartPlusMinusTwoMonths.Series.Add(series2);
            this.chartPlusMinusTwoMonths.Size = new System.Drawing.Size(1180, 397);
            this.chartPlusMinusTwoMonths.TabIndex = 10;
            this.chartPlusMinusTwoMonths.Text = "chart2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(289, 401);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(204, 24);
            this.label3.TabIndex = 11;
            this.label3.Text = "Rückstand +/- 40 Tage:";
            // 
            // lblRueckstandPlusMinusFourtyDays
            // 
            this.lblRueckstandPlusMinusFourtyDays.AutoSize = true;
            this.lblRueckstandPlusMinusFourtyDays.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRueckstandPlusMinusFourtyDays.Location = new System.Drawing.Point(515, 401);
            this.lblRueckstandPlusMinusFourtyDays.Name = "lblRueckstandPlusMinusFourtyDays";
            this.lblRueckstandPlusMinusFourtyDays.Size = new System.Drawing.Size(61, 24);
            this.lblRueckstandPlusMinusFourtyDays.TabIndex = 12;
            this.lblRueckstandPlusMinusFourtyDays.Text = "0.00 h";
            // 
            // lblGesamtRueckstandAbteilung
            // 
            this.lblGesamtRueckstandAbteilung.AutoSize = true;
            this.lblGesamtRueckstandAbteilung.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGesamtRueckstandAbteilung.Location = new System.Drawing.Point(210, 643);
            this.lblGesamtRueckstandAbteilung.Name = "lblGesamtRueckstandAbteilung";
            this.lblGesamtRueckstandAbteilung.Size = new System.Drawing.Size(73, 17);
            this.lblGesamtRueckstandAbteilung.TabIndex = 13;
            this.lblGesamtRueckstandAbteilung.Text = "Soll Brutto";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(291, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(345, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Wenn Datum ausgewählt: Vergangenheit - Heute oder Heute - Zukunft ";
            // 
            // btnEineWocheZurueck
            // 
            this.btnEineWocheZurueck.BackColor = System.Drawing.SystemColors.Info;
            this.btnEineWocheZurueck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEineWocheZurueck.Location = new System.Drawing.Point(685, 28);
            this.btnEineWocheZurueck.Name = "btnEineWocheZurueck";
            this.btnEineWocheZurueck.Size = new System.Drawing.Size(75, 23);
            this.btnEineWocheZurueck.TabIndex = 15;
            this.btnEineWocheZurueck.Text = "-1 Woche";
            this.btnEineWocheZurueck.UseVisualStyleBackColor = false;
            this.btnEineWocheZurueck.Click += new System.EventHandler(this.btnEineWocheZurueck_Click);
            // 
            // btnEineWocheVor
            // 
            this.btnEineWocheVor.BackColor = System.Drawing.SystemColors.Info;
            this.btnEineWocheVor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEineWocheVor.Location = new System.Drawing.Point(766, 28);
            this.btnEineWocheVor.Name = "btnEineWocheVor";
            this.btnEineWocheVor.Size = new System.Drawing.Size(75, 23);
            this.btnEineWocheVor.TabIndex = 16;
            this.btnEineWocheVor.Text = "+1 Woche";
            this.btnEineWocheVor.UseVisualStyleBackColor = false;
            this.btnEineWocheVor.Click += new System.EventHandler(this.btnEineWocheVor_Click);
            // 
            // btnZweiWochenZurueck
            // 
            this.btnZweiWochenZurueck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnZweiWochenZurueck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZweiWochenZurueck.Location = new System.Drawing.Point(685, 57);
            this.btnZweiWochenZurueck.Name = "btnZweiWochenZurueck";
            this.btnZweiWochenZurueck.Size = new System.Drawing.Size(75, 23);
            this.btnZweiWochenZurueck.TabIndex = 17;
            this.btnZweiWochenZurueck.Text = "-2 Woche";
            this.btnZweiWochenZurueck.UseVisualStyleBackColor = false;
            this.btnZweiWochenZurueck.Click += new System.EventHandler(this.btnZweiWochenZurueck_Click);
            // 
            // btnZweiWochenVor
            // 
            this.btnZweiWochenVor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnZweiWochenVor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZweiWochenVor.Location = new System.Drawing.Point(766, 57);
            this.btnZweiWochenVor.Name = "btnZweiWochenVor";
            this.btnZweiWochenVor.Size = new System.Drawing.Size(75, 23);
            this.btnZweiWochenVor.TabIndex = 18;
            this.btnZweiWochenVor.Text = "+2 Woche";
            this.btnZweiWochenVor.UseVisualStyleBackColor = false;
            this.btnZweiWochenVor.Click += new System.EventHandler(this.btnZweiWochenVor_Click);
            // 
            // btnResett
            // 
            this.btnResett.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnResett.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResett.Location = new System.Drawing.Point(847, 28);
            this.btnResett.Name = "btnResett";
            this.btnResett.Size = new System.Drawing.Size(75, 52);
            this.btnResett.TabIndex = 19;
            this.btnResett.Text = "Resett";
            this.btnResett.UseVisualStyleBackColor = false;
            this.btnResett.Click += new System.EventHandler(this.btnResett_Click);
            // 
            // Form_Rueckstand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1481, 837);
            this.Controls.Add(this.btnResett);
            this.Controls.Add(this.btnZweiWochenVor);
            this.Controls.Add(this.btnZweiWochenZurueck);
            this.Controls.Add(this.btnEineWocheVor);
            this.Controls.Add(this.btnEineWocheZurueck);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblGesamtRueckstandAbteilung);
            this.Controls.Add(this.lblRueckstandPlusMinusFourtyDays);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chartPlusMinusTwoMonths);
            this.Controls.Add(this.lblGesamt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblVergueten);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chartVergueten);
            this.Controls.Add(this.dateTimePickerRueckstandAb);
            this.Controls.Add(this.dGv_AuswahlBelag);
            this.Name = "Form_Rueckstand";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_Rueckstand";
            ((System.ComponentModel.ISupportInitialize)(this.dGv_AuswahlBelag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVergueten)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPlusMinusTwoMonths)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dGv_AuswahlBelag;
        private System.Windows.Forms.DateTimePicker dateTimePickerRueckstandAb;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVergueten;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVergueten;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblGesamt;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPlusMinusTwoMonths;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblRueckstandPlusMinusFourtyDays;
        private System.Windows.Forms.Label lblGesamtRueckstandAbteilung;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnEineWocheZurueck;
        private System.Windows.Forms.Button btnEineWocheVor;
        private System.Windows.Forms.Button btnZweiWochenZurueck;
        private System.Windows.Forms.Button btnZweiWochenVor;
        private System.Windows.Forms.Button btnResett;
    }
}
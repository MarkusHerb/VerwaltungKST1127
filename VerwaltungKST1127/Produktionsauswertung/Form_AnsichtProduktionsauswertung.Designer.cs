namespace VerwaltungKST1127.Produktionsauswertung
{
    partial class Form_AnsichtProduktionsauswertung
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerLastProduction = new System.Windows.Forms.DateTimePicker();
            this.txtBoxLastTimeA20 = new System.Windows.Forms.TextBox();
            this.txtBoxLastTimeA25 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA30 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA35 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA40 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA45 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA50 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA60 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtBoxLastTimeA65 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBoxProductivA65 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA60 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA50 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA45 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA40 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA35 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA30 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA25 = new System.Windows.Forms.TextBox();
            this.txtBoxProductivA20 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chartGesamtProduktiv = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxChargenA20 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtBoxChargenA25 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA35 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA30 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA45 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA40 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA60 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA50 = new System.Windows.Forms.TextBox();
            this.txtBoxChargenA65 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA65 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA60 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA50 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA45 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA40 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA35 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA30 = new System.Windows.Forms.TextBox();
            this.txtBoxFehlerA25 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtBoxFehlerA20 = new System.Windows.Forms.TextBox();
            this.dgvArtikelStueck = new System.Windows.Forms.DataGridView();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblStueckAusgewaehlt = new System.Windows.Forms.Label();
            this.Btn_InfoProduction = new System.Windows.Forms.Button();
            this.lblProbeA20 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblProbeA25 = new System.Windows.Forms.Label();
            this.lblProbeA30 = new System.Windows.Forms.Label();
            this.lblProbeA35 = new System.Windows.Forms.Label();
            this.lblProbeA40 = new System.Windows.Forms.Label();
            this.lblProbeA45 = new System.Windows.Forms.Label();
            this.lblProbeA50 = new System.Windows.Forms.Label();
            this.lblProbeA60 = new System.Windows.Forms.Label();
            this.lblProbeA65 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblGesamtProduktiv = new System.Windows.Forms.Label();
            this.lblGesamtChargen = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartGesamtProduktiv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArtikelStueck)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Produktionsdaten";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "A20:";
            // 
            // dateTimePickerLastProduction
            // 
            this.dateTimePickerLastProduction.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dateTimePickerLastProduction.Location = new System.Drawing.Point(340, 101);
            this.dateTimePickerLastProduction.Name = "dateTimePickerLastProduction";
            this.dateTimePickerLastProduction.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerLastProduction.TabIndex = 3;
            this.dateTimePickerLastProduction.ValueChanged += new System.EventHandler(this.dateTimePickerLastProduction_ValueChanged);
            // 
            // txtBoxLastTimeA20
            // 
            this.txtBoxLastTimeA20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA20.Location = new System.Drawing.Point(119, 149);
            this.txtBoxLastTimeA20.Name = "txtBoxLastTimeA20";
            this.txtBoxLastTimeA20.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA20.TabIndex = 4;
            this.txtBoxLastTimeA20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxLastTimeA25
            // 
            this.txtBoxLastTimeA25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA25.Location = new System.Drawing.Point(119, 175);
            this.txtBoxLastTimeA25.Name = "txtBoxLastTimeA25";
            this.txtBoxLastTimeA25.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA25.TabIndex = 6;
            this.txtBoxLastTimeA25.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "A25:";
            // 
            // txtBoxLastTimeA30
            // 
            this.txtBoxLastTimeA30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA30.Location = new System.Drawing.Point(119, 201);
            this.txtBoxLastTimeA30.Name = "txtBoxLastTimeA30";
            this.txtBoxLastTimeA30.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA30.TabIndex = 8;
            this.txtBoxLastTimeA30.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "A30:";
            // 
            // txtBoxLastTimeA35
            // 
            this.txtBoxLastTimeA35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA35.Location = new System.Drawing.Point(119, 227);
            this.txtBoxLastTimeA35.Name = "txtBoxLastTimeA35";
            this.txtBoxLastTimeA35.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA35.TabIndex = 10;
            this.txtBoxLastTimeA35.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 227);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 20);
            this.label6.TabIndex = 9;
            this.label6.Text = "A35:";
            // 
            // txtBoxLastTimeA40
            // 
            this.txtBoxLastTimeA40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA40.Location = new System.Drawing.Point(119, 253);
            this.txtBoxLastTimeA40.Name = "txtBoxLastTimeA40";
            this.txtBoxLastTimeA40.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA40.TabIndex = 12;
            this.txtBoxLastTimeA40.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 253);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 20);
            this.label7.TabIndex = 11;
            this.label7.Text = "A40:";
            // 
            // txtBoxLastTimeA45
            // 
            this.txtBoxLastTimeA45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA45.Location = new System.Drawing.Point(119, 279);
            this.txtBoxLastTimeA45.Name = "txtBoxLastTimeA45";
            this.txtBoxLastTimeA45.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA45.TabIndex = 14;
            this.txtBoxLastTimeA45.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 279);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 20);
            this.label8.TabIndex = 13;
            this.label8.Text = "A45:";
            // 
            // txtBoxLastTimeA50
            // 
            this.txtBoxLastTimeA50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA50.Location = new System.Drawing.Point(119, 305);
            this.txtBoxLastTimeA50.Name = "txtBoxLastTimeA50";
            this.txtBoxLastTimeA50.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA50.TabIndex = 16;
            this.txtBoxLastTimeA50.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 305);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 20);
            this.label9.TabIndex = 15;
            this.label9.Text = "A50:";
            // 
            // txtBoxLastTimeA60
            // 
            this.txtBoxLastTimeA60.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA60.Location = new System.Drawing.Point(119, 331);
            this.txtBoxLastTimeA60.Name = "txtBoxLastTimeA60";
            this.txtBoxLastTimeA60.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA60.TabIndex = 18;
            this.txtBoxLastTimeA60.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(15, 331);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 20);
            this.label10.TabIndex = 17;
            this.label10.Text = "A60:";
            // 
            // txtBoxLastTimeA65
            // 
            this.txtBoxLastTimeA65.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxLastTimeA65.Location = new System.Drawing.Point(119, 357);
            this.txtBoxLastTimeA65.Name = "txtBoxLastTimeA65";
            this.txtBoxLastTimeA65.Size = new System.Drawing.Size(100, 20);
            this.txtBoxLastTimeA65.TabIndex = 20;
            this.txtBoxLastTimeA65.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(15, 357);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 20);
            this.label11.TabIndex = 19;
            this.label11.Text = "A65:";
            // 
            // txtBoxProductivA65
            // 
            this.txtBoxProductivA65.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA65.Location = new System.Drawing.Point(225, 357);
            this.txtBoxProductivA65.Name = "txtBoxProductivA65";
            this.txtBoxProductivA65.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA65.TabIndex = 30;
            this.txtBoxProductivA65.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA60
            // 
            this.txtBoxProductivA60.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA60.Location = new System.Drawing.Point(225, 331);
            this.txtBoxProductivA60.Name = "txtBoxProductivA60";
            this.txtBoxProductivA60.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA60.TabIndex = 29;
            this.txtBoxProductivA60.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA50
            // 
            this.txtBoxProductivA50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA50.Location = new System.Drawing.Point(225, 305);
            this.txtBoxProductivA50.Name = "txtBoxProductivA50";
            this.txtBoxProductivA50.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA50.TabIndex = 28;
            this.txtBoxProductivA50.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA45
            // 
            this.txtBoxProductivA45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA45.Location = new System.Drawing.Point(225, 279);
            this.txtBoxProductivA45.Name = "txtBoxProductivA45";
            this.txtBoxProductivA45.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA45.TabIndex = 27;
            this.txtBoxProductivA45.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA40
            // 
            this.txtBoxProductivA40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA40.Location = new System.Drawing.Point(225, 253);
            this.txtBoxProductivA40.Name = "txtBoxProductivA40";
            this.txtBoxProductivA40.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA40.TabIndex = 26;
            this.txtBoxProductivA40.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA35
            // 
            this.txtBoxProductivA35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA35.Location = new System.Drawing.Point(225, 227);
            this.txtBoxProductivA35.Name = "txtBoxProductivA35";
            this.txtBoxProductivA35.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA35.TabIndex = 25;
            this.txtBoxProductivA35.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA30
            // 
            this.txtBoxProductivA30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA30.Location = new System.Drawing.Point(225, 201);
            this.txtBoxProductivA30.Name = "txtBoxProductivA30";
            this.txtBoxProductivA30.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA30.TabIndex = 24;
            this.txtBoxProductivA30.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA25
            // 
            this.txtBoxProductivA25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA25.Location = new System.Drawing.Point(225, 175);
            this.txtBoxProductivA25.Name = "txtBoxProductivA25";
            this.txtBoxProductivA25.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA25.TabIndex = 23;
            this.txtBoxProductivA25.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxProductivA20
            // 
            this.txtBoxProductivA20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxProductivA20.Location = new System.Drawing.Point(225, 149);
            this.txtBoxProductivA20.Name = "txtBoxProductivA20";
            this.txtBoxProductivA20.Size = new System.Drawing.Size(100, 20);
            this.txtBoxProductivA20.TabIndex = 22;
            this.txtBoxProductivA20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(222, 130);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(94, 13);
            this.label12.TabIndex = 31;
            this.label12.Text = "Produktive Zeit";
            // 
            // chartGesamtProduktiv
            // 
            chartArea1.Name = "ChartArea1";
            this.chartGesamtProduktiv.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartGesamtProduktiv.Legends.Add(legend1);
            this.chartGesamtProduktiv.Location = new System.Drawing.Point(19, 453);
            this.chartGesamtProduktiv.Name = "chartGesamtProduktiv";
            this.chartGesamtProduktiv.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartGesamtProduktiv.Series.Add(series1);
            this.chartGesamtProduktiv.Size = new System.Drawing.Size(603, 262);
            this.chartGesamtProduktiv.TabIndex = 32;
            this.chartGesamtProduktiv.Text = "chart1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(116, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Letzte Charge";
            // 
            // txtBoxChargenA20
            // 
            this.txtBoxChargenA20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA20.Location = new System.Drawing.Point(440, 149);
            this.txtBoxChargenA20.Name = "txtBoxChargenA20";
            this.txtBoxChargenA20.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA20.TabIndex = 34;
            this.txtBoxChargenA20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(437, 130);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 35;
            this.label13.Text = "Chargen";
            // 
            // txtBoxChargenA25
            // 
            this.txtBoxChargenA25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA25.Location = new System.Drawing.Point(440, 175);
            this.txtBoxChargenA25.Name = "txtBoxChargenA25";
            this.txtBoxChargenA25.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA25.TabIndex = 36;
            this.txtBoxChargenA25.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA35
            // 
            this.txtBoxChargenA35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA35.Location = new System.Drawing.Point(440, 227);
            this.txtBoxChargenA35.Name = "txtBoxChargenA35";
            this.txtBoxChargenA35.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA35.TabIndex = 38;
            this.txtBoxChargenA35.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA30
            // 
            this.txtBoxChargenA30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA30.Location = new System.Drawing.Point(440, 201);
            this.txtBoxChargenA30.Name = "txtBoxChargenA30";
            this.txtBoxChargenA30.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA30.TabIndex = 37;
            this.txtBoxChargenA30.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA45
            // 
            this.txtBoxChargenA45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA45.Location = new System.Drawing.Point(440, 279);
            this.txtBoxChargenA45.Name = "txtBoxChargenA45";
            this.txtBoxChargenA45.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA45.TabIndex = 40;
            this.txtBoxChargenA45.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA40
            // 
            this.txtBoxChargenA40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA40.Location = new System.Drawing.Point(440, 253);
            this.txtBoxChargenA40.Name = "txtBoxChargenA40";
            this.txtBoxChargenA40.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA40.TabIndex = 39;
            this.txtBoxChargenA40.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA60
            // 
            this.txtBoxChargenA60.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA60.Location = new System.Drawing.Point(440, 331);
            this.txtBoxChargenA60.Name = "txtBoxChargenA60";
            this.txtBoxChargenA60.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA60.TabIndex = 42;
            this.txtBoxChargenA60.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA50
            // 
            this.txtBoxChargenA50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA50.Location = new System.Drawing.Point(440, 305);
            this.txtBoxChargenA50.Name = "txtBoxChargenA50";
            this.txtBoxChargenA50.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA50.TabIndex = 41;
            this.txtBoxChargenA50.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxChargenA65
            // 
            this.txtBoxChargenA65.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxChargenA65.Location = new System.Drawing.Point(440, 357);
            this.txtBoxChargenA65.Name = "txtBoxChargenA65";
            this.txtBoxChargenA65.Size = new System.Drawing.Size(100, 20);
            this.txtBoxChargenA65.TabIndex = 43;
            this.txtBoxChargenA65.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA65
            // 
            this.txtBoxFehlerA65.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA65.Location = new System.Drawing.Point(334, 357);
            this.txtBoxFehlerA65.Name = "txtBoxFehlerA65";
            this.txtBoxFehlerA65.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA65.TabIndex = 53;
            this.txtBoxFehlerA65.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA60
            // 
            this.txtBoxFehlerA60.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA60.Location = new System.Drawing.Point(334, 331);
            this.txtBoxFehlerA60.Name = "txtBoxFehlerA60";
            this.txtBoxFehlerA60.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA60.TabIndex = 52;
            this.txtBoxFehlerA60.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA50
            // 
            this.txtBoxFehlerA50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA50.Location = new System.Drawing.Point(334, 305);
            this.txtBoxFehlerA50.Name = "txtBoxFehlerA50";
            this.txtBoxFehlerA50.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA50.TabIndex = 51;
            this.txtBoxFehlerA50.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA45
            // 
            this.txtBoxFehlerA45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA45.Location = new System.Drawing.Point(334, 279);
            this.txtBoxFehlerA45.Name = "txtBoxFehlerA45";
            this.txtBoxFehlerA45.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA45.TabIndex = 50;
            this.txtBoxFehlerA45.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA40
            // 
            this.txtBoxFehlerA40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA40.Location = new System.Drawing.Point(334, 253);
            this.txtBoxFehlerA40.Name = "txtBoxFehlerA40";
            this.txtBoxFehlerA40.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA40.TabIndex = 49;
            this.txtBoxFehlerA40.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA35
            // 
            this.txtBoxFehlerA35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA35.Location = new System.Drawing.Point(334, 227);
            this.txtBoxFehlerA35.Name = "txtBoxFehlerA35";
            this.txtBoxFehlerA35.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA35.TabIndex = 48;
            this.txtBoxFehlerA35.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA30
            // 
            this.txtBoxFehlerA30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA30.Location = new System.Drawing.Point(334, 201);
            this.txtBoxFehlerA30.Name = "txtBoxFehlerA30";
            this.txtBoxFehlerA30.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA30.TabIndex = 47;
            this.txtBoxFehlerA30.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBoxFehlerA25
            // 
            this.txtBoxFehlerA25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA25.Location = new System.Drawing.Point(334, 175);
            this.txtBoxFehlerA25.Name = "txtBoxFehlerA25";
            this.txtBoxFehlerA25.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA25.TabIndex = 46;
            this.txtBoxFehlerA25.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(331, 130);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 45;
            this.label14.Text = "Fehlerzeit";
            // 
            // txtBoxFehlerA20
            // 
            this.txtBoxFehlerA20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxFehlerA20.Location = new System.Drawing.Point(334, 149);
            this.txtBoxFehlerA20.Name = "txtBoxFehlerA20";
            this.txtBoxFehlerA20.Size = new System.Drawing.Size(100, 20);
            this.txtBoxFehlerA20.TabIndex = 44;
            this.txtBoxFehlerA20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dgvArtikelStueck
            // 
            this.dgvArtikelStueck.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvArtikelStueck.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvArtikelStueck.Location = new System.Drawing.Point(643, 130);
            this.dgvArtikelStueck.Name = "dgvArtikelStueck";
            this.dgvArtikelStueck.RowHeadersVisible = false;
            this.dgvArtikelStueck.Size = new System.Drawing.Size(521, 585);
            this.dgvArtikelStueck.TabIndex = 54;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(12, 97);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(271, 24);
            this.label15.TabIndex = 55;
            this.label15.Text = "Tagesauswertung Produktivzeit";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(639, 97);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(184, 24);
            this.label16.TabIndex = 56;
            this.label16.Text = "Artikel + Stückzahlen";
            // 
            // lblStueckAusgewaehlt
            // 
            this.lblStueckAusgewaehlt.AutoSize = true;
            this.lblStueckAusgewaehlt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStueckAusgewaehlt.Location = new System.Drawing.Point(1043, 114);
            this.lblStueckAusgewaehlt.Name = "lblStueckAusgewaehlt";
            this.lblStueckAusgewaehlt.Size = new System.Drawing.Size(53, 13);
            this.lblStueckAusgewaehlt.TabIndex = 57;
            this.lblStueckAusgewaehlt.Text = "Gesamt:";
            // 
            // Btn_InfoProduction
            // 
            this.Btn_InfoProduction.BackColor = System.Drawing.SystemColors.Info;
            this.Btn_InfoProduction.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Btn_InfoProduction.Location = new System.Drawing.Point(1078, 9);
            this.Btn_InfoProduction.Name = "Btn_InfoProduction";
            this.Btn_InfoProduction.Size = new System.Drawing.Size(86, 31);
            this.Btn_InfoProduction.TabIndex = 58;
            this.Btn_InfoProduction.Text = "Information";
            this.Btn_InfoProduction.UseVisualStyleBackColor = false;
            this.Btn_InfoProduction.Click += new System.EventHandler(this.Btn_InfoProduction_Click);
            // 
            // lblProbeA20
            // 
            this.lblProbeA20.AutoSize = true;
            this.lblProbeA20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA20.Location = new System.Drawing.Point(541, 154);
            this.lblProbeA20.Name = "lblProbeA20";
            this.lblProbeA20.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA20.TabIndex = 59;
            this.lblProbeA20.Text = "ProbeA20";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(541, 130);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 13);
            this.label18.TabIndex = 60;
            this.label18.Text = "Proben";
            // 
            // lblProbeA25
            // 
            this.lblProbeA25.AutoSize = true;
            this.lblProbeA25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA25.Location = new System.Drawing.Point(541, 178);
            this.lblProbeA25.Name = "lblProbeA25";
            this.lblProbeA25.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA25.TabIndex = 61;
            this.lblProbeA25.Text = "ProbeA25";
            // 
            // lblProbeA30
            // 
            this.lblProbeA30.AutoSize = true;
            this.lblProbeA30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA30.Location = new System.Drawing.Point(541, 204);
            this.lblProbeA30.Name = "lblProbeA30";
            this.lblProbeA30.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA30.TabIndex = 62;
            this.lblProbeA30.Text = "ProbeA30";
            // 
            // lblProbeA35
            // 
            this.lblProbeA35.AutoSize = true;
            this.lblProbeA35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA35.Location = new System.Drawing.Point(541, 230);
            this.lblProbeA35.Name = "lblProbeA35";
            this.lblProbeA35.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA35.TabIndex = 63;
            this.lblProbeA35.Text = "ProbeA35";
            // 
            // lblProbeA40
            // 
            this.lblProbeA40.AutoSize = true;
            this.lblProbeA40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA40.Location = new System.Drawing.Point(541, 256);
            this.lblProbeA40.Name = "lblProbeA40";
            this.lblProbeA40.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA40.TabIndex = 64;
            this.lblProbeA40.Text = "ProbeA40";
            // 
            // lblProbeA45
            // 
            this.lblProbeA45.AutoSize = true;
            this.lblProbeA45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA45.Location = new System.Drawing.Point(541, 282);
            this.lblProbeA45.Name = "lblProbeA45";
            this.lblProbeA45.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA45.TabIndex = 65;
            this.lblProbeA45.Text = "ProbeA45";
            // 
            // lblProbeA50
            // 
            this.lblProbeA50.AutoSize = true;
            this.lblProbeA50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA50.Location = new System.Drawing.Point(541, 308);
            this.lblProbeA50.Name = "lblProbeA50";
            this.lblProbeA50.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA50.TabIndex = 66;
            this.lblProbeA50.Text = "ProbeA50";
            // 
            // lblProbeA60
            // 
            this.lblProbeA60.AutoSize = true;
            this.lblProbeA60.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA60.Location = new System.Drawing.Point(541, 334);
            this.lblProbeA60.Name = "lblProbeA60";
            this.lblProbeA60.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA60.TabIndex = 67;
            this.lblProbeA60.Text = "ProbeA60";
            // 
            // lblProbeA65
            // 
            this.lblProbeA65.AutoSize = true;
            this.lblProbeA65.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProbeA65.Location = new System.Drawing.Point(541, 360);
            this.lblProbeA65.Name = "lblProbeA65";
            this.lblProbeA65.Size = new System.Drawing.Size(62, 13);
            this.lblProbeA65.TabIndex = 68;
            this.lblProbeA65.Text = "ProbeA65";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(95, 401);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(124, 17);
            this.label17.TabIndex = 69;
            this.label17.Text = "Gesamt Produktiv:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(63, 418);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(156, 17);
            this.label19.TabIndex = 70;
            this.label19.Text = "Chargen, ohne Proben:";
            // 
            // lblGesamtProduktiv
            // 
            this.lblGesamtProduktiv.AutoSize = true;
            this.lblGesamtProduktiv.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGesamtProduktiv.Location = new System.Drawing.Point(225, 401);
            this.lblGesamtProduktiv.Name = "lblGesamtProduktiv";
            this.lblGesamtProduktiv.Size = new System.Drawing.Size(17, 17);
            this.lblGesamtProduktiv.TabIndex = 71;
            this.lblGesamtProduktiv.Text = "0";
            // 
            // lblGesamtChargen
            // 
            this.lblGesamtChargen.AutoSize = true;
            this.lblGesamtChargen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGesamtChargen.Location = new System.Drawing.Point(225, 418);
            this.lblGesamtChargen.Name = "lblGesamtChargen";
            this.lblGesamtChargen.Size = new System.Drawing.Size(17, 17);
            this.lblGesamtChargen.TabIndex = 72;
            this.lblGesamtChargen.Text = "0";
            // 
            // Form_AnsichtProduktionsauswertung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 727);
            this.Controls.Add(this.lblGesamtChargen);
            this.Controls.Add(this.lblGesamtProduktiv);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lblProbeA65);
            this.Controls.Add(this.lblProbeA60);
            this.Controls.Add(this.lblProbeA50);
            this.Controls.Add(this.lblProbeA45);
            this.Controls.Add(this.lblProbeA40);
            this.Controls.Add(this.lblProbeA35);
            this.Controls.Add(this.lblProbeA30);
            this.Controls.Add(this.lblProbeA25);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.lblProbeA20);
            this.Controls.Add(this.Btn_InfoProduction);
            this.Controls.Add(this.lblStueckAusgewaehlt);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.dgvArtikelStueck);
            this.Controls.Add(this.txtBoxFehlerA65);
            this.Controls.Add(this.txtBoxFehlerA60);
            this.Controls.Add(this.txtBoxFehlerA50);
            this.Controls.Add(this.txtBoxFehlerA45);
            this.Controls.Add(this.txtBoxFehlerA40);
            this.Controls.Add(this.txtBoxFehlerA35);
            this.Controls.Add(this.txtBoxFehlerA30);
            this.Controls.Add(this.txtBoxFehlerA25);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtBoxFehlerA20);
            this.Controls.Add(this.txtBoxChargenA65);
            this.Controls.Add(this.txtBoxChargenA60);
            this.Controls.Add(this.txtBoxChargenA50);
            this.Controls.Add(this.txtBoxChargenA45);
            this.Controls.Add(this.txtBoxChargenA40);
            this.Controls.Add(this.txtBoxChargenA35);
            this.Controls.Add(this.txtBoxChargenA30);
            this.Controls.Add(this.txtBoxChargenA25);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtBoxChargenA20);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chartGesamtProduktiv);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtBoxProductivA65);
            this.Controls.Add(this.txtBoxProductivA60);
            this.Controls.Add(this.txtBoxProductivA50);
            this.Controls.Add(this.txtBoxProductivA45);
            this.Controls.Add(this.txtBoxProductivA40);
            this.Controls.Add(this.txtBoxProductivA35);
            this.Controls.Add(this.txtBoxProductivA30);
            this.Controls.Add(this.txtBoxProductivA25);
            this.Controls.Add(this.txtBoxProductivA20);
            this.Controls.Add(this.txtBoxLastTimeA65);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtBoxLastTimeA60);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtBoxLastTimeA50);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtBoxLastTimeA45);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtBoxLastTimeA40);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtBoxLastTimeA35);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtBoxLastTimeA30);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBoxLastTimeA25);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBoxLastTimeA20);
            this.Controls.Add(this.dateTimePickerLastProduction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "Form_AnsichtProduktionsauswertung";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_AnsichtProduktionsauswertung";
            ((System.ComponentModel.ISupportInitialize)(this.chartGesamtProduktiv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArtikelStueck)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePickerLastProduction;
        private System.Windows.Forms.TextBox txtBoxLastTimeA20;
        private System.Windows.Forms.TextBox txtBoxLastTimeA25;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBoxLastTimeA30;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBoxLastTimeA35;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBoxLastTimeA40;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBoxLastTimeA45;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBoxLastTimeA50;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBoxLastTimeA60;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtBoxLastTimeA65;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtBoxProductivA65;
        private System.Windows.Forms.TextBox txtBoxProductivA60;
        private System.Windows.Forms.TextBox txtBoxProductivA50;
        private System.Windows.Forms.TextBox txtBoxProductivA45;
        private System.Windows.Forms.TextBox txtBoxProductivA40;
        private System.Windows.Forms.TextBox txtBoxProductivA35;
        private System.Windows.Forms.TextBox txtBoxProductivA30;
        private System.Windows.Forms.TextBox txtBoxProductivA25;
        private System.Windows.Forms.TextBox txtBoxProductivA20;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGesamtProduktiv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxChargenA20;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtBoxChargenA25;
        private System.Windows.Forms.TextBox txtBoxChargenA35;
        private System.Windows.Forms.TextBox txtBoxChargenA30;
        private System.Windows.Forms.TextBox txtBoxChargenA45;
        private System.Windows.Forms.TextBox txtBoxChargenA40;
        private System.Windows.Forms.TextBox txtBoxChargenA60;
        private System.Windows.Forms.TextBox txtBoxChargenA50;
        private System.Windows.Forms.TextBox txtBoxChargenA65;
        private System.Windows.Forms.TextBox txtBoxFehlerA65;
        private System.Windows.Forms.TextBox txtBoxFehlerA60;
        private System.Windows.Forms.TextBox txtBoxFehlerA50;
        private System.Windows.Forms.TextBox txtBoxFehlerA45;
        private System.Windows.Forms.TextBox txtBoxFehlerA40;
        private System.Windows.Forms.TextBox txtBoxFehlerA35;
        private System.Windows.Forms.TextBox txtBoxFehlerA30;
        private System.Windows.Forms.TextBox txtBoxFehlerA25;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtBoxFehlerA20;
        private System.Windows.Forms.DataGridView dgvArtikelStueck;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblStueckAusgewaehlt;
        private System.Windows.Forms.Button Btn_InfoProduction;
        private System.Windows.Forms.Label lblProbeA20;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblProbeA25;
        private System.Windows.Forms.Label lblProbeA30;
        private System.Windows.Forms.Label lblProbeA35;
        private System.Windows.Forms.Label lblProbeA40;
        private System.Windows.Forms.Label lblProbeA45;
        private System.Windows.Forms.Label lblProbeA50;
        private System.Windows.Forms.Label lblProbeA60;
        private System.Windows.Forms.Label lblProbeA65;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblGesamtProduktiv;
        private System.Windows.Forms.Label lblGesamtChargen;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel.Design.Serialization;

namespace VerwaltungKST1127.Auftragsverwaltung
{
    public partial class Form_Rueckstand : Form
    {
        // Enthält die Belagdaten (aus der Hauptansicht) für die Auswahl im Grid.
        private readonly DataTable belagData;
        // Verbindung zur Verwaltungsdatenbank für Rückstandsabfragen.
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");
        // Der aktuell ausgewählte Belag (aus dem Grid).
        private string selectedBelag;

        // Standardkonstruktor (wird z. B. vom Designer verwendet).
        public Form_Rueckstand()
            : this(null)
        {
        }

        // Konstruktor mit Belagdaten aus der Hauptansicht.
        public Form_Rueckstand(DataTable belagData)
        {
            InitializeComponent();
            this.belagData = belagData;
            // Event-Handler registrieren, damit das Formular konsistent reagiert.
            Load += Form_Rueckstand_Load;
            dGv_AuswahlBelag.CellClick += dGv_AuswahlBelag_CellClick;
            dateTimePickerRueckstandAb.ValueChanged += dateTimePickerRueckstandAb_ValueChanged;
        }

        // Initialisierung beim Laden des Formulars.
        private void Form_Rueckstand_Load(object sender, EventArgs e)
        {
            LoadBelagData();
            dateTimePickerRueckstandAb.Value = DateTime.Today;
            // Gesamt-Rückstand bezieht sich auf den aktuell gewählten Stichtag.
            UpdateGesamtRueckstand();
            // Das +/- 2 Monate Diagramm soll immer relativ zu "heute" geladen werden.
            RefreshRueckstandPlusMinusTwoMonthsChart();
        }

        // Lädt die Belag-Auswahldaten in das Grid und formatiert die Darstellung.
        private void LoadBelagData()
        {
            if (belagData == null)
            {
                dGv_AuswahlBelag.DataSource = null;
                return;
            }

            dGv_AuswahlBelag.DataSource = belagData.Copy();
            dGv_AuswahlBelag.AllowUserToAddRows = false;
            dGv_AuswahlBelag.AllowUserToDeleteRows = false;
            dGv_AuswahlBelag.AllowUserToResizeColumns = false;
            dGv_AuswahlBelag.AllowUserToResizeRows = false;
            dGv_AuswahlBelag.ReadOnly = true;
            dGv_AuswahlBelag.RowHeadersVisible = false;
            dGv_AuswahlBelag.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGv_AuswahlBelag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dGv_AuswahlBelag.Columns.Contains("AVOs"))
            {
                dGv_AuswahlBelag.Columns["AVOs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGv_AuswahlBelag.Columns["AVOs"].DefaultCellStyle.Format = "N0";
            }

            foreach (DataGridViewColumn column in dGv_AuswahlBelag.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        // Bei Auswahl eines Belags wird das Detaildiagramm aktualisiert.
        private void dGv_AuswahlBelag_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            string belag = dGv_AuswahlBelag.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(belag))
            {
                return;
            }

            selectedBelag = belag;
            RefreshRueckstandVerguetenChart();
        }

        // Stichtag geändert: Gesamt-Rückstand neu berechnen und ggf. Belagdiagramm aktualisieren.
        private void dateTimePickerRueckstandAb_ValueChanged(object sender, EventArgs e)
        {
            UpdateGesamtRueckstand();

            if (string.IsNullOrWhiteSpace(selectedBelag))
            {
                return;
            }

            RefreshRueckstandVerguetenChart();
        }

        // Aktualisiert das Diagramm für den aktuell ausgewählten Belag.
        private void RefreshRueckstandVerguetenChart()
        {
            DateTime selectedDate = dateTimePickerRueckstandAb.Value.Date;
            bool singleDay = selectedDate == DateTime.Today;

            Dictionary<DateTime, decimal> rueckstandByDate = CalculateRueckstandVerguetenByDate(selectedBelag, selectedDate);
            UpdateVerguetenChart(selectedBelag, rueckstandByDate, singleDay, selectedDate);
        }

        // Aktualisiert den Gesamt-Rückstand über alle Beläge bis zum ausgewählten Stichtag.
        private void UpdateGesamtRueckstand()
        {
            DateTime selectedDate = dateTimePickerRueckstandAb.Value.Date;
            Dictionary<DateTime, decimal> rueckstandByDate = CalculateRueckstandVerguetenByDateAllBelags(selectedDate);
            decimal total = rueckstandByDate.Values.Sum();
            lblGesamt.Text = $"{total:0.00}h";
        }

        // Erzeugt das +/- 2 Monate Diagramm relativ zu "heute" (unabhängig vom DateTimePicker).
        private void RefreshRueckstandPlusMinusTwoMonthsChart()
        {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddMonths(-2).Date;
            DateTime endDate = today.AddMonths(2).Date;

            Dictionary<DateTime, decimal> rueckstandByDate = CalculateRueckstandVerguetenByDateAllBelagsRange(startDate, endDate);
            UpdatePlusMinusTwoMonthsChart(rueckstandByDate, startDate, endDate);
        }

        // Berechnet Rückstände je Tag für einen Belag (nur bis zum Stichtag).
        private Dictionary<DateTime, decimal> CalculateRueckstandVerguetenByDate(string belag, DateTime endDate)
        {
            Dictionary<DateTime, decimal> rueckstandByDate = new Dictionary<DateTime, decimal>();
            string belagOhneBindestrich = belag.Replace("-", "").ToUpper();
            string belagMitBindestrich = belagOhneBindestrich.Insert(1, "-");

            string query = @"
                SELECT
                    CONVERT(date, a.trdf_enddate) AS Enddatum,
                    a.mitm_teilenr AS Artikel,
                    a.qplo_sollstk AS SollStk,
                    a.qcmp_iststk AS IstStk,
                    passendVorbereiten.qcmp2_vorstk AS VorStk,
                    CASE
                        WHEN a.txta_avoinfo LIKE '%III%' OR a.txta_avoinfo LIKE '%Iii%' OR a.txta_avoinfo LIKE '%IIi%' OR a.txta_avoinfo LIKE '%iii%' OR a.txta_avoinfo LIKE '%iII%' THEN 0
                        WHEN a.txta_avoinfo LIKE '%Ii%' OR a.txta_avoinfo LIKE '%iI%' OR a.txta_avoinfo LIKE '%ii%' OR a.txta_avoinfo LIKE '%II%' THEN 2
                        WHEN a.txta_avoinfo LIKE '%i%' OR a.txta_avoinfo LIKE '%I%' THEN 1
                        ELSE 0
                    END AS Seite
                FROM
                    LN_ProdOrders_PRD a
                OUTER APPLY (
                    SELECT TOP 1 b.qcmp2_vorstk
                    FROM LN_ProdOrders_PRD b
                    WHERE b.pdno_prodnr = a.pdno_prodnr
                      AND b.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vorbereiten%'
                      AND b.trdf_enddate < a.trdf_enddate
                    ORDER BY b.trdf_enddate DESC
                ) AS passendVorbereiten
                WHERE
                    a.opsta_avostat IN ('Active', 'Planned', 'Released')
                    AND (
                        a.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE @BelagValue1
                        OR a.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE @BelagValue2
                    )
                    AND a.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vergüten%'
                    AND CONVERT(date, a.trdf_enddate) <= @EndDate;";

            // Die Abfrage sucht nach AVOs, die den Belag (mit oder ohne Bindestrich) enthalten, und berücksichtigt nur Vergüten-Schritte bis zum Stichtag.
            using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
            {
                command.Parameters.AddWithValue("@BelagValue1", "%" + belagOhneBindestrich + "%");
                command.Parameters.AddWithValue("@BelagValue2", "%" + belagMitBindestrich + "%");
                command.Parameters.AddWithValue("@EndDate", endDate);

                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    sqlConnectionVerwaltung.Open();
                    adapter.Fill(dataTable);
                }
                finally
                {
                    if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                    {
                        sqlConnectionVerwaltung.Close();
                    }
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["Enddatum"] == DBNull.Value)
                    {
                        continue;
                    }

                    DateTime enddatum = Convert.ToDateTime(row["Enddatum"]).Date;
                    string artikel = row["Artikel"]?.ToString() ?? string.Empty;
                    int seite = Convert.ToInt32(row["Seite"]);
                    decimal sollStk = row["SollStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["SollStk"]);
                    decimal istStk = row["IstStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["IstStk"]);
                    decimal vorStk = row["VorStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["VorStk"]);

                    if (string.IsNullOrWhiteSpace(artikel))
                    {
                        continue;
                    }

                    if (vorStk <= istStk)
                    {
                        continue;
                    }

                    (decimal stkCharge, decimal chargenzeit) = GetStkChargeUndChargenzeit(artikel, seite, belagOhneBindestrich);
                    if (stkCharge <= 0 || chargenzeit <= 0)
                    {
                        continue;
                    }

                    decimal rueckstandStk = sollStk - istStk;
                    if (rueckstandStk <= 0)
                    {
                        continue;
                    }

                    decimal rueckstandZeit = (rueckstandStk / stkCharge) * chargenzeit;

                    if (!rueckstandByDate.ContainsKey(enddatum))
                    {
                        rueckstandByDate[enddatum] = 0m;
                    }

                    rueckstandByDate[enddatum] += rueckstandZeit;
                }
            }

            return rueckstandByDate;
        }

        // Berechnet Rückstände je Tag für alle Beläge bis zum Stichtag.
        private Dictionary<DateTime, decimal> CalculateRueckstandVerguetenByDateAllBelags(DateTime endDate)
        {
            Dictionary<DateTime, decimal> rueckstandByDate = new Dictionary<DateTime, decimal>();

            string query = @"
                SELECT
                    CONVERT(date, a.trdf_enddate) AS Enddatum,
                    a.mitm_teilenr AS Artikel,
                    a.qplo_sollstk AS SollStk,
                    a.qcmp_iststk AS IstStk,
                    passendVorbereiten.qcmp2_vorstk AS VorStk,
                    CASE
                        WHEN a.txta_avoinfo LIKE '%III%' OR a.txta_avoinfo LIKE '%Iii%' OR a.txta_avoinfo LIKE '%IIi%' OR a.txta_avoinfo LIKE '%iii%' OR a.txta_avoinfo LIKE '%iII%' THEN 0
                        WHEN a.txta_avoinfo LIKE '%Ii%' OR a.txta_avoinfo LIKE '%iI%' OR a.txta_avoinfo LIKE '%ii%' OR a.txta_avoinfo LIKE '%II%' THEN 2
                        WHEN a.txta_avoinfo LIKE '%i%' OR a.txta_avoinfo LIKE '%I%' THEN 1
                        ELSE 0
                    END AS Seite,
                    a.txta_avoinfo AS AVOinfo
                FROM
                    LN_ProdOrders_PRD a
                OUTER APPLY (
                    SELECT TOP 1 b.qcmp2_vorstk
                    FROM LN_ProdOrders_PRD b
                    WHERE b.pdno_prodnr = a.pdno_prodnr
                      AND b.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vorbereiten%'
                      AND b.trdf_enddate < a.trdf_enddate
                    ORDER BY b.trdf_enddate DESC
                ) AS passendVorbereiten
                WHERE
                    a.opsta_avostat IN ('Active', 'Planned', 'Released')
                    AND a.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vergüten%'
                    AND CONVERT(date, a.trdf_enddate) <= @EndDate;";

            using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
            {
                command.Parameters.AddWithValue("@EndDate", endDate);

                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    sqlConnectionVerwaltung.Open();
                    adapter.Fill(dataTable);
                }
                finally
                {
                    if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                    {
                        sqlConnectionVerwaltung.Close();
                    }
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["Enddatum"] == DBNull.Value)
                    {
                        continue;
                    }

                    DateTime enddatum = Convert.ToDateTime(row["Enddatum"]).Date;
                    string artikel = row["Artikel"]?.ToString() ?? string.Empty;
                    int seite = Convert.ToInt32(row["Seite"]);
                    decimal sollStk = row["SollStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["SollStk"]);
                    decimal istStk = row["IstStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["IstStk"]);
                    decimal vorStk = row["VorStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["VorStk"]);
                    string avoInfo = row["AVOinfo"]?.ToString() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(artikel))
                    {
                        continue;
                    }

                    if (vorStk <= istStk)
                    {
                        continue;
                    }

                    string belag = CleanBelag(avoInfo);
                    if (string.IsNullOrWhiteSpace(belag))
                    {
                        continue;
                    }

                    (decimal stkCharge, decimal chargenzeit) = GetStkChargeUndChargenzeit(artikel, seite, belag);
                    if (stkCharge <= 0 || chargenzeit <= 0)
                    {
                        continue;
                    }

                    decimal rueckstandStk = sollStk - istStk;
                    if (rueckstandStk <= 0)
                    {
                        continue;
                    }

                    decimal rueckstandZeit = (rueckstandStk / stkCharge) * chargenzeit;

                    if (!rueckstandByDate.ContainsKey(enddatum))
                    {
                        rueckstandByDate[enddatum] = 0m;
                    }

                    rueckstandByDate[enddatum] += rueckstandZeit;
                }
            }

            return rueckstandByDate;
        }

        // Berechnet Rückstände je Tag für alle Beläge in einem Zeitraum (inklusive Start/Ende).
        private Dictionary<DateTime, decimal> CalculateRueckstandVerguetenByDateAllBelagsRange(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, decimal> rueckstandByDate = new Dictionary<DateTime, decimal>();

            string query = @"
                SELECT
                    CONVERT(date, a.trdf_enddate) AS Enddatum,
                    a.mitm_teilenr AS Artikel,
                    a.qplo_sollstk AS SollStk,
                    a.qcmp_iststk AS IstStk,
                    passendVorbereiten.qcmp2_vorstk AS VorStk,
                    CASE
                        WHEN a.txta_avoinfo LIKE '%III%' OR a.txta_avoinfo LIKE '%Iii%' OR a.txta_avoinfo LIKE '%IIi%' OR a.txta_avoinfo LIKE '%iii%' OR a.txta_avoinfo LIKE '%iII%' THEN 0
                        WHEN a.txta_avoinfo LIKE '%Ii%' OR a.txta_avoinfo LIKE '%iI%' OR a.txta_avoinfo LIKE '%ii%' OR a.txta_avoinfo LIKE '%II%' THEN 2
                        WHEN a.txta_avoinfo LIKE '%i%' OR a.txta_avoinfo LIKE '%I%' THEN 1
                        ELSE 0
                    END AS Seite,
                    a.txta_avoinfo AS AVOinfo
                FROM
                    LN_ProdOrders_PRD a
                OUTER APPLY (
                    SELECT TOP 1 b.qcmp2_vorstk
                    FROM LN_ProdOrders_PRD b
                    WHERE b.pdno_prodnr = a.pdno_prodnr
                      AND b.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vorbereiten%'
                      AND b.trdf_enddate < a.trdf_enddate
                    ORDER BY b.trdf_enddate DESC
                ) AS passendVorbereiten
                WHERE
                    a.opsta_avostat IN ('Active', 'Planned', 'Released')
                    AND a.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vergüten%'
                    AND CONVERT(date, a.trdf_enddate) >= @StartDate
                    AND CONVERT(date, a.trdf_enddate) <= @EndDate;";

            using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    sqlConnectionVerwaltung.Open();
                    adapter.Fill(dataTable);
                }
                finally
                {
                    if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                    {
                        sqlConnectionVerwaltung.Close();
                    }
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["Enddatum"] == DBNull.Value)
                    {
                        continue;
                    }

                    DateTime enddatum = Convert.ToDateTime(row["Enddatum"]).Date;
                    string artikel = row["Artikel"]?.ToString() ?? string.Empty;
                    int seite = Convert.ToInt32(row["Seite"]);
                    decimal sollStk = row["SollStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["SollStk"]);
                    decimal istStk = row["IstStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["IstStk"]);
                    decimal vorStk = row["VorStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["VorStk"]);
                    string avoInfo = row["AVOinfo"]?.ToString() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(artikel))
                    {
                        continue;
                    }

                    if (vorStk <= istStk)
                    {
                        continue;
                    }

                    string belag = CleanBelag(avoInfo);
                    if (string.IsNullOrWhiteSpace(belag))
                    {
                        continue;
                    }

                    (decimal stkCharge, decimal chargenzeit) = GetStkChargeUndChargenzeit(artikel, seite, belag);
                    if (stkCharge <= 0 || chargenzeit <= 0)
                    {
                        continue;
                    }

                    decimal rueckstandStk = sollStk - istStk;
                    if (rueckstandStk <= 0)
                    {
                        continue;
                    }

                    decimal rueckstandZeit = (rueckstandStk / stkCharge) * chargenzeit;

                    if (!rueckstandByDate.ContainsKey(enddatum))
                    {
                        rueckstandByDate[enddatum] = 0m;
                    }

                    rueckstandByDate[enddatum] += rueckstandZeit;
                }
            }

            return rueckstandByDate;
        }

        // Bereinigt die Belagsinformation aus dem AVO-Text.
        private string CleanBelag(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            input = System.Text.RegularExpressions.Regex.Replace(input, "vergüten", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
            var match = System.Text.RegularExpressions.Regex.Match(input, @"B-?\d+");

            if (match.Success)
            {
                return match.Value.Replace("-", "").ToUpper();
            }

            return "";
        }

        // Liefert Chargengröße und Chargenzeit für einen Artikel/Seite/Belag.
        private (decimal stkCharge, decimal chargenzeit) GetStkChargeUndChargenzeit(string artikel, int seite, string belag)
        {
            string query = @"
                SELECT STK_CHARGE, CHARGENZEIT
                FROM Serienlinsen
                WHERE ARTNR = @Artikel
                  AND SEITE = @Seite
                  AND (
                      REPLACE(VERGBELAG, '-', '') = @Belag
                      OR REPLACE(Belag1, '-', '') = @Belag
                  );";

            using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
            {
                command.Parameters.AddWithValue("@Artikel", artikel);
                command.Parameters.AddWithValue("@Seite", seite);
                command.Parameters.AddWithValue("@Belag", belag);

                try
                {
                    sqlConnectionVerwaltung.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal stkCharge = reader["STK_CHARGE"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["STK_CHARGE"]);
                            decimal chargenzeit = reader["CHARGENZEIT"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["CHARGENZEIT"]);
                            return (stkCharge, chargenzeit);
                        }
                    }
                }
                finally
                {
                    if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                    {
                        sqlConnectionVerwaltung.Close();
                    }
                }
            }

            return (0m, 0m);
        }

        // Zeichnet das Belagdiagramm (ein Tag oder kumulativ je Tag).
        private void UpdateVerguetenChart(string belag, Dictionary<DateTime, decimal> rueckstandByDate, bool singleDay, DateTime selectedDate)
        {
            if (chartVergueten.Series.Count == 0)
            {
                chartVergueten.Series.Add(new Series("Series1"));
            }

            Series series = chartVergueten.Series[0]; // Es gibt nur eine Serie, daher immer Series[0]
            series.ChartType = SeriesChartType.Column;
            series.Points.Clear();
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "0.00";
            series.SmartLabelStyle.Enabled = true;
            // Linien von chart ausblenden
            chartVergueten.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            if (singleDay)
            {
                decimal total = rueckstandByDate.Values.Sum();
                series.Points.AddXY(belag, Math.Round(total, 2));
                lblVergueten.Text = $"{total:0.00}h";
                return;
            }

            foreach (DateTime date in rueckstandByDate.Keys.OrderBy(key => key))
            {
                series.Points.AddXY(date.ToString("dd.MM"), Math.Round(rueckstandByDate[date], 2));
            }

            decimal sum = rueckstandByDate.Values.Sum();
            lblVergueten.Text = $"{sum:0.00}h";
        }

        // Zeichnet das +/- 2 Monate Diagramm (täglich, inkl. Nullwerte und Top-5-Ausreißer).
        private void UpdatePlusMinusTwoMonthsChart(Dictionary<DateTime, decimal> rueckstandByDate, DateTime startDate, DateTime endDate)
        {
            if (chartPlusMinusTwoMonths.Series.Count == 0)
            {
                chartPlusMinusTwoMonths.Series.Add(new Series("Series1"));
            }

            Series series = chartPlusMinusTwoMonths.Series[0];
            series.ChartType = SeriesChartType.Column;
            series.Points.Clear();
            series.IsValueShownAsLabel = false;
            series.SmartLabelStyle.Enabled = true;
            chartPlusMinusTwoMonths.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            // Alle Tage im Zeitraum berücksichtigen (inkl. Nullwerte).
            Dictionary<DateTime, decimal> filledDates = FillMissingDates(rueckstandByDate, startDate, endDate);

            foreach (DateTime date in filledDates.Keys.OrderBy(key => key))
            {
                int pointIndex = series.Points.AddXY(date.ToString("dd.MM"), Math.Round(filledDates[date], 2));
                DataPoint point = series.Points[pointIndex];
                point.Color = Color.SteelBlue;
            }

            // Top 5 Ausreißer (höchste Rückstände) markieren.
            foreach (DataPoint point in series.Points.Cast<DataPoint>()
                         .OrderByDescending(p => p.YValues.FirstOrDefault())
                         .Take(5))
            {
                point.Color = Color.Red;
            }
        }

        // Füllt fehlende Tage im Zeitraum mit Nullwerten auf.
        private Dictionary<DateTime, decimal> FillMissingDates(Dictionary<DateTime, decimal> rueckstandByDate, DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, decimal> filledDates = new Dictionary<DateTime, decimal>();
            // Alle Tage von Start- bis Enddatum durchlaufen und Werte aus rueckstandByDate übernehmen oder mit 0 auffüllen.
            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                filledDates[date] = rueckstandByDate.TryGetValue(date, out decimal value) ? value : 0m;
            }

            return filledDates;
        }
    }
}

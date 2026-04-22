using System;                                           // Basis-Typen wie String, DateTime, Math
using System.Collections.Generic;                       // List<T>, Dictionary<TKey,TValue>, IEnumerable<T>
using System.Data;                                      // DataTable, DataRow, DataSet – ADO.NET Kern
using System.Drawing;                                   // Farben, Grafiken, Sizes, Points für UI
using System.Linq;                                      // LINQ-Funktionen
using System.Data.SqlClient;                            // SQL-Server Zugriff
using System.Windows.Forms;                             // Windows-Forms
using System.Windows.Forms.DataVisualization.Charting;  // Chart-Steuerelemente
using System.Threading.Tasks;                           // async/await, Task.Run

namespace VerwaltungKST1127.Auftragsverwaltung
{
    /// <summary>
    /// Formular zur Visualisierung von Rückständen (Vergüten) gesamt + pro Belag.
    /// Clean-Performance-Version:
    /// - Stammdaten (Serienlinsen) einmalig in Cache
    /// - Vergüten-AVOs für ein definiertes Zeitfenster einmalig laden
    /// - Werte voraggregieren (pro Belag, pro Tag, gesamt)
    /// - DGV/Charts aus Cache (keine DB-Hits mehr im UI)
    /// </summary>
    public partial class Form_Rueckstand : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Konfiguration: Datenfenster für den einmaligen LN-Ladevorgang
        // -----------------------------------------------------------------------------------------------------------------
        private const int PreloadPastDays = 180;   // z. B. 180 Tage Historie ab heute rückwärts
        private const int PreloadFutureDays = 60;  // z. B. 60 Tage in die Zukunft (für ±40 ausreichend)

        // -----------------------------------------------------------------------------------------------------------------
        // Felder
        // -----------------------------------------------------------------------------------------------------------------

        private readonly DataTable belagData;

        private readonly SqlConnection sqlConnectionVerwaltung =
            new(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private string selectedBelag;

        // --- Caches & Aggregate (werden beim Start genau einmal aufgebaut) ---

        /// <summary>Serienlinsen-Cache: (Artikel, Seite, Belag) -> (Stück/Charge, Zeit/Charge)</summary>
        private Dictionary<(string Art, int Seite, string Belag), (decimal StkCharge, decimal Zeit)> _serienlinsenCache
            = new Dictionary<(string, int, string), (decimal, decimal)>();

        /// <summary>Rückstand gesamt (h) pro Belag bis heute, mit korrekter Logik: VorStk - IstStk</summary>
        private Dictionary<string, decimal> _aggBelagRueckstandBisHeuteHours
            = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Soll-Rückstand (h) pro Belag bis heute (komplette Sollmenge in Stunden)</summary>
        private Dictionary<string, decimal> _aggBelagSollBisHeuteHours
            = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Rückstand (h) aller Beläge pro Tag (für das ±40 Tage Chart)</summary>
        private Dictionary<DateTime, decimal> _aggRueckstandAllBelagsByDay
            = new Dictionary<DateTime, decimal>();

        /// <summary>Rückstand (h) pro Belag pro Tag (für das obere Belag-Chart)</summary>
        private Dictionary<string, Dictionary<DateTime, decimal>> _aggBelagByDay
            = new Dictionary<string, Dictionary<DateTime, decimal>>(StringComparer.OrdinalIgnoreCase);

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktoren
        // -----------------------------------------------------------------------------------------------------------------

        public Form_Rueckstand() : this(null) { }

        public Form_Rueckstand(DataTable belagData)
        {
            InitializeComponent();
            this.belagData = belagData;

            Load += Form_Rueckstand_Load;
            dGv_AuswahlBelag.CellClick += dGv_AuswahlBelag_CellClick;
            dateTimePickerRueckstandAb.ValueChanged += dateTimePickerRueckstandAb_ValueChanged;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Formular-Initialisierung
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Startet UI sofort, lädt Daten & Aggregate im Hintergrund, füllt danach Grid/Charts.
        /// </summary>
        private async void Form_Rueckstand_Load(object sender, EventArgs e)
        {
            // UI-Grundaufbau
            LoadBelagData();
            dateTimePickerRueckstandAb.Value = DateTime.Today;

            // Daten + Aggregate in Hintergrund-Thread aufbauen
            await Task.Run(BuildAllCachesAndAggregates);

            // Danach: Grid-Spalten mit Cache-Werten befüllen + Charts aktualisieren (UI-Thread)
            FillBelagTotalsFromCaches();
            // Label der gesamten Soll Rückstände aktualisieren
            UpdateGesamtRueckstandAbteilungLabel();
            UpdateGesamtRueckstand();                 // nutzt Aggregat (keine DB)
            RefreshRueckstandPlusMinusTwoMonthsChart(); // nutzt Aggregat (keine DB)
        }

        // -----------------------------------------------------------------------------------------------------------------
        // UI: Grid & DatePicker
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Bindet das Belag-Grid und legt die zwei zusätzlichen Spalten an (Rückstand gesamt, Soll-Rückstand).
        /// </summary>
        private void LoadBelagData()
        {
            if (belagData == null)
            {
                dGv_AuswahlBelag.DataSource = null;
                return;
            }

            dGv_AuswahlBelag.DataSource = belagData.Copy();
            // Mittag anordnen

            if (dGv_AuswahlBelag.Columns.Contains("Realer Rücks."))
                dGv_AuswahlBelag.Columns["Realer Rücks."].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

            if (dGv_AuswahlBelag.Columns.Contains("Brutto Rücks."))
                dGv_AuswahlBelag.Columns["Brutto Rücks."].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

            dGv_AuswahlBelag.AllowUserToAddRows = false;
            dGv_AuswahlBelag.AllowUserToDeleteRows = false;
            dGv_AuswahlBelag.AllowUserToResizeColumns = false;
            dGv_AuswahlBelag.AllowUserToResizeRows = false;
            dGv_AuswahlBelag.ReadOnly = true;
            dGv_AuswahlBelag.RowHeadersVisible = false;
            dGv_AuswahlBelag.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dGv_AuswahlBelag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Zusatzspalten anlegen (falls nicht vorhanden)
            EnsureDgvTotalsColumns();

            // Optionale Spalte "AVOs"
            if (dGv_AuswahlBelag.Columns.Contains("AVOs"))
            {
                dGv_AuswahlBelag.Columns["AVOs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGv_AuswahlBelag.Columns["AVOs"].DefaultCellStyle.Format = "N0";
            }

            foreach (DataGridViewColumn c in dGv_AuswahlBelag.Columns)
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Legt die beiden Zusatzspalten an: "AktuellStk" (echter Rückstand in h) und "SollStk" (Soll in h), beide bis heute.
        /// </summary>
        private void EnsureDgvTotalsColumns()
        {
            // Spalte 1: Realer Rückstand
            const string ColRueck = "Realer Rücks.";
            if (!dGv_AuswahlBelag.Columns.Contains(ColRueck))
            {
                var col = new DataGridViewTextBoxColumn();
                col.Name = ColRueck;
                col.HeaderText = "Realer Rücks.";
                col.ReadOnly = true;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Style EINZELN setzen
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Format = "N2";

                dGv_AuswahlBelag.Columns.Add(col);
            }

            // Spalte 2: Brutto Rückstand
            const string ColSoll = "Brutto Rücks.";
            if (!dGv_AuswahlBelag.Columns.Contains(ColSoll))
            {
                var col = new DataGridViewTextBoxColumn();
                col.Name = ColSoll;
                col.HeaderText = "Brutto Rücks.";
                col.ReadOnly = true;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Format = "N2";

                dGv_AuswahlBelag.Columns.Add(col);
            }
        }

        /// <summary>
        /// Klick auf Belag-Zeile → Merken + oberes Chart für Belag aktualisieren (aus Cache, ohne DB).
        /// </summary>
        private void dGv_AuswahlBelag_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string belag = dGv_AuswahlBelag.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(belag)) return;

            selectedBelag = belag;
            RefreshRueckstandVerguetenChart(); // nutzt Cache
        }

        /// <summary>
        /// DatePicker geändert → nur Summen & Belag-Chart neu (nutzt Aggregate).
        /// </summary>
        private void dateTimePickerRueckstandAb_ValueChanged(object sender, EventArgs e)
        {
            UpdateGesamtRueckstand();      // Summe bis Stichtag (aus _aggRueckstandAllBelagsByDay)
            if (!string.IsNullOrWhiteSpace(selectedBelag))
                RefreshRueckstandVerguetenChart(); // Belag-Chart aus _aggBelagByDay
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Clean-Performance: Einmaliges Laden + Aggregation
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Einmaliger Build aller Caches/Aggregate:
        /// 1) Serienlinsen laden (Cache)
        /// 2) Vergüten-AVOs im Fenster [heute-PreloadPastDays .. heute+PreloadFutureDays] laden
        /// 3) pro Datensatz: Belag extrahieren, Rückstände berechnen/aggregieren (pro Belag, pro Tag, gesamt)
        /// </summary>
        private void BuildAllCachesAndAggregates()
        {
            // 1) Serienlinsen-Cache laden (inkl. Belag & Belag1)
            LoadSerienlinsenCache();

            // 2) Vergüten-Zeilen laden (einmal) für Fenster
            DateTime today = DateTime.Today;
            DateTime start = today.AddDays(-PreloadPastDays);
            DateTime end = today.AddDays(PreloadFutureDays);

            DataTable lnTable = LoadLnVerguetenWindow(start, end);

            // 3) HashSet mit Belägen aus dem Grid (um unnötige Beläge rauszufiltern)
            var belagSet = ExtractBelagSetFromGrid();

            // 4) Aggregate vorbereiten (lokal, dann am Ende in Felder schreiben)
            var belagRueckBisHeute = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var belagSollBisHeute = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var allByDay = new Dictionary<DateTime, decimal>();
            var belagByDay = new Dictionary<string, Dictionary<DateTime, decimal>>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in lnTable.Rows)
            {
                // Enddatum & Filter (nur valide)
                if (row["Enddatum"] == DBNull.Value)
                    continue;

                DateTime day = Convert.ToDateTime(row["Enddatum"]).Date;
                string artikel = row["Artikel"]?.ToString() ?? "";
                string avoInfo = row["AVOinfo"]?.ToString() ?? "";
                int seite = row["Seite"] == DBNull.Value ? 0 : Convert.ToInt32(row["Seite"]);
                decimal ist = row["IstStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["IstStk"]);
                decimal vor = row["VorStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["VorStk"]);
                decimal soll = row["SollStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["SollStk"]);

                // Belag normalisieren (B-123 -> B123)
                string belag = CleanBelag(avoInfo);
                if (string.IsNullOrWhiteSpace(belag))
                    continue;

                // Nur Beläge, die im Grid existieren (spart Arbeit)
                if (belagSet != null && belagSet.Count > 0 && !belagSet.Contains(belag))
                    continue;

                // Stammdaten holen (Cache)
                var key = (artikel, seite, belag);
                if (!_serienlinsenCache.TryGetValue(key, out var param))
                    continue; // ohne Parameter keine Zeitberechnung

                if (param.StkCharge <= 0 || param.Zeit <= 0)
                    continue;

                // ECHTER Rückstand: Vorbereitet - Vergütet
                decimal rueckStk = vor - ist;
                if (rueckStk > 0)
                {
                    decimal rueckZeit = (rueckStk / param.StkCharge) * param.Zeit;

                    // pro Belag pro Tag (für oberes Chart)
                    if (!belagByDay.TryGetValue(belag, out var dict))
                    {
                        dict = new Dictionary<DateTime, decimal>();
                        belagByDay[belag] = dict;
                    }
                    if (!dict.ContainsKey(day)) dict[day] = 0m;
                    dict[day] += Math.Round(rueckZeit, 4);

                    // gesamt pro Tag (für ±40 Chart, enthält ALLE Beläge)
                    if (!allByDay.ContainsKey(day)) allByDay[day] = 0m;
                    allByDay[day] += Math.Round(rueckZeit, 4);

                    // bis HEUTE -> Summen pro Belag (für DGV „Rückstand gesamt (h)“)
                    if (day <= today)
                    {
                        if (!belagRueckBisHeute.ContainsKey(belag)) belagRueckBisHeute[belag] = 0m;
                        belagRueckBisHeute[belag] += Math.Round(rueckZeit, 4);
                    }
                }

                // SOLL-Rückstand: komplette Sollmenge in h, bis HEUTE
                if (day <= today && soll > 0)
                {
                    decimal sollZeit = (soll / param.StkCharge) * param.Zeit;
                    if (!belagSollBisHeute.ContainsKey(belag)) belagSollBisHeute[belag] = 0m;
                    belagSollBisHeute[belag] += Math.Round(sollZeit, 4);
                }
            }

            // 5) Ergebnisse in Felder übernehmen
            _aggBelagRueckstandBisHeuteHours = belagRueckBisHeute;
            _aggBelagSollBisHeuteHours = belagSollBisHeute;
            _aggRueckstandAllBelagsByDay = allByDay;
            _aggBelagByDay = belagByDay;
        }

        /// <summary>
        /// Serienlinsen einmalig laden und in _serienlinsenCache ablegen.
        /// Legt je Datensatz 1..2 Schlüssel an (VERGBELAG und Belag1).
        /// </summary>
        private void LoadSerienlinsenCache()
        {
            var cache = new Dictionary<(string, int, string), (decimal, decimal)>();

            string sql = @"
                SELECT 
                    ARTNR,
                    SEITE,
                    REPLACE(VERGBELAG,'-','') AS BelagVerg,
                    REPLACE(Belag1,'-','')    AS BelagAlt,
                    STK_CHARGE,
                    CHARGENZEIT
                FROM Serienlinsen;";

            using (var cmd = new SqlCommand(sql, sqlConnectionVerwaltung))
            {
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);

                sqlConnectionVerwaltung.Open();
                da.Fill(dt);
                sqlConnectionVerwaltung.Close();

                foreach (DataRow r in dt.Rows)
                {
                    string art = r["ARTNR"]?.ToString() ?? "";
                    int seite = int.TryParse(r["SEITE"]?.ToString(), out var result) ? result : 0;
                    string belagVerg = (r["BelagVerg"]?.ToString() ?? "").ToUpper();
                    string belagAlt = (r["BelagAlt"]?.ToString() ?? "").ToUpper();

                    decimal stk = r["STK_CHARGE"] == DBNull.Value ? 0m : Convert.ToDecimal(r["STK_CHARGE"]);
                    decimal zeit = r["CHARGENZEIT"] == DBNull.Value ? 0m : Convert.ToDecimal(r["CHARGENZEIT"]);
                    var val = (stk, zeit);

                    if (!string.IsNullOrWhiteSpace(belagVerg))
                        cache[(art, seite, belagVerg)] = val;

                    if (!string.IsNullOrWhiteSpace(belagAlt))
                        cache[(art, seite, belagAlt)] = val;
                }
            }

            _serienlinsenCache = cache;
        }

        /// <summary>
        /// Lädt alle Vergüten-AVOs im Fenster [start..end] (einmal), inkl. OUTER APPLY auf Vorbereiten.
        /// </summary>
        private DataTable LoadLnVerguetenWindow(DateTime start, DateTime end)
        {
            var dt = new DataTable();

            string sql = @"
                SELECT
                    CONVERT(date, a.trdf_enddate) AS Enddatum,
                    a.mitm_teilenr               AS Artikel,
                    a.qplo_sollstk               AS SollStk,
                    a.qcmp_iststk                AS IstStk,
                    prep.qcmp2_vorstk            AS VorStk,
                    a.txta_avoinfo               AS AVOinfo,
                    CASE
                        WHEN a.txta_avoinfo LIKE '%III%' OR a.txta_avoinfo LIKE '%Iii%' OR a.txta_avoinfo LIKE '%IIi%' OR a.txta_avoinfo LIKE '%iii%' OR a.txta_avoinfo LIKE '%iII%' THEN 0
                        WHEN a.txta_avoinfo LIKE '%Ii%'  OR a.txta_avoinfo LIKE '%iI%'  OR a.txta_avoinfo LIKE '%ii%'  OR a.txta_avoinfo LIKE '%II%'  THEN 2
                        WHEN a.txta_avoinfo LIKE '%i%'   OR a.txta_avoinfo LIKE '%I%'   THEN 1
                        ELSE 0
                    END AS Seite
                FROM LN_ProdOrders_PRD a
                OUTER APPLY (
                    SELECT TOP 1 b.qcmp2_vorstk
                    FROM LN_ProdOrders_PRD b
                    WHERE b.pdno_prodnr = a.pdno_prodnr
                      AND b.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vorbereiten%'
                      AND b.trdf_enddate < a.trdf_enddate
                    ORDER BY b.trdf_enddate DESC
                ) prep
                WHERE
                    a.opsta_avostat IN ('Active', 'Planned', 'Released')
                    AND a.txta_avoinfo COLLATE Latin1_General_CI_AS LIKE '%Vergüten%'
                    AND CONVERT(date, a.trdf_enddate) BETWEEN @Start AND @End;";

            using (var cmd = new SqlCommand(sql, sqlConnectionVerwaltung))
            {
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);

                var da = new SqlDataAdapter(cmd);

                sqlConnectionVerwaltung.Open();
                da.Fill(dt);
                sqlConnectionVerwaltung.Close();
            }

            return dt;
        }

        /// <summary>
        /// Nimmt die Belag-Liste aus dem Grid (Spalte „Belag“) und liefert ein HashSet (normalisiert).
        /// Falls belagData null ist, wird null zurückgegeben (dann keine Filterung).
        /// </summary>
        private HashSet<string> ExtractBelagSetFromGrid()
        {
            if (belagData == null || !belagData.Columns.Contains("Belag"))
                return null;

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow r in belagData.Rows)
            {
                string b = r["Belag"]?.ToString();
                if (string.IsNullOrWhiteSpace(b)) continue;

                b = b.Replace("-", "").ToUpper();
                set.Add(b);
            }
            return set;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Grid-Befüllung aus Aggregaten (ohne DB)
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Füllt die beiden Zusatzspalten im DGV aus den Aggregaten.
        /// </summary>
        private void FillBelagTotalsFromCaches()
        {
            const string ColRueck = "Realer Rücks.";
            const string ColSoll = "Brutto Rücks.";

            if (!dGv_AuswahlBelag.Columns.Contains(ColRueck) ||
                !dGv_AuswahlBelag.Columns.Contains(ColSoll))
                return;

            foreach (DataGridViewRow row in dGv_AuswahlBelag.Rows)
            {
                if (row.IsNewRow) continue;

                string belag = row.Cells["Belag"]?.Value?.ToString();
                if (string.IsNullOrWhiteSpace(belag)) { row.Cells[ColRueck].Value = null; row.Cells[ColSoll].Value = null; continue; }

                string clean = belag.Replace("-", "").ToUpper();

                decimal rueck = _aggBelagRueckstandBisHeuteHours.TryGetValue(clean, out var rr) ? rr : 0m;
                decimal soll = _aggBelagSollBisHeuteHours.TryGetValue(clean, out var sr) ? sr : 0m;

                row.Cells[ColRueck].Value = Math.Round(rueck, 2);
                row.Cells[ColSoll].Value = Math.Round(soll, 2);
            }
        }

        /// <summary>
        /// Summiert die Spalte "SollStk" im DGV und zeigt das Ergebnis als Stunden im Label an.
        /// </summary>
        private void UpdateGesamtRueckstandAbteilungLabel()
        {
            const string SollCol = "Brutto Rücks.";  // Name deiner rechten Spalte im DGV
            if (!dGv_AuswahlBelag.Columns.Contains(SollCol))
            {
                lblGesamtRueckstandAbteilung.Text = "0,00h";
                return;
            }

            decimal sum = 0m;
            foreach (DataGridViewRow row in dGv_AuswahlBelag.Rows)
            {
                if (row.IsNewRow) continue;
                object val = row.Cells[SollCol].Value;
                if (val == null || val == DBNull.Value) continue;

                // DGV liefert hier bereits Zahlen (wir formatieren später nur die Anzeige)
                if (decimal.TryParse(Convert.ToString(val), out decimal d))
                    sum += d;
            }

            lblGesamtRueckstandAbteilung.Text = $"{sum:0.00}h";
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Summen/Charts
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gesamt-Rückstand über alle Beläge bis Stichtag (aus Aggregat _aggRueckstandAllBelagsByDay).
        /// </summary>
        private void UpdateGesamtRueckstand()
        {
            DateTime sel = dateTimePickerRueckstandAb.Value.Date;
            decimal sum = _aggRueckstandAllBelagsByDay
                .Where(kv => kv.Key <= sel)
                .Sum(kv => kv.Value);

            lblGesamt.Text = $"{Math.Round(sum, 2):0.00}h";
        }

        /// <summary>
        /// ±40-Tage-Chart (unten) aus Aggregat _aggRueckstandAllBelagsByDay, ohne DB.
        /// </summary>
        private void RefreshRueckstandPlusMinusTwoMonthsChart()
        {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(-40);
            DateTime endDate = today.AddDays(+40);

            // Filter aus All-Belags-By-Day
            var dict = _aggRueckstandAllBelagsByDay
                .Where(kv => kv.Key >= startDate && kv.Key <= endDate)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Label für Summe
            lblRueckstandPlusMinusFourtyDays.Text = $"{dict.Values.Sum():0.00}h";

            UpdatePlusMinusTwoMonthsChart(dict, today);
        }

        /// <summary>
        /// Oberes Chart (Belag) aus Aggregat _aggBelagByDay. Vergangenheit: [selected..heute], Zukunft: [heute..selected].
        /// </summary>
        private void RefreshRueckstandVerguetenChart()
        {
            if (string.IsNullOrWhiteSpace(selectedBelag))
                return;

            DateTime selectedDate = dateTimePickerRueckstandAb.Value.Date;
            DateTime today = DateTime.Today;

            DateTime start = selectedDate <= today ? selectedDate : today;
            DateTime end = selectedDate <= today ? today : selectedDate;

            string clean = selectedBelag.Replace("-", "").ToUpper();

            // Belag-spezifische Tageswerte aus Aggregat
            var src = _aggBelagByDay.TryGetValue(clean, out var dictBelag) ? dictBelag : new Dictionary<DateTime, decimal>();

            var rangeDict = src
                .Where(kv => kv.Key >= start && kv.Key <= end)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Lücken auffüllen (0), damit Achse durchgängig ist
            var filled = FillMissingDates(rangeDict, start, end);

            bool singleDay = selectedDate == today;
            UpdateVerguetenChart(selectedBelag, filled, singleDay, selectedDate);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Diagramme / Charts (gestylt wie zuvor, nur Datenquelle jetzt Aggregate)
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Zeichnet das Belag-Chart (oben), inkl. dezenter Trendlinie.
        /// </summary>
        private void UpdateVerguetenChart(
            string belag,
            Dictionary<DateTime, decimal> rueckstandByDate,
            bool singleDay,
            DateTime selectedDate)
        {
            var chart = chartVergueten;

            if (chart.Series.IndexOf("Series1") == -1)
                chart.Series.Add(new Series("Series1"));

            Series series = chart.Series["Series1"];
            series.Points.Clear();
            series.ChartType = SeriesChartType.Column;

            // moderner, dezenter Stil
            series.Color = Color.FromArgb(90, 140, 240);
            series.BorderWidth = 0;
            series.IsValueShownAsLabel = true;
            series.LabelForeColor = Color.Black;
            series.LabelFormat = "0.00";
            series.LegendText = "Tagerückstand";
            series.ToolTip = "#AXISLABEL: #VAL{N2} h";

            ChartArea area = chart.ChartAreas[0];
            area.AxisX.MajorGrid.LineWidth = 0;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.LineColor = Color.LightGray;
            area.AxisY.LineColor = Color.LightGray;

            if (singleDay)
            {
                decimal total = rueckstandByDate.Values.Sum();
                series.Points.AddXY(belag, Math.Round(total, 2));

                lblVergueten.Text = $"{total:0.00}h";
                RemoveTrendLine();
                return;
            }

            foreach (var kv in rueckstandByDate.OrderBy(k => k.Key))
                series.Points.AddXY(kv.Key.ToString("dd.MM"), Math.Round(kv.Value, 2));

            decimal sum = rueckstandByDate.Values.Sum();
            lblVergueten.Text = $"{sum:0.00}h";

            // Trendlinie
            Series trend;
            if (chart.Series.IndexOf("Trend") == -1)
            {
                trend = new Series("Trend");
                trend.ChartType = SeriesChartType.Line;
                trend.BorderWidth = 2;
                trend.Color = Color.DimGray;
                trend.IsVisibleInLegend = true;
                trend.LegendText = "Trendlinie";
                chart.Series.Add(trend);
            }
            else trend = chart.Series["Trend"];

            trend.Points.Clear();

            var ordered = rueckstandByDate.OrderBy(x => x.Key).ToArray();
            var vals = ordered.Select(x => x.Value).ToArray();
            var dates = ordered.Select(x => x.Key).ToArray();

            int window = 2;
            for (int i = 0; i < vals.Length; i++)
            {
                decimal avg = 0; int count = 0;
                for (int w = i - window; w <= i + window; w++)
                {
                    if (w >= 0 && w < vals.Length) { avg += vals[w]; count++; }
                }
                avg /= Math.Max(1, count);

                trend.Points.AddXY(dates[i].ToString("dd.MM"), Math.Round(avg, 2));
            }

            void RemoveTrendLine()
            {
                if (chart.Series.IndexOf("Trend") != -1)
                    chart.Series["Trend"].Points.Clear();
            }
        }

        /// <summary>
        /// Zeichnet das ±40-Tage-Chart (unten), inkl. Trendlinien, Top-3 Markierung (dezent).
        /// </summary>
        private void UpdatePlusMinusTwoMonthsChart(
    Dictionary<DateTime, decimal> rueckstandByDate,
    DateTime selectedDate)
        {
            var chart = chartPlusMinusTwoMonths;

            DateTime startDate = selectedDate.AddDays(-40);
            DateTime endDate = selectedDate.AddDays(40);

            if (chart.Series.IndexOf("Series1") == -1)
                chart.Series.Add(new Series("Series1"));

            Series series = chart.Series["Series1"];
            series.Points.Clear();
            series.ChartType = SeriesChartType.Column;

            series.Color = Color.FromArgb(100, 150, 240);
            series.BorderWidth = 0;
            series.IsValueShownAsLabel = false;
            series.ToolTip = "#AXISLABEL: #VAL{N2} h";
            series.LegendText = "Tagerückstand";

            ChartArea area = chart.ChartAreas[0];
            area.AxisX.MajorGrid.LineWidth = 0;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.LineColor = Color.LightGray;
            area.AxisY.LineColor = Color.LightGray;

            // ----- Originales Befüllen (X = "dd.MM") -----
            var filled = FillMissingDates(rueckstandByDate, startDate, endDate);
            foreach (var kv in filled.OrderBy(k => k.Key))
                series.Points.AddXY(kv.Key.ToString("dd.MM"), Math.Round(kv.Value, 2));

            // Top-3 dezent markieren (unverändert)
            foreach (var p in series.Points.Cast<DataPoint>().OrderByDescending(x => x.YValues[0]).Take(3))
            {
                p.Color = Color.FromArgb(255, 240, 130, 130);
                p.MarkerStyle = MarkerStyle.Circle;
                p.MarkerSize = 8;
                p.MarkerColor = Color.DimGray;
                p.IsValueShownAsLabel = true;
                p.LabelForeColor = Color.DimGray;
                p.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            }

            // Trends (unverändert)
            Series trend1 = EnsureSeries(chart, "Trend1", SeriesChartType.Line, 2, Color.OrangeRed, "Trend (lang)");
            Series trend2 = EnsureSeries(chart, "Trend2", SeriesChartType.Line, 2, Color.DimGray, "Trend (kurz)");
            trend1.Points.Clear(); trend2.Points.Clear();

            var ordered = filled.OrderBy(x => x.Key).ToArray();
            var vals = ordered.Select(x => x.Value).ToArray();
            var days = ordered.Select(x => x.Key).ToArray();

            int w1 = 3; // 7-Punkte-Fenster
            for (int i = 0; i < vals.Length; i++)
            {
                decimal avg = 0; int cnt = 0; 
                for (int w = i - w1; w <= i + w1; w++) 
                    if (w >= 0 && w < vals.Length) { avg += vals[w]; cnt++; }

                avg /= Math.Max(1, cnt);
                trend1.Points.AddXY(days[i].ToString("dd.MM"), Math.Round(avg, 2));
            }

            for (int i = 0; i < vals.Length; i++)
            {
                decimal smooth = vals[i];
                if (i > 0 && i < vals.Length - 1)
                    smooth = (vals[i - 1] + vals[i] + vals[i + 1]) / 3m;

                trend2.Points.AddXY(days[i].ToString("dd.MM"), Math.Round(smooth, 2));
            }

            // =====================================================================
            // 🔹 NEU: Senkrechte „Heute“-Linie auf kategorialer X-Achse (dd.MM)
            // =====================================================================
            // 1) Heutiges Label wie bei den Balken berechnen:
            string todayLabel = DateTime.Today.ToString("dd.MM");

            // 2) Index (1-basiert) der heutigen Kategorie finden:
            int todayIndex1Based = -1;
            for (int i = 0; i < series.Points.Count; i++)
            {
                // Achtung: bei Kategorieachsen steckt das Kategoriename in AxisLabel
                if (string.Equals(series.Points[i].AxisLabel, todayLabel, StringComparison.Ordinal))
                {
                    todayIndex1Based = i + 1; // Chart verwendet Kategorieindex ab 1
                    break;
                }
            }

            // 3) Linie nur setzen, wenn „heute“ im Sichtfenster liegt:
            area.AxisX.StripLines.Clear();
            if (todayIndex1Based > 0)
            {
                var todayStrip = new StripLine
                {
                    Interval = 0,
                    IntervalOffset = todayIndex1Based,         // Kategorie-Position (1-basiert!)
                    StripWidth = 0,                            // nur Linie
                    BorderColor = Color.FromArgb(140, 140, 140),
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash
                };
                area.AxisX.StripLines.Add(todayStrip);
            }
            // =====================================================================
        }

        private Series EnsureSeries(Chart chart, string name, SeriesChartType type, int borderWidth, Color color, string legendText)
        {
            Series s;
            if (chart.Series.IndexOf(name) == -1)
            {
                s = new Series(name);
                s.ChartType = type;
                s.BorderWidth = borderWidth;
                s.Color = color;
                s.IsVisibleInLegend = true;
                s.LegendText = legendText;
                chart.Series.Add(s);
            }
            else
            {
                s = chart.Series[name];
                s.ChartType = type;
                s.BorderWidth = borderWidth;
                s.Color = color;
                s.IsVisibleInLegend = true;
                s.LegendText = legendText;
            }
            return s;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Hilfsfunktionen
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Füllt alle Tage im Bereich [start..end] auf, fehlende Tage = 0.
        /// </summary>
        private Dictionary<DateTime, decimal> FillMissingDates(Dictionary<DateTime, decimal> dict, DateTime start, DateTime end)
        {
            var res = new Dictionary<DateTime, decimal>();
            for (DateTime d = start.Date; d <= end.Date; d = d.AddDays(1))
                res[d] = dict.TryGetValue(d, out var v) ? v : 0m;

            return res;
        }

        /// <summary>
        /// Extrahiert und normalisiert die Belagsinformation aus einem AVO-Text (z. B. "Vergüten B-123" -> "B123").
        /// </summary>
        private string CleanBelag(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = System.Text.RegularExpressions.Regex
                .Replace(input, "vergüten", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                .Trim();

            var match = System.Text.RegularExpressions.Regex.Match(input, @"B-?\d+");
            if (match.Success)
                return match.Value.Replace("-", "").ToUpper();

            return "";
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Event-Handler für die ±1 Woche und ±2 Wochen Buttons (optional, je nach UI-Design)
        // -----------------------------------------------------------------------------------------------------------------

        private void btnEineWocheZurueck_Click(object sender, EventArgs e)
        {
            // den DatePicker um 7 Tage zurücksetzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(-7);
        }

        private void btnEineWocheVor_Click(object sender, EventArgs e)
        {
            // den DatePicker um 7 Tage vorwärts setzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(7);
        }

        private void btnZweiWochenZurueck_Click(object sender, EventArgs e)
        {
            // den DatePicker um 14 Tage zurücksetzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(-14);
        }

        private void btnZweiWochenVor_Click(object sender, EventArgs e)
        {
            // den DatePicker um 14 Tage vorwärts setzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(14);
        }

        private void btnResett_Click(object sender, EventArgs e)
        {
            // DatePicker auf heute zurücksetzen
            dateTimePickerRueckstandAb.Value = DateTime.Today;
        }
    }
}
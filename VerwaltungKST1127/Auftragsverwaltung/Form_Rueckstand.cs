// ===================================================================================================
// "using"-Anweisungen importieren Funktionen aus den .NET-Bibliotheken (Sammlungen von fertigem Code).
// Damit müssen wir z. B. nicht "System.String" schreiben, sondern können einfach "string" verwenden.
// ===================================================================================================
using System;                                           // Basis-Typen wie String, DateTime, Math
using System.Collections.Generic;                       // List<T>, Dictionary<TKey,TValue>, IEnumerable<T>
using System.Data;                                      // DataTable, DataRow, DataSet – ADO.NET Kern (Daten in Tabellenform)
using System.Drawing;                                   // Farben, Grafiken, Sizes, Points für UI
using System.Linq;                                      // LINQ-Funktionen (Where, Select, Sum, OrderBy ...)
using System.Data.SqlClient;                            // SQL-Server Zugriff (SqlConnection, SqlCommand, ...)
using System.Windows.Forms;                             // Windows-Forms (Fenster, Buttons, DataGridView ...)
using System.Windows.Forms.DataVisualization.Charting;  // Chart-Steuerelemente (Diagramme)
using System.Threading.Tasks;                           // async/await, Task.Run – für Hintergrund-Aufgaben

// "namespace" ist wie ein Ordner für Klassen. Verhindert Namenskonflikte zwischen Projekten.
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
    // "public partial class" = öffentliche Klasse, deren Code auf mehrere Dateien aufgeteilt ist
    // (z. B. Form_Rueckstand.cs + Form_Rueckstand.Designer.cs für die UI-Definitionen).
    // Sie erbt von "Form" und ist damit selbst ein Windows-Fenster.
    public partial class Form_Rueckstand : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Konfiguration: Datenfenster für den einmaligen LN-Ladevorgang
        // "const" = Konstante, kann sich nie ändern. Wird zur Compile-Zeit eingesetzt.
        // -----------------------------------------------------------------------------------------------------------------
        private const int PreloadPastDays = 180;   // 180 Tage Historie ab heute rückwärts laden
        private const int PreloadFutureDays = 60;  // 60 Tage in die Zukunft (für ±40 Tage-Chart ausreichend)

        // -----------------------------------------------------------------------------------------------------------------
        // Felder = Variablen, die zur Klasse gehören und in jeder Methode dieser Klasse sichtbar sind.
        // "private" = nur innerhalb dieser Klasse sichtbar.
        // "readonly" = darf nur einmal beim Anlegen oder im Konstruktor gesetzt werden.
        // -----------------------------------------------------------------------------------------------------------------

        // Tabelle mit den Belägen (Eingabe von außen, wurde dem Konstruktor übergeben).
        private readonly DataTable belagData;

        // Verbindung zur SQL-Server-Datenbank. "new(...)" ist die Kurzform für "new SqlConnection(...)".
        // Der String dahinter ist der "Connection-String": Server, Datenbank, Authentifizierung.
        private readonly SqlConnection sqlConnectionVerwaltung =
            new(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // Welcher Belag gerade in der Tabelle angeklickt wurde. Anfangs null = nichts ausgewählt.
        private string selectedBelag;

        // --- Caches & Aggregate (werden beim Start genau einmal aufgebaut) ---
        // Caches sind Zwischenspeicher: Wir laden Daten einmal aus der DB und merken sie uns,
        // damit wir die DB nicht ständig neu fragen müssen → schneller.

        /// <summary>Serienlinsen-Cache: (Artikel, Seite, Belag) -> (Stück/Charge, Zeit/Charge)</summary>
        // Dictionary = Nachschlagewerk wie ein Telefonbuch: zu einem "Schlüssel" gibt es einen "Wert".
        // Hier ist der Schlüssel ein Tupel (Artikel, Seite, Belag), der Wert ein Tupel (StkCharge, Zeit).
        private Dictionary<(string Art, int Seite, string Belag), (decimal StkCharge, decimal Zeit)> _serienlinsenCache
            = new Dictionary<(string, int, string), (decimal, decimal)>();

        /// <summary>Rückstand gesamt (h) pro Belag bis heute, mit korrekter Logik: VorStk - IstStk</summary>
        // OrdinalIgnoreCase = Groß-/Kleinschreibung wird beim Vergleich der Schlüssel ignoriert ("B123" == "b123").
        private Dictionary<string, decimal> _aggBelagRueckstandBisHeuteHours
            = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Soll-Rückstand (h) pro Belag bis heute (komplette Sollmenge in Stunden)</summary>
        private Dictionary<string, decimal> _aggBelagSollBisHeuteHours
            = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Rückstand (h) aller Beläge pro Tag (für das ±40 Tage Chart)</summary>
        private Dictionary<DateTime, decimal> _aggRueckstandAllBelagsByDay
            = new Dictionary<DateTime, decimal>();

        /// <summary>Rückstand (h) pro Belag pro Tag (für das obere Belag-Chart)</summary>
        // Verschachteltes Dictionary: erst über Belag-Namen, dann pro Tag.
        private Dictionary<string, Dictionary<DateTime, decimal>> _aggBelagByDay
            = new Dictionary<string, Dictionary<DateTime, decimal>>(StringComparer.OrdinalIgnoreCase);

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktoren = Methoden, die beim "new Form_Rueckstand(...)" automatisch laufen
        // und das Objekt initialisieren.
        // -----------------------------------------------------------------------------------------------------------------

        // Parameterloser Konstruktor – ruft den Hauptkonstruktor mit "null" auf (": this(null)").
        public Form_Rueckstand() : this(null) { }

        // Hauptkonstruktor mit einer Belag-Tabelle als Eingabe.
        public Form_Rueckstand(DataTable belagData)
        {
            // Erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei).
            InitializeComponent();

            // Eingabe-DataTable im Feld speichern, damit alle Methoden sie verwenden können.
            this.belagData = belagData;

            // Event-Handler "anhängen": "+="" registriert eine Methode, die bei einem Ereignis aufgerufen wird.
            Load += Form_Rueckstand_Load;                                            // beim Öffnen des Formulars
            dGv_AuswahlBelag.CellClick += dGv_AuswahlBelag_CellClick;                // Klick auf Tabellenzelle
            dateTimePickerRueckstandAb.ValueChanged += dateTimePickerRueckstandAb_ValueChanged; // Datum geändert
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Formular-Initialisierung
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Startet UI sofort, lädt Daten & Aggregate im Hintergrund, füllt danach Grid/Charts.
        /// </summary>
        // "async" = Methode kann auf langsame Operationen warten, ohne die UI einzufrieren.
        private async void Form_Rueckstand_Load(object sender, EventArgs e)
        {
            // 1) UI-Grundaufbau (lokal, schnell)
            LoadBelagData();
            dateTimePickerRueckstandAb.Value = DateTime.Today; // DatePicker auf heute setzen

            // 2) Daten + Aggregate in Hintergrund-Thread aufbauen.
            //    "await Task.Run(...)" startet eine Methode auf einem anderen Thread und wartet,
            //    bis sie fertig ist. So bleibt das Fenster bedienbar während die DB-Abfrage läuft.
            await Task.Run(BuildAllCachesAndAggregates);

            // 3) Danach (wieder im UI-Thread!): Grid-Spalten mit Cache-Werten befüllen + Charts aktualisieren.
            FillBelagTotalsFromCaches();
            UpdateGesamtRueckstandAbteilungLabel();      // Summen-Label oben aktualisieren
            UpdateGesamtRueckstand();                    // nutzt Aggregat (keine DB)
            RefreshRueckstandPlusMinusTwoMonthsChart();  // unteres Chart aus Aggregat (keine DB)
        }

        // -----------------------------------------------------------------------------------------------------------------
        // UI: Grid & DatePicker
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Bindet das Belag-Grid und legt die zwei zusätzlichen Spalten an (Rückstand gesamt, Soll-Rückstand).
        /// </summary>
        private void LoadBelagData()
        {
            // Wenn keine Daten übergeben wurden, Grid leer lassen und Methode beenden.
            if (belagData == null)
            {
                dGv_AuswahlBelag.DataSource = null;
                return;
            }

            // ".Copy()" liefert eine unabhängige Kopie der Tabelle – Änderungen im Grid wirken nicht zurück.
            dGv_AuswahlBelag.DataSource = belagData.Copy();

            // Spalten zentriert ausrichten – nur falls die Spalte überhaupt existiert.
            if (dGv_AuswahlBelag.Columns.Contains("Realer Rücks."))
                dGv_AuswahlBelag.Columns["Realer Rücks."].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

            if (dGv_AuswahlBelag.Columns.Contains("Brutto Rücks."))
                dGv_AuswahlBelag.Columns["Brutto Rücks."].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

            // Grid "schreibgeschützt" machen + Optik konfigurieren.
            dGv_AuswahlBelag.AllowUserToAddRows = false;        // keine leere Zeile am Ende
            dGv_AuswahlBelag.AllowUserToDeleteRows = false;     // Zeilen können nicht gelöscht werden
            dGv_AuswahlBelag.AllowUserToResizeColumns = false;  // Spaltenbreite fix
            dGv_AuswahlBelag.AllowUserToResizeRows = false;     // Zeilenhöhe fix
            dGv_AuswahlBelag.ReadOnly = true;                   // keine Bearbeitung möglich
            dGv_AuswahlBelag.RowHeadersVisible = false;         // den linken grauen Balken ausblenden
            dGv_AuswahlBelag.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // immer ganze Zeile markieren
            dGv_AuswahlBelag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Spalten füllen Breite

            // Zusatzspalten "Realer Rücks." und "Brutto Rücks." anlegen, falls nicht vorhanden.
            EnsureDgvTotalsColumns();

            // Optionale Spalte "AVOs": als Ganzzahl ohne Nachkommastellen, zentriert.
            if (dGv_AuswahlBelag.Columns.Contains("AVOs"))
            {
                dGv_AuswahlBelag.Columns["AVOs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGv_AuswahlBelag.Columns["AVOs"].DefaultCellStyle.Format = "N0";
            }

            // Alle Spaltenüberschriften zentrieren.
            foreach (DataGridViewColumn c in dGv_AuswahlBelag.Columns)
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Legt die beiden Zusatzspalten an: "Realer Rücks." (echter Rückstand in h) und "Brutto Rücks." (Soll in h).
        /// </summary>
        private void EnsureDgvTotalsColumns()
        {
            // Spalte 1: Realer Rückstand
            const string ColRueck = "Realer Rücks.";
            if (!dGv_AuswahlBelag.Columns.Contains(ColRueck)) // nur anlegen, wenn noch nicht da
            {
                var col = new DataGridViewTextBoxColumn();
                col.Name = ColRueck;
                col.HeaderText = "Realer Rücks.";
                col.ReadOnly = true;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // Breite passt sich an Inhalt an

                // Style "DefaultCellStyle" gilt für alle Zellen dieser Spalte:
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Format = "N2"; // 2 Nachkommastellen (z. B. 12,34)

                dGv_AuswahlBelag.Columns.Add(col);
            }

            // Spalte 2: Brutto Rückstand (analog)
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
        /// Klick auf Belag-Zeile → Belag merken + oberes Chart für diesen Belag aktualisieren.
        /// </summary>
        private void dGv_AuswahlBelag_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Wenn Header (RowIndex = -1) oder ungültige Zeile angeklickt → nichts tun.
            if (e.RowIndex < 0) return;

            // ?. = "null-bedingter Operator": ".ToString()" wird nur aufgerufen, wenn Value nicht null ist.
            string belag = dGv_AuswahlBelag.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(belag)) return; // leere/whitespace-Strings ignorieren

            selectedBelag = belag;
            RefreshRueckstandVerguetenChart(); // oberes Chart aus Cache neu zeichnen
        }

        /// <summary>
        /// DatePicker geändert → Summen + Belag-Chart neu (nutzt Aggregate, keine DB).
        /// </summary>
        private void dateTimePickerRueckstandAb_ValueChanged(object sender, EventArgs e)
        {
            UpdateGesamtRueckstand(); // Summe bis Stichtag (aus _aggRueckstandAllBelagsByDay)

            // Nur wenn ein Belag ausgewählt wurde, das obere Chart neu zeichnen.
            if (!string.IsNullOrWhiteSpace(selectedBelag))
                RefreshRueckstandVerguetenChart();
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
            // 1) Stammdaten der Serienlinsen einmalig in den Cache laden.
            LoadSerienlinsenCache();

            // 2) Vergüten-Zeilen im konfigurierten Zeitfenster laden.
            DateTime today = DateTime.Today;
            DateTime start = today.AddDays(-PreloadPastDays);
            DateTime end = today.AddDays(PreloadFutureDays);

            DataTable lnTable = LoadLnVerguetenWindow(start, end);

            // 3) Beläge aus dem Grid in ein HashSet packen (für schnelles "ist enthalten?"-Prüfen).
            //    HashSet ist optimiert für Contains-Abfragen.
            var belagSet = ExtractBelagSetFromGrid();

            // 4) Lokale Aggregate vorbereiten (am Ende kopieren wir sie in die Felder).
            var belagRueckBisHeute = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var belagSollBisHeute = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var allByDay = new Dictionary<DateTime, decimal>();
            var belagByDay = new Dictionary<string, Dictionary<DateTime, decimal>>(StringComparer.OrdinalIgnoreCase);

            // Über jede Zeile der DB-Antwort iterieren.
            foreach (DataRow row in lnTable.Rows)
            {
                // Zeilen ohne Enddatum überspringen ("DBNull" = SQL NULL).
                if (row["Enddatum"] == DBNull.Value)
                    continue;

                // Werte aus der Zeile in passende C#-Typen konvertieren.
                DateTime day = Convert.ToDateTime(row["Enddatum"]).Date;             // nur Datumsteil, ohne Uhrzeit
                string artikel = row["Artikel"]?.ToString() ?? "";                   // ?? "" = falls null, dann ""
                string avoInfo = row["AVOinfo"]?.ToString() ?? "";
                int seite = row["Seite"] == DBNull.Value ? 0 : Convert.ToInt32(row["Seite"]);
                decimal ist = row["IstStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["IstStk"]);   // 0m = decimal-Null
                decimal vor = row["VorStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["VorStk"]);
                decimal soll = row["SollStk"] == DBNull.Value ? 0m : Convert.ToDecimal(row["SollStk"]);

                // Belag-Bezeichnung aus dem AVO-Text extrahieren und normalisieren ("B-123" → "B123").
                string belag = CleanBelag(avoInfo);
                if (string.IsNullOrWhiteSpace(belag))
                    continue;

                // Wenn wir eine Belag-Filterliste haben, Beläge außerhalb davon überspringen.
                if (belagSet != null && belagSet.Count > 0 && !belagSet.Contains(belag))
                    continue;

                // Stammdaten für diesen (Artikel, Seite, Belag)-Schlüssel im Cache nachschlagen.
                var key = (artikel, seite, belag);
                if (!_serienlinsenCache.TryGetValue(key, out var param))
                    continue; // ohne Stammdaten keine Zeitberechnung möglich

                // Wenn StkCharge oder Zeit 0/negativ ist → keine sinnvolle Berechnung möglich.
                if (param.StkCharge <= 0 || param.Zeit <= 0)
                    continue;

                // ECHTER Rückstand in Stück: Vorbereitet - bereits Vergütet.
                decimal rueckStk = vor - ist;
                if (rueckStk > 0)
                {
                    // Rückstand in Stunden umrechnen: (Stück / Stück_pro_Charge) * Zeit_pro_Charge.
                    decimal rueckZeit = (rueckStk / param.StkCharge) * param.Zeit;

                    // a) pro Belag pro Tag (für oberes Chart)
                    if (!belagByDay.TryGetValue(belag, out var dict))
                    {
                        dict = new Dictionary<DateTime, decimal>();
                        belagByDay[belag] = dict;
                    }
                    if (!dict.ContainsKey(day)) dict[day] = 0m;
                    dict[day] += Math.Round(rueckZeit, 4); // auf 4 Nachkommastellen runden

                    // b) gesamt pro Tag (für ±40-Chart, enthält ALLE Beläge)
                    if (!allByDay.ContainsKey(day)) allByDay[day] = 0m;
                    allByDay[day] += Math.Round(rueckZeit, 4);

                    // c) bis HEUTE → Summen pro Belag (für DGV-Spalte „Realer Rücks.“)
                    if (day <= today)
                    {
                        if (!belagRueckBisHeute.ContainsKey(belag)) belagRueckBisHeute[belag] = 0m;
                        belagRueckBisHeute[belag] += Math.Round(rueckZeit, 4);
                    }
                }

                // SOLL-Rückstand: komplette Sollmenge in Stunden, nur bis HEUTE.
                if (day <= today && soll > 0)
                {
                    decimal sollZeit = (soll / param.StkCharge) * param.Zeit;
                    if (!belagSollBisHeute.ContainsKey(belag)) belagSollBisHeute[belag] = 0m;
                    belagSollBisHeute[belag] += Math.Round(sollZeit, 4);
                }
            }

            // 5) Lokale Aggregate in die Felder der Klasse übernehmen.
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

            // SQL-Abfrage: Serienlinsen-Stammdaten holen.
            // REPLACE(... , '-', '') entfernt Bindestriche (z. B. "B-123" → "B123").
            string sql = @"
                SELECT 
                    ARTNR,
                    SEITE,
                    REPLACE(VERGBELAG,'-','') AS BelagVerg,
                    REPLACE(Belag1,'-','')    AS BelagAlt,
                    STK_CHARGE,
                    CHARGENZEIT
                FROM Serienlinsen;";

            // "using" sorgt dafür, dass das SqlCommand am Ende automatisch korrekt freigegeben wird.
            using (var cmd = new SqlCommand(sql, sqlConnectionVerwaltung))
            {
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);

                // Verbindung öffnen, Tabelle füllen, Verbindung wieder schließen.
                sqlConnectionVerwaltung.Open();
                da.Fill(dt);
                sqlConnectionVerwaltung.Close();

                // Jede Zeile in den Cache übernehmen (1 oder 2 Einträge je Zeile).
                foreach (DataRow r in dt.Rows)
                {
                    string art = r["ARTNR"]?.ToString() ?? "";
                    // TryParse: liefert "true" + Wert in "result", wenn die Konvertierung klappt.
                    int seite = int.TryParse(r["SEITE"]?.ToString(), out var result) ? result : 0;
                    string belagVerg = (r["BelagVerg"]?.ToString() ?? "").ToUpper();
                    string belagAlt = (r["BelagAlt"]?.ToString() ?? "").ToUpper();

                    decimal stk = r["STK_CHARGE"] == DBNull.Value ? 0m : Convert.ToDecimal(r["STK_CHARGE"]);
                    decimal zeit = r["CHARGENZEIT"] == DBNull.Value ? 0m : Convert.ToDecimal(r["CHARGENZEIT"]);
                    var val = (stk, zeit);

                    // Beide Belag-Schreibweisen als Schlüssel anlegen, damit beide Varianten gefunden werden.
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

            // SQL mit Parametern (@Start, @End) – KEINE String-Verkettung, das schützt vor SQL-Injection.
            // OUTER APPLY: hängt pro Zeile a einen verwandten "Vorbereiten"-Vorstk-Wert dran (falls vorhanden).
            // CASE: liest aus dem Text "Vergüten Iii", "II" usw. die Seite (0/1/2) heraus.
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
                // Parameterwerte sicher übergeben.
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);

                var da = new SqlDataAdapter(cmd);

                sqlConnectionVerwaltung.Open();
                da.Fill(dt);
                sqlConnectionVerwaltung.Close();
            }

            return dt; // gefüllte Tabelle zurückgeben
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

                // Bindestriche entfernen + Großbuchstaben → einheitliche Schreibweise zum Vergleichen.
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

            // Wenn eine der beiden Spalten fehlt, abbrechen.
            if (!dGv_AuswahlBelag.Columns.Contains(ColRueck) ||
                !dGv_AuswahlBelag.Columns.Contains(ColSoll))
                return;

            // Jede Zeile durchgehen.
            foreach (DataGridViewRow row in dGv_AuswahlBelag.Rows)
            {
                if (row.IsNewRow) continue; // die leere "Neue Zeile" überspringen

                string belag = row.Cells["Belag"]?.Value?.ToString();
                if (string.IsNullOrWhiteSpace(belag))
                {
                    row.Cells[ColRueck].Value = null;
                    row.Cells[ColSoll].Value = null;
                    continue;
                }

                // Belag-Schlüssel normalisieren (so wie beim Cache-Aufbau).
                string clean = belag.Replace("-", "").ToUpper();

                // TryGetValue: Wert holen wenn vorhanden, sonst 0.
                decimal rueck = _aggBelagRueckstandBisHeuteHours.TryGetValue(clean, out var rr) ? rr : 0m;
                decimal soll = _aggBelagSollBisHeuteHours.TryGetValue(clean, out var sr) ? sr : 0m;

                row.Cells[ColRueck].Value = Math.Round(rueck, 2);
                row.Cells[ColSoll].Value = Math.Round(soll, 2);
            }
        }

        /// <summary>
        /// Summiert die Spalte "Brutto Rücks." im DGV und zeigt das Ergebnis als Stunden im Label an.
        /// </summary>
        private void UpdateGesamtRueckstandAbteilungLabel()
        {
            const string SollCol = "Brutto Rücks.";
            if (!dGv_AuswahlBelag.Columns.Contains(SollCol))
            {
                lblGesamtRueckstandAbteilung.Text = "0,00h";
                return;
            }

            decimal sum = 0m; // 0m = decimal-Null
            foreach (DataGridViewRow row in dGv_AuswahlBelag.Rows)
            {
                if (row.IsNewRow) continue;
                object val = row.Cells[SollCol].Value;
                if (val == null || val == DBNull.Value) continue;

                // Wert in decimal umwandeln; nur addieren, wenn Konvertierung klappt.
                if (decimal.TryParse(Convert.ToString(val), out decimal d))
                    sum += d;
            }

            // String-Interpolation: $"...{sum:0.00}h..." formatiert die Zahl.
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

            // LINQ-Kette:
            // .Where(...)  filtert auf alle Tage <= Stichtag
            // .Sum(...)    summiert die Werte
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

            // Alle Tage aus dem Aggregat im Zeitfenster auf ein Dictionary reduzieren.
            var dict = _aggRueckstandAllBelagsByDay
                .Where(kv => kv.Key >= startDate && kv.Key <= endDate)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Summe als Label anzeigen.
            lblRueckstandPlusMinusFourtyDays.Text = $"{dict.Values.Sum():0.00}h";

            // Chart zeichnen.
            UpdatePlusMinusTwoMonthsChart(dict, today);
        }

        /// <summary>
        /// Oberes Chart (Belag) aus Aggregat _aggBelagByDay.
        /// Vergangenheit: [selected..heute], Zukunft: [heute..selected].
        /// </summary>
        private void RefreshRueckstandVerguetenChart()
        {
            if (string.IsNullOrWhiteSpace(selectedBelag))
                return;

            DateTime selectedDate = dateTimePickerRueckstandAb.Value.Date;
            DateTime today = DateTime.Today;

            // Bereich so wählen, dass Start <= Ende ist (egal ob Stichtag in Vergangenheit oder Zukunft).
            DateTime start = selectedDate <= today ? selectedDate : today;
            DateTime end = selectedDate <= today ? today : selectedDate;

            string clean = selectedBelag.Replace("-", "").ToUpper();

            // Belag-spezifische Tageswerte aus Aggregat (oder leeres Dict, falls Belag unbekannt).
            var src = _aggBelagByDay.TryGetValue(clean, out var dictBelag)
                        ? dictBelag
                        : new Dictionary<DateTime, decimal>();

            // Zeitbereich filtern.
            var rangeDict = src
                .Where(kv => kv.Key >= start && kv.Key <= end)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Lücken (Tage ohne Wert) mit 0 auffüllen, damit die X-Achse durchgehend ist.
            var filled = FillMissingDates(rangeDict, start, end);

            // Sonderfall: Stichtag = heute → nur ein einzelner Balken (Single-Day-Modus).
            bool singleDay = selectedDate == today;
            UpdateVerguetenChart(selectedBelag, filled, singleDay, selectedDate);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Diagramme / Charts (modernisiertes Design)
        // -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Zeichnet das Belag-Chart (oben) – modernisiertes Design.
        /// Funktionen unverändert: Single-Day-Anzeige, Tagesbalken mit Wertelabel,
        /// Trendlinie (5-Pkt-Fenster), ToolTips, Summen-Label.
        /// </summary>
        private void UpdateVerguetenChart(
            string belag,
            Dictionary<DateTime, decimal> rueckstandByDate,
            bool singleDay,
            DateTime selectedDate)
        {
            // Verweis aufs Chart-Objekt – kürzere Schreibweise.
            var chart = chartVergueten;

            // =============================================================
            // Globale Chart-Optik: Hintergrund, Rahmen, Anti-Aliasing
            // =============================================================
            chart.AntiAliasing = AntiAliasingStyles.All;                 // Kanten glätten (alles)
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High; // Text scharf
            chart.BackColor = Color.White;                               // primärer Hintergrund
            chart.BackSecondaryColor = Color.FromArgb(248, 250, 253);    // sekundär (für Verlauf)
            chart.BackGradientStyle = GradientStyle.TopBottom;           // dezenter Top-Down-Verlauf
            chart.BorderlineColor = Color.FromArgb(225, 230, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.None;                      // keine eingebaute Farbpalette

            // Legende modernisieren (rechts angedockt).
            if (chart.Legends.Count > 0)
            {
                var lg = chart.Legends[0];
                lg.Docking = Docking.Right;
                lg.Alignment = StringAlignment.Center;
                lg.BackColor = Color.Transparent;
                lg.BorderColor = Color.Transparent;
                lg.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                lg.ForeColor = Color.FromArgb(80, 90, 105);
            }

            // ChartArea = der eigentliche "Plot-Bereich" mit Achsen.
            ChartArea area = chart.ChartAreas[0];
            area.BackColor = Color.Transparent;
            area.BackSecondaryColor = Color.Transparent;
            area.BorderColor = Color.Transparent;

            // X-Achse (waagerecht): keine Major-Gridlines, dünne Linie, kleine Schrift.
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LineColor = Color.FromArgb(220, 225, 235);
            area.AxisX.LineWidth = 1;
            area.AxisX.MajorTickMark.Enabled = false;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisX.LabelStyle.ForeColor = Color.FromArgb(110, 120, 135);

            // Y-Achse (senkrecht): zarte horizontale Grid-Linien, Werte in "h".
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 238, 244);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.LineColor = Color.Transparent;
            area.AxisY.MajorTickMark.Enabled = false;
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisY.LabelStyle.ForeColor = Color.FromArgb(110, 120, 135);
            area.AxisY.LabelStyle.Format = "0.# h";
            area.AxisY.IsStartedFromZero = true;

            // Mehr "Atemraum" rund um den Plot (Werte in % der ChartArea).
            area.InnerPlotPosition.Auto = false;
            area.InnerPlotPosition.X = 4;
            area.InnerPlotPosition.Y = 6;
            area.InnerPlotPosition.Width = 88;
            area.InnerPlotPosition.Height = 82;

            // =============================================================
            // Hauptserie: Tagerückstand (Balken)
            // =============================================================
            // Falls Serie noch nicht existiert, anlegen.
            if (chart.Series.IndexOf("Series1") == -1)
                chart.Series.Add(new Series("Series1"));

            Series series = chart.Series["Series1"];
            series.Points.Clear();                              // alte Daten entfernen
            series.ChartType = SeriesChartType.Column;          // Säulendiagramm
            series["PointWidth"] = "0.55";                      // Balkenbreite (von 0..1)
            series["DrawingStyle"] = "Default";
            series.Color = Color.FromArgb(95, 145, 235);        // Hauptfarbe oben
            series.BackSecondaryColor = Color.FromArgb(165, 200, 250); // unten heller (Verlauf)
            series.BackGradientStyle = GradientStyle.TopBottom;
            series.BorderWidth = 0;
            series.IsValueShownAsLabel = true;                  // Zahl über jedem Balken
            series.LabelForeColor = Color.FromArgb(60, 70, 85);
            series.Font = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold);
            series.LabelFormat = "0.00";                        // 2 Nachkommastellen
            series.LegendText = "Tagerückstand";
            series.ToolTip = "#AXISLABEL: #VAL{N2} h";          // Tooltip beim Hovern

            // =============================================================
            // Single-Day-Modus: nur ein einzelner Balken mit Belag-Namen
            // =============================================================
            if (singleDay)
            {
                decimal total = rueckstandByDate.Values.Sum();
                series.Points.AddXY(belag, Math.Round(total, 2));

                lblVergueten.Text = $"{total:0.00}h";

                // Trend-Serie leeren (im Single-Day nicht sinnvoll).
                if (chart.Series.IndexOf("Trend") != -1)
                    chart.Series["Trend"].Points.Clear();

                return;
            }

            // =============================================================
            // Multi-Day: Tagesbalken befüllen (chronologisch sortiert)
            // =============================================================
            foreach (var kv in rueckstandByDate.OrderBy(k => k.Key))
                series.Points.AddXY(kv.Key.ToString("dd.MM"), Math.Round(kv.Value, 2));

            decimal sum = rueckstandByDate.Values.Sum();
            lblVergueten.Text = $"{sum:0.00}h";

            // Bei vielen Tagen die X-Achsen-Beschriftung ausdünnen (jede 2. oder 5. Beschriftung).
            int pointCount = series.Points.Count;
            area.AxisX.Interval = pointCount > 30 ? 5 : (pointCount > 14 ? 2 : 1);

            // =============================================================
            // Trendlinie als Spline (sanft geschwungen)
            // =============================================================
            Series trend = EnsureSeries(chart, "Trend", SeriesChartType.Spline, 2,
                                         Color.FromArgb(95, 105, 120), "Trendlinie");
            trend.Points.Clear();
            trend.ShadowOffset = 0;
            trend.BorderDashStyle = ChartDashStyle.Solid;
            trend["LineTension"] = "0.6"; // 0 = eckig, 1 = sehr rund

            // Daten in Arrays übertragen, damit wir per Index drüberlaufen können.
            var ordered = rueckstandByDate.OrderBy(x => x.Key).ToArray();
            var vals = ordered.Select(x => x.Value).ToArray();
            var dates = ordered.Select(x => x.Key).ToArray();

            // Gleitender Mittelwert: für jeden Punkt i den Durchschnitt von [i-2..i+2].
            int window = 2; // 5-Punkte-Fenster
            for (int i = 0; i < vals.Length; i++)
            {
                decimal avg = 0; int count = 0;
                for (int w = i - window; w <= i + window; w++)
                {
                    if (w >= 0 && w < vals.Length) { avg += vals[w]; count++; }
                }
                avg /= Math.Max(1, count); // schützt vor Division durch 0

                trend.Points.AddXY(dates[i].ToString("dd.MM"), Math.Round(avg, 2));
            }
        }

        /// <summary>
        /// Zeichnet das ±40-Tage-Chart (unten) – modernisiertes Design.
        /// Funktionen unverändert: Tagesbalken, Trend (lang) 7-Pkt-MA, Trend (kurz) 3-Pkt-Glättung,
        /// Top-3 Hervorhebung, senkrechte „Heute"-Linie, ToolTips.
        /// </summary>
        private void UpdatePlusMinusTwoMonthsChart(
            Dictionary<DateTime, decimal> rueckstandByDate,
            DateTime selectedDate)
        {
            var chart = chartPlusMinusTwoMonths;

            // Datumsfenster ±40 Tage rund um den Stichtag.
            DateTime startDate = selectedDate.AddDays(-40);
            DateTime endDate = selectedDate.AddDays(40);

            // =============================================================
            // Globale Chart-Optik (analog zum oberen Chart)
            // =============================================================
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            chart.BackColor = Color.White;
            chart.BackSecondaryColor = Color.FromArgb(248, 250, 253);
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineColor = Color.FromArgb(225, 230, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.None;

            // Legende rechts platzieren + dezent stylen.
            if (chart.Legends.Count > 0)
            {
                var lg = chart.Legends[0];
                lg.Docking = Docking.Right;
                lg.Alignment = StringAlignment.Center;
                lg.BackColor = Color.Transparent;
                lg.BorderColor = Color.Transparent;
                lg.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                lg.ForeColor = Color.FromArgb(80, 90, 105);
            }

            // Plot-Bereich.
            ChartArea area = chart.ChartAreas[0];
            area.BackColor = Color.Transparent;
            area.BackSecondaryColor = Color.Transparent;
            area.BorderColor = Color.Transparent;

            // X-Achse: Beschriftung jeden 5. Tag (Datumsdichte reduzieren).
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LineColor = Color.FromArgb(220, 225, 235);
            area.AxisX.LineWidth = 1;
            area.AxisX.MajorTickMark.Enabled = false;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisX.LabelStyle.ForeColor = Color.FromArgb(110, 120, 135);
            area.AxisX.Interval = 5;

            // Y-Achse: feine horizontale Gridlines.
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 238, 244);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.LineColor = Color.Transparent;
            area.AxisY.MajorTickMark.Enabled = false;
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            area.AxisY.LabelStyle.ForeColor = Color.FromArgb(110, 120, 135);
            area.AxisY.LabelStyle.Format = "0.# h";
            area.AxisY.IsStartedFromZero = true;

            area.InnerPlotPosition.Auto = false;
            area.InnerPlotPosition.X = 4;
            area.InnerPlotPosition.Y = 4;
            area.InnerPlotPosition.Width = 88;
            area.InnerPlotPosition.Height = 86;

            // =============================================================
            // Hauptserie: Tagerückstand
            // =============================================================
            if (chart.Series.IndexOf("Series1") == -1)
                chart.Series.Add(new Series("Series1"));

            Series series = chart.Series["Series1"];
            series.Points.Clear();
            series.ChartType = SeriesChartType.Column;
            series["PointWidth"] = "0.55";
            series["DrawingStyle"] = "Default";
            series.Color = Color.FromArgb(95, 145, 235);
            series.BackSecondaryColor = Color.FromArgb(165, 200, 250);
            series.BackGradientStyle = GradientStyle.TopBottom;
            series.BorderWidth = 0;
            series.IsValueShownAsLabel = false;                 // Standard ohne Labels (nur Top-3 bekommt welche)
            series.ToolTip = "#AXISLABEL: #VAL{N2} h";
            series.LegendText = "Tagerückstand";

            // Lücken auffüllen + Daten chronologisch eintragen.
            var filled = FillMissingDates(rueckstandByDate, startDate, endDate);
            foreach (var kv in filled.OrderBy(k => k.Key))
                series.Points.AddXY(kv.Key.ToString("dd.MM"), Math.Round(kv.Value, 2));

            // =============================================================
            // Top-3 Hervorhebung: die 3 höchsten Tagesbalken einfärben + beschriften
            // =============================================================
            // .Cast<DataPoint>()  → typisierte Sequenz
            // .Where(...)         → nur positive Werte
            // .OrderByDescending  → größter Wert zuerst
            // .Take(3)            → die ersten 3 nehmen
            foreach (var p in series.Points.Cast<DataPoint>()
                                            .Where(x => x.YValues[0] > 0)
                                            .OrderByDescending(x => x.YValues[0])
                                            .Take(3))
            {
                p.Color = Color.FromArgb(235, 105, 110);            // rote Balkenfarbe
                p.BackSecondaryColor = Color.FromArgb(255, 170, 170);
                p.BackGradientStyle = GradientStyle.TopBottom;
                p.MarkerStyle = MarkerStyle.Circle;
                p.MarkerSize = 9;
                p.MarkerColor = Color.FromArgb(80, 90, 105);
                p.MarkerBorderColor = Color.White;                  // weißer Rand → "Pillen-Look"
                p.MarkerBorderWidth = 2;
                p.IsValueShownAsLabel = true;
                p.LabelForeColor = Color.FromArgb(60, 70, 85);
                p.LabelBackColor = Color.Transparent;
                p.Font = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold);
            }

            // =============================================================
            // Zwei Trendlinien als Splines (lang & kurz)
            // =============================================================
            // EnsureSeries: holt Serie oder legt sie an, Stil immer aktualisiert.
            Series trend1 = EnsureSeries(chart, "Trend1", SeriesChartType.Spline, 3,
                                          Color.FromArgb(235, 110, 95), "Trend (lang)");
            Series trend2 = EnsureSeries(chart, "Trend2", SeriesChartType.Spline, 2,
                                          Color.FromArgb(95, 105, 120), "Trend (kurz)");

            trend1.Points.Clear();
            trend2.Points.Clear();
            trend1.ShadowOffset = 0;
            trend2.ShadowOffset = 0;
            trend1.BorderDashStyle = ChartDashStyle.Solid;
            trend2.BorderDashStyle = ChartDashStyle.Dash;          // kurz = gestrichelt
            trend1["LineTension"] = "0.6";
            trend2["LineTension"] = "0.6";

            // Daten in Arrays für indexbasierten Zugriff.
            var ordered = filled.OrderBy(x => x.Key).ToArray();
            var vals = ordered.Select(x => x.Value).ToArray();
            var days = ordered.Select(x => x.Key).ToArray();

            // Trend (lang): 7-Punkte-Fenster (i-3..i+3) → glättet stärker.
            int w1 = 3;
            for (int i = 0; i < vals.Length; i++)
            {
                decimal avg = 0; int cnt = 0;
                for (int w = i - w1; w <= i + w1; w++)
                    if (w >= 0 && w < vals.Length) { avg += vals[w]; cnt++; }

                avg /= Math.Max(1, cnt);
                trend1.Points.AddXY(days[i].ToString("dd.MM"), Math.Round(avg, 2));
            }

            // Trend (kurz): 3-Punkte-Glättung (i-1, i, i+1) → reagiert schneller.
            for (int i = 0; i < vals.Length; i++)
            {
                decimal smooth = vals[i];
                if (i > 0 && i < vals.Length - 1)
                    smooth = (vals[i - 1] + vals[i] + vals[i + 1]) / 3m;

                trend2.Points.AddXY(days[i].ToString("dd.MM"), Math.Round(smooth, 2));
            }

            // =============================================================
            // Senkrechte „Heute"-Linie auf der kategorialen X-Achse (dd.MM)
            // =============================================================
            // Die X-Achse ist hier "kategorial" – sie nummeriert die Kategorien (Tage) ab 1.
            // Wir suchen die Position des "heute"-Labels im Punkte-Array.
            string todayLabel = DateTime.Today.ToString("dd.MM");
            int todayIndex1Based = -1;
            for (int i = 0; i < series.Points.Count; i++)
            {
                if (string.Equals(series.Points[i].AxisLabel, todayLabel, StringComparison.Ordinal))
                {
                    todayIndex1Based = i + 1; // Chart verwendet Kategorieindex ab 1
                    break;
                }
            }

            // Alte StripLines löschen, neue setzen.
            area.AxisX.StripLines.Clear();
            if (todayIndex1Based > 0)
            {
                var todayStrip = new StripLine
                {
                    Interval = 0,                                     // einmalige Linie (kein Wiederholungsraster)
                    IntervalOffset = todayIndex1Based,                // bei dieser Kategorie zeichnen
                    StripWidth = 0,                                   // 0 = nur die Linie, kein Streifen
                    BorderColor = Color.FromArgb(180, 110, 130, 160), // Alpha 180 = leicht transparent
                    BorderWidth = 1,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Text = "  Heute",                                 // Beschriftung neben der Linie
                    ForeColor = Color.FromArgb(110, 130, 160),
                    Font = new Font("Segoe UI", 7.5f, FontStyle.Regular),
                    TextAlignment = StringAlignment.Near,
                    TextLineAlignment = StringAlignment.Near
                };
                area.AxisX.StripLines.Add(todayStrip);
            }
        }

        /// <summary>
        /// Holt eine Serie oder legt sie an, falls sie nicht existiert.
        /// In beiden Fällen wird der Stil aktualisiert (Farbe, Linienstärke, Legende ...).
        /// </summary>
        private Series EnsureSeries(Chart chart, string name, SeriesChartType type, int borderWidth, Color color, string legendText)
        {
            Series s;
            if (chart.Series.IndexOf(name) == -1)
            {
                // Serie noch nicht da → neu anlegen und einfügen.
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
                // Serie existiert → bestehende holen und Stil neu setzen.
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

            // Schleife: von start bis end, jeden Tag einen Eintrag schreiben.
            for (DateTime d = start.Date; d <= end.Date; d = d.AddDays(1))
                res[d] = dict.TryGetValue(d, out var v) ? v : 0m;

            return res;
        }

        /// <summary>
        /// Extrahiert und normalisiert die Belagsinformation aus einem AVO-Text
        /// (z. B. "Vergüten B-123" -> "B123").
        /// </summary>
        private string CleanBelag(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            // 1) Schlagwort "vergüten" entfernen (egal ob groß/klein).
            input = System.Text.RegularExpressions.Regex
                .Replace(input, "vergüten", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                .Trim();

            // 2) Suche Muster "B" + optionalem "-" + Ziffern (z. B. "B123" oder "B-12").
            //    Match.Value liefert den Treffer als String zurück.
            var match = System.Text.RegularExpressions.Regex.Match(input, @"B-?\d+");
            if (match.Success)
                return match.Value.Replace("-", "").ToUpper();

            return ""; // nichts gefunden → leerer String
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Event-Handler für die ±1 Woche und ±2 Wochen Buttons
        // (jedes Click-Event verschiebt den DatePicker, was wiederum ValueChanged auslöst → Charts/Summen aktualisieren sich)
        // -----------------------------------------------------------------------------------------------------------------

        private void btnEineWocheZurueck_Click(object sender, EventArgs e)
        {
            // DatePicker um 7 Tage zurücksetzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(-7);
        }

        private void btnEineWocheVor_Click(object sender, EventArgs e)
        {
            // DatePicker um 7 Tage vorwärts setzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(7);
        }

        private void btnZweiWochenZurueck_Click(object sender, EventArgs e)
        {
            // DatePicker um 14 Tage zurücksetzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(-14);
        }

        private void btnZweiWochenVor_Click(object sender, EventArgs e)
        {
            // DatePicker um 14 Tage vorwärts setzen
            dateTimePickerRueckstandAb.Value = dateTimePickerRueckstandAb.Value.AddDays(14);
        }

        private void btnResett_Click(object sender, EventArgs e)
        {
            // DatePicker auf heute zurücksetzen
            dateTimePickerRueckstandAb.Value = DateTime.Today;
        }
    }
}
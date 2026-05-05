// -----------------------------
// USING-DIREKTIVEN (mit Kurzbeschreibung)
// -----------------------------
using System;                                             // Grundlegende Typen/Funktionen (DateTime, Exception, Math, etc.)
using System.Collections.Generic;                         // Generische Collections (List<T>, Dictionary<K,V>, ...)
using System.Data;                                        // DataTable/DataSet sowie SqlDbType
using System.Data.SqlClient;                              // SQL Server Datenzugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Drawing;                                     // Farben, Schriftarten, GDI+ Typen
using System.Globalization;                               // Kulturabhängige Formatierung/Parsing (DateTime, Zahlformate)
using System.IO;                                          // Datei- & Verzeichniszugriff (File, Directory, StreamReader/Writer)
using System.Linq;                                        // LINQ-Erweiterungsmethoden (Where, Select, OrderBy, ...)
using System.Text;                                        // Encoding, StringBuilder
using System.Threading.Tasks;                             // Tasks/Parallelisierung (Task, Task.Run, async/await)
using System.Windows.Forms;                               // WinForms-Basis (Form, Controls, Events, ...)
using System.Windows.Forms.DataVisualization.Charting;


namespace VerwaltungKST1127.Produktionsauswertung
{
    /// <summary>
    /// Formular zur Produktionsauswertung.
    /// Enthält:
    /// - Einlesen/Filtern großer Logdateien (pro Anlage) für den gewählten Tag
    /// - Aggregation: neueste Uhrzeit, Summe produktiver Zeiten, Summe Fehlerzeiten
    /// - Ausgabe in TextBoxen je Anlage (Zeit, Produktiv, Fehler, Chargen)
    /// - Erzeugung einer JSON mit den gefilterten Zeilen
    /// - Visualisierung als Balkendiagramm (Produktivzeit) mit farblicher Einteilung
    /// - Parallele Verarbeitung je Anlage (UI bleibt responsiv)
    /// - Aus SQL Chargenprotokolle lesen und in dgvArtikelStueck aufbereiten
    /// - Zählen der Proben je Anlage und Anzeige in lblProbeA20 ... lblProbeA65
    /// </summary>
    public partial class Form_AnsichtProduktionsauswertung : Form
    {
        public Form_AnsichtProduktionsauswertung()
        {
            InitializeComponent();
        }

        // -----------------------------------------------------------------
        // SQL: Zählt Chargen (Zeilen) pro Tabelle für das ausgewählte Datum
        // Führt alle Tabellen-Abfragen parallel aus (Connection Pooling nutzt mehrere Verbindungen effizient)
        // Schreibt die Ergebnisse in die zugehörigen Chargen-TextBoxen
        // A25 ist integriert (Chargenprotokoll25)
        // -----------------------------------------------------------------
        private async Task UpdateChargenTextboxesAsync(DateTime selectedDate)
        {
            var map = new Dictionary<string, TextBox>
            {
                { "Chargenprotokoll20", txtBoxChargenA20 },
                { "Chargenprotokoll25", txtBoxChargenA25 },
                { "Chargenprotokoll30", txtBoxChargenA30 },
                { "Chargenprotokoll35", txtBoxChargenA35 },
                { "Chargenprotokoll40", txtBoxChargenA40 },
                { "Chargenprotokoll45", txtBoxChargenA45 },
                { "Chargenprotokoll50", txtBoxChargenA50 },
                { "Chargenprotokoll60", txtBoxChargenA60 },
                { "Chargenprotokoll65", txtBoxChargenA65 },
            };

            string connectionString =
                @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False"; // Verbindungszeichenfolge

            var tasks = map.Select(kvp => CountRowsForTableAsync(connectionString, kvp.Key, selectedDate)).ToArray(); // Parallele Zählung je Tabelle
            var results = await Task.WhenAll(tasks); // Warten auf alle Ergebnisse

            foreach (var res in results) // Ergebnisse verarbeiten
            {
                if (map.TryGetValue(res.Table, out var targetBox) && targetBox != null) // Ziel-TextBox finden
                {
                    string text = res.Count > 0 ? res.Count.ToString(CultureInfo.InvariantCulture) : string.Empty; // Text vorbereiten
                    if (targetBox.InvokeRequired) // UI-Thread prüfen
                        targetBox.Invoke((Action)(() => targetBox.Text = text)); // Im UI-Thread aktualisieren
                    else
                        targetBox.Text = text; // Direkt aktualisieren
                }
            }
        }

        // Ergebnis-Objekt für eine Tabellenzählung
        private sealed class CountResult
        {
            public string Table { get; set; } = string.Empty; // Tabellenname
            public int Count { get; set; } // Anzahl der Zeilen
        }

        // Führt ein SELECT COUNT(*) auf einer Tabelle am gewählten Datum aus.
        // Versucht zuerst Spalte [Datum]; wenn sie nicht existiert, wird [ Datum] probiert.
        // Existiert die Tabelle (noch) nicht, wird 0 geliefert.
        private static async Task<CountResult> CountRowsForTableAsync(string connectionString, string table, DateTime date)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                async Task<int> CountWithColumnAsync(string columnToken)
                {
                    string sql = $"SELECT COUNT(*) FROM [{table}] WHERE CAST({columnToken} AS date) = @d";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;
                        object o = await cmd.ExecuteScalarAsync();
                        return (o != null && o != DBNull.Value)
                            ? Convert.ToInt32(o, CultureInfo.InvariantCulture)
                            : 0;
                    }
                }

                try
                {
                    try
                    {
                        int cnt = await CountWithColumnAsync("[Datum]");
                        return new CountResult { Table = table, Count = cnt };
                    }
                    catch (SqlException ex) when (ex.Number == 207) // Invalid column name
                    {
                        int cnt = await CountWithColumnAsync("[ Datum]");
                        return new CountResult { Table = table, Count = cnt };
                    }
                }
                catch (SqlException ex) when (ex.Number == 208) // Invalid object name (Tabelle fehlt)
                {
                    return new CountResult { Table = table, Count = 0 };
                }
            }
        }

        // Ergebnis-Objekt für eine Tabellenzählung (separat von CountResult für Proben)
        private sealed class ProbeCountResult
        {
            public string Table { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        // Zählt Proben je Tabelle am gewählten Datum.
        // Kriterium: Artikelnummer1 = 'Probe' (case-insensitive) UND Datum = selectedDate
        // Robuste Behandlung: [Datum] vs. [ Datum] und optional nicht vorhandene Tabelle.
        private static async Task<ProbeCountResult> CountProbesAsync(string connectionString, string table, DateTime date)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                async Task<int> CountWithColumnAsync(string columnToken)
                {
                    string sql = $"SELECT COUNT(*) FROM [{table}] WHERE CAST({columnToken} AS date) = @d AND UPPER(ISNULL(Artikelnummer1,'')) = 'PROBE'";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@d", SqlDbType.Date).Value = date.Date;
                        object o = await cmd.ExecuteScalarAsync();
                        return (o != null && o != DBNull.Value)
                            ? Convert.ToInt32(o, CultureInfo.InvariantCulture)
                            : 0;
                    }
                }

                try
                {
                    try
                    {
                        int cnt = await CountWithColumnAsync("[Datum]");
                        return new ProbeCountResult { Table = table, Count = cnt };
                    }
                    catch (SqlException ex) when (ex.Number == 207) // Invalid column name
                    {
                        int cnt = await CountWithColumnAsync("[ Datum]");
                        return new ProbeCountResult { Table = table, Count = cnt };
                    }
                }
                catch (SqlException ex) when (ex.Number == 208) // Invalid object name (Tabelle fehlt)
                {
                    return new ProbeCountResult { Table = table, Count = 0 };
                }
            }
        }

        // Zählt Proben über alle Anlagen (inkl. A25) und aktualisiert die Labels lblProbeAxx
        private async Task UpdateProbeLabelsAsync(DateTime selectedDate)
        {
            var map = new Dictionary<string, Label>
            {
                { "Chargenprotokoll20", lblProbeA20 },
                { "Chargenprotokoll25", lblProbeA25 },
                { "Chargenprotokoll30", lblProbeA30 },
                { "Chargenprotokoll35", lblProbeA35 },
                { "Chargenprotokoll40", lblProbeA40 },
                { "Chargenprotokoll45", lblProbeA45 },
                { "Chargenprotokoll50", lblProbeA50 },
                { "Chargenprotokoll60", lblProbeA60 },
                { "Chargenprotokoll65", lblProbeA65 },
            };

            string connectionString =
                @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

            var tasks = map.Select(kvp => CountProbesAsync(connectionString, kvp.Key, selectedDate)).ToArray();
            var results = await Task.WhenAll(tasks);

            foreach (var res in results)
            {
                if (map.TryGetValue(res.Table, out var label) && label != null)
                {
                    string text = res.Count > 0 ? res.Count.ToString(CultureInfo.InvariantCulture) : string.Empty;
                    if (label.InvokeRequired)
                        label.Invoke((Action)(() => label.Text = text));
                    else
                        label.Text = text;
                }
            }
        }

        // ------------------------------------------------------------
        // Datenstruktur: Ein einzelner Datensatz aus der Logdatei (für JSON/Debug)
        // ------------------------------------------------------------
        private class MaschinenRecord
        {
            public string Datum { get; set; }    // "dd.MM.yyyy"
            public string Uhrzeit { get; set; }  // "HH:mm:ss"
            public string Status { get; set; }   // Optionaler Teil ab "Maschinenstatus = ..."
            public string RawLine { get; set; }  // Vollständige Zeile (Nachvollziehbarkeit / JSON)
        }

        // ------------------------------------------------------------
        // Hilfsmethode: flexible Zeitdauer (Produktivzeit) in TimeSpan umwandeln
        // Unterstützt Formate: "ss", "mm:ss", "hh:mm:ss"
        // ------------------------------------------------------------
        private TimeSpan ParseZeitdauerToTimeSpan(string zeitdauer)
        {
            if (string.IsNullOrWhiteSpace(zeitdauer))
                return TimeSpan.Zero;

            var parts = zeitdauer.Split(':');

            if (parts.Length == 1)
            {
                if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds))
                    return TimeSpan.FromSeconds(seconds);
            }
            else if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int minutes) &&
                    int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds))
                    return new TimeSpan(0, minutes, seconds);
            }
            else if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int hours) &&
                    int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int minutes) &&
                    int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds))
                    return new TimeSpan(hours, minutes, seconds);
            }

            return TimeSpan.Zero;
        }

        // ------------------------------------------------------------
        // PERFORMANCE-KRITISCH:
        // Liest eine Maschinen-Logdatei schnell ein, filtert auf selectedDate,
        // summiert produktive "Zeitdauer" (nach Rezept-Whitelist),
        // ermittelt die späteste Uhrzeit und summiert die "Fehlerzeit" (Status 8 bis Folge-7).
        // ------------------------------------------------------------
        private Tuple<string, TimeSpan, TimeSpan, List<MaschinenRecord>> ProcessMachineFile(
            string machineFilePath,
            DateTime selectedDate,
            string[] auszuschliessen)
        {
            var records = new List<MaschinenRecord>(); // gesammelte Datensätze für JSON/Debug

            // Falls die Datei (noch) nicht vorhanden ist (z. B. Anlage offline):
            // Abbruch mit leeren/neutralen Werten, damit der Aufrufer robust weiterarbeiten kann.
            if (!File.Exists(machineFilePath))
                return Tuple.Create("", TimeSpan.Zero, TimeSpan.Zero, records);

            // Das Zieldatum als String in exakt dem Format, das in den Logzeilen am Zeilenanfang steht.
            // (Vergleich per string.Substring ist deutlich schneller als DateTime.Parse pro Zeile.)
            string dateString = selectedDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

            string latestTime = "";                    // späteste Uhrzeit im Tageslog (HH:mm:ss)
            TimeSpan produktivGesamt = TimeSpan.Zero;   // aufsummierte Produktivzeit für erlaubte Rezepte
            TimeSpan fehlerGesamt = TimeSpan.Zero;    // aufsummierte Fehlerzeit (8 → Start, letzte 7 → Ende)

            bool fehlerAktiv = false;                   // sind wir aktuell in einem Fehlerblock?
            int fehlerStartSec = 0;                    // Startzeitpunkt des Fehlerblocks in Sekunden seit 00:00:00
            int letzterBestaetigtSec = -1;             // Zeit der letzten "7" innerhalb eines aktiven Blocks (in Sekunden)

            // Mikro-Parser: HH:mm:ss → Sekunden. Spart Allokationen und ist schneller als DateTime.ParseExact.
            int ParseHmsToSec(string hms)
            {
                int hh = (hms[0] - '0') * 10 + (hms[1] - '0');
                int mm = (hms[3] - '0') * 10 + (hms[4] - '0');
                int ss = (hms[6] - '0') * 10 + (hms[7] - '0');
                return hh * 3600 + mm * 60 + ss;
            }

            // Großer Lesepuffer und ReadWrite-Share: Datei kann gleichzeitig von der Maschine beschrieben werden.
            var fs = new FileStream(
                machineFilePath,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite,
                bufferSize: 1 << 20 // 1 MiB Puffer für weniger I/O-Aufrufe
            );

            try
            {
                // Encoding gemäß Quelle (Windows-1252). BOM-Erkennung aktiv, falls einmal UTF-8 o. ä. auftaucht.
                var reader = new StreamReader(
                    fs,
                    Encoding.GetEncoding("windows-1252"),
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1 << 20
                );

                using (reader) // sorgt dafür, dass reader (und damit fs) am Ende geschlossen wird
                {
                    string line;
                    while ((line = reader.ReadLine()) != null) // zeilenweise einlesen (streamend → speicherschonend)
                    {
                        // Vorbereinigung: Steuerzeichen (außer Tab) entfernen, Whitespace außen kappen.
                        line = SanitizeLine(line);
                        if (string.IsNullOrWhiteSpace(line)) continue; // leere/whitespace-Zeilen überspringen
                        line = line.Trim();

                        // Minimalformat prüfen: "dd.MM.yyyy HH:mm:ss" hat exakt 19 Zeichen am Anfang
                        if (line.Length < 19) continue; // zu kurze Zeilen ignorieren

                        // Tagesfilter: Wir vergleichen nur die ersten 10 Zeichen (Datumsteil) exakt mit dateString.
                        if (!string.Equals(line.Substring(0, 10), dateString, StringComparison.Ordinal))
                            continue; // Zeile gehört nicht zum ausgewählten Datum

                        // Fachliche Ausschlüsse: Zeilen mit bestimmten Schlüsselwörtern zählen nicht zur Produktivzeit.
                        bool skip = false;
                        for (int i = 0; i < auszuschliessen.Length; i++)
                        {
                            if (line.IndexOf(auszuschliessen[i], StringComparison.OrdinalIgnoreCase) >= 0)
                            { skip = true; break; }
                        }
                        if (skip) continue; // Zeile wegen Ausschlussbegriff verworfen

                        // Uhrzeit aus fester Position extrahieren (11..18) und "neueste Uhrzeit" tracken.
                        string uhrzeit = line.Substring(11, 8); // Format "HH:mm:ss"
                        if (string.CompareOrdinal(uhrzeit, latestTime) > 0)
                            latestTime = uhrzeit;              // lexikographischer Vergleich reicht, da Format fix ist

                        int tSec = ParseHmsToSec(uhrzeit);     // Uhrzeit in Sekunden (für Fehlerzeitlogik)

                        // Produktivzeit nur für erlaubte Rezepte addieren.
                        int rezeptPos = line.IndexOf("Rezept = ", StringComparison.Ordinal);
                        if (rezeptPos >= 0)
                        {
                            string rezeptName = line.Substring(rezeptPos + "Rezept = ".Length).Trim();

                            // Whitelist: nur bestimmte Präfixe zählen als produktiv.
                            bool rezeptOk =
                                   rezeptName.StartsWith("B", StringComparison.OrdinalIgnoreCase)
                                || rezeptName.StartsWith("P", StringComparison.OrdinalIgnoreCase)
                                || rezeptName.StartsWith("U", StringComparison.OrdinalIgnoreCase)
                                || rezeptName.StartsWith("W", StringComparison.OrdinalIgnoreCase)
                                || rezeptName.StartsWith("A", StringComparison.OrdinalIgnoreCase)
                                || (rezeptName.Length >= 2 && rezeptName.StartsWith("S", StringComparison.OrdinalIgnoreCase) && char.IsDigit(rezeptName[1]));

                            if (rezeptOk)
                            {
                                // Zeitdauer extrahieren und zur Produktivsumme addieren (falls vorhanden/nicht leer).
                                int zdPos = line.IndexOf("Zeitdauer = ", StringComparison.Ordinal);
                                if (zdPos >= 0)
                                {
                                    string zeitdauer = line.Substring(zdPos + "Zeitdauer = ".Length).Trim();
                                    if (!string.IsNullOrEmpty(zeitdauer))
                                        produktivGesamt += ParseZeitdauerToTimeSpan(zeitdauer);
                                }
                            }
                        }

                        // Fehlerstatus erfassen: 8 beginnt einen Block, 7 markiert das Ende (wir nehmen die letzte 7).
                        int msPos = line.IndexOf("Maschinenstatus = ", StringComparison.Ordinal);
                        if (msPos >= 0)
                        {
                            // Zahl hinter "Maschinenstatus = " als int extrahieren (ohne Allokationen durch Regex etc.).
                            int codeIdx = msPos + "Maschinenstatus = ".Length;
                            int code = -1;
                            if (codeIdx < line.Length)
                            {
                                int i = codeIdx;
                                int acc = 0; bool any = false;
                                while (i < line.Length)
                                {
                                    char c = line[i];
                                    if (c < '0' || c > '9') break; // nur zusammenhängende Ziffern konsumieren
                                    acc = acc * 10 + (c - '0');
                                    any = true; i++;
                                }
                                if (any) code = acc;
                            }

                            if (code == 8) // Fehler wird ausgelöst (noch nicht bestätigt)
                            {
                                if (!fehlerAktiv)
                                {
                                    fehlerAktiv = true;        // Block beginnt
                                    fehlerStartSec = tSec;     // Startzeit merken
                                    letzterBestaetigtSec = -1; // bisher keine Bestätigung gesehen
                                }
                            }
                            else if (code == 7) // Fehler bestätigt (Endekandidat)
                            {
                                if (fehlerAktiv)
                                {
                                    letzterBestaetigtSec = tSec; // Ende auf die ZEIT DER LETZTEN 7 setzen
                                }
                            }
                            else // anderer Status: schließt einen aktiven Block, sofern wir eine 7 gesehen haben
                            {
                                if (fehlerAktiv && letzterBestaetigtSec >= 0)
                                {
                                    int diff = letzterBestaetigtSec - fehlerStartSec; // Dauer des Fehlerblocks
                                    if (diff > 0)
                                        fehlerGesamt += TimeSpan.FromSeconds(diff);   // nur positive Dauer aufsummieren
                                    fehlerAktiv = false;           // Block ist beendet
                                    letzterBestaetigtSec = -1;     // Reset für nächsten Block
                                }
                            }
                        }

                        // Den Status-Teil (ab "Maschinenstatus = ") optional als Text sichern – hilfreich für JSON/Debug.
                        string statusTxt = "";
                        if (msPos > 0)
                            statusTxt = line.Substring(msPos).Trim();

                        // Vollständige, bereits bereinigte Zeile mit Metadaten aufnehmen.
                        records.Add(new MaschinenRecord
                        {
                            Datum = dateString,
                            Uhrzeit = uhrzeit,
                            Status = statusTxt,
                            RawLine = line
                        });
                    }
                }
            }
            finally
            {
                // Sicherstellen, dass der FileStream auch bei Exceptions sauber geschlossen wird.
                fs?.Dispose();
            }

            // Dateiende: Falls noch ein Fehlerblock offen ist und wir mindestens eine 7 gesehen haben,
            // schließen wir ihn hier ab (letzte 7 minus Start 8).
            if (fehlerAktiv && letzterBestaetigtSec >= 0)
            {
                int diff = letzterBestaetigtSec - fehlerStartSec;
                if (diff > 0)
                    fehlerGesamt += TimeSpan.FromSeconds(diff);
            }

            // Rückgabe der ermittelten Kennzahlen und der gesammelten Datensätze.
            return Tuple.Create(latestTime, produktivGesamt, fehlerGesamt, records);
        }

        // ------------------------------------------------------------
        // Entfernt Steuerzeichen (z. B. \0), lässt Tabs bestehen.
        // Hintergrund: Manche Logquellen enthalten sporadisch nicht druckbare Zeichen,
        // die spätere Textoperationen (IndexOf, Substring) stören könnten.
        // ------------------------------------------------------------
        private static string SanitizeLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return line; // Null/leer schonen
            var sb = new StringBuilder(line.Length);     // vermeiden von String-Konkatenationen in Schleife
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (!char.IsControl(c) || c == '\t')    // Tabs erlauben, alle anderen Steuerzeichen filtern
                    sb.Append(c);
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------
        // Liest für das gewählte Datum pro Chargenprotokoll die Spalten
        // Artikelnummer1/2/3, Seite1/2/3, Stk1/2/3.
        // Erstellt eine DataTable und bindet sie an dgvArtikelStueck.
        // Leere Artikelnummern werden übersprungen (Seite/Stk dann ignoriert).
        // Tabellenliste enthält A25 (Chargenprotokoll25). Fehlende Tabellen werden ignoriert.
        // ------------------------------------------------------------
        private async Task UpdateDgvArtikelStueckAsync(DateTime selectedDate)
        {
            // Roh-DataTable vorbereiten: zunächst eine flache Liste aller Vorkommen aufnehmen,
            // später werden gleiche Schlüssel aggregiert (Summe Stk).
            var dt = new DataTable();
            dt.Columns.Add("Anlage", typeof(string));
            dt.Columns.Add("Datum", typeof(DateTime));
            dt.Columns.Add("Artikelnummer", typeof(string));
            dt.Columns.Add("Seite", typeof(string));
            dt.Columns.Add("Stk", typeof(int));

            string connectionString =
                @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

            // Alle relevanten Protokolltabellen – A25 ist enthalten. Falls es (zeitweise) keine Tabelle gibt,
            // wird diese später abgefangen.
            string[] tables = { "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30", "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65" };

            // Jede Tabelle parallel abfragen (Task.Run), um Wartezeiten zu überlappen.
            var tableTasks = tables.Select(tableName => Task.Run(async () =>
            {
                // Aus dem Tabellennamen die Anlagenkennung erzeugen (z. B. "Chargenprotokoll20" → "A20").
                string anlage = "A" + new string(tableName.Where(char.IsDigit).ToArray());

                // Lokale Funktion: Abfrage mit variablem Datumsspaltennamen ("[Datum]" oder "[ Datum]").
                async Task ReadWithColumnAsync(string columnToken)
                {
                    string sql =
                        $"SELECT " +
                        $"  {columnToken} AS Datum, " +
                        $"  Artikelnummer1, Seite1, Stk1, " +
                        $"  Artikelnummer2, Seite2, Stk2, " +
                        $"  Artikelnummer3, Seite3, Stk3 " +
                        $"FROM [{tableName}] " +
                        $"WHERE CAST({columnToken} AS date) = @d";

                    using (var conn = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@d", SqlDbType.Date).Value = selectedDate.Date; // Datum filtern (ohne Uhrzeit)
                        await conn.OpenAsync();
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync()) // jede Zeile der Tabelle verarbeiten
                            {
                                // Datum der Zeile robust ermitteln (direkt als DateTime oder per Parse)
                                DateTime datum = selectedDate.Date;
                                if (!(rdr["Datum"] is DBNull))
                                {
                                    if (rdr["Datum"] is DateTime dtVal)
                                        datum = dtVal.Date;
                                    else
                                    {
                                        DateTime.TryParse(Convert.ToString(rdr["Datum"], CultureInfo.InvariantCulture), out datum);
                                        datum = datum.Date;
                                    }
                                }

                                // Bis zu drei mögliche Artikelspalten verarbeiten (1..3 pro Datenzeile)
                                for (int i = 1; i <= 3; i++)
                                {
                                    string artCol = $"Artikelnummer{i}";
                                    string seiteCol = $"Seite{i}";
                                    string stkCol = $"Stk{i}";

                                    string artikelnummer = rdr[artCol] is DBNull ? string.Empty : Convert.ToString(rdr[artCol], CultureInfo.InvariantCulture)?.Trim();
                                    if (string.IsNullOrEmpty(artikelnummer))
                                        continue; // ohne Artikelnummer ignorieren wir die zugehörige Seite/Stk

                                    string seite = rdr[seiteCol] is DBNull ? string.Empty : Convert.ToString(rdr[seiteCol], CultureInfo.InvariantCulture)?.Trim();

                                    int stk = 0; // Stückzahl robust lesen (int oder string → int)
                                    if (!(rdr[stkCol] is DBNull))
                                    {
                                        var raw = rdr[stkCol];
                                        if (raw is int i32) stk = i32; // schnellster Weg, wenn bereits int
                                        else int.TryParse(Convert.ToString(raw, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out stk);
                                    }

                                    // Fadenkreuz-Synchronisation: mehrere Tabellen werden parallel eingelesen,
                                    // daher schützen wir den gemeinsamen DataTable-Zugriff mit einem Lock.
                                    lock (dt)
                                    {
                                        dt.Rows.Add(anlage, datum, artikelnummer, seite, stk);
                                    }
                                }
                            }
                        }
                    }
                }

                try
                {
                    // Bevorzugt wird die saubere Spalte [Datum]; bei Legacy-Tabellen kann es [ Datum] sein.
                    try
                    {
                        await ReadWithColumnAsync("[Datum]");
                    }
                    catch (SqlException ex) when (ex.Number == 207) // Spalte existiert nicht
                    {
                        await ReadWithColumnAsync("[ Datum]");
                    }
                }
                catch (SqlException ex) when (ex.Number == 208) // Tabelle existiert nicht (z. B. Anlage temporär außer Betrieb)
                {
                    // bewusst ignorieren – es gibt dann einfach keine Datensätze für diese Anlage
                }
            })).ToArray();

            // Warten, bis alle Tabellen eingelesen wurden
            await Task.WhenAll(tableTasks);

            // Aggregation: gleiche Schlüssel (Anlage, Datum, Artikelnummer, Seite) zusammenfassen und Stk aufsummieren.
            var grouped = dt.AsEnumerable()
                .GroupBy(r => new
                {
                    Anlage = r.Field<string>("Anlage"),
                    Datum = r.Field<DateTime>("Datum"),
                    Artikelnummer = (r.Field<string>("Artikelnummer") ?? string.Empty).Trim(),
                    Seite = (r.Field<string>("Seite") ?? string.Empty).Trim()
                })
                .Select(g => new
                {
                    g.Key.Anlage,
                    g.Key.Datum,
                    g.Key.Artikelnummer,
                    g.Key.Seite,
                    Stk = g.Sum(r => r.Field<int?>("Stk") ?? 0)
                });

            // Ergebnis-DataTable für das DataGridView: verständliche Spaltenbezeichnungen
            var dtAgg = new DataTable();
            dtAgg.Columns.Add("Anlage", typeof(string));
            dtAgg.Columns.Add("Datum", typeof(DateTime));
            dtAgg.Columns.Add("Artikelnummer", typeof(string));
            dtAgg.Columns.Add("Seite", typeof(string));
            dtAgg.Columns.Add("Stück", typeof(int));

            foreach (var x in grouped)
                dtAgg.Rows.Add(x.Anlage, x.Datum, x.Artikelnummer, x.Seite, x.Stk);

            // Sortierung für eine konsistente, gut lesbare Darstellung im Grid
            DataView dv = dtAgg.DefaultView;
            dv.Sort = "Anlage ASC, Datum ASC, Artikelnummer ASC, Seite ASC";
            var sorted = dv.ToTable();

            // Binding auf den UI-Thread marshallen (WinForms-Thread-Affinität beachten)
            if (dgvArtikelStueck.InvokeRequired)
            {
                dgvArtikelStueck.Invoke((Action)(() =>
                {
                    dgvArtikelStueck.AutoGenerateColumns = true;
                    dgvArtikelStueck.DataSource = sorted;
                }));
            }
            else
            {
                dgvArtikelStueck.AutoGenerateColumns = true;
                dgvArtikelStueck.DataSource = sorted;
            }
        }

        // ------------------------------------------------------------
        // Event-Handler: Datum im DateTimePicker hat sich geändert
        // Ablauf:
        // - UI-TextBoxen leeren
        // - Paralleles Einlesen aller Anlagen (Produktiv- & Fehlerzeit, neueste Uhrzeit)
        // - Chargen pro Anlage aus SQL laden
        // - Artikel/Seite/Stk ins DataGridView laden
        // - JSON schreiben (Datensätze des ausgewählten Tages)
        // - Diagramm aktualisieren (Produktivzeit)
        // - Proben-Labels aktualisieren
        // ------------------------------------------------------------
        private async void dateTimePickerLastProduction_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 1) UI-Reset: Alle Ausgabe-TextBoxen leeren, damit für das neu gewählte Datum
                //    keine Werte vom vorherigen Datum stehen bleiben. Das verhindert Missverständnisse
                //    und sorgt dafür, dass bei Fehlern/Leerständen klar ersichtlich ist, was neu geladen wurde.
                txtBoxLastTimeA20.Text = txtBoxLastTimeA25.Text = txtBoxLastTimeA30.Text =
                txtBoxLastTimeA35.Text = txtBoxLastTimeA40.Text = txtBoxLastTimeA45.Text =
                txtBoxLastTimeA50.Text = txtBoxLastTimeA60.Text = txtBoxLastTimeA65.Text = "";

                txtBoxProductivA20.Text = txtBoxProductivA25.Text = txtBoxProductivA30.Text =
                txtBoxProductivA35.Text = txtBoxProductivA40.Text = txtBoxProductivA45.Text =
                txtBoxProductivA50.Text = txtBoxProductivA60.Text = txtBoxProductivA65.Text = "";

                txtBoxFehlerA20.Text = txtBoxFehlerA25.Text = txtBoxFehlerA30.Text =
                txtBoxFehlerA35.Text = txtBoxFehlerA40.Text = txtBoxFehlerA45.Text =
                txtBoxFehlerA50.Text = txtBoxFehlerA60.Text = txtBoxFehlerA65.Text = "";

                txtBoxChargenA20.Text = txtBoxChargenA25.Text = txtBoxChargenA30.Text =
                txtBoxChargenA35.Text = txtBoxChargenA40.Text = txtBoxChargenA45.Text =
                txtBoxChargenA50.Text = txtBoxChargenA60.Text = txtBoxChargenA65.Text = "";

                // 2) Das neu gewählte Datum (nur Datumsteil, Uhrzeit wird ignoriert)
                //    Dieses Datum ist die zentrale Filterbedingung für Log- und SQL-Abfragen.
                DateTime selectedDate = dateTimePickerLastProduction.Value.Date;

                // 3) Abbildung Anlagenordner → zugehörige UI-TextBoxen (späteste Zeit, Produktivzeit, Fehlerzeit)
                //    Diese Map eliminiert wiederholten Code und hält die Zuordnung an einer Stelle konsistent.
                var anlagen = new Dictionary<string, (TextBox timeBox, TextBox prodBox, TextBox fehlerBox)>
        {
            { "Anl_20", (txtBoxLastTimeA20, txtBoxProductivA20, txtBoxFehlerA20) },
            { "Anl_25", (txtBoxLastTimeA25, txtBoxProductivA25, txtBoxFehlerA25) },
            { "Anl_30", (txtBoxLastTimeA30, txtBoxProductivA30, txtBoxFehlerA30) },
            { "Anl_35", (txtBoxLastTimeA35, txtBoxProductivA35, txtBoxFehlerA35) },
            { "Anl_40", (txtBoxLastTimeA40, txtBoxProductivA40, txtBoxFehlerA40) },
            { "Anl_45", (txtBoxLastTimeA45, txtBoxProductivA45, txtBoxFehlerA45) },
            { "Anl_50", (txtBoxLastTimeA50, txtBoxProductivA50, txtBoxFehlerA50) },
            { "Anl_60", (txtBoxLastTimeA60, txtBoxProductivA60, txtBoxFehlerA60) },
            { "Anl_65", (txtBoxLastTimeA65, txtBoxProductivA65, txtBoxFehlerA65) },
        };

                // 4) Basisverzeichnis für die Logdateien. Jede Anlage hat darunter einen eigenen Ordner.
                //    Beispielpfad: P:\TEDuTOZ\MDE\Anl_20\machine_state_ge.txt
                string basePath = @"P:\\TEDuTOZ\\MDE";

                // 5) Zeilen mit diesen Schlüsselwörtern werden beim Log-Einlesen ignoriert (fachlich nicht produktiv).
                string[] auszuschliessen = new[] { "Heizen", "Lecktest", "Saugleistung" };

                // 6) Sammlung aller Records pro Anlage – wird später als JSON-Datei geschrieben
                //    und kann z. B. für Debugging oder weitere Auswertungen genutzt werden.
                var allResults = new Dictionary<string, List<MaschinenRecord>>();

                // 7) Wir starten pro Anlage einen Hintergrund-Task, damit die UI nicht blockiert und
                //    mehrere Dateien parallel verarbeitet werden können (I/O-bound → gute Parallelisierungswirkung).
                var tasks = new List<Task>();

                foreach (var kvp in anlagen)
                {
                    string anlFolder = kvp.Key;                 // Ordnername der Anlage (z. B. "Anl_20")
                    var boxes = kvp.Value;                      // Ziel-TextBoxen für Ausgabe (Zeit, Produktiv, Fehler)

                    string machineFilePath = Path.Combine(basePath, anlFolder, "machine_state_ge.txt"); // vollständiger Logpfad

                    // Für jede Anlage einen Task anlegen. Achtung: UI-Controls nur über Invoke anfassen!
                    tasks.Add(Task.Run(() =>
                    {
                        // Logdatei der Anlage einlesen und auswerten
                        var result = ProcessMachineFile(machineFilePath, selectedDate, auszuschliessen);
                        string latest = result.Item1;                  // späteste Uhrzeit
                        TimeSpan produktiv = result.Item2;             // Summe Produktivzeit
                        TimeSpan fehler = result.Item3;                // Summe Fehlerzeit
                        List<MaschinenRecord> recs = result.Item4;     // Zeilen für JSON

                        // UI-Update muss auf dem UI-Thread erfolgen (WinForms-Thread-Affinität)
                        this.Invoke((Action)(() =>
                        {
                            // Späteste Uhrzeit der Anlage anzeigen (leer, wenn keine Daten)
                            boxes.timeBox.Text = latest ?? string.Empty;

                            // Produktivzeit im Format HH:MM:SS h anzeigen – aber nur, wenn > 0
                            boxes.prodBox.Text = produktiv > TimeSpan.Zero
                                ? string.Format(CultureInfo.InvariantCulture,
                                                "{0:D2}:{1:D2}:{2:D2} h",
                                                (int)produktiv.TotalHours, produktiv.Minutes, produktiv.Seconds)
                                : string.Empty;

                            // Fehlerzeit im selben Format anzeigen – ebenfalls nur, wenn > 0
                            boxes.fehlerBox.Text = fehler > TimeSpan.Zero
                                ? string.Format(CultureInfo.InvariantCulture,
                                                "{0:D2}:{1:D2}:{2:D2} h",
                                                (int)fehler.TotalHours, fehler.Minutes, fehler.Seconds)
                                : string.Empty;

                            // Records der Anlage für den späteren JSON-Export sichern
                            allResults[anlFolder] = recs;
                        }));
                    }));
                }

                // 8) Warten, bis alle Anlagen-Tasks abgeschlossen sind (Synchronisationspunkt)
                await Task.WhenAll(tasks);

                // 9) Chargen je Anlage aus der Datenbank lesen (Anzahl Chargen pro Anlage/Datum)
                await UpdateChargenTextboxesAsync(selectedDate);

                // 10) Artikeldetails (Artikelnummer/Seite/Stk) laden und in DataGridView anzeigen
                await UpdateDgvArtikelStueckAsync(selectedDate);

                // 11) Proben je Anlage zählen und in die entsprechenden Labels schreiben
                await UpdateProbeLabelsAsync(selectedDate);

                // 12) JSON-Export der Logdaten (nur der gewählte Tag); nützlich für Nachvollziehbarkeit oder externe Tools
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maschinenstatus.json");
                using (var fs2 = new FileStream(jsonPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    System.Text.Json.JsonSerializer.Serialize(
                        fs2,
                        allResults,
                        new System.Text.Json.JsonSerializerOptions { WriteIndented = true } // menschenlesbar formatieren
                    );
                }

                // 13) Gesamt-Stückzahl über alle angezeigten Zeilen im DGV berechnen.
                //     Die Aggregation in UpdateDgvArtikelStueckAsync fasst bereits Duplikate zusammen; hier summieren wir nur die Spalte "Stück".
                int totalStueck = 0;
                foreach (DataGridViewRow row in dgvArtikelStueck.Rows)
                {
                    if (row.Cells["Stück"].Value is int stk)
                        totalStueck += stk; // leere/NULL-Zellen werden ignoriert
                }
                lblStueckAusgewaehlt.Text = $"Gesamt Stück: {totalStueck}"; // verständlicher Labeltext für die UI

                // 14) Balkendiagramm und Summen (Produktivstunden, effektive Chargen ohne Proben) aktualisieren
                UpdateGesamtProduktivChart();
        
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung: zeigt die Ursache an. In produktiven Umgebungen optional zusätzlich Logging (EventLog/Datei).
                MessageBox.Show(string.Format("Fehler: {0}", ex.Message));
            }
        }

        // ------------------------------------------------------------
        // Diagramm aktualisieren (Produktivzeit)
        // ------------------------------------------------------------
        private void UpdateGesamtProduktivChart()
        {
            // Diagramm bei jedem Aufruf komplett neu aufbauen, damit keine
            // Artefakte aus vorherigen Ladungen stehen bleiben.
            chartGesamtProduktiv.Series.Clear();
            chartGesamtProduktiv.ChartAreas.Clear();
            chartGesamtProduktiv.Legends.Clear();

            // ── Chart-Rahmen ────────────────────────────────────────────────
            // Kein sichtbarer Außenrahmen, weißer Hintergrund, keine Farbpalette
            // (Farben werden pro Datenpunkt individuell gesetzt).
            chartGesamtProduktiv.BackColor = Color.White;
            chartGesamtProduktiv.BorderlineWidth = 0;
            chartGesamtProduktiv.BorderlineDashStyle = ChartDashStyle.NotSet;
            chartGesamtProduktiv.Palette = ChartColorPalette.None;

            // ── Zeichenfläche (ChartArea) ────────────────────────────────────
            var ca = new ChartArea("main");
            ca.BackColor = Color.White;
            ca.BorderColor = Color.Transparent;
            ca.BorderWidth = 0;

            // Achsentitel beschreiben Kategorie (X) und Metrik (Y).
            ca.AxisX.Title = "Anlage";
            ca.AxisY.Title = "Produktive Stunden";

            // ── Schriften ────────────────────────────────────────────────────
            // Regular statt Bold – wirkt leichter und moderner.
            ca.AxisX.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            ca.AxisY.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            ca.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Regular);
            ca.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Regular);

            // ── Achsenfarben ─────────────────────────────────────────────────
            // Mittleres Grau für Labels, etwas dunkler für Titel –
            // vermeidet den harten Kontrast von reinem Schwarz.
            ca.AxisX.LabelStyle.ForeColor = Color.FromArgb(110, 110, 110);
            ca.AxisY.LabelStyle.ForeColor = Color.FromArgb(110, 110, 110);
            ca.AxisX.TitleForeColor = Color.FromArgb(80, 80, 80);
            ca.AxisY.TitleForeColor = Color.FromArgb(80, 80, 80);

            // Achsenlinien und Tick-Marks in sehr hellem Grau –
            // strukturieren das Diagramm, ohne zu dominieren.
            ca.AxisX.LineColor = Color.FromArgb(210, 210, 210);
            ca.AxisY.LineColor = Color.FromArgb(210, 210, 210);
            ca.AxisX.MajorTickMark.LineColor = Color.FromArgb(210, 210, 210);
            ca.AxisY.MajorTickMark.LineColor = Color.FromArgb(210, 210, 210);

            // ── Rasterlinien ─────────────────────────────────────────────────
            // Nur auf der Y-Achse: geben dem Auge Orientierung beim Ablesen
            // der Stundenwerte, ohne das Bild zu überladen.
            // X-Rasterlinien bleiben aus, da die Balken selbst die Kategorien trennen.
            ca.AxisY.MajorGrid.Enabled = true;
            ca.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);
            ca.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            ca.AxisX.MajorGrid.Enabled = false;

            // Y-Skalierung automatisch – die Chart-Komponente wählt sinnvolle Schrittweiten.
            ca.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

            chartGesamtProduktiv.ChartAreas.Add(ca);

            // ── Datenserie ───────────────────────────────────────────────────
            // Column = stehende Balken; Wert direkt am Balken als Label anzeigen,
            // Legende wird nicht benötigt (Anlagenname steht bereits auf der X-Achse).
            var series = new Series("Produktivzeit")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                BorderWidth = 0,               // kein Balkenrahmen
                BorderColor = Color.Transparent,
            };

            // PointWidth: 0 = volle Breite, 1 = kein Balken sichtbar.
            // 0.55 lässt genug Luft zwischen den Säulen für einen luftigen Look.
            series["PointWidth"] = "0.55";

            // Label-Schrift: leicht und dezent, passend zum Gesamtdesign.
            series.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            series.LabelForeColor = Color.FromArgb(70, 70, 70);

            // SmartLabel verhindert, dass Labels sich bei ähnlichen Werten überschneiden.
            // Partial erlaubt ein minimales Herausragen aus dem Plot-Bereich.
            series.SmartLabelStyle.Enabled = true;
            series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Partial;

            chartGesamtProduktiv.Series.Add(series);

            // ── Datenquelle ──────────────────────────────────────────────────
            // Jede Anlage wird einer TextBox zugeordnet, die den Produktivzeitwert
            // im Format "HH:MM:SS h" enthält (aus SQL befüllt).
            var felder = new Tuple<string, TextBox>[]
            {
        Tuple.Create("A20", txtBoxProductivA20),
        Tuple.Create("A25", txtBoxProductivA25),
        Tuple.Create("A30", txtBoxProductivA30),
        Tuple.Create("A35", txtBoxProductivA35),
        Tuple.Create("A40", txtBoxProductivA40),
        Tuple.Create("A45", txtBoxProductivA45),
        Tuple.Create("A50", txtBoxProductivA50),
        Tuple.Create("A60", txtBoxProductivA60),
        Tuple.Create("A65", txtBoxProductivA65),
            };

            double totalHours = 0.0; // Summe aller Anlagen für lblGesamtProduktiv

            // ── Datenpunkte befüllen ─────────────────────────────────────────
            for (int i = 0; i < felder.Length; i++)
            {
                string label = felder[i].Item1;
                TextBox box = felder[i].Item2;

                string raw = (box.Text ?? string.Empty).Trim();

                // "HH:MM:SS h" → "HH:MM:SS" – das "h"-Suffix stört den Parser.
                if (raw.EndsWith("h", StringComparison.OrdinalIgnoreCase))
                    raw = raw.Substring(0, raw.Length - 1).Trim();

                TimeSpan ts = ParseZeitdauerToTimeSpan(raw); // ungültige Eingaben → 0:00:00
                double hours = Math.Round(ts.TotalHours, 2);
                totalHours += hours;

                int pointIndex = series.Points.AddXY(label, hours);
                DataPoint point = series.Points[pointIndex];

                // Label am Balken: kulturabhängige Dezimalformatierung (z. B. "7,25 h").
                point.Label = hours.ToString("0.##", CultureInfo.CurrentCulture) + " h";

                // ── Farbstufen nach Auslastung ───────────────────────────────
                // Gedämpfte, desaturierte Töne statt knalliger Ampelfarben –
                // gleiche Aussagekraft, aber ruhigere Optik.
                if (hours < 3)
                    point.Color = Color.FromArgb(196, 107, 105);   // Rosa-Rot  → sehr geringe Auslastung
                else if (hours < 6)
                    point.Color = Color.FromArgb(210, 150, 100);   // Terrakotta → geringe Auslastung
                else if (hours < 9)
                    point.Color = Color.FromArgb(198, 188, 100);   // Sandgelb  → mittlere Auslastung
                else if (hours < 12)
                    point.Color = Color.FromArgb(100, 168, 182);   // Stahlblau → gute Auslastung
                else
                    point.Color = Color.FromArgb(100, 158, 124);   // Salbeigrün → sehr gute Auslastung
            }

            // Gesamtstunden aller Anlagen im Summary-Label anzeigen.
            if (lblGesamtProduktiv != null)
                lblGesamtProduktiv.Text = totalHours.ToString("0.##", CultureInfo.CurrentCulture) + " h";

            // ── Chargen-Summe (exkl. Proben) ────────────────────────────────
            // Pro Anlage: Gesamtchargen minus Probechargen = produktive Chargen.
            // Beide Arrays sind index-synchron mit dem felder-Array oben.
            int totalChargen = 0;

            var chargeTextBoxes = new TextBox[]
            {
        txtBoxChargenA20, txtBoxChargenA25, txtBoxChargenA30,
        txtBoxChargenA35, txtBoxChargenA40, txtBoxChargenA45,
        txtBoxChargenA50, txtBoxChargenA60, txtBoxChargenA65,
            };
            var probeLabels = new Label[]
            {
        lblProbeA20, lblProbeA25, lblProbeA30,
        lblProbeA35, lblProbeA40, lblProbeA45,
        lblProbeA50, lblProbeA60, lblProbeA65,
            };

            for (int i = 0; i < chargeTextBoxes.Length; i++)
            {
                int chargeCount = 0;
                if (int.TryParse(chargeTextBoxes[i].Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int c))
                    chargeCount = c;

                int probeCount = 0;
                if (int.TryParse(probeLabels[i].Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int p))
                    probeCount = p;

                totalChargen += (chargeCount - probeCount);
            }

            if (lblGesamtChargen != null)
                lblGesamtChargen.Text = totalChargen.ToString(CultureInfo.CurrentCulture);
        }

        // Öffnet ein Informationsfenster zur Produktionsübersicht.
        private void Btn_InfoProduction_Click(object sender, EventArgs e)
        {
            Form_InfoProduktionsauswertung infoForm = new Form_InfoProduktionsauswertung();
            infoForm.Show();
        }

        // Öffnet das Fenster für die Stückzahlen
        private void btnStkVorAvo_Click(object sender, EventArgs e)
        {
            Form_StkVorAvo frmStkVorAvo = new Form_StkVorAvo();
            frmStkVorAvo.Show();
        }       
    }
}

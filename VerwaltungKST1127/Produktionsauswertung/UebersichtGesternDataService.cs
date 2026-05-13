// -----------------------------------------------------------------------------
// Datenservice für die "Produktion gestern"-Übersicht.
//
// Liest für einen ausgewählten Tag (typischerweise gestern) Daten aus:
//   - SQL: 9 Tabellen Chargenprotokoll20..65 in SOA127_Chargenprotokoll
//     (Stückzahlen, Artikel, Rezept, Datum/Uhrzeit, Probe-Flag)
//   - Filesystem: P:\TEDuTOZ\MDE\Anl_XX\machine_state_ge.txt
//     (Produktivzeit, Fehlerzeit, Maschinenstatus-Verlauf für Timeline)
//
// Liefert ein flaches, JSON-serialisierbares Modell (DashboardData),
// das von der HTML/ECharts-Oberfläche direkt verwendet werden kann.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerwaltungKST1127.Produktionsauswertung
{
    /// <summary>
    /// Aggregierte Tagesdaten für das ECharts-Dashboard.
    /// Wird als JSON ins HTML eingebettet (window.__DASHBOARD_DATA__).
    /// </summary>
    public class DashboardData
    {
        public string Datum { get; set; }                                 // "13.05.2026"
        public string DatumLang { get; set; }                             // "Mittwoch, 13.05.2026"
        public int GesamtStk { get; set; }                                // Summe Stk über alle Anlagen
        public int AnzahlChargen { get; set; }                            // Chargen exkl. Proben
        public int AnzahlProben { get; set; }                             // nur Proben
        public int AnzahlArtikel { get; set; }                            // unterschiedliche Artikelnummern
        public int AnzahlRezepte { get; set; }                            // unterschiedliche Rezepte
        public double ProduktivStunden { get; set; }                      // Summe aller Anlagen
        public double FehlerStunden { get; set; }                         // Summe aller Anlagen
        public double AuslastungProzent { get; set; }                     // Ø Auslastung (24h-Basis)
        public List<AnlageKpi> Anlagen { get; set; } = new List<AnlageKpi>();
        public List<HeatmapZelle> Heatmap { get; set; } = new List<HeatmapZelle>();
        public List<TimelineBlock> Timeline { get; set; } = new List<TimelineBlock>();
        public List<ArtikelKnoten> ArtikelBaum { get; set; } = new List<ArtikelKnoten>();
        public List<TopArtikel> TopArtikel { get; set; } = new List<TopArtikel>();
        public List<RezeptAnteil> Rezepte { get; set; } = new List<RezeptAnteil>();
    }

    public class AnlageKpi
    {
        public string Name { get; set; }                                   // "A20"
        public int Stk { get; set; }
        public int Chargen { get; set; }
        public int Proben { get; set; }
        public double ProduktivStunden { get; set; }
        public double FehlerStunden { get; set; }
        public double AuslastungProzent { get; set; }
        public string LetzteUhrzeit { get; set; }                          // "21:47:03"
    }

    public class HeatmapZelle
    {
        public int Stunde { get; set; }                                    // 0..23
        public string Anlage { get; set; }                                 // "A20"
        public int Stk { get; set; }
    }

    public class TimelineBlock
    {
        public string Anlage { get; set; }                                 // "A20"
        public long StartMs { get; set; }                                  // ms seit 00:00 des Tages
        public long EndeMs { get; set; }
        public string Typ { get; set; }                                    // "produktiv" | "fehler"
        public string Info { get; set; }                                   // Tooltip-Text
    }

    public class ArtikelKnoten
    {
        public string Name { get; set; }                                   // Anlage oder Artikelnr
        public int Value { get; set; }
        public List<ArtikelKnoten> Children { get; set; }
    }

    public class TopArtikel
    {
        public string Artikelnummer { get; set; }
        public int Stk { get; set; }
    }

    public class RezeptAnteil
    {
        public string Name { get; set; }                                   // "B...", "P...", etc.
        public int Anzahl { get; set; }                                    // Anzahl Chargen
    }

    /// <summary>
    /// Sammelt und aggregiert alle Tageskennzahlen für das Dashboard.
    /// </summary>
    public static class UebersichtGesternDataService
    {
        private const string ConnectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

        private const string LogBasePath = @"P:\TEDuTOZ\MDE";

        private static readonly string[] AusschlussRezeptKeywords =
            { "Heizen", "Lecktest", "Saugleistung" };

        private static readonly Dictionary<string, string> AnlageTabellen =
            new Dictionary<string, string>
            {
                { "A20", "Chargenprotokoll20" },
                { "A25", "Chargenprotokoll25" },
                { "A30", "Chargenprotokoll30" },
                { "A35", "Chargenprotokoll35" },
                { "A40", "Chargenprotokoll40" },
                { "A45", "Chargenprotokoll45" },
                { "A50", "Chargenprotokoll50" },
                { "A60", "Chargenprotokoll60" },
                { "A65", "Chargenprotokoll65" },
            };

        /// <summary>
        /// Hauptmethode: Sammelt alle Daten für den gewählten Tag parallel
        /// (SQL pro Anlage + Logdatei pro Anlage) und liefert das fertige
        /// JSON-fähige Modell.
        /// </summary>
        public static async Task<DashboardData> LoadAsync(DateTime tag)
        {
            var data = new DashboardData
            {
                Datum = tag.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                DatumLang = tag.ToString("dddd, dd.MM.yyyy",
                    new CultureInfo("de-DE")),
            };

            // ── SQL-Daten je Anlage parallel laden ───────────────────────────
            var sqlTasks = AnlageTabellen
                .Select(kvp => LadeAnlageSqlAsync(kvp.Key, kvp.Value, tag))
                .ToArray();

            // ── Logdaten je Anlage parallel verarbeiten ──────────────────────
            var logTasks = AnlageTabellen.Keys
                .Select(name => Task.Run(() => LadeAnlageLog(name, tag)))
                .ToArray();

            var sqlResults = await Task.WhenAll(sqlTasks);
            var logResults = await Task.WhenAll(logTasks);

            // ── Zusammenführen pro Anlage ───────────────────────────────────
            var rezeptZaehler = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var artikelGesamt = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < sqlResults.Length; i++)
            {
                var sql = sqlResults[i];
                var log = logResults[i];

                var anlage = new AnlageKpi
                {
                    Name = sql.Anlage,
                    Stk = sql.GesamtStk,
                    Chargen = sql.GesamtChargen - sql.GesamtProben,
                    Proben = sql.GesamtProben,
                    ProduktivStunden = Math.Round(log.Produktiv.TotalHours, 2),
                    FehlerStunden = Math.Round(log.Fehler.TotalHours, 2),
                    LetzteUhrzeit = log.LetzteUhrzeit ?? string.Empty,
                };

                // Auslastung: produktive Stunden / 24h
                anlage.AuslastungProzent =
                    Math.Round(anlage.ProduktivStunden / 24.0 * 100.0, 1);

                data.Anlagen.Add(anlage);

                // Heatmap-Zellen aus SQL
                foreach (var kv in sql.StkProStunde)
                {
                    data.Heatmap.Add(new HeatmapZelle
                    {
                        Anlage = sql.Anlage,
                        Stunde = kv.Key,
                        Stk = kv.Value,
                    });
                }

                // Timeline-Blöcke aus Log
                data.Timeline.AddRange(log.Bloecke);

                // Rezept- und Artikel-Sammlung über alle Anlagen aufaddieren
                foreach (var r in sql.Rezepte)
                {
                    if (!rezeptZaehler.ContainsKey(r.Key))
                        rezeptZaehler[r.Key] = 0;
                    rezeptZaehler[r.Key] += r.Value;
                }

                foreach (var a in sql.ArtikelStk)
                {
                    if (!artikelGesamt.ContainsKey(a.Key))
                        artikelGesamt[a.Key] = 0;
                    artikelGesamt[a.Key] += a.Value;
                }

                // Artikel-Baum: Ebene 1 = Anlage, Ebene 2 = Artikel
                if (sql.ArtikelStk.Count > 0)
                {
                    var kinder = sql.ArtikelStk
                        .OrderByDescending(x => x.Value)
                        .Select(x => new ArtikelKnoten { Name = x.Key, Value = x.Value })
                        .ToList();

                    data.ArtikelBaum.Add(new ArtikelKnoten
                    {
                        Name = sql.Anlage,
                        Value = sql.GesamtStk,
                        Children = kinder,
                    });
                }
            }

            // ── Gesamtwerte berechnen ────────────────────────────────────────
            data.GesamtStk = data.Anlagen.Sum(a => a.Stk);
            data.AnzahlChargen = data.Anlagen.Sum(a => a.Chargen);
            data.AnzahlProben = data.Anlagen.Sum(a => a.Proben);
            data.AnzahlArtikel = artikelGesamt.Count;
            data.AnzahlRezepte = rezeptZaehler.Count;
            data.ProduktivStunden = Math.Round(data.Anlagen.Sum(a => a.ProduktivStunden), 1);
            data.FehlerStunden = Math.Round(data.Anlagen.Sum(a => a.FehlerStunden), 1);

            // Ø Auslastung: produktive Stunden / (Anzahl Anlagen * 24h)
            double moeglich = data.Anlagen.Count * 24.0;
            data.AuslastungProzent = moeglich > 0
                ? Math.Round(data.ProduktivStunden / moeglich * 100.0, 1)
                : 0;

            // Top-10 Artikel
            data.TopArtikel = artikelGesamt
                .OrderByDescending(kv => kv.Value)
                .Take(10)
                .Select(kv => new TopArtikel { Artikelnummer = kv.Key, Stk = kv.Value })
                .ToList();

            // Rezept-Verteilung (Top 12 für saubere Darstellung)
            data.Rezepte = rezeptZaehler
                .OrderByDescending(kv => kv.Value)
                .Take(12)
                .Select(kv => new RezeptAnteil { Name = kv.Key, Anzahl = kv.Value })
                .ToList();

            return data;
        }

        // ── SQL pro Anlage ───────────────────────────────────────────────────

        private class AnlageSqlResult
        {
            public string Anlage { get; set; }
            public int GesamtStk { get; set; }
            public int GesamtChargen { get; set; }
            public int GesamtProben { get; set; }
            public Dictionary<int, int> StkProStunde { get; set; } = new Dictionary<int, int>();
            public Dictionary<string, int> ArtikelStk { get; set; }
                = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, int> Rezepte { get; set; }
                = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Liest ein Chargenprotokoll der gewählten Anlage am gewählten Tag aus.
        /// Behandelt sowohl die Spalte [Datum] als auch die Legacy-Spalte [ Datum].
        /// </summary>
        private static async Task<AnlageSqlResult> LadeAnlageSqlAsync(
            string anlage, string tabelle, DateTime tag)
        {
            var result = new AnlageSqlResult { Anlage = anlage };

            async Task ReadWithColumnAsync(string col)
            {
                // Datum mit Uhrzeit selektieren (Stundenverteilung für Heatmap)
                // Probe wird über Artikelnummer1='Probe' erkannt.
                string sql =
                    $"SELECT {col} AS Datum, " +
                    $"  Rezept, " +
                    $"  Artikelnummer1, Stk1, " +
                    $"  Artikelnummer2, Stk2, " +
                    $"  Artikelnummer3, Stk3 " +
                    $"FROM [{tabelle}] " +
                    $"WHERE CAST({col} AS date) = @d";

                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@d", SqlDbType.Date).Value = tag.Date;
                    await conn.OpenAsync();
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        int idxDatum = rdr.GetOrdinal("Datum");
                        int idxRezept = SafeOrdinal(rdr, "Rezept");

                        while (await rdr.ReadAsync())
                        {
                            result.GesamtChargen++;

                            // Stunde der Charge bestimmen (für Heatmap)
                            int stunde = 0;
                            if (!rdr.IsDBNull(idxDatum))
                            {
                                var val = rdr.GetValue(idxDatum);
                                if (val is DateTime dt)
                                {
                                    stunde = dt.Hour;
                                }
                                else
                                {
                                    DateTime.TryParse(
                                        Convert.ToString(val, CultureInfo.InvariantCulture),
                                        out DateTime parsed);
                                    stunde = parsed.Hour;
                                }
                            }

                            // Rezept zählen
                            if (idxRezept >= 0 && !rdr.IsDBNull(idxRezept))
                            {
                                string rezept = (rdr.GetValue(idxRezept) as string ?? "").Trim();
                                if (!string.IsNullOrEmpty(rezept)
                                    && !IstAusschlussRezept(rezept))
                                {
                                    if (!result.Rezepte.ContainsKey(rezept))
                                        result.Rezepte[rezept] = 0;
                                    result.Rezepte[rezept]++;
                                }
                            }

                            // Bis zu 3 Artikel pro Charge: Stk aufaddieren
                            int chargenStk = 0;
                            bool istProbe = false;
                            for (int i = 1; i <= 3; i++)
                            {
                                string artCol = $"Artikelnummer{i}";
                                string stkCol = $"Stk{i}";

                                string artikel = ReadString(rdr, artCol);
                                if (string.IsNullOrEmpty(artikel))
                                    continue;

                                int stk = ReadInt(rdr, stkCol);

                                if (string.Equals(artikel, "Probe",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    istProbe = true;
                                }
                                else
                                {
                                    if (!result.ArtikelStk.ContainsKey(artikel))
                                        result.ArtikelStk[artikel] = 0;
                                    result.ArtikelStk[artikel] += stk;
                                    chargenStk += stk;
                                }
                            }

                            if (istProbe) result.GesamtProben++;
                            result.GesamtStk += chargenStk;

                            if (!result.StkProStunde.ContainsKey(stunde))
                                result.StkProStunde[stunde] = 0;
                            result.StkProStunde[stunde] += chargenStk;
                        }
                    }
                }
            }

            try
            {
                try
                {
                    await ReadWithColumnAsync("[Datum]");
                }
                catch (SqlException ex) when (ex.Number == 207)
                {
                    await ReadWithColumnAsync("[ Datum]");
                }
            }
            catch (SqlException ex) when (ex.Number == 208)
            {
                // Tabelle existiert nicht (Anlage außer Betrieb) – leeres Ergebnis liefern
            }
            catch
            {
                // andere Fehler still tolerieren, damit eine Anlage nicht das ganze
                // Dashboard zum Absturz bringt
            }

            return result;
        }

        // ── Log pro Anlage ───────────────────────────────────────────────────

        private class AnlageLogResult
        {
            public TimeSpan Produktiv { get; set; }
            public TimeSpan Fehler { get; set; }
            public string LetzteUhrzeit { get; set; }
            public List<TimelineBlock> Bloecke { get; set; } = new List<TimelineBlock>();
        }

        /// <summary>
        /// Verarbeitet die Maschinen-Logdatei einer Anlage für den gewählten Tag.
        /// Erzeugt Timeline-Blöcke (Produktiv / Fehler) für die Gantt-Ansicht
        /// und summiert Zeiten für die KPI-Karten.
        /// </summary>
        private static AnlageLogResult LadeAnlageLog(string anlage, DateTime tag)
        {
            var result = new AnlageLogResult();

            string folder = "Anl_" + anlage.Substring(1); // "A20" -> "Anl_20"
            string path = Path.Combine(LogBasePath, folder, "machine_state_ge.txt");
            if (!File.Exists(path))
                return result;

            string dateString = tag.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime tagesAnfang = tag.Date;

            string latestTime = "";
            bool fehlerAktiv = false;
            int fehlerStartSec = 0;
            int letzterBestaetigtSec = -1;

            int ParseHmsToSec(string hms)
            {
                int hh = (hms[0] - '0') * 10 + (hms[1] - '0');
                int mm = (hms[3] - '0') * 10 + (hms[4] - '0');
                int ss = (hms[6] - '0') * 10 + (hms[7] - '0');
                return hh * 3600 + mm * 60 + ss;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite, bufferSize: 1 << 20))
                using (var reader = new StreamReader(fs,
                    Encoding.GetEncoding("windows-1252"),
                    detectEncodingFromByteOrderMarks: true, bufferSize: 1 << 20))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = Sanitize(line);
                        if (string.IsNullOrWhiteSpace(line) || line.Length < 19) continue;
                        line = line.Trim();
                        if (!string.Equals(line.Substring(0, 10), dateString, StringComparison.Ordinal))
                            continue;

                        bool skip = false;
                        for (int i = 0; i < AusschlussRezeptKeywords.Length; i++)
                        {
                            if (line.IndexOf(AusschlussRezeptKeywords[i], StringComparison.OrdinalIgnoreCase) >= 0)
                            { skip = true; break; }
                        }
                        if (skip) continue;

                        string uhrzeit = line.Substring(11, 8);
                        if (string.CompareOrdinal(uhrzeit, latestTime) > 0)
                            latestTime = uhrzeit;

                        int tSec = ParseHmsToSec(uhrzeit);

                        // Produktivzeit + Timeline-Block bei gültigem Rezept
                        int rezeptPos = line.IndexOf("Rezept = ", StringComparison.Ordinal);
                        if (rezeptPos >= 0)
                        {
                            string rezeptName = line.Substring(rezeptPos + "Rezept = ".Length).Trim();
                            if (IstProduktivRezept(rezeptName))
                            {
                                int zdPos = line.IndexOf("Zeitdauer = ", StringComparison.Ordinal);
                                if (zdPos >= 0)
                                {
                                    string zd = line.Substring(zdPos + "Zeitdauer = ".Length).Trim();
                                    var dauer = ParseZeitdauer(zd);
                                    if (dauer > TimeSpan.Zero)
                                    {
                                        result.Produktiv += dauer;

                                        // Block-Ende ist der Zeitstempel der Zeile, Anfang = Ende - Dauer
                                        long endeMs = (long)(tSec * 1000L);
                                        long startMs = endeMs - (long)dauer.TotalMilliseconds;
                                        if (startMs < 0) startMs = 0;

                                        result.Bloecke.Add(new TimelineBlock
                                        {
                                            Anlage = anlage,
                                            StartMs = startMs,
                                            EndeMs = endeMs,
                                            Typ = "produktiv",
                                            Info = $"{rezeptName} ({FormatSpan(dauer)})",
                                        });
                                    }
                                }
                            }
                        }

                        // Fehlerblock-Erkennung: Status 8 startet, letzte 7 schließt ab
                        int msPos = line.IndexOf("Maschinenstatus = ", StringComparison.Ordinal);
                        if (msPos >= 0)
                        {
                            int codeIdx = msPos + "Maschinenstatus = ".Length;
                            int code = ReadLeadingInt(line, codeIdx);

                            if (code == 8)
                            {
                                if (!fehlerAktiv)
                                {
                                    fehlerAktiv = true;
                                    fehlerStartSec = tSec;
                                    letzterBestaetigtSec = -1;
                                }
                            }
                            else if (code == 7)
                            {
                                if (fehlerAktiv)
                                    letzterBestaetigtSec = tSec;
                            }
                            else
                            {
                                if (fehlerAktiv && letzterBestaetigtSec >= 0)
                                {
                                    int diff = letzterBestaetigtSec - fehlerStartSec;
                                    if (diff > 0)
                                    {
                                        result.Fehler += TimeSpan.FromSeconds(diff);
                                        result.Bloecke.Add(new TimelineBlock
                                        {
                                            Anlage = anlage,
                                            StartMs = (long)fehlerStartSec * 1000L,
                                            EndeMs = (long)letzterBestaetigtSec * 1000L,
                                            Typ = "fehler",
                                            Info = $"Fehler ({FormatSpan(TimeSpan.FromSeconds(diff))})",
                                        });
                                    }
                                    fehlerAktiv = false;
                                    letzterBestaetigtSec = -1;
                                }
                            }
                        }
                    }
                }

                if (fehlerAktiv && letzterBestaetigtSec >= 0)
                {
                    int diff = letzterBestaetigtSec - fehlerStartSec;
                    if (diff > 0)
                    {
                        result.Fehler += TimeSpan.FromSeconds(diff);
                        result.Bloecke.Add(new TimelineBlock
                        {
                            Anlage = anlage,
                            StartMs = (long)fehlerStartSec * 1000L,
                            EndeMs = (long)letzterBestaetigtSec * 1000L,
                            Typ = "fehler",
                            Info = $"Fehler ({FormatSpan(TimeSpan.FromSeconds(diff))})",
                        });
                    }
                }
            }
            catch
            {
                // Fehler in einer einzelnen Datei (z. B. Netzlaufwerk weg) sollen
                // das Dashboard nicht blockieren.
            }

            result.LetzteUhrzeit = latestTime;
            return result;
        }

        // ── Hilfsmethoden ────────────────────────────────────────────────────

        private static bool IstProduktivRezept(string rezept)
        {
            if (string.IsNullOrEmpty(rezept)) return false;
            char c = char.ToUpperInvariant(rezept[0]);
            if (c == 'B' || c == 'P' || c == 'U' || c == 'W' || c == 'A')
                return true;
            if (c == 'S' && rezept.Length >= 2 && char.IsDigit(rezept[1]))
                return true;
            return false;
        }

        private static bool IstAusschlussRezept(string rezept)
        {
            for (int i = 0; i < AusschlussRezeptKeywords.Length; i++)
            {
                if (rezept.IndexOf(AusschlussRezeptKeywords[i],
                    StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private static TimeSpan ParseZeitdauer(string zd)
        {
            if (string.IsNullOrWhiteSpace(zd)) return TimeSpan.Zero;
            var parts = zd.Split(':');
            try
            {
                if (parts.Length == 1
                    && int.TryParse(parts[0], NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int s1))
                    return TimeSpan.FromSeconds(s1);
                if (parts.Length == 2
                    && int.TryParse(parts[0], NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int m2)
                    && int.TryParse(parts[1], NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int s2))
                    return new TimeSpan(0, m2, s2);
                if (parts.Length == 3
                    && int.TryParse(parts[0], NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int h3)
                    && int.TryParse(parts[1], NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int m3)
                    && int.TryParse(parts[2], NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int s3))
                    return new TimeSpan(h3, m3, s3);
            }
            catch { }
            return TimeSpan.Zero;
        }

        private static string FormatSpan(TimeSpan t)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0:D2}:{1:D2}:{2:D2}",
                (int)t.TotalHours, t.Minutes, t.Seconds);
        }

        private static int ReadLeadingInt(string s, int from)
        {
            int acc = 0; bool any = false;
            int i = from;
            while (i < s.Length)
            {
                char c = s[i];
                if (c < '0' || c > '9') break;
                acc = acc * 10 + (c - '0');
                any = true; i++;
            }
            return any ? acc : -1;
        }

        private static string Sanitize(string line)
        {
            if (string.IsNullOrEmpty(line)) return line;
            var sb = new StringBuilder(line.Length);
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (!char.IsControl(c) || c == '\t') sb.Append(c);
            }
            return sb.ToString();
        }

        private static int SafeOrdinal(IDataRecord rdr, string name)
        {
            for (int i = 0; i < rdr.FieldCount; i++)
                if (string.Equals(rdr.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        private static string ReadString(IDataRecord rdr, string name)
        {
            int idx = SafeOrdinal(rdr, name);
            if (idx < 0 || rdr.IsDBNull(idx)) return string.Empty;
            return (Convert.ToString(rdr.GetValue(idx),
                CultureInfo.InvariantCulture) ?? "").Trim();
        }

        private static int ReadInt(IDataRecord rdr, string name)
        {
            int idx = SafeOrdinal(rdr, name);
            if (idx < 0 || rdr.IsDBNull(idx)) return 0;
            var v = rdr.GetValue(idx);
            if (v is int i32) return i32;
            int.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed);
            return parsed;
        }
    }
}

// -----------------------------------------------------------------------------
// Datenservice für das "Waschanlagen-Dashboard" (Live-Tagesansicht).
//
// Quelle: SQL-Server-DB "SOA127_Waschtragerl", Tabelle "RFID_Aufzeichnung"
// (dieselbe Quelle wie die bestehende RFID-Ansicht, Form_RFIDAnsichtWaschanlagen).
//
//   Spalten (Roh → Bedeutung):
//     Datum            → Zeitstempel der Buchung (Datum + Uhrzeit)
//     DB27             → Waschanlage  ("Elma Aceton" / "UCM497")
//     DB25             → Stückzahl (Stk.)
//     DB4 + DB5        → Artikelnummer (zusammengesetzt)
//     DB28 + DB29+DB30 → Auftragsnummer (zusammengesetzt)
//
// Aggregiert die Buchungen EINES Tages zu einem flachen, JSON-serialisierbaren
// Modell (WaschanlagenDashboardData), das die HTML/ECharts-Oberfläche
// (Resources/WaschanlagenDashboard.html) direkt als window.__WASCH_DATA__ nutzt.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace VerwaltungKST1127.Produktionsauswertung
{
    // ── JSON-Modell (wird ins HTML injiziert) ───────────────────────────────

    /// <summary>Gesamtmodell für einen Tag, das die Oberfläche rendert.</summary>
    public class WaschanlagenDashboardData
    {
        public string DatumLang { get; set; }        // "Dienstag, 17.06.2026"
        public bool IstHeute { get; set; }
        public string Stand { get; set; }            // "Stand 07:58" (nur heute) bzw. ""

        public int TragerlGesamt { get; set; }       // Anzahl Buchungen (Tragerl) am Tag
        public int StkGesamt { get; set; }           // Summe Stückzahl
        public int AktiveAnlagen { get; set; }       // Anlagen mit Buchungen am Tag
        public int AnzahlAnlagen { get; set; }       // fix: 2
        public int AnzahlArtikel { get; set; }       // verschiedene Artikelnummern
        public string LetzteBuchung { get; set; }    // "07:58" / "—"

        public List<AnlageStatus> Anlagen { get; set; } = new List<AnlageStatus>();
        public string[] Stunden { get; set; }                       // ["06" … "21"]
        public List<int[]> Heat { get; set; } = new List<int[]>();  // [stundeIdx, anlageIdx, anzahl]
        public List<NameWert> Artikel { get; set; } = new List<NameWert>();   // Anteil Tragerl je Artikel
        public List<FlussEintrag> Fluss { get; set; } = new List<FlussEintrag>(); // letzte Buchungen
    }

    /// <summary>Status-Kachel einer einzelnen Waschanlage.</summary>
    public class AnlageStatus
    {
        public string Name { get; set; }
        public string Status { get; set; }           // "run" (läuft) | "idle" (inaktiv)
        public int Tragerl { get; set; }
        public int Stk { get; set; }
        public int AnteilProzent { get; set; }       // Anteil an den heutigen Tragerl (Gauge)
        public string LetzteBuchung { get; set; }    // "07:58" / "—"
        public string LetzterArtikel { get; set; }   // zuletzt gewaschene Artikelnummer
    }

    /// <summary>Generisches Name/Wert-Paar (für die Artikel-Verteilung).</summary>
    public class NameWert
    {
        public string Name { get; set; }
        public int Wert { get; set; }
    }

    /// <summary>Eine Zeile im RFID-Tragerl-Fluss (Live-Ticker).</summary>
    public class FlussEintrag
    {
        public string Zeit { get; set; }             // "07:58"
        public string Artikel { get; set; }          // Artikelnummer (DB4+DB5)
        public string Auftrag { get; set; }          // Auftragsnummer (DB28+DB29+DB30)
        public string Anlage { get; set; }           // "Elma Aceton" / "UCM497"
        public int Stk { get; set; }
        public string Status { get; set; }           // Status der Anlage ("run"/"idle")
    }

    // ── Service ──────────────────────────────────────────────────────────────

    public static class WaschanlagenDashboardDataService
    {
        private const string ConnectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Waschtragerl;Integrated Security=True;Encrypt=False";

        // Feste Anlagen-Liste in gewünschter Anzeigereihenfolge. Auch ohne
        // Buchungen am Tag erscheinen beide Kacheln (dann eben "inaktiv").
        public static readonly string[] AnlagenNamen = { "Elma Aceton", "UCM497" };

        // Tages-Zeitfenster der Heatmap (Produktion läuft in dieser Spanne).
        private const int StundeVon = 6;
        private const int StundeBis = 21;

        // "läuft": letzte Buchung liegt höchstens so viele Minuten zurück (nur heute).
        private const int RunFensterMinuten = 90;

        /// <summary>
        /// Lädt und aggregiert alle Buchungen des angegebenen Tages.
        /// </summary>
        public static Task<WaschanlagenDashboardData> LoadAsync(DateTime tag)
        {
            return Task.Run(() => Load(tag));
        }

        private static WaschanlagenDashboardData Load(DateTime tag)
        {
            tag = tag.Date;
            var deDE = new CultureInfo("de-DE");
            bool istHeute = tag == DateTime.Today;
            DateTime jetzt = DateTime.Now;

            // Rohbuchungen des Tages einlesen.
            var buchungen = LeseBuchungen(tag);

            var result = new WaschanlagenDashboardData
            {
                DatumLang = tag.ToString("dddd, dd.MM.yyyy", deDE),
                IstHeute = istHeute,
                AnzahlAnlagen = AnlagenNamen.Length,
                Stunden = Enumerable.Range(StundeVon, StundeBis - StundeVon + 1)
                                    .Select(h => h.ToString("00")).ToArray(),
            };

            result.TragerlGesamt = buchungen.Count;
            result.StkGesamt = buchungen.Sum(b => b.Stk);
            result.AnzahlArtikel = buchungen.Where(b => !string.IsNullOrWhiteSpace(b.Artikel))
                                            .Select(b => b.Artikel).Distinct().Count();

            DateTime? letzteGesamt = buchungen.Count > 0 ? buchungen.Max(b => b.Zeit) : (DateTime?)null;
            result.LetzteBuchung = letzteGesamt.HasValue ? letzteGesamt.Value.ToString("HH:mm") : "—";
            result.Stand = istHeute && letzteGesamt.HasValue
                ? "Stand " + jetzt.ToString("HH:mm")
                : string.Empty;

            // ── Anlagen-Kacheln ──────────────────────────────────────────────
            int aktive = 0;
            foreach (var name in AnlagenNamen)
            {
                var eigene = buchungen.Where(b => string.Equals(b.Anlage, name, StringComparison.OrdinalIgnoreCase)).ToList();
                DateTime? letzte = eigene.Count > 0 ? eigene.Max(b => b.Zeit) : (DateTime?)null;
                if (eigene.Count > 0) aktive++;

                bool laeuft = istHeute && letzte.HasValue
                              && (jetzt - letzte.Value).TotalMinutes <= RunFensterMinuten;

                string letzterArtikel = eigene
                    .OrderByDescending(b => b.Zeit)
                    .Select(b => b.Artikel)
                    .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a)) ?? "—";

                result.Anlagen.Add(new AnlageStatus
                {
                    Name = name,
                    Status = laeuft ? "run" : "idle",
                    Tragerl = eigene.Count,
                    Stk = eigene.Sum(b => b.Stk),
                    AnteilProzent = result.TragerlGesamt > 0
                        ? (int)Math.Round(eigene.Count * 100.0 / result.TragerlGesamt) : 0,
                    LetzteBuchung = letzte.HasValue ? letzte.Value.ToString("HH:mm") : "—",
                    LetzterArtikel = letzterArtikel,
                });
            }
            result.AktiveAnlagen = aktive;

            // ── Heatmap: Anzahl Tragerl je Stunde × Anlage ───────────────────
            for (int ai = 0; ai < AnlagenNamen.Length; ai++)
            {
                var name = AnlagenNamen[ai];
                for (int si = 0; si < result.Stunden.Length; si++)
                {
                    int stunde = StundeVon + si;
                    int anzahl = buchungen.Count(b =>
                        string.Equals(b.Anlage, name, StringComparison.OrdinalIgnoreCase) &&
                        Clamp(b.Zeit.Hour, StundeVon, StundeBis) == stunde);
                    result.Heat.Add(new[] { si, ai, anzahl });
                }
            }

            // ── Artikel-Verteilung (Anteil Tragerl, Top 8 + "Übrige") ────────
            var proArtikel = buchungen
                .Where(b => !string.IsNullOrWhiteSpace(b.Artikel))
                .GroupBy(b => b.Artikel)
                .Select(g => new NameWert { Name = g.Key, Wert = g.Count() })
                .OrderByDescending(x => x.Wert)
                .ToList();

            if (proArtikel.Count > 8)
            {
                var top = proArtikel.Take(8).ToList();
                int rest = proArtikel.Skip(8).Sum(x => x.Wert);
                top.Add(new NameWert { Name = "Übrige", Wert = rest });
                result.Artikel = top;
            }
            else
            {
                result.Artikel = proArtikel;
            }

            // ── RFID-Tragerl-Fluss: letzte 12 Buchungen (neueste zuerst) ─────
            var statusMap = result.Anlagen.ToDictionary(a => a.Name, a => a.Status, StringComparer.OrdinalIgnoreCase);
            result.Fluss = buchungen
                .OrderByDescending(b => b.Zeit)
                .Take(12)
                .Select(b => new FlussEintrag
                {
                    Zeit = b.Zeit.ToString("HH:mm"),
                    Artikel = string.IsNullOrWhiteSpace(b.Artikel) ? "—" : b.Artikel,
                    Auftrag = string.IsNullOrWhiteSpace(b.Auftrag) ? "—" : b.Auftrag,
                    Anlage = b.Anlage,
                    Stk = b.Stk,
                    Status = statusMap.TryGetValue(b.Anlage ?? "", out var s) ? s : "idle",
                })
                .ToList();

            return result;
        }

        // Eine einzelne Rohbuchung aus RFID_Aufzeichnung.
        private class Buchung
        {
            public DateTime Zeit;
            public string Anlage;
            public string Artikel;
            public string Auftrag;
            public int Stk;
        }

        // Liest die Buchungen eines Tages aus der Datenbank.
        private static List<Buchung> LeseBuchungen(DateTime tag)
        {
            var liste = new List<Buchung>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                const string query = "SELECT * FROM RFID_Aufzeichnung WHERE Datum BETWEEN @Ab AND @Bis";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Ab", tag);
                    cmd.Parameters.AddWithValue("@Bis", tag.AddDays(1).AddSeconds(-1));

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        bool hasDatum = dt.Columns.Contains("Datum");
                        foreach (DataRow row in dt.Rows)
                        {
                            // Zeitstempel: aus "Datum" (Datum + Uhrzeit). Fällt der
                            // Wert weg, wird die Zeile ans Tagesende einsortiert.
                            DateTime zeit = tag;
                            if (hasDatum && row["Datum"] != DBNull.Value)
                            {
                                try { zeit = Convert.ToDateTime(row["Datum"]); }
                                catch { zeit = tag; }
                            }

                            liste.Add(new Buchung
                            {
                                Zeit = zeit,
                                Anlage = Feld(row, "DB27"),
                                Artikel = (Feld(row, "DB4") + Feld(row, "DB5")).Trim(),
                                Auftrag = (Feld(row, "DB28") + Feld(row, "DB29") + Feld(row, "DB30")).Trim(),
                                Stk = ParseInt(Feld(row, "DB25")),
                            });
                        }
                    }
                }
            }

            return liste;
        }

        // Liest ein Feld als String (leer, wenn Spalte fehlt oder NULL).
        private static string Feld(DataRow row, string spalte)
        {
            if (!row.Table.Columns.Contains(spalte)) return string.Empty;
            object v = row[spalte];
            return v == DBNull.Value ? string.Empty : v.ToString().Trim();
        }

        private static int ParseInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            return int.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out int v) ? v : 0;
        }

        private static int Clamp(int wert, int min, int max)
            => wert < min ? min : (wert > max ? max : wert);
    }
}

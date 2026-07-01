// -----------------------------------------------------------------------------
// Datenservice für das "Produktions-Trend"-Cockpit (Zeitraum-Auswertung).
//
// Aggregiert die Tageskennzahlen über einen frei wählbaren Zeitraum
// (ein einzelner Tag, eine Woche, ein Monat oder beliebig weit zurück).
// Wiederverwendet bewusst den bestehenden Tages-Datenservice
// (UebersichtGesternDataService.LoadAsync) pro Tag und fasst die Ergebnisse
// zusammen – dadurch bleibt die gesamte SQL-/Logdatei-Logik an einer Stelle.
//
// Liefert ein flaches, JSON-serialisierbares Modell (TrendDashboardData),
// das von der HTML/ECharts-Oberfläche (TrendDashboard.html) direkt verwendet wird.
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VerwaltungKST1127.Produktionsauswertung
{
    /// <summary>
    /// Aggregierte Zeitraum-Daten für das ECharts-Trend-Cockpit.
    /// Wird als JSON ins HTML eingebettet (window.__TREND_DATA__).
    /// </summary>
    public class TrendDashboardData
    {
        public string VonDatum { get; set; }            // "01.06.2026"
        public string BisDatum { get; set; }            // "07.06.2026"
        public string ZeitraumLang { get; set; }        // "Mo 01.06. – So 07.06.2026 · 7 Tage"
        public int AnzahlTage { get; set; }              // Kalendertage im Zeitraum
        public int Produktionstage { get; set; }         // Tage mit tatsächlicher Produktion (> 0 Stk)

        public int GesamtStk { get; set; }
        public int AnzahlChargen { get; set; }
        public int AnzahlProben { get; set; }
        public int AnzahlArtikel { get; set; }          // verschiedene Artikel im Zeitraum
        public int AnzahlRezepte { get; set; }          // verschiedene Rezepte im Zeitraum
        public double ProduktivStunden { get; set; }
        public double FehlerStunden { get; set; }
        // Ø Auslastung auf Basis der realen Kapazität je aktiver Anlage und Tag.
        // Diese ist wochentagsabhängig: Mo–Do 16 h, Fr 11 h (verkürzter Tag),
        // Sa (nur wenn einbezogen) 11 h, So 0 h.
        public double AuslastungProzent { get; set; }

        // Ob der Samstag in dieser Auswertung berücksichtigt wurde (Häkchen im
        // Cockpit). Sonntag wird generell ignoriert. Wird der HTML-Oberfläche
        // übergeben, um die Auslastungs-Basis transparent anzuzeigen.
        public bool SamstagEinbezogen { get; set; }

        public double StkProTag { get; set; }            // Ø über Produktionstage (nicht über Null-Tage)
        public double ProduktivStundenProTag { get; set; }

        public List<TrendTag> Tage { get; set; } = new List<TrendTag>();
        public List<TrendAnlage> Anlagen { get; set; } = new List<TrendAnlage>();
        public List<TopArtikel> TopArtikel { get; set; } = new List<TopArtikel>();
        public List<RezeptAnteil> Rezepte { get; set; } = new List<RezeptAnteil>();
    }

    /// <summary>Kennzahlen eines einzelnen Tages im Zeitraum.</summary>
    public class TrendTag
    {
        public string Datum { get; set; }               // "2026-06-01" (ISO – für Kalender-Heatmap)
        public string DatumKurz { get; set; }           // "Mo 01.06."
        public int Wochentag { get; set; }              // 1=Mo … 7=So
        public int Stk { get; set; }
        public int Chargen { get; set; }
        public double ProduktivStunden { get; set; }
        public double FehlerStunden { get; set; }
        public double AuslastungProzent { get; set; }
    }

    /// <summary>Über den Zeitraum aufsummierte Kennzahlen je Anlage.</summary>
    public class TrendAnlage
    {
        public string Name { get; set; }                // "A20"
        public int Stk { get; set; }
        public int Chargen { get; set; }
        public double ProduktivStunden { get; set; }
        public double FehlerStunden { get; set; }
    }

    /// <summary>
    /// Sammelt und aggregiert die Tageskennzahlen über einen Zeitraum.
    /// </summary>
    public static class ProduktionsTrendDataService
    {
        // Wie viele Tage gleichzeitig geladen werden. Jeder Tag öffnet intern
        // mehrere SQL-Verbindungen und liest mehrere Logdateien – die Drossel
        // verhindert, dass ein Monat hunderte parallele Verbindungen erzeugt.
        private const int MaxParalleleTage = 6;

        // Sicherheitsgrenze, damit ein versehentlich riesiger Zeitraum nicht den
        // SQL-Server flutet (z. B. mehrere Jahre). Knapp über 1 Jahr.
        private const int MaxTage = 400;

        // Nominelle Tageskapazität je Anlage – abhängig vom Wochentag. Eine Anlage
        // läuft bei uns von Montag bis Donnerstag maximal ~16 h pro Tag
        // (Zwei-Schicht-Betrieb). Der Freitag ist ein Sonderfall mit verkürztem
        // Tag: maximal 11 h Produktion. Diese Kapazität bildet die Basis der
        // Auslastungs-Berechnung – nicht 24 h.
        private const double NennstundenMoDo = 16.0;
        private const double NennstundenFreitag = 11.0;
        // Samstag ist ein reiner Sonderschicht-Tag (nur wenn im Cockpit
        // ausdrücklich zugeschaltet). Verkürzt wie der Freitag – bei Bedarf hier
        // anpassen. Sonntag hat keine Kapazität (nie Produktion).
        private const double NennstundenSamstag = 11.0;

        /// <summary>
        /// Nominelle Tageskapazität je Anlage für den jeweiligen Wochentag
        /// (Basis der Auslastung). Mo–Do 16 h, Fr 11 h, Sa 11 h, So 0 h.
        /// </summary>
        private static double NennstundenFuer(DateTime tag)
        {
            switch (tag.DayOfWeek)
            {
                case DayOfWeek.Friday:   return NennstundenFreitag;
                case DayOfWeek.Saturday: return NennstundenSamstag;
                case DayOfWeek.Sunday:   return 0.0;
                default:                 return NennstundenMoDo;
            }
        }

        /// <summary>
        /// Ob ein Tag überhaupt in die Auswertung einfließt. Sonntag wird generell
        /// ignoriert (nie Produktion); Samstag nur, wenn per Häkchen zugeschaltet.
        /// Mo–Fr sind immer dabei.
        /// </summary>
        private static bool TagEinbezogen(DateTime tag, bool samstagEinbeziehen)
        {
            switch (tag.DayOfWeek)
            {
                case DayOfWeek.Sunday:   return false;
                case DayOfWeek.Saturday: return samstagEinbeziehen;
                default:                 return true;
            }
        }

        /// <summary>
        /// Lädt alle Tage zwischen <paramref name="von"/> und <paramref name="bis"/>
        /// (jeweils inklusive) und aggregiert sie zu einem Trend-Modell.
        /// Ist von == bis, entsteht eine Ein-Tages-Auswertung.
        ///
        /// <paramref name="ausgeschlosseneAnlagen"/> (optional): Anlagen, die komplett
        /// aus der Auswertung herausgerechnet werden sollen (KPIs, Trend, Ranking,
        /// Auslastung, Artikel). Null/leer = alle Anlagen einbeziehen.
        ///
        /// <paramref name="samstagEinbeziehen"/> (optional, Standard false): Steuert
        /// den Wochenend-Sonderfall. Sonntag wird generell ignoriert (nie Produktion).
        /// Der Samstag wird nur berücksichtigt, wenn dieses Häkchen gesetzt ist –
        /// andernfalls fällt er wie der Sonntag komplett aus der Auswertung heraus.
        /// </summary>
        public static async Task<TrendDashboardData> LoadAsync(
            DateTime von, DateTime bis, ISet<string> ausgeschlosseneAnlagen = null,
            bool samstagEinbeziehen = false)
        {
            von = von.Date;
            bis = bis.Date;
            if (bis < von)
            {
                var tmp = von; von = bis; bis = tmp;
            }

            // Ausschlussliste in eine case-insensitive Menge normalisieren.
            var excl = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (ausgeschlosseneAnlagen != null)
                foreach (var a in ausgeschlosseneAnlagen) excl.Add(a);

            // Zeitraum auf MaxTage begrenzen (vom Ende her).
            var tage = new List<DateTime>();
            for (var d = von; d <= bis; d = d.AddDays(1))
                tage.Add(d);
            if (tage.Count > MaxTage)
            {
                tage = tage.Skip(tage.Count - MaxTage).ToList();
                von = tage[0];
            }

            var deDE = new CultureInfo("de-DE");

            // ── Tage parallel (gedrosselt) laden ─────────────────────────────
            var sem = new SemaphoreSlim(MaxParalleleTage);
            var dayTasks = tage.Select(async t =>
            {
                await sem.WaitAsync();
                try { return await UebersichtGesternDataService.LoadAsync(t); }
                finally { sem.Release(); }
            }).ToArray();

            // Reihenfolge des Arrays bleibt erhalten → dayResults[i] gehört zu tage[i].
            var dayResults = await Task.WhenAll(dayTasks);

            var result = new TrendDashboardData
            {
                VonDatum = von.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                BisDatum = bis.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                SamstagEinbezogen = samstagEinbeziehen,
            };

            // Aggregations-Sammler
            var anlagenAgg = new Dictionary<string, TrendAnlage>(StringComparer.OrdinalIgnoreCase);
            var artikelGesamt = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var rezeptGesamt = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Summe der realen Tageskapazität (wochentagsabhängig je aktiver Anlage
            // und Tag: Mo–Do 16 h, Fr 11 h, Sa 11 h). Null-Tage (Feiertage ohne
            // Produktion) tragen 0 bei und verfälschen damit die Auslastung nicht.
            double kapazitaetStunden = 0;
            int produktionstage = 0;
            int probenGesamt = 0;

            for (int i = 0; i < dayResults.Length; i++)
            {
                var tag = tage[i];
                var day = dayResults[i];

                // Wochenend-Sonderregel: Sonntag immer ignorieren (nie Produktion),
                // Samstag nur, wenn per Häkchen zugeschaltet. Ein ignorierter Tag
                // fällt komplett aus der Auswertung – er zählt weder in den KPIs
                // noch in Trend, Ranking oder Auslastung.
                if (!TagEinbezogen(tag, samstagEinbeziehen))
                    continue;

                // Nur die einbezogenen Anlagen dieses Tages.
                var inkl = day.Anlagen.Where(a => !excl.Contains(a.Name)).ToList();

                int tagStk = inkl.Sum(a => a.Stk);
                int tagChargen = inkl.Sum(a => a.Chargen);
                int tagProben = inkl.Sum(a => a.Proben);
                double tagProduktiv = Math.Round(inkl.Sum(a => a.ProduktivStunden), 2);
                double tagFehler = Math.Round(inkl.Sum(a => a.FehlerStunden), 1);
                probenGesamt += tagProben;

                // An diesem Tag tatsächlich aktive Anlagen (produktiv gelaufen oder Stk).
                int aktiveAnlagen = inkl.Count(a => a.ProduktivStunden > 0 || a.Stk > 0);
                // Reale Tageskapazität je Anlage abhängig vom Wochentag
                // (Mo–Do 16 h, Fr 11 h, Sa 11 h) – Basis der Auslastung.
                double tagKapazitaet = aktiveAnlagen * NennstundenFuer(tag);
                kapazitaetStunden += tagKapazitaet;

                if (tagStk > 0 || tagProduktiv > 0) produktionstage++;

                // Tages-Auslastung auf Basis der wochentagsabhängigen Kapazität
                // der aktiven Anlagen (Mo–Do 16 h, Fr 11 h, Sa 11 h je Anlage).
                double tagAusl = tagKapazitaet > 0
                    ? Math.Round(tagProduktiv / tagKapazitaet * 100.0, 1) : 0;

                // Tages-Punkt für den Trend-Chart
                result.Tage.Add(new TrendTag
                {
                    Datum = tag.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DatumKurz = tag.ToString("ddd dd.MM.", deDE),
                    Wochentag = ((int)tag.DayOfWeek + 6) % 7 + 1, // So=7, Mo=1
                    Stk = tagStk,
                    Chargen = tagChargen,
                    ProduktivStunden = tagProduktiv,
                    FehlerStunden = tagFehler,
                    AuslastungProzent = tagAusl,
                });

                // Anlagen-Ranking aufsummieren (nur einbezogene Anlagen)
                foreach (var a in inkl)
                {
                    if (!anlagenAgg.TryGetValue(a.Name, out var agg))
                    {
                        agg = new TrendAnlage { Name = a.Name };
                        anlagenAgg[a.Name] = agg;
                    }
                    agg.Stk += a.Stk;
                    agg.Chargen += a.Chargen;
                    agg.ProduktivStunden += a.ProduktivStunden;
                    agg.FehlerStunden += a.FehlerStunden;
                }

                // Verschiedene Artikel + Mengen: aus dem Artikel-Baum (enthält je
                // Anlage ALLE Artikel des Tages, nicht nur die Top-Liste).
                foreach (var anlageKnoten in day.ArtikelBaum)
                {
                    if (anlageKnoten.Children == null) continue;
                    if (excl.Contains(anlageKnoten.Name)) continue;   // Anlage ausgeschlossen
                    foreach (var art in anlageKnoten.Children)
                    {
                        if (string.IsNullOrEmpty(art.Name)) continue;
                        artikelGesamt.TryGetValue(art.Name, out int v);
                        artikelGesamt[art.Name] = v + art.Value;
                    }
                }

                // Verschiedene Rezepte + Häufigkeiten. Hinweis: Die Tages-Rezeptliste
                // liegt nur anlagen-übergreifend vor; eine ausgeschlossene Anlage kann
                // hier nicht abgezogen werden. Für nicht-produzierende Anlagen (ohne
                // Rezepte) ist das irrelevant.
                foreach (var r in day.Rezepte)
                {
                    rezeptGesamt.TryGetValue(r.Name, out int v);
                    rezeptGesamt[r.Name] = v + r.Anzahl;
                }
            }

            // ── Gesamtwerte ─────────────────────────────────────────────────
            // AnzahlTage = tatsächlich ausgewertete Tage (ohne ignorierte
            // Wochenend-Tage), nicht die reinen Kalendertage des Zeitraums.
            result.AnzahlTage = result.Tage.Count;
            result.GesamtStk = result.Tage.Sum(t => t.Stk);
            result.AnzahlChargen = result.Tage.Sum(t => t.Chargen);
            result.AnzahlProben = probenGesamt;
            result.ProduktivStunden = Math.Round(result.Tage.Sum(t => t.ProduktivStunden), 2);
            result.FehlerStunden = Math.Round(result.Tage.Sum(t => t.FehlerStunden), 1);
            result.AnzahlArtikel = artikelGesamt.Count;
            result.AnzahlRezepte = rezeptGesamt.Count;
            result.Produktionstage = produktionstage;

            // Ø nur über Tage mit echter Produktion – Null-Tage würden den
            // Durchschnitt sonst künstlich nach unten ziehen.
            result.StkProTag = produktionstage > 0
                ? Math.Round((double)result.GesamtStk / produktionstage, 0) : 0;
            result.ProduktivStundenProTag = produktionstage > 0
                ? Math.Round(result.ProduktivStunden / produktionstage, 2) : 0;

            // Ø Auslastung: produktive Stunden / reale Kapazität (wochentagsabhängig
            // je aktiver Anlage und Tag: Mo–Do 16 h, Fr 11 h, Sa 11 h). Sonderschichten
            // können dabei kurzzeitig > 100 % ergeben – das ist gewollt und korrekt.
            result.AuslastungProzent = kapazitaetStunden > 0
                ? Math.Round(result.ProduktivStunden / kapazitaetStunden * 100.0, 1) : 0;

            // Anlagen-Ranking (absteigend nach Stk)
            result.Anlagen = anlagenAgg.Values
                .Select(a => new TrendAnlage
                {
                    Name = a.Name,
                    Stk = a.Stk,
                    Chargen = a.Chargen,
                    ProduktivStunden = Math.Round(a.ProduktivStunden, 2),
                    FehlerStunden = Math.Round(a.FehlerStunden, 1),
                })
                .OrderByDescending(a => a.Stk)
                .ToList();

            // Top-15 Artikel im Zeitraum
            result.TopArtikel = artikelGesamt
                .OrderByDescending(kv => kv.Value)
                .Take(15)
                .Select(kv => new TopArtikel { Artikelnummer = kv.Key, Stk = kv.Value })
                .ToList();

            // Alle Rezepte (sortiert nach Häufigkeit)
            result.Rezepte = rezeptGesamt
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new RezeptAnteil { Name = kv.Key, Anzahl = kv.Value })
                .ToList();

            // ── Zeitraum-Beschriftung ───────────────────────────────────────
            if (result.AnzahlTage <= 1)
            {
                result.ZeitraumLang = von.ToString("dddd, dd.MM.yyyy", deDE);
            }
            else
            {
                result.ZeitraumLang = string.Format(
                    "{0} – {1} · {2} Werktage · {3} mit Produktion{4}",
                    von.ToString("ddd dd.MM.yyyy", deDE),
                    bis.ToString("ddd dd.MM.yyyy", deDE),
                    result.AnzahlTage, produktionstage,
                    samstagEinbeziehen ? " · inkl. Sa" : "");
            }

            return result;
        }
    }
}

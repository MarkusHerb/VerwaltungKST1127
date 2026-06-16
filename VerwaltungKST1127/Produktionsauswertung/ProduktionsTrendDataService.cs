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
        public int AnzahlTage { get; set; }

        public int GesamtStk { get; set; }
        public int AnzahlChargen { get; set; }
        public int AnzahlProben { get; set; }
        public int AnzahlArtikel { get; set; }          // verschiedene Artikel im Zeitraum
        public int AnzahlRezepte { get; set; }          // verschiedene Rezepte im Zeitraum
        public double ProduktivStunden { get; set; }
        public double FehlerStunden { get; set; }
        public double AuslastungProzent { get; set; }   // Ø über Anlagen × Tage (24h-Basis)

        public double StkProTag { get; set; }
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

        /// <summary>
        /// Lädt alle Tage zwischen <paramref name="von"/> und <paramref name="bis"/>
        /// (jeweils inklusive) und aggregiert sie zu einem Trend-Modell.
        /// Ist von == bis, entsteht eine Ein-Tages-Auswertung.
        /// </summary>
        public static async Task<TrendDashboardData> LoadAsync(DateTime von, DateTime bis)
        {
            von = von.Date;
            bis = bis.Date;
            if (bis < von)
            {
                var tmp = von; von = bis; bis = tmp;
            }

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
                AnzahlTage = tage.Count,
                VonDatum = von.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                BisDatum = bis.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            };

            // Aggregations-Sammler
            var anlagenAgg = new Dictionary<string, TrendAnlage>(StringComparer.OrdinalIgnoreCase);
            var artikelGesamt = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var rezeptGesamt = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int anlagenProTag = 0;

            for (int i = 0; i < dayResults.Length; i++)
            {
                var tag = tage[i];
                var day = dayResults[i];

                anlagenProTag = Math.Max(anlagenProTag, day.Anlagen.Count);

                // Tages-Punkt für den Trend-Chart
                result.Tage.Add(new TrendTag
                {
                    Datum = tag.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DatumKurz = tag.ToString("ddd dd.MM.", deDE),
                    Wochentag = ((int)tag.DayOfWeek + 6) % 7 + 1, // So=7, Mo=1
                    Stk = day.GesamtStk,
                    Chargen = day.AnzahlChargen,
                    ProduktivStunden = day.ProduktivStunden,
                    FehlerStunden = day.FehlerStunden,
                    AuslastungProzent = day.AuslastungProzent,
                });

                // Anlagen-Ranking aufsummieren
                foreach (var a in day.Anlagen)
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
                    foreach (var art in anlageKnoten.Children)
                    {
                        if (string.IsNullOrEmpty(art.Name)) continue;
                        artikelGesamt.TryGetValue(art.Name, out int v);
                        artikelGesamt[art.Name] = v + art.Value;
                    }
                }

                // Verschiedene Rezepte + Häufigkeiten
                foreach (var r in day.Rezepte)
                {
                    rezeptGesamt.TryGetValue(r.Name, out int v);
                    rezeptGesamt[r.Name] = v + r.Anzahl;
                }
            }

            // ── Gesamtwerte ─────────────────────────────────────────────────
            result.GesamtStk = result.Tage.Sum(t => t.Stk);
            result.AnzahlChargen = result.Tage.Sum(t => t.Chargen);
            result.AnzahlProben = dayResults.Sum(d => d.AnzahlProben);
            result.ProduktivStunden = Math.Round(result.Tage.Sum(t => t.ProduktivStunden), 2);
            result.FehlerStunden = Math.Round(result.Tage.Sum(t => t.FehlerStunden), 1);
            result.AnzahlArtikel = artikelGesamt.Count;
            result.AnzahlRezepte = rezeptGesamt.Count;

            result.StkProTag = result.AnzahlTage > 0
                ? Math.Round((double)result.GesamtStk / result.AnzahlTage, 0) : 0;
            result.ProduktivStundenProTag = result.AnzahlTage > 0
                ? Math.Round(result.ProduktivStunden / result.AnzahlTage, 2) : 0;

            // Ø Auslastung: produktive Stunden / (Anlagen × 24h × Tage)
            if (anlagenProTag == 0) anlagenProTag = anlagenAgg.Count;
            double moeglich = anlagenProTag * 24.0 * result.AnzahlTage;
            result.AuslastungProzent = moeglich > 0
                ? Math.Round(result.ProduktivStunden / moeglich * 100.0, 1) : 0;

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
                    "{0} – {1} · {2} Tage",
                    von.ToString("ddd dd.MM.yyyy", deDE),
                    bis.ToString("ddd dd.MM.yyyy", deDE),
                    result.AnzahlTage);
            }

            return result;
        }
    }
}

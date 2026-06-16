// -----------------------------------------------------------------------------
// Produktions-Trend – Zeitraum-Cockpit
//
// Zeigt eine Produktionsauswertung über einen frei wählbaren Zeitraum:
//   - ein einzelner Tag (Von = Bis)
//   - eine Woche / ein Monat (Schnellwahl-Buttons)
//   - beliebig weit zurück (Von/Bis frei wählbar)
//
// Visualisierungen (ECharts, analog zum "Produktion gestern"-Dashboard):
//   - KPI-Karten (Stk, Ø Stk/Tag, Chargen, Artikel/Rezepte, Produktivzeit, Auslastung)
//   - Tages-Trend (Stk-Balken + Produktivstunden-Linie + gleitender Ø)
//   - Anlagen-Ranking über den Zeitraum
//   - Auslastungs-Kalender (Heatmap je Tag)
//   - Rezept-Verteilung
//
// Renderer: WebView2 mit eingebettetem HTML (Resources/TrendDashboard.html).
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_ProduktionsTrend : Form
    {
        private bool _isLoading;
        private bool _initialized;      // erst nach WebView-Init dürfen Datumswechsel laden
        private bool _suppressReload;   // verhindert Doppellauf beim Setzen beider Picker
        private string _htmlTemplate;

        // Pro Anlage eine Checkbox; unangehakt = aus der Auswertung ausgeschlossen.
        private readonly List<CheckBox> _anlageChecks = new List<CheckBox>();

        // Auto-Refresh: alle 5 Minuten, sekündlicher Countdown im Label.
        // Trend-Daten ändern sich langsamer als die Tagesübersicht, daher 5 Min.
        private const int AutoRefreshSekunden = 5 * 60;
        private int _autoRefreshRest;

        public Form_ProduktionsTrend()
        {
            InitializeComponent();
        }

        private async void Form_ProduktionsTrend_Load(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Initialisiere WebView2…";
                BaueAnlagenCheckboxen();
                await InitWebViewAsync();

                // Standard beim Öffnen: laufende Woche (Montag bis heute), damit
                // sofort die aktuelle Wochenleistung sichtbar ist. Der ValueChanged-
                // Handler ist durch _initialized=false noch deaktiviert.
                dtpVon.Value = WochenAnfang(DateTime.Today);
                dtpBis.Value = DateTime.Today;

                _initialized = true;
                await LadeDashboardAsync();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Fehler: " + ex.Message;
                MessageBox.Show(this, ex.ToString(),
                    "Fehler beim Laden des Trend-Cockpits",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // WebView2-Initialisierung mit eigenem User-Data-Ordner (geteilt mit dem
        // Tages-Dashboard, liegt in %LOCALAPPDATA%).
        private async Task InitWebViewAsync()
        {
            string userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VerwaltungKST1127", "WebView2");
            Directory.CreateDirectory(userDataFolder);

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await webView.EnsureCoreWebView2Async(env);

            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        // Daten beschaffen, JSON ins HTML-Template injizieren und navigieren.
        private async Task LadeDashboardAsync()
        {
            if (_isLoading) return;
            _isLoading = true;
            try
            {
                SetEingabenAktiv(false);
                lblStatus.Text = "Lade Produktionsdaten…";

                DateTime von = dtpVon.Value.Date;
                DateTime bis = dtpBis.Value.Date;

                // Ausgeschlossene Anlagen aus den (nicht angehakten) Checkboxen.
                var ausgeschlossen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var cb in _anlageChecks)
                    if (!cb.Checked) ausgeschlossen.Add(cb.Text);

                // 1) Datenservice (aggregiert die Tage des Zeitraums)
                var data = await ProduktionsTrendDataService.LoadAsync(von, bis, ausgeschlossen);

                // 2) HTML-Template einmalig laden und cachen
                if (_htmlTemplate == null)
                    _htmlTemplate = LadeHtmlTemplate();

                // 3) JSON in <head> injizieren (vor allen weiteren Scripts)
                string json = JsonConvert.SerializeObject(data);
                string inject =
                    "<script>window.__TREND_DATA__ = " + json + ";</script>";
                string finalHtml = _htmlTemplate.Replace("</head>", inject + "</head>");

                // 4) HTML im WebView2 anzeigen
                webView.CoreWebView2.NavigateToString(finalHtml);

                lblStatus.Text = string.Format(
                    "{0}  ·  {1:N0} Stk  ·  {2} Chargen  ·  {3:N1} h produktiv  ·  Ø {4:N1} % Auslastung",
                    data.ZeitraumLang, data.GesamtStk, data.AnzahlChargen,
                    data.ProduktivStunden, data.AuslastungProzent);
            }
            finally
            {
                SetEingabenAktiv(true);
                _isLoading = false;

                if (chkAutoRefresh.Checked)
                    ResetAutoRefreshCountdown();
            }
        }

        private void SetEingabenAktiv(bool aktiv)
        {
            btnNeuLaden.Enabled = aktiv;
            dtpVon.Enabled = aktiv;
            dtpBis.Enabled = aktiv;
            btnGestern.Enabled = aktiv;
            btnHeute.Enabled = aktiv;
            btnMinus2Wochen.Enabled = aktiv;
            btnMinus30Tage.Enabled = aktiv;
            flowAnlagen.Enabled = aktiv;
        }

        // Baut für jede Anlage eine Checkbox (standardmäßig angehakt). Das Abwählen
        // schließt die Anlage komplett aus der Auswertung aus.
        private void BaueAnlagenCheckboxen()
        {
            if (_anlageChecks.Count > 0) return;   // nur einmal aufbauen

            foreach (var name in UebersichtGesternDataService.AnlagenNamen)
            {
                var cb = new CheckBox
                {
                    Text = name,
                    Checked = true,
                    AutoSize = true,
                    ForeColor = Color.FromArgb(230, 235, 250),
                    Font = new Font("Segoe UI", 9.5F),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(0, 4, 14, 0),
                };
                cb.CheckedChanged += anlageCheck_CheckedChanged;
                _anlageChecks.Add(cb);
                flowAnlagen.Controls.Add(cb);
            }
        }

        private async void anlageCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!_initialized || _suppressReload) return;
            await LadeDashboardAsync();
        }

        // Setzt beide Picker ohne Zwischen-Reload und lädt danach genau einmal.
        private async Task SetzeZeitraumAsync(DateTime von, DateTime bis)
        {
            _suppressReload = true;
            try
            {
                // Reihenfolge so wählen, dass Von <= Bis nie verletzt wird
                // (die Picker erlauben beliebige Daten, aber sauberer Ablauf).
                dtpVon.Value = von;
                dtpBis.Value = bis;
            }
            finally { _suppressReload = false; }

            await LadeDashboardAsync();
        }

        // ── Schnellwahl-Buttons ───────────────────────────────────────────────

        private async void btnGestern_Click(object sender, EventArgs e)
        {
            DateTime g = DateTime.Today.AddDays(-1);
            await SetzeZeitraumAsync(g, g);
        }

        private async void btnHeute_Click(object sender, EventArgs e)
        {
            await SetzeZeitraumAsync(DateTime.Today, DateTime.Today);
        }

        // Letzte 2 Wochen bis heute.
        private async void btnMinus2Wochen_Click(object sender, EventArgs e)
        {
            await SetzeZeitraumAsync(DateTime.Today.AddDays(-14), DateTime.Today);
        }

        // Letzte 30 Tage bis heute.
        private async void btnMinus30Tage_Click(object sender, EventArgs e)
        {
            await SetzeZeitraumAsync(DateTime.Today.AddDays(-30), DateTime.Today);
        }

        // Montag der Woche, in der das Datum liegt.
        private static DateTime WochenAnfang(DateTime tag)
        {
            int diff = ((int)tag.DayOfWeek + 6) % 7; // Mo=0 … So=6
            return tag.Date.AddDays(-diff);
        }

        // ── Auto-Refresh ───────────────────────────────────────────────────────

        private void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoRefresh.Checked)
            {
                ResetAutoRefreshCountdown();
                timerAutoRefresh.Start();
            }
            else
            {
                timerAutoRefresh.Stop();
                lblAutoRefreshTimer.Text = string.Empty;
            }
        }

        private void ResetAutoRefreshCountdown()
        {
            _autoRefreshRest = AutoRefreshSekunden;
            AktualisiereTimerLabel();
        }

        private void AktualisiereTimerLabel()
        {
            int m = _autoRefreshRest / 60;
            int s = _autoRefreshRest % 60;
            lblAutoRefreshTimer.Text = string.Format("{0:00}:{1:00}", m, s);
        }

        private async void timerAutoRefresh_Tick(object sender, EventArgs e)
        {
            if (!chkAutoRefresh.Checked) return;

            _autoRefreshRest--;
            if (_autoRefreshRest <= 0)
            {
                _autoRefreshRest = AutoRefreshSekunden;
                AktualisiereTimerLabel();
                await LadeDashboardAsync();
            }
            else
            {
                AktualisiereTimerLabel();
            }
        }

        // Lädt das HTML-Template aus dem eingebetteten Ressourcen-Stream.
        private string LadeHtmlTemplate()
        {
            var asm = Assembly.GetExecutingAssembly();
            string resName = "VerwaltungKST1127.Produktionsauswertung.Resources.TrendDashboard.html";
            using (var stream = asm.GetManifestResourceStream(resName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(
                        "Eingebettete Ressource '" + resName + "' nicht gefunden. " +
                        "Prüfen, ob TrendDashboard.html als EmbeddedResource im csproj registriert ist.");
                }
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        private async void btnNeuLaden_Click(object sender, EventArgs e)
        {
            await LadeDashboardAsync();
        }

        private async void dtpVon_ValueChanged(object sender, EventArgs e)
        {
            if (!_initialized || _suppressReload) return;
            await LadeDashboardAsync();
        }

        private async void dtpBis_ValueChanged(object sender, EventArgs e)
        {
            if (!_initialized || _suppressReload) return;
            await LadeDashboardAsync();
        }
    }
}

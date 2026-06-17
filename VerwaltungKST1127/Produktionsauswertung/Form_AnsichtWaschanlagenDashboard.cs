// -----------------------------------------------------------------------------
// Waschanlagen-Dashboard – Live-Tagesansicht (Elma Aceton / UCM497)
//
// Visualisierungen (ECharts, analog zum Produktions-Trend):
//   - KPI-Karten (Tragerl, Stück, aktive Anlagen, versch. Artikel, letzte Buchung)
//   - Anlagen-Statusboard mit Auslastungs-Gauges
//   - Heatmap "Tragerl je Stunde × Anlage"
//   - Artikel-Verteilung (Donut)
//   - RFID-Tragerl-Fluss (Live-Ticker, inkl. Artikelnummer)
//
// Daten: WaschanlagenDashboardDataService (DB SOA127_Waschtragerl, RFID_Aufzeichnung).
// Renderer: WebView2 mit eingebettetem HTML (Resources/WaschanlagenDashboard.html).
// -----------------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_AnsichtWaschanlagenDashboard : Form
    {
        private bool _isLoading;
        private bool _initialized;      // erst nach WebView-Init darf ein Datumswechsel laden
        private string _htmlTemplate;

        // Auto-Refresh: alle 60 Sekunden (Live-Charakter), sekündlicher Countdown.
        private const int AutoRefreshSekunden = 60;
        private int _autoRefreshRest;

        public Form_AnsichtWaschanlagenDashboard()
        {
            InitializeComponent();
        }

        private async void Form_AnsichtWaschanlagenDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Initialisiere WebView2…";
                await InitWebViewAsync();

                // Standard beim Öffnen: heute.
                dtpDatum.Value = DateTime.Today;

                _initialized = true;
                await LadeDashboardAsync();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Fehler: " + ex.Message;
                MessageBox.Show(this, ex.ToString(),
                    "Fehler beim Laden des Waschanlagen-Dashboards",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // WebView2-Initialisierung mit eigenem User-Data-Ordner (geteilt mit den
        // übrigen Dashboards, liegt in %LOCALAPPDATA%).
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
                lblStatus.Text = "Lade Waschanlagen-Daten…";

                DateTime tag = dtpDatum.Value.Date;

                // 1) Datenservice (aggregiert die Buchungen des Tages)
                var data = await WaschanlagenDashboardDataService.LoadAsync(tag);

                // 2) HTML-Template einmalig laden und cachen
                if (_htmlTemplate == null)
                    _htmlTemplate = LadeHtmlTemplate();

                // 3) JSON in <head> injizieren (vor allen weiteren Scripts)
                string json = JsonConvert.SerializeObject(data);
                string inject = "<script>window.__WASCH_DATA__ = " + json + ";</script>";
                string finalHtml = _htmlTemplate.Replace("</head>", inject + "</head>");

                // 4) HTML im WebView2 anzeigen
                webView.CoreWebView2.NavigateToString(finalHtml);

                lblStatus.Text = string.Format(
                    "{0}  ·  {1:N0} Tragerl  ·  {2:N0} Stk  ·  {3}/{4} Anlagen aktiv  ·  letzte {5}",
                    data.DatumLang, data.TragerlGesamt, data.StkGesamt,
                    data.AktiveAnlagen, data.AnzahlAnlagen, data.LetzteBuchung);
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
            dtpDatum.Enabled = aktiv;
            btnHeute.Enabled = aktiv;
        }

        // ── Bedienelemente ───────────────────────────────────────────────────

        private async void btnHeute_Click(object sender, EventArgs e)
        {
            // ValueChanged löst bei tatsächlicher Änderung das Laden aus; ist
            // bereits "heute" eingestellt, laden wir hier explizit neu.
            if (dtpDatum.Value.Date == DateTime.Today)
                await LadeDashboardAsync();
            else
                dtpDatum.Value = DateTime.Today;
        }

        private async void btnNeuLaden_Click(object sender, EventArgs e)
        {
            await LadeDashboardAsync();
        }

        private async void dtpDatum_ValueChanged(object sender, EventArgs e)
        {
            if (!_initialized) return;
            await LadeDashboardAsync();
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
            string resName = "VerwaltungKST1127.Produktionsauswertung.Resources.WaschanlagenDashboard.html";
            using (var stream = asm.GetManifestResourceStream(resName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(
                        "Eingebettete Ressource '" + resName + "' nicht gefunden. " +
                        "Prüfen, ob WaschanlagenDashboard.html als EmbeddedResource im csproj registriert ist.");
                }
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}

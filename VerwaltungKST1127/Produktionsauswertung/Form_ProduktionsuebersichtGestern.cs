// -----------------------------------------------------------------------------
// Produktion gestern – Dashboard-Fenster
//
// Beim Öffnen wird das Datum auf "gestern" vorbelegt und automatisch eine
// Tagesübersicht mit modernen ECharts-Visualisierungen gerendert:
//   - KPI-Karten (Stk, Chargen, Proben, Artikel/Rezepte, Produktiv-/Fehlerzeit)
//   - Heatmap Stunde × Anlage
//   - Gantt-Timeline pro Anlage (Produktiv / Fehler)
//   - Treemap (Anlage → Artikel)
//   - Rezept-Verteilung
//
// Renderer: WebView2 mit eingebettetem HTML (Resources/Dashboard.html).
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
    public partial class Form_ProduktionsuebersichtGestern : Form
    {
        private bool _isLoading;
        private bool _initialized;     // erst nach WebView-Init dürfen Datumswechsel laden
        private string _htmlTemplate;

        public Form_ProduktionsuebersichtGestern()
        {
            InitializeComponent();
        }

        // Beim ersten Laden des Fensters: WebView2 initialisieren, gestriges Datum
        // vorbelegen und Dashboard rendern.
        private async void Form_ProduktionsuebersichtGestern_Load(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Initialisiere WebView2…";
                await InitWebViewAsync();

                // Datum auf "gestern" vorbelegen. Der ValueChanged-Handler ist durch
                // _initialized=false noch deaktiviert, damit hier kein Doppellauf passiert.
                dateTimePickerTag.Value = DateTime.Today.AddDays(-1);

                _initialized = true;
                await LadeDashboardAsync();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Fehler: " + ex.Message;
                MessageBox.Show(this, ex.ToString(),
                    "Fehler beim Laden des Dashboards",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // WebView2-Initialisierung mit eigenem User-Data-Ordner (vermeidet Konflikte
        // mit anderen WebView2-Instanzen und liegt in %LOCALAPPDATA%).
        private async Task InitWebViewAsync()
        {
            string userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VerwaltungKST1127", "WebView2");
            Directory.CreateDirectory(userDataFolder);

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await webView.EnsureCoreWebView2Async(env);

            // Kontextmenü und DevTools im Produktivbetrieb deaktivieren –
            // das Fenster soll wie eine Cockpit-Anwendung wirken.
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
                btnNeuLaden.Enabled = false;
                dateTimePickerTag.Enabled = false;
                lblStatus.Text = "Lade Produktionsdaten…";

                DateTime tag = dateTimePickerTag.Value.Date;

                // 1) Datenservice (parallel SQL + Logdateien)
                var data = await UebersichtGesternDataService.LoadAsync(tag);

                // 2) HTML-Template einmalig laden und cachen
                if (_htmlTemplate == null)
                    _htmlTemplate = LadeHtmlTemplate();

                // 3) JSON in <head> injizieren (vor allen weiteren Scripts)
                string json = JsonConvert.SerializeObject(data);
                string inject =
                    "<script>window.__DASHBOARD_DATA__ = " + json + ";</script>";
                string finalHtml = _htmlTemplate.Replace("</head>", inject + "</head>");

                // 4) HTML im WebView2 anzeigen
                webView.CoreWebView2.NavigateToString(finalHtml);

                lblStatus.Text = string.Format(
                    "Stand: {0}  ·  {1:N0} Stk  ·  {2} Chargen  ·  {3:N1} h produktiv",
                    data.DatumLang, data.GesamtStk, data.AnzahlChargen,
                    data.ProduktivStunden);
            }
            finally
            {
                btnNeuLaden.Enabled = true;
                dateTimePickerTag.Enabled = true;
                _isLoading = false;
            }
        }

        // Lädt das HTML-Template aus dem eingebetteten Ressourcen-Stream.
        private string LadeHtmlTemplate()
        {
            var asm = Assembly.GetExecutingAssembly();
            string resName = "VerwaltungKST1127.Produktionsauswertung.Resources.Dashboard.html";
            using (var stream = asm.GetManifestResourceStream(resName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException(
                        "Eingebettete Ressource '" + resName + "' nicht gefunden. " +
                        "Prüfen, ob Dashboard.html als EmbeddedResource im csproj registriert ist.");
                }
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        private async void btnNeuLaden_Click(object sender, EventArgs e)
        {
            await LadeDashboardAsync();
        }

        private async void dateTimePickerTag_ValueChanged(object sender, EventArgs e)
        {
            // Beim Datumswechsel sofort neu laden, aber erst wenn WebView fertig
            // initialisiert ist (verhindert Race mit dem Vorbelegen in Form_Load).
            if (!_initialized) return;
            await LadeDashboardAsync();
        }
    }
}

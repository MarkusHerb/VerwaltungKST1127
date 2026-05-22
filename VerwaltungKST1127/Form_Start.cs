// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben können.
// ===================================================================================================
using System;                                                // Basis-Typen (string, int, DateTime, EventArgs, ...)
using System.Data.SqlClient;                                 // SQL-Server-Zugriff (SqlConnection, SqlCommand)
using System.Diagnostics;                                    // Prozesse starten, Debug.WriteLine, PerformanceCounter
using System.Drawing;                                        // Farben, Icons, Bitmaps, Fonts
using System.IO;                                             // Datei-Operationen (File.Exists, ...)
using System.Threading.Tasks;                                // async/await, Task.Run, Task.WhenAll
using System.Windows.Forms;                                  // Windows-Forms (Form, Button, ListBox, MessageBox ...)
using System.Windows.Forms.DataVisualization.Charting;       // Diagramm-Steuerelement (Chart, Series ...)
using VerwaltungKST1127.Auftragsverwaltung;                  // Eigene Sub-Namespaces des Projekts:
using VerwaltungKST1127.EingabeSerienartikelPrototyp;
using VerwaltungKST1127.Farbauswertung;
using VerwaltungKST1127.GlasWaschDaten;
using VerwaltungKST1127.Personal;
using VerwaltungKST1127.Produktionsauswertung;
using VerwaltungKST1127.RingSpannzange;

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127
{
    // public partial class Form_Start : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien verteilt sein (z. B. Form_Start.Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_Start : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Felder ("Variablen der Klasse"). private = nur in dieser Klasse sichtbar.
        // -----------------------------------------------------------------------------------------------------------------

        // PerformanceCounter = Windows-Mechanismus zum Auslesen von System-Werten (CPU, RAM, ...).
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;

        // Connection-String zentral definieren → keine Wiederholungen in jeder Methode.
        // readonly = darf nur einmal (hier oder im Konstruktor) gesetzt werden.
        private readonly string connectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

        private const string ConnectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False";

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_Start()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_Start()
        {
            InitializeComponent();        // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)
            InitializeChart();            // CPU/RAM-Diagramm vorbereiten

            InitializeDocumentList();     // ListBox mit Dokumentnamen befüllen + Hover-Events einrichten

            // Aktuell angemeldeten Windows-Benutzer im Label anzeigen.
            lblAngemeldet.Text = "Angemeldet: " + Environment.UserName;

            // Uhrzeit/Datum sofort befüllen (damit nicht erst nach 1s der erste Tick kommt).
            UpdateZeitDatum();

            try
            {
                // Fenster-/Taskleisten-Icon setzen.
                this.Icon = new Icon("electromagnetic_spectrum_icon.ico");
            }
            catch { /* Falls das Icon fehlt: ignorieren, damit der Start nicht abbricht. */ }

            // Load-Event abonnieren ("+="" = neue Methode an das Ereignis hängen).
            // Hier kommen alle länger dauernden Aufgaben rein, damit das Fenster sofort sichtbar ist.
            this.Load += Form_Start_Load;

            // Einheitliches Design (Konzept "Hell & Klar") anwenden – siehe UiTheme.cs.
            UiTheme.Apply(this);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wird aufgerufen, sobald das Formular geladen ist.
        // "async" = darf "await" verwenden → wartet auf langsame Aufgaben, ohne die UI einzufrieren.
        // -----------------------------------------------------------------------------------------------------------------
        private async void Form_Start_Load(object sender, EventArgs e)
        {
            // Timer für Datum/Uhrzeit starten.
            TimerDatumUhrzeit.Start();

            // 1) PerformanceCounter im Hintergrund-Thread initialisieren.
            //    "Task.Run(() => ...)" lagert eine Aktion in einen anderen Thread aus.
            //    "() => { ... }" ist ein Lambda-Ausdruck (anonyme Methode).
            await Task.Run(() =>
            {
                InitializePerformanceCounters();
                InitializeMemoryCounter();
            });

            // CPU-Timer erst starten, wenn der Counter wirklich existiert.
            TimerCpu.Interval = 500; // 500 ms = 0,5 s
            TimerCpu.Start();

            // RAM-Timer ebenfalls starten.
            TimerRam.Interval = 500;
            TimerRam.Start();

            // 2) Mehrere DB-Abfragen parallel anstoßen (gleichzeitig statt nacheinander).
            //    Wir starten alle Tasks und sammeln die Task-Objekte.
            Task aufgabeHeute = UpdateOberflaechenHeuteAsync();
            Task aufgabeGestern = UpdateOberflaechenGesternAsync();
            Task aufgabeGesamt = UpdateOberflaechenGesamtAsync();
            Task aufgabeBestellung = BestellstatusAbfragenAsync();

            // Task.WhenAll wartet, bis ALLE Tasks fertig sind. UI bleibt währenddessen reaktiv.
            await Task.WhenAll(aufgabeHeute, aufgabeGestern, aufgabeGesamt, aufgabeBestellung);

            // Erst jetzt den 30-Sekunden-Refresh-Timer starten.
            TimerOberflaechenHeute.Interval = 30000; // 30.000 ms = 30 s
            TimerOberflaechenHeute.Start();
        }

        // ##################################################################################################
        // Selbst erstellte Funktionen (asynchron, damit die UI flüssig bleibt)
        // ##################################################################################################

        // -----------------------------------------------------------------------------------------------------------------
        // Prüft, ob in der Tabelle "Materiallager" Datensätze mit BestellStatus = 'Bestellen' existieren.
        // Wenn ja → Rufezeichen-Bild einblenden, sonst PictureBox ausblenden.
        // -----------------------------------------------------------------------------------------------------------------
        private async Task BestellstatusAbfragenAsync()
        {
            try
            {
                // "using" sorgt dafür, dass die Verbindung am Ende sauber geschlossen wird.
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // OpenAsync = öffnet die DB-Verbindung asynchron (UI bleibt reaktiv).
                    await con.OpenAsync();

                    string query = "SELECT COUNT(*) FROM Materiallager WHERE BestellStatus = 'Bestellen'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // ExecuteScalarAsync: liefert den ersten Wert der ersten Zeile (hier: COUNT-Ergebnis).
                        int count = (int)await cmd.ExecuteScalarAsync();

                        // Hilfsvariable: 1 = Bestellung notwendig, 0 = nichts zu tun.
                        // (count > 0) ? 1 : 0  → ternärer Operator (Kurzform für if/else).
                        int Bestellung = (count > 0) ? 1 : 0;

                        string imagePath = "";
                        if (Bestellung == 1)
                        {
                            imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\RufezeichenFuerBestellung.png";
                        }

                        // File.Exists kann auf langsamen Netzlaufwerken hängen → in Task.Run auslagern.
                        bool bildExistiert = await Task.Run(() =>
                            !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath));

                        if (bildExistiert)
                        {
                            PictureBoxBestellung.Visible = true;
                            PictureBoxBestellung.Enabled = false;
                            PictureBoxBestellung.Image = new Bitmap(imagePath);
                            PictureBoxBestellung.Refresh();
                        }
                        else
                        {
                            PictureBoxBestellung.Image = null;
                            PictureBoxBestellung.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Debug.WriteLine schreibt nur in das Visual-Studio-Ausgabefenster, nicht ins UI.
                Debug.WriteLine("Fehler beim Abfragen des Bestellstatus: " + ex.Message);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Summiert die heutigen Stückzahlen über alle Chargenprotokoll-Tabellen und schreibt sie ins Label.
        // -----------------------------------------------------------------------------------------------------------------
        private async Task UpdateOberflaechenHeuteAsync()
        {
            // Array mit allen relevanten Tabellennamen.
            string[] tables = {
                "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30",
                "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45",
                "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65"
            };
            int totalSum = 0; // Gesamtsumme über alle Tabellen

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();

                    // Pro Tabelle eine Summe abfragen und zur Gesamtsumme addieren.
                    foreach (string table in tables)
                    {
                        // ISNULL(SUM(...), 0) → wenn die Spalte komplett leer ist, kommt 0 statt NULL zurück.
                        // CAST(... AS DATE) → nur Datumsteil, ohne Uhrzeit.
                        string query = $"SELECT ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) " +
                                       $"FROM {table} WHERE CAST(Datum AS DATE) = CAST(GETDATE() AS DATE)";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            totalSum += (int)await cmd.ExecuteScalarAsync();
                        }
                    }
                }
                lblOberflaechenHeute.Text = $"{totalSum} Stk.";
            }
            catch (Exception)
            {
                lblOberflaechenHeute.Text = "Fehler"; // sichtbarer Fallback bei DB-Problemen
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wie oben, aber für GESTERN (GETDATE() - 1).
        // -----------------------------------------------------------------------------------------------------------------
        private async Task UpdateOberflaechenGesternAsync()
        {
            string[] tables = {
                "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30",
                "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45",
                "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65"
            };
            int totalSum = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    foreach (string table in tables)
                    {
                        string query = $"SELECT ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) " +
                                       $"FROM {table} WHERE CAST(Datum AS DATE) = CAST(GETDATE() - 1 AS DATE)";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            totalSum += (int)await cmd.ExecuteScalarAsync();
                        }
                    }
                }
                lblOberflaechenGestern.Text = $"{totalSum} Stk.";
            }
            catch (Exception) { lblOberflaechenGestern.Text = "Fehler"; }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Gesamtsumme ab dem 01.03.2024 (fixes Startdatum).
        // -----------------------------------------------------------------------------------------------------------------
        private async Task UpdateOberflaechenGesamtAsync()
        {
            string[] tables = {
                "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30",
                "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45",
                "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65"
            };
            int totalSum = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    foreach (string table in tables)
                    {
                        string query = $"SELECT ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) " +
                                       $"FROM {table} WHERE Datum >= '2024-03-01'";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            totalSum += (int)await cmd.ExecuteScalarAsync();
                        }
                    }
                }
                lblOberflaechenGesamt.Text = $"{totalSum} Stk.";
            }
            catch (Exception) { lblOberflaechenGesamt.Text = "Fehler"; }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // PerformanceCounter erzeugen.
        // "Processor / % Processor Time / _Total" = gesamte CPU-Auslastung.
        // -----------------------------------------------------------------------------------------------------------------
        private void InitializePerformanceCounters()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        // "Memory / % Committed Bytes In Use" = belegter Arbeitsspeicher in %.
        private void InitializeMemoryCounter()
        {
            memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Aktuelle Uhrzeit + Datum ins Label schreiben.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now;
            // ToString("dd.MM.yyyy HH:mm:ss") = formatierte Ausgabe.
            LblUhrzeitDatum.Text = aktuell.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // ##################################################################################################
        // Diagramm (Chart) vorbereiten und aktualisieren
        // ##################################################################################################

        private void InitializeChart()
        {
            // Datenserie für CPU (grün, Linie).
            // Object-Initializer-Syntax: { Eigenschaft = Wert, ... } direkt nach "new ...".
            Series seriesCpu = new Series("CPU")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green
            };
            chartPerformance.Series.Add(seriesCpu);

            // Datenserie für RAM (orange, Linie).
            Series seriesRam = new Series("RAM")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkOrange
            };
            chartPerformance.Series.Add(seriesRam);

            // X-Achse fest auf 0..100 setzen (wird später dynamisch angepasst).
            chartPerformance.ChartAreas[0].AxisX.Minimum = 0;
            chartPerformance.ChartAreas[0].AxisX.Maximum = 100;

            // Legende hübschen.
            chartPerformance.Series[0].Name = "Legende";
            chartPerformance.Legends[0].BackColor = Color.FromArgb(0, 0, 0, 0); // Alpha 0 → komplett durchsichtig

            // ChartArea-Hintergrund ebenfalls transparent.
            chartPerformance.ChartAreas[0].BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Timer-Tick für CPU: alle 500 ms aktuellen Wert lesen und ins Diagramm + Label schreiben.
        // -----------------------------------------------------------------------------------------------------------------
        private void TimerCpu_Tick(object sender, EventArgs e)
        {
            // Schutz: Wenn der Counter noch nicht initialisiert ist → Tick einfach überspringen.
            if (cpuCounter == null) return;

            try
            {
                // Random + kleiner Bias (0,05..0,2) sorgt dafür, dass die Linie auch bei 0% CPU "lebt".
                Random random = new Random();
                float cpu = (float)(random.NextDouble() * (0.2 - 0.05) + 0.05);

                float cpuUsage = cpuCounter.NextValue(); // aktueller CPU-Wert in %
                LblCpu.Text = string.Format("CPU: {0:F3}%", cpu + cpuUsage); // F3 = 3 Nachkommastellen
                UpdateChart(chartPerformance.Series["CPU"], cpu + cpuUsage);
            }
            catch { /* Ignorieren bei kurzzeitigen Auslesefehlern. */ }
        }

        // Timer-Tick für RAM: alle 500 ms aktuellen Wert lesen.
        private void TimerRam_Tick(object sender, EventArgs e)
        {
            if (memoryCounter == null) return;

            try
            {
                float memoryUsage = memoryCounter.NextValue();
                LblRam.Text = string.Format("RAM: {0:F3}%", memoryUsage);
                UpdateChart(chartPerformance.Series["RAM"], memoryUsage);
            }
            catch { /* Ignorieren */ }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Hängt einen neuen Wert ans Diagramm an, scrollt bei mehr als 50 Punkten weiter,
        // und passt die Y-Achse dynamisch an den größten Wert an.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateChart(Series series, float value)
        {
            // Punkt am Ende anhängen.
            series.Points.AddY(value);

            // Bei mehr als 50 Punkten den ältesten entfernen → "rollendes" Diagramm.
            if (series.Points.Count > 50)
            {
                series.Points.RemoveAt(0);
            }

            // X-Werte neu vergeben (0, 1, 2, ...), damit die Punkte gleichmäßig verteilt sind.
            for (int i = 0; i < series.Points.Count; i++)
            {
                series.Points[i].XValue = i;
            }

            // Größten Y-Wert beider Serien (CPU + RAM) suchen.
            double maxYValue = 0;
            foreach (var point in chartPerformance.Series["CPU"].Points)
            {
                if (point.YValues[0] > maxYValue) maxYValue = point.YValues[0];
            }
            foreach (var point in chartPerformance.Series["RAM"].Points)
            {
                if (point.YValues[0] > maxYValue) maxYValue = point.YValues[0];
            }

            // Etwas Puffer drauf, damit oben Luft bleibt.
            double buffer = 5;
            chartPerformance.ChartAreas[0].AxisY.Maximum = maxYValue + buffer;

            // X-Achse fix auf 0..50.
            chartPerformance.ChartAreas[0].AxisX.Minimum = 0;
            chartPerformance.ChartAreas[0].AxisX.Maximum = 50;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Timer-Tick: Uhrzeit aktualisieren + Bestellstatus prüfen (asynchron).
        // -----------------------------------------------------------------------------------------------------------------
        private async void TimerDatumUhrzeit_Tick(object sender, EventArgs e)
        {
            UpdateZeitDatum();
            await BestellstatusAbfragenAsync();
        }

        // Timer-Tick: alle 30 s heutige Oberflächen neu zählen.
        private async void TimerOberflaechenHeute_Tick(object sender, EventArgs e)
        {
            await UpdateOberflaechenHeuteAsync();
        }

        // ##################################################################################################
        // Buttons: öffnen jeweils ein anderes Fenster mit ".Show()" (nicht modal → Hauptfenster bleibt aktiv).
        // ##################################################################################################

        private void BtnSerienartikelPrototyp_Click(object sender, EventArgs e)
        {
            Form_EingabeSeriePrototyp form_EingabeSeriePrototyp = new Form_EingabeSeriePrototyp();
            form_EingabeSeriePrototyp.Show();
        }

        private void BtnPrototypenAuftragErstellen_Click(object sender, EventArgs e)
        {
            Form_PrototypenauftragErstellen form_PrototypenauftragErstellen = new Form_PrototypenauftragErstellen();
            form_PrototypenauftragErstellen.Show();
        }

        private void BtnFarbwerte_Click(object sender, EventArgs e)
        {
            Form_Farbauswertung form_Farbauswertung = new Form_Farbauswertung();
            form_Farbauswertung.Show();
        }

        private void BtnMateriallager_Click(object sender, EventArgs e)
        {
            Form_Materiallager form_Materiallager = new Form_Materiallager();
            form_Materiallager.Show();
        }

        // Öffnet eine URL im Standardbrowser des Systems.
        private void BtnHomepage_Click(object sender, EventArgs e)
        {
            string url = "https://www.swarovskioptik.com/at/de/jagd";
            Process.Start(url);
        }

        private void BtnLupe_Click(object sender, EventArgs e)
        {
            string url = "https://swarovskioptik.sharepoint.com/";
            Process.Start(url);
        }

        private void BtnInformation_Click(object sender, EventArgs e)
        {
            Form_Copyright form_Copyright = new Form_Copyright();
            form_Copyright.Show();
        }

        private void BtnPersonalliste_Click(object sender, EventArgs e)
        {
            Form_Personalliste form_Personalliste = new Form_Personalliste();
            form_Personalliste.Show();
        }

        private void BtnArtikelAendern_Click(object sender, EventArgs e)
        {
            // Konstruktor mit Parametern (Artikel + Seite).
            Form_ArtikelPrototypAendern form_ArtikelAender = new Form_ArtikelPrototypAendern("12-2044", "1");
            form_ArtikelAender.Show();
            form_ArtikelAender.BringToFront(); // direkt nach vorn holen
        }

        // ##################################################################################################
        // Dokumenten-Liste (ListBox) mit Hover-Effekt
        // ##################################################################################################

        // Zweidimensionales Array: pro Zeile [Anzeigename, Dateipfad].
        private readonly string[,] documents = {
            {"Grundeinstellungen",   @"P:\TEDuTOZ\Beschichtungsanlagen\Grundeinstellungen_aktuell.xlsx"},
            {"Anlagen - Fehlerliste",@"P:\TEDuTOZ\Beschichtungsanlagen\Fehlersuche an Vergütungsanlagen.xlsx"},
            {"Abriebtest",           @"P:\TEDuTOZ\QS\Abriebtest Taber Abraser.xlsx"},
            {"Verluste",             @"P:\TEDuTOZ\Beläge\Verluste\Verluste.xlsx"},
            {"Vorlage Verlust",      @"P:\TEDuTOZ\Uvwinlab\MAYA\Vorlage_____Verlust in Totalreflexion am MAYA-hop.xlsx"},
            {"Kratztest",            @"P:\TEDuTOZ\QS\Kratz und Kochtest B117.xlsx"},
            {"Messarten",            @"P:\TEDuTOZ\Beläge\Vorlage Messen Lambda 1050\Messarten für Lambda 1050 und L650.xlsx"},
            {"Messzellen",           @"P:\TEDuTOZ\Formulare\Arbeitsanweisungen\Messzellen Übersicht.Xlsx"},
            {"Thermofühler",         @"P:\TEDuTOZ\Beschichtungsanlagen\Anlagendaten Betriebstemperaturen Schaltschrank Schauglas-Dm usw..xlsx"},
            {"Ausschussliste",       @"P:\TEDuTOZ\Verschiedenes\Fehlerliste.xlsx"},
            {" ",                    @"C:\"}, // Platzhalter / Trennlinie (Leerstring als Anzeige)
            {"Ordner - Beläge",      @"P:\TEDuTOZ\Beläge"},
            {"Ordner - Anlagen",     @"P:\TEDuTOZ\Beschichtungsanlagen"},
            {"Ordner - BBM",         @"P:\TEDuTOZ\BBM"},
            {"Ordner - Messdaten",   @"P:\Messdata\UV"},
            // Weitere Einträge nach demselben Muster: {"Anzeigename", @"Pfad"}
        };

        // -----------------------------------------------------------------------------------------------------------------
        // Initialisiert die ListBox: Owner-Draw-Modus (wir zeichnen die Items selbst) + Hover-Events.
        // -----------------------------------------------------------------------------------------------------------------
        private void InitializeDocumentList()
        {
            // OwnerDrawFixed → wir bestimmen das Aussehen der Items selbst (DrawItem-Event).
            ListBoxDocuments.DrawMode = DrawMode.OwnerDrawFixed;

            // Eigene Methoden an die Events hängen.
            ListBoxDocuments.DrawItem += ListBoxDocuments_DrawItem;
            ListBoxDocuments.MouseMove += ListBoxDocuments_MouseMove;
            ListBoxDocuments.MouseLeave += ListBoxDocuments_MouseLeave;

            // Anzeigetexte (Spalte 0) in die ListBox eintragen.
            for (int i = 0; i < documents.GetLength(0); i++)
            {
                ListBoxDocuments.Items.Add(documents[i, 0]);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Öffnet das gerade ausgewählte Dokument mit dem zugehörigen Standardprogramm.
        // -----------------------------------------------------------------------------------------------------------------
        private void OpenSelectedDocument()
        {
            int selectedIndex = ListBoxDocuments.SelectedIndex;
            if (selectedIndex >= 0)
            {
                string documentPath = documents[selectedIndex, 1];

                try
                {
                    Process.Start(documentPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Öffnen des Dokuments: " + ex.Message);
                }
            }
        }

        // Welches Item liegt gerade unter der Maus? -1 = keines.
        private int hoveredIndex = -1;

        // -----------------------------------------------------------------------------------------------------------------
        // Wird für jedes Item aufgerufen, das gezeichnet werden soll. Hier passen wir das Aussehen an.
        // -----------------------------------------------------------------------------------------------------------------
        private void ListBoxDocuments_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground(); // Standard-Hintergrund zeichnen
            if (e.Index >= 0)
            {
                string text = ListBoxDocuments.Items[e.Index].ToString();
                Font font = e.Font;

                // Wenn die Maus über diesem Item steht → Schrift fett + kursiv + unterstrichen.
                // "|" verbindet mehrere Flags zu einer Kombination.
                if (e.Index == hoveredIndex)
                {
                    font = new Font(e.Font, FontStyle.Underline | FontStyle.Italic | FontStyle.Bold);
                }

                TextRenderer.DrawText(e.Graphics, text, font, e.Bounds, e.ForeColor, TextFormatFlags.Left);
            }
            e.DrawFocusRectangle(); // gestricheltes Fokusrechteck (falls Fokus auf der Liste)
        }

        // Maus bewegt sich über die ListBox → prüfen, ob ein anderes Item "gehovert" wird.
        private void ListBoxDocuments_MouseMove(object sender, MouseEventArgs e)
        {
            int newHoveredIndex = ListBoxDocuments.IndexFromPoint(e.Location);

            if (newHoveredIndex != hoveredIndex)
            {
                hoveredIndex = newHoveredIndex;
                // Invalidate erzwingt ein Neuzeichnen → DrawItem läuft erneut.
                ListBoxDocuments.Invalidate();
            }
        }

        // Maus verlässt die ListBox → kein Hover mehr.
        private void ListBoxDocuments_MouseLeave(object sender, EventArgs e)
        {
            hoveredIndex = -1;
            ListBoxDocuments.Invalidate();
        }

        // Doppelklick auf ein Item → Dokument öffnen.
        private void ListBoxDocuments_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedDocument();
        }

        // ##################################################################################################
        // Weitere Buttons (öffnen jeweils ein eigenes Fenster)
        // ##################################################################################################

        private void BtnEingabeRingSpannzange_Click(object sender, EventArgs e)
        {
            DgvZuordnungArtikel form_UebersichtRingSpannzangen = new DgvZuordnungArtikel();
            form_UebersichtRingSpannzangen.Show();
        }

        private void BtngesamtOberflaechen_Click(object sender, EventArgs e)
        {
            Form_AnsichtOberflaechen form_AnsichtOberflaechen = new Form_AnsichtOberflaechen();
            form_AnsichtOberflaechen.Show();
        }

        private void BtnGlasWaschDaten_Click(object sender, EventArgs e)
        {
            Form_GlasWaschDaten form_glasWaschenDaten = new Form_GlasWaschDaten();
            form_glasWaschenDaten.Show();
        }

        private void BtnVerwaltung_Click(object sender, EventArgs e)
        {
            Form_VerwaltungHauptansicht form_VerwaltungHauptansicht = new Form_VerwaltungHauptansicht();
            form_VerwaltungHauptansicht.Show();
        }

        private void btnProduktionsauswertung_Click(object sender, EventArgs e)
        {
            Form_AnsichtProduktionsauswertung form_AnsichtProduktionsauswertung = new Form_AnsichtProduktionsauswertung();
            form_AnsichtProduktionsauswertung.Show();
        }

        // Öffnet das "Produktion gestern"-Dashboard (WebView2 + ECharts).
        // Vorbelegt mit dem gestrigen Datum; Datumswechsel im Dashboard lädt neu.
        private void btnProduktionGestern_Click(object sender, EventArgs e)
        {
            var form = new Form_ProduktionsuebersichtGestern();
            form.Show();
        }

        private void BtnRFIDAnischtWaschen_Click(object sender, EventArgs e)
        {
            Form_RFIDAnsichtWaschanlagen form_RFIDWaschen = new Form_RFIDAnsichtWaschanlagen();
            form_RFIDWaschen.Show();
        }

        private void btnDatenExportTabellen_Click(object sender, EventArgs e)
        {
            Form_DatenExportExcel form_DatenExportExcel = new Form_DatenExportExcel();
            form_DatenExportExcel.Show();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Prämienbewertung" → erst Passwort abfragen, dann Excel-Datei öffnen.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnPraemienbewertung_Click(object sender, EventArgs e)
        {
            // Eigenen Passwort-Dialog anzeigen (siehe ZeigePasswortDialog unten).
            string passwort = ZeigePasswortDialog();

            if (passwort == "verguetung")
            {
                string dateipfad = @"K:\Kst_127\Personal\Lohntabelle\PrämieSageAuflistung.xlsx";

                try
                {
                    // ProcessStartInfo mit UseShellExecute = true → öffnet die Datei mit dem zugeordneten Programm (z. B. Excel).
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = dateipfad,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Öffnen der Datei: {ex.Message}");
                }
            }
            else if (passwort != null)
            {
                // passwort == null → Benutzer hat den Dialog mit "Abbrechen" geschlossen.
                MessageBox.Show("Falsches Passwort.");
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Selbst gebauter Mini-Dialog zur Passwort-Eingabe.
        // Liefert das eingegebene Passwort (oder null bei Abbruch).
        // -----------------------------------------------------------------------------------------------------------------
        private string ZeigePasswortDialog()
        {
            // "using (Form prompt ...)" → Form wird am Ende automatisch entsorgt.
            using (Form prompt = new Form())
            {
                // Fenster konfigurieren.
                prompt.Width = 250;
                prompt.Height = 150;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Passwort eingeben";
                prompt.StartPosition = FormStartPosition.CenterParent;

                // Steuerelemente programmatisch erzeugen + positionieren.
                Label textLabel = new Label() { Left = 10, Top = 10, Text = "Passwort:" };

                // PasswordChar = '*' → Eingabe wird mit Sternen maskiert.
                TextBox textBox = new TextBox() { Left = 10, Top = 30, Width = 200, PasswordChar = '*' };

                // Button gibt beim Klick automatisch DialogResult.OK zurück.
                Button confirmation = new Button()
                {
                    Text = "OK",
                    Left = 10,
                    Width = 100,
                    Top = 60,
                    DialogResult = DialogResult.OK
                };

                // Steuerelemente in das Formular einfügen.
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);

                // Enter-Taste löst den OK-Button aus.
                prompt.AcceptButton = confirmation;

                // ShowDialog blockiert, bis der Dialog geschlossen wird.
                // Bei OK → eingegebenes Passwort zurückgeben, sonst null (Abbruch / Schließen).
                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }

        
    }
}
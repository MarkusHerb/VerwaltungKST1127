using System; // Importieren des System-Namespace für grundlegende Funktionalitäten
using System.Data.SqlClient; // Importieren für SQL-Datenbankverbindungen
using System.Diagnostics; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen
using System.Drawing; // Importieren für grafische Elemente wie Farben, Fonts und Icons
using System.IO; // Importieren für Datei- und Ordneroperationen (z. B. File.Exists)
using System.Threading.Tasks; // NEU: Wird für asynchrone Programmierung (async/await) und Multithreading benötigt
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für GUI-Funktionalität
using System.Windows.Forms.DataVisualization.Charting; // Um Diagramm-Funktionen (Charting) zu verwenden
using VerwaltungKST1127.Auftragsverwaltung;
using VerwaltungKST1127.EingabeSerienartikelPrototyp;
using VerwaltungKST1127.Farbauswertung;
using VerwaltungKST1127.GlasWaschDaten;
using VerwaltungKST1127.Personal;
using VerwaltungKST1127.Produktionsauswertung;
using VerwaltungKST1127.RingSpannzange;

namespace VerwaltungKST1127
{
    public partial class Form_Start : Form
    {
        // Feld für den PerformanceCounter (CPU)
        private PerformanceCounter cpuCounter;
        // Feld für den PerformanceCounter (Arbeitsspeicher)
        private PerformanceCounter memoryCounter;

        // Verbindungszeichenfolge für die SQL Server-Datenbank (Global definiert, um Wiederholungen zu vermeiden)
        private readonly string connectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

        public Form_Start()
        {
            InitializeComponent();
            InitializeChart();

            // ListBoxDocuments laden
            InitializeDocumentList();

            // Anzeigen, welcher Benutzer angemeldet ist (Rechtschreibfehler korrigiert)
            lblAngemeldet.Text = "Angemeldet: " + Environment.UserName;

            // Timer für Uhrzeit/Datum starten (aktualisiert die UI sofort)
            UpdateZeitDatum();

            try
            {
                // Icon für die Taskleiste und das Fenster
                this.Icon = new Icon("electromagnetic_spectrum_icon.ico");
            }
            catch { /* Fehler abfangen, falls das Icon fehlt, damit das Programm beim Start nicht abstürzt */ }

            // NEU: Lade-Event abonnieren. Hier packen wir alle zeitaufwändigen Aufgaben rein,
            // damit sich das Fenster sofort öffnet und nicht blockiert wird.
            this.Load += Form_Start_Load;
        }

        // Event-Handler: Wird ausgeführt, sobald das Formular geladen wird (Asynchron für schnelleren Start)
        private async void Form_Start_Load(object sender, EventArgs e)
        {
            // Timer für Uhrzeit/Datum regulär starten
            TimerDatumUhrzeit.Start();

            // 1. Initialisieren der Leistungsindikatoren - CPU & RAM
            // Wird in einen Hintergrund-Thread ausgelagert, da PerformanceCounter manchmal kurz das System blockieren
            await Task.Run(() =>
            {
                InitializePerformanceCounters();
                InitializeMemoryCounter();
            });

            // Starten des Timers für die CPU-Auslastung erst nach der Initialisierung
            TimerCpu.Interval = 500; // Aktualisierung alle 0,5 Sekunden
            TimerCpu.Start();

            // Starten des Timers für die RAM-Auslastung erst nach der Initialisierung
            TimerRam.Interval = 500; // Aktualisierung alle 0,5 Sekunden
            TimerRam.Start();

            // 2. Datenbankabfragen parallel (gleichzeitig) starten, das spart massiv Zeit!
            Task aufgabeHeute = UpdateOberflaechenHeuteAsync();
            Task aufgabeGestern = UpdateOberflaechenGesternAsync();
            Task aufgabeGesamt = UpdateOberflaechenGesamtAsync();
            Task aufgabeBestellung = BestellstatusAbfragenAsync();

            // Warten bis alle asynchronen Datenbankabfragen fertig sind (ohne die GUI einzufrieren)
            await Task.WhenAll(aufgabeHeute, aufgabeGestern, aufgabeGesamt, aufgabeBestellung);

            // Starten des Timers für die Oberflächen heute (erst wenn der erste Ladevorgang abgeschlossen ist)
            TimerOberflaechenHeute.Interval = 30000; // Aktualisierung alle 30 Sekunden
            TimerOberflaechenHeute.Start();
        }

        // ############## Selbst erstellte Funktionen (Optimiert für asynchrone Ausführung) ##############

        // Funktion zum Abfragen des Bestellstatus
        private async Task BestellstatusAbfragenAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Datenbank asynchron öffnen, um die GUI nicht zu blockieren
                    await con.OpenAsync();

                    // SQL-Abfrage, um nach "Bestellen" in der Spalte BestellStatus zu suchen
                    string query = "SELECT COUNT(*) FROM Materiallager WHERE BestellStatus = 'Bestellen'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Ausführen der Abfrage und Ergebnisse asynchron abrufen
                        int count = (int)await cmd.ExecuteScalarAsync();

                        // Variable Bestellung setzen
                        int Bestellung = (count > 0) ? 1 : 0;

                        // ImagePath initialisieren
                        string imagePath = "";

                        // Rufezeichen anzeigen oder nicht
                        if (Bestellung == 1)
                        {
                            imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\RufezeichenFuerBestellung.png";
                        }

                        // Überprüfen, ob ein gültiger Bildpfad gefunden wurde und das Bild existiert.
                        // Das Auslagern in Task.Run verhindert Hänger, falls das Netzlaufwerk (P:) langsam reagiert.
                        bool bildExistiert = await Task.Run(() => !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath));

                        if (bildExistiert)
                        {
                            // Das Bild der PictureBox zuweisen
                            PictureBoxBestellung.Visible = true;
                            PictureBoxBestellung.Enabled = false;
                            PictureBoxBestellung.Image = new Bitmap(imagePath);
                            PictureBoxBestellung.Refresh();
                        }
                        else
                        {
                            // Anderenfalls die PictureBox leeren oder ausblenden
                            PictureBoxBestellung.Image = null;
                            PictureBoxBestellung.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Datenbankfehler beim Start im Ausgabefenster loggen, anstatt das Programm abstürzen zu lassen
                Debug.WriteLine("Fehler beim Abfragen des Bestellstatus: " + ex.Message);
            }
        }

        // Funktion zum Berechnen und Anzeigen der Oberflächen von heute
        private async Task UpdateOberflaechenHeuteAsync()
        {
            string[] tables = { "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30", "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65" };
            int totalSum = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    foreach (string table in tables)
                    {
                        string query = $"SELECT ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) FROM {table} WHERE CAST(Datum AS DATE) = CAST(GETDATE() AS DATE)";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // Asynchroner DB Zugriff -> Zählt die Summe aus allen Tabellen zusammen, ohne die UI einzufrieren
                            totalSum += (int)await cmd.ExecuteScalarAsync();
                        }
                    }
                }
                lblOberflaechenHeute.Text = $"{totalSum} Stk.";
            }
            catch (Exception) { lblOberflaechenHeute.Text = "Fehler"; } // Fallback bei DB-Fehler
        }

        // Funktion zum Berechnen und Anzeigen der Oberflächen von gestern
        private async Task UpdateOberflaechenGesternAsync()
        {
            string[] tables = { "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30", "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65" };
            int totalSum = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    foreach (string table in tables)
                    {
                        string query = $"SELECT ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) FROM {table} WHERE CAST(Datum AS DATE) = CAST(GETDATE() - 1 AS DATE)";
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

        // Funktion zum Berechnen der gesamten Oberflächen ab 03.2024
        private async Task UpdateOberflaechenGesamtAsync()
        {
            string[] tables = { "Chargenprotokoll20", "Chargenprotokoll25", "Chargenprotokoll30", "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65" };
            int totalSum = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    await con.OpenAsync();
                    foreach (string table in tables)
                    {
                        string query = $"SELECT ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) FROM {table} WHERE Datum >= '2024-03-01'";
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

        // Funktion für die Instanziierung des PerformanceCounters für die CPU
        private void InitializePerformanceCounters()
        {
            // Instanziieren des PerformanceCounter-Objekts für die CPU-Auslastung
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        // Initialisierung des PerformanceCounters für den Arbeitsspeicher
        private void InitializeMemoryCounter()
        {
            // Instanziieren des PerformanceCounter-Objekts für die Arbeitsspeicher-Auslastung
            memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }

        // Uhrzeit- und Datumsfunktion
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now; // Aktuelles Datum und Uhrzeit abrufen
            LblUhrzeitDatum.Text = aktuell.ToString("dd.MM.yyyy HH:mm:ss"); // Datum und Uhrzeit im zugewiesenen Label anzeigen
        }

        // ############## Event-Handler für die unterschiedlichen Items aus der Toolbox ##############

        private void InitializeChart()
        {
            // Initialisiere die Datenserien für CPU und RAM
            Series seriesCpu = new Series("CPU")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green
            };
            chartPerformance.Series.Add(seriesCpu);

            Series seriesRam = new Series("RAM")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkOrange
            };
            chartPerformance.Series.Add(seriesRam);

            // Setze die X-Achse auf einen festen Bereich von 0 bis 100
            chartPerformance.ChartAreas[0].AxisX.Minimum = 0;
            chartPerformance.ChartAreas[0].AxisX.Maximum = 100;

            // Legendenbeschriftung ändern  
            chartPerformance.Series[0].Name = "Legende";
            // Hintergrund der Legende verändern
            chartPerformance.Legends[0].BackColor = Color.FromArgb(0, 0, 0, 0); // Transparent

            // Setze die Hintergrundfarbe des ChartArea
            chartPerformance.ChartAreas[0].BackColor = Color.FromArgb(0, 0, 0, 0); // Transparent
        }

        // Timer-Event für die CPU-Auslastung
        private void TimerCpu_Tick(object sender, EventArgs e)
        {
            // Verhindern von Abstürzen, falls der Timer tickt bevor der Counter fertig geladen ist
            if (cpuCounter == null) return;

            try
            {
                Random random = new Random();
                float cpu = (float)(random.NextDouble() * (0.2 - 0.05) + 0.05);

                float cpuUsage = cpuCounter.NextValue(); // CPU-Auslastung abrufen
                LblCpu.Text = string.Format("CPU: {0:F3}%", cpu + cpuUsage); // Auslastung im Label anzeigen
                UpdateChart(chartPerformance.Series["CPU"], cpu + cpuUsage); // Diagramm aktualisieren
            }
            catch { /* Ignorieren bei temporären Auslesefehlern im Counter */ }
        }

        // Timer-Event für die RAM-Auslastung
        private void TimerRam_Tick(object sender, EventArgs e)
        {
            if (memoryCounter == null) return;

            try
            {
                float memoryUsage = memoryCounter.NextValue(); // Arbeitsspeicher-Auslastung abrufen
                LblRam.Text = string.Format("RAM: {0:F3}%", memoryUsage); // Auslastung im Label anzeigen
                UpdateChart(chartPerformance.Series["RAM"], memoryUsage); // Diagramm aktualisieren
            }
            catch { /* Ignorieren bei temporären Auslesefehlern im Counter */ }
        }

        // Funktion zum Aktualisieren des Diagramms
        private void UpdateChart(Series series, float value)
        {
            // Füge den neuen Wert hinzu
            series.Points.AddY(value);

            // Entferne den ersten Wert, wenn die Anzahl der Punkte 50 (früher 20) überschreitet
            if (series.Points.Count > 50)
            {
                series.Points.RemoveAt(0);
            }

            // Aktualisiere die X-Werte
            for (int i = 0; i < series.Points.Count; i++)
            {
                series.Points[i].XValue = i;
            }

            // Berechne den maximalen Y-Wert aus beiden Serien und füge einen Puffer hinzu
            double maxYValue = 0;
            foreach (var point in chartPerformance.Series["CPU"].Points)
            {
                if (point.YValues[0] > maxYValue) maxYValue = point.YValues[0];
            }
            foreach (var point in chartPerformance.Series["RAM"].Points)
            {
                if (point.YValues[0] > maxYValue) maxYValue = point.YValues[0];
            }

            double buffer = 5; // Fester Pufferwert
            chartPerformance.ChartAreas[0].AxisY.Maximum = maxYValue + buffer;

            // Aktualisiere die X-Achse, um den Bereich von 0 bis 50 anzuzeigen
            chartPerformance.ChartAreas[0].AxisX.Minimum = 0;
            chartPerformance.ChartAreas[0].AxisX.Maximum = 50;
        }

        // Timer-Event für Datum/Uhrzeit und Bestellstatus
        // async void ermöglicht Hintergrundabfragen, ohne dass die Benutzeroberfläche beim Ticken ruckelt
        private async void TimerDatumUhrzeit_Tick(object sender, EventArgs e)
        {
            UpdateZeitDatum();
            // Bestellstatus wird ständig aktualisiert
            await BestellstatusAbfragenAsync();
        }

        // Timer-Event für die Oberflächen heute
        private async void TimerOberflaechenHeute_Tick(object sender, EventArgs e)
        {
            await UpdateOberflaechenHeuteAsync();
        }

        // ##### Buttons (Klick-Events zum Öffnen der verschiedenen Formulare) #####

        // Button-Event: Formular EingabeSeriePrototyp öffnen
        private void BtnSerienartikelPrototyp_Click(object sender, EventArgs e)
        {
            Form_EingabeSeriePrototyp form_EingabeSeriePrototyp = new Form_EingabeSeriePrototyp();
            form_EingabeSeriePrototyp.Show();
        }

        // Button-Event: Formular PrototypenauftragErstellen öffnen
        private void BtnPrototypenAuftragErstellen_Click(object sender, EventArgs e)
        {
            Form_PrototypenauftragErstellen form_PrototypenauftragErstellen = new Form_PrototypenauftragErstellen();
            form_PrototypenauftragErstellen.Show();
        }

        // Button-Event: Formular Farbauswertung öffnen
        private void BtnFarbwerte_Click(object sender, EventArgs e)
        {
            Form_Farbauswertung form_Farbauswertung = new Form_Farbauswertung();
            form_Farbauswertung.Show();
        }

        // Button-Event: Formular Materiallager öffnen
        private void BtnMateriallager_Click(object sender, EventArgs e)
        {
            Form_Materiallager form_Materiallager = new Form_Materiallager();
            form_Materiallager.Show();
        }

        // Button-Event: Homepage im Standardbrowser öffnen
        private void BtnHomepage_Click(object sender, EventArgs e)
        {
            string url = "https://www.swarovskioptik.com/at/de/jagd";
            Process.Start(url);
        }

        // Button-Event: Sharepoint (Lupe) im Standardbrowser öffnen
        private void BtnLupe_Click(object sender, EventArgs e)
        {
            string url = "https://swarovskioptik.sharepoint.com/";
            Process.Start(url);
        }

        // Button-Event: Formular Copyright/Informationen öffnen
        private void BtnInformation_Click(object sender, EventArgs e)
        {
            Form_Copyright form_Copyright = new Form_Copyright();
            form_Copyright.Show();
        }

        // Button-Event: Formular Personalliste öffnen
        private void BtnPersonalliste_Click(object sender, EventArgs e)
        {
            Form_Personalliste form_Personalliste = new Form_Personalliste();
            form_Personalliste.Show();
        }

        // Button-Event: Formular ArtikelPrototyp ändern öffnen
        private void BtnArtikelAendern_Click(object sender, EventArgs e)
        {
            Form_ArtikelPrototypAendern form_ArtikelAender = new Form_ArtikelPrototypAendern("12-2044", "1");
            form_ArtikelAender.Show();
            form_ArtikelAender.BringToFront(); // Bringt das neue Formular direkt in den Vordergrund
        }

        // #### ListBox Dokumentenverwaltung ####

        // Array zur Speicherung der Dokumentnamen und Dateipfade
        private readonly string[,] documents = {
            {"Grundeinstellungen", @"P:\TEDuTOZ\Beschichtungsanlagen\Grundeinstellungen_aktuell.xlsx"},
            {"Anlagen - Fehlerliste", @"P:\TEDuTOZ\Beschichtungsanlagen\Fehlersuche an Vergütungsanlagen.xlsx"},
            {"Abriebtest", @"P:\TEDuTOZ\QS\Abriebtest Taber Abraser.xlsx"},
            {"Verluste", @"P:\TEDuTOZ\Beläge\Verluste\Verluste.xlsx"},
            {"Vorlage Verlust", @"P:\TEDuTOZ\Uvwinlab\MAYA\Vorlage_____Verlust in Totalreflexion am MAYA-hop.xlsx"},
            {"Kratztest", @"P:\TEDuTOZ\QS\Kratz und Kochtest B117.xlsx"},
            {"Messarten", @"P:\TEDuTOZ\Beläge\Vorlage Messen Lambda 1050\Messarten für Lambda 1050 und L650.xlsx"},
            {"Messzellen", @"P:\TEDuTOZ\Formulare\Arbeitsanweisungen\Messzellen Übersicht.Xlsx"},
            {"Thermofühler", @"P:\TEDuTOZ\Beschichtungsanlagen\Anlagendaten Betriebstemperaturen Schaltschrank Schauglas-Dm usw..xlsx"},
            {"Ausschussliste", @"P:\TEDuTOZ\Verschiedenes\Fehlerliste.xlsx"},
            {" ", @"C:\"}, // Platzhalter/Trennlinie
            {"Ordner - Beläge", @"P:\TEDuTOZ\Beläge"},
            {"Ordner - Anlagen", @"P:\TEDuTOZ\Beschichtungsanlagen"},
            {"Ordner - BBM", @"P:\TEDuTOZ\BBM"},
            {"Ordner - Messdaten", @"P:\Messdata\UV"},
            // Weitere Dokumente hinzufügen {"", @""},
        };

        // ListBox mit Dokumentnamen füllen und Events registrieren
        private void InitializeDocumentList()
        {
            ListBoxDocuments.DrawMode = DrawMode.OwnerDrawFixed;
            ListBoxDocuments.DrawItem += ListBoxDocuments_DrawItem;
            ListBoxDocuments.MouseMove += ListBoxDocuments_MouseMove;
            ListBoxDocuments.MouseLeave += ListBoxDocuments_MouseLeave;

            for (int i = 0; i < documents.GetLength(0); i++)
            {
                ListBoxDocuments.Items.Add(documents[i, 0]);
            }
        }

        // Event-Handler: Logik zum Öffnen, wenn ein Dokument ausgewählt wird
        private void OpenSelectedDocument()
        {
            // Index des ausgewählten Elements in der ListBox erhalten
            int selectedIndex = ListBoxDocuments.SelectedIndex;

            // Sicherstellen, dass ein Element ausgewählt wurde
            if (selectedIndex >= 0)
            {
                // Pfad des ausgewählten Dokuments abrufen
                string documentPath = documents[selectedIndex, 1];

                // Dokument öffnen
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

        // Variable speichert den Index des Elements, über dem die Maus aktuell schwebt
        private int hoveredIndex = -1;

        // DrawItem-Event: Format (Schriftart) wird dynamisch angepasst, wenn das Item ausgewählt oder gehovert wird
        private void ListBoxDocuments_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                string text = ListBoxDocuments.Items[e.Index].ToString();
                Font font = e.Font;

                // Wenn das Element ausgewählt ist oder die Maus über dem Element schwebt, Schriftart anpassen (fett, kursiv, unterstrichen)
                if (e.Index == hoveredIndex)
                {
                    font = new Font(e.Font, FontStyle.Underline | FontStyle.Italic | FontStyle.Bold);
                }

                TextRenderer.DrawText(e.Graphics, text, font, e.Bounds, e.ForeColor, TextFormatFlags.Left);
            }
            e.DrawFocusRectangle();
        }

        // Event-Handler: Wird ausgelöst, wenn man mit der Maus über ein ListBox-Item fährt
        private void ListBoxDocuments_MouseMove(object sender, MouseEventArgs e)
        {
            int newHoveredIndex = ListBoxDocuments.IndexFromPoint(e.Location);

            if (newHoveredIndex != hoveredIndex)
            {
                hoveredIndex = newHoveredIndex;
                ListBoxDocuments.Invalidate(); // Neu zeichnen, um den Hover-Effekt sichtbar zu machen
            }
        }

        // Event-Handler: Wird ausgelöst, wenn die Maus das ListBox-Element verlässt
        private void ListBoxDocuments_MouseLeave(object sender, EventArgs e)
        {
            hoveredIndex = -1;
            ListBoxDocuments.Invalidate(); // Neu zeichnen, um das Format zurückzusetzen
        }

        // Event-Handler: Wenn ein Element in der ListBox doppelt geklickt wird
        private void ListBoxDocuments_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedDocument();
        }

        // Button-Event: Formular Übersicht Ring-/Spannzangen öffnen
        private void BtnEingabeRingSpannzange_Click(object sender, EventArgs e)
        {
            DgvZuordnungArtikel form_UebersichtRingSpannzangen = new DgvZuordnungArtikel();
            form_UebersichtRingSpannzangen.Show();
        }

        // Button-Event: Formular Ansicht Gesamtoberflächen öffnen
        private void BtngesamtOberflaechen_Click(object sender, EventArgs e)
        {
            Form_AnsichtOberflaechen form_AnsichtOberflaechen = new Form_AnsichtOberflaechen();
            form_AnsichtOberflaechen.Show();
        }

        // Button-Event: Formular GlasWaschDaten öffnen
        private void BtnGlasWaschDaten_Click(object sender, EventArgs e)
        {
            Form_GlasWaschDaten form_glasWaschenDaten = new Form_GlasWaschDaten();
            form_glasWaschenDaten.Show();
        }

        // Button-Event: Formular Verwaltungshauptansicht öffnen
        private void BtnVerwaltung_Click(object sender, EventArgs e)
        {
            Form_VerwaltungHauptansicht form_VerwaltungHauptansicht = new Form_VerwaltungHauptansicht();
            form_VerwaltungHauptansicht.Show();
        }

        // Button-Event: Wird aufgerufen, wenn der Button "Produktionsauswertung" geklickt wird
        private void btnProduktionsauswertung_Click(object sender, EventArgs e)
        {
            Form_AnsichtProduktionsauswertung form_AnsichtProduktionsauswertung = new Form_AnsichtProduktionsauswertung();
            form_AnsichtProduktionsauswertung.Show();
        }

        // Button-Event: Wird aufgerufen, wenn der Button "RFID Ansicht Waschen" geklickt wird
        private void BtnRFIDAnischtWaschen_Click(object sender, EventArgs e)
        {
            Form_RFIDAnsichtWaschanlagen form_RFIDWaschen = new Form_RFIDAnsichtWaschanlagen();
            form_RFIDWaschen.Show();
        }

        // Button-Event: Wird aufgerufen, wenn der Button "Prämienbewertung" geklickt wird
        private void btnPraemienbewertung_Click(object sender, EventArgs e)
        {
            // Öffne ein einfaches Passwort-Eingabefenster (selbst gebaut)
            string passwort = ZeigePasswortDialog();

            // Prüfe das eingegebene Passwort
            if (passwort == "verguetung")
            {
                // Korrektes Passwort: Excel-Datei öffnen
                string dateipfad = @"K:\Kst_127\Personal\Lohntabelle\PrämieSageAuflistung.xlsx";

                try
                {
                    // Öffne die Datei mit dem Standardprogramm (normalerweise Excel)
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = dateipfad,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    // Fehler beim Öffnen behandeln (z.B. wenn die Datei verschoben wurde)
                    MessageBox.Show($"Fehler beim Öffnen der Datei: {ex.Message}");
                }
            }
            else if (passwort != null)
            {
                // Falsches Passwort eingegeben (Benutzer hat nicht auf 'Abbrechen' geklickt)
                MessageBox.Show("Falsches Passwort.");
            }
        }

        // Methode: Zeigt einen kleinen Dialog zur Passwort-Eingabe an
        private string ZeigePasswortDialog()
        {
            using (Form prompt = new Form())
            {
                // Grundlegende Eigenschaften des Fensters festlegen
                prompt.Width = 250;
                prompt.Height = 150;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Passwort eingeben";
                prompt.StartPosition = FormStartPosition.CenterParent;

                // Label für den Hinweistext
                Label textLabel = new Label() { Left = 10, Top = 10, Text = "Passwort:" };

                // TextBox für die Passworteingabe (mit Maskierung durch Sterne)
                TextBox textBox = new TextBox() { Left = 10, Top = 30, Width = 200, PasswordChar = '*' };

                // OK-Button, der das DialogResult.OK zurückliefert
                Button confirmation = new Button() { Text = "OK", Left = 10, Width = 100, Top = 60, DialogResult = DialogResult.OK };

                // Steuerelemente zum Formular hinzufügen
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);

                // Enter-Taste auf den OK-Button legen
                prompt.AcceptButton = confirmation;

                // Dialog anzeigen und zurückgeben, was der Benutzer eingegeben hat (null bei Abbruch)
                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }
    }
}
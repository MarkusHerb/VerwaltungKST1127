using System; // Importieren des System-Namespace für grundlegende Funktionalitäten
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen
using System.IO;
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für GUI-Funktionalität
using System.Windows.Forms.DataVisualization.Charting; // Um SQL funktionen zu verwenden
using VerwaltungKST1127.Auftragsverwaltung;
using VerwaltungKST1127.EingabeSerienartikelPrototyp;
using VerwaltungKST1127.Farbauswertung;
using VerwaltungKST1127.Personal;
using VerwaltungKST1127.RingSpannzange; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen

namespace VerwaltungKST1127
{

    public partial class Form_Start : Form
    {
        // Feld für den PerformanceCounter
        private PerformanceCounter cpuCounter;
        // Feld für den PerformanceCounter
        private PerformanceCounter memoryCounter;

        public Form_Start()
        {
            InitializeComponent();
            InitializeChart();
            // Timer für Uhrzeit/Datum starten
            TimerDatumUhrzeit.Start();
            UpdateZeitDatum();
            // ListBoxDocuments laden
            InitializeDocumentList();
            // Initialisieren der Leistungsindikatoren - CPU
            InitializePerformanceCounters();
            // Initialisieren der Leistungsindikatoren - RAM
            InitializeMemoryCounter();
            // Starten des Timers für die CPU-Auslastung
            TimerCpu.Interval = 500; // Aktualisierung alle 0,5 Sekunde
            TimerCpu.Start();
            // Starten des Timers für die RAM-Auslastung
            TimerRam.Interval = 500; // Aktualisierung alle 0,5 Sekunde
            TimerRam.Start();
        }

        // ############## Selbst erstellte Funktionen 
        private void BestellstatusAbfragen()
        {
            // Verbindungszeichenfolge für die SQL Server-Datenbank // Wird benötigt, um den Bestellstatus abzufragen
            string connectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Datenbank öfnnen
                con.Open();
                // Sql-Abfrage, um nach "Bestellen" in der Spalte BestellStatus zu suchen
                string query = "SELECT COUNT(*) FROM Materiallager WHERE BestellStatus = 'Bestellen'";
                SqlCommand cmd = new SqlCommand(query, con);
                // Ausführen der Abfrage und Ergebnise abrufen
                int count = (int)cmd.ExecuteScalar();
                // Variable Bestellung setzten
                int Bestellung = (count > 0) ? 1 : 0;
                // ImagePath
                string imagePath = "";
                // Rufezeichen anzeigen oder nicht
                if (Bestellung == 1)
                {
                    imagePath = "P:\\TEDuTOZ\\Auftragsverwaltung Daten\\VerwaltungKst1127\\Bilder\\RufezeichenFuerBestellung.png";
                }
                // Überprüfen, ob ein gültiger Bildpfad gefunden wurde und das Bild existiert
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // Das Bild der PictureBox zuweisen
                    PictureBoxBestellung.Visible = true;
                    PictureBoxBestellung.Enabled = false;
                    PictureBoxBestellung.Image = new Bitmap(imagePath);
                    PictureBoxBestellung.Refresh();
                }
                else
                {
                    // Anderenfalls die PictureBox leeren oder ein Platzhalterbild anzeigen
                    PictureBoxBestellung.Image = null;
                    PictureBoxBestellung.Visible = false;
                }
            }
        }

        // Funktion für die instanziierung des PerformanceCounters
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

        // Uhrzeit und Datumsfunktion
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now; //Aktuelles Datum und Uhrzeit abrufen
            LblUhrzeitDatum.Text = aktuell.ToString("dd.MM.yyyy HH:mm:ss"); // Datum und Uhrzeit im zugeweisenen Label anzeigen
            // Tickevent des UhrzeitsTimers verwenden damit die PictureBoxBestellung ständig aktualisiert wird
        }

        // ############## Event-Handler für die unterschiedlichen Items aus der Toolbox

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

            // Setze die Hintergrundfarbe des ChartArea
            chartPerformance.ChartAreas[0].BackColor = Color.FromArgb(224, 224, 224);

        }

        // Timer-Event für die CPU-Auslastung
        private void TimerCpu_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            float cpu = (float)(random.NextDouble() * (0.2 - 0.05) + 0.05);

            if (cpuCounter != null)
            {
                float cpuUsage = cpuCounter.NextValue(); // CPU-Auslastung abrufen
                LblCpu.Text = string.Format("CPU: {0:F3}%", cpu + cpuUsage); // Auslastung in Label anzeigen
                UpdateChart(chartPerformance.Series["CPU"], cpu + cpuUsage); // Diagramm aktualisieren
            }
            else
            {
                MessageBox.Show("cpuCounter wurde nicht initialisiert.");
            }
        }

        // Timer-Event für die RAM-Auslastung
        private void TimerRam_Tick(object sender, EventArgs e)
        {
            if (memoryCounter != null)
            {
                float memoryUsage = memoryCounter.NextValue(); // Arbeitsspeicher-Auslastung abrufen
                LblRam.Text = string.Format("RAM: {0:F3}%", memoryUsage); // Auslastung in Label anzeigen
                UpdateChart(chartPerformance.Series["RAM"], memoryUsage); // Diagramm aktualisieren
            }
            else
            {
                MessageBox.Show("memoryCounter wurde nicht initialisiert.");
            }
        }

        // Funktion zum Aktualisieren des Diagramms
        private void UpdateChart(Series series, float value)
        {
            // Füge den neuen Wert hinzu
            series.Points.AddY(value);

            // Entferne den ersten Wert, wenn die Anzahl der Punkte 20 überschreitet
            if (series.Points.Count > 50)
            {
                series.Points.RemoveAt(0);
            }

            // Aktualisiere die X-Werte, um sicherzustellen, dass sie von 0 bis 99 gehen
            for (int i = 0; i < series.Points.Count; i++)
            {
                series.Points[i].XValue = i;
            }

            // Berechne den maximalen Y-Wert aus beiden Serien und füge einen Puffer hinzu
            double maxYValue = 0;
            foreach (var point in chartPerformance.Series["CPU"].Points)
            {
                if (point.YValues[0] > maxYValue)
                {
                    maxYValue = point.YValues[0];
                }
            }
            foreach (var point in chartPerformance.Series["RAM"].Points)
            {
                if (point.YValues[0] > maxYValue)
                {
                    maxYValue = point.YValues[0];
                }
            }
            double buffer = 5; // Fester Pufferwert
            chartPerformance.ChartAreas[0].AxisY.Maximum = maxYValue + buffer;

            // Aktualisiere die X-Achse, um den Bereich von 0 bis 100 anzuzeigen
            chartPerformance.ChartAreas[0].AxisX.Minimum = 0;
            chartPerformance.ChartAreas[0].AxisX.Maximum = 50;
        }

        // Timer Event für Datum/Uhrzeit
        private void TimerDatumUhrzeit_Tick(object sender, EventArgs e)
        {
            UpdateZeitDatum();
            BestellstatusAbfragen();
        }
        // ##### Buttons #####
        // Button Event, wenn man darauf klickt
        private void BtnSerienartikelPrototyp_Click(object sender, EventArgs e)
        {
            // Formular EingabeSeriePrototyp öffnen
            Form_EingabeSeriePrototyp form_EingabeSeriePrototyp = new Form_EingabeSeriePrototyp();
            form_EingabeSeriePrototyp.ShowDialog();
        }

        // Button Event, wenn man darauf klickt
        private void BtnPrototypenAuftragErstellen_Click(object sender, EventArgs e)
        {
            Form_PrototypenauftragErstellen form_PrototypenauftragErstellen = new Form_PrototypenauftragErstellen();
            form_PrototypenauftragErstellen.ShowDialog();
        }

        // Button Event, wenn man darauf klickt
        private void BtnFarbwerte_Click(object sender, EventArgs e)
        {
            Form_Farbauswertung form_Farbauswertung = new Form_Farbauswertung();
            form_Farbauswertung.Show();
        }

        // Button Event, wenn man darauf klickt
        private void BtnMateriallager_Click(object sender, EventArgs e)
        {
            Form_Materiallager form_Materiallager = new Form_Materiallager();
            form_Materiallager.Show();
        }

        // Button Event, wenn man darauf klickt
        private void BtnHomepage_Click(object sender, EventArgs e)
        {
            string url = "https://www.swarovskioptik.com/at/de/jagd";
            Process.Start(url);
        }

        // Button Event, wenn man darauf klickt
        private void BtnLupe_Click(object sender, EventArgs e)
        {
            string url = "http://lupe.swarovskioptik.at/";
            Process.Start(url);
        }

        // Button Event, wenn man darauf klickt
        private void BtnInformation_Click(object sender, EventArgs e)
        {
            Form_Copyright form_Copyright = new Form_Copyright();
            form_Copyright.Show();
        }

        // Button Event, wenn man darauf klickt
        private void BtnPersonalliste_Click(object sender, EventArgs e)
        {
            Form_Personalliste form_Personalliste = new Form_Personalliste();
            form_Personalliste.Show();
        }

        // Event-Handler, wenn man darauf klickt
        private void BtnAuftragsverwaltung_Click(object sender, EventArgs e)
        {
            Form_HauptansichtAuftragsverwaltung form_hauptansichtAuftragsverwaltung = new Form_HauptansichtAuftragsverwaltung();
            form_hauptansichtAuftragsverwaltung.Show();
        }

        // #### ListBox ####
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
            {"Ausschussliste", @"P:\TEDuTOZ\Verschiedenes\Alte Dateien\Fehlerliste für Produktionschargen 2008.xls"},
            {" ", @"C:\"},
            {"Ordner - Beläge", @"P:\TEDuTOZ\Beläge"},
            {"Ordner - Anlagen", @"P:\TEDuTOZ\Beschichtungsanlagen"},
            {"Ordner - BBM", @"P:\TEDuTOZ\BBM"},
            {"Ordner - Messdaten", @"P:\Messdata\UV"},
            // Weitere Dokumente hinzufügen {"", @""},
        };

        // ListBox mit Dokumentnamen fülle
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

        // Event-Handler, wenn ein Dokument ausgewählt wird
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

        // Index des Elements, über dem die Maus schwebt
        private int hoveredIndex = -1;

        // Wenn das Item ausgewählt wird -> Format wird angepasst
        private void ListBoxDocuments_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                string text = ListBoxDocuments.Items[e.Index].ToString();
                Font font = e.Font;

                // Wenn das Element ausgewählt ist, oder die Maus über dem Element schwebt, Schriftart fett machen
                if (e.Index == hoveredIndex)
                {
                    font = new Font(e.Font, FontStyle.Underline | FontStyle.Italic | FontStyle.Bold);
                }

                TextRenderer.DrawText(e.Graphics, text, font, e.Bounds, e.ForeColor, TextFormatFlags.Left);
            }
            e.DrawFocusRectangle();
        }

        // Event-Handler, dass wenn ich über ein Item fahre mit der Maus, etwas passiert
        private void ListBoxDocuments_MouseMove(object sender, MouseEventArgs e)
        {
            int newHoveredIndex = ListBoxDocuments.IndexFromPoint(e.Location);

            if (newHoveredIndex != hoveredIndex)
            {
                hoveredIndex = newHoveredIndex;
                ListBoxDocuments.Invalidate(); // Neu zeichnen
            }
        }

        // Event-Handler, dass wenn ich mit der Maus das Item verlasse
        private void ListBoxDocuments_MouseLeave(object sender, EventArgs e)
        {
            hoveredIndex = -1;
            ListBoxDocuments.Invalidate(); // Neu zeichnen
        }

        // Event-Handler, wenn aus der ListBox etwas ausgewählt wird
        private void ListBoxDocuments_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedDocument();
        }

        // Event-Handler, wenn der Button "Dokument öffnen" geklickt wird
        private void BtnEingabeRingSpannzange_Click(object sender, EventArgs e)
        {
            DgvZuordnungArtikel form_UebersichtRingSpannzangen = new DgvZuordnungArtikel();
            form_UebersichtRingSpannzangen.Show();
        }
    }
}

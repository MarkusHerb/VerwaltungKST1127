using System; // Importieren des System-Namespace für grundlegende Funktionalitäten
using System.Diagnostics;
using System.Drawing; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für GUI-Funktionalität
using VerwaltungKST1127.EingabeSerienartikelPrototyp;
using VerwaltungKST1127.Farbauswertung;
using VerwaltungKST1127.Personal; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen
using System.Data.SqlClient;
using System.IO;
using VerwaltungKST1127.Auftragsverwaltung; // Um SQL funktionen zu verwenden

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
                    imagePath = "K:\\Kst_127\\Programmierung\\Programierung Markus\\VerwaltungKST1127\\VerwaltungKST1127\\Bilder\\RufezeichenFuerBestellung.png";
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

        // Tick-Event für die sekündliche Aktualisierung der CPU Auslasung
        private void TimerCpu_Tick(object sender, EventArgs e)
        {
            // Erstellen einer Instanz von Random
            Random random = new Random();

            // Generieren eines Zufallswerts zwischen 0.05 und 0.2
            float cpu = (float)(random.NextDouble() * (0.2 - 0.05) + 0.05);

            // Überprüfen, ob cpuCounter initialisiert wurde
            if (cpuCounter != null)
            {
                // Abrufen des aktuellen CPU-Auslastungswerts
                float cpuUsage = cpuCounter.NextValue();

                // Anzeigen der CPU-Auslastung auf einem Label
                LblCpu.Text = string.Format("CPU: {0:F3}%", cpu + cpuUsage);
            }
            else
            {
                // Wenn cpuCounter nicht initialisiert wurde, eine Fehlermeldung anzeigen
                MessageBox.Show("cpuCounter wurde nicht initialisiert.");
            }
        }

        // Tick-Event für die sekündliche Aktualisierung der RAM Auslastung
        private void TimerRam_Tick(object sender, EventArgs e)
        {
            // Überprüfen, ob memoryCounter initialisiert wurde
            if (memoryCounter != null)
            {
                // Abrufen des aktuellen RAM-Auslastungswerts
                float memoryUsage = memoryCounter.NextValue();

                // Anzeigen der RAM-Auslastung auf einem Label
                LblRam.Text = string.Format("RAM: {0:F3}%", memoryUsage);
            }
            else
            {
                // Wenn memoryCounter nicht initialisiert wurde, eine Fehlermeldung anzeigen
                MessageBox.Show("memoryCounter wurde nicht initialisiert.");
            }
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
        private string[,] documents = {
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

        
    }
}

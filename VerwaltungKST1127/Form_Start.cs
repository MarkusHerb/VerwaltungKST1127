using System; // Importieren des System-Namespace für grundlegende Funktionalitäten
using System.Diagnostics;
using System.Drawing; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für GUI-Funktionalität
using VerwaltungKST1127.EingabeSerienartikelPrototyp;
using VerwaltungKST1127.Farbauswertung;
using VerwaltungKST1127.Personal; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen

namespace VerwaltungKST1127
{
    public partial class Form_Start : Form
    {
        public Form_Start()
        {
            InitializeComponent();
            // Timer für Uhrzeit/Datum starten
            TimerDatumUhrzeit.Start(); 
            UpdateZeitDatum();
            // ListBoxDocuments laden
            InitializeDocumentList();
        }

        // ############## Selbst erstellte Funktionen 

        // Uhrzeit und Datumsfunktion
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now; //Aktuelles Datum und Uhrzeit abrufen
            LblUhrzeitDatum.Text = aktuell.ToString("dd.MM.yyyy HH:mm:ss"); // Datum und Uhrzeit im zugeweisenen Label anzeigen
        }

        // ############## Event-Handler für die unterschiedlichen Items aus der Toolbox

        // Timer Event für Datum/Uhrzeit
        private void TimerDatumUhrzeit_Tick(object sender, EventArgs e)
        {
            UpdateZeitDatum();
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
            form_Farbauswertung.ShowDialog();
        }

        // Button Event, wenn man darauf klickt
        private void BtnMateriallager_Click(object sender, EventArgs e)
        {
            Form_Materiallager form_Materiallager = new Form_Materiallager();
            form_Materiallager.ShowDialog();
        }

        // Button Event, wenn man darauf klickt
        private void BtnHomepage_Click(object sender, EventArgs e)
        {
            string url = "https://www.swarovskioptik.com/at/de/jagd";
            System.Diagnostics.Process.Start(url);
        }

        // Button Event, wenn man darauf klickt
        private void BtnLupe_Click(object sender, EventArgs e)
        {
            string url = "http://lupe.swarovskioptik.at/";
            System.Diagnostics.Process.Start(url);  
        }

        // Button Event, wenn man darauf klickt
        private void BtnInformation_Click(object sender, EventArgs e)
        {
            Form_Copyright form_Copyright = new Form_Copyright();
            form_Copyright.ShowDialog();    
        }

        // Button Event, wenn man darauf klickt
        private void BtnPersonalliste_Click(object sender, EventArgs e)
        {
            Form_Personalliste form_Personalliste = new Form_Personalliste();
            form_Personalliste.ShowDialog();
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

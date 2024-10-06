using System; // Importieren des System-Namespace für grundlegende Funktionalitäten
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für GUI-Funktionalität
using VerwaltungKST1127.EingabeSerienartikelPrototyp; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen

namespace VerwaltungKST1127
{
    public partial class Form_Start : Form
    {
        public Form_Start()
        {
            InitializeComponent();
            TimerDatumUhrzeit.Start(); // Timer für Uhrzeit/Datum starten
            UpdateZeitDatum();
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

        // Button Event, wenn man darauf klickt
        private void BtnSerienartikelPrototyp_Click(object sender, EventArgs e)
        {
            // Formular EingabeSeriePrototyp öffnen
            Form_EingabeSeriePrototyp form_EingabeSeriePrototyp = new Form_EingabeSeriePrototyp();
            form_EingabeSeriePrototyp.ShowDialog();
        }

        // Button Event, wenn man darauf klickt
        private void BtnArtikelAendern_Click(object sender, EventArgs e)
        {
            // Formular ArtikelPrototypAendern öffnen
            Form_ArtikelPrototypAendern form_ArtikelAender = new Form_ArtikelPrototypAendern();
            form_ArtikelAender.ShowDialog();
        }
    }
}

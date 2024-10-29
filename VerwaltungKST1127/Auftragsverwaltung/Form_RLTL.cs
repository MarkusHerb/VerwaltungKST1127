using Newtonsoft.Json; // Importiere die JSON-Bibliothek für die Serialisierung und Deserialisierung
using System; // Importiere grundlegende Systemfunktionen
using System.IO; // Importiere Funktionen zum Arbeiten mit Dateien
using System.Linq;
using System.Windows.Forms; // Importiere Windows Forms für die Benutzeroberfläche

namespace VerwaltungKST1127.Auftragsverwaltung // Definiere den Namespace
{
    public partial class Form_RLTL : Form // Definiere die Klasse für das Formular, das die RL/TL-Verwaltung behandelt
    {
        private RLTLData rltlData; // Deklariere eine Variable für die RL/TL-Daten
        private readonly string jsonFilePath = "rltl_data.json"; // Pfad zur JSON-Datei für die Speicherung der Daten

        // Konstruktor für das Formular
        public Form_RLTL()
        {
            InitializeComponent(); // Initialisiere die Benutzeroberfläche
            LoadData(); // Lade die RL/TL-Daten aus der JSON-Datei
        }

        // Methode zum Laden der Daten aus der JSON-Datei
        private void LoadData()
        {
            // Überprüfe, ob die JSON-Datei existiert
            if (File.Exists(jsonFilePath))
            {
                // Lese den Inhalt der JSON-Datei
                var json = File.ReadAllText(jsonFilePath);
                // Deserialisiere die JSON-Daten in ein RLTLData-Objekt
                rltlData = JsonConvert.DeserializeObject<RLTLData>(json);
            }
            else
            {
                // Wenn die Datei nicht existiert, initialisiere rltlData als neues RLTLData-Objekt
                rltlData = new RLTLData();
            }
            UpdateUI(); // Aktualisiere die Benutzeroberfläche mit den geladenen Daten
        }

        // Methode zum Speichern der Daten in der JSON-Datei
        private void SaveData()
        {
            // Serialisiere die rltlData in JSON-Format mit eingerückten Zeilen
            var json = JsonConvert.SerializeObject(rltlData, Newtonsoft.Json.Formatting.Indented);
            // Schreibe die JSON-Daten in die Datei
            File.WriteAllText(jsonFilePath, json);
        }

        // Methode zur Aktualisierung der Benutzeroberfläche (UI)
        private void UpdateUI()
        {
            // Leere die Listen für RL und TL
            lstRL.Items.Clear();
            lstTL.Items.Clear();
            // Füge die Elemente aus rltlData in die Listen ein, sortiere sie absteigend
            lstRL.Items.AddRange(rltlData.RL.OrderBy(item => item).ToArray());
            lstTL.Items.AddRange(rltlData.TL.OrderBy(item => item).ToArray());
        }

        // Event-Handler für das Hinzufügen von RL
        private void btnAddRL_Click(object sender, EventArgs e)
        {
            // Überprüfe, ob das Eingabefeld nicht leer ist und ob der Wert nicht bereits existiert
            if (!string.IsNullOrWhiteSpace(txtRL.Text) && !rltlData.RL.Contains(txtRL.Text))
            {
                // Füge den neuen RL-Wert zur Liste hinzu
                rltlData.RL.Add(txtRL.Text);
                txtRL.Clear(); // Leere das Eingabefeld
                UpdateUI(); // Aktualisiere die Benutzeroberfläche
            }
        }

        // Event-Handler für das Hinzufügen von TL
        private void btnAddTL_Click(object sender, EventArgs e)
        {
            // Überprüfe, ob das Eingabefeld nicht leer ist und ob der Wert nicht bereits existiert
            if (!string.IsNullOrWhiteSpace(txtTL.Text) && !rltlData.TL.Contains(txtTL.Text))
            {
                // Füge den neuen TL-Wert zur Liste hinzu
                rltlData.TL.Add(txtTL.Text);
                txtTL.Clear(); // Leere das Eingabefeld
                UpdateUI(); // Aktualisiere die Benutzeroberfläche
            }
        }

        // Event-Handler für das Entfernen von RL
        private void btnRemoveRL_Click(object sender, EventArgs e)
        {
            // Überprüfe, ob ein Element in der Liste ausgewählt ist
            if (lstRL.SelectedItem != null)
            {
                // Entferne das ausgewählte Element aus der RL-Liste
                rltlData.RL.Remove(lstRL.SelectedItem.ToString());
                UpdateUI(); // Aktualisiere die Benutzeroberfläche
            }
        }

        // Event-Handler für das Entfernen von TL
        private void btnRemoveTL_Click(object sender, EventArgs e)
        {
            // Überprüfe, ob ein Element in der Liste ausgewählt ist
            if (lstTL.SelectedItem != null)
            {
                // Entferne das ausgewählte Element aus der TL-Liste
                rltlData.TL.Remove(lstTL.SelectedItem.ToString());
                UpdateUI(); // Aktualisiere die Benutzeroberfläche
            }
        }

        // Event-Handler für das Schließen des Formulars
        private void Form_RLTL_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData(); // Speichere die Daten, bevor das Formular geschlossen wird
        }
    }
}

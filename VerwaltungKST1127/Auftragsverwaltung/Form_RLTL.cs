// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus externen Bibliotheken bzw. dem .NET-Framework.
// Damit müssen wir z. B. nicht "Newtonsoft.Json.JsonConvert" schreiben, sondern nur "JsonConvert".
// ===================================================================================================
using Newtonsoft.Json;            // JSON-Bibliothek (NuGet-Paket): Objekte ↔ JSON umwandeln
using System;                     // Basis-Typen wie string, int, EventArgs, ...
using System.IO;                  // Dateioperationen: File.Exists, File.ReadAllText, File.WriteAllText
using System.Linq;                // LINQ-Funktionen wie OrderBy, Contains, ToArray
using System.Windows.Forms;       // Windows Forms (Form, Button, ListBox, TextBox ...)

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127.Auftragsverwaltung
{
    // "public partial class Form_RLTL : Form"
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien aufgeteilt sein
    //             (Form_RLTL.cs + Form_RLTL.Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_RLTL : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Felder = Variablen, die zur ganzen Klasse gehören und in jeder Methode sichtbar sind.
        // "private" = nur innerhalb dieser Klasse sichtbar.
        // -----------------------------------------------------------------------------------------------------------------

        // Hier werden die geladenen RL/TL-Daten zur Laufzeit gehalten.
        private RLTLData rltlData;

        // "readonly" = darf nur einmal (hier oder im Konstruktor) gesetzt werden, danach nicht mehr ändern.
        // Pfad zur JSON-Datei, in der wir die Listen dauerhaft speichern.
        // Relativer Pfad → wird im Arbeitsverzeichnis der Anwendung abgelegt.
        private readonly string jsonFilePath = "rltl_data.json";

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft automatisch bei "new Form_RLTL()".
        // -----------------------------------------------------------------------------------------------------------------
        public Form_RLTL()
        {
            InitializeComponent(); // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)
            LoadData();            // gleich beim Öffnen die gespeicherten Daten einlesen
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Lädt die Daten aus der JSON-Datei in das Feld "rltlData".
        // -----------------------------------------------------------------------------------------------------------------
        private void LoadData()
        {
            // File.Exists prüft, ob die Datei existiert.
            if (File.Exists(jsonFilePath))
            {
                // Den kompletten Datei-Inhalt als String einlesen.
                var json = File.ReadAllText(jsonFilePath);

                // JSON-String → C#-Objekt umwandeln ("deserialisieren").
                // Ergebnis wird als RLTLData behandelt (Typ-Parameter <RLTLData>).
                rltlData = JsonConvert.DeserializeObject<RLTLData>(json);
            }
            else
            {
                // Datei gibt es noch nicht (z. B. erster Start) → leeres Daten-Objekt anlegen.
                rltlData = new RLTLData();
            }

            UpdateUI(); // ListBoxes mit den geladenen Werten füllen
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Speichert das aktuelle Daten-Objekt als JSON in die Datei.
        // -----------------------------------------------------------------------------------------------------------------
        private void SaveData()
        {
            // C#-Objekt → JSON-String umwandeln ("serialisieren").
            // "Formatting.Indented" sorgt für eingerückten, gut lesbaren JSON-Text.
            var json = JsonConvert.SerializeObject(rltlData, Newtonsoft.Json.Formatting.Indented);

            // Den JSON-Text in die Datei schreiben (überschreibt die alte Version komplett).
            File.WriteAllText(jsonFilePath, json);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Aktualisiert die Benutzeroberfläche (die beiden ListBoxen) auf Basis von rltlData.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateUI()
        {
            // Beide Listen leeren, damit wir keine doppelten Einträge bekommen.
            lstRL.Items.Clear();
            lstTL.Items.Clear();

            // Die Werte aus den C#-Listen sortiert in die UI-Listen übernehmen.
            // OrderBy(item => item) → alphabetisch aufsteigend (Lambda-Ausdruck: für jedes Item den Wert selbst als Sortierschlüssel nehmen).
            // ToArray() → da AddRange ein Array erwartet.
            lstRL.Items.AddRange(rltlData.RL.OrderBy(item => item).ToArray());
            lstTL.Items.AddRange(rltlData.TL.OrderBy(item => item).ToArray());
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "RL hinzufügen": Wert aus der TextBox in die RL-Liste übernehmen.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnAddRL_Click(object sender, EventArgs e)
        {
            // Bedingung mit zwei Teilen, verbunden mit "&&" (und):
            //  1) txtRL.Text darf nicht leer/whitespace sein
            //  2) Der Wert darf noch nicht in der Liste vorkommen (Contains prüft auf Vorhandensein)
            if (!string.IsNullOrWhiteSpace(txtRL.Text) && !rltlData.RL.Contains(txtRL.Text))
            {
                // Wert zur RL-Liste hinzufügen.
                rltlData.RL.Add(txtRL.Text);

                // Eingabefeld leeren – bessere Bedienung für den Nutzer.
                txtRL.Clear();

                // ListBox neu befüllen.
                UpdateUI();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "TL hinzufügen": analog zu btnAddRL_Click, aber für die TL-Liste.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnAddTL_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTL.Text) && !rltlData.TL.Contains(txtTL.Text))
            {
                rltlData.TL.Add(txtTL.Text);
                txtTL.Clear();
                UpdateUI();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "RL entfernen": das in der ListBox markierte Element löschen.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnRemoveRL_Click(object sender, EventArgs e)
        {
            // SelectedItem ist null, wenn nichts markiert wurde → dann einfach nichts tun.
            if (lstRL.SelectedItem != null)
            {
                // Das markierte Element ist ein object → mit ToString() in einen String umwandeln,
                // damit Remove den passenden Wert aus der Liste löschen kann.
                rltlData.RL.Remove(lstRL.SelectedItem.ToString());
                UpdateUI();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "TL entfernen": analog zu btnRemoveRL_Click.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnRemoveTL_Click(object sender, EventArgs e)
        {
            if (lstTL.SelectedItem != null)
            {
                rltlData.TL.Remove(lstTL.SelectedItem.ToString());
                UpdateUI();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wird aufgerufen, BEVOR das Fenster geschlossen wird (Event "FormClosing").
        // Hier speichern wir die Daten, damit beim nächsten Start alles wieder da ist.
        // -----------------------------------------------------------------------------------------------------------------
        private void Form_RLTL_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData();
        }
    }
}
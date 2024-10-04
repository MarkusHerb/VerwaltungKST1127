using Microsoft.Office.Interop.Excel; // Importieren der Excel-Interop-Bibliothek für die Automatisierung und Interaktion mit Microsoft Excel (z.B. zum Erstellen, Lesen und Bearbeiten von Excel-Dateien)
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic; // Importieren des System.Collections.Generic-Namespace für generische Sammlungen (z.B. List<T>, Dictionary<TKey, TValue>)
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für Komponentenmodelle und Datenbindung (z.B. zum Arbeiten mit Events und benachrichtigungsfähigen Eigenschaften)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Diagnostics; // Importieren des System.Diagnostics-Namespace für die Diagnose und Protokollierung (z.B. zum Starten von Prozessen, Debuggen und Ereignisprotokollierung)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Runtime.InteropServices; // Importieren des System.Runtime.InteropServices-Namespace für den Zugriff auf nicht verwalteten Code und Interoperabilität mit COM-Objekten (z.B. für die Interaktion mit Windows-APIs oder älteren Systemen)
using System.Security.Cryptography;
using System.Threading.Tasks; // Importieren des System.Threading.Tasks-Namespace für die Arbeit mit asynchronen Tasks und paralleler Programmierung (z.B. zur Ausführung von Aufgaben im Hintergrund)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)


namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    public partial class Form_EingabeSeriePrototyp : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank Verwaltung
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");
        // SOA127 Shuttle
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        public Form_EingabeSeriePrototyp()
        {
            InitializeComponent();
            // Befülle die GNummer-ComboBox beim Initialisieren des Formulars
            FillComboBoxGNummer();
            // Eventhandler für die ComboBoxGNummer hinzufügen, um auf Änderungen zu reagieren
            ComboboxGNummer.SelectedIndexChanged += ComboboxGNummer_SelectedIndexChanged;
            // Initialisiere die Glassorten-ComboBox mit allen Glassorten (keine GNummer-Auswahl)
            FillComboboxGlassorte(null);
            // Befülle die GNummer-ComboBox beim Initialisieren des Formulars
            FillComboboxRing();
        }

        // ############## Selbst erstellte Funktionen ################

        // Funktion um die ComboboxGNummer mit eindeutigen GNummer-Werten zu befüllen
        private void FillComboBoxGNummer()
        {
            // SQL-Abfrage, um alle eindeutigen GNummern abzurufen
            string query = "SELECT DISTINCT GNummer FROM Glassdaten";

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole die GNummer aus der aktuellen Zeile
                            string gNummerValue = reader["GNummer"].ToString();

                            // Füge die GNummer der Combobox hinzu
                            ComboboxGNummer.Items.Add(gNummerValue);
                        }
                    }
                }

                // Sortiere die Einträge in der ComboBox alphabetisch
                ComboboxGNummer.Sorted = true;
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der GNummern: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Funktion um die ComboboxGlassorte basierend auf der ausgewählten GNummer zu befüllen
        private void FillComboboxGlassorte(string gNummer)
        {
            // Wenn keine GNummer ausgewählt ist, lade alle Glassorten
            string query;

            if (string.IsNullOrEmpty(gNummer))
            {
                // Lade alle Glassorten, wenn keine GNummer ausgewählt wurde
                query = "SELECT DISTINCT Glass FROM Glassdaten";
            }
            else
            {
                // Lade nur die Glassorten, die zur ausgewählten GNummer gehören
                query = "SELECT DISTINCT Glass FROM Glassdaten WHERE GNummer = @GNummer";
            }

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Wenn eine GNummer vorhanden ist, füge den Parameter hinzu
                    if (!string.IsNullOrEmpty(gNummer))
                    {
                        sqlCommand.Parameters.AddWithValue("@GNummer", gNummer);
                    }

                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die Glassorten-ComboBox, bevor neue Einträge hinzugefügt werden
                        ComboboxGlassorte.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole den Glass-Wert aus der aktuellen Zeile
                            string glassValue = reader["Glass"].ToString();

                            // Füge die Glassorte der ComboBox hinzu
                            ComboboxGlassorte.Items.Add(glassValue);
                        }
                    }
                }

                // Sortiere die Einträge in der ComboBox alphabetisch
                ComboboxGlassorte.Sorted = true;
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Glassorten: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Funktion um die ComboboxRing basierend auf der ausgewählten Durchmesser zu befüllen
        // Klassenvariable definieren
        private void FillComboboxRing()
        {
            // SQL-Abfrage, um Vorrichtungsnummer und den dazugehörigen Durchmesser_Innen abzurufen und nach Durchmesser_Innen zu sortieren
            string query = @"
            SELECT DISTINCT Vorrichtungsnummer, Durchmesser_Innen
            FROM Ring_Stamm
            WHERE Vorrichtungsnummer NOT LIKE 'Z%'  -- Ausschließen von Vorrichtungsnummern, die mit 'Z' beginnen
            ORDER BY Durchmesser_Innen ASC"; // Sortierung nach Durchmesser_Innen in aufsteigender Reihenfolge

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionShuttle.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionShuttle))
                {
                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die ComboBox bevor neue Einträge hinzugefügt werden
                        ComboboxRing.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole die Vorrichtungsnummer und den Durchmesser_Innen aus der aktuellen Zeile
                            string ringNummerValue = reader["Vorrichtungsnummer"].ToString();
                            string durchmesserValue = reader["Durchmesser_Innen"].ToString();

                            // Kombiniere Vorrichtungsnummer und Durchmesser_Innen für die Anzeige in der ComboBox
                            string displayValue = $"{ringNummerValue} - {durchmesserValue} mm";

                            // Füge den kombinierten Wert der ComboBox hinzu
                            ComboboxRing.Items.Add(displayValue);
                        }
                    }
                }

                // Deaktiviere die automatische Sortierung der ComboBox
                ComboboxRing.Sorted = false; // Setzen auf false, um die Einträge in der Reihenfolge zu lassen, in der sie hinzugefügt wurden
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Ringe: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionShuttle.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionShuttle.Close();
                }
            }
        }

        // Funktion, dass die Daten in der Serienlinsen Tabelle gespeichert werden
        private void SpeichereDatenInDatenbank(string artikelnummer, string bezeichnung, string status, string gruppenname, string zukauf, string flaeche
            , string gNummer)
        {
            try
            {
                sqlConnectionVerwaltung.Open();
                // SQL-Befehl zum Einfügen der Daten
                string query = @"
                INSERT INTO Serienlinsen (ARTNR, BEZ, Status, GruppenName, Zukauf, Innen-Aussen, G_Nummer)
                VALUES (@ARTNR, @BEZ, @Status, @GruppenName, @Zukauf, @Innen-Aussen, @G_Numer)";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Werte als Parameter hinzufügen
                    sqlCommand.Parameters.AddWithValue("@ARTNR", artikelnummer);
                    sqlCommand.Parameters.AddWithValue("@BEZ", bezeichnung);
                    sqlCommand.Parameters.AddWithValue("@Status", status);
                    sqlCommand.Parameters.AddWithValue("@GruppenName", gruppenname);
                    sqlCommand.Parameters.AddWithValue("@Zukauf", zukauf);
                    sqlCommand.Parameters.AddWithValue("@Innen-Aussen", flaeche);
                    sqlCommand.Parameters.AddWithValue("@G_Nummer", gNummer);

                    // SQL-Befehl ausführen
                    sqlCommand.ExecuteNonQuery();
                }

                MessageBox.Show("Daten erfolgreich gespeichert!");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
            sqlConnectionVerwaltung.Close();
        }


        // ############## Event-Handler  ################

        // Eventhandler für die Änderung der Auswahl in der ComboboxGNummer
        private void ComboboxGNummer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfe, ob eine Auswahl in der Combobox getroffen wurde
            if (ComboboxGNummer.SelectedItem != null)
            {
                // Wenn eine GNummer ausgewählt wurde, rufe die ausgewählte GNummer ab
                string selectedGNummer = ComboboxGNummer.SelectedItem.ToString();

                // Befülle die ComboboxGlassorte mit den Glassorten, die zu dieser GNummer gehören
                FillComboboxGlassorte(selectedGNummer);
            }
            else
            {
                // Wenn keine GNummer ausgewählt wurde, lade alle Glassorten
                FillComboboxGlassorte(null);
            }
        }
        // Eventhandler wenn auf die PB doppelt geklickt wird
        private void PictureboxAuflegenLinsenPrismen_DoubleClick(object sender, EventArgs e)
        {
            // Erstelle und konfiguriere den OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // Zeige den Dialog an und überprüfe, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Hole den ausgewählten Dateipfad
                    string selectedFilePath = openFileDialog.FileName;

                    // Lade das Bild in die PictureBox
                    PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(selectedFilePath);

                    // Aktualisiere das Label mit dem vollständigen Pfad
                    LblPfadAuflegenLinsenPrismen.Text = selectedFilePath;
                }
            }
        }

        // Eventhandler wenn auf die PB doppelt geklickt wird
        private void PictureboxZusatzinfo_DoubleClick(object sender, EventArgs e)
        {
            // Erstelle und konfiguriere den OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung\InfoZeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // Zeige den Dialog an und überprüfe, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Hole den ausgewählten Dateipfad
                    string selectedFilePath = openFileDialog.FileName;

                    // Lade das Bild in die PictureBox
                    PictureboxZusatzinfo.Image = Image.FromFile(selectedFilePath);

                    // Aktualisiere das Label mit dem vollständigen Pfad
                    LblPfadZusatzinfo.Text = selectedFilePath;
                }
            }
        }

        // Eventhandler wenn auf den Button geklickt wird
        private void BtnArtikelSpeichern_Click(object sender, EventArgs e)
        {
            // Werte aus den Steuerelementen holen
            string artikelnummer = TxtboxArtikelnummer.Text;
            string bezeichnung = TxtboxBezeichnung.Text;
            string status = ComboboxStatus.Text;
            string[] gruppennameArray = artikelnummer.Split('-').Select(s => s.Trim()).ToArray();
            string gruppenname = gruppennameArray[0];
            string zukauf = ComboBoxZukauf.Text;
            string flaeche = ComboBoxFlaeche.Text;
            string gNummer = ComboboxGNummer.Text;
            string glassorte = ComboboxGlassorte.Text;
            string durchmesser = TxtboxDurchmesser.Text;
            string durchmesserWaschen = TxtboxDmWaschen.Text;
            string freibereich = TxtboxFreibereich.Text;
            string dicke = TxtboxDicke.Text;
            string seite = ComboboxSeite.Text;
            string brechwert = TxtboxBrechwert.Text;
            string radiusVerguetung = TxtboxRadiusVerguetung.Text;
            string radiusRueckseite = TxtboxRadiusRueckseite.Text;
            string belagVerguetung = TxtboxBelagVerguetung.Text;
            string prozess = TxtboxBelagProzess.Text;
            string belagRueckseite = TxtboxBelagRueckseite.Text;
            string ring = ComboboxRing.Text;
            string stkSegment = TxtboxStkSegment.Text;
            string stkGesamt = TxtboxStkCharge.Text;
            string zeitProzess = TxtboxZeitProzess.Text;
            string eingabedatum = DateTimePickerAufgenommenLinsePrisma.Text.ToString();
            string bemerkungArtikel = RichtxtboxBemerkung.Text;
            string vorreinigen = ComboboxVorreinigen.Text;
            string ucm = ComboboxUCM497.Text;
            string aceton = ComboboxAceton.Text;   
            string bemerkungWaschen = RichtxtboxBemerkungWaschen.Text;
            string pfadZeichnungAuflegen = LblPfadAuflegenLinsenPrismen.Text;
            string pfadZusatzinfo = LblPfadZusatzinfo.Text;
            string textZusatzinfo = RichtxtboxZusatzinfo.Text;

            // Funktion zum Speichern der Daten aufrufen
            SpeichereDatenInDatenbank(artikelnummer, bezeichnung, status, gruppenname, zukauf, flaeche, gNummer);
        }
    }
}

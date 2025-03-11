using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für die Implementierung von Komponenten und Steuerelementen (z.B. für die Datenbindung und -validierung)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.GlasWaschDaten
{
    public partial class Form_GlasWaschDaten : Form
    {
        // Verbindungszeichenfolgen für die SQL Server-Datenbanken
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        public Form_GlasWaschDaten()
        {
            InitializeComponent(); // Initialisierung der Benutzeroberfläche (GUI) des Formulars
            FillComboBoxArtikel(); // Befüllen der ComboBoxArtikel beim Start des Formulars
            FillComboBoxGNummer(null); // Befüllen der ComboBoxGNummer beim Start des Formulars
            FillComboBoxGlassorte(null); // Befüllen der ComboBoxGlassorte beim Start des Formulars
            UpdateLabels(); // Aktualisieren der Labels mit der Anzahl der einzigartigen GNummern und Glassorten
        }

        // Methode zum Befüllen der ComboBoxArtikel
        private void FillComboBoxArtikel()
        {
            // Öffnen der Datenbankverbindung
            sqlConnectionVerwaltung.Open();

            // SQL-Abfrage, um die eindeutigen Artikelnummern (ARTNR) und Seiteninformationen (SEITE) aus der Tabelle 'Serienlinsen' abzurufen
            string query = @"
                SELECT DISTINCT ARTNR, SEITE
                FROM Serienlinsen
                ORDER BY ARTNR ASC"; // Sortiert die Ergebnisse aufsteigend nach ARTNR

            try
            {
                // Erstellen des SqlCommand-Objekts mit der Abfrage und der geöffneten Verbindung
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Ausführen der Abfrage und Abrufen der Ergebnisse mit einem SqlDataReader
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leeren der ComboBox, um sicherzustellen, dass keine doppelten Einträge vorhanden sind
                        ComboBoxArtikel.Items.Clear();

                        // Durchlaufen der Ergebnisse aus der Abfrage
                        while (reader.Read())
                        {
                            // Abrufen der Werte für Artikelnummer und Seite aus dem aktuellen Datensatz
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();

                            // Zusammenfügen der Artikelnummer und der Seite zu einem Anzeigeformat, z.B. "12345 - links"
                            string displayValue = $"{artikelNummerValue} - {seiteValue}";

                            // Hinzufügen des Anzeigeformats zur ComboBox
                            ComboBoxArtikel.Items.Add(displayValue);
                        }
                    }
                }

                // Sicherstellen, dass die ComboBox nicht automatisch sortiert wird, da dies durch die SQL-Abfrage gesteuert wird
                ComboBoxArtikel.Sorted = false;
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung: Zeigt eine Meldung an, falls beim Laden der Artikel ein Fehler auftritt
                MessageBox.Show("Fehler beim Laden der Artikel: " + ex.Message);
            }
            finally
            {
                // Überprüfen, ob die Datenbankverbindung noch geöffnet ist, und sie bei Bedarf schließen
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Methode zum Befüllen der ComboBoxGNummer
        private void FillComboBoxGNummer(string glasssorte = null)
        {
            // SQL-Abfrage, um alle eindeutigen GNummern abzurufen
            string query;

            if (string.IsNullOrEmpty(glasssorte))
            {
                // Lade alle GNummern, wenn keine Glasssorte ausgewählt wurde
                query = "SELECT DISTINCT GNummer FROM Glassdaten";
            }
            else
            {
                // Lade nur die GNummern, die zur ausgewählten Glasssorte gehören
                query = "SELECT DISTINCT GNummer FROM Glassdaten WHERE Glass = @Glasssorte";
            }

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Wenn eine Glasssorte vorhanden ist, füge den Parameter hinzu
                    if (!string.IsNullOrEmpty(glasssorte))
                    {
                        sqlCommand.Parameters.AddWithValue("@Glasssorte", glasssorte);
                    }

                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die GNummer-ComboBox, bevor neue Einträge hinzugefügt werden
                        ComboBoxGNummer.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole die GNummer aus der aktuellen Zeile
                            string gNummerValue = reader["GNummer"].ToString();

                            // Füge die GNummer der Combobox hinzu
                            ComboBoxGNummer.Items.Add(gNummerValue);
                        }
                    }
                }

                // Sortiere die Einträge in der ComboBox alphabetisch
                ComboBoxGNummer.Sorted = true;
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

        // Methode zum Befüllen der ComboBoxGlassorte
        private void FillComboBoxGlassorte(string gNummer = null)
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
                        ComboBoxGlassorte.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole den Glass-Wert aus der aktuellen Zeile
                            string glassValue = reader["Glass"].ToString();

                            // Füge die Glassorte der ComboBox hinzu
                            ComboBoxGlassorte.Items.Add(glassValue);
                        }
                    }
                }

                // Sortiere die Einträge in der ComboBox alphabetisch
                ComboBoxGlassorte.Sorted = true;
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

        // Event-Handler für die Auswahl eines Artikels in der ComboBox
        private void ComboBoxArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDataGridView();
        }

        // Event-Handler für die Auswahl einer GNummer in der ComboBox
        private void ComboBoxGNummer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfe, ob eine Auswahl in der Combobox getroffen wurde
            if (ComboBoxGNummer.SelectedItem != null)
            {
                // Wenn eine GNummer ausgewählt wurde, rufe die ausgewählte GNummer ab
                string selectedGNummer = ComboBoxGNummer.SelectedItem.ToString();

                // Befülle die ComboboxGlassorte mit den Glassorten, die zu dieser GNummer gehören
                FillComboBoxGlassorte(selectedGNummer);
                LoadDataGridView();
            }
            else
            {
                // Wenn keine GNummer ausgewählt wurde, lade alle Glassorten
                FillComboBoxGlassorte(null);
            }
        }

        // Event-Handler für die Auswahl einer Glasssorte in der ComboBox
        private void ComboBoxGlassorte_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfe, ob eine Auswahl in der Combobox getroffen wurde
            if (ComboBoxGlassorte.SelectedItem != null)
            {
                // Wenn eine Glasssorte ausgewählt wurde, rufe die ausgewählte Glasssorte ab
                string selectedGlasssorte = ComboBoxGlassorte.SelectedItem.ToString();

                // Befülle die ComboboxGNummer mit den GNummern, die zu dieser Glasssorte gehören
                FillComboBoxGNummer(selectedGlasssorte);
                LoadDataGridView();
            }
            else
            {
                // Wenn keine Glasssorte ausgewählt wurde, lade alle GNummern
                FillComboBoxGNummer(null);
            }
        }

        // Methode um alle ComboBoxen zurückzusetzen
        private void ResetComboBoxes()
        {
            // Setze die Auswahl in allen ComboBoxen zurück
            ComboBoxArtikel.SelectedIndex = -1;
            ComboBoxGNummer.SelectedIndex = -1;
            ComboBoxGlassorte.SelectedIndex = -1;
            //ComboBoxen wieder richtig befüllen
            FillComboBoxArtikel();
            FillComboBoxGNummer(null);
            FillComboBoxGlassorte(null);
            // Text der ComoBoxen zurücksetzen
            ComboBoxArtikel.Text = "";
            ComboBoxGNummer.Text = "";
            ComboBoxGlassorte.Text = "";
            // Das Dgv leeren
            DgvAuswahlGlasdaten.DataSource = null;
        }

        // Event-Handler für den Button "Zurücksetzen"
        private void BtnResett_Click(object sender, EventArgs e)
        {
            ResetComboBoxes();
        }

        // Methode zum Laden der Daten in das DataGridView basierend auf den ausgewählten Werten in den ComboBoxen
        private void LoadDataGridView()
        {
            // Basis-SQL-Abfrage
            string query = "SELECT * FROM Serienlinsen WHERE 1=1";

            // Überprüfen, ob ein Artikel ausgewählt wurde
            if (ComboBoxArtikel.SelectedItem != null)
            {
                // Artikelnummer extrahieren und das letzte Segment ignorieren, wenn es eine Zahl ist
                string selectedArtikel = ComboBoxArtikel.SelectedItem.ToString();
                string[] artikelParts = selectedArtikel.Split('-');
                if (artikelParts.Length > 1 && int.TryParse(artikelParts.Last(), out _))
                {
                    selectedArtikel = string.Join("-", artikelParts.Take(artikelParts.Length - 1));
                }
                query += " AND ARTNR = @Artikel";
            }

            // Überprüfen, ob eine GNummer ausgewählt wurde
            if (ComboBoxGNummer.SelectedItem != null)
            {
                string selectedGNummer = ComboBoxGNummer.SelectedItem.ToString();
                query += " AND [G_Nummer] = @GNummer";
            }

            // Überprüfen, ob eine Glasssorte ausgewählt wurde
            if (ComboBoxGlassorte.SelectedItem != null)
            {
                string selectedGlasssorte = ComboBoxGlassorte.SelectedItem.ToString();
                query += " AND GLASSORTE = @Glasssorte";
            }

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Parameter hinzufügen, wenn ein Artikel ausgewählt wurde
                    if (ComboBoxArtikel.SelectedItem != null)
                    {
                        // Artikelnummer extrahieren und das letzte Segment ignorieren, wenn es eine Zahl ist
                        string selectedArtikel = ComboBoxArtikel.SelectedItem.ToString();
                        string[] artikelParts = selectedArtikel.Split('-');
                        if (artikelParts.Length > 1 && int.TryParse(artikelParts.Last(), out _))
                        {
                            selectedArtikel = string.Join("-", artikelParts.Take(artikelParts.Length - 1));
                        }
                        sqlCommand.Parameters.AddWithValue("@Artikel", selectedArtikel);
                    }

                    // Parameter hinzufügen, wenn eine GNummer ausgewählt wurde
                    if (ComboBoxGNummer.SelectedItem != null)
                    {
                        string selectedGNummer = ComboBoxGNummer.SelectedItem.ToString();
                        sqlCommand.Parameters.AddWithValue("@GNummer", selectedGNummer);
                    }

                    // Parameter hinzufügen, wenn eine Glasssorte ausgewählt wurde
                    if (ComboBoxGlassorte.SelectedItem != null)
                    {
                        string selectedGlasssorte = ComboBoxGlassorte.SelectedItem.ToString();
                        sqlCommand.Parameters.AddWithValue("@Glasssorte", selectedGlasssorte);
                    }

                    // Daten in ein DataTable laden
                    DataTable dataTable = new DataTable();
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    // DataGridView mit den Daten füllen
                    DgvAuswahlGlasdaten.DataSource = dataTable;

                    // Header-Schrift mittig ausrichten
                    foreach (DataGridViewColumn column in DgvAuswahlGlasdaten.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Folgende Reihen ausblenden
                    int[] columnsToHide = { 0, 1, 2, 4, 5, 6, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 28, 30, 31, 
                        36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 55, 57 };
                    foreach (int index in columnsToHide)
                    {
                        if (index < DgvAuswahlGlasdaten.Columns.Count)
                        {
                            DgvAuswahlGlasdaten.Columns[index].Visible = false;
                        }
                    }
                    // Absteigend nach Artikelnummer sortieren
                    DgvAuswahlGlasdaten.Sort(DgvAuswahlGlasdaten.Columns["ARTNR"], ListSortDirection.Descending);
                    // Dgv an die eingestellte Größe anpassen
                    DgvAuswahlGlasdaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Daten: " + ex.Message);
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

        // Methode zum Aktualisieren der Labels mit der Anzahl der einzigartigen GNummern und Glassorten
        private void UpdateLabels()
        {
            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Abfrage zum Zählen der einzigartigen GNummern
                string queryGNummern = "SELECT COUNT(DISTINCT [G_Nummer]) FROM Serienlinsen";
                using (SqlCommand sqlCommand = new SqlCommand(queryGNummern, sqlConnectionVerwaltung))
                {
                    int anzahlGNummern = (int)sqlCommand.ExecuteScalar();
                    LblAnzahlGNummern.Text = $"Anzahl G-Nummern: {anzahlGNummern}";
                }

                // SQL-Abfrage zum Zählen der einzigartigen Glassorten
                string queryGlassorten = "SELECT COUNT(DISTINCT GLASSORTE) FROM Serienlinsen";
                using (SqlCommand sqlCommand = new SqlCommand(queryGlassorten, sqlConnectionVerwaltung))
                {
                    int anzahlGlassorten = (int)sqlCommand.ExecuteScalar();
                    LblAnzahlGlassorten.Text = $"Anzahl Glassorten: {anzahlGlassorten}";
                }
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Aktualisieren der Labels: " + ex.Message);
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

    }
}

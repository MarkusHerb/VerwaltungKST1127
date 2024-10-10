using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic;
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Drawing.Printing;
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    public partial class Form_PrototypenauftragErstellen : Form
    {
        // Verbindungszeichenfolgen für die SQL Server-Datenbanken
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private PrintDocument printDocument;  // Deklarieren eines PrintDocument-Objekts für den Druckprozess

        public Form_PrototypenauftragErstellen()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Entfernt die Titelleiste und die Rahmen des Formulars
            UpdateZeitDatum();
            FillComboBoxArtikel();
            // Initialisieren des PrintDocument-Objekts und Registrieren des PrintPage-Event-Handlers
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);  // Event-Handler für die Druckseite hinzufügen
            // Querformat für das PrintDocument festlegen
            printDocument.DefaultPageSettings.Landscape = true;  // Querformat aktivieren
        }

        // Methode zum Füllen der ComboBox mit Artikelnummern und Seiteninformationen
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
                        ComboboxArtikel.Items.Clear();

                        // Durchlaufen der Ergebnisse aus der Abfrage
                        while (reader.Read())
                        {
                            // Abrufen der Werte für Artikelnummer und Seite aus dem aktuellen Datensatz
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();

                            // Zusammenfügen der Artikelnummer und der Seite zu einem Anzeigeformat, z.B. "12345 - links"
                            string displayValue = $"{artikelNummerValue} / Seite: {seiteValue}";

                            // Hinzufügen des Anzeigeformats zur ComboBox
                            ComboboxArtikel.Items.Add(displayValue);
                        }
                    }
                }

                // Sicherstellen, dass die ComboBox nicht automatisch sortiert wird, da dies durch die SQL-Abfrage gesteuert wird
                ComboboxArtikel.Sorted = false;
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


        // Uhrzeit und Datumsfunktion
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now; //Aktuelles Datum und Uhrzeit abrufen
            LblErstelltAm.Text = aktuell.ToString("D"); // Datum und Uhrzeit im zugeweisenen Label anzeigen
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Erstellen eines Bitmaps, das nur den Clientbereich des Formulars widerspiegelt (ohne Rahmen und Titelleiste)
            Bitmap bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            this.DrawToBitmap(bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height)); // Nur den Clientbereich auf das Bitmap rendern

            // Berechnung der Druckbreite und Druckhöhe ohne Rand
            int printWidth = e.PageBounds.Width;
            int printHeight = e.PageBounds.Height;

            // Bild proportional skalieren
            float ratioX = (float)printWidth / (float)bmp.Width;
            float ratioY = (float)printHeight / (float)bmp.Height;
            float ratio = Math.Min(ratioX, ratioY);

            // Neue Breite und Höhe für das skalierte Bild berechnen
            int scaledWidth = (int)(bmp.Width * ratio);
            int scaledHeight = (int)(bmp.Height * ratio);

            // Zentrierung des Bildes auf der Druckseite
            int posX = (printWidth - scaledWidth) / 2;   // Berechnung der X-Position für zentriertes Bild
            int posY = (printHeight - scaledHeight) / 2; // Berechnung der Y-Position für zentriertes Bild

            // Das Bild proportional und zentriert auf der Druckseite platzieren
            e.Graphics.DrawImage(bmp, posX, posY, scaledWidth, scaledHeight);
        }

        // Wenn auf den Button Drucken gedrückt wird
        private void BtnDrucken_Click_1(object sender, EventArgs e)
        {
            string artikelnummer = ComboboxArtikel.Text.ToString();
            ComboboxArtikel.Text = "PR " + artikelnummer;
            BtnClose.Visible = false;
            BtnDrucken.Visible = false;
            // PrintDialog anzeigen, um den Benutzer einen Drucker auswählen zu lassen
            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument // Zuweisen des PrintDocuments zum PrintDialog
            }; // Erstellen eines PrintDialogs

            // Überprüfen, ob der Benutzer im PrintDialog auf "OK" klickt
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Diagramm drucken
                printDocument.Print(); // Starten des Druckprozesses
            }
        }

        // Druckformular beenden
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Wenn aus der Combobox etwas ausgewählt wird
        private void ComboboxArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    
}

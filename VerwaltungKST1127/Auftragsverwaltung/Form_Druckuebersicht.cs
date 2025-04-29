using Newtonsoft.Json;
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic; // Importieren des System.Collections.Generic-Namespace für die Arbeit mit generischen Sammlungen (z.B. List<T>, Dictionary<TKey, TValue>)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Drawing.Imaging; // Importieren des System.Drawing.Imaging-Namespace für die Arbeit mit Bildformaten und -konvertierungen
using System.Runtime.InteropServices; // Importieren des System.Runtime.InteropServices-Namespace für die Interoperabilität zwischen verwalteten und nicht verwalteten Codes
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Windows-Formularanwendungen
using ZXing; // Importieren des ZXing-Namespace für die Erstellung von Barcodes
using Excel = Microsoft.Office.Interop.Excel; // Importieren des Microsoft.Office.Interop.Excel-Namespace für die Arbeit mit Excel-Dateien
using System.Text.Json;
using System.IO;

namespace VerwaltungKST1127.Auftragsverwaltung
{
    public partial class Form_Druckuebersicht : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank Verwaltung
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // Klassenvariablen zum Speichern der übergebenen Werte
        private string enddatum;
        private string teilebez;
        private string auftragsNr;
        private string artikel;
        private string status;
        private string avoInfo;
        private string material; 
        private string seite;
        private string sollStk;
        private string istStk;
        private string vorStk;
        private string teilelager;
        private string bereitstell;
        private string jahresbedarf;
        private string zukauf;
        private string dringend;
        private string aktualisiert;
        private string selectedBelagValue;

        // Konstruktor der Form_Druckuebersicht-Klasse, der die übergebenen Werte initialisiert und die Labels aktualisiert
        public Form_Druckuebersicht(string enddatum, string teilebez, string auftragsNr, string artikel, string status, string avoInfo, string material, string seite,
            string sollStk, string istStk, string vorStk, string teilelager, string bereitstell, string jahresbedarf, string zukauf, string dringend, string aktualisiert,
            string selectedBelagValue)
        {
            InitializeComponent(); // Initialisiert die Komponenten der Form

            // Werte in die Klassenvariablen speichern
            this.enddatum = enddatum;
            this.teilebez = teilebez;
            this.auftragsNr = auftragsNr;
            this.artikel = artikel;
            this.status = status;
            this.avoInfo = avoInfo;
            this.material = material;
            this.seite = seite;
            this.sollStk = sollStk;
            this.istStk = istStk;
            this.vorStk = vorStk;
            this.teilelager = teilelager;
            this.bereitstell = bereitstell;
            this.jahresbedarf = jahresbedarf;
            this.zukauf = zukauf;
            this.dringend = dringend;
            this.aktualisiert = aktualisiert;
            this.selectedBelagValue = selectedBelagValue;

            // ########### Werte in die Labels schreiben
            // Auftragsnummer, Artikel und Teilebezeichnung
            lblAuftragNummerBauteilname.Text = auftragsNr + " - " + artikel + " - " + teilebez;
            // Sollstück
            lblSollstueck.Text = "Sollstück: " + sollStk + "Stk.";
            // Zukauf
            if (zukauf == "R-Lager")
            {
                lblZukauf.Text = "Zukauf (Rohteilelager)";
                lblZukauf.ForeColor = Color.DarkOliveGreen;
            }
            else if (zukauf == "T-Lager")
            {
                lblZukauf.Text = "Zukauf (Teilelager)";
                lblZukauf.ForeColor = Color.DarkOrange;
            }
            else
            {
                lblZukauf.Text = "Eigenfertigung";
            }
            // Belag + Prozess 
            lblProzess.Text = selectedBelagValue + " - " + material;
            lblProzess.ForeColor = Color.Red;

            // Aktualisiert die Labels mit den aktuellen Werten
            UpdateLabels();
        }

        // Methode zum Aktualisieren der Labels mit den aktuellen Werten aus der Datenbank
        private void UpdateLabels()
        {
            try
            {
                // SQL-Abfrage, um die Avoinfo, opno_avonr, refo_avonr aus der Tabelle zu holen
                string query = @"
                    SELECT txta_Avoinfo, opno_avonr, refo_avonr
                    FROM LN_ProdOrders_PRD
                    WHERE pdno_prodnr = @Auftragsnummer";

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@Auftragsnummer", auftragsNr); // Füge den Auftragsnummer-Wert als Parameter hinzu

                    sqlConnectionVerwaltung.Open(); // Öffnet die Verbindung zur Datenbank
                    using (SqlDataReader reader = command.ExecuteReader()) // Führt die Abfrage aus und liest die Ergebnisse
                    {
                        // Listen zum Speichern der Werte aus der Datenbank
                        List<string> vorbereitenList = new List<string>();
                        List<string> verguetenList = new List<string>();
                        List<string> positionVorbereitenList = new List<string>();
                        List<string> positionVerguetenList = new List<string>();
                        List<string> avoNrVorbereitenList = new List<string>();
                        List<string> avoNrVerguetenList = new List<string>();

                        // Liest die Ergebnisse der Abfrage und speichert sie in den Listen
                        while (reader.Read())
                        {
                            string avoInfo = reader["txta_Avoinfo"].ToString();
                            string position = reader["opno_avonr"].ToString();
                            string avoNr = reader["refo_avonr"].ToString();

                            if (avoInfo.Contains("Vorbereiten") || avoInfo.Contains("vorbereiten"))
                            {
                                vorbereitenList.Add(avoInfo);
                                positionVorbereitenList.Add(position);
                                avoNrVorbereitenList.Add(avoNr);
                            }

                            if (avoInfo.Contains("Vergüten") || avoInfo.Contains("vergüten"))
                            {
                                verguetenList.Add(avoInfo);
                                positionVerguetenList.Add(position);
                                avoNrVerguetenList.Add(avoNr);
                            }
                        }

                        // Arrays zum Speichern der ersten vier Werte aus den Listen
                        string[] vorbereiten = new string[4];
                        string[] vergueten = new string[4];
                        string[] positionVorbereiten = new string[4];
                        string[] positionVergueten = new string[4];
                        string[] avoNrVorbereiten = new string[4];
                        string[] avoNrVergueten = new string[4];

                        // Füllen der Arrays mit den gesammelten Werten
                        for (int i = 0; i < vorbereitenList.Count && i < 4; i++)
                        {
                            vorbereiten[i] = vorbereitenList[i];
                            positionVorbereiten[i] = positionVorbereitenList[i];
                            avoNrVorbereiten[i] = avoNrVorbereitenList[i];
                        }

                        for (int i = 0; i < verguetenList.Count && i < 4; i++)
                        {
                            vergueten[i] = verguetenList[i];
                            positionVergueten[i] = positionVerguetenList[i];
                            avoNrVergueten[i] = avoNrVerguetenList[i];
                        }

                        // Aktualisieren der Labels mit den Werten aus den Arrays
                        lblArbeitsvorgangVorbereiten1.Text = vorbereiten[0] ?? "";
                        lblArbeitsvorgangVorbereiten2.Text = vorbereiten[1] ?? "";
                        lblArbeitsvorgangVorbereiten3.Text = vorbereiten[2] ?? "";
                        lblArbeitsvorgangVorbereiten4.Text = vorbereiten[3] ?? "";

                        lblArbeitsvorgangVergueten1.Text = vergueten[0] ?? "";
                        lblArbeitsvorgangVergueten2.Text = vergueten[1] ?? "";
                        lblArbeitsvorgangVergueten3.Text = vergueten[2] ?? "";
                        lblArbeitsvorgangVergueten4.Text = vergueten[3] ?? "";

                        lblPositionVorbereiten1.Text = positionVorbereiten[0] ?? "";
                        lblPositionVorbereiten2.Text = positionVorbereiten[1] ?? "";
                        lblPositionVorbereiten3.Text = positionVorbereiten[2] ?? "";
                        lblPositionVorbereiten4.Text = positionVorbereiten[3] ?? "";

                        lblPositionVergueten1.Text = positionVergueten[0] ?? "";
                        lblPositionVergueten2.Text = positionVergueten[1] ?? "";
                        lblPositionVergueten3.Text = positionVergueten[2] ?? "";
                        lblPositionVergueten4.Text = positionVergueten[3] ?? "";

                        lblAvoNrZuVorbereiten1.Text = avoNrVorbereiten[0] ?? "";
                        lblAvoNrZuVorbereiten2.Text = avoNrVorbereiten[1] ?? "";
                        lblAvoNrZuVorbereiten3.Text = avoNrVorbereiten[2] ?? "";
                        lblAvoNrZuVorbereiten4.Text = avoNrVorbereiten[3] ?? "";

                        lblAvoNrZuVergueten1.Text = avoNrVergueten[0] ?? "";
                        lblAvoNrZuVergueten2.Text = avoNrVergueten[1] ?? "";
                        lblAvoNrZuVergueten3.Text = avoNrVergueten[2] ?? "";
                        lblAvoNrZuVergueten4.Text = avoNrVergueten[3] ?? "";

                        // Strichcode Labels setzen, wenn lblPositionVorbereiten und lblPositionVergueten nicht leer sind
                        // Wenn das Position-Label leer ist, wird das Strichcode-Label ebenfalls leer gesetzt

                        // Für PositionVorbereiten
                        lblStrichcodeZuPositionVorbereiten1.Text = string.IsNullOrEmpty(lblPositionVorbereiten1.Text) ? "" : $"{auftragsNr}_{lblPositionVorbereiten1.Text}";
                        lblStrichcodeZuPositionVorbereiten2.Text = string.IsNullOrEmpty(lblPositionVorbereiten2.Text) ? "" : $"{auftragsNr}_{lblPositionVorbereiten2.Text}";
                        lblStrichcodeZuPositionVorbereiten3.Text = string.IsNullOrEmpty(lblPositionVorbereiten3.Text) ? "" : $"{auftragsNr}_{lblPositionVorbereiten3.Text}";
                        lblStrichcodeZuPositionVorbereiten4.Text = string.IsNullOrEmpty(lblPositionVorbereiten4.Text) ? "" : $"{auftragsNr}_{lblPositionVorbereiten4.Text}";

                        // Für PositionVergueten
                        lblStrichcodeZuPositionVergueten1.Text = string.IsNullOrEmpty(lblPositionVergueten1.Text) ? "" : $"{auftragsNr}_{lblPositionVergueten1.Text}";
                        lblStrichcodeZuPositionVergueten2.Text = string.IsNullOrEmpty(lblPositionVergueten2.Text) ? "" : $"{auftragsNr}_{lblPositionVergueten2.Text}";
                        lblStrichcodeZuPositionVergueten3.Text = string.IsNullOrEmpty(lblPositionVergueten3.Text) ? "" : $"{auftragsNr}_{lblPositionVergueten3.Text}";
                        lblStrichcodeZuPositionVergueten4.Text = string.IsNullOrEmpty(lblPositionVergueten4.Text) ? "" : $"{auftragsNr}_{lblPositionVergueten4.Text}";
                    }
                    sqlConnectionVerwaltung.Close(); // Schließt die Verbindung zur Datenbank
                }

                // Holen der SEITE-Werte für jedes Label (AvoNrZuVergueten1 bis 4)
                UpdateSeiteLabels();
            }
            catch (Exception ex)
            {
                // Zeigt eine Fehlermeldung an, wenn ein Fehler auftritt
                MessageBox.Show($"Fehler beim Abrufen der Daten: {ex.Message}");
            }
            finally
            {
                // Stellt sicher, dass die Verbindung geschlossen wird, selbst bei einem Fehler
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Methode, die die SEITE für jedes AvoNr in den Labels aktualisiert
        private void UpdateSeiteLabels()
        {
            try
            {
                // Nur die Labels aktualisieren, wenn der Wert im Label nicht leer ist
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten1.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten1.Text, lblSeite1);
                }
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten2.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten2.Text, lblSeite2);
                }
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten3.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten3.Text, lblSeite3);
                }
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten4.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten4.Text, lblSeite4);
                }
            }
            catch (Exception ex)
            {
                // Zeigt eine Fehlermeldung an, wenn ein Fehler auftritt
                MessageBox.Show($"Fehler beim Aktualisieren der SEITE-Daten: {ex.Message}");
            }
        }

        // Methode, die die SEITE für ein spezifisches Label basierend auf AvoNr und Artikel aktualisiert
        private void UpdateSeiteFürLabel(string avoNr, Label lblSeite)
        {
            if (string.IsNullOrEmpty(avoNr) || string.IsNullOrEmpty(artikel))
            {
                lblSeite.Text = ""; // Falls der AvoNr- oder Artikel-Wert leer oder null ist, setze das Label leer
                return;
            }

            try
            {
                // SQL-Abfrage um die SEITE für den gegebenen refo_avonr und ARTNR zu holen
                string query = @"
            SELECT SEITE
            FROM Serienlinsen
            WHERE refo_avonr = @AvoNr AND ARTNR = @Artikel";

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@AvoNr", avoNr);  // Füge den AvoNr-Wert als Parameter hinzu
                    command.Parameters.AddWithValue("@Artikel", artikel);  // Füge den Artikel-Wert als Parameter hinzu

                    sqlConnectionVerwaltung.Open(); // Verbindung öffnen
                    object result = command.ExecuteScalar(); // Einzeln das Ergebnis holen (SEITE)

                    if (result != DBNull.Value && result != null)
                    {
                        // Falls ein Wert gefunden wurde, setze das Label
                        lblSeite.Text = result.ToString();
                    }
                    else
                    {
                        // Falls kein Wert gefunden wurde, setze das Label auf "leer"
                        lblSeite.Text = ""; // Das Label bleibt leer
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, wenn ein Fehler auftritt
                MessageBox.Show($"Fehler beim Abrufen der SEITE-Daten für AvoNr {avoNr} und Artikel {artikel}: {ex.Message}");
            }
            finally
            {
                // Stelle sicher, dass die Verbindung geschlossen wird, selbst bei einem Fehler
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Event-Handler für den Klick auf den Druck-Auftragszettel-Button
        private void btnDruckAuftragszettel_Click(object sender, EventArgs e)
        {
            // Pfad zur Excel-Datei
            string excelFilePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Auftragszettel.xlsx";

            // Excel-Anwendung und Arbeitsmappe initialisieren
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = null;

            // Liste der geänderten Sheets
            List<string> modifiedSheets = new List<string>();

            try
            {
                // Excel-Anwendung starten und Arbeitsmappe öffnen
                workbook = excelApp.Workbooks.Open(excelFilePath);

                // Funktion zum Schreiben der Auftragsnummer, Artikelnummer, Teilenummer, Sollstückzahl, Labelwert, Enddatum und Serienlinsen-Daten in die angegebenen Zellen
                void WriteToSheet(string sheetName, string auftragsNr, string artikel, string teilebez, string sollStk, string labelWert, string enddatum, Dictionary<string, string> serienlinsenDaten)
                {
                    Excel.Worksheet worksheet = workbook.Sheets[sheetName];
                    worksheet.Cells[2, 6].Value = auftragsNr; // Zelle F2
                    worksheet.Cells[2, 2].NumberFormat = "@"; // Format der Zelle auf Text setzen
                    worksheet.Cells[2, 2].Value = artikel; // Zelle B2
                    worksheet.Cells[2, 10].Value = teilebez; // Zelle J2
                    worksheet.Cells[4, 13].Value = sollStk; // Zelle M4
                    worksheet.Cells[4, 7].Value = labelWert; // Zelle G4
                    worksheet.Cells[5, 7].Value = enddatum; // Zelle G5

                    if (serienlinsenDaten != null)
                    {
                        worksheet.Cells[4, 3].Value = serienlinsenDaten["Belag1"]; // Zelle C4
                        worksheet.Cells[5, 3].Value = serienlinsenDaten["Material"]; // Zelle C5
                        worksheet.Cells[9, 9].Value = serienlinsenDaten["Radius2"]; // Zelle I9
                        worksheet.Cells[17, 9].Value = serienlinsenDaten["Radius1"]; // Zelle I17
                        worksheet.Cells[20, 11].Value = serienlinsenDaten["ND"]; // Zelle K20
                        worksheet.Cells[21, 11].Value = serienlinsenDaten["G_Nummer"]; // Zelle K21
                        worksheet.Cells[22, 11].Value = serienlinsenDaten["GLASSORTE"]; // Zelle K22
                        worksheet.Cells[23, 11].Value = serienlinsenDaten["DM"]; // Zelle K23
                        worksheet.Cells[24, 11].Value = serienlinsenDaten["DICKE"]; // Zelle K24
                        worksheet.Cells[25, 10].Value = serienlinsenDaten["InfoZeichnung_Bemerkungen"]; // Zelle J26

                        // Bild einfügen Auflegeinformation (Zeichnungspfad)
                        string bildPfad1 = serienlinsenDaten["Zeichnungspfad"]; // Pfad zum Bild
                        if (!string.IsNullOrEmpty(bildPfad1) && System.IO.File.Exists(bildPfad1)) // Überprüfen, ob das Bild existiert
                        {
                            Excel.Range oRange = worksheet.get_Range("I10", "K16"); // Bereich für das Bild

                            // Löschen vorhandener Bilder im Bereich
                            List<Excel.Shape> shapesToDelete = new List<Excel.Shape>();
                            foreach (Excel.Shape shape in worksheet.Shapes)
                            {
                                if (shape.TopLeftCell.Row >= oRange.Row && shape.TopLeftCell.Column >= oRange.Column &&
                                    shape.TopLeftCell.Row <= oRange.Row + oRange.Rows.Count - 1 &&
                                    shape.TopLeftCell.Column <= oRange.Column + oRange.Columns.Count - 1)
                                {
                                    shapesToDelete.Add(shape);
                                }
                            }

                            foreach (Excel.Shape shape in shapesToDelete)
                            {
                                shape.Delete();
                            }

                            // Neues Bild einfügen
                            float left = (float)((double)oRange.Left); // Position des Bereichs
                            float top = (float)((double)oRange.Top); // Position des Bereichs
                            float width = (float)((double)oRange.Width); // Breite des Bereichs
                            float height = (float)((double)oRange.Height); // Höhe des Bereichs
                            worksheet.Shapes.AddPicture(bildPfad1, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, left, top, width, height); // Bild einfügen
                        }

                        // Bild einfügen Info Zeichnung
                        string bildPfad2 = serienlinsenDaten["InfoZeichnung"]; // Pfad zum Bild
                        if (!string.IsNullOrEmpty(bildPfad2) && System.IO.File.Exists(bildPfad2)) // Überprüfen, ob das Bild existiert
                        {
                            Excel.Range oRange = worksheet.get_Range("M10", "N16"); // Bereich für das Bild

                            // Löschen vorhandener Bilder im Bereich
                            List<Excel.Shape> shapesToDelete = new List<Excel.Shape>();
                            foreach (Excel.Shape shape in worksheet.Shapes)
                            {
                                if (shape.TopLeftCell.Row >= oRange.Row && shape.TopLeftCell.Column >= oRange.Column &&
                                    shape.TopLeftCell.Row <= oRange.Row + oRange.Rows.Count - 1 &&
                                    shape.TopLeftCell.Column <= oRange.Column + oRange.Columns.Count - 1)
                                {
                                    shapesToDelete.Add(shape);
                                }
                            }

                            foreach (Excel.Shape shape in shapesToDelete)
                            {
                                shape.Delete();
                            }

                            // Neues Bild einfügen
                            float left = (float)((double)oRange.Left); // Position des Bereichs
                            float top = (float)((double)oRange.Top); // Position des Bereichs
                            float width = (float)((double)oRange.Width); // Breite des Bereichs
                            float height = (float)((double)oRange.Height); // Höhe des Bereichs
                            worksheet.Shapes.AddPicture(bildPfad2, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, left, top, width, height); // Bild einfügen
                        }
                        // Barcode in Zelle C27 einfügen
                        string[] barcodeTexts = new string[]
                        {
                            lblStrichcodeZuPositionVergueten1.Text,
                            lblStrichcodeZuPositionVergueten2.Text,
                            lblStrichcodeZuPositionVergueten3.Text,
                            lblStrichcodeZuPositionVergueten4.Text
                        };

                        string[] sheetNames = new string[] { "Seite1", "Seite2", "Seite3", "Seite4" };

                        // Barcode für jede Zelle erstellen
                        for (int i = 0; i < barcodeTexts.Length; i++)
                        {
                            // Wenn der Barcode-Text nicht leer ist
                            if (!string.IsNullOrEmpty(barcodeTexts[i]))
                            {
                                Image barcodeImage = GenerateBarcode(barcodeTexts[i]); // Barcode-Bild generieren
                                string tempPath = System.IO.Path.GetTempFileName(); // Temporäre Datei erstellen
                                barcodeImage.Save(tempPath, ImageFormat.Png); // Barcode-Bild speichern
                                Excel.Worksheet currentWorksheet = workbook.Sheets[sheetNames[i]]; // Aktuelles Arbeitsblatt
                                Excel.Range barcodeRange = currentWorksheet.get_Range("C27"); // Zelle C27

                                // Löschen vorhandener Bilder in der Zelle
                                List<Excel.Shape> shapesToDelete = new List<Excel.Shape>();
                                foreach (Excel.Shape shape in currentWorksheet.Shapes)
                                {
                                    if (shape.TopLeftCell.Address == barcodeRange.Address)
                                    {
                                        shapesToDelete.Add(shape); // Shape zur Liste hinzufügen
                                    }
                                }

                                foreach (Excel.Shape shape in shapesToDelete)
                                {
                                    shape.Delete();
                                }

                                // Neues Bild einfügen
                                float barcodeLeft = (float)((double)barcodeRange.Left); // Linke Position der Zelle
                                float barcodeTop = (float)((double)barcodeRange.Top); // Obere Position der Zelle
                                currentWorksheet.Shapes.AddPicture(tempPath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, barcodeLeft, barcodeTop, barcodeImage.Width, barcodeImage.Height); // Barcode-Bild einfügen

                                System.IO.File.Delete(tempPath); // Temporäre Datei löschen
                                Marshal.ReleaseComObject(currentWorksheet); // Freigeben des Arbeitsblatts

                                // Sheet als geändert markieren
                                if (!modifiedSheets.Contains(sheetNames[i]))
                                {
                                    modifiedSheets.Add(sheetNames[i]);
                                }
                            }
                        }
                    }

                    Marshal.ReleaseComObject(worksheet);

                    // Auftragsnummer in JSON-Datei speichern
                    FügeAuftragsnummerZurJsonDateiHinzu(auftragsNr);
                }


                // Funktion zum Abrufen des Enddatums aus der SQL-Tabelle
                string GetEnddatum(string auftragsNr, string arbeitsvorgang)
                {
                    string query = @"
                SELECT trdf_enddate
                FROM LN_ProdOrders_PRD
                WHERE pdno_prodnr = @Auftragsnummer AND txta_Avoinfo = @Arbeitsvorgang";

                    using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        command.Parameters.AddWithValue("@Auftragsnummer", auftragsNr); // Füge den Auftragsnummer-Wert als Parameter hinzu
                        command.Parameters.AddWithValue("@Arbeitsvorgang", arbeitsvorgang); // Füge den Arbeitsvorgang-Wert als Parameter hinzu

                        sqlConnectionVerwaltung.Open();
                        object result = command.ExecuteScalar(); // Einzeln das Ergebnis holen (Enddatum)
                        sqlConnectionVerwaltung.Close();
                        // Wenn das Ergebnis nicht null oder DBNull ist, wird es als String zurückgegeben
                        if (result != DBNull.Value && result != null)
                        {
                            return result.ToString();
                        }
                        return string.Empty;
                    }
                }

                // Funktion zum Abrufen der Serienlinsen-Daten aus der SQL-Tabelle
                Dictionary<string, string> GetSerienlinsenDaten(string artikel, string seite)
                {
                    string query = @"
                        SELECT Belag1, Material, Radius2, Radius1, ND, G_Nummer, GLASSORTE, DM, DICKE, InfoZeichnung_Bemerkungen, InfoZeichnung, Zeichnungspfad
                        FROM Serienlinsen
                        WHERE ARTNR = @Artikel AND SEITE = @Seite";

                    using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        command.Parameters.AddWithValue("@Artikel", artikel);
                        command.Parameters.AddWithValue("@Seite", seite);

                        sqlConnectionVerwaltung.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var daten = new Dictionary<string, string>
                {
                    { "Belag1", reader["Belag1"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "Material", reader["Material"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "Radius2", reader["Radius2"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "Radius1", reader["Radius1"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "ND", reader["ND"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "G_Nummer", reader["G_Nummer"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "GLASSORTE", reader["GLASSORTE"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "DM", reader["DM"].ToString().Replace(',', '.') }, // Füge die Werte in das Dictionary ein
                    { "DICKE", reader["DICKE"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "InfoZeichnung_Bemerkungen", reader["InfoZeichnung_Bemerkungen"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "InfoZeichnung", reader["InfoZeichnung"].ToString() }, // Füge die Werte in das Dictionary ein
                    { "Zeichnungspfad", reader["Zeichnungspfad"].ToString() } // Füge die Werte in das Dictionary ein
                };
                                sqlConnectionVerwaltung.Close();
                                return daten;
                            }
                        }
                        sqlConnectionVerwaltung.Close();
                        return null;
                    }
                }

                // Auftragsnummer, Artikelnummer, Teilenummer, Sollstückzahl, Labelwert, Enddatum und Serienlinsen-Daten in die entsprechenden Sheets schreiben, wenn die Labels Werte haben
                if (!string.IsNullOrEmpty(lblSeite1.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten1.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite1.Text);
                    WriteToSheet("Seite1", auftragsNr, artikel, teilebez, sollStk, lblSeite1.Text, enddatum, serienlinsenDaten);
                }
                if (!string.IsNullOrEmpty(lblSeite2.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten2.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite2.Text);
                    WriteToSheet("Seite2", auftragsNr, artikel, teilebez, sollStk, lblSeite2.Text, enddatum, serienlinsenDaten);
                }
                if (!string.IsNullOrEmpty(lblSeite3.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten3.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite3.Text);
                    WriteToSheet("Seite3", auftragsNr, artikel, teilebez, sollStk, lblSeite3.Text, enddatum, serienlinsenDaten);
                }
                if (!string.IsNullOrEmpty(lblSeite4.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten4.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite4.Text);
                    WriteToSheet("Seite4", auftragsNr, artikel, teilebez, sollStk, lblSeite4.Text, enddatum, serienlinsenDaten);
                }

                // Änderungen speichern und Arbeitsmappe schließen
                workbook.Save();

                ////// Geänderte Sheets drucken und nur die erste Seite davon
                ////foreach (string sheetName in modifiedSheets)
                ////{
                ////    Excel.Worksheet worksheet = workbook.Sheets[sheetName];
                ////    worksheet.PrintOutEx(1, 1, 1, false);
                ////}
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, falls ein Fehler auftritt
                MessageBox.Show($"Fehler beim Schreiben in die Excel-Datei: {ex.Message}");
            }
            finally
            {
                // Arbeitsmappe und Excel-Anwendung schließen
                if (workbook != null)
                {
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }
            }
        }

        // Funktion um die Auftragsnummer in der JsonDatei zu speichern
        private void FügeAuftragsnummerZurJsonDateiHinzu(string neueAuftragsnummer)
        {
            string jsonPfad = "letzteAuftragsnummern.json"; // Datei im aktuellen Verzeichnis

            try
            {
                List<string> auftragsnummern = new List<string>();

                if (File.Exists(jsonPfad))
                {
                    string vorhandenerJsonInhalt = File.ReadAllText(jsonPfad);
                    if (!string.IsNullOrWhiteSpace(vorhandenerJsonInhalt))
                    {
                        auftragsnummern = System.Text.Json.JsonSerializer.Deserialize<List<string>>(vorhandenerJsonInhalt) ?? new List<string>();
                    }
                }

                if (!auftragsnummern.Contains(neueAuftragsnummer))
                {
                    auftragsnummern.Add(neueAuftragsnummer);
                    string neuerJsonInhalt = System.Text.Json.JsonSerializer.Serialize(auftragsnummern, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(jsonPfad, neuerJsonInhalt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Schreiben der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Funktion zum Generieren eines Barcodes
        private Image GenerateBarcode(string text)
        {
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128, // Barcode-Format
                Options = new ZXing.Common.EncodingOptions 
                {
                    Height = 32, // Höhe des Barcodes
                    Width = 150 // Breite des Barcodes
                }
            };
            return barcodeWriter.Write(text); // Barcode-Bild generieren
        }
    }
}

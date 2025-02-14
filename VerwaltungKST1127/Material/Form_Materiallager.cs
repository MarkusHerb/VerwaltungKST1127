using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer und grundlegende Ausnahmen)
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für Komponenten und Ereignismodelle (z.B. für das Arbeiten mit Entwurfsmustern und benachrichtigungsfähigen Eigenschaften in Formularanwendungen)
using System.Data; // Importieren des System.Data-Namespace für Datenoperationen und die Arbeit mit Datenquellen (z.B. DataTable, DataSet und andere ADO.NET-Funktionalitäten)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für den Zugriff auf SQL Server-Datenbanken (z.B. zum Arbeiten mit SQL-Verbindungen, Befehlen und Datenlesern in ADO.NET)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Farben, Bilder, Schriften und andere grafische Ressourcen)
using System.IO; // Importieren des System.IO-Namespace für Dateioperationen und Stream-E/A (z.B. zum Lesen und Schreiben von Dateien, Arbeiten mit Verzeichnissen)
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel; // Importieren des Microsoft.Office.Interop.Excel-Namespace für den Zugriff auf Excel-Objekte und -Funktionen (z.B. Excel-Anwendungen, Arbeitsmappen, Tabellen)
using VerwaltungKST1127.Material; // Importieren des System.Windows.Forms-Namespace für Windows Forms-Steuerelemente und Benutzeroberflächen (z.B. Button, TextBox, Label für GUI-Entwicklung)


namespace VerwaltungKST1127
{
    public partial class Form_Materiallager : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank
        private readonly SqlConnection sqlConnection = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False");

        // Variable für die aktuelle ID im DataGridView
        int currentId;

        public Form_Materiallager()
        {
            InitializeComponent();
            // Aktualisieren des DataGridView
            UpdateDgvMateriallager();
            // Sichtbarkeit festlegen
            BtnSpeichern.Visible = false;
            //BtnLoeschen.Visible = false;
        }

        // Methode zur Aktualisierung und Formatierung des DataGridView für das Materiallager
        public void UpdateDgvMateriallager()
        {
            try
            {
                sqlConnection.Open(); // Verbindung zur Datenbank öffnen
                string query = "SELECT * FROM MaterialLager"; // SQL-Abfrage für die Daten
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection); // SQLDataAdapter für die Datenbankabfrage
                DataSet dataSet = new DataSet(); // Neues DataSet erstellen
                sqlDataAdapter.Fill(dataSet); // Daten in das DataSet einfügen
                DgvMateriallager.DataSource = dataSet.Tables[0]; // Datenquelle für das DataGridView festlegen
                DgvMateriallager.Sort(DgvMateriallager.Columns[1], ListSortDirection.Ascending); // DataGridView nach Kategorie sortieren
                // DataGridView-Formatierung: Header-Schriftart und Ausrichtung
                DgvMateriallager.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(DataGridView.DefaultFont, FontStyle.Bold);
                DgvMateriallager.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // Spaltenbreite des Dgvs festlegen
                DgvMateriallager.Columns[0].Width = 30;
                DgvMateriallager.Columns[2].Width = 220;
                DgvMateriallager.Columns[7].Width = 445;

                // Überprüfen, ob Lagerbestand den Mindestbestand unterschreitet und Datenbank aktualisieren
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    int lagerstand = Convert.ToInt32(row["Lagerstand"]);
                    int mindestbestand = Convert.ToInt32(row["Mindestbestand"]);

                    if (lagerstand <= mindestbestand) //
                    {
                        row["BestellStatus"] = "Bestellen";
                        // SQL-Update-Befehl für "Bestellen"
                        string updateQuery = "UPDATE MaterialLager SET BestellStatus = 'Bestellen' WHERE ID = @ID";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
                        updateCommand.Parameters.AddWithValue("@ID", row["ID"]);
                        updateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        row["BestellStatus"] = string.Empty;
                        // SQL-Update-Befehl für leeren BestellStatus
                        string updateQuery = "UPDATE MaterialLager SET BestellStatus = '' WHERE ID = @ID";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
                        updateCommand.Parameters.AddWithValue("@ID", row["ID"]);
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // lblAnzahlArtikelBestellen aktualisieren (Anzahl der Artikel, die bestellt werden müssen)
                int anzahlArtikelBestellen = 0;
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    if (row["BestellStatus"].ToString() == "Bestellen")
                    {
                        anzahlArtikelBestellen++;
                        lblAnzahlArtikelBestellen.Text = anzahlArtikelBestellen.ToString() + " Artikel müssen bestellt werden!";
                    }
                    // Wenn in der Reihe "Bemerkungen" in einer Zeile das Wort "bestellt, Bestellt vorkommt, dann soll die variable anzahlArtikelBestellen um 1 verringert werden
                    if (row["Bemerkungen"].ToString().ToLower().Contains("bestellt am"))
                    {
                        anzahlArtikelBestellen--;
                        lblAnzahlArtikelBestellen.Text = anzahlArtikelBestellen.ToString() + " Artikel müssen bestellt werden!";
                    }
                }

                sqlConnection.Close(); // Datenbankverbindung trennen
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Methode zur Ausführung einer SQL-Abfrage
        private void ExecuteQuery(string query)
        {
            try
            {
                sqlConnection.Open(); // Öffne die SQL-Verbindung
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // Erstelle ein SqlCommand-Objekt mit der übergebenen Abfrage
                sqlCommand.ExecuteNonQuery(); // Führe die Abfrage aus, ohne ein Ergebnis zurückzugeben
                sqlConnection.Close(); // Schließe die SQL-Verbindung
            }
            catch (Exception ex)
            {
                // Behandele und zeige Ausnahmen an, die während der Ausführung der Abfrage auftreten können
                MessageBox.Show(ex.Message);
            }
        }

        // Methode zur Ermittlung der nächsthöheren verfügbaren ID
        private int GetNextId()
        {
            try
            {
                sqlConnection.Open(); // Verbindung zur Datenbank öffnen
                string query = "SELECT MAX(Id) FROM MaterialLager"; // SQL-Abfrage, um die höchste ID zu ermitteln
                SqlCommand command = new SqlCommand(query, sqlConnection);
                object result = command.ExecuteScalar(); // Ergebnis der Abfrage abrufen
                sqlConnection.Close(); // Datenbankverbindung trennen

                if (result != DBNull.Value)
                {
                    return Convert.ToInt32(result) + 1; // Nächste ID zurückgeben
                }
                else
                {
                    return 1; // Falls keine ID vorhanden, mit 1 beginnen
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Ermitteln der nächsten ID: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1; // Standardwert bei Fehler
            }
        }

        // Event-Handler wenn der Button "Material austragen" gedrückt wird
        private void BtnAustragen_Click(object sender, EventArgs e)
        {
            if (currentId == 0)
            {
                MessageBox.Show("Bitte zuerst einen Artikel in der Tabelle auswählen.");
                return;
            }

            using (Form_InputMenge inputMenge = new Form_InputMenge(TextBoxEinheit1.Text))
            {
                if (inputMenge.ShowDialog() == DialogResult.OK)
                {
                    if (int.TryParse(inputMenge.InputValue, out int abzugMenge))
                    {
                        int neueMenge = int.Parse(TextBoxLagerstand.Text) - abzugMenge;
                        string query = $"UPDATE MaterialLager SET Lagerstand = {neueMenge} WHERE Id = {currentId}";
                        ExecuteQuery(query);

                        // Überprüfen, ob Lagerstand <= Mindestbestand ist und BestellStatus setzen
                        CheckAndUpdateBestellStatus(neueMenge, int.Parse(TextBoxMindestbestand.Text));

                        MessageBox.Show("Erfolgreich " + abzugMenge + " " + TextBoxEinheit1.Text + " abgezogen!");
                        UpdateDgvMateriallager();
                        ClearTextBoxes();
                    }
                }
            }
        }


        // Event-Handler für den Button "Hinzufügen"
        private void BtnHinzufuegen_Click(object sender, EventArgs e)
        {
            // Textfelder leeren, um neue Daten einzugeben
            ClearTextBoxes();
            // Setze die aktuelle ID auf die nächsthöhere verfügbare ID
            currentId = GetNextId();
            TextBoxId.Text = currentId.ToString(); // Setze die neue ID in das ID-Textfeld

            // Speicherbutton sichtbar machen
            BtnSpeichern.Visible = true;

            // Hinweis an den Benutzer, dass er nun neue Daten eingeben kann
            MessageBox.Show("Alle Felder ausfüllen und dann auf 'Speichern' klicken!", "Neues Material hinzufügen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Event-Handler für den Button "Ändern"
        private void BtnAendern_Click(object sender, EventArgs e)
        {
            try
            {
                // Überprüfen, ob eine gültige ID ausgewählt ist
                if (currentId == 0)
                {
                    MessageBox.Show("Bitte zuerst einen Artikel in der Tabelle auswählen.");
                    return;
                }

                // Daten aus den Textboxen entnehmen
                string kategorie = TextBoxKategorie.Text;
                string artikel = TextBoxArtikel.Text;
                int lagerstand = int.Parse(TextBoxLagerstand.Text);
                int mindestbestand = int.Parse(TextBoxMindestbestand.Text);
                string einheit = TextBoxEinheit1.Text;
                string bemerkung = RichTextBoxBemerkung.Text;

                // Bestätigungsdialog anzeigen
                DialogResult result = MessageBox.Show("Änderungen speichern?", "Änderungen speichern", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Nur fortfahren, wenn der Benutzer "Ja" auswählt
                if (result == DialogResult.Yes)
                {
                    // SQL-Update-Statement erstellen
                    string query = $"UPDATE MaterialLager SET Kategorie = '{kategorie}', Artikel = '{artikel}', " +
                                   $"Lagerstand = {lagerstand}, Mindestbestand = {mindestbestand}, " +
                                   $"Einheit = '{einheit}', Bemerkungen = '{bemerkung}' WHERE Id = {currentId}";

                    // SQL-Abfrage ausführen
                    ExecuteQuery(query);

                    // Erfolgsnachricht anzeigen
                    MessageBox.Show("Änderungen erfolgreich gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Aktualisiere das DataGridView, um die Änderungen anzuzeigen
                    UpdateDgvMateriallager();
                }
            }
            catch (Exception ex)
            {
                // Fehlernachricht anzeigen, falls ein Fehler auftritt
                MessageBox.Show($"Fehler beim Speichern der Änderungen: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event-Handler für den Button "Speichern"
        private void BtnSpeichern_Click(object sender, EventArgs e)
        {
            // Neues Material speichern
            SpeichereNeuesMaterial();

            // Speicherbutton nach dem Speichern wieder ausblenden
            BtnSpeichern.Visible = false;
        }

        // Event-Handler für den Button "Löschen"
        private void BtnLoeschen_Click(object sender, EventArgs e)
        {
            try
            {
                // Überprüfen, ob eine gültige ID ausgewählt ist
                if (currentId == 0)
                {
                    MessageBox.Show("Bitte zuerst einen Artikel in der Tabelle auswählen.");
                    return;
                }

                // Bestätigungsdialog anzeigen
                DialogResult result = MessageBox.Show("Ausgewählte Zeile wirklich löschen?", "Eintrag löschen", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                // Nur fortfahren, wenn der Benutzer "OK" auswählt
                if (result == DialogResult.OK)
                {
                    // Passwortabfrage anzeigen
                    using (Form_Pw passwordPrompt = new Form_Pw())
                    {
                        if (passwordPrompt.ShowDialog() == DialogResult.OK)
                        {
                            // Das korrekte Passwort festlegen (dieses Passwort kann später durch ein sichereres Verfahren ersetzt werden)
                            string correctPassword = "1127"; // Ersetze "deinPasswort" durch das tatsächliche Passwort

                            // Überprüfen, ob das eingegebene Passwort korrekt ist
                            if (passwordPrompt.Passwort == correctPassword)
                            {
                                // SQL-Delete-Statement erstellen
                                string query = $"DELETE FROM MaterialLager WHERE Id = {currentId}";

                                // SQL-Abfrage ausführen
                                ExecuteQuery(query);

                                // Erfolgsnachricht anzeigen
                                MessageBox.Show("Eintrag erfolgreich gelöscht.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Aktualisiere das DataGridView, um die Änderungen anzuzeigen
                                UpdateDgvMateriallager();

                                // Textfelder leeren
                                ClearTextBoxes();

                                // Setze currentId auf 0, da kein Eintrag mehr ausgewählt ist
                                currentId = 0;
                            }
                            else
                            {
                                // Fehlermeldung anzeigen, wenn das Passwort falsch ist
                                MessageBox.Show("Falsches Passwort. Der Eintrag wird nicht gelöscht.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehlernachricht anzeigen, falls ein Fehler auftritt
                MessageBox.Show($"Fehler beim Löschen des Eintrags: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Methode zum Speichern eines neuen Materials
        private void SpeichereNeuesMaterial()
        {
            try
            {
                string kategorie = TextBoxKategorie.Text;
                string artikel = TextBoxArtikel.Text;
                int lagerstand = int.Parse(TextBoxLagerstand.Text);
                int mindestbestand = int.Parse(TextBoxMindestbestand.Text);
                string einheit = TextBoxEinheit1.Text;
                string bemerkung = RichTextBoxBemerkung.Text;

                string query = $"INSERT INTO MaterialLager (Kategorie, Artikel, Lagerstand, Mindestbestand, Einheit, Bemerkungen) " +
                               $"VALUES ('{kategorie}', '{artikel}', {lagerstand}, {mindestbestand}, '{einheit}', '{bemerkung}')";
                ExecuteQuery(query);

                // Überprüfen, ob der Lagerstand den Mindestbestand unterschreitet und ggf. Bestellstatus setzen
                CheckAndUpdateBestellStatus(lagerstand, mindestbestand);

                MessageBox.Show("Neues Material erfolgreich hinzugefügt.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateDgvMateriallager();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Hinzufügen des neuen Materials: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckAndUpdateBestellStatus(int lagerstand, int mindestbestand)
        {
            try
            {
                string bestellStatus = lagerstand <= mindestbestand ? "Bestellen" : string.Empty;
                string query = $"UPDATE MaterialLager SET BestellStatus = '{bestellStatus}' WHERE Id = {currentId}";
                ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Aktualisieren des Bestellstatus: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Event-Handler-Methode, die ausgelöst wird, wenn auf eine Zelle im DataGridView geklickt wird
        private void DgvMateriallager_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Überprüfen, ob der Index der Zeile gültig ist
                if (e.RowIndex >= 0)
                {
                    // Ausgewählte Zeile im DataGridView abrufen
                    DataGridViewRow selectedRow = DgvMateriallager.Rows[e.RowIndex];
                    // ID der ausgewählten Teile speichern
                    currentId = (int)selectedRow.Cells[0].Value;
                    // Daten aus der ausgewählten Zeile in die entsprechenden TextBoxen laden
                    TextBoxId.Text = currentId.ToString();
                    TextBoxKategorie.Text = selectedRow.Cells[1].Value.ToString();
                    TextBoxArtikel.Text = selectedRow.Cells[2].Value.ToString();
                    TextBoxLagerstand.Text = selectedRow.Cells[3].Value.ToString();
                    TextBoxMindestbestand.Text = selectedRow.Cells[4].Value.ToString();
                    TextBoxEinheit1.Text = selectedRow.Cells[5].Value.ToString();
                    TextBoxEinheit2.Text = TextBoxEinheit1.Text;
                    RichTextBoxBemerkung.Text = selectedRow.Cells[7].Value.ToString();

                    // Pfad zum Bild
                    string imagePath = "";

                    // Überprüfen, welcher Artikel ausgewählt wurde und den entsprechenden Bildpfad zuweisen
                    if (TextBoxArtikel.Text == "Mo Liner 0 95 023")
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 023.png";
                    }
                    else if (TextBoxArtikel.Text == "Mo Liner 0 95 025")
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 025.png";
                    }
                    else if (TextBoxArtikel.Text == "Mo Liner 0 95 033")
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 033.png";
                    }
                    else if (TextBoxArtikel.Text == "Mo Liner 0 95 058")
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 058.png";
                    }
                    else if (TextBoxArtikel.Text == "Mo Liner 0 95 110")
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 110.png";
                    }
                    else if (TextBoxArtikel.Text == "Deckel für Liner")
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Deckel für Liner.png";
                    }
                    else
                    {
                        imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Ansicht.png";
                    }

                    // Überprüfen, ob ein gültiger Bildpfad gefunden wurde und das Bild existiert
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        // Das Bild der PictureBox zuweisen
                        PictureBoxInfo.Image = new Bitmap(imagePath);
                        PictureBoxInfo.Refresh();
                    }
                    else
                    {
                        // Anderenfalls die PictureBox leeren oder ein Platzhalterbild anzeigen
                        PictureBoxInfo.Image = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Event-Handler-Methode, die ausgeführt wird, wenn das DataGridView geladen wird
        private void DgvMateriallager_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Einfärben nach Kategorie
            try
            {
                if (e.ColumnIndex == DgvMateriallager.Columns["Kategorie"].Index && e.Value != null)
                {
                    string kategorie = e.Value.ToString();
                    switch (kategorie)
                    {
                        case "Aufdampfmaterial":
                            e.CellStyle.BackColor = Color.LightGreen;
                            break;
                        case "Draht":
                            e.CellStyle.BackColor = Color.LightCoral;
                            break;
                        case "Lampen":
                            e.CellStyle.BackColor = Color.LightGoldenrodYellow;
                            break;
                        case "Liner":
                            e.CellStyle.BackColor = Color.LightSeaGreen;
                            break;
                        case "Strahlsand":
                            e.CellStyle.BackColor = Color.LightSalmon;
                            break;
                        case "Öl":
                            e.CellStyle.BackColor = Color.LightPink;
                            break;
                        case "Quarze":
                            e.CellStyle.BackColor = Color.Aqua;
                            break;
                        case "Schutzglas":
                            e.CellStyle.BackColor = Color.LightSkyBlue;
                            break;
                        case "IR Strahler":
                            e.CellStyle.BackColor = Color.Azure;
                            break;
                        case "Kathode":
                            e.CellStyle.BackColor = Color.Orange;
                            break;
                    }
                }
                if (e.ColumnIndex == DgvMateriallager.Columns["BestellStatus"].Index && e.Value.ToString() == "Bestellen")
                {
                    e.CellStyle.BackColor = Color.Red;
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Funktion zum Leeren aller Textfelder
        private void ClearTextBoxes()
        {
            TextBoxId.Text = string.Empty;
            TextBoxKategorie.Text = string.Empty;
            TextBoxArtikel.Text = string.Empty;
            TextBoxLagerstand.Text = string.Empty;
            TextBoxMindestbestand.Text = string.Empty;
            TextBoxEinheit1.Text = string.Empty;
            TextBoxEinheit2.Text = string.Empty;
            RichTextBoxBemerkung.Text = string.Empty;
        }

        // Event-Handler für den Button "Drucken"
        private void BtnInventur_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            // Excel - Anwendung erstellen
            Excel.Application excelApp = new Excel.Application
            {
                Visible = true              
            };

            // Neue Arbeitsmappe hinzufügen
            Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Excel.Worksheet worksheet = workbook.Sheets[1];
            worksheet.Name = "Lager Checkliste";

            // Überschrift hinzufügen
            worksheet.Cells[1, 1] = "Checkliste für Vergütungslager --> Wenn Lagerstand abweicht, bitte richtigen Wert daneben hin schreiben.";
            Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, DgvMateriallager.Columns.Count]];
            headerRange.Merge();
            headerRange.Font.Size = 18;
            headerRange.Font.Bold = true;
            headerRange.Font.Color = Color.Blue;
            headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // Leere Zeile nach der Überschrift
            int startRow = 3;

            // Spaltenüberschriften hinzufügen
            for (int i = 0; i < DgvMateriallager.Columns.Count; i++)
            {
                worksheet.Cells[startRow, i + 1] = DgvMateriallager.Columns[i].HeaderText;
                worksheet.Cells[startRow, i + 1].Font.Bold = true;
            }

            // Daten hinzufügen und unterstreichen
            for (int i = 0; i < DgvMateriallager.Rows.Count; i++)
            {
                for (int j = 0; j < DgvMateriallager.Columns.Count; j++)
                {
                    worksheet.Cells[i + startRow + 1, j + 1] = DgvMateriallager.Rows[i].Cells[j].Value?.ToString();
                }

                // Unterstreichen der gesamten Zeile
                Excel.Range dataRowRange = worksheet.Range[worksheet.Cells[i + startRow + 1, 1], worksheet.Cells[i + startRow + 1, DgvMateriallager.Columns.Count]];
                dataRowRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            // Prüfen, ob in irgendeiner Zelle in Spalte G (ab G4) etwas steht und Text in Rot formatieren
            for (int i = 4; i <= DgvMateriallager.Rows.Count + startRow; i++)
            {
                Excel.Range cell = worksheet.Cells[i, 7]; // G4, G5, ..., Gn
                if (cell.Value != null)
                {
                    cell.Font.Color = ColorTranslator.ToOle(Color.Red);
                }
            }

            // AutoFit der Spalten
            worksheet.Columns.AutoFit();

            // Druckeinstellungen anpassen
            worksheet.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
            worksheet.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;
            worksheet.PageSetup.FitToPagesWide = 1;
            worksheet.PageSetup.FitToPagesTall = 1;
            worksheet.PageSetup.Zoom = false; // Deaktiviert die Zoom-Einstellung, um FitToPagesWide und FitToPagesTall zu aktivieren
        }

        // Event-Handler für den Button "Info Materiallager"
        private void BtnInfoMateriallager_Click(object sender, EventArgs e)
        {
            Form_InfoMateriallager infoMateriallager = new Form_InfoMateriallager();
            infoMateriallager.ShowDialog();
        }
    }
}

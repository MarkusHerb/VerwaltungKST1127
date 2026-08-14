using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer und grundlegende Ausnahmen)
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für Komponenten und Ereignismodelle (z.B. für das Arbeiten mit Entwurfsmustern und benachrichtigungsfähigen Eigenschaften in Formularanwendungen)
using System.Data; // Importieren des System.Data-Namespace für Datenoperationen und die Arbeit mit Datenquellen (z.B. DataTable, DataSet und andere ADO.NET-Funktionalitäten)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für den Zugriff auf SQL Server-Datenbanken (z.B. zum Arbeiten mit SQL-Verbindungen, Befehlen und Datenlesern in ADO.NET)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Farben, Bilder, Schriften und andere grafische Ressourcen)
using System.IO; // Importieren des System.IO-Namespace für Dateioperationen und Stream-E/A (z.B. zum Lesen und Schreiben von Dateien, Arbeiten mit Verzeichnissen)
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel; // Importieren des Microsoft.Office.Interop.Excel-Namespace für den Zugriff auf Excel-Objekte und -Funktionen (z.B. Excel-Anwendungen, Arbeitsmappen, Tabellen)
using VerwaltungKST1127.Material; // Importieren des System.Windows.Forms-Namespace für Windows Forms-Steuerelemente und Benutzeroberflächen (z.B. Button, TextBox, Label für GUI-Entwicklung)


namespace VerwaltungKST1127
{
    public partial class Form_Materiallager : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank
        private const string ConnectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

        // Variable für die aktuelle ID im DataGridView
        int currentId;

        public Form_Materiallager()
        {
            InitializeComponent();
            // Event-Handler für das Laden des Formulars hinzufügen
            Load += Form_Materiallager_Load;
            // Event-Handler für die Zellformatierung des DataGridView "DgvMateriallager" hinzufügen
            DgvBestellRadar.CellFormatting += DgvBestellRadar_CellFormatting;
            // Event-Handler für das MouseEnter-Ereignis des DataGridView "DgvBestellRadar" hinzufügen
            DgvBestellRadar.CellMouseEnter += DgvBestellRadar_CellMouseEnter;
            // Event-Handler für das CellClick-Ereignis des DataGridView "DgvBestellRadar" hinzufügen
            DgvBestellRadar.CellClick += DgvBestellRadar_CellClick;
            // Sichtbarkeit festlegen
            BtnSpeichern.Visible = false;
            // BtnLoeschen.Visible = false;
        }

        private async void Form_Materiallager_Load(object sender, EventArgs e)
        {
            await UpdateDgvMateriallagerAsync();
        }

        // Methode zur Aktualisierung und Formatierung des DataGridView für das Materiallager
        public async Task UpdateDgvMateriallagerAsync()
        {
            try
            {
                DataTable table = await Task.Run(LoadMateriallagerTable);

                DgvMateriallager.SuspendLayout();
                DgvMateriallager.DataSource = table; // Datenquelle für das DataGridView festlegen
                DgvMateriallager.Sort(DgvMateriallager.Columns[1], ListSortDirection.Ascending); // DataGridView nach Kategorie sortieren
                // DataGridView-Formatierung: Header-Schriftart und Ausrichtung
                DgvMateriallager.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(DataGridView.DefaultFont, FontStyle.Bold);
                DgvMateriallager.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // Spaltenbreite des Dgvs festlegen
                DgvMateriallager.Columns[0].Width = 30;
                DgvMateriallager.Columns[2].Width = 220;
                DgvMateriallager.Columns[7].Width = 445;
                DgvMateriallager.ResumeLayout();

                UpdateBestellLabel(table);
                UpdateRadarDashboard(table);
                UpdateBestellRadar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        private DataTable LoadMateriallagerTable()
        {
            const string query = @"
                SELECT
                    ID,
                    Kategorie,
                    Artikel,
                    Lagerstand,
                    Mindestbestand,
                    Einheit,
                    CASE WHEN Lagerstand <= Mindestbestand THEN 'Bestellen' ELSE '' END AS BestellStatus,
                    Bemerkungen
                FROM MaterialLager
                ORDER BY Kategorie, Artikel";

            using (var connection = new SqlConnection(ConnectionString))
            using (var adapter = new SqlDataAdapter(query, connection))
            {
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private void UpdateBestellLabel(DataTable table)
        {
            int anzahlArtikelBestellen = 0;

            foreach (DataRow row in table.Rows)
            {
                bool mussBestelltWerden = row["BestellStatus"].ToString() == "Bestellen";
                bool istBereitsBestellt = row["Bemerkungen"].ToString().IndexOf("bestellt am", StringComparison.OrdinalIgnoreCase) >= 0;

                if (mussBestelltWerden && !istBereitsBestellt)
                {
                    anzahlArtikelBestellen++;
                }
            }

            lblAnzahlArtikelBestellen.Text = anzahlArtikelBestellen == 1
                ? "1 Artikel muss bestellt werden!"
                : anzahlArtikelBestellen + " Artikel müssen bestellt werden!";
        }

        // Methode zur Ausführung einer SQL-Abfrage
        private async Task ExecuteNonQueryAsync(string query, Action<SqlParameterCollection> addParameters)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    addParameters?.Invoke(command.Parameters);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Behandele und zeige Ausnahmen an, die während der Ausführung der Abfrage auftreten können
                MessageBox.Show(ex.Message);
            }
        }

        // Methode zur Ermittlung der nächsthöheren verfügbaren ID
        private async Task<int> GetNextIdAsync()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand("SELECT MAX(Id) FROM MaterialLager", connection))
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();
                    return result != DBNull.Value ? Convert.ToInt32(result) + 1 : 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Ermitteln der nächsten ID: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1; // Standardwert bei Fehler
            }
        }

        // Event-Handler wenn der Button "Material austragen" gedrückt wird
        private async void BtnAustragen_Click(object sender, EventArgs e)
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
                        int mindestbestand = int.Parse(TextBoxMindestbestand.Text);

                        await ExecuteNonQueryAsync(
                            "UPDATE MaterialLager SET Lagerstand = @Lagerstand, BestellStatus = CASE WHEN @Lagerstand <= @Mindestbestand THEN 'Bestellen' ELSE '' END WHERE Id = @Id",
                            parameters =>
                            {
                                parameters.AddWithValue("@Lagerstand", neueMenge);
                                parameters.AddWithValue("@Mindestbestand", mindestbestand);
                                parameters.AddWithValue("@Id", currentId);
                            });

                        MessageBox.Show("Erfolgreich " + abzugMenge + " " + TextBoxEinheit1.Text + " abgezogen!");
                        await UpdateDgvMateriallagerAsync();
                        ClearTextBoxes();
                    }
                }
            }
        }


        // Event-Handler für den Button "Hinzufügen"
        private async void BtnHinzufuegen_Click(object sender, EventArgs e)
        {
            // Textfelder leeren, um neue Daten einzugeben
            ClearTextBoxes();
            // Setze die aktuelle ID auf die nächsthöhere verfügbare ID
            currentId = await GetNextIdAsync();
            TextBoxId.Text = currentId.ToString(); // Setze die neue ID in das ID-Textfeld

            // Speicherbutton sichtbar machen
            BtnSpeichern.Visible = true;

            // Hinweis an den Benutzer, dass er nun neue Daten eingeben kann
            MessageBox.Show("Alle Felder ausfüllen und dann auf 'Speichern' klicken!", "Neues Material hinzufügen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Event-Handler für den Button "Ändern"
        private async void BtnAendern_Click(object sender, EventArgs e)
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
                    // SQL-Abfrage ausführen
                    await ExecuteNonQueryAsync(
                        @"UPDATE MaterialLager
                          SET Kategorie = @Kategorie,
                              Artikel = @Artikel,
                              Lagerstand = @Lagerstand,
                              Mindestbestand = @Mindestbestand,
                              Einheit = @Einheit,
                              Bemerkungen = @Bemerkungen,
                              BestellStatus = CASE WHEN @Lagerstand <= @Mindestbestand THEN 'Bestellen' ELSE '' END
                          WHERE Id = @Id",
                        parameters =>
                        {
                            parameters.AddWithValue("@Kategorie", kategorie);
                            parameters.AddWithValue("@Artikel", artikel);
                            parameters.AddWithValue("@Lagerstand", lagerstand);
                            parameters.AddWithValue("@Mindestbestand", mindestbestand);
                            parameters.AddWithValue("@Einheit", einheit);
                            parameters.AddWithValue("@Bemerkungen", bemerkung);
                            parameters.AddWithValue("@Id", currentId);
                        });

                    // Erfolgsnachricht anzeigen
                    MessageBox.Show("Änderungen erfolgreich gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Aktualisiere das DataGridView, um die Änderungen anzuzeigen
                    await UpdateDgvMateriallagerAsync();
                }
            }
            catch (Exception ex)
            {
                // Fehlernachricht anzeigen, falls ein Fehler auftritt
                MessageBox.Show($"Fehler beim Speichern der Änderungen: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event-Handler für den Button "Speichern"
        private async void BtnSpeichern_Click(object sender, EventArgs e)
        {
            // Neues Material speichern
            await SpeichereNeuesMaterialAsync();

            // Speicherbutton nach dem Speichern wieder ausblenden
            BtnSpeichern.Visible = false;
        }

        // Event-Handler für den Button "Löschen"
        private async void BtnLoeschen_Click(object sender, EventArgs e)
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
                                // SQL-Abfrage ausführen
                                await ExecuteNonQueryAsync(
                                    "DELETE FROM MaterialLager WHERE Id = @Id",
                                    parameters => parameters.AddWithValue("@Id", currentId));

                                // Erfolgsnachricht anzeigen
                                MessageBox.Show("Eintrag erfolgreich gelöscht.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Aktualisiere das DataGridView, um die Änderungen anzuzeigen
                                await UpdateDgvMateriallagerAsync();

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
        private async Task SpeichereNeuesMaterialAsync()
        {
            try
            {
                string kategorie = TextBoxKategorie.Text;
                string artikel = TextBoxArtikel.Text;
                int lagerstand = int.Parse(TextBoxLagerstand.Text);
                int mindestbestand = int.Parse(TextBoxMindestbestand.Text);
                string einheit = TextBoxEinheit1.Text;
                string bemerkung = RichTextBoxBemerkung.Text;

                await ExecuteNonQueryAsync(
                    @"INSERT INTO MaterialLager (Kategorie, Artikel, Lagerstand, Mindestbestand, Einheit, Bemerkungen, BestellStatus)
                      VALUES (@Kategorie, @Artikel, @Lagerstand, @Mindestbestand, @Einheit, @Bemerkungen,
                              CASE WHEN @Lagerstand <= @Mindestbestand THEN 'Bestellen' ELSE '' END)",
                    parameters =>
                    {
                        parameters.AddWithValue("@Kategorie", kategorie);
                        parameters.AddWithValue("@Artikel", artikel);
                        parameters.AddWithValue("@Lagerstand", lagerstand);
                        parameters.AddWithValue("@Mindestbestand", mindestbestand);
                        parameters.AddWithValue("@Einheit", einheit);
                        parameters.AddWithValue("@Bemerkungen", bemerkung);
                    });

                MessageBox.Show("Neues Material erfolgreich hinzugefügt.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await UpdateDgvMateriallagerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Hinzufügen des neuen Materials: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    LoadMaterialImage(TextBoxArtikel.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        private void LoadMaterialImage(string artikel)
        {
            string imagePath;

            switch (artikel)
            {
                case "Mo Liner 0 95 023":
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 023.png";
                    break;
                case "Mo Liner 0 95 025":
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 025.png";
                    break;
                case "Mo Liner 0 95 033":
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 033.png";
                    break;
                case "Mo Liner 0 95 058":
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 058.png";
                    break;
                case "Mo Liner 0 95 110":
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Mo Liner 0 95 110.png";
                    break;
                case "Deckel für Liner":
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Deckel für Liner.png";
                    break;
                default:
                    imagePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Bilder\Ansicht.png";
                    break;
            }

            Image previousImage = PictureBoxInfo.Image;
            PictureBoxInfo.Image = File.Exists(imagePath) ? new Bitmap(imagePath) : null;
            previousImage?.Dispose();
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
                if (e.ColumnIndex == DgvMateriallager.Columns["BestellStatus"].Index && e.Value?.ToString() == "Bestellen")
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

        // Methode zum Aktualisieren des Bestell-Radars.
        //
        // Zweck:
        // Das DataGridView "DgvBestellRadar" zeigt alle im Materiallager vorhandenen
        // Artikel an und sortiert diese automatisch nach ihrer Kritikalität.
        //
        // Die Sortierung erfolgt über das Verhältnis:
        //
        // Lagerstand / Mindestbestand
        //
        // Beispiel:
        // Lagerstand = 12
        // Mindestbestand = 10
        // Ergebnis = 120 %
        //
        // Dadurch stehen die Artikel, die am nächsten an der Mindestbestellmenge liegen,
        // immer ganz oben in der Liste.
        //
        // Vorteile:
        // - Kritische Artikel werden sofort sichtbar
        // - Lagerengpässe können frühzeitig erkannt werden
        // - Kein manuelles Kontrollieren aller Lagerbestände notwendig
        private void UpdateBestellRadar()
        {
            try
            {
                // SQL-Abfrage zum Laden aller Materiallager-Artikel
                const string query = @"
        SELECT
            Artikel,
                CAST (Lagerstand AS VARCHAR(20)) + ' / ' + CAST (Mindestbestand AS VARCHAR(20)) AS Bestand,
            Lagerstand,
            Mindestbestand,

            -- Berechnung des Lagerbestandes in Prozent
            -- Beispiel:
            -- Lagerstand = 15
            -- Mindestbestand = 10
            -- Ergebnis = 150 %
            CAST(
                CASE
                    WHEN Mindestbestand > 0
                    THEN (Lagerstand * 100.0 / Mindestbestand)
                    ELSE 0
                END
            AS INT) AS BestandProzent

        FROM MaterialLager

        -- Sortierung nach Kritikalität:
        -- Die kleinsten Prozentwerte erscheinen zuerst.
        -- Damit stehen die kritischsten Artikel automatisch ganz oben.
        ORDER BY
            CASE
                WHEN Lagerstand <= Mindestbestand
                THEN 0
                ELSE 1
            END,
            CASE
                WHEN Mindestbestand > 0
                THEN CAST(Lagerstand AS FLOAT) / Mindestbestand
                ELSE 999999
            END ASC";

                // Verbindung zur SQL-Datenbank herstellen
                using (SqlConnection connection = new SqlConnection(ConnectionString))

                // SqlDataAdapter verwenden, um die Daten abzufragen
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    // Neues DataTable-Objekt erstellen
                    DataTable table = new DataTable();

                    // SQL-Abfrage ausführen und Ergebnis in DataTable laden
                    adapter.Fill(table);

                    // DataTable als Datenquelle für das Bestell-Radar festlegen
                    DgvBestellRadar.DataSource = table;

                    // Zeilenköpfe (graue Auswahlspalte links) ausblenden
                    DgvBestellRadar.RowHeadersVisible = false;

                    // Benutzer darf keine neuen Zeilen hinzufügen
                    DgvBestellRadar.AllowUserToAddRows = false;

                    // Benutzer darf keine Zeilen löschen
                    DgvBestellRadar.AllowUserToDeleteRows = false;

                    // Benutzer darf die Zeilenhöhe nicht verändern
                    DgvBestellRadar.AllowUserToResizeRows = false;

                    // DataGridView dient nur zur Anzeige
                    DgvBestellRadar.ReadOnly = true;

                    // Es darf immer nur eine Zeile markiert werden
                    DgvBestellRadar.MultiSelect = false;

                    // Gesamte Zeile markieren statt einzelner Zellen
                    DgvBestellRadar.SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect;

                    // Benutzerfreundliche Spaltenüberschriften festlegen
                    DgvBestellRadar.Columns["Artikel"].HeaderText = "Artikel";
                    DgvBestellRadar.Columns["BestandProzent"].HeaderText = "%";

                    // Spaltenbreiten festlegen
                    DgvBestellRadar.Columns["Artikel"].Width = 163;
                    DgvBestellRadar.Columns["Bestand"].Width = 90;
                    DgvBestellRadar.Columns["BestandProzent"].Width = 55;

                    // Spaltenüberschrift für die Spalte "Bestand" ändern
                    DgvBestellRadar.Columns["Bestand"].HeaderText = "Lager";

                    // Zentrierte Ausrichtung der Prozentwerte festlegen
                    DgvBestellRadar.Columns["BestandProzent"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Technische Hilfsspalten ausblenden,
                    // da diese für den Benutzer nicht relevant sind
                    DgvBestellRadar.Columns["Lagerstand"].Visible = false;
                    DgvBestellRadar.Columns["Mindestbestand"].Visible = false;

                    // Zentrierte Ausrichtung der Spaltenüberschriften
                    DgvBestellRadar.ColumnHeadersDefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;

                    // Überschriften fett darstellen
                    DgvBestellRadar.ColumnHeadersDefaultCellStyle.Font =
                        new Font(DataGridView.DefaultFont, FontStyle.Bold);

                    // Nach dem Laden keine Zeile vorauswählen
                    DgvBestellRadar.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                // Fehler beim Laden des Bestell-Radars anzeigen
                MessageBox.Show(
                    "Fehler beim Laden des Bestellradars:\n" + ex.Message,
                    "Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // Formatierung des Bestell-Radars.
        //
        // Die Prozentwerte werden je nach Lagerreserve farblich markiert:
        //
        // Rot   = Kritisch (<= 120 %)
        // Gelb  = Beobachten (<= 200 %)
        // Grün  = Ausreichender Lagerbestand (> 200 %)
        //
        // Beispiele:
        // 90 %  -> Rot
        // 150 % -> Gelb
        // 350 % -> Grün
        private void DgvBestellRadar_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // Nur die Prozentspalte formatieren
                if (DgvBestellRadar.Columns[e.ColumnIndex].Name == "BestandProzent")
                {
                    // Prozentwert aus der aktuellen Zelle lesen
                    int prozent = Convert.ToInt32(e.Value);

                    // Kritischer Bereich
                    if (prozent <= 120)
                    {
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                        // Fett darstellen, um die Dringlichkeit zu unterstreichen
                        e.CellStyle.Font = new Font(DgvBestellRadar.Font, FontStyle.Bold);
                    }
                    // Beobachtungsbereich
                    else if (prozent <= 200)
                    {
                        e.CellStyle.BackColor = Color.Gold;
                        e.CellStyle.ForeColor = Color.Black;
                    }
                    // Ausreichender Lagerbestand
                    else
                    {
                        e.CellStyle.BackColor = Color.LightGreen;
                        e.CellStyle.ForeColor = Color.Black;
                    }

                    // Prozentwert mittig ausrichten
                    e.CellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;

                    // Prozentzeichen ergänzen
                    e.Value = prozent + "%";

                    // Kennzeichnen, dass die Formatierung bereits erfolgt ist
                    e.FormattingApplied = true;
                }

            }
            catch
            {
                // Fehler bewusst ignorieren,
                // damit ein fehlerhafter Datensatz das Grid nicht blockiert
            }
        }



        private void DgvBestellRadar_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = DgvBestellRadar.Rows[e.RowIndex];

                string artikel =
                row.Cells["Artikel"].Value?.ToString();

                string lagerstand =
                row.Cells["Lagerstand"].Value?.ToString();

                string mindestbestand =
                row.Cells["Mindestbestand"].Value?.ToString();

                int differenz =
                Convert.ToInt32(lagerstand) -
                Convert.ToInt32(mindestbestand);

                row.Cells["Artikel"].ToolTipText =
                $"Artikel: {artikel}\n" +
                $"Lagerstand: {lagerstand}\n" +
                $"Mindestbestand: {mindestbestand}\n" +
                $"Reserve: {differenz}";
            }
            catch
            {
            }
        }

        private void DgvBestellRadar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                string artikel =
                DgvBestellRadar
                .Rows[e.RowIndex]
                .Cells["Artikel"]
                .Value
                .ToString();

                foreach (DataGridViewRow row in DgvMateriallager.Rows)
                {
                    if (row.Cells["Artikel"].Value?.ToString() == artikel)
                    {
                        DgvMateriallager.ClearSelection();

                        row.Selected = true;
                        DgvMateriallager_CellClick(DgvMateriallager, new DataGridViewCellEventArgs(0, row.Index));

                        DgvMateriallager.CurrentCell =
                        row.Cells["Artikel"];

                        DgvMateriallager.FirstDisplayedScrollingRowIndex =
                        row.Index;

                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void UpdateRadarDashboard(DataTable table)
        {
            int kritisch = 0;
            int beobachten = 0;
            int ok = 0;

            foreach (DataRow row in table.Rows)
            {
                int lagerstand =
                    Convert.ToInt32(row["Lagerstand"]);

                int mindestbestand =
                    Convert.ToInt32(row["Mindestbestand"]);

                if (mindestbestand == 0)
                {
                    ok++;
                    continue;
                }

                double reserve =
                    (double)lagerstand / mindestbestand;

                if (reserve <= 1.2)
                {
                    kritisch++;
                }
                else if (reserve <= 2.0)
                {
                    beobachten++;
                }
                else
                {
                    ok++;
                }
            }

            lblKritisch.Text =
                $"🔴 Kritisch: {kritisch}";
            lblKritisch.ForeColor = Color.Red;
            // Fett geschrieben
            lblKritisch.Font = new Font(lblKritisch.Font, FontStyle.Bold);

            lblBeobachten.Text =
                $"🟡 Beobachten: {beobachten}";
            lblBeobachten.ForeColor = Color.DarkOrange;
            lblBeobachten.Font = new Font(lblBeobachten.Font, FontStyle.Bold);

            lblOk.Text =
                $"🟢 OK: {ok}";
            lblOk.ForeColor = Color.Green;
            lblOk.Font = new Font(lblOk.Font, FontStyle.Bold);
        }
    }
}

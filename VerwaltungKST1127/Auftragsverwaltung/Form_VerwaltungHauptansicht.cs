using Newtonsoft.Json;
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic;
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.IO; // Für File-Operationen
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms;
using VerwaltungKST1127.EingabeSerienartikelPrototyp;

namespace VerwaltungKST1127.Auftragsverwaltung
{
    public partial class Form_VerwaltungHauptansicht : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank Verwaltung
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private readonly SqlConnection sqlConnectionVerwaltungAlt = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung;Integrated Security=True;Encrypt=False");

        // Zum Speichern der vollständigen Daten
        private DataTable _auftraegeDataTable = null;

        // Feld zum Speichern des ausgewählten Belag-Werts
        private string selectedBelagValue;

        public Form_VerwaltungHauptansicht()
        {
            InitializeComponent(); // Initialisierung der Komponenten des Formulars
            LoadDataForDgvAuftragZuBelag(); // Laden der Daten für das DataGridView dGvLadeBelaeg
            ZaehleGestarteteAuftraege(); // Funktion aufrufen
        }

        // Filter des Zukaufes
        private void ApplyFilterAndDisplayData()
        {
            int activeCount = 0; // Zähler für "Active" Status initialisieren

            try
            {
                if (_auftraegeDataTable == null)
                {
                    DgvAnsichtAuftraege.DataSource = null; // Keine Daten zum Anzeigen
                    return;
                }

                // Prüfen, ob gefiltert werden soll
                if (checkBoxShowZukauf.Checked) // Annahme: Deine CheckBox heißt checkBoxShowZukauf
                {
                    // Erstelle eine DataView für die Filterung
                    DataView dv = new DataView(_auftraegeDataTable);
                    // Filter anwenden: Zeige nur Zeilen, bei denen 'Zukauf' nicht leer oder null ist
                    dv.RowFilter = "[Zukauf] IS NOT NULL AND [Zukauf] <> ''";
                    DgvAnsichtAuftraege.DataSource = dv;
                }
                else
                {
                    // Zeige alle Daten an
                    DgvAnsichtAuftraege.DataSource = _auftraegeDataTable;
                }

                // ----- Formatierungscode (aus deinem Original übernommen) -----
                if (DgvAnsichtAuftraege.DataSource != null && DgvAnsichtAuftraege.Columns.Count > 0)
                {
                    // Spalten anpassen, sodass sie das DataGridView ausfüllen
                    DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Sicherstellen, dass die Spalten existieren, bevor darauf zugegriffen wird
                    if (DgvAnsichtAuftraege.Columns.Contains("SollStk."))
                        DgvAnsichtAuftraege.Columns["SollStk."].DefaultCellStyle.Format = "N0";
                    if (DgvAnsichtAuftraege.Columns.Contains("IstStk."))
                        DgvAnsichtAuftraege.Columns["IstStk."].DefaultCellStyle.Format = "N0";
                    if (DgvAnsichtAuftraege.Columns.Contains("VorStk."))
                        DgvAnsichtAuftraege.Columns["VorStk."].DefaultCellStyle.Format = "N0";
                    if (DgvAnsichtAuftraege.Columns.Contains("Teilelager"))
                        DgvAnsichtAuftraege.Columns["Teilelager"].DefaultCellStyle.Format = "N0";
                    if (DgvAnsichtAuftraege.Columns.Contains("Bereitstell"))
                        DgvAnsichtAuftraege.Columns["Bereitstell"].DefaultCellStyle.Format = "N0";
                    if (DgvAnsichtAuftraege.Columns.Contains("Jahresbedarf"))
                        DgvAnsichtAuftraege.Columns["Jahresbedarf"].DefaultCellStyle.Format = "N0";

                    // Werte in bestimmten Spalten mittig ausrichten
                    if (DgvAnsichtAuftraege.Columns.Contains("SollStk."))
                        DgvAnsichtAuftraege.Columns["SollStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("IstStk."))
                        DgvAnsichtAuftraege.Columns["IstStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("VorStk."))
                        DgvAnsichtAuftraege.Columns["VorStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Teilelager"))
                        DgvAnsichtAuftraege.Columns["Teilelager"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Bereitstell"))
                        DgvAnsichtAuftraege.Columns["Bereitstell"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Jahresbedarf"))
                        DgvAnsichtAuftraege.Columns["Jahresbedarf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Zukauf"))
                        DgvAnsichtAuftraege.Columns["Zukauf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Dringend"))
                        DgvAnsichtAuftraege.Columns["Dringend"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Headertexte der Spalten mittig ausrichten
                    foreach (DataGridViewColumn column in DgvAnsichtAuftraege.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Optional: Spalten automatisch anpassen (erneut, falls Fill nicht reicht)
                    // DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // Alternative probieren
                    DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Zurück zu Fill

                    // Reihenbreite anpassen (ggf. erst nach AutoSize prüfen)
                    if (DgvAnsichtAuftraege.Columns.Contains("Teilebez."))
                        DgvAnsichtAuftraege.Columns["Teilebez."].Width = 170; // Ggf. anpassen oder FillWeight verwenden
                    if (DgvAnsichtAuftraege.Columns.Contains("Seite"))
                        DgvAnsichtAuftraege.Columns["Seite"].Width = 60;
                    if (DgvAnsichtAuftraege.Columns.Contains("AVOinfo"))
                        DgvAnsichtAuftraege.Columns["AVOinfo"].Width = 140;

                    // ----- NEU: Zählung der 'Active' Status -----
                    // Sicherstellen, dass die Spalte "Status" existiert
                    if (DgvAnsichtAuftraege.Columns.Contains("Status"))
                    {
                        // Iteriere durch die tatsächlich angezeigten Zeilen im DataGridView
                        foreach (DataGridViewRow row in DgvAnsichtAuftraege.Rows)
                        {
                            // Zugriff auf den Wert der Zelle in der Spalte "Status"
                            // Verwende ?.Value für Null-Sicherheit
                            object cellValue = row.Cells["Status"]?.Value;

                            // Prüfen, ob der Wert nicht null ist und "Active" entspricht (Groß/Klein ignorieren)
                            if (cellValue != null && cellValue.ToString().Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                activeCount++; // Zähler erhöhen
                            }
                        }
                    }
                    // ----- Ende Zählung -----
                }
                else
                {
                    // Wenn keine Datenquelle gesetzt ist, gibt es auch nichts zu formatieren
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Anzeigen/Formatieren/Zählen der Daten: " + ex.Message);
                // Auch im Fehlerfall das Label auf 0 setzen oder letzte bekannte Zahl? Hier 0:
                activeCount = 0;
            }
            finally // Der finally-Block wird immer ausgeführt, auch nach einem Fehler im try
            {
                // Aktualisiere das Label mit dem gezählten Wert
                // Das Format kannst du anpassen (z.B. nur die Zahl oder mit Text)
                lblGestartet.Text = activeCount.ToString();
            }
        }

        // Event-Handler für das Klicken auf den Button BtnZukauf
        private void BtnZukauf_Click(object sender, EventArgs e)
        {
            Form_RLTL rltlForm = new Form_RLTL();
            rltlForm.ShowDialog();
        }

        private void checkBoxShowZukauf_CheckedChanged(object sender, EventArgs e)
        {
            // Rufe die Funktion auf, die die Daten basierend auf dem aktuellen CheckBox-Status filtert und anzeigt
            ApplyFilterAndDisplayData();
        }

        // --- Hilfsfunktion zum Bereinigen der Belag-Bezeichnung ---
        // Entfernt den Begriff "Vergüten" (unabhängig von Groß-/Kleinschreibung)
        // und extrahiert nur die "Bxxx" Information (z.B. B103, B146 usw.)
        private string CleanBelag(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // 1. "Vergüten" entfernen, egal wie geschrieben (Regex mit IgnoreCase)
            input = System.Text.RegularExpressions.Regex.Replace(input, "vergüten", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

            // 2. Mit Regex nach einem Belag suchen:
            // Suche nach "B" gefolgt von optional "-" und Ziffern, z.B. "B103", "B-103"
            var match = System.Text.RegularExpressions.Regex.Match(input, @"B-?\d+");

            if (match.Success)
            {
                // Gefundenen Belag zurückgeben, Bindestrich entfernen, immer großschreiben
                return match.Value.Replace("-", "").ToUpper();
            }
            else
            {
                // Wenn kein passender Belag gefunden wurde, leeres Ergebnis zurückgeben
                return "";
            }
        }

        // Methode zum Löschen von Einträgen aus der Ansicht_Bildschirm-Tabelle
        private void DeleteFromAnsichtBildschirm(string auftragsNr, string artikel)
        {
            try
            {
                // SQL-Abfrage zum Löschen der Daten aus der Ansicht_Bildschirm-Tabelle
                string deleteQuery = @"
                    DELETE FROM Ansicht_Bildschirm
                    WHERE Auftrag = @Auftrag
                    AND Teilenummer = @Teilenummer";

                // SQL-Befehl vorbereiten und Parameter hinzufügen
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnectionVerwaltung))
                {
                    deleteCommand.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    deleteCommand.Parameters.AddWithValue("@Teilenummer", artikel);

                    // Verbindung öffnen und Daten löschen
                    sqlConnectionVerwaltung.Open();
                    deleteCommand.ExecuteNonQuery();
                    sqlConnectionVerwaltung.Close();
                }

                // SQL-Abfrage zum Löschen der Daten aus der Ainsicht_Bildschirm-Tabelle
                string deleteQueryAlt = @"
                    DELETE FROM Ainsicht_Bildschirm
                    WHERE Auftrag = @Auftrag
                    AND Artikel = @Artikel";

                // SQL-Befehl vorbereiten und Parameter hinzufügen
                using (SqlCommand deleteCommandAlt = new SqlCommand(deleteQueryAlt, sqlConnectionVerwaltungAlt))
                {
                    deleteCommandAlt.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    deleteCommandAlt.Parameters.AddWithValue("@Artikel", artikel);

                    // Verbindung öffnen und Daten löschen
                    sqlConnectionVerwaltungAlt.Open();
                    deleteCommandAlt.ExecuteNonQuery();
                    sqlConnectionVerwaltungAlt.Close();
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
                MessageBox.Show($"Fehler beim Löschen der Daten: {ex.Message}");
            }
            finally
            {
                // Sicherstellen, dass die Verbindungen geschlossen werden
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
                if (sqlConnectionVerwaltungAlt.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltungAlt.Close();
                }
            }
        }

        // Event-Handler für das Klicken auf eine Zelle im DataGridView dGvAnsichtAuftraege
        private void DgvAnsichtAuftraege_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Überprüfen, ob eine Zeile ausgewählt ist
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                // Die ausgewählte Zeile abrufen
                DataGridViewRow selectedRow = DgvAnsichtAuftraege.CurrentRow;

                // Überprüfen, ob die Zellen "Artikel" und "Seite" nicht null oder leer sind
                if (selectedRow.Cells["Artikel"].Value != null && !string.IsNullOrEmpty(selectedRow.Cells["Artikel"].Value.ToString()) &&
                    selectedRow.Cells["Seite"].Value != null && !string.IsNullOrEmpty(selectedRow.Cells["Seite"].Value.ToString()))
                {
                    // Artikelnummer und Seite aus der ausgewählten Zeile abrufen
                    string artikelNr = selectedRow.Cells["Artikel"].Value.ToString();
                    int seite;
                    if (int.TryParse(selectedRow.Cells["Seite"].Value.ToString(), out seite))
                    {
                        // Zeichnungspfad abrufen
                        string zeichnungspfad = GetZeichnungspfad(artikelNr, seite);

                        // Wenn der Zeichnungspfad nicht leer ist, das Bild in der PictureBox anzeigen
                        if (!string.IsNullOrEmpty(zeichnungspfad))
                        {
                            PictureBoxZeichnung.ImageLocation = zeichnungspfad;
                        }
                        else
                        {
                            // Optional: Wenn kein Bild gefunden wird, ein Platzhalterbild oder eine Nachricht setzen
                            PictureBoxZeichnung.Image = null; // oder ein Standardbild setzen
                        }
                    }
                }
            }
            // Überprüfen, ob der Klick in einer gültigen Zeile stattgefunden hat
            if (e.RowIndex >= 0)
            {
                // Auftragsnummer aus der geklickten Zeile abrufen
                var auftragsNummer = DgvAnsichtAuftraege.Rows[e.RowIndex].Cells["Auftragsnr."].Value?.ToString();

                // Nur wenn die Auftragsnummer nicht null oder leer ist, die Datenbankabfrage starten
                if (!string.IsNullOrEmpty(auftragsNummer))
                {
                    // Funktion aufrufen, die die SQL-Datenbank abfragt und das DataGridView aktualisiert
                    UpdateDgvInformationZuAuftrag(auftragsNummer);
                }
            }
        }

        // Wenn auf eine Reihe doppelt geklickt wird, öffnet sich das Formular Form_Druckuebersiht und gewisse Werte werden übergeben
        private void DgvAnsichtAuftraege_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Überprüfen, ob eine gültige Zeile ausgewählt ist
            if (e.RowIndex >= 0)
            {
                // Die ausgewählte Zeile abrufen
                DataGridViewRow selectedRow = DgvAnsichtAuftraege.Rows[e.RowIndex];

                // Werte aus der ausgewählten Zeile abrufen
                string enddatum = selectedRow.Cells["Enddatum"].Value?.ToString();
                string teilebez = selectedRow.Cells["Teilebez."].Value?.ToString();
                string auftragsNr = selectedRow.Cells["Auftragsnr."].Value?.ToString();
                string artikel = selectedRow.Cells["Artikel"].Value?.ToString();
                string status = selectedRow.Cells["Status"].Value?.ToString();
                string avoInfo = selectedRow.Cells["AVOinfo"].Value?.ToString();
                string material = selectedRow.Cells["Material"].Value?.ToString();
                string seite = selectedRow.Cells["Seite"].Value?.ToString();
                string sollStk = Convert.ToDecimal(selectedRow.Cells["SollStk."].Value).ToString("0");
                string istStk = selectedRow.Cells["IstStk."].Value?.ToString();
                string vorStk = selectedRow.Cells["VorStk."].Value?.ToString();
                string teilelager = selectedRow.Cells["Teilelager"].Value?.ToString();
                string bereitstell = selectedRow.Cells["Bereitstell"].Value?.ToString();
                string jahresbedarf = selectedRow.Cells["Jahresbedarf"].Value?.ToString();
                string zukauf = selectedRow.Cells["Zukauf"].Value?.ToString();
                string dringend = selectedRow.Cells["Dringend"].Value?.ToString();
                string aktualisiert = selectedRow.Cells["Aktualisiert"].Value?.ToString();

                // Neues Formular Form_Druckübersicht öffnen und Werte übergeben
                Form_Druckuebersicht druckuebersichtForm = new Form_Druckuebersicht(
                    enddatum, teilebez, auftragsNr, artikel, status, avoInfo, material, seite, sollStk, istStk, vorStk, teilelager, bereitstell, jahresbedarf, zukauf, dringend, aktualisiert, selectedBelagValue);
                druckuebersichtForm.ShowDialog();
            }
        }

        // Grafische gestaltung des Dgvs
        private void DgvAnsichtAuftraege_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Wenn in der Reihe Status Active steht, dann soll die ganze Spalte blau hinterlegt werden
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Status")
            {
                if (e.Value != null && e.Value.ToString() == "Active")
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    //dGvAnsichtAuswahlAuftrag.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }

            // Überprüfen, ob es sich um eine Spalte handelt
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Status")
            {
                // Überprüfen, ob der Status "Gestartet" ist
                if (e.Value != null && e.Value.ToString() == "Gestartet")
                {
                    e.CellStyle.BackColor = Color.LightSkyBlue;
                    //dGvAnsichtAuswahlAuftrag.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }

            // Überprüfe, ob in der Reihe ZU etwas steht
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Zukauf")
            {
                if (e.Value != null && e.Value.ToString() == "R-Lager")
                {
                    e.CellStyle.BackColor = Color.GreenYellow;
                }
                else if (e.Value != null && e.Value.ToString() == "T-Lager")
                {
                    e.CellStyle.BackColor = Color.Salmon;
                }
            }

            // Überprüfe, ob in der Reihe dringend etwas steht
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Dringend")
            {
                if (e.Value != null && e.Value.ToString() == "1")
                {
                    e.CellStyle.BackColor = Color.Orange;
                }
                else if (e.Value != null && e.Value.ToString() == "2")
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
            }

            // Sicherstellen, dass Zeile und Spalte gültig sind und der Wert nicht null ist
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                // Prüfen, ob es sich um die Spalte "Enddatum" handelt
                if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Enddatum") // Oder .HeaderText je nach Konfiguration
                {
                    DateTime endDate;
                    // Versuchen, den Zellwert als DateTime zu parsen
                    if (DateTime.TryParse(e.Value.ToString(), out endDate))
                    {
                        // Datum der Zelle mit dem heutigen Datum vergleichen (nur das Datum, ohne Zeit)
                        if (endDate.Date < DateTime.Today)
                        {
                            // Wenn das Datum in der Vergangenheit liegt, Hintergrundfarbe der Zelle auf Hellgrau setzen
                            e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                        }
                        else
                        {
                            // Wenn das Datum nicht in der Vergangenheit liegt, sicherstellen, dass die Hintergrundfarbe Standard ist
                            // Dies ist wichtig, da Zellen wiederverwendet werden (Cell Recycling)
                            e.CellStyle.BackColor = DgvAnsichtAuftraege.DefaultCellStyle.BackColor;
                        }
                    }
                }
            }

            // --- Auftragsnummern hervorheben, die in der JSON-Datei gespeichert sind ---
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Auftragsnr." && e.Value != null)
            {
                string auftragsNummer = e.Value.ToString();
                string jsonPfad = "letzteAuftragsnummern.json";

                if (File.Exists(jsonPfad))
                {
                    try
                    {
                        string jsonInhalt = File.ReadAllText(jsonPfad);
                        List<string> gespeicherteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();

                        if (gespeicherteNummern.Contains(auftragsNummer))
                        {
                            e.CellStyle.BackColor = Color.SkyBlue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Lesen der JSON-Datei: {ex.Message}");
                    }
                }
            }

            // --- Schriftfarbe in "Teilelager" ändern, wenn Wert kleiner als "Bereitstell" ---
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Teilelager")
            {
                object teilelagerWertObj = e.Value;
                object bereitstellWertObj = DgvAnsichtAuftraege.Rows[e.RowIndex].Cells["Bereitstell"].Value;

                if (bereitstellWertObj != null && teilelagerWertObj != null &&
                    double.TryParse(teilelagerWertObj.ToString(), out double teilelagerWert) &&
                    double.TryParse(bereitstellWertObj.ToString(), out double bereitstellWert))
                {
                    if (teilelagerWert < bereitstellWert)
                    {
                        e.CellStyle.ForeColor = Color.DarkRed;
                        e.CellStyle.BackColor = Color.LightSalmon;
                    }
                }
            }
        }

        private void DgvAnsichtAuftraege_MouseDown(object sender, MouseEventArgs e)
        {
            // Überprüfen, ob die rechte Maustaste gedrückt wurde
            if (e.Button == MouseButtons.Right)
            {
                // Ermitteln, welche Zelle unter dem Mauszeiger liegt
                var hti = DgvAnsichtAuftraege.HitTest(e.X, e.Y);

                // Auswahl im DataGridView löschen
                DgvAnsichtAuftraege.ClearSelection();

                // Überprüfen, ob eine gültige Zeile getroffen wurde
                if (hti.RowIndex >= 0)
                {
                    // Die getroffene Zeile auswählen
                    DgvAnsichtAuftraege.Rows[hti.RowIndex].Selected = true;
                    // Kontextmenü erstellen
                    ContextMenuStrip contextMenu = new ContextMenuStrip();
                    // Menüeinträge erstellen
                    ToolStripMenuItem setzeDringend1 = new ToolStripMenuItem("Setze Dringend 1");
                    ToolStripMenuItem setzeDringend2 = new ToolStripMenuItem("Setze Dringend 2");
                    ToolStripMenuItem resettDringend = new ToolStripMenuItem("Resett Dringend");
                    ToolStripMenuItem serienlinse = new ToolStripMenuItem("Serienlinse");
                    ToolStripMenuItem resettAuftrag = new ToolStripMenuItem("Auftrag abschließen");
                    // Event-Handler für die Menüeinträge hinzufügen
                    setzeDringend1.Click += SetzeDringend1_Click;
                    setzeDringend2.Click += SetzeDringend2_Click;
                    resettDringend.Click += ResettDringend_Click;
                    serienlinse.Click += Serienlinse_Click;
                    resettAuftrag.Click += ResettAuftrag_Click;
                    // Verknüpfen Sie die anderen Menüeinträge mit ihren entsprechenden Methoden, falls erforderlich
                    // Menüeinträge zum Kontextmenü hinzufügen
                    contextMenu.Items.AddRange(new ToolStripItem[] { setzeDringend1, setzeDringend2, resettDringend, serienlinse, resettAuftrag });
                    // Kontextmenü an der Position des Mauszeigers anzeigen
                    contextMenu.Show(DgvAnsichtAuftraege, e.Location);
                }
            }
        }

        // Event-Handler für das Klicken auf eine Zelle im DataGridView dGvLadeBelaege
        private void DgvLadeBelaege_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Überprüfen, ob der Klick in einer gültigen Zeile stattgefunden hat
            if (e.RowIndex >= 0)
            {
                // Wert, der bei Belag steht, abrufen
                selectedBelagValue = DgvLadeBelaege.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
                // Nur falls der Wert nicht null ist, wird er weiterverarbeitet
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuftraege(selectedBelagValue);
                }
            }
        }

        // Funktion zum Abrufen des Materials basierend auf mitm_teilenr und Seite
        private string GetMaterialFromSerienlinsen(string mitmTeilenNr, int seite)
        {
            string material = string.Empty;

            try
            {
                string query = @"
                    SELECT MATERIAL
                    FROM Serienlinsen
                    WHERE ARTNR = @MitmTeilenNr
                    AND SEITE = @Seite";

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@MitmTeilenNr", mitmTeilenNr);
                    command.Parameters.AddWithValue("@Seite", seite);

                    sqlConnectionVerwaltung.Open();
                    object result = command.ExecuteScalar();
                    sqlConnectionVerwaltung.Close();

                    if (result != null)
                    {
                        material = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Abrufen des Materials: {ex.Message}");
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }

            return material;
        }

        // Methode, um den Zeichnungspfad zu erhalten
        private string GetZeichnungspfad(string artikelNr, int seite)
        {
            string zeichnungspfad = string.Empty;
            try // Versuche, den Zeichnungspfad aus der Datenbank abzurufen
            {
                string query = @"
                    SELECT Zeichnungspfad
                    FROM Serienlinsen
                    WHERE ARTNR = @ArtikelNr AND Seite = @Seite";

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@ArtikelNr", artikelNr); // Setze den Parameter für die Artikelnummer
                    command.Parameters.AddWithValue("@Seite", seite); // Setze den Parameter für die Seite

                    sqlConnectionVerwaltung.Open();
                    var result = command.ExecuteScalar(); // Führe den SQL-Befehl aus und speichere das Ergebnis
                    if (result != null) // Überprüfe, ob das Ergebnis nicht null ist
                    {
                        zeichnungspfad = result.ToString(); // Setze den Zeichnungspfad auf den Wert aus der Datenbank
                        lblPfad.Text = zeichnungspfad; // Setze den Text des Labels auf den Zeichnungspfad
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Abrufen des Zeichnungspfads: {ex.Message}");
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
            return zeichnungspfad;
        }

        // Funktion, wenn die Auftragsverwaltung geöffnet wird
        private void LoadDataForDgvAuftragZuBelag()
        {
            try
            {
                // --- 1. SQL-Abfrage ---
                // Hole aus der Datenbank alle Aufträge, deren AVO-Info "Vergüten" enthält,
                // gruppiert nach txta_avoinfo, und zähle jeweils die Anzahl der Aufträge (AVOs).
                string query = @"
                SELECT
                    txta_avoinfo AS Belag,
                COUNT(DISTINCT pdno_prodnr) AS AVOs
                FROM
                    LN_ProdOrders_PRD
                WHERE
                    opsta_avostat IN ('Active', 'Planned', 'Released')
                AND txta_avoinfo LIKE '%Vergüten%'
                GROUP BY
                    txta_avoinfo
                HAVING
                COUNT(DISTINCT pdno_prodnr) > 0
                ORDER BY
                    Belag ASC;
                ";

                SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    // Öffne die Datenbankverbindung und fülle das DataTable mit den Ergebnissen
                    sqlConnectionVerwaltung.Open();
                    adapter.Fill(dataTable);
                }
                catch (SqlException sqlEx)
                {
                    // Fehlerbehandlung bei Problemen mit der SQL-Abfrage
                    MessageBox.Show($"SQL Fehler beim Laden der Belag-Daten: {sqlEx.Message}", "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    // Stelle sicher, dass die Verbindung am Ende wieder geschlossen wird
                    if (sqlConnectionVerwaltung != null && sqlConnectionVerwaltung.State == ConnectionState.Open)
                    {
                        sqlConnectionVerwaltung.Close();
                    }
                }

                // --- 2. Nachbearbeitung der Daten ---
                // Ziel: Belag-Namen vereinheitlichen und gleiche Beläge zusammenfassen (z.B. B103 == B-103)

                // Erzeuge ein Dictionary, um Beläge zu sammeln: Key = gereinigter Belag (z.B. "B103"), Value = Summe AVOs
                Dictionary<string, int> belagCounts = new Dictionary<string, int>();

                foreach (DataRow row in dataTable.Rows)
                {
                    // Originaler Belag-Text aus der Datenbank
                    string originalBelag = row["Belag"].ToString();
                    int avos = Convert.ToInt32(row["AVOs"]);

                    // Belag-Text bereinigen (siehe CleanBelag-Methode unten)
                    string cleanedBelag = CleanBelag(originalBelag);

                    // Wenn kein gültiger Belag gefunden wurde, überspringen
                    if (string.IsNullOrEmpty(cleanedBelag))
                        continue;

                    // Belag in Dictionary einfügen oder AVOs dazu addieren, falls schon vorhanden
                    if (!belagCounts.ContainsKey(cleanedBelag))
                    {
                        belagCounts[cleanedBelag] = 0;
                    }
                    belagCounts[cleanedBelag] += avos;
                }

                // --- 3. Neue bereinigte DataTable aufbauen ---
                DataTable cleanedTable = new DataTable();
                cleanedTable.Columns.Add("Belag", typeof(string)); // Spalte für Belag (z.B. "B103")
                cleanedTable.Columns.Add("AVOs", typeof(int));      // Spalte für Anzahl AVOs

                // Werte aus dem Dictionary in die neue Tabelle übernehmen
                foreach (var item in belagCounts)
                {
                    DataRow newRow = cleanedTable.NewRow();
                    newRow["Belag"] = item.Key;
                    newRow["AVOs"] = item.Value;
                    cleanedTable.Rows.Add(newRow);
                }

                // --- 4. Ergebnisse als JSON speichern ---
                // Ziel: Die aufbereiteten Beläge in eine JSON-Datei schreiben
                string jsonFilePath = "GeladeneBelaege.json";
                string jsonData = JsonConvert.SerializeObject(cleanedTable, Formatting.Indented); // JSON schön formatiert
                File.WriteAllText(jsonFilePath, jsonData);

                // --- 5. JSON-Datei lesen und ins DataGridView laden ---
                if (File.Exists(jsonFilePath))
                {
                    string json = File.ReadAllText(jsonFilePath);
                    DataTable jsonDataTable = JsonConvert.DeserializeObject<DataTable>(json);
                    DgvLadeBelaege.DataSource = jsonDataTable;
                }
                else
                {
                    DgvLadeBelaege.DataSource = null;
                    MessageBox.Show($"JSON-Datei '{jsonFilePath}' nicht gefunden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // --- 6. DataGridView-Optik einstellen ---
                if (DgvLadeBelaege.DataSource != null)
                {
                    DgvLadeBelaege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Spalten füllen automatisch die Breite

                    if (DgvLadeBelaege.Columns.Contains("AVOs"))
                    {
                        DgvLadeBelaege.Columns["AVOs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // AVO-Zahlen zentrieren
                        DgvLadeBelaege.Columns["AVOs"].DefaultCellStyle.Format = "N0"; // Zahlenformat ohne Dezimalstellen
                    }

                    // Alle Spaltenüberschriften zentrieren
                    foreach (DataGridViewColumn column in DgvLadeBelaege.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                // Fehlerbehandlung bei Problemen mit JSON-Verarbeitung
                MessageBox.Show($"JSON Fehler beim Laden der Belag-Daten: {jsonEx.Message}", "JSON Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DgvLadeBelaege.DataSource = null;
            }
            catch (Exception ex)
            {
                // Allgemeine Fehlerbehandlung
                MessageBox.Show($"Allgemeiner Fehler in LoadDataForDgvAuftragZuBelag: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DgvLadeBelaege.DataSource = null;
            }
        }

        // Wenn auf Resett Auftrag geklickt wird, wird die Auftragsnummer aus der JSON-Datei gelöscht
        private void ResettAuftrag_Click(object sender, EventArgs e)
        {
            // Die ausgewählte Zeile aus dem DataGridView holen
            var selectedRow = DgvAnsichtAuftraege.SelectedRows[0];

            // Die Auftragsnummer aus der Zelle "Auftragsnr." holen
            string auftragsNummer = selectedRow.Cells["Auftragsnr."].Value.ToString();

            string jsonPfad = "letzteAuftragsnummern.json";

            // Überprüfen, ob die JSON-Datei existiert
            if (File.Exists(jsonPfad))
            {
                try
                {
                    // Den Inhalt der JSON-Datei lesen
                    string jsonInhalt = File.ReadAllText(jsonPfad);

                    // Die Auftragsnummern aus der JSON-Datei deserialisieren
                    List<string> gespeicherteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();

                    // Prüfen, ob die Auftragsnummer in der Liste enthalten ist
                    if (gespeicherteNummern.Contains(auftragsNummer))
                    {
                        // Die Auftragsnummer aus der Liste entfernen
                        gespeicherteNummern.Remove(auftragsNummer);

                        // Die aktualisierte Liste wieder in die JSON-Datei schreiben
                        string neuerJsonInhalt = JsonConvert.SerializeObject(gespeicherteNummern, Formatting.Indented);
                        File.WriteAllText(jsonPfad, neuerJsonInhalt);

                        // Optional: Nachricht anzeigen, dass die Auftragsnummer gelöscht wurde
                        MessageBox.Show($"Auftragsnummer {auftragsNummer} wurde aus der gestarteten Auftragsliste gelöscht.", "Auftrag abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateDgvAnsichtAuftraege2();
                    }
                    else
                    {
                        // Optional: Nachricht anzeigen, falls die Auftragsnummer nicht in der Datei gefunden wurde
                        MessageBox.Show($"Auftragsnummer {auftragsNummer} wurde nicht in der gestarteten Auftragsliste gefunden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung
                    MessageBox.Show($"Fehler beim Bearbeiten der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Optional: Nachricht anzeigen, falls die JSON-Datei nicht existiert
                MessageBox.Show($"Die Datei '{jsonPfad}' existiert nicht.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Wenn auf Resett Dringend geklickt wird dann wird die ausgewählte Zeile aus Ainsicht_Bildschirm_Shopfloor und Ansicht_Bildschirm gelöscht
        private void ResettDringend_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile im DataGridView ausgewählt ist
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                // Auftragsnummer aus der ausgewählten Zeile abrufen
                string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
                // Artikelnummer aus der ausgewählten Zeile abrufen
                string artikel = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
                // Überprüfen, ob Auftragsnummer und Artikelnummer nicht null oder leer sind
                if (!string.IsNullOrEmpty(auftragsNr) && !string.IsNullOrEmpty(artikel))
                {
                    // Methode aufrufen, um die Daten in die Ansicht_Bildschirm-Tabelle zu speichern
                    DeleteFromAnsichtBildschirm(auftragsNr, artikel);
                    UpdateDgvAnsichtAuftraege2();
                }
            }
        }

        // Methode zum Speichern von Daten in die Ansicht_Bildschirm-Tabelle
        private void SaveToAnsichtBildschirm(string auftragsNr, string artikel, string dringendWert)
        {
            try
            {
                // SQL-Abfrage, um die Daten aus der Serienlinsen-Tabelle abzurufen
                string query = @"
        SELECT SEITE, Belag1
        FROM Serienlinsen
        WHERE ARTNR = @Artikel
        AND Status = 'Serie'";

                // Dictionary zum Speichern der abgerufenen Daten
                Dictionary<int, string> belagDict = new Dictionary<int, string>();

                // SQL-Befehl vorbereiten und Parameter hinzufügen
                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltungAlt))
                {
                    command.Parameters.AddWithValue("@Artikel", artikel);

                    // Verbindung öffnen und Daten abrufen
                    sqlConnectionVerwaltungAlt.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Daten aus der Datenbank abrufen und im Dictionary speichern
                            string seite = reader.GetString(reader.GetOrdinal("SEITE"));
                            string belag1 = reader.GetString(reader.GetOrdinal("Belag1"));
                            belagDict[int.Parse(seite)] = belag1;
                        }
                    }
                    sqlConnectionVerwaltungAlt.Close();
                }
                // Werte für die Seiten abrufen oder Standardwerte setzen
                string seite1 = belagDict.ContainsKey(1) ? belagDict[1] : "-";
                string seite2 = belagDict.ContainsKey(2) ? belagDict[2] : "-";
                string seite0 = belagDict.ContainsKey(0) ? belagDict[0] : "-";
                // SQL-Abfrage zum Einfügen der Daten in die Ainsicht_Bildschirm-Tabelle
                string insertQueryAlt = @"
        INSERT INTO Ainsicht_Bildschirm (Auftrag, Artikel, Seite_1, Seite_2, Seite_0, Dringend)
        VALUES (@Auftrag, @Artikel, @Seite1, @Seite2, @Seite0, @Dringend)";

                // SQL-Befehl vorbereiten und Parameter hinzufügen
                using (SqlCommand insertCommand = new SqlCommand(insertQueryAlt, sqlConnectionVerwaltungAlt))
                {
                    insertCommand.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    insertCommand.Parameters.AddWithValue("@Artikel", artikel);
                    insertCommand.Parameters.AddWithValue("@Seite1", seite1);
                    insertCommand.Parameters.AddWithValue("@Seite2", seite2);
                    insertCommand.Parameters.AddWithValue("@Seite0", seite0);
                    insertCommand.Parameters.AddWithValue("@Dringend", dringendWert);
                    // Verbindung öffnen und Daten einfügen
                    sqlConnectionVerwaltungAlt.Open();
                    insertCommand.ExecuteNonQuery();
                    sqlConnectionVerwaltungAlt.Close();
                }

                // SQL-Abfrage zum Einfügen der Daten in die Ansicht_Bildschirm-Tabelle
                string insertQuery = @"
        INSERT INTO Ansicht_Bildschirm (Auftrag, Teilenummer, Seite_1, Seite_2, Seite_0, Dringend)
        VALUES (@Auftrag, @Teilenummer, @Seite1, @Seite2, @Seite0, @Dringend)";

                // SQL-Befehl vorbereiten und Parameter hinzufügen
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnectionVerwaltung))
                {
                    insertCommand.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    insertCommand.Parameters.AddWithValue("@Teilenummer", artikel);
                    insertCommand.Parameters.AddWithValue("@Seite1", seite1);
                    insertCommand.Parameters.AddWithValue("@Seite2", seite2);
                    insertCommand.Parameters.AddWithValue("@Seite0", seite0);
                    insertCommand.Parameters.AddWithValue("@Dringend", dringendWert);

                    // Verbindung öffnen und Daten einfügen
                    sqlConnectionVerwaltung.Open();
                    insertCommand.ExecuteNonQuery();
                    sqlConnectionVerwaltung.Close();
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
                MessageBox.Show($"Fehler beim Speichern der Daten: {ex.Message}");
            }
            finally
            {
                // Sicherstellen, dass die Verbindungen geschlossen werden
                if (sqlConnectionVerwaltungAlt.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltungAlt.Close();
                }
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Wenn auf Serienlinse geklickt wird geht das Fenster mit allen Informationen zu der Serienlinse auf
        private void Serienlinse_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile im DataGridView ausgewählt ist
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                // Artikelnummer und Seite aus der ausgewählten Zeile abrufen
                string artikelNr = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
                string seite = DgvAnsichtAuftraege.CurrentRow.Cells["Seite"].Value?.ToString();

                // Überprüfen, ob Artikelnummer und Seite nicht null oder leer sind
                if (!string.IsNullOrEmpty(artikelNr) && !string.IsNullOrEmpty(seite))
                {
                    // Das Formular öffnen und die Artikelnummer und Seite übergeben
                    Form_ArtikelPrototypAendern artikelPrototypAendern = new Form_ArtikelPrototypAendern(artikelNr, seite);
                    artikelPrototypAendern.ShowDialog();
                }
            }
        }

        // Wenn auf Setze Dringend 1 geklickt wird
        private void SetzeDringend1_Click(object sender, EventArgs e)
        {
            // Auftragsnummer aus der ausgewählten Zeile abrufen
            string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
            // Artikelnummer aus der ausgewählten Zeile abrufen
            string artikel = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
            // Dringend-Status aus der ausgewählten Zeile abrufen
            string dringendStatus = DgvAnsichtAuftraege.CurrentRow.Cells["Dringend"].Value?.ToString();

            // Überprüfen, ob Auftragsnummer und Artikelnummer nicht null oder leer sind
            if (!string.IsNullOrEmpty(auftragsNr) && !string.IsNullOrEmpty(artikel))
            {
                // Überprüfen, ob bereits ein Dringend-Status gesetzt ist
                if (dringendStatus == "1" || dringendStatus == "2")
                {
                    // Warnung anzeigen
                    MessageBox.Show("Es ist bereits ein Dringend-Status gesetzt. Bitte setzen Sie den Status zurück, bevor Sie einen neuen Dringend-Status setzen.", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Methode aufrufen, um die Daten in die Ansicht_Bildschirm-Tabelle zu speichern
                    SaveToAnsichtBildschirm(auftragsNr, artikel, "1");
                    UpdateDgvAnsichtAuftraege2();
                }
            }
        }

        // Wenn auf Setze Dringend 2 geklickt wird
        private void SetzeDringend2_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile im DataGridView ausgewählt ist
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                // Auftragsnummer aus der ausgewählten Zeile abrufen
                string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
                // Artikelnummer aus der ausgewählten Zeile abrufen
                string artikel = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
                // Dringend-Status aus der ausgewählten Zeile abrufen
                string dringendStatus = DgvAnsichtAuftraege.CurrentRow.Cells["Dringend"].Value?.ToString();

                // Überprüfen, ob Auftragsnummer und Artikelnummer nicht null oder leer sind
                if (!string.IsNullOrEmpty(auftragsNr) && !string.IsNullOrEmpty(artikel))
                {
                    // Überprüfen, ob bereits ein Dringend-Status gesetzt ist
                    if (dringendStatus == "1" || dringendStatus == "2")
                    {
                        // Warnung anzeigen
                        MessageBox.Show("Es ist bereits ein Dringend-Status gesetzt. Bitte setzen Sie den Status zurück, bevor Sie einen neuen Dringend-Status setzen.", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Methode aufrufen, um die Daten in die Ansicht_Bildschirm-Tabelle zu speichern
                        SaveToAnsichtBildschirm(auftragsNr, artikel, "2");
                        UpdateDgvAnsichtAuftraege2();
                    }
                }
            }
        }

        // Funktion zum Aktualisieren des DataGridView dGvAnsichtAuftraege basierend auf der Auswahl in dGvLadeBelaege
        private void UpdateDgvAnsichtAuftraege(string selectedBelagValue)
        {
            try
            {
                // Zuerst die mögliche Varianten des Belags vorbereiten: mit und ohne Bindestrich, Groß- und Kleinschreibung ignorieren
                // Beispiel: selectedBelagValue = "B103"
                // Gesuchte Varianten: '%B103%', '%B-103%', '%b103%', '%b-103%'
                string belagOhneBindestrich = selectedBelagValue.ToUpper();
                string belagMitBindestrich = belagOhneBindestrich.Insert(1, "-");

                // SQL-Abfrage anpassen: Suche nach beiden Varianten (Bindestrich und ohne Bindestrich) und case-insensitive (durch COLLATE)
                string query = $@"
SELECT
    CONVERT(date, trdf_enddate) AS Enddatum,
    dsca_teilebez AS [Teilebez.],
    pdno_prodnr AS [Auftragsnr.],
    mitm_teilenr AS Artikel,
    opsta_avostat AS Status,
    txta_avoinfo AS AVOinfo,
    '' AS Material, -- Spalte Material bleibt leer
    CASE
        WHEN txta_avoinfo LIKE '%III%' OR txta_avoinfo LIKE '%Iii%' OR txta_avoinfo LIKE '%IIi%' OR txta_avoinfo LIKE '%iii%' OR txta_avoinfo LIKE '%iII%' THEN '0'
        WHEN txta_avoinfo LIKE '%Ii%' OR txta_avoinfo LIKE '%iI%' OR txta_avoinfo LIKE '%ii%' OR txta_avoinfo LIKE '%II%' THEN '2'
        WHEN txta_avoinfo LIKE '%i%' OR txta_avoinfo LIKE '%I%' THEN '1'
        ELSE '0'
    END AS Seite,
    qplo_sollstk AS [SollStk.],
    qcmp_iststk AS [IstStk.],
    qcmp2_vorstk AS [VorStk.],
    qhnd1_stk_teilelager AS Teilelager,
    qana_bereitstellbestand AS Bereitstell,
    demand_jahresbedarf AS Jahresbedarf,
    '' AS Zukauf, -- aktuell leer
    '' AS Dringend, -- aktuell leer
    CONVERT(date, import_date) AS Aktualisiert
FROM
    LN_ProdOrders_PRD
WHERE
    opsta_avostat IN ('Active', 'Planned', 'Released')
    AND (
         txta_avoinfo COLLATE Latin1_General_CI_AS LIKE @BelagValue1
         OR
         txta_avoinfo COLLATE Latin1_General_CI_AS LIKE @BelagValue2
    )
    AND txta_avoinfo LIKE '%Vergüten%'
ORDER BY
    trdf_enddate ASC;";

                // SQL-Befehl vorbereiten
                SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung);

                // Parameter befüllen
                command.Parameters.AddWithValue("@BelagValue1", "%" + belagOhneBindestrich + "%");
                command.Parameters.AddWithValue("@BelagValue2", "%" + belagMitBindestrich + "%");

                // Datenadapter und DataTable initialisieren
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable(); // Lokale DataTable zum Befüllen

                // Verbindung öffnen und Daten laden
                sqlConnectionVerwaltung.Open();
                adapter.Fill(dataTable);
                sqlConnectionVerwaltung.Close();

                // Material für jede Zeile setzen
                foreach (DataRow row in dataTable.Rows)
                {
                    string artikelNr = row["Artikel"].ToString();
                    int seite = int.Parse(row["Seite"].ToString());
                    string material = GetMaterialFromSerienlinsen(artikelNr, seite); // eigene Funktion
                    row["Material"] = material;
                }

                // RL/TL Zukauf-Daten aus JSON laden
                RLTLData rltlData;
                string jsonFilePath2 = "rltl_data.json";
                if (File.Exists(jsonFilePath2))
                {
                    var json = File.ReadAllText(jsonFilePath2);
                    rltlData = JsonConvert.DeserializeObject<RLTLData>(json);
                }
                else
                {
                    rltlData = new RLTLData(); // Leeres Objekt
                }

                // Zukauf-Spalte basierend auf RL/TL-Liste setzen
                foreach (DataRow row in dataTable.Rows)
                {
                    string artikelNr = row["Artikel"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(artikelNr))
                    {
                        if (rltlData.RL != null && rltlData.RL.Contains(artikelNr))
                        {
                            row["Zukauf"] = "R-Lager";
                        }
                        else if (rltlData.TL != null && rltlData.TL.Contains(artikelNr))
                        {
                            row["Zukauf"] = "T-Lager";
                        }
                    }
                }

                // Dringend-Informationen laden
                string dringendQuery = @"SELECT Auftrag, Dringend FROM Ansicht_Bildschirm";
                SqlCommand dringendCommand = new SqlCommand(dringendQuery, sqlConnectionVerwaltung);
                SqlDataAdapter dringendAdapter = new SqlDataAdapter(dringendCommand);
                DataTable dringendTable = new DataTable();

                sqlConnectionVerwaltung.Open();
                dringendAdapter.Fill(dringendTable);
                sqlConnectionVerwaltung.Close();

                var dringendDict = dringendTable.AsEnumerable()
                    .ToDictionary(row => row.Field<string>("Auftrag"),
                                  row => row.Field<string>("Dringend"));

                // Dringend-Wert in DataTable setzen
                foreach (DataRow row in dataTable.Rows)
                {
                    string auftragsNr = row["Auftragsnr."].ToString();
                    if (dringendDict.TryGetValue(auftragsNr, out string dringendWert))
                    {
                        row["Dringend"] = dringendWert;
                    }
                    else
                    {
                        row["Dringend"] = string.Empty;
                    }
                }

                // Aufbereitete Daten speichern
                _auftraegeDataTable = dataTable;

                // JSON speichern (optional)
                string jsonFilePath = "AnsichtAuftraege.json";
                string jsonData = JsonConvert.SerializeObject(_auftraegeDataTable, Formatting.Indented);
                File.WriteAllText(jsonFilePath, jsonData);

                // Zeige die Daten im DataGridView
                ApplyFilterAndDisplayData();

                // Bildbox resetten
                PictureBoxZeichnung.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler in UpdateDgvAnsichtAuftraege: " + ex.Message);
                _auftraegeDataTable = null;
                DgvAnsichtAuftraege.DataSource = null;
            }
            finally
            {
                if (sqlConnectionVerwaltung != null && sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Hilfsfunktion um das DgvAnsichtAuftraege zu aktualisieren
        private void UpdateDgvAnsichtAuftraege2()
        {
            if (DgvLadeBelaege.CurrentRow != null)
            {
                // Wert, der bei Belag steht, abrufen
                selectedBelagValue = DgvLadeBelaege.CurrentRow.Cells["Belag"].Value?.ToString();
                // Nur falls der Wert nicht null ist, wird er weiterverarbeitet
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuftraege(selectedBelagValue);
                }
            }
        }

        // Methode zum Abrufen der Daten zu einem Auftrag und zur Aktualisierung des DataGridViews
        private void UpdateDgvInformationZuAuftrag(string auftragsNummer)
        {
            // SQL-Abfrage zur Auswahl aller Datensätze mit passender Auftragsnummer
            string query = "SELECT * FROM LN_ProdOrders_PRD WHERE pdno_prodnr = @Auftragsnummer";

            DataTable dataTable = new DataTable();

            try
            {
                // SQL-Befehl mit Parameter zum Schutz vor SQL-Injection
                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@Auftragsnummer", auftragsNummer);

                    // Adapter initialisieren und Daten abrufen
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }

                // Daten im DataGridView anzeigen
                DgvInformationZuAuftrag.DataSource = dataTable;

                // Nicht relevante Spalten ausblenden
                string[] auszublendendeSpalten = new string[]
                {
            "pdsta_prodstat", "refo_avotext", "mtyp_anlage", "trdt_startdate", "cwoc_kst",
            "opno2_voravo", "rutm2_sollzeit", "sutm_sollruest", "rutm_vorgabezeit",
            "dsca_teilebez", "srce_itemquelle", "goca_warentraeger", "pkdf_transportbehaelter",
            "qhnd1_stk_teilelager", "qhnd2_lagerbestand", "qana_bereitstellbestand",
            "demand_jahresbedarf", "created_date", "Last_modified", "import_date"
                };

                foreach (string spaltenName in auszublendendeSpalten)
                {
                    if (DgvInformationZuAuftrag.Columns.Contains(spaltenName))
                    {
                        DgvInformationZuAuftrag.Columns[spaltenName].Visible = false;
                    }
                }

                // Formatierung: Datum
                if (DgvInformationZuAuftrag.Columns.Contains("trdf_enddate"))
                {
                    DgvInformationZuAuftrag.Columns["trdf_enddate"].DefaultCellStyle.Format = "dd.MM.yyyy";
                }

                // Formatierung: Zahlen ohne Nachkommastellen
                string[] zahlenspalten = { "qplo_sollstk", "qcmp_iststk", "qcmp2_vorstk" };
                foreach (string spalte in zahlenspalten)
                {
                    if (DgvInformationZuAuftrag.Columns.Contains(spalte))
                    {
                        DgvInformationZuAuftrag.Columns[spalte].DefaultCellStyle.Format = "N0";
                    }
                }

                // Spaltenbreite automatisch an Inhalt anpassen
                DgvInformationZuAuftrag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Fehlerausgabe
                MessageBox.Show("Fehler beim Abrufen der Daten: " + ex.Message);
            }
            finally
            {
                // Verbindung sicher schließen
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }


        // Zählt die gestarteten Aufträge die in der Json Datei stehen
        private void ZaehleGestarteteAuftraege()
        {
            string jsonPfad = "letzteAuftragsnummern.json";

            if (File.Exists(jsonPfad))
            {
                try
                {
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    List<string> auftragsnummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();

                    // Eindeutige Auftragsnummern zählen
                    int anzahlEindeutig = auftragsnummern.Distinct().Count();

                    // Ergebnis im Label anzeigen
                    lblGestarteAuftraege.Text = anzahlEindeutig.ToString();
                }
                catch (Exception ex)
                {
                    lblGestarteAuftraege.Text = "Fehler beim Lesen der Datei";
                    Console.WriteLine($"Fehler beim Lesen der JSON-Datei: {ex.Message}");
                }
            }
            else
            {
                lblGestarteAuftraege.Text = "Keine JSON-Datei vorhanden.";
            }
        }
    }
}
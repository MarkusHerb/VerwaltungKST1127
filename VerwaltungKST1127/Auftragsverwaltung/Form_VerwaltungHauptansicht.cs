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
using VerwaltungKST1127.Produktionsauswertung;

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

        // Funktion zum Zählen der "Gestartet" Aufträge
        private void ApplyFilterAndDisplayData()
        {
            // Ignorierte Auftragsnummern aus JSON laden
            List<string> ignorierteNummern = new List<string>();
            string jsonPfad = "IgnorierteAuftragsnummern.json";
            if (File.Exists(jsonPfad))
            {
                try
                {
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    ignorierteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();
                }
                catch { /* Fehler ignorieren, falls Datei defekt */ }
            }

            int activeCount = 0; // Zähler für "Active" Status

            try
            {
                // Überprüfen, ob die Datenquelle null ist
                if (_auftraegeDataTable == null)
                {
                    DgvAnsichtAuftraege.DataSource = null;
                    lblGestartet.Text = "0";
                    return;
                }

                // --- Kombinierter Filter für Zukauf und ignorierte Nummern ---
                DataView dv = new DataView(_auftraegeDataTable);
                
                string filter = "";

                // Wenn die CheckBox aktiviert ist, nur Zukauf-Aufträge anzeigen
                if (checkBoxShowZukauf.Checked)
                {
                    filter = "[Zukauf] IS NOT NULL AND [Zukauf] <> ''";
                }

                // Ignorierte Auftragsnummern aus dem Filter entfernen
                if (ignorierteNummern.Count > 0)
                {
                    string ignoreFilter = string.Join("','", ignorierteNummern.Select(x => x.Replace("'", "''")));
                    if (!string.IsNullOrEmpty(filter))
                        filter += " AND ";
                    filter += $"NOT [Auftragsnr.] IN ('{ignoreFilter}')";
                }
    
                dv.RowFilter = filter;
                DgvAnsichtAuftraege.DataSource = dv;

                // --- Formatierung und Zählung ---
                if (DgvAnsichtAuftraege.DataSource != null && DgvAnsichtAuftraege.Columns.Count > 0)
                {
                    DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    string[] zahlenSpalten = { "SollStk.", "IstStk.", "VorStk.", "Teilelager", "Bereitstell", "Jahresbedarf" };
                    foreach (var spalte in zahlenSpalten)
                    {
                        if (DgvAnsichtAuftraege.Columns.Contains(spalte))
                        {
                            DgvAnsichtAuftraege.Columns[spalte].DefaultCellStyle.Format = "N0";
                            DgvAnsichtAuftraege.Columns[spalte].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                    // Spezielle Ausrichtung für Zukauf und Dringend Spalten
                    if (DgvAnsichtAuftraege.Columns.Contains("Zukauf"))
                        DgvAnsichtAuftraege.Columns["Zukauf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Dringend"))
                        DgvAnsichtAuftraege.Columns["Dringend"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Zentrierung der Header-Texte
                    foreach (DataGridViewColumn column in DgvAnsichtAuftraege.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    if (DgvAnsichtAuftraege.Columns.Contains("Teilebez."))
                        DgvAnsichtAuftraege.Columns["Teilebez."].Width = 170;
                    if (DgvAnsichtAuftraege.Columns.Contains("Seite"))
                        DgvAnsichtAuftraege.Columns["Seite"].Width = 60;
                    if (DgvAnsichtAuftraege.Columns.Contains("AVOinfo"))
                        DgvAnsichtAuftraege.Columns["AVOinfo"].Width = 140;

                    // Zählung der "Active"-Status
                    if (DgvAnsichtAuftraege.Columns.Contains("Status"))
                    {
                        foreach (DataGridViewRow row in DgvAnsichtAuftraege.Rows)
                        {
                            object cellValue = row.Cells["Status"]?.Value;
                            if (cellValue != null && cellValue.ToString().Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                activeCount++;
                            }
                        }
                    }
                }
                else
                {
                    // Keine Datenquelle gesetzt
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Anzeigen/Formatieren/Zählen der Daten: " + ex.Message);
                activeCount = 0;
            }
            finally
            {
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

                // Wenn der Column Name VorStk. , IstStk. , SollStk. ist, dann soll die Schriftart fett sein
                if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "VorStk." ||
                    DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "IstStk." ||
                    DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "SollStk.")
                {
                    e.CellStyle.Font = new Font(DgvAnsichtAuftraege.Font, FontStyle.Bold);
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
                    ToolStripMenuItem ignoreAuftrag = new ToolStripMenuItem("Auftrag ignorieren");
                    ToolStripMenuItem copyAuftragsNr = new ToolStripMenuItem("Auftragsnummer kopieren");
                    // Event-Handler für die Menüeinträge hinzufügen
                    setzeDringend1.Click += SetzeDringend1_Click;
                    setzeDringend2.Click += SetzeDringend2_Click;
                    resettDringend.Click += ResettDringend_Click;
                    serienlinse.Click += Serienlinse_Click;
                    resettAuftrag.Click += ResettAuftrag_Click;
                    ignoreAuftrag.Click += IgnoreAuftrag_Click;
                    copyAuftragsNr.Click += CopyAuftragsNr_Click;
                    // Verknüpfen Sie die anderen Menüeinträge mit ihren entsprechenden Methoden, falls erforderlich
                    // Menüeinträge zum Kontextmenü hinzufügen
                    contextMenu.Items.AddRange(new ToolStripItem[] { setzeDringend1, setzeDringend2, resettDringend, serienlinse, resettAuftrag, ignoreAuftrag, copyAuftragsNr });
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

        //////// Funktion zum Abrufen des Materials basierend auf mitm_teilenr und Seite
        //////private string GetMaterialFromSerienlinsen(string mitmTeilenNr, int seite)
        //////{
        //////    string material = string.Empty;

        //////    try
        //////    {
        //////        string query = @"
        //////            SELECT MATERIAL
        //////            FROM Serienlinsen
        //////            WHERE ARTNR = @MitmTeilenNr
        //////            AND SEITE = @Seite";

        //////        using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
        //////        {
        //////            command.Parameters.AddWithValue("@MitmTeilenNr", mitmTeilenNr);
        //////            command.Parameters.AddWithValue("@Seite", seite);

        //////            sqlConnectionVerwaltung.Open();
        //////            object result = command.ExecuteScalar();
        //////            sqlConnectionVerwaltung.Close();

        //////            if (result != null)
        //////            {
        //////                material = result.ToString();
        //////            }
        //////        }
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        MessageBox.Show($"Fehler beim Abrufen des Materials: {ex.Message}");
        //////    }
        //////    finally
        //////    {
        //////        if (sqlConnectionVerwaltung.State == ConnectionState.Open)
        //////        {
        //////            sqlConnectionVerwaltung.Close();
        //////        }
        //////    }

        //////    return material;
        //////}             !!!!!!!!!   Aktuell nicht in Verwendung !!!!!!!!!

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
                        // Label aktualisieren
                        ZaehleGestarteteAuftraege();
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

        // Wenn auf ignoreAuftrag geklickt wird, dann wird der Auftrag in einer JSON-Datei gespeichert und nicht mehr in der Tabelle angezeigt
        private void IgnoreAuftrag_Click(object sender, EventArgs e)
        {
            // Die ausgewählte Zeile aus dem DataGridView holen
            var selectedRow = DgvAnsichtAuftraege.SelectedRows[0];
            // Die Auftragsnummer aus der Zelle "Auftragsnr." holen
            string auftragsNummer = selectedRow.Cells["Auftragsnr."].Value.ToString();
            string jsonPfad = "IgnorierteAuftragsnummern.json";
            List<string> ignorierteNummern = new List<string>();
            // Überprüfen, ob die JSON-Datei existiert
            if (File.Exists(jsonPfad))
            {
                try
                {
                    // Den Inhalt der JSON-Datei lesen
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    // Die Auftragsnummern aus der JSON-Datei deserialisieren
                    ignorierteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung
                    MessageBox.Show($"Fehler beim Lesen der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // Prüfen, ob die Auftragsnummer bereits in der Liste ist
            if (!ignorierteNummern.Contains(auftragsNummer))
            {
                // Die Auftragsnummer zur Liste hinzufügen
                ignorierteNummern.Add(auftragsNummer);
                try
                {
                    // Die aktualisierte Liste wieder in die JSON-Datei schreiben
                    string neuerJsonInhalt = JsonConvert.SerializeObject(ignorierteNummern, Formatting.Indented);
                    File.WriteAllText(jsonPfad, neuerJsonInhalt);
                    // Optional: Nachricht anzeigen, dass die Auftragsnummer ignoriert wurde
                    MessageBox.Show($"Auftragsnummer {auftragsNummer} wurde zur Ignorierliste hinzugefügt.", "Auftrag ignoriert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateDgvAnsichtAuftraege2();
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung
                    MessageBox.Show($"Fehler beim Schreiben der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Optional: Nachricht anzeigen, dass diese Auftragsnummer nun ignoriert wird
                MessageBox.Show($"Auftragsnummer {auftragsNummer} ist bereits in der Ignorierliste.", "Auftrag bereits ignoriert", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Wenn ich mit Rechtsklick auf Copy AuftragsNr klicke, dann wird die Auftragsnummer in die Zwischenablage kopiert
        private void CopyAuftragsNr_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile im DataGridView ausgewählt ist
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                // Auftragsnummer aus der ausgewählten Zeile abrufen
                string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
                // Überprüfen, ob die Auftragsnummer nicht null oder leer ist
                if (!string.IsNullOrEmpty(auftragsNr))
                {
                    // Auftragsnummer in die Zwischenablage kopieren
                    Clipboard.SetText(auftragsNr);
                    // Optional: Nachricht anzeigen, dass die Auftragsnummer kopiert wurde
                    MessageBox.Show($"Auftragsnummer {auftragsNr} wurde in die Zwischenablage kopiert.", "Kopiert", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (string.IsNullOrWhiteSpace(selectedBelagValue))
                {
                    _auftraegeDataTable = null;
                    DgvAnsichtAuftraege.DataSource = null;
                    return;
                }

                // Belag-Muster vorbereiten: z. B. "B103" -> suchen nach "%B103%" und "%B-103%"
                var belagOhneBindestrich = selectedBelagValue.ToUpperInvariant();
                var belagMitBindestrich = belagOhneBindestrich.Insert(1, "-");

                // EINZIGE Abfrage: Lädt Hauptdaten + VorStk (via APPLY) + Material (JOIN Serienlinsen) + Dringend (JOIN Ansicht_Bildschirm)
                // Wichtig: Parameter-Typen explizit setzen (bessere Planqualität als AddWithValue)
                const string sql = @"
                ;WITH base AS (
                    SELECT
                        CONVERT(date, a.trdf_enddate)         AS Enddatum,
                        a.dsca_teilebez                       AS [Teilebez.],
                        a.pdno_prodnr                         AS [Auftragsnr.],
                        a.mitm_teilenr                        AS Artikel,
                        a.opsta_avostat                       AS Status,
                        a.txta_avoinfo                        AS AVOinfo,
                        /* Seite wie in deiner bisherigen Logik */
                        CASE
                            WHEN a.txta_avoinfo LIKE '%III%' OR a.txta_avoinfo LIKE '%Iii%' OR a.txta_avoinfo LIKE '%IIi%' OR a.txta_avoinfo LIKE '%iii%' OR a.txta_avoinfo LIKE '%iII%' THEN '0'
                            WHEN a.txta_avoinfo LIKE '%Ii%'  OR a.txta_avoinfo LIKE '%iI%'  OR a.txta_avoinfo LIKE '%ii%'  OR a.txta_avoinfo LIKE '%II%'  THEN '2'
                            WHEN a.txta_avoinfo LIKE '%i%'   OR a.txta_avoinfo LIKE '%I%'   THEN '1'
                            ELSE '0'
                        END                                    AS Seite,
                        a.qplo_sollstk                         AS [SollStk.],
                        a.qcmp_iststk                          AS [IstStk.],
                        pv.qcmp2_vorstk                        AS [VorStk.],
                        a.qhnd1_stk_teilelager                 AS Teilelager,
                        a.qana_bereitstellbestand              AS Bereitstell,
                        a.demand_jahresbedarf                  AS Jahresbedarf,
                        CONVERT(date, a.import_date)           AS Aktualisiert
                    FROM LN_ProdOrders_PRD a
                    OUTER APPLY (
                        SELECT TOP (1) b.qcmp2_vorstk
                        FROM LN_ProdOrders_PRD b
                        WHERE b.pdno_prodnr = a.pdno_prodnr
                          AND b.txta_avoinfo LIKE '%Vorbereiten%'
                          AND b.trdf_enddate < a.trdf_enddate
                        ORDER BY b.trdf_enddate DESC
                    ) AS pv
                    WHERE a.opsta_avostat IN ('Active','Planned','Released')
                      AND (a.txta_avoinfo LIKE @BelagPattern1 OR a.txta_avoinfo LIKE @BelagPattern2)
                      AND a.txta_avoinfo LIKE '%Vergüten%'
                )
                SELECT
                    base.Enddatum,
                    base.[Teilebez.],
                    base.[Auftragsnr.],
                    base.Artikel,
                    base.Status,
                    base.AVOinfo,
                    ISNULL(sl.MATERIAL, '')    AS Material,
                    base.Seite,
                    base.[SollStk.],
                    base.[IstStk.],
                    base.[VorStk.],
                    base.Teilelager,
                    base.Bereitstell,
                    base.Jahresbedarf,
                    CAST('' AS nvarchar(20))   AS Zukauf,   -- wird danach im Code gesetzt (RL/TL)
                    ISNULL(ab.Dringend, '')    AS Dringend,
                    base.Aktualisiert
                FROM base
                LEFT JOIN Serienlinsen sl
                    ON sl.ARTNR = base.Artikel AND sl.Seite = base.Seite
                LEFT JOIN Ansicht_Bildschirm ab
                    ON ab.Auftrag = base.[Auftragsnr.]
                ORDER BY base.Enddatum ASC;";

                var dt = new DataTable();

                // Lokale Connection (ConnectionString aus deinem vorhandenen Feld)
                using (var conn = new SqlConnection(sqlConnectionVerwaltung.ConnectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    // Parameter mit Typ/Länge – vermeidet Suboptimalität durch AddWithValue
                    var p1 = cmd.Parameters.Add("@BelagPattern1", SqlDbType.NVarChar, 200);
                    p1.Value = "%" + belagOhneBindestrich + "%";
                    var p2 = cmd.Parameters.Add("@BelagPattern2", SqlDbType.NVarChar, 200);
                    p2.Value = "%" + belagMitBindestrich + "%";

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                }

                // RL/TL (Zukauf) aus JSON nachtragen – wie bisher
                RLTLData rltlData;
                var jsonFilePath2 = "rltl_data.json";
                if (File.Exists(jsonFilePath2))
                {
                    var json = File.ReadAllText(jsonFilePath2);
                    rltlData = JsonConvert.DeserializeObject<RLTLData>(json) ?? new RLTLData();
                }
                else
                {
                    rltlData = new RLTLData();
                }

                foreach (DataRow row in dt.Rows)
                {
                    var artikelNr = row["Artikel"]?.ToString() ?? string.Empty;
                    if (artikelNr.Length == 0) continue;

                    if (rltlData.RL != null && rltlData.RL.Contains(artikelNr))
                        row["Zukauf"] = "R-Lager";
                    else if (rltlData.TL != null && rltlData.TL.Contains(artikelNr))
                        row["Zukauf"] = "T-Lager";
                }

                // Ergebnis im Formular übernehmen
                _auftraegeDataTable = dt;

                // (Optional) JSON-Dump nur wenn du es brauchst – sonst auskommentieren:
                // File.WriteAllText("AnsichtAuftraege.json", JsonConvert.SerializeObject(_auftraegeDataTable, Formatting.Indented));

                // Anzeigen & formatieren
                ApplyFilterAndDisplayData();
                PictureBoxZeichnung.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler in UpdateDgvAnsichtAuftraege (optimiert): " + ex.Message);
                _auftraegeDataTable = null;
                DgvAnsichtAuftraege.DataSource = null;
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

        private DataTable BuildBelagTableFromGrid()
        {
            DataTable belagTable = new DataTable();
            belagTable.Columns.Add("Belag", typeof(string));
            belagTable.Columns.Add("AVOs", typeof(int));

            if (!DgvLadeBelaege.Columns.Contains("Belag") || !DgvLadeBelaege.Columns.Contains("AVOs"))
            {
                return belagTable;
            }

            foreach (DataGridViewRow row in DgvLadeBelaege.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string belag = row.Cells["Belag"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(belag))
                {
                    continue;
                }

                int avos = 0;
                int.TryParse(row.Cells["AVOs"].Value?.ToString(), out avos);
                belagTable.Rows.Add(belag, avos);
            }

            return belagTable;
        }

        // Wenn auf den Button geklickt wird, dann öffnet sich das Fenster mit den offenen Stückzahlen
        private void btnShowStkOffen_Click(object sender, EventArgs e)
        {
            Form_StkVorAvo form_StkVorAvo = new Form_StkVorAvo();
            form_StkVorAvo.ShowDialog();
        }

        // Wenn auf den Rückstand Button geklickt wird, dann öffnet sich das Fenster mit dem Rückstand
        private void btnRueckstand_Click(object sender, EventArgs e)
        {
            DataTable belagTable = (DgvLadeBelaege.DataSource as DataTable)?.Copy() ?? BuildBelagTableFromGrid();
            Form_Rueckstand form_Rueckstand = new Form_Rueckstand(belagTable);
            form_Rueckstand.ShowDialog();
        }
    }
}
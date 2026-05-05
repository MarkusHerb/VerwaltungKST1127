// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken bzw. NuGet-Paketen.
// Damit können wir z. B. statt "Newtonsoft.Json.JsonConvert" einfach "JsonConvert" schreiben.
// ===================================================================================================
using Newtonsoft.Json;                                       // JSON-Bibliothek (Objekt ↔ JSON umwandeln)
using System;                                                // Basis-Typen (string, int, EventArgs, Exception ...)
using System.Collections.Generic;                            // Sammlungen (List<T>, Dictionary<TKey,TValue>)
using System.Data;                                           // DataTable, DataView, DataRow, ConnectionState
using System.Data.SqlClient;                                 // SQL-Server (SqlConnection, SqlCommand, SqlDataAdapter)
using System.Drawing;                                        // Farben, Schriften (Color, Font ...)
using System.IO;                                             // Datei-Operationen (File.Exists, ReadAllText ...)
using System.Linq;                                           // LINQ (Where, Select, Distinct, Count ...)
using System.Windows.Forms;                                  // Windows-Forms (Form, DataGridView, MessageBox ...)
using VerwaltungKST1127.EingabeSerienartikelPrototyp;        // Eigenes Sub-Projekt: Form_ArtikelPrototypAendern
using VerwaltungKST1127.Produktionsauswertung;               // Eigenes Sub-Projekt: Form_StkVorAvo

// "namespace" gruppiert Klassen logisch und vermeidet Namenskonflikte mit anderen Projekten.
namespace VerwaltungKST1127.Auftragsverwaltung
{
    // "public partial class Form_VerwaltungHauptansicht : Form"
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien aufgeteilt sein (eine ist die Designer-Datei)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_VerwaltungHauptansicht : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Felder ("Variablen der Klasse"). private = nur in dieser Klasse sichtbar.
        // readonly = darf nur einmal (hier oder im Konstruktor) gesetzt werden.
        // -----------------------------------------------------------------------------------------------------------------

        // Verbindung zur "neuen" Verwaltungs-Datenbank.
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // Verbindung zur "alten" Verwaltungs-Datenbank (parallel verwendet).
        private readonly SqlConnection sqlConnectionVerwaltungAlt = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung;Integrated Security=True;Encrypt=False");

        // Hier merken wir uns die komplette Auftrags-Tabelle, damit wir filtern können
        // ohne erneut die DB anzusprechen. "= null" → anfangs leer.
        private DataTable _auftraegeDataTable = null;

        // Aktuell ausgewählter Belag (z. B. "B103") – wird im Klick-Event gesetzt.
        private string selectedBelagValue;

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_VerwaltungHauptansicht()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_VerwaltungHauptansicht()
        {
            InitializeComponent();             // erzeugt alle UI-Steuerelemente (in der Designer-Datei definiert)
            LoadDataForDgvAuftragZuBelag();    // Linkes Grid (Belag-Übersicht) befüllen
            ZaehleGestarteteAuftraege();       // "Gestartete"-Label aktualisieren
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wendet Filter (Zukauf-Checkbox + ignorierte Auftragsnummern) auf die geladene Tabelle an
        // und zeigt das Ergebnis im DataGridView. Außerdem wird die Anzahl "Active" gezählt.
        // -----------------------------------------------------------------------------------------------------------------
        private void ApplyFilterAndDisplayData()
        {
            // ----- 1) Ignorierte Auftragsnummern aus JSON-Datei laden -----
            List<string> ignorierteNummern = new List<string>();
            string jsonPfad = "IgnorierteAuftragsnummern.json";
            if (File.Exists(jsonPfad))
            {
                try
                {
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    // Wenn JSON null/leer, leere Liste verwenden (?? = Fallback bei null).
                    ignorierteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();
                }
                catch { /* Fehler ignorieren, falls Datei defekt */ }
            }

            int activeCount = 0; // Zähler für Status "Active"

            try
            {
                // Wenn keine Daten geladen → Grid leeren und Methode beenden.
                if (_auftraegeDataTable == null)
                {
                    DgvAnsichtAuftraege.DataSource = null;
                    lblGestartet.Text = "0";
                    return;
                }

                // ----- 2) Filter zusammenbauen (DataView = "Sicht" auf eine DataTable, die filtern/sortieren kann) -----
                DataView dv = new DataView(_auftraegeDataTable);
                string filter = "";

                // Wenn Checkbox aktiv → nur Zukauf-Aufträge zeigen.
                if (checkBoxShowZukauf.Checked)
                {
                    filter = "[Zukauf] IS NOT NULL AND [Zukauf] <> ''";
                }

                // Ignorierte Nummern in den Filter packen (NOT IN ('A','B',...)).
                if (ignorierteNummern.Count > 0)
                {
                    // Apostroph in den Werten wird verdoppelt (SQL-Escape), dann mit "','" verbunden.
                    string ignoreFilter = string.Join("','", ignorierteNummern.Select(x => x.Replace("'", "''")));
                    if (!string.IsNullOrEmpty(filter))
                        filter += " AND ";
                    filter += $"NOT [Auftragsnr.] IN ('{ignoreFilter}')";
                }

                dv.RowFilter = filter;
                DgvAnsichtAuftraege.DataSource = dv;

                // ----- 3) Optik & Spaltenformatierung -----
                if (DgvAnsichtAuftraege.DataSource != null && DgvAnsichtAuftraege.Columns.Count > 0)
                {
                    DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Zahlen-Spalten zentrieren + ohne Nachkommastellen anzeigen.
                    string[] zahlenSpalten = { "SollStk.", "IstStk.", "VorStk.", "Teilelager", "Bereitstell", "Jahresbedarf" };
                    foreach (var spalte in zahlenSpalten)
                    {
                        if (DgvAnsichtAuftraege.Columns.Contains(spalte))
                        {
                            DgvAnsichtAuftraege.Columns[spalte].DefaultCellStyle.Format = "N0";
                            DgvAnsichtAuftraege.Columns[spalte].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }

                    // Zukauf + Dringend zentriert.
                    if (DgvAnsichtAuftraege.Columns.Contains("Zukauf"))
                        DgvAnsichtAuftraege.Columns["Zukauf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (DgvAnsichtAuftraege.Columns.Contains("Dringend"))
                        DgvAnsichtAuftraege.Columns["Dringend"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Alle Spaltenüberschriften zentrieren.
                    foreach (DataGridViewColumn column in DgvAnsichtAuftraege.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Feste Breiten für bestimmte Spalten.
                    if (DgvAnsichtAuftraege.Columns.Contains("Teilebez."))
                        DgvAnsichtAuftraege.Columns["Teilebez."].Width = 170;
                    if (DgvAnsichtAuftraege.Columns.Contains("Seite"))
                        DgvAnsichtAuftraege.Columns["Seite"].Width = 60;
                    if (DgvAnsichtAuftraege.Columns.Contains("AVOinfo"))
                        DgvAnsichtAuftraege.Columns["AVOinfo"].Width = 140;

                    // ----- 4) Aktive Aufträge zählen -----
                    if (DgvAnsichtAuftraege.Columns.Contains("Status"))
                    {
                        foreach (DataGridViewRow row in DgvAnsichtAuftraege.Rows)
                        {
                            object cellValue = row.Cells["Status"]?.Value;
                            // Equals mit "OrdinalIgnoreCase" → Vergleich ohne Groß-/Kleinschreibung.
                            if (cellValue != null && cellValue.ToString().Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                activeCount++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Anzeigen/Formatieren/Zählen der Daten: " + ex.Message);
                activeCount = 0;
            }
            finally
            {
                // finally läuft IMMER (auch im Fehlerfall) → Label wird zuverlässig gesetzt.
                lblGestartet.Text = activeCount.ToString();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "Zukauf"-Button → eigenes Fenster zur Verwaltung von R-/T-Lager öffnen.
        // ShowDialog() = modal (das Hauptfenster wartet, bis der Dialog geschlossen wird).
        // -----------------------------------------------------------------------------------------------------------------
        private void BtnZukauf_Click(object sender, EventArgs e)
        {
            Form_RLTL rltlForm = new Form_RLTL();
            rltlForm.ShowDialog();
        }

        // Checkbox geändert → Filter erneut anwenden.
        private void checkBoxShowZukauf_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFilterAndDisplayData();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Hilfsfunktion: Belag-Bezeichnung normalisieren.
        // Aus z. B. "Vergüten B-103" wird "B103". Mit Hilfe von Regex (Regular Expression).
        // -----------------------------------------------------------------------------------------------------------------
        private string CleanBelag(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // 1) "Vergüten" entfernen (Groß-/Kleinschreibung egal).
            input = System.Text.RegularExpressions.Regex.Replace(
                input, "vergüten", "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

            // 2) Suche "B" + optionalen "-" + Ziffern → z. B. "B103" oder "B-103".
            //    @"..." = "verbatim string": Backslashes müssen nicht verdoppelt werden.
            var match = System.Text.RegularExpressions.Regex.Match(input, @"B-?\d+");

            if (match.Success)
            {
                // Treffer ohne Bindestrich, in Großbuchstaben.
                return match.Value.Replace("-", "").ToUpper();
            }
            else
            {
                return ""; // nichts gefunden
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Löscht einen Auftrag aus den Bildschirm-Anzeigetabellen (in beiden Datenbanken parallel).
        // -----------------------------------------------------------------------------------------------------------------
        private void DeleteFromAnsichtBildschirm(string auftragsNr, string artikel)
        {
            try
            {
                // Erste DB: Spaltenname "Teilenummer".
                string deleteQuery = @"
                    DELETE FROM Ansicht_Bildschirm
                    WHERE Auftrag = @Auftrag
                    AND Teilenummer = @Teilenummer";

                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnectionVerwaltung))
                {
                    // Parameter ergänzen → schützt vor SQL-Injection.
                    deleteCommand.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    deleteCommand.Parameters.AddWithValue("@Teilenummer", artikel);

                    sqlConnectionVerwaltung.Open();
                    deleteCommand.ExecuteNonQuery();   // ExecuteNonQuery → für INSERT/UPDATE/DELETE (kein Ergebnis-Set)
                    sqlConnectionVerwaltung.Close();
                }

                // Zweite DB (alt): Spaltenname "Artikel" (statt "Teilenummer").
                string deleteQueryAlt = @"
                    DELETE FROM Ainsicht_Bildschirm
                    WHERE Auftrag = @Auftrag
                    AND Artikel = @Artikel";

                using (SqlCommand deleteCommandAlt = new SqlCommand(deleteQueryAlt, sqlConnectionVerwaltungAlt))
                {
                    deleteCommandAlt.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    deleteCommandAlt.Parameters.AddWithValue("@Artikel", artikel);

                    sqlConnectionVerwaltungAlt.Open();
                    deleteCommandAlt.ExecuteNonQuery();
                    sqlConnectionVerwaltungAlt.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Löschen der Daten: {ex.Message}");
            }
            finally
            {
                // Sicherheitsnetz: beide Verbindungen schließen.
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

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf eine Zelle im Auftragsgrid:
        //  - Wenn Artikel + Seite gefüllt sind → zugehörigen Zeichnungs-Pfad holen und Bild anzeigen.
        //  - Außerdem das Detail-Grid (DgvInformationZuAuftrag) für die Auftragsnummer aktualisieren.
        // -----------------------------------------------------------------------------------------------------------------
        private void DgvAnsichtAuftraege_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                DataGridViewRow selectedRow = DgvAnsichtAuftraege.CurrentRow;

                // Sicherstellen, dass Artikel + Seite gesetzt sind.
                if (selectedRow.Cells["Artikel"].Value != null && !string.IsNullOrEmpty(selectedRow.Cells["Artikel"].Value.ToString()) &&
                    selectedRow.Cells["Seite"].Value != null && !string.IsNullOrEmpty(selectedRow.Cells["Seite"].Value.ToString()))
                {
                    string artikelNr = selectedRow.Cells["Artikel"].Value.ToString();
                    int seite;

                    // TryParse: liefert true, wenn die Konvertierung in int klappt.
                    if (int.TryParse(selectedRow.Cells["Seite"].Value.ToString(), out seite))
                    {
                        string zeichnungspfad = GetZeichnungspfad(artikelNr, seite);

                        if (!string.IsNullOrEmpty(zeichnungspfad))
                        {
                            // ImageLocation = Bild von einem Pfad/URL nachladen.
                            PictureBoxZeichnung.ImageLocation = zeichnungspfad;
                        }
                        else
                        {
                            PictureBoxZeichnung.Image = null;
                        }
                    }
                }
            }

            // Wenn ein gültiger Zeilen-Index angeklickt wurde → Detail-Grid aktualisieren.
            if (e.RowIndex >= 0)
            {
                var auftragsNummer = DgvAnsichtAuftraege.Rows[e.RowIndex].Cells["Auftragsnr."].Value?.ToString();
                if (!string.IsNullOrEmpty(auftragsNummer))
                {
                    UpdateDgvInformationZuAuftrag(auftragsNummer);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Doppelklick auf eine Auftragszeile → Druckübersichts-Fenster mit allen Werten öffnen.
        // -----------------------------------------------------------------------------------------------------------------
        private void DgvAnsichtAuftraege_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = DgvAnsichtAuftraege.Rows[e.RowIndex];

                // Werte aus der Zeile lesen. ?. = Null-bedingt → kein Crash, wenn Value null ist.
                string enddatum = selectedRow.Cells["Enddatum"].Value?.ToString();
                string teilebez = selectedRow.Cells["Teilebez."].Value?.ToString();
                string auftragsNr = selectedRow.Cells["Auftragsnr."].Value?.ToString();
                string artikel = selectedRow.Cells["Artikel"].Value?.ToString();
                string status = selectedRow.Cells["Status"].Value?.ToString();
                string avoInfo = selectedRow.Cells["AVOinfo"].Value?.ToString();
                string material = selectedRow.Cells["Material"].Value?.ToString();
                string seite = selectedRow.Cells["Seite"].Value?.ToString();

                // SollStk explizit als decimal parsen und ohne Nachkommastellen formatieren.
                string sollStk = Convert.ToDecimal(selectedRow.Cells["SollStk."].Value).ToString("0");

                string istStk = selectedRow.Cells["IstStk."].Value?.ToString();
                string vorStk = selectedRow.Cells["VorStk."].Value?.ToString();
                string teilelager = selectedRow.Cells["Teilelager"].Value?.ToString();
                string bereitstell = selectedRow.Cells["Bereitstell"].Value?.ToString();
                string jahresbedarf = selectedRow.Cells["Jahresbedarf"].Value?.ToString();
                string zukauf = selectedRow.Cells["Zukauf"].Value?.ToString();
                string dringend = selectedRow.Cells["Dringend"].Value?.ToString();
                string aktualisiert = selectedRow.Cells["Aktualisiert"].Value?.ToString();

                // Druckübersicht öffnen (modal).
                Form_Druckuebersicht druckuebersichtForm = new Form_Druckuebersicht(
                    enddatum, teilebez, auftragsNr, artikel, status, avoInfo, material, seite,
                    sollStk, istStk, vorStk, teilelager, bereitstell, jahresbedarf, zukauf,
                    dringend, aktualisiert, selectedBelagValue);
                druckuebersichtForm.ShowDialog();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // CellFormatting: wird beim Zeichnen jeder Zelle aufgerufen.
        // Hier setzen wir Hintergrundfarbe / Schriftfarbe / Schriftstil je nach Inhalt.
        // -----------------------------------------------------------------------------------------------------------------
        private void DgvAnsichtAuftraege_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // ---- Status: "Active" → grün ----
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Status")
            {
                if (e.Value != null && e.Value.ToString() == "Active")
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                }
            }

            // ---- Status: "Gestartet" → hellblau ----
            if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Status")
            {
                if (e.Value != null && e.Value.ToString() == "Gestartet")
                {
                    e.CellStyle.BackColor = Color.LightSkyBlue;
                }
            }

            // ---- Zukauf: R-Lager (grün) / T-Lager (rot/lachs) ----
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

            // ---- Dringend: 1 → orange, 2 → gelb ----
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

            // ---- Enddatum in der Vergangenheit → grau ----
            // Sonst zurück auf Standardfarbe (wichtig wegen "Cell Recycling": Zellen werden wiederverwendet).
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "Enddatum")
                {
                    DateTime endDate;
                    if (DateTime.TryParse(e.Value.ToString(), out endDate))
                    {
                        if (endDate.Date < DateTime.Today)
                        {
                            e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                        }
                        else
                        {
                            e.CellStyle.BackColor = DgvAnsichtAuftraege.DefaultCellStyle.BackColor;
                        }
                    }
                }

                // ---- VorStk., IstStk., SollStk. → fett ----
                if (DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "VorStk." ||
                    DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "IstStk." ||
                    DgvAnsichtAuftraege.Columns[e.ColumnIndex].Name == "SollStk.")
                {
                    e.CellStyle.Font = new Font(DgvAnsichtAuftraege.Font, FontStyle.Bold);
                }
            }

            // ---- Auftragsnummer hervorheben, wenn sie in letzteAuftragsnummern.json steht ----
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
                        // Console.WriteLine schreibt in das Debug-/Konsole-Fenster, nicht in die UI.
                        Console.WriteLine($"Fehler beim Lesen der JSON-Datei: {ex.Message}");
                    }
                }
            }

            // ---- Teilelager < Bereitstell → rote Schrift + lachsroter Hintergrund ----
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

        // -----------------------------------------------------------------------------------------------------------------
        // Rechtsklick auf das Auftrags-Grid → Kontextmenü mit verschiedenen Aktionen aufbauen und anzeigen.
        // -----------------------------------------------------------------------------------------------------------------
        private void DgvAnsichtAuftraege_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // HitTest: liefert Info, welche Zelle/Zeile unter den Maus-Koordinaten liegt.
                var hti = DgvAnsichtAuftraege.HitTest(e.X, e.Y);

                DgvAnsichtAuftraege.ClearSelection(); // alte Selektion zurücksetzen

                if (hti.RowIndex >= 0)
                {
                    DgvAnsichtAuftraege.Rows[hti.RowIndex].Selected = true;

                    // Kontextmenü zur Laufzeit erzeugen.
                    ContextMenuStrip contextMenu = new ContextMenuStrip();

                    ToolStripMenuItem setzeDringend1 = new ToolStripMenuItem("Setze Dringend 1");
                    ToolStripMenuItem setzeDringend2 = new ToolStripMenuItem("Setze Dringend 2");
                    ToolStripMenuItem resettDringend = new ToolStripMenuItem("Resett Dringend");
                    ToolStripMenuItem serienlinse = new ToolStripMenuItem("Serienlinse");
                    ToolStripMenuItem resettAuftrag = new ToolStripMenuItem("Auftrag abschließen");
                    ToolStripMenuItem ignoreAuftrag = new ToolStripMenuItem("Auftrag ignorieren");
                    ToolStripMenuItem copyAuftragsNr = new ToolStripMenuItem("Auftragsnummer kopieren");

                    // Klick-Events der Menüeinträge mit Methoden verbinden.
                    setzeDringend1.Click += SetzeDringend1_Click;
                    setzeDringend2.Click += SetzeDringend2_Click;
                    resettDringend.Click += ResettDringend_Click;
                    serienlinse.Click += Serienlinse_Click;
                    resettAuftrag.Click += ResettAuftrag_Click;
                    ignoreAuftrag.Click += IgnoreAuftrag_Click;
                    copyAuftragsNr.Click += CopyAuftragsNr_Click;

                    // Alle Einträge dem Menü zufügen.
                    contextMenu.Items.AddRange(new ToolStripItem[]
                    {
                        setzeDringend1, setzeDringend2, resettDringend,
                        serienlinse, resettAuftrag, ignoreAuftrag, copyAuftragsNr
                    });

                    // Menü an der Maus-Position öffnen.
                    contextMenu.Show(DgvAnsichtAuftraege, e.Location);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick im linken Belag-Grid → ausgewählten Belag merken und das Auftragsgrid danach filtern/laden.
        // -----------------------------------------------------------------------------------------------------------------
        private void DgvLadeBelaege_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedBelagValue = DgvLadeBelaege.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuftraege(selectedBelagValue);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // (Auskommentierte Hilfs-Methode aus früherer Version – aktuell nicht in Verwendung.)
        // Bewusst stehen gelassen, falls man sie wieder brauchen möchte.
        // -----------------------------------------------------------------------------------------------------------------
        //private string GetMaterialFromSerienlinsen(string mitmTeilenNr, int seite) { ... }

        // -----------------------------------------------------------------------------------------------------------------
        // Holt für (Artikel, Seite) den Pfad zur Zeichnungsdatei aus der Tabelle "Serienlinsen".
        // -----------------------------------------------------------------------------------------------------------------
        private string GetZeichnungspfad(string artikelNr, int seite)
        {
            string zeichnungspfad = string.Empty;
            try
            {
                string query = @"
                    SELECT Zeichnungspfad
                    FROM Serienlinsen
                    WHERE ARTNR = @ArtikelNr AND Seite = @Seite";

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@ArtikelNr", artikelNr);
                    command.Parameters.AddWithValue("@Seite", seite);

                    sqlConnectionVerwaltung.Open();

                    // ExecuteScalar: holt den ersten Wert der ersten Zeile (oder null).
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        zeichnungspfad = result.ToString();
                        lblPfad.Text = zeichnungspfad;
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

        // -----------------------------------------------------------------------------------------------------------------
        // Lädt die Belag-Übersicht (links) beim Start.
        // 1) DB abfragen → 2) Beläge bereinigen/zusammenfassen → 3) JSON speichern → 4) JSON ins Grid laden.
        // -----------------------------------------------------------------------------------------------------------------
        private void LoadDataForDgvAuftragZuBelag()
        {
            try
            {
                // ----- 1. SQL: alle Vergüten-Beläge gruppiert mit Anzahl AVOs -----
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
                    sqlConnectionVerwaltung.Open();
                    adapter.Fill(dataTable);
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"SQL Fehler beim Laden der Belag-Daten: {sqlEx.Message}", "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    if (sqlConnectionVerwaltung != null && sqlConnectionVerwaltung.State == ConnectionState.Open)
                    {
                        sqlConnectionVerwaltung.Close();
                    }
                }

                // ----- 2. Bereinigung: gleiche Beläge zusammenfassen (B103 == B-103) -----
                Dictionary<string, int> belagCounts = new Dictionary<string, int>();

                foreach (DataRow row in dataTable.Rows)
                {
                    string originalBelag = row["Belag"].ToString();
                    int avos = Convert.ToInt32(row["AVOs"]);

                    string cleanedBelag = CleanBelag(originalBelag);
                    if (string.IsNullOrEmpty(cleanedBelag))
                        continue;

                    // Falls Belag noch nicht im Dictionary → mit 0 initialisieren, dann AVOs aufaddieren.
                    if (!belagCounts.ContainsKey(cleanedBelag))
                    {
                        belagCounts[cleanedBelag] = 0;
                    }
                    belagCounts[cleanedBelag] += avos;
                }

                // ----- 3. Neue saubere DataTable bauen -----
                DataTable cleanedTable = new DataTable();
                cleanedTable.Columns.Add("Belag", typeof(string));
                cleanedTable.Columns.Add("AVOs", typeof(int));

                foreach (var item in belagCounts)
                {
                    DataRow newRow = cleanedTable.NewRow();
                    newRow["Belag"] = item.Key;
                    newRow["AVOs"] = item.Value;
                    cleanedTable.Rows.Add(newRow);
                }

                // ----- 4. Tabelle als JSON speichern (Zwischenablage / Debugging-Zweck) -----
                string jsonFilePath = "GeladeneBelaege.json";
                string jsonData = JsonConvert.SerializeObject(cleanedTable, Formatting.Indented);
                File.WriteAllText(jsonFilePath, jsonData);

                // ----- 5. JSON wieder einlesen und ins Grid binden -----
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

                // ----- 6. Optik des Grids einstellen -----
                if (DgvLadeBelaege.DataSource != null)
                {
                    DgvLadeBelaege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    if (DgvLadeBelaege.Columns.Contains("AVOs"))
                    {
                        DgvLadeBelaege.Columns["AVOs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        DgvLadeBelaege.Columns["AVOs"].DefaultCellStyle.Format = "N0";
                    }

                    foreach (DataGridViewColumn column in DgvLadeBelaege.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"JSON Fehler beim Laden der Belag-Daten: {jsonEx.Message}", "JSON Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DgvLadeBelaege.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Allgemeiner Fehler in LoadDataForDgvAuftragZuBelag: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DgvLadeBelaege.DataSource = null;
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Auftrag abschließen": Auftragsnummer aus letzteAuftragsnummern.json entfernen.
        // -----------------------------------------------------------------------------------------------------------------
        private void ResettAuftrag_Click(object sender, EventArgs e)
        {
            // [0] = erste markierte Zeile.
            var selectedRow = DgvAnsichtAuftraege.SelectedRows[0];
            string auftragsNummer = selectedRow.Cells["Auftragsnr."].Value.ToString();

            string jsonPfad = "letzteAuftragsnummern.json";

            if (File.Exists(jsonPfad))
            {
                try
                {
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    List<string> gespeicherteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();

                    if (gespeicherteNummern.Contains(auftragsNummer))
                    {
                        gespeicherteNummern.Remove(auftragsNummer);

                        // Aktualisierte Liste zurückschreiben.
                        string neuerJsonInhalt = JsonConvert.SerializeObject(gespeicherteNummern, Formatting.Indented);
                        File.WriteAllText(jsonPfad, neuerJsonInhalt);

                        MessageBox.Show($"Auftragsnummer {auftragsNummer} wurde aus der gestarteten Auftragsliste gelöscht.", "Auftrag abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Anzeige aktualisieren.
                        UpdateDgvAnsichtAuftraege2();
                        ZaehleGestarteteAuftraege();
                    }
                    else
                    {
                        MessageBox.Show($"Auftragsnummer {auftragsNummer} wurde nicht in der gestarteten Auftragsliste gefunden.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Bearbeiten der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Die Datei '{jsonPfad}' existiert nicht.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Resett Dringend": markierten Auftrag aus den Bildschirm-Anzeigetabellen löschen.
        // -----------------------------------------------------------------------------------------------------------------
        private void ResettDringend_Click(object sender, EventArgs e)
        {
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
                string artikel = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();

                if (!string.IsNullOrEmpty(auftragsNr) && !string.IsNullOrEmpty(artikel))
                {
                    DeleteFromAnsichtBildschirm(auftragsNr, artikel);
                    UpdateDgvAnsichtAuftraege2();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Speichert eine neue Bildschirm-Anzeige (Dringend 1 oder 2) in beide DBs:
        //  - holt zuerst die Belag-Bezeichnungen aus Serienlinsen
        //  - fügt dann je einen INSERT in beide Tabellen ein.
        // -----------------------------------------------------------------------------------------------------------------
        private void SaveToAnsichtBildschirm(string auftragsNr, string artikel, string dringendWert)
        {
            try
            {
                // --- 1) Beläge je Seite aus Serienlinsen abfragen ---
                string query = @"
                    SELECT SEITE, Belag1
                    FROM Serienlinsen
                    WHERE ARTNR = @Artikel
                    AND Status = 'Serie'";

                Dictionary<int, string> belagDict = new Dictionary<int, string>();

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltungAlt))
                {
                    command.Parameters.AddWithValue("@Artikel", artikel);

                    sqlConnectionVerwaltungAlt.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Reader liefert Zeile für Zeile.
                        while (reader.Read())
                        {
                            string seite = reader.GetString(reader.GetOrdinal("SEITE"));
                            string belag1 = reader.GetString(reader.GetOrdinal("Belag1"));
                            // Im Dictionary speichern (Schlüssel = Seite als int).
                            belagDict[int.Parse(seite)] = belag1;
                        }
                    }
                    sqlConnectionVerwaltungAlt.Close();
                }

                // Werte je Seite holen, mit Standard "-" wenn nicht vorhanden.
                string seite1 = belagDict.ContainsKey(1) ? belagDict[1] : "-";
                string seite2 = belagDict.ContainsKey(2) ? belagDict[2] : "-";
                string seite0 = belagDict.ContainsKey(0) ? belagDict[0] : "-";

                // --- 2) INSERT in alte DB ---
                string insertQueryAlt = @"
                    INSERT INTO Ainsicht_Bildschirm (Auftrag, Artikel, Seite_1, Seite_2, Seite_0, Dringend)
                    VALUES (@Auftrag, @Artikel, @Seite1, @Seite2, @Seite0, @Dringend)";

                using (SqlCommand insertCommand = new SqlCommand(insertQueryAlt, sqlConnectionVerwaltungAlt))
                {
                    insertCommand.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    insertCommand.Parameters.AddWithValue("@Artikel", artikel);
                    insertCommand.Parameters.AddWithValue("@Seite1", seite1);
                    insertCommand.Parameters.AddWithValue("@Seite2", seite2);
                    insertCommand.Parameters.AddWithValue("@Seite0", seite0);
                    insertCommand.Parameters.AddWithValue("@Dringend", dringendWert);

                    sqlConnectionVerwaltungAlt.Open();
                    insertCommand.ExecuteNonQuery();
                    sqlConnectionVerwaltungAlt.Close();
                }

                // --- 3) INSERT in neue DB (analog, Spalte heißt hier "Teilenummer") ---
                string insertQuery = @"
                    INSERT INTO Ansicht_Bildschirm (Auftrag, Teilenummer, Seite_1, Seite_2, Seite_0, Dringend)
                    VALUES (@Auftrag, @Teilenummer, @Seite1, @Seite2, @Seite0, @Dringend)";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnectionVerwaltung))
                {
                    insertCommand.Parameters.AddWithValue("@Auftrag", auftragsNr);
                    insertCommand.Parameters.AddWithValue("@Teilenummer", artikel);
                    insertCommand.Parameters.AddWithValue("@Seite1", seite1);
                    insertCommand.Parameters.AddWithValue("@Seite2", seite2);
                    insertCommand.Parameters.AddWithValue("@Seite0", seite0);
                    insertCommand.Parameters.AddWithValue("@Dringend", dringendWert);

                    sqlConnectionVerwaltung.Open();
                    insertCommand.ExecuteNonQuery();
                    sqlConnectionVerwaltung.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Daten: {ex.Message}");
            }
            finally
            {
                if (sqlConnectionVerwaltungAlt.State == ConnectionState.Open) sqlConnectionVerwaltungAlt.Close();
                if (sqlConnectionVerwaltung.State == ConnectionState.Open) sqlConnectionVerwaltung.Close();
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Serienlinse" im Kontextmenü → Stammdaten-Fenster für (Artikel, Seite) öffnen.
        // -----------------------------------------------------------------------------------------------------------------
        private void Serienlinse_Click(object sender, EventArgs e)
        {
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                string artikelNr = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
                string seite = DgvAnsichtAuftraege.CurrentRow.Cells["Seite"].Value?.ToString();

                if (!string.IsNullOrEmpty(artikelNr) && !string.IsNullOrEmpty(seite))
                {
                    Form_ArtikelPrototypAendern artikelPrototypAendern = new Form_ArtikelPrototypAendern(artikelNr, seite);
                    artikelPrototypAendern.ShowDialog();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Auftrag ignorieren": Auftragsnummer in IgnorierteAuftragsnummern.json aufnehmen.
        // -----------------------------------------------------------------------------------------------------------------
        private void IgnoreAuftrag_Click(object sender, EventArgs e)
        {
            var selectedRow = DgvAnsichtAuftraege.SelectedRows[0];
            string auftragsNummer = selectedRow.Cells["Auftragsnr."].Value.ToString();

            string jsonPfad = "IgnorierteAuftragsnummern.json";
            List<string> ignorierteNummern = new List<string>();

            // Bestehende Liste laden, falls vorhanden.
            if (File.Exists(jsonPfad))
            {
                try
                {
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    ignorierteNummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Lesen der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Nur hinzufügen, wenn Nummer noch nicht in der Liste ist.
            if (!ignorierteNummern.Contains(auftragsNummer))
            {
                ignorierteNummern.Add(auftragsNummer);
                try
                {
                    string neuerJsonInhalt = JsonConvert.SerializeObject(ignorierteNummern, Formatting.Indented);
                    File.WriteAllText(jsonPfad, neuerJsonInhalt);

                    MessageBox.Show($"Auftragsnummer {auftragsNummer} wurde zur Ignorierliste hinzugefügt.", "Auftrag ignoriert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateDgvAnsichtAuftraege2();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Schreiben der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Auftragsnummer {auftragsNummer} ist bereits in der Ignorierliste.", "Auftrag bereits ignoriert", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Auftragsnummer kopieren" → Wert in die Zwischenablage legen.
        // -----------------------------------------------------------------------------------------------------------------
        private void CopyAuftragsNr_Click(object sender, EventArgs e)
        {
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
                if (!string.IsNullOrEmpty(auftragsNr))
                {
                    Clipboard.SetText(auftragsNr); // Zwischenablage befüllen
                    MessageBox.Show($"Auftragsnummer {auftragsNr} wurde in die Zwischenablage kopiert.", "Kopiert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Setze Dringend 1": prüft, ob bereits 1/2 gesetzt ist – sonst Eintrag mit Wert "1" speichern.
        // -----------------------------------------------------------------------------------------------------------------
        private void SetzeDringend1_Click(object sender, EventArgs e)
        {
            string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
            string artikel = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
            string dringendStatus = DgvAnsichtAuftraege.CurrentRow.Cells["Dringend"].Value?.ToString();

            if (!string.IsNullOrEmpty(auftragsNr) && !string.IsNullOrEmpty(artikel))
            {
                if (dringendStatus == "1" || dringendStatus == "2")
                {
                    MessageBox.Show("Es ist bereits ein Dringend-Status gesetzt. Bitte setzen Sie den Status zurück, bevor Sie einen neuen Dringend-Status setzen.", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    SaveToAnsichtBildschirm(auftragsNr, artikel, "1");
                    UpdateDgvAnsichtAuftraege2();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Setze Dringend 2": analog zu Dringend 1, nur Wert "2".
        // -----------------------------------------------------------------------------------------------------------------
        private void SetzeDringend2_Click(object sender, EventArgs e)
        {
            if (DgvAnsichtAuftraege.CurrentRow != null)
            {
                string auftragsNr = DgvAnsichtAuftraege.CurrentRow.Cells["Auftragsnr."].Value?.ToString();
                string artikel = DgvAnsichtAuftraege.CurrentRow.Cells["Artikel"].Value?.ToString();
                string dringendStatus = DgvAnsichtAuftraege.CurrentRow.Cells["Dringend"].Value?.ToString();

                if (!string.IsNullOrEmpty(auftragsNr) && !string.IsNullOrEmpty(artikel))
                {
                    if (dringendStatus == "1" || dringendStatus == "2")
                    {
                        MessageBox.Show("Es ist bereits ein Dringend-Status gesetzt. Bitte setzen Sie den Status zurück, bevor Sie einen neuen Dringend-Status setzen.", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        SaveToAnsichtBildschirm(auftragsNr, artikel, "2");
                        UpdateDgvAnsichtAuftraege2();
                    }
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Lädt die Aufträge zum gewählten Belag aus der DB (große, optimierte Abfrage).
        // - holt VorStk per OUTER APPLY
        // - JOIN mit Serienlinsen (Material) und Ansicht_Bildschirm (Dringend)
        // - ergänzt anschließend die Spalte "Zukauf" anhand einer JSON-Datei
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateDgvAnsichtAuftraege(string selectedBelagValue)
        {
            try
            {
                // Wenn kein Belag gewählt → Tabelle leeren.
                if (string.IsNullOrWhiteSpace(selectedBelagValue))
                {
                    _auftraegeDataTable = null;
                    DgvAnsichtAuftraege.DataSource = null;
                    return;
                }

                // Zwei Suchmuster: "B103" und "B-103". ToUpperInvariant = sprachunabhängige Großschreibung.
                var belagOhneBindestrich = selectedBelagValue.ToUpperInvariant();
                var belagMitBindestrich = belagOhneBindestrich.Insert(1, "-");

                // const string = SQL-Statement (mit Common Table Expression "WITH base").
                // OUTER APPLY hängt zu jeder Zeile a den passenden Vorbereiten-VorStk-Wert dran.
                const string sql = @"
                ;WITH base AS (
                    SELECT
                        CONVERT(date, a.trdf_enddate)         AS Enddatum,
                        a.dsca_teilebez                       AS [Teilebez.],
                        a.pdno_prodnr                         AS [Auftragsnr.],
                        a.mitm_teilenr                        AS Artikel,
                        a.opsta_avostat                       AS Status,
                        a.txta_avoinfo                        AS AVOinfo,
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

                // Eigene Connection (gleiche ConnectionString-Daten wie das Klassenfeld).
                using (var conn = new SqlConnection(sqlConnectionVerwaltung.ConnectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    // Parameter mit explizitem Typ + Länge → bessere Performance/Indexnutzung als AddWithValue.
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

                // ----- RL/TL aus JSON nachtragen → Spalte "Zukauf" -----
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

                // Tabelle merken (für Filterung).
                _auftraegeDataTable = dt;

                // (Optional) Daten als JSON dumpen – aktuell deaktiviert.
                // File.WriteAllText("AnsichtAuftraege.json", JsonConvert.SerializeObject(_auftraegeDataTable, Formatting.Indented));

                // Anzeigen + altes Bild aus PictureBox entfernen.
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

        // -----------------------------------------------------------------------------------------------------------------
        // Bequemlichkeits-Wrapper: nimmt den aktuell markierten Belag und ruft UpdateDgvAnsichtAuftraege damit auf.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateDgvAnsichtAuftraege2()
        {
            if (DgvLadeBelaege.CurrentRow != null)
            {
                selectedBelagValue = DgvLadeBelaege.CurrentRow.Cells["Belag"].Value?.ToString();
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuftraege(selectedBelagValue);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Detail-Grid (DgvInformationZuAuftrag) für eine Auftragsnummer befüllen + bestimmte Spalten ausblenden/formatieren.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateDgvInformationZuAuftrag(string auftragsNummer)
        {
            string query = "SELECT * FROM LN_ProdOrders_PRD WHERE pdno_prodnr = @Auftragsnummer";
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@Auftragsnummer", auftragsNummer);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // SqlDataAdapter öffnet/schließt Verbindung automatisch
                }

                DgvInformationZuAuftrag.DataSource = dataTable;

                // ---- Nicht relevante Spalten ausblenden ----
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

                // Datumsspalte formatieren.
                if (DgvInformationZuAuftrag.Columns.Contains("trdf_enddate"))
                {
                    DgvInformationZuAuftrag.Columns["trdf_enddate"].DefaultCellStyle.Format = "dd.MM.yyyy";
                }

                // Zahlen ohne Nachkommastellen.
                string[] zahlenspalten = { "qplo_sollstk", "qcmp_iststk", "qcmp2_vorstk" };
                foreach (string spalte in zahlenspalten)
                {
                    if (DgvInformationZuAuftrag.Columns.Contains(spalte))
                    {
                        DgvInformationZuAuftrag.Columns[spalte].DefaultCellStyle.Format = "N0";
                    }
                }

                DgvInformationZuAuftrag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Abrufen der Daten: " + ex.Message);
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Zählt eindeutige Auftragsnummern in letzteAuftragsnummern.json und schreibt das Ergebnis ins Label.
        // -----------------------------------------------------------------------------------------------------------------
        private void ZaehleGestarteteAuftraege()
        {
            string jsonPfad = "letzteAuftragsnummern.json";

            if (File.Exists(jsonPfad))
            {
                try
                {
                    string jsonInhalt = File.ReadAllText(jsonPfad);
                    List<string> auftragsnummern = JsonConvert.DeserializeObject<List<string>>(jsonInhalt) ?? new List<string>();

                    // .Distinct() entfernt Duplikate, .Count() zählt die übrigen.
                    int anzahlEindeutig = auftragsnummern.Distinct().Count();

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

        // -----------------------------------------------------------------------------------------------------------------
        // Baut aus dem Belag-Grid eine kleine DataTable (Belag, AVOs) zusammen.
        // Wird verwendet, wenn das Grid keine DataTable-DataSource hat (Fallback).
        // -----------------------------------------------------------------------------------------------------------------
        public DataTable BuildBelagTableFromGrid()
        {
            DataTable belagTable = new DataTable();
            belagTable.Columns.Add("Belag", typeof(string));
            belagTable.Columns.Add("AVOs", typeof(int));

            // Wenn die nötigen Spalten fehlen → leere Tabelle zurückgeben.
            if (!DgvLadeBelaege.Columns.Contains("Belag") || !DgvLadeBelaege.Columns.Contains("AVOs"))
            {
                return belagTable;
            }

            foreach (DataGridViewRow row in DgvLadeBelaege.Rows)
            {
                if (row.IsNewRow) continue; // letzte leere "Neue Zeile" überspringen

                string belag = row.Cells["Belag"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(belag)) continue;

                int avos = 0;
                int.TryParse(row.Cells["AVOs"].Value?.ToString(), out avos);
                belagTable.Rows.Add(belag, avos);
            }
            return belagTable;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf Button "Stk. offen" → entsprechendes Fenster (modal) öffnen.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnShowStkOffen_Click(object sender, EventArgs e)
        {
            Form_StkVorAvo form_StkVorAvo = new Form_StkVorAvo();
            form_StkVorAvo.ShowDialog();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf Button "Rückstand":
        //  - DataSource versuchen als DataTable zu casten und kopieren
        //  - falls das nicht klappt: Tabelle aus dem Grid rekonstruieren (Fallback)
        //  - Form_Rueckstand öffnen (nicht modal, .Show() → Hauptfenster bleibt bedienbar)
        // -----------------------------------------------------------------------------------------------------------------
        private void btnRueckstand_Click(object sender, EventArgs e)
        {
            // "as DataTable" → Cast, der bei Fehlern null liefert (wirft keine Exception).
            // "?.Copy()" → nur wenn nicht null, kopieren.
            // "?? ..." → falls null, nutze stattdessen die Fallback-Tabelle.
            DataTable belagTable = (DgvLadeBelaege.DataSource as DataTable)?.Copy() ?? BuildBelagTableFromGrid();

            Form_Rueckstand form_Rueckstand = new Form_Rueckstand(belagTable);
            form_Rueckstand.Show();
        }
    }
}
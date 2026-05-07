using System;                       // Basis-.NET-Typen wie String, Exception, EventArgs
using System.Collections.Generic;   // Generische Collections wie List<T>
using System.ComponentModel;        // ListSortDirection für die DGV-Sortierung
using System.Data;                  // DataTable, DataRow, DataView – alles ADO.NET-In-Memory
using System.Data.SqlClient;        // SqlConnection, SqlCommand, SqlDataReader für SQL Server
using System.Linq;                  // LINQ-Erweiterungsmethoden wie Last(), Take(), ToArray()
using System.Windows.Forms;         // Windows-Forms-Steuerelemente: Form, ComboBox, DataGridView usw.

namespace VerwaltungKST1127.GlasWaschDaten
{
    public partial class Form_GlasWaschDaten : Form
    {
        // Verbindungszeichenfolge zur SQL-Server-Datenbank.
        // Als const, damit sie nur an einer Stelle gepflegt werden muss.
        private const string ConnectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False";

        // Merker, ob die DGV-Spalten (Header-Ausrichtung, ausgeblendete Spalten, AutoSize)
        // bereits konfiguriert wurden. Spart Zeit beim Filtern, da die Konfiguration
        // sich beim Filtern nicht ändert und nicht jedes Mal neu gesetzt werden muss.
        private bool _dgvConfigured = false;

        // Schalter, um SelectedIndexChanged-Events der ComboBoxen zu unterdrücken,
        // während wir die ComboBoxen programmatisch neu befüllen. Ohne diesen Schalter
        // würden sich die ComboBoxen während des Befüllens gegenseitig triggern.
        private bool _suppressEvents = false;

        public Form_GlasWaschDaten()
        {
            InitializeComponent();   // Erzeugt die im Designer angelegten Steuerelemente

            // Während des initialen Befüllens dürfen keine Filter-Events feuern
            _suppressEvents = true;
            FillComboBoxArtikel();
            FillComboBoxGNummer(null);
            FillComboBoxGlassorte(null);
            _suppressEvents = false;

            UpdateLabels();        // Anzahl-Labels berechnen
            LoadDataGridView();    // DGV direkt beim Öffnen mit allen Daten befüllen
        }

        // Hilfsmethode: schneidet vom Anzeigetext der Artikel-ComboBox
        // (Format "ARTNR - SEITE") nur die ARTNR ab.
        // Wird beim Laden des DGV als Filterparameter benötigt.
        private static string ExtractArtikelNummer(string comboValue)
        {
            if (string.IsNullOrEmpty(comboValue)) return comboValue;

            string[] parts = comboValue.Split('-');

            // Nur abschneiden, wenn der letzte Teil eine reine Zahl ist (= SEITE),
            // sonst wäre eine ARTNR mit Bindestrich z.B. "12-345" beschädigt.
            if (parts.Length > 1 && int.TryParse(parts.Last().Trim(), out _))
            {
                return string.Join("-", parts.Take(parts.Length - 1)).Trim();
            }
            return comboValue.Trim();
        }

        // Befüllt die ArtikelComboBox mit "ARTNR - SEITE"-Einträgen
        private void FillComboBoxArtikel()
        {
            // Sortierung erfolgt direkt in SQL – das ist schneller, als später in der ComboBox zu sortieren
            const string query = @"
                SELECT DISTINCT ARTNR, SEITE
                FROM Serienlinsen
                ORDER BY ARTNR ASC";

            try
            {
                // Erst alle Daten in eine Liste laden, dann auf einen Schlag in die ComboBox.
                // Das ist deutlich schneller als pro Zeile direkt in die ComboBox einzufügen.
                var items = new List<string>();

                // 'using' garantiert, dass conn und cmd am Ende des Blocks (auch im Fehlerfall)
                // automatisch geschlossen und freigegeben werden – ohne explizites Close()/Dispose().
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    // 'using' auch hier: der DataReader wird sicher geschlossen,
                    // sodass die Verbindung danach wieder für andere Befehle frei ist.
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Anzeigeformat z.B. "12345 - links"
                            items.Add($"{reader["ARTNR"]} - {reader["SEITE"]}");
                        }
                    }
                }

                // BeginUpdate/EndUpdate verhindern Neuzeichnen der ComboBox bei jedem Add
                ComboBoxArtikel.BeginUpdate();
                ComboBoxArtikel.Items.Clear();
                ComboBoxArtikel.Sorted = false;                  // Reihenfolge kommt schon aus der DB
                ComboBoxArtikel.Items.AddRange(items.ToArray()); // Massenladen statt Schleife
                ComboBoxArtikel.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Artikel: " + ex.Message);
            }
        }

        // Befüllt die GNummer-ComboBox – optional gefiltert nach einer Glassorte
        private void FillComboBoxGNummer(string glasssorte = null)
        {
            // Je nach Filter unterschiedliche Abfrage
            string query = string.IsNullOrEmpty(glasssorte)
                ? "SELECT DISTINCT GNummer FROM Glassdaten"
                : "SELECT DISTINCT GNummer FROM Glassdaten WHERE Glass = @Glasssorte";

            try
            {
                var items = new List<string>();

                // 'using' = automatisches Schließen und Aufräumen der Verbindung und des Commands.
                // Wir nutzen pro Aufruf eine neue Verbindung – dank Connection-Pooling
                // ist das nicht teurer und vermeidet Probleme mit hängengebliebenen Verbindungen.
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(glasssorte))
                        cmd.Parameters.AddWithValue("@Glasssorte", glasssorte);

                    conn.Open();

                    // 'using' für den Reader – wird beim Verlassen sauber geschlossen
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            items.Add(reader["GNummer"].ToString());
                    }
                }

                ComboBoxGNummer.BeginUpdate();
                ComboBoxGNummer.Items.Clear();
                ComboBoxGNummer.Sorted = true;
                ComboBoxGNummer.Items.AddRange(items.ToArray());
                ComboBoxGNummer.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der GNummern: " + ex.Message);
            }
        }

        // Befüllt die Glassorten-ComboBox – optional gefiltert nach einer GNummer
        private void FillComboBoxGlassorte(string gNummer = null)
        {
            string query = string.IsNullOrEmpty(gNummer)
                ? "SELECT DISTINCT Glass FROM Glassdaten"
                : "SELECT DISTINCT Glass FROM Glassdaten WHERE GNummer = @GNummer";

            try
            {
                var items = new List<string>();

                // 'using' sorgt dafür, dass conn und cmd nach dem Block automatisch
                // geschlossen werden – auch dann, wenn eine Exception fliegt.
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(gNummer))
                        cmd.Parameters.AddWithValue("@GNummer", gNummer);

                    conn.Open();

                    // Auch der Reader wird per 'using' deterministisch freigegeben
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            items.Add(reader["Glass"].ToString());
                    }
                }

                ComboBoxGlassorte.BeginUpdate();
                ComboBoxGlassorte.Items.Clear();
                ComboBoxGlassorte.Sorted = true;
                ComboBoxGlassorte.Items.AddRange(items.ToArray());
                ComboBoxGlassorte.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Glassorten: " + ex.Message);
            }
        }

        // Reagiert auf Auswahl in der Artikel-ComboBox -> DGV neu filtern
        private void ComboBoxArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;   // beim programmatischen Befüllen NICHT filtern
            LoadDataGridView();
        }

        // Reagiert auf Auswahl einer GNummer
        private void ComboBoxGNummer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            if (ComboBoxGNummer.SelectedItem != null)
            {
                string selectedGNummer = ComboBoxGNummer.SelectedItem.ToString();

                // Glassorten passend zur GNummer nachladen, dabei keine Folge-Events erzeugen
                _suppressEvents = true;
                FillComboBoxGlassorte(selectedGNummer);
                _suppressEvents = false;

                LoadDataGridView();
            }
            else
            {
                // Keine Auswahl mehr -> Glassorten wieder komplett laden
                _suppressEvents = true;
                FillComboBoxGlassorte(null);
                _suppressEvents = false;
            }
        }

        // Reagiert auf Auswahl einer Glassorte
        private void ComboBoxGlassorte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            if (ComboBoxGlassorte.SelectedItem != null)
            {
                string selectedGlasssorte = ComboBoxGlassorte.SelectedItem.ToString();

                // GNummern passend zur Glassorte nachladen
                _suppressEvents = true;
                FillComboBoxGNummer(selectedGlasssorte);
                _suppressEvents = false;

                LoadDataGridView();
            }
            else
            {
                _suppressEvents = true;
                FillComboBoxGNummer(null);
                _suppressEvents = false;
            }
        }

        // Setzt alle ComboBoxen, die Spalten-Konfiguration und das DGV vollständig zurück.
        // Wichtig: hier NICHT auf vorherigen DGV-Status verlassen.
        private void ResetComboBoxes()
        {
            // Während des Resets keine Folge-Events der ComboBoxen feuern lassen
            _suppressEvents = true;

            // Auswahl und Eingabetext aller ComboBoxen leeren
            ComboBoxArtikel.SelectedIndex = -1;
            ComboBoxGNummer.SelectedIndex = -1;
            ComboBoxGlassorte.SelectedIndex = -1;
            ComboBoxArtikel.Text = "";
            ComboBoxGNummer.Text = "";
            ComboBoxGlassorte.Text = "";

            // Inhalte der ComboBoxen ohne Filter neu laden
            FillComboBoxArtikel();
            FillComboBoxGNummer(null);
            FillComboBoxGlassorte(null);

            _suppressEvents = false;

            // ---- WICHTIG für den Reset-Bug ----
            // Vorherige Sortierung, DataSource-Bindung und Spalten-Konfiguration
            // komplett verwerfen, damit das DGV wirklich frisch befüllt wird.
            DgvAuswahlGlasdaten.DataSource = null;
            DgvAuswahlGlasdaten.Rows.Clear();      // sicherheitshalber, falls noch Restzeilen
            DgvAuswahlGlasdaten.Columns.Clear();   // alle alten Spalten weg
            _dgvConfigured = false;                 // Spalten beim nächsten Laden neu konfigurieren

            // DGV mit ALLEN Daten neu befüllen
            LoadDataGridView();
        }

        // Klick auf Zurücksetzen-Button
        private void BtnResett_Click(object sender, EventArgs e)
        {
            ResetComboBoxes();
        }

        // Lädt das DGV anhand der aktuell in den ComboBoxen ausgewählten Filter
        private void LoadDataGridView()
        {
            // Basis-Query mit "1=1", damit weitere Filter einfach mit AND angehängt werden können
            string query = "SELECT * FROM Serienlinsen WHERE 1=1";

            // Filterwerte einmal extrahieren und merken,
            // damit Query-Aufbau und Parameter-Binding nicht doppelt rechnen.
            string selectedArtikel = null;
            string selectedGNummer = null;
            string selectedGlasssorte = null;

            if (ComboBoxArtikel.SelectedItem != null)
            {
                selectedArtikel = ExtractArtikelNummer(ComboBoxArtikel.SelectedItem.ToString());
                query += " AND ARTNR = @Artikel";
            }
            if (ComboBoxGNummer.SelectedItem != null)
            {
                selectedGNummer = ComboBoxGNummer.SelectedItem.ToString();
                query += " AND [G_Nummer] = @GNummer";
            }
            if (ComboBoxGlassorte.SelectedItem != null)
            {
                selectedGlasssorte = ComboBoxGlassorte.SelectedItem.ToString();
                query += " AND GLASSORTE = @Glasssorte";
            }

            try
            {
                var dataTable = new DataTable();

                // 'using' für conn und cmd: am Ende des Blocks werden Verbindung und Command
                // automatisch geschlossen/disposed – kein manuelles Close() nötig,
                // und auch im Fehlerfall keine offenen Verbindungen.
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (selectedArtikel != null) cmd.Parameters.AddWithValue("@Artikel", selectedArtikel);
                    if (selectedGNummer != null) cmd.Parameters.AddWithValue("@GNummer", selectedGNummer);
                    if (selectedGlasssorte != null) cmd.Parameters.AddWithValue("@Glasssorte", selectedGlasssorte);

                    conn.Open();

                    // 'using' für den DataAdapter: Adapter wird sauber freigegeben,
                    // nachdem er die Daten in die DataTable übertragen hat.
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                // Performance: Layout während der Aktualisierung pausieren, sonst zeichnet das DGV
                // bei jeder Eigenschaftsänderung neu.
                DgvAuswahlGlasdaten.SuspendLayout();
                DgvAuswahlGlasdaten.DataSource = dataTable;

                // Die Spalten-Konfiguration (Header-Mitte, Ausblenden, AutoSize)
                // wird nur einmal pro DGV-Instanz gesetzt – beim Filtern ändern sich die
                // Spalten nicht, also kein Grund das jedes Mal neu zu machen.
                if (!_dgvConfigured && DgvAuswahlGlasdaten.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in DgvAuswahlGlasdaten.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Indizes der Spalten, die nicht angezeigt werden sollen
                    int[] columnsToHide = { 0, 1, 2, 4, 5, 6, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 28, 30, 31,
                        36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 55, 57 };
                    foreach (int index in columnsToHide)
                    {
                        if (index < DgvAuswahlGlasdaten.Columns.Count)
                            DgvAuswahlGlasdaten.Columns[index].Visible = false;
                    }

                    DgvAuswahlGlasdaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    _dgvConfigured = true;
                }

                // Sortierung absteigend nach ARTNR – nur, wenn die Spalte existiert
                // und überhaupt Daten vorhanden sind, sonst wirft Sort() eine Exception.
                if (DgvAuswahlGlasdaten.Columns.Contains("ARTNR") && dataTable.Rows.Count > 0)
                {
                    DgvAuswahlGlasdaten.Sort(DgvAuswahlGlasdaten.Columns["ARTNR"], ListSortDirection.Ascending);
                }

                DgvAuswahlGlasdaten.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Daten: " + ex.Message);
            }
        }

        // Zeigt die Anzahl unterschiedlicher GNummern und Glassorten in den Labels an
        private void UpdateLabels()
        {
            // Beide Counts in einer einzigen Abfrage = nur ein Datenbank-Roundtrip
            const string query = @"
                SELECT 
                    (SELECT COUNT(DISTINCT [G_Nummer]) FROM Serienlinsen) AS AnzahlGNummern,
                    (SELECT COUNT(DISTINCT GLASSORTE) FROM Serienlinsen) AS AnzahlGlassorten";

            try
            {
                // 'using' schließt Verbindung und Command zuverlässig nach Gebrauch
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    // 'using' für den Reader – auch er wird sauber geschlossen,
                    // damit die Verbindung danach wieder voll nutzbar ist.
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LblAnzahlGNummern.Text = $"Anzahl G-Nummern: {reader["AnzahlGNummern"]}";
                            LblAnzahlGlassorten.Text = $"Anzahl Glassorten: {reader["AnzahlGlassorten"]}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren der Labels: " + ex.Message);
            }
        }
    }
}
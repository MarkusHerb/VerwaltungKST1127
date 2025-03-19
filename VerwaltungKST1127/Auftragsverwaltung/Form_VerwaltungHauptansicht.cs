using Newtonsoft.Json;
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic;
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
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

        public Form_VerwaltungHauptansicht()
        {
            InitializeComponent(); // Initialisierung der Komponenten des Formulars
            LoadDataForDgvAuftragZuBelag(); // Laden der Daten für das DataGridView dGvLadeBelaeg
        }

        private void LoadDataForDgvAuftragZuBelag()
        {
            try
            {
                // SQL-Abfrage
                string query = @"SELECT
                            Belag,
                            Chargen_Infor AS AVOs,
                            Gestartet,
                            Material,
                            BBM
                        FROM
                            [Auswahl Belag]
                        WHERE
                            Chargen_Infor > 0
                        ORDER BY
                            Belag ASC";
                // SQLCommand and SQLDataAdapter initialisieren
                SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                //Verbindung öffnen und schließen
                sqlConnectionVerwaltung.Open();
                adapter.Fill(dataTable);
                sqlConnectionVerwaltung.Close();

                // JSON-Datei speichern
                string jsonFilePath = "GeladeneBelaege.json";
                string jsonData = JsonConvert.SerializeObject(dataTable);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);

                // JSON-Datei laden und in DataGridView dGvLadeBelaeg anzeigen
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    DataTable jsonDataTable = JsonConvert.DeserializeObject<DataTable>(json);
                    DgvLadeBelaege.DataSource = jsonDataTable;
                }

                // Spalten anpassen, sodass sie das DataGridView ausfüllen
                DgvLadeBelaege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler in der Funktion LoadDataForDgvAuftragZuBelag: " + ex.Message);
            }
            finally
            {
                // Sicherstellen, dass die Verbindung geschlossen wird
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
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
                var selectedBelagValue = DgvLadeBelaege.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
                // Nur falls der Wert nicht null ist, wird er weiterverarbeitet
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuftraege(selectedBelagValue);
                }
            }
        }

        // Hilfsfunktion um das DgvAnsichtAuftraege zu aktualisieren
        private void UpdateDgvAnsichtAuftraege2()
        {
            if (DgvLadeBelaege.CurrentRow != null)
            {
                // Wert, der bei Belag steht, abrufen
                var selectedBelagValue = DgvLadeBelaege.CurrentRow.Cells["Belag"].Value?.ToString();
                // Nur falls der Wert nicht null ist, wird er weiterverarbeitet
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuftraege(selectedBelagValue);
                }
            }
        }

        // Funktion zum Aktualisieren des DataGridView dGvAnsichtAuftraege basierend auf der Auswahl in dGvLadeBelaege
        private void UpdateDgvAnsichtAuftraege(string selectedBelagValue)
        {
            try
            {
                // SQL-Abfrage mit LIKE-Klausel, um nach 'belagValue' in 'txta_acoinfo' zu suchen
                string query = @"
        SELECT 
            CONVERT(date, trdf_enddate) AS Enddatum,
            dsca_teilebez AS [Teilebez.],
            pdno_prodnr AS [Auftragsnr.],
            mitm_teilenr AS Artikel,
            opsta_avostat AS Status, 
            txta_avoinfo AS AVOinfo, 
            '' AS Material,  -- Spalte Material bleibt leer
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
            '' AS Zukauf, -- Spalte bleibt aktuell leer
            '' AS Dringend, -- Spalte bleibt aktuell leer
            CONVERT(date, import_date) AS Aktualisiert
        FROM 
            LN_ProdOrders_PRD 
        WHERE 
            opsta_avostat IN ('Active', 'Planned', 'Released') 
            AND txta_avoinfo LIKE '%' + @BelagValue + '%' 
            AND txta_avoinfo LIKE '%Vergüten%'
        ORDER
            BY trdf_enddate ASC";

                // SQL-Befehl vorbereiten und Parameter hinzufügen
                SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung);
                command.Parameters.AddWithValue("@BelagValue", selectedBelagValue);

                // Datenadapter und DataTable initialisieren
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                // Verbindung öffnen, Daten abrufen und in DataGridView laden
                sqlConnectionVerwaltung.Open();
                adapter.Fill(dataTable);
                sqlConnectionVerwaltung.Close();


                // Material für jede Zeile abrufen und setzen
                foreach (DataRow row in dataTable.Rows)
                {
                    string artikelNr = row["Artikel"].ToString();
                    int seite = int.Parse(row["Seite"].ToString());
                    string material = GetMaterialFromSerienlinsen(artikelNr, seite);
                    row["Material"] = material;
                }

                // JSON-Datei laden und deserialisieren
                RLTLData rltlData;
                string jsonFilePath2 = "rltl_data.json";
                if (System.IO.File.Exists(jsonFilePath2))
                {
                    var json = System.IO.File.ReadAllText(jsonFilePath2);
                    rltlData = JsonConvert.DeserializeObject<RLTLData>(json);
                }
                else
                {
                    rltlData = new RLTLData();
                }

                // Überprüfen Sie jede Zeile im DataTable und setzen Sie die ZU-Spalte basierend auf der JSON-Datei
                foreach (DataRow row in dataTable.Rows)
                {
                    // Sicherstellen, dass die Artikelnummer nicht null ist
                    string artikelNr = row["Artikel"].ToString();

                    // Überprüfen, ob die Artikelnummer in der RL- oder TL-Liste enthalten ist und setze den Wert entsprechend
                    if (rltlData.RL.Contains(artikelNr))
                    {
                        row["Zukauf"] = "R-Lager";
                    }
                    else if (rltlData.TL.Contains(artikelNr))
                    {
                        row["Zukauf"] = "T-Lager";
                    }
                }

                // Dringend-Daten aus Ansicht_Bildschirm abfragen
                string dringendQuery = @"SELECT Auftrag, Dringend FROM Ansicht_Bildschirm";
                SqlCommand dringendCommand = new SqlCommand(dringendQuery, sqlConnectionVerwaltung);
                SqlDataAdapter dringendAdapter = new SqlDataAdapter(dringendCommand);
                DataTable dringendTable = new DataTable();

                sqlConnectionVerwaltung.Open();
                dringendAdapter.Fill(dringendTable);
                sqlConnectionVerwaltung.Close();

                // Dictionary für schnelles Nachschlagen von Dringend-Werten basierend auf der Auftragsnummer erstellen
                var dringendDict = dringendTable.AsEnumerable()
                    .ToDictionary(row => row.Field<string>("Auftrag"),
                                  row => row.Field<string>("Dringend"));

                // Setzen des Dringend-Werts im Haupt-DataTable basierend auf Auftragsnummer
                foreach (DataRow row in dataTable.Rows)
                {
                    string auftragsNr = row["Auftragsnr."].ToString();

                    // Nachsehen, ob die Auftragsnummer in dringendDict existiert und Wert setzen
                    if (dringendDict.TryGetValue(auftragsNr, out string dringendWert))
                    {
                        row["Dringend"] = dringendWert;
                    }
                    else
                    {
                        row["Dringend"] = string.Empty; // Standardwert, falls kein Dringend-Wert gefunden wird
                    }
                }

                // JSON-Datei speichern für das ganze DgvAnsichtAuftraege
                string jsonFilePath = "AnsichtAuftraege.json";
                string jsonData = JsonConvert.SerializeObject(dataTable);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);

                // JSON-Datei laden und in DataGridView dGvAnsichtAuftraege anzeigen
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    DataTable jsonDataTable = JsonConvert.DeserializeObject<DataTable>(json);
                    DgvAnsichtAuftraege.DataSource = jsonDataTable;
                }

                // Spalten anpassen, sodass sie das DataGridView ausfüllen
                DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Zahlenformat der relevanten Spalten anpassen
                DgvAnsichtAuftraege.Columns["SollStk."].DefaultCellStyle.Format = "N0";
                DgvAnsichtAuftraege.Columns["IstStk."].DefaultCellStyle.Format = "N0";
                DgvAnsichtAuftraege.Columns["VorStk."].DefaultCellStyle.Format = "N0";
                DgvAnsichtAuftraege.Columns["Teilelager"].DefaultCellStyle.Format = "N0";
                DgvAnsichtAuftraege.Columns["Bereitstell"].DefaultCellStyle.Format = "N0";
                DgvAnsichtAuftraege.Columns["Jahresbedarf"].DefaultCellStyle.Format = "N0";
                // Werte in bestimmten Spalten mittig ausrichten
                DgvAnsichtAuftraege.Columns["SollStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["IstStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["VorStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["Teilelager"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["Bereitstell"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["Jahresbedarf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["Zukauf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DgvAnsichtAuftraege.Columns["Dringend"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // Headertexte der Spalten mittig ausrichten
                foreach (DataGridViewColumn column in DgvAnsichtAuftraege.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                // Optional: Spalten automatisch anpassen
                DgvAnsichtAuftraege.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                // Reihenbreite anpassen
                DgvAnsichtAuftraege.Columns["Teilebez."].Width = 170;
                DgvAnsichtAuftraege.Columns["Seite"].Width = 60;
                DgvAnsichtAuftraege.Columns["AVOinfo"].Width = 140;
                // Spalte ausblenden
                // DgvAnsichtAuftraege.Columns["Sortierwert"].Visible = false;
                // Picturebox zurücksetzten
                PictureBoxZeichnung.Image = null;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Fehler in der Funktion UpdateDgvAnsichtAuftraege: " + ex.Message);
            }
            finally
            {
                // Sicherstellen, dass die Verbindung geschlossen wird
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
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

        // Funktion zum Abrufen der Daten aus der Datenbank und zum Aktualisieren von DgvInformationZuAuftrag
        private void UpdateDgvInformationZuAuftrag(string auftragsNummer)
        {
            // SQL-Abfrage, um alle Datensätze mit der passenden Auftragsnummer aus LN_ProdOrders_PRD abzurufen
            string query = "SELECT * FROM LN_ProdOrders_PRD WHERE pdno_prodnr = @Auftragsnummer";

            // DataTable zur Speicherung der Ergebnisse
            DataTable dataTable = new DataTable();

            try
            {
                // SQL-Verbindung öffnen
                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Parameter zur Abfrage hinzufügen, um SQL-Injection zu vermeiden
                    command.Parameters.AddWithValue("@Auftragsnummer", auftragsNummer);

                    // Adapter einrichten und die Ergebnisse in die DataTable füllen
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }

                // Die abgerufenen Daten in das DataGridView DgvInformationZuAuftrag laden
                DgvInformationZuAuftrag.DataSource = dataTable;
                DgvInformationZuAuftrag.Columns["pdsta_prodstat"].Visible = false;
                DgvInformationZuAuftrag.Columns["refo_avotext"].Visible = false;
                DgvInformationZuAuftrag.Columns["mtyp_anlage"].Visible = false;
                DgvInformationZuAuftrag.Columns["trdt_startdate"].Visible = false;
                DgvInformationZuAuftrag.Columns["cwoc_kst"].Visible = false;
                DgvInformationZuAuftrag.Columns["opno2_voravo"].Visible = false;
                DgvInformationZuAuftrag.Columns["rutm2_sollzeit"].Visible = false;
                DgvInformationZuAuftrag.Columns["sutm_sollruest"].Visible = false;
                DgvInformationZuAuftrag.Columns["rutm_vorgabezeit"].Visible = false;
                DgvInformationZuAuftrag.Columns["dsca_teilebez"].Visible = false;
                DgvInformationZuAuftrag.Columns["srce_itemquelle"].Visible = false;
                DgvInformationZuAuftrag.Columns["goca_warentraeger"].Visible = false;
                DgvInformationZuAuftrag.Columns["pkdf_transportbehaelter"].Visible = false;
                DgvInformationZuAuftrag.Columns["qhnd1_stk_teilelager"].Visible = false;
                DgvInformationZuAuftrag.Columns["qhnd2_lagerbestand"].Visible = false;
                DgvInformationZuAuftrag.Columns["qana_bereitstellbestand"].Visible = false;
                DgvInformationZuAuftrag.Columns["demand_jahresbedarf"].Visible = false;
                DgvInformationZuAuftrag.Columns["created_date"].Visible = false;
                DgvInformationZuAuftrag.Columns["Last_modified"].Visible = false;
                DgvInformationZuAuftrag.Columns["import_date"].Visible = false;
                // Überprüfen, ob die relevanten Spalten existieren, bevor auf sie zugegriffen wird
                if (DgvInformationZuAuftrag.Columns["trdf_enddate"] != null)
                {
                    DgvInformationZuAuftrag.Columns["trdf_enddate"].DefaultCellStyle.Format = "dd.MM.yyyy";
                }

                // Zahlenformatierung für die Spalten von DgvInformationZuAuftrag
                if (DgvInformationZuAuftrag.Columns["qplo_sollstk"] != null)
                {
                    DgvInformationZuAuftrag.Columns["qplo_sollstk"].DefaultCellStyle.Format = "N0";
                }
                if (DgvInformationZuAuftrag.Columns["qcmp_iststk"] != null)
                {
                    DgvInformationZuAuftrag.Columns["qcmp_iststk"].DefaultCellStyle.Format = "N0";
                }
                if (DgvInformationZuAuftrag.Columns["qcmp2_vorstk"] != null)
                {
                    DgvInformationZuAuftrag.Columns["qcmp2_vorstk"].DefaultCellStyle.Format = "N0";
                }

                // Spalten anpassen, sodass sie das DataGridView ausfüllen
                DgvInformationZuAuftrag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung (z.B. Fehlerprotokollierung oder Benachrichtigung)
                MessageBox.Show("Fehler beim Abrufen der Daten: " + ex.Message);
            }
            finally
            {
                // Verbindung schließen
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
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

        // Event-Handler für das Klicken auf den Button BtnZukauf
        private void BtnZukauf_Click(object sender, EventArgs e)
        {
            Form_RLTL rltlForm = new Form_RLTL();
            rltlForm.ShowDialog();
        }

        // Grafische gestaltung des Dgvs 
        private void DgvAnsichtAuftraege_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
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
                    // Event-Handler für die Menüeinträge hinzufügen
                    setzeDringend1.Click += SetzeDringend1_Click;
                    setzeDringend2.Click += SetzeDringend2_Click;
                    resettDringend.Click += ResettDringend_Click;
                    serienlinse.Click += Serienlinse_Click;
                    // Verknüpfen Sie die anderen Menüeinträge mit ihren entsprechenden Methoden, falls erforderlich
                    // Menüeinträge zum Kontextmenü hinzufügen
                    contextMenu.Items.AddRange(new ToolStripItem[] { setzeDringend1, setzeDringend2, resettDringend, serienlinse });
                    // Kontextmenü an der Position des Mauszeigers anzeigen
                    contextMenu.Show(DgvAnsichtAuftraege, e.Location);
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
    }
}

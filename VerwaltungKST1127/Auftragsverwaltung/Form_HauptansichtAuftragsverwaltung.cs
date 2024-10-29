using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms;
using System.IO; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.Auftragsverwaltung
{
    public partial class Form_HauptansichtAuftragsverwaltung : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank Verwaltung
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        public Form_HauptansichtAuftragsverwaltung()
        {
            InitializeComponent();
            LoadDataForDgvAuftragZuBelag();
        }

        // Funtkion um das DataGridView dGvAuftragZuBelag zu befüllen
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

                //DataGridView befüllen
                dGvAuftragZuBelag.DataSource = dataTable;

                // Spalten anpassen, sodass sie das DataGridView ausfüllen
                dGvAuftragZuBelag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler in der Funktion LoadDataForDgvAuftragZuBelag" + ex.Message);
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

        // Wenn im DgvAuftragZuBelag geklickt wird
        private void dGvAuftragZuBelag_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Überprüfen, ob ein gültiger Klick erfolg ist
            if (e.RowIndex >= 0)
            {
                // Wert, der bei Belag steht, abrufen
                var selectedBelagValue = dGvAuftragZuBelag.Rows[e.RowIndex].Cells["Belag"].Value?.ToString();
                // Nur falls der Wert nicht null ist, wird er weiterverarbeitet
                if (!string.IsNullOrEmpty(selectedBelagValue))
                {
                    UpdateDgvAnsichtAuswahlAuftrag(selectedBelagValue);
                }
            }
        }

        // Das DgvAnsichtAuswahlAuftrag nach Belagsauswahl laden/aktualisieren
        private void UpdateDgvAnsichtAuswahlAuftrag(string selectedBelagValue)
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
                pdsta_prodstat AS Status, 
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
                '' AS Dringed, -- Spalte bleibt aktuell leer
                CONVERT(date, import_date) AS Aktualisiert
            FROM 
                LN_ProdOrders_PRD 
            WHERE 
                pdsta_prodstat IN ('Active', 'Planned', 'Released') 
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

                // JSON-Datei laden und deserialisieren
                RLTLData rltlData;
                string jsonFilePath = "rltl_data.json";
                if (File.Exists(jsonFilePath))
                {
                    var json = File.ReadAllText(jsonFilePath);
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
                        row["Zukauf"] = "TL-Lager";
                    }

                    // Material abfragen und setzen
                    int seite = int.Parse(row["Seite"].ToString()); // Die Seite von der Abfrage abholen
                    string material = GetMaterialFromSerienlinsen(artikelNr, seite);
                    row["Material"] = material;

                    //// Hole den Zeichnungspfad aus der dbo_Serienlinsen-Tabelle
                    //string zeichnungspfad = GetZeichnungspfad(artikelNr, seite);
                    //if (!string.IsNullOrEmpty(zeichnungspfad))
                    //{
                    //    // Bildpfad setzen
                    //    pictureBoxAnsichtArtikel.ImageLocation = zeichnungspfad; // pictureBoxZeichnung ist die PictureBox
                    //}
                }

                // Zweite Abfrage: Holen Sie alle 'pdno_prodnr' mit Status 'Gestartet' aus der Tabelle 'ProdOrders_Stamm'
                string startedQuery = @"
                    SELECT 
                        Auftrag 
                    FROM 
                        ProdOrders_Stamm 
                    WHERE 
                        opsta_avostat = 'Gestartet'";

                SqlCommand startedCommand = new SqlCommand(startedQuery, sqlConnectionVerwaltung);
                SqlDataAdapter startedAdapter = new SqlDataAdapter(startedCommand);
                DataTable startedTable = new DataTable();
                sqlConnectionVerwaltung.Open();
                startedAdapter.Fill(startedTable);
                sqlConnectionVerwaltung.Close();

                // Alle Auftragsnummern mit Status 'Gestartet' in eine Liste konvertieren
                List<string> gestarteteAuftragsNummern = startedTable.AsEnumerable()
                    .Select(row => row.Field<string>("Auftrag"))
                    .ToList();

                // Überprüfen Sie jede Zeile im DataTable und setzen Sie den Status auf 'Gestartet', falls erforderlich
                foreach (DataRow row in dataTable.Rows)
                {
                    // Sicherstellen, dass die Auftragsnummer nicht null ist
                    string auftragsNummer = row["Auftragsnr."].ToString();

                    // Überprüfen, ob die Auftragsnummer in der gestarteten Liste enthalten ist
                    if (gestarteteAuftragsNummern.Contains(auftragsNummer))
                    {
                        // Setzen des Wertes der Status-Zelle auf 'Gestartet', wenn Auftragsnummer übereinstimmt
                        row["Status"] = "Gestartet";
                    }
                    // Material abfragen und setzen
                    string mitmTeilenNr = row["Artikel"].ToString();
                    int seite = int.Parse(row["Seite"].ToString()); // Die Seite von der Abfrage abholen
                    string material = GetMaterialFromSerienlinsen(mitmTeilenNr, seite);
                    row["Material"] = material;
                }

                // Füge eine neue Spalte für den Sortierwert hinzu
                dataTable.Columns.Add("Sortierwert", typeof(int));


                // Setze den Sortierwert basierend auf dem Status
                foreach (DataRow row in dataTable.Rows)
                {
                    switch (row["Status"].ToString())
                    {
                        case "Gestartet":
                            row["Sortierwert"] = 1;
                            break;
                        case "Active":
                            row["Sortierwert"] = 2;
                            break;
                        case "Released":
                            row["Sortierwert"] = 3;
                            break;
                        case "Planned":
                            row["Sortierwert"] = 4;
                            break;
                        default:
                            row["Sortierwert"] = 5; // für unerwartete Status
                            break;
                    }
                }

                // Sortiere die DataTable nach der neuen Spalte und dann nach Enddatum
                DataView dataView = new DataView(dataTable);
                dataView.Sort = "Sortierwert ASC, Enddatum ASC"; // Zuerst nach Sortierwert, dann nach Enddatum

                // DataGridView mit der sortierten DataView füllen
                dGvAnsichtAuswahlAuftrag.DataSource = dataView.ToTable();

                // Zahlenformat der relevanten Spalten anpassen
                dGvAnsichtAuswahlAuftrag.Columns["SollStk."].DefaultCellStyle.Format = "N0";
                dGvAnsichtAuswahlAuftrag.Columns["IstStk."].DefaultCellStyle.Format = "N0";
                dGvAnsichtAuswahlAuftrag.Columns["VorStk."].DefaultCellStyle.Format = "N0";
                dGvAnsichtAuswahlAuftrag.Columns["Teilelager"].DefaultCellStyle.Format = "N0";
                dGvAnsichtAuswahlAuftrag.Columns["Bereitstell"].DefaultCellStyle.Format = "N0";
                dGvAnsichtAuswahlAuftrag.Columns["Jahresbedarf"].DefaultCellStyle.Format = "N0";
                // Werte in bestimmten Spalten mittig ausrichten
                dGvAnsichtAuswahlAuftrag.Columns["SollStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvAnsichtAuswahlAuftrag.Columns["IstStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvAnsichtAuswahlAuftrag.Columns["VorStk."].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvAnsichtAuswahlAuftrag.Columns["Teilelager"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvAnsichtAuswahlAuftrag.Columns["Bereitstell"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvAnsichtAuswahlAuftrag.Columns["Jahresbedarf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvAnsichtAuswahlAuftrag.Columns["Zukauf"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // Headertexte der Spalten mittig ausrichten
                foreach (DataGridViewColumn column in dGvAnsichtAuswahlAuftrag.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                // Optional: Spalten automatisch anpassen
                dGvAnsichtAuswahlAuftrag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                // Reihenbreite anpassen
                dGvAnsichtAuswahlAuftrag.Columns["Teilebez."].Width = 170;
                dGvAnsichtAuswahlAuftrag.Columns["Seite"].Width = 60;
                // Spalte ausblenden
                dGvAnsichtAuswahlAuftrag.Columns["Sortierwert"].Visible = false;
                // Picturebox zurücksetzten
                pictureBoxAnsichtArtikel.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Aktualisieren von dGvTest1: {ex.Message}");
            }
            finally
            {
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

        // Grafische gestaltung des dgvs
        private void dGvAnsichtAuswahlAuftrag_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            // Überprüfen, ob es sich um eine Spalte handelt
            if (dGvAnsichtAuswahlAuftrag.Columns[e.ColumnIndex].Name == "Status")
            {
                // Überprüfen, ob der Status "Gestartet" ist
                if (e.Value != null && e.Value.ToString() == "Gestartet")
                {
                    dGvAnsichtAuswahlAuftrag.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }

            // Überprüfe, ob in der Reihe ZU etwas steht
            if (dGvAnsichtAuswahlAuftrag.Columns[e.ColumnIndex].Name == "Zukauf")
            {
                if (e.Value != null && e.Value.ToString() == "R-Lager")
                {
                    e.CellStyle.BackColor = Color.GreenYellow;
                }
                else if ( e.Value != null && e.Value.ToString() == "TL-Lager")
                {
                    e.CellStyle.BackColor = Color.Salmon;
                }
            }

        }

        // Öffnet das Formular um Artikel für Zukauf einzugeben
        private void btnOpenTLTLForm_Click(object sender, EventArgs e)
        {
            Form_RLTL rltlForm = new Form_RLTL();
            rltlForm.ShowDialog();
        }

        private void dGvAnsichtAuswahlAuftrag_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile ausgewählt ist
            if (dGvAnsichtAuswahlAuftrag.CurrentRow != null)
            {
                // Die ausgewählte Zeile abrufen
                DataGridViewRow selectedRow = dGvAnsichtAuswahlAuftrag.CurrentRow;

                // Artikelnummer und Seite aus der ausgewählten Zeile abrufen
                string artikelNr = selectedRow.Cells["Artikel"].Value.ToString();
                int seite = int.Parse(selectedRow.Cells["Seite"].Value.ToString());

                // Zeichnungspfad abrufen
                string zeichnungspfad = GetZeichnungspfad(artikelNr, seite);

                // Wenn der Zeichnungspfad nicht leer ist, das Bild in der PictureBox anzeigen
                if (!string.IsNullOrEmpty(zeichnungspfad))
                {
                    pictureBoxAnsichtArtikel.ImageLocation = zeichnungspfad;
                }
                else
                {
                    // Optional: Wenn kein Bild gefunden wird, ein Platzhalterbild oder eine Nachricht setzen
                    pictureBoxAnsichtArtikel.Image = null; // oder ein Standardbild setzen
                }
            }
        }
    }
}

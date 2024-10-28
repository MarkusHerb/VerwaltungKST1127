using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

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
                trdf_enddate,
                pdno_prodnr, 
                pdsta_prodstat, 
                txta_avoinfo, 
                qplo_sollstk,
                qcmp_iststk,
                dsca_teilebez
            FROM 
                LN_ProdOrders_PRD 
            WHERE 
                pdsta_prodstat = 'Active'
                AND txta_avoinfo LIKE '%' + @BelagValue + '%'";

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

                // DataGridView dGvTest1 mit den neuen Daten füllen
                dGvAnsichtAuswahlAuftrag.DataSource = dataTable;

                // Optional: Spalten automatisch anpassen
                dGvAnsichtAuswahlAuftrag.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
    }
}

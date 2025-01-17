using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)


namespace VerwaltungKST1127.RingSpannzange
{
    public partial class Form_NeuerArtikelZuRing : Form
    {
        // Eigenschaft RingId
        public string RingId { get; set; }

        // SOA127 Shuttle
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        public Form_NeuerArtikelZuRing(string ringId)
        {
            InitializeComponent();
            RingId = ringId;
            lblID.Text = ringId;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Variablen abspeichern
            string ringId = RingId;
            string artikelNummer = txtBoxArtnr.Text;
            string seite = txtBoxSeite.Text;
            string belag = txtBoxBelag.Text;
            string bbm = string.Empty; // Initialisierung von bbm entfernt

            // Validierung der txtBoxBBM
            if (!string.IsNullOrEmpty(txtBoxBBM.Text) && txtBoxBBM.Text != "BBM")
            {
                MessageBox.Show("Wenn ein Wert in BBM eingetragen ist, darf er nur 'BBM' sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Wenn "BBM" eingegeben wurde, setzen wir den Wert, sonst bleibt bbm leer
            if (txtBoxBBM.Text == "BBM")
            {
                bbm = "BBM";
            }

            string bemerkung = txtBoxBemerkung.Text;

            if (string.IsNullOrWhiteSpace(artikelNummer) || string.IsNullOrWhiteSpace(seite) || string.IsNullOrWhiteSpace(belag))
            {
                MessageBox.Show("Artikelnummer, Seite und Belag muss ausgefüllt sein!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True"))
                {
                    sqlConnectionShuttle.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO Artikel ([Ring_ID], [Artikelnummer], [Seite], [Belag], [BBM], [Bemerkungen]) VALUES (@Ring_ID, @Artikelnummer, @Seite, @Belag, @BBM, @Bemerkungen)", sqlConnectionShuttle))
                    {
                        sqlCommand.Parameters.AddWithValue("@Ring_ID", ringId);
                        sqlCommand.Parameters.AddWithValue("@Artikelnummer", artikelNummer);
                        sqlCommand.Parameters.AddWithValue("@Seite", seite);
                        sqlCommand.Parameters.AddWithValue("@Belag", belag);
                        sqlCommand.Parameters.AddWithValue("@BBM", bbm);
                        sqlCommand.Parameters.AddWithValue("@Bemerkungen", bemerkung);

                        sqlCommand.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Artikel erfolgreich gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Schließen des Formulars nach dem Speichern
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnectionShuttle.Close();
                this.Close();
            }
        }
    }
}

using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)


namespace VerwaltungKST1127.RingSpannzange
{
    public partial class Form_NeuerArtikelZuRing : Form
    {
        // Öffentliche Eigenschaft zur Speicherung der Ring-ID
        public string RingId { get; set; }

        // SQL-Verbindungsobjekt zur Datenbank "SOA127_Shuttle"
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        // Konstruktor, erhält die Ring-ID als Parameter
        public Form_NeuerArtikelZuRing(string ringId)
        {
            InitializeComponent();     // Initialisiert UI-Komponenten
            RingId = ringId;           // Speichert Ring-ID
            lblID.Text = ringId;       // Zeigt die Ring-ID im Label an
        }

        // Event-Handler für den Klick auf den "Speichern"-Button
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Eingabewerte aus den Textfeldern holen
            string ringId = RingId;
            string artikelNummer = txtBoxArtnr.Text;
            string seite = txtBoxSeite.Text;
            string belag = txtBoxBelag.Text;
            string bbm = string.Empty; // Initialisierung von BBM mit leerem String

            // Validierung: BBM darf nur leer sein oder exakt "BBM" enthalten
            if (!string.IsNullOrEmpty(txtBoxBBM.Text) && txtBoxBBM.Text != "BBM")
            {
                MessageBox.Show("Wenn ein Wert in BBM eingetragen ist, darf er nur 'BBM' sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Abbruch bei ungültigem BBM-Wert
            }

            // Wenn der Benutzer "BBM" eingegeben hat, wird der Wert gesetzt
            if (txtBoxBBM.Text == "BBM")
            {
                bbm = "BBM";
            }

            // Kommentartextfeld übernehmen
            string bemerkung = txtBoxBemerkung.Text;

            // Pflichtfeldprüfung: Artikelnummer, Seite und Belag müssen ausgefüllt sein
            if (string.IsNullOrWhiteSpace(artikelNummer) || string.IsNullOrWhiteSpace(seite) || string.IsNullOrWhiteSpace(belag))
            {
                MessageBox.Show("Artikelnummer, Seite und Belag muss ausgefüllt sein!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Abbruch, wenn Pflichtfelder fehlen
            }

            try
            {
                // Öffnen der SQL-Verbindung in einem using-Block zur sicheren Verwaltung von Ressourcen
                using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True"))
                {
                    sqlConnectionShuttle.Open(); // Verbindung öffnen

                    // SQL-Befehl zur Einfügung eines neuen Artikels vorbereiten
                    using (SqlCommand sqlCommand = new SqlCommand(
                        "INSERT INTO Artikel ([Ring_ID], [Artikelnummer], [Seite], [Belag], [BBM], [Bemerkungen]) " +
                        "VALUES (@Ring_ID, @Artikelnummer, @Seite, @Belag, @BBM, @Bemerkungen)",
                        sqlConnectionShuttle))
                    {
                        // Übergabe der Parameterwerte an das SQL-Kommando
                        sqlCommand.Parameters.AddWithValue("@Ring_ID", ringId);
                        sqlCommand.Parameters.AddWithValue("@Artikelnummer", artikelNummer);
                        sqlCommand.Parameters.AddWithValue("@Seite", seite);
                        sqlCommand.Parameters.AddWithValue("@Belag", belag);
                        sqlCommand.Parameters.AddWithValue("@BBM", bbm);
                        sqlCommand.Parameters.AddWithValue("@Bemerkungen", bemerkung);

                        sqlCommand.ExecuteNonQuery(); // SQL-Befehl ausführen
                    }
                }

                // Erfolgreiche Speicherung bestätigen
                MessageBox.Show("Artikel erfolgreich gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Formular nach dem Speichern schließen
                this.Close();
            }
            catch (SqlException sqlEx)
            {
                // Fehlerbehandlung bei SQL-spezifischen Problemen
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Allgemeine Fehlerbehandlung
                MessageBox.Show("Fehler: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Sicherstellen, dass Verbindung geschlossen wird (obwohl innerhalb von `using` redundant)
                sqlConnectionShuttle.Close();

                // Formular sicherheitshalber schließen
                this.Close();
            }
        }
    }
}


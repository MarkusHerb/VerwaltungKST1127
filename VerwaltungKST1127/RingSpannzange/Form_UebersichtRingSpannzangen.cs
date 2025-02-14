using Microsoft.Identity.Client;
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für die Implementierung von Komponenten und Steuerelementen (z.B. IContainer, Container, Component)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)



namespace VerwaltungKST1127.RingSpannzange
{
    public partial class DgvZuordnungArtikel : Form
    {
        // SOA127 Shuttle
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        // Deklaration der Variablen auf Klassenebene
        public string RingId { get; private set; }

        // Klasse für die Passwort-Eingabeaufforderung
        public static class Prompt
        {
            
            public static string ShowDialog(string text, string caption) 
            {
                // Erstellen eines neuen Formulars
                Form prompt = new Form()
                {
                    Width = 400,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };

                Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 340 }; // Text für die Passwort-Eingabeaufforderung
                TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 340, UseSystemPasswordChar = true }; // Textbox für die Passworteingabe
                Button confirmation = new Button() { Text = "OK", Left = 250, Width = 100, Top = 80, DialogResult = DialogResult.OK }; // Button für die Bestätigung der Passworteingabe
                confirmation.Click += (sender, e) => { prompt.Close(); }; // Event-Handler für den Button "OK"
                // Hinzufügen der Steuerelemente zum Formular
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;
                // Anzeigen des Dialogs
                return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : null;
            }
        }

        public DgvZuordnungArtikel()
        {
            InitializeComponent();
            UpdateDgvAnsichtRinge();
            txtBoxStkZange.Text = "1";
            txtBoxVorrichtungsnummerZange.Text = "Z";
            txtBoxVorrichtungsNummerRing.Text = "R";
            // Setzen der SelectionMode-Eigenschaft auf FullRowSelect
            DgvArtikel.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        // Event-Händler für den Button "Ring hinzufügen"
        private void BtnRingSave_Click(object sender, EventArgs e)
        {
            string vorrichtungsnummerRing = txtBoxVorrichtungsNummerRing.Text;

            if (!decimal.TryParse(txtBoxDmInnenRing.Text, out decimal dmInnen))
            {
                MessageBox.Show("Ungültiger Wert für DM Ring. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            dmInnen = Math.Round(dmInnen, 1);

            if (!decimal.TryParse(txtBoxDmAussenRing.Text, out decimal dmAussen))
            {
                MessageBox.Show("Ungültiger Wert für DM Ring Außen. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            dmAussen = Math.Round(dmAussen, 1);

            if (!decimal.TryParse(txtBoxDmFreibereich.Text, out decimal dmFreibereich))
            {
                MessageBox.Show("Ungültiger Wert für DM Freibereich. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            dmFreibereich = Math.Round(dmFreibereich, 1);

            string lochSegment = comboBoxLochSegment.Text;
            string anzahlRing = txtBoxAnzahlProRing.Text;
            string anzahlRingGesamt = txtBoxRingeGesamt.Text;

            // Speichern der eingegebenen Daten in der Datenbank
            SaveDataRing(vorrichtungsnummerRing, dmInnen, dmAussen, dmFreibereich, lochSegment, anzahlRing, anzahlRingGesamt);
        }

        // Event-Händler für den Button "Spannzange hinzufügen"
        private void BtnSpannzangeSave_Click(object sender, EventArgs e)
        {
            string vorrichtungsnummerZange = txtBoxVorrichtungsnummerZange.Text;

            if (!decimal.TryParse(txtBoxDmZange.Text, out decimal dmZange))
            {
                MessageBox.Show("Ungültiger Wert für DM Zange. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            dmZange = Math.Round(dmZange, 1);

            string anzahlZange = txtBoxStkZange.Text;

            // Speichern der eingegebenen Daten in der Datenbank
            SaveDataZange(vorrichtungsnummerZange, dmZange, anzahlZange);
        }

        // Funktion zum Speichern der eingegebenen Daten in die Datenbank
        private void SaveDataRing(string vorrichtungsnummerRing, decimal dmInnen, decimal dmAussen, decimal dmFreibereich, string lochSegment, string anzahlRing, string anzahlRingGesamt)
        {
            // Verbindung zur Datenbank herstellen
            try
            {
                using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source = sqlvgt.swarovskioptik.at; Initial Catalog = SOA127_Shuttle; Integrated Security = True"))
                {
                    sqlConnectionShuttle.Open();

                    // Überprüfen, ob die Vorrichtungsnummer bereits existiert
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Ring_Stamm WHERE [Vorrichtungsnummer] = @Vorrichtungsnummer", sqlConnectionShuttle))
                    {
                        sqlCommand.Parameters.AddWithValue("@Vorrichtungsnummer", vorrichtungsnummerRing);
                        int count = (int)sqlCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Die Vorrichtungsnummer existiert bereits. Bitte geben Sie eine andere Vorrichtungsnummer ein.");
                            return;
                        }
                    }

                    // Einfügen der Daten in die Datenbank
                    using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO Ring_Stamm ([Vorrichtungsnummer], [Durchmesser_Innen], [Segmetdurchmesser], [Freidurchmesser], [Gerundet_SegmentDM], [AnzahlRing], [Stück]) " +
                        "VALUES (@Vorrichtungsnummer, @Durchmesser_Innen, @Segmetdurchmesser, @Freidurchmesser, @Gerundet_SegmentDM, @AnzahlRing, @Stueck)", sqlConnectionShuttle))
                    {
                        sqlCommand.Parameters.AddWithValue("@Vorrichtungsnummer", vorrichtungsnummerRing);
                        sqlCommand.Parameters.AddWithValue("@Durchmesser_Innen", dmInnen);
                        sqlCommand.Parameters.AddWithValue("@Segmetdurchmesser", dmAussen);
                        sqlCommand.Parameters.AddWithValue("@Freidurchmesser", dmFreibereich);
                        sqlCommand.Parameters.AddWithValue("@Gerundet_SegmentDM", lochSegment);
                        sqlCommand.Parameters.AddWithValue("@AnzahlRing", anzahlRing);
                        sqlCommand.Parameters.AddWithValue("@Stueck", anzahlRingGesamt);

                        sqlCommand.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Daten erfolgreich in die Datenbank gespeichert.");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
            finally
            {
                UpdateDgvAnsichtRinge();
                sqlConnectionShuttle.Close();
            }
        }

        // Funktion zum Speichern der eingegebenen Daten in die Datenbank
        private void SaveDataZange(string vorrichtungsnummerZange, decimal dmZange, string anzahlZange)
        {
            // Verbindung zur Datenbank herstellen
            try
            {
                using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source = sqlvgt.swarovskioptik.at; Initial Catalog = SOA127_Shuttle; Integrated Security = True"))
                {
                    sqlConnectionShuttle.Open();

                    // Überprüfen, ob die Vorrichtungsnummer bereits existiert
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Ring_Stamm WHERE [Vorrichtungsnummer] = @Vorrichtungsnummer", sqlConnectionShuttle))
                    {
                        sqlCommand.Parameters.AddWithValue("@Vorrichtungsnummer", vorrichtungsnummerZange);
                        int count = (int)sqlCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Die Vorrichtungsnummer existiert bereits. Bitte geben Sie eine andere Vorrichtungsnummer ein.");
                            return;
                        }
                    }

                    // Einfügen der Daten in die Datenbank
                    using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO Ring_Stamm ([Vorrichtungsnummer],[Durchmesser_Innen],[AnzahlRing]) VALUES (@Vorrichtungsnummer, @Durchmesser_Innen, @AnzahlRing)", sqlConnectionShuttle))
                    {
                        sqlCommand.Parameters.AddWithValue("@Vorrichtungsnummer", vorrichtungsnummerZange);
                        sqlCommand.Parameters.AddWithValue("@Durchmesser_Innen", dmZange);
                        sqlCommand.Parameters.AddWithValue("@AnzahlRing", anzahlZange);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Daten erfolgreich in die Datenbank gespeichert.");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
            finally
            {
                UpdateDgvAnsichtRinge();
                sqlConnectionShuttle.Close();
            }
        }

        // Funktion zum Aktualisieren der Daten in der DataGridView
        private void UpdateDgvAnsichtRinge()
        {
            try
            {
                using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source = sqlvgt.swarovskioptik.at; Initial Catalog = SOA127_Shuttle; Integrated Security = True"))
                {
                    sqlConnectionShuttle.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Ring_Stamm", sqlConnectionShuttle))
                    {
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataTable dataTable = new DataTable();
                        sqlDataAdapter.Fill(dataTable);
                        DgvAnsichtAlleRingeSpannzangen.DataSource = dataTable;
                        //Absteigend nach vorrichtungsnummer sortieren
                        DgvAnsichtAlleRingeSpannzangen.Sort(DgvAnsichtAlleRingeSpannzangen.Columns[1], ListSortDirection.Ascending);
                        DgvAnsichtAlleRingeSpannzangen.Columns[7].Visible = false;
                        DgvAnsichtAlleRingeSpannzangen.Columns[9].Visible = false;
                        // Den Headertext im DGV selbst ändern
                        DgvAnsichtAlleRingeSpannzangen.Columns[0].HeaderText = "ID";
                        DgvAnsichtAlleRingeSpannzangen.Columns[1].HeaderText = "Vor_Nr";
                        DgvAnsichtAlleRingeSpannzangen.Columns[2].HeaderText = "DM_Innen";
                        DgvAnsichtAlleRingeSpannzangen.Columns[3].HeaderText = "Frei_DM";
                        DgvAnsichtAlleRingeSpannzangen.Columns[4].HeaderText = "'Stk.";
                        DgvAnsichtAlleRingeSpannzangen.Columns[5].HeaderText = "DM_Außen";
                        DgvAnsichtAlleRingeSpannzangen.Columns[6].HeaderText = "LochSem.";
                        DgvAnsichtAlleRingeSpannzangen.Columns[8].HeaderText = "Stk/Ring";

                        // Breite der Spalten automatisch anpassen an das DGV
                        DgvAnsichtAlleRingeSpannzangen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Wenn auf einen Zelle geklickt wird, dann soll im DgvBoxen, passierend auf die RingId, die Daten aus der Tabelle Ring_Detail angezeigt werden. 

                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
            finally
            {
                sqlConnectionShuttle.Close();
            }
        }

        // Event-Händler für das Klicken auf eine Zelle in der DgvAnsichtAlleRingeSpannzangen
        private void DgvAnsichtAlleRingeSpannzangen_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Ausgewählte Zeile abrufen
                DataGridViewRow row = DgvAnsichtAlleRingeSpannzangen.Rows[e.RowIndex];

                // Wert der öffentlichen Variablen setzen
                RingId = row.Cells["Ring_ID"].Value.ToString();

                // Angeklickte Zeile in Textboxen anzeigen
                // Wenn Vorrichtungsnummer mit R, A, K oder T beginnt

                if (row.Cells["Vorrichtungsnummer"].Value.ToString().StartsWith("R") || row.Cells["Vorrichtungsnummer"].Value.ToString().StartsWith("T") 
                    || row.Cells["Vorrichtungsnummer"].Value.ToString().StartsWith("A") || row.Cells["Vorrichtungsnummer"].Value.ToString().StartsWith("K"))
                {
                    txtBoxVorrichtungsNummerRing.Text = row.Cells["Vorrichtungsnummer"].Value.ToString();
                    txtBoxDmInnenRing.Text = row.Cells["Durchmesser_Innen"].Value.ToString();
                    txtBoxDmAussenRing.Text = row.Cells["Segmetdurchmesser"].Value.ToString();
                    txtBoxDmFreibereich.Text = row.Cells["Freidurchmesser"].Value.ToString();
                    comboBoxLochSegment.Text = row.Cells["Gerundet_SegmentDM"].Value.ToString();
                    txtBoxAnzahlProRing.Text = row.Cells["AnzahlRing"].Value.ToString();
                    txtBoxVorrichtungsnummerZange.Text = "Z";
                    txtBoxDmZange.Text = string.Empty;
                    txtBoxStkZange.Text = "1";
                    txtBoxRingeGesamt.Text = row.Cells["Stück"].Value.ToString();
                }
                // Wenn mit Vorrichtunsnummer mit Z beginnt
                else if (row.Cells["Vorrichtungsnummer"].Value.ToString().StartsWith("Z"))
                {
                    txtBoxVorrichtungsnummerZange.Text = row.Cells["Vorrichtungsnummer"].Value.ToString();
                    txtBoxDmZange.Text = row.Cells["Durchmesser_Innen"].Value.ToString();
                    txtBoxStkZange.Text = row.Cells["AnzahlRing"].Value.ToString();
                    txtBoxVorrichtungsNummerRing.Text = "R";
                    txtBoxDmInnenRing.Text = string.Empty;
                    txtBoxDmAussenRing.Text = string.Empty;
                    txtBoxDmFreibereich.Text = string.Empty;
                    comboBoxLochSegment.Text = string.Empty;
                    txtBoxAnzahlProRing.Text = string.Empty;
                    txtBoxRingeGesamt.Text = string.Empty;
                }


                try
                {
                    using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True"))
                    {
                        sqlConnectionShuttle.Open(); // Öffnen der Verbindung zur Datenbank

                        // Daten für DgvBoxen abrufen
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Ring_Detail WHERE [Ring_ID] = @Ring_ID", sqlConnectionShuttle))
                        {
                            sqlCommand.Parameters.AddWithValue("@Ring_ID", RingId);
                            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                            DataTable dataTable = new DataTable();
                            sqlDataAdapter.Fill(dataTable);
                            DgvBoxen.DataSource = dataTable;
                            DgvBoxen.Columns[0].Visible = false;
                            DgvBoxen.Columns[4].Visible = false;
                            DgvBoxen.Columns[5].Visible = false;
                            DgvBoxen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }

                        // Daten für DgvArtikel abrufen
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Artikel WHERE [Ring_ID] = @Ring_ID", sqlConnectionShuttle))
                        {
                            sqlCommand.Parameters.AddWithValue("@Ring_ID", RingId);
                            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                            DataTable dataTable = new DataTable();
                            sqlDataAdapter.Fill(dataTable);
                            DgvArtikel.DataSource = dataTable;
                            DgvArtikel.Columns[0].Visible = false;
                            DgvArtikel.Columns[3].Visible = false;
                            DgvArtikel.Columns[7].Visible = false;
                            DgvArtikel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            DgvArtikel.Columns["Bemerkungen"].DisplayIndex = DgvArtikel.Columns.Count - 1;
                            DgvArtikel.Columns[1].HeaderText = "ArtNr.";
                            DgvArtikel.Columns[4].Width = 40;
                            DgvArtikel.Columns[5].Width = 50;
                            DgvArtikel.Columns[6].Width = 50;
                            DgvArtikel.Columns[2].HeaderText = "Info";
                            DgvArtikel.Columns[2].Width = 50;
                            // Seite mittig stellen
                            DgvArtikel.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }

        // Event-Händler für das Klicken auf den Button "Artikel hinzufügen"
        private void BtnNeuerArtikelZuRing_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob RingId gesetzt ist
            if (string.IsNullOrEmpty(RingId))
            {
                MessageBox.Show("Bitte wählen Sie zuerst einen Ring aus.",
                                "Hinweis",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Neues Formular öffnen und RingId übergeben
            Form_NeuerArtikelZuRing form_NeuerArtikelZuRing = new Form_NeuerArtikelZuRing(RingId);
            form_NeuerArtikelZuRing.Show();
            // Wenn das Formular geschlossen wird, soll die DataGridView aktualisiert werden
            form_NeuerArtikelZuRing.FormClosed += (s, args) => UpdateDgvArtikel();
        }

        // Event-Händler für das Klicken auf den Button "Artikel löschen"
        private void BtnArtikelLoeschen_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile im DgvArtikel ausgewählt ist
            if (DgvArtikel.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bitte wählen Sie zuerst einen Artikel aus.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Passwort-Eingabeaufforderung anzeigen
            string enteredPassword = Prompt.ShowDialog("Bitte geben Sie das Passwort ein, um den Artikel zu löschen:", "Passwort erforderlich");
            if (enteredPassword != "1127")
            {
                MessageBox.Show("Falsches Passwort. Der Artikel wurde nicht gelöscht.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bestätigungsdialog anzeigen
            DialogResult result = MessageBox.Show("Sind Sie sicher, dass Sie diesen Artikel löschen möchten?", "Bestätigung", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }

            // Ausgewählte Zeile abrufen
            DataGridViewRow selectedRow = DgvArtikel.SelectedRows[0];
            string artikelId = selectedRow.Cells["Artikel_ID"].Value.ToString();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True"))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("DELETE FROM Artikel WHERE [Artikel_ID] = @Artikel_ID", sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Artikel_ID", artikelId);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Artikel erfolgreich gelöscht.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateDgvAnsichtRinge(); // Aktualisieren der DataGridView
                UpdateDgvArtikel(); // Aktualisieren des DgvArtikel
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Funktion um das DgvAritkel zu aktualisieren
        private void UpdateDgvArtikel()
        {
            try
            {
                using (SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True"))
                {
                    sqlConnectionShuttle.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Artikel WHERE [Ring_ID] = @Ring_ID", sqlConnectionShuttle))
                    {
                        sqlCommand.Parameters.AddWithValue("@Ring_ID", RingId);
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                        DataTable dataTable = new DataTable();
                        sqlDataAdapter.Fill(dataTable);
                        DgvArtikel.DataSource = dataTable;
                        DgvArtikel.Columns[0].Visible = true;
                        DgvArtikel.Columns[3].Visible = false;
                        DgvArtikel.Columns[7].Visible = false;
                        DgvArtikel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        DgvArtikel.Columns["Bemerkungen"].DisplayIndex = DgvArtikel.Columns.Count - 1;
                        DgvArtikel.Columns[0].HeaderText = "ID";
                        DgvArtikel.Columns[1].HeaderText = "ArtNr.";
                        DgvArtikel.Columns[2].HeaderText = "Info";
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }
    }
}

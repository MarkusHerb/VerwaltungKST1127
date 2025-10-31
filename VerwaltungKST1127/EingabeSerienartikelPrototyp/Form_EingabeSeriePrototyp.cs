using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic;
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    public partial class Form_EingabeSeriePrototyp : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank Verwaltung
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // SOA127 Shuttle
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        public Form_EingabeSeriePrototyp()
        {
            InitializeComponent();
            // Befülle die GNummer-ComboBox beim Initialisieren des Formulars
            FillComboBoxGNummer();
            // Event-Handler für die ComboBoxBelagVerguetung verbinden
            ComboboxBelagVerguetung.SelectedIndexChanged += ComboboxBelagVerguetung_SelectedIndexChanged;
            // Eventhandler für die ComboBoxGNummer hinzufügen, um auf Änderungen zu reagieren
            ComboboxGNummer.SelectedIndexChanged += ComboboxGNummer_SelectedIndexChanged;
            // Initialisiere die Glassorten-ComboBox mit allen Glassorten (keine GNummer-Auswahl)
            FillComboboxGlassorte(null);
            // Befülle die GNummer-ComboBox beim Initialisieren des Formulars
            FillComboboxRing();
            FillComboBoxBelagVerguetungRueckseite();
            SetPlaceholders();
            lblWichtig1.Enabled = false;
            lblWichtig2.Enabled = false;
        }

        // ############## Selbst erstellte Funktionen ################
        // Funktion um die ComboboxBelagVerguetung/Rueckseite mit eindeutigen GNummer-Werten zu befüllen
        private void FillComboBoxBelagVerguetungRueckseite()
        {
            // SQL-Abfrage, um alle eindeutigen Werte aus der Spalte 'Belag' zu erhalten
            string query = "SELECT DISTINCT Belag FROM Maximalwerte_Farbauswertung";

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // Erstelle einen SqlCommand für die Abfrage
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Führe die Abfrage aus und erhalte einen SqlDataReader
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die ComboBox, bevor neue Einträge hinzugefügt werden
                        ComboboxBelagVerguetung.Items.Clear();
                        ComboboxBelagRueckseite.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole den Wert der Spalte 'Belag'
                            string belagValue = reader["Belag"].ToString();

                            // Füge den Wert der ComboBox hinzu
                            ComboboxBelagVerguetung.Items.Add(belagValue);
                            ComboboxBelagRueckseite.Items.Add(belagValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Belagvergütungen: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Funktion um die ComboboxProzess basierend auf der ausgewählten Belagvergütung zu befüllen
        private void FillComboboxProzess(string belagVerguetung)
        {
            // SQL-Abfrage, um die zugehörigen Prozesse und Brechwerte für die ausgewählte Belagvergütung abzurufen
            string query = @"
        SELECT DISTINCT Prozess, Brechwert
        FROM Maximalwerte_Farbauswertung
        WHERE Belag = @Belag
        ORDER BY Prozess ASC"; // Sortierung nach Prozess in aufsteigender Reihenfolge

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Parameter für die Abfrage setzen
                    sqlCommand.Parameters.AddWithValue("@Belag", belagVerguetung);

                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die ComboBox bevor neue Einträge hinzugefügt werden
                        ComboboxProzess.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole den Prozess und den Brechwert aus der aktuellen Zeile
                            string prozessValue = reader["Prozess"].ToString();
                            string brechwertValue = reader["Brechwert"].ToString();

                            // Kombiniere Prozess und Brechwert für die Anzeige in der ComboBox
                            string displayValue = $"{prozessValue} {brechwertValue}";

                            // Füge den kombinierten Wert der ComboBox hinzu
                            ComboboxProzess.Items.Add(displayValue);
                        }
                    }
                }

                // Deaktiviere die automatische Sortierung der ComboBox
                ComboboxProzess.Sorted = false; // Setzen auf false, um die Einträge in der Reihenfolge zu lassen, in der sie hinzugefügt wurden
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Prozesse: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Funktion um die ComboboxGNummer mit eindeutigen GNummer-Werten zu befüllen
        private void FillComboBoxGNummer()
        {
            // SQL-Abfrage, um alle eindeutigen GNummern abzurufen
            string query = "SELECT DISTINCT GNummer FROM Glassdaten";

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole die GNummer aus der aktuellen Zeile
                            string gNummerValue = reader["GNummer"].ToString();

                            // Füge die GNummer der Combobox hinzu
                            ComboboxGNummer.Items.Add(gNummerValue);
                        }
                    }
                }

                // Sortiere die Einträge in der ComboBox alphabetisch
                ComboboxGNummer.Sorted = true;
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der GNummern: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Funktion um die ComboboxGlassorte basierend auf der ausgewählten GNummer zu befüllen
        private void FillComboboxGlassorte(string gNummer)
        {
            // Wenn keine GNummer ausgewählt ist, lade alle Glassorten
            string query;

            if (string.IsNullOrEmpty(gNummer))
            {
                // Lade alle Glassorten, wenn keine GNummer ausgewählt wurde
                query = "SELECT DISTINCT Glass FROM Glassdaten";
            }
            else
            {
                // Lade nur die Glassorten, die zur ausgewählten GNummer gehören
                query = "SELECT DISTINCT Glass FROM Glassdaten WHERE GNummer = @GNummer";
            }

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionVerwaltung.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Wenn eine GNummer vorhanden ist, füge den Parameter hinzu
                    if (!string.IsNullOrEmpty(gNummer))
                    {
                        sqlCommand.Parameters.AddWithValue("@GNummer", gNummer);
                    }

                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die Glassorten-ComboBox, bevor neue Einträge hinzugefügt werden
                        ComboboxGlassorte.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole den Glass-Wert aus der aktuellen Zeile
                            string glassValue = reader["Glass"].ToString();

                            // Füge die Glassorte der ComboBox hinzu
                            ComboboxGlassorte.Items.Add(glassValue);
                        }
                        sqlConnectionVerwaltung.Close();
                    }
                }

                // Sortiere die Einträge in der ComboBox alphabetisch
                ComboboxGlassorte.Sorted = true;
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Glassorten: " + ex.Message);
            }
        }

        // Funktion um die ComboboxRing basierend auf der ausgewählten Durchmesser zu befüllen
        private void FillComboboxRing()
        {
            // SQL-Abfrage, um Vorrichtungsnummer und den dazugehörigen Durchmesser_Innen abzurufen und nach Durchmesser_Innen zu sortieren
            string query = @"
            SELECT DISTINCT Vorrichtungsnummer, Durchmesser_Innen
            FROM Ring_Stamm
            WHERE Vorrichtungsnummer NOT LIKE 'Z%'  -- Ausschließen von Vorrichtungsnummern, die mit 'Z' beginnen
            ORDER BY Vorrichtungsnummer ASC"; // Sortierung nach Durchmesser_Innen in aufsteigender Reihenfolge

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionShuttle.Open();

                // SQL-Befehl mit der Abfrage erstellen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionShuttle))
                {
                    // Führe die Abfrage aus und erhalte die Daten
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leere die ComboBox bevor neue Einträge hinzugefügt werden
                        ComboboxRing.Items.Clear();

                        // Überprüfe, ob es Daten gibt
                        while (reader.Read())
                        {
                            // Hole die Vorrichtungsnummer und den Durchmesser_Innen aus der aktuellen Zeile
                            string ringNummerValue = reader["Vorrichtungsnummer"].ToString();
                            string durchmesserValue = reader["Durchmesser_Innen"].ToString();

                            // Kombiniere Vorrichtungsnummer und Durchmesser_Innen für die Anzeige in der ComboBox
                            string displayValue = $"{ringNummerValue} - {durchmesserValue} mm";

                            // Füge den kombinierten Wert der ComboBox hinzu
                            ComboboxRing.Items.Add(displayValue);
                        }
                    }
                }

                // Deaktiviere die automatische Sortierung der ComboBox
                ComboboxRing.Sorted = false; // Setzen auf false, um die Einträge in der Reihenfolge zu lassen, in der sie hinzugefügt wurden
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, wenn etwas schiefgeht
                MessageBox.Show("Fehler beim Laden der Ringe: " + ex.Message);
            }
            finally
            {
                // Stelle sicher, dass die Verbindung nach der Abfrage geschlossen wird
                if (sqlConnectionShuttle.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionShuttle.Close();
                }
            }
        }

        // Wenn der Button geklickt wird, wird die ComboboxRing nach dem Durchmesser sortiert
        private void BtnOrderDmRing_Click(object sender, EventArgs e)
        {
            // Liste zum Speichern der Einträge aus der ComboBox
            List<Tuple<string, double>> ringList = new List<Tuple<string, double>>();

            // Extrahiere die Einträge aus der ComboBox
            foreach (var item in ComboboxRing.Items)
            {
                string displayValue = item.ToString();
                string[] parts = displayValue.Split('-');
                if (parts.Length > 1 && double.TryParse(parts[1].Replace(" mm", "").Trim(), out double durchmesser))
                {
                    string ringNummer = parts[0].Trim();
                    ringList.Add(new Tuple<string, double>(ringNummer, durchmesser));
                }
            }

            // Sortiere die Liste nach Durchmesser_Innen
            ringList = ringList.OrderBy(r => r.Item2).ToList();

            // Leere die ComboBox und füge die sortierten Einträge hinzu
            ComboboxRing.Items.Clear();
            foreach (var ring in ringList)
            {
                string displayValue = $"{ring.Item1} - {ring.Item2} mm";
                ComboboxRing.Items.Add(displayValue);
            }
        }

        // Wenn der index der ComboboxRing geändert wird
        // Aktualisierte Methode zur Auswahländerung in der Combobox
        private void ComboboxRing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxRing.SelectedItem != null)
            {
                // Den ausgewählten Eintrag aus der ComboBox extrahieren
                string selectedItem = ComboboxRing.SelectedItem.ToString();

                // Extrahiere den Ringnamen (z.B., "R450") aus dem kombinierten String in der ComboBox
                string ringName = selectedItem.Split('-')[0].Trim();

                // Rufe die Methode auf und übergebe den Ringnamen, um die Stückzahl zu laden
                GetStueckSegmentCharge(ringName);
            }
        }

        // Methode um die Stückzahl für den ausgewählten Ring zu erhalten und anzuzeigen
        private void GetStueckSegmentCharge(string ringName)
        {
            // SQL-Abfrage, um die Werte der Spalten Gerundet_SegmentDM und AnzahlRing für den ausgewählten Ring in Ring_Stamm zu finden
            string query = @"
    SELECT Gerundet_SegmentDM, AnzahlRing
    FROM Ring_Stamm
    WHERE Vorrichtungsnummer = @RingName"; // Filtere die Zeilen, die genau mit dem Ringnamen übereinstimmen

            try
            {
                // Öffne die SQL-Verbindung
                sqlConnectionShuttle.Open();

                // SQL-Befehl erstellen und Parameter setzen
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionShuttle))
                {
                    sqlCommand.Parameters.AddWithValue("@RingName", ringName);

                    // Führe die Abfrage aus und erhalte die Werte
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int gerundetSegment = reader.GetInt32(reader.GetOrdinal("Gerundet_SegmentDM"));
                            int anzahlRing = reader.GetInt32(reader.GetOrdinal("AnzahlRing"));

                            // Dictionary mit festen Werten für Gerundet_Segment
                            Dictionary<int, int> stueckProSegment = new Dictionary<int, int>
                    {
                        {0,0},
                        {2,2},
                        {5,5},
                        {7,7},
                        {13,458},
                        {18,242},
                        {20,210},
                        {21,210},
                        {24,171},
                        {28,120},
                        {30,119},
                        {32,96},
                        {37,84},
                        {39,66},
                        {47,49},
                        {53,36},
                        {59,28},
                        {62,28},
                        {67,28},
                        {73,23},
                        {85,12},
                        {86,17},
                        {93,15},
                        {105,16},
                        {126,7},
                        {999,0}
                    };

                            // Überprüfen, ob der Wert im Dictionary vorhanden ist und die entsprechende Stückzahl abrufen
                            if (stueckProSegment.TryGetValue(gerundetSegment, out int stueckzahl))
                            {
                                // Wenn AnzahlRing nicht 0 oder 1 ist, multipliziere stueckzahl mit AnzahlRing
                                if (anzahlRing != 0 && anzahlRing != 1)
                                {
                                    stueckzahl *= anzahlRing;
                                }

                                // Aktualisiere die Textbox mit der gefundenen Stückzahl
                                TxtboxStkSegment.Text = stueckzahl.ToString();

                                // Optional: Berechne und setze die Gesamtstückzahl in einer anderen Textbox
                                int stueckzahlGesamt = stueckzahl * 3;
                                TxtboxStkCharge.Text = stueckzahlGesamt.ToString();
                            }
                            else
                            {
                                // Wenn kein passender Wert gefunden wurde, setze die Textboxen auf 0
                                TxtboxStkSegment.Text = "0";
                                TxtboxStkCharge.Text = "0";
                            }
                        }
                        else
                        {
                            // Wenn keine Daten gefunden wurden, setze die Textboxen auf 0
                            TxtboxStkSegment.Text = "0";
                            TxtboxStkCharge.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Zeige eine Fehlermeldung an, falls ein Fehler auftritt
                MessageBox.Show("Fehler beim Abrufen der Stückzahl: " + ex.Message);
            }
            finally
            {
                // Schließe die Verbindung
                if (sqlConnectionShuttle.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionShuttle.Close();
                }
            }
        }

        // Funktion um die Daten in die Datenbank zu speichern
        private void SpeichereDatenInDatenbank(string artikelnummer, string bezeichnung, string status, string gruppenname, string zukauf, string flaeche
            , string gNummer, string glassorte, decimal durchmesser, string durchmesserWaschen, decimal freibereich, decimal dicke, string seite, decimal brechwert
            , string radiusVerguetung, string radiusRueckseite, string belagVerguetung, string prozess, string belagRueckseite, string ring, string stkSegment
            , string stkGesamt, decimal zeitProzess, DateTime eingabedatum, string bemerkungArtikel, string vorreinigen, string ucm, string aceton, string bemerkungWaschen
            , string revoNummer, string pfadZeichnungAuflegen, string pfadZusatzinfo, string textZusatzinfo)
        {
            try
            {
                using (SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False"))
                {
                    sqlConnectionVerwaltung.Open();

                    string query = @"
                INSERT INTO Serienlinsen ([ARTNR], [BEZ], [Status], [GruppenName], [Zukauf], [Innen-Aussen], [G_Nummer], [GLASSORTE], [DM], [Waschen_DM], [FREI], [DICKE], [SEITE],
                    [ND], [Radius1], [Radius2], [Belag1], [VERGBELAG], [MATERIAL], [Belag2], [RING], [STK_SEGM], [STK_CHARGE], [CHARGENZEIT], [Anmerkungsdatum], [BEMERKUNG],
                    [Vorreinigung], [HFE_Anlage], [Aceton], [Waschanmerkungen], [refo_avonr], [Zeichnungspfad], [InfoZeichnung], [InfoZeichnung_Bemerkungen])
                VALUES (@ARTNR, @BEZ, @Status, @GruppenName, @Zukauf, @InnenAussen, @G_Nummer, @GLASSORTE, @DM, @Waschen_DM, @FREI, @DICKE, @SEITE,
                    @ND, @Radius1, @Radius2, @Belag1, @VERGBELAG, @MATERIAL, @Belag2, @RING, @STK_SEGM, @STK_CHARGE, @CHARGENZEIT, @Anmerkungsdatum, @BEMERKUNG,
                    @Vorreinigung, @HFE_Anlage, @Aceton, @Waschanmerkungen, @refo_avonr, @Zeichnungspfad, @InfoZeichnung, @InfoZeichnung_Bemerkungen)";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        // Parameter hinzufügen
                        sqlCommand.Parameters.AddWithValue("@ARTNR", artikelnummer);
                        sqlCommand.Parameters.AddWithValue("@BEZ", bezeichnung);
                        sqlCommand.Parameters.AddWithValue("@Status", status);
                        sqlCommand.Parameters.AddWithValue("@GruppenName", gruppenname);
                        sqlCommand.Parameters.AddWithValue("@Zukauf", zukauf);
                        sqlCommand.Parameters.AddWithValue("@InnenAussen", flaeche);
                        sqlCommand.Parameters.AddWithValue("@G_Nummer", gNummer);
                        sqlCommand.Parameters.AddWithValue("@GLASSORTE", glassorte);
                        sqlCommand.Parameters.AddWithValue("@DM", durchmesser);
                        sqlCommand.Parameters.AddWithValue("@Waschen_DM", durchmesserWaschen);
                        sqlCommand.Parameters.AddWithValue("@FREI", freibereich);
                        sqlCommand.Parameters.AddWithValue("@DICKE", dicke);
                        sqlCommand.Parameters.AddWithValue("@SEITE", seite);
                        sqlCommand.Parameters.AddWithValue("@ND", brechwert);
                        sqlCommand.Parameters.AddWithValue("@Radius1", radiusVerguetung);
                        sqlCommand.Parameters.AddWithValue("@Radius2", radiusRueckseite);
                        sqlCommand.Parameters.AddWithValue("@Belag1", belagVerguetung);
                        sqlCommand.Parameters.AddWithValue("@VERGBELAG", belagVerguetung);
                        sqlCommand.Parameters.AddWithValue("@MATERIAL", prozess);
                        sqlCommand.Parameters.AddWithValue("@Belag2", belagRueckseite);
                        sqlCommand.Parameters.AddWithValue("@RING", ring);
                        sqlCommand.Parameters.AddWithValue("@STK_SEGM", stkSegment);
                        sqlCommand.Parameters.AddWithValue("@STK_CHARGE", stkGesamt);
                        sqlCommand.Parameters.AddWithValue("@CHARGENZEIT", zeitProzess);
                        sqlCommand.Parameters.AddWithValue("@Anmerkungsdatum", eingabedatum);
                        sqlCommand.Parameters.AddWithValue("@BEMERKUNG", bemerkungArtikel);
                        sqlCommand.Parameters.AddWithValue("@Vorreinigung", vorreinigen);
                        sqlCommand.Parameters.AddWithValue("@HFE_Anlage", ucm);
                        sqlCommand.Parameters.AddWithValue("@Aceton", aceton);
                        sqlCommand.Parameters.AddWithValue("@Waschanmerkungen", bemerkungWaschen);
                        sqlCommand.Parameters.AddWithValue("@refo_avonr", revoNummer);
                        sqlCommand.Parameters.AddWithValue("@Zeichnungspfad", pfadZeichnungAuflegen);
                        sqlCommand.Parameters.AddWithValue("@InfoZeichnung", pfadZusatzinfo);
                        sqlCommand.Parameters.AddWithValue("@InfoZeichnung_Bemerkungen", textZusatzinfo);

                        sqlCommand.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Daten erfolgreich gespeichert!");

                DialogResult result = MessageBox.Show("Noch eine weitere Seite eingeben?", "Frage", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    ClearFildsForSecoundThird();
                }
                else
                {
                    ClearAllFields();
                    this.Close();
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

        // Funktion um die felder zu leeren
        private void ClearAllFields()
        {
            // Setze alle TextBoxen auf String.Empty
            TxtboxArtikelnummer.Text = string.Empty;
            TxtboxBezeichnung.Text = string.Empty;
            TxtboxDurchmesser.Text = string.Empty;
            TxtboxDmWaschen.Text = string.Empty;
            TxtboxFreibereich.Text = string.Empty;
            TxtboxDicke.Text = string.Empty;
            TxtboxBrechwert.Text = string.Empty;
            TxtboxZeitProzess.Text = string.Empty;
            TxtboxRadiusVerguetung.Text = string.Empty;
            TxtboxRadiusRueckseite.Text = string.Empty;
            TxtboxStkSegment.Text = string.Empty;
            TxtboxStkCharge.Text = string.Empty;
            txtBoxRevoNr.Text = string.Empty;
            // Setze alle ComboBoxen auf ihre Standardwerte zurück
            ComboboxBelagVerguetung.SelectedIndex = -1;
            ComboboxBelagRueckseite.SelectedIndex = -1;
            ComboboxProzess.SelectedIndex = -1;
            ComboboxStatus.SelectedIndex = -1;
            ComboBoxZukauf.SelectedIndex = -1;
            ComboBoxFlaeche.SelectedIndex = -1;
            ComboboxGlassorte.SelectedIndex = -1;
            ComboboxGNummer.SelectedIndex = -1;
            ComboboxSeite.SelectedIndex = -1;
            ComboboxRing.SelectedIndex = -1;
            ComboboxVorreinigen.SelectedIndex = -1;
            ComboboxUCM497.SelectedIndex = -1;
            ComboboxAceton.SelectedIndex = -1;
            // Setze alle RichTextBoxen auf String.Empty
            RichtxtboxBemerkung.Text = string.Empty;
            RichtxtboxBemerkungWaschen.Text = string.Empty;
            RichtxtboxZusatzinfo.Text = string.Empty;
            // Setze alle Labels + Picturrboxen auf Ausgangszustand
            PictureboxAuflegenLinsenPrismen.Image = null;
            PictureboxZusatzinfo.Image = null;
            // Setze eine Hintergrundfarbe für die PictureBoxen, damit sie sichtbar bleiben
            PictureboxAuflegenLinsenPrismen.BackColor = Color.White; // Oder eine andere Farbe, die sich vom Formularhintergrund abhebt
            PictureboxZusatzinfo.BackColor = Color.White;
            LblPfadAuflegenLinsenPrismen.Text = "Doppelklick auf Bild um Pfad zu öffnen";
            LblPfadZusatzinfo.Text = "Doppelklick auf Bild um Pfad zu öffnen";
            // Setze den DateTimePicker auf das heutige Datum
            DateTimePickerAufgenommenLinsePrisma.Value = DateTime.Now;
        }

        // Funktion um die felder zu leeren wenn eine weiter seite eingegeben werden soll
        private void ClearFildsForSecoundThird()
        {
            // Setze alle TextBoxen auf String.Empty
            TxtboxFreibereich.Text = string.Empty;
            TxtboxZeitProzess.Text = string.Empty;
            TxtboxRadiusVerguetung.Text = string.Empty;
            TxtboxRadiusRueckseite.Text = string.Empty;
            TxtboxRadiusVerguetung.Text = string.Empty;
            TxtboxStkSegment.Text = string.Empty;
            TxtboxStkCharge.Text = string.Empty;
            txtBoxRevoNr.Text = string.Empty;
            // Setze alle ComboBoxen auf ihre Standardwerte zurück
            ComboBoxFlaeche.SelectedIndex = -1;
            ComboboxSeite.SelectedIndex = -1;
            ComboboxVorreinigen.SelectedIndex = -1;
            ComboboxUCM497.SelectedIndex = -1;
            ComboboxAceton.SelectedIndex = -1;
            ComboboxBelagVerguetung.SelectedIndex = -1;
            ComboboxBelagRueckseite.SelectedIndex = -1;
            ComboboxProzess.SelectedIndex = -1;
            ComboboxSeite.SelectedIndex = -1;
            ComboboxRing.SelectedIndex = -1;
            // Setze alle RichTextBoxen auf String.Empty
            RichtxtboxBemerkung.Text = string.Empty;
            RichtxtboxBemerkungWaschen.Text = string.Empty;
            RichtxtboxZusatzinfo.Text = string.Empty;
            // Setze alle Labels + Picturrboxen auf Ausgangszustand
            PictureboxAuflegenLinsenPrismen.Image = null;
            PictureboxZusatzinfo.Image = null;
            // Setze eine Hintergrundfarbe für die PictureBoxen, damit sie sichtbar bleiben
            PictureboxAuflegenLinsenPrismen.BackColor = Color.White; // Oder eine andere Farbe, die sich vom Formularhintergrund abhebt
            PictureboxZusatzinfo.BackColor = Color.White;
            LblPfadAuflegenLinsenPrismen.Text = "Doppelklick auf Bild um Pfad zu öffnen";
            LblPfadZusatzinfo.Text = "Doppelklick auf Bild um Pfad zu öffnen";
            // Setze den DateTimePicker auf das heutige Datum
            DateTimePickerAufgenommenLinsePrisma.Value = DateTime.Now;
            // Info für Bildeingabe wieder auf visible setzten
            lblWichtig1.Visible = true;
            lblWichtig2.Visible = true;
        }

        // Funktion um die felder zu leeren wenn eine weiter seite eingegeben werden soll
        private void SetPlaceholders()
        {
            SetPlaceholder(TxtboxDurchmesser, "xx,xx");
            SetPlaceholder(TxtboxDmWaschen, "00xx");
            SetPlaceholder(TxtboxFreibereich, "xx,xx");
            SetPlaceholder(TxtboxDicke, "x,xx bzw xx,xx");
            SetPlaceholder(TxtboxZeitProzess, "x,xx");
            SetPlaceholder(TxtboxBrechwert, "x,xx");
        }

        // Funktion um Platzhalter in TextBoxen zu setzen
        private void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholderText;

            textBox.GotFocus += (sender, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = SystemColors.WindowText;
                }
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        // ############## Event-Handler  ################
        // Eventhandler für die Änderung der Auswahl in der ComboboxBelagVerguetung
        private void ComboboxBelagVerguetung_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxBelagVerguetung.SelectedItem != null)
            {
                // Hole die ausgewählte Belagvergütung
                string selectedBelagVerguetung = ComboboxBelagVerguetung.SelectedItem.ToString();

                // Rufe die Methode auf, um die ComboboxProzess zu befüllen
                FillComboboxProzess(selectedBelagVerguetung);
            }
        }

        // Eventhandler für die Änderung der Auswahl in der ComboboxGNummer
        private void ComboboxGNummer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfe, ob eine Auswahl in der Combobox getroffen wurde
            if (ComboboxGNummer.SelectedItem != null)
            {
                // Wenn eine GNummer ausgewählt wurde, rufe die ausgewählte GNummer ab
                string selectedGNummer = ComboboxGNummer.SelectedItem.ToString();

                // Befülle die ComboboxGlassorte mit den Glassorten, die zu dieser GNummer gehören
                FillComboboxGlassorte(selectedGNummer);
            }
            else
            {
                // Wenn keine GNummer ausgewählt wurde, lade alle Glassorten
                FillComboboxGlassorte(null);
            }
        }

        // Eventhandler wenn auf die PB doppelt geklickt wird
        private void PictureboxAuflegenLinsenPrismen_DoubleClick(object sender, EventArgs e)
        {
            // Erstelle und konfiguriere den OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // Zeige den Dialog an und überprüfe, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Hole den ausgewählten Dateipfad
                    string selectedFilePath = openFileDialog.FileName;

                    // Lade das Bild in die PictureBox
                    PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(selectedFilePath);

                    // Aktualisiere das Label mit dem vollständigen Pfad
                    LblPfadAuflegenLinsenPrismen.Text = selectedFilePath;
                    lblWichtig1.Visible = false;
                }
            }
        }

        // Eventhandler wenn auf die PB doppelt geklickt wird
        private void PictureboxZusatzinfo_DoubleClick(object sender, EventArgs e)
        {
            // Erstelle und konfiguriere den OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung\InfoZeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // Zeige den Dialog an und überprüfe, ob der Benutzer eine Datei ausgewählt hat
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Hole den ausgewählten Dateipfad
                    string selectedFilePath = openFileDialog.FileName;

                    // Lade das Bild in die PictureBox
                    PictureboxZusatzinfo.Image = Image.FromFile(selectedFilePath);

                    // Aktualisiere das Label mit dem vollständigen Pfad
                    LblPfadZusatzinfo.Text = selectedFilePath;
                    lblWichtig2.Visible = false;
                }
            }
        }

        // Eventhandler wenn auf den Button geklickt wird
        private void BtnArtikelSpeichern_Click(object sender, EventArgs e)
        {
            // Werte aus den Steuerelementen holen
            string artikelnummer = TxtboxArtikelnummer.Text;
            string bezeichnung = TxtboxBezeichnung.Text;
            string status = ComboboxStatus.Text;
            string[] gruppennameArray = artikelnummer.Split('-').Select(s => s.Trim()).ToArray();
            string gruppenname = gruppennameArray[0];
            string zukauf = ComboBoxZukauf.Text;
            string flaeche = ComboBoxFlaeche.Text;
            string gNummer = ComboboxGNummer.Text;
            string glassorte = ComboboxGlassorte.Text;

            // Konvertiere Strings zu Decimals und runde auf zwei Nachkommastellen
            if (!decimal.TryParse(TxtboxDurchmesser.Text, out decimal durchmesser))
            {
                MessageBox.Show("Ungültiger Wert für Durchmesser. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            durchmesser = Math.Round(durchmesser, 2);

            if (!decimal.TryParse(TxtboxFreibereich.Text, out decimal freibereich))
            {
                MessageBox.Show("Ungültiger Wert für Freibereich. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            freibereich = Math.Round(freibereich, 2);

            if (!decimal.TryParse(TxtboxDicke.Text, out decimal dicke))
            {
                MessageBox.Show("Ungültiger Wert für Dicke. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            dicke = Math.Round(dicke, 2);

            if (!decimal.TryParse(TxtboxBrechwert.Text, out decimal brechwert))
            {
                MessageBox.Show("Ungültiger Wert für Brechwert. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            brechwert = Math.Round(brechwert, 2);

            if (!decimal.TryParse(TxtboxZeitProzess.Text, out decimal zeitProzess))
            {
                MessageBox.Show("Ungültiger Wert für Zeit Prozess. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            zeitProzess = Math.Round(zeitProzess, 2);

            string durchmesserWaschen = TxtboxDmWaschen.Text;
            string seite = ComboboxSeite.Text;
            string radiusVerguetung = TxtboxRadiusVerguetung.Text;
            string radiusRueckseite = TxtboxRadiusRueckseite.Text;
            string belagVerguetung = ComboboxBelagVerguetung.Text;
            string belagRueckseite = ComboboxBelagRueckseite.Text;
            string prozesseingabe = ComboboxProzess.Text;
            string[] prozessArray = prozesseingabe.Split(' ').Select(s => s.Trim()).ToArray();
            string prozess = prozessArray[0];
            string ringeingabe = ComboboxRing.Text;
            string[] ringnameArray = ringeingabe.Split('-').Select(x => x.Trim()).ToArray();
            string ring = ringnameArray[0];
            string stkSegment = TxtboxStkSegment.Text;
            string stkGesamt = TxtboxStkCharge.Text;

            // Datum vom DateTimePicker
            if (!DateTime.TryParse(DateTimePickerAufgenommenLinsePrisma.Text, out DateTime eingabedatum))
            {
                MessageBox.Show("Ungültiges Datum. Bitte überprüfen Sie das Format.");
                return; // Beende die Methode, wenn das Datum ungültig ist
            }
            string bemerkungArtikel = RichtxtboxBemerkung.Text;
            string vorreinigen = ComboboxVorreinigen.Text;
            string ucm = ComboboxUCM497.Text;
            string aceton = ComboboxAceton.Text;
            string bemerkungWaschen = RichtxtboxBemerkungWaschen.Text;
            string revoNummer = txtBoxRevoNr.Text;
            string pfadZeichnungAuflegen = LblPfadAuflegenLinsenPrismen.Text;
            string pfadZusatzinfo = LblPfadZusatzinfo.Text;
            string textZusatzinfo = RichtxtboxZusatzinfo.Text;

            // Funktion zum Speichern der Daten aufrufen
            SpeichereDatenInDatenbank(artikelnummer, bezeichnung, status, gruppenname, zukauf, flaeche, gNummer, glassorte, durchmesser
                , durchmesserWaschen, freibereich, dicke, seite, brechwert, radiusVerguetung, radiusRueckseite, belagVerguetung, prozess
                , belagRueckseite, ring, stkSegment, stkGesamt, zeitProzess, eingabedatum, bemerkungArtikel, vorreinigen, ucm, aceton
                , bemerkungWaschen, revoNummer, pfadZeichnungAuflegen, pfadZusatzinfo, textZusatzinfo);
        }

        // Event-Handler wenn auf den Button geklickt wird
        private void BtnArtikelAendern_Click(object sender, EventArgs e)
        {
            Form_ArtikelPrototypAendern form_ArtikelPrototypAendern = new Form_ArtikelPrototypAendern("12-2044", "1");
            form_ArtikelPrototypAendern.Show(); // Zeigt das neue Formular an
            form_ArtikelPrototypAendern.BringToFront(); // Bringt das neue Formular in den Vordergrund
        }
    }
}
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    public partial class Form_ArtikelPrototypAendern : Form
    {
        // Verbindungszeichenfolgen für die SQL Server-Datenbanken
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        public Form_ArtikelPrototypAendern(string artikelNr, string seite)
        {
            InitializeComponent();
            FillComboBoxArtikel();
            ComboboxAuswahlArtikel.SelectedIndexChanged += ComboboxAuswahlArtikel_SelectedIndexChanged;

            // Setze die Artikelnummer und die Seite in der ComboBox wenn in der Auftragsverwaltung ein Artikel ausgewählt wurde
            string artikelSeite = $"{artikelNr} - {seite}";

            // Überprüfen, ob das Element bereits in der ComboBox vorhanden ist
            int index = ComboboxAuswahlArtikel.Items.IndexOf(artikelSeite);

            if (index != -1)
            {
                // Wenn das Element vorhanden ist, wähle es aus
                ComboboxAuswahlArtikel.SelectedIndex = index;
            }
        }

        // Methode zum Füllen der ComboBox mit Artikelnummern und Seiteninformationen
        private void FillComboBoxArtikel()
        {
            // Öffnen der Datenbankverbindung
            sqlConnectionVerwaltung.Open();

            // SQL-Abfrage, um die eindeutigen Artikelnummern (ARTNR) und Seiteninformationen (SEITE) aus der Tabelle 'Serienlinsen' abzurufen
            string query = @"
        SELECT DISTINCT ARTNR, SEITE
        FROM Serienlinsen
        ORDER BY ARTNR ASC"; // Sortiert die Ergebnisse aufsteigend nach ARTNR

            try
            {
                // Erstellen des SqlCommand-Objekts mit der Abfrage und der geöffneten Verbindung
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Ausführen der Abfrage und Abrufen der Ergebnisse mit einem SqlDataReader
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Leeren der ComboBox, um sicherzustellen, dass keine doppelten Einträge vorhanden sind
                        ComboboxAuswahlArtikel.Items.Clear();

                        // Durchlaufen der Ergebnisse aus der Abfrage
                        while (reader.Read())
                        {
                            // Abrufen der Werte für Artikelnummer und Seite aus dem aktuellen Datensatz
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();

                            // Zusammenfügen der Artikelnummer und der Seite zu einem Anzeigeformat, z.B. "12345 - links"
                            string displayValue = $"{artikelNummerValue} - {seiteValue}";

                            // Hinzufügen des Anzeigeformats zur ComboBox
                            ComboboxAuswahlArtikel.Items.Add(displayValue);
                        }
                    }
                }

                // Sicherstellen, dass die ComboBox nicht automatisch sortiert wird, da dies durch die SQL-Abfrage gesteuert wird
                ComboboxAuswahlArtikel.Sorted = false;
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung: Zeigt eine Meldung an, falls beim Laden der Artikel ein Fehler auftritt
                MessageBox.Show("Fehler beim Laden der Artikel: " + ex.Message);
            }
            finally
            {
                // Überprüfen, ob die Datenbankverbindung noch geöffnet ist, und sie bei Bedarf schließen
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Event-Handler für das Auswählen eines Artikels in der ComboBox
        private void ComboboxAuswahlArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxAuswahlArtikel.SelectedItem != null)
            {
                string selectedValue = ComboboxAuswahlArtikel.SelectedItem.ToString();
                string[] values = selectedValue.Split('-').Select(s => s.Trim()).ToArray();

                if (values.Length >= 3) // Überprüfen, ob wir mindestens 3 Teile haben
                {
                    // Die letzten zwei Teile sind die Seite und der Rest die Artikelnummer
                    string artikelnummer = string.Join("-", values.Take(values.Length - 1)); // Alle bis auf das letzte Element als Artikelnummer
                    string seite = values.Last(); // Letztes Element als Seite

                    // Lade die Artikel-Details
                    LadeArtikelDetails(artikelnummer, seite);
                }
            }
        }

        // Lade die Artikel-Details aus der Datenbank
        private void LadeArtikelDetails(string artikelnummer, string seite)
        {
            string query = @"
                SELECT BEZ, Status, GruppenName, ARTNR, Zukauf, [Innen-Aussen], G_Nummer, GLASSORTE, DM, Waschen_DM, FREI, DICKE, ND,
                    Radius1, Radius2, Belag1, VERGBELAG, MATERIAL, Belag2, RING, STK_SEGM, STK_CHARGE, CHARGENZEIT, SEITE,
                    Anmerkungsdatum, BEMERKUNG, Vorreinigung, HFE_Anlage, Aceton, Waschanmerkungen, refo_avonr, Zeichnungspfad,
                    InfoZeichnung, InfoZeichnung_Bemerkungen
                FROM Serienlinsen
                WHERE ARTNR = @ARTNR AND SEITE = @SEITE";

            // Verwende einen using-Block für die Verbindung, um sicherzustellen, dass die Ressourcen korrekt freigegeben werden
            using (SqlConnection sqlConnection = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False"))
            {
                try
                {
                    // Öffne die Verbindung
                    sqlConnection.Open();

                    // Verwende einen using-Block für das SqlCommand-Objekt
                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        // Füge Parameter mit spezifischen Typen hinzu, um die Abfrage effizienter zu gestalten
                        sqlCommand.Parameters.Add(new SqlParameter("@ARTNR", SqlDbType.NVarChar) { Value = artikelnummer });
                        sqlCommand.Parameters.Add(new SqlParameter("@SEITE", SqlDbType.NVarChar) { Value = seite });

                        // Verwende einen SqlDataReader in einem using-Block, um die Daten sicher zu lesen
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Fülle die Textfelder mit den geladenen Daten oder setze sie auf Standardwerte
                                TxtboxArtikelnummer.Text = reader["ARTNR"].ToString();
                                TxtboxBezeichnung.Text = reader["BEZ"].ToString();
                                ComboboxStatus.Text = reader["Status"].ToString();
                                ComboBoxZukauf.Text = reader["Zukauf"].ToString();
                                ComboBoxFlaeche.Text = reader["Innen-Aussen"].ToString();
                                TxtboxGNummer.Text = reader["G_Nummer"].ToString();
                                TxtboxGlassorte.Text = reader["GLASSORTE"].ToString();
                                TxtboxDurchmesser.Text = reader["DM"].ToString();
                                TxtboxDmWaschen.Text = reader["Waschen_DM"].ToString();
                                TxtboxFreibereich.Text = reader["FREI"].ToString();
                                TxtboxDicke.Text = reader["DICKE"].ToString();
                                TxtboxSeite.Text = reader["SEITE"].ToString();
                                TxtboxBrechwert.Text = reader["ND"].ToString();
                                TxtboxBelagProzess.Text = reader["Material"].ToString();
                                TxtboxRadiusVerguetung.Text = reader["Radius1"].ToString();
                                TxtboxRadiusRueckseite.Text = reader["Radius2"].ToString();
                                TxtboxBelagVerguetung.Text = reader["Belag1"].ToString();
                                TxtboxBelagRueckseite.Text = reader["Belag2"].ToString();
                                TxtboxStkSegment.Text = reader["STK_SEGM"].ToString();
                                TxtboxStkCharge.Text = reader["STK_CHARGE"].ToString();
                                TxtboxZeitProzess.Text = reader["CHARGENZEIT"].ToString();
                                // Überprüfen auf Nullwerte und setze auf einen leeren String, falls notwendig
                                TxtboxRing.Text = reader["RING"] as string ?? string.Empty;
                                DateTimePickerGeaendertLinsePrisma.Value = reader["Anmerkungsdatum"] as DateTime? ?? DateTime.Now;
                                RichtxtboxBemerkung.Text = reader["BEMERKUNG"] as string ?? string.Empty;
                                ComboboxVorreinigen.Text = reader["Vorreinigung"] as string ?? string.Empty;
                                ComboboxUCM497.Text = reader["HFE_Anlage"] as string ?? string.Empty;
                                ComboboxAceton.Text = reader["Aceton"] as string ?? string.Empty;
                                RichtxtboxBemerkungWaschen.Text = reader["Waschanmerkungen"] as string ?? string.Empty;
                                txtBoxRevoNr.Text = reader["refo_avonr"] as string ?? string.Empty;
                                LblPfadAuflegenLinsenPrismen.Text = reader["Zeichnungspfad"] as string ?? string.Empty;
                                LblPfadZusatzinfo.Text = reader["InfoZeichnung"] as string ?? string.Empty;
                                RichtxtboxZusatzinfo.Text = reader["InfoZeichnung_Bemerkungen"] as string ?? string.Empty;
                                // Bilder in PictureBoxen laden
                                LoadImages();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung mit einer detaillierten Fehlermeldung
                    MessageBox.Show("Fehler beim Abrufen der Artikel-Details: " + ex.Message);
                }
            }
        }

        // Methode zum Laden der Bilder in die PictureBoxen
        private void LoadImages()
        {
            try
            {
                // Bild für die erste PictureBox laden
                string pathAuflegen = LblPfadAuflegenLinsenPrismen.Text;
                if (System.IO.File.Exists(pathAuflegen))
                {
                    PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(pathAuflegen);
                }
                else
                {
                    MessageBox.Show("Das Bild für Auflegen Linsen Prismen existiert nicht: " + pathAuflegen);
                    PictureboxAuflegenLinsenPrismen.Image = null; // Setze das Bild auf null, wenn die Datei nicht existiert
                }

                // Bild für die zweite PictureBox laden
                string pathZusatzinfo = LblPfadZusatzinfo.Text;
                if (System.IO.File.Exists(pathZusatzinfo))
                {
                    PictureboxZusatzinfo.Image = Image.FromFile(pathZusatzinfo);
                }
                else
                {
                    MessageBox.Show("Das Bild für Zusatzinfo existiert nicht: " + pathZusatzinfo);
                    PictureboxZusatzinfo.Image = null; // Setze das Bild auf null, wenn die Datei nicht existiert
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Bilder: " + ex.Message);
            }
        }

        private void PictureboxAuflegenLinsenPrismen_DoubleClick_1(object sender, EventArgs e)
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
                }
            }
        }

        private void PictureboxZusatzinfo_DoubleClick_1(object sender, EventArgs e)
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
                }
            }
        }

        private void AktualisiereDatenInDatenbank(string artikelnummer, string bezeichnung, string status, string gruppenname, string zukauf, string flaeche
    , string gNummer, string glassorte, decimal durchmesser, string durchmesserWaschen, decimal freibereich, decimal dicke, string seite, decimal brechwert
    , string radiusVerguetung, string radiusRueckseite, string belagVerguetung, string prozess, string belagRueckseite, string ring, string stkSegment
    , string stkGesamt, decimal zeitProzess, DateTime eaenderungsDatum, string bemerkungArtikel, string vorreinigen, string ucm, string aceton, string bemerkungWaschen
    , string revoNummer, string pfadZeichnungAuflegen, string pfadZusatzinfo, string textZusatzinfo)
        {
            try
            {
                using (SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False"))
                {
                    sqlConnectionVerwaltung.Open();

                    string query = @"
            UPDATE Serienlinsen
            SET
                [BEZ] = @BEZ,
                [Status] = @Status,
                [GruppenName] = @GruppenName,
                [Zukauf] = @Zukauf,
                [Innen-Aussen] = @InnenAussen,
                [G_Nummer] = @G_Nummer,
                [GLASSORTE] = @GLASSORTE,
                [DM] = @DM,
                [Waschen_DM] = @Waschen_DM,
                [FREI] = @FREI,
                [DICKE] = @DICKE,
                [SEITE] = @SEITE,
                [ND] = @ND,
                [Radius1] = @Radius1,
                [Radius2] = @Radius2,
                [Belag1] = @Belag1,
                [VERGBELAG] = @VERGBELAG,
                [MATERIAL] = @MATERIAL,
                [Belag2] = @Belag2,
                [RING] = @RING,
                [STK_SEGM] = @STK_SEGM,
                [STK_CHARGE] = @STK_CHARGE,
                [CHARGENZEIT] = @CHARGENZEIT,
                [Änderungsdatum] = @Änderungsdatum,
                [BEMERKUNG] = @BEMERKUNG,
                [Vorreinigung] = @Vorreinigung,
                [HFE_Anlage] = @HFE_Anlage,
                [Aceton] = @Aceton,
                [Waschanmerkungen] = @Waschanmerkungen,
                [refo_avonr] = @refo_avonr,
                [Zeichnungspfad] = @Zeichnungspfad,
                [InfoZeichnung] = @InfoZeichnung,
                [InfoZeichnung_Bemerkungen] = @InfoZeichnung_Bemerkungen
            WHERE [ARTNR] = @ARTNR AND [SEITE] = @SEITE"; // Update-Kriterium mit der Seite ergänzt

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
                        sqlCommand.Parameters.AddWithValue("@SEITE", seite); // Seite als Parameter hinzugefügt
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
                        sqlCommand.Parameters.AddWithValue("@Änderungsdatum", eaenderungsDatum);
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

                MessageBox.Show("Daten erfolgreich aktualisiert!");
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

        private void BtnArtikelUpdaten_Click(object sender, EventArgs e)
        {
            // Werte aus den Steuerelementen holen
            string artikelnummer = TxtboxArtikelnummer.Text;
            string bezeichnung = TxtboxBezeichnung.Text;
            string status = ComboboxStatus.Text;
            string[] gruppennameArray = artikelnummer.Split('-').Select(s => s.Trim()).ToArray();
            string gruppenname = gruppennameArray[0];
            string zukauf = ComboBoxZukauf.Text;
            string flaeche = ComboBoxFlaeche.Text;
            string gNummer = TxtboxGNummer.Text;
            string glassorte = TxtboxGlassorte.Text;

            // Konvertiere Strings zu Decimals und runde auf zwei Nachkommastellen
            if (!decimal.TryParse(TxtboxDurchmesser.Text, out decimal durchmesser))
            {
                MessageBox.Show("Ungültiger Wert für Durchmesser. Bitte überprüfen Sie das Format oder eine 0 eingebenoder.");
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
            string seite = TxtboxSeite.Text;
            string radiusVerguetung = TxtboxRadiusVerguetung.Text;
            string radiusRueckseite = TxtboxRadiusRueckseite.Text;
            string belagVerguetung = TxtboxBelagVerguetung.Text;
            string belagRueckseite = TxtboxBelagRueckseite.Text;
            //string[] prozessArray = belagVerguetung.Split(' ').Select(s => s.Trim()).ToArray();
            //string prozess = prozessArray[0];
            string prozess = TxtboxBelagProzess.Text;
            string ringeingabe = TxtboxRing.Text;
            string[] ringnameArray = ringeingabe.Split('-').Select(x => x.Trim()).ToArray();
            string ring = ringnameArray[0];
            string stkSegment = TxtboxStkSegment.Text;
            string stkGesamt = TxtboxStkCharge.Text;

            // Datum vom DateTimePicker
            DateTime eanderungsDatum;
            if (!DateTime.TryParse(DateTimePickerGeändert.Text, out eanderungsDatum))
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
            AktualisiereDatenInDatenbank(artikelnummer, bezeichnung, status, gruppenname, zukauf, flaeche, gNummer, glassorte, durchmesser
                , durchmesserWaschen, freibereich, dicke, seite, brechwert, radiusVerguetung, radiusRueckseite, belagVerguetung, prozess
                , belagRueckseite, ring, stkSegment, stkGesamt, zeitProzess, eanderungsDatum, bemerkungArtikel, vorreinigen, ucm, aceton
                , bemerkungWaschen, revoNummer, pfadZeichnungAuflegen, pfadZusatzinfo, textZusatzinfo);
        }
    }
}
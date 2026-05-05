// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben können.
// ===================================================================================================
using System;                                  // Basis-Typen (string, int, DateTime, EventArgs, Exception ...)
using System.Data;                             // DataTable, ConnectionState, SqlDbType, ...
using System.Data.SqlClient;                   // SQL-Server-Zugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Drawing;                          // Image, Color, Font (für Bilder/Grafik)
using System.Linq;                             // LINQ-Methoden (Select, Take, Last, ToArray ...)
using System.Windows.Forms;                    // Windows-Forms (Form, ComboBox, MessageBox, OpenFileDialog ...)

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    // public partial class ... : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien verteilt sein (z. B. ...Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_ArtikelPrototypAendern : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Felder ("Variablen der Klasse"). private = nur innerhalb dieser Klasse sichtbar.
        // readonly = darf nur einmal (hier oder im Konstruktor) gesetzt werden.
        // -----------------------------------------------------------------------------------------------------------------

        // Verbindung zur Verwaltungs-Datenbank.
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // Zweite Verbindung zur Shuttle-Datenbank (wird angelegt, falls später benötigt).
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_ArtikelPrototypAendern(artikelNr, seite)" automatisch.
        // Bekommt eine Artikelnummer + Seite, um die ComboBox direkt vorzubelegen.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_ArtikelPrototypAendern(string artikelNr, string seite)
        {
            InitializeComponent();          // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)
            FillComboBoxArtikel();          // ComboBox mit allen Artikel-Seite-Kombinationen befüllen

            // Event-Handler ans ComboBox-Auswahlereignis hängen.
            ComboboxAuswahlArtikel.SelectedIndexChanged += ComboboxAuswahlArtikel_SelectedIndexChanged;

            // Anzeigeformat der ComboBox-Einträge ist "Artikel - Seite" → den passenden Eintrag zusammenbauen.
            string artikelSeite = $"{artikelNr} - {seite}";

            // Index dieses Eintrags in der Liste suchen (-1 = nicht gefunden).
            int index = ComboboxAuswahlArtikel.Items.IndexOf(artikelSeite);

            if (index != -1)
            {
                // Wenn der Eintrag existiert, ihn auswählen → das löst SelectedIndexChanged automatisch aus.
                ComboboxAuswahlArtikel.SelectedIndex = index;
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Holt aus der Tabelle "Serienlinsen" alle Artikel + Seiten und füllt die ComboBox damit.
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboBoxArtikel()
        {
            sqlConnectionVerwaltung.Open();

            // SQL: DISTINCT = Duplikate weglassen, ORDER BY = aufsteigend nach Artikelnummer sortieren.
            string query = @"
        SELECT DISTINCT ARTNR, SEITE
        FROM Serienlinsen
        ORDER BY ARTNR ASC";

            try
            {
                // "using" sorgt dafür, dass das SqlCommand am Ende automatisch freigegeben wird.
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Reader liest die Ergebnisse Zeile für Zeile.
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // ComboBox vorher leeren → keine Doppeleinträge bei erneutem Laden.
                        ComboboxAuswahlArtikel.Items.Clear();

                        while (reader.Read())
                        {
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();

                            // Anzeigeformat: "12-2044 - 1"
                            string displayValue = $"{artikelNummerValue} - {seiteValue}";

                            ComboboxAuswahlArtikel.Items.Add(displayValue);
                        }
                    }
                }

                // Sorted = false → die ComboBox sortiert nicht selbst, wir vertrauen der SQL-Sortierung.
                ComboboxAuswahlArtikel.Sorted = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Artikel: " + ex.Message);
            }
            finally
            {
                // Verbindung in jedem Fall (auch im Fehlerfall) schließen.
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wird aufgerufen, sobald in der ComboBox ein Eintrag gewählt wird.
        // Zerlegt den Anzeige-String "Artikel-Teil1-Teil2 - Seite" in Artikelnummer + Seite und lädt die Details.
        // -----------------------------------------------------------------------------------------------------------------
        private void ComboboxAuswahlArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxAuswahlArtikel.SelectedItem != null)
            {
                string selectedValue = ComboboxAuswahlArtikel.SelectedItem.ToString();

                // Beispiel: "12-2044 - 1" → Split('-') liefert ["12", "2044 ", " 1"]
                // .Select(s => s.Trim()) entfernt Leerzeichen → ["12", "2044", "1"]
                string[] values = selectedValue.Split('-').Select(s => s.Trim()).ToArray();

                // Mindestens 3 Teile? (Artikel kann selbst Bindestriche enthalten, z. B. "12-2044", plus die Seite hinten.)
                if (values.Length >= 3)
                {
                    // Alle Teile bis auf den letzten = Artikelnummer (mit "-" zusammenfügen).
                    // Take(n)  → erste n Elemente einer Sequenz.
                    string artikelnummer = string.Join("-", values.Take(values.Length - 1));

                    // Last() = letztes Element = Seite.
                    string seite = values.Last();

                    LadeArtikelDetails(artikelnummer, seite);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Lädt für (Artikelnummer, Seite) die Artikeldetails aus der DB und füllt damit die UI-Felder.
        // -----------------------------------------------------------------------------------------------------------------
        private void LadeArtikelDetails(string artikelnummer, string seite)
        {
            string query = @"
                SELECT BEZ, Status, GruppenName, ARTNR, Zukauf, [Innen-Aussen], G_Nummer, GLASSORTE, DM, Waschen_DM, FREI, DICKE, ND,
                    Radius1, Radius2, Belag1, VERGBELAG, MATERIAL, Belag2, RING, STK_SEGM, STK_CHARGE, CHARGENZEIT, SEITE,
                    Anmerkungsdatum, BEMERKUNG, Vorreinigung, HFE_Anlage, Aceton, Waschanmerkungen, refo_avonr, Zeichnungspfad,
                    InfoZeichnung, InfoZeichnung_Bemerkungen
                FROM Serienlinsen
                WHERE ARTNR = @ARTNR AND SEITE = @SEITE";

            // Eigene Verbindung lokal aufmachen (nicht das Klassenfeld) – sauberer Lebenszyklus pro Aufruf.
            using (SqlConnection sqlConnection = new SqlConnection(
                @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False"))
            {
                try
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        // Parameter mit explizitem Typ → schützt vor SQL-Injection und ist performanter als AddWithValue.
                        sqlCommand.Parameters.Add(new SqlParameter("@ARTNR", SqlDbType.NVarChar) { Value = artikelnummer });
                        sqlCommand.Parameters.Add(new SqlParameter("@SEITE", SqlDbType.NVarChar) { Value = seite });

                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            // Wir erwarten höchstens eine Zeile → einfacher if statt while.
                            if (reader.Read())
                            {
                                // Felder mit den Werten aus der DB befüllen.
                                // .ToString() funktioniert auch für DBNull → liefert dann einen leeren String.
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

                                // "as string" = sicherer Cast – liefert null statt einer Exception, wenn der Cast nicht passt.
                                // "?? string.Empty" = Fallback: wenn null, dann leerer String.
                                TxtboxRing.Text = reader["RING"] as string ?? string.Empty;

                                // "as DateTime?" = nullable DateTime; "?? DateTime.Now" = wenn kein Datum, nimm aktuelles.
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

                                // Bilder anhand der gerade gesetzten Pfad-Labels nachladen.
                                LoadImages();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Abrufen der Artikel-Details: " + ex.Message);
                }
            } // Verbindung wird hier automatisch geschlossen
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Lädt die zwei Bilder aus den im Label gespeicherten Pfaden.
        // Wenn Datei nicht existiert: Hinweis + Bild = null.
        // -----------------------------------------------------------------------------------------------------------------
        private void LoadImages()
        {
            try
            {
                // Bild 1: Auflegeinformation
                string pathAuflegen = LblPfadAuflegenLinsenPrismen.Text;
                if (System.IO.File.Exists(pathAuflegen))
                {
                    PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(pathAuflegen);
                }
                else
                {
                    MessageBox.Show("Das Bild für Auflegen Linsen Prismen existiert nicht: " + pathAuflegen);
                    PictureboxAuflegenLinsenPrismen.Image = null;
                }

                // Bild 2: Zusatzinfo
                string pathZusatzinfo = LblPfadZusatzinfo.Text;
                if (System.IO.File.Exists(pathZusatzinfo))
                {
                    PictureboxZusatzinfo.Image = Image.FromFile(pathZusatzinfo);
                }
                else
                {
                    MessageBox.Show("Das Bild für Zusatzinfo existiert nicht: " + pathZusatzinfo);
                    PictureboxZusatzinfo.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Bilder: " + ex.Message);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Doppelklick auf "AuflegenLinsenPrismen"-PictureBox → Datei-Dialog öffnen, Bild laden, Pfad-Label aktualisieren.
        // -----------------------------------------------------------------------------------------------------------------
        private void PictureboxAuflegenLinsenPrismen_DoubleClick_1(object sender, EventArgs e)
        {
            // OpenFileDialog wird in "using" gepackt → wird sicher freigegeben.
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Startverzeichnis und Filteroptionen vorbelegen.
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // ShowDialog() blockiert, bis der Benutzer OK oder Abbrechen klickt.
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // Bild aus der gewählten Datei laden und im PictureBox anzeigen.
                    PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(selectedFilePath);

                    // Pfad im Label merken (wird später beim Update in die DB geschrieben).
                    LblPfadAuflegenLinsenPrismen.Text = selectedFilePath;
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Doppelklick auf "Zusatzinfo"-PictureBox → analog zu oben.
        // -----------------------------------------------------------------------------------------------------------------
        private void PictureboxZusatzinfo_DoubleClick_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung\InfoZeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    PictureboxZusatzinfo.Image = Image.FromFile(selectedFilePath);
                    LblPfadZusatzinfo.Text = selectedFilePath;
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Schreibt alle Felder als UPDATE in die Tabelle "Serienlinsen".
        // (Sehr viele Parameter – pro Spalte einer.)
        // -----------------------------------------------------------------------------------------------------------------
        private void AktualisiereDatenInDatenbank(string artikelnummer, string bezeichnung, string status, string gruppenname, string zukauf, string flaeche
            , string gNummer, string glassorte, decimal durchmesser, string durchmesserWaschen, decimal freibereich, decimal dicke, string seite, decimal brechwert
            , string radiusVerguetung, string radiusRueckseite, string belagVerguetung, string prozess, string belagRueckseite, string ring, string stkSegment
            , string stkGesamt, decimal zeitProzess, DateTime eaenderungsDatum, string bemerkungArtikel, string vorreinigen, string ucm, string aceton, string bemerkungWaschen
            , string revoNummer, string pfadZeichnungAuflegen, string pfadZusatzinfo, string textZusatzinfo)
        {
            try
            {
                // Eigene lokale Verbindung pro Aufruf.
                using (SqlConnection sqlConnectionVerwaltung = new SqlConnection(
                    @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False"))
                {
                    sqlConnectionVerwaltung.Open();

                    // UPDATE: setzt alle aufgelisteten Spalten gleichzeitig.
                    // [BEZ] in eckigen Klammern → Schutz für Spaltennamen mit Sonderzeichen oder reservierten Wörtern.
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
            WHERE [ARTNR] = @ARTNR AND [SEITE] = @SEITE";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        // Alle Parameter befüllen (AddWithValue ist bequem, aber Typ wird automatisch erraten).
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

                        // ExecuteNonQuery → für INSERT/UPDATE/DELETE; liefert die Anzahl betroffener Zeilen zurück.
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Daten erfolgreich aktualisiert!");
            }
            catch (SqlException sqlEx)
            {
                // SQL-spezifischer Fehler (Tabelle fehlt, Constraint-Verletzung etc.)
                MessageBox.Show("SQL-Fehler: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Alle anderen Fehler.
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "Artikel updaten":
        //  - Werte aus den UI-Feldern lesen
        //  - Zahlen sicher parsen (TryParse) und runden
        //  - Schließlich AktualisiereDatenInDatenbank(...) aufrufen.
        // -----------------------------------------------------------------------------------------------------------------
        private void BtnArtikelUpdaten_Click(object sender, EventArgs e)
        {
            // ----- String-Werte direkt aus den Steuerelementen holen -----
            string artikelnummer = TxtboxArtikelnummer.Text;
            string bezeichnung = TxtboxBezeichnung.Text;
            string status = ComboboxStatus.Text;

            // Den vorderen Teil der Artikelnummer (vor dem ersten "-") als Gruppennamen extrahieren.
            // Beispiel: "12-2044" → ["12", "2044"] → "12".
            string[] gruppennameArray = artikelnummer.Split('-').Select(s => s.Trim()).ToArray();
            string gruppenname = gruppennameArray[0];

            string zukauf = ComboBoxZukauf.Text;
            string flaeche = ComboBoxFlaeche.Text;
            string gNummer = TxtboxGNummer.Text;
            string glassorte = TxtboxGlassorte.Text;

            // ----- Zahlen aus Textboxen sicher in decimal umwandeln -----
            // TryParse: liefert true, wenn die Konvertierung klappt; bei false → Fehlermeldung + return.
            if (!decimal.TryParse(TxtboxDurchmesser.Text, out decimal durchmesser))
            {
                MessageBox.Show("Ungültiger Wert für Durchmesser. Bitte überprüfen Sie das Format oder eine 0 eingebenoder.");
                return;
            }
            durchmesser = Math.Round(durchmesser, 2); // auf 2 Nachkommastellen runden

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

            // ----- Restliche Werte aus den Eingabefeldern -----
            string durchmesserWaschen = TxtboxDmWaschen.Text;
            string seite = TxtboxSeite.Text;
            string radiusVerguetung = TxtboxRadiusVerguetung.Text;
            string radiusRueckseite = TxtboxRadiusRueckseite.Text;
            string belagVerguetung = TxtboxBelagVerguetung.Text;
            string belagRueckseite = TxtboxBelagRueckseite.Text;

            // (Auskommentiert: alternative Variante, den Prozess vom Belag-String abzuleiten.)
            //string[] prozessArray = belagVerguetung.Split(' ').Select(s => s.Trim()).ToArray();
            //string prozess = prozessArray[0];
            string prozess = TxtboxBelagProzess.Text;

            // Vom Ring-Eingabefeld nur den Teil vor dem ersten "-" verwenden.
            string ringeingabe = TxtboxRing.Text;
            string[] ringnameArray = ringeingabe.Split('-').Select(x => x.Trim()).ToArray();
            string ring = ringnameArray[0];

            string stkSegment = TxtboxStkSegment.Text;
            string stkGesamt = TxtboxStkCharge.Text;

            // ----- Datum aus DateTimePicker parsen -----
            DateTime eanderungsDatum;
            if (!DateTime.TryParse(DateTimePickerGeändert.Text, out eanderungsDatum))
            {
                MessageBox.Show("Ungültiges Datum. Bitte überprüfen Sie das Format.");
                return;
            }

            // ----- Restliche Felder -----
            string bemerkungArtikel = RichtxtboxBemerkung.Text;
            string vorreinigen = ComboboxVorreinigen.Text;
            string ucm = ComboboxUCM497.Text;
            string aceton = ComboboxAceton.Text;
            string bemerkungWaschen = RichtxtboxBemerkungWaschen.Text;
            string revoNummer = txtBoxRevoNr.Text;
            string pfadZeichnungAuflegen = LblPfadAuflegenLinsenPrismen.Text;
            string pfadZusatzinfo = LblPfadZusatzinfo.Text;
            string textZusatzinfo = RichtxtboxZusatzinfo.Text;

            // ----- Update tatsächlich ausführen -----
            AktualisiereDatenInDatenbank(
                artikelnummer, bezeichnung, status, gruppenname, zukauf, flaeche,
                gNummer, glassorte, durchmesser, durchmesserWaschen, freibereich,
                dicke, seite, brechwert, radiusVerguetung, radiusRueckseite,
                belagVerguetung, prozess, belagRueckseite, ring, stkSegment,
                stkGesamt, zeitProzess, eanderungsDatum, bemerkungArtikel,
                vorreinigen, ucm, aceton, bemerkungWaschen, revoNummer,
                pfadZeichnungAuflegen, pfadZusatzinfo, textZusatzinfo);
        }
    }
}
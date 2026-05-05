// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben können.
// ===================================================================================================
using System;                                  // Basis-Typen (string, int, DateTime, EventArgs, Exception ...)
using System.Collections.Generic;              // Sammlungen wie List<T>, Dictionary<TKey, TValue>, Tuple<,>
using System.Data;                             // ConnectionState, SqlDbType (ADO.NET-Kern)
using System.Data.SqlClient;                   // SQL-Server-Zugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Drawing;                          // Color, Image, SystemColors (Grafik/UI)
using System.Linq;                             // LINQ-Methoden (Select, OrderBy, ToArray, ToList ...)
using System.Windows.Forms;                    // Windows-Forms (Form, ComboBox, MessageBox, OpenFileDialog ...)

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    // public partial class ... : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien aufgeteilt sein (z. B. ...Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_EingabeSeriePrototyp : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Felder ("Variablen der Klasse").
        // private  = nur in dieser Klasse sichtbar.
        // readonly = darf nur einmal (hier oder im Konstruktor) gesetzt werden.
        // -----------------------------------------------------------------------------------------------------------------

        // Verbindung zur Verwaltungs-Datenbank.
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // Verbindung zur Shuttle-Datenbank (für Ring-/Vorrichtungs-Stammdaten).
        private readonly SqlConnection sqlConnectionShuttle = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True");

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_EingabeSeriePrototyp()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_EingabeSeriePrototyp()
        {
            InitializeComponent();                // erzeugt alle UI-Steuerelemente (Designer-Datei)

            FillComboBoxGNummer();                // GNummer-ComboBox mit Werten aus der DB befüllen

            // Event-Handler verbinden: jedes Mal, wenn sich die Auswahl ändert, läuft unsere Methode.
            ComboboxBelagVerguetung.SelectedIndexChanged += ComboboxBelagVerguetung_SelectedIndexChanged;
            ComboboxGNummer.SelectedIndexChanged += ComboboxGNummer_SelectedIndexChanged;

            FillComboboxGlassorte(null);          // Glassorten initial mit allen verfügbaren befüllen
            FillComboboxRing();                   // Ring-ComboBox aus der Shuttle-DB befüllen
            FillComboBoxBelagVerguetungRueckseite(); // Belag-Vergütung/Rückseite-ComboBoxen befüllen

            SetPlaceholders();                    // Platzhaltertexte für Eingabefelder setzen

            // Diese Hinweis-Labels sind anfangs deaktiviert (ausgegraut).
            lblWichtig1.Enabled = false;
            lblWichtig2.Enabled = false;
        }

        // ##################################################################################################
        // Selbst erstellte Funktionen
        // ##################################################################################################

        // -----------------------------------------------------------------------------------------------------------------
        // Befüllt zwei ComboBoxen (Belag-Vergütung und Belag-Rückseite) mit allen einzigartigen Belag-Werten
        // aus der Tabelle "Maximalwerte_Farbauswertung".
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboBoxBelagVerguetungRueckseite()
        {
            // SQL: DISTINCT entfernt Duplikate.
            string query = "SELECT DISTINCT Belag FROM Maximalwerte_Farbauswertung";

            try
            {
                sqlConnectionVerwaltung.Open();

                // "using" sorgt dafür, dass das SqlCommand am Ende sauber freigegeben wird.
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Vor dem Befüllen leeren → keine Doppeleinträge bei erneutem Aufruf.
                        ComboboxBelagVerguetung.Items.Clear();
                        ComboboxBelagRueckseite.Items.Clear();

                        // reader.Read() liefert true, solange noch eine Zeile übrig ist.
                        while (reader.Read())
                        {
                            string belagValue = reader["Belag"].ToString();

                            // In beide ComboBoxen denselben Wert einfügen.
                            ComboboxBelagVerguetung.Items.Add(belagValue);
                            ComboboxBelagRueckseite.Items.Add(belagValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Belagvergütungen: " + ex.Message);
            }
            finally
            {
                // Verbindung sicher schließen, falls noch offen.
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Befüllt die Prozess-ComboBox abhängig vom gewählten Belag.
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboboxProzess(string belagVerguetung)
        {
            // SQL mit Parameter @Belag → schützt vor SQL-Injection.
            string query = @"
        SELECT DISTINCT Prozess, Brechwert
        FROM Maximalwerte_Farbauswertung
        WHERE Belag = @Belag
        ORDER BY Prozess ASC";

            try
            {
                sqlConnectionVerwaltung.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    sqlCommand.Parameters.AddWithValue("@Belag", belagVerguetung);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        ComboboxProzess.Items.Clear();

                        while (reader.Read())
                        {
                            string prozessValue = reader["Prozess"].ToString();
                            string brechwertValue = reader["Brechwert"].ToString();

                            // Anzeigeformat: "Prozess Brechwert".
                            string displayValue = $"{prozessValue} {brechwertValue}";
                            ComboboxProzess.Items.Add(displayValue);
                        }
                    }
                }

                // Sorted = false → Reihenfolge bleibt so, wie wir sie eingefügt haben (per ORDER BY).
                ComboboxProzess.Sorted = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Prozesse: " + ex.Message);
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Befüllt die GNummer-ComboBox mit eindeutigen Werten aus der Glassdaten-Tabelle.
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboBoxGNummer()
        {
            string query = "SELECT DISTINCT GNummer FROM Glassdaten";

            try
            {
                sqlConnectionVerwaltung.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string gNummerValue = reader["GNummer"].ToString();
                            ComboboxGNummer.Items.Add(gNummerValue);
                        }
                    }
                }

                // ComboBox alphabetisch sortieren.
                ComboboxGNummer.Sorted = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der GNummern: " + ex.Message);
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Befüllt die Glassorten-ComboBox.
        // - Wenn "gNummer" leer/null ist: ALLE Glassorten laden.
        // - Sonst: nur die Glassorten, die zur gewählten GNummer gehören.
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboboxGlassorte(string gNummer)
        {
            string query;

            if (string.IsNullOrEmpty(gNummer))
            {
                query = "SELECT DISTINCT Glass FROM Glassdaten";
            }
            else
            {
                query = "SELECT DISTINCT Glass FROM Glassdaten WHERE GNummer = @GNummer";
            }

            try
            {
                sqlConnectionVerwaltung.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Parameter nur hinzufügen, wenn er auch im SQL vorkommt.
                    if (!string.IsNullOrEmpty(gNummer))
                    {
                        sqlCommand.Parameters.AddWithValue("@GNummer", gNummer);
                    }

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        ComboboxGlassorte.Items.Clear();

                        while (reader.Read())
                        {
                            string glassValue = reader["Glass"].ToString();
                            ComboboxGlassorte.Items.Add(glassValue);
                        }
                        // Verbindung hier (innen) schließen.
                        sqlConnectionVerwaltung.Close();
                    }
                }

                ComboboxGlassorte.Sorted = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Glassorten: " + ex.Message);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Befüllt die Ring-ComboBox aus der Shuttle-DB.
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboboxRing()
        {
            // Ringe, die mit "Z" beginnen, werden ausgeschlossen ("NOT LIKE 'Z%'").
            string query = @"
            SELECT DISTINCT Vorrichtungsnummer, Durchmesser_Innen
            FROM Ring_Stamm
            WHERE Vorrichtungsnummer NOT LIKE 'Z%'
            ORDER BY Vorrichtungsnummer ASC";

            try
            {
                sqlConnectionShuttle.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionShuttle))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        ComboboxRing.Items.Clear();

                        while (reader.Read())
                        {
                            string ringNummerValue = reader["Vorrichtungsnummer"].ToString();
                            string durchmesserValue = reader["Durchmesser_Innen"].ToString();

                            // Anzeigeformat z. B. "R450 - 28 mm"
                            string displayValue = $"{ringNummerValue} - {durchmesserValue} mm";
                            ComboboxRing.Items.Add(displayValue);
                        }
                    }
                }

                // Reihenfolge laut SQL beibehalten.
                ComboboxRing.Sorted = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Ringe: " + ex.Message);
            }
            finally
            {
                if (sqlConnectionShuttle.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionShuttle.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "Sortiere Ringe nach Durchmesser":
        // liest die ComboBox-Einträge, parst Name + Durchmesser, sortiert nach Durchmesser und füllt neu.
        // -----------------------------------------------------------------------------------------------------------------
        private void BtnOrderDmRing_Click(object sender, EventArgs e)
        {
            // Tuple<string, double> = Wertepaar (Ringname, Durchmesser).
            List<Tuple<string, double>> ringList = new List<Tuple<string, double>>();

            foreach (var item in ComboboxRing.Items)
            {
                string displayValue = item.ToString();
                string[] parts = displayValue.Split('-');

                // Erwartet: parts[0] = Ringname, parts[1] = "28 mm".
                if (parts.Length > 1 && double.TryParse(parts[1].Replace(" mm", "").Trim(), out double durchmesser))
                {
                    string ringNummer = parts[0].Trim();
                    ringList.Add(new Tuple<string, double>(ringNummer, durchmesser));
                }
            }

            // Nach Durchmesser (Item2) aufsteigend sortieren.
            ringList = ringList.OrderBy(r => r.Item2).ToList();

            // ComboBox leeren und neu befüllen.
            ComboboxRing.Items.Clear();
            foreach (var ring in ringList)
            {
                string displayValue = $"{ring.Item1} - {ring.Item2} mm";
                ComboboxRing.Items.Add(displayValue);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wenn ein Ring gewählt wird, die zugehörigen Stückzahlen aus der DB nachladen.
        // -----------------------------------------------------------------------------------------------------------------
        private void ComboboxRing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxRing.SelectedItem != null)
            {
                string selectedItem = ComboboxRing.SelectedItem.ToString();

                // Vor dem ersten "-" steht der Ringname → "R450" extrahieren.
                string ringName = selectedItem.Split('-')[0].Trim();

                GetStueckSegmentCharge(ringName);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Liefert für einen Ring (Vorrichtungsnummer) die Werte "Gerundet_SegmentDM" und "AnzahlRing"
        // und ermittelt darüber die Stückzahl pro Segment + Charge anhand eines fest hinterlegten Dictionaries.
        // -----------------------------------------------------------------------------------------------------------------
        private void GetStueckSegmentCharge(string ringName)
        {
            string query = @"
    SELECT Gerundet_SegmentDM, AnzahlRing
    FROM Ring_Stamm
    WHERE Vorrichtungsnummer = @RingName";

            try
            {
                sqlConnectionShuttle.Open();

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionShuttle))
                {
                    sqlCommand.Parameters.AddWithValue("@RingName", ringName);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Werte typsicher als int auslesen.
                            int gerundetSegment = reader.GetInt32(reader.GetOrdinal("Gerundet_SegmentDM"));
                            int anzahlRing = reader.GetInt32(reader.GetOrdinal("AnzahlRing"));

                            // Nachschlagewerk: SegmentDM (gerundet) → Stückzahl pro Segment.
                            // (Kommt aus betrieblichen Erfahrungswerten und ist hier hartkodiert.)
                            Dictionary<int, int> stueckProSegment = new Dictionary<int, int>
                            {
                                {0,0},   {2,2},   {5,5},   {7,7},
                                {13,458},{18,242},{20,210},{21,210},
                                {24,171},{28,120},{30,119},{32,96},
                                {37,84}, {39,66}, {47,49}, {53,36},
                                {59,28}, {62,28}, {67,28}, {73,23},
                                {85,12}, {86,17}, {93,15}, {105,16},
                                {126,7}, {999,0}
                            };

                            // TryGetValue: wenn der Schlüssel im Dictionary existiert, "stueckzahl" wird gesetzt.
                            if (stueckProSegment.TryGetValue(gerundetSegment, out int stueckzahl))
                            {
                                // Wenn AnzahlRing > 1: Stückzahl entsprechend multiplizieren.
                                if (anzahlRing != 0 && anzahlRing != 1)
                                {
                                    stueckzahl *= anzahlRing;
                                }

                                TxtboxStkSegment.Text = stueckzahl.ToString();

                                // Eine Charge besteht aus 3 Segmenten → mal 3.
                                int stueckzahlGesamt = stueckzahl * 3;
                                TxtboxStkCharge.Text = stueckzahlGesamt.ToString();
                            }
                            else
                            {
                                // Unbekannte Segmentgröße → 0 anzeigen.
                                TxtboxStkSegment.Text = "0";
                                TxtboxStkCharge.Text = "0";
                            }
                        }
                        else
                        {
                            // Ringname nicht in der Tabelle → 0 anzeigen.
                            TxtboxStkSegment.Text = "0";
                            TxtboxStkCharge.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Abrufen der Stückzahl: " + ex.Message);
            }
            finally
            {
                if (sqlConnectionShuttle.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionShuttle.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Schreibt einen NEUEN Datensatz in die Tabelle "Serienlinsen".
        // (Sehr viele Parameter – einer pro Spalte.)
        // -----------------------------------------------------------------------------------------------------------------
        private void SpeichereDatenInDatenbank(string artikelnummer, string bezeichnung, string status, string gruppenname, string zukauf, string flaeche
            , string gNummer, string glassorte, decimal durchmesser, string durchmesserWaschen, decimal freibereich, decimal dicke, string seite, decimal brechwert
            , string radiusVerguetung, string radiusRueckseite, string belagVerguetung, string prozess, string belagRueckseite, string ring, string stkSegment
            , string stkGesamt, decimal zeitProzess, DateTime eingabedatum, string bemerkungArtikel, string vorreinigen, string ucm, string aceton, string bemerkungWaschen
            , string revoNummer, string pfadZeichnungAuflegen, string pfadZusatzinfo, string textZusatzinfo)
        {
            try
            {
                // Eigene lokale Verbindung pro Aufruf → sauberer Lebenszyklus.
                using (SqlConnection sqlConnectionVerwaltung = new SqlConnection(
                    @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False"))
                {
                    sqlConnectionVerwaltung.Open();

                    // INSERT INTO ... VALUES (...) → fügt eine neue Zeile in die Tabelle ein.
                    string query = @"
                INSERT INTO Serienlinsen ([ARTNR], [BEZ], [Status], [GruppenName], [Zukauf], [Innen-Aussen], [G_Nummer], [GLASSORTE], [DM], [Waschen_DM], [FREI], [DICKE], [SEITE],
                    [ND], [Radius1], [Radius2], [Belag1], [VERGBELAG], [MATERIAL], [Belag2], [RING], [STK_SEGM], [STK_CHARGE], [CHARGENZEIT], [Anmerkungsdatum], [BEMERKUNG],
                    [Vorreinigung], [HFE_Anlage], [Aceton], [Waschanmerkungen], [refo_avonr], [Zeichnungspfad], [InfoZeichnung], [InfoZeichnung_Bemerkungen])
                VALUES (@ARTNR, @BEZ, @Status, @GruppenName, @Zukauf, @InnenAussen, @G_Nummer, @GLASSORTE, @DM, @Waschen_DM, @FREI, @DICKE, @SEITE,
                    @ND, @Radius1, @Radius2, @Belag1, @VERGBELAG, @MATERIAL, @Belag2, @RING, @STK_SEGM, @STK_CHARGE, @CHARGENZEIT, @Anmerkungsdatum, @BEMERKUNG,
                    @Vorreinigung, @HFE_Anlage, @Aceton, @Waschanmerkungen, @refo_avonr, @Zeichnungspfad, @InfoZeichnung, @InfoZeichnung_Bemerkungen)";

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        // Alle Parameter (Platzhalter @...) mit Werten füllen.
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

                        // ExecuteNonQuery → für INSERT/UPDATE/DELETE.
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Daten erfolgreich gespeichert!");

                // Nachfrage, ob direkt eine weitere Seite eingegeben werden soll.
                DialogResult result = MessageBox.Show(
                    "Noch eine weitere Seite eingeben?",
                    "Frage",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Nur die seitenspezifischen Felder leeren (Artikel-Daten bleiben erhalten).
                    ClearFildsForSecoundThird();
                }
                else
                {
                    // Komplettes Reset → Fenster schließen.
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

        // -----------------------------------------------------------------------------------------------------------------
        // Setzt ALLE Eingabefelder auf den Ausgangszustand (kompletter Reset).
        // -----------------------------------------------------------------------------------------------------------------
        private void ClearAllFields()
        {
            // TextBoxen zurücksetzen.
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

            // ComboBoxen: SelectedIndex = -1 → keine Auswahl.
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

            // RichTextBoxen leeren.
            RichtxtboxBemerkung.Text = string.Empty;
            RichtxtboxBemerkungWaschen.Text = string.Empty;
            RichtxtboxZusatzinfo.Text = string.Empty;

            // Bilder entfernen + Hintergrund weiß setzen.
            PictureboxAuflegenLinsenPrismen.Image = null;
            PictureboxZusatzinfo.Image = null;
            PictureboxAuflegenLinsenPrismen.BackColor = Color.White;
            PictureboxZusatzinfo.BackColor = Color.White;

            // Pfad-Labels auf Hinweistext zurücksetzen.
            LblPfadAuflegenLinsenPrismen.Text = "Doppelklick auf Bild um Pfad zu öffnen";
            LblPfadZusatzinfo.Text = "Doppelklick auf Bild um Pfad zu öffnen";

            // Datum auf "heute".
            DateTimePickerAufgenommenLinsePrisma.Value = DateTime.Now;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Setzt nur die seitenspezifischen Felder zurück (für 2./3. Seite desselben Artikels).
        // -----------------------------------------------------------------------------------------------------------------
        private void ClearFildsForSecoundThird()
        {
            // Nur seitenabhängige Felder leeren.
            TxtboxFreibereich.Text = string.Empty;
            TxtboxZeitProzess.Text = string.Empty;
            TxtboxRadiusVerguetung.Text = string.Empty;
            TxtboxRadiusRueckseite.Text = string.Empty;
            TxtboxRadiusVerguetung.Text = string.Empty;  // (doppelt – beabsichtigt im Original)
            TxtboxStkSegment.Text = string.Empty;
            TxtboxStkCharge.Text = string.Empty;
            txtBoxRevoNr.Text = string.Empty;

            // Bestimmte ComboBoxen zurücksetzen, andere (z. B. Status, Glassorte) bleiben erhalten.
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

            // RichTextBoxen leeren.
            RichtxtboxBemerkung.Text = string.Empty;
            RichtxtboxBemerkungWaschen.Text = string.Empty;
            RichtxtboxZusatzinfo.Text = string.Empty;

            // Bilder + Pfade zurücksetzen.
            PictureboxAuflegenLinsenPrismen.Image = null;
            PictureboxZusatzinfo.Image = null;
            PictureboxAuflegenLinsenPrismen.BackColor = Color.White;
            PictureboxZusatzinfo.BackColor = Color.White;
            LblPfadAuflegenLinsenPrismen.Text = "Doppelklick auf Bild um Pfad zu öffnen";
            LblPfadZusatzinfo.Text = "Doppelklick auf Bild um Pfad zu öffnen";

            // Datum auf "heute".
            DateTimePickerAufgenommenLinsePrisma.Value = DateTime.Now;

            // Hinweise wieder einblenden (für die nächste Seite).
            lblWichtig1.Visible = true;
            lblWichtig2.Visible = true;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Setzt für mehrere Eingabefelder die Beispiel-Platzhaltertexte (graue Schrift).
        // -----------------------------------------------------------------------------------------------------------------
        private void SetPlaceholders()
        {
            SetPlaceholder(TxtboxDurchmesser, "xx,xx");
            SetPlaceholder(TxtboxDmWaschen, "00xx");
            SetPlaceholder(TxtboxFreibereich, "xx,xx");
            SetPlaceholder(TxtboxDicke, "x,xx bzw xx,xx");
            SetPlaceholder(TxtboxZeitProzess, "x,xx");
            SetPlaceholder(TxtboxBrechwert, "x,xx");
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Generischer Helfer: setzt einen grauen Beispieltext in eine TextBox.
        // Beim Hineinklicken wird der Text gelöscht, beim Verlassen leerer Box wird er wieder angezeigt.
        // -----------------------------------------------------------------------------------------------------------------
        private void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholderText;

            // Lambda-Ausdruck "(sender, e) => { ... }" = anonyme Methode, die wir an das Event hängen.
            textBox.GotFocus += (sender, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = SystemColors.WindowText; // schwarz (Standard-Textfarbe)
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

        // ##################################################################################################
        // Event-Handler
        // ##################################################################################################

        // -----------------------------------------------------------------------------------------------------------------
        // ComboBox Belag-Vergütung → wenn Auswahl ändert: passende Prozesse nachladen.
        // -----------------------------------------------------------------------------------------------------------------
        private void ComboboxBelagVerguetung_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxBelagVerguetung.SelectedItem != null)
            {
                string selectedBelagVerguetung = ComboboxBelagVerguetung.SelectedItem.ToString();
                FillComboboxProzess(selectedBelagVerguetung);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // ComboBox GNummer → wenn Auswahl ändert: passende Glassorten nachladen
        // (oder alle Glassorten, falls keine GNummer mehr ausgewählt).
        // -----------------------------------------------------------------------------------------------------------------
        private void ComboboxGNummer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboboxGNummer.SelectedItem != null)
            {
                string selectedGNummer = ComboboxGNummer.SelectedItem.ToString();
                FillComboboxGlassorte(selectedGNummer);
            }
            else
            {
                FillComboboxGlassorte(null);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Doppelklick auf "Auflegen Linsen Prismen"-Bild → Datei-Dialog → Bild laden, Pfad merken.
        // -----------------------------------------------------------------------------------------------------------------
        private void PictureboxAuflegenLinsenPrismen_DoubleClick(object sender, EventArgs e)
        {
            // OpenFileDialog wird in "using" verpackt → wird sicher freigegeben.
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"P:\TEDuTOZ\Auftragsverwaltung Daten\Zeichnung";
                openFileDialog.Filter = "Alle Dateien (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // ShowDialog blockiert, bis OK oder Abbrechen geklickt wird.
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // Bild aus Datei laden + Pfad in Label speichern.
                    PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(selectedFilePath);
                    LblPfadAuflegenLinsenPrismen.Text = selectedFilePath;

                    // Hinweis-Label ausblenden.
                    lblWichtig1.Visible = false;
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Doppelklick auf "Zusatzinfo"-Bild → analog zu oben.
        // -----------------------------------------------------------------------------------------------------------------
        private void PictureboxZusatzinfo_DoubleClick(object sender, EventArgs e)
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
                    lblWichtig2.Visible = false;
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Artikel speichern"-Button:
        //  - liest alle Eingabefelder
        //  - validiert Zahlen / Datum
        //  - ruft SpeichereDatenInDatenbank(...) mit allen Werten auf
        // -----------------------------------------------------------------------------------------------------------------
        private void BtnArtikelSpeichern_Click(object sender, EventArgs e)
        {
            // ----- String-Werte direkt aus den Steuerelementen holen -----
            string artikelnummer = TxtboxArtikelnummer.Text;
            string bezeichnung = TxtboxBezeichnung.Text;
            string status = ComboboxStatus.Text;

            // Ersten Teil der Artikelnummer (vor "-") als Gruppennamen verwenden.
            string[] gruppennameArray = artikelnummer.Split('-').Select(s => s.Trim()).ToArray();
            string gruppenname = gruppennameArray[0];

            string zukauf = ComboBoxZukauf.Text;
            string flaeche = ComboBoxFlaeche.Text;
            string gNummer = ComboboxGNummer.Text;
            string glassorte = ComboboxGlassorte.Text;

            // ----- Zahlen sicher in decimal umwandeln; bei Fehler Hinweis + return -----
            if (!decimal.TryParse(TxtboxDurchmesser.Text, out decimal durchmesser))
            {
                MessageBox.Show("Ungültiger Wert für Durchmesser. Bitte überprüfen Sie das Format oder eine 0 eingeben.");
                return;
            }
            durchmesser = Math.Round(durchmesser, 2); // 2 Nachkommastellen

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

            // ----- Restliche Werte einlesen -----
            string durchmesserWaschen = TxtboxDmWaschen.Text;
            string seite = ComboboxSeite.Text;
            string radiusVerguetung = TxtboxRadiusVerguetung.Text;
            string radiusRueckseite = TxtboxRadiusRueckseite.Text;
            string belagVerguetung = ComboboxBelagVerguetung.Text;
            string belagRueckseite = ComboboxBelagRueckseite.Text;

            // Vom Prozess-Anzeigeformat ("Prozess Brechwert") nur den ersten Teil nehmen.
            string prozesseingabe = ComboboxProzess.Text;
            string[] prozessArray = prozesseingabe.Split(' ').Select(s => s.Trim()).ToArray();
            string prozess = prozessArray[0];

            // Vom Ring-Anzeigeformat ("Ring - DM mm") nur den Ringnamen nehmen.
            string ringeingabe = ComboboxRing.Text;
            string[] ringnameArray = ringeingabe.Split('-').Select(x => x.Trim()).ToArray();
            string ring = ringnameArray[0];

            string stkSegment = TxtboxStkSegment.Text;
            string stkGesamt = TxtboxStkCharge.Text;

            // Datum aus DateTimePicker validieren.
            if (!DateTime.TryParse(DateTimePickerAufgenommenLinsePrisma.Text, out DateTime eingabedatum))
            {
                MessageBox.Show("Ungültiges Datum. Bitte überprüfen Sie das Format.");
                return;
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

            // ----- Speichern ausführen -----
            SpeichereDatenInDatenbank(
                artikelnummer, bezeichnung, status, gruppenname, zukauf, flaeche,
                gNummer, glassorte, durchmesser, durchmesserWaschen, freibereich,
                dicke, seite, brechwert, radiusVerguetung, radiusRueckseite,
                belagVerguetung, prozess, belagRueckseite, ring, stkSegment,
                stkGesamt, zeitProzess, eingabedatum, bemerkungArtikel,
                vorreinigen, ucm, aceton, bemerkungWaschen, revoNummer,
                pfadZeichnungAuflegen, pfadZusatzinfo, textZusatzinfo);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // "Artikel ändern"-Button → öffnet das Änderungsformular mit fest hinterlegten Beispiel-Werten.
        // -----------------------------------------------------------------------------------------------------------------
        private void BtnArtikelAendern_Click(object sender, EventArgs e)
        {
            Form_ArtikelPrototypAendern form_ArtikelPrototypAendern = new Form_ArtikelPrototypAendern("12-2044", "1");
            form_ArtikelPrototypAendern.Show();    // Fenster anzeigen (nicht modal)
            form_ArtikelPrototypAendern.BringToFront(); // sofort in den Vordergrund holen
        }
    }
}
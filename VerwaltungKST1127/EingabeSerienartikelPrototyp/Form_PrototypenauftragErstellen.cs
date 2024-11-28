using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften und Bildern in der GUI)
using System.Drawing.Printing; // Importieren des System.Drawing.Printing-Namespace für Druckfunktionen (z.B. zum Drucken von Dokumenten und zur Verwaltung von Druckereinstellungen)
using System.IO; // Importieren des System.IO-Namespace für die Ein- und Ausgabefunktionen (z.B. zum Lesen und Schreiben von Dateien und Datenströmen)
using System.Runtime.InteropServices;
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)
using Excel = Microsoft.Office.Interop.Excel;

namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{

    public partial class Form_PrototypenauftragErstellen : Form
    {
        //private Excel.Application excelApp; // Speichern der Excel-Anwendung
        //private Excel.Workbook workbook;

        // Verbindungszeichenfolgen für die SQL Server-Datenbanken
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private PrintDocument printDocument;  // Deklarieren eines PrintDocument-Objekts für den Druckprozess
        // Instanzvariablen
        private string Auftragsnummer;
        private string artikel;
        private string seiteArtikel;
        private string bezeichnung;
        private string belag;
        private string prozess;
        private string durchmesser;
        private string brechwert;
        private string radiusVerguetung;
        private string radiusRueckseite;
        private string gNummer;
        private string glassorte;
        private string dicke;
        private string bemerkung;
        private string zusatzinfo;
        private string vorreinigung;
        private string handreinigung;
        private string bildPfad;

        public Form_PrototypenauftragErstellen()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Entfernt die Titelleiste und die Rahmen des Formulars
            UpdateZeitDatum();
            FillComboBoxArtikel();
            ComboboxArtikel.SelectedIndexChanged += ComboboxArtikel_SelectedIndexChanged; // Event-Handler registrieren
            // Initialisieren des PrintDocument-Objekts und Registrieren des PrintPage-Event-Handlers
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);  // Event-Handler für die Druckseite hinzufügen
            printDocument.DefaultPageSettings.Landscape = true;  // Querformat aktivieren
            SetPlaceholders();
        }

        // Methode zum Füllen der ComboBox mit Artikelnummern und Seiteninformationen
        private void FillComboBoxArtikel()
        {
            // Öffnet die Verbindung zur Datenbank
            sqlConnectionVerwaltung.Open();

            // SQL-Abfrage, um eindeutige Artikelnummern (ARTNR) und Seiten (SEITE) aus der Tabelle 'Serienlinsen' zu erhalten
            string query = @"
                SELECT DISTINCT ARTNR, SEITE
                FROM Serienlinsen
                ORDER BY ARTNR ASC";

            try
            {
                // Erstellt ein neues SqlCommand-Objekt, das die Abfrage und die Datenbankverbindung verwendet
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Führt die Abfrage aus und erhält einen SqlDataReader zum Lesen der Ergebnisse
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Löscht alle vorhandenen Einträge in der ComboBox, bevor sie mit neuen Daten befüllt wird
                        ComboboxArtikel.Items.Clear();

                        // Iteriert durch die Ergebnisse der Abfrage
                        while (reader.Read())
                        {
                            // Extrahiert die Werte für Artikelnummer (ARTNR) und Seite (SEITE) aus dem aktuellen Datensatz
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();

                            // Kombiniert die Werte zu einem anzeigbaren String im Format "ARTNR / Seite: SEITE"
                            string displayValue = $"{artikelNummerValue} / Seite: {seiteValue}";

                            // Fügt den formatierten String zur ComboBox hinzu
                            ComboboxArtikel.Items.Add(displayValue);
                        }
                    }
                }

                // Deaktiviert die automatische Sortierung der ComboBox, da die Sortierung bereits durch die SQL-Abfrage erfolgt
                ComboboxArtikel.Sorted = false;
            }
            catch (Exception ex)
            {
                // Zeigt eine Fehlermeldung an, falls beim Laden der Artikel ein Fehler auftritt
                MessageBox.Show("Fehler beim Laden der Artikel: " + ex.Message);
            }
            finally
            {
                // Schließt die Datenbankverbindung, falls sie noch geöffnet ist, um Ressourcen freizugeben
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }


        // Uhrzeit und Datumsfunktion
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now;
            LblErstelltAm.Text = "Auftrag erstellt am: " + aktuell.ToString("D");
        }


        // Hilfsmethode zum Clampen eines Wertes zwischen einem minimalen und maximalen Wert
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Justierbare Ränder in Pixeln
            int marginLeft = 20;    // Linker Rand
            int marginRight = 30;   // Rechter Rand
            int marginTop = 20;     // Oberer Rand
            int marginBottom = -15;  // Unterer Rand

            int dpi = 600; // DPI für die Druckqualität

            // Erstellen eines Bitmaps basierend auf der Größe des Formulars mit dem angegebenen DPI
            Bitmap bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            bmp.SetResolution(dpi, dpi);
            this.DrawToBitmap(bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));

            // Bild schärfen
            Bitmap sharpenedBmp = SharpenImage(bmp);

            // Temporären Speicherort für das PNG-Bild
            string tempPngPath = Path.Combine(Path.GetTempPath(), "tempPrintImage.png");

            // PNG-Bild speichern
            sharpenedBmp.Save(tempPngPath, System.Drawing.Imaging.ImageFormat.Png);

            // Berechnung der verfügbaren Breite und Höhe basierend auf den Rändern
            int printWidth = e.PageBounds.Width - marginLeft - marginRight;
            int printHeight = e.PageBounds.Height - marginTop - marginBottom;

            // Berechnung der Verhältnisse zur Anpassung des Bildes an die Druckfläche
            float scaleX = (float)printWidth / sharpenedBmp.Width;
            float scaleY = (float)printHeight / sharpenedBmp.Height;
            float scale = Math.Min(scaleX, scaleY); // Verwenden des kleineren Skalierungsfaktors, um das Bild proportional zu halten

            // Berechnung der neuen Größe des Bildes basierend auf dem Skalierungsfaktor
            int newWidth = (int)(sharpenedBmp.Width * scale);
            int newHeight = (int)(sharpenedBmp.Height * scale);

            // Berechnung der Startpositionen unter Berücksichtigung der Ränder
            int posX = marginLeft + (printWidth - newWidth) / 2;
            int posY = marginTop + (printHeight - newHeight) / 2;

            // Anti-Aliasing und TextRenderingHint aktivieren für eine bessere Druckqualität
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Das PNG-Bild laden und zeichnen
            using (Image printImage = Image.FromFile(tempPngPath))
            {
                e.Graphics.DrawImage(printImage, posX, posY, newWidth, newHeight);
            }

            //////////////////////////////////////////Test
            // Schriftarten definieren
            Font fontProjektNr = new Font("Arial", 17, FontStyle.Bold); // Dicke Schriftart für lblProjektNr
            Font fontAuftragsnummer = new Font("Arial", 16); // Schriftart für txtboxAuftragsnummer
            Font fontBezeichnungBold = new Font("Arial", 15, FontStyle.Bold); // Schriftart für mehrere elemente
            // Font fontBezeichnungBoldUnderlined = new Font("Arial", 15, FontStyle.Bold | FontStyle.Underline);
            Font fontBezeichnung = new Font("Arial", 15); // Schriftart für mehrere elemente
            Font fonBezeichnungKlein = new Font("Arial", 10);

            float xProjektNr = 39; float yProjektNr = 50; float xAuftragsnummer = 350; float yAuftragsnummer = 55;
            float xArtikelnummerLabel = 39; float yArtikelnummerLabel = 90; float xArtikelnummerBox = 195; float yArtikelnummerBox = 90;
            float xBezeichnungLabel = 39; float yBezeichnungLabel = 165; float xBezeichnungBox = 195; float yBezeichnungerBox = 165;
            float xMengeLabel = 39; float yMengeLabel = 200; float xMengeBox = 195; float yMengeBox = 200;
            float xBelagLabel = 39; float yBelagLabel = 235; float xBelagBox = 195; float yBelagBox = 235;
            float xProzessLabel = 350; float yProzessLabel = 235; float xProzessBox = 495; float yProzessBox = 235;
            float xInfoAuflegen = 720; float yInfoAulegen = 60; float xDatenArtikel = 39; float yDatenArtikel = 295;
            float xDurchmesser = 39; float yDurchmesser = 340; float xDurchmesserBox = 195; float yDurchmesserBox = 340;
            float xRadiusVerguetung = 39; float yRadiusVerguetung = 375; float xRadiusVerguetungBox = 195; float yRadiusVerguetungBox = 375;
            float xGnummer = 39; float yGnummer = 410; float xGnummerBox = 195; float yGnummerBox = 410;
            float xDicke = 350; float yDicke = 305; float xDickeBox = 495; float yDickeBox = 305;
            float xBrechwert = 350; float yBrechwert = 340; float xBrechwertBox = 495; float yBrechwertBox = 340;
            float xRadiusRueckseite = 350; float yRadiusRueckseite = 375; float xRadiusRueckseiteBox = 495; float yRadiusRueckseiteBox = 375;
            float xGlassorte = 350; float yGlassorte = 410; float xGlassorteBox = 495; float yGlassorteBox = 410;
            float xZusatzInfo = 720; float yZusatzInfo = 430; float xVorbehandlung = 39; float yVorbehandlung = 465;
            float xVorreinigen = 39; float yVorreinigen = 515; float xVorreinigenBox = 185; float yVorreinigenBox = 515;
            float xHandreinigung = 350; float yHandreinigung = 515; float xHandreinigungBox = 495; float yHandreinigungBox = 515;
            float xBearbeitung = 39; float yBearbeitung = 570; float xDatum = 120; float yDatum = 615;
            float xName = 265; float yName = 615; float xStueck = 393; float yStueck = 615;
            float xStueckGes = 475; float yStueckGes = 615; float xZeit = 598; float yZeit = 615;
            float xDatumAktuell = 39; float yDatumAktuell = 753; float xDokument = 730; float yDokument = 753;

            // Texte zeichnen
            e.Graphics.DrawString(lblNrProjekt.Text, fontProjektNr, Brushes.DarkGreen, xProjektNr, yProjektNr);
            e.Graphics.DrawString(txtboxAuftragsnummer.Text, fontAuftragsnummer, Brushes.Black, xAuftragsnummer, yAuftragsnummer);
            e.Graphics.DrawString(lblArtikelnummer.Text, fontProjektNr, Brushes.Black, xArtikelnummerLabel, yArtikelnummerLabel);
            e.Graphics.DrawString(ComboboxArtikel.Text, fontBezeichnung, Brushes.Black, xArtikelnummerBox, yArtikelnummerBox);
            e.Graphics.DrawString(lblBezeichnung.Text, fontBezeichnungBold, Brushes.Black, xBezeichnungLabel, yBezeichnungLabel);
            e.Graphics.DrawString(txtboxBezeichnung.Text, fontBezeichnung, Brushes.Black, xBezeichnungBox, yBezeichnungerBox);
            e.Graphics.DrawString(lblMenge.Text, fontBezeichnungBold, Brushes.Black, xMengeLabel, yMengeLabel);
            e.Graphics.DrawString(TxtboxMenge.Text, fontBezeichnung, Brushes.Black, xMengeBox, yMengeBox);
            e.Graphics.DrawString(lblBelag.Text, fontBezeichnungBold, Brushes.Black, xBelagLabel, yBelagLabel);
            e.Graphics.DrawString(txtboxBelag.Text, fontBezeichnung, Brushes.Blue, xBelagBox, yBelagBox);
            e.Graphics.DrawString(lblProzess.Text, fontBezeichnungBold, Brushes.Black, xProzessLabel, yProzessLabel);
            e.Graphics.DrawString(txtboxProzess.Text, fontBezeichnung, Brushes.Red, xProzessBox, yProzessBox);
            e.Graphics.DrawString(lblInfoAuflegen.Text, fontBezeichnungBold, Brushes.Black, xInfoAuflegen, yInfoAulegen);
            e.Graphics.DrawString(lblDatenArtikel.Text, fontBezeichnungBold, Brushes.Black, xDatenArtikel, yDatenArtikel);
            e.Graphics.DrawString(lblDurchmesser.Text, fontBezeichnungBold, Brushes.Black, xDurchmesser, yDurchmesser);
            e.Graphics.DrawString(txtboxDurchmesser.Text, fontBezeichnung, Brushes.Black, xDurchmesserBox, yDurchmesserBox);
            e.Graphics.DrawString(lblRadiusverguetung.Text, fontBezeichnungBold, Brushes.Black, xRadiusVerguetung, yRadiusVerguetung);
            e.Graphics.DrawString(txtboxRadiusVerguetung.Text, fontBezeichnung, Brushes.Black, xRadiusVerguetungBox, yRadiusVerguetungBox);
            e.Graphics.DrawString(lblGnummer.Text, fontBezeichnungBold, Brushes.Black, xGnummer, yGnummer);
            e.Graphics.DrawString(txtboxGnummer.Text, fontBezeichnung, Brushes.Black, xGnummerBox, yGnummerBox);
            e.Graphics.DrawString(lblDicke.Text, fontBezeichnungBold, Brushes.Black, xDicke, yDicke);
            e.Graphics.DrawString(txtboxDicke.Text, fontBezeichnung, Brushes.Black, xDickeBox, yDickeBox);
            e.Graphics.DrawString(lblBrechwert.Text, fontBezeichnungBold, Brushes.Black, xBrechwert, yBrechwert);
            e.Graphics.DrawString(txtboxBrechwert.Text, fontBezeichnung, Brushes.Black, xBrechwertBox, yBrechwertBox);
            e.Graphics.DrawString(lblRadiusRueckseite.Text, fontBezeichnungBold, Brushes.Black, xRadiusRueckseite, yRadiusRueckseite);
            e.Graphics.DrawString(txtboxRadiusRueckseite.Text, fontBezeichnung, Brushes.Black, xRadiusRueckseiteBox, yRadiusRueckseiteBox);
            e.Graphics.DrawString(lblGlassorte.Text, fontBezeichnungBold, Brushes.Black, xGlassorte, yGlassorte);
            e.Graphics.DrawString(txtboxGlassorte.Text, fontBezeichnung, Brushes.Black, xGlassorteBox, yGlassorteBox);
            e.Graphics.DrawString(lblVorbehandlung.Text, fontBezeichnungBold, Brushes.Black, xVorbehandlung, yVorbehandlung);
            e.Graphics.DrawString(lblZusatzinfo.Text, fontBezeichnungBold, Brushes.Black, xZusatzInfo, yZusatzInfo);
            e.Graphics.DrawString(lblVorreinigung.Text, fontBezeichnungBold, Brushes.Black, xVorreinigen, yVorreinigen);
            e.Graphics.DrawString(txtboxVorreinigung.Text, fontBezeichnung, Brushes.Black, xVorreinigenBox, yVorreinigenBox);
            e.Graphics.DrawString(lblHandreinigung.Text, fontBezeichnungBold, Brushes.Black, xHandreinigung, yHandreinigung);
            e.Graphics.DrawString(txtboxHandreinigung.Text, fontBezeichnung, Brushes.Black, xHandreinigungBox, yHandreinigungBox);
            e.Graphics.DrawString(lblWerAufgelegt.Text, fontBezeichnungBold, Brushes.Black, xBearbeitung, yBearbeitung);
            e.Graphics.DrawString(lblDatum.Text, fonBezeichnungKlein, Brushes.Black, xDatum, yDatum);
            e.Graphics.DrawString(lblName.Text, fonBezeichnungKlein, Brushes.Black, xName, yName);
            e.Graphics.DrawString(lblStueck.Text, fonBezeichnungKlein, Brushes.Black, xStueck, yStueck);
            e.Graphics.DrawString(lblStueckGes.Text, fonBezeichnungKlein, Brushes.Black, xStueckGes, yStueckGes);
            e.Graphics.DrawString(lblZeit.Text, fonBezeichnungKlein, Brushes.Black, xZeit, yZeit);
            e.Graphics.DrawString(LblErstelltAm.Text, fonBezeichnungKlein, Brushes.Black, xDatumAktuell, yDatumAktuell);
            e.Graphics.DrawString(lblDokument.Text, fonBezeichnungKlein, Brushes.Black, xDokument, yDokument);

            // Aufräumen
            bmp.Dispose();
            sharpenedBmp.Dispose(); // Freigabe der scharfen Bitmap
                                    
            File.Delete(tempPngPath); // Temporäre Datei löschen
        }

        // Wenn auf den Button Drucken gedrückt wird
        private void BtnDrucken_Click_1(object sender, EventArgs e)
        {

            // Setze die Texte für den Druck
            //ComboboxArtikel.Text = "PR " + ComboboxArtikel.Text;
            TxtboxMenge.Text = TxtboxMenge.Text + " Stk.";
            txtboxDurchmesser.Text = txtboxDurchmesser.Text + " mm";
            txtboxDicke.Text = txtboxDicke.Text + " mm";
            Auftragsnummer = txtboxAuftragsnummer.Text.ToString();

            // Elemente unsichtbar machen
            VisibleFalse();

            // Fokus auf ein anderes Steuerelement setzen
            this.ActiveControl = null;

            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
                BtnClose.Visible = true;
                // "Stk." aus der Menge-Textbox entfernen
                if (!string.IsNullOrEmpty(TxtboxMenge.Text) && TxtboxMenge.Text.Contains(" Stk."))
                {
                    TxtboxMenge.Text = TxtboxMenge.Text.Replace(" Stk.", "");
                }

            }
            else
            {
                BtnClose.Visible = true;
                BtnDrucken.Visible = true;
                return;
            }

            // Optional: Setze die Sichtbarkeit der Elemente zurück, wenn der Druck abgeschlossen ist
            VisibleTrue();
            // Bild in die Zwischenablage kopieren
            CopyImageToClipboard();
            CopyDataToExcel(@"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Chargenbegleitblatt.xlsx");
        }

        private void VisibleFalse()
        {
            lblNrProjekt.Visible = false;
            txtboxAuftragsnummer.Visible = false;
            lblArtikelnummer.Visible = false;
            ComboboxArtikel.Visible = false;
            lblBezeichnung.Visible = false;
            txtboxBezeichnung.Visible = false;
            lblMenge.Visible = false;
            TxtboxMenge.Visible = false;
            lblBelag.Visible = false;
            txtboxBelag.Visible = false;
            lblProzess.Visible = false;
            txtboxProzess.Visible = false;
            lblInfoAuflegen.Visible = false;
            lblDatenArtikel.Visible = false;
            lblDurchmesser.Visible = false;
            txtboxDurchmesser.Visible = false;
            lblRadiusverguetung.Visible = false;
            txtboxRadiusVerguetung.Visible = false;
            lblGnummer.Visible = false;
            txtboxGnummer.Visible = false;
            lblDicke.Visible = false;
            txtboxDicke.Visible = false;
            lblBrechwert.Visible = false;
            txtboxBrechwert.Visible = false;
            lblRadiusRueckseite.Visible = false;
            txtboxRadiusRueckseite.Visible = false;
            lblGlassorte.Visible = false;
            txtboxGlassorte.Visible = false;
            lblVorbehandlung.Visible = false;
            lblZusatzinfo.Visible = false;
            lblVorreinigung.Visible = false;
            txtboxVorreinigung.Visible = false;
            lblHandreinigung.Visible = false;
            txtboxHandreinigung.Visible = false;
            lblWerAufgelegt.Visible = false;
            lblDatum.Visible = false;
            lblName.Visible = false;
            lblStueck.Visible = false;
            lblStueckGes.Visible = false;
            lblZeit.Visible = false;
            LblErstelltAm.Visible = false;
            lblDokument.Visible = false;
            BtnDrucken.Visible = false;
            BtnClose.Visible = false;
        }
        private void VisibleTrue()
        {
            lblNrProjekt.Visible = true;
            txtboxAuftragsnummer.Visible = true;
            lblArtikelnummer.Visible = true;
            ComboboxArtikel.Visible = true;
            lblBezeichnung.Visible = true;
            txtboxBezeichnung.Visible = true;
            lblMenge.Visible = true;
            TxtboxMenge.Visible = true;
            lblBelag.Visible = true;
            txtboxBelag.Visible = true;
            lblProzess.Visible = true;
            txtboxProzess.Visible = true;
            lblInfoAuflegen.Visible = true;
            lblDatenArtikel.Visible = true;
            lblDurchmesser.Visible = true;
            txtboxDurchmesser.Visible = true;
            lblRadiusverguetung.Visible = true;
            txtboxRadiusVerguetung.Visible = true;
            lblGnummer.Visible = true;
            txtboxGnummer.Visible = true;
            lblBrechwert.Visible = true;
            txtboxBrechwert.Visible = true;
            lblDicke.Visible = true;
            txtboxDicke.Visible = true;
            lblRadiusRueckseite.Visible = true;
            txtboxRadiusRueckseite.Visible = true;
            lblGlassorte.Visible = true;
            txtboxGlassorte.Visible = true;
            lblVorbehandlung.Visible = true;
            lblZusatzinfo.Visible = true;
            lblVorreinigung.Visible = true;
            txtboxVorreinigung.Visible = true;
            lblHandreinigung.Visible = true;
            txtboxHandreinigung.Visible = true;
            lblWerAufgelegt.Visible = true;
            lblDatum.Visible = true;
            lblName.Visible = true;
            lblStueck.Visible = true;
            lblStueckGes.Visible = true;
            lblZeit.Visible = true;
            LblErstelltAm.Visible = true;
            lblDokument.Visible = true;
            BtnDrucken.Visible = true;
            BtnClose.Visible = true;
        }

        // Druckformular beenden
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ComboboxArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Artikelnummer und Seite extrahieren
            string selectedValue = ComboboxArtikel.SelectedItem.ToString();
            string[] parts = selectedValue.Split(new[] { " / Seite: " }, StringSplitOptions.None);

            // Überprüfen, ob beide Teile extrahiert werden konnten
            if (parts.Length == 2)
            {
                string artikelNummer = parts[0]; // Artikelnummer extrahieren
                string seite = parts[1]; // Seite extrahieren

                // Zusätzliche Informationen basierend auf Artikelnummer und Seite laden
                LoadAdditionalInfo(artikelNummer, seite);
                lblZuzerstAuswaehlen.Visible = false;
            }
            else
            {
                // Fehlerbehandlung oder Logik für den Fall, dass die Extraktion fehlschlägt
                MessageBox.Show("Bitte wählen Sie einen gültigen Artikel.");
            }
        }



        private void LoadAdditionalInfo(string artikelNummer, string seite)
        {
            sqlConnectionVerwaltung.Open();
            string query = @"
                SELECT * 
                FROM Serienlinsen
                WHERE ARTNR = @artikelNummer AND SEITE = @seite"; // Seite in die Abfrage einbeziehen

            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    sqlCommand.Parameters.AddWithValue("@artikelNummer", artikelNummer); // Parameter für die Artikelnummer
                    sqlCommand.Parameters.AddWithValue("@seite", seite); // Parameter für die Seite

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Werte aus dem Reader lesen und in die entsprechenden TextBoxen/ComboBoxen setzen
                            txtboxBezeichnung.Text = reader["BEZ"].ToString();
                            txtboxBelag.Text = reader["VERGBELAG"].ToString(); // Beispiel für eine TextBox
                            txtboxProzess.Text = reader["MATERIAL"].ToString(); // Beispiel für eine TextBox
                            txtboxDurchmesser.Text = reader["DM"].ToString();
                            txtboxBrechwert.Text = reader["ND"].ToString();
                            txtboxRadiusVerguetung.Text = reader["Radius1"].ToString();
                            txtboxRadiusRueckseite.Text = reader["Radius2"].ToString();
                            txtboxGnummer.Text = reader["G_Nummer"].ToString();
                            txtboxGlassorte.Text = reader["GLASSORTE"].ToString();
                            txtboxDicke.Text = reader["DICKE"].ToString();
                            richtxtboxInforamationAuflegen.Text = reader["BEMERKUNG"].ToString();
                            richtxtboxZusatzinfo.Text = reader["InfoZeichnung_Bemerkungen"].ToString();
                            txtboxVorreinigung.Text = reader["Vorreinigung"].ToString();
                            txtboxHandreinigung.Text = reader["Handreinigung"].ToString();

                            //Öffentlich zugänglich machen
                            //Auftragsnummer = txtboxAuftragsnummer.Text.ToString();
                            artikel = reader["ARTNR"].ToString();
                            seiteArtikel = reader["SEITE"].ToString();
                            bezeichnung = reader["BEZ"].ToString();
                            belag = reader["VERGBELAG"].ToString();
                            prozess = reader["MATERIAL"].ToString();
                            durchmesser = reader["DM"].ToString();
                            brechwert = reader["ND"].ToString();
                            radiusVerguetung = reader["Radius1"].ToString();
                            radiusRueckseite = reader["Radius2"].ToString();
                            gNummer = reader["G_Nummer"].ToString();
                            glassorte = reader["GLASSORTE"].ToString();
                            dicke = reader["DICKE"].ToString();
                            bemerkung = reader["BEMERKUNG"].ToString();
                            zusatzinfo = reader["InfoZeichnung_Bemerkungen"].ToString();
                            vorreinigung = reader["Vorreinigung"].ToString();
                            handreinigung = reader["Handreinigung"].ToString();
                            bildPfad = reader["Zeichnungspfad"].ToString();

                            // Bildpfad aus der Datenbank abfragen
                            string bildPfadInfoZeichnung = reader["Zeichnungspfad"].ToString();
                            string bildPfadInfoZeichnungBemerkung = reader["InfoZeichnung"].ToString();
                            // Überprüfen, ob der Pfad gültig ist und das Bild existiert
                            if (!string.IsNullOrEmpty(bildPfadInfoZeichnung) && System.IO.File.Exists(bildPfadInfoZeichnung))
                            {
                                // Bild in der PictureBox anzeigen
                                PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(bildPfadInfoZeichnung);
                            }
                            else
                            {
                                // Fehlerbehandlung, wenn das Bild nicht gefunden wird
                                MessageBox.Show("Das Bild konnte nicht gefunden werden.");
                                PictureboxAuflegenLinsenPrismen.Image = null; // Bild zurücksetzen
                            }
                            if (!string.IsNullOrEmpty(bildPfadInfoZeichnungBemerkung) && System.IO.File.Exists(bildPfadInfoZeichnungBemerkung))
                            {
                                PictureboxZusatzinfo.Image = Image.FromFile(bildPfadInfoZeichnungBemerkung);
                            }
                            else
                            {
                                // Fehlerbehandlung, wenn das Bild nicht gefunden wird
                                MessageBox.Show("Das Bild InfoZeichnungBemerkungen konnte nicht gefunden werden.");
                                PictureboxAuflegenLinsenPrismen.Image = null; // Bild zurücksetzen
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Informationen: " + ex.Message);
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == System.Data.ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // Funktion um die felder zu leeren wenn eine weiter seite eingegeben werden soll
        private void SetPlaceholders()
        {
            SetPlaceholder(txtboxAuftragsnummer, "XXXXXXXXXXX");
        }

        // Waserzeichen setzten
        private void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            // Setzt die Textfarbe auf Grau und zeigt den Platzhaltertext an
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholderText;

            // Ereignis-Handler für den GotFocus (wenn das Textfeld den Fokus erhält)
            textBox.GotFocus += (sender, e) =>
            {
                // Überprüft, ob der aktuelle Text der Platzhalter ist
                if (textBox.Text == placeholderText)
                {
                    // Löscht den Platzhaltertext und setzt die Textfarbe auf die Standardfarbe
                    textBox.Text = "";
                    textBox.ForeColor = SystemColors.WindowText;
                }
            };

            // Ereignis-Handler für den LostFocus (wenn das Textfeld den Fokus verliert)
            textBox.LostFocus += (sender, e) =>
            {
                // Überprüft, ob das Textfeld leer ist
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    // Setzt den Platzhaltertext zurück und die Textfarbe auf Grau
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private Bitmap SharpenImage(Bitmap image)
        {
            // Erstellt eine neue Bitmap mit der gleichen Größe wie das Originalbild
            Bitmap sharpenedImage = new Bitmap(image.Width, image.Height);

            // Definiert einen milderen Schärfungsfilter mit einem 3x3-Kernel
            float[,] kernel = new float[,]
            {
                { 0, -0.2f, 0 },
                { -0.2f, 1.8f, -0.2f },
                { 0, -0.2f, 0 }
            };

            // Wendet den Schärfungsfilter auf jedes Pixel des Bildes an (ausgenommen Randpixel)
            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    // Initialisiert die Variablen für die RGB-Kanäle
                    float r = 0, g = 0, b = 0;

                    // Schleife über die Nachbarpixel zur Anwendung des Schärfungsfilters
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            // Holt den Farbwert des Nachbarpixels
                            Color pixel = image.GetPixel(x + i, y + j);
                            // Wendet den Kernelwert auf die RGB-Kanäle an
                            r += pixel.R * kernel[i + 1, j + 1];
                            g += pixel.G * kernel[i + 1, j + 1];
                            b += pixel.B * kernel[i + 1, j + 1];
                        }
                    }

                    // Normalisiert die RGB-Werte, um sicherzustellen, dass sie im Bereich 0-255 liegen
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    // Setzt den neuen Farbwert in der geschärften Bitmap
                    Color newColor = Color.FromArgb((int)r, (int)g, (int)b);
                    sharpenedImage.SetPixel(x, y, newColor);
                }
            }
            // Gibt das geschärfte Bild zurück
            return sharpenedImage;
        }

        private void CopyImageToClipboard()
        {
            if (PictureboxAuflegenLinsenPrismen.Image != null)
            {
                Clipboard.SetImage(PictureboxAuflegenLinsenPrismen.Image);
            }
            else
            {
                MessageBox.Show("Kein Bild vorhanden, das kopiert werden kann.");
            }
        }

        // Funktion zum Kopieren der Daten in Excel
        public void CopyDataToExcel(string filePath)
        {
            // Zellenzuweisungen
            var cellAssignments = new System.Collections.Generic.Dictionary<string, string>
            {
                { "H1", Auftragsnummer },{"H24", Auftragsnummer},
                { "D2", artikel }, { "D25", artikel },
                { "F2", bezeichnung }, { "F25", bezeichnung },
                { "D3", seiteArtikel }, { "D26", seiteArtikel },
                { "C8", belag }, { "C31", belag },
                { "E8", prozess }, { "E31", prozess },
                { "G11", radiusVerguetung }, { "G34", radiusVerguetung },
                { "G5", radiusRueckseite }, { "G28", radiusRueckseite },
                { "I14", gNummer }, { "I37", gNummer },
                { "I15", glassorte }, { "I38", glassorte },
                { "I16", durchmesser + " mm" }, { "I39", durchmesser + " mm" },
                { "I17", dicke + " mm" }, { "I40", dicke + " mm" },
                { "B15", bemerkung }, {"B38", bemerkung },
            };

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                Console.WriteLine("Starte Excel Anwendung...");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Die Datei existiert nicht: " + filePath);
                    return;
                }

                workbook = excelApp.Workbooks.Open(filePath);
                excelApp.Visible = true;
                worksheet = (Excel.Worksheet)workbook.Sheets[1];

                foreach (var assignment in cellAssignments)
                {
                    Console.WriteLine($"Schreibe in Zelle {assignment.Key}: {assignment.Value}");
                    worksheet.Range[assignment.Key].Value = assignment.Value;
                }

                // Bild aus der Zwischenablage einfügen
                Console.WriteLine("Füge Bild aus der Zwischenablage ein...");

                // Füge das Bild in die Zelle G6 ein
                Excel.Range targetCell = worksheet.Cells[6, 7]; // Zelle G6
                targetCell.Select(); // Wähle die Zielzelle aus
                excelApp.ActiveSheet.Paste(); // Füge das Bild ein

                // Größe des Bildes anpassen
                var picture = worksheet.Shapes.Item(worksheet.Shapes.Count); // Das zuletzt eingefügte Bild
                picture.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue; // Seitenverhältnis beibehalten
                picture.Width = 140; // Hier die Breite anpassen
                picture.Height = 90; // Hier die Höhe anpassen

                // Füge das Bild in die Zelle G29 ein
                Excel.Range targetCell1 = worksheet.Cells[29, 7]; // Zelle G29
                targetCell1.Select(); // Wähle die Zielzelle aus
                excelApp.ActiveSheet.Paste(); // Füge das Bild ein

                // Größe des zweiten Bildes anpassen
                var picture1 = worksheet.Shapes.Item(worksheet.Shapes.Count); // Das zuletzt eingefügte Bild
                picture1.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue; // Seitenverhältnis beibehalten
                picture1.Width = 140; // Hier die Breite anpassen
                picture1.Height = 90; // Hier die Höhe anpassen

                // Drucken der ersten Seite
                Console.WriteLine("Drucke die erste Seite der Excel-Datei...");
                worksheet.PrintOut(From: 1, To: 1); // Drucken nur der ersten Seite

                // Optional: Warte einige Sekunden, um sicherzustellen, dass der Druckauftrag verarbeitet wird
                System.Threading.Thread.Sleep(4000); // Pause für 2 Sekunden
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
            }
            finally
            {
                try // Versuche, die Excel-Objekte zu schließen und freizugeben
                {
                    if (workbook != null)
                    {
                        workbook.Close(false); // Schließe die Arbeitsmappe ohne zu speichern
                        Marshal.ReleaseComObject(workbook);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    if (excelApp != null)
                    {
                        excelApp.Quit(); // Schließe die Excel-Anwendung
                        Marshal.ReleaseComObject(excelApp);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception closeEx)
                {
                    Console.WriteLine($"Fehler beim Schließen der Excel-Objekte: {closeEx.Message}");
                }
                finally
                {
                    worksheet = null;
                    workbook = null;
                    excelApp = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers(); 
        }
    }
}
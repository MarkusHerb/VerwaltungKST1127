// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben können.
// ===================================================================================================
using System;                                           // Basis-Typen (string, int, DateTime, EventArgs, Exception ...)
using System.Data.SqlClient;                            // SQL-Server-Zugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Drawing;                                   // Grafische Typen (Color, Font, Bitmap, Image, Brushes ...)
using System.Drawing.Printing;                          // Druckklassen (PrintDocument, PrintPageEventArgs, PrintDialog ...)
using System.IO;                                        // Datei-/Pfad-Operationen (File, Path ...)
using System.Runtime.InteropServices;                   // Marshal.ReleaseComObject (für Excel-COM-Interop)
using System.Windows.Forms;                             // Windows-Forms (Form, Button, ComboBox, Clipboard ...)
using Excel = Microsoft.Office.Interop.Excel;           // Alias "Excel" für den Excel-Namespace (kürzere Schreibweise)

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    // public partial class ... : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien aufgeteilt sein (z. B. ...Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_PrototypenauftragErstellen : Form
    {
        // (Optional gewesene Felder – aktuell auskommentiert.)
        //private Excel.Application excelApp;
        //private Excel.Workbook workbook;

        // -----------------------------------------------------------------------------------------------------------------
        // Felder ("Variablen der Klasse"). private = nur in dieser Klasse sichtbar.
        // readonly = darf nur einmal (hier oder im Konstruktor) gesetzt werden.
        // -----------------------------------------------------------------------------------------------------------------

        // Verbindung zur Verwaltungs-Datenbank.
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // PrintDocument repräsentiert das, was gedruckt werden soll.
        // Wir hängen ihm das PrintPage-Event an und sagen damit, WAS auf jeder Seite erscheint.
        private PrintDocument printDocument;

        // Instanzvariablen: speichern die zuletzt geladenen Artikel-Werte
        // (werden später beim Druck und beim Excel-Export benötigt).
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

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_PrototypenauftragErstellen()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_PrototypenauftragErstellen()
        {
            InitializeComponent();                  // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)

            // Fensterrahmen + Titelleiste komplett entfernen → Vollflächige eigene Optik.
            this.FormBorderStyle = FormBorderStyle.None;

            UpdateZeitDatum();                       // "Erstellt am"-Label setzen
            FillComboBoxArtikel();                   // ComboBox mit allen Artikeln + Seiten befüllen

            // Event-Handler verbinden → reagiert auf Auswahländerungen in der ComboBox.
            ComboboxArtikel.SelectedIndexChanged += ComboboxArtikel_SelectedIndexChanged;

            // PrintDocument vorbereiten:
            // - PrintPage-Event verbinden (dort zeichnen wir den Inhalt der gedruckten Seite)
            // - Querformat aktivieren
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
            printDocument.DefaultPageSettings.Landscape = true;

            SetPlaceholders();                       // Platzhaltertexte (graue Beispiel-Eingaben) setzen
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Befüllt die Artikel-ComboBox mit eindeutigen Artikel-/Seiten-Kombinationen aus der DB.
        // -----------------------------------------------------------------------------------------------------------------
        private void FillComboBoxArtikel()
        {
            sqlConnectionVerwaltung.Open();

            // SQL: DISTINCT entfernt Duplikate; ORDER BY sortiert aufsteigend nach ARTNR.
            string query = @"
                SELECT DISTINCT ARTNR, SEITE
                FROM Serienlinsen
                ORDER BY ARTNR ASC";

            try
            {
                // "using" sorgt dafür, dass das SqlCommand am Ende automatisch freigegeben wird.
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // Erst leeren → keine Doppeleinträge bei erneutem Befüllen.
                        ComboboxArtikel.Items.Clear();

                        // reader.Read() liefert true, solange noch eine Zeile übrig ist.
                        while (reader.Read())
                        {
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();

                            // Anzeigeformat: "12-2044 / Seite: 1"
                            string displayValue = $"{artikelNummerValue} / Seite: {seiteValue}";
                            ComboboxArtikel.Items.Add(displayValue);
                        }
                    }
                }

                // Sorted = false → Reihenfolge bleibt so wie via SQL ORDER BY geliefert.
                ComboboxArtikel.Sorted = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Artikel: " + ex.Message);
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
        // Aktualisiert das Label mit dem aktuellen Datum (Format "D" = lange Datumsschreibweise).
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now;
            LblErstelltAm.Text = "Auftrag erstellt am: " + aktuell.ToString("D");
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Hilfsfunktion: begrenzt einen Wert auf einen Bereich [min..max].
        // (Aktuell ungenutzt, aber zur Sicherheit drin.)
        // -----------------------------------------------------------------------------------------------------------------
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // PrintPage-Event: hier wird beschrieben, WAS auf einer gedruckten Seite gezeichnet werden soll.
        // - rendert das Formular als Bitmap
        // - schärft das Bild
        // - skaliert es auf den Druckbereich
        // - zeichnet zusätzlich Texte mit großen Schriftgrößen drüber
        // -----------------------------------------------------------------------------------------------------------------
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // ----- Ränder in Pixeln -----
            int marginLeft = 20;
            int marginRight = 30;
            int marginTop = 20;
            int marginBottom = -15;

            int dpi = 600; // hohe Druckauflösung

            // Bitmap in der Größe des Formulars erzeugen → DPI setzen → Formular hineinrendern.
            Bitmap bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            bmp.SetResolution(dpi, dpi);
            this.DrawToBitmap(bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));

            // Bild schärfen (3x3-Filter, siehe Methode SharpenImage).
            Bitmap sharpenedBmp = SharpenImage(bmp);

            // Geschärftes Bild als temporäre PNG-Datei zwischenspeichern.
            string tempPngPath = Path.Combine(Path.GetTempPath(), "tempPrintImage.png");
            sharpenedBmp.Save(tempPngPath, System.Drawing.Imaging.ImageFormat.Png);

            // Verfügbare Druckfläche (Seitengröße minus Ränder) berechnen.
            int printWidth = e.PageBounds.Width - marginLeft - marginRight;
            int printHeight = e.PageBounds.Height - marginTop - marginBottom;

            // Skalierungsfaktor in X- und Y-Richtung berechnen.
            float scaleX = (float)printWidth / sharpenedBmp.Width;
            float scaleY = (float)printHeight / sharpenedBmp.Height;
            // Den kleineren Faktor verwenden → Bild bleibt proportional, ohne abgeschnitten zu werden.
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(sharpenedBmp.Width * scale);
            int newHeight = (int)(sharpenedBmp.Height * scale);

            // Bild zentriert in den Druckbereich legen.
            int posX = marginLeft + (printWidth - newWidth) / 2;
            int posY = marginTop + (printHeight - newHeight) / 2;

            // Render-Optionen für hohe Druckqualität.
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Bild aus der Temp-Datei laden und auf der Druckseite zeichnen.
            using (Image printImage = Image.FromFile(tempPngPath))
            {
                e.Graphics.DrawImage(printImage, posX, posY, newWidth, newHeight);
            }

            //////////////////////////////////////////
            // Zusätzliche Texte direkt auf die Seite zeichnen (für gute Lesbarkeit beim Druck)
            //////////////////////////////////////////

            // Schriftarten anlegen (nur einmal definieren, dann oft verwenden).
            Font fontProjektNr = new Font("Arial", 17, FontStyle.Bold);
            Font fontAuftragsnummer = new Font("Arial", 16);
            Font fontBezeichnungBold = new Font("Arial", 15, FontStyle.Bold);
            Font fontBezeichnung = new Font("Arial", 15);
            Font fonBezeichnungKlein = new Font("Arial", 10);

            // ----- Positionen der einzelnen Beschriftungen / Werte (X, Y) -----
            float xProjektNr = 39; float yProjektNr = 50;
            float xAuftragsnummer = 350; float yAuftragsnummer = 55;
            float xArtikelnummerLabel = 39; float yArtikelnummerLabel = 90;
            float xArtikelnummerBox = 195; float yArtikelnummerBox = 90;
            float xBezeichnungLabel = 39; float yBezeichnungLabel = 165;
            float xBezeichnungBox = 195; float yBezeichnungerBox = 165;
            float xMengeLabel = 39; float yMengeLabel = 200;
            float xMengeBox = 195; float yMengeBox = 200;
            float xBelagLabel = 39; float yBelagLabel = 235;
            float xBelagBox = 195; float yBelagBox = 235;
            float xProzessLabel = 350; float yProzessLabel = 235;
            float xProzessBox = 495; float yProzessBox = 235;
            float xInfoAuflegen = 720; float yInfoAulegen = 60;
            float xDatenArtikel = 39; float yDatenArtikel = 295;
            float xDurchmesser = 39; float yDurchmesser = 340;
            float xDurchmesserBox = 195; float yDurchmesserBox = 340;
            float xRadiusVerguetung = 39; float yRadiusVerguetung = 375;
            float xRadiusVerguetungBox = 195; float yRadiusVerguetungBox = 375;
            float xGnummer = 39; float yGnummer = 410;
            float xGnummerBox = 195; float yGnummerBox = 410;
            float xDicke = 350; float yDicke = 305;
            float xDickeBox = 495; float yDickeBox = 305;
            float xBrechwert = 350; float yBrechwert = 340;
            float xBrechwertBox = 495; float yBrechwertBox = 340;
            float xRadiusRueckseite = 350; float yRadiusRueckseite = 375;
            float xRadiusRueckseiteBox = 495; float yRadiusRueckseiteBox = 375;
            float xGlassorte = 350; float yGlassorte = 410;
            float xGlassorteBox = 495; float yGlassorteBox = 410;
            float xZusatzInfo = 720; float yZusatzInfo = 430;
            float xVorbehandlung = 39; float yVorbehandlung = 465;
            float xVorreinigen = 39; float yVorreinigen = 515;
            float xVorreinigenBox = 185; float yVorreinigenBox = 515;
            float xHandreinigung = 350; float yHandreinigung = 515;
            float xHandreinigungBox = 495; float yHandreinigungBox = 515;
            float xBearbeitung = 39; float yBearbeitung = 570;
            float xDatum = 120; float yDatum = 615;
            float xName = 265; float yName = 615;
            float xStueck = 393; float yStueck = 615;
            float xStueckGes = 475; float yStueckGes = 615;
            float xZeit = 598; float yZeit = 615;
            float xDatumAktuell = 39; float yDatumAktuell = 753;
            float xDokument = 730; float yDokument = 753;

            // DrawString(text, font, brush, x, y) malt einen Text auf die Druckseite.
            // Brushes.Black/Blue/Red sind vordefinierte Pinsel mit fester Farbe.
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

            // ----- Aufräumen: Bitmaps freigeben + temporäre Datei löschen -----
            // Dispose() gibt unmanaged Ressourcen (Speicher des Bildes) sofort frei.
            bmp.Dispose();
            sharpenedBmp.Dispose();
            File.Delete(tempPngPath);
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "Drucken":
        //  - Texte für den Druck vorbereiten (Einheiten anhängen, Auftragsnummer merken)
        //  - UI-Elemente ausblenden (damit nur das Druckbild erscheint)
        //  - Druck-Dialog öffnen → bei OK drucken
        //  - Anschließend Excel-Datei mit den Daten füllen + drucken
        // -----------------------------------------------------------------------------------------------------------------
        private void BtnDrucken_Click_1(object sender, EventArgs e)
        {
            // Einheiten an Anzeigetexte anhängen.
            TxtboxMenge.Text = TxtboxMenge.Text + " Stk.";
            txtboxDurchmesser.Text = txtboxDurchmesser.Text + " mm";
            txtboxDicke.Text = txtboxDicke.Text + " mm";

            // Auftragsnummer in Klassenvariable speichern (wird später für Excel gebraucht).
            Auftragsnummer = txtboxAuftragsnummer.Text.ToString();

            // Alle UI-Elemente unsichtbar machen, damit das Drucken sauber verläuft.
            VisibleFalse();

            // Fokus von allen Steuerelementen entfernen (kein Cursor-Strich auf dem Druck).
            this.ActiveControl = null;

            // PrintDialog: zeigt das Standard-Druckerauswahl-Fenster.
            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Druck starten → ruft im Hintergrund das PrintPage-Event auf.
                printDocument.Print();

                BtnClose.Visible = true;

                // " Stk." wieder aus der Menge entfernen, falls sie nochmals editiert werden soll.
                if (!string.IsNullOrEmpty(TxtboxMenge.Text) && TxtboxMenge.Text.Contains(" Stk."))
                {
                    TxtboxMenge.Text = TxtboxMenge.Text.Replace(" Stk.", "");
                }
            }
            else
            {
                // Druck abgebrochen → Schließen + Drucken-Button wieder einblenden, dann zurück.
                BtnClose.Visible = true;
                BtnDrucken.Visible = true;
                return;
            }

            // Nach dem Drucken: alle UI-Elemente wieder sichtbar machen.
            VisibleTrue();

            // Bild aus der oberen PictureBox in die Zwischenablage legen → wird später von Excel eingefügt.
            CopyImageToClipboard();

            // Excel-Datei (Chargenbegleitblatt) mit den Daten füllen + drucken.
            CopyDataToExcel(@"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Chargenbegleitblatt.xlsx");
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Blendet alle relevanten UI-Steuerelemente aus (für den "Druckmodus" der Form).
        // -----------------------------------------------------------------------------------------------------------------
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

        // -----------------------------------------------------------------------------------------------------------------
        // Setzt alle UI-Steuerelemente nach dem Drucken wieder sichtbar.
        // -----------------------------------------------------------------------------------------------------------------
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

        // Schließt das Formular.
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wird aufgerufen, sobald in der Artikel-ComboBox ein Eintrag gewählt wird.
        // Trennt den Anzeige-String "ARTNR / Seite: SEITE" und lädt die Detaildaten dazu.
        // -----------------------------------------------------------------------------------------------------------------
        private void ComboboxArtikel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = ComboboxArtikel.SelectedItem.ToString();

            // Split mit String-Separator: zerlegt am Trennzeichen " / Seite: ".
            string[] parts = selectedValue.Split(new[] { " / Seite: " }, StringSplitOptions.None);

            if (parts.Length == 2)
            {
                string artikelNummer = parts[0];
                string seite = parts[1];

                LoadAdditionalInfo(artikelNummer, seite);
                lblZuzerstAuswaehlen.Visible = false; // Hinweis "Bitte zuerst auswählen" ausblenden
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie einen gültigen Artikel.");
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Holt für die übergebene Artikelnummer + Seite alle Daten aus der DB
        // und schreibt sie in die Felder + in die Klassenvariablen.
        // -----------------------------------------------------------------------------------------------------------------
        private void LoadAdditionalInfo(string artikelNummer, string seite)
        {
            sqlConnectionVerwaltung.Open();

            // SELECT * holt alle Spalten – praktisch hier, weil wir viele Werte brauchen.
            string query = @"
                SELECT *
                FROM Serienlinsen
                WHERE ARTNR = @artikelNummer AND SEITE = @seite";

            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Parameter sicher übergeben (SQL-Injection-Schutz).
                    sqlCommand.Parameters.AddWithValue("@artikelNummer", artikelNummer);
                    sqlCommand.Parameters.AddWithValue("@seite", seite);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // ----- Felder in die UI-Steuerelemente schreiben -----
                            txtboxBezeichnung.Text = reader["BEZ"].ToString();
                            txtboxBelag.Text = reader["VERGBELAG"].ToString();
                            txtboxProzess.Text = reader["MATERIAL"].ToString();
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

                            // ----- Werte zusätzlich in Klassenvariablen ablegen (für Druck/Excel-Export) -----
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

                            // ----- Bilder anhand der Pfade aus der DB laden -----
                            string bildPfadInfoZeichnung = reader["Zeichnungspfad"].ToString();
                            string bildPfadInfoZeichnungBemerkung = reader["InfoZeichnung"].ToString();

                            // Bild "Auflegen" – nur laden, wenn Pfad existiert und Datei vorhanden ist.
                            if (!string.IsNullOrEmpty(bildPfadInfoZeichnung) && System.IO.File.Exists(bildPfadInfoZeichnung))
                            {
                                PictureboxAuflegenLinsenPrismen.Image = Image.FromFile(bildPfadInfoZeichnung);
                            }
                            else
                            {
                                MessageBox.Show("Das Bild konnte nicht gefunden werden.");
                                PictureboxAuflegenLinsenPrismen.Image = null;
                            }

                            // Bild "Zusatzinfo" – analog.
                            if (!string.IsNullOrEmpty(bildPfadInfoZeichnungBemerkung) && System.IO.File.Exists(bildPfadInfoZeichnungBemerkung))
                            {
                                PictureboxZusatzinfo.Image = Image.FromFile(bildPfadInfoZeichnungBemerkung);
                            }
                            else
                            {
                                MessageBox.Show("Das Bild InfoZeichnungBemerkungen konnte nicht gefunden werden.");
                                PictureboxAuflegenLinsenPrismen.Image = null; // (im Original wird hier dieselbe PB zurückgesetzt)
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

        // -----------------------------------------------------------------------------------------------------------------
        // Setzt für ausgewählte Eingabefelder graue Beispiel-Texte (Platzhalter / "Wasserzeichen").
        // -----------------------------------------------------------------------------------------------------------------
        private void SetPlaceholders()
        {
            SetPlaceholder(txtboxAuftragsnummer, "XXXXXXXXXXX");
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Generischer Helfer:
        // - schreibt graunen Beispieltext in eine TextBox
        // - beim Hineinklicken (GotFocus) wird er gelöscht
        // - beim Verlassen leerer Box (LostFocus) wird er wieder gesetzt
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
                    textBox.ForeColor = SystemColors.WindowText; // schwarze Standardfarbe
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

        // -----------------------------------------------------------------------------------------------------------------
        // Bildschärfung mit einem 3x3-Faltungsfilter (Kernel).
        // Pixel für Pixel werden die Nachbarn gewichtet zusammengezählt → schärferes Ergebnis.
        // ACHTUNG: GetPixel/SetPixel ist langsam, aber für gelegentliche Drucke akzeptabel.
        // -----------------------------------------------------------------------------------------------------------------
        private Bitmap SharpenImage(Bitmap image)
        {
            Bitmap sharpenedImage = new Bitmap(image.Width, image.Height);

            // Schärfungs-Kernel (3x3). Mitte = 1,8 (verstärkt sich selbst), Nachbarn = -0,2 (ziehen ab).
            float[,] kernel = new float[,]
            {
                { 0,    -0.2f,  0    },
                { -0.2f, 1.8f, -0.2f },
                { 0,    -0.2f,  0    }
            };

            // Über jedes Pixel laufen (außer Randpixel, die haben keine vollen Nachbarn).
            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    float r = 0, g = 0, b = 0;

                    // 3x3-Nachbarschaft durchlaufen.
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = image.GetPixel(x + i, y + j);

                            // Pro Farbkanal mit dem Kernel-Gewicht multiplizieren und aufsummieren.
                            r += pixel.R * kernel[i + 1, j + 1];
                            g += pixel.G * kernel[i + 1, j + 1];
                            b += pixel.B * kernel[i + 1, j + 1];
                        }
                    }

                    // Werte in den gültigen Farbbereich [0..255] zwingen.
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    // Neues Pixel setzen.
                    Color newColor = Color.FromArgb((int)r, (int)g, (int)b);
                    sharpenedImage.SetPixel(x, y, newColor);
                }
            }

            return sharpenedImage;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Kopiert das Bild aus PictureboxAuflegenLinsenPrismen in die System-Zwischenablage.
        // (Wird später in Excel als Bild eingefügt.)
        // -----------------------------------------------------------------------------------------------------------------
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

        // -----------------------------------------------------------------------------------------------------------------
        // Öffnet die Excel-Datei "Chargenbegleitblatt", trägt die Werte in vorgegebene Zellen ein,
        // fügt das Bild aus der Zwischenablage in zwei Zellen ein, druckt die erste Seite und schließt Excel sauber.
        // -----------------------------------------------------------------------------------------------------------------
        public void CopyDataToExcel(string filePath)
        {
            // Dictionary "Zelle -> Wert": dadurch kompakter Code und einfache Erweiterbarkeit.
            var cellAssignments = new System.Collections.Generic.Dictionary<string, string>
            {
                { "H1", Auftragsnummer },        { "H24", Auftragsnummer },
                { "D2", artikel },               { "D25", artikel },
                { "F2", bezeichnung },           { "F25", bezeichnung },
                { "D3", seiteArtikel },          { "D26", seiteArtikel },
                { "C8", belag },                 { "C31", belag },
                { "E8", prozess },               { "E31", prozess },
                { "G11", radiusVerguetung },     { "G34", radiusVerguetung },
                { "G5", radiusRueckseite },      { "G28", radiusRueckseite },
                { "I14", gNummer },              { "I37", gNummer },
                { "I15", glassorte },            { "I38", glassorte },
                { "I16", durchmesser + " mm" },  { "I39", durchmesser + " mm" },
                { "I17", dicke + " mm" },        { "I40", dicke + " mm" },
                { "B15", bemerkung },            { "B38", bemerkung },
            };

            // COM-Interop-Objekte für Excel.
            // Diese MÜSSEN am Ende mit Marshal.ReleaseComObject freigegeben werden,
            // sonst bleibt EXCEL.EXE im Hintergrund laufen.
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

                // Vorlage öffnen.
                workbook = excelApp.Workbooks.Open(filePath);
                excelApp.Visible = true;

                // Erstes Tabellenblatt holen (Index 1, NICHT 0!).
                worksheet = (Excel.Worksheet)workbook.Sheets[1];

                // Alle Werte aus dem Dictionary in die Zellen schreiben.
                foreach (var assignment in cellAssignments)
                {
                    Console.WriteLine($"Schreibe in Zelle {assignment.Key}: {assignment.Value}");
                    worksheet.Range[assignment.Key].Value = assignment.Value;
                }

                Console.WriteLine("Füge Bild aus der Zwischenablage ein...");

                // ----- Bild in Zelle G6 einfügen -----
                Excel.Range targetCell = worksheet.Cells[6, 7]; // Reihe 6, Spalte 7 = G6
                targetCell.Select();
                excelApp.ActiveSheet.Paste();

                // Letztes (= gerade eingefügtes) Shape holen und Größe setzen.
                var picture = worksheet.Shapes.Item(worksheet.Shapes.Count);
                picture.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue; // Seitenverhältnis halten
                picture.Width = 140;
                picture.Height = 90;

                // ----- Dasselbe Bild zusätzlich in Zelle G29 einfügen -----
                Excel.Range targetCell1 = worksheet.Cells[29, 7]; // G29
                targetCell1.Select();
                excelApp.ActiveSheet.Paste();

                var picture1 = worksheet.Shapes.Item(worksheet.Shapes.Count);
                picture1.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
                picture1.Width = 140;
                picture1.Height = 90;

                // Erste Seite drucken (von Seite 1 bis Seite 1).
                Console.WriteLine("Drucke die erste Seite der Excel-Datei...");
                worksheet.PrintOut(From: 1, To: 1);

                // Kurz warten, damit der Druckauftrag intern verarbeitet werden kann.
                System.Threading.Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
            }
            finally
            {
                // ----- Excel-Objekte sauber schließen und freigeben -----
                try
                {
                    if (workbook != null)
                    {
                        workbook.Close(false); // false = Änderungen NICHT speichern
                        Marshal.ReleaseComObject(workbook);

                        // Garbage Collector zwingen, Speicher freizugeben.
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    if (excelApp != null)
                    {
                        excelApp.Quit();
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
                    // Referenzen entfernen, damit die GC sie aufräumen kann.
                    worksheet = null;
                    workbook = null;
                    excelApp = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            // Sicherheitshalber nochmal aufräumen.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
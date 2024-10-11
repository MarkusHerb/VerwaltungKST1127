using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    public partial class Form_PrototypenauftragErstellen : Form
    {
        // Verbindungszeichenfolgen für die SQL Server-Datenbanken
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private PrintDocument printDocument;  // Deklarieren eines PrintDocument-Objekts für den Druckprozess

        public Form_PrototypenauftragErstellen()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Entfernt die Titelleiste und die Rahmen des Formulars
            UpdateZeitDatum();
            //FillComboBoxArtikel();
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
            sqlConnectionVerwaltung.Open();
            string query = @"
                SELECT DISTINCT ARTNR, SEITE
                FROM Serienlinsen
                ORDER BY ARTNR ASC";

            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        ComboboxArtikel.Items.Clear();

                        while (reader.Read())
                        {
                            string artikelNummerValue = reader["ARTNR"].ToString();
                            string seiteValue = reader["SEITE"].ToString();
                            string displayValue = $"{artikelNummerValue} / Seite: {seiteValue}";
                            ComboboxArtikel.Items.Add(displayValue);
                        }
                    }
                }

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

        // Uhrzeit und Datumsfunktion
        private void UpdateZeitDatum()
        {
            DateTime aktuell = DateTime.Now;
            LblErstelltAm.Text = aktuell.ToString("D");
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
            int marginRight = 40;   // Rechter Rand
            int marginTop = 20;     // Oberer Rand
            int marginBottom = 40;  // Unterer Rand

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
            Font fontAuftragsnummer = new Font("Arial", 15); // Schriftart für txtboxAuftragsnummer
            Font fontBezeichnungBold = new Font("Arial", 15, FontStyle.Bold); // Schriftart für mehrere elemente
            Font fontBezeichnung = new Font("Arial", 15); // Schriftart für mehrere elemente

            float xProjektNr = 39;
            float yProjektNr = 85;
            float xAuftragsnummer = 350;
            float yAuftragsnummer = 90;
            float xArtikelnummerLabel = 39;
            float yArtikelnummerLabel = 125;
            float xArtikelnummerBox = 185;
            float yArtikelnummerBox = 125;
            float xBezeichnungLabel = 39;
            float yBezeichnungLabel = 175;
            float xBezeichnungBox = 185;
            float yBezeichnungerBox = 175;
            float xMengeLabel = 39;
            float yMengeLabel = 210;
            float xMengeBox = 185;
            float yMengeBox = 210;
            float xBelagLabel = 39;
            float yBelagLabel = 245;
            float xBelagBox = 185;
            float yBelagBox = 245;
            float xProzessLabel = 350;
            float yProzessLabel = 245;
            float xProzessBox = 496;
            float yProzessBox = 245;

            // Texte zeichnen
            e.Graphics.DrawString(lblNrProjekt.Text, fontProjektNr, Brushes.Black, xProjektNr, yProjektNr);
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

            //////////////////////////////////////////Test

            // Aufräumen
            bmp.Dispose();
            sharpenedBmp.Dispose(); // Freigabe der scharfen Bitmap
                                    // Temporäre Datei löschen
            File.Delete(tempPngPath);
        }

        // Wenn auf den Button Drucken gedrückt wird
        private void BtnDrucken_Click_1(object sender, EventArgs e)
        {
            // ##########Alte Texte in Variablen speichern damit man eine zweite seite bei bedarf drucken kann
            string projektNrText = lblNrProjekt.Text;
            string auftragsnummerText = txtboxAuftragsnummer.Text;

            // Setze die Texte für den Druck
            ComboboxArtikel.Text = "PR " + ComboboxArtikel.Text;
            TxtboxMenge.Text = TxtboxMenge.Text + " Stk.";

            // Elemente unsichtbar machen
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
            }
            else
            {
                BtnClose.Visible = true;
                BtnDrucken.Visible = true;
            }

            // Optional: Setze die Sichtbarkeit der Elemente zurück, wenn der Druck abgeschlossen ist
            lblNrProjekt.Visible = true;
            txtboxAuftragsnummer.Visible = true;
            lblArtikelnummer.Visible = true;
            ComboboxArtikel.Visible = true;
            lblBezeichnung.Visible = true;
            txtboxBezeichnung.Visible = true;
            lblMenge.Visible = true;
            TxtboxMenge.Visible=true;
            lblBelag.Visible = true;
            txtboxBelag.Visible = true;
            lblProzess.Visible = true;
            txtboxProzess.Visible = true;
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
                            richtxtboxInforamationAuflegen.Text = reader["BEMERKUNG"].ToString();
                            richtxtboxZusatzinfo.Text = reader["InfoZeichnung_Bemerkungen"].ToString();
                            txtboxVorreinigung.Text = reader["Vorreinigung"].ToString();
                            txtboxHandreinigung.Text = reader["Handreinigung"].ToString();
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

        private Bitmap SharpenImage(Bitmap image)
        {
            Bitmap sharpenedImage = new Bitmap(image.Width, image.Height);

            // Definieren eines milderen Schärfungsfilters
            float[,] kernel = new float[,]
            {
                { 0, -0.2f, 0 },
                { -0.2f, 1.8f, -0.2f },
                { 0, -0.2f, 0 }
            };

            // Wenden des Schärfungsfilters auf das Bild an
            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    float r = 0, g = 0, b = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = image.GetPixel(x + i, y + j);
                            r += pixel.R * kernel[i + 1, j + 1];
                            g += pixel.G * kernel[i + 1, j + 1];
                            b += pixel.B * kernel[i + 1, j + 1];
                        }
                    }

                    // Normalisieren der RGB-Werte
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    // Setze den neuen Farbwert in den scharfen Bitmap
                    Color newColor = Color.FromArgb((int)r, (int)g, (int)b);
                    sharpenedImage.SetPixel(x, y, newColor);
                }
            }

            return sharpenedImage;
        }

    }
}

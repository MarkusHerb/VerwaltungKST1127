using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Farben, Bilder, etc.)
using System.Drawing.Printing;  // Importieren des System.Drawing.Printing-Namespace für Druckfunktionalitäten (ermöglicht das Drucken in .NET-Anwendungen)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für Windows Forms-Steuerelemente und Benutzeroberflächen (UI)
using System.IO; // Importieren des System.IO-Namespace für Dateisystemoperationen (z.B. das Arbeiten mit Dateien und Verzeichnissen)

namespace VerwaltungKST1127.Farbauswertung
{
    public partial class Form_Messbild : Form
    {
        // Deklarieren eines PrintDocument-Objekts für den Druckprozess
        private PrintDocument printDocument;

        public Form_Messbild(string dateValue, string chargValue, string XiValue, string XaValue, string YiValue,
            string YaValue, string ZiValue, string ZaValue, string belag, string process, string anlage)

        {
            InitializeComponent();

            // Anzeigen der ausgewählten Charge und weiterer Parameter
            LblAnzeigeMessbild.Text = "Ausgewählte Charge: " + dateValue + "-" + chargValue;
            LblBelagProcess.Text = belag + " - " + process;
            LblAnlage.Text = "Anlage " + anlage;
            Lbl_X_I.Text = "Xi:  " + XiValue + "%"; Lbl_X_A.Text = "Xa: " + XaValue + "%";
            Lbl_Y_I.Text = "Yi:  " + YiValue + "%"; Lbl_Y_A.Text = "Ya: " + YaValue + "%";
            Lbl_Z_I.Text = "Zi:  " + ZiValue + "%"; Lbl_Z_A.Text = "Za: " + ZaValue + "%";

            // Farbgestaltung der Label: Blau für die I-Werte, Rot für die A-Werte
            Lbl_X_I.ForeColor = Color.Blue;
            Lbl_Y_I.ForeColor = Color.Blue;
            Lbl_Z_I.ForeColor = Color.Blue;
            Lbl_X_A.ForeColor = Color.Red;
            Lbl_Y_A.ForeColor = Color.Red;
            Lbl_Z_A.ForeColor = Color.Red;

            // Bildanzeige basierend auf Datum und Charge
            UpdatePictureBox(dateValue, chargValue);

            // Initialisieren des PrintDocument-Objekts und Hinzufügen eines Event-Handlers für den Druckvorgang
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            // Querformat aktivieren
            printDocument.DefaultPageSettings.Landscape = true;
        }

        // Methode, um das Bild in die PictureBox zu laden, basierend auf dem Datum und der Charge
        private void UpdatePictureBox(string dateValue, string chargValue)
        {
            // Versuche, das Datum zu parsen und es in das gewünschte Format zu konvertieren
            if (DateTime.TryParse(dateValue, out DateTime parsedDate))
            {
                // Formatierung des Datums für die Bilddatei: "MMddyy"
                string formattedDate = parsedDate.ToString("ddMMyy");
                string formattedDay = parsedDate.Day.ToString("00"); // Tag mit führender Null
                string formattedMonth = parsedDate.Month.ToString("00"); // Monat mit führender Null
                string formattedYear = parsedDate.ToString("yy"); // Nur die letzten zwei Ziffern des Jahres
                formattedDate = formattedYear + formattedMonth + formattedDay; // Datum umformatieren

                // Bildname erstellen basierend auf Datum und Charge
                string imageName = $"{formattedDate}-{chargValue}.png";
                string imagePath = Path.Combine(@"P:\Messdata\UV\MessBilder", imageName); // Bildpfad erstellen

                // Überprüfen, ob das Bild existiert, und es in die PictureBox laden
                if (File.Exists(imagePath))
                {
                    pictureBoxMessung.Image = Image.FromFile(imagePath); // Bild laden
                }
                else
                {
                    // Fehlermeldung anzeigen, wenn das Bild nicht gefunden wird
                    MessageBox.Show("Das Bild konnte nicht gefunden werden: " + imagePath, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    pictureBoxMessung.Image = null; // Leere PictureBox, falls kein Bild vorhanden ist
                }
            }
            else
            {
                // Fehlermeldung anzeigen, wenn das Datum ungültig ist
                MessageBox.Show("Ungültiges Datum: " + dateValue, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event-Handler für den Druck-Button
        private void BtnDrucken_Click(object sender, EventArgs e)
        {
            BtnDrucken.Visible = false;
            // Erstellen und Anzeigen des PrintDialogs, damit der Benutzer einen Drucker auswählen kann
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument; // Verknüpfen des PrintDocuments mit dem Dialog

            // Überprüfen, ob der Benutzer im Dialog auf "OK" klickt
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Druckvorgang starten
                printDocument.Print();
            }
            BtnDrucken.Visible = true;
        }

        // Event-Handler für das PrintPage-Ereignis, um das Formular zu drucken
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {

            // Erstellen eines Bitmaps, das die aktuelle Größe des Formulars widerspiegelt
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height)); // Das Formular auf das Bitmap rendern

            // Rand in Punkt (ca. 1,5 cm = 42,5 Punkte)
            int margin = 43; // 1,5 cm auf jeder Seite

            // Berechnung der Druckbreite und Druckhöhe mit Rand
            int printWidth = e.PageBounds.Width - 2 * margin;  // Breite des Druckbereichs unter Berücksichtigung des Randes
            int printHeight = e.PageBounds.Height - 2 * margin; // Höhe des Druckbereichs unter Berücksichtigung des Randes

            // Position für das Bild (unter Berücksichtigung des Randes)
            int posX = margin;
            int posY = margin;

            // Bild proportional skalieren
            float ratioX = (float)printWidth / (float)bmp.Width;
            float ratioY = (float)printHeight / (float)bmp.Height;
            float ratio = Math.Min(ratioX, ratioY); // Kleineren Skalierungsfaktor verwenden, um das Bild proportional zu skalieren

            // Neue Breite und Höhe für das skalierte Bild berechnen
            int scaledWidth = (int)(bmp.Width * ratio);
            int scaledHeight = (int)(bmp.Height * ratio * 1.3);

            // Das Bild proportional und mit Rand auf der Druckseite platzieren
            e.Graphics.DrawImage(bmp, posX, posY, scaledWidth, scaledHeight);
        }
    }
}

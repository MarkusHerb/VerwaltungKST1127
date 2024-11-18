using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace VerwaltungKST1127.Farbauswertung
{
    // Deklarieren eines PrintDocument-Objekts für den Druckprozess
    public partial class Form_Monatsuebersicht : Form
    {
        private PrintDocument printDocument;

        public object LabelPosition { get; private set; }

        public Form_Monatsuebersicht()
        {
            InitializeComponent();
            // Initialisieren des PrintDocument-Objekts und Registrieren des PrintPage-Event-Handlers
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);  // Event-Handler für die Druckseite hinzufügen

            // Querformat für das PrintDocument festlegen
            printDocument.DefaultPageSettings.Landscape = true;  // Querformat aktivieren
            // Ramen des formulars ausbleden
            ////this.FormBorderStyle = FormBorderStyle.None;            
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
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
            int scaledHeight = (int)(bmp.Height * ratio);

            // Das Bild proportional und mit Rand auf der Druckseite platzieren
            e.Graphics.DrawImage(bmp, posX, posY, scaledWidth, scaledHeight);
        }

        // Event-Handler für den Druck-Button
        private void BtnDrucken_Click(object sender, EventArgs e)
        {
            BtnDrucken.Visible = false; // Druck-Button ausblenden
            this.FormBorderStyle = FormBorderStyle.None; // Entfernt die Titelleiste und die Rahmen des Formulars
            // PrintDialog anzeigen, um den Benutzer einen Drucker auswählen zu lassen
            PrintDialog printDialog = new PrintDialog(); // Erstellen eines PrintDialogs
            printDialog.Document = printDocument; // Zuweisen des PrintDocuments zum PrintDialog

            // Überprüfen, ob der Benutzer im PrintDialog auf "OK" klickt
            if (printDialog.ShowDialog() == DialogResult.OK)
            {               
                // Diagramm drucken
                printDocument.Print(); // Starten des Druckprozesses
            }
            else
            {
                BtnDrucken.Visible = true;
                this.FormBorderStyle = FormBorderStyle.Sizable; // Ramen wieder einblenden 
            }
        }

        // Daten für die Diagramme einlesen
        public void SetDataFromInputForm(DataTable data)
        {
            // --- Diagramm: Monatsübersicht ---

            // Löschen der vorhandenen Datenpunkte aus dem Monatsübersicht-Diagramm
            ChartMonatsuebersicht.Series[0].Points.Clear();

            // Sortieren der Daten nach dem Datum in aufsteigender Reihenfolge
            DataView dv = data.DefaultView;
            dv.Sort = "Datum ASC";  // Sortieren der Daten nach Datum in aufsteigender Reihenfolge
            DataTable sortedData = dv.ToTable();  // Sortierte Daten als DataTable speichern

            // Ermitteln des frühesten und spätesten Datums in den Daten
            DateTime minDate = sortedData.AsEnumerable()
                                         .Min(row => Convert.ToDateTime(row["Datum"]));
            DateTime maxDate = sortedData.AsEnumerable()
                                         .Max(row => Convert.ToDateTime(row["Datum"]));

            // Aktualisieren des Labels lblEingelesenAb mit dem minDate
            lblEingelesenAb.Text = $"Eingelesen ab: {minDate:dd.MM.yyyy}";

            // Schleife über alle Monate zwischen dem frühesten und spätesten Datum
            for (DateTime date = new DateTime(minDate.Year, minDate.Month, 1);
                 date <= new DateTime(maxDate.Year, maxDate.Month, 1);
                 date = date.AddMonths(1))
            {
                string monthYear = date.ToString("MM.yy");  // Monat und Jahr im Format MM.yy

                // Zählen der Datensätze für den aktuellen Monat
                int count = sortedData.AsEnumerable()
                                      .Count(row => Convert.ToDateTime(row["Datum"]).Year == date.Year && Convert.ToDateTime(row["Datum"]).Month == date.Month);

                // Hinzufügen des Datenpunkts zum Diagramm
                ChartMonatsuebersicht.Series[0].Points.AddXY(monthYear, count);

                // Setzen der Beschriftung für jeden Balken
                var point = ChartMonatsuebersicht.Series[0].Points.Last();
                point.Label = count.ToString();  // Beschriftung auf den Wert setzen

                // Farbliche Gestaltung basierend auf dem Wert
                if (count > 650)
                {
                    point.Color = Color.LightGreen; // Hoher Wert: Grün
                }
                else if (count > 350)
                {
                    point.Color = Color.Orange; // Mittelwert: Orange
                }
                else
                {
                    point.Color = Color.OrangeRed; // Niedriger Wert: Rot
                }

                // Tooltip hinzufügen
                point.ToolTip = $"Monat: {monthYear}\nWert: {count}";
            }

            // Setzen des Intervalls auf 1, damit jeder Monat auf der X-Achse angezeigt wird
            ChartMonatsuebersicht.ChartAreas[0].AxisX.Interval = 1;

            // Formatieren der X-Achsenbeschriftung
            ChartMonatsuebersicht.ChartAreas[0].AxisX.LabelStyle.Format = "MM.yy";

            // Entfernen der Gitternetzlinien für X- und Y-Achse in der Monatsübersicht
            ChartMonatsuebersicht.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            ChartMonatsuebersicht.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            // Schriftgröße der Beschriftungen über den Balken anpassen
            foreach (var series in ChartMonatsuebersicht.Series)
            {
                series.LabelForeColor = Color.Black;  // Optional: Farbe der Beschriftungen
                series.Font = new Font("Arial", 8, FontStyle.Bold);  // Schriftart und -größe für die Balkenbeschriftungen
            }

            // Umbenennen der Series[0] in "Monate"
            ChartMonatsuebersicht.Series[0].Name = "Monate";

            // Berechnung des Durchschnitts der Chargen pro Monat
            int totalCount = ChartMonatsuebersicht.Series[0].Points.Sum(point => (int)point.YValues[0]);  // Summe aller Y-Werte (Chargen)
            int numberOfMonths = ChartMonatsuebersicht.Series[0].Points.Count;  // Anzahl der Monate
            double average = (double)totalCount / numberOfMonths;  // Durchschnitt berechnen

            // Setzen des Durchschnitts in das Label
            LblAverage.Text = $"Ø Chargen/Monat: {average:F2}";  // Durchschnitt mit 2 Dezimalstellen anzeigen

            // --- Diagramm: Belagsübersicht ---

            // Erstellen eines Wörterbuchs, um die Anzahl der Vorkommen jeder Belag-Nummer zu speichern
            Dictionary<string, int> belagCounts = new Dictionary<string, int>();

            // Durchlaufen aller Zeilen im DataTable
            foreach (DataRow row in data.Rows)
            {
                // Überprüfen, ob die Spalte "Belag" existiert und einen Wert enthält
                if (row.Table.Columns.Contains("Belag") && row["Belag"] != DBNull.Value)
                {
                    string belagNummer = row["Belag"].ToString();

                    // Zählen der Vorkommen jeder Belag-Nummer
                    if (belagCounts.ContainsKey(belagNummer))
                    {
                        belagCounts[belagNummer]++;
                    }
                    else
                    {
                        belagCounts[belagNummer] = 1;
                    }
                }
            }

            // Sortieren des Wörterbuchs nach den Werten (absteigend)
            var sortedBelagCounts = belagCounts.OrderByDescending(entry => entry.Value);

            // Bereinigen der vorhandenen Daten im Belagsübersicht-Diagramm
            ChartBelagsuebersicht.Series[0].Points.Clear();

            // Hinzufügen der gesammelten und sortierten Daten zum Diagramm
            foreach (var entry in sortedBelagCounts)
            {
                // Hinzufügen des Datenpunkts zum Diagramm (Belag-Nummer, Anzahl)
                ChartBelagsuebersicht.Series[0].Points.AddXY(entry.Key, entry.Value);

                // Beschriftung des Balkens mit der Anzahl
                var point = ChartBelagsuebersicht.Series[0].Points.Last();
                point.Label = entry.Value.ToString();  // Beschriftung auf den Wert setzen

                // Tooltip hinzufügen
                point.ToolTip = $"Belag: {entry.Key}\nAnzahl: {entry.Value}";
            }

            // Optionale Anpassung der X-Achse, damit alle "Belag"-Nummern korrekt angezeigt werden
            ChartBelagsuebersicht.ChartAreas[0].AxisX.Interval = 1;

            // Entfernen der Gitternetzlinien im Chart
            ChartBelagsuebersicht.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            ChartBelagsuebersicht.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;

            // Schriftgröße der Beschriftungen über den Balken anpassen
            foreach (var series in ChartBelagsuebersicht.Series)
            {
                series.LabelForeColor = Color.Black;  // Optional: Farbe der Beschriftungen
                series.Font = new Font("Arial", 8, FontStyle.Bold);  // Schriftart und -größe für die Balkenbeschriftungen
            }

            // Entfernen der Umrandungen der Balken (keine Linien)
            ChartBelagsuebersicht.Series[0].BorderWidth = 0;

            // Umbenennen der Series[0] in "Monate"
            ChartBelagsuebersicht.Series[0].Name = "Belagsname";
        }
    }
}

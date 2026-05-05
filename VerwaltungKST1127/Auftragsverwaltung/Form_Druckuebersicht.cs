// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. nicht "System.Windows.Forms.Form" schreiben müssen, sondern einfach "Form".
// ===================================================================================================
using System;                              // Basis-Typen (string, DateTime, Math, Exception ...)
using System.Collections.Generic;          // Sammlungen wie List<T>, Dictionary<TKey, TValue>
using System.Data;                         // DataTable, DataSet, ConnectionState (ADO.NET-Kern)
using System.Data.SqlClient;               // SQL-Server-Zugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Drawing;                      // Farben, Schriften, Bilder (Image, Color, Font ...)
using System.Drawing.Imaging;              // Bildformate (ImageFormat.Png ...)
using System.IO;                           // Datei- und Pfadoperationen (File, Path, ...)
using System.Runtime.InteropServices;      // Interop mit Excel (Marshal.ReleaseComObject)
using System.Text.Json;                    // JSON serialisieren/deserialisieren
using System.Windows.Forms;                // Windows-Forms (Form, Label, Button, MessageBox ...)
using ZXing;                               // Barcode-Erstellung (Bibliothek)
using Excel = Microsoft.Office.Interop.Excel; // Alias "Excel" für den Excel-COM-Namespace (kürzere Schreibweise)

// "namespace" gruppiert Klassen logisch und vermeidet Namenskonflikte mit anderen Projekten.
namespace VerwaltungKST1127.Auftragsverwaltung
{
    // "public partial class": öffentliche Klasse, deren Code auf mehrere Dateien aufgeteilt sein kann
    // (z. B. Form_Druckuebersicht.cs + Form_Druckuebersicht.Designer.cs).
    // Sie erbt von "Form" und ist damit ein Windows-Fenster.
    public partial class Form_Druckuebersicht : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Verbindung zur SQL-Server-Datenbank.
        // "private"  = nur innerhalb dieser Klasse sichtbar.
        // "readonly" = darf nur einmal (hier oder im Konstruktor) zugewiesen werden.
        // Der String enthält Server, Datenbankname und Authentifizierungs-Infos ("Connection-String").
        // -----------------------------------------------------------------------------------------------------------------
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        // -----------------------------------------------------------------------------------------------------------------
        // Klassenvariablen ("Felder"): speichern die im Konstruktor übergebenen Werte,
        // damit auch andere Methoden in dieser Klasse darauf zugreifen können.
        // -----------------------------------------------------------------------------------------------------------------
        private string enddatum;
        private string teilebez;
        private string auftragsNr;
        private string artikel;
        private string status;
        private string avoInfo;
        private string material;
        private string seite;
        private string sollStk;
        private string istStk;
        private string vorStk;
        private string teilelager;
        private string bereitstell;
        private string jahresbedarf;
        private string zukauf;
        private string dringend;
        private string aktualisiert;
        private string selectedBelagValue;

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft automatisch beim "new Form_Druckuebersicht(...)".
        // Hier werden die übergebenen Parameter in die Klassenfelder gespeichert
        // und die Labels initial befüllt.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_Druckuebersicht(string enddatum, string teilebez, string auftragsNr, string artikel, string status, string avoInfo, string material, string seite,
            string sollStk, string istStk, string vorStk, string teilelager, string bereitstell, string jahresbedarf, string zukauf, string dringend, string aktualisiert,
            string selectedBelagValue)
        {
            InitializeComponent(); // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)

            // ----- Übergebene Werte in die Klassenfelder speichern (this.* = das Feld der Klasse) -----
            this.enddatum = enddatum;
            this.teilebez = teilebez;
            this.auftragsNr = auftragsNr;
            this.artikel = artikel;
            this.status = status;
            this.avoInfo = avoInfo;
            this.material = material;
            this.seite = seite;
            this.sollStk = sollStk;
            this.istStk = istStk;
            this.vorStk = vorStk;
            this.teilelager = teilelager;
            this.bereitstell = bereitstell;
            this.jahresbedarf = jahresbedarf;
            this.zukauf = zukauf;
            this.dringend = dringend;
            this.aktualisiert = aktualisiert;
            this.selectedBelagValue = selectedBelagValue;

            // ########### Werte in die Labels schreiben ###########

            // Auftragsnummer + Artikel + Teilebezeichnung in eine einzige Zeile.
            // "+" verbindet (konkateniert) Strings.
            lblAuftragNummerBauteilname.Text = auftragsNr + " - " + artikel + " - " + teilebez;

            // Sollstück anzeigen.
            lblSollstueck.Text = "Sollstück: " + sollStk + "Stk.";

            // Zukauf-Logik: je nach Wert anderer Text und andere Schriftfarbe.
            if (zukauf == "R-Lager")
            {
                lblZukauf.Text = "Zukauf (Rohteilelager)";
                lblZukauf.ForeColor = Color.DarkOliveGreen;
            }
            else if (zukauf == "T-Lager")
            {
                lblZukauf.Text = "Zukauf (Teilelager)";
                lblZukauf.ForeColor = Color.DarkOrange;
            }
            else
            {
                // Standardfall (kein Zukauf hinterlegt) → Eigenfertigung.
                lblZukauf.Text = "Eigenfertigung";
            }

            // Belag + Material kombinieren ("B123 - GlasXY") und rot einfärben.
            lblProzess.Text = selectedBelagValue + " - " + material;
            lblProzess.ForeColor = Color.Red;

            // Restliche Labels über DB-Abfrage befüllen.
            UpdateLabels();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Holt die Arbeitsvorgänge (Vorbereiten/Vergüten) aus der DB
        // und füllt die zugehörigen Labels (4 Plätze pro Vorgangstyp).
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateLabels()
        {
            // try/catch fängt Fehler ab; finally läuft IMMER (z. B. um Verbindungen zu schließen).
            try
            {
                // SQL-Abfrage mit Parameter (@Auftragsnummer) → schützt vor SQL-Injection.
                string query = @"
                    SELECT txta_Avoinfo, opno_avonr, refo_avonr
                    FROM LN_ProdOrders_PRD
                    WHERE pdno_prodnr = @Auftragsnummer";

                // "using" sorgt dafür, dass das SqlCommand am Ende automatisch korrekt aufgeräumt wird.
                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    // Wert für den Platzhalter @Auftragsnummer setzen.
                    command.Parameters.AddWithValue("@Auftragsnummer", auftragsNr);

                    sqlConnectionVerwaltung.Open(); // Datenbank-Verbindung öffnen

                    // Reader liest die Ergebniszeilen Stück für Stück.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Listen, in denen wir gefundene Werte sammeln (Vorbereiten und Vergüten getrennt).
                        List<string> vorbereitenList = new List<string>();
                        List<string> verguetenList = new List<string>();
                        List<string> positionVorbereitenList = new List<string>();
                        List<string> positionVerguetenList = new List<string>();
                        List<string> avoNrVorbereitenList = new List<string>();
                        List<string> avoNrVerguetenList = new List<string>();

                        // reader.Read() liefert true, solange noch eine weitere Zeile da ist.
                        while (reader.Read())
                        {
                            string avoInfo = reader["txta_Avoinfo"].ToString();
                            string position = reader["opno_avonr"].ToString();
                            string avoNr = reader["refo_avonr"].ToString();

                            // Heuristik: Wenn der Text "Vorbereiten" enthält → Vorbereiten-Liste.
                            if (avoInfo.Contains("Vorbereiten") || avoInfo.Contains("vorbereiten"))
                            {
                                vorbereitenList.Add(avoInfo);
                                positionVorbereitenList.Add(position);
                                avoNrVorbereitenList.Add(avoNr);
                            }

                            // Wenn der Text "Vergüten" enthält → Vergüten-Liste.
                            if (avoInfo.Contains("Vergüten") || avoInfo.Contains("vergüten"))
                            {
                                verguetenList.Add(avoInfo);
                                positionVerguetenList.Add(position);
                                avoNrVerguetenList.Add(avoNr);
                            }
                        }

                        // Die UI hat 4 Label-Plätze – also Arrays mit fester Länge 4.
                        string[] vorbereiten = new string[4];
                        string[] vergueten = new string[4];
                        string[] positionVorbereiten = new string[4];
                        string[] positionVergueten = new string[4];
                        string[] avoNrVorbereiten = new string[4];
                        string[] avoNrVergueten = new string[4];

                        // Erste bis zu 4 Vorbereiten-Werte aus der Liste in das Array kopieren.
                        for (int i = 0; i < vorbereitenList.Count && i < 4; i++)
                        {
                            vorbereiten[i] = vorbereitenList[i];
                            positionVorbereiten[i] = positionVorbereitenList[i];
                            avoNrVorbereiten[i] = avoNrVorbereitenList[i];
                        }

                        // Dasselbe für Vergüten.
                        for (int i = 0; i < verguetenList.Count && i < 4; i++)
                        {
                            vergueten[i] = verguetenList[i];
                            positionVergueten[i] = positionVerguetenList[i];
                            avoNrVergueten[i] = avoNrVerguetenList[i];
                        }

                        // ---- Labels mit den Array-Werten füllen ----
                        // "?? """ = wenn der Wert null ist, statt dessen leeren String verwenden.
                        lblArbeitsvorgangVorbereiten1.Text = vorbereiten[0] ?? "";
                        lblArbeitsvorgangVorbereiten2.Text = vorbereiten[1] ?? "";
                        lblArbeitsvorgangVorbereiten3.Text = vorbereiten[2] ?? "";
                        lblArbeitsvorgangVorbereiten4.Text = vorbereiten[3] ?? "";

                        lblArbeitsvorgangVergueten1.Text = vergueten[0] ?? "";
                        lblArbeitsvorgangVergueten2.Text = vergueten[1] ?? "";
                        lblArbeitsvorgangVergueten3.Text = vergueten[2] ?? "";
                        lblArbeitsvorgangVergueten4.Text = vergueten[3] ?? "";

                        lblPositionVorbereiten1.Text = positionVorbereiten[0] ?? "";
                        lblPositionVorbereiten2.Text = positionVorbereiten[1] ?? "";
                        lblPositionVorbereiten3.Text = positionVorbereiten[2] ?? "";
                        lblPositionVorbereiten4.Text = positionVorbereiten[3] ?? "";

                        lblPositionVergueten1.Text = positionVergueten[0] ?? "";
                        lblPositionVergueten2.Text = positionVergueten[1] ?? "";
                        lblPositionVergueten3.Text = positionVergueten[2] ?? "";
                        lblPositionVergueten4.Text = positionVergueten[3] ?? "";

                        lblAvoNrZuVorbereiten1.Text = avoNrVorbereiten[0] ?? "";
                        lblAvoNrZuVorbereiten2.Text = avoNrVorbereiten[1] ?? "";
                        lblAvoNrZuVorbereiten3.Text = avoNrVorbereiten[2] ?? "";
                        lblAvoNrZuVorbereiten4.Text = avoNrVorbereiten[3] ?? "";

                        lblAvoNrZuVergueten1.Text = avoNrVergueten[0] ?? "";
                        lblAvoNrZuVergueten2.Text = avoNrVergueten[1] ?? "";
                        lblAvoNrZuVergueten3.Text = avoNrVergueten[2] ?? "";
                        lblAvoNrZuVergueten4.Text = avoNrVergueten[3] ?? "";

                        // ---- Strichcode-Labels setzen ----
                        // Wenn das zugehörige Position-Label leer ist, bleibt auch der Strichcode leer.
                        // Sonst: "AuftragsNr" + "Position" → daraus wird später der Barcode.
                        // $"{x}{y}" = String-Interpolation (gleiche Wirkung wie x + y).

                        // Vorbereiten
                        lblStrichcodeZuPositionVorbereiten1.Text = string.IsNullOrEmpty(lblPositionVorbereiten1.Text) ? "" : $"{auftragsNr}{lblPositionVorbereiten1.Text}";
                        lblStrichcodeZuPositionVorbereiten2.Text = string.IsNullOrEmpty(lblPositionVorbereiten2.Text) ? "" : $"{auftragsNr}{lblPositionVorbereiten2.Text}";
                        lblStrichcodeZuPositionVorbereiten3.Text = string.IsNullOrEmpty(lblPositionVorbereiten3.Text) ? "" : $"{auftragsNr}{lblPositionVorbereiten3.Text}";
                        lblStrichcodeZuPositionVorbereiten4.Text = string.IsNullOrEmpty(lblPositionVorbereiten4.Text) ? "" : $"{auftragsNr}{lblPositionVorbereiten4.Text}";

                        // Vergüten
                        lblStrichcodeZuPositionVergueten1.Text = string.IsNullOrEmpty(lblPositionVergueten1.Text) ? "" : $"{auftragsNr}{lblPositionVergueten1.Text}";
                        lblStrichcodeZuPositionVergueten2.Text = string.IsNullOrEmpty(lblPositionVergueten2.Text) ? "" : $"{auftragsNr}{lblPositionVergueten2.Text}";
                        lblStrichcodeZuPositionVergueten3.Text = string.IsNullOrEmpty(lblPositionVergueten3.Text) ? "" : $"{auftragsNr}{lblPositionVergueten3.Text}";
                        lblStrichcodeZuPositionVergueten4.Text = string.IsNullOrEmpty(lblPositionVergueten4.Text) ? "" : $"{auftragsNr}{lblPositionVergueten4.Text}";
                    }
                    sqlConnectionVerwaltung.Close(); // Verbindung schließen
                }

                // Im Anschluss die Seite-Werte (1/2) zu jedem AvoNr-Label nachladen.
                UpdateSeiteLabels();
            }
            catch (Exception ex)
            {
                // ex.Message enthält die Fehlerbeschreibung.
                MessageBox.Show($"Fehler beim Abrufen der Daten: {ex.Message}");
            }
            finally
            {
                // Sicherheitsnetz: Verbindung schließen, falls sie noch offen ist.
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Ruft pro Vergüten-AvoNr-Label die zugehörige SEITE aus der DB ab.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateSeiteLabels()
        {
            try
            {
                // Nur wenn ein AvoNr-Label überhaupt einen Wert hat, holen wir die Seite.
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten1.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten1.Text, lblSeite1);
                }
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten2.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten2.Text, lblSeite2);
                }
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten3.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten3.Text, lblSeite3);
                }
                if (!string.IsNullOrEmpty(lblAvoNrZuVergueten4.Text))
                {
                    UpdateSeiteFürLabel(lblAvoNrZuVergueten4.Text, lblSeite4);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Aktualisieren der SEITE-Daten: {ex.Message}");
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Holt die SEITE aus der Tabelle "Serienlinsen" für eine bestimmte AvoNr + Artikel
        // und schreibt das Ergebnis in das übergebene Label.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateSeiteFürLabel(string avoNr, Label lblSeite)
        {
            // Frühzeitig abbrechen, wenn Eingaben fehlen.
            if (string.IsNullOrEmpty(avoNr) || string.IsNullOrEmpty(artikel))
            {
                lblSeite.Text = "";
                return;
            }

            try
            {
                // Parameterisierte SQL-Abfrage (sicher gegen SQL-Injection).
                string query = @"
            SELECT SEITE
            FROM Serienlinsen
            WHERE refo_avonr = @AvoNr AND ARTNR = @Artikel";

                using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                {
                    command.Parameters.AddWithValue("@AvoNr", avoNr);
                    command.Parameters.AddWithValue("@Artikel", artikel);

                    sqlConnectionVerwaltung.Open();

                    // ExecuteScalar liefert den ersten Wert der ersten Zeile (oder null).
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        lblSeite.Text = result.ToString();
                    }
                    else
                    {
                        // Kein Treffer → Label leeren.
                        lblSeite.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Abrufen der SEITE-Daten für AvoNr {avoNr} und Artikel {artikel}: {ex.Message}");
            }
            finally
            {
                if (sqlConnectionVerwaltung.State == ConnectionState.Open)
                {
                    sqlConnectionVerwaltung.Close();
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "Druck Auftragszettel": öffnet eine Excel-Vorlage,
        // schreibt für jede aktive Seite Daten + Bilder + Barcode hinein und speichert.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnDruckAuftragszettel_Click(object sender, EventArgs e)
        {
            // Pfad zur Vorlage (Netzwerklaufwerk).
            string excelFilePath = @"P:\TEDuTOZ\Auftragsverwaltung Daten\VerwaltungKst1127\Auftragszettel.xlsx";

            // Excel-Anwendung über COM-Interop starten.
            // Wichtig: COM-Objekte am Ende mit Marshal.ReleaseComObject freigeben (sonst bleibt EXCEL.EXE im Hintergrund).
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = null;

            // Welche Sheets wurden angefasst (für späteren Druck).
            List<string> modifiedSheets = new List<string>();

            try
            {
                // Vorlage öffnen.
                workbook = excelApp.Workbooks.Open(excelFilePath);

                // -----------------------------------------------------------------------------------------------------
                // Lokale Funktion (nur in dieser Methode sichtbar): schreibt alle Daten in EIN Sheet.
                // C# erlaubt geschachtelte Funktionen seit C# 7.
                // -----------------------------------------------------------------------------------------------------
                void WriteToSheet(string sheetName, string auftragsNr, string artikel, string teilebez, string sollStk, string labelWert, string enddatum, Dictionary<string, string> serienlinsenDaten)
                {
                    Excel.Worksheet worksheet = workbook.Sheets[sheetName];

                    // Excel.Cells[row, col].Value = Wert in eine bestimmte Zelle schreiben.
                    worksheet.Cells[2, 6].Value = auftragsNr;          // Zelle F2
                    worksheet.Cells[2, 2].NumberFormat = "@";          // Format auf Text setzen, damit führende Nullen erhalten bleiben
                    worksheet.Cells[2, 2].Value = artikel;             // Zelle B2
                    worksheet.Cells[2, 10].Value = teilebez;           // Zelle J2
                    worksheet.Cells[4, 13].Value = sollStk;            // Zelle M4
                    worksheet.Cells[4, 7].Value = labelWert;           // Zelle G4
                    worksheet.Cells[5, 7].Value = enddatum;            // Zelle G5

                    // Wenn Serienlinsen-Daten vorhanden sind: weitere Felder + Bilder + Barcode setzen.
                    if (serienlinsenDaten != null)
                    {
                        worksheet.Cells[4, 3].Value = serienlinsenDaten["Belag1"];                        // C4
                        worksheet.Cells[5, 3].Value = serienlinsenDaten["Material"];                      // C5
                        worksheet.Cells[9, 9].Value = serienlinsenDaten["Radius2"];                       // I9
                        worksheet.Cells[17, 9].Value = serienlinsenDaten["Radius1"];                      // I17
                        worksheet.Cells[20, 11].Value = serienlinsenDaten["ND"];                          // K20
                        worksheet.Cells[21, 11].Value = serienlinsenDaten["G_Nummer"];                    // K21
                        worksheet.Cells[22, 11].Value = serienlinsenDaten["GLASSORTE"];                   // K22
                        worksheet.Cells[23, 11].Value = serienlinsenDaten["DM"];                          // K23
                        worksheet.Cells[24, 11].Value = serienlinsenDaten["DICKE"];                       // K24
                        worksheet.Cells[25, 10].Value = serienlinsenDaten["InfoZeichnung_Bemerkungen"];   // J26 (laut Kommentar)

                        // ---------- Bild "Auflegeinformation" (Zeichnungspfad) einfügen ----------
                        string bildPfad1 = serienlinsenDaten["Zeichnungspfad"];
                        if (!string.IsNullOrEmpty(bildPfad1) && System.IO.File.Exists(bildPfad1))
                        {
                            // Zellbereich, in dem das Bild platziert werden soll.
                            Excel.Range oRange = worksheet.get_Range("I10", "K16");

                            // Vorhandene Bilder (Shapes) im Bereich finden und sammeln, dann löschen.
                            // (Beim Iterieren nicht direkt löschen – sonst Index-Probleme.)
                            List<Excel.Shape> shapesToDelete = new List<Excel.Shape>();
                            foreach (Excel.Shape shape in worksheet.Shapes)
                            {
                                if (shape.TopLeftCell.Row >= oRange.Row && shape.TopLeftCell.Column >= oRange.Column &&
                                    shape.TopLeftCell.Row <= oRange.Row + oRange.Rows.Count - 1 &&
                                    shape.TopLeftCell.Column <= oRange.Column + oRange.Columns.Count - 1)
                                {
                                    shapesToDelete.Add(shape);
                                }
                            }
                            foreach (Excel.Shape shape in shapesToDelete)
                            {
                                shape.Delete();
                            }

                            // Bild an exakt diese Position (Pixelkoordinaten in Excel-Punkten) einfügen.
                            float left = (float)((double)oRange.Left);
                            float top = (float)((double)oRange.Top);
                            float width = (float)((double)oRange.Width);
                            float height = (float)((double)oRange.Height);
                            worksheet.Shapes.AddPicture(
                                bildPfad1,
                                Microsoft.Office.Core.MsoTriState.msoFalse,   // nicht verlinken
                                Microsoft.Office.Core.MsoTriState.msoCTrue,   // in Datei einbetten
                                left, top, width, height);
                        }

                        // ---------- Bild "Info Zeichnung" einfügen (analog zu oben) ----------
                        string bildPfad2 = serienlinsenDaten["InfoZeichnung"];
                        if (!string.IsNullOrEmpty(bildPfad2) && System.IO.File.Exists(bildPfad2))
                        {
                            Excel.Range oRange = worksheet.get_Range("M10", "N16");

                            List<Excel.Shape> shapesToDelete = new List<Excel.Shape>();
                            foreach (Excel.Shape shape in worksheet.Shapes)
                            {
                                if (shape.TopLeftCell.Row >= oRange.Row && shape.TopLeftCell.Column >= oRange.Column &&
                                    shape.TopLeftCell.Row <= oRange.Row + oRange.Rows.Count - 1 &&
                                    shape.TopLeftCell.Column <= oRange.Column + oRange.Columns.Count - 1)
                                {
                                    shapesToDelete.Add(shape);
                                }
                            }
                            foreach (Excel.Shape shape in shapesToDelete)
                            {
                                shape.Delete();
                            }

                            float left = (float)((double)oRange.Left);
                            float top = (float)((double)oRange.Top);
                            float width = (float)((double)oRange.Width);
                            float height = (float)((double)oRange.Height);
                            worksheet.Shapes.AddPicture(bildPfad2,
                                Microsoft.Office.Core.MsoTriState.msoFalse,
                                Microsoft.Office.Core.MsoTriState.msoCTrue,
                                left, top, width, height);
                        }

                        // ---------- Barcodes generieren und in B27 jedes Sheets einfügen ----------
                        // Wir gehen durch die 4 Vergüten-Strichcode-Texte und die 4 Sheet-Namen parallel.
                        string[] barcodeTexts = new string[]
                        {
                            lblStrichcodeZuPositionVergueten1.Text,
                            lblStrichcodeZuPositionVergueten2.Text,
                            lblStrichcodeZuPositionVergueten3.Text,
                            lblStrichcodeZuPositionVergueten4.Text
                        };
                        string[] sheetNames = new string[] { "Seite1", "Seite2", "Seite3", "Seite4" };

                        for (int i = 0; i < barcodeTexts.Length; i++)
                        {
                            // Nur wenn ein Barcode-Text vorhanden ist.
                            if (!string.IsNullOrEmpty(barcodeTexts[i]))
                            {
                                // 1) Barcode als Bild erzeugen.
                                Image barcodeImage = GenerateBarcode(barcodeTexts[i]);

                                // 2) Bild in eine temporäre PNG-Datei schreiben (Excel braucht einen Pfad zum Einfügen).
                                string tempPath = System.IO.Path.GetTempFileName();
                                barcodeImage.Save(tempPath, ImageFormat.Png);

                                // 3) Aktuelles Arbeitsblatt + Zielzelle holen.
                                Excel.Worksheet currentWorksheet = workbook.Sheets[sheetNames[i]];
                                Excel.Range barcodeRange = currentWorksheet.get_Range("B27");

                                // 4) Vorhandene Bilder direkt in der Zelle B27 entfernen.
                                List<Excel.Shape> shapesToDelete = new List<Excel.Shape>();
                                foreach (Excel.Shape shape in currentWorksheet.Shapes)
                                {
                                    if (shape.TopLeftCell.Address == barcodeRange.Address)
                                    {
                                        shapesToDelete.Add(shape);
                                    }
                                }
                                foreach (Excel.Shape shape in shapesToDelete)
                                {
                                    shape.Delete();
                                }

                                // 5) Barcode-Bild in die Zelle einfügen.
                                float barcodeLeft = (float)((double)barcodeRange.Left);
                                float barcodeTop = (float)((double)barcodeRange.Top);
                                currentWorksheet.Shapes.AddPicture(
                                    tempPath,
                                    Microsoft.Office.Core.MsoTriState.msoFalse,
                                    Microsoft.Office.Core.MsoTriState.msoCTrue,
                                    barcodeLeft, barcodeTop,
                                    barcodeImage.Width, barcodeImage.Height);

                                // 6) Aufräumen: Temp-Datei löschen, COM-Objekt freigeben.
                                System.IO.File.Delete(tempPath);
                                Marshal.ReleaseComObject(currentWorksheet);

                                // Sheet als geändert merken (für späteren Druck).
                                if (!modifiedSheets.Contains(sheetNames[i]))
                                {
                                    modifiedSheets.Add(sheetNames[i]);
                                }
                            }
                        }
                    }

                    // Worksheet-Objekt freigeben (Excel-Hintergrundprozesse vermeiden).
                    Marshal.ReleaseComObject(worksheet);

                    // Auftragsnummer in lokale JSON-Liste schreiben (Historie der zuletzt gedruckten Aufträge).
                    FügeAuftragsnummerZurJsonDateiHinzu(auftragsNr);
                }

                // -----------------------------------------------------------------------------------------------------
                // Lokale Funktion: holt das Enddatum eines bestimmten Arbeitsvorgangs.
                // -----------------------------------------------------------------------------------------------------
                string GetEnddatum(string auftragsNr, string arbeitsvorgang)
                {
                    string query = @"
                SELECT trdf_enddate
                FROM LN_ProdOrders_PRD
                WHERE pdno_prodnr = @Auftragsnummer AND txta_Avoinfo = @Arbeitsvorgang";

                    using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        command.Parameters.AddWithValue("@Auftragsnummer", auftragsNr);
                        command.Parameters.AddWithValue("@Arbeitsvorgang", arbeitsvorgang);

                        sqlConnectionVerwaltung.Open();
                        object result = command.ExecuteScalar(); // einzelner Wert
                        sqlConnectionVerwaltung.Close();

                        if (result != DBNull.Value && result != null)
                        {
                            return result.ToString();
                        }
                        return string.Empty; // nichts gefunden → leerer String
                    }
                }

                // -----------------------------------------------------------------------------------------------------
                // Lokale Funktion: holt alle Stammdaten für einen Artikel + Seite aus "Serienlinsen"
                // und packt sie in ein Dictionary (Spaltenname → Wert).
                // -----------------------------------------------------------------------------------------------------
                Dictionary<string, string> GetSerienlinsenDaten(string artikel, string seite)
                {
                    string query = @"
                        SELECT Belag1, Material, Radius2, Radius1, ND, G_Nummer, GLASSORTE, DM, DICKE, InfoZeichnung_Bemerkungen, InfoZeichnung, Zeichnungspfad
                        FROM Serienlinsen
                        WHERE ARTNR = @Artikel AND SEITE = @Seite";

                    using (SqlCommand command = new SqlCommand(query, sqlConnectionVerwaltung))
                    {
                        command.Parameters.AddWithValue("@Artikel", artikel);
                        command.Parameters.AddWithValue("@Seite", seite);

                        sqlConnectionVerwaltung.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Es wird nur die erste Zeile gelesen.
                            if (reader.Read())
                            {
                                var daten = new Dictionary<string, string>
                                {
                                    { "Belag1", reader["Belag1"].ToString() },
                                    { "Material", reader["Material"].ToString() },
                                    { "Radius2", reader["Radius2"].ToString() },
                                    { "Radius1", reader["Radius1"].ToString() },
                                    { "ND", reader["ND"].ToString() },
                                    { "G_Nummer", reader["G_Nummer"].ToString() },
                                    { "GLASSORTE", reader["GLASSORTE"].ToString() },
                                    // DM-Wert: Komma → Punkt (für englische Zahlen-Formate).
                                    { "DM", reader["DM"].ToString().Replace(',', '.') },
                                    { "DICKE", reader["DICKE"].ToString() },
                                    { "InfoZeichnung_Bemerkungen", reader["InfoZeichnung_Bemerkungen"].ToString() },
                                    { "InfoZeichnung", reader["InfoZeichnung"].ToString() },
                                    { "Zeichnungspfad", reader["Zeichnungspfad"].ToString() }
                                };
                                sqlConnectionVerwaltung.Close();
                                return daten;
                            }
                        }
                        sqlConnectionVerwaltung.Close();
                        return null; // nichts gefunden
                    }
                }

                // -----------------------------------------------------------------------------------------------------
                // Für jede Seite mit gesetztem Label: Daten holen und in das passende Sheet schreiben.
                // -----------------------------------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(lblSeite1.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten1.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite1.Text);
                    WriteToSheet("Seite1", auftragsNr, artikel, teilebez, sollStk, lblSeite1.Text, enddatum, serienlinsenDaten);
                }
                if (!string.IsNullOrEmpty(lblSeite2.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten2.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite2.Text);
                    WriteToSheet("Seite2", auftragsNr, artikel, teilebez, sollStk, lblSeite2.Text, enddatum, serienlinsenDaten);
                }
                if (!string.IsNullOrEmpty(lblSeite3.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten3.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite3.Text);
                    WriteToSheet("Seite3", auftragsNr, artikel, teilebez, sollStk, lblSeite3.Text, enddatum, serienlinsenDaten);
                }
                if (!string.IsNullOrEmpty(lblSeite4.Text))
                {
                    string enddatum = GetEnddatum(auftragsNr, lblArbeitsvorgangVergueten4.Text);
                    var serienlinsenDaten = GetSerienlinsenDaten(artikel, lblSeite4.Text);
                    WriteToSheet("Seite4", auftragsNr, artikel, teilebez, sollStk, lblSeite4.Text, enddatum, serienlinsenDaten);
                }

                // Alle Änderungen in der Excel-Vorlage speichern.
                workbook.Save();

                // ############### Druckübersicht Ein/Ausschalten ######################
                // Der folgende Block ist absichtlich auskommentiert (Tests/Optionen).
                ////// Geänderte Sheets drucken (nur erste Seite):
                ////foreach (string sheetName in modifiedSheets)
                ////{
                ////    Excel.Worksheet worksheet = workbook.Sheets[sheetName];
                ////    worksheet.PrintOutEx(1, 1, 1, false);
                ////}
                ////this.Close(); // Formular schließen, wenn etwas gedruckt wurde
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Schreiben in die Excel-Datei: {ex.Message}");
            }
            finally
            {
                // Egal was passiert: Excel sauber schließen.
                // Reihenfolge wichtig: zuerst Workbook, dann Application.
                if (workbook != null)
                {
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Schreibt die Auftragsnummer in eine lokale JSON-Datei (Historie zuletzt gedruckter Aufträge).
        // - liest evtl. vorhandene Datei ein
        // - prüft auf Duplikate
        // - speichert die aktualisierte Liste
        // -----------------------------------------------------------------------------------------------------------------
        private void FügeAuftragsnummerZurJsonDateiHinzu(string neueAuftragsnummer)
        {
            string jsonPfad = "letzteAuftragsnummern.json"; // relativ zum Arbeitsverzeichnis

            try
            {
                List<string> auftragsnummern = new List<string>();

                // Falls die Datei schon existiert, ihren Inhalt einlesen.
                if (File.Exists(jsonPfad))
                {
                    string vorhandenerJsonInhalt = File.ReadAllText(jsonPfad);

                    // Nur deserialisieren, wenn der Inhalt nicht leer/nur Whitespace ist.
                    if (!string.IsNullOrWhiteSpace(vorhandenerJsonInhalt))
                    {
                        // JSON → C#-Liste umwandeln; falls null zurückkommt, leere Liste verwenden.
                        auftragsnummern = JsonSerializer.Deserialize<List<string>>(vorhandenerJsonInhalt) ?? new List<string>();
                    }
                }

                // Nur hinzufügen, wenn die Auftragsnummer noch nicht in der Liste ist.
                if (!auftragsnummern.Contains(neueAuftragsnummer))
                {
                    auftragsnummern.Add(neueAuftragsnummer);

                    // C#-Liste → JSON-Text (mit Einrückung für bessere Lesbarkeit).
                    string neuerJsonInhalt = JsonSerializer.Serialize(
                        auftragsnummern,
                        new JsonSerializerOptions { WriteIndented = true });

                    // Datei überschreiben.
                    File.WriteAllText(jsonPfad, neuerJsonInhalt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Schreiben der JSON-Datei: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // -----------------------------------------------------------------------------------------------------------------
        // Erzeugt aus einem Text einen Code-128-Barcode als Bild (Image).
        // Wird oben für die Strichcode-Bilder in Excel verwendet.
        // -----------------------------------------------------------------------------------------------------------------
        private Image GenerateBarcode(string text)
        {
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128, // Barcode-Format (sehr verbreitet, alphanumerisch)
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 32, // Höhe des Barcodes in Pixel
                    Width = 150  // Breite des Barcodes in Pixel
                }
            };

            // Write(text) erzeugt aus dem String das fertige Bild.
            return barcodeWriter.Write(text);
        }
    }
}
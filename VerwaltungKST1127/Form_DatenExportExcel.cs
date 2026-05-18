using Microsoft.Office.Interop.Excel;          // Excel-Interop für Automatisierung (Workbook, Worksheet, Range, ChartObject, ...)
using System;                                  // Basisklassen wie EventArgs, DateTime, Exception
using System.Collections.Generic;              // Generische Collections (List<T>)
using System.ComponentModel;                   // Komponenten-Unterstützung für Designer
using System.Data;                             // DataTable, DataSet
using System.Data.SqlClient;                   // SQL Server Zugriff (SqlConnection, SqlCommand)
using System.Drawing;                          // System.Drawing.Color (für ColorTranslator.ToOle)
using System.IO;                               // Für Dateipfad- und Verzeichnisoperationen
using System.Linq;                             // LINQ-Erweiterungen
using System.Runtime.InteropServices;          // Marshal.ReleaseComObject für sauberes COM-Cleanup
using System.Threading.Tasks;                  // async/await
using System.Windows.Forms;                    // WinForms (Form, ListBox, MessageBox)

namespace VerwaltungKST1127
{
    public partial class Form_DatenExportExcel : Form
    {
        // Connection String für die Datenbank "SOA127_Chargenprotokoll"
        private const string ConnectionStringChargenprotokoll =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

        // Connection String für die ALTE Verwaltungsdatenbank "SOA127_Verwaltung"
        private const string ConnectionStringVerwaltung =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung;Integrated Security=True;Encrypt=False";

        // Connection String für die NEUE Verwaltungsdatenbank "SOA127_Verwaltung2022"
        private const string ConnectionStringVerwaltung2022 =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False";

        // Connection String für die Shuttle-Datenbank "SOA127_Shuttle"
        private const string ConnectionStringShuttle =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Shuttle;Integrated Security=True";

        // Connection String für die Auswertungs-Datenbank
        private const string ConnectionStringAuswertung =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Auswertung;Integrated Security=True;Encrypt=False";

        // Connection String für die Waschträgerl-Datenbank
        private const string ConnectionStringWaschtragerl =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Waschtragerl;Integrated Security=True;Encrypt=False";

        // Connection String für die Mess-Datenbank
        private const string ConnectionStringMessen =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Messen;Integrated Security=True;Encrypt=False";

        // Sammelliste aller ListBoxen, damit wir bequem über sie iterieren können
        private System.Windows.Forms.ListBox[] _alleListBoxen;

        // Schutz-Flag, damit das Zurücksetzen der Auswahl nicht selbst wieder
        // SelectedIndexChanged-Events auslöst (Endlosschleifen-Vermeidung)
        private bool _aktualisiereAuswahl = false;

        // Hilfsklasse zur sauberen Strukturierung unserer "Wichtigen Tabellen"
        private class ExportJob
        {
            public string ConnectionString { get; set; }
            public string Schema { get; set; }
            public string Tabelle { get; set; }
            public int ZeilenAnzahl { get; set; }
            public System.Data.DataTable Daten { get; set; }
            public string ZielDateiName { get; set; } // Der gewünschte Name der Excel-Datei
        }

        // Konstruktor des Formulars
        public Form_DatenExportExcel()
        {
            InitializeComponent();

            // Beim Laden des Formulars werden alle ListBoxen befüllt
            this.Load += Form_DatenExportExcel_Load;
        }

        /// <summary>
        /// Wird beim Laden des Formulars aufgerufen und befüllt alle ListBoxen
        /// mit den Tabellennamen aus der jeweils zugehörigen Datenbank.
        /// </summary>
        private void Form_DatenExportExcel_Load(object sender, EventArgs e)
        {
            BefuelleListBoxMitTabellen(listBoxVerwaltungAlt, ConnectionStringVerwaltung);
            BefuelleListBoxMitTabellen(listBoxVerwaltungNeu, ConnectionStringVerwaltung2022);
            BefuelleListBoxMitTabellen(listBoxShuttle, ConnectionStringShuttle);
            BefuelleListBoxMitTabellen(listBoxAuswertung, ConnectionStringAuswertung);
            BefuelleListBoxMitTabellen(listBoxMessen, ConnectionStringMessen);
            BefuelleListBoxMitTabellen(listBoxWaschtragerl, ConnectionStringWaschtragerl);
            BefuelleListBoxMitTabellen(listBoxChargenprotokoll, ConnectionStringChargenprotokoll);

            // Alle ListBoxen in einem Array sammeln, damit sie zentral verwaltet werden können
            _alleListBoxen = new[]
            {
                listBoxVerwaltungAlt,
                listBoxVerwaltungNeu,
                listBoxShuttle,
                listBoxAuswertung,
                listBoxMessen,
                listBoxWaschtragerl,
                listBoxChargenprotokoll
            };

            // An jede ListBox denselben Event-Handler hängen,
            // sodass nur in einer Box gleichzeitig etwas markiert ist
            foreach (System.Windows.Forms.ListBox lb in _alleListBoxen)
            {
                lb.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            }
        }

        /// <summary>
        /// Sorgt dafür, dass beim Auswählen eines Eintrags in einer ListBox
        /// die Markierung aller anderen ListBoxen entfernt wird.
        /// </summary>
        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_aktualisiereAuswahl)
                return;

            System.Windows.Forms.ListBox aktuelle = sender as System.Windows.Forms.ListBox;
            if (aktuelle == null)
                return;

            _aktualisiereAuswahl = true;
            try
            {
                foreach (System.Windows.Forms.ListBox lb in _alleListBoxen)
                {
                    if (lb != aktuelle)
                    {
                        lb.ClearSelected();
                    }
                }
            }
            finally
            {
                _aktualisiereAuswahl = false;
            }
        }

        /// <summary>
        /// Liest alle Benutzer-Tabellen aus der Datenbank und schreibt sie als
        /// "Schema.Tabellenname" in die übergebene ListBox.
        /// </summary>
        private void BefuelleListBoxMitTabellen(System.Windows.Forms.ListBox listBox, string connectionString)
        {
            try
            {
                listBox.Items.Clear();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql =
                        "SELECT TABLE_SCHEMA, TABLE_NAME " +
                        "FROM INFORMATION_SCHEMA.TABLES " +
                        "WHERE TABLE_TYPE = 'BASE TABLE' " +
                        "ORDER BY TABLE_SCHEMA, TABLE_NAME";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string schema = reader.GetString(0);
                            string tabelle = reader.GetString(1);
                            listBox.Items.Add($"{schema}.{tabelle}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fehler beim Laden der Tabellen für '{listBox.Name}':\n{ex.Message}",
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Ermittelt, in welcher ListBox aktuell etwas markiert ist
        /// und gibt die zugehörige Verbindungszeichenfolge zurück.
        /// </summary>
        private string ErmittleConnectionStringFuerAuswahl(out System.Windows.Forms.ListBox ausgewaehlteListBox)
        {
            ausgewaehlteListBox = null;

            foreach (System.Windows.Forms.ListBox lb in _alleListBoxen)
            {
                if (lb.SelectedItem != null)
                {
                    ausgewaehlteListBox = lb;
                    break;
                }
            }

            if (ausgewaehlteListBox == null)
                return null;

            if (ausgewaehlteListBox == listBoxVerwaltungAlt) return ConnectionStringVerwaltung;
            if (ausgewaehlteListBox == listBoxVerwaltungNeu) return ConnectionStringVerwaltung2022;
            if (ausgewaehlteListBox == listBoxShuttle) return ConnectionStringShuttle;
            if (ausgewaehlteListBox == listBoxAuswertung) return ConnectionStringAuswertung;
            if (ausgewaehlteListBox == listBoxMessen) return ConnectionStringMessen;
            if (ausgewaehlteListBox == listBoxWaschtragerl) return ConnectionStringWaschtragerl;
            if (ausgewaehlteListBox == listBoxChargenprotokoll) return ConnectionStringChargenprotokoll;

            return null;
        }

        /// <summary>
        /// Klick auf den Export-Button für eine einzelne Tabelle (manuelle Auswahl)
        /// </summary>
        private async void btnExportTable_Click(object sender, EventArgs e)
        {
            // 1) Connection String + ausgewählte ListBox bestimmen
            string connectionString = ErmittleConnectionStringFuerAuswahl(out System.Windows.Forms.ListBox ausgewaehlteListBox);
            if (connectionString == null || ausgewaehlteListBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Bitte zuerst eine Tabelle in einer der Listen auswählen.",
                    "Keine Auswahl",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Der ListBox-Eintrag hat das Format "Schema.Tabellenname"
            string vollerName = ausgewaehlteListBox.SelectedItem.ToString();
            string[] teile = vollerName.Split('.');
            if (teile.Length != 2)
            {
                MessageBox.Show("Tabellenname konnte nicht interpretiert werden.",
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string schema = teile[0];
            string tabelle = teile[1];

            try
            {
                // 2) Zeilenanzahl ermitteln und Benutzer fragen
                int zeilenAnzahl = ErmittleZeilenAnzahl(connectionString, schema, tabelle);

                DialogResult antwort = MessageBox.Show(
                    $"Wollen Sie den Datenexport von {zeilenAnzahl} Zeilen durchführen?\n\n" +
                    "Hinweis: Bei großen Datenmengen kann der Vorgang einige Zeit dauern.",
                    "Datenexport bestätigen",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (antwort != DialogResult.Yes)
                    return;

                // 3) Daten laden
                System.Data.DataTable daten = LadeTabellenDaten(connectionString, schema, tabelle);

                // 4) Nach Excel exportieren (Interop)
                await ExportiereNachExcelInteropAsync(daten, $"{schema}.{tabelle}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fehler beim Datenexport:\n{ex.Message}",
                    "Exportfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// SELECT COUNT(*) auf die angegebene Tabelle.
        /// </summary>
        private int ErmittleZeilenAnzahl(string connectionString, string schema, string tabelle)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM [{schema}].[{tabelle}]";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        /// <summary>
        /// Lädt alle Daten der angegebenen Tabelle in eine DataTable.
        /// </summary>
        private System.Data.DataTable LadeTabellenDaten(string connectionString, string schema, string tabelle)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = $"SELECT * FROM [{schema}].[{tabelle}]";
                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Schreibt die Daten der DataTable per Microsoft.Office.Interop.Excel
        /// in eine neue Excel-Mappe und öffnet diese für den Benutzer.
        /// </summary>
        private async Task ExportiereNachExcelInteropAsync(System.Data.DataTable daten, string blattName)
        {
            string sicherenName = blattName.Replace("[", "").Replace("]", "").Replace("*", "").Replace("?", "").Replace("/", "_").Replace("\\", "_").Replace(":", "_");
            if (sicherenName.Length > 31) sicherenName = sicherenName.Substring(0, 31);

            Workbook workbook = null;
            Worksheet worksheet = null;
            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                excelApp.Visible = false;
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Worksheet)workbook.ActiveSheet;
                worksheet.Name = sicherenName;

                int spaltenAnzahl = daten.Columns.Count;
                int zeilenAnzahl = daten.Rows.Count;

                // Header formatieren
                for (int c = 0; c < spaltenAnzahl; c++)
                {
                    var headerCell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, c + 1];
                    headerCell.Value = daten.Columns[c].ColumnName;
                    headerCell.Font.Bold = true;
                    headerCell.Font.Size = 12;
                    headerCell.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
                    headerCell.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.FromArgb(0x1F, 0x4E, 0x78));
                    headerCell.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    headerCell.VerticalAlignment = XlVAlign.xlVAlignCenter;

                    Borders borders = headerCell.Borders;
                    borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
                    borders[XlBordersIndex.xlEdgeBottom].Color = ColorTranslator.ToOle(System.Drawing.Color.Black);
                    borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;
                    Marshal.ReleaseComObject(borders);
                    Marshal.ReleaseComObject(headerCell);
                }

                ((Microsoft.Office.Interop.Excel.Range)worksheet.Rows[1]).RowHeight = 22;

                Microsoft.Office.Interop.Excel.Range headerRangeForFilter = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, spaltenAnzahl]];
                headerRangeForFilter.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
                Marshal.ReleaseComObject(headerRangeForFilter);

                excelApp.ActiveWindow.SplitRow = 1;
                excelApp.ActiveWindow.FreezePanes = true;

                // Daten schreiben
                if (zeilenAnzahl > 0)
                {
                    object[,] data = await Task.Run(() =>
                    {
                        var arr = new object[zeilenAnzahl, spaltenAnzahl];
                        for (int r = 0; r < zeilenAnzahl; r++)
                        {
                            DataRow row = daten.Rows[r];
                            for (int c = 0; c < spaltenAnzahl; c++)
                            {
                                object val = row[c];
                                arr[r, c] = (val == DBNull.Value) ? null : val;
                            }
                        }
                        return arr;
                    });

                    Microsoft.Office.Interop.Excel.Range dataRange = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[zeilenAnzahl + 1, spaltenAnzahl]];
                    dataRange.Value = data;
                    dataRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    Marshal.ReleaseComObject(dataRange);
                }

                Microsoft.Office.Interop.Excel.Range usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();
                Marshal.ReleaseComObject(usedRange);

                MessageBox.Show($"Export erfolgreich!\n{zeilenAnzahl} Zeilen wurden in Excel geladen.\nSpeichern Sie die Mappe direkt aus Excel.", "Export abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                try
                {
                    if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                    if (excelApp != null) excelApp.Quit();
                }
                catch { }

                MessageBox.Show($"Fehler beim Excel-Export:\n{ex.Message}", "Exportfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Klick auf den Button "Wichtigste Tabellen":
        /// Speichert die 5 wichtigen Tabellen vollautomatisch als separate Dateien im Austauschordner.
        /// </summary>
        private async void btnWichtigsteTabellen_Click(object sender, EventArgs e)
        {
            // 1. Definition unserer wichtigsten Tabellen inkl. der gewünschten Dateinamen
            var jobs = new List<ExportJob>
            {
                new ExportJob { ConnectionString = ConnectionStringVerwaltung2022, Schema = "dbo", Tabelle = "Maximalwerte_Farbauswertung", ZielDateiName = "MaximalwerteFarbauswertung.xlsx" },
                new ExportJob { ConnectionString = ConnectionStringVerwaltung2022, Schema = "dbo", Tabelle = "Serienlinsen", ZielDateiName = "Serienlinsen.xlsx" },
                new ExportJob { ConnectionString = ConnectionStringVerwaltung2022, Schema = "dbo", Tabelle = "Glassdaten", ZielDateiName = "Glasdaten.xlsx" }, // ss in DB, s im Dateinamen
                new ExportJob { ConnectionString = ConnectionStringMessen, Schema = "dbo", Tabelle = "GesamteGruppen", ZielDateiName = "Gesamtdaten Messungen.xlsx" },
                new ExportJob { ConnectionString = ConnectionStringShuttle, Schema = "dbo", Tabelle = "Ring_Stamm", ZielDateiName = "Ringe.xlsx" }
            };

            // Der gewünschte Zielpfad laut Screenshot
            string zielPfad = @"P:\TEDuTOZ\Verschiedenes\Austauschordner_Kst146\Daten_TabellenAusSqlServer\";

            try
            {
                // Sicherstellen, dass der Ordner existiert (falls P: mal nicht verbunden ist, fliegt hier direkt ein Fehler, was gut ist)
                if (!Directory.Exists(zielPfad))
                {
                    Directory.CreateDirectory(zielPfad);
                }

                // 2. Zeilen zählen und Bestätigungs-Meldung aufbauen
                string meldung = $"Folgende Tabellen werden in den Ordner\n'{zielPfad}'\nexportiert:\n\n";
                int gesamtZeilen = 0;

                foreach (var job in jobs)
                {
                    job.ZeilenAnzahl = ErmittleZeilenAnzahl(job.ConnectionString, job.Schema, job.Tabelle);
                    gesamtZeilen += job.ZeilenAnzahl;
                    meldung += $"- {job.ZielDateiName}: {job.ZeilenAnzahl} Zeilen\n";
                }

                meldung += $"\nGesamtanzahl zu exportierender Zeilen: {gesamtZeilen}\n";
                meldung += "\nMöchten Sie den Datenexport jetzt starten?";

                DialogResult antwort = MessageBox.Show(
                    meldung,
                    "Automatischer Batch-Export",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);

                if (antwort != DialogResult.OK)
                    return;

                // 3. ProgressBar einstellen (2 Schritte pro Tabelle: Daten laden + Datei speichern)
                progressBar1.Minimum = 0;
                progressBar1.Maximum = jobs.Count * 2;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                // 4. Den Sammel-Export in separate Dateien starten
                await ExportiereInSeparateDateienAsync(jobs, zielPfad);

                MessageBox.Show(
                    "Alle 5 Dateien wurden erfolgreich erstellt und in den Austauschordner kopiert!",
                    "Export abgeschlossen",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Batch-Export:\n{ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Am Ende die ProgressBar wieder ausblenden/zurücksetzen
                progressBar1.Value = 0;
                progressBar1.Visible = false;
            }
        }

        /// <summary>
        /// Exportiert eine Liste von Tabellen im Hintergrund als separate Excel-Dateien
        /// in das angegebene Zielverzeichnis. Erstellt eine Kopie mit Nummer (z.B. _1),
        /// falls die Originaldatei gerade von jemandem geöffnet ist.
        /// </summary>
        private async Task ExportiereInSeparateDateienAsync(List<ExportJob> jobs, string zielOrdner)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;

            try
            {
                // Wir starten Excel einmalig im Hintergrund
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false; // Überschreiben von Dateien ohne Popups bestätigen

                foreach (var job in jobs)
                {
                    // a) Daten aus der DB laden
                    job.Daten = await Task.Run(() => LadeTabellenDaten(job.ConnectionString, job.Schema, job.Tabelle));
                    progressBar1.Value += 1; // Ladefortschritt

                    Workbook workbook = null;
                    Worksheet worksheet = null;

                    try
                    {
                        // b) Neue Mappe für diese Tabelle erstellen
                        workbook = excelApp.Workbooks.Add(Type.Missing);
                        worksheet = (Worksheet)workbook.ActiveSheet;

                        string blattName = job.ZielDateiName.Replace(".xlsx", "");
                        if (blattName.Length > 31) blattName = blattName.Substring(0, 31);
                        worksheet.Name = blattName;

                        int spaltenAnzahl = job.Daten.Columns.Count;
                        int zeilenAnzahl = job.Daten.Rows.Count;

                        if (spaltenAnzahl > 0)
                        {
                            // c) Header und Formatierung aufbauen
                            for (int c = 0; c < spaltenAnzahl; c++)
                            {
                                var headerCell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, c + 1];
                                headerCell.Value = job.Daten.Columns[c].ColumnName;
                                headerCell.Font.Bold = true;
                                headerCell.Font.Size = 12;
                                headerCell.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
                                headerCell.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.FromArgb(0x1F, 0x4E, 0x78));
                                headerCell.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                                headerCell.VerticalAlignment = XlVAlign.xlVAlignCenter;

                                Borders borders = headerCell.Borders;
                                borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
                                borders[XlBordersIndex.xlEdgeBottom].Color = ColorTranslator.ToOle(System.Drawing.Color.Black);
                                borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;
                                Marshal.ReleaseComObject(borders);
                                Marshal.ReleaseComObject(headerCell);
                            }

                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Rows[1]).RowHeight = 22;

                            Microsoft.Office.Interop.Excel.Range headerRangeForFilter = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, spaltenAnzahl]];
                            headerRangeForFilter.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
                            Marshal.ReleaseComObject(headerRangeForFilter);

                            excelApp.ActiveWindow.SplitRow = 1;
                            excelApp.ActiveWindow.FreezePanes = true;

                            // d) Daten in 2D-Array packen und übergeben
                            if (zeilenAnzahl > 0)
                            {
                                object[,] data = await Task.Run(() =>
                                {
                                    var arr = new object[zeilenAnzahl, spaltenAnzahl];
                                    for (int r = 0; r < zeilenAnzahl; r++)
                                    {
                                        DataRow row = job.Daten.Rows[r];
                                        for (int c = 0; c < spaltenAnzahl; c++)
                                        {
                                            object val = row[c];
                                            arr[r, c] = (val == DBNull.Value) ? null : val;
                                        }
                                    }
                                    return arr;
                                });

                                Microsoft.Office.Interop.Excel.Range dataRange = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[zeilenAnzahl + 1, spaltenAnzahl]];
                                dataRange.Value = data;
                                dataRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                                Marshal.ReleaseComObject(dataRange);
                            }

                            Microsoft.Office.Interop.Excel.Range usedRange = worksheet.UsedRange;
                            usedRange.Columns.AutoFit();
                            Marshal.ReleaseComObject(usedRange);
                        }

                        // e) Dateinamen sicherstellen (Neu: Prüfung auf Dateisperre)
                        string vollerPfad = Path.Combine(zielOrdner, job.ZielDateiName);
                        int counter = 1;

                        // Solange die Datei existiert UND von jemandem geöffnet ist, suchen wir einen neuen Namen
                        while (File.Exists(vollerPfad) && IstDateiGesperrt(vollerPfad))
                        {
                            string dateiOhneEndung = Path.GetFileNameWithoutExtension(job.ZielDateiName);
                            string endung = Path.GetExtension(job.ZielDateiName);

                            // Erstellt z.B. "Serienlinsen_1.xlsx", "Serienlinsen_2.xlsx"
                            vollerPfad = Path.Combine(zielOrdner, $"{dateiOhneEndung}_{counter}{endung}");
                            counter++;
                        }

                        // Datei speichern (wenn sie existiert, aber NICHT geöffnet ist, wird sie ganz normal überschrieben)
                        workbook.SaveAs(vollerPfad, XlFileFormat.xlOpenXMLWorkbook);
                    }
                    finally
                    {
                        // Einzelne Mappe schließen und Objekte sofort freigeben
                        if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                        if (workbook != null)
                        {
                            workbook.Close(false);
                            Marshal.ReleaseComObject(workbook);
                        }
                    }

                    progressBar1.Value += 1; // Speicherfortschritt
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fehler während des Speicherns der Excel-Dateien.", ex);
            }
            finally
            {
                // Haupt-Excel-Instanz komplett beenden und Speicher freigeben
                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Prüft, ob eine Datei aktuell gesperrt ist (z.B. weil sie in Excel geöffnet ist).
        /// Das erreichen wir, indem wir versuchen, die Datei exklusiv im Schreibzugriff zu öffnen.
        /// </summary>
        private bool IstDateiGesperrt(string dateiPfad)
        {
            try
            {
                // Versuchen, die Datei exklusiv zu öffnen
                using (FileStream stream = File.Open(dateiPfad, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                // Wenn ein IOException auftritt, greift gerade ein anderer Prozess darauf zu
                return true;
            }

            // Datei ist frei
            return false;
        }
    }
}
using Microsoft.Office.Interop.Excel;          // Excel-Interop für Automatisierung (Workbook, Worksheet, Range, ChartObject, ...)
using System;                                  // Basisklassen wie EventArgs, DateTime, Exception
using System.Collections.Generic;              // Generische Collections (List<T>)
using System.ComponentModel;                   // Komponenten-Unterstützung für Designer
using System.Data;                             // DataTable, DataSet
using System.Data.SqlClient;                   // SQL Server Zugriff (SqlConnection, SqlCommand)
using System.Drawing;                          // System.Drawing.Color (für ColorTranslator.ToOle)
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
        /// Klick auf den Export-Button:
        /// 1) Ausgewählte Tabelle ermitteln
        /// 2) Zeilen zählen + Bestätigung einholen
        /// 3) Daten laden und per Excel-Interop in eine neue Mappe schreiben
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
        /// in eine neue Excel-Mappe (gleicher Stil wie Form_Farbauswertung).
        /// Header wird formatiert, AutoFilter aktiviert, Daten als 2D-Array
        /// in einem Rutsch geschrieben (deutlich schneller als Zelle-für-Zelle).
        /// Excel wird am Ende sichtbar geöffnet, der Benutzer speichert selbst.
        /// </summary>
        private async Task ExportiereNachExcelInteropAsync(System.Data.DataTable daten, string blattName)
        {
            // Excel-Worksheet-Namen dürfen max. 31 Zeichen lang sein und keine Sonderzeichen enthalten
            string sicherenName = blattName.Replace("[", "").Replace("]", "")
                                           .Replace("*", "").Replace("?", "")
                                           .Replace("/", "_").Replace("\\", "_")
                                           .Replace(":", "_");
            if (sicherenName.Length > 31)
                sicherenName = sicherenName.Substring(0, 31);

            Workbook workbook = null;
            Worksheet worksheet = null;
            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                excelApp.Visible = false; // erst am Ende einblenden

                // Neue Mappe + erstes Tabellenblatt umbenennen
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Worksheet)workbook.ActiveSheet;
                worksheet.Name = sicherenName;

                int spaltenAnzahl = daten.Columns.Count;
                int zeilenAnzahl = daten.Rows.Count;

                // ----- Header schreiben + formatieren (Zeile 1) -----
                for (int c = 0; c < spaltenAnzahl; c++)
                {
                    var headerCell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, c + 1];
                    headerCell.Value = daten.Columns[c].ColumnName;
                    headerCell.Font.Bold = true;
                    headerCell.Font.Size = 12;
                    headerCell.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
                    headerCell.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.FromArgb(0x1F, 0x4E, 0x78)); // Dunkelblau
                    headerCell.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    headerCell.VerticalAlignment = XlVAlign.xlVAlignCenter;

                    Borders borders = headerCell.Borders;
                    borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
                    borders[XlBordersIndex.xlEdgeBottom].Color = ColorTranslator.ToOle(System.Drawing.Color.Black);
                    borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;
                    Marshal.ReleaseComObject(borders);
                    Marshal.ReleaseComObject(headerCell);
                }

                // Header-Zeile etwas höher
                ((Microsoft.Office.Interop.Excel.Range)worksheet.Rows[1]).RowHeight = 22;

                // AutoFilter auf Headerzeile aktivieren
                Microsoft.Office.Interop.Excel.Range headerRangeForFilter =
                    worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, spaltenAnzahl]];
                headerRangeForFilter.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
                Marshal.ReleaseComObject(headerRangeForFilter);

                // Erste Zeile fixieren -> Header bleibt beim Scrollen sichtbar
                excelApp.ActiveWindow.SplitRow = 1;
                excelApp.ActiveWindow.FreezePanes = true;

                // ----- Daten schreiben -----
                if (zeilenAnzahl > 0)
                {
                    // 2D-Array in einem Hintergrund-Task befüllen, damit die UI reaktiv bleibt
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

                    // Komplettes Array in einem Rutsch in Excel schreiben (deutlich schneller als Zelle-für-Zelle)
                    Microsoft.Office.Interop.Excel.Range dataRange =
                        worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[zeilenAnzahl + 1, spaltenAnzahl]];
                    dataRange.Value = data;
                    dataRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    Marshal.ReleaseComObject(dataRange);
                }

                // Spaltenbreiten automatisch anpassen
                Microsoft.Office.Interop.Excel.Range usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();
                Marshal.ReleaseComObject(usedRange);

                // Erfolgsmeldung + Excel sichtbar machen, der Benutzer kann selbst speichern
                MessageBox.Show(
                    $"Export erfolgreich!\n{zeilenAnzahl} Zeilen wurden in Excel geladen.\n" +
                    "Speichern Sie die Mappe direkt aus Excel.",
                    "Export abgeschlossen",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                // Bei Fehler: Excel-Instanz wieder schließen, damit kein Geist-Prozess übrig bleibt
                try
                {
                    if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                    if (excelApp != null) excelApp.Quit();
                }
                catch { /* ignorieren */ }

                MessageBox.Show(
                    $"Fehler beim Excel-Export:\n{ex.Message}",
                    "Exportfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // COM-Objekte sauber freigeben — sonst bleibt EXCEL.EXE im Task-Manager
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                worksheet = null;
                workbook = null;
                excelApp = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
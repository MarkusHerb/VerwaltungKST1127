using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_RFIDAnsichtWaschanlagen : Form
    {
        private const string ConnectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Waschtragerl;Integrated Security=True;Encrypt=False";

        private System.Data.DataTable _rfidTable;
        private DataView _rfidView;
        private bool _columnsConfigured;

        public Form_RFIDAnsichtWaschanlagen()
        {
            InitializeComponent();

            // Sanfteres Rendering durch Double-Buffering (nicht öffentlich in WinForms, daher Reflection).
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(dGvRFID, true);

            TimerDatumUhrzeit.Start();
            TimerDatumUhrzeit.Tick += TimerDatumUhrzeit_Tick;
            UpdateZeitDatum();

            dateTimePickerDatumAb.Value = DateTime.Today.AddDays(-30);

            dateTimePickerDatumAb.ValueChanged += DateFilterChanged_ReloadFromDb;
            dateTimePickerDatumBis.ValueChanged += DateFilterChanged_ReloadFromDb;

            txtBoxArtikelnummer.KeyDown      += TextFilter_KeyDown;
            txtBoxAuftragsnummer.KeyDown     += TextFilter_KeyDown;
            txtBoxWaschprogramm.KeyDown      += TextFilter_KeyDown;
            txtBoxWaschprogrammAceton.KeyDown += TextFilter_KeyDown;
            txtBoxUID.KeyDown                += TextFilter_KeyDown;

            LoadRFIDData();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Uhrzeit/Datum-Handling
        // -----------------------------------------------------------------------------------------------------------------

        private void UpdateZeitDatum() =>
            lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

        private void TimerDatumUhrzeit_Tick(object sender, EventArgs e) => UpdateZeitDatum();

        // -----------------------------------------------------------------------------------------------------------------
        // UI-Zustand während des Ladevorgangs sperren / freigeben.
        // -----------------------------------------------------------------------------------------------------------------
        private void SetUIBusy(bool busy)
        {
            btnLoescheFilter.Enabled          = !busy;
            dateTimePickerDatumAb.Enabled     = !busy;
            dateTimePickerDatumBis.Enabled    = !busy;
            cListBoxWaschanlage.Enabled       = !busy;
            if (busy) lblEingeleseneWaschkoerbe.Text = "Lade Daten ...";
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Daten asynchron laden → UI bleibt während der DB-Abfrage reaktionsfähig.
        // -----------------------------------------------------------------------------------------------------------------
        private async void LoadRFIDData()
        {
            SetUIBusy(true);
            try
            {
                // Datum-Werte auf dem UI-Thread lesen, bevor wir in den Thread-Pool wechseln.
                var datumAb  = dateTimePickerDatumAb.Value.Date;
                var datumBis = dateTimePickerDatumBis.Value.Date.AddDays(1).AddSeconds(-1);

                // DB-Abfrage im Hintergrund → kein Einfrieren der Anwendung.
                var dataTable = await Task.Run(() => FetchDataFromDb(datumAb, datumBis));

                _rfidTable = dataTable;
                _rfidView  = new DataView(_rfidTable);

                // DataSource setzen; Spalten nur beim allerersten Laden konfigurieren.
                dGvRFID.DataSource = _rfidView;
                if (!_columnsConfigured)
                {
                    ConfigureColumns();
                    // Einmalige Breitenberechnung nur auf sichtbaren Zellen – deutlich schneller als AllCells.
                    dGvRFID.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                    // Kein automatisches Neuerstellen der Spalten bei späteren Ladevorgängen.
                    dGvRFID.AutoGenerateColumns = false;
                    _columnsConfigured = true;
                }

                ApplyCombinedFilter();

                lblEingeleseneWaschkoerbe.Text = $"Eingelesene Waschträger: {_rfidView.Count}";
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der RFID-Daten: " + ex.Message);
            }
            finally
            {
                SetUIBusy(false);
            }
        }

        // Datenbankabfrage – läuft im Thread-Pool, darf KEINEN UI-Zugriff machen.
        // Artikelnummer (DB4+DB5) und Auftragsnummer (DB28+DB29+DB30) werden direkt im SQL
        // zusammengesetzt; der C#-Zeilendurchlauf entfällt damit vollständig.
        // Nur benötigte Spalten werden abgefragt (kein SELECT *).
        private static System.Data.DataTable FetchDataFromDb(DateTime datumAb, DateTime datumBis)
        {
            const string query = @"
                SELECT
                    Datum,
                    ISNULL(CAST(DB4  AS NVARCHAR(50)),'') + ISNULL(CAST(DB5  AS NVARCHAR(50)),'')                                             AS Artikelnummer,
                    ISNULL(CAST(DB28 AS NVARCHAR(50)),'') + ISNULL(CAST(DB29 AS NVARCHAR(50)),'') + ISNULL(CAST(DB30 AS NVARCHAR(50)),'')      AS Auftragsnummer,
                    DB11, DB14, DB15, DB16, DB17, DB18, DB19,
                    DB22, DB23, DB25, DB26, DB27, DB31,
                    UID
                FROM RFID_Aufzeichnung
                WHERE Datum BETWEEN @DatumAb AND @DatumBis
                ORDER BY Datum DESC";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DatumAb", datumAb);
                    command.Parameters.AddWithValue("@DatumBis", datumBis);
                    // Erhöhtes Timeout für große Zeiträume (Standard: 30 s).
                    command.CommandTimeout = 120;

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new System.Data.DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        // Spalten umbenennen, Anzeigereihenfolge festlegen – nur einmal nach dem ersten Laden nötig.
        private void ConfigureColumns()
        {
            var renameMap = new Dictionary<string, string>
            {
                { "DB22", "KST"            },
                { "DB23", "Vor.-Nr."       },
                { "DB25", "Stk."           },
                { "DB26", "Höhe Ausfahrt"  },
                { "DB27", "Waschanlage"    },
                { "DB31", "AVO"            },
                { "DB11", "Ausfahrt UCM"   },
                { "DB17", "Waschpr. UCM"   },
                { "DB18", "Einfahrt UCM"   },
                { "DB19", "Start UCM"      },
                { "DB14", "Waschpr. Aceton"},
                { "DB15", "Einfahrt Aceton"},
                { "DB16", "Ausfahrt Aceton"},
            };

            foreach (var kvp in renameMap)
            {
                if (!dGvRFID.Columns.Contains(kvp.Key)) continue;
                var col        = dGvRFID.Columns[kvp.Key];
                col.HeaderText = kvp.Value;
                col.Name       = kvp.Value;
                // DataPropertyName bleibt unverändert (z. B. "DB22") → Bindung zur DataTable stimmt weiter.
            }

            // Gewünschte Spaltenreihenfolge in der Anzeige.
            var order = new[]
            {
                "Datum","Artikelnummer","Auftragsnummer","AVO","Waschanlage",
                "Waschpr. UCM","Einfahrt UCM","Start UCM","Ausfahrt UCM",
                "Waschpr. Aceton","Einfahrt Aceton","Ausfahrt Aceton"
            };
            int idx = 0;
            foreach (var name in order)
                if (dGvRFID.Columns.Contains(name))
                    dGvRFID.Columns[name].DisplayIndex = idx++;

            // None = keine automatische Breitenberechnung beim Scrollen (AllCells war sehr langsam).
            dGvRFID.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Filter-Logik
        // -----------------------------------------------------------------------------------------------------------------

        private void TextFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled       = true;
            e.SuppressKeyPress = true;
            ApplyCombinedFilter();
        }

        private void DateFilterChanged_ReloadFromDb(object sender, EventArgs e) => LoadRFIDData();

        // Baut den RowFilter aus allen aktiven Eingabefeldern zusammen (AND-Verknüpfung).
        // Der Datumsbereich wird bereits in SQL gefiltert → kein Datums-RowFilter nötig.
        private void ApplyCombinedFilter()
        {
            if (_rfidView == null) return;

            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(txtBoxArtikelnummer.Text))
                parts.Add($"(Artikelnummer LIKE '%{EscapeForRowFilter(txtBoxArtikelnummer.Text.Trim())}%')");

            if (!string.IsNullOrWhiteSpace(txtBoxAuftragsnummer.Text))
                parts.Add($"(Auftragsnummer LIKE '%{EscapeForRowFilter(txtBoxAuftragsnummer.Text.Trim())}%')");

            if (!string.IsNullOrWhiteSpace(txtBoxWaschprogramm.Text))
                parts.Add($"(CONVERT(DB17, 'System.String') LIKE '%{EscapeForRowFilter(txtBoxWaschprogramm.Text.Trim())}%')");

            if (!string.IsNullOrWhiteSpace(txtBoxWaschprogrammAceton.Text))
                parts.Add($"(CONVERT(DB14, 'System.String') LIKE '%{EscapeForRowFilter(txtBoxWaschprogrammAceton.Text.Trim())}%')");

            if (!string.IsNullOrWhiteSpace(txtBoxUID.Text) && _rfidTable.Columns.Contains("UID"))
                parts.Add($"(CONVERT(UID, 'System.String') LIKE '%{EscapeForRowFilter(txtBoxUID.Text.Trim())}%')");

            // Waschanlagen-Checkbox → DB27-Filter.
            if (_rfidTable.Columns.Contains("DB27") && cListBoxWaschanlage.CheckedItems.Count > 0)
            {
                var codes = new List<string>();
                foreach (var item in cListBoxWaschanlage.CheckedItems)
                {
                    switch (item.ToString())
                    {
                        case "UCM497": codes.Add("'FCD1'"); break;
                        case "Aceton": codes.Add("'ACET'"); break;
                    }
                }
                if (codes.Count > 0)
                    parts.Add($"(DB27 IN ({string.Join(",", codes)}))");
            }

            _rfidView.RowFilter = parts.Count > 0 ? string.Join(" AND ", parts) : string.Empty;

            lblEingeleseneWaschkoerbe.Text = $"Eingelesene Waschträger: {_rfidView.Count}";
            UpdateLabels();
        }

        private static string EscapeForRowFilter(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("]", "]]");
        }

        private void btnLoescheFilter_Click(object sender, EventArgs e)
        {
            txtBoxArtikelnummer.Clear();
            txtBoxAuftragsnummer.Clear();
            txtBoxWaschprogramm.Clear();
            txtBoxWaschprogrammAceton.Clear();
            txtBoxUID.Clear();

            for (int i = 0; i < cListBoxWaschanlage.Items.Count; i++)
                cListBoxWaschanlage.SetItemChecked(i, false);

            dateTimePickerDatumAb.Value  = DateTime.Today.AddDays(-30);
            dateTimePickerDatumBis.Value = DateTime.Now;

            LoadRFIDData();
        }

        private void cListBoxWaschanlage_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke(new System.Action(ApplyCombinedFilter));
        }

        // -----------------------------------------------------------------------------------------------------------------
        // KST-Aufschlüsselung in den Detail-Labels aktualisieren.
        // switch statt if-else-Kette → schnellere Auswertung bei vielen Zeilen.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateLabels()
        {
            int count127 = 0, count126 = 0, count124 = 0, count125 = 0, countMontage = 0, countEmptyKST = 0;

            if (_rfidTable?.Columns.Contains("DB22") == true)
            {
                foreach (DataRowView rowView in _rfidView)
                {
                    var val = rowView["DB22"];
                    if (val == DBNull.Value) { countEmptyKST++; continue; }

                    switch (val.ToString())
                    {
                        case "127": case "1127": case "0127": count127++; break;
                        case "126": case "1126": case "0126": count126++; break;
                        case "124": case "1124": case "0124": count124++; break;
                        case "125": case "1125": case "0125": count125++; break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString())) countEmptyKST++;
                            else countMontage++;
                            break;
                    }
                }
            }

            lblEingeleseneWaschkoerbeVerguetung.Text = $"Waschträger Vergütung: {count127}";
            lblEingeleseneWaschkoerbeKitterei.Text   = $"Waschträger Kitterei: {count126}";
            lblEingeleseneWaschkoerbePlanoptik.Text  = $"Waschträger Planoptik: {count124}";
            lblEingeleseneWaschkoerbeRundoptik.Text  = $"Waschträger Rundoptik: {count125}";
            lblEingeleseneWaschkoerbeMontagen.Text   = $"Waschträger Montagen: {countMontage}";
            lblEingelesenOhneZuordnung.Text          = $"Ohne KST: {countEmptyKST}";
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Aktuell angezeigte (gefilterte) Daten als xlsx in Excel öffnen – User speichert selbst.
        // -----------------------------------------------------------------------------------------------------------------
        private async void btnExportXlsx_Click(object sender, EventArgs e)
        {
            if (_rfidView == null || _rfidView.Count == 0)
            {
                MessageBox.Show("Keine Daten zum Exportieren vorhanden.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Sichtbare Spalten in aktueller Anzeigereihenfolge ermitteln.
            var sichtbareSpalten = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in dGvRFID.Columns)
                if (col.Visible) sichtbareSpalten.Add(col);
            sichtbareSpalten.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));

            int spaltenAnzahl = sichtbareSpalten.Count;
            int zeilenAnzahl  = _rfidView.Count;

            string[] headers = new string[spaltenAnzahl];
            for (int c = 0; c < spaltenAnzahl; c++)
                headers[c] = sichtbareSpalten[c].HeaderText;

            // Daten im Hintergrund in ein 2D-Array packen (schneller Bulk-Write nach Excel).
            object[,] data = await Task.Run(() =>
            {
                var arr = new object[zeilenAnzahl, spaltenAnzahl];
                for (int r = 0; r < zeilenAnzahl; r++)
                {
                    DataRowView rowView = _rfidView[r];
                    for (int c = 0; c < spaltenAnzahl; c++)
                    {
                        string prop = sichtbareSpalten[c].DataPropertyName;
                        if (string.IsNullOrEmpty(prop)) continue;
                        object val = rowView[prop];
                        arr[r, c] = val == DBNull.Value ? null : val;
                    }
                }
                return arr;
            });

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook  workbook  = null;
            Worksheet worksheet = null;

            try
            {
                btnExportXlsx.Enabled = false;
                Cursor = Cursors.WaitCursor;

                excelApp         = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                workbook         = excelApp.Workbooks.Add(Type.Missing);
                worksheet        = (Worksheet)workbook.ActiveSheet;
                worksheet.Name   = "RFID Waschanlagen";

                // Header-Zeile formatieren (dunkles Blau / weisse Schrift).
                for (int c = 0; c < spaltenAnzahl; c++)
                {
                    var cell = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, c + 1];
                    cell.Value              = headers[c];
                    cell.Font.Bold          = true;
                    cell.Font.Color         = ColorTranslator.ToOle(Color.White);
                    cell.Interior.Color     = ColorTranslator.ToOle(Color.FromArgb(0x1F, 0x4E, 0x78));
                    cell.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    Marshal.ReleaseComObject(cell);
                }

                ((Microsoft.Office.Interop.Excel.Range)worksheet.Rows[1]).RowHeight = 22;

                // AutoFilter + erste Zeile einfrieren.
                var headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, spaltenAnzahl]];
                headerRange.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
                Marshal.ReleaseComObject(headerRange);
                excelApp.ActiveWindow.SplitRow    = 1;
                excelApp.ActiveWindow.FreezePanes = true;

                // Daten als Block schreiben – deutlich schneller als zellweise.
                if (zeilenAnzahl > 0)
                {
                    var dataRange = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[zeilenAnzahl + 1, spaltenAnzahl]];
                    dataRange.Value = data;
                    Marshal.ReleaseComObject(dataRange);
                }

                // Spaltenbreiten auf Inhalt anpassen.
                var usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();
                Marshal.ReleaseComObject(usedRange);

                excelApp.Visible = true;

                MessageBox.Show(
                    $"Export abgeschlossen – {zeilenAnzahl} Zeilen wurden in Excel geöffnet.\nSpeichern Sie die Datei direkt aus Excel.",
                    "Export abgeschlossen",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { workbook?.Close(false); excelApp?.Quit(); } catch { }
                MessageBox.Show("Fehler beim Excel-Export:\n" + ex.Message, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook  != null) Marshal.ReleaseComObject(workbook);
                if (excelApp  != null) Marshal.ReleaseComObject(excelApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                btnExportXlsx.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}

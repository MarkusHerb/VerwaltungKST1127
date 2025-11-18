// -----------------------------
// USING-DIREKTIVEN (kompakt)
// -----------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_RFIDAnsichtWaschanlagen : Form
    {
        // SQL-Verbindung
        private readonly SqlConnection sqlConnectionWaschtragerl = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Waschtragerl;Integrated Security=True;Encrypt=False");

        // Zwischenspeicher für Grid
        private DataTable _rfidTable;
        private DataView _rfidView;

        public Form_RFIDAnsichtWaschanlagen()
        {
            InitializeComponent();

            // Uhrzeit
            TimerDatumUhrzeit.Start();
            TimerDatumUhrzeit.Tick += TimerDatumUhrzeit_Tick;
            UpdateZeitDatum();

            // Grundwert: heute - 30 Tage
            dateTimePickerDatumAb.Value = DateTime.Today.AddDays(-30);

            // Datumsänderungen -> neu aus DB laden
            dateTimePickerDatumAb.ValueChanged += DateFilterChanged_ReloadFromDb;
            dateTimePickerDatumBis.ValueChanged += DateFilterChanged_ReloadFromDb;

            // Textboxen: ENTER löst Filter aus
            txtBoxArtikelnummer.KeyDown += TextFilter_KeyDown;
            txtBoxAuftragsnummer.KeyDown += TextFilter_KeyDown;
            txtBoxWaschprogramm.KeyDown += TextFilter_KeyDown;
            txtBoxUID.KeyDown += TextFilter_KeyDown;

            // Erstes Laden
            LoadRFIDData();
        }

        // -----------------------------
        // Uhrzeit/Datum-Handling
        // -----------------------------
        private void UpdateZeitDatum()
        {
            lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }
        private void TimerDatumUhrzeit_Tick(object sender, EventArgs e) => UpdateZeitDatum();

        // -----------------------------
        // Laden aus DB + Aufbereitung
        // -----------------------------
        private void LoadRFIDData()
        {
            try
            {
                sqlConnectionWaschtragerl.Open();

                string query = "SELECT * FROM RFID_Aufzeichnung WHERE Datum BETWEEN @DatumAb AND @DatumBis";
                using (SqlCommand command = new SqlCommand(query, sqlConnectionWaschtragerl))
                {
                    command.Parameters.AddWithValue("@DatumAb", dateTimePickerDatumAb.Value.Date);
                    command.Parameters.AddWithValue("@DatumBis", dateTimePickerDatumBis.Value.Date.AddDays(1).AddSeconds(-1));

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // --- 1) DB4 + DB5 => Artikelnummer
                        int db4Ordinal = dataTable.Columns.Contains("DB4") ? dataTable.Columns["DB4"].Ordinal : dataTable.Columns.Count;
                        if (!dataTable.Columns.Contains("Artikelnummer"))
                            dataTable.Columns.Add("Artikelnummer", typeof(string));
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string db4 = dataTable.Columns.Contains("DB4") && row["DB4"] != DBNull.Value ? row["DB4"].ToString() : string.Empty;
                            string db5 = dataTable.Columns.Contains("DB5") && row["DB5"] != DBNull.Value ? row["DB5"].ToString() : string.Empty;
                            row["Artikelnummer"] = db4 + db5;
                        }
                        dataTable.Columns["Artikelnummer"].SetOrdinal(Math.Min(db4Ordinal, dataTable.Columns.Count - 1));

                        // --- 2) DB28 + DB29 + DB30 => Auftragsnummer
                        int db28Ordinal = dataTable.Columns.Contains("DB28") ? dataTable.Columns["DB28"].Ordinal : dataTable.Columns.Count;
                        if (!dataTable.Columns.Contains("Auftragsnummer"))
                            dataTable.Columns.Add("Auftragsnummer", typeof(string));
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string db28 = dataTable.Columns.Contains("DB28") && row["DB28"] != DBNull.Value ? row["DB28"].ToString() : string.Empty;
                            string db29 = dataTable.Columns.Contains("DB29") && row["DB29"] != DBNull.Value ? row["DB29"].ToString() : string.Empty;
                            string db30 = dataTable.Columns.Contains("DB30") && row["DB30"] != DBNull.Value ? row["DB30"].ToString() : string.Empty;
                            row["Auftragsnummer"] = db28 + db29 + db30;
                        }
                        dataTable.Columns["Auftragsnummer"].SetOrdinal(Math.Min(db28Ordinal, dataTable.Columns.Count - 1));

                        // --- 3) Grid-Bindung über DataView
                        _rfidTable = dataTable;
                        _rfidView = new DataView(_rfidTable);
                        dGvRFID.AutoGenerateColumns = true;
                        dGvRFID.DataSource = _rfidView;

                        // --- 4) Spalten ausblenden
                        string[] toHide =
                        {
                            "ID","DB6","DB7","DB8","DB9","DB10","DB12","DB13","DB24",
                            "DB20","DB21","Gesendet","DB4","DB5","DB28","DB29","DB30"
                        };
                        foreach (var colName in toHide)
                        {
                            if (dGvRFID.Columns.Contains(colName)) dGvRFID.Columns[colName].Visible = false;
                        }

                        // --- 5) Spalten für Anzeige umbenennen
                        var renameMap = new Dictionary<string, string>
                        {
                            // allgemeine DBs
                            { "DB22", "KST" },
                            { "DB23", "Vor.-Nr." },
                            { "DB25", "Stk." },
                            { "DB26", "Höhe Ausfahrt" },
                            { "DB27", "Waschanlage" },
                            { "DB31", "AVO" },
                            { "Artikelnummer", "Artikelnummer" },
                            { "Auftragsnummer", "Auftragsnummer" },
                            // UCM
                            { "DB11", "Ausfahrt UCM" },
                            { "DB17", "Waschpr. UCM" },
                            { "DB18", "Einfahrt UCM" },
                            { "DB19", "Start UCM" },
                            // Aceton
                            { "DB14", "Waschpr. Aceton" },
                            { "DB15", "Einfahrt Aceton" },
                            { "DB16", "Ausfahrt Aceton" }
                        };
                        foreach (var kvp in renameMap)
                        {
                            if (dGvRFID.Columns.Contains(kvp.Key))
                            {
                                var col = dGvRFID.Columns[kvp.Key];
                                col.HeaderText = kvp.Value;
                                col.Name = kvp.Value; // nur im Grid (lesbarer)
                            }
                        }

                        // --- 6) Anzeige optimieren
                        dGvRFID.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dGvRFID.AutoResizeColumns();

                        // --- 7) Spaltenreihenfolge
                        var order = new[]
                        {
                            "Datum","Artikelnummer","Auftragsnummer","AVO","Waschanlage",
                            "Waschpr. UCM","Einfahrt UCM","Start UCM","Ausfahrt UCM",
                            "Waschpr. Aceton","Einfahrt Aceton","Ausfahrt Aceton"
                        };
                        int idx = 0;
                        foreach (var n in order)
                            if (dGvRFID.Columns.Contains(n)) dGvRFID.Columns[n].DisplayIndex = idx++;

                        // --- 8) Sortierung nach Datum absteigend
                        if (dGvRFID.Columns.Contains("Datum"))
                            dGvRFID.Sort(dGvRFID.Columns["Datum"], System.ComponentModel.ListSortDirection.Descending);

                        // bereits gesetzte Text-Filter erneut anwenden
                        ApplyCombinedFilter();

                        // --- 9) das Label lblEingeleseneWaschkörbe nach anzahl der eingelesenen Spalten im Dgv aktualisieren
                        lblEingeleseneWaschkoerbe.Text = $"Eingelesene Waschträger: {_rfidView.Count}";

                        // --- 10) das Label lblEingeleseneWaschkoerbeVerguetung (127 oder 1127), lblEingeleseneWaschkoerbeRundoptik (124 oder 1124), lblEingeleseneWaschkoerbeMontagen (sonstige Zahlen), lblEingeleseneWaschkoerbeKitterei, lblEingeleseneWaschkoerbeQS, Ohne zuordnung aktualisieren
                        int count127 = 0;
                        int count126 = 0;
                        int count124 = 0;
                        int countMontage = 0;
                        int countEmptyKST = 0;

                        foreach (DataRow row in _rfidView.ToTable().Rows)
                        {
                            if (row["DB22"] != DBNull.Value)
                            {
                                string kst = row["DB22"].ToString();
                                if (kst == "127" || kst == "1127" || kst == "0127")
                                    count127++;
                                else if (kst == "126" || kst == "1126" || kst == "0126")
                                    count126++;
                                else if (kst == "124" || kst == "1124" || kst == "0124")
                                    count124++;                                
                                // wenn nichts in der Zelle steht countEmtyKst ++
                                else if (string.IsNullOrWhiteSpace(kst))
                                    countEmptyKST++;
                                else
                                    countMontage++;
                            }
                        }
                        // Labels aktualisieren
                        lblEingeleseneWaschkoerbeVerguetung.Text = $"Waschträger Vergütung: {count127}";
                        lblEingeleseneWaschkoerbeKitterei.Text = $"Waschträger Kitterei: {count126}";
                        lblEingeleseneWaschkoerbeRundoptik.Text = $"Waschträger Rundoptik: {count124}";                       
                        lblEingeleseneWaschkoerbeMontagen.Text = $"Waschträger Montagen: {countMontage}";
                        lblEingelesenOhneZuordnung.Text = $"Ohne KST: {countEmptyKST}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der RFID-Daten: " + ex.Message);
            }
            finally
            {
                sqlConnectionWaschtragerl.Close();
            }
        }

        // -----------------------------
        // Filter-Logik (kombinierbar)
        // -----------------------------
        private void TextFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // kein System-Beep
                ApplyCombinedFilter();
            }
        }
        private void DateFilterChanged_ReloadFromDb(object sender, EventArgs e) => LoadRFIDData();

        private void ApplyCombinedFilter()
        {
            if (_rfidView == null) return;

            var parts = new List<string>();

            // 1) Datumsbereich im RowFilter korrekt (US-Format + #...#)
            string dateFrom = dateTimePickerDatumAb.Value.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string dateTo = dateTimePickerDatumBis.Value.Date.AddDays(1).AddSeconds(-1)
                               .ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            parts.Add($"(Datum >= #{dateFrom}# AND Datum <= #{dateTo}#)");

            // 2) Artikelnummer (String)
            if (!string.IsNullOrWhiteSpace(txtBoxArtikelnummer.Text))
            {
                string v = EscapeForRowFilter(txtBoxArtikelnummer.Text.Trim());
                parts.Add($"(Artikelnummer LIKE '%{v}%')");
            }

            // 3) Auftragsnummer (String)
            if (!string.IsNullOrWhiteSpace(txtBoxAuftragsnummer.Text))
            {
                string v = EscapeForRowFilter(txtBoxAuftragsnummer.Text.Trim());
                parts.Add($"(Auftragsnummer LIKE '%{v}%')");
            }

            // 4) Waschprogramm – DB17 kann numerisch sein -> in String wandeln
            if (!string.IsNullOrWhiteSpace(txtBoxWaschprogramm.Text))
            {
                string v = EscapeForRowFilter(txtBoxWaschprogramm.Text.Trim());
                parts.Add($"(CONVERT(DB17, 'System.String') LIKE '%{v}%')");
            }

            // 5) UID – ggf. numerisch -> als String vergleichen
            if (!string.IsNullOrWhiteSpace(txtBoxUID.Text) && _rfidTable.Columns.Contains("UID"))
            {
                string v = EscapeForRowFilter(txtBoxUID.Text.Trim());
                parts.Add($"(CONVERT(UID, 'System.String') LIKE '%{v}%')");
            }

            _rfidView.RowFilter = parts.Count > 0 ? string.Join(" AND ", parts) : string.Empty;

            lblEingeleseneWaschkoerbe.Text = $"Eingelesene Waschträger: {_rfidView.Count}";
        }

        private static string EscapeForRowFilter(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            // ' verdoppeln, eckige Klammern korrekt maskieren
            return input
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("]", "]]");
        }

        // Wenn auf den Button geklickt wird, dann werden alle Filter zurückgesetzt
        private void btnLoescheFilter_Click(object sender, EventArgs e)
        {
            // Dgv auf Zustand zurücksetzten, der beim öffnen war
            txtBoxArtikelnummer.Clear();
            txtBoxAuftragsnummer.Clear();
            txtBoxWaschprogramm.Clear();
            txtBoxUID.Clear();
            dateTimePickerDatumAb.Value = DateTime.Today.AddDays(-30);
            dateTimePickerDatumBis.Value = DateTime.Now;
            LoadRFIDData();
        }
    }
}

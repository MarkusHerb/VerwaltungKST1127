using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VerwaltungKST1127.Farbauswertung
{
    public partial class Form_Farbauswertung : Form
    {
        int currentId;
        private readonly string _connectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Auswertung;Integrated Security=True;Encrypt=False";
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;
        private readonly System.Data.DataTable originalDataTable = new System.Data.DataTable();

        // Cached dates – avoids DateTime.Now.Date per rendered cell in CellFormatting
        private DateTime _cachedToday = DateTime.Now.Date;
        private DateTime _cachedYesterday = DateTime.Now.Date.AddDays(-1);

        // --- Design-Feature 1: Ladeoverlay ---
        private Panel _pnlLoading;
        private Label _lblLoading;

        // --- Design-Feature 3: Statusleiste ---
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusConnection;
        private ToolStripStatusLabel _statusRecordCount;
        private ToolStripStatusLabel _statusLastLoad;

        // --- Design-Feature 5: Anlage-Badge-Farben ---
        private static readonly Dictionary<string, Color> AnlageColors = new Dictionary<string, Color>
        {
            { "20", Color.CornflowerBlue },
            { "25", Color.SteelBlue },
            { "30", Color.MediumSeaGreen },
            { "35", Color.MediumOrchid },
            { "40", Color.Coral },
            { "45", Color.Goldenrod },
            { "50", Color.CadetBlue },
            { "60", Color.IndianRed },
            { "65", Color.MediumPurple }
        };

        public Form_Farbauswertung()
        {
            InitializeComponent();

            Timer1.Start();
            UpdateDateTime();
            DateTimePickerDgv.Value = new DateTime(2021, 1, 4);

            InitializePerformanceCounters();
            InitializeMemoryCounter();
            TimerCpu.Start();
            TimerMemory.Start();

            // Flicker-freies Scrollen im DataGridView
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, DgvFarbauswertung, new object[] { true });

            CreateLoadingOverlay();   // Design 1
            CreateStatusBar();        // Design 3
            StyleAnlageBadges();      // Design 5
        }

        // =====================================================================
        // Design-Feature 1: Ladeoverlay
        // Halbdunkles Panel über dem DataGridView, sichtbar während SQL lädt.
        // =====================================================================
        private void CreateLoadingOverlay()
        {
            _lblLoading = new Label
            {
                Text = "Daten werden geladen...",
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            _pnlLoading = new Panel
            {
                BackColor = Color.FromArgb(55, 55, 55),
                Location = DgvFarbauswertung.Location,
                Size = DgvFarbauswertung.Size,
                Visible = false
            };
            _pnlLoading.Controls.Add(_lblLoading);
            Controls.Add(_pnlLoading);
            _pnlLoading.BringToFront();
        }

        // =====================================================================
        // Design-Feature 3: Statusleiste
        // Zeigt Verbindungsstatus, Datensatzanzahl und letzten Ladezeitpunkt.
        // =====================================================================
        private void CreateStatusBar()
        {
            _statusConnection = new ToolStripStatusLabel
            {
                Text = "● Nicht verbunden",
                ForeColor = Color.OrangeRed,
                Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold)
            };

            _statusRecordCount = new ToolStripStatusLabel
            {
                Text = "Datensätze: –",
                ForeColor = Color.White
            };

            _statusLastLoad = new ToolStripStatusLabel
            {
                Text = "Letztes Laden: –",
                ForeColor = Color.LightGray,
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight
            };

            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(30, 30, 30),
                Dock = DockStyle.Bottom,
                SizingGrip = false
            };
            _statusStrip.Items.Add(_statusConnection);
            _statusStrip.Items.Add(new ToolStripSeparator());
            _statusStrip.Items.Add(_statusRecordCount);
            _statusStrip.Items.Add(new ToolStripSeparator());
            _statusStrip.Items.Add(_statusLastLoad);

            Controls.Add(_statusStrip);
        }

        // =====================================================================
        // Design-Feature 5: Anlage-Badges
        // Vorhandene Anlage-Labels (LblA??Heute / LblA??Vortag) werden als
        // farbige, anklickbare Chips gestylt. Klick setzt den Anlage-Filter.
        // =====================================================================
        private void StyleAnlageBadges()
        {
            var mapping = new[]
            {
                ("20", LblA20Heute, LblA20Vortag),
                ("30", LblA30Heute, LblA30Vortag),
                ("35", LblA35Heute, LblA35Vortag),
                ("40", LblA40Heute, LblA40Vortag),
                ("45", LblA45Heute, LblA45Vortag),
                ("50", LblA50Heute, LblA50Vortag),
                ("60", LblA60Heute, LblA60Vortag),
                ("65", LblA65Heute, LblA65Vortag),
            };

            foreach (var (prefix, lblHeute, lblVortag) in mapping)
            {
                Color badgeColor = AnlageColors.TryGetValue(prefix, out Color c) ? c : Color.Gray;
                StyleBadgeLabel(lblHeute, badgeColor, prefix);
                StyleBadgeLabel(lblVortag, ControlPaint.Dark(badgeColor, 0.3f), prefix);
            }
        }

        private void StyleBadgeLabel(Label lbl, Color backColor, string anlagePrefix)
        {
            lbl.BackColor = backColor;
            lbl.ForeColor = Color.White;
            lbl.Cursor = Cursors.Hand;
            lbl.Padding = new Padding(4, 2, 4, 2);
            lbl.AutoSize = true;

            lbl.Click += (sender, e) =>
            {
                // Toggle: nochmaliger Klick auf dasselbe Anlage hebt den Filter auf
                if (ComboBoxAnlage.SelectedItem?.ToString() == anlagePrefix)
                {
                    ComboBoxAnlage.SelectedIndex = -1;
                    ComboBoxAnlage.Text = string.Empty;
                    ApplyFiltersAndUpdateTotalRowCount();
                }
                else
                {
                    foreach (var item in ComboBoxAnlage.Items)
                    {
                        if (item.ToString() == anlagePrefix)
                        {
                            ComboBoxAnlage.SelectedItem = item;
                            return; // SelectedIndexChanged übernimmt den Rest
                        }
                    }
                    ComboBoxAnlage.Text = anlagePrefix;
                    ApplyFiltersAndUpdateTotalRowCount();
                }
            };
        }

        // =====================================================================
        // Performance-Counter
        // =====================================================================
        private void InitializePerformanceCounters()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        private void InitializeMemoryCounter()
        {
            memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
            _cachedToday = DateTime.Now.Date;
            _cachedYesterday = _cachedToday.AddDays(-1);
        }

        private void TimerCpu_Tick(object sender, EventArgs e)
        {
            if (cpuCounter != null)
                LblCpuUsage.Text = string.Format("CPU-Auslastung:   {0:F1}%", cpuCounter.NextValue());
            else
                MessageBox.Show("cpuCounter wurde nicht initialisiert.");
        }

        private void TimerMemory_Tick(object sender, EventArgs e)
        {
            if (memoryCounter != null)
                LblMemoryUsage.Text = string.Format("RAM-Auslastung:  {0:F1}%", memoryCounter.NextValue());
            else
                MessageBox.Show("memoryCounter wurde nicht initialisiert.");
        }

        // =====================================================================
        // Datenladen – asynchron, UI friert nicht ein
        // =====================================================================

        // Form_Load: async, damit alle abhängigen Initialisierungen
        // erst nach dem Datenladen laufen (FillComboBox, CountChargen).
        private async void Form_Farbauswertung_Load(object sender, EventArgs e)
        {
            LblChargenAb.Text = "Eingelesen ab: 04.01.2021";
            await UpdateDgvFarbauswertungAsync();
            FillComboBoxBelag();
            FillComboBoxProzess();
            CountChargen();
        }

        // Kernmethode – SQL läuft auf Background-Thread, Overlay und
        // Statusleiste werden synchron im UI-Thread aktualisiert.
        public async Task UpdateDgvFarbauswertungAsync()
        {
            try
            {
                if (_pnlLoading != null) _pnlLoading.Visible = true;
                SetConnectionStatus(false, true);
                Cursor = Cursors.WaitCursor;

                System.Data.DataTable table = null;
                await Task.Run(() =>
                {
                    using (var conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        // WITH (NOLOCK): schnellere Lesezugriffe ohne Share-Locks
                        using (var adapter = new SqlDataAdapter(
                            "SELECT * FROM Geamtauswertungstabelle WITH (NOLOCK)", conn))
                        {
                            table = new System.Data.DataTable();
                            adapter.Fill(table);
                        }
                    }
                });

                originalDataTable.Clear();
                originalDataTable.Merge(table);

                DgvFarbauswertung.DataSource = table;
                DgvFarbauswertung.Sort(DgvFarbauswertung.Columns[0], ListSortDirection.Descending);

                ApplyColumnSettings();
                UpdateTotalRowCount();

                SetConnectionStatus(true, false);
                if (_statusLastLoad != null)
                    _statusLastLoad.Text = $"Letztes Laden: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
            }
            catch (Exception ex)
            {
                SetConnectionStatus(false, false);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_pnlLoading != null) _pnlLoading.Visible = false;
                Cursor = Cursors.Default;
            }
        }

        // Öffentliche Wrapper-Methode für externe/fire-and-forget-Aufrufe
        public void UpdateDgvFarbauswertung()
        {
            _ = UpdateDgvFarbauswertungAsync();
        }

        private void SetConnectionStatus(bool connected, bool connecting)
        {
            if (_statusConnection == null) return;
            if (connecting)
            {
                _statusConnection.Text = "● Verbinde...";
                _statusConnection.ForeColor = Color.Yellow;
            }
            else if (connected)
            {
                _statusConnection.Text = "● Verbunden";
                _statusConnection.ForeColor = Color.LimeGreen;
            }
            else
            {
                _statusConnection.Text = "● Verbindungsfehler";
                _statusConnection.ForeColor = Color.OrangeRed;
            }
        }

        // Spaltenbreiten / Sichtbarkeit – einmalig nach dem Datenladen angewendet
        private void ApplyColumnSettings()
        {
            var columnSettings = new (int Index, bool Visible, int Width)[]
            {
                (0, false, 60),  (1, false, 0),   (2, false, 0),
                (3, true, 60),   (4, true, 140),  (5, true, 65),
                (6, false, 0),   (7, true, 65),   (8, true, 65),
                (9, true, 65),   (10, true, 97),  (11, true, 60),
                (12, true, 60),  (13, true, 60),  (14, true, 60),
                (15, true, 60),  (16, true, 70),  (17, false, 0),
                (18, false, 0),  (19, false, 0),  (20, false, 0),
                (21, true, 60),  (22, true, 80),  (23, true, 60),
                (24, true, 60),  (25, true, 60),  (26, true, 75),
                (27, false, 0),  (28, false, 0),  (29, false, 0),
                (30, false, 0),  (31, true, 65),  (32, true, 80),
                (33, false, 0),  (34, false, 0),  (35, true, 110),
                (36, false, 0)
            };

            foreach (var (Index, Visible, Width) in columnSettings)
            {
                if (Index < DgvFarbauswertung.Columns.Count)
                {
                    DgvFarbauswertung.Columns[Index].Visible = Visible;
                    if (Visible)
                        DgvFarbauswertung.Columns[Index].Width = Width;
                }
            }

            DgvFarbauswertung.ColumnHeadersDefaultCellStyle.Font =
                new Font(DataGridView.DefaultFont, FontStyle.Bold);
            DgvFarbauswertung.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
        }

        // ExecuteQuery nutzt pro Aufruf eine eigene Verbindung (thread-sicher)
        public void ExecuteQuery(string query)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(query, conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =====================================================================
        // Chargen-Zählung – direkt aus originalDataTable (nicht DGV-Rows)
        // =====================================================================
        public void CountChargen()
        {
            int countToday = 0;
            int countYesterday = 0;

            var prefixes = new List<string> { "20", "30", "35", "40", "45", "50", "60", "65" };
            var todayPrefixCounts = new Dictionary<string, int>();
            var yesterdayPrefixCounts = new Dictionary<string, int>();
            foreach (var p in prefixes) { todayPrefixCounts[p] = 0; yesterdayPrefixCounts[p] = 0; }

            DateTime currentDate = _cachedToday;
            DateTime previousDate = _cachedYesterday;

            foreach (DataRow row in originalDataTable.Rows)
            {
                if (row["Datum"] == DBNull.Value) continue;
                if (!DateTime.TryParse(row["Datum"].ToString(), out DateTime cellDate)) continue;

                string chargeValue = row["Charge"] == DBNull.Value ? null : row["Charge"].ToString();
                if (chargeValue == null) continue;

                if (cellDate.Date == currentDate)
                {
                    countToday++;
                    CountPrefixes(chargeValue, todayPrefixCounts);
                }
                else if (cellDate.Date == previousDate)
                {
                    countYesterday++;
                    CountPrefixes(chargeValue, yesterdayPrefixCounts);
                }
            }

            LblHeute.Text = "Chargen Heute: " + countToday;
            LblHeute.ForeColor = Color.Orange;
            UpdatePrefixLabels(todayPrefixCounts, "Heute");

            LblVortag.Text = "Chargen gestern: " + countYesterday;
            LblVortag.ForeColor = Color.Yellow;
            UpdatePrefixLabels(yesterdayPrefixCounts, "Vortag");
        }

        private void CountPrefixes(string chargeValue, Dictionary<string, int> prefixCounts)
        {
            foreach (var prefix in prefixCounts.Keys.ToList())
            {
                if (chargeValue.StartsWith(prefix))
                    prefixCounts[prefix]++;
            }
        }

        private void UpdatePrefixLabels(Dictionary<string, int> prefixCounts, string dayLabel)
        {
            foreach (var prefix in prefixCounts)
            {
                if (dayLabel == "Heute")
                {
                    switch (prefix.Key)
                    {
                        case "20": LblA20Heute.Text = $"A20: {prefix.Value}"; break;
                        case "30": LblA30Heute.Text = $"A30: {prefix.Value}"; break;
                        case "35": LblA35Heute.Text = $"A35: {prefix.Value}"; break;
                        case "40": LblA40Heute.Text = $"A40: {prefix.Value}"; break;
                        case "45": LblA45Heute.Text = $"A45: {prefix.Value}"; break;
                        case "50": LblA50Heute.Text = $"A50: {prefix.Value}"; break;
                        case "60": LblA60Heute.Text = $"A60: {prefix.Value}"; break;
                        case "65": LblA65Heute.Text = $"A65: {prefix.Value}"; break;
                    }
                }
                else if (dayLabel == "Vortag")
                {
                    switch (prefix.Key)
                    {
                        case "20": LblA20Vortag.Text = $"A20: {prefix.Value}"; break;
                        case "30": LblA30Vortag.Text = $"A30: {prefix.Value}"; break;
                        case "35": LblA35Vortag.Text = $"A35: {prefix.Value}"; break;
                        case "40": LblA40Vortag.Text = $"A40: {prefix.Value}"; break;
                        case "45": LblA45Vortag.Text = $"A45: {prefix.Value}"; break;
                        case "50": LblA50Vortag.Text = $"A50: {prefix.Value}"; break;
                        case "60": LblA60Vortag.Text = $"A60: {prefix.Value}"; break;
                        case "65": LblA65Vortag.Text = $"A65: {prefix.Value}"; break;
                    }
                }
            }
        }

        // =====================================================================
        // Button-Handler
        // =====================================================================
        private void BtnHeute_Click(object sender, EventArgs e)
        {
            DateTimePickerDgv.Value = DateTime.Today;
            DateTimePickerDgvBis.Value = DateTime.Today;
            DgvFarbauswertung.CellFormatting -= DgvFarbauswertung_CellFormatting;
        }

        private void BtnGestern_Click(object sender, EventArgs e)
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            DateTimePickerDgv.Value = yesterday;
            DateTimePickerDgvBis.Value = yesterday;
        }

        private void BtnMonatsAuswertung_Click(object sender, EventArgs e)
        {
            Form_Monatsuebersicht form_Monatsuebersicht = new Form_Monatsuebersicht();
            form_Monatsuebersicht.SetDataFromInputForm(GetDataGridViewData());
            form_Monatsuebersicht.Show();
            DgvFarbauswertung.Sort(DgvFarbauswertung.Columns[0], ListSortDirection.Descending);
        }

        private async void BtnClearAllFilters_Click(object sender, EventArgs e)
        {
            ComboBoxAnlage.SelectedIndex = -1;
            ComboBoxBelag.SelectedIndex = -1;
            ComboBoxProzess.SelectedIndex = -1;
            ComboBoxAnlage.Text = string.Empty;
            ComboBoxBelag.Text = string.Empty;
            ComboBoxProzess.Text = string.Empty;
            DateTimePickerDgv.Value = DateTime.Now;
            DateTimePickerDgvBis.Value = DateTime.Now;

            await UpdateDgvFarbauswertungAsync();
            UpdateTotalRowCount();

            DateTimePickerDgv.Value = new DateTime(2021, 1, 4);
            LblChargenAb.Text = "Chargen ab 04.01.2021: ";

            TextBoxId.Text = string.Empty;
            TextBoxAnlage.Text = string.Empty;
            TextBoxDatum.Text = string.Empty;
            TextBoxCharge.Text = string.Empty;
            TextBoxBelag.Text = string.Empty;
            TextBoxProzess.Text = string.Empty;
            TextBoxTestlinse.Text = string.Empty;
            TextBoxMessgeraet.Text = string.Empty;
            TextBoxXi.Text = string.Empty;
            TextBoxYi.Text = string.Empty;
            TextBoxZi.Text = string.Empty;
            TextBoxCabI.Text = string.Empty;
            TextBoxWinkelI.Text = string.Empty;
            TextBoxMessI.Text = string.Empty;
            TextBoxXa.Text = string.Empty;
            TextBoxYa.Text = string.Empty;
            TextBoxZa.Text = string.Empty;
            TextBoxCabA.Text = string.Empty;
            TextBoxWinkelA.Text = string.Empty;
            TextBoxMessA.Text = string.Empty;
            TextBoxEgun1.Text = string.Empty;
            TextBoxEgun2.Text = string.Empty;
            TextBoxReinigung.Text = string.Empty;

            DgvFarbauswertung.CellFormatting += DgvFarbauswertung_CellFormatting;

            for (int i = 0; i < ListBoxXYZ.Items.Count; i++)
                ListBoxXYZ.SetItemChecked(i, false);

            CountChargen();
        }

        private void BtnInformaitonMessungen_Click(object sender, EventArgs e)
        {
            Form_InformationZuMessung form_InformationZuMessung = new Form_InformationZuMessung();
            form_InformationZuMessung.Show();
        }

        // =====================================================================
        // DGV-Events
        // =====================================================================
        private void DgvFarbauswertung_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SaveDataForMessurement();
        }

        private void DgvFarbauswertung_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (DgvFarbauswertung.SelectedRows.Count > 0 && e.RowIndex >= 0)
                    PopulateTextBoxesFromRow(DgvFarbauswertung.SelectedRows[0]);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DgvFarbauswertung_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                    PopulateTextBoxesFromRow(DgvFarbauswertung.Rows[e.RowIndex]);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Zusammengeführt aus CellClick + CellContentClick (war doppelter Code)
        private void PopulateTextBoxesFromRow(DataGridViewRow row)
        {
            currentId = (int)row.Cells[0].Value;
            TextBoxId.Text = currentId.ToString();
            TextBoxAnlage.Text = row.Cells[3].Value.ToString();

            if (DateTime.TryParse(row.Cells[4].Value.ToString(), out DateTime parsedDate))
                TextBoxDatum.Text = parsedDate.ToString("dd.MM.yyyy");
            else
                TextBoxDatum.Text = row.Cells[4].Value.ToString();

            TextBoxCharge.Text = row.Cells[5].Value.ToString();
            TextBoxBelag.Text = row.Cells[7].Value.ToString();
            TextBoxProzess.Text = row.Cells[8].Value.ToString();
            TextBoxTestlinse.Text = row.Cells[9].Value.ToString();
            TextBoxMessgeraet.Text = row.Cells[35].Value.ToString();
            TextBoxXi.Text = row.Cells[13].Value.ToString();
            TextBoxYi.Text = row.Cells[14].Value.ToString();
            TextBoxZi.Text = row.Cells[15].Value.ToString();
            TextBoxCabI.Text = row.Cells[21].Value.ToString();
            TextBoxWinkelI.Text = row.Cells[22].Value.ToString();
            TextBoxMessI.Text = row.Cells[16].Value.ToString();
            TextBoxXa.Text = row.Cells[23].Value.ToString();
            TextBoxYa.Text = row.Cells[24].Value.ToString();
            TextBoxZa.Text = row.Cells[25].Value.ToString();
            TextBoxCabA.Text = row.Cells[31].Value.ToString();
            TextBoxWinkelA.Text = row.Cells[32].Value.ToString();
            TextBoxMessA.Text = row.Cells[26].Value.ToString();
            TextBoxEgun1.Text = row.Cells[11].Value.ToString();
            TextBoxEgun2.Text = row.Cells[12].Value.ToString();
            TextBoxReinigung.Text = row.Cells[10].Value.ToString();
        }

        // CellFormatting: mit gecachten Datumswerten – kein DateTime.Now.Date per Zelle
        private void DgvFarbauswertung_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value == null) return;
            if (DgvFarbauswertung.Columns[e.ColumnIndex].Name != "Datum") return;

            if (DateTime.TryParse(e.Value.ToString(), out DateTime cellDate))
            {
                if (cellDate.Date == _cachedToday)
                    DgvFarbauswertung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                else if (cellDate.Date == _cachedYesterday)
                    DgvFarbauswertung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            }
        }

        // =====================================================================
        // ComboBox / Filter
        // =====================================================================

        // FillComboBox liest direkt aus DataTable – iterieren über DGV-Rows ist langsamer
        private void FillComboBoxBelag()
        {
            try
            {
                var uniqueValues = new HashSet<string>();
                foreach (DataRow row in originalDataTable.Rows)
                    if (row[7] != DBNull.Value) uniqueValues.Add(row[7].ToString());
                foreach (string v in uniqueValues) ComboBoxBelag.Items.Add(v);
                ComboBoxBelag.Sorted = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void FillComboBoxProzess()
        {
            try
            {
                var uniqueValues = new HashSet<string>();
                foreach (DataRow row in originalDataTable.Rows)
                    if (row[8] != DBNull.Value) uniqueValues.Add(row[8].ToString());
                foreach (string v in uniqueValues) ComboBoxProzess.Items.Add(v);
                ComboBoxProzess.Sorted = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ApplyFilters()
        {
            try
            {
                string belagFilter = ComboBoxBelag.SelectedItem != null
                    ? $"Belag = '{ComboBoxBelag.SelectedItem}'" : string.Empty;

                string anlageFilter = ComboBoxAnlage.SelectedItem != null
                    ? int.TryParse(ComboBoxAnlage.SelectedItem.ToString(), out int anlage)
                        ? $"Anlage = '{anlage}'"
                        : "1 = 0"
                    : string.Empty;

                string prozessFilter = ComboBoxProzess.SelectedItem != null
                    ? $"Prozess = '{ComboBoxProzess.SelectedItem}'" : string.Empty;

                string datumFilter = DateTimePickerDgv.Checked && DateTimePickerDgvBis.Checked
                    ? $"[Datum] >= #{DateTimePickerDgv.Value.Date:MM/dd/yyyy}# AND [Datum] <= #{DateTimePickerDgvBis.Value.Date:MM/dd/yyyy}#"
                    : string.Empty;

                var filters = new List<string>();
                if (!string.IsNullOrEmpty(belagFilter)) filters.Add(belagFilter);
                if (!string.IsNullOrEmpty(anlageFilter)) filters.Add(anlageFilter);
                if (!string.IsNullOrEmpty(prozessFilter)) filters.Add(prozessFilter);
                if (!string.IsNullOrEmpty(datumFilter)) filters.Add(datumFilter);

                // DataView direkt als DataSource – spart das Erstellen einer neuen DataTable
                var dataView = new DataView(originalDataTable)
                {
                    RowFilter = string.Join(" AND ", filters),
                    Sort = "Datum DESC, Charge DESC"
                };
                DgvFarbauswertung.DataSource = dataView;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void UpdateTotalRowCount()
        {
            int count = DgvFarbauswertung.Rows.Count;
            TextBoxChargenGesamtNachBelag.Text = count.ToString();
            TextBoxChargenGesamtNachBelag.TextAlign = HorizontalAlignment.Center;
            if (_statusRecordCount != null)
                _statusRecordCount.Text = $"Datensätze: {count:N0}";
        }

        private void ApplyFiltersAndUpdateTotalRowCount()
        {
            ApplyFilters();
            UpdateTotalRowCount();
        }

        private void ComboBoxBelag_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
                ApplyFiltersAndUpdateTotalRowCount();
            else { UpdateDgvFarbauswertung(); UpdateTotalRowCount(); }
        }

        private void ComboBoxAnlage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
                ApplyFiltersAndUpdateTotalRowCount();
            else { UpdateDgvFarbauswertung(); UpdateTotalRowCount(); }
        }

        private void ComboBoxProzess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
                ApplyFiltersAndUpdateTotalRowCount();
            else { UpdateDgvFarbauswertung(); UpdateTotalRowCount(); }
        }

        private void ComboBoxBelag_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
                ApplyFiltersAndUpdateTotalRowCount();
            else { UpdateDgvFarbauswertung(); UpdateTotalRowCount(); }
        }

        // Doppelaufruf von ApplyFilters() entfernt – nur noch via AndUpdateTotalRowCount
        private void DateTimePickerDgv_ValueChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndUpdateTotalRowCount();
            LblChargenAb.Text = $"Eingelesen ab {DateTimePickerDgv.Value:dd.MM.yyyy}:";
        }

        private void DateTimePickerDgvBis_ValueChanged(object sender, EventArgs e)
        {
            if (DateTimePickerDgvBis.Value < DateTimePickerDgv.Value)
            {
                DateTimePickerDgvBis.Value = DateTime.Today;
                return;
            }
            ApplyFiltersAndUpdateTotalRowCount();
        }

        // =====================================================================
        // Hilfsmethoden
        // =====================================================================
        private void UpdateDateTime()
        {
            LblTimeDate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Liefert die aktuelle DataTable auch wenn DataSource eine DataView ist
        private System.Data.DataTable GetDataGridViewData()
        {
            if (DgvFarbauswertung.DataSource is System.Data.DataTable dt && dt.Rows.Count > 0)
                return dt;
            if (DgvFarbauswertung.DataSource is DataView dv)
                return dv.ToTable();
            return new System.Data.DataTable();
        }

        private void SaveDataForMessurement()
        {
            Form_Messbild form_Messbild = new Form_Messbild(
                TextBoxDatum.Text, TextBoxCharge.Text,
                TextBoxXi.Text, TextBoxXa.Text,
                TextBoxYi.Text, TextBoxYa.Text,
                TextBoxZi.Text, TextBoxZa.Text,
                TextBoxBelag.Text, TextBoxProzess.Text, TextBoxAnlage.Text);
            form_Messbild.Show();
        }

        // =====================================================================
        // Excel-Export
        // =====================================================================
        private async void BtnExportExcel_Click(object sender, EventArgs e)
        {
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            var excelApp = new Excel.Application();
            try
            {
                DialogResult result = MessageBox.Show(
                    "Das Exportieren kann bei einer großen Datenmenge etwas länger dauern.\n\nKlicken Sie auf 'OK', um fortzufahren, oder auf 'Abbrechen', um den Vorgang abzubrechen.",
                    "Hinweis", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Cancel) return;

                excelApp.Visible = false;
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                int columnCount = 1;
                foreach (DataGridViewColumn column in DgvFarbauswertung.Columns)
                {
                    if (column.Visible)
                    {
                        var headerRange = (Excel.Range)worksheet.Cells[1, columnCount];
                        headerRange.Font.Bold = true;
                        headerRange.Value = column.HeaderText;
                        headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
                        headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.WhiteSmoke);
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        headerRange.Font.Size = 12;
                        Excel.Borders borders = headerRange.Borders;
                        borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        borders[Excel.XlBordersIndex.xlEdgeBottom].Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThick;
                        columnCount++;
                        Marshal.ReleaseComObject(headerRange);
                        Marshal.ReleaseComObject(borders);
                    }
                }

                var headerRangeForFilter = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, columnCount - 1]];
                headerRangeForFilter.AutoFilter(1, Type.Missing, Excel.XlAutoFilterOperator.xlAnd, Type.Missing, true);

                await CopyDataToExcelAsync(worksheet);

                if (ListBoxXYZ.CheckedItems.Contains("X-Werte")) CreateLineChartX(worksheet);
                if (ListBoxXYZ.CheckedItems.Contains("Y-Werte")) CreateLineChartY(worksheet);
                if (ListBoxXYZ.CheckedItems.Contains("Z-Werte")) CreateLineChartZ(worksheet);
                if (ListBoxXYZ.CheckedItems.Contains("C_AB")) CreateLineChartCAB(worksheet);

                MessageBox.Show("Das Excelfile wurde erstellt.\nOk drücken um das File zu öffnen");
                excelApp.Visible = true;
                LblProgress.Text = "0%";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Exportieren der Daten: " + ex.Message);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);
                worksheet = null; workbook = null; excelApp = null;
                GC.Collect(); GC.WaitForPendingFinalizers();
                GC.Collect(); GC.WaitForPendingFinalizers();
            }
        }

        private async Task CopyDataToExcelAsync(Excel.Worksheet worksheet)
        {
            try
            {
                int rowCount = DgvFarbauswertung.Rows.Count;
                int columnCount = DgvFarbauswertung.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);

                ProgressBarImport.Minimum = 0;
                ProgressBarImport.Maximum = rowCount;
                ProgressBarImport.Value = 0;
                ProgressBarImport.Step = 1;

                var data = new object[rowCount, columnCount];

                await Task.Run(() =>
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        int visibleColumnIndex = 0;
                        for (int j = 0; j < DgvFarbauswertung.Columns.Count; j++)
                        {
                            if (DgvFarbauswertung.Columns[j].Visible)
                            {
                                data[i, visibleColumnIndex] = DgvFarbauswertung.Rows[i].Cells[j].Value;
                                visibleColumnIndex++;
                            }
                        }
                        ProgressBarImport.Invoke((System.Action)(() =>
                        {
                            ProgressBarImport.PerformStep();
                            double pct = (double)(i + 1) / rowCount * 100;
                            LblProgress.Text = $"Fortschritt: {pct:F2}% in Zwischenspeicher kopiert!";
                            if (ProgressBarImport.Value == ProgressBarImport.Maximum)
                            {
                                ProgressBarImport.Value = 0;
                                LblProgress.Text = "Daten vollständig in Zwischenspeicher abgelegt. Excel wird erstellt";
                            }
                        }));
                    }
                });

                Excel.Range dataRange = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[rowCount + 1, columnCount]];
                dataRange.Value = data;
                dataRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                Marshal.ReleaseComObject(dataRange);
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Kopieren der Daten in Excel: " + ex.Message); }
        }

        private void CreateLineChartZ(Excel.Worksheet worksheet)
        {
            try
            {
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartObject = chartObjects.Add(400, 40, 600, 300);
                Excel.Chart chart = chartObject.Chart;
                chart.ChartType = Excel.XlChartType.xlLine;
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Excel.Range yRange1 = worksheet.Range["L2:L" + (dataRowCount + 1)];
                Excel.Range yRange2 = worksheet.Range["R2:R" + (dataRowCount + 1)];
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++) xValues[i] = i + 1;
                Excel.Series series1 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues; series1.Values = yRange1; series1.Name = "Z_I";
                Excel.Series series2 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues; series2.Values = yRange2; series2.Name = "Z_A";
                Excel.Trendline trendline1 = (Excel.Trendline)series1.Trendlines().Add();
                trendline1.Type = Excel.XlTrendlineType.xlLinear; trendline1.Name = "Trend_Z_I"; trendline1.Border.Color = Color.DarkBlue;
                Excel.Trendline trendline2 = (Excel.Trendline)series2.Trendlines().Add();
                trendline2.Type = Excel.XlTrendlineType.xlLinear; trendline2.Name = "Trend_Z_A"; trendline2.Border.Color = Color.DarkOrange;
                chart.HasTitle = true; chart.ChartTitle.Text = "Z-Werte";
                Excel.Axis xAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true; xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->        Altäre Chargen";
                Excel.Axis yAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true; yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message); }
        }

        private void CreateLineChartY(Excel.Worksheet worksheet)
        {
            try
            {
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartObject = chartObjects.Add(500, 100, 600, 300);
                Excel.Chart chart = chartObject.Chart;
                chart.ChartType = Excel.XlChartType.xlLine;
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Excel.Range yRange1 = worksheet.Range["K2:K" + (dataRowCount + 1)];
                Excel.Range yRange2 = worksheet.Range["Q2:Q" + (dataRowCount + 1)];
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++) xValues[i] = i + 1;
                Excel.Series series1 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues; series1.Values = yRange1; series1.Name = "Y_I";
                Excel.Series series2 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues; series2.Values = yRange2; series2.Name = "Y_A";
                Excel.Trendline trendline1 = (Excel.Trendline)series1.Trendlines().Add();
                trendline1.Type = Excel.XlTrendlineType.xlLinear; trendline1.Name = "Trend_A_I"; trendline1.Border.Color = Color.DarkBlue;
                Excel.Trendline trendline2 = (Excel.Trendline)series2.Trendlines().Add();
                trendline2.Type = Excel.XlTrendlineType.xlLinear; trendline2.Name = "Trend_A_A"; trendline2.Border.Color = Color.DarkOrange;
                chart.HasTitle = true; chart.ChartTitle.Text = "Y-Werte";
                Excel.Axis xAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true; xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->            Altäre Chargen";
                Excel.Axis yAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true; yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message); }
        }

        private void CreateLineChartX(Excel.Worksheet worksheet)
        {
            try
            {
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartObject = chartObjects.Add(600, 160, 600, 300);
                Excel.Chart chart = chartObject.Chart;
                chart.ChartType = Excel.XlChartType.xlLine;
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Excel.Range yRange1 = worksheet.Range["J2:J" + (dataRowCount + 1)];
                Excel.Range yRange2 = worksheet.Range["P2:P" + (dataRowCount + 1)];
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++) xValues[i] = i + 1;
                Excel.Series series1 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues; series1.Values = yRange1; series1.Name = "X_I";
                Excel.Series series2 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues; series2.Values = yRange2; series2.Name = "X_A";
                Excel.Trendline trendline1 = (Excel.Trendline)series1.Trendlines().Add();
                trendline1.Type = Excel.XlTrendlineType.xlLinear; trendline1.Name = "Trend_X_I"; trendline1.Border.Color = Color.DarkBlue;
                Excel.Trendline trendline2 = (Excel.Trendline)series2.Trendlines().Add();
                trendline2.Type = Excel.XlTrendlineType.xlLinear; trendline2.Name = "Trend_X_A"; trendline2.Border.Color = Color.DarkOrange;
                chart.HasTitle = true; chart.ChartTitle.Text = "X-Werte";
                Excel.Axis xAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true; xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->        Altäre Chargen";
                Excel.Axis yAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true; yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message); }
        }

        private void CreateLineChartCAB(Excel.Worksheet worksheet)
        {
            try
            {
                Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartObject = chartObjects.Add(700, 220, 600, 300);
                Excel.Chart chart = chartObject.Chart;
                chart.ChartType = Excel.XlChartType.xlLine;
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Excel.Range yRange1 = worksheet.Range["N2:N" + (dataRowCount + 1)];
                Excel.Range yRange2 = worksheet.Range["T2:T" + (dataRowCount + 1)];
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++) xValues[i] = i + 1;
                Excel.Series series1 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues; series1.Values = yRange1; series1.Name = "C_AB_I";
                Excel.Series series2 = (Excel.Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues; series2.Values = yRange2; series2.Name = "C_AB_A";
                Excel.Trendline trendline1 = (Excel.Trendline)series1.Trendlines().Add();
                trendline1.Type = Excel.XlTrendlineType.xlLinear; trendline1.Name = "Trend_C_AB_I"; trendline1.Border.Color = Color.DarkBlue;
                Excel.Trendline trendline2 = (Excel.Trendline)series2.Trendlines().Add();
                trendline2.Type = Excel.XlTrendlineType.xlLinear; trendline2.Name = "Trend_C_AB_A"; trendline2.Border.Color = Color.DarkOrange;
                chart.HasTitle = true; chart.ChartTitle.Text = "C_AB-Werte";
                Excel.Axis xAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true; xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->        Altäre Chargen";
                Excel.Axis yAxis = (Excel.Axis)chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true; yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message); }
        }
    }
}

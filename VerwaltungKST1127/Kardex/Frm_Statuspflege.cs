// Frm_Statuspflege.cs
// C# WinForms / .NET Framework 4.8 / C# 7.3
// Alle SQL-Abfragen und die Verbindungsverwaltung sind bewusst IN DIESER DATEI.
// WICHTIG: Diese Datei wurde nicht ausgeführt und nicht gegen eine Datenbank getestet.
// Die SQL-Spaltentypen und Primärschlüssel müssen im Zielsystem geprüft werden.
// In Frm_Statuspflege.Designer.cs muss der Namespace identisch sein.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace VerwaltungKST1127.Kardex
{
    public partial class Frm_Statuspflege : Form
    {
        // KONFIGURATION: Hier die bestehende Verbindungszeichenfolge für SOA127_Shuttle einsetzen.
        // Keine Zugangsdaten fest im Quelltext hinterlegen, wenn dein Programm bereits eine
        // sichere Konfiguration nutzt. Alternativ die ConnectionString-Überladung verwenden.
        // Beispiel ohne Kennwort: "Server=DEIN_SQL_SERVER;Database=SOA127_Shuttle;Integrated Security=True;"
        private const string DefaultShuttleConnectionString = "";

        private readonly string _shuttleConnectionString;
        private readonly Dictionary<int, TextBox> _ring33 = new Dictionary<int, TextBox>();
        private readonly Dictionary<int, TextBox> _ring15 = new Dictionary<int, TextBox>();
        private readonly Dictionary<int, TextBox> _segmente = new Dictionary<int, TextBox>();

        private int? _ring33SlotId;
        private int? _ring15SlotId;
        private int? _segmentSlotId;
        private int? _ring33DetailId;
        private int? _ring15DetailId;
        private int? _ring33Position;
        private int? _ring15Position;
        private int? _segmentPosition;

        public Frm_Statuspflege() : this(DefaultShuttleConnectionString)
        {
        }

        // Zum Einbinden in ein Verwaltungsprogramm mit bereits vorhandener DB-Konfiguration:
        // new Frm_Statuspflege(deinConnectionString).ShowDialog();
        public Frm_Statuspflege(string shuttleConnectionString)
        {
            _shuttleConnectionString = shuttleConnectionString ?? string.Empty;
            InitializeComponent();
            ConfigureControlsAndEvents();
        }

        private void ConfigureControlsAndEvents()
        {
            // WinForms-Designer: keinerlei Runtime-Events oder Datenbankaktivitäten initialisieren.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode) return;

            // Ein Form-Designer sollte niemals eine SQL-Verbindung benötigen.
            for (int i = 1; i <= 33; i++)
            {
                TextBox box = FindControlRecursive(this, "Box_" + i) as TextBox;
                if (box == null) throw new InvalidOperationException("Designer-Steuerelement fehlt: Box_" + i);
                _ring33.Add(i, box);
                int position = i;
                box.ReadOnly = true;
                box.Cursor = Cursors.Hand;
                box.Click += (sender, args) => ExecuteUi(() => SelectPosition('R', position));
            }

            for (int i = 1; i <= 15; i++)
            {
                TextBox box = FindControlRecursive(this, "Box" + i) as TextBox;
                if (box == null) throw new InvalidOperationException("Designer-Steuerelement fehlt: Box" + i);
                _ring15.Add(i, box);
                int position = i;
                box.ReadOnly = true;
                box.Cursor = Cursors.Hand;
                box.Click += (sender, args) => ExecuteUi(() => SelectPosition('G', position));
            }

            for (int i = 1; i <= 6; i++)
            {
                TextBox box = FindControlRecursive(this, "Segmentplatz_" + i) as TextBox;
                if (box == null) throw new InvalidOperationException("Designer-Steuerelement fehlt: Segmentplatz_" + i);
                _segmente.Add(i + 11, box); // Segmentplätze sind in der DB 12 bis 17.
                int position = i + 11;
                box.ReadOnly = true;
                box.Cursor = Cursors.Hand;
                box.Click += (sender, args) => ExecuteUi(() => SelectPosition('S', position));
            }

            Tab_Statuspflege.SelectedIndexChanged += (sender, args) => ExecuteUi(OnTabChanged);
            ComboBox_Tablar_Ringe.SelectedIndexChanged += (sender, args) => ExecuteUi(() => OnTraySelected('R'));
            ComboBox_Tablar_Ringe_15.SelectedIndexChanged += (sender, args) => ExecuteUi(() => OnTraySelected('G'));
            ComboBox_Tablar_Segmente.SelectedIndexChanged += (sender, args) => ExecuteUi(() => OnTraySelected('S'));

            Cmd_Ring_Einlagern.Click += (sender, args) => ExecuteUi(() => LoadAvailableRings(false));
            Cmd_Ring_15_Einlagern.Click += (sender, args) => ExecuteUi(() => LoadAvailableRings(true));
            Cmd_Segment_Einlagern.Click += (sender, args) => ExecuteUi(LoadAvailableSegments);

            DataGridView_Neue_Ringe.CellClick += (sender, args) => ExecuteUi(() => SelectAvailableRing(false, args.RowIndex));
            DataGridView_Neue_Ringe_15.CellClick += (sender, args) => ExecuteUi(() => SelectAvailableRing(true, args.RowIndex));
            DataGridView_Neue_Segmente.CellClick += (sender, args) => ExecuteUi(() => SelectAvailableSegment(args.RowIndex));

            Cmd_Speichern.Click += (sender, args) => ExecuteUi(() => SaveRing(false));
            Cmd_Speichern_15.Click += (sender, args) => ExecuteUi(() => SaveRing(true));
            Cmd_Segment_Speichern.Click += (sender, args) => ExecuteUi(SaveSegment);
            Cmd_Box_Löschen.Click += (sender, args) => ExecuteUi(() => RemoveRing(false));
            Cmd_Box_15_Löschen.Click += (sender, args) => ExecuteUi(() => RemoveRing(true));

            Cmd_Neues_Tablar_Ringe14.Click += (sender, args) => ExecuteUi(() => CreateTray('R'));
            Cmd_Neues_Tablar_Ringe_15_Boxen.Click += (sender, args) => ExecuteUi(() => CreateTray('G'));
            Cmd_Neues_Tablar_Segmente.Click += (sender, args) => ExecuteUi(() => CreateTray('S'));

            Load += (sender, args) =>
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
                ExecuteUi(() =>
                {
                    if (!HasConnectionString()) return;
                    Tab_Statuspflege.SelectedTab = Tab_Ringe;
                    LoadTrayNumbers('R');
                });
            };

            // Die eingefügten Daten sind nur Auswahlwerte; direkte Grid-Änderungen unterbinden.
            foreach (DataGridView grid in new[] { DataGridView_Neue_Ringe, DataGridView_Neue_Ringe_15, DataGridView_Neue_Segmente })
            {
                grid.ReadOnly = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.MultiSelect = false;
                grid.RowHeadersVisible = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }

            ResetDetails('R');
            ResetDetails('G');
            ResetDetails('S');
            ResetColors(_ring33);
            ResetColors(_ring15);
            ResetColors(_segmente);
        }

        private static Control FindControlRecursive(Control parent, string name)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.Name == name) return child;
                Control result = FindControlRecursive(child, name);
                if (result != null) return result;
            }
            return null;
        }

        // Dezente, gut lesbare Statusfarben: Weiß / Grün / Rot / Blau.
        private static void ResetColors(Dictionary<int, TextBox> controls)
        {
            foreach (TextBox box in controls.Values)
            {
                box.BackColor = Color.White;
                box.ForeColor = Color.FromArgb(33, 55, 79);
            }
        }

        private static void PaintPosition(Dictionary<int, TextBox> controls, int position, string status)
        {
            TextBox box;
            if (!controls.TryGetValue(position, out box)) return;
            switch (status)
            {
                case "1":
                    box.BackColor = Color.FromArgb(198, 241, 215);  // im Shuttle
                    box.ForeColor = Color.FromArgb(23, 94, 56);
                    break;
                case "2":
                    box.BackColor = Color.FromArgb(255, 215, 216);  // zugeordnet
                    box.ForeColor = Color.FromArgb(158, 39, 45);
                    break;
                case "3":
                    box.BackColor = Color.FromArgb(214, 230, 252);  // Wartung
                    box.ForeColor = Color.FromArgb(33, 86, 155);
                    break;
                default:
                    box.BackColor = Color.White;
                    box.ForeColor = Color.FromArgb(33, 55, 79);
                    break;
            }
        }

        // Die Auswahlliste schiebt die Detailkarte nach unten, statt sie zu überdecken.
        // Koordinaten beziehen sich auf die TabPages der optimierten Designer-Datei.
        private void SetChooserLayout(char kind, bool show)
        {
            Panel details;
            DataGridView chooser;
            switch (kind)
            {
                case 'R': details = Panel2; chooser = DataGridView_Neue_Ringe; break;
                case 'G': details = Panel4; chooser = DataGridView_Neue_Ringe_15; break;
                case 'S': details = Panel3; chooser = DataGridView_Neue_Segmente; break;
                default: throw new ArgumentException("Ungültiger Tablartyp.");
            }
            details.Top = show ? 494 : 354;
            chooser.Visible = show;
            if (show) chooser.BringToFront();
        }

        private bool HasConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(_shuttleConnectionString)) return true;
            MessageBox.Show(this,
                "Es wurde keine SQL-Server-Verbindungszeichenfolge hinterlegt. " +
                "Trage DefaultShuttleConnectionString in Frm_Statuspflege.cs ein oder " +
                "übergib sie an den Konstruktor. Es wurde keine Verbindung hergestellt.",
                "Shuttle-Verbindung nicht konfiguriert", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void ExecuteUi(Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Statuspflege – Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private SqlConnection NewConnection()
        {
            if (string.IsNullOrWhiteSpace(_shuttleConnectionString))
                throw new InvalidOperationException("Keine Shuttle-Verbindungszeichenfolge konfiguriert.");
            return new SqlConnection(_shuttleConnectionString);
        }

        // Alle SQL-Hilfsfunktionen sind absichtlich Teil dieser Form – keine zusätzliche Datei.
        private static SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        private static string ReadText(IDataRecord record, string field)
        {
            int index = record.GetOrdinal(field);
            return record.IsDBNull(index) ? string.Empty : Convert.ToString(record.GetValue(index), CultureInfo.InvariantCulture);
        }

        private static decimal NumericValue(string text, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            decimal result;
            string normalized = text.Trim().Replace(',', '.');
            if (!decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out result))
                throw new FormatException("Ungültiger Zahlenwert für " + fieldName + ": " + text);
            return result;
        }

        private static int ExecuteNonQuery(SqlConnection conn, SqlTransaction tx, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, conn, tx))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }

        private object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = NewConnection())
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.Parameters.AddRange(parameters);
                conn.Open();
                return command.ExecuteScalar();
            }
        }

        private DataTable ExecuteTable(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = NewConnection())
            using (SqlCommand command = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.Parameters.AddRange(parameters);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private void InTransaction(Action<SqlConnection, SqlTransaction> action)
        {
            using (SqlConnection conn = NewConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        action(conn, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        try { transaction.Rollback(); } catch (SqlException) { /* ursprünglichen Fehler erhalten */ }
                        throw;
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // Tablarauswahl und Statusfarben
        // ---------------------------------------------------------
        private static string TypeName(char kind)
        {
            switch (kind)
            {
                case 'R': return "Ringtablar (33 Positionen)";
                case 'G': return "Ringtablar (15 Positionen)";
                case 'S': return "Segmenttablar";
                default: throw new ArgumentException("Ungültiger Tablartyp.");
            }
        }

        private ComboBox TrayCombo(char kind)
        {
            switch (kind)
            {
                case 'R': return ComboBox_Tablar_Ringe;
                case 'G': return ComboBox_Tablar_Ringe_15;
                case 'S': return ComboBox_Tablar_Segmente;
                default: throw new ArgumentException("Ungültiger Tablartyp.");
            }
        }

        private Dictionary<int, TextBox> Boxes(char kind)
        {
            switch (kind)
            {
                case 'R': return _ring33;
                case 'G': return _ring15;
                case 'S': return _segmente;
                default: throw new ArgumentException("Ungültiger Tablartyp.");
            }
        }

        private void OnTabChanged()
        {
            if (!HasConnectionString()) return;
            if (Tab_Statuspflege.SelectedTab == Tab_Ringe) LoadTrayNumbers('R');
            else if (Tab_Statuspflege.SelectedTab == Tab_Ringe_15) LoadTrayNumbers('G');
            else if (Tab_Statuspflege.SelectedTab == Tab_Segmente) LoadTrayNumbers('S');
            else if (Tab_Statuspflege.SelectedTab == Tab_Übersicht) LoadOverview();
        }

        private void LoadTrayNumbers(char kind)
        {
            ResetDetails(kind);
            ResetColors(Boxes(kind));
            ComboBox combo = TrayCombo(kind);
            combo.BeginUpdate();
            try
            {
                combo.Items.Clear();
                combo.Text = string.Empty;
                DataTable trays = ExecuteTable(
                    "SELECT DISTINCT CAST(TablarNr AS int) AS TablarNr " +
                    "FROM Shuttle_Detail WHERE Flag = @flag ORDER BY TablarNr",
                    Param("@flag", kind.ToString()));
                foreach (DataRow row in trays.Rows)
                    combo.Items.Add(Convert.ToString(row["TablarNr"], CultureInfo.InvariantCulture));
            }
            finally { combo.EndUpdate(); }
        }

        private void OnTraySelected(char kind)
        {
            ResetDetails(kind);
            LoadPositions(kind);
        }

        private void LoadPositions(char kind)
        {
            ResetColors(Boxes(kind));
            string tray = TrayCombo(kind).Text.Trim();
            if (tray.Length == 0) return;
            DataTable rows = ExecuteTable(
                "SELECT Position, Zustand FROM Shuttle_Detail " +
                "WHERE TablarNr = @tablar AND Flag = @flag",
                Param("@tablar", tray), Param("@flag", kind.ToString()));
            foreach (DataRow row in rows.Rows)
            {
                int position;
                if (int.TryParse(Convert.ToString(row["Position"], CultureInfo.InvariantCulture), out position))
                    PaintPosition(Boxes(kind), position, row["Zustand"] == DBNull.Value ? "0" : Convert.ToString(row["Zustand"], CultureInfo.InvariantCulture));
            }
        }

        private void ResetDetails(char kind)
        {
            if (kind == 'R')
            {
                _ring33SlotId = null; _ring33Position = null; _ring33DetailId = null;
                TextBox_Position.Clear();
                foreach (TextBox t in new[] { TextBox_R_Inhalt, TextBox_R_Stück, TextBox_R_Dm,
                         TextBox_R_frei, TextBox_R_Segment_Dm, TextBox_R_Sandstrahlen }) t.Clear();
                DataGridView_Neue_Ringe.Visible = false;
                SetChooserLayout('R', false);
                Cmd_Ring_Einlagern.Visible = false;
                Cmd_Speichern.Visible = false;
                Cmd_Box_Löschen.Visible = false;
            }
            else if (kind == 'G')
            {
                _ring15SlotId = null; _ring15Position = null; _ring15DetailId = null;
                TextBox_Position_15.Clear();
                foreach (TextBox t in new[] { TextBox_R_15_Inhalt, TextBox_R_15_Stück, TextBox_R_15_DM,
                         TextBox_R_15_Frei, TextBox_R_15_Segment_DM, TextBox_R_15_Sandstrahlen }) t.Clear();
                DataGridView_Neue_Ringe_15.Visible = false;
                SetChooserLayout('G', false);
                Cmd_Ring_15_Einlagern.Visible = false;
                Cmd_Speichern_15.Visible = false;
                Cmd_Box_15_Löschen.Visible = false;
            }
            else if (kind == 'S')
            {
                _segmentSlotId = null; _segmentPosition = null;
                TextBox_Segment_Position.Clear();
                foreach (TextBox t in new[] { TextBox_S_Inhalt, TextBox_S_Loch_Dm,
                         TextBox_S_Segment_Loch, TextBox_S_Bemerkungen, TextBox_S_Sandstrahlen }) t.Clear();
                DataGridView_Neue_Segmente.Visible = false;
                SetChooserLayout('S', false);
                Cmd_Segment_Einlagern.Visible = false;
                Cmd_Segment_Speichern.Visible = false;
            }
        }

        private void SelectPosition(char kind, int position)
        {
            string tray = TrayCombo(kind).Text.Trim();
            if (tray.Length == 0) { MessageBox.Show(this, "Bitte zuerst ein Tablar auswählen."); return; }
            ResetDetails(kind);
            using (SqlConnection conn = NewConnection())
            using (SqlCommand command = new SqlCommand(
                "SELECT * FROM Shuttle_Detail WHERE TablarNr = @tablar " +
                "AND Position = @position AND Flag = @flag", conn))
            {
                command.Parameters.AddRange(new[] { Param("@tablar", tray), Param("@position", position), Param("@flag", kind.ToString()) });
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new InvalidOperationException("Die gewählte Tablarposition ist in der Datenbank nicht vorhanden.");
                    int slotId = Convert.ToInt32(reader["ID"], CultureInfo.InvariantCulture);
                    string inhalt = ReadText(reader, "Inhalt");
                    string status = ReadText(reader, "Zustand");
                    bool isEmpty = (status == "0" || status == "") &&
                        (inhalt == "Leer" || string.IsNullOrEmpty(inhalt));

                    if (kind == 'R')
                    {
                        _ring33SlotId = slotId; _ring33Position = position;
                        TextBox_Position.Text = position.ToString(CultureInfo.InvariantCulture);
                        TextBox_R_Inhalt.Text = inhalt;
                        TextBox_R_Stück.Text = ReadText(reader, "R_Stk");
                        TextBox_R_Dm.Text = ReadText(reader, "R_DM");
                        TextBox_R_frei.Text = ReadText(reader, "R_Frei");
                        TextBox_R_Segment_Dm.Text = ReadText(reader, "R_SegDM");
                        TextBox_R_Sandstrahlen.Text = ReadText(reader, "G_Sandstrahlen");
                        Cmd_Ring_Einlagern.Visible = isEmpty;
                        Cmd_Box_Löschen.Visible = status == "1" && !isEmpty;
                    }
                    else if (kind == 'G')
                    {
                        _ring15SlotId = slotId; _ring15Position = position;
                        TextBox_Position_15.Text = position.ToString(CultureInfo.InvariantCulture);
                        TextBox_R_15_Inhalt.Text = inhalt;
                        TextBox_R_15_Stück.Text = ReadText(reader, "R_Stk");
                        TextBox_R_15_DM.Text = ReadText(reader, "R_DM");
                        TextBox_R_15_Frei.Text = ReadText(reader, "R_Frei");
                        TextBox_R_15_Segment_DM.Text = ReadText(reader, "R_SegDM");
                        TextBox_R_15_Sandstrahlen.Text = ReadText(reader, "G_Sandstrahlen");
                        Cmd_Ring_15_Einlagern.Visible = isEmpty;
                        Cmd_Box_15_Löschen.Visible = status == "1" && !isEmpty;
                    }
                    else
                    {
                        _segmentSlotId = slotId; _segmentPosition = position;
                        TextBox_Segment_Position.Text = position.ToString(CultureInfo.InvariantCulture);
                        TextBox_S_Inhalt.Text = inhalt;
                        TextBox_S_Loch_Dm.Text = ReadText(reader, "S_LochDM");
                        TextBox_S_Segment_Loch.Text = ReadText(reader, "S_Loch");
                        TextBox_S_Bemerkungen.Text = ReadText(reader, "S_Bemerkungen");
                        TextBox_S_Sandstrahlen.Text = ReadText(reader, "G_Sandstrahlen");
                        Cmd_Segment_Einlagern.Visible = isEmpty;
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // Nicht eingelagerte Ringe und Segmente suchen
        // ---------------------------------------------------------
        private void LoadAvailableRings(bool tablar15)
        {
            int? id = tablar15 ? _ring15SlotId : _ring33SlotId;
            if (!id.HasValue) throw new InvalidOperationException("Bitte zuerst eine freie Ringposition auswählen.");
            DataGridView grid = tablar15 ? DataGridView_Neue_Ringe_15 : DataGridView_Neue_Ringe;
            grid.DataSource = ExecuteTable(
                "SELECT Ring_Detail.Boxnummer AS Inhalt, " +
                "Ring_Stamm.Durchmesser_Innen AS [Dm Innen], " +
                "Ring_Stamm.Freidurchmesser AS [Dm Frei], Ring_Detail.Stück, " +
                "Ring_Stamm.Gerundet_SegmentDm AS [DM Segment], Ring_Detail.Ring_Detail_ID " +
                "FROM Ring_Stamm INNER JOIN Ring_Detail ON Ring_Stamm.Ring_ID = Ring_Detail.Ring_ID " +
                "WHERE Ring_Detail.Shuttle_ID = 0 ORDER BY Ring_Detail.Boxnummer");
            if (grid.Columns.Contains("Ring_Detail_ID")) grid.Columns["Ring_Detail_ID"].Visible = false;
            SetChooserLayout(tablar15 ? 'G' : 'R', grid.Rows.Count > 0);
            if (grid.Rows.Count == 0) MessageBox.Show(this, "Es sind keine freien Ringboxen vorhanden.");
        }

        private void LoadAvailableSegments()
        {
            if (!_segmentSlotId.HasValue) throw new InvalidOperationException("Bitte zuerst eine freie Segmentposition auswählen.");
            DataGridView_Neue_Segmente.DataSource = ExecuteTable(
                "SELECT Vorrichtungsnummer AS Inhalt, Lochdurchmesser AS [Dm], " +
                "Loch_Segment AS [Anzahl Loch], Bemerkungen " +
                "FROM Segment_Stamm WHERE Shuttle_ID = 0 ORDER BY Vorrichtungsnummer");
            SetChooserLayout('S', DataGridView_Neue_Segmente.Rows.Count > 0);
            if (DataGridView_Neue_Segmente.Rows.Count == 0)
                MessageBox.Show(this, "Es sind keine freien Segmente vorhanden.");
        }

        private static string CellText(DataGridViewRow row, string column)
        {
            object value = row.Cells[column].Value;
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        private void SelectAvailableRing(bool tablar15, int rowIndex)
        {
            DataGridView grid = tablar15 ? DataGridView_Neue_Ringe_15 : DataGridView_Neue_Ringe;
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;
            DataGridViewRow row = grid.Rows[rowIndex];
            int ringId = Convert.ToInt32(row.Cells["Ring_Detail_ID"].Value, CultureInfo.InvariantCulture);
            if (tablar15)
            {
                _ring15DetailId = ringId;
                TextBox_R_15_Inhalt.Text = CellText(row, "Inhalt");
                TextBox_R_15_Stück.Text = CellText(row, "Stück");
                TextBox_R_15_DM.Text = CellText(row, "Dm Innen");
                TextBox_R_15_Frei.Text = CellText(row, "Dm Frei");
                TextBox_R_15_Segment_DM.Text = CellText(row, "DM Segment");
                TextBox_R_15_Sandstrahlen.Text = "100";
                Cmd_Ring_15_Einlagern.Visible = false;
                Cmd_Speichern_15.Visible = true;
            }
            else
            {
                _ring33DetailId = ringId;
                TextBox_R_Inhalt.Text = CellText(row, "Inhalt");
                TextBox_R_Stück.Text = CellText(row, "Stück");
                TextBox_R_Dm.Text = CellText(row, "Dm Innen");
                TextBox_R_frei.Text = CellText(row, "Dm Frei");
                TextBox_R_Segment_Dm.Text = CellText(row, "DM Segment");
                TextBox_R_Sandstrahlen.Text = "100";
                Cmd_Ring_Einlagern.Visible = false;
                Cmd_Speichern.Visible = true;
            }
        }

        private void SelectAvailableSegment(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= DataGridView_Neue_Segmente.Rows.Count) return;
            DataGridViewRow row = DataGridView_Neue_Segmente.Rows[rowIndex];
            TextBox_S_Inhalt.Text = CellText(row, "Inhalt");
            TextBox_S_Loch_Dm.Text = CellText(row, "Dm");
            TextBox_S_Segment_Loch.Text = CellText(row, "Anzahl Loch");
            TextBox_S_Bemerkungen.Text = CellText(row, "Bemerkungen");
            TextBox_S_Sandstrahlen.Text = "100";
            Cmd_Segment_Einlagern.Visible = false;
            Cmd_Segment_Speichern.Visible = true;
        }

        // ---------------------------------------------------------
        // Buchungen; beide beteiligten Tabellen immer atomar aktualisieren
        // ---------------------------------------------------------
        private void SaveRing(bool tablar15)
        {
            char kind = tablar15 ? 'G' : 'R';
            int? slotId = tablar15 ? _ring15SlotId : _ring33SlotId;
            int? ringId = tablar15 ? _ring15DetailId : _ring33DetailId;
            int? position = tablar15 ? _ring15Position : _ring33Position;
            if (!slotId.HasValue || !ringId.HasValue || !position.HasValue)
                throw new InvalidOperationException("Bitte zuerst einen freien Platz und eine Ringbox auswählen.");

            string tray = TrayCombo(kind).Text;
            string inhalt = tablar15 ? TextBox_R_15_Inhalt.Text : TextBox_R_Inhalt.Text;
            decimal dm = NumericValue(tablar15 ? TextBox_R_15_DM.Text : TextBox_R_Dm.Text, "Ring-Durchmesser");
            decimal frei = NumericValue(tablar15 ? TextBox_R_15_Frei.Text : TextBox_R_frei.Text, "Freidurchmesser");
            decimal stk = NumericValue(tablar15 ? TextBox_R_15_Stück.Text : TextBox_R_Stück.Text, "Stückzahl");
            decimal segdm = NumericValue(tablar15 ? TextBox_R_15_Segment_DM.Text : TextBox_R_Segment_Dm.Text, "Segment-Durchmesser");
            decimal strahlen = NumericValue(tablar15 ? TextBox_R_15_Sandstrahlen.Text : TextBox_R_Sandstrahlen.Text, "Sandstrahlen");

            InTransaction((conn, tx) =>
            {
                int slotRows = ExecuteNonQuery(conn, tx,
                    "UPDATE Shuttle_Detail SET Inhalt = @inhalt, Zustand = '1', " +
                    "R_DM = @dm, R_Frei = @frei, R_Stk = @stk, R_SegDM = @segdm, G_Sandstrahlen = @strahlen " +
                    "WHERE ID = @id AND TablarNr = @tablar AND Position = @position AND Flag = @flag " +
                    "AND (Zustand = '0' OR Zustand IS NULL) AND (Inhalt = 'Leer' OR Inhalt IS NULL)",
                    Param("@inhalt", inhalt), Param("@dm", dm), Param("@frei", frei), Param("@stk", stk),
                    Param("@segdm", segdm), Param("@strahlen", strahlen), Param("@id", slotId.Value),
                    Param("@tablar", tray), Param("@position", position.Value), Param("@flag", kind.ToString()));
                if (slotRows != 1) throw new InvalidOperationException("Die Tablarposition ist nicht mehr frei oder nicht eindeutig.");
                int ringRows = ExecuteNonQuery(conn, tx,
                    "UPDATE Ring_Detail SET Shuttle_ID = @id WHERE Ring_Detail_ID = @ringId " +
                    "AND (Shuttle_ID = 0 OR Shuttle_ID IS NULL)",
                    Param("@id", slotId.Value), Param("@ringId", ringId.Value));
                if (ringRows != 1) throw new InvalidOperationException("Die Ringbox ist bereits gebucht oder nicht eindeutig.");
            });
            LoadPositions(kind);
            SelectPosition(kind, position.Value);
        }

        private void SaveSegment()
        {
            if (!_segmentSlotId.HasValue || !_segmentPosition.HasValue ||
                string.IsNullOrWhiteSpace(TextBox_S_Inhalt.Text))
                throw new InvalidOperationException("Bitte zuerst einen freien Platz und ein Segment auswählen.");

            int slotId = _segmentSlotId.Value;
            int position = _segmentPosition.Value;
            string tray = ComboBox_Tablar_Segmente.Text;
            string inhalt = TextBox_S_Inhalt.Text;
            decimal lochDm = NumericValue(TextBox_S_Loch_Dm.Text, "Lochdurchmesser");
            decimal loch = NumericValue(TextBox_S_Segment_Loch.Text, "Anzahl Löcher");
            decimal strahlen = NumericValue(TextBox_S_Sandstrahlen.Text, "Sandstrahlen");
            string bemerkungen = TextBox_S_Bemerkungen.Text;

            InTransaction((conn, tx) =>
            {
                int slotRows = ExecuteNonQuery(conn, tx,
                    "UPDATE Shuttle_Detail SET Inhalt = @inhalt, Zustand = '1', " +
                    "S_LochDM = @dm, S_Loch = @loch, S_Bemerkungen = @bem, G_Sandstrahlen = @strahlen " +
                    "WHERE ID = @id AND TablarNr = @tablar AND Position = @position AND Flag = 'S' " +
                    "AND (Zustand = '0' OR Zustand IS NULL) AND (Inhalt = 'Leer' OR Inhalt IS NULL)",
                    Param("@inhalt", inhalt), Param("@dm", lochDm), Param("@loch", loch),
                    Param("@bem", bemerkungen), Param("@strahlen", strahlen),
                    Param("@id", slotId), Param("@tablar", tray), Param("@position", position));
                if (slotRows != 1) throw new InvalidOperationException("Die Segmentposition ist nicht mehr frei oder nicht eindeutig.");
                int segRows = ExecuteNonQuery(conn, tx,
                    "UPDATE Segment_Stamm SET Shuttle_ID = @id " +
                    "WHERE Vorrichtungsnummer = @inhalt AND (Shuttle_ID = 0 OR Shuttle_ID IS NULL)",
                    Param("@id", slotId), Param("@inhalt", inhalt));
                if (segRows != 1) throw new InvalidOperationException("Das Segment ist bereits gebucht oder nicht eindeutig.");
            });
            LoadPositions('S');
            SelectPosition('S', position);
        }

        private void RemoveRing(bool tablar15)
        {
            char kind = tablar15 ? 'G' : 'R';
            int? slotId = tablar15 ? _ring15SlotId : _ring33SlotId;
            int? position = tablar15 ? _ring15Position : _ring33Position;
            if (!slotId.HasValue || !position.HasValue)
                throw new InvalidOperationException("Bitte zuerst eine belegte Ringposition auswählen.");
            string inhalt = tablar15 ? TextBox_R_15_Inhalt.Text : TextBox_R_Inhalt.Text;
            if (string.IsNullOrWhiteSpace(inhalt) || inhalt == "Leer")
                throw new InvalidOperationException("Diese Position enthält keine Ringbox.");
            if (MessageBox.Show(this, "Ringbox " + inhalt + " aus dem Tablar entfernen? " +
                    "Die Ringbox bleibt in Ring_Detail erhalten.", "Auslagern bestätigen",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            string tray = TrayCombo(kind).Text;

            InTransaction((conn, tx) =>
            {
                int slotRows = ExecuteNonQuery(conn, tx,
                    "UPDATE Shuttle_Detail SET Inhalt = 'Leer', Zustand = '0', Ring_Stamm_ID = '0', " +
                    "Artikelnummer = '', R_DM = '0', R_Frei = '0', R_Stk = '0', R_SegDM = '0', G_Sandstrahlen = '0' " +
                    "WHERE ID = @id AND TablarNr = @tablar AND Position = @position AND Flag = @flag " +
                    "AND Zustand = '1' AND Inhalt = @inhalt",
                    Param("@id", slotId.Value), Param("@tablar", tray), Param("@position", position.Value),
                    Param("@flag", kind.ToString()), Param("@inhalt", inhalt));
                if (slotRows != 1) throw new InvalidOperationException("Die Ringposition hat sich geändert oder ist nicht eindeutig.");
                int ringRows = ExecuteNonQuery(conn, tx,
                    "UPDATE Ring_Detail SET Shuttle_ID = 0 WHERE Boxnummer = @inhalt AND Shuttle_ID = @id",
                    Param("@inhalt", inhalt), Param("@id", slotId.Value));
                if (ringRows != 1) throw new InvalidOperationException("Die Ringbox-Zuordnung ist nicht eindeutig. Buchung zurückgesetzt.");
            });
            LoadPositions(kind);
            SelectPosition(kind, position.Value);
        }

        // ---------------------------------------------------------
        // Neues Tablar anlegen: R=33, G=15, S=6 Plätze (12...17)
        // ---------------------------------------------------------
        private void CreateTray(char kind)
        {
            string message = "Neues " + TypeName(kind) + " anlegen?";
            if (MessageBox.Show(this, message, "Tablar erstellen", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            int newTray = 0;
            InTransaction((conn, tx) =>
            {
                // TABLOCKX/HOLDLOCK verhindert gleiche Tablarnummern bei parallelen Neuanlagen.
                using (SqlCommand command = new SqlCommand(
                    "SELECT ISNULL(MAX(CAST(TablarNr AS int)), 0) + 1 " +
                    "FROM Shuttle_Detail WITH (TABLOCKX, HOLDLOCK)", conn, tx))
                {
                    newTray = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
                }
                int first = kind == 'S' ? 12 : 1;
                int last = kind == 'S' ? 17 : (kind == 'G' ? 15 : 33);
                for (int pos = first; pos <= last; pos++)
                {
                    int count = ExecuteNonQuery(conn, tx,
                        "INSERT INTO Shuttle_Detail (TablarNr, Position, Flag, Inhalt, Zustand) " +
                        "VALUES (@tablar, @position, @flag, 'Leer', '0')",
                        Param("@tablar", newTray), Param("@position", pos), Param("@flag", kind.ToString()));
                    if (count != 1) throw new InvalidOperationException("Tablarposition " + pos + " konnte nicht angelegt werden.");
                }
            });
            MessageBox.Show(this, "Tablar " + newTray + " wurde angelegt.", "Tablar erstellen",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (kind == 'R' && Tab_Statuspflege.SelectedTab == Tab_Ringe ||
                kind == 'G' && Tab_Statuspflege.SelectedTab == Tab_Ringe_15 ||
                kind == 'S' && Tab_Statuspflege.SelectedTab == Tab_Segmente)
            {
                LoadTrayNumbers(kind);
                TrayCombo(kind).SelectedItem = newTray.ToString(CultureInfo.InvariantCulture);
            }
            else if (Tab_Statuspflege.SelectedTab == Tab_Übersicht) LoadOverview();
        }

        // ---------------------------------------------------------
        // Übersicht: Datenherkunft wie im ursprünglichen VB-Code
        // ---------------------------------------------------------
        private void LoadOverview()
        {
            TextBox_Anzahl_T_Ringe.Text = ScalarString(
                "SELECT COUNT(DISTINCT TablarNr) FROM Shuttle_Detail WHERE Flag IN ('R', 'G')");
            TextBox_Boxen_Ringe.Text = ScalarString(
                "SELECT COUNT(*) FROM Ring_Detail WHERE Boxnummer LIKE 'R%'");
            TextBox_Summe_Ringe.Text = ScalarString(
                "SELECT ISNULL(SUM(Stück), 0) FROM Ring_Stamm WHERE Vorrichtungsnummer LIKE 'R%'");
            TextBox_Summe_Trapeze.Text = ScalarString(
                "SELECT ISNULL(SUM(Stück), 0) FROM Ring_Stamm WHERE Vorrichtungsnummer LIKE 'T%'");
            TextBox_Anzahl_T_Segmente.Text = ScalarString(
                "SELECT COUNT(DISTINCT TablarNr) FROM Shuttle_Detail WHERE Flag = 'S'");
            TextBox_Summe_Segmente.Text = ScalarString(
                "SELECT COUNT(*) FROM Segment_Stamm");
            TextBox_Summe_Spannzangen.Text = ScalarString(
                "SELECT ISNULL(SUM(Stück), 0) FROM Ring_Stamm WHERE Vorrichtungsnummer LIKE 'Z%'");
            int ringTrays = Convert.ToInt32(TextBox_Anzahl_T_Ringe.Text, CultureInfo.InvariantCulture);
            int segmentTrays = Convert.ToInt32(TextBox_Anzahl_T_Segmente.Text, CultureInfo.InvariantCulture);
            TextBox_Tablar_Gesamt.Text = (ringTrays + segmentTrays).ToString(CultureInfo.InvariantCulture);
        }

        private string ScalarString(string sql)
        {
            object result = ExecuteScalar(sql);
            return result == null || result == DBNull.Value
                ? "0" : Convert.ToString(result, CultureInfo.CurrentCulture);
        }
    }
}

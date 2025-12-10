// -----------------------------
// USING-DIREKTIVEN (mit Kurzbeschreibung)
// -----------------------------
using System;                                             // Grundlegende Typen/Funktionen (DateTime, Exception, Math, etc.)
using System.Collections.Generic;                         // Generische Collections (List<T>, Dictionary<K,V>, ...)
using System.Data;                                        // DataTable/DataSet sowie SqlDbType
using System.Data.SqlClient;                              // SQL Server Datenzugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Drawing;                                     // Farben, Schriftarten, GDI+ Typen
using System.Globalization;                               // Kulturabhängige Formatierung/Parsing (DateTime, Zahlformate)
using System.IO;                                          // Datei- & Verzeichniszugriff (File, Directory, StreamReader/Writer)
using System.Linq;                                        // LINQ-Erweiterungsmethoden (Where, Select, OrderBy, ...)
using System.Text;                                        // Encoding, StringBuilder
using System.Text.RegularExpressions;
using System.Threading.Tasks;                             // Tasks/Parallelisierung (Task, Task.Run, async/await)
using System.Windows.Forms;                               // WinForms-Basis (Form, Controls, Events, ...)
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;    // Chart/Diagramm-Komponenten für WinForms

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_StkVorAvo : Form
    {
        private readonly SqlConnection sqlConnectionVerwaltung =
            new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        private static readonly HashSet<string> VisibleCols = new HashSet<string>(StringComparer.Ordinal)
        {
            "mitm_teilenr","pdno_prodnr","refo_avotext","qplo_sollstk",
            "qcmp_iststk","qcmp2_vorstk","pdsta_prodstat","Belag","Seite","Offen"
        };

        private static readonly Regex BelagRegex = new Regex(@"\b(B[0-9A-Z]+)\b",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static readonly Regex RomanAtEnd = new Regex(@"(?:^|\s)(I{1,3})\s*[,.;:)]?$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public Form_StkVorAvo()
        {
            InitializeComponent();
            this.Load += Form_StkVorAvo_Load;
        }

        private void Form_StkVorAvo_Load(object sender, EventArgs e)
        {
            LoadDataIntoGrid();
        }

        private void LoadDataIntoGrid()
        {
            try
            {
                const string query = @"
                    SELECT 
                        mitm_teilenr,
                        pdno_prodnr, 
                        refo_avotext, 
                        qplo_sollstk, 
                        qcmp_iststk, 
                        qcmp2_vorstk,
                        pdsta_prodstat,
                        opsta_avostat
                    FROM LN_ProdOrders_PRD 
                    WHERE refo_avonr >= 10127000 
                      AND refo_avonr <  10128000
                      AND pdsta_prodstat <> 'Closed'
                      AND opsta_avostat IN ('Active', 'Released')
                      AND (refo_avotext LIKE '%Vorbereiten%' OR refo_avotext LIKE '%vorbereiten%')";

                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(query, sqlConnectionVerwaltung))
                {
                    adapter.Fill(dt);
                }

                // Zusatzspalten im DataTable -> Sortierung möglich
                if (!dt.Columns.Contains("Belag")) dt.Columns.Add("Belag", typeof(string));
                if (!dt.Columns.Contains("Seite")) dt.Columns.Add("Seite", typeof(int));

                foreach (DataRow r in dt.Rows)
                {
                    var text = Convert.ToString(r["refo_avotext"]) ?? "";
                    string belag; int seite;
                    ParseBelagUndSeite(text, out belag, out seite);
                    r["Belag"] = belag;
                    r["Seite"] = seite;
                }

                // SORTIERUNG: Belag ↑, Art-Nr. ↓, Seite ↑
                dt.DefaultView.Sort = "Belag ASC, mitm_teilenr DESC, Seite ASC";
                var sorted = dt.DefaultView.ToTable();

                dGvStkVorKst.AllowUserToAddRows = false;
                dGvStkVorKst.AutoGenerateColumns = true;
                dGvStkVorKst.DataSource = sorted;

                // Offen-Spalte im Grid (berechnet)
                var colOffen = new DataGridViewTextBoxColumn { Name = "Offen", HeaderText = "Offen", ReadOnly = true };
                dGvStkVorKst.Columns.Add(colOffen);

                foreach (DataGridViewRow row in dGvStkVorKst.Rows)
                {
                    if (row.IsNewRow) continue;

                    var istCell = row.Cells["qcmp_iststk"];
                    var vorCell = row.Cells["qcmp2_vorstk"];
                    var sollCell = row.Cells["qplo_sollstk"];
                    if (istCell == null || vorCell == null || istCell.Value == null || vorCell.Value == null)
                    {
                        row.Cells["Offen"].Value = "";
                        continue;
                    }

                    decimal ist = SafeToDecimal(istCell.Value);
                    decimal vor = SafeToDecimal(vorCell.Value);
                    decimal soll = SafeToDecimal(sollCell.Value);

                    if (vor == 0 && ist > 0)
                    {
                        row.Cells["Offen"].Value = "Zukauf"; // warum: Sonderfall hervorheben
 
                        //Wenn Zukauf in Spalte Offen geschrieben wird, gleich folgende formartierung: Wert aus qplo_sollstk - ist in green und bold; Zukauf löschen und die Summe in Offen schreiben
                        if ((string)row.Cells["Offen"].Value == "Zukauf")
                        {
                            row.Cells["Offen"].Value = soll - ist;
                            row.Cells["Offen"].Style.ForeColor = Color.Green;
                            row.Cells["Offen"].Style.Font = new Font(dGvStkVorKst.Font, FontStyle.Bold);
                        }
                    }
                    else 
                    {
                        decimal offen = vor - ist;
                        row.Cells["Offen"].Value = offen;

                        if (offen < 0)
                        {
                            row.Cells["Offen"].Style.ForeColor = Color.Red;
                            row.Cells["Offen"].Style.Font = new Font(dGvStkVorKst.Font, FontStyle.Bold);
                        }
                        else if (offen > 0)
                        {
                            row.Cells["Offen"].Style.ForeColor = Color.Green;
                            row.Cells["Offen"].Style.Font = new Font(dGvStkVorKst.Font, FontStyle.Bold);
                        }
                        else
                        {
                            row.Cells["Offen"].Style.ForeColor = Color.Black;
                            row.Cells["Offen"].Style.Font = dGvStkVorKst.Font;
                        }
                    }
                }

                // Sichtbarkeit
                foreach (DataGridViewColumn col in dGvStkVorKst.Columns)
                    col.Visible = VisibleCols.Contains(col.Name);

                // Header
                dGvStkVorKst.Columns["mitm_teilenr"].HeaderText = "Art-Nr.";
                dGvStkVorKst.Columns["pdno_prodnr"].HeaderText = "Auftragsnummer";
                dGvStkVorKst.Columns["refo_avotext"].HeaderText = "Text";
                dGvStkVorKst.Columns["qplo_sollstk"].HeaderText = "Soll";
                dGvStkVorKst.Columns["qcmp_iststk"].HeaderText = "Ist";
                dGvStkVorKst.Columns["qcmp2_vorstk"].HeaderText = "Vor";
                dGvStkVorKst.Columns["pdsta_prodstat"].HeaderText = "Status";

                // Format/Ausrichtung
                dGvStkVorKst.Columns["qplo_sollstk"].DefaultCellStyle.Format = "N0";
                dGvStkVorKst.Columns["qcmp_iststk"].DefaultCellStyle.Format = "N0";
                dGvStkVorKst.Columns["qcmp2_vorstk"].DefaultCellStyle.Format = "N0";
                dGvStkVorKst.Columns["Offen"].DefaultCellStyle.Format = "N0";
                dGvStkVorKst.Columns["Seite"].DefaultCellStyle.Format = "N0";

                dGvStkVorKst.Columns["qplo_sollstk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvStkVorKst.Columns["qcmp_iststk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvStkVorKst.Columns["qcmp2_vorstk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvStkVorKst.Columns["Seite"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGvStkVorKst.Columns["Offen"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Reihenfolge Zusatzspalten
                dGvStkVorKst.Columns["Offen"].DisplayIndex = dGvStkVorKst.Columns.Count - 1;
                dGvStkVorKst.Columns["Seite"].DisplayIndex = dGvStkVorKst.Columns["Offen"].DisplayIndex - 1;
                dGvStkVorKst.Columns["Belag"].DisplayIndex = dGvStkVorKst.Columns["Seite"].DisplayIndex - 1;           
                
                loadLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }

        }

        private static void ParseBelagUndSeite(string text, out string belag, out int seite)
        {
            belag = "";
            seite = 0;
            if (string.IsNullOrWhiteSpace(text)) return;

            var mBelag = BelagRegex.Match(text);
            if (mBelag.Success) belag = mBelag.Groups[1].Value.ToUpperInvariant();

            var mRoman = RomanAtEnd.Match(text);
            if (mRoman.Success)
            {
                var roman = mRoman.Groups[1].Value.ToUpperInvariant();
                if (roman == "I") seite = 1;
                else if (roman == "II") seite = 2;
                else if (roman == "III") seite = 0; // Anforderung: leer/III = 0
            }
        }

        private static decimal SafeToDecimal(object value)
        {
            if (value == null || value is DBNull) return 0m;
            try { return Convert.ToDecimal(value); } catch { return 0m; }
        }

        private void loadLabels()
        {
            // eingefügte Labels aktualisieren
            // das lblOffenStk soll Summe aus allen Positivien Zahlen summieren und dann anzeigen
            decimal gesamtOffen = 0m;
            foreach (DataGridViewRow row in dGvStkVorKst.Rows)
            {
                if (row.IsNewRow) continue;
                var offenCell = row.Cells["Offen"];
                if (offenCell != null && offenCell.Value != null)
                {
                    if (offenCell.Value is string && (string)offenCell.Value == "Zukauf")
                    {
                        // Sonderfall Zukauf nicht in Summe aufnehmen
                    }
                    else
                    {
                        decimal offen = SafeToDecimal(offenCell.Value);
                        if (offen > 0)
                        {
                            gesamtOffen += offen;
                        }
                    }
                }
            }
            lblOffenStk.Text = $"Gesamt offen: {gesamtOffen:N0} Stk";

            // das lblOffen1 soll aktualisiert werden mit der gesamten gutmenge, wenn bei der Spalte "Seite " eine 1 steht
            decimal gesamtOffenSeite1 = 0m;
            foreach (DataGridViewRow row in dGvStkVorKst.Rows)
            {
                if (row.IsNewRow) continue;
                var offenCell = row.Cells["Offen"];
                var seiteCell = row.Cells["Seite"];
                if (offenCell != null && offenCell.Value != null && seiteCell != null && seiteCell.Value != null)
                {
                    int seite = (int)SafeToDecimal(seiteCell.Value);
                    if (seite == 1)
                    {
                        if (offenCell.Value is string && (string)offenCell.Value == "Zukauf")
                        {
                            // Sonderfall Zukauf nicht in Summe aufnehmen
                        }
                        else
                        {
                            decimal offen = SafeToDecimal(offenCell.Value);
                            if (offen > 0)
                            {
                                gesamtOffenSeite1 += offen;
                            }
                        }
                    }
                }
            }
            lblOffen1.Text = $"Offen Seite 1: {gesamtOffenSeite1:N0} Stk";

            // das lblOffen2 soll aktualisiert werden mit der gesamten gutmenge, wenn bei der Spalte "Seite " eine 2 steht
            decimal gesamtOffenSeite2 = 0m;
            foreach (DataGridViewRow row in dGvStkVorKst.Rows)
            {
                if (row.IsNewRow) continue;
                var offenCell = row.Cells["Offen"];
                var seiteCell = row.Cells["Seite"];
                if (offenCell != null && offenCell.Value != null && seiteCell != null && seiteCell.Value != null)
                {
                    int seite = (int)SafeToDecimal(seiteCell.Value);
                    if (seite == 2)
                    {
                        if (offenCell.Value is string && (string)offenCell.Value == "Zukauf")
                        {
                            // Sonderfall Zukauf nicht in Summe aufnehmen
                        }
                        else
                        {
                            decimal offen = SafeToDecimal(offenCell.Value);
                            if (offen > 0)
                            {
                                gesamtOffenSeite2 += offen;
                            }
                        }
                    }
                }
            }
            lblOffen2.Text = $"Offen Seite 2: {gesamtOffenSeite2:N0} Stk";

            // das lblOffen0 soll aktualisiert werden mit der gesamten gutmenge, wenn bei der Spalte "Seite " eine 0 steht
            decimal gesamtOffenSeite0 = 0m;
            foreach (DataGridViewRow row in dGvStkVorKst.Rows)
            {
                if (row.IsNewRow) continue;
                var offenCell = row.Cells["Offen"];
                var seiteCell = row.Cells["Seite"];
                if (offenCell != null && offenCell.Value != null && seiteCell != null && seiteCell.Value != null)
                {
                    int seite = (int)SafeToDecimal(seiteCell.Value);
                    if (seite == 0)
                    {
                        if (offenCell.Value is string && (string)offenCell.Value == "Zukauf")
                        {
                            // Sonderfall Zukauf nicht in Summe aufnehmen
                        }
                        else
                        {
                            decimal offen = SafeToDecimal(offenCell.Value);
                            if (offen > 0)
                            {
                                gesamtOffenSeite0 += offen;
                            }
                        }
                    }
                }
            }
            lblOffen0.Text = $"Offen Seite 0: {gesamtOffenSeite0:N0} Stk";                      
        }


    }
}
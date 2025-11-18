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
using System.Threading.Tasks;                             // Tasks/Parallelisierung (Task, Task.Run, async/await)
using System.Windows.Forms;                               // WinForms-Basis (Form, Controls, Events, ...)
using System.Windows.Forms.DataVisualization.Charting;    // Chart/Diagramm-Komponenten für WinForms

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_StkVorAvo : Form
    {
        // Verbindungszeichenfolge für die SQL Server-Datenbank Verwaltung
        private readonly SqlConnection sqlConnectionVerwaltung = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Verwaltung2022;Integrated Security=True;Encrypt=False");

        public Form_StkVorAvo()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form_StkVorAvo_Load);
        }

        private void Form_StkVorAvo_Load(object sender, EventArgs e)
        {
            LoadDataIntoGrid();
        }

        private void LoadDataIntoGrid()
        {
            try
            {
                // Schnellste Variante für Swarovski: letzte 5 Stellen = AVO-Nummer
                string query = @"
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
              AND (refo_avotext LIKE '%Vorbereiten%' 
                OR refo_avotext LIKE '%vorbereiten%')";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnectionVerwaltung))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dGvStkVorKst.AllowUserToAddRows = false;
                    dGvStkVorKst.DataSource = dt;
                    // Neue Spalte "Offen" hinzufügen
                    DataGridViewTextBoxColumn colOffen = new DataGridViewTextBoxColumn();
                    colOffen.Name = "Offen";
                    colOffen.HeaderText = "Offen";
                    colOffen.ReadOnly = true;
                    dGvStkVorKst.Columns.Add(colOffen);

                    // Werte berechnen und eintragen
                    foreach (DataGridViewRow row in dGvStkVorKst.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var istCell = row.Cells["qcmp_iststk"];
                        var vorCell = row.Cells["qcmp2_vorstk"];

                        if (istCell.Value == null || vorCell.Value == null)
                        {
                            row.Cells["Offen"].Value = "";
                            continue;
                        }

                        decimal ist = Convert.ToDecimal(istCell.Value);
                        decimal vor = Convert.ToDecimal(vorCell.Value);

                        if (vor == 0 && ist > 0)
                        {
                            row.Cells["Offen"].Value = "Zukauf";
                            row.Cells["Offen"].Style.ForeColor = Color.Red;         // optional: rot markieren
                            row.Cells["Offen"].Style.Font = new Font(dGvStkVorKst.Font, FontStyle.Bold);
                        }
                        else
                        {
                            decimal offen = vor - ist;
                            row.Cells["Offen"].Value = offen;

                            // Optional: negativ = rot, positiv = grün, 0 = schwarz
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
                                row.Cells["Offen"].Style.ForeColor = Color.Black;
                        }
                    }

                    // Spalte "Offen" ganz rechts stellen
                    dGvStkVorKst.Columns["Offen"].DisplayIndex = dGvStkVorKst.Columns.Count - 1;

                    // Rechtsbündig + Formatierung ohne Nachkommastellen
                    dGvStkVorKst.Columns["Offen"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dGvStkVorKst.Columns["Offen"].DefaultCellStyle.Format = "N0";

                    // Nur gewünschte Spalten anzeigen
                    foreach (DataGridViewColumn col in dGvStkVorKst.Columns)
                    {
                        col.Visible = col.Name == "mitm_teilenr" ||
                                      col.Name == "pdno_prodnr" ||
                                      col.Name == "refo_avotext" ||
                                      col.Name == "qplo_sollstk" ||
                                      col.Name == "qcmp_iststk" ||
                                      col.Name == "pdsta_prodstat" ||
                                      col.Name == "Offen" ||
                                      col.Name == "qcmp2_vorstk";
                    }

                    // ---------- Deutsche Überschriften ----------
                    dGvStkVorKst.Columns["mitm_teilenr"].HeaderText = "Art-Nr.";
                    dGvStkVorKst.Columns["pdno_prodnr"].HeaderText = "Auftragsnummer";   // oder "pdno_prodr" je nach Alias oben
                    dGvStkVorKst.Columns["refo_avotext"].HeaderText = "Text";
                    dGvStkVorKst.Columns["qplo_sollstk"].HeaderText = "Soll";
                    dGvStkVorKst.Columns["qcmp_iststk"].HeaderText = "Ist";
                    dGvStkVorKst.Columns["qcmp2_vorstk"].HeaderText = "Vor";
                    dGvStkVorKst.Columns["pdsta_prodstat"].HeaderText = "Status";

                    // ---------- Formatierung der Mengenspalten (keine Nullen mehr!) ----------
                    dGvStkVorKst.Columns["qplo_sollstk"].DefaultCellStyle.Format = "N0";   // N0 = Tausendertrennzeichen, keine Nachkommastellen
                    dGvStkVorKst.Columns["qcmp_iststk"].DefaultCellStyle.Format = "N0";
                    dGvStkVorKst.Columns["qcmp2_vorstk"].DefaultCellStyle.Format = "N0";

                    // ---------- Rechtsbündig (sieht bei Zahlen besser aus)
                    dGvStkVorKst.Columns["qplo_sollstk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dGvStkVorKst.Columns["qcmp_iststk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dGvStkVorKst.Columns["qcmp2_vorstk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }
    }
}
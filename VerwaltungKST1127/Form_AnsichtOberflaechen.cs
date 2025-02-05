using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Drawing; // Importieren des System.Diagnostics-Namespace für prozessbezogene Operationen
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für GUI-Funktionalität
using System.Windows.Forms.DataVisualization.Charting; // Um SQL funktionen zu verwenden

namespace VerwaltungKST1127
{
    public partial class Form_AnsichtOberflaechen : Form
    {
        public Form_AnsichtOberflaechen()
        {
            InitializeComponent();
            UpdateChartOberflaechen();
        }

        // Funktion zum Aktualisieren des Diagramms mit den monatlichen Summen der Oberflächen
        private void UpdateChartOberflaechen()
        {
            // Verbindungszeichenfolge für die SQL Server-Datenbank
            string connectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";
            // Tabellen, die abgefragt werden sollen
            string[] tables = { "Chargenprotokoll20", "Chargenprotokoll30", "Chargenprotokoll35", "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50", "Chargenprotokoll60", "Chargenprotokoll65" };

            // Datenstruktur zum Speichern der monatlichen Summen
            var monthlySums = new Dictionary<string, int>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (string table in tables)
                {
                    // SQL-Abfrage zum Berechnen der monatlichen Summen ab April 2024
                    string query = $@"
                        SELECT 
                            DATEPART(YEAR, Datum) AS Jahr, 
                            DATEPART(MONTH, Datum) AS Monat, 
                            ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) AS Summe
                        FROM {table}
                        WHERE Datum >= '2024-01-03'
                        GROUP BY DATEPART(YEAR, Datum), DATEPART(MONTH, Datum)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Jahr und Monat als Schlüssel für das Dictionary
                                string yearMonth = $"{reader["Jahr"]}-{reader["Monat"]:D2}";
                                int sum = (int)reader["Summe"];

                                // Summen für den jeweiligen Monat hinzufügen oder aktualisieren
                                if (monthlySums.ContainsKey(yearMonth))
                                {
                                    monthlySums[yearMonth] += sum;
                                }
                                else
                                {
                                    monthlySums[yearMonth] = sum;
                                }
                            }
                        }
                    }
                }
            }

            // Daten in das Diagramm einfügen
            ChartOberflaechen.Series.Clear();
            Series series = new Series
            {
                Name = "Oberflächen",
                ChartType = SeriesChartType.Column,
            };
            ChartOberflaechen.Series.Add(series);

            // Datenpunkte zum Diagramm hinzufügen
            foreach (var entry in monthlySums.OrderBy(e => e.Key))
            {
                DataPoint point = new DataPoint();
                point.SetValueXY(entry.Key, entry.Value);
                point.IsValueShownAsLabel = true; // Wert über dem Balken anzeigen
                series.Points.Add(point);
            }

            // Diagramm anpassen
            ChartOberflaechen.ChartAreas[0].AxisX.Title = "Monat";
            ChartOberflaechen.ChartAreas[0].AxisY.Title = "Oberflächen";
            ChartOberflaechen.ChartAreas[0].AxisX.Interval = 1;
            ChartOberflaechen.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            // Hilfslinien entfernen
            ChartOberflaechen.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            ChartOberflaechen.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            ChartOberflaechen.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            ChartOberflaechen.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
        }
    }
}


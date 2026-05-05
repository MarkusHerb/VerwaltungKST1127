// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben können.
// ===================================================================================================
using System.Collections.Generic;                            // Sammlungen wie Dictionary<TKey, TValue>
using System.Data.SqlClient;                                 // SQL-Server-Zugriff (SqlConnection, SqlCommand, SqlDataReader)
using System.Linq;                                           // LINQ-Funktionen (OrderBy, Select, ...)
using System.Drawing;                                        // Grafische Typen (Font, Color)
using System.Windows.Forms;                                  // Windows-Forms (Form, Button, ...)
using System.Windows.Forms.DataVisualization.Charting;       // Diagramm-Steuerelement (Chart, Series, DataPoint)

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127
{
    // public partial class Form_AnsichtOberflaechen : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien aufgeteilt sein (z. B. ...Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_AnsichtOberflaechen : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_AnsichtOberflaechen()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_AnsichtOberflaechen()
        {
            InitializeComponent();         // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)
            UpdateChartOberflaechen();     // 1. Diagramm: monatliche Summen ab März 2024 zeichnen
            UpdateChartBelaegeAnsicht();   // 2. Diagramm: Summen pro Belag (B000–B999) zeichnen
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Erstellt das obere Diagramm: Summe der Stückzahlen pro Monat über alle Chargenprotokoll-Tabellen.
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateChartOberflaechen()
        {
            // Connection-String: Adresse der DB, Datenbankname, Authentifizierungsmodus.
            string connectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

            // Liste aller Tabellen, deren Werte addiert werden sollen.
            string[] tables = {
                "Chargenprotokoll20", "Chargenprotokoll30", "Chargenprotokoll35",
                "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50",
                "Chargenprotokoll60", "Chargenprotokoll65"
            };

            // Dictionary = Nachschlagewerk: Schlüssel "yyyy-MM" → Summe der Stückzahlen.
            var monthlySums = new Dictionary<string, int>();

            // "using" sorgt dafür, dass die DB-Verbindung am Ende sauber geschlossen wird.
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Über jede Tabelle gehen und die monatlichen Summen abfragen.
                foreach (string table in tables)
                {
                    // SQL: pro (Jahr, Monat) die Summe der Spalten Stk1+Stk2+Stk3 berechnen.
                    // ISNULL(..., 0) → wenn die Summe NULL wäre (leere Spalte), nehmen wir 0.
                    // GROUP BY → fasst Zeilen pro Jahr/Monat zusammen.
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
                        // Reader liest die Ergebniszeilen Stück für Stück.
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // reader.Read() liefert true, solange noch eine Zeile übrig ist.
                            while (reader.Read())
                            {
                                // Schlüsselformat "yyyy-MM": :D2 erzwingt 2-stellige Monatsanzeige (01, 02, ...).
                                string yearMonth = $"{reader["Jahr"]}-{reader["Monat"]:D2}";
                                int sum = (int)reader["Summe"];

                                // Wenn dieser Monat schon existiert → addieren, sonst neu eintragen.
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
            } // hier wird die Verbindung automatisch geschlossen

            // ---- Diagramm vorbereiten ----
            ChartOberflaechen.Series.Clear(); // alte Daten entfernen

            // "Object-Initializer": Eigenschaften gleich beim Erzeugen setzen.
            Series series = new Series
            {
                Name = "Oberflächen/Monat ab 03.24",
                ChartType = SeriesChartType.Column // Säulendiagramm
            };
            ChartOberflaechen.Series.Add(series);

            // Datenpunkte hinzufügen – sortiert nach dem Schlüssel (also chronologisch).
            foreach (var entry in monthlySums.OrderBy(e => e.Key))
            {
                DataPoint point = new DataPoint();
                point.SetValueXY(entry.Key, entry.Value); // X = "yyyy-MM", Y = Summe
                point.IsValueShownAsLabel = true;         // Zahl über dem Balken anzeigen
                series.Points.Add(point);
            }

            // ---- Achsen / Layout konfigurieren ----
            ChartOberflaechen.ChartAreas[0].AxisX.Title = "Monat";
            ChartOberflaechen.ChartAreas[0].AxisY.Title = "Oberflächen";
            ChartOberflaechen.ChartAreas[0].AxisX.Interval = 1;          // jede Kategorie beschriften
            ChartOberflaechen.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Beschriftung schräg → mehr Platz

            // Gitterlinien (sowohl Major als auch Minor) ausblenden – cleanere Optik.
            ChartOberflaechen.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            ChartOberflaechen.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            ChartOberflaechen.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            ChartOberflaechen.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Erstellt das untere Diagramm: Summe der Stückzahlen pro Belag (z. B. B103, B146 ...).
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateChartBelaegeAnsicht()
        {
            string connectionString = @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Chargenprotokoll;Integrated Security=True;Encrypt=False";

            string[] tables = {
                "Chargenprotokoll20", "Chargenprotokoll30", "Chargenprotokoll35",
                "Chargenprotokoll40", "Chargenprotokoll45", "Chargenprotokoll50",
                "Chargenprotokoll60", "Chargenprotokoll65"
            };

            // Dictionary: Schlüssel = Belag-Name (z. B. "B103"), Wert = Summe.
            var belagSums = new Dictionary<string, int>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                foreach (string table in tables)
                {
                    // SQL: pro Belag die Summe Stk1+Stk2+Stk3 berechnen.
                    // LIKE 'B[0-9][0-9][0-9]' → genau "B" gefolgt von 3 Ziffern (z. B. B100..B999).
                    string query = $@"
                SELECT 
                    Belag, 
                    ISNULL(SUM(Stk1), 0) + ISNULL(SUM(Stk2), 0) + ISNULL(SUM(Stk3), 0) AS Summe
                FROM {table}
                WHERE Belag LIKE 'B[0-9][0-9][0-9]'
                GROUP BY Belag";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string belag = reader["Belag"].ToString();
                                int sum = (int)reader["Summe"];

                                // Wenn der Belag im Dictionary existiert → addieren, sonst neu anlegen.
                                if (belagSums.ContainsKey(belag))
                                {
                                    belagSums[belag] += sum;
                                }
                                else
                                {
                                    belagSums[belag] = sum;
                                }
                            }
                        }
                    }
                }
            }

            // ---- Diagramm vorbereiten ----
            ChartBelaegeAnsicht.Series.Clear();
            Series series = new Series
            {
                Name = "Oberflächen nach Belag",
                ChartType = SeriesChartType.Column
            };
            ChartBelaegeAnsicht.Series.Add(series);

            // Datenpunkte einfügen, sortiert nach Belag-Name.
            foreach (var entry in belagSums.OrderBy(e => e.Key))
            {
                DataPoint point = new DataPoint();
                point.SetValueXY(entry.Key, entry.Value);
                point.IsValueShownAsLabel = true;

                // Beschriftungsgröße abhängig vom Wert:
                // große Säulen (>20000) bekommen größere Schrift, kleinere bleiben kompakt.
                if (entry.Value > 20000)
                    point.Font = new Font("Arial", 9);
                else
                    point.Font = new Font("Arial", 7);

                series.Points.Add(point);
            }

            // ---- Achsen / Layout konfigurieren ----
            ChartBelaegeAnsicht.ChartAreas[0].AxisX.Title = "Belag";
            ChartBelaegeAnsicht.ChartAreas[0].AxisY.Title = "Oberflächen";
            ChartBelaegeAnsicht.ChartAreas[0].AxisX.Interval = 1;
            ChartBelaegeAnsicht.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

            // Gitterlinien aus.
            ChartBelaegeAnsicht.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            ChartBelaegeAnsicht.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            ChartBelaegeAnsicht.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            ChartBelaegeAnsicht.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
        }
    }
}
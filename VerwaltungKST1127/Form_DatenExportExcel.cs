using Microsoft.Office.Interop.Excel;          // Excel-Interop für Automatisierung (Workbook, Worksheet, Range, ChartObject, ...)
using System;                                  // Basisklassen wie EventArgs, DateTime, Exception
using System.Collections.Generic;              // Generische Collections (List<T>)
using System.ComponentModel;                   // Komponenten-Unterstützung für Designer
using System.Data;                             // DataTable, DataSet
using System.Data.SqlClient;                   // SQL Server Zugriff (SqlConnection, SqlCommand)
using System.Drawing;                          // System.Drawing.Color (für ColorTranslator.ToOle)
using System.IO;                               // File, Directory, Path - für das Speichern in den Zielordner
using System.Linq;                             // LINQ-Erweiterungen
using System.Runtime.InteropServices;          // Marshal.ReleaseComObject für sauberes COM-Cleanup
using System.Text;                             // StringBuilder für die Bestätigungs-Meldungen
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

        // Zielordner für den Sammel-Export der wichtigsten Tabellen
        private const string ZielordnerWichtigeTabellen =
            @"P:\TEDuTOZ\Verschiedenes\Austauschordner_Kst146\Daten_TabellenAusSqlServer";

        // Sammelliste aller ListBoxen, damit wir bequem über sie iterieren können
        private System.Windows.Forms.ListBox[] _alleListBoxen;

        // Schutz-Flag, damit das Zurücksetzen der Auswahl nicht selbst wieder
        // SelectedIndexChanged-Events auslöst (Endlosschleifen-Vermeidung)
        private bool _aktualisiereAuswahl = false;


        // ------------------------------------------------------------------
        // KONFIGURATION für den "Wichtigste Tabellen"-Export
        // ------------------------------------------------------------------
        // HIER eintragen, wenn du eine neue Tabelle hinzufügen willst:
        //   new WichtigeTabelleDefinition(
        //       <ConnectionString>,
        //       "<Schema>",
        //       "<Tabellenname>",
        //       "<Dateiname ohne .xlsx>",
        //       sortSpaltenIndex: <Spaltennummer 1=A, 2=B, 3=C, 4=D, ...>   (optional)
        //       sortAbsteigend:   true=Z->A,  false=A->Z                    (optional, Default false)
        //   )
        //
        // Wird sortSpaltenIndex weggelassen, wird NICHT sortiert.
        // ------------------------------------------------------------------
        private List<WichtigeTabelleDefinition> ErzeugeWichtigeTabellenListe()
        {
            return new List<WichtigeTabelleDefinition>
            {
                // Verwaltung Neu (SOA127_Verwaltung2022)
                new WichtigeTabelleDefinition(ConnectionStringVerwaltung2022, "dbo", "Glassdaten",                   "Glasdaten"),
                new WichtigeTabelleDefinition(ConnectionStringVerwaltung2022, "dbo", "Maximalwerte_Farbauswertung", "MaximalwerteFarbauswertung",
                                              sortSpaltenIndex: 2, sortAbsteigend: false),  // Spalte B aufsteigend (A->Z)
                new WichtigeTabelleDefinition(ConnectionStringVerwaltung2022, "dbo", "Serienlinsen",                "Serienlinsen",
                                              sortSpaltenIndex: 4, sortAbsteigend: false),  // Spalte D aufsteigend (A->Z)

                // Shuttle (SOA127_Shuttle)
                new WichtigeTabelleDefinition(ConnectionStringShuttle,        "dbo", "Ring_Stamm",                  "Ringe",
                                              sortSpaltenIndex: 2, sortAbsteigend: false),  // Spalte B aufsteigend (A->Z)

                // Messen (SOA127_Messen)
                new WichtigeTabelleDefinition(ConnectionStringMessen,         "dbo", "GesamteGruppen",              "Gesamtdaten Messungen",
                                              sortSpaltenIndex: 2, sortAbsteigend: true),   // Spalte B absteigend (Z->A)
            };
        }

        // Eine kleine Klasse, die einen einzelnen Export-Eintrag beschreibt.
        private class WichtigeTabelleDefinition
        {
            public string ConnectionString { get; }
            public string Schema { get; }
            public string Tabelle { get; }
            public string DateiName { get; }      // ohne ".xlsx" – die Endung wird automatisch ergänzt
            public int? SortSpaltenIndex { get; } // 1-basiert; null = nicht sortieren (1=A, 2=B, 3=C, 4=D ...)
            public bool SortAbsteigend { get; }   // false = A->Z, true = Z->A

            public WichtigeTabelleDefinition(
                string connectionString,
                string schema,
                string tabelle,
                string dateiName,
                int? sortSpaltenIndex = null,
                bool sortAbsteigend = false)
            {
                ConnectionString = connectionString;
                Schema = schema;
                Tabelle = tabelle;
                DateiName = dateiName;
                SortSpaltenIndex = sortSpaltenIndex;
                SortAbsteigend = sortAbsteigend;
            }
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

                // 3) Daten laden (ohne Sortierung)
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
        /// Lädt alle Daten der angegebenen Tabelle in eine DataTable (unsortiert).
        /// </summary>
        private System.Data.DataTable LadeTabellenDaten(string connectionString, string schema, string tabelle)
        {
            return LadeTabellenDatenMitSortierung(connectionString, schema, tabelle, null, false);
        }

        /// <summary>
        /// Lädt alle Daten der angegebenen Tabelle in eine DataTable - optional
        /// sortiert nach der angegebenen Spaltenposition (1-basiert, entspricht
        /// Excel-Spaltenbuchstabe: 1=A, 2=B, 3=C, 4=D, ...).
        /// Wird sortSpaltenIndex == null übergeben, erfolgt kein ORDER BY.
        /// </summary>
        private System.Data.DataTable LadeTabellenDatenMitSortierung(
            string connectionString,
            string schema,
            string tabelle,
            int? sortSpaltenIndex,
            bool sortAbsteigend)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            // ORDER BY <ordinale Spaltenposition> ist in SQL Server zulässig
            // und entspricht damit der Excel-Spalte (Spalte 1=A, 2=B, ...).
            string orderByClause = "";
            if (sortSpaltenIndex.HasValue && sortSpaltenIndex.Value > 0)
            {
                orderByClause = $" ORDER BY {sortSpaltenIndex.Value} {(sortAbsteigend ? "DESC" : "ASC")}";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM [{schema}].[{tabelle}]{orderByClause}";

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
            string sicherenName = MachWorksheetNameSicher(blattName);

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
                SchreibeHeaderInWorksheet(worksheet, daten, spaltenAnzahl);

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
                    object[,] data = await Task.Run(() => BaueDatenArray(daten, zeilenAnzahl, spaltenAnzahl));

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


        // ====================================================================
        // SAMMEL-EXPORT der wichtigsten Tabellen in den Austausch-Ordner
        // ====================================================================

        /// <summary>
        /// Klick auf "Wichtigste Tabellen exportieren":
        /// Exportiert alle in <see cref="ErzeugeWichtigeTabellenListe"/> definierten
        /// Tabellen als einzelne .xlsx-Dateien in den Zielordner.
        /// Bestehende Dateien werden überschrieben - außer sie sind gerade geöffnet,
        /// dann wird eine Kopie mit Zeitstempel angelegt.
        /// Eine ProgressBar (progressBarImportantTables) wird von 0 auf 100% gezählt.
        /// </summary>
        private async void btnExportMostImportant_Click(object sender, EventArgs e)
        {
            // 0) ProgressBar zurücksetzen
            progressBarImportantTables.Minimum = 0;
            progressBarImportantTables.Maximum = 100;
            progressBarImportantTables.Value = 0;

            // 1) Liste der wichtigen Tabellen laden
            List<WichtigeTabelleDefinition> wichtigeTabellen = ErzeugeWichtigeTabellenListe();

            // 2) Zielordner prüfen (falls Netzlaufwerk nicht verbunden -> abbrechen)
            if (!Directory.Exists(ZielordnerWichtigeTabellen))
            {
                MessageBox.Show(
                    $"Der Zielordner ist nicht erreichbar:\n{ZielordnerWichtigeTabellen}\n\n" +
                    "Bitte prüfen Sie, ob das Netzlaufwerk verbunden ist.",
                    "Zielordner nicht gefunden",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 3) Zeilenanzahlen aus allen Tabellen ermitteln (für die Bestätigungs-Meldung)
            var zeilenInfos = new List<(WichtigeTabelleDefinition def, int zeilen)>();
            int gesamtZeilen = 0;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                foreach (WichtigeTabelleDefinition def in wichtigeTabellen)
                {
                    int anzahl = ErmittleZeilenAnzahl(def.ConnectionString, def.Schema, def.Tabelle);
                    zeilenInfos.Add((def, anzahl));
                    gesamtZeilen += anzahl;
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Fehler beim Ermitteln der Zeilenanzahlen:\n{ex.Message}",
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            // 4) Bestätigungs-Meldung zusammenbauen und anzeigen
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Es werden {wichtigeTabellen.Count} Excel-Dateien mit insgesamt {gesamtZeilen:N0} Zeilen exportiert:");
            sb.AppendLine();
            foreach (var info in zeilenInfos)
            {
                sb.AppendLine($"   • {info.def.DateiName}.xlsx   ({info.zeilen:N0} Zeilen)");
            }
            sb.AppendLine();
            sb.AppendLine($"Zielordner: {ZielordnerWichtigeTabellen}");
            sb.AppendLine();
            sb.AppendLine("Bestehende Dateien werden überschrieben.");
            sb.AppendLine("Falls eine Datei gerade geöffnet ist, wird eine Kopie mit Zeitstempel angelegt.");
            sb.AppendLine();
            sb.AppendLine("Hinweis: Bei großen Datenmengen kann der Vorgang einige Zeit dauern.");
            sb.AppendLine();
            sb.AppendLine("Möchten Sie den Export jetzt starten?");

            DialogResult antwort = MessageBox.Show(
                sb.ToString(),
                "Wichtigste Tabellen exportieren",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (antwort != DialogResult.Yes) return;

            // 5) Export durchführen - eine Excel-Instanz für ALLE Dateien (deutlich schneller)
            this.Cursor = Cursors.WaitCursor;

            // Buttons während des Exports sperren (ProgressBar bleibt aber bedienbar/sichtbar)
            btnExportMostImportant.Enabled = false;
            btnExportTable.Enabled = false;

            var ergebnisse = new List<string>();
            int erfolgreichCount = 0;
            int totalSchritte = wichtigeTabellen.Count;
            int abgeschlossen = 0;

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;       // wichtig fürs stille Überschreiben
                excelApp.ScreenUpdating = false;      // beschleunigt das Schreiben merklich

                foreach (var info in zeilenInfos)
                {
                    try
                    {
                        // Daten sortiert in einem Hintergrund-Thread laden
                        System.Data.DataTable daten = await Task.Run(() =>
                            LadeTabellenDatenMitSortierung(
                                info.def.ConnectionString,
                                info.def.Schema,
                                info.def.Tabelle,
                                info.def.SortSpaltenIndex,
                                info.def.SortAbsteigend));

                        // Nach dem Laden: halber Schritt fertig
                        SetzeFortschritt(abgeschlossen + 0.5, totalSchritte);

                        // Excel-Export
                        string gespeicherterPfad = await ExportiereEinzelneTabelleAlsDateiAsync(excelApp, daten, info.def);
                        string gespeicherterDateiName = Path.GetFileName(gespeicherterPfad);

                        bool warKopie = !string.Equals(
                            gespeicherterDateiName,
                            info.def.DateiName + ".xlsx",
                            StringComparison.OrdinalIgnoreCase);

                        string suffix = warKopie ? "   (als Kopie – Original ist geöffnet)" : "";
                        ergebnisse.Add($"   ✓  {gespeicherterDateiName}   ({info.zeilen:N0} Zeilen){suffix}");
                        erfolgreichCount++;
                    }
                    catch (Exception ex)
                    {
                        ergebnisse.Add($"   ✗  {info.def.DateiName}.xlsx   – FEHLER: {ex.Message}");
                    }

                    // Schritt vollständig abgeschlossen (egal ob Erfolg oder Fehler)
                    abgeschlossen++;
                    SetzeFortschritt(abgeschlossen, totalSchritte);
                }

                // Sicherheitshalber explizit auf 100% setzen
                progressBarImportantTables.Value = 100;
            }
            finally
            {
                // Excel-Instanz sauber schließen
                try { if (excelApp != null) excelApp.Quit(); } catch { /* ignorieren */ }
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);
                excelApp = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                this.Cursor = Cursors.Default;
                btnExportMostImportant.Enabled = true;
                btnExportTable.Enabled = true;
            }

            // 6) Ergebnis anzeigen
            StringBuilder ergebnisSb = new StringBuilder();
            ergebnisSb.AppendLine($"Export abgeschlossen: {erfolgreichCount} von {wichtigeTabellen.Count} Dateien erfolgreich.");
            ergebnisSb.AppendLine();
            foreach (string r in ergebnisse) ergebnisSb.AppendLine(r);
            ergebnisSb.AppendLine();
            ergebnisSb.AppendLine($"Zielordner: {ZielordnerWichtigeTabellen}");

            MessageBox.Show(
                ergebnisSb.ToString(),
                "Export abgeschlossen",
                MessageBoxButtons.OK,
                erfolgreichCount == wichtigeTabellen.Count ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Setzt den ProgressBar-Wert auf den Anteil <paramref name="aktuell"/> /
        /// <paramref name="gesamt"/> in Prozent (0..100), sicher gekappt.
        /// </summary>
        private void SetzeFortschritt(double aktuell, int gesamt)
        {
            if (gesamt <= 0) { progressBarImportantTables.Value = 0; return; }
            int prozent = (int)Math.Round(aktuell * 100.0 / gesamt);
            if (prozent < 0) prozent = 0;
            if (prozent > 100) prozent = 100;
            progressBarImportantTables.Value = prozent;
        }

        /// <summary>
        /// Exportiert die übergebene DataTable in eine einzelne .xlsx-Datei
        /// im Zielordner. Verwendet die übergebene (gemeinsam genutzte) Excel-Instanz.
        /// Existiert die Zieldatei und ist sie geöffnet, wird stattdessen eine
        /// Kopie mit Zeitstempel angelegt. Liefert den tatsächlich gespeicherten Pfad zurück.
        /// </summary>
        private async Task<string> ExportiereEinzelneTabelleAlsDateiAsync(
            Microsoft.Office.Interop.Excel.Application excelApp,
            System.Data.DataTable daten,
            WichtigeTabelleDefinition def)
        {
            string zielPfad = Path.Combine(ZielordnerWichtigeTabellen, def.DateiName + ".xlsx");

            // Wenn Datei gerade geöffnet ist (z. B. in Excel) -> Kopie mit Zeitstempel anlegen
            if (IstDateiGesperrt(zielPfad))
            {
                string zeitstempel = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                zielPfad = Path.Combine(
                    ZielordnerWichtigeTabellen,
                    $"{def.DateiName}_Kopie_{zeitstempel}.xlsx");
            }

            string blattName = MachWorksheetNameSicher(def.DateiName);

            Workbook workbook = null;
            Worksheet worksheet = null;

            try
            {
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Worksheet)workbook.ActiveSheet;
                worksheet.Name = blattName;

                int spaltenAnzahl = daten.Columns.Count;
                int zeilenAnzahl = daten.Rows.Count;

                // Header schreiben + formatieren
                SchreibeHeaderInWorksheet(worksheet, daten, spaltenAnzahl);

                // AutoFilter auf der Headerzeile
                Microsoft.Office.Interop.Excel.Range headerRangeForFilter =
                    worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, spaltenAnzahl]];
                headerRangeForFilter.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
                Marshal.ReleaseComObject(headerRangeForFilter);

                // Erste Zeile fixieren – funktioniert auch bei nicht-sichtbarem Excel
                try
                {
                    Microsoft.Office.Interop.Excel.Window win = excelApp.Windows[workbook.Name];
                    win.SplitRow = 1;
                    win.FreezePanes = true;
                    Marshal.ReleaseComObject(win);
                }
                catch { /* falls nicht zugänglich, Frieren überspringen */ }

                // Daten schreiben
                if (zeilenAnzahl > 0)
                {
                    object[,] data = await Task.Run(() => BaueDatenArray(daten, zeilenAnzahl, spaltenAnzahl));

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

                // Speichern (DisplayAlerts=false sorgt dafür, dass bestehende Datei
                // ohne Rückfrage überschrieben wird)
                try
                {
                    workbook.SaveAs(
                        zielPfad,
                        XlFileFormat.xlOpenXMLWorkbook,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        XlSaveAsAccessMode.xlNoChange,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                catch (COMException)
                {
                    // Race Condition: Datei wurde zwischen Lock-Check und Save geöffnet
                    // -> Fallback auf Kopie mit Zeitstempel
                    string zeitstempel = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                    zielPfad = Path.Combine(
                        ZielordnerWichtigeTabellen,
                        $"{def.DateiName}_Kopie_{zeitstempel}.xlsx");

                    workbook.SaveAs(
                        zielPfad,
                        XlFileFormat.xlOpenXMLWorkbook,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        XlSaveAsAccessMode.xlNoChange,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }

                workbook.Close(false, Type.Missing, Type.Missing);

                return zielPfad;
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                worksheet = null;
                workbook = null;
            }
        }

        /// <summary>
        /// Schreibt die Spaltenüberschriften der DataTable in Zeile 1 des Worksheets
        /// (formatiert: Fett, Weiß auf Dunkelblau, zentriert, schwarzer Unterstrich).
        /// </summary>
        private void SchreibeHeaderInWorksheet(Worksheet worksheet, System.Data.DataTable daten, int spaltenAnzahl)
        {
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

            ((Microsoft.Office.Interop.Excel.Range)worksheet.Rows[1]).RowHeight = 22;
        }

        /// <summary>
        /// Konvertiert die DataTable-Zeilen in ein 2D-object-Array, das in
        /// einem Rutsch nach Excel geschrieben werden kann (DBNull -> null).
        /// </summary>
        private object[,] BaueDatenArray(System.Data.DataTable daten, int zeilenAnzahl, int spaltenAnzahl)
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
        }

        /// <summary>
        /// Excel-Tabellenblatt-Namen dürfen max. 31 Zeichen lang sein und keine
        /// der Sonderzeichen [, ], *, ?, /, \, : enthalten.
        /// </summary>
        private string MachWorksheetNameSicher(string name)
        {
            string sicher = name
                .Replace("[", "").Replace("]", "")
                .Replace("*", "").Replace("?", "")
                .Replace("/", "_").Replace("\\", "_")
                .Replace(":", "_");
            if (sicher.Length > 31)
                sicher = sicher.Substring(0, 31);
            return sicher;
        }

        /// <summary>
        /// Prüft, ob die angegebene Datei aktuell exklusiv gesperrt ist
        /// (z. B. weil sie in Excel geöffnet ist).
        /// </summary>
        private bool IstDateiGesperrt(string pfad)
        {
            if (!File.Exists(pfad)) return false;

            try
            {
                using (FileStream fs = new FileStream(pfad, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Datei kann exklusiv geöffnet werden -> NICHT gesperrt
                }
                return false;
            }
            catch (IOException)
            {
                return true;   // Datei wird gerade von einem anderen Prozess verwendet
            }
            catch (UnauthorizedAccessException)
            {
                return false;  // Berechtigungsproblem - aber kein Lock
            }
        }
    }
}
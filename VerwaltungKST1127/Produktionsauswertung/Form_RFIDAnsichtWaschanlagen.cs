// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken,
// damit wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben können.
// ===================================================================================================
using System;                                  // Basis-Typen (string, int, DateTime, EventArgs ...)
using System.Collections.Generic;              // Sammlungen (List<T>, Dictionary<TKey, TValue>)
using System.Data;                             // DataTable, DataView, DataRow, DBNull
using System.Data.SqlClient;                   // SQL-Server-Zugriff (SqlConnection, SqlCommand, SqlDataAdapter)
using System.Globalization;                    // CultureInfo (sprachneutrale Formatierung)
using System.Windows.Forms;                    // Windows-Forms (Form, DataGridView, MessageBox ...)

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127.Produktionsauswertung
{
    // public partial class ... : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien verteilt sein (z. B. ...Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_RFIDAnsichtWaschanlagen : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Konstanten / Felder ("Variablen der Klasse")
        // private = nur in dieser Klasse sichtbar.
        // const   = Wert kann sich nie mehr ändern.
        // -----------------------------------------------------------------------------------------------------------------

        // Connection-String zentral definieren (vermeidet Wiederholungen).
        private const string ConnectionString =
            @"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Waschtragerl;Integrated Security=True;Encrypt=False";

        // _rfidTable = die geladenen Rohdaten aus der DB.
        // _rfidView  = "Sicht" auf die Tabelle (für schnelles Filtern/Sortieren ohne DB-Zugriff).
        private DataTable _rfidTable;
        private DataView _rfidView;

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_RFIDAnsichtWaschanlagen()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_RFIDAnsichtWaschanlagen()
        {
            InitializeComponent();              // erzeugt alle UI-Steuerelemente (Designer-Datei)

            // Uhrzeit-Timer starten und Tick-Event verbinden.
            TimerDatumUhrzeit.Start();
            TimerDatumUhrzeit.Tick += TimerDatumUhrzeit_Tick;
            UpdateZeitDatum();                  // Sofort einen Anfangswert anzeigen (nicht erst nach 1s)

            // Standard-Datum: heute minus 30 Tage als Untergrenze.
            dateTimePickerDatumAb.Value = DateTime.Today.AddDays(-30);

            // Wenn der Benutzer das Datum ändert → Daten neu aus der DB laden.
            // "+= Methode" hängt einen weiteren Event-Handler an das Ereignis.
            dateTimePickerDatumAb.ValueChanged += DateFilterChanged_ReloadFromDb;
            dateTimePickerDatumBis.ValueChanged += DateFilterChanged_ReloadFromDb;

            // Bei den Textboxen: Drücken von ENTER startet die Filterung.
            txtBoxArtikelnummer.KeyDown += TextFilter_KeyDown;
            txtBoxAuftragsnummer.KeyDown += TextFilter_KeyDown;
            txtBoxWaschprogramm.KeyDown += TextFilter_KeyDown;
            txtBoxWaschprogrammAceton.KeyDown += TextFilter_KeyDown;
            txtBoxUID.KeyDown += TextFilter_KeyDown;

            // Daten beim Öffnen des Fensters einmal laden.
            LoadRFIDData();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Uhrzeit/Datum-Handling
        // -----------------------------------------------------------------------------------------------------------------

        // Aktuelles Datum + Uhrzeit ins Label schreiben.
        private void UpdateZeitDatum()
        {
            lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Timer-Tick (z. B. jede Sekunde) → Anzeige aktualisieren.
        // "=>" ist die Kurzform "Expression-bodied" für eine Methode mit nur einer Anweisung.
        private void TimerDatumUhrzeit_Tick(object sender, EventArgs e) => UpdateZeitDatum();

        // -----------------------------------------------------------------------------------------------------------------
        // Daten aus der DB laden, aufbereiten und ans DataGridView binden.
        // -----------------------------------------------------------------------------------------------------------------
        private void LoadRFIDData()
        {
            try
            {
                // "using" sorgt dafür, dass die Verbindung am Ende sauber geschlossen wird.
                using (var sqlConnectionWaschtragerl = new SqlConnection(ConnectionString))
                {
                    sqlConnectionWaschtragerl.Open();

                    // Datumsbereich als Parameter (@DatumAb / @DatumBis) → SQL-Injection-Schutz.
                    string query = "SELECT * FROM RFID_Aufzeichnung WHERE Datum BETWEEN @DatumAb AND @DatumBis";

                    using (SqlCommand command = new SqlCommand(query, sqlConnectionWaschtragerl))
                    {
                        // .Date → nur Datumsteil; +1 Tag -1 Sekunde → Ende des Tages 23:59:59.
                        command.Parameters.AddWithValue("@DatumAb", dateTimePickerDatumAb.Value.Date);
                        command.Parameters.AddWithValue("@DatumBis", dateTimePickerDatumBis.Value.Date.AddDays(1).AddSeconds(-1));

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Ergebnis in eine DataTable füllen.
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // ----- 1) Hilfsspalten "Artikelnummer" und "Auftragsnummer" zusammenbauen -----
                            // Aus DB4+DB5 → Artikelnummer; aus DB28+DB29+DB30 → Auftragsnummer.
                            bool hasDb4 = dataTable.Columns.Contains("DB4");
                            bool hasDb5 = dataTable.Columns.Contains("DB5");
                            bool hasDb28 = dataTable.Columns.Contains("DB28");
                            bool hasDb29 = dataTable.Columns.Contains("DB29");
                            bool hasDb30 = dataTable.Columns.Contains("DB30");

                            // Position (Spaltenindex) merken, um die neuen Spalten in die richtige Stelle einzusortieren.
                            int db4Ordinal = hasDb4 ? dataTable.Columns["DB4"].Ordinal : dataTable.Columns.Count;
                            if (!dataTable.Columns.Contains("Artikelnummer"))
                                dataTable.Columns.Add("Artikelnummer", typeof(string));

                            int db28Ordinal = hasDb28 ? dataTable.Columns["DB28"].Ordinal : dataTable.Columns.Count;
                            if (!dataTable.Columns.Contains("Auftragsnummer"))
                                dataTable.Columns.Add("Auftragsnummer", typeof(string));

                            // Pro Zeile: die Werte zusammensetzen und in die neuen Spalten schreiben.
                            foreach (DataRow row in dataTable.Rows)
                            {
                                // Wenn Spalte fehlt oder NULL → leerer String. Sonst Wert als string.
                                string db4 = hasDb4 && row["DB4"] != DBNull.Value ? row["DB4"].ToString() : string.Empty;
                                string db5 = hasDb5 && row["DB5"] != DBNull.Value ? row["DB5"].ToString() : string.Empty;
                                row["Artikelnummer"] = db4 + db5;

                                string db28 = hasDb28 && row["DB28"] != DBNull.Value ? row["DB28"].ToString() : string.Empty;
                                string db29 = hasDb29 && row["DB29"] != DBNull.Value ? row["DB29"].ToString() : string.Empty;
                                string db30 = hasDb30 && row["DB30"] != DBNull.Value ? row["DB30"].ToString() : string.Empty;
                                row["Auftragsnummer"] = db28 + db29 + db30;
                            }

                            // SetOrdinal: schiebt die neuen Spalten an die gewünschte Position innerhalb der Tabelle.
                            dataTable.Columns["Artikelnummer"].SetOrdinal(Math.Min(db4Ordinal, dataTable.Columns.Count - 1));
                            dataTable.Columns["Auftragsnummer"].SetOrdinal(Math.Min(db28Ordinal, dataTable.Columns.Count - 1));

                            // ----- 2)/3) DataView erzeugen und ans Grid binden -----
                            _rfidTable = dataTable;
                            _rfidView = new DataView(_rfidTable);

                            // AutoGenerateColumns = true → DGV legt automatisch eine Spalte pro Tabellenspalte an.
                            dGvRFID.AutoGenerateColumns = true;
                            dGvRFID.DataSource = _rfidView;

                            // ----- 4) Unwichtige Spalten ausblenden -----
                            string[] toHide =
                            {
                                "ID","DB6","DB7","DB8","DB9","DB10","DB12","DB13","DB24",
                                "DB20","DB21","Gesendet","DB4","DB5","DB28","DB29","DB30"
                            };
                            foreach (var colName in toHide)
                            {
                                if (dGvRFID.Columns.Contains(colName))
                                    dGvRFID.Columns[colName].Visible = false;
                            }

                            // ----- 5) Spalten umbenennen (nur Anzeige im Grid) -----
                            // Dictionary "Original-Name -> Anzeigename".
                            var renameMap = new Dictionary<string, string>
                            {
                                // Allgemeine DB-Felder
                                { "DB22", "KST" },
                                { "DB23", "Vor.-Nr." },
                                { "DB25", "Stk." },
                                { "DB26", "Höhe Ausfahrt" },
                                { "DB27", "Waschanlage" },
                                { "DB31", "AVO" },
                                { "Artikelnummer", "Artikelnummer" },
                                { "Auftragsnummer", "Auftragsnummer" },
                                // UCM-Daten
                                { "DB11", "Ausfahrt UCM" },
                                { "DB17", "Waschpr. UCM" },
                                { "DB18", "Einfahrt UCM" },
                                { "DB19", "Start UCM" },
                                // Aceton-Daten
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
                                    // Name auch ändern → spätere Zugriffe werden lesbarer (col["Datum"] statt col["DB1"]).
                                    col.Name = kvp.Value;
                                }
                            }

                            // ----- 6) Spaltenbreiten automatisch an Inhalt anpassen -----
                            dGvRFID.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                            dGvRFID.AutoResizeColumns();

                            // ----- 7) Reihenfolge der sichtbaren Spalten festlegen -----
                            var order = new[]
                            {
                                "Datum","Artikelnummer","Auftragsnummer","AVO","Waschanlage",
                                "Waschpr. UCM","Einfahrt UCM","Start UCM","Ausfahrt UCM",
                                "Waschpr. Aceton","Einfahrt Aceton","Ausfahrt Aceton"
                            };
                            int idx = 0;
                            foreach (var n in order)
                                if (dGvRFID.Columns.Contains(n))
                                    dGvRFID.Columns[n].DisplayIndex = idx++;

                            // ----- 8) Sortierung: Datum absteigend (neueste zuerst) -----
                            if (dGvRFID.Columns.Contains("Datum"))
                                dGvRFID.Sort(dGvRFID.Columns["Datum"], System.ComponentModel.ListSortDirection.Descending);

                            // Bereits eingegebene Filter (Textboxen / Checkboxen) erneut anwenden.
                            ApplyCombinedFilter();

                            // ----- 9) Anzahl-Label aktualisieren -----
                            lblEingeleseneWaschkoerbe.Text = $"Eingelesene Waschträger: {_rfidView.Count}";

                            // ----- 10) Spezifische KST-Aufschlüsselung in den Labels aktualisieren -----
                            UpdateLabels();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der RFID-Daten: " + ex.Message);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Filter-Logik (mehrere Filter werden mit AND verknüpft)
        // -----------------------------------------------------------------------------------------------------------------

        // ENTER in einer Textbox → Filter neu anwenden.
        private void TextFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // verhindert den Windows-Beep beim Drücken
                ApplyCombinedFilter();
            }
        }

        // Datums-Änderung → Daten komplett neu aus der DB holen (anderer Zeitraum).
        private void DateFilterChanged_ReloadFromDb(object sender, EventArgs e) => LoadRFIDData();

        // -----------------------------------------------------------------------------------------------------------------
        // Baut den RowFilter-Ausdruck für das DataView aus allen aktiven Filtern zusammen.
        // RowFilter-Syntax ist eine vereinfachte SQL-WHERE-Syntax, mit '#...#' für DateTime-Werte.
        // -----------------------------------------------------------------------------------------------------------------
        private void ApplyCombinedFilter()
        {
            if (_rfidView == null) return;

            // Liste sammelt einzelne Filter-Stücke, die später per AND verbunden werden.
            var parts = new List<string>();

            // 1) Datumsbereich: in US-Format formatieren ("MM/dd/yyyy"), DataView erwartet das.
            //    CultureInfo.InvariantCulture → ignoriert die Sprache des Systems.
            string dateFrom = dateTimePickerDatumAb.Value.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string dateTo = dateTimePickerDatumBis.Value.Date.AddDays(1).AddSeconds(-1)
                               .ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            parts.Add($"(Datum >= #{dateFrom}# AND Datum <= #{dateTo}#)");

            // 2) Artikelnummer: LIKE '%Wert%' → Teil-Treffer (egal an welcher Position).
            if (!string.IsNullOrWhiteSpace(txtBoxArtikelnummer.Text))
            {
                string v = EscapeForRowFilter(txtBoxArtikelnummer.Text.Trim());
                parts.Add($"(Artikelnummer LIKE '%{v}%')");
            }

            // 3) Auftragsnummer
            if (!string.IsNullOrWhiteSpace(txtBoxAuftragsnummer.Text))
            {
                string v = EscapeForRowFilter(txtBoxAuftragsnummer.Text.Trim());
                parts.Add($"(Auftragsnummer LIKE '%{v}%')");
            }

            // 4) Waschprogramm UCM: DB17 könnte numerisch sein → vorher in Text wandeln (CONVERT(...,'System.String')).
            if (!string.IsNullOrWhiteSpace(txtBoxWaschprogramm.Text))
            {
                string v = EscapeForRowFilter(txtBoxWaschprogramm.Text.Trim());
                parts.Add($"(CONVERT(DB17, 'System.String') LIKE '%{v}%')");
            }

            // 5) Waschprogramm Aceton (DB14, analog).
            if (!string.IsNullOrWhiteSpace(txtBoxWaschprogrammAceton.Text))
            {
                string v = EscapeForRowFilter(txtBoxWaschprogrammAceton.Text.Trim());
                parts.Add($"(CONVERT(DB14, 'System.String') LIKE '%{v}%')");
            }

            // 6) UID-Filter (nur wenn Spalte existiert).
            if (!string.IsNullOrWhiteSpace(txtBoxUID.Text) && _rfidTable.Columns.Contains("UID"))
            {
                string v = EscapeForRowFilter(txtBoxUID.Text.Trim());
                parts.Add($"(CONVERT(UID, 'System.String') LIKE '%{v}%')");
            }

            // 7) Waschanlagen-Auswahl aus CheckedListBox in DB27-Filter umwandeln.
            //    UI zeigt freundliche Namen ("UCM497"/"Aceton"), DB enthält Codes ("FCD1"/"ACET").
            if (_rfidTable.Columns.Contains("DB27") && cListBoxWaschanlage.CheckedItems.Count > 0)
            {
                var waschanlagen = new List<string>();

                foreach (var item in cListBoxWaschanlage.CheckedItems)
                {
                    switch (item.ToString())
                    {
                        case "UCM497":
                            waschanlagen.Add("'FCD1'");
                            break;
                        case "Aceton":
                            waschanlagen.Add("'ACET'");
                            break;
                    }
                }

                if (waschanlagen.Count > 0)
                {
                    // string.Join: "'FCD1','ACET'" → kann direkt nach "IN (...)".
                    parts.Add($"(DB27 IN ({string.Join(",", waschanlagen)}))");
                }
            }

            // Wenn es Filterteile gibt → mit AND verbinden, sonst leerer Filter (alles anzeigen).
            _rfidView.RowFilter = parts.Count > 0 ? string.Join(" AND ", parts) : string.Empty;

            // Anzeige + Detail-Labels aktualisieren.
            lblEingeleseneWaschkoerbe.Text = $"Eingelesene Waschträger: {_rfidView.Count}";
            UpdateLabels();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Maskiert Sonderzeichen in Filter-Eingaben, damit sie nicht den RowFilter-Syntax stören.
        // - '  → '' (zwei Hochkommas in einem Feld)
        // - [  → [[]
        // - ]  → ]]
        // -----------------------------------------------------------------------------------------------------------------
        private static string EscapeForRowFilter(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("]", "]]");
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Klick auf "Filter löschen": setzt UI zurück und lädt erneut.
        // -----------------------------------------------------------------------------------------------------------------
        private void btnLoescheFilter_Click(object sender, EventArgs e)
        {
            // Alle Eingabefelder leeren.
            txtBoxArtikelnummer.Clear();
            txtBoxAuftragsnummer.Clear();
            txtBoxWaschprogramm.Clear();
            txtBoxWaschprogrammAceton.Clear();
            txtBoxUID.Clear();

            // Alle Häkchen in der CheckedListBox entfernen.
            for (int i = 0; i < cListBoxWaschanlage.Items.Count; i++)
            {
                cListBoxWaschanlage.SetItemChecked(i, false);
            }

            // Standard-Datumsbereich wiederherstellen.
            dateTimePickerDatumAb.Value = DateTime.Today.AddDays(-30);
            dateTimePickerDatumBis.Value = DateTime.Now;

            // Daten neu aus DB laden.
            LoadRFIDData();
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Wenn ein Häkchen in der Waschanlagen-Liste sich ändert:
        // BeginInvoke verzögert den Aufruf, damit der neue Häkchen-Zustand bereits gesetzt ist,
        // wenn ApplyCombinedFilter() ihn ausliest.
        // -----------------------------------------------------------------------------------------------------------------
        private void cListBoxWaschanlage_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke(new Action(ApplyCombinedFilter));
        }

        // -----------------------------------------------------------------------------------------------------------------
        // Aktualisiert die Detail-Labels: Anzahl Waschträger pro Kostenstelle (KST = Spalte DB22).
        // -----------------------------------------------------------------------------------------------------------------
        private void UpdateLabels()
        {
            // Zähler initialisieren.
            int count127 = 0;        // Vergütung
            int count126 = 0;        // Kitterei
            int count124 = 0;        // Planoptik
            int count125 = 0;        // Rundoptik
            int countMontage = 0;    // alles andere
            int countEmptyKST = 0;   // ohne Eintrag

            // Nur durchgehen, wenn Spalte DB22 existiert (sonst gibt's keinen KST-Wert).
            if (_rfidTable.Columns.Contains("DB22"))
            {
                // Wir laufen über die GEFILTERTE Sicht (_rfidView), nicht über die ganze Tabelle.
                foreach (DataRowView rowView in _rfidView)
                {
                    if (rowView["DB22"] != DBNull.Value)
                    {
                        string kst = rowView["DB22"].ToString();

                        // Mehrere Schreibweisen pro Kostenstelle abdecken (mit/ohne führender Null/Eins).
                        if (kst == "127" || kst == "1127" || kst == "0127")
                            count127++;
                        else if (kst == "126" || kst == "1126" || kst == "0126")
                            count126++;
                        else if (kst == "124" || kst == "1124" || kst == "0124")
                            count124++;
                        else if (kst == "125" || kst == "1125" || kst == "0125")
                            count125++;
                        else if (string.IsNullOrWhiteSpace(kst))
                            countEmptyKST++;
                        else
                            countMontage++; // alle nicht zugeordneten Werte landen hier
                    }
                }
            }

            // Ergebnisse in die Labels schreiben.
            lblEingeleseneWaschkoerbeVerguetung.Text = $"Waschträger Vergütung: {count127}";
            lblEingeleseneWaschkoerbeKitterei.Text = $"Waschträger Kitterei: {count126}";
            lblEingeleseneWaschkoerbePlanoptik.Text = $"Waschträger Planoptik: {count124}";
            lblEingeleseneWaschkoerbeRundoptik.Text = $"Waschträger Rundoptik: {count125}";
            lblEingeleseneWaschkoerbeMontagen.Text = $"Waschträger Montagen: {countMontage}";
            lblEingelesenOhneZuordnung.Text = $"Ohne KST: {countEmptyKST}";
        }
    }
}
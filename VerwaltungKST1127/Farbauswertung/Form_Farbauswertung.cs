using Microsoft.Office.Interop.Excel; // Importieren der Excel-Interop-Bibliothek für die Automatisierung und Interaktion mit Microsoft Excel (z.B. zum Erstellen, Lesen und Bearbeiten von Excel-Dateien)
using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Collections.Generic; // Importieren des System.Collections.Generic-Namespace für generische Sammlungen (z.B. List<T>, Dictionary<TKey, TValue>)
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für Komponentenmodelle und Datenbindung (z.B. zum Arbeiten mit Events und benachrichtigungsfähigen Eigenschaften)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Diagnostics; // Importieren des System.Diagnostics-Namespace für die Diagnose und Protokollierung (z.B. zum Starten von Prozessen, Debuggen und Ereignisprotokollierung)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Runtime.InteropServices; // Importieren des System.Runtime.InteropServices-Namespace für den Zugriff auf nicht verwalteten Code und Interoperabilität mit COM-Objekten (z.B. für die Interaktion mit Windows-APIs oder älteren Systemen)
using System.Threading.Tasks; // Importieren des System.Threading.Tasks-Namespace für die Arbeit mit asynchronen Tasks und paralleler Programmierung (z.B. zur Ausführung von Aufgaben im Hintergrund)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.Farbauswertung
{
    public partial class Form_Farbauswertung : Form
    {
        // Variable für die aktuelle ID im DataGridView
        int currentId;
        // Verbindungszeichenfolge für die SQL Server-Datenbank
        private readonly SqlConnection sqlConnection = new SqlConnection(@"Data Source=sqlvgt.swarovskioptik.at;Initial Catalog=SOA127_Auswertung;Integrated Security=True;Encrypt=False");
        // Feld für den PerformanceCounter
        private PerformanceCounter cpuCounter;
        // Feld für den PerformanceCounter
        private PerformanceCounter memoryCounter;
        // Deklaration der Instanzvariablen originalDataTable für die ursprünglichen Daten
        private System.Data.DataTable originalDataTable = new System.Data.DataTable();

        public Form_Farbauswertung()
        {
            InitializeComponent(); // Initialisierung der Komponenten des Formulars
            Timer1.Start(); // Starten des Timers für die Uhranzeige
            UpdateDateTime(); // Aktualisieren des Datums und der Uhrzeit
            UpdateDgvFarbauswertung(); // Aktualisieren des DataGridView für die Farbauswertung
            FillComboBoxBelag(); // Befüllen der ComboBoxBelag mit den Werten für die Filterung
            FillComboBoxProzess(); // Befüllen der ComboBoxProzess mit den Werten für die Filterung
            DateTimePickerDgv.Value = new DateTime(2021, 1, 4); // Setzen des Datums für DateTimePickerDgv auf den 04.01.2021
            CountChargen(); // Chargen von Heute und von Gestern zählen

            // Initialisieren der Leistungsindikatoren - CPU
            InitializePerformanceCounters();
            // Initialisieren der Leistungsindikatoren - RAM
            InitializeMemoryCounter();
            // Starten des Timers für die CPU-Auslastung
            TimerCpu.Interval = 1000; // Aktualisierung alle 1 Sekunde
            TimerCpu.Start();
            // Starten des Timers für die RAM-Auslastung
            TimerMemory.Interval = 1000; // Aktualisierung alle 1 Sekunde
            TimerMemory.Start();
        }

        // Funktion für die instanziierung des PerformanceCounters
        private void InitializePerformanceCounters()
        {
            // Instanziieren des PerformanceCounter-Objekts für die CPU-Auslastung
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        // Initialisierung des PerformanceCounters für den Arbeitsspeicher
        private void InitializeMemoryCounter()
        {
            // Instanziieren des PerformanceCounter-Objekts für die Arbeitsspeicher-Auslastung
            memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }

        // Tick-Event des Timers für die Uhranzeige
        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateDateTime(); // Aktualisieren der Uhrzeit
        }

        // Tick-Event für die sekündliche Aktualisierung der CPU Auslasung
        private void TimerCpu_Tick(object sender, EventArgs e)
        {
            // Überprüfen, ob cpuCounter initialisiert wurde
            if (cpuCounter != null)
            {
                // Abrufen des aktuellen CPU-Auslastungswerts
                float cpuUsage = cpuCounter.NextValue();

                // Anzeigen der CPU-Auslastung auf einem Label
                LblCpuUsage.Text = string.Format("CPU-Auslastung:   {0}%", cpuUsage);
            }
            else
            {
                // Wenn cpuCounter nicht initialisiert wurde, eine Fehlermeldung anzeigen
                MessageBox.Show("cpuCounter wurde nicht initialisiert.");
            }
        }

        // Tick-Event für die sekündliche Aktualisierung der RAM Auslastung
        private void TimerMemory_Tick(object sender, EventArgs e)
        {
            // Überprüfen, ob memoryCounter initialisiert wurde
            if (memoryCounter != null)
            {
                // Abrufen des aktuellen RAM-Auslastungswerts
                float memoryUsage = memoryCounter.NextValue();

                // Anzeigen der RAM-Auslastung auf einem Label
                LblMemoryUsage.Text = string.Format("RAM-Auslastung:  {0}%", memoryUsage);
            }
            else
            {
                // Wenn memoryCounter nicht initialisiert wurde, eine Fehlermeldung anzeigen
                MessageBox.Show("memoryCounter wurde nicht initialisiert.");
            }
        }

        // Diese Methode ruft die Daten aus dem DataGridView ab und gibt sie zurück
        private System.Data.DataTable GetDataGridViewData()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            // Hier gehen wir davon aus, dass Ihr DataGridView "DgvFarbauswertung" heißt
            if (DgvFarbauswertung.DataSource != null && DgvFarbauswertung.Rows.Count > 0)
            {
                dt = (System.Data.DataTable)DgvFarbauswertung.DataSource;
            }
            return dt;
        }

        // Funktion um die Chargen von Heute und vom Vortag zu zählen
        public void CountChargen()
        {
            // Zähler für die Anzahl der Chargen von heute und gestern
            int countToday = 0;
            int countYesterday = 0;

            // Dictionarys zur Speicherung der Zählung der Präfixe für heute und gestern
            Dictionary<string, int> todayPrefixCounts = new Dictionary<string, int>();
            Dictionary<string, int> yesterdayPrefixCounts = new Dictionary<string, int>();

            // Liste der Präfixe, die gezählt werden sollen
            List<string> prefixes = new List<string> { "20", "30", "35", "40", "45", "50", "60", "65" };

            // Initialisiere Dictionarys mit Präfixen und setze die Zähler auf 0
            foreach (var prefix in prefixes)
            {
                todayPrefixCounts[prefix] = 0;
                yesterdayPrefixCounts[prefix] = 0;
            }

            // Holen des aktuellen Datums und des Vortages
            DateTime currentDate = DateTime.Now.Date;
            DateTime previousDate = currentDate.AddDays(-1);

            // Schleife durch alle Zeilen des DataGridView (DgvFarbauswertung)
            foreach (DataGridViewRow row in DgvFarbauswertung.Rows)
            {
                // Überprüfen, ob das Datum in der Zeile gültig ist
                if (row.Cells["Datum"].Value != null && DateTime.TryParse(row.Cells["Datum"].Value.ToString(), out DateTime cellDate))
                {
                    // Hole den Wert der Charge-Spalte, falls vorhanden
                    string chargeValue = row.Cells["Charge"].Value?.ToString();
                    if (chargeValue == null)
                        continue; // Überspringe die Zeile, falls die Charge leer ist

                    // Überprüfen, ob das Datum dem heutigen Datum entspricht
                    if (cellDate.Date == currentDate)
                    {
                        countToday++; // Inkrementiere den Zähler für heute
                        CountPrefixes(chargeValue, todayPrefixCounts); // Zähle Präfixe für heute
                    }
                    // Überprüfen, ob das Datum dem Vortag entspricht
                    else if (cellDate.Date == previousDate)
                    {
                        countYesterday++; // Inkrementiere den Zähler für den Vortag
                        CountPrefixes(chargeValue, yesterdayPrefixCounts); // Zähle Präfixe für den Vortag
                    }
                }
            }

            // Aktualisiere die Labels für die Gesamtanzahl der Chargen heute
            LblHeute.Text = "Chargen Heute: " + countToday.ToString();
            LblHeute.ForeColor = Color.Orange;

            // Aktualisiere die Labels für die Präfix-Zählungen heute
            UpdatePrefixLabels(todayPrefixCounts, "Heute");

            // Aktualisiere die Labels für die Gesamtanzahl der Chargen gestern
            LblVortag.Text = "Chargen gestern: " + countYesterday.ToString();
            LblVortag.ForeColor = Color.Yellow;

            // Aktualisiere die Labels für die Präfix-Zählungen gestern
            UpdatePrefixLabels(yesterdayPrefixCounts, "Vortag");
        }

        // Methode zur Zählung der Präfixe in der Chargennummer
        private void CountPrefixes(string chargeValue, Dictionary<string, int> prefixCounts)
        {
            // Schleife durch alle definierten Präfixe
            foreach (var prefix in prefixCounts.Keys.ToList())
            {
                // Wenn die Chargennummer mit dem Präfix beginnt, Zähler erhöhen
                if (chargeValue.StartsWith(prefix))
                {
                    prefixCounts[prefix]++;
                }
            }
        }

        // Methode zur Aktualisierung der Labels basierend auf den Präfix-Zählungen
        private void UpdatePrefixLabels(Dictionary<string, int> prefixCounts, string dayLabel)
        {
            // Schleife durch jedes Präfix und aktualisiere das entsprechende Label
            foreach (var prefix in prefixCounts)
            {
                // Je nach Tag (Heute oder Vortag) das passende Label aktualisieren
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
        private void Form_Farbauswertung_Load(object sender, EventArgs e)
        {
            LblChargenAb.Text = "Eingelesen ab: 04.01.2021";
        }

        // Event-Handler für den Button BtnHeute
        private void BtnHeute_Click(object sender, EventArgs e)
        {
            DateTimePickerDgv.Value = DateTime.Today;
            DateTimePickerDgvBis.Value = DateTime.Today;
            // Entferne das Event-Handling für das CellFormatting-Ereignis
            DgvFarbauswertung.CellFormatting -= DgvFarbauswertung_CellFormatting;
        }

        // Event-Handler für den Button BtnGestern
        private void BtnGestern_Click(object sender, EventArgs e)
        {
            // Heutiges Datum
            DateTime today = DateTime.Now;

            // Datum von gestern
            DateTime yesterday = today.AddDays(-1);
            DateTimePickerDgv.Value = yesterday;
            DateTimePickerDgvBis.Value = yesterday;
        }

        // Event-Handler damit die Monatsauswertung aufgeht
        private void BtnMonatsAuswertung_Click(object sender, EventArgs e)
        {
            Form_Monatsuebersicht form_Monatsuebersicht = new Form_Monatsuebersicht();
            form_Monatsuebersicht.SetDataFromInputForm(GetDataGridViewData());
            form_Monatsuebersicht.Show();
            // DataGridView nach der ID absteigend sortieren
            DgvFarbauswertung.Sort(DgvFarbauswertung.Columns[0], ListSortDirection.Descending);
        }

        // Event-Handler wenn im DGV doppelt geklickt wird
        private void DgvFarbauswertung_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SaveDataForMessurement();
        }

        // Funktion die die Werte für das Messbild abspeichert
        private void SaveDataForMessurement()
        {
            // Hole die Werte aus den Textboxen
            string dateValue = TextBoxDatum.Text;
            string chargValue = TextBoxCharge.Text;
            string XiValue = TextBoxXi.Text;
            string XaValue = TextBoxXa.Text;
            string YiValue = TextBoxYi.Text;
            string YaValue = TextBoxYa.Text;
            string ZiValue = TextBoxZi.Text;
            string ZaValue = TextBoxZa.Text;
            string belag = TextBoxBelag.Text;
            string process = TextBoxProzess.Text;
            string anlage = TextBoxAnlage.Text;
            // Neue Instanz zu Form_Messbild - Values übergeben
            Form_Messbild form_Messbild = new Form_Messbild(dateValue, chargValue, XiValue, XaValue, YiValue,
                YaValue, ZiValue, ZaValue, belag, process, anlage);
            form_Messbild.Show(); // Formular anzeigen
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn der Button zum Löschen aller Filter geklickt wird
        private void BtnClearAllFilters_Click(object sender, EventArgs e)
        {
            ComboBoxAnlage.SelectedIndex = -1; // Auswahl in ComboBox "Anlage" löschen und Index zurücksetzen
            ComboBoxBelag.SelectedIndex = -1; // Auswahl in ComboBox "Belag" löschen und Index zurücksetzen
            ComboBoxProzess.SelectedIndex = -1; // Auswahl in ComboBox "Prozess" löschen und Index zurücksetzen
            ComboBoxAnlage.Text = string.Empty; // Auswahl in ComboBox "Anlage" löschen
            ComboBoxBelag.Text = string.Empty; // Auswahl in ComboBox "Belag" löschen
            ComboBoxProzess.Text = string.Empty; // Auswahl in ComboBox "Prozess" löschen
            DateTimePickerDgv.Value = DateTime.Now; // Datum im DateTimePicker auf das aktuelle Datum setzen
            DateTimePickerDgvBis.Value = DateTime.Now; // Datum im DateTimePickerBis auf das aktuelle Datum setzen
            UpdateDgvFarbauswertung(); // DataGridView aktualisieren und alle Filter löschen
            UpdateTotalRowCount();
            // Setzen des Datums für DateTimePickerDgv auf den 04.01.2021
            DateTimePickerDgv.Value = new DateTime(2021, 1, 4);
            LblChargenAb.Text = "Chargen ab 04.01.2021: ";

            // Daten aus den Textboxen löschen
            TextBoxId.Text = string.Empty;
            TextBoxAnlage.Text = string.Empty;
            TextBoxDatum.Text = string.Empty; ;
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
            // Hinzufügen des Event-Handling für das CellFormatting-Ereignis
            DgvFarbauswertung.CellFormatting += DgvFarbauswertung_CellFormatting;
            // ListBoxXYZ löschen
            for (int i = 0; i < ListBoxXYZ.Items.Count; i++)
            {
                ListBoxXYZ.SetItemChecked(i, false);
            }
            // Chargenanzahl nochmals aktualisieren
            CountChargen();
        }

        // Methode zur Aktualisierung der Uhrzeit
        private void UpdateDateTime()
        {
            DateTime current = DateTime.Now; // Aktuelle Zeit abrufen
            LblTimeDate.Text = current.ToString("dd.MM.yyyy HH:mm:ss"); // Anzeige der Zeit im entsprechenden Label
        }

        // Hier werden die Funktionen, die für die Aktualisierung,
        // bzw. für das Filtern des DataGridVies zuständig sind
        // aufgelistet. 

        // Ruft immer die aktuellen Daten aus der Tabelle Farbauswertung ab
        // Formatiert den Header
        // Methode zur Aktualisierung und Formatierung des DataGridView für die Farbauswertung
        public void UpdateDgvFarbauswertung()
        {
            try
            {
                sqlConnection.Open(); // Verbindung zur Datenbank öffnen
                string query = "SELECT * FROM Geamtauswertungstabelle"; // SQL-Abfrage für die Daten
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection); // SQLDataAdapter für die Datenbankabfrage
                originalDataTable.Clear(); // Ursprüngliche Daten löschen
                sqlDataAdapter.Fill(originalDataTable); // Daten aus der Datenbank abrufen und in die DataTable einfügen
                DataSet dataSet = new DataSet(); // Neues DataSet erstellen
                sqlDataAdapter.Fill(dataSet); // Daten in das DataSet einfügen
                DgvFarbauswertung.DataSource = dataSet.Tables[0]; // Datenquelle des DataGridViews festlegen
                DgvFarbauswertung.Sort(DgvFarbauswertung.Columns[0], ListSortDirection.Descending); // DataGridView nach der ID absteigend sortieren
                // Spalten des DataGridViews festlegen (Sichtbarkeit und Breite)
                DgvFarbauswertung.Columns[0].Visible = false; DgvFarbauswertung.Columns[0].Width = 60;
                DgvFarbauswertung.Columns[1].Visible = false;
                DgvFarbauswertung.Columns[2].Visible = false;
                DgvFarbauswertung.Columns[3].Visible = true; DgvFarbauswertung.Columns[3].Width = 60;
                DgvFarbauswertung.Columns[4].Visible = true; DgvFarbauswertung.Columns[4].Width = 140;
                DgvFarbauswertung.Columns[5].Visible = true; DgvFarbauswertung.Columns[5].Width = 65;
                DgvFarbauswertung.Columns[6].Visible = false;
                DgvFarbauswertung.Columns[7].Visible = true; DgvFarbauswertung.Columns[7].Width = 65;
                DgvFarbauswertung.Columns[8].Visible = true; DgvFarbauswertung.Columns[8].Width = 65;
                DgvFarbauswertung.Columns[9].Visible = true; DgvFarbauswertung.Columns[9].Width = 65;
                DgvFarbauswertung.Columns[10].Visible = true; DgvFarbauswertung.Columns[10].Width = 97;
                DgvFarbauswertung.Columns[11].Visible = true; DgvFarbauswertung.Columns[11].Width = 60;
                DgvFarbauswertung.Columns[12].Visible = true; DgvFarbauswertung.Columns[12].Width = 60;
                DgvFarbauswertung.Columns[13].Visible = true; DgvFarbauswertung.Columns[13].Width = 60;
                DgvFarbauswertung.Columns[14].Visible = true; DgvFarbauswertung.Columns[14].Width = 60;
                DgvFarbauswertung.Columns[15].Visible = true; DgvFarbauswertung.Columns[15].Width = 60;
                DgvFarbauswertung.Columns[16].Visible = true; DgvFarbauswertung.Columns[16].Width = 70;
                DgvFarbauswertung.Columns[17].Visible = false;
                DgvFarbauswertung.Columns[18].Visible = false;
                DgvFarbauswertung.Columns[19].Visible = false;
                DgvFarbauswertung.Columns[20].Visible = false;
                DgvFarbauswertung.Columns[21].Visible = true; DgvFarbauswertung.Columns[21].Width = 60;
                DgvFarbauswertung.Columns[22].Visible = true; DgvFarbauswertung.Columns[22].Width = 80;
                DgvFarbauswertung.Columns[23].Visible = true; DgvFarbauswertung.Columns[23].Width = 60;
                DgvFarbauswertung.Columns[24].Visible = true; DgvFarbauswertung.Columns[24].Width = 60;
                DgvFarbauswertung.Columns[25].Visible = true; DgvFarbauswertung.Columns[25].Width = 60;
                DgvFarbauswertung.Columns[26].Visible = true; DgvFarbauswertung.Columns[26].Width = 75;
                DgvFarbauswertung.Columns[27].Visible = false;
                DgvFarbauswertung.Columns[28].Visible = false;
                DgvFarbauswertung.Columns[29].Visible = false;
                DgvFarbauswertung.Columns[30].Visible = false;
                DgvFarbauswertung.Columns[31].Visible = true; DgvFarbauswertung.Columns[31].Width = 65;
                DgvFarbauswertung.Columns[32].Visible = true; DgvFarbauswertung.Columns[32].Width = 80;
                DgvFarbauswertung.Columns[33].Visible = false;
                DgvFarbauswertung.Columns[34].Visible = false;
                DgvFarbauswertung.Columns[35].Visible = true; DgvFarbauswertung.Columns[35].Width = 110;
                DgvFarbauswertung.Columns[36].Visible = false;
                // DataGridView-Formatierung: Header-Schriftart und -Ausrichtung
                DgvFarbauswertung.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(DataGridView.DefaultFont, FontStyle.Bold);
                DgvFarbauswertung.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                UpdateTotalRowCount();
                sqlConnection.Close(); // Datenbankverbindung schließen
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Methode zur Ausführung einer SQL-Abfrage
        public void ExecuteQuery(string query)
        {
            try
            {
                sqlConnection.Open(); // Verbindung zur Datenbank öffnen
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // SqlCommand-Objekt für die Abfrage erstellen
                sqlCommand.ExecuteNonQuery(); // Ausführen der SQL-Abfrage ohne Rückgabewert
                sqlConnection.Close(); // Datenbankverbindung schließen
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn auf den Inhalt einer Zelle im DataGridView geklickt wird
        private void DgvFarbauswertung_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Überprüfen, ob eine Zeile ausgewählt wurde und ob der Index der Zeile gültig ist
                if (DgvFarbauswertung.SelectedRows.Count > 0 && e.RowIndex >= 0)
                {
                    // ID der ausgewählten Zeile im DataGridView speichern
                    currentId = (int)DgvFarbauswertung.SelectedRows[0].Cells[0].Value;
                    // Daten aus der ausgewählten Zeile in die entsprechenden Textboxen laden
                    TextBoxId.Text = currentId.ToString();
                    TextBoxAnlage.Text = DgvFarbauswertung.SelectedRows[0].Cells[3].Value.ToString();
                    // Datum formatieren, bevor es in die Textbox geladen wird
                    string dateValue = DgvFarbauswertung.SelectedRows[0].Cells[4].Value.ToString();
                    if (DateTime.TryParse(dateValue, out DateTime parsedDate))
                    {
                        TextBoxDatum.Text = parsedDate.ToString("dd.MM.yyyy"); // Nur Datum anzeigen
                    }
                    else
                    {
                        TextBoxDatum.Text = dateValue; // Falls nicht geparst, den Originalwert anzeigen
                    }
                    TextBoxCharge.Text = DgvFarbauswertung.SelectedRows[0].Cells[5].Value.ToString();
                    TextBoxBelag.Text = DgvFarbauswertung.SelectedRows[0].Cells[7].Value.ToString();
                    TextBoxProzess.Text = DgvFarbauswertung.SelectedRows[0].Cells[8].Value.ToString();
                    TextBoxTestlinse.Text = DgvFarbauswertung.SelectedRows[0].Cells[9].Value.ToString();
                    TextBoxMessgeraet.Text = DgvFarbauswertung.SelectedRows[0].Cells[35].Value.ToString();
                    TextBoxXi.Text = DgvFarbauswertung.SelectedRows[0].Cells[13].Value.ToString();
                    TextBoxYi.Text = DgvFarbauswertung.SelectedRows[0].Cells[14].Value.ToString();
                    TextBoxZi.Text = DgvFarbauswertung.SelectedRows[0].Cells[15].Value.ToString();
                    TextBoxCabI.Text = DgvFarbauswertung.SelectedRows[0].Cells[21].Value.ToString();
                    TextBoxWinkelI.Text = DgvFarbauswertung.SelectedRows[0].Cells[22].Value.ToString();
                    TextBoxMessI.Text = DgvFarbauswertung.SelectedRows[0].Cells[16].Value.ToString();
                    TextBoxXa.Text = DgvFarbauswertung.SelectedRows[0].Cells[23].Value.ToString();
                    TextBoxYa.Text = DgvFarbauswertung.SelectedRows[0].Cells[24].Value.ToString();
                    TextBoxZa.Text = DgvFarbauswertung.SelectedRows[0].Cells[25].Value.ToString();
                    TextBoxCabA.Text = DgvFarbauswertung.SelectedRows[0].Cells[31].Value.ToString();
                    TextBoxWinkelA.Text = DgvFarbauswertung.SelectedRows[0].Cells[32].Value.ToString();
                    TextBoxMessA.Text = DgvFarbauswertung.SelectedRows[0].Cells[26].Value.ToString();
                    TextBoxEgun1.Text = DgvFarbauswertung.SelectedRows[0].Cells[11].Value.ToString();
                    TextBoxEgun2.Text = DgvFarbauswertung.SelectedRows[0].Cells[12].Value.ToString();
                    TextBoxReinigung.Text = DgvFarbauswertung.SelectedRows[0].Cells[10].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn auf eine Zelle im DataGridView geklickt wird
        private void DgvFarbauswertung_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Überprüfen, ob der Index der Zeile gültig ist
                if (e.RowIndex >= 0)
                {
                    // Ausgewählte Zeile im DataGridView abrufen
                    DataGridViewRow selectedRow = DgvFarbauswertung.Rows[e.RowIndex];
                    // ID der ausgewählten Zeile speichern
                    currentId = (int)selectedRow.Cells[0].Value;
                    // Daten aus der ausgewählten Zeile in die entsprechenden Textboxen laden
                    TextBoxId.Text = currentId.ToString();
                    TextBoxAnlage.Text = selectedRow.Cells[3].Value.ToString();
                    // Datum formatieren, bevor es in die Textbox geladen wird
                    if (DateTime.TryParse(selectedRow.Cells[4].Value.ToString(), out DateTime parsedDate))
                    {
                        TextBoxDatum.Text = parsedDate.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        TextBoxDatum.Text = selectedRow.Cells[4].Value.ToString(); // Falls das Datum nicht geparst werden kann, originalen Wert setzen
                    }
                    TextBoxCharge.Text = selectedRow.Cells[5].Value.ToString();
                    TextBoxBelag.Text = selectedRow.Cells[7].Value.ToString();
                    TextBoxProzess.Text = selectedRow.Cells[8].Value.ToString();
                    TextBoxTestlinse.Text = selectedRow.Cells[9].Value.ToString();
                    TextBoxMessgeraet.Text = selectedRow.Cells[35].Value.ToString();
                    TextBoxXi.Text = selectedRow.Cells[13].Value.ToString();
                    TextBoxYi.Text = selectedRow.Cells[14].Value.ToString();
                    TextBoxZi.Text = selectedRow.Cells[15].Value.ToString();
                    TextBoxCabI.Text = selectedRow.Cells[21].Value.ToString();
                    TextBoxWinkelI.Text = selectedRow.Cells[22].Value.ToString();
                    TextBoxMessI.Text = selectedRow.Cells[16].Value.ToString();
                    TextBoxXa.Text = selectedRow.Cells[23].Value.ToString();
                    TextBoxYa.Text = selectedRow.Cells[24].Value.ToString();
                    TextBoxZa.Text = selectedRow.Cells[25].Value.ToString();
                    TextBoxCabA.Text = selectedRow.Cells[31].Value.ToString();
                    TextBoxWinkelA.Text = selectedRow.Cells[32].Value.ToString();
                    TextBoxMessA.Text = selectedRow.Cells[26].Value.ToString();
                    TextBoxEgun1.Text = selectedRow.Cells[11].Value.ToString();
                    TextBoxEgun2.Text = selectedRow.Cells[12].Value.ToString();
                    TextBoxReinigung.Text = selectedRow.Cells[10].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Methode zur grafischen Gestaltung des DGV
        private void DgvFarbauswertung_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Überprüfen, ob es sich um eine Zeile handelt und ob die Zelle das Datum enthält
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string dateColumnName = "Datum"; // Name der Datumsspalte

                // Überprüfen, ob die aktuelle Zelle sich in der Datumsspalte befindet
                if (DgvFarbauswertung.Columns[e.ColumnIndex].Name == dateColumnName)
                {
                    DateTime currentDate = DateTime.Now.Date; // Aktuelles Datum ohne Zeitkomponente
                    DateTime previousDate = currentDate.AddDays(-1); // Datum des Vortages

                    // Wert der Zelle als Datum interpretieren
                    if (DateTime.TryParse(e.Value.ToString(), out DateTime cellDate))
                    {
                        // Wenn das Datum in der Zelle dem aktuellen Datum entspricht, die gesamte Zeile einfärben
                        if (cellDate.Date == currentDate)
                        {
                            DgvFarbauswertung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                        }
                        // Wenn das Datum in der Zelle dem Vortag entspricht, die gesamte Zeile in Gelb einfärben
                        else if (cellDate.Date == previousDate)
                        {
                            DgvFarbauswertung.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                        }
                    }
                }
            }
        }

        // Methode zum Befüllen der ComboBox mit eindeutigen Werten aus der Spalte "Belag" des DataGridViews
        private void FillComboBoxBelag()
        {
            try
            {
                HashSet<string> uniqueValues = new HashSet<string>(); // HashSet zum Speichern eindeutiger Werte erstellen

                // DataGridView durchlaufen und eindeutige Werte zum HashSet hinzufügen
                foreach (DataGridViewRow row in DgvFarbauswertung.Rows)
                {
                    // Überprüfen, ob die Zelle gültig ist und nicht null ist
                    if (row.Cells[7].Value != null)
                    {
                        string value = row.Cells[7].Value.ToString(); // Wert der Zelle abrufen
                        uniqueValues.Add(value); // Eindeutigen Wert zur HashSet hinzufügen
                    }
                }

                // Fügen Sie die eindeutigen Werte aus dem HashSet zur ComboBox hinzu
                foreach (string value in uniqueValues)
                {
                    ComboBoxBelag.Items.Add(value); // Eindeutigen Wert zur ComboBox hinzufügen
                }
                ComboBoxBelag.Sorted = true; // Sortieren Sie die ComboBox-Einträge
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Methode zum Befüllen der ComboBox mit eindeutigen Werten aus der Spalte "Prozess" des DataGridViews
        private void FillComboBoxProzess()
        {
            try
            {
                HashSet<string> uniqueValues = new HashSet<string>(); // HashSet zum Speichern eindeutiger Werte erstellen

                // DataGridView durchlaufen und eindeutige Werte zum HashSet hinzufügen
                foreach (DataGridViewRow row in DgvFarbauswertung.Rows)
                {
                    // Überprüfen, ob die Zelle gültig ist und nicht null ist
                    if (row.Cells[8].Value != null)
                    {
                        string value = row.Cells[8].Value.ToString(); // Wert der Zelle abrufen
                        uniqueValues.Add(value); // Eindeutigen Wert zur HashSet hinzufügen
                    }
                }

                // Fügen Sie die eindeutigen Werte aus dem HashSet zur ComboBox hinzu
                foreach (string value in uniqueValues)
                {
                    ComboBoxProzess.Items.Add(value); // Eindeutigen Wert zur ComboBox hinzufügen
                }
                ComboBoxProzess.Sorted = true; // Sortieren Sie die ComboBox-Einträge
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Methode zum Anwenden von Filtern basierend auf Benutzereingaben für Belag, Anlage und/oder Datum
        private void ApplyFilters()
        {
            try
            {
                // Filter für Belag
                string belagFilter = ComboBoxBelag.SelectedItem != null ?
                    $"Belag = '{ComboBoxBelag.SelectedItem.ToString()}'" :
                    string.Empty;

                // Filter für Anlage
                string anlageFilter = ComboBoxAnlage.SelectedItem != null ?
                    int.TryParse(ComboBoxAnlage.SelectedItem.ToString(), out int anlage) ?
                        $"Anlage = '{anlage}'" :
                        "1 = 0" : // Füge einen Filter hinzu, der immer falsch ist, wenn die Anlage nicht gültig ist
                    string.Empty;

                // Filter für Prozess
                string prozessFilter = ComboBoxProzess.SelectedItem != null ?
                    $"Prozess = '{ComboBoxProzess.SelectedItem.ToString()}'" :
                    string.Empty;

                // Filter für Datum
                string datumFilter = DateTimePickerDgv.Checked && DateTimePickerDgvBis.Checked ?
                    $"[Datum] >= #{DateTimePickerDgv.Value.Date:MM/dd/yyyy}# AND [Datum] <= #{DateTimePickerDgvBis.Value.Date:MM/dd/yyyy}#" :
                    string.Empty;

                // Kombinieren aller Filter
                List<string> filters = new List<string>();

                if (!string.IsNullOrEmpty(belagFilter))
                    filters.Add(belagFilter);
                if (!string.IsNullOrEmpty(anlageFilter))
                    filters.Add(anlageFilter);
                if (!string.IsNullOrEmpty(prozessFilter))
                    filters.Add(prozessFilter);
                if (!string.IsNullOrEmpty(datumFilter))
                    filters.Add(datumFilter);

                string combinedFilter = string.Join(" AND ", filters);

                // Filter auf DataView anwenden und DataSource des DataGridViews aktualisieren
                DataView dataView = new DataView(originalDataTable)
                {
                    RowFilter = combinedFilter, // Filterbedingung auf DataView anwenden                    
                    Sort = "Datum DESC, Charge DESC" // Ergebnisse nach Datum absteigend sortieren, dann nach Charge
                };

                DgvFarbauswertung.DataSource = dataView.ToTable(); // DataView als neue DataSource für das DataGridView festlegen
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Fehlermeldung anzeigen, falls ein Fehler auftritt
            }
        }

        // Methode zum Aktualisieren der Anzeige der Gesamtanzahl von Datensätzen im DataGridView
        private void UpdateTotalRowCount()
        {
            int totalRowCount = DgvFarbauswertung.Rows.Count;
            TextBoxChargenGesamtNachBelag.Text = totalRowCount.ToString();
            TextBoxChargenGesamtNachBelag.TextAlign = HorizontalAlignment.Center;
        }

        // Methode zum Anwenden von Filtern und Aktualisieren der Gesamtanzahl von Datensätzen
        private void ApplyFiltersAndUpdateTotalRowCount()
        {
            ApplyFilters(); // Filter anwenden
            UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn die Auswahl in der ComboBox "Belag" geändert wird
        private void ComboBoxBelag_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfen, ob mindestens ein DateTimePicker aktiv ist
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
            {
                ApplyFilters(); // Filter anwenden, wenn eine neue Auswahl getroffen wird
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
            else
            {
                UpdateDgvFarbauswertung(); // Aktualisieren des DataGridViews ohne Filter, wenn keine DateTimePicker aktiv sind
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn die Auswahl in der ComboBox "Anlage" geändert wird
        private void ComboBoxAnlage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfen, ob mindestens ein DateTimePicker aktiv ist
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
            {
                ApplyFilters(); // Filter anwenden, wenn eine neue Auswahl getroffen wird
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
            else
            {
                UpdateDgvFarbauswertung(); // Aktualisieren des DataGridViews ohne Filter, wenn keine DateTimePicker aktiv sind
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn die Auswahl in der ComboBox "Prozes" geändert wird
        private void ComboBoxProzess_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Überprüfen, ob mindestens ein DateTimePicker aktiv ist
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
            {
                ApplyFilters(); // Filter anwenden, wenn eine neue Auswahl getroffen wird
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
            else
            {
                UpdateDgvFarbauswertung(); // Aktualisieren des DataGridViews ohne Filter, wenn keine DateTimePicker aktiv sind
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn die Auswahl in der ComboBox "Belag" geändert wird
        private void ComboBoxBelag_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Überprüfen, ob mindestens ein DateTimePicker aktiv ist
            if (DateTimePickerDgv.Checked || DateTimePickerDgvBis.Checked)
            {
                ApplyFilters(); // Filter anwenden, wenn eine neue Auswahl getroffen wird
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
            else
            {
                UpdateDgvFarbauswertung(); // Aktualisieren des DataGridViews ohne Filter, wenn keine DateTimePicker aktiv sind
                UpdateTotalRowCount(); // Anzahl der Datensätze aktualisieren
            }
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn das Datum im DateTimePickerAb geändert wird
        private void DateTimePickerDgv_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters(); // Filter anwenden, wenn ein neues Datum ausgewählt wird
            ApplyFiltersAndUpdateTotalRowCount();
            // Aktualisieren des Labels mit dem ausgewählten Datum
            LblChargenAb.Text = $"Eingelesen ab {DateTimePickerDgv.Value.ToString("dd.MM.yyyy")}:";
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn das Datum im DateTimePickerBis geändert wird
        private void DateTimePickerDgvBis_ValueChanged(object sender, EventArgs e)
        {
            // Sicherstellen, dass das Bis Datum nicht kleinern als das Ab Datum ist
            if (DateTimePickerDgvBis.Value < DateTimePickerDgv.Value)
            {
                DateTimePickerDgvBis.Value = DateTime.Today;
                return;
            }

            ApplyFilters(); // Filter anwenden, wenn ein neues Datum ausgewählt wird
            ApplyFiltersAndUpdateTotalRowCount();
        }

        // Event-Handler-Methode, die ausgelöst wird, wenn der Button zum Exportieren der Daten in ein Excel-File geklickt wird
        private async void BtnExportExcel_Click(object sender, EventArgs e)
        {
            Workbook workbook = null;
            Worksheet worksheet = null;
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                // Anzeige einer Warnung vor dem Exportvorgang und Prüfung auf Benutzerreaktion
                DialogResult result = MessageBox.Show(
                    "Das Exportieren kann bei einer großen Datenmenge etwas länger dauern.\n\nKlicken Sie auf 'OK', um fortzufahren, oder auf 'Abbrechen', um den Vorgang abzubrechen.",
                    "Hinweis",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);

                // Überprüfen, ob der Benutzer den Export fortsetzen möchte
                if (result == DialogResult.Cancel)
                {
                    return; // Vorgang abbrechen, wenn der Benutzer auf 'Abbrechen' klickt
                }

                // Erstellung einer neuen Instanz der Excel-Anwendung
                excelApp.Visible = false; // Excel-Anwendung nicht sofort öffnen

                // Erstellung eines neuen Arbeitsblatts in Excel
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Worksheet)workbook.ActiveSheet;

                // Kopieren des Headers aus dem DataGridView in das Excel-Arbeitsblatt
                int columnCount = 1;
                foreach (DataGridViewColumn column in DgvFarbauswertung.Columns)
                {
                    if (column.Visible)
                    {
                        // Formatierung der Header-Zelle
                        var headerRange = (Range)worksheet.Cells[1, columnCount];
                        headerRange.Font.Bold = true; // Fettschrift für den Text
                        headerRange.Value = column.HeaderText; // Header-Text setzen
                        headerRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue); // Textfarbe ändern
                        headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.WhiteSmoke); // Hintergrundfarbe der Zelle ändern
                        headerRange.HorizontalAlignment = XlHAlign.xlHAlignCenter; // Text zentrieren
                        headerRange.Font.Size = 12; // Schriftgröße ändern

                        // Unterstrichene Linie unter dem Header hinzufügen
                        Borders borders = headerRange.Borders;
                        borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
                        borders[XlBordersIndex.xlEdgeBottom].Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                        borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;

                        columnCount++; // Spaltenzähler erhöhen

                        // Freigeben der verwendeten COM-Objekte
                        Marshal.ReleaseComObject(headerRange);
                        Marshal.ReleaseComObject(borders);
                    }
                }

                // Aktivieren der Filterfunktion im Header
                var headerRangeForFilter = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, columnCount - 1]];
                headerRangeForFilter.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);

                // Asynchrones Kopieren der Daten aus dem DataGridView in das Excel-Arbeitsblatt
                await CopyDataToExcelAsync(worksheet);

                // Erstellen des Liniendiagramms durch Auswahl aus der ListBox
                if (ListBoxXYZ.CheckedItems.Contains("X-Werte"))
                {
                    CreateLineChartX(worksheet);
                }
                if (ListBoxXYZ.CheckedItems.Contains("Y-Werte"))
                {
                    CreateLineChartY(worksheet);
                }
                if (ListBoxXYZ.CheckedItems.Contains("Z-Werte"))
                {
                    CreateLineChartZ(worksheet);
                }
                if (ListBoxXYZ.CheckedItems.Contains("C_AB"))
                {
                    CreateLineChartCAB(worksheet);
                }

                // Erfolgsmeldung anzeigen
                MessageBox.Show("Das Excelfile wurde erstellt.\nOk drücken um das File zu öffnen");

                // Öffnen der Excel-Anwendung
                excelApp.Visible = true;

                // Labeltext anpassen
                LblProgress.Text = "0%";
            }
            catch (Exception ex)
            {
                // Fehlermeldung anzeigen, wenn ein Fehler auftritt
                MessageBox.Show("Fehler beim Exportieren der Daten: " + ex.Message);
            }
            finally
            {
                // Sicherstellen, dass alle Excel-Objekte freigegeben werden
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

        // Asynchrone Methode zum Kopieren der Daten aus dem DataGridView in das Excel-Arbeitsblatt
        private async Task CopyDataToExcelAsync(Worksheet worksheet)
        {
            try
            {
                // Anzahl der Zeilen und sichtbaren Spalten im DataGridView abrufen
                int rowCount = DgvFarbauswertung.Rows.Count;
                int columnCount = DgvFarbauswertung.Columns.Cast<DataGridViewColumn>().Count(column => column.Visible);

                // Fortschrittsbalken initialisieren
                ProgressBarImport.Minimum = 0;
                ProgressBarImport.Maximum = rowCount;
                ProgressBarImport.Value = 0;
                ProgressBarImport.Step = 1;

                // Erstellung eines zweidimensionalen Arrays zur Speicherung der Daten
                var data = new object[rowCount, columnCount];

                // Die Daten werden in einem asynchronen Task kopiert, um die Benutzeroberfläche reaktionsschnell zu halten
                await Task.Run(() =>
                {
                    // Schleife durchläuft jede Zeile des DataGridViews
                    for (int i = 0; i < rowCount; i++)
                    {
                        int visibleColumnIndex = 0; // Index für sichtbare Spalten initialisieren

                        // Schleife durchläuft jede Spalte des DataGridViews
                        for (int j = 0; j < DgvFarbauswertung.Columns.Count; j++)
                        {
                            // Überprüfen, ob die Spalte sichtbar ist
                            if (DgvFarbauswertung.Columns[j].Visible)
                            {
                                // Daten aus dem DataGridView werden in das Array kopiert
                                data[i, visibleColumnIndex] = DgvFarbauswertung.Rows[i].Cells[j].Value;
                                visibleColumnIndex++; // Inkrementiere den Index für sichtbare Spalten
                            }
                        }

                        // Fortschrittsbalken aktualisieren
                        ProgressBarImport.Invoke((System.Action)(() =>
                        {
                            ProgressBarImport.PerformStep(); // Schritt des Fortschrittsbalkens erhöhen
                                                             // Fortschritt in Prozent berechnen und im Label anzeigen
                            double progressbarPercentage = (double)(i + 1) / rowCount * 100;
                            LblProgress.Text = $"Fortschritt: {progressbarPercentage:F2}% in Zwischenspeicher kopiert!";
                            // Überprüfen, ob der Fortschrittsbalken den maximalen Wert erreicht hat
                            if (ProgressBarImport.Value == ProgressBarImport.Maximum)
                            {
                                // Fortschrittsbalken zurücksetzen
                                ProgressBarImport.Value = 0;
                                LblProgress.Text = "Daten vollständig in Zwischenspeicher abgelegt. Excel wird erstellt";
                            }
                        }));
                    }
                });

                // Kopieren der Daten aus dem Array in das Excel-Arbeitsblatt
                Range dataRange = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[rowCount + 1, columnCount]];
                dataRange.Value = data;

                // Zellenformatierung: Textausrichtung zentrieren
                dataRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                // Freigeben der verwendeten COM-Objekte
                Marshal.ReleaseComObject(dataRange);
            }
            catch (Exception ex)
            {
                // Fehlermeldung anzeigen, wenn ein Fehler auftritt
                MessageBox.Show("Fehler beim Kopieren der Daten in Excel: " + ex.Message);
            }
        }

        // Methode zum Erstellen des Liniendiagramms_Z
        private void CreateLineChartZ(Worksheet worksheet)
        {
            try
            {
                // Erstellen des Chart-Objekts
                ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                ChartObject chartObject = chartObjects.Add(400, 40, 600, 300);
                Chart chart = chartObject.Chart;

                // Diagrammtyp festlegen
                chart.ChartType = XlChartType.xlLine;

                // Datenbereich für das Diagramm festlegen
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Range yRange1 = worksheet.Range["L2:L" + (dataRowCount + 1)]; // Erster Wert
                Range yRange2 = worksheet.Range["R2:R" + (dataRowCount + 1)]; // Zweiter Wert

                // Fortlaufende numerische Werte für die X-Achse festlegen
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++)
                {
                    xValues[i] = i + 1;
                }

                // Erste Linie hinzufügen
                Series series1 = (Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues;
                series1.Values = yRange1;
                series1.Name = "Z_I"; // Name der ersten Linie

                // Zweite Linie hinzufügen
                Series series2 = (Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues;
                series2.Values = yRange2;
                series2.Name = "Z_A"; // Name der zweiten Linie

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline1 = (Trendline)series1.Trendlines().Add();
                trendline1.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline1.Name = "Trend_Z_I";
                trendline1.Border.Color = Color.DarkBlue;

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline2 = (Trendline)series2.Trendlines().Add();
                trendline2.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline2.Name = "Trend_Z_A";
                trendline2.Border.Color = Color.DarkOrange;

                // Diagrammtitel setzen
                chart.HasTitle = true;
                chart.ChartTitle.Text = "Z-Werte";

                // Achsenbeschriftungen setzen
                Axis xAxis = (Axis)chart.Axes(XlAxisType.xlCategory, XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->        Altäre Chargen";

                Axis yAxis = (Axis)chart.Axes(XlAxisType.xlValue, XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message);
            }
        }

        // Methode zum Erstellen des Liniendiagramms_Y
        private void CreateLineChartY(Worksheet worksheet)
        {
            try
            {
                // Erstellen des Chart-Objekts
                ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                ChartObject chartObject = chartObjects.Add(500, 100, 600, 300);
                Chart chart = chartObject.Chart;

                // Diagrammtyp festlegen
                chart.ChartType = XlChartType.xlLine;

                // Datenbereich für das Diagramm festlegen
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Range yRange1 = worksheet.Range["K2:K" + (dataRowCount + 1)]; // Erster Wert
                Range yRange2 = worksheet.Range["Q2:Q" + (dataRowCount + 1)]; // Zweiter Wert

                // Fortlaufende numerische Werte für die X-Achse festlegen
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++)
                {
                    xValues[i] = i + 1;
                }

                // Erste Linie hinzufügen
                Series series1 = (Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues;
                series1.Values = yRange1;
                series1.Name = "Y_I"; // Name der ersten Linie

                // Zweite Linie hinzufügen
                Series series2 = (Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues;
                series2.Values = yRange2;
                series2.Name = "Y_A"; // Name der zweiten Linie

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline1 = (Trendline)series1.Trendlines().Add();
                trendline1.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline1.Name = "Trend_A_I";
                trendline1.Border.Color = Color.DarkBlue;

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline2 = (Trendline)series2.Trendlines().Add();
                trendline2.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline2.Name = "Trend_A_A";
                trendline2.Border.Color = Color.DarkOrange;

                // Diagrammtitel setzen
                chart.HasTitle = true;
                chart.ChartTitle.Text = "Y-Werte";

                // Achsenbeschriftungen setzen
                Axis xAxis = (Axis)chart.Axes(XlAxisType.xlCategory, XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->            Altäre Chargen";

                Axis yAxis = (Axis)chart.Axes(XlAxisType.xlValue, XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message);
            }
        }

        // Methode zum Erstellen des Liniendiagramms_X
        private void CreateLineChartX(Worksheet worksheet)
        {
            try
            {
                // Erstellen des Chart-Objekts
                ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                ChartObject chartObject = chartObjects.Add(600, 160, 600, 300);
                Chart chart = chartObject.Chart;

                // Diagrammtyp festlegen
                chart.ChartType = XlChartType.xlLine;

                // Datenbereich für das Diagramm festlegen
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Range yRange1 = worksheet.Range["J2:J" + (dataRowCount + 1)]; // Erster Wert
                Range yRange2 = worksheet.Range["P2:P" + (dataRowCount + 1)]; // Zweiter Wert

                // Fortlaufende numerische Werte für die X-Achse festlegen
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++)
                {
                    xValues[i] = i + 1;
                }

                // Erste Linie hinzufügen
                Series series1 = (Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues;
                series1.Values = yRange1;
                series1.Name = "X_I"; // Name der ersten Linie

                // Zweite Linie hinzufügen
                Series series2 = (Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues;
                series2.Values = yRange2;
                series2.Name = "X_A"; // Name der zweiten Linie

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline1 = (Trendline)series1.Trendlines().Add();
                trendline1.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline1.Name = "Trend_X_I";
                trendline1.Border.Color = Color.DarkBlue;

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline2 = (Trendline)series2.Trendlines().Add();
                trendline2.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline2.Name = "Trend_X_A";
                trendline2.Border.Color = Color.DarkOrange;

                // Diagrammtitel setzen
                chart.HasTitle = true;
                chart.ChartTitle.Text = "X-Werte";

                // Achsenbeschriftungen setzen
                Axis xAxis = (Axis)chart.Axes(XlAxisType.xlCategory, XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->        Altäre Chargen";

                Axis yAxis = (Axis)chart.Axes(XlAxisType.xlValue, XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message);
            }
        }

        // Methode zum Erstellen des Liniendiagramms_C_AB
        private void CreateLineChartCAB(Worksheet worksheet)
        {
            try
            {
                // Erstellen des Chart-Objekts
                ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
                ChartObject chartObject = chartObjects.Add(700, 220, 600, 300);
                Chart chart = chartObject.Chart;

                // Diagrammtyp festlegen
                chart.ChartType = XlChartType.xlLine;

                // Datenbereich für das Diagramm festlegen
                int dataRowCount = DgvFarbauswertung.Rows.Count;
                Range yRange1 = worksheet.Range["N2:N" + (dataRowCount + 1)]; // Erster Wert
                Range yRange2 = worksheet.Range["T2:T" + (dataRowCount + 1)]; // Zweiter Wert

                // Fortlaufende numerische Werte für die X-Achse festlegen
                object[] xValues = new object[dataRowCount];
                for (int i = 0; i < dataRowCount; i++)
                {
                    xValues[i] = i + 1;
                }

                // Erste Linie hinzufügen
                Series series1 = (Series)chart.SeriesCollection().NewSeries();
                series1.XValues = xValues;
                series1.Values = yRange1;
                series1.Name = "C_AB_I"; // Name der ersten Linie

                // Zweite Linie hinzufügen
                Series series2 = (Series)chart.SeriesCollection().NewSeries();
                series2.XValues = xValues;
                series2.Values = yRange2;
                series2.Name = "C_AB_A"; // Name der zweiten Linie

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline1 = (Trendline)series1.Trendlines().Add();
                trendline1.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline1.Name = "Trend_C_AB_I";
                trendline1.Border.Color = Color.DarkBlue;

                // Trendlinie zur ersten Linie hinzufügen
                Trendline trendline2 = (Trendline)series2.Trendlines().Add();
                trendline2.Type = XlTrendlineType.xlLinear; // Typ der Trendlinie festlegen
                trendline2.Name = "Trend_C_AB_A";
                trendline2.Border.Color = Color.DarkOrange;

                // Diagrammtitel setzen
                chart.HasTitle = true;
                chart.ChartTitle.Text = "C_AB-Werte";

                // Achsenbeschriftungen setzen
                Axis xAxis = (Axis)chart.Axes(XlAxisType.xlCategory, XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = "Neuesten Chargen        <- Anzahl ->        Altäre Chargen";

                Axis yAxis = (Axis)chart.Axes(XlAxisType.xlValue, XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = "Wert";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Liniendiagramms: " + ex.Message);
            }
        }


    }
}

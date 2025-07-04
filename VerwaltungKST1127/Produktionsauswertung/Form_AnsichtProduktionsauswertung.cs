using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;

namespace VerwaltungKST1127.Produktionsauswertung
{
    public partial class Form_AnsichtProduktionsauswertung : Form
    {
        public Form_AnsichtProduktionsauswertung()
        {
            InitializeComponent();            
        }

        // Klasse für einen einzelnen Maschinen-Datensatz in der JSON-Datei
        private class MaschinenRecord
        {
            public string Datum { get; set; }
            public string Uhrzeit { get; set; }
            public string Status { get; set; }
            public string RawLine { get; set; }
        }

        // Helfer-Methode: wandelt Zeitdauer in Sekunden/Minuten/Stunden zu Minuten (double)
        // Erkennt flexibel: ss, mm:ss, hh:mm:ss
        // Begrenzt Ergebnis auf max. 16 Stunden (960 Minuten), damit keine unrealistischen Werte entstehen
        private double ParseZeitdauerToMinutes(string zeitdauer)
        {
            if (string.IsNullOrWhiteSpace(zeitdauer)) return 0;

            var parts = zeitdauer.Split(':');

            double resultMinutes = 0;

            if (parts.Length == 1)
            {
                // Nur Sekunden
                if (int.TryParse(parts[0], out int seconds))
                    resultMinutes = seconds / 60.0;
            }
            else if (parts.Length == 2)
            {
                // Minuten:Sekunden
                if (int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int seconds))
                    resultMinutes = minutes + (seconds / 60.0);
            }
            else if (parts.Length == 3)
            {
                // Stunden:Minuten:Sekunden
                if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes) && int.TryParse(parts[2], out int seconds))
                    resultMinutes = (hours * 60) + minutes + (seconds / 60.0);
            }

            // Sicherheitslimit: Max. 16 Stunden (960 Minuten)
            if (resultMinutes > 960)
                resultMinutes = 960;

            return resultMinutes;
        }

        // Hilfsmethode: versucht eine Zeitdauer flexibel in TimeSpan zu parsen (hh:mm:ss oder mm:ss oder ss)
        private TimeSpan ParseZeitdauerToTimeSpan(string zeitdauer)
        {
            if (string.IsNullOrWhiteSpace(zeitdauer)) return TimeSpan.Zero;

            var parts = zeitdauer.Split(':');

            if (parts.Length == 1)
            {
                // Sekunden
                if (int.TryParse(parts[0], out int seconds))
                    return TimeSpan.FromSeconds(seconds);
            }
            else if (parts.Length == 2)
            {
                // Minuten:Sekunden
                if (int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int seconds))
                    return new TimeSpan(0, minutes, seconds);
            }
            else if (parts.Length == 3)
            {
                // Stunden:Minuten:Sekunden
                if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes) && int.TryParse(parts[2], out int seconds))
                    return new TimeSpan(hours, minutes, seconds);
            }

            return TimeSpan.Zero;
        }

        // Event-Handler: wird aufgerufen, wenn sich das Datum im DateTimePicker ändert
        private void dateTimePickerLastProduction_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Alle Textboxen für die späteste Zeit zurücksetzen
                txtBoxLastTimeA20.Text = "";
                txtBoxLastTimeA25.Text = "";
                txtBoxLastTimeA30.Text = "";
                txtBoxLastTimeA35.Text = "";
                txtBoxLastTimeA40.Text = "";
                txtBoxLastTimeA45.Text = "";
                txtBoxLastTimeA50.Text = "";
                txtBoxLastTimeA60.Text = "";
                txtBoxLastTimeA65.Text = "";

                // Alle Produktivitäts-Textboxen zurücksetzen
                txtBoxProductivA20.Text = "";
                txtBoxProductivA25.Text = "";
                txtBoxProductivA30.Text = "";
                txtBoxProductivA35.Text = "";
                txtBoxProductivA40.Text = "";
                txtBoxProductivA45.Text = "";
                txtBoxProductivA50.Text = "";
                txtBoxProductivA60.Text = "";
                txtBoxProductivA65.Text = "";

                // Gewähltes Datum aus dem DateTimePicker in deutsches Datumsformat
                DateTime selectedDate = dateTimePickerLastProduction.Value.Date;
                string dateString = selectedDate.ToString("dd.MM.yyyy");

                // Mapping der Ordnernamen auf die entsprechenden Textboxen für Zeit und Produktivität
                var anlagen = new Dictionary<string, (TextBox TimeBox, TextBox ProductiveBox)>
        {
            {"Anl_20", (txtBoxLastTimeA20, txtBoxProductivA20) },
            { "Anl_25", (txtBoxLastTimeA25, txtBoxProductivA25) },
            { "Anl_30", (txtBoxLastTimeA30, txtBoxProductivA30) },
            { "Anl_35", (txtBoxLastTimeA35, txtBoxProductivA35) },
            { "Anl_40", (txtBoxLastTimeA40, txtBoxProductivA40) },
            { "Anl_45", (txtBoxLastTimeA45, txtBoxProductivA45) },
            { "Anl_50", (txtBoxLastTimeA50, txtBoxProductivA50) },
            { "Anl_60", (txtBoxLastTimeA60, txtBoxProductivA60) },
            { "Anl_65", (txtBoxLastTimeA65, txtBoxProductivA65) },
        };

                // Basis-Netzlaufwerkpfad
                string basePath = @"P:\\TEDuTOZ\\MDE";

                // Schlüsselwörter, die in den Zeilen nicht erlaubt sind (werden herausgefiltert)
                var auszuschliessen = new[] { "Heizen", "Lecktest", "Saugleistung" };

                // Gesamtergebnisse für JSON-Erstellung sammeln
                var allResults = new Dictionary<string, List<MaschinenRecord>>();

                // Schleife über alle Anlagenordner und zugehörigen Textboxen
                foreach (var kvp in anlagen)
                {
                    string anlFolder = kvp.Key;
                    var (timeBox, productiveBox) = kvp.Value;

                    // Dateipfad zur Logdatei dieser Anlage
                    string machineFilePath = Path.Combine(basePath, anlFolder, "machine_state_ge.log");

                    var records = new List<MaschinenRecord>();

                    // Prüfen ob Datei existiert
                    if (File.Exists(machineFilePath))
                    {
                        using (var reader = new StreamReader(machineFilePath, Encoding.GetEncoding("windows-1252")))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                line = line.Trim();

                                // Überspringen wenn die Zeile zu kurz oder leer ist
                                if (string.IsNullOrWhiteSpace(line) || line.Length < 19) continue;

                                // Steuerzeichen (z. B. Nullbytes) aus der Zeile entfernen
                                line = Regex.Replace(line, "[\u0000-\u001F\u007F]+", "");

                                // Wenn einer der Filterbegriffe in der ZEILE vorkommt -> sofort raus
                                if (auszuschliessen.Any(keyword => line.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                                    continue;

                                // Datum+Uhrzeit auslesen und prüfen
                                string dateTimePart = line.Substring(0, 19);
                                if (!DateTime.TryParseExact(dateTimePart, "dd.MM.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
                                    continue;
                                if (parsedDateTime.Date != selectedDate.Date) continue;

                                // Rezept-Teil gezielt prüfen
                                int rezeptPos = line.IndexOf("Rezept = ");
                                if (rezeptPos >= 0)
                                {
                                    string rezeptName = line.Substring(rezeptPos + "Rezept = ".Length).Trim();
                                    if (auszuschliessen.Any(keyword => rezeptName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                                        continue;
                                }

                                // Alles okay -> Record speichern
                                records.Add(new MaschinenRecord
                                {
                                    Datum = line.Substring(0, 10),
                                    Uhrzeit = line.Substring(11, 8),
                                    Status = line.IndexOf("Maschinenstatus = ") > 0 ? line.Substring(line.IndexOf("Maschinenstatus = ")).Trim() : "",
                                    RawLine = line
                                });
                            }
                        }
                    }

                    // Ergebnisse im Gesamt-Dictionary für JSON sichern
                    allResults[anlFolder] = records;

                    if (records.Any())
                    {
                        // Späteste Produktionszeit bestimmen und in Textbox schreiben
                        var latestRecord = records
                            .OrderBy(r => DateTime.ParseExact(r.Datum + " " + r.Uhrzeit, "dd.MM.yyyy HH:mm:ss", null))
                            .Last();

                        timeBox.Text = latestRecord.Uhrzeit;

                        TimeSpan totalDuration = TimeSpan.Zero;

                        foreach (var rec in records)
                        {
                            int pos = rec.RawLine.IndexOf("Zeitdauer = ");
                            if (pos >= 0)
                            {
                                // Prüfen ob Rezeptteil mit "Rezept = B" beginnt
                                int rezeptPos = rec.RawLine.IndexOf("Rezept = ");
                                if (rezeptPos >= 0)
                                {
                                    string rezeptName = rec.RawLine.Substring(rezeptPos + "Rezept = ".Length).Trim();

                                    // Nur Rezepte zählen, die mit B, S, P, U, A oder W beginnen
                                    if (!(rezeptName.StartsWith("B", StringComparison.OrdinalIgnoreCase)
                                          || rezeptName.StartsWith("P", StringComparison.OrdinalIgnoreCase)
                                          || rezeptName.StartsWith("U", StringComparison.OrdinalIgnoreCase)
                                          || rezeptName.StartsWith("W", StringComparison.OrdinalIgnoreCase)
                                          || rezeptName.StartsWith("A", StringComparison.OrdinalIgnoreCase)
                                          || (rezeptName.Length >= 2 && rezeptName.StartsWith("S", StringComparison.OrdinalIgnoreCase) && char.IsDigit(rezeptName[1]))))
                                          // Weitere Ausnahmen einfügen
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    // Kein Rezept-Teil? Überspringen.
                                    continue;
                                }

                                // Zeitdauer-Teil auslesen und summieren
                                string zeitdauer = rec.RawLine.Substring(pos + "Zeitdauer = ".Length).Trim();
                                if (!string.IsNullOrEmpty(zeitdauer))
                                {
                                    totalDuration += ParseZeitdauerToTimeSpan(zeitdauer);
                                }
                            }
                        }

                        // Ergebnis schön formatieren
                        productiveBox.Text = string.Format("{0:D2}:{1:D2}:{2:D2} h", (int)totalDuration.TotalHours, totalDuration.Minutes, totalDuration.Seconds);

                    }
                    else
                    {
                        // Keine Daten -> Textboxen leer lassen
                        timeBox.Text = "";
                        productiveBox.Text = "";
                    }
                }

                // JSON-Datei mit allen Daten im Projektordner speichern
                string jsonFileName = "Maschinenstatus.json";
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFileName);

                using (FileStream fs = new FileStream(jsonPath, FileMode.Create, FileAccess.Write))
                {
                    System.Text.Json.JsonSerializer.Serialize(fs, allResults, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                }

                MessageBox.Show($"JSON-Datei 'Maschinenstatus.json' wurde erfolgreich erstellt/überschrieben im Projektordner.");
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung mit Meldung
                MessageBox.Show($"Fehler: {ex.Message}");
            }
        }
    }
}

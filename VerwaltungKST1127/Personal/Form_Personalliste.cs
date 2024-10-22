using System; // Grundlegender Namespace, der grundlegende Datentypen und Funktionen (wie Exception-Handling, Datums- und Zeitfunktionen) bereitstellt.
using System.Collections.Generic; // Namespace für die Nutzung generischer Datentypen wie Listen, Dictionaries etc.
using System.ComponentModel; // Importieren des System.ComponentModel-Namespace für Komponentenmodelle und Datenbindung (z.B. zum Arbeiten mit Events und benachrichtigungsfähigen Eigenschaften)
using System.Drawing; // Namespace für grafische Elemente wie Farben, Schriftarten, Bilder und Formen. Wird häufig in der Gestaltung von Benutzeroberflächen verwendet.
using System.Linq; // Namespace, der LINQ (Language Integrated Query) bereitstellt. Ermöglicht das Durchführen von Abfragen und das Verarbeiten von Sammlungen (z. B. List<T>, Arrays) auf eine deklarative Weise.
using System.Windows.Forms; // Namespace für Windows Forms-Anwendungen. Enthält Klassen für die Erstellung grafischer Benutzeroberflächen (GUIs) wie Formulare, Schaltflächen, Textfelder etc.

namespace VerwaltungKST1127.Personal
{
    public partial class Form_Personalliste : Form
    {
        private List<Mitarbeiter> _mitarbeiterListe = new List<Mitarbeiter>();
        private MitarbeiterService _mitarbeiterService;
        private int _currentMitarbeiterId = 0;

        public Form_Personalliste()
        {
            InitializeComponent();

            // Initialisieren des MitarbeiterService
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mitarbeiter.json");
            _mitarbeiterService = new MitarbeiterService(filePath);

            // Mitarbeiterdaten laden und DataGridView initialisieren
            _mitarbeiterListe = _mitarbeiterService.LoadMitarbeiter();

            // Höchste ID finden und setzen
            if (_mitarbeiterListe.Any())
            {
                _currentMitarbeiterId = _mitarbeiterListe.Max(m => m.Id);
            }

            RefreshDataGridView();
            // Registrierung des CellFormatting-Ereignisses
            DgvPersonalliste.CellFormatting += DgvPersonalliste_CellFormatting;
            SetPlaceholders();
        }

        // Aktualisiert das DataGridView mit der Mitarbeiterliste
        private void RefreshDataGridView()
        {
            // Berechne Alter und Jahre im Unternehmen für jeden Mitarbeiter in der Liste
            foreach (var mitarbeiter in _mitarbeiterListe)
            {
                mitarbeiter.Alter = BerechneAlter(mitarbeiter.Geburtstag);
                mitarbeiter.JahreUnternehmen = BerechneJahreImUnternehmen(mitarbeiter.Eintritt);
            }

            // Sortiere die Liste nach dem "Team"
            var sortierteListe = _mitarbeiterListe.OrderBy(m => m.Team).ToList();

            // Erstelle eine BindingSource
            BindingSource bindingSource = new BindingSource();

            // Binde die sortierte Liste an die BindingSource
            bindingSource.DataSource = sortierteListe;

            // Weise die BindingSource dem DataGridView zu
            DgvPersonalliste.DataSource = bindingSource;

            // Formatieren der Datums-Spalten
            DgvPersonalliste.Columns["Eintritt"].DefaultCellStyle.Format = "dd.MM.yyyy";
            DgvPersonalliste.Columns["Geburtstag"].DefaultCellStyle.Format = "dd.MM.yyyy";

            // Optional: Die Spaltenbreite anpassen; mittig anzeigen
            //DgvPersonalliste.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(DataGridView.DefaultFont, FontStyle.Bold);
            DgvPersonalliste.ColumnHeadersDefaultCellStyle.Alignment =DataGridViewContentAlignment.MiddleCenter;
            //DgvPersonalliste.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DgvPersonalliste.Columns[0].Width = 50;
            DgvPersonalliste.Columns[1].Width = 90;
            DgvPersonalliste.Columns[4].Width = 170;
            DgvPersonalliste.Columns[8].Width = 110;
            DgvPersonalliste.Columns[10].Width = 80;
            DgvPersonalliste.Columns[13].Width = 110;
        }

        // Methode zur Berechnung des Alters basierend auf dem Geburtstag
        private int BerechneAlter(DateTime geburtstag)
        {
            var heute = DateTime.Today;
            var alter = heute.Year - geburtstag.Year;
            if (geburtstag > heute.AddYears(-alter)) alter--; // Falls der Geburtstag noch nicht erreicht wurde, ein Jahr abziehen
            return alter;
        }

        // Methode zur Berechnung der Jahre im Unternehmen basierend auf dem Eintrittsdatum
        private int BerechneJahreImUnternehmen(DateTime eintritt)
        {
            var heute = DateTime.Today;
            var jahreImUnternehmen = heute.Year - eintritt.Year;
            if (eintritt > heute.AddYears(-jahreImUnternehmen)) jahreImUnternehmen--; // Falls der Eintrittstag noch nicht erreicht wurde, ein Jahr abziehen
            return jahreImUnternehmen;
        }

        // Funktion um die felder zu leeren wenn eine weiter seite eingegeben werden soll
        private void SetPlaceholders()
        {
            SetPlaceholder(txtboxPersonalnummer, "123456789");
            SetPlaceholder(txtboxVorname, "Günter");
            SetPlaceholder(txtboxNachname, "Günsterer");

            SetPlaceholder(txtboxTelefonnummer, "0664/9999999");
            SetPlaceholder(txtboxWochenstunden, "00,0");


            // Füge Platzhalter zu ComboBoxen hinzu
            AddPlaceholderToComboBox(comboBoxPosition, "Dropdown verwenden");
            AddPlaceholderToComboBox(comboboxTeam, "Dropdown verwenden");
            AddPlaceholderToComboBox(comboboxDirekterVorgesetzter, "Dropdown verwenden");
            AddPlaceholderToComboBox(comboboxLohngruppe, "Dropdown verwenden");
        }

        private void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholderText;

            textBox.GotFocus += (sender, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = ""; // Leere den Text
                    textBox.ForeColor = SystemColors.WindowText; // Setze die Schriftfarbe auf Standard
                }
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = placeholderText; // Setze den Platzhaltertext zurück
                    textBox.ForeColor = Color.Gray; // Setze die Schriftfarbe wieder auf Grau
                }
            };
        }

        private void AddPlaceholderToComboBox(ComboBox comboBox, string placeholderText)
        {
            comboBox.ForeColor = Color.Gray; // Setze die Schriftfarbe auf Grau
            comboBox.Text = placeholderText; // Setze den Platzhaltertext

            comboBox.GotFocus += (sender, e) =>
            {
                if (comboBox.Text == placeholderText)
                {
                    comboBox.Text = ""; // Leere den Text
                    comboBox.ForeColor = SystemColors.WindowText; // Setze die Schriftfarbe auf Standard
                }
            };

            comboBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrEmpty(comboBox.Text))
                {
                    comboBox.Text = placeholderText; // Setze den Platzhaltertext zurück
                    comboBox.ForeColor = Color.Gray; // Setze die Schriftfarbe wieder auf Grau
                }
            };

            comboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (comboBox.SelectedIndex != -1) // Wenn eine Auswahl getroffen wurde
                {
                    comboBox.ForeColor = SystemColors.WindowText; // Setze die Schriftfarbe auf Standard
                }
            };
        }

        // Setzt die Eingabefelder zurück
        private void ClearInputFields()
        {
            txtboxPersonalnummer.Clear();
            txtboxVorname.Clear();
            txtboxNachname.Clear();
            comboBoxPosition.SelectedIndex = -1;
            datetimepickerEintritt.Value = DateTime.Now;
            datetimepickerGeburtstag.Value = DateTime.Now;
            comboboxTeam.SelectedIndex = -1;
            comboboxDirekterVorgesetzter.SelectedIndex = -1;
            txtboxTelefonnummer.Clear();
            txtboxWochenstunden.Clear();
            comboboxLohngruppe.SelectedIndex = -1;
            SetPlaceholders();
        }

        // ######################Button Events########
        private void BtnNeuerMitarbeiter_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob alle erforderlichen Felder ausgefüllt sind
            if (IsFieldEmpty(txtboxPersonalnummer, "123456789") ||
                IsFieldEmpty(txtboxVorname, "Günter") ||
                IsFieldEmpty(txtboxNachname, "Günsterer") ||
                IsFieldEmpty(comboBoxPosition, "Dropdown verwenden") ||
                IsFieldEmpty(comboboxTeam, "Dropdown verwenden") ||
                IsFieldEmpty(comboboxDirekterVorgesetzter, "Dropdown verwenden") ||
                IsFieldEmpty(txtboxTelefonnummer, "0664/9999999") ||
                IsFieldEmpty(txtboxWochenstunden, "00,0") ||
                IsFieldEmpty(comboboxLohngruppe, "Dropdown verwenden"))
            {
                MessageBox.Show("Bitte füllen Sie alle erforderlichen Felder aus!", "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Abbrechen, wenn ein Feld leer ist
            }

            // Überprüfen, ob Personalnummer und Wochenstunden gültige Zahlen sind
            if (!int.TryParse(txtboxPersonalnummer.Text, out int personalnummer))
            {
                MessageBox.Show("Gültige Personalnummer auswählen!", "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtboxWochenstunden.Text, out decimal wochenstunden))
            {
                MessageBox.Show("Gültige Wochenstunden eingeben!", "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sicherheitsabfrage, bevor ein neuer Mitarbeiter hinzugefügt wird
            var result = MessageBox.Show("Mitarbeiter hinzufügen?",
                                          "Mitarbeiter hinzufügen",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var neuerMitarbeiter = new Mitarbeiter
                {
                    Id = ++_currentMitarbeiterId,
                    Personalnummer = personalnummer,
                    Vorname = txtboxVorname.Text,
                    Nachname = txtboxNachname.Text,
                    Position = comboBoxPosition.Text,
                    Eintritt = datetimepickerEintritt.Value,
                    Geburtstag = datetimepickerGeburtstag.Value,
                    Team = int.Parse(comboboxTeam.Text),
                    DirekterVorgesetzter = comboboxDirekterVorgesetzter.Text,
                    Telefonnummer = txtboxTelefonnummer.Text,
                    Wochenstunden = wochenstunden,
                    Lohngruppe = comboboxLohngruppe.Text,
                };

                _mitarbeiterListe.Add(neuerMitarbeiter);
                _mitarbeiterService.SaveMitarbeiter(_mitarbeiterListe); // Liste speichern
                RefreshDataGridView();
                ClearInputFields(); // Textfelder zurücksetzen
            }
        }

        // Neue Methode um zu prüfen, ob das Feld leer ist oder der Platzhaltertext angezeigt wird
        private bool IsFieldEmpty(Control control, string placeholderText)
        {
            if (control is TextBox textBox)
            {
                return string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == placeholderText;
            }
            else if (control is ComboBox comboBox)
            {
                return string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.Text == placeholderText;
            }
            return false;
        }

        private void BtnMitarbeiterLoeschen_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob alle erforderlichen Felder ausgefüllt sind
            if (IsFieldEmpty(txtboxPersonalnummer, "123456789") ||
                IsFieldEmpty(txtboxVorname, "Günter") ||
                IsFieldEmpty(txtboxNachname, "Günsterer") ||
                IsFieldEmpty(comboBoxPosition, "Dropdown verwenden") ||
                IsFieldEmpty(comboboxTeam, "Dropdown verwenden") ||
                IsFieldEmpty(comboboxDirekterVorgesetzter, "Dropdown verwenden") ||
                IsFieldEmpty(txtboxTelefonnummer, "0664/9999999") ||
                IsFieldEmpty(txtboxWochenstunden, "00,0") ||
                IsFieldEmpty(comboboxLohngruppe, "Dropdown verwenden"))
            {
                MessageBox.Show("Mitarbeiter zuerst auswählen!", "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Abbrechen, wenn ein Feld leer ist
            }

            if (DgvPersonalliste.CurrentRow != null)
            {
                var result = MessageBox.Show("Mitarbeiter löschen?",
                                              "Mitarbeiter löschen",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var selectedMitarbeiter = (Mitarbeiter)DgvPersonalliste.CurrentRow.DataBoundItem;
                    _mitarbeiterListe.Remove(selectedMitarbeiter);
                    _mitarbeiterService.SaveMitarbeiter(_mitarbeiterListe); // Liste speichern
                    RefreshDataGridView();
                    ClearInputFields(); // Textfelder zurücksetzen
                }
            }
            else
            {
                MessageBox.Show("Zuerst Mitarbeiter aus Liste auswählen.",
                                "Mitarbeiter löschen",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void BtnMitarbeiterAnpassen_Click(object sender, EventArgs e)
        {
            // Überprüfen, ob eine Zeile im DataGridView ausgewählt ist
            if (IsFieldEmpty(txtboxPersonalnummer, "123456789") ||
                IsFieldEmpty(txtboxVorname, "Günter") ||
                IsFieldEmpty(txtboxNachname, "Günsterer") ||
                IsFieldEmpty(comboBoxPosition, "Dropdown verwenden") ||
                IsFieldEmpty(comboboxTeam, "Dropdown verwenden") ||
                IsFieldEmpty(comboboxDirekterVorgesetzter, "Dropdown verwenden") ||
                IsFieldEmpty(txtboxTelefonnummer, "0664/9999999") ||
                IsFieldEmpty(txtboxWochenstunden, "00,0") ||
                IsFieldEmpty(comboboxLohngruppe, "Dropdown verwenden"))
            {
                MessageBox.Show("Mitarbeiter zuerst auswählen!", "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Abbrechen, wenn ein Feld leer ist
            }

            if (DgvPersonalliste.CurrentRow != null)
            {
                var result = MessageBox.Show("Änderungen von Mitarbeiter speichern?",
                                              "Mitarbeiter anpassen",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Den ausgewählten Mitarbeiter aus dem DataGridView abrufen
                    var selectedMitarbeiter = (Mitarbeiter)DgvPersonalliste.CurrentRow.DataBoundItem;

                    // Die Änderungen aus den Eingabefeldern in das Mitarbeiter-Objekt übertragen
                    selectedMitarbeiter.Personalnummer = int.Parse(txtboxPersonalnummer.Text);
                    selectedMitarbeiter.Vorname = txtboxVorname.Text;
                    selectedMitarbeiter.Nachname = txtboxNachname.Text;
                    selectedMitarbeiter.Position = comboBoxPosition.Text;
                    selectedMitarbeiter.Eintritt = datetimepickerEintritt.Value; // Datum formatieren
                    selectedMitarbeiter.Geburtstag = datetimepickerGeburtstag.Value; // Datum formatieren
                    selectedMitarbeiter.Team = int.Parse(comboboxTeam.Text);
                    selectedMitarbeiter.DirekterVorgesetzter = comboboxDirekterVorgesetzter.Text;
                    selectedMitarbeiter.Telefonnummer = txtboxTelefonnummer.Text;
                    selectedMitarbeiter.Wochenstunden = decimal.Parse(txtboxWochenstunden.Text);
                    selectedMitarbeiter.Lohngruppe = comboboxLohngruppe.Text;

                    // Die aktualisierte Mitarbeiterliste speichern
                    _mitarbeiterService.SaveMitarbeiter(_mitarbeiterListe);

                    // DataGridView aktualisieren
                    RefreshDataGridView();

                    // Eingabefelder zurücksetzen
                    ClearInputFields();
                }
            }
            else
            {
                MessageBox.Show("Zuerst Mitarbeiter aus Liste auswählen.",
                                "Mitarbeiter anpassen",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        // Wird ausgelöst, wenn auf eine Spalte geklickt wird
        private void DgvPersonalliste_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Überprüfen, ob eine Zeile im DataGridView ausgewählt ist
            if (e.RowIndex >= 0) // Stelle sicher, dass die Zeile gültig ist
            {
                // Den ausgewählten Mitarbeiter aus dem DataGridView abrufen
                var selectedMitarbeiter = (Mitarbeiter)DgvPersonalliste.Rows[e.RowIndex].DataBoundItem;

                // Die Werte des ausgewählten Mitarbeiters in die Eingabefelder einfüllen
                txtboxPersonalnummer.Text = selectedMitarbeiter.Personalnummer.ToString();
                txtboxVorname.Text = selectedMitarbeiter.Vorname;
                txtboxNachname.Text = selectedMitarbeiter.Nachname;
                comboBoxPosition.Text = selectedMitarbeiter.Position;
                datetimepickerEintritt.Value = selectedMitarbeiter.Eintritt; // DateTimePicker direkt setzen
                datetimepickerGeburtstag.Value = selectedMitarbeiter.Geburtstag; // DateTimePicker direkt setzen
                comboboxTeam.Text = selectedMitarbeiter.Team.ToString(); // Hier sicherstellen, dass Team auch als string gesetzt werden kann
                comboboxDirekterVorgesetzter.Text = selectedMitarbeiter.DirekterVorgesetzter;
                txtboxTelefonnummer.Text = selectedMitarbeiter.Telefonnummer;
                txtboxWochenstunden.Text = selectedMitarbeiter.Wochenstunden.ToString();
                comboboxLohngruppe.Text = selectedMitarbeiter.Lohngruppe;
                // Setze die Schriftfarbe auf Standard (schwarz)
                txtboxPersonalnummer.ForeColor = SystemColors.WindowText;
                txtboxVorname.ForeColor = SystemColors.WindowText;
                txtboxNachname.ForeColor = SystemColors.WindowText;
                comboBoxPosition.ForeColor = SystemColors.WindowText;
                datetimepickerEintritt.ForeColor = SystemColors.WindowText;
                datetimepickerGeburtstag.ForeColor = SystemColors.WindowText;
                comboboxTeam.ForeColor = SystemColors.WindowText;
                comboboxDirekterVorgesetzter.ForeColor = SystemColors.WindowText;
                txtboxTelefonnummer.ForeColor = SystemColors.WindowText;
                txtboxWochenstunden.ForeColor = SystemColors.WindowText;
                comboboxLohngruppe.ForeColor = SystemColors.WindowText;
            }
            else
            {
                // Falls kein Mitarbeiter ausgewählt ist, Eingabefelder zurücksetzen
                ClearInputFields();
            }
        }

        // Grafische gesaltung des Dgvs
        private void DgvPersonalliste_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Prüfen, ob die Spalte "Team" betroffen ist (angenommen die Spalte heißt "Team")
            if (DgvPersonalliste.Columns[e.ColumnIndex].Name == "Team")
            {
                // Den Wert der Zelle in der Spalte "Team" holen
                var teamValue = DgvPersonalliste.Rows[e.RowIndex].Cells["Team"].Value;

                if (teamValue != null)
                {
                    int teamNumber;
                    // Versuche den Wert in eine Zahl zu konvertieren
                    if (int.TryParse(teamValue.ToString(), out teamNumber))
                    {
                        // Farben für die verschiedenen Teams festlegen
                        switch (teamNumber)
                        {
                            case 136:
                                DgvPersonalliste.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.SandyBrown; // Farbe für Team 136
                                break;
                            case 158:
                                DgvPersonalliste.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Silver; // Farbe für Team 158
                                break;
                            case 159:
                                DgvPersonalliste.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightSlateGray; // Farbe für Team 159
                                break;
                            case 160:
                                DgvPersonalliste.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen; // Farbe für Team 160
                                break;
                            default:
                                DgvPersonalliste.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White; // Standardfarbe
                                break;
                        }
                    }
                }
            }
        }
    }
}

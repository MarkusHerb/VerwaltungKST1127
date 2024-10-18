using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für Windows Forms-Steuerungen und UI-Elemente

namespace VerwaltungKST1127
{
    // Definiere den Namespace StatistikFarbauswertung
    public partial class Form_InputMenge : Form
    {
        // Deklariere eine öffentliche Eigenschaft namens InputValue vom Typ string mit privatem Setzer
        public string InputValue { get; private set; }

        // Deklariere eine öffentliche Eigenschaft namens EinheitMain vom Typ string mit privatem Setzer
        public string EinheitMain { get; set; }

        // Konstruktor für die Klasse Form_InputMenge
        public Form_InputMenge(string einheit)
        {
            // Initialisiere die Komponenten des Formulars
            InitializeComponent();
            EinheitMain = einheit;
            LblEinheit.Text = EinheitMain;
        }

        // Event-Handler für die Tastatureingabe im Eingabefeld
        private void TextBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Überprüfe, ob die Enter-Taste gedrückt wurde
                if (e.KeyCode == Keys.Enter)
                {
                    // Versuche, den Text aus dem Eingabefeld in eine Ganzzahl zu konvertieren
                    if (int.TryParse(TextBoxInput.Text, out int inputValue))
                    {
                        // Überprüfe, ob die eingegebene Zahl kleiner als 0 ist
                        if (inputValue < 0)
                        {
                            // Zeige eine Fehlermeldung an und beende die Methode, um zu verhindern, dass der Wert als Lagerbestand verwendet wird
                            MessageBox.Show("Gewünschte Menge ohne '-' eingeben.");
                            return;
                        }
                        // Weise den konvertierten Wert der Eigenschaft InputValue zu und setze das Dialogergebnis auf OK
                        InputValue = inputValue.ToString();
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        // Zeige eine Fehlermeldung an, wenn die Eingabe keine gültige Ganzzahl ist
                        MessageBox.Show("Ungültige Eingabe. Bitte eine ganze Zahl eingeben.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Behandele und zeige Ausnahmen an, die während der Ausführung auftreten können
                MessageBox.Show(ex.Message);
            }
        }

        // Event-Handler für das Klicken des Bestätigungsbuttons
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                // Versuche, den Text aus dem Eingabefeld in eine Ganzzahl zu konvertieren
                if (int.TryParse(TextBoxInput.Text, out int inputValue))
                {
                    // Überprüfe, ob die eingegebene Zahl kleiner als 0 ist
                    if (inputValue < 0)
                    {
                        // Zeige eine Fehlermeldung an und beende die Methode, um zu verhindern, dass der Wert als Lagerbestand verwendet wird
                        MessageBox.Show("Gewünschte Menge ohne '-' eingeben.");
                        return;
                    }

                    // Weise den konvertierten Wert der Eigenschaft InputValue zu und setze das Dialogergebnis auf OK
                    InputValue = inputValue.ToString();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    // Zeige eine Fehlermeldung an, wenn die Eingabe keine gültige Ganzzahl ist
                    MessageBox.Show("Ungültige Eingabe. Bitte eine ganze Zahl eingeben.");
                }
            }
            catch (Exception ex)
            {
                // Behandele und zeige Ausnahmen an, die während der Ausführung auftreten können
                MessageBox.Show(ex.Message);
            }
        }
    }
}


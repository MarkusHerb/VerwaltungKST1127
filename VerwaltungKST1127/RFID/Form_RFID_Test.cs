// ===================================================================================================
// "using"-Anweisungen importieren fertige Funktionen aus den .NET-Bibliotheken bzw. NuGet-Paketen.
// Damit können wir z. B. statt "System.Windows.Forms.Form" einfach "Form" schreiben.
// ===================================================================================================
using System;                      // Basis-Typen (string, int, EventArgs, Exception ...)
using System.Text;                 // StringBuilder (effizientes Zusammenbauen von Strings)
using System.Windows.Forms;        // Windows-Forms (Form, Button, MessageBox, ...)
using PCSC;                        // PC/SC-Bibliothek (NuGet-Paket): Zugriff auf Smartcard-/RFID-Reader
using System.ServiceProcess;       // Windows-Dienste steuern (z. B. SCardSvr) – aktuell nicht aktiv genutzt

// "namespace" gruppiert Klassen logisch (wie ein Ordner) und vermeidet Namenskonflikte.
namespace VerwaltungKST1127.RFID
{
    // public partial class ... : Form
    //   public  = von außen sichtbar
    //   partial = der Klassen-Code darf auf mehrere Dateien aufgeteilt sein (z. B. ...Designer.cs)
    //   : Form  = erbt von "Form" → diese Klasse IST ein Windows-Fenster
    public partial class Form_RFID_Test : Form
    {
        // -----------------------------------------------------------------------------------------------------------------
        // Felder ("Variablen der Klasse"). private = nur in dieser Klasse sichtbar.
        // -----------------------------------------------------------------------------------------------------------------

        // SCardContext = Verbindung zum Smartcard-Subsystem von Windows.
        // Über den "Kontext" können wir verfügbare Reader auflisten und ansprechen.
        private SCardContext _context;

        // SCardReader = der konkrete Kartenleser, mit dem wir später kommunizieren.
        private SCardReader _reader;

        // Name des Readers (z. B. "ACS ACR122U PICC Interface 0").
        private string _readerName;

        // -----------------------------------------------------------------------------------------------------------------
        // Konstruktor: läuft beim "new Form_RFID_Test()" automatisch.
        // -----------------------------------------------------------------------------------------------------------------
        public Form_RFID_Test()
        {
            InitializeComponent();         // erzeugt alle UI-Steuerelemente (definiert in der Designer-Datei)

            // Sicherstellen, dass der Smartcard-Dienst (SCardSvr) erreichbar ist.
            // Wir öffnen einen Test-Kontext und schließen ihn sofort wieder.
            // Wenn der Dienst nicht läuft, wirft Establish(...) eine Exception.
            try
            {
                var sc = ContextFactory.Instance.Establish(SCardScope.System);
                sc.Dispose(); // sofort wieder freigeben (war nur ein Funktionstest)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Starten des Smartcarddienstes: " + ex.Message);
            }
        }

        // =========================================================================================
        // FORM LOAD: wird aufgerufen, sobald das Fenster geladen ist
        // → Reader suchen und das Reader-Objekt erstellen.
        // =========================================================================================
        private void Form_RFID_Test_Load(object sender, EventArgs e)
        {
            try
            {
                // (SCardContext) Cast: ContextFactory.Establish liefert ein Interface zurück,
                // wir wollen aber die konkrete Klasse, weil sie weitere Funktionen bietet.
                _context = (SCardContext)ContextFactory.Instance.Establish(SCardScope.System);

                // Liste aller angeschlossenen Reader holen.
                var readers = _context.GetReaders();
                if (readers == null || readers.Length == 0)
                {
                    MessageBox.Show("Kein RFID Reader gefunden");
                    return;
                }

                // Wir nehmen einfach den ersten Reader aus der Liste.
                _readerName = readers[0];

                // Reader-Objekt mit unserem Kontext verbinden (noch nicht kommunizieren).
                _reader = new SCardReader(_context);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =========================================================================================
        // BUTTON CLICK: liest die ersten 12 Blöcke vom Tag und zeigt sie als ASCII-Text an
        // =========================================================================================
        private void btnTestRfid_Click(object sender, EventArgs e)
        {
            try
            {
                // Verbindung zum aktuell aufliegenden Tag aufbauen.
                //   Shared        → andere Anwendungen können den Reader auch noch nutzen
                //   SCardProtocol.Any → Reader entscheidet selbst (T0 oder T1)
                _reader.Connect(
                    _readerName,
                    SCardShareMode.Shared,
                    SCardProtocol.Any
                );

                // StringBuilder ist effizienter als ständiges "string += ..." in einer Schleife.
                StringBuilder result = new StringBuilder();

                // Blöcke 0..11 nacheinander auslesen.
                for (byte block = 0; block < 12; block++)
                {
                    byte[] data = ReadSingleBlock(block);

                    if (data != null)
                    {
                        // Jedes empfangene Byte in ein druckbares ASCII-Zeichen umwandeln,
                        // alle nicht-druckbaren Bytes durch '.' ersetzen.
                        foreach (byte b in data)
                        {
                            // ASCII-Bereich 32 (Leerzeichen) bis 126 (~) ist sicher anzeigbar.
                            result.Append(b >= 32 && b <= 126 ? (char)b : '.');
                        }
                        result.Append("_"); // Trennzeichen zwischen den Blöcken
                    }
                }

                // Das letzte "_" wieder abschneiden (ist überflüssig).
                if (result.Length > 0)
                    result.Length--;

                // Ergebnis in das Label schreiben.
                lblTestRfid.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // finally läuft IMMER (auch im Fehlerfall):
                // Karte/Reader korrekt wieder freigeben, damit andere Anwendungen drauf zugreifen können.
                // SCardReaderDisposition.Leave = Karte unverändert lassen (kein Reset, kein Eject).
                if (_reader != null && _reader.IsConnected)
                    _reader.Disconnect(SCardReaderDisposition.Leave);
            }
        }

        // =========================================================================================
        // READ SINGLE BLOCK (ISO15693)
        // Sendet einen Lesebefehl an den Tag, der genau einen Block liefert.
        // Rückgabewert: 4 Bytes Nutzdaten oder null bei Fehler.
        // =========================================================================================
        private byte[] ReadSingleBlock(byte blockNumber)
        {
            // APDU-Befehl ("Application Protocol Data Unit") laut Reader-Doku:
            //   0xFF, 0x00, 0x00, 0x00  → "transparent exchange" (direkt zum Tag senden)
            //   0x03                    → Anzahl Bytes Nutzlast (Lc)
            //   0x02                    → ISO15693-Flags (High Data Rate)
            //   0x20                    → Befehl "Read Single Block"
            //   blockNumber             → welcher Block ausgelesen werden soll
            byte[] command = new byte[]
            {
                0xFF,       // CLA  (Klassenbyte)
                0x00,       // INS  (Befehl: transparent exchange)
                0x00,       // P1   (Parameter 1)
                0x00,       // P2   (Parameter 2)
                0x03,       // Lc   (Länge der Daten dahinter = 3 Bytes)
                0x02,       // ISO15693 Flags
                0x20,       // ISO15693 Befehl: Read Single Block
                blockNumber // Blocknummer
            };

            // Empfangspuffer + dessen Größe als "ref"-Parameter.
            // Nach dem Aufruf enthält receiveLength die tatsächlich empfangene Anzahl Bytes.
            byte[] receiveBuffer = new byte[64];
            int receiveLength = receiveBuffer.Length;

            // Den Befehl an den Reader/Tag schicken.
            // SCardPCI.GetPci(...) liefert die korrekte Protokoll-Information (T0/T1) für den Transport.
            var sc = _reader.Transmit(
                SCardPCI.GetPci(_reader.ActiveProtocol),
                command,
                command.Length,
                null,
                receiveBuffer,
                ref receiveLength
            );

            // Wenn die Übertragung selbst nicht erfolgreich war → null zurück.
            if (sc != SCardError.Success)
                return null;

            // ISO15693-Antwort:
            //   Byte 0 = Status (0x00 = OK, sonst Fehlercode)
            //   Byte 1..4 = die 4 Nutz-Datenbytes des Blocks
            //   (am Ende kommen außerdem 2 Status-Bytes SW1/SW2 vom Reader)
            if (receiveLength >= 5 && receiveBuffer[0] == 0x00)
            {
                return new byte[]
                {
                    receiveBuffer[1],
                    receiveBuffer[2],
                    receiveBuffer[3],
                    receiveBuffer[4]
                };
            }

            // Status nicht OK oder zu wenig Daten → null.
            return null;
        }
    }
}
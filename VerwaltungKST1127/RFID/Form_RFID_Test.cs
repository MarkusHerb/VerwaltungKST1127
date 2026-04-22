using System;
using System.Text;
using System.Windows.Forms;
using PCSC;

namespace VerwaltungKST1127.RFID
{
    public partial class Form_RFID_Test : Form
    {
        private SCardContext _context;
        private SCardReader _reader;
        private string _readerName;

        public Form_RFID_Test()
        {
            InitializeComponent();
        }

        // =========================
        // FORM LOAD
        // =========================
        private void Form_RFID_Test_Load(object sender, EventArgs e)
        {
            try
            {
                _context = (SCardContext)ContextFactory.Instance.Establish(SCardScope.System);

                var readers = _context.GetReaders();
                if (readers == null || readers.Length == 0)
                {
                    MessageBox.Show("Kein RFID Reader gefunden");
                    return;
                }

                _readerName = readers[0];
                _reader = new SCardReader(_context);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =========================
        // BUTTON CLICK
        // =========================
        private void btnTestRfid_Click(object sender, EventArgs e)
        {
            try
            {
                _reader.Connect(
                    _readerName,
                    SCardShareMode.Shared,
                    SCardProtocol.Any
                );

                // wir lesen die ersten 12 Blöcke (48 Byte)
                StringBuilder result = new StringBuilder();

                for (byte block = 0; block < 12; block++)
                {
                    byte[] data = ReadSingleBlock(block);

                    if (data != null)
                    {
                        foreach (byte b in data)
                        {
                            // ASCII-Ausgabe, nicht druckbare → .
                            result.Append(b >= 32 && b <= 126 ? (char)b : '.');
                        }
                        result.Append("_");
                    }
                }

                // letztes "_" entfernen
                if (result.Length > 0)
                    result.Length--;

                lblTestRfid.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_reader != null && _reader.IsConnected)
                    _reader.Disconnect(SCardReaderDisposition.Leave);
            }
        }

        // =========================
        // READ SINGLE BLOCK (ISO15693)
        // =========================
        private byte[] ReadSingleBlock(byte blockNumber)
        {
            byte[] command = new byte[]
            {
                0xFF,       // CLA
                0x00,       // INS (transparent exchange)
                0x00,       // P1
                0x00,       // P2
                0x03,       // Lc
                0x02,       // Flags (High Data Rate)
                0x20,       // ISO15693 Read Single Block
                blockNumber // Blocknummer
            };

            byte[] receiveBuffer = new byte[64];
            int receiveLength = receiveBuffer.Length;

            var sc = _reader.Transmit(
                SCardPCI.GetPci(_reader.ActiveProtocol),
                command,
                command.Length,
                null,
                receiveBuffer,
                ref receiveLength
            );

            if (sc != SCardError.Success)
                return null;

            // ISO15693: Byte 0 = Status (0x00 = OK)
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

            return null;
        }
    }
}
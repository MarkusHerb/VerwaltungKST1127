using System; // Importieren des System-Namespace für grundlegende .NET-Klassen und -Typen (z.B. grundlegende Datentypen wie String, Integer, Exception-Handling)
using System.Data; // Importieren des System.Data-Namespace für den Zugriff auf Datenbankfunktionalitäten (z.B. DataTable, DataSet und andere ADO.NET-Funktionen)
using System.Data.SqlClient; // Importieren des System.Data.SqlClient-Namespace für die Arbeit mit SQL Server-Datenbanken (z.B. für die Verwaltung von SQL-Verbindungen, -Befehlen und -Abfragen)
using System.Drawing; // Importieren des System.Drawing-Namespace für Grafiken und Bildverarbeitung (z.B. Arbeiten mit Farben, Schriften, und Bildern in der GUI)
using System.Linq; // Importieren des System.Linq-Namespace für LINQ-Abfragen (z.B. für die Abfrage von Datenquellen wie Arrays, Listen und Datenbanken in einer deklarativen Syntax)
using System.Windows.Forms; // Importieren des System.Windows.Forms-Namespace für die Erstellung von Benutzeroberflächen (GUI) mit Windows Forms-Steuerelementen (z.B. Button, TextBox, Form)

namespace VerwaltungKST1127.EingabeSerienartikelPrototyp
{
    public partial class Form_Chargenbegleitblatt : Form
    {
        public string Projektnummer { get; set; }
        public string Bezeichnung { get; set; }
        public string Artikelnummer { get; set; }
        public string Belag { get; set; }
        public string Prozess { get; set; }
        public string RadiusVerguetung { get; set; }
        public string RadiusRueckseite { get; set; }
        public string GNummer { get; set; }
        public string Glassorte { get; set; }
        public string Durchmesser { get; set; }
        public string Mittendicke { get; set; }
        public string Bemerkung { get; set; }
        public string ErstelltAm { get; set; }
        public string PfadBild { get; set; }

        public Form_Chargenbegleitblatt()
        {
            InitializeComponent();
        }
        public void FillFormWithData()
        {

        }
    }
}

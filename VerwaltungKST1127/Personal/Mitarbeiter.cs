using System; // Grundlegender Namespace, der grundlegende Datentypen und Funktionen (wie Exception-Handling, Datums- und Zeitfunktionen) bereitstellt.

namespace VerwaltungKST1127.Personal
{
    internal class Mitarbeiter
    {
        public int Id { get; set; }
        public int Personalnummer { get; set; }
        public string Vorname { get; set; }
        public string Nachname {  get; set; }
        public string Position { get; set; }
        public DateTime Eintritt { get; set; }
        public DateTime Geburtstag { get; set; }
        public int Alter { get; set; }
        public int JahreUnternehmen { get; set; }
        public string Telefonnummer { get; set; }
        public int Team {  get; set; }  
        public decimal Wochenstunden { get; set; }
        public string Lohngruppe { get; set; }
        public string DirekterVorgesetzter { get; set; }        
    }
}

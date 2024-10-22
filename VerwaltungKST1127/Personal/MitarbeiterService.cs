using System.Collections.Generic; // Namespace für generische Datentypen wie List, Dictionary etc.
using System.IO;                  // Namespace für Dateizugriffsfunktionen wie File, FileStream etc.
using System.Text.Json;           // Namespace für JSON-Funktionalitäten, wie Serialisierung und Deserialisierung von Objekten

namespace VerwaltungKST1127.Personal
{
    internal class MitarbeiterService
    {
        private readonly string _filePath;

        // Konstruktor, der den Dateipfad zum Speichern und Laden der Mitarbeiter setzt
        public MitarbeiterService(string filePath)
        {
            _filePath = filePath;
        }

        // Speichert die Liste von Mitarbeitern in einer JSON-Datei
        public void SaveMitarbeiter(List<Mitarbeiter> mitarbeiterListe)
        {
            // Serialisiert die Mitarbeiterliste in einen JSON-String
            var jsonString = JsonSerializer.Serialize(mitarbeiterListe, new JsonSerializerOptions { WriteIndented = true });

            // Schreibt den JSON-String in die Datei
            File.WriteAllText(_filePath, jsonString);
        }

        // Lädt die Liste von Mitarbeitern aus einer JSON-Datei
        public List<Mitarbeiter> LoadMitarbeiter()
        {
            // Überprüfen, ob die Datei existiert, andernfalls eine leere Liste zurückgeben
            if (!File.Exists(_filePath))
                return new List<Mitarbeiter>();

            // Liest den Inhalt der JSON-Datei
            var jsonString = File.ReadAllText(_filePath);

            // Deserialisiert den JSON-String zurück in eine Liste von Mitarbeitern
            return JsonSerializer.Deserialize<List<Mitarbeiter>>(jsonString);
        }
    }
}

using System;
using System.IO;

namespace Notenverwaltung
{
    /// Name:               Watcher
    /// Version:            1.X
    /// Erstelldatum:       Februar 2015
    /// Verantwortliche(r): Martin Salfer
    /// Umgebung:           Win 7 Prof SP 1; C#.NET 2012 (Framework 4.5)
    /// Beschreibung:       Klasse für einen Watcher, der Verzeichnisse und Dateien überwacht
    /// ---------------------------------------------------------------------------
    ///    Änderungen:         Datum       Name        Änderung
    ///                        März 2015   Tobias      Konstruktoren zusammengefasst; funktioniert nun auch bei mir.
    /// </summary>
    public class Watcher : FileSystemWatcher
    {
        /// <summary>
        /// Standardkonstruktor zur Initialisierung der richtigen Einstellungen 
        /// </summary>
        /// <param name="path">Dateipfad zur Festlegung des Überwachungsbereichs</param>
        public Watcher(string path)
            : base(path)
        {
            this.IncludeSubdirectories = true; // Untersuchung der Unterverzeichnisse
            this.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName; // Überwachungsaktionen definieren
            this.Renamed += Watcher_Renamed;
            this.Changed += Watcher_Changed;
            this.Created += Watcher_Changed;
            this.Deleted += Watcher_Changed;
            this.EnableRaisingEvents = true;
        }


        #region Öffentliche Funktionen





        #endregion

        #region Event Handler

        /// <summary>
        /// Event-Handler beim Umbenennen von Dateien und Verzeichnissen
        /// </summary>
        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("{0} umbenannt in {1}", e.OldFullPath, e.FullPath);
        }

        /// <summary>
        /// Event-Handler beim Ändern von Dateien und Verzeichnissen
        /// </summary>
        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Datei: " + e.FullPath + " | ChangeType: " + e.ChangeType);
        }

        #endregion


        //todo: Einfügen von (ein oder mehreren) PDF Dateien (-> File Watcher, Naming Patterns)
        //todo: was soll passieren, wenn der komplette Pfad gelöscht wird (das wird mit diesem Watcher nicht überprüft!)
    }
}

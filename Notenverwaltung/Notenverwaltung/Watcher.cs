﻿using System;
using System.IO;

namespace Notenverwaltung
{
    /// <summary>
    /// Watcher, der Verzichnisse und Dateien überwacht
    /// </summary>
    public partial class Watcher : FileSystemWatcher
    {
        #region "Konstruktor"
        /// <summary>
        /// Standardkonstruktor zur Initialisierung der richtigen Einstellungen 
        /// </summary>
        public Watcher()
        {
            this.IncludeSubdirectories = true; // Untersuchung der Unterverzeichnisse
            this.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName; // Überwachungsaktionen definieren
            this.Renamed += Watcher_Renamed;
            this.Changed += Watcher_Changed;
            this.Created += Watcher_Changed;
            this.Deleted += Watcher_Changed;
        }

        /// <summary>
        /// Überladung des Konstruktors mit Pfadangabe
        /// </summary>
        /// <param name="Pfad">Dateipfad zur Festlegung des Überwachungsbereichs</param>
        public Watcher(string path): base()
        {
           this.Path = path;
        }
        #endregion

        #region "öffentliche Funktionen"




        #endregion

        #region "Event Handler"
        /// <summary>
        /// Event-Handler beim Umbenennen von Dateien und Verzeichnissen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("{0} umbenannt in {1}", e.OldFullPath, e.FullPath);
        }

        /// <summary>
        /// Event-Handler beim Ändern von Dateien und Verzeichnissen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Datei: " + e.FullPath + " | ChangeType: " + e.ChangeType);
        }

        #endregion
        //todo: Einfügen von (ein oder mehreren) PDF Dateien (-> File Watcher, Naming Patterns)
        //todo: was soll passieren, wenn der komplette Pfad gelöscht wird (das wird mit diesem Watcher nicht überprüft!)
    }
}
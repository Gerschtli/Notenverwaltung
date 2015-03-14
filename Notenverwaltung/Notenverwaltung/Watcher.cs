using System;
using System.IO;

namespace Notenverwaltung
{
 /// <summary>
 /// abgeleitete FilesystemWachter-Klasse
 /// </summary>
    public class Watcher : FileSystemWatcher
    {

        public WorkList wl = WorkList.Load();

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
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("{0} umbenannt in {1}", e.OldFullPath, e.FullPath);
            wl.RenameDirOrFile(e.OldFullPath, e.FullPath);
        }

        /// <summary>
        /// Event-Handler beim Ändern von Dateien und Verzeichnissen
        /// </summary>
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Datei: " + e.FullPath + " | ChangeType: " + e.ChangeType);
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    wl.NewDirOrFile(e.FullPath);
                    break;
                case WatcherChangeTypes.Deleted:
                    wl.DelDirOrFile(e.FullPath);
                    break;
                //case WatcherChangeTypes.Changed // hier passiert überhaupt nichts
            }
        }

        #endregion

    }
}

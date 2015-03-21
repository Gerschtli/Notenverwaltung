using System;
using System.IO;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// abgeleitete FilesystemWachter-Klasse
    /// </summary>
    public class Watcher : FileSystemWatcher
    {
        /// <summary>
        /// Standardkonstruktor zur Initialisierung der richtigen Einstellungen 
        /// </summary>
        public Watcher()
            : base(Config.StoragePath)
        {
            this.IncludeSubdirectories = true; // Untersuchung der Unterverzeichnisse
            this.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName; // Überwachungsaktionen definieren
            this.Renamed += Watcher_Renamed;
            this.Changed += Watcher_Changed;
            this.Created += Watcher_Changed;
            this.Deleted += Watcher_Changed;
            this.EnableRaisingEvents = true;
        }

        #region Event Handler

        /// <summary>
        /// Event-Handler beim Umbenennen von Dateien und Verzeichnissen
        /// </summary>
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string oldName = e.OldFullPath.Replace(Path, "").TrimStart('\\');
            Console.WriteLine("{0} umbenannt in {1}", oldName, e.Name);

            bool dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
            WorkList.RenameDirOrFile(oldName, e.Name, dir);
        }

        /// <summary>
        /// Event-Handler beim Ändern von Dateien und Verzeichnissen
        /// </summary>
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Datei: " + e.Name + " | ChangeType: " + e.ChangeType);
            bool dir;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
                    WorkList.NewDirOrFile(e.Name, dir);
                    break;
                case WatcherChangeTypes.Deleted:
                    WorkList.DelDirOrFile(e.Name);
                    break;
                case WatcherChangeTypes.Changed:
                    dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
                    WorkList.ChangeDirOrFile(e.FullPath, e.Name, dir);
                    break;
            }
        }

        #endregion

    }
}

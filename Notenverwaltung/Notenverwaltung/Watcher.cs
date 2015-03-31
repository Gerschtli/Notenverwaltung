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
        public WorkList WorkList { get; set; }

        /// <summary>
        /// Standardkonstruktor zur Initialisierung der richtigen Einstellungen 
        /// </summary>
        public Watcher(WorkList workList)
            : base(Config.StoragePath)
        {
            this.IncludeSubdirectories = true; // Untersuchung der Unterverzeichnisse
            this.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName; // Überwachungsaktionen definieren
            this.Renamed += Watcher_Renamed;
            this.Changed += Watcher_Changed;
            this.Created += Watcher_Changed;
            this.Deleted += Watcher_Changed;
            this.EnableRaisingEvents = true;
            this.WorkList = workList;
        }

        #region Event Handler

        /// <summary>
        /// Event-Handler beim Umbenennen von Dateien und Verzeichnissen
        /// </summary>
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string oldName = e.OldFullPath.Substring(Path.Length).TrimStart('\\');
            bool dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
            WorkList.RenameDirOrFile(oldName, e.Name, dir);
        }

        /// <summary>
        /// Event-Handler beim Ändern von Dateien und Verzeichnissen
        /// </summary>
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name.EndsWith(".xml")) // XML-Dateien sollen nicht überwacht werden
                return;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    bool dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
                    WorkList.NewDirOrFile(e.Name, dir);
                    break;
                case WatcherChangeTypes.Changed:
                    if (File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory))
                        WorkList.ChangeDir(e.Name);
                    break;
                case WatcherChangeTypes.Deleted:
                    WorkList.DelDirOrFile(e.Name);
                    break;
            }
        }

        #endregion

    }
}

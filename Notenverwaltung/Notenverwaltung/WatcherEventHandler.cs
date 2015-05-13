using System;
using System.IO;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt die Eventhandler für Watcher Events zur Verfügung.
    /// </summary>
    public class WatcherEventHandler
    {
        private static Config config;
        private static Config Config
        {
            get
            {
                if (config == null)
                    config = Notenverwaltung.Config.GetInstance();
                return config;
            }
        }

        private static WorkList workList;
        private static WorkList WorkList
        {
            get
            {
                if (workList == null)
                    workList = Notenverwaltung.WorkList.GetInstance();
                return workList;
            }
        }

        /// <summary>
        /// Aktionen bei Erstellung einer PDF-Datei oder eines Verzeichnisses.
        /// </summary>
        public static void Created(object sender, FileSystemEventArgs e)
        {
            bool dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
            string path = e.Name;

            if (dir)
            {
                new FileSystemChecker(path).CheckStructure();
                new NameNormalizer().CheckSystem(path);
            }
            else if (path.ToLower().EndsWith(".pdf"))
            {
                string filename = path.Split('\\').Last();
                string folder = path.Substring(0, path.Length - filename.Length).Trim('\\');

                new FileSystemChecker(folder).CheckStructure();

                if(File.Exists(Path.Combine(Config.StoragePath, String.Format(Config.MetaPath, folder))))
                    new NameNormalizer().NormalizeFile(path);
            }
        }

        /// <summary>
        /// Aktionen bei Umbenennung eines Verzeichnisses oder einer Datei.
        /// </summary>
        public static void Renamed(object sender, RenamedEventArgs e)
        {
            string oldPath = e.OldFullPath.Substring(Config.StoragePath.Length).Trim('\\');
            string newPath = e.Name;
            bool dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);

            if (dir)
            {
                WorkList.DeleteRefs(oldPath, true);

                if (File.Exists(Path.Combine(Config.StoragePath, String.Format(Config.MetaPath, newPath))))
                    newPath = new NameNormalizer().NormalizeFolder(newPath);

                WorkList.ReplaceRefs(oldPath, newPath);
                Folder.ReplacePathForAll(oldPath, newPath);
            }
            else
            {
                bool oldIsPdf = oldPath.ToLower().EndsWith(".pdf");
                bool newIsPdf = newPath.ToLower().EndsWith(".pdf");

                if (oldIsPdf)
                    WorkList.DeleteRefs(oldPath);

                if (newIsPdf)
                {
                    string filename = newPath.Split('\\').Last();
                    string folder = newPath.Substring(0, newPath.Length - filename.Length).Trim('\\');

                    if (folder == "")
                        return;

                    if (File.Exists(Path.Combine(Config.StoragePath, String.Format(Config.MetaPath, newPath))))
                        newPath = new NameNormalizer().NormalizeFolder(newPath);

                    if (!oldIsPdf)
                        new FileSystemChecker(folder);
                }
            }
        }

        /// <summary>
        /// Aktionen bei Änderung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        public static void Changed(object sender, FileSystemEventArgs e)
        {
            bool dir = File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory);
            string path = e.Name;

            if (!dir)
                return;

            if (File.Exists(Path.Combine(Config.StoragePath, String.Format(Config.MetaPath, e.Name))))
            {
                Song song = new Song(path);
                Instrument value;

                for (int i = song.MetaInfo.FallbackInstrumentation.Count - 1; i >= 0; i--)
                {
                    value = song.MetaInfo.FallbackInstrumentation.ElementAt(i).Value;

                    if (!song.ExInstrumentation.Instruments.Exists(inst => inst == value))
                    {
                        song.MetaInfo.FallbackInstrumentation.Remove(song.MetaInfo.FallbackInstrumentation.ElementAt(i).Key);
                    }
                }
                Save.Meta(song.MetaInfo);
            }
            else
            {
                // Entfernt die Aufgabe, da Ordner kein Liedordner mehr ist
                WorkList.LoTasks.Remove(new Task()
                {
                    Type = TaskType.FolderNamePattern,
                    Path = path
                });
                Folder.DeletePathForAll(path, true);
            }
        }

        /// <summary>
        /// Aktionen bei Löschung einer Datei oder eines Verzeichnisses.
        /// </summary>
        public static void Deleted(object sender, FileSystemEventArgs e)
        {
            string path = e.Name;

            WorkList.DeleteRefs(path);
            Folder.DeletePathForAll(path);
        }
    }
}

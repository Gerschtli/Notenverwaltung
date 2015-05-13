using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt Funktionalität zur Normalisierung des Dateisystems zur Verfügung.
    /// </summary>
    public class NameNormalizer
    {
        private Config config;
        private NamePattern namePattern;

        public NameNormalizer()
        {
            config = Config.GetInstance();
            namePattern = Factory.GetNamePattern();
        }

        /// <summary>
        /// Überprüft das Dateisystem und normalisiert alle Dateien und Ordner bzw. fügt Aufgaben der Worklist hinzu.
        /// </summary>
        /// <param name="startFolder">Verzeichnis, ab welchem normalisiert werden soll</param>
        public void CheckSystem(string startFolder = "")
        {
            List<string> loSongFolders = Song.LoadAll();
            string newName;

            foreach (string songFolder in loSongFolders)
            {
                if (songFolder.StartsWith(startFolder))
                {
                    Meta meta = Factory.GetMeta(songFolder);
                    if (meta.SongFolder != songFolder)
                    {
                        Folder.ReplacePathForAll(meta.SongFolder, songFolder, true);
                        meta.SongFolder = songFolder;
                        Save.Meta(meta);
                    }

                    newName = NormalizeFolder(songFolder);

                    foreach (string file in Directory.EnumerateFiles(Path.Combine(config.StoragePath, newName), "*.pdf"))
                    {
                        NormalizeFile(file.Substring(config.StoragePath.Length).Trim('\\'));
                    }
                }
            }

            List<Folder> loFolders = Factory.GetFolders();

            foreach (Folder folder in loFolders)
            {
                folder.CheckSongs();
            }

            Save.Folders(loFolders);
        }

        /// <summary>
        /// Normalisiert den Dateinamen und benennt die Datei ggf. um. (Bedingung: endet mit .pdf)
        /// </summary>
        public string NormalizeFile(string path)
        {
            string filename = path.Split('\\').Last();
            string newName = filename.Substring(0, filename.Length - 4);

            if (namePattern.IsNormalizedInstrument(newName))
                return path;

            newName = namePattern.NormalizeInstrument(newName);

            if (newName == "") // -> Pattern unbekannt
            {
                WorkList.GetInstance().LoTasks.Add(new Task()
                {
                    Type = TaskType.FileNamePattern,
                    Path = path
                });

                return path;
            }
            else
            {
                string newPath = path.Substring(0, path.Length - filename.Length) + newName + ".pdf";

                File.Move(
                    Path.Combine(config.StoragePath, path),
                    Path.Combine(config.StoragePath, newPath)
                ); // todo: Exceptionhandling fehlt: Was passiert, wenn neuer Pfad schon vorhanden?

                return newPath;
            }
        }

        /// <summary>
        /// Normalisiert den Ordnernamen und benennt den Ordner ggf. um.
        /// </summary>
        public string NormalizeFolder(string path)
        {
            string folderName = path.Split('\\').Last();
            string newName = folderName;

            if (namePattern.IsNormalizedSong(folderName))
                return path;

            newName = namePattern.NormalizeSong(folderName);

            if (newName == "") // -> Pattern unbekannt
            {
                WorkList.GetInstance().LoTasks.Add(new Task()
                {
                    Type = TaskType.FolderNamePattern,
                    Path = path
                });

                return path;
            }
            else
            {
                string newPath = path.Substring(0, path.Length - folderName.Length) + newName;

                Directory.Move(
                    Path.Combine(config.StoragePath, path),
                    Path.Combine(config.StoragePath, newPath)
                ); // todo: Exceptionhandling fehlt: Was passiert, wenn neuer Pfad schon vorhanden?

                return newPath;
            }
        }
    }
}

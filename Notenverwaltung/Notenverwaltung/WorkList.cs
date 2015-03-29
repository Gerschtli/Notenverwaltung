using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;

namespace Notenverwaltung
{
    /// <summary>
    /// Verwaltet die Liste von Aufgaben, die durch den Watcher erkannt werden und abgearbeitet werden müssen.
    /// </summary>
    public static class WorkList
    {
        // Diese Liste sollte nicht von außerhalb der Klasse verändert werden.
        public static List<Task> LoTasks = new List<Task>();

        #region Öffentliche Funktionen

        /// <summary>
        /// Wird zur Initialisierung der Aufgabenliste und vor der Instanzierung des Watchers aufgerufen.
        /// </summary>
        public static void Initialize()
        {
            CheckFileSystem();

            List<string> loSongFolders = Song.LoadAll();
            string newName;

            foreach (string songFolder in loSongFolders)
            {
                newName = NormalizeFolder(songFolder);

                foreach (string file in Directory.EnumerateFiles(Config.StoragePath + newName, "*.pdf"))
                {
                    NormalizeFile(file.Substring(Config.StoragePath.Length));
                }
            }

            List<Folder> loFolders = Folder.Load();

            foreach (Folder folder in loFolders)
            {
                folder.CheckSongs(); // todo: Problematisch, da eine Liedordnerumbenennung, während das Programm nicht geöffnet ist, dazu führt, dass dieser Eintrag gelöscht wird.
            }

            Folder.Save(loFolders);
        }

        /// <summary>
        /// Aktionen bei Erstellung einer PDF-Datei oder eines Verzeichnisses.
        /// </summary>
        /// <param name="path">relative Pfadangabe einer Datei oder eines Verzeichnisses</param>
        public static void NewDirOrFile(string path, bool dir)
        {
            if (dir)
            {
                CheckFileSystem(path);

                if (File.Exists(Config.StoragePath + path + @"\Meta.xml"))
                {
                    NormalizeFolder(path);

                    foreach (string file in Directory.EnumerateFiles(Config.StoragePath + path, "*.pdf"))
                    {
                        NormalizeFile(file.Substring(Config.StoragePath.Length));
                    }
                }
                else
                {
                    // Für den Fall, dass ein Ordner mit Inhalt hinzugefügt wird, muss dessen Inhalt manuell gescannt werden
                    foreach (string folder in Directory.EnumerateDirectories(Config.StoragePath + path, "*", SearchOption.AllDirectories))
                    {
                        if (File.Exists(folder + @"\Meta.xml"))
                        {
                            NormalizeFolder(folder.Substring(Config.StoragePath.Length));

                            foreach (string file in Directory.EnumerateFiles(folder, "*.pdf"))
                            {
                                NormalizeFile(file.Substring(Config.StoragePath.Length));
                            }

                            break;
                        }
                    }
                }
            }
            else if (path.ToLower().EndsWith(".pdf"))
            {
                string filename = path.Split('\\').Last();

                CheckFileSystem(path.Substring(0, path.Length - filename.Length - 1));

                NormalizeFile(path);
            }
        }

        /// <summary>
        /// Aktionen bei Umbenennung eines Verzeichnisses oder einer Datei.
        /// </summary>
        /// <param name="oldPath">alte relative Pfadangabe eines Verzeichnisses oder einer Datei</param>
        /// <param name="newPath">neue relative Pfadangabe eines Verzeichnisses oder einer Datei</param>
        public static void RenameDirOrFile(string oldPath, string newPath, bool dir)
        {
            if (dir)
            {
                DeleteRefsWorkList(oldPath, true);

                if (File.Exists(Config.StoragePath + newPath + @"\Meta.xml"))
                    newPath = NormalizeFolder(newPath);

                ReplaceRefsWorkList(oldPath, newPath);
                ReplaceRefsFolders(oldPath, newPath);
            }
            else
            {
                bool oldIsPdf = oldPath.ToLower().EndsWith(".pdf");
                bool newIsPdf = newPath.ToLower().EndsWith(".pdf");

                if (oldIsPdf)
                    DeleteRefsWorkList(oldPath);

                if (newIsPdf)
                {
                    newPath = NormalizeFile(newPath);

                    if (!oldIsPdf)
                    {
                        string filename = newPath.Split('\\').Last();

                        CheckFileSystem(newPath.Substring(0, newPath.Length - filename.Length - 1));
                    }
                }
            }
        }

        /// <summary>
        /// Aktionen bei Änderung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Verzeichnisses</param>
        public static void ChangeDir(string path)
        {
            if (File.Exists(path + @"\Meta.xml"))
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
                song.MetaInfo.Save();
            }
            else
            {
                // Entfernt die Aufgabe, da Ordner kein Liedordner mehr ist
                LoTasks.Remove(new Task()
                {
                    Type = TaskType.FolderNamePattern,
                    Path = path
                });
                DeleteRefsFolders(path, true);
            }
        }

        /// <summary>
        /// Aktionen bei Löschung einer Datei oder eines Verzeichnisses.
        /// </summary>
        /// <param name="path">Relative Pfadangabe einer Datei oder eines Verzeichnisses</param>
        public static void DelDirOrFile(string path)
        {
            DeleteRefsWorkList(path);
            DeleteRefsFolders(path);
        }

        /// <summary>
        /// Durchsucht alle Ordner nach PDF-Dateien und Meta.xml, damit in jedem Liedordner eine Meta.xml ist.
        /// Muss eine Meta.xml gelöscht werden, wird eine weitere Methode aufgerufen.
        /// </summary>
        /// <param name="folder">Zu prüfender Pfad.</param>
        public static void CheckFileSystem(string folder = "")
        {
            List<string> needMeta = NeedMetaList(folder);

            List<string> delMeta = DelMetaList(folder, needMeta);

            // Leere Meta.xml in potentiellen Liedordnern erstellen
            Meta meta = new Meta();
            needMeta.ForEach(path =>
            {
                meta.SongFolder = path;
                meta.Save();
            });

            // Meta.xml in Root löschen (ohne Benutzerinformation)
            if (delMeta.Remove(""))
                DeleteFile(Config.StoragePath + @"\Meta.xml");
            // Meta.xmls löschen (mit Benutzerinteraktion)
            delMeta.ForEach(path => ConfirmDelMeta(path));
        }

        #endregion

        #region Hilfsfunktionen

        /// <summary>
        /// Ersetzt in der Worklist das alte Element mit dem neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldItem">Altes zu ersetzendes Element</param>
        /// <param name="newItem">Neues Element</param>
        /// <returns>Gibt an, ob das alte Element vorhanden war</returns>
        private static bool ReplaceRefsWorkList(string oldPath, string newPath)
        {
            bool ret = false;

            for (int i = 0; i < LoTasks.Count; i++)
            {
                if (LoTasks[i].Path.StartsWith(oldPath + "\\"))
                {
                    LoTasks[i].Path = newPath + LoTasks[i].Path.Substring(oldPath.Length);
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Löscht alle Aufgaben, die sich auf den genannten Pfad oder darunter liegende Elemente beziehen.
        /// </summary>
        /// <param name="path">Gelöschter Pfad</param>
        /// <param name="onlyThis">Gibt an, ob nur genau dieser Eintrag gelöscht werden soll.</param>
        private static void DeleteRefsWorkList(string path, bool onlyThis = false)
        {
            for (int i = LoTasks.Count - 1; i >= 0; i--)
            {
                if (LoTasks[i].Path == path || (!onlyThis && LoTasks[i].Path.StartsWith(path + "\\")))
                    LoTasks.RemoveAt(i);
            }
        }

        /// <summary>
        /// Ersetzt in Mappen.xml den alten Pfad durch den neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldPath">Alter zu ersetzender Pfad</param>
        /// <param name="newPath">Neuer Pfad</param>
        private static void ReplaceRefsFolders(string oldPath, string newPath)
        {
            List<string> loSongFolders = Song.LoadAll();
            List<Folder> folders = Folder.Load();

            for (int i = 0; i < folders.Count; i++)
            {
                folders[i].ReplacePath(oldPath, newPath);
            }

            Folder.Save(folders);
        }

        /// <summary>
        /// Löscht alle Einträge in Mappe.xml, die sich auf den genannten Pfad oder darunter liegende Elemente beziehen.
        /// </summary>
        /// <param name="path">Gelöschter Pfad</param>
        /// <param name="onlyThis">Gibt an, ob nur genau dieser Eintrag gelöscht werden soll.</param>
        private static void DeleteRefsFolders(string path, bool onlyThis = false)
        {
            List<Folder> folders = Folder.Load();

            for (int i = folders.Count - 1; i >= 0; i--)
            {
                for (int j = folders[i].Order.Count - 1; j >= 0; j--)
                {
                    if (folders[i].Order.ElementAt(j).Value == path || (!onlyThis && folders[i].Order.ElementAt(j).Value.StartsWith(path + "\\")))
                        folders[i].Order.Remove(folders[i].Order.ElementAt(j).Key);
                }
            }

            Folder.Save(folders);
        }


        /// <summary>
        /// Zeigt eine MessageBox für den Benutzer bzgl. PDF-Dateien, die nicht mit der momentanen Struktur zu vereienen sind, und reagiert auf dessen Eingabe.
        /// </summary>
        /// <param name="folder">Relative Pfadangabe eines Verzeichnisses, welches eine Meta.xml enthält, die an dieser Stelle nicht erlaubt ist.</param>
        private static void ConfirmDelMeta(string folder)
        {
            /* Fenster für User:
             * Mögliche Problemursachen:
             * - Meta.xml(1) liegt über Meta.xml(2)
             *   -> User fragen, ob Ordner von (2) Liedordner ist, wenn ja (1) und deren PDFs löschen, wenn nein (2) löschen
             *      Die Namen aller zu löschenden PDFs anzeigen.
             * - PDFs liegen über Meta.xml (Fall ausgeschlossen, da automatisch eine Meta.xml erstellt wurde)
             * - Meta.xml hat keine PDFs auf gleicher Ebene
             *   -> User fragen, ob Ordner Liedordner ist, wenn ja nichts tun, wenn nein Meta.xml löschen
             */
            string metaPath;
            List<string> pdfs = InvalidPdfs(folder, out metaPath);


            string pdfText = "";

            pdfs.ForEach(name => pdfText += "\n      - " + name); // Liste formatieren für MessageBox


            string message = "Es ist ein Problem aufgetreten. Ist in folgendem Ordner ein Lied gespeichert?\n\n      " + folder; // 2.Fall

            if (pdfs.Count != 0) // 1.Fall
            {
                message += "\n\nWenn dies der Fall ist, müssen folgende PDF-Dateien entweder gelöscht werden oder aus dem Dateisystem entfernt werden:\n"
                    + pdfText + "\n\nWenn Sie auf \"Ja\" drücken, werden die genannten Dateien gelöscht, sofern sie noch vorhanden sind.";
            }

            MessageBoxResult result = MessageBox.Show(message, "Problem im Dateisystem", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result)
            {
                case MessageBoxResult.No: // Angegebenes Verzeichnis ist kein Liedordner
                    DeleteFile(Config.StoragePath + folder + @"\Meta.xml");
                    break;
                case MessageBoxResult.Yes: // Angegebenes Verzeichnis ist Liedordner
                    DeleteFile(Config.StoragePath + metaPath);
                    pdfs.ForEach(name => DeleteFile(Config.StoragePath + name));
                    break;
            }
        }

        /// <summary>
        /// Generiert eine Liste von PDFs relativ zum Speicherpfad der Notenverwaltung, die dort nicht liegen dürfen, wenn "folder" ein Liedordner ist.
        /// </summary>
        /// <param name="folder">Potentieller Liedordner</param>
        /// <param name="metaPath">Pfad zur ersten Meta.xml Datei</param>
        private static List<string> InvalidPdfs(string folder, out string metaPath)
        {
            string[] split = folder.Split('\\');
            string path;

            metaPath = null;
            List<string> pdfs = new List<string>();

            for (int i = 1; i < split.Length; i++) // Liste der Dateien erstellen, welche gelöscht werden müssten
            {
                path = "";
                for (int j = 0; j < i; j++)
                    path += split[j] + "\\";


                if (metaPath == null && File.Exists(Config.StoragePath + path + "Meta.xml"))
                    metaPath = path + "Meta.xml";

                if (metaPath != null)
                {
                    foreach (string item in Directory.EnumerateFiles(Config.StoragePath + path, "*.pdf"))
                        pdfs.Add(item.Substring(Config.StoragePath.Length));
                }
            }

            return pdfs;
        }

        /// <summary>
        /// Generiert eine Liste von Ordnernamen relativ zum Speicherpfad der Notenverwaltung, welche PDFs enthalten und nach der Definition eines Liedordners eine Meta.xml brauchen.
        /// </summary>
        /// <param name="folder">Zu prüfender Pfad.</param>
        private static List<string> NeedMetaList(string folder)
        {
            var allPdfs = Directory.EnumerateFiles(Config.StoragePath + folder, "*.pdf", SearchOption.AllDirectories);
            Console.WriteLine("AllDirs: " + folder);
            if (folder != "")
            {
                string[] split = folder.Split('\\');
                string path;

                for (int i = split.Length - 1; i >= 1; i--) // i >= 1, da Rootverzeichnis nicht gescannt werden muss
                {
                    path = "";
                    for (int j = 0; j < i; j++)
                        path += split[j] + "\\";

                    Console.WriteLine("Pfad: " + path);

                    allPdfs = allPdfs.Concat(Directory.EnumerateFiles(Config.StoragePath + path, "*.pdf"));
                }
            }

            allPdfs = from file in allPdfs
                      where file.Split('\\').Length > Config.StoragePath.Split('\\').Length // sortiert Dateien im Rootverzeichnis aus
                      orderby file.Split('\\').Length descending
                      select file;

            List<string> needMeta = new List<string>();
            string songFolder;

            foreach (string path in allPdfs) // Pfade, die potentielle Liedordner sind, in needMeta speichern
            {
                songFolder = path.Substring(Config.StoragePath.Length, path.Length - Config.StoragePath.Length - path.Split('\\').Last().Length).Trim('\\');

                needMeta.RemoveAll(name => name.StartsWith(songFolder + '\\'));
                if (!needMeta.Contains(songFolder))
                    needMeta.Add(songFolder);
            }
            return needMeta;
        }

        /// <summary>
        /// Generiert eine Liste von Ordnernamen relativ zum Speicherpfad der Notenverwaltung, welche eine Meta.xml enthalten, jedoch aufgrund der Dateistruktur keine Liedordner sein können.
        /// </summary>
        /// <param name="folder">Zu prüfender Pfad.</param>
        /// <param name="needMeta">Liste von Ordnernamen; wird manipuliert, um vorhandene Liedordner aus Liste zu löschen.</param>
        private static List<string> DelMetaList(string folder, List<string> needMeta)
        {
            var allMetas = Directory.EnumerateFiles(Config.StoragePath + folder, "Meta.xml", SearchOption.AllDirectories).ToList();

            if (folder != "")
            {
                string[] split = folder.Split('\\');
                string path;

                for (int i = split.Length - 1; i >= 0; i--)
                {
                    path = "";
                    for (int j = 0; j < i; j++)
                        path += split[j] + "\\";

                    if (File.Exists(Config.StoragePath + path + @"\Meta.xml"))
                        allMetas.Add(Config.StoragePath + path + @"\Meta.xml");
                }
            }

            List<string> delMeta = new List<string>();
            string songFolder;

            foreach (string path in allMetas)
            {
                songFolder = path.Substring(Config.StoragePath.Length, path.Length - Config.StoragePath.Length - path.Split('\\').Last().Length).Trim('\\');

                if (needMeta.RemoveAll(name => name == songFolder) == 0) // Vorhandene Meta.xml aus needMeta löschen
                    delMeta.Add(songFolder);
            }

            return delMeta;
        }


        /// <summary>
        /// Normalisiert den Dateinamen und benennt die Datei ggf. um.
        /// </summary>
        private static string NormalizeFile(string path)
        {
            string filename = path.Split('\\').Last();
            string name = filename.Substring(0, filename.Length - 4);

            if (!NamePattern.IsNormalizedInstrument(name))
            {
                name = NamePattern.NormalizeInstrument(name);
                if (name == null) // null -> Pattern unbekannt
                {
                    LoTasks.Add(new Task()
                    {
                        Type = TaskType.FileNamePattern,
                        Path = path
                    });
                }
                else
                {
                    File.Move(Config.StoragePath + path, Config.StoragePath + path.Substring(0, path.Length - filename.Length) + name + ".pdf"); // todo: Exceptionhandling fehlt: Was passiert, wenn neuer Pfad schon vorhanden?

                    return path.Substring(0, path.Length - filename.Length) + name + ".pdf";
                }
            }

            return path;
        }

        /// <summary>
        /// Normalisiert den Ordnernamen und benennt den Ordner ggf. um.
        /// </summary>
        private static string NormalizeFolder(string path)
        {
            string folderName = path.Split('\\').Last();
            string newName = folderName;

            if (!NamePattern.IsNormalizedSong(folderName))
            {
                newName = NamePattern.NormalizeSong(folderName);
                if (newName == null) // null -> Pattern unbekannt
                {
                    LoTasks.Add(new Task()
                    {
                        Type = TaskType.FolderNamePattern,
                        Path = path
                    });
                }
                else
                {
                    Directory.Move(Config.StoragePath + path, Config.StoragePath + path.Substring(0, path.Length - folderName.Length) + newName); // todo: Exceptionhandling fehlt: Was passiert, wenn neuer Pfad schon vorhanden?
                    
                    return path.Substring(0, path.Length - folderName.Length) + newName;
                }
            }

            return path;
        }

        /// <summary>
        /// Löscht eine Datei, ohne dass eine Exception geworfen wird.
        /// </summary>
        /// <param name="path">Pfad der zu löschenden Datei</param>
        private static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                return;
            }
        }

        #endregion
    }
}

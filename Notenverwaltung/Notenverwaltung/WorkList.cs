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
    public class WorkList : INotifyPropertyChanged
    {
        private static ObservableCollection<Task> _LoTasks;
        public static ObservableCollection<Task> LoTasks
        {
            get
            {
                if (_LoTasks == null)
                    Initialize();

                return _LoTasks;
            }
        }

        #region Öffentliche Funktionen

        /// <summary>
        /// Aktionen bei Erstellung eines neuen Verzeichnisses oder eines neuen Dokuments.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public static void NewDirOrFile(string path, bool dir)
        {
            if (dir)
            {
                // remark: Wird für jeden Ordner ausgeführt, auch für Strukturordner und Ordner für zusätzliche Dateien; Überprüfung bei Abarbeitung?
                LoTasks.Add(new Task()
                {
                    Type = TaskType.FolderNamePattern,
                    Path = path
                });

                // Wenn ein Ordner mit Dateien eingefügt wird, müssen diese auch in die Worklist eingetragen werden
                var pdfs = Directory.EnumerateFiles(Config.StoragePath + path, "*.pdf", SearchOption.AllDirectories);
                var dirs = Directory.EnumerateDirectories(Config.StoragePath + path, "*", SearchOption.AllDirectories);

                foreach (string pdfPath in pdfs)
                    NewDirOrFile(pdfPath.Substring(Config.StoragePath.Length), false);

                foreach (string dirPath in dirs)
                {
                    LoTasks.Add(new Task()
                    {
                        Type = TaskType.FolderNamePattern,
                        Path = dirPath.Substring(Config.StoragePath.Length)
                    });
                }
            }
            else
            {
                // remark: Wird für jede PDF ausgeführt, auch für PDFs, die als zusätzliche Datei gedacht sind; Überprüfung bei Abarbeitung?
                if (!path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    return;

                LoTasks.Add(new Task()
                {
                    Type = TaskType.FileNamePattern,
                    Path = path
                });
            }
        }

        /// <summary>
        /// Aktionen bei Umbenennung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="oldPath">alte relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="newPath">neue relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public static void RenameDirOrFile(string oldPath, string newPath, bool dir)
        {
            if (dir)
            {
                if (!ReplaceRefsWorkList(oldPath, newPath))
                {
                    // remark: Wird für jeden Ordner ausgeführt, auch für Strukturordner und Ordner für zusätzliche Dateien; Überprüfung bei Abarbeitung?
                    LoTasks.Add(new Task()
                    {
                        Type = TaskType.FolderNamePattern,
                        Path = newPath
                    });
                }
                ReplaceRefsFolders(oldPath, newPath);
            }
            else
            {
                // remark: Wird für jede PDF ausgeführt, auch für PDFs, die als zusätzliche Datei gedacht sind; Überprüfung bei Abarbeitung?
                bool isOldPdf = oldPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
                bool isNewPdf = newPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

                if (isOldPdf && isNewPdf)
                {
                    if (!ReplaceRefsWorkList(oldPath, newPath))
                    {
                        LoTasks.Add(new Task()
                        {
                            Type = TaskType.FileNamePattern,
                            Path = newPath
                        });
                    }
                }
                else if (isOldPdf && !isNewPdf)
                {
                    DeleteRefsWorkList(oldPath);
                }
                else if (!isOldPdf && isNewPdf)
                {
                    LoTasks.Add(new Task()
                    {
                        Type = TaskType.FileNamePattern,
                        Path = newPath
                    });
                }
            }
        }

        /// <summary>
        /// Aktionen bei Änderung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="absPath">Absolute Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="relPath">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public static void ChangeDirOrFile(string absPath, string relPath, bool dir)
        {
            if (dir)
            {
                if (File.Exists(absPath + @"\Meta.xml"))
                {
                    Song song = new Song(relPath);
                    Instrument value;

                    for (int i = 0; i < song.MetaInfo.FallbackInstrumentation.Count; i++)
                    {
                        value = song.MetaInfo.FallbackInstrumentation.ElementAt(i).Value;

                        if (!song.ExInstrumentation.Instruments.Exists(inst => inst == value))
                        {
                            song.MetaInfo.FallbackInstrumentation.Remove(song.MetaInfo.FallbackInstrumentation.ElementAt(i).Key);
                        }
                    }
                    song.MetaInfo.Save();
                }
            }
        }

        /// <summary>
        /// Aktionen bei Löschung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        public static void DelDirOrFile(string path)
        {
            DeleteRefsWorkList(path);
            DeleteRefsFolders(path);
        }

        /// <summary>
        /// Gibt eine Liste mit allen zu bearbeitenden Aufgaben zurück und sortiert die Liste nach Typ und Verschachtelungstiefe.
        /// </summary>
        /// <param name="loSongFolders">Liste der Ordner, die überprüft werden sollen.</param>
        public static List<Task> FilterList(List<string> loSongFolders)
        {
            CheckFileSystem(loSongFolders);

            string path, filename;
            List<Task> filteredList = new List<Task>();
            var sortedList = from task in LoTasks
                             orderby task.Type descending, task.Path.Split('\\').Length, task.Path
                             select task;

            foreach (var item in sortedList)
            {
                switch (item.Type)
                {
                    case TaskType.FileNamePattern:
                        filename = item.Path.Split('\\').Last();
                        path = item.Path.Substring(0, item.Path.Length - filename.Length);
                        break;
                    case TaskType.FolderNamePattern:
                        path = item.Path + "\\";
                        break;
                    default:
                        continue;
                }

                if (loSongFolders.Count != 0 && !loSongFolders.Exists(folder => path.StartsWith(folder)))
                    continue;

                if (File.Exists(Config.StoragePath + path + "Meta.xml"))
                {
                    // remark: Wenn etwas im Dateisystem hinzugefügt oder gelöscht wird, bleibt die Liste gleich
                    filteredList.Add(item);
                }
            }

            return filteredList;
        }

        /// <summary>
        /// Durchsucht alle Ordner nach PDF-Dateien und Meta.xml, damit in jedem Liedordner eine Meta.xml ist.
        /// Muss eine Meta.xml gelöscht werden, wird eine weitere Methode aufgerufen.
        /// </summary>
        /// <param name="loSongFolders">Liste der Ordner, die überprüft werden sollen.</param>
        public static void CheckFileSystem(List<string> loSongFolders)
        {
            var allMetas = Directory.EnumerateFiles(Config.StoragePath, "Meta.xml", SearchOption.AllDirectories);
            var allPdfs = Directory.EnumerateFiles(Config.StoragePath, "*.pdf", SearchOption.AllDirectories).OrderByDescending(path => path.Split('\\').Length);

            List<string> needMeta = new List<string>();
            List<string> delMeta = new List<string>();
            string songFolder;

            foreach (string path in allPdfs) // Pfade, die potentielle Liedordner sind, in needMeta speichern
            {
                songFolder = path.Substring(Config.StoragePath.Length, path.Length - Config.StoragePath.Length - path.Split('\\').Last().Length).Trim('\\');

                needMeta.RemoveAll(name => name.StartsWith(songFolder + '\\'));
                if (!needMeta.Contains(songFolder))
                    needMeta.Add(songFolder);
            }

            if (needMeta.Exists(name => name == "")) // Root Verzeichnis ist nie Liedordner, daher löschen
                ConfirmDelMeta("");

            foreach (string path in allMetas)
            {
                songFolder = path.Substring(Config.StoragePath.Length, path.Length - Config.StoragePath.Length - path.Split('\\').Last().Length).Trim('\\');

                if (needMeta.RemoveAll(name => name == songFolder) == 0) // Vorhandene Meta.xml aus needMeta löschen
                    delMeta.Add(songFolder);
            }

            // Operationen auf angegebene Liedordner beschränken
            loSongFolders.ForEach(folder =>
            {
                needMeta.RemoveAll(path => !folder.StartsWith(path + '\\') && path != folder);
                delMeta.RemoveAll(path => !folder.StartsWith(path + '\\') && path != folder);
            });

            // Leere Meta.xml in potentiellen Liedordnern erstellen
            Meta meta = new Meta();
            needMeta.ForEach(path =>
            {
                meta.SongFolder = path;
                meta.Save();
            });

            // Meta.xmls löschen
            delMeta.ForEach(path => ConfirmDelMeta(path));
        }

        /// <summary>
        /// Überladung der Methode, um das gesamte Dateisystem zu prüfen.
        /// </summary>
        public static void CheckFileSystem()
        {
            CheckFileSystem(new List<string>());
        }

        /// <summary>
        /// Überladung der Methode, um nur nach einem Ordner zu prüfen.
        /// </summary>
        /// <param name="songFolder">Zu prüfender Ordner</param>
        public static void CheckFileSystem(string songFolder)
        {
            List<string> list = new List<string>();
            list.Add(songFolder);
            CheckFileSystem(list);
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
                if (LoTasks[i].Path.StartsWith(oldPath))
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
        private static void DeleteRefsWorkList(string path)
        {
            for (int i = LoTasks.Count - 1; i >= 0; i--)
            {
                if (LoTasks[i].Path == path || LoTasks[i].Path.StartsWith(path + "\\"))
                    LoTasks.RemoveAt(i);
            }
        }

        /// <summary>
        /// Ersetzt in Mappen.xml den alten Pfad mit dem neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldPath">Alter zu ersetzender Pfad</param>
        /// <param name="newPath">Neuer Pfad</param>
        private static void ReplaceRefsFolders(string oldPath, string newPath)
        {
            List<Folder> folders = Folder.Load();

            for (int i = 0; i < folders.Count; i++)
            {
                for (int j = 0; j < folders[i].Order.Count; j++)
                {
                    if (folders[i].Order.ElementAt(j).Value.StartsWith(oldPath))
                        folders[i].Order.ElementAt(j).Value.Replace(oldPath, newPath);
                }
            }

            Folder.Save(folders);
        }

        /// <summary>
        /// Löscht alle Einträge in Mappe.xml, die sich auf den genannten Pfad oder darunter liegende Elemente beziehen.
        /// </summary>
        /// <param name="path">Gelöschter Pfad</param>
        private static void DeleteRefsFolders(string path)
        {
            List<Folder> folders = Folder.Load();

            for (int i = folders.Count - 1; i >= 0; i--)
            {
                for (int j = folders[i].Order.Count - 1; j >= 0; j--)
                {
                    if (folders[i].Order.ElementAt(j).Value == path || folders[i].Order.ElementAt(j).Value.StartsWith(path + "\\"))
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
             * - PDFs oder Meta.xml in Root
             *   -> User informieren, dann PDFs und Meta.xml löschen
             * - Meta.xml(1) liegt über Meta.xml(2)
             *   -> User fragen, ob Ordner von (2) Liedordner ist, wenn ja (1) und deren PDFs löschen, wenn nein (2) löschen
             *      Die Namen aller zu löschenden PDFs anzeigen.
             * - PDFs liegen über Meta.xml (Fall ausgeschlossen, da automatisch eine Meta.xml erstellt wurde)
             * - Meta.xml hat keine PDFs auf gleicher Ebene
             *   -> User fragen, ob Ordner Liedordner ist, wenn ja nichts tun, wenn nein Meta.xml löschen
             */
            string[] split = folder.Split('\\');
            string path;

            string metaPath = "";
            List<string> pdfs = new List<string>();

            for (int i = 0; i < split.Length; i++) // Liste der Dateien erstellen, welche gelöscht werden müssten
            {
                path = "";
                for (int j = 0; j < i; j++)
                    path += split[j] + "\\";

                if (metaPath == "" && File.Exists(Config.StoragePath + path + "Meta.xml"))
                    metaPath = path + "Meta.xml";

                if (metaPath != "" || (metaPath == "" && path == ""))
                {
                    foreach (string item in Directory.EnumerateFiles(Config.StoragePath + path, "*.pdf"))
                        pdfs.Add(item.Substring(Config.StoragePath.Length));
                }
            }

            string message, pdfText = "";
            MessageBoxResult result;

            pdfs.ForEach(name => pdfText += "\n      - " + name); // Liste formatieren für MessageBox

            if (folder == "") // Das Root-Verzeichnis stellt nie einen Liedordner dar
            {
                if (pdfs.Count != 0)
                {
                    message = "Es ist ein Problem aufgetreten. Folgende PDF-Dateien müssen gelöscht werden:\n"
                        + pdfText + "\n\nWenn Sie das Fenster schließen, werden die genannten Dateien gelöscht, sofern sie noch vorhanden sind.";

                    MessageBox.Show(message, "Problem im Dateisystem", MessageBoxButton.OK, MessageBoxImage.Warning);

                }

                result = MessageBoxResult.Yes;
            }
            else
            {
                message = "Es ist ein Problem aufgetreten. Ist in folgendem Ordner ein Lied gespeichert?\n\n      " + folder; // 2.Fall

                if (pdfs.Count != 0) // 1.Fall
                {
                    message += "\n\nWenn dies der Fall ist, müssen folgende PDF-Dateien entweder gelöscht werden oder aus dem Dateisystem entfernt werden:\n"
                        + pdfText + "\n\nWenn Sie auf \"Ja\" drücken, werden die genannten Dateien gelöscht, sofern sie noch vorhanden sind.";
                }

                result = MessageBox.Show(message, "Problem im Dateisystem", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

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

        #region Laden/Speichern

        private static readonly string _Path = @"Worklist.xml";

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Eventhandler, wenn die Liste verändert wird.
        /// </summary>
        private static void LoTasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            XmlHandler.SaveObject(Config.StoragePath + _Path, LoTasks);
        }

        /// <summary>
        /// Initialisiert die Klassenvariable.
        /// </summary>
        private static void Initialize()
        {
            _LoTasks = XmlHandler.GetObject<ObservableCollection<Task>>(Config.StoragePath + _Path);
            _LoTasks.CollectionChanged += LoTasks_CollectionChanged;
        }

        #endregion
    }
}

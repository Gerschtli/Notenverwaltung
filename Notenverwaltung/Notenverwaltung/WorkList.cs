using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
                // todo: Aktion bei Erstellung eines Verseichnisses
            }
            else
            {
                // todo: Wird für jede PDF ausgeführt, auch für PDFs, die als zusätzliche Datei gedacht sind; Überprüfung bei Abarbeitung?
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
                // todo: Aktion bei Umbennenung eines Verseichnisses
            }
            else
            {
                // todo: Wird für jede PDF ausgeführt, auch für PDFs, die als zusätzliche Datei gedacht sind; Überprüfung bei Abarbeitung?
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
        /// <param name="path">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public static void ChangeDirOrFile(string path, bool dir)
        {
            if (dir)
            {
                // todo: Aktion bei Änderung des Inhalts eines Verseichnisses
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
            for (int i = 0; i < LoTasks.Count; i++)
            {
                if (LoTasks[i].Path == path || LoTasks[i].Path.StartsWith(path + "\\"))
                {
                    LoTasks.RemoveAt(i);
                }
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

            for (int i = 0; i < folders.Count; i++)
            {
                for (int j = 0; j < folders[i].Order.Count; j++)
                {
                    if (folders[i].Order.ElementAt(j).Value == path || folders[i].Order.ElementAt(j).Value.StartsWith(path + "\\"))
                        folders[i].Order.Remove(folders[i].Order.ElementAt(j).Key);
                }
            }

            Folder.Save(folders);
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

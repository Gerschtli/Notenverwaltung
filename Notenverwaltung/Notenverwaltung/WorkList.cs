using System;
using System.Linq;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Liste von Verzeichnissen und Dateien, die z.B. durch den Watcher erkannt werden und abgearbeitet werden müssen.
    /// Jeder Schritt wird direkt in die Worklist gespeichert um Informationsverlust zu verhindern.
    /// </summary>
    public class WorkList
    {
        public List<Task> loTasks = new List<Task>();

        #region Öffentliche Funktionen

        /// <summary>
        /// Aktionen bei Erstellung eines neuen Verzeichnisses oder eines neuen Dokuments.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public void NewDirOrFile(string path, bool dir)
        {
            if (dir)
            {

            }
            else
            {
                if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) // todo: Sinnvoll nach Dateiendung zu gehen?
                    return;

                loTasks.Add(new Task()
                {
                    Type = TaskType.FileNamePattern,
                    Path = path
                });
            }

            Save();
        }

        /// <summary>
        /// Aktionen bei Umbenennung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="oldPath">alte relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="newPath">neue relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public void RenameDirOrFile(string oldPath, string newPath, bool dir)
        {
            if (dir)
            {

            }
            else
            {
                if (oldPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || newPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) // todo: Sinnvoll nach Dateiendung zu gehen?
                    return;

                Task oldItem = new Task()
                {
                    Type = TaskType.FileNamePattern,
                    Path = oldPath
                };
                Task newItem = new Task()
                {
                    Type = TaskType.FileNamePattern,
                    Path = newPath
                };

                if (!ReplaceRefsWorkList(oldPath, newPath))
                    loTasks.Add(newItem);
            }

            Save();
        }

        /// <summary>
        /// Aktionen bei Änderung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public void ChangeDirOrFile(string path, bool dir)
        {
            if (dir)
            {

                Save();
            }
        }

        /// <summary>
        /// Aktionen bei Löschung eines Verzeichnisses oder eines Dokuments.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="dir">Wahr, falls Element ein Verzeichnis ist</param>
        public void DelDirOrFile(string path, bool dir)
        {
            if (dir)
            {

                Save();
            }
        }

        #endregion

        #region Hilfsfunktionen

        /// <summary>
        /// Ersetzt in der Worklist das alte Element mit dem neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldItem">Altes zu ersetzendes Element</param>
        /// <param name="newItem">Neues Element</param>
        /// <returns>Gibt an, ob das alte Element vorhanden war</returns>
        private bool ReplaceRefsWorkList(string oldPath, string newPath)
        {
            bool ret = false;

            for (int i = 0; i < loTasks.Count; i++)
            {
                if (loTasks[i].Path.StartsWith(oldPath))
                {
                    loTasks[i].Path = newPath + loTasks[i].Path.Substring(oldPath.Length);
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Löscht alle Aufgaben, die sich auf den genannten Pfad oder darunter liegende Elemente beziehen.
        /// </summary>
        /// <param name="path">Gelöschter Pfad</param>
        private void DeleteRefsWorkList(string path)
        {
            loTasks.RemoveAll(task => task.Path.StartsWith(path)); // todo: Nicht eindeutig! Bsp.: Ordner "bla/test" gelöscht -> würde auch "bla/test#text" löschen
        }

        /// <summary>
        /// Ersetzt in Mappen.xml den alten Pfad mit dem neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldPath">Alter zu ersetzender Pfad</param>
        /// <param name="newPath">Neuer Pfad</param>
        private void ReplaceRefsFolders(string oldPath, string newPath)
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
        private void DeleteRefsFolders(string path)
        {
            List<Folder> folders = Folder.Load();

            for (int i = 0; i < folders.Count; i++)
            {
                for (int j = 0; j < folders[i].Order.Count; j++)
                {
                    if (folders[i].Order.ElementAt(j).Value.StartsWith(path)) // todo: Nicht eindeutig! Bsp.: Ordner "bla/test" gelöscht -> würde auch "bla/test#text" löschen
                        folders[i].Order.Remove(folders[i].Order.ElementAt(j).Key);
                }
            }

            Folder.Save(folders);
        }

        #endregion

        #region Laden/Speichern

        private static readonly string _Path = @"Worklist.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static WorkList Load()
        {
            return XmlHandler.GetObject<WorkList>(Config.StoragePath + _Path);
        }

        /// <summary>
        /// Speichert die als Parameter angegebene Instanz.
        /// </summary>
        public void Save()
        {
            XmlHandler.SaveObject(Config.StoragePath + _Path, this);
        }
        #endregion
    }
}
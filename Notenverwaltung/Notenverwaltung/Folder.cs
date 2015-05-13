using System.Collections.Generic;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt die Reihenfolge von Musikstücken in einer Mappe dar.
    /// </summary>
    public class Folder
    {
        public string Name { get; set; }

        public SerializableDictionary<int, string> Order { get; set; }

        public Folder()
        {
            Order = new SerializableDictionary<int, string>();
        }

        #region Öffentliche Funktionen

        /// <summary>
        /// Fügt den gewünschten Eintrag ein, wenn dieser die Kriterien erfüllt.
        /// </summary>
        /// <param name="pos">Liednummer</param>
        /// <param name="songFolder">Liedordner</param>
        /// <returns>Ob Eintrag hinzugefügt wurde oder nicht</returns>
        public bool AddSong(int pos, string songFolder)
        {
            List<string> loSongFolders = Song.LoadAll();

            if (pos > 0 && !Order.ContainsKey(pos) && loSongFolders.Contains(songFolder))
            {
                Order.Add(pos, songFolder);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ersetzt in Order den alten Pfad durch den neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldPath">Alter zu ersetzender Pfad</param>
        /// <param name="newPath">Neuer Pfad</param>
        public void ReplacePath(string oldPath, string newPath)
        {
            List<string> loSongFolders = Song.LoadAll();
            string folder;

            for (int i = Order.Count - 1; i >= 0; i--)
            {
                if (Order.ElementAt(i).Value == oldPath || Order.ElementAt(i).Value.StartsWith(oldPath + "\\"))
                {
                    folder = newPath + Order.ElementAt(i).Value.Substring(oldPath.Length);

                    if (loSongFolders.Contains(folder))
                        Order[Order.ElementAt(i).Key] = folder;
                    else
                        Order.Remove(Order.ElementAt(i).Key);
                }
            }
        }

        /// <summary>
        /// Ersetzt in allen Ordner in Order den alten Pfad durch den neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldPath">Alter zu ersetzender Pfad</param>
        /// <param name="newPath">Neuer Pfad</param>
        public static void ReplacePathForAll(string oldPath, string newPath)
        {
            List<Folder> folders = Factory.GetFolders();

            for (int i = 0; i < folders.Count; i++)
            {
                folders[i].ReplacePath(oldPath, newPath);
            }

            Save.Folders(folders);
        }

        /// <summary>
        /// Löscht alle Einträge in allen Ordnern, die sich auf den genannten Pfad oder darunter liegende Elemente beziehen.
        /// </summary>
        /// <param name="path">Gelöschter Pfad</param>
        /// <param name="onlyThis">Gibt an, ob nur genau dieser Eintrag gelöscht werden soll.</param>
        public static void DeletePathForAll(string path, bool onlyThis = false)
        {
            List<Folder> folders = Factory.GetFolders();

            for (int i = folders.Count - 1; i >= 0; i--)
            {
                for (int j = folders[i].Order.Count - 1; j >= 0; j--)
                {
                    if (folders[i].Order.ElementAt(j).Value == path || (!onlyThis && folders[i].Order.ElementAt(j).Value.StartsWith(path + "\\")))
                        folders[i].Order.Remove(folders[i].Order.ElementAt(j).Key);
                }
            }

            Save.Folders(folders);
        }

        /// <summary>
        /// Ermittelt die fehlenden Nummern in der Auflistung.
        /// </summary>
        /// <returns>Sortierte Liste von Nummern</returns>   
        public List<int> GetMissingNumbers()
        {
            List<int> keyList = new List<int>(this.Order.Keys);
            if (keyList.Count == 0) // wenn das Dictionary leer ist, nichts zurückgeben
            {
                return null;
            }
            keyList.Sort();
            List<int> missList = new List<int>();
            for (int i = 1; i <= keyList[keyList.Count - 1]; i++)
            {
                if (keyList.Contains(i) == false)
                {
                    missList.Add(i);
                }
            }
            return missList;
        }

        /// <summary>
        /// Löscht Einträge, die keine Lieder sind.
        /// </summary>
        public void CheckSongs()
        {
            List<string> loSongFolders = Song.LoadAll();

            for (int i = Order.Count - 1; i >= 0; i--)
            {
                if (!loSongFolders.Contains(Order.ElementAt(i).Value))
                {
                    Order.Remove(Order.ElementAt(i).Key);
                }
            }
        }

        #endregion
    }
}

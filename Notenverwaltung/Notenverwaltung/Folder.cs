using System;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt die Reihenfolge von Musikstücken in einer Mappe dar.
    /// </summary>
    public class Folder
    {
        public string Name;

        public SerializableDictionary<int, string> Order = new SerializableDictionary<int, string>();

        #region Speicherung

        private static readonly string _Path = @"Mappen.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static List<Folder> Load()
        {
            return XmlHandler.GetObject<List<Folder>>(Config.StoragePath + _Path);
        }

        /// <summary>
        /// Speichert die als Parameter angegebene Instanz.
        /// </summary>
        public static void Save(List<Folder> folders)
        {
            XmlHandler.SaveObject(Config.StoragePath + _Path, folders);
        }

        #endregion

        #region "öffentliche Funktionen"
        /// <summary>
        /// ermittelt die fehlenden Nummern in der Auflistung
        /// </summary>
        /// <returns>sortierte Liste von Nummern</returns>   
        public List<int> GetMissingNumbers()
            {
                List<int> keyList = new List<int>(this.Order.Keys);
                if (keyList.Count == 0) // wenn das Dictionary leer ist, nicht zurückgeben
                {
                    return null;
                }
                keyList.Sort();
                List<int> missList = new List<int> ();
                for (int i = 1; i <= keyList[keyList.Count - 1]; i++)
                {
                    if (keyList.Contains(i) == false)
                    {
                        missList.Add(i);
                    }
                }
                return missList;
            }


        #endregion
    }
}

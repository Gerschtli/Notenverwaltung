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

        public Dictionary<int, string> Order; // todo: Überprüfen, ob Musikstück existiert

        #region Speicherung

        private static readonly string _Path = @"\Einstellungen\Mappen.xml";

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
    }
}

using System;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Klasse zum Editieren der in der Datei "Kategorien.xml" gespeicherten Kategorien, sowie zur Speicherung der zu einem Musikstück gehörigen Kategorien.
    /// </summary>
    public class Categories
    {
        public List<string> Names;

        #region Speicherung (alle Kategorien)

        private static readonly string _Path = @"Kategorien.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static Categories Load()
        {
            return XmlHandler.GetObject<Categories>(Config.StoragePath + _Path);
        }

        /// <summary>
        /// Speichert die aktuelle Instanz.
        /// </summary>
        public void Save()
        {
            XmlHandler.SaveObject(Config.StoragePath + _Path, this);
        }

        #endregion
    }
}

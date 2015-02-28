using System;

namespace Notenverwaltung
{
    /// <summary>
    /// Klasse zum Editieren der in der Datei "Kategorien.xml" gespeicherten Kategorien.
    /// </summary>
    public class Categories
    {
        private static string CategoriesPath = @"\Einstellungen\Kategorien.xml";

        public string[] Names;

        /// <summary>
        /// Lädt die Liste aller Kategorien.
        /// </summary>
        public Categories()
        {
            Names = XmlHandler.GetObject<Categories>(Config.StoragePath + CategoriesPath).Names;
        }

        /// <summary>
        /// Speichert die Liste der Kategorien.
        /// </summary>
        public void Save()
        {
            XmlHandler.SaveObject(Config.StoragePath + CategoriesPath, this);
        }
    }
}

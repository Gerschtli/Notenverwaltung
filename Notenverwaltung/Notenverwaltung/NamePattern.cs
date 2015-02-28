using System;

namespace Notenverwaltung
{
    /// <summary>
    /// Verwaltet alle NamePatterns.
    /// </summary>
    public class NamePattern
    {
        // todo: System zur Speicherung der NamePatterns entwickeln
        //public List<string> Names;

        #region Speicherung

        private static readonly string _Path = @"\Einstellungen\Kategorien.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static NamePattern Load(string songFolder)
        {
            return XmlHandler.GetObject<NamePattern>(Config.StoragePath + _Path);
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

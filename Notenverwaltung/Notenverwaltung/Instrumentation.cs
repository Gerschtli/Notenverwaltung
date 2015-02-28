using System;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt eine vollständige Besetzung dar.
    /// </summary>
    public class Instrumentation
    {
        public string Name;

        public List<Instrument> Instruments;

        #region Speicherung

        private static readonly string _Path = @"\Einstellungen\Besetzungen.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static List<Instrumentation> Load()
        {
            return XmlHandler.GetObject<List<Instrumentation>>(Config.StoragePath + _Path);
        }

        /// <summary>
        /// Speichert die als Parameter angegebene Instanz.
        /// </summary>
        public static void Save(List<Instrumentation> instrumentations)
        {
            XmlHandler.SaveObject(Config.StoragePath + _Path, instrumentations);
        }

        #endregion
    }
}

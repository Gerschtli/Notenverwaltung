using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Notenverwaltung
{
    /// <summary>
    /// Enthält Metainformationen zu einem Musikstück.
    /// </summary>
    public class Meta
    {
        public Categories Category;

        public List<Instrumentation> OriginalInstrumentation;

        public List<Instrumentation> FallbackInstrumentation;

        #region Speicherung

        private static string _Path = @"\Musikstücke\{0}\Meta.xml";

        [XmlIgnore]
        public string SongFolder;

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static Meta Load(string songFolder)
        {
            Meta meta = XmlHandler.GetObject<Meta>(Config.StoragePath + String.Format(_Path, songFolder));
            meta.SongFolder = songFolder;
            return meta;
        }

        /// <summary>
        /// Speichert die aktuelle Instanz.
        /// </summary>
        public void Save()
        {
            XmlHandler.SaveObject(Config.StoragePath + String.Format(_Path, SongFolder), this);
        }

        #endregion
    }
}

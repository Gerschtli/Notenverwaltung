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
        public Categories Category { get; set; }

        public Instrumentation OriginalInstrumentation { get; set; }

        /// <summary>
        /// Verwendung wie folgt:
        /// Da [Key] nicht besetzt ist, nehme [Value] stattdessen.
        /// </summary>
        public SerializableDictionary<Instrument, Instrument> FallbackInstrumentation { get; set; }

        public Meta()
        {
            Category = new Categories();
            OriginalInstrumentation = new Instrumentation();
            FallbackInstrumentation = new SerializableDictionary<Instrument, Instrument>();
        }

        #region Speicherung

        private static string _Path = @"{0}\Meta.xml";

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

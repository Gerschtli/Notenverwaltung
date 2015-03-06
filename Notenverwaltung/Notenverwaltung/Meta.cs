using System;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Enthält Metainformationen zu einem Musikstück.
    /// </summary>
    public class Meta //UnresolvedMergeConflict: wieso bauen wir hier Metadaten, wenn wir bei Instrumenten die Objekte ausbilden? - Um die Meta-Daten als Objekt in der XML-Datei Meta.xml speichern können
    {
        public Categories Category;

        public List<Instrumentation> OriginalInstrumentation;

        public List<Instrumentation> FallbackInstrumentation;

        #region Speicherung

        private static string _Path = @"\Musikstücke\{0}\Meta.xml";

        private string _SongFolder; // Deklarierung als private, damit es nicht in der XML-Datei gespeichert wird.

        public void SetSongFolder(string songFolder)
        {
            _SongFolder = songFolder;
        }

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static Meta Load(string songFolder)
        {
            Meta meta = XmlHandler.GetObject<Meta>(Config.StoragePath + String.Format(_Path, songFolder));
            meta.SetSongFolder(songFolder);
            return meta;
        }

        /// <summary>
        /// Speichert die aktuelle Instanz.
        /// </summary>
        public void Save()
        {
            XmlHandler.SaveObject(Config.StoragePath + String.Format(_Path, _SongFolder), this);
        }

        #endregion
    }
}

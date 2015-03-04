using System;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Enthält Metainformationen zu einem Musikstück.
    /// </summary>
    public class Meta //UnresolvedMergeConflict: wieso bauen wir hier Metadaten, wenn wir bei Instrumenten die Objekte ausbilden?
    {
        public Categories Category;

        public List<Instrumentation> OriginalInstrumentation;

        public List<Instrumentation> FallbackInstrumentation;

        #region Speicherung

        private static string _Path = @"\Musikstücke\{0}\Meta.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static Meta Load(string songFolder)
        {
            return XmlHandler.GetObject<Meta>(Config.StoragePath + String.Format(_Path, songFolder));
        }

        /// <summary>
        /// Speichert die aktuelle Instanz.
        /// </summary>
        public void Save(string songFolder)
        {
            XmlHandler.SaveObject(Config.StoragePath + String.Format(_Path, songFolder), this);
        }

        #endregion
    }
}

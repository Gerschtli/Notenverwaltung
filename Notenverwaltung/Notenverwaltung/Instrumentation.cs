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

        public List<Instrument> Instruments = new List<Instrument>();
        
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

        #region "öffentliche Funktionen"
        /// <summary>
        /// ermittelt die fehlenden Stimmen in der Besetzung
        /// </summary>
        /// <param name="b">Besetzung</param>
        /// <returns></returns>
        public List<Instrument> GetMissingInstruments(Instrumentation b)
        {
            return compareInstrumentation(this.Instruments, b.Instruments);
        }

        /// <summary>
        /// ermittelt die überzähligen/überflüssigen Stimmen in der Besetzung
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<Instrument> GetNeedlessInstruments(Instrumentation b)
        {
            return compareInstrumentation(b.Instruments, this.Instruments);
        }

        #endregion

        #region "Hilfsfunktionen"
        /// <summary>
        /// ermittelt die fehlenden Instrumente
        /// </summary>
        /// <param name="a">Besetzungsliste a</param>
        /// <param name="b">Besetzungsliste b</param>
        /// <returns></returns>
        private List<Instrument> compareInstrumentation(List<Instrument> a, List<Instrument> b)
        {
            List<Instrument> loInstruments = new List<Instrument>();

            foreach (Instrument i in a)
            {
                if (b.Contains(i) == false)
                {
                    loInstruments.Add(i);
                }
            }
            return loInstruments;
        }
        #endregion

    }
}

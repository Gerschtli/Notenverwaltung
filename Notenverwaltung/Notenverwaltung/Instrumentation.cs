using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt eine vollständige Besetzung dar.
    /// </summary>
    public class Instrumentation
    {
        public string Name { get; set; }

        public List<Instrument> Instruments { get; set; }

        public Instrumentation()
        {
            Instruments = new List<Instrument>();
        }

        #region Öffentliche Funktionen

        /// <summary>
        /// Ermittelt die fehlenden Stimmen in der Besetzung
        /// </summary>
        public List<Instrument> GetMissingInstruments(Instrumentation b)
        {
            return CompareInstrumentation(this.Instruments, b.Instruments);
        }

        /// <summary>
        /// Ermittelt die überzähligen/überflüssigen Stimmen in der Besetzung
        /// </summary>
        public List<Instrument> GetNeedlessInstruments(Instrumentation b)
        {
            return CompareInstrumentation(b.Instruments, this.Instruments);
        }

        #endregion

        #region Hilfsfunktionen

        /// <summary>
        /// Ermittelt die fehlenden Instrumente
        /// </summary>
        /// <param name="a">Besetzungsliste a</param>
        /// <param name="b">Besetzungsliste b</param>
        private List<Instrument> CompareInstrumentation(List<Instrument> a, List<Instrument> b)
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

using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Enthält Metainformationen zu einem Musikstück.
    /// </summary>
    public class Meta
    {
        public List<string> Category { get; set; }

        public Instrumentation OriginalInstrumentation { get; set; }

        public string SongFolder { get; set; }

        /// <summary>
        /// Verwendung wie folgt:
        /// Da [Key] nicht besetzt ist, nehme [Value] stattdessen.
        /// </summary>
        public SerializableDictionary<Instrument, Instrument> FallbackInstrumentation { get; set; }

        public Meta()
        {
            Category = new List<string>();
            OriginalInstrumentation = new Instrumentation();
            FallbackInstrumentation = new SerializableDictionary<Instrument, Instrument>();
        }
    }
}

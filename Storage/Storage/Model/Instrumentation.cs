using System.Collections.Generic;

namespace Storage.Model
{
    public class Instrumentation
    {
        public string Name { get; set; }

        public ISet<Instrument> Instruments { get; set; }
    }
}

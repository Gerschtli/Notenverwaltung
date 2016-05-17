using System.Collections.Generic;

namespace Storage.Model
{
    public class Folder
    {
        public string Name { get; set; }

        public IDictionary<int, Song> Order { get; set; }
    }
}

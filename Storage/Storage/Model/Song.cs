using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Storage.Model
{
    public class Song : IEquatable<Song>
    {
        [IgnoreDataMember]
        public string Path { get; set; }

        public string Name { get; set; }

        public string Composer { get; set; }

        public string Arranger { get; set; }

        public ISet<Category> Categories { get; set; }

        public ISet<Instrument> Instruments { get; set; }

        public IDictionary<Instrument, Instrument> FallbackInstruments { get; set; }

        #region Overridden Methods

        public bool Equals(Song other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return string.Equals(Path, other.Path) && string.Equals(Name, other.Name)
                && string.Equals(Composer, other.Composer) && string.Equals(Arranger, other.Arranger);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((Song) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hashCode = Path != null ? Path.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Composer != null ? Composer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Arranger != null ? Arranger.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Song left, Song right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Song left, Song right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}

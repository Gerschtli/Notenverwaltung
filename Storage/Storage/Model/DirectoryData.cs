using System;

namespace Storage.Model
{
    public class DirectoryData : IEquatable<DirectoryData>
    {
        public DirectoryStatus Status { get; set; }

        public Song Song { get; set; }

        #region Overridden Methods

        public bool Equals(DirectoryData other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Status == other.Status && Equals(Song, other.Song);
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
            return Equals((DirectoryData) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                return ((int) Status * 397) ^ (Song != null ? Song.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DirectoryData left, DirectoryData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DirectoryData left, DirectoryData right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}

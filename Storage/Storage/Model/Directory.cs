using System;

namespace Storage.Model
{
    public class Directory : IEquatable<Directory>
    {
        public string Path { get; set; }

        #region Overridden Methods

        public bool Equals(Directory other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return string.Equals(Path, other.Path);
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
            return Equals((Directory) obj);
        }

        public override int GetHashCode()
        {
            return Path != null ? Path.GetHashCode() : 0;
        }

        public static bool operator ==(Directory left, Directory right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Directory left, Directory right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}

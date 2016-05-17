using System;

namespace Storage.Model
{
    public class Task : IEquatable<Task>
    {
        public string Path { get; set; }

        public TaskType Type { get; set; }

        #region Overridden Methods

        public bool Equals(Task other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return string.Equals(Path, other.Path) && Type == other.Type;
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
            return Equals((Task) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ (int) Type;
            }
        }

        public static bool operator ==(Task left, Task right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Task left, Task right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}

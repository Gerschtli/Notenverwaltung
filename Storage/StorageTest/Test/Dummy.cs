using System;

namespace StorageTest.Test
{
    public class Dummy : IEquatable<Dummy>
    {
        public int TestInt { get; set; }

        public string TestString { get; set; }

        #region Overridden Methods

        public override string ToString()
        {
            return string.Format("TestInt: {0}, TestString: {1}", TestInt, TestString);
        }

        public bool Equals(Dummy other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return TestInt == other.TestInt && string.Equals(TestString, other.TestString);
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
            return Equals((Dummy) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                return (TestInt * 397) ^ (TestString != null ? TestString.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Dummy left, Dummy right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Dummy left, Dummy right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}

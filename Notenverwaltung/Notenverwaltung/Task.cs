using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt die verschiedenen Typen der Aufgaben dar.
    /// </summary>
    public enum TaskType
    {
        Default,
        FileNamePattern,
        FolderNamePattern
    }

    /// <summary>
    /// Repräsentiert eine Aufgabe in der Worklist.
    /// </summary>
    public class Task : IEquatable<Task>
    {
        public TaskType Type = TaskType.Default;

        public string Path;

        #region Überschriebene Methoden

        /// <summary>
        /// Gleichheitsprüfung über Objekte
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Task task = obj as Task;
            if ((System.Object)task == null)
                return false;

            if (task.Type == Type && string.Compare(task.Path, Path, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gleichheitsprüfung mit typisiertem Objekt
        /// </summary>
        public bool Equals(Task task)
        {
            if (task == null)
                return false;

            if (task.Type == Type && string.Compare(task.Path, Path, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gleichheitsprüfung ==-Operator
        /// </summary>
        public static bool operator ==(Task a, Task b)
        {
            if (System.Object.ReferenceEquals(a, b)) // Wenn beide Objekte null oder die gleiche Instanz sind -> true
                return true;

            if (((object)a == null) || ((object)b == null)) // Wenn ein Objekt null ist, aber nicht beide -> false.
                return false;

            if (a.Type == b.Type && string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gleichheitsprüfung !=-Operator
        /// </summary>
        public static bool operator !=(Task a, Task b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Überschreiben der GetHashCode Methode
        /// </summary>
        public override int GetHashCode()
        {
            return Type.GetHashCode() + Path.GetHashCode();
        }

        #endregion
    }
}

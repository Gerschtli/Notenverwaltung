namespace Notenverwaltung
{
    /// <summary>
    /// Verwaltet die Liste von Aufgaben, die durch den Watcher erkannt werden und abgearbeitet werden müssen.
    /// </summary>
    public class WorkList
    {
        // Diese Liste sollte nicht von außerhalb der Klasse verändert werden.
        public MTObservableCollection<Task> LoTasks { get; private set; }

        private static WorkList instance = null;

        /// <summary>
        /// Wird zur Initialisierung der Aufgabenliste und vor der Instanzierung des Watchers aufgerufen.
        /// </summary>
        private WorkList()
        {
            LoTasks = new MTObservableCollection<Task>();
        }

        public static WorkList GetInstance()
        {
            if (instance == null)
                instance = new WorkList();

            return instance;
        }

        /// <summary>
        /// Ersetzt in der Worklist das alte Element mit dem neuen, funktioniert auch nur mit dem Anfang des Pfades.
        /// </summary>
        /// <param name="oldItem">Altes zu ersetzendes Element</param>
        /// <param name="newItem">Neues Element</param>
        /// <returns>Gibt an, ob das alte Element vorhanden war</returns>
        public bool ReplaceRefs(string oldPath, string newPath)
        {
            bool ret = false;

            for (int i = 0; i < LoTasks.Count; i++)
            {
                if (LoTasks[i].Path.StartsWith(oldPath + "\\"))
                {
                    LoTasks[i].Path = newPath + LoTasks[i].Path.Substring(oldPath.Length);
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Löscht alle Aufgaben, die sich auf den genannten Pfad oder darunter liegende Elemente beziehen.
        /// </summary>
        /// <param name="path">Gelöschter Pfad</param>
        /// <param name="onlyThis">Gibt an, ob nur genau dieser Eintrag gelöscht werden soll.</param>
        public void DeleteRefs(string path, bool onlyThis = false)
        {
            for (int i = LoTasks.Count - 1; i >= 0; i--)
            {
                if (LoTasks[i].Path == path || (!onlyThis && LoTasks[i].Path.StartsWith(path + "\\")))
                    LoTasks.RemoveAt(i);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Notenverwaltung
{
    /// <summary>
    /// Liste von Verzeichnissen und Dateien, die z.B. durch den Watcher erkannt werden und abgearbeitet werden müssen.
    /// Jeder Schritt wird direkt in die Worklist gespeichert um Informationsverlust zu verhindern.
    /// </summary>
    public class WorkList // UnresolvedMergeConflict: Wozu brauchen wir eine komplette Auflistung aller Elemente in unserem Dateisystem?
    {
        public List<string> loDirOrFiles = new List<string>(); // todo: nur lesend die Liste veröffentlichen?!

        /// <summary>
        /// Neues Verzeichnis oder Datei zum bearbeiten eintragen
        /// </summary>
        /// <param name="DirOrFile">Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        public void NewDirOrFile(string DirOrFile)
        {
            if (this.loDirOrFiles.Exists(x => string.Compare(x, DirOrFile, StringComparison.OrdinalIgnoreCase) != 0))
            {
                this.loDirOrFiles.Add(DirOrFile);
                this.Save();
            }
        }

        /// <summary>
        /// in der Liste stehendes oder bereits bearbeitetes Dokument / Verzeichnis löschen
        /// </summary>
        /// <param name="DirOrFile">Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        public void DelDirOrFile(string DirOrFile)
        {
            if (this.loDirOrFiles.Exists(x => string.Compare(x, DirOrFile, StringComparison.OrdinalIgnoreCase) == 0))
            {
                loDirOrFiles.Remove(DirOrFile);
                this.Save();
            }
            else
            { } //todo: Verhalten bei bestehenden Dokumenten
        }

        /// <summary>
        /// in der Liste stehendes oder bereits bearbeitetes Dokument / Verzeichnis wird im Namen geändert
        /// </summary>
        /// <param name="oldDirOrFile">alte Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        /// <param name="newDirOrFile">neue Pfadangabe eines Dokuments oder eines Verzeichnisses</param>
        public void RenameDirOrFile(string oldDirOrFile, string newDirOrFile)
        {
            if (this.loDirOrFiles.Exists(x => string.Compare(x, oldDirOrFile, StringComparison.OrdinalIgnoreCase) == 0))
            {
                loDirOrFiles.Remove(oldDirOrFile); //alten Eintrag löschen ...
                this.loDirOrFiles.Add(newDirOrFile); //... und neuen Eintrag erstellen
            }
            else
            { } //todo: Verhalten bei bestehenden Dokumenten
        }

        #region Laden/Speichern

        private static readonly string _Path = @"\Einstellungen\Worklist.xml";

        /// <summary>
        /// Lädt das gespeicherte Objekt.
        /// </summary>
        public static WorkList Load()
        {
            return XmlHandler.GetObject<WorkList>(Config.StoragePath + _Path);
        }

        /// <summary>
        /// Speichert die als Parameter angegebene Instanz.
        /// </summary>
        public void Save()
        {
            XmlHandler.SaveObject(Config.StoragePath + _Path, this);
        }
        #endregion
    }
}
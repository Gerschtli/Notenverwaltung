using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Notenverwaltung
{
    /// <summary>
    /// Überprüft das Dateisystem.
    /// </summary>
    public class FileSystemChecker
    {
        private Config config;
        private string folder;

        /// <summary>
        /// Initialisiert eine Instanz für einen bestimmten Pfad.
        /// </summary>
        /// <param name="folder">Zu prüfender Pfad.</param>
        public FileSystemChecker(string folder = "")
        {
            this.folder = folder;
            config = Config.GetInstance();
        }

        /// <summary>
        /// Durchsucht alle Ordner nach PDF-Dateien und Meta.xml, damit in jedem Liedordner eine Meta.xml ist.
        /// Muss eine Meta.xml gelöscht werden, wird eine weitere Methode aufgerufen.
        /// </summary>
        public void CheckStructure()
        {
            List<string> needMeta = NeedMetaList();

            List<string> delMeta = DelMetaList(needMeta);

            // Leere Meta.xml in potentiellen Liedordnern erstellen
            Meta meta = new Meta();
            needMeta.ForEach(path =>
            {
                meta.SongFolder = path;
                Save.Meta(meta);
            });

            // Meta.xml in Root löschen (ohne Benutzerinformation)
            if (delMeta.Remove(""))
                DeleteFile(config.MetaPath.Substring(4));
            // Meta.xmls löschen (mit Benutzerinteraktion)
            delMeta.ForEach(path => ConfirmDelMeta(path));
        }

        #region Hilfsfunktionen

        /// <summary>
        /// Generiert eine Liste von Ordnernamen relativ zum Speicherpfad der Notenverwaltung,
        /// welche PDFs enthalten und nach der Definition eines Liedordners eine Meta.xml brauchen.
        /// </summary>
        private List<string> NeedMetaList()
        {
            IEnumerable<string> allPdfs = GetFilePaths("*.pdf");

            allPdfs = from file in allPdfs
                      where file.Split('\\').Length > config.StoragePath.Split('\\').Length + 1 // sortiert Dateien im Rootverzeichnis aus
                      orderby file.Split('\\').Length descending
                      select file;

            List<string> needMeta = new List<string>();
            string songFolder;

            foreach (string path in allPdfs) // Pfade, die potentielle Liedordner sind, in needMeta speichern
            {
                songFolder = ExtractSongFolder(path);

                needMeta.RemoveAll(name => name.StartsWith(songFolder + '\\'));
                if (!needMeta.Contains(songFolder))
                    needMeta.Add(songFolder);
            }
            return needMeta;
        }

        /// <summary>
        /// Generiert eine Liste von Ordnernamen relativ zum Speicherpfad der Notenverwaltung,
        /// welche eine Meta.xml enthalten, jedoch aufgrund der Dateistruktur keine Liedordner sein können.
        /// </summary>
        /// <param name="needMeta">Liste von Ordnernamen; wird manipuliert, um vorhandene Liedordner aus
        /// Liste zu löschen.</param>
        private List<string> DelMetaList(List<string> needMeta)
        {
            IEnumerable<string> allMetas = GetFilePaths(config.MetaPath.Substring(4));

            List<string> delMeta = new List<string>();
            string songFolder;

            foreach (string path in allMetas)
            {
                songFolder = ExtractSongFolder(path);

                if (needMeta.RemoveAll(name => name == songFolder) == 0) // Vorhandene Metadatei aus needMeta löschen
                    delMeta.Add(songFolder);
            }

            return delMeta;
        }

        /// <summary>
        /// Zeigt eine MessageBox für den Benutzer bzgl. PDF-Dateien, die nicht mit der momentanen Struktur
        /// zu vereienen sind, und reagiert auf dessen Eingabe.
        /// </summary>
        /// <param name="path">Relative Pfadangabe eines Verzeichnisses, welches eine Meta.xml enthält,
        /// die an dieser Stelle nicht erlaubt ist.</param>
        private void ConfirmDelMeta(string path)
        {
            /* Fenster für User:
             * Mögliche Problemursachen:
             * - Metadatei(1) liegt über Metadatei(2)
             *   -> User fragen, ob Ordner von (2) Liedordner ist, wenn ja (1) und deren PDFs löschen, wenn nein (2) löschen
             *      Die Namen aller zu löschenden PDFs anzeigen.
             * - Metadatei hat keine PDFs auf gleicher Ebene
             *   -> User fragen, ob Ordner Liedordner ist, wenn ja nichts tun, wenn nein Metadatei löschen
             */
            string metaPath;
            List<string> pdfs = InvalidPdfs(path, out metaPath);


            string pdfText = "";

            pdfs.ForEach(name => pdfText += "\n      - " + name); // Liste formatieren für MessageBox


            string message = "Es ist ein Problem aufgetreten. Ist in folgendem Ordner ein Lied gespeichert?\n\n      " + path; // 2.Fall

            if (pdfs.Count != 0) // 1.Fall
            {
                message += "\n\nWenn dies der Fall ist, müssen folgende PDF-Dateien entweder gelöscht werden oder aus dem Dateisystem entfernt werden:\n"
                    + pdfText + "\n\nWenn Sie auf \"Ja\" drücken, werden die genannten Dateien gelöscht, sofern sie noch vorhanden sind.";
            }

            MessageBoxResult result = MessageBox.Show(message, "Problem im Dateisystem", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result)
            {
                case MessageBoxResult.No: // Angegebenes Verzeichnis ist kein Liedordner
                    DeleteFile(String.Format(config.MetaPath, path));
                    break;
                case MessageBoxResult.Yes: // Angegebenes Verzeichnis ist Liedordner
                    DeleteFile(metaPath);
                    pdfs.ForEach(name => DeleteFile(name));
                    break;
            }
        }

        /// <summary>
        /// Generiert eine Liste von PDFs relativ zum Speicherpfad der Notenverwaltung, die dort nicht
        /// liegen dürfen, wenn "checkFolder" ein Liedordner ist.
        /// </summary>
        /// <param name="checkFolder">Potentieller Liedordner</param>
        /// <param name="metaPath">Pfad zur ersten Meta.xml Datei</param>
        private List<string> InvalidPdfs(string checkFolder, out string metaPath)
        {
            string[] split = checkFolder.Split('\\');
            string path;

            metaPath = null;
            List<string> pdfs = new List<string>();

            for (int i = 1; i < split.Length; i++) // Liste der Dateien erstellen, welche gelöscht werden müssten
            {
                path = "";
                for (int j = 0; j < i; j++)
                    path += split[j] + "\\";


                if (metaPath == null && File.Exists(Path.Combine(config.StoragePath, String.Format(config.MetaPath, path))))
                    metaPath = String.Format(config.MetaPath, path);

                if (metaPath != null)
                {
                    foreach (string item in Directory.EnumerateFiles(Path.Combine(config.StoragePath, path), "*.pdf"))
                        pdfs.Add(item.Substring(config.StoragePath.Length).Trim('\\'));
                }
            }

            return pdfs;
        }

        /// <summary>
        /// Extrahiert aus einem String im Format Config.StoragePath + SongFolder + Dateiname den SongFolder.
        /// </summary>
        private string ExtractSongFolder(string path)
        {
            return
                path.Substring(
                    config.StoragePath.Length,
                    path.Length - config.StoragePath.Length - path.Split('\\').Last().Length
                ).Trim('\\');
        }

        /// <summary>
        /// Erzeugt eine IEnumerable mit allen Elementen, die dem gesuchten Typ entsprechen.
        /// </summary>
        /// <param name="type">Gesuchter Typ</param>
        private IEnumerable<string> GetFilePaths(string type)
        {
            var allFiles = Directory.EnumerateFiles(Path.Combine(config.StoragePath, folder), type, SearchOption.AllDirectories);

            if (folder != "")
            {
                string[] split = folder.Split('\\');
                string path;

                // Iteration über Array split, um alle Ordner über folder zu scannen.
                for (int i = split.Length - 1; i >= 0; i--)
                {
                    path = "";
                    for (int j = 0; j < i; j++)
                        path += split[j] + "\\";

                    allFiles = allFiles.Concat(Directory.EnumerateFiles(Path.Combine(config.StoragePath, path), type));
                }
            }

            return allFiles;
        }

        /// <summary>
        /// Löscht eine Datei, ohne dass eine Exception geworfen wird.
        /// </summary>
        /// <param name="path">Pfad der zu löschenden Datei</param>
        private void DeleteFile(string path)
        {
            try
            {
                File.Delete(Path.Combine(config.StoragePath, path));
            }
            catch
            {
                return;
            }
        }

        #endregion
    }
}

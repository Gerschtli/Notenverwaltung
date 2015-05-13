using System;
using System.Collections.Generic;
using System.IO;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt die Funktionalität zum Laden und Erstellen von Objekten zur Verfügung.
    /// </summary>
    public static class Factory
    {
        private static Config config;
        private static Config Config
        {
            get
            {
                if (config == null)
                    config = Config.GetInstance();
                return config;
            }
        }

        /// <summary>
        /// Lädt das Config Objekt.
        /// </summary>
        /// <returns>Config Instanz</returns>
        public static Config GetConfig()
        {
            return GetObject<Config>(Properties.Settings.Default.ConfigPath, false);
        }

        /// <summary>
        /// Initialisiert einen Watcher mit vordefinierten Eigenschaften.
        /// </summary>
        /// <returns>FileSystemWatcher Instanz</returns>
        public static FileSystemWatcher GetWatcher()
        {
            var fsWatcher = new FileSystemWatcher(Config.StoragePath)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                EnableRaisingEvents = true
            };

            fsWatcher.Created += WatcherEventHandler.Created;
            fsWatcher.Renamed += WatcherEventHandler.Renamed;
            fsWatcher.Changed += WatcherEventHandler.Changed;
            fsWatcher.Deleted += WatcherEventHandler.Deleted;

            return fsWatcher;
        }

        /// <summary>
        /// Lädt die Liste aller Kategorien.
        /// </summary>
        /// <returns>Liste aller Kategorien</returns>
        public static List<string> GetCategories()
        {
            return GetObject<List<string>>(Config.CategoriesPath);
        }

        /// <summary>
        /// Lädt die Liste aller Ordner.
        /// </summary>
        /// <returns>Liste aller Ordner</returns>
        public static List<Folder> GetFolders()
        {
            return GetObject<List<Folder>>(Config.FoldersPath);
        }

        /// <summary>
        /// Lädt die Liste aller Besetzungen.
        /// </summary>
        /// <returns>Liste aller Besetzungen</returns>
        public static List<Instrumentation> GetInstrumentations()
        {
            return GetObject<List<Instrumentation>>(Config.FoldersPath);
        }

        /// <summary>
        /// Lädt ein Meta Objekt.
        /// </summary>
        /// <param name="songFolder">Liedordner</param>
        /// <returns>Meta Instanz</returns>
        public static Meta GetMeta(string songFolder)
        {
            return GetObject<Meta>(String.Format(Config.MetaPath, songFolder));
        }

        /// <summary>
        /// Lädt das NamePattern Objekt.
        /// </summary>
        /// <returns>NamePattern Instanz</returns>
        public static NamePattern GetNamePattern()
        {
            return GetObject<NamePattern>(Config.NamePatternPath);
        }

        /// <summary>
        /// Lädt ein Objekt von der angegebenen Speicherstelle.
        /// </summary>
        /// <typeparam name="T">Typ des zu ladenden Objekts</typeparam>
        /// <param name="path">Relativer Pfad zum Speicherort</param>
        /// <returns>Zu ladendes Objekt</returns>
        private static T GetObject<T>(string path, bool storagePath = true)
        {
            if (storagePath)
                path = Path.Combine(Config.StoragePath, path);

            return XmlHandler.GetObject<T>(path);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt die Funktionalität zum Speichern von Objekten zur Verfügung.
    /// </summary>
    public static class Save
    {
        private static Config config;
        private static Config ConfigObj
        {
            get
            {
                if (config == null)
                    config = Notenverwaltung.Config.GetInstance();
                return config;
            }
        }

        /// <summary>
        /// Speichert das Config Objekt.
        /// </summary>
        /// <param name="config">Config Instanz</param>
        public static void Config(Config config)
        {
            Store(Properties.Settings.Default.ConfigPath, config, false);
        }

        /// <summary>
        /// Speichert die Liste aller Kategorien.
        /// </summary>
        /// <param name="categories">Liste der Kategorien</param>
        public static void Categories(IEnumerable<string> categories)
        {
            Store(ConfigObj.CategoriesPath, categories);
        }

        /// <summary>
        /// Speichert die Liste aller Ordner.
        /// </summary>
        /// <param name="folders">Liste der Ordner</param>
        public static void Folders(IEnumerable<Folder> folders)
        {
            Store(ConfigObj.FoldersPath, folders);
        }

        /// <summary>
        /// Speichert die Liste der Besetzungen.
        /// </summary>
        /// <param name="instrumentations">Liste der Besetzungen</param>
        public static void Instrumentations(IEnumerable<Instrumentation> instrumentations)
        {
            Store(ConfigObj.InstrumentationsPath, instrumentations);
        }

        /// <summary>
        /// Speichert das NamePattern Objekt.
        /// </summary>
        /// <param name="namePattern">NamePattern Instanz</param>
        public static void NamePattern(NamePattern namePattern)
        {
            Store(ConfigObj.NamePatternPath, namePattern);
        }

        /// <summary>
        /// Speichert eine Meta Instanz.
        /// </summary>
        /// <param name="meta">Meta Instanz</param>
        public static void Meta(Meta meta)
        {
            Store(String.Format(ConfigObj.MetaPath, meta.SongFolder), meta);
        }

        /// <summary>
        /// Speichert übergebenes Objekt an angegebem Pfad.
        /// </summary>
        /// <param name="path">Relativer Pfad des Speicherorts</param>
        /// <param name="source">Zu speicherndes Objekt</param>
        private static void Store(string path, object source, bool storagePath = true)
        {
            if (storagePath)
                path = Path.Combine(ConfigObj.StoragePath, path);

            XmlHandler.SaveObject(path, source);
        }
    }
}

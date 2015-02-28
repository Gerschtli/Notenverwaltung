using System;

namespace Notenverwaltung
{
    /// <summary>
    /// Statische Klasse zum Verwalten von Einstellungsdaten.
    /// </summary>
    public static class Config
    {
        private static string ConfigPath = @"..\..\config.xml";

        private static string _StoragePath;
        public static string StoragePath
        {
            get
            {
                if (_StoragePath == null)
                    Initialize();

                return _StoragePath;
            }
            set
            {
                _StoragePath = value.TrimEnd('\\');

                var config = new ConfigSerializable()
                {
                    StoragePath = _StoragePath
                };

                XmlHandler.SaveObject(ConfigPath, config);
            }   
        }

        /// <summary>
        /// Instanziert die Klassenvariablen.
        /// </summary>
        private static void Initialize()
        {
            StoragePath = XmlHandler.GetObject<ConfigSerializable>(ConfigPath).StoragePath;
        }
    }

    /// <summary>
    /// Instanzierbares Abbild der statischen Klasse. Wird für die Speicherung benötigt.
    /// </summary>
    public class ConfigSerializable
    {
        public string StoragePath;
    }
}

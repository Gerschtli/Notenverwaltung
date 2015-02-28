using System;

namespace Notenverwaltung
{
    /// <summary>
    /// Statische Klasse zum Verwalten von Einstellungsdaten.
    /// </summary>
    public static class Config
    {
        private static readonly string _Path = @"..\..\config.xml";

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

                XmlHandler.SaveObject(_Path, config);
            }   
        }

        /// <summary>
        /// Initialisiert die Klassenvariablen.
        /// </summary>
        private static void Initialize()
        {
            StoragePath = XmlHandler.GetObject<ConfigSerializable>(_Path).StoragePath;
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

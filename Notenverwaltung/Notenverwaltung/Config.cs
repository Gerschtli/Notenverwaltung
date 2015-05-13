namespace Notenverwaltung
{
    /// <summary>
    /// Klasse zum Verwalten von Einstellungsdaten.
    /// </summary>
    public class Config
    {
        private static Config instance;

        private string storagePath = "";
        public string StoragePath
        {
            get { return storagePath; }
            set
            {
                if (storagePath != "" && value != storagePath)
                    PropertyChanged();

                storagePath = value;
            }
        }

        private string categoriesPath = "";
        public string CategoriesPath
        {
            get { return categoriesPath; }
            set
            {
                if (categoriesPath != "" && value != categoriesPath)
                    PropertyChanged();

                categoriesPath = value;
            }
        }

        private string foldersPath = "";
        public string FoldersPath
        {
            get { return foldersPath; }
            set
            {
                if (foldersPath != "" && value != foldersPath)
                    PropertyChanged();

                foldersPath = value;
            }
        }

        private string instrumentationsPath = "";
        public string InstrumentationsPath
        {
            get { return instrumentationsPath; }
            set
            {
                if (instrumentationsPath != "" && value != instrumentationsPath)
                    PropertyChanged();

                instrumentationsPath = value;
            }
        }

        private string namePatternPath = "";
        public string NamePatternPath
        {
            get { return namePatternPath; }
            set
            {
                if (namePatternPath != "" && value != namePatternPath)
                    PropertyChanged();

                namePatternPath = value;
            }
        }

        private string metaPath = "";
        public string MetaPath
        {
            get { return metaPath; }
            set
            {
                if (value.StartsWith("{0}\\")) // Platzhalter für Ordernamen
                {
                    if (metaPath != "" && value != metaPath)
                        PropertyChanged();

                    metaPath = value;
                }
            }
        }

        public static Config GetInstance()
        {
            if (instance == null)
                instance = Factory.GetConfig();

            return instance;
        }

        private void PropertyChanged()
        {
            Save.Config(this);
        }
    }
}

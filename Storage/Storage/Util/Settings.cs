using AppSettings = Storage.Properties.Settings;

namespace Storage.Util
{
    public class Settings : ISettings
    {
        #region Properties

        public string DataFilename
        {
            get { return AppSettings.Default.DataFilename; }
        }

        public string DataPath
        {
            get { return AppSettings.Default.DataPath; }
        }

        #endregion
    }
}

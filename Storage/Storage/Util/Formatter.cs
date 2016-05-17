using System.IO.Abstractions;

namespace Storage.Util
{
    public class Formatter : IFormatter
    {
        #region Constructor

        public Formatter(IFileSystem fileSystem, ISettings settings)
        {
            this.fileSystem = fileSystem;
            this.settings = settings;
        }

        #endregion

        #region Methods

        public string StripDataPath(string path)
        {
            return path.Substring(settings.DataPath.Length).Trim(fileSystem.Path.DirectorySeparatorChar);
        }

        #endregion

        #region Fields

        private readonly IFileSystem fileSystem;
        private readonly ISettings settings;

        #endregion
    }
}

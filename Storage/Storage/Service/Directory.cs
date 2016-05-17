using Storage.IO;
using Storage.Model;
using DirectoryModel = Storage.Model.Directory;
using SongModel = Storage.Model.Song;

namespace Storage.Service
{
    public class Directory
    {
        #region Fields

        private readonly IExporter exporter;
        private readonly IImporter importer;

        #endregion

        #region Constructor

        public Directory(IExporter exporter, IImporter importer)
        {
            this.exporter = exporter;
            this.importer = importer;
        }

        #endregion

        public void ChangeStatus(DirectoryModel directory)
        {
            var directoryData = importer.Read<DirectoryData>(directory.Path)
                ?? new DirectoryData {Status = DirectoryStatus.SONG};

            if (directoryData.Song == null) {
                directoryData.Song = new SongModel {Path = directory.Path};
            }

            directoryData.Song.Name = directory.Path;

            exporter.Write(directory.Path, directoryData);
        }
    }
}

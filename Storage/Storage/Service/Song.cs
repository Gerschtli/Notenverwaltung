using System.IO.Abstractions;
using Storage.IO;
using Storage.Model;
using Storage.Util;
using SongModel = Storage.Model.Song;

namespace Storage.Service
{
    public class Song : ISong
    {
        #region Constructor

        public Song(IExporter exporter, ISettings settings, IFileSystem fileSystem)
        {
            this.exporter = exporter;
            this.settings = settings;
            this.fileSystem = fileSystem;
        }

        #endregion

        #region Methods

        public void UpdateDetails(SongModel song)
        {
            var path = fileSystem.Path.Combine(song.Path, settings.DataFilename);
            var data = new DirectoryData {Status = DirectoryStatus.SONG, Song = song};
            exporter.Write(path, data);
        }

        #endregion

        #region Fields

        private readonly IExporter exporter;
        private readonly IFileSystem fileSystem;
        private readonly ISettings settings;

        #endregion
    }
}

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Storage.IO;
using Storage.Model;
using Storage.Util;
using DirectoryModel = Storage.Model.Directory;

namespace Storage.Service.FileSystem
{
    public class Checker : IChecker
    {
        #region Constructor

        public Checker(
            IExporter exporter, IFileSystem fileSystem, IImporter importer, ISettings settings,
            IDirectories directoriesService)
        {
            this.exporter = exporter;
            this.fileSystem = fileSystem;
            this.importer = importer;
            this.settings = settings;
            this.directoriesService = directoriesService;
        }

        #endregion

        #region Events

        public event TriggerHandler Finished;

        #endregion

        #region Fields

        private readonly IExporter exporter;
        private readonly IFileSystem fileSystem;
        private readonly IImporter importer;
        private readonly ISettings settings;
        private readonly IDirectories directoriesService;

        #endregion

        #region Methods

        public void Start(string dataPath = "")
        {
            var initial = dataPath == "";

            var directories = initial
                ? GetDirectories(settings.DataPath)
                : new List<string>(GetDirectories(dataPath)) {dataPath};

            foreach (
                var path in directories.Select(
                    directory => fileSystem.Path.Combine(directory, settings.DataFilename))
                ) {
                if (fileSystem.File.Exists(path)) {
                    FileExistsHandler(path);
                } else {
                    CreateFileHandler(path);
                }
            }

            if (initial && Finished != null) {
                Finished();
            }
        }

        private void FileExistsHandler(string path)
        {
            var directoryData = importer.Read<DirectoryData>(path);

            if (directoryData == null) {
                CreateFileHandler(path);
            } else {
                switch (directoryData.Status) {
                    case DirectoryStatus.SONG:
                        if (directoryData.Song != null) {
                            AddSong(directoryData.Song, path);
                        } else {
                            AddTask(TaskType.EMPTY_SONG, path);
                        }
                        break;
                    case DirectoryStatus.UNKNOWN:
                        AddTask(TaskType.UNKNOWN, path);
                        break;
                    case DirectoryStatus.NO_SONG:
                        AddDirectory(path);
                        break;
                }
            }
        }

        private void CreateFileHandler(string path)
        {
            exporter.Write(path, new DirectoryData());
            AddTask(TaskType.UNKNOWN, path);
        }

        #endregion

        #region Helper Methods

        private IEnumerable<string> GetDirectories(string path)
        {
            return fileSystem.Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);
        }

        private void AddDirectory(string path)
        {
            directoriesService.AddDirectory(new DirectoryModel {Path = GetDirectoryName(path)});
        }

        private void AddSong(Model.Song song, string path)
        {
            song.Path = GetDirectoryName(path);
            directoriesService.AddSong(song);
        }

        private void AddTask(TaskType type, string path)
        {
            directoriesService.AddTask(new Task {Type = type, Path = GetDirectoryName(path)});
        }

        private string GetDirectoryName(string path)
        {
            return fileSystem.Path.GetDirectoryName(path);
        }

        #endregion
    }
}

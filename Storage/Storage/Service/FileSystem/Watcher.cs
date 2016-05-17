using System.IO;
using System.IO.Abstractions;
using Storage.Util;

namespace Storage.Service.FileSystem
{
    public class Watcher : IWatcher
    {
        #region Constructor

        public Watcher(
            IChecker checker, IDirectories directoriesService, ISettings settings, FileSystemWatcherBase watcher)
        {
            this.checker = checker;
            this.directoriesService = directoriesService;
            this.settings = settings;
            this.watcher = watcher;
        }

        #endregion

        #region Fields

        private readonly IChecker checker;
        private readonly IDirectories directoriesService;
        private readonly ISettings settings;
        private readonly FileSystemWatcherBase watcher;

        #endregion

        #region Methods

        public void Start()
        {
            if (!watcher.EnableRaisingEvents) {
                watcher.Path = settings.DataPath;
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.DirectoryName;

                watcher.Created += CreatedHandler;
                watcher.Renamed += RenamedHandler;
                watcher.Deleted += DeletedHandler;

                watcher.EnableRaisingEvents = true;
            }
        }

        public void Dispose()
        {
            watcher.Dispose();
        }

        private void CreatedHandler(object sender, FileSystemEventArgs e)
        {
            checker.Start(e.FullPath);
        }

        private void RenamedHandler(object sender, RenamedEventArgs e)
        {
            directoriesService.UpdatePath(e.OldFullPath, e.FullPath);
        }

        private void DeletedHandler(object sender, FileSystemEventArgs e)
        {
            directoriesService.DeleteByPath(e.FullPath);
        }

        #endregion
    }
}

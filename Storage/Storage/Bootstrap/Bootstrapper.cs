using System.IO.Abstractions;
using System.Windows;
using Storage.Service.FileSystem;
using Storage.Util;
using Storage.Util.Interface;
using Storage.ViewModel;

namespace Storage.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {
        #region Constructor

        public Bootstrapper(
            IChecker checker, IExitable exitable, IFileSystem fileSystem, IMainViewModel mainViewModel,
            IMainWindow mainWindow, IMessageBox messageBox, ISettings settings, IWatcher watcher)
        {
            this.checker = checker;
            this.exitable = exitable;
            this.fileSystem = fileSystem;
            this.mainViewModel = mainViewModel;
            this.mainWindow = mainWindow;
            this.messageBox = messageBox;
            this.settings = settings;
            this.watcher = watcher;
        }

        #endregion

        #region Fields

        private readonly IChecker checker;
        private readonly IExitable exitable;
        private readonly IFileSystem fileSystem;
        private readonly IMainViewModel mainViewModel;
        private readonly IMainWindow mainWindow;
        private readonly IMessageBox messageBox;
        private readonly ISettings settings;
        private readonly IWatcher watcher;

        #endregion

        #region Methods

        public void Start()
        {
            if (!fileSystem.Directory.Exists(settings.DataPath)) {
                Exit();
            } else {
                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();

                mainViewModel.ApplicationShutDown += watcher.Dispose;
                checker.Finished += watcher.Start;
                checker.Start();
            }
        }

        private void Exit()
        {
            var path = fileSystem.Path.GetFullPath(settings.DataPath);
            var message = string.Format("Der angegebene Pfad zum Notenverzeichnis existiert nicht.\nPfad: {0}", path);

            messageBox.Show(message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            exitable.Shutdown();
        }

        #endregion
    }
}

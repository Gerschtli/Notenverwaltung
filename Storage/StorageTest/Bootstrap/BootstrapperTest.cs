using System.IO.Abstractions;
using System.Windows;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Bootstrap;
using Storage.Service.FileSystem;
using Storage.Util;
using Storage.Util.Interface;
using Storage.ViewModel;

namespace StorageTest.Bootstrap
{
    [TestClass]
    public class BootstrapperTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var watcher = mock.Mock<IWatcher>();
                watcher.Setup(m => m.Dispose());

                var bootstrapper = mock.Create<Bootstrapper>();

                Assert.IsInstanceOfType(bootstrapper, typeof (Bootstrapper));
                Assert.IsInstanceOfType(bootstrapper, typeof (IBootstrapper));
            }
        }

        [TestMethod]
        public void TestStart()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("ordner");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.Setup(m => m.Directory.Exists("ordner")).Returns(true);

                var mainViewModel = mock.Mock<IMainViewModel>();

                var mainWindow = mock.Mock<IMainWindow>();
                mainWindow.SetupSet(m => m.DataContext = mainViewModel.Object);
                mainWindow.Setup(m => m.Show());

                var checker = mock.Mock<IChecker>();
                checker.Setup(m => m.Start(""));

                var watcher = mock.Mock<IWatcher>();
                watcher.Setup(m => m.Dispose());

                var bootstrapper = mock.Create<Bootstrapper>();

                bootstrapper.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystem.Verify(m => m.Directory.Exists("ordner"), Times.Once);
                mainWindow.VerifySet(m => m.DataContext = mainViewModel.Object, Times.Once);
                mainWindow.Verify(m => m.Show(), Times.Once);
                checker.Verify(m => m.Start(""), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithRaisingFinishedEvent()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("ordner");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.Setup(m => m.Directory.Exists("ordner")).Returns(true);

                var mainViewModel = mock.Mock<IMainViewModel>();

                var mainWindow = mock.Mock<IMainWindow>();
                mainWindow.SetupSet(m => m.DataContext = mainViewModel.Object);
                mainWindow.Setup(m => m.Show());

                var checker = mock.Mock<IChecker>();
                checker.Setup(m => m.Start(""));

                var watcher = mock.Mock<IWatcher>();
                watcher.Setup(m => m.Start());
                watcher.Setup(m => m.Dispose());

                var bootstrapper = mock.Create<Bootstrapper>();

                bootstrapper.Start();

                checker.Raise(m => m.Finished += null);

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystem.Verify(m => m.Directory.Exists("ordner"), Times.Once);
                mainWindow.VerifySet(m => m.DataContext = mainViewModel.Object, Times.Once);
                mainWindow.Verify(m => m.Show(), Times.Once);
                checker.Verify(m => m.Start(""), Times.Once);
                watcher.Verify(m => m.Start(), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithRaisingApplicationShutDownEvent()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("ordner");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.Setup(m => m.Directory.Exists("ordner")).Returns(true);

                var mainViewModel = mock.Mock<IMainViewModel>();

                var mainWindow = mock.Mock<IMainWindow>();
                mainWindow.SetupSet(m => m.DataContext = mainViewModel.Object);
                mainWindow.Setup(m => m.Show());

                var checker = mock.Mock<IChecker>();
                checker.Setup(m => m.Start(""));

                var watcher = mock.Mock<IWatcher>();
                watcher.Setup(m => m.Dispose());

                var bootstrapper = mock.Create<Bootstrapper>();

                bootstrapper.Start();

                mainViewModel.Raise(m => m.ApplicationShutDown += null);

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystem.Verify(m => m.Directory.Exists("ordner"), Times.Once);
                mainWindow.VerifySet(m => m.DataContext = mainViewModel.Object, Times.Once);
                mainWindow.Verify(m => m.Show(), Times.Once);
                checker.Verify(m => m.Start(""), Times.Once);
                watcher.Verify(m => m.Dispose(), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithError()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("ordner");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.Setup(m => m.Directory.Exists("ordner")).Returns(false);
                fileSystem.Setup(m => m.Path.GetFullPath("ordner")).Returns("fullpath");

                var watcher = mock.Mock<IWatcher>();
                watcher.Setup(m => m.Dispose());

                var messageBox = mock.Mock<IMessageBox>();
                messageBox.Setup(
                    m =>
                        m.Show(
                            "Der angegebene Pfad zum Notenverzeichnis existiert nicht.\nPfad: fullpath", "Fehler",
                            MessageBoxButton.OK, MessageBoxImage.Error)
                    ).Returns(null);

                var exitable = mock.Mock<IExitable>();
                exitable.Setup(m => m.Shutdown());

                var bootstrapper = mock.Create<Bootstrapper>();

                bootstrapper.Start();

                settings.VerifyGet(m => m.DataPath, Times.Exactly(2));
                fileSystem.Verify(m => m.Directory.Exists("ordner"), Times.Once);
                fileSystem.Verify(m => m.Path.GetFullPath("ordner"), Times.Once);
                messageBox.Verify(
                    m =>
                        m.Show(
                            "Der angegebene Pfad zum Notenverzeichnis existiert nicht.\nPfad: fullpath", "Fehler",
                            MessageBoxButton.OK, MessageBoxImage.Error), Times.Once);
                exitable.Verify(m => m.Shutdown(), Times.Once);
            }
        }
    }
}

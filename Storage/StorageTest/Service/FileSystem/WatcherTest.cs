using System;
using System.IO;
using System.IO.Abstractions;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Service;
using Storage.Service.FileSystem;
using Storage.Util;

namespace StorageTest.Service.FileSystem
{
    [TestClass]
    public class WatcherTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetLoose()) {
                var watcher = mock.Create<Watcher>();

                Assert.IsInstanceOfType(watcher, typeof (Watcher));
                Assert.IsInstanceOfType(watcher, typeof (IWatcher));
                Assert.IsInstanceOfType(watcher, typeof (IDisposable));
            }
        }

        [TestMethod]
        public void TestStart()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("DataPath");

                var fileSystemWatcherBase = mock.Mock<FileSystemWatcherBase>();
                fileSystemWatcherBase.SetupGet(m => m.EnableRaisingEvents).Returns(false);
                fileSystemWatcherBase.SetupSet(m => m.Path = "DataPath");
                fileSystemWatcherBase.SetupSet(m => m.IncludeSubdirectories = true);
                fileSystemWatcherBase.SetupSet(m => m.NotifyFilter = NotifyFilters.DirectoryName);
                fileSystemWatcherBase.SetupSet(m => m.EnableRaisingEvents = true);
                fileSystemWatcherBase.Setup(m => m.Dispose());

                var watcher = mock.Create<Watcher>();

                watcher.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystemWatcherBase.VerifyGet(m => m.EnableRaisingEvents, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.Path = "DataPath", Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.IncludeSubdirectories = true, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.NotifyFilter = NotifyFilters.DirectoryName, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.EnableRaisingEvents = true, Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWhenWatcherIsAlreadyActive()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystemWatcherBase = mock.Mock<FileSystemWatcherBase>();
                fileSystemWatcherBase.SetupGet(m => m.EnableRaisingEvents).Returns(true);
                fileSystemWatcherBase.Setup(m => m.Dispose());

                var watcher = mock.Create<Watcher>();

                watcher.Start();

                fileSystemWatcherBase.VerifyGet(m => m.EnableRaisingEvents, Times.Once);
            }
        }

        [TestMethod]
        public void TestStartRaisingCreatedEvent()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("DataPath");

                var fileSystemWatcherBase = mock.Mock<FileSystemWatcherBase>();
                fileSystemWatcherBase.SetupGet(m => m.EnableRaisingEvents).Returns(false);
                fileSystemWatcherBase.SetupSet(m => m.Path = "DataPath");
                fileSystemWatcherBase.SetupSet(m => m.IncludeSubdirectories = true);
                fileSystemWatcherBase.SetupSet(m => m.NotifyFilter = NotifyFilters.DirectoryName);
                fileSystemWatcherBase.SetupSet(m => m.EnableRaisingEvents = true);
                fileSystemWatcherBase.Setup(m => m.Dispose());

                var checker = mock.Mock<IChecker>();
                checker.Setup(m => m.Start(@"test\bla"));

                var watcher = mock.Create<Watcher>();

                watcher.Start();

                fileSystemWatcherBase.Raise(
                    m => m.Created += null, null,
                    new FileSystemEventArgs(WatcherChangeTypes.Created, "test", "bla"));

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystemWatcherBase.VerifyGet(m => m.EnableRaisingEvents, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.Path = "DataPath", Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.IncludeSubdirectories = true, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.NotifyFilter = NotifyFilters.DirectoryName, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.EnableRaisingEvents = true, Times.Once);
                checker.Verify(m => m.Start(@"test\bla"), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartRaisingRenamedEvent()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("DataPath");

                var fileSystemWatcherBase = mock.Mock<FileSystemWatcherBase>();
                fileSystemWatcherBase.SetupGet(m => m.EnableRaisingEvents).Returns(false);
                fileSystemWatcherBase.SetupSet(m => m.Path = "DataPath");
                fileSystemWatcherBase.SetupSet(m => m.IncludeSubdirectories = true);
                fileSystemWatcherBase.SetupSet(m => m.NotifyFilter = NotifyFilters.DirectoryName);
                fileSystemWatcherBase.SetupSet(m => m.EnableRaisingEvents = true);
                fileSystemWatcherBase.Setup(m => m.Dispose());

                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.UpdatePath(@"test\old", @"test\bla"));

                var watcher = mock.Create<Watcher>();

                watcher.Start();

                fileSystemWatcherBase.Raise(
                    m => m.Renamed += null, null,
                    new RenamedEventArgs(WatcherChangeTypes.Renamed, "test", "bla", "old"));

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystemWatcherBase.VerifyGet(m => m.EnableRaisingEvents, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.Path = "DataPath", Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.IncludeSubdirectories = true, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.NotifyFilter = NotifyFilters.DirectoryName, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.EnableRaisingEvents = true, Times.Once);
                directoriesService.Verify(m => m.UpdatePath(@"test\old", @"test\bla"), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartRaisingDeletedEvent()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("DataPath");

                var fileSystemWatcherBase = mock.Mock<FileSystemWatcherBase>();
                fileSystemWatcherBase.SetupGet(m => m.EnableRaisingEvents).Returns(false);
                fileSystemWatcherBase.SetupSet(m => m.Path = "DataPath");
                fileSystemWatcherBase.SetupSet(m => m.IncludeSubdirectories = true);
                fileSystemWatcherBase.SetupSet(m => m.NotifyFilter = NotifyFilters.DirectoryName);
                fileSystemWatcherBase.SetupSet(m => m.EnableRaisingEvents = true);
                fileSystemWatcherBase.Setup(m => m.Dispose());

                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.DeleteByPath(@"test\bla"));

                var watcher = mock.Create<Watcher>();

                watcher.Start();

                fileSystemWatcherBase.Raise(
                    m => m.Deleted += null, null,
                    new FileSystemEventArgs(WatcherChangeTypes.Created, "test", "bla"));

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystemWatcherBase.VerifyGet(m => m.EnableRaisingEvents, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.Path = "DataPath", Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.IncludeSubdirectories = true, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.NotifyFilter = NotifyFilters.DirectoryName, Times.Once);
                fileSystemWatcherBase.VerifySet(m => m.EnableRaisingEvents = true, Times.Once);
                directoriesService.Verify(m => m.DeleteByPath(@"test\bla"), Times.Once);
            }
        }

        [TestMethod]
        public void TestDispose()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystemWatcherBase = mock.Mock<FileSystemWatcherBase>();
                fileSystemWatcherBase.Setup(m => m.Dispose());

                var watcher = mock.Create<Watcher>();

                watcher.Dispose();

                fileSystemWatcherBase.Verify(m => m.Dispose(), Times.Once);
            }
        }
    }
}

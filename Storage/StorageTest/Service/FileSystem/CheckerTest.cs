using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.IO;
using Storage.Model;
using Storage.Service;
using Storage.Service.FileSystem;
using Storage.Util;
using DirectoryModel = Storage.Model.Directory;
using SongModel = Storage.Model.Song;

namespace StorageTest.Service.FileSystem
{
    [TestClass]
    public class CheckerTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var checker = mock.Create<Checker>();

                Assert.IsInstanceOfType(checker, typeof (Checker));
                Assert.IsInstanceOfType(checker, typeof (IChecker));
            }
        }

        [TestMethod]
        public void TestStart()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithoutXmlFile()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var expectedDirectoryStatus = new DirectoryData {Status = DirectoryStatus.UNKNOWN, Song = null};
                var exporter = mock.Mock<IExporter>();
                exporter.Setup(m => m.Write(@"C:\root\bla\song.xml", expectedDirectoryStatus));

                var expectedTask = new Task {Type = TaskType.UNKNOWN, Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddTask(expectedTask));

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                settings.VerifyGet(m => m.DataFilename, Times.Once);
                exporter.Verify(m => m.Write(@"C:\root\bla\song.xml", expectedDirectoryStatus), Times.Once);
                directoriesService.Verify(m => m.AddTask(expectedTask), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithInvalidXmlFile()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()},
                        {@"C:\root\bla\song.xml", new MockFileData("xml") {Attributes = FileAttributes.Hidden}}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var importer = mock.Mock<IImporter>();
                importer.Setup(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml")).Returns(null as DirectoryData);

                var expectedDirectoryStatus = new DirectoryData {Status = DirectoryStatus.UNKNOWN, Song = null};
                var exporter = mock.Mock<IExporter>();
                exporter.Setup(m => m.Write(@"C:\root\bla\song.xml", expectedDirectoryStatus));

                var expectedTask = new Task {Type = TaskType.UNKNOWN, Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddTask(expectedTask));

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                settings.VerifyGet(m => m.DataFilename, Times.Once);
                importer.Verify(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml"), Times.Once);
                exporter.Verify(m => m.Write(@"C:\root\bla\song.xml", expectedDirectoryStatus), Times.Once);
                directoriesService.Verify(m => m.AddTask(expectedTask), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithSongObject()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()},
                        {@"C:\root\bla\song.xml", new MockFileData("xml") {Attributes = FileAttributes.Hidden}}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var expectedDirectoryData = new DirectoryData {Status = DirectoryStatus.SONG, Song = new SongModel()};
                var importer = mock.Mock<IImporter>();
                importer.Setup(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml")).Returns(expectedDirectoryData);

                var expectedSong = new SongModel {Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddSong(expectedSong));

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                settings.VerifyGet(m => m.DataFilename, Times.Once);
                importer.Verify(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml"), Times.Once);
                directoriesService.Verify(m => m.AddSong(expectedSong), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithNoSongObjectAndStatusSong()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()},
                        {@"C:\root\bla\song.xml", new MockFileData("xml") {Attributes = FileAttributes.Hidden}}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var expectedDirectoryData = new DirectoryData {Status = DirectoryStatus.SONG, Song = null};
                var importer = mock.Mock<IImporter>();
                importer.Setup(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml")).Returns(expectedDirectoryData);

                var expectedTask = new Task {Type = TaskType.EMPTY_SONG, Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddTask(expectedTask));

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                settings.VerifyGet(m => m.DataFilename, Times.Once);
                importer.Verify(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml"), Times.Once);
                directoriesService.Verify(m => m.AddTask(expectedTask), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithStatusUnknownType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()},
                        {@"C:\root\bla\song.xml", new MockFileData("xml") {Attributes = FileAttributes.Hidden}}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var expectedDirectoryData = new DirectoryData {Status = DirectoryStatus.UNKNOWN};
                var importer = mock.Mock<IImporter>();
                importer.Setup(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml")).Returns(expectedDirectoryData);

                var expectedTask = new Task {Type = TaskType.UNKNOWN, Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddTask(expectedTask));

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                settings.VerifyGet(m => m.DataFilename, Times.Once);
                importer.Verify(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml"), Times.Once);
                directoriesService.Verify(m => m.AddTask(expectedTask), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithStatusNoSong()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()},
                        {@"C:\root\bla\song.xml", new MockFileData("xml") {Attributes = FileAttributes.Hidden}}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var expectedDirectoryData = new DirectoryData {Status = DirectoryStatus.NO_SONG};
                var importer = mock.Mock<IImporter>();
                importer.Setup(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml")).Returns(expectedDirectoryData);

                var expectedDirectory = new DirectoryModel {Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddDirectory(expectedDirectory));

                var checker = mock.Create<Checker>();

                checker.Start();

                settings.VerifyGet(m => m.DataPath, Times.Once);
                settings.VerifyGet(m => m.DataFilename, Times.Once);
                importer.Verify(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml"), Times.Once);
                directoriesService.Verify(m => m.AddDirectory(expectedDirectory), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithSubFolderWithParameter()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()},
                        {@"C:\root\bla\", new MockDirectoryData()},
                        {@"C:\root\bla\song.xml", new MockFileData("xml") {Attributes = FileAttributes.Hidden}}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.Setup(m => m.DataFilename).Returns("song.xml");

                var expectedDirectoryData = new DirectoryData {Status = DirectoryStatus.UNKNOWN};
                var importer = mock.Mock<IImporter>();
                importer.Setup(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml")).Returns(expectedDirectoryData);

                var expectedTask = new Task {Type = TaskType.UNKNOWN, Path = @"C:\root\bla"};
                var directoriesService = mock.Mock<IDirectories>();
                directoriesService.Setup(m => m.AddTask(expectedTask));

                var checker = mock.Create<Checker>();

                checker.Start(@"C:\root\bla");

                settings.VerifyGet(m => m.DataFilename, Times.Once);
                importer.Verify(m => m.Read<DirectoryData>(@"C:\root\bla\song.xml"), Times.Once);
                directoriesService.Verify(m => m.AddTask(expectedTask), Times.Once);
            }
        }

        [TestMethod]
        public void TestStartWithFinishedEvent()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"C:\root\", new MockDirectoryData()}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"C:\root");

                var checker = mock.Create<Checker>();

                var called = false;
                checker.Finished += () => called = true;

                checker.Start();

                Assert.IsTrue(called);

                settings.VerifyGet(m => m.DataPath, Times.Once);
            }
        }
    }
}

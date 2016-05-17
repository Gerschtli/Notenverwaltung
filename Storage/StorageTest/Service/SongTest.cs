using System.IO.Abstractions;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.IO;
using Storage.Model;
using Storage.Service;
using Storage.Util;
using Song = Storage.Service.Song;
using SongModel = Storage.Model.Song;

namespace StorageTest.Service
{
    [TestClass]
    public class SongTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var songService = mock.Create<Song>();

                Assert.IsInstanceOfType(songService, typeof (Song));
                Assert.IsInstanceOfType(songService, typeof (ISong));
            }
        }

        [TestMethod]
        public void TestUpdateDetails()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataFilename).Returns("song.xml");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.Setup(m => m.Path.Combine(@"C:\test\bla", "song.xml")).Returns(@"C:\test\bla\song.xml");

                var song = new SongModel {Path = @"C:\test\bla"};
                var data = new DirectoryData {Status = DirectoryStatus.SONG, Song = song};

                var exporter = mock.Mock<IExporter>();
                exporter.Setup(m => m.Write(@"C:\test\bla\song.xml", data));

                var songService = mock.Create<Song>();

                songService.UpdateDetails(song);

                settings.VerifyGet(m => m.DataFilename, Times.Once);
                fileSystem.Verify(m => m.Path.Combine(@"C:\test\bla", "song.xml"), Times.Once);
                exporter.Verify(m => m.Write(@"C:\test\bla\song.xml", data), Times.Once);
            }
        }
    }
}

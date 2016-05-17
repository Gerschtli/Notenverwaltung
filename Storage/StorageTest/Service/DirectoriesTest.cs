using System;
using System.Collections.ObjectModel;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Model;
using Storage.Service;
using Storage.Util;
using Storage.Util.Interface;
using Storage.ViewModel;
using SongModel = Storage.Model.Song;
using DirectoryModel = Storage.Model.Directory;

namespace StorageTest.Service
{
    [TestClass]
    public class DirectoriesTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var directoriesService = mock.Create<Directories>();

                Assert.IsInstanceOfType(directoriesService, typeof (Directories));
                Assert.IsInstanceOfType(directoriesService, typeof (IDirectories));
            }
        }

        [TestMethod]
        public void TestAddDirectory()
        {
            using (var mock = AutoMock.GetStrict()) {
                var collection = new ObservableCollection<IDirectoryListItem>();
                var directory = new DirectoryModel {Path = "Lied"};

                var directoryViewModel = mock.Mock<IDirectoryViewModel>();
                directoryViewModel.SetupSet(m => m.Directory = directory);

                var dispatcher = mock.Mock<IDispatcher>();
                dispatcher.Setup(m => m.Invoke(It.IsAny<Action>())).Callback<Action>(action => action());

                var model = mock.Mock<IDirectoryCollectionProperty>();
                model.SetupGet(m => m.Directories).Returns(collection);

                var directoriesService = mock.Create<Directories>();

                directoriesService.AddDirectory(directory);

                Assert.AreEqual(1, collection.Count);
                Assert.IsTrue(collection.Contains(directoryViewModel.Object));

                directoryViewModel.VerifySet(m => m.Directory = directory, Times.Once);
                dispatcher.Verify(m => m.Invoke(It.IsAny<Action>()), Times.Once);
                model.VerifyGet(m => m.Directories, Times.Once);
            }
        }

        [TestMethod]
        public void TestAddSong()
        {
            using (var mock = AutoMock.GetStrict()) {
                var collection = new ObservableCollection<IDirectoryListItem>();
                var song = new SongModel {Name = "Lied"};

                var songViewModel = mock.Mock<ISongViewModel>();
                songViewModel.SetupSet(m => m.Song = song);

                var dispatcher = mock.Mock<IDispatcher>();
                dispatcher.Setup(m => m.Invoke(It.IsAny<Action>())).Callback<Action>(action => action());

                var model = mock.Mock<IDirectoryCollectionProperty>();
                model.SetupGet(m => m.Directories).Returns(collection);

                var directoriesService = mock.Create<Directories>();

                directoriesService.AddSong(song);

                Assert.AreEqual(1, collection.Count);
                Assert.IsTrue(collection.Contains(songViewModel.Object));

                songViewModel.VerifySet(m => m.Song = song, Times.Once);
                dispatcher.Verify(m => m.Invoke(It.IsAny<Action>()), Times.Once);
                model.VerifyGet(m => m.Directories, Times.Once);
            }
        }

        [TestMethod]
        public void TestAddTask()
        {
            using (var mock = AutoMock.GetStrict()) {
                var collection = new ObservableCollection<IDirectoryListItem>();
                var task = new Task {Path = "Lied"};

                var taskViewModel = mock.Mock<ITaskViewModel>();
                taskViewModel.SetupSet(m => m.Task = task);

                var dispatcher = mock.Mock<IDispatcher>();
                dispatcher.Setup(m => m.Invoke(It.IsAny<Action>())).Callback<Action>(action => action());

                var model = mock.Mock<IDirectoryCollectionProperty>();
                model.SetupGet(m => m.Directories).Returns(collection);

                var directoriesService = mock.Create<Directories>();

                directoriesService.AddTask(task);

                Assert.AreEqual(1, collection.Count);
                Assert.IsTrue(collection.Contains(taskViewModel.Object));

                taskViewModel.VerifySet(m => m.Task = task, Times.Once);
                dispatcher.Verify(m => m.Invoke(It.IsAny<Action>()), Times.Once);
                model.VerifyGet(m => m.Directories, Times.Once);
            }
        }

        [TestMethod]
        public void TestUpdatePath()
        {
            using (var mock = AutoMock.GetStrict()) {
                using (var mock2 = AutoMock.GetStrict()) {
                    using (var mock3 = AutoMock.GetStrict()) {
                        var songViewModel1 = mock.Mock<ISongViewModel>();
                        songViewModel1.SetupGet(m => m.Path).Returns("bla");
                        songViewModel1.SetupSet(m => m.Path = "blub");

                        var songViewModel2 = mock2.Mock<ISongViewModel>();
                        songViewModel2.SetupGet(m => m.Path).Returns(@"bla\test");
                        songViewModel2.SetupSet(m => m.Path = @"blub\test");

                        var songViewModel3 = mock3.Mock<ISongViewModel>();
                        songViewModel3.SetupGet(m => m.Path).Returns(@"x\bla\test");

                        var collection = new ObservableCollection<IDirectoryListItem> {
                            songViewModel1.Object,
                            songViewModel2.Object,
                            songViewModel3.Object
                        };

                        var dispatcher = mock.Mock<IDispatcher>();
                        dispatcher.Setup(m => m.Invoke(It.IsAny<Action>())).Callback<Action>(action => action());

                        var model = mock.Mock<IDirectoryCollectionProperty>();
                        model.SetupGet(m => m.Directories).Returns(collection);

                        var directoriesService = mock.Create<Directories>();

                        directoriesService.UpdatePath("bla", "blub");

                        songViewModel1.VerifyGet(m => m.Path, Times.Exactly(2));
                        songViewModel1.VerifySet(m => m.Path = "blub", Times.Once);
                        songViewModel2.VerifyGet(m => m.Path, Times.Exactly(2));
                        songViewModel2.VerifySet(m => m.Path = @"blub\test", Times.Once);
                        songViewModel3.VerifyGet(m => m.Path, Times.Once);
                        dispatcher.Verify(m => m.Invoke(It.IsAny<Action>()), Times.Once);
                        model.VerifyGet(m => m.Directories, Times.Once);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDeleteByPath()
        {
            using (var mock = AutoMock.GetStrict()) {
                using (var mock2 = AutoMock.GetStrict()) {
                    using (var mock3 = AutoMock.GetStrict()) {
                        var songViewModel1 = mock.Mock<ISongViewModel>();
                        songViewModel1.SetupGet(m => m.Path).Returns("bla");

                        var songViewModel2 = mock2.Mock<ISongViewModel>();
                        songViewModel2.SetupGet(m => m.Path).Returns(@"bla\test");

                        var songViewModel3 = mock3.Mock<ISongViewModel>();
                        songViewModel3.SetupGet(m => m.Path).Returns(@"x\bla\test");

                        var collection = new ObservableCollection<IDirectoryListItem> {
                            songViewModel1.Object,
                            songViewModel2.Object,
                            songViewModel3.Object
                        };

                        var dispatcher = mock.Mock<IDispatcher>();
                        dispatcher.Setup(m => m.Invoke(It.IsAny<Action>())).Callback<Action>(action => action());

                        var model = mock.Mock<IDirectoryCollectionProperty>();
                        model.SetupGet(m => m.Directories).Returns(collection);

                        var directoriesService = mock.Create<Directories>();

                        directoriesService.DeleteByPath("bla");

                        Assert.AreEqual(1, collection.Count);
                        Assert.IsFalse(collection.Contains(songViewModel1.Object));
                        Assert.IsFalse(collection.Contains(songViewModel2.Object));
                        Assert.IsTrue(collection.Contains(songViewModel3.Object));

                        songViewModel1.VerifyGet(m => m.Path, Times.Once);
                        songViewModel2.VerifyGet(m => m.Path, Times.Once);
                        songViewModel3.VerifyGet(m => m.Path, Times.Once);
                        dispatcher.Verify(m => m.Invoke(It.IsAny<Action>()), Times.Once);
                        model.VerifyGet(m => m.Directories, Times.Exactly(3));
                    }
                }
            }
        }
    }
}

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Autofac.Extras.Moq;
using Autofac.Features.Indexed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Helper;
using Storage.Model;
using Storage.Util.Interface;
using Storage.View;
using Storage.ViewModel;
using StorageTest.Test;

namespace StorageTest.ViewModel
{
    [TestClass]
    public class MainViewModelTest : ViewModelTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();

                Assert.IsInstanceOfType(viewModel, typeof (MainViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (BaseViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (IMainViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (IDirectoryCollectionProperty));
            }
        }

        [TestMethod]
        public void TestPropertySelectedDirectory()
        {
            TestViewModelProperties<MainViewModel>(
                "SelectedDirectory", viewModel =>
                {
                    var item = new TaskViewModel(null);
                    viewModel.SelectedDirectory = item;

                    Assert.AreEqual(item, viewModel.SelectedDirectory);
                });
        }

        [TestMethod]
        public void TestPropertyCurrentPage()
        {
            TestViewModelProperties<MainViewModel>(
                "CurrentPage", viewModel =>
                {
                    var item = new DefaultView();
                    viewModel.CurrentPage = item;

                    Assert.AreEqual(item, viewModel.CurrentPage);
                });
        }

        [TestMethod]
        [Ignore]
        public void TestPropertyCurrentPageWithDefault()
        {
            using (var mock = AutoMock.GetStrict()) {
                var userControl = mock.Mock<IUserControl>();

                var index = mock.Mock<IIndex<ViewType, IUserControl>>();
                index.Setup(m => m[ViewType.DEFAULT]).Returns(userControl.Object);

                var viewModel = mock.Create<MainViewModel>();

                Assert.AreEqual(userControl.Object, viewModel.CurrentPage);

                index.Verify(m => m[ViewType.DEFAULT], Times.Once);
            }
        }

        [TestMethod]
        public void TestPropertyDirectories()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();

                Assert.IsInstanceOfType(viewModel.Directories, typeof (ObservableCollection<IDirectoryListItem>));
                Assert.AreEqual(0, viewModel.Directories.Count);
            }
        }

        [TestMethod]
        public void TestDirectoriesSongs()
        {
            using (var mock = AutoMock.GetStrict()) {
                var item = mock.Mock<IDirectoryListItem>();
                item.SetupGet(m => m.Type).Returns(DirectoryStatus.SONG);

                var viewModel = mock.Create<MainViewModel>();
                var directoriesSongs = viewModel.DirectoriesSongs;
                var filter = directoriesSongs.Filter;

                Assert.IsTrue(filter(item.Object));

                item.VerifyGet(m => m.Type, Times.Once);
            }
        }

        [TestMethod]
        public void TestDirectoriesSongsWhenTask()
        {
            using (var mock = AutoMock.GetStrict()) {
                var item = mock.Mock<IDirectoryListItem>();
                item.SetupGet(m => m.Type).Returns(DirectoryStatus.UNKNOWN);

                var viewModel = mock.Create<MainViewModel>();
                var directoriesSongs = viewModel.DirectoriesSongs;
                var filter = directoriesSongs.Filter;

                Assert.IsFalse(filter(item.Object));

                item.VerifyGet(m => m.Type, Times.Once);
            }
        }

        [TestMethod]
        public void TestDirectoriesSongsWhenNull()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var directoriesSongs = viewModel.DirectoriesSongs;
                var filter = directoriesSongs.Filter;

                Assert.IsFalse(filter(null));
            }
        }

        [TestMethod]
        public void TestDirectoriesSongsWhenOtherType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var directoriesSongs = viewModel.DirectoriesSongs;
                var filter = directoriesSongs.Filter;

                Assert.IsFalse(filter(5));
            }
        }

        [TestMethod]
        public void TestDirectoriesTasks()
        {
            using (var mock = AutoMock.GetStrict()) {
                var item = mock.Mock<IDirectoryListItem>();
                item.SetupGet(m => m.Type).Returns(DirectoryStatus.UNKNOWN);

                var viewModel = mock.Create<MainViewModel>();
                var directoriesTasks = viewModel.DirectoriesTasks;
                var filter = directoriesTasks.Filter;

                Assert.IsTrue(filter(item.Object));

                item.VerifyGet(m => m.Type, Times.Once);
            }
        }

        [TestMethod]
        public void TestDirectoriesTasksWhenSong()
        {
            using (var mock = AutoMock.GetStrict()) {
                var item = mock.Mock<IDirectoryListItem>();
                item.SetupGet(m => m.Type).Returns(DirectoryStatus.SONG);

                var viewModel = mock.Create<MainViewModel>();
                var directoriesTasks = viewModel.DirectoriesTasks;
                var filter = directoriesTasks.Filter;

                Assert.IsFalse(filter(item.Object));

                item.VerifyGet(m => m.Type, Times.Once);
            }
        }

        [TestMethod]
        public void TestDirectoriesTasksWhenNull()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var directoriesTasks = viewModel.DirectoriesTasks;
                var filter = directoriesTasks.Filter;

                Assert.IsFalse(filter(null));
            }
        }

        [TestMethod]
        public void TestDirectoriesTasksWhenOtherType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var directoriesTasks = viewModel.DirectoriesTasks;
                var filter = directoriesTasks.Filter;

                Assert.IsFalse(filter(5));
            }
        }

        [TestMethod]
        public void TestDirectoriesTasksGrouping()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var directoriesTasks = viewModel.DirectoriesTasks;

                Assert.IsTrue(
                    directoriesTasks.GroupDescriptions.Any(
                        m =>
                        {
                            var propertyGroupDescription = m as PropertyGroupDescription;
                            return propertyGroupDescription != null && propertyGroupDescription.PropertyName == "Type";
                        }));
            }
        }

        [TestMethod]
        public void TestSelectDirectory()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var selectDirectory = viewModel.SelectDirectory;

                Assert.IsTrue(selectDirectory.CanExecute(null));
                Assert.IsInstanceOfType(selectDirectory, typeof (RelayCommand<IDirectoryListItem>));
            }
        }

        [TestMethod]
        public void TestWindowClosing()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<MainViewModel>();
                var saveDetails = viewModel.WindowClosing;

                Assert.IsTrue(saveDetails.CanExecute(null));
                Assert.IsInstanceOfType(saveDetails, typeof (RelayCommand));
            }
        }

        [TestMethod]
        public void TestWindowClosingExecute()
        {
            using (var mock = AutoMock.GetStrict()) {
                var called = false;

                var viewModel = mock.Create<MainViewModel>();

                viewModel.ApplicationShutDown += () => called = true;

                var saveDetails = viewModel.WindowClosing;

                viewModel.WindowClosing.Execute(null);

                Assert.IsInstanceOfType(saveDetails, typeof (RelayCommand));
                Assert.IsTrue(called);
            }
        }
    }
}

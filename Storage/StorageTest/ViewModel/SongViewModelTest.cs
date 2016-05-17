using System.Collections.Generic;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Helper;
using Storage.Model;
using Storage.Service;
using Storage.View;
using Storage.ViewModel;
using StorageTest.Test;
using Song = Storage.Model.Song;

namespace StorageTest.ViewModel
{
    [TestClass]
    public class SongViewModelTest : ViewModelTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<SongViewModel>();

                Assert.IsInstanceOfType(viewModel, typeof (SongViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (BaseViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (ISongViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (IDirectoryListItem));
            }
        }

        [TestMethod]
        public void TestPropertyType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<SongViewModel>();

                Assert.AreEqual(DirectoryStatus.SONG, viewModel.Type);
            }
        }

        [TestMethod]
        public void TestPropertyViewType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<SongViewModel>();

                Assert.AreEqual(ViewType.SONG, viewModel.ViewType);
            }
        }

        [TestMethod]
        public void TestPropertyDisplayText()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<SongViewModel>();

                viewModel.Song = new Song {Name = "blaa"};

                Assert.AreEqual("blaa", viewModel.DisplayText);
            }
        }

        [TestMethod]
        public void TestPropertyPath()
        {
            TestViewModelProperties<SongViewModel>(
                "Path", viewModel =>
                {
                    viewModel.Song = new Song {Path = "name"};
                    viewModel.Path = "neu";

                    Assert.AreEqual("neu", viewModel.Path);
                });
        }

        [TestMethod]
        public void TestPropertyName()
        {
            TestViewModelProperties<SongViewModel>(
                "Name", "DisplayText", viewModel =>
                {
                    viewModel.Song = new Song {Name = "name"};
                    viewModel.Name = "neu";

                    Assert.AreEqual("neu", viewModel.Name);
                });
        }

        [TestMethod]
        public void TestPropertyArranger()
        {
            TestViewModelProperties<SongViewModel>(
                "Arranger", viewModel =>
                {
                    viewModel.Song = new Song {Arranger = "name"};
                    viewModel.Arranger = "neu";

                    Assert.AreEqual("neu", viewModel.Arranger);
                });
        }

        [TestMethod]
        public void TestPropertyComposer()
        {
            TestViewModelProperties<SongViewModel>(
                "Composer", viewModel =>
                {
                    viewModel.Song = new Song {Composer = "name"};
                    viewModel.Composer = "neu";

                    Assert.AreEqual("neu", viewModel.Composer);
                });
        }

        [TestMethod]
        public void TestPropertyCategories()
        {
            TestViewModelProperties<SongViewModel>(
                "Categories", viewModel =>
                {
                    var setOld = new SortedSet<Category>();
                    var setNew = new SortedSet<Category>();

                    viewModel.Song = new Song {Categories = setOld};
                    viewModel.Categories = setNew;

                    Assert.AreEqual(setNew, viewModel.Categories);
                    Assert.AreNotEqual(setOld, viewModel.Categories);
                });
        }

        [TestMethod]
        public void TestSaveDetails()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<SongViewModel>();
                var saveDetails = viewModel.SaveDetails;

                Assert.IsTrue(saveDetails.CanExecute(null));
                Assert.IsInstanceOfType(saveDetails, typeof (RelayCommand));
            }
        }

        [TestMethod]
        public void TestSaveDetailsExecute()
        {
            using (var mock = AutoMock.GetStrict()) {
                var song = new Song {Name = "test"};

                var songService = mock.Mock<ISong>();
                songService.Setup(m => m.UpdateDetails(song));

                var viewModel = mock.Create<SongViewModel>();
                var saveDetails = viewModel.SaveDetails;

                viewModel.Song = song;
                viewModel.SaveDetails.Execute(null);

                Assert.IsInstanceOfType(saveDetails, typeof (RelayCommand));

                songService.Verify(m => m.UpdateDetails(song), Times.Once);
            }
        }
    }
}

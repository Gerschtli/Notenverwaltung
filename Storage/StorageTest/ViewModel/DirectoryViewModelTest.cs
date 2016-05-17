using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Model;
using Storage.Util;
using Storage.View;
using Storage.ViewModel;
using StorageTest.Test;

namespace StorageTest.ViewModel
{
    [TestClass]
    public class DirectoryViewModelTest : ViewModelTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<DirectoryViewModel>();

                Assert.IsInstanceOfType(viewModel, typeof (DirectoryViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (BaseViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (IDirectoryViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (IDirectoryListItem));
            }
        }

        [TestMethod]
        public void TestPropertyType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<DirectoryViewModel>();

                Assert.AreEqual(DirectoryStatus.NO_SONG, viewModel.Type);
            }
        }

        [TestMethod]
        public void TestPropertyViewType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<DirectoryViewModel>();

                Assert.AreEqual(ViewType.DIRECTORY, viewModel.ViewType);
            }
        }

        [TestMethod]
        public void TestPropertyDisplayText()
        {
            using (var mock = AutoMock.GetStrict()) {
                var formatter = mock.Mock<IFormatter>();
                formatter.Setup(m => m.StripDataPath(@"x\blaa")).Returns("blaa");

                var viewModel = mock.Create<DirectoryViewModel>();

                viewModel.Directory = new Directory {Path = @"x\blaa"};

                Assert.AreEqual("blaa", viewModel.DisplayText);

                formatter.Verify(m => m.StripDataPath(@"x\blaa"), Times.Once);
            }
        }

        [TestMethod]
        public void TestPropertyPath()
        {
            TestViewModelProperties<DirectoryViewModel>(
                "Path", "DisplayText", viewModel =>
                {
                    viewModel.Directory = new Directory {Path = "name"};
                    viewModel.Path = "neu";

                    Assert.AreEqual("neu", viewModel.Path);
                });
        }
    }
}

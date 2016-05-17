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
    public class TaskViewModelTest : ViewModelTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<TaskViewModel>();

                Assert.IsInstanceOfType(viewModel, typeof (TaskViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (BaseViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (ITaskViewModel));
                Assert.IsInstanceOfType(viewModel, typeof (IDirectoryListItem));
            }
        }

        [TestMethod]
        public void TestPropertyType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<TaskViewModel>();

                Assert.AreEqual(DirectoryStatus.UNKNOWN, viewModel.Type);
            }
        }

        [TestMethod]
        public void TestPropertyViewType()
        {
            using (var mock = AutoMock.GetStrict()) {
                var viewModel = mock.Create<TaskViewModel>();

                Assert.AreEqual(ViewType.TASK, viewModel.ViewType);
            }
        }

        [TestMethod]
        public void TestPropertyDisplayText()
        {
            using (var mock = AutoMock.GetStrict()) {
                var formatter = mock.Mock<IFormatter>();
                formatter.Setup(m => m.StripDataPath(@"x\blaa")).Returns("blaa");

                var viewModel = mock.Create<TaskViewModel>();

                viewModel.Task = new Task {Path = @"x\blaa"};

                Assert.AreEqual("blaa", viewModel.DisplayText);

                formatter.Verify(m => m.StripDataPath(@"x\blaa"), Times.Once);
            }
        }

        [TestMethod]
        public void TestPropertyPath()
        {
            TestViewModelProperties<TaskViewModel>(
                "Path", "DisplayText", viewModel =>
                {
                    viewModel.Task = new Task {Path = "name"};
                    viewModel.Path = "neu";

                    Assert.AreEqual("neu", viewModel.Path);
                });
        }
    }
}

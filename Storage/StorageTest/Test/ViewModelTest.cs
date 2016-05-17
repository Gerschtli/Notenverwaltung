using System;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.ViewModel;

namespace StorageTest.Test
{
    public abstract class ViewModelTest
    {
        protected void TestViewModelProperties<T>(string propertyName, Action<T> action) where T : BaseViewModel
        {
            using (var mock = AutoMock.GetStrict()) {
                var called = false;
                var viewModel = mock.Create<T>();

                viewModel.PropertyChanged += (sender, args) =>
                {
                    called = true;
                    Assert.AreEqual(viewModel, sender);
                    Assert.AreEqual(propertyName, args.PropertyName);
                };

                action(viewModel);

                Assert.IsTrue(called);
            }
        }

        protected void TestViewModelProperties<T>(string propertyName1, string propertyName2, Action<T> action)
            where T : BaseViewModel
        {
            using (var mock = AutoMock.GetStrict()) {
                var called = 0;
                var viewModel = mock.Create<T>();

                viewModel.PropertyChanged += (sender, args) =>
                {
                    called++;
                    Assert.AreEqual(viewModel, sender);
                    if (called == 1) {
                        Assert.AreEqual(propertyName1, args.PropertyName);
                    } else if (called == 2) {
                        Assert.AreEqual(propertyName2, args.PropertyName);
                    }
                };

                action(viewModel);

                Assert.AreEqual(2, called);
            }
        }
    }
}

using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using Storage.Util.Interface;

namespace StorageTest
{
    [TestClass]
    public class AppTest
    {
        [TestMethod]
        public void TestInstance()
        {
            var app = new App();

            Assert.IsInstanceOfType(app, typeof (App));
            Assert.IsInstanceOfType(app, typeof (Application));
            Assert.IsInstanceOfType(app, typeof (IDispatcherProperty));
            Assert.IsInstanceOfType(app, typeof (IExitable));
        }
    }
}

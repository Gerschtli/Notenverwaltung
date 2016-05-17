using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.Util;

namespace StorageTest.Util
{
    [TestClass]
    public class DispatcherTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var dispatcher = mock.Create<Dispatcher>();

                Assert.IsInstanceOfType(dispatcher, typeof (Dispatcher));
                Assert.IsInstanceOfType(dispatcher, typeof (IDispatcher));
            }
        }
    }
}

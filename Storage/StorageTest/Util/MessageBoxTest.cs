using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.Util;

namespace StorageTest.Util
{
    [TestClass]
    public class MessageBoxTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var messageBox = mock.Create<MessageBox>();

                Assert.IsInstanceOfType(messageBox, typeof (MessageBox));
                Assert.IsInstanceOfType(messageBox, typeof (IMessageBox));
            }
        }
    }
}

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using Storage.Util.Interface;

namespace StorageTest
{
    [TestClass]
    public class MainWindowTest
    {
        [TestMethod]
        public void TestInstance()
        {
            var interfaces = typeof (MainWindow).GetInterfaces();

            Assert.IsTrue(interfaces.Contains(typeof (IMainWindow)));
        }
    }
}

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.Util.Interface;
using Storage.View;

namespace StorageTest.View
{
    [TestClass]
    public class DirectoryViewTest
    {
        [TestMethod]
        public void TestInstance()
        {
            var interfaces = typeof (DirectoryView).GetInterfaces();

            Assert.IsTrue(interfaces.Contains(typeof (IUserControl)));
        }
    }
}

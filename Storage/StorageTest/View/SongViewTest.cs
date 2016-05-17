using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.Util.Interface;
using Storage.View;

namespace StorageTest.View
{
    [TestClass]
    public class SongViewTest
    {
        [TestMethod]
        public void TestInstance()
        {
            var interfaces = typeof (SongView).GetInterfaces();

            Assert.IsTrue(interfaces.Contains(typeof (IUserControl)));
        }
    }
}

using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.Util.Interface;
using Storage.View;

namespace StorageTest.View
{
    [TestClass]
    public class DefaultViewTest
    {
        [TestMethod]
        public void TestInstance()
        {
            var userControl = new DefaultView();

            Assert.IsInstanceOfType(userControl, typeof (DefaultView));
            Assert.IsInstanceOfType(userControl, typeof (UserControl));
            Assert.IsInstanceOfType(userControl, typeof (IUserControl));
        }
    }
}

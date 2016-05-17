using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.Util;

namespace StorageTest.Util
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Create<Settings>();

                Assert.IsInstanceOfType(settings, typeof (Settings));
                Assert.IsInstanceOfType(settings, typeof (ISettings));
            }
        }

        [TestMethod]
        public void TestProperties()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Create<Settings>();

                Assert.AreEqual(@"..\..\..\..\Externer Ordner", settings.DataPath);
                Assert.AreEqual("song.xml", settings.DataFilename);
            }
        }
    }
}

using System.IO.Abstractions;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Storage.Util;

namespace StorageTest.Util
{
    [TestClass]
    public class FormatterTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var formatter = mock.Create<Formatter>();

                Assert.IsInstanceOfType(formatter, typeof (Formatter));
                Assert.IsInstanceOfType(formatter, typeof (IFormatter));
            }
        }

        [TestMethod]
        public void TestStripDataPath()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns("abc");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.SetupGet(m => m.Path.DirectorySeparatorChar).Returns('\\');

                var formatter = mock.Create<Formatter>();

                Assert.AreEqual("test.txt", formatter.StripDataPath(@"abc\test.txt"));

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystem.VerifyGet(m => m.Path.DirectorySeparatorChar, Times.Once);
            }
        }

        [TestMethod]
        public void TestStripDataPathWithTrailingSlash()
        {
            using (var mock = AutoMock.GetStrict()) {
                var settings = mock.Mock<ISettings>();
                settings.SetupGet(m => m.DataPath).Returns(@"abc\");

                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.SetupGet(m => m.Path.DirectorySeparatorChar).Returns('\\');

                var formatter = mock.Create<Formatter>();

                Assert.AreEqual("test.txt", formatter.StripDataPath(@"abc\test.txt"));

                settings.VerifyGet(m => m.DataPath, Times.Once);
                fileSystem.VerifyGet(m => m.Path.DirectorySeparatorChar, Times.Once);
            }
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.IO;
using StorageTest.Test;

namespace StorageTest.IO
{
    [TestClass]
    public class XmlImporterTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var importer = mock.Create<XmlImporter>();

                Assert.IsInstanceOfType(importer, typeof (XmlImporter));
                Assert.IsInstanceOfType(importer, typeof (FileHandler));
                Assert.IsInstanceOfType(importer, typeof (IImporter));
            }
        }

        [TestMethod]
        public void TestRead()
        {
            using (var mock = AutoMock.GetStrict()) {
                var mockData = new MockFileData("Dummy Text") {Attributes = FileAttributes.Hidden};
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"..\..\Test\importer-valid.xml", mockData}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var importer = mock.Create<XmlImporter>();

                var data = importer.Read<Dummy>(@"..\..\Test\importer-valid.xml");
                var expected = new Dummy {TestInt = 4, TestString = "bla"};

                Assert.AreEqual(expected, data);
                Assert.IsTrue(mockData.Attributes.HasFlag(FileAttributes.Hidden));
            }
        }

        [TestMethod]
        public void TestReadWhenFileIsVisible()
        {
            using (var mock = AutoMock.GetStrict()) {
                var mockData = new MockFileData("Dummy Text") {Attributes = FileAttributes.Normal};
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"..\..\Test\importer-valid.xml", mockData}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var importer = mock.Create<XmlImporter>();

                var data = importer.Read<Dummy>(@"..\..\Test\importer-valid.xml");
                var expected = new Dummy {TestInt = 4, TestString = "bla"};

                Assert.AreEqual(expected, data);
                Assert.IsTrue(mockData.Attributes.HasFlag(FileAttributes.Hidden));
            }
        }

        [TestMethod]
        public void TestReadWhenFileIsNotFound()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem();
                mock.Provide<IFileSystem>(fileSystem);

                var importer = mock.Create<XmlImporter>();

                var data = importer.Read<Dummy>(@"..\..\Test\importer-not-existent.xml");

                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestReadWhenFileIsNotValidXml()
        {
            using (var mock = AutoMock.GetStrict()) {
                var mockData = new MockFileData("Dummy Text") {Attributes = FileAttributes.Hidden};
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"..\..\Test\importer-invalid.xml", mockData}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var importer = mock.Create<XmlImporter>();

                var data = importer.Read<Dummy>(@"..\..\Test\importer-invalid.xml");

                Assert.IsNull(data);
                Assert.IsTrue(mockData.Attributes.HasFlag(FileAttributes.Hidden));
            }
        }
    }
}

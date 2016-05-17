using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage.IO;
using StorageTest.Test;

namespace StorageTest.IO
{
    [TestClass]
    public class XmlExporterTest
    {
        [TestMethod]
        public void TestInstance()
        {
            using (var mock = AutoMock.GetStrict()) {
                var exporter = mock.Create<XmlExporter>();

                Assert.IsInstanceOfType(exporter, typeof (XmlExporter));
                Assert.IsInstanceOfType(exporter, typeof (FileHandler));
                Assert.IsInstanceOfType(exporter, typeof (IExporter));
            }
        }

        [TestMethod]
        public void TestWrite()
        {
            File.Delete(@"..\..\Test\exporter.xml");
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = new MockFileSystem();
                mock.Provide<IFileSystem>(fileSystem);

                var exporter = mock.Create<XmlExporter>();

                exporter.Write(
                    @"..\..\Test\exporter.xml",
                    new Dummy {TestInt = 4, TestString = "bla"});

                AssertSameFileContent(@"..\..\Test\exporter.xml", @"..\..\Test\exporter-expected.xml");
            }
        }

        [TestMethod]
        public void TestWriteWhenFileIsHidden()
        {
            using (var mock = AutoMock.GetStrict()) {
                var mockData = new MockFileData("Dummy Text") {Attributes = FileAttributes.Hidden};
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"..\..\Test\exporter.xml", mockData}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var exporter = mock.Create<XmlExporter>();

                exporter.Write(
                    @"..\..\Test\exporter.xml",
                    new Dummy {TestInt = 4, TestString = "bla"});

                AssertSameFileContent(@"..\..\Test\exporter.xml", @"..\..\Test\exporter-expected.xml");
                Assert.IsTrue(mockData.Attributes.HasFlag(FileAttributes.Hidden));
            }
        }

        [TestMethod]
        public void TestWriteWhenFileIsVisible()
        {
            using (var mock = AutoMock.GetStrict()) {
                var mockData = new MockFileData("Dummy Text") {Attributes = FileAttributes.Normal};
                var fileSystem = new MockFileSystem(
                    new Dictionary<string, MockFileData> {
                        {@"..\..\Test\exporter.xml", mockData}
                    });
                mock.Provide<IFileSystem>(fileSystem);

                var exporter = mock.Create<XmlExporter>();

                exporter.Write(
                    @"..\..\Test\exporter.xml",
                    new Dummy {TestInt = 4, TestString = "bla"});

                AssertSameFileContent(@"..\..\Test\exporter.xml", @"..\..\Test\exporter-expected.xml");
                Assert.IsTrue(mockData.Attributes.HasFlag(FileAttributes.Hidden));
            }
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void TestWriteWhenExceptionIsThrown()
        {
            using (var mock = AutoMock.GetStrict()) {
                var fileSystem = mock.Mock<IFileSystem>();
                fileSystem.Setup(m => m.File.GetAttributes("file.xml")).Throws(new Exception());

                var exporter = mock.Create<XmlExporter>();

                exporter.Write("file.xml", null as Dummy);
            }
        }

        private void AssertSameFileContent(string pathExpected, string pathTest)
        {
            var linesExpected = File.ReadAllLines(pathExpected);
            var linesTest = File.ReadAllLines(pathTest);

            Assert.AreEqual(0, linesTest.Except(linesExpected).Count());
            Assert.AreEqual(0, linesExpected.Except(linesTest).Count());
        }
    }
}

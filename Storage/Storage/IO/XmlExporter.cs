using System.IO;
using System.IO.Abstractions;
using System.Runtime.Serialization;
using System.Xml;

namespace Storage.IO
{
    public class XmlExporter : FileHandler, IExporter
    {
        #region Constructor

        public XmlExporter(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        #endregion

        #region Methods

        public void Write<T>(string path, T data)
        {
            SetVisible(path);

            var serializer = new DataContractSerializer(typeof (T));
            using (var streamWriter = new StreamWriter(path)) {
                using (var writer = new XmlTextWriter(streamWriter)) {
                    writer.Formatting = Formatting.Indented;
                    serializer.WriteObject(writer, data);
                    writer.Flush();
                }
            }

            SetHidden(path);
        }

        #endregion
    }
}

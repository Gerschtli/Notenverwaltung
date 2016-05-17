using System.IO;
using System.IO.Abstractions;
using System.Runtime.Serialization;
using System.Xml;

namespace Storage.IO
{
    public class XmlImporter : FileHandler, IImporter
    {
        #region Constructor

        public XmlImporter(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        #endregion

        #region Methods

        public T Read<T>(string path)
        {
            if (!fileSystem.File.Exists(path)) {
                return default(T);
            }

            SetVisible(path);

            var serializer = new DataContractSerializer(typeof (T));
            T data;
            using (var streamReader = new StreamReader(path)) {
                using (var reader = new XmlTextReader(streamReader)) {
                    try {
                        data = (T) serializer.ReadObject(reader);
                    } catch (SerializationException) {
                        data = default(T);
                    } finally {
                        reader.Close();
                    }
                }
            }

            SetHidden(path);

            return data;
        }

        #endregion
    }
}

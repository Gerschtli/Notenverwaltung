using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Notenverwaltung
{
    static class XmlHandler // todo: Einkommentieren, damit alle XML-Dateien versteckt werden.
    {

        /// <summary>
        /// Serialisiert und speichert das Objekt in der angegebenen XML Datei.
        /// </summary>
        public static void SaveObject(string path, object source)
        {
            try
            {
                //FileAttributes attributes = File.GetAttributes(path);
                //File.SetAttributes(path, attributes & ~FileAttributes.Hidden);

                XmlSerializer serializer = new XmlSerializer(source.GetType());
                StreamWriter writer = new StreamWriter(path);
                serializer.Serialize(writer, source);
                writer.Close();

                //if (!path.Contains("config"))
                //    File.SetAttributes(path, attributes | FileAttributes.Hidden);
            }
            catch (Exception e)
            {
                // todo: Message ändern
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Stellt das in der angegebenen XML Datei gespeicherte Objekt wieder her.
        /// </summary>
        public static TObject GetObject<TObject>(string path)
        {
            try
            {
                //FileAttributes attributes = File.GetAttributes(path);
                //File.SetAttributes(path, attributes & ~FileAttributes.Hidden);

                XmlSerializer serializer = new XmlSerializer(typeof(TObject));
                StreamReader reader = new StreamReader(path);
                TObject obj = (TObject)serializer.Deserialize(reader);
                reader.Close();

                //if (!path.Contains("config"))
                //    File.SetAttributes(path, attributes | FileAttributes.Hidden);

                return obj;
            }
            catch (Exception e)
            {
                // todo: Message ändern
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return default(TObject);
            }
        }

    }
}

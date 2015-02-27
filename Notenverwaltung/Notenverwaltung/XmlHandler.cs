﻿using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Notenverwaltung
{
    static class XmlHandler
    {

        /// <summary>
        /// Serialisiert und speichert das Objekt in der angegebenen XML Datei.
        /// </summary>
        public static void SaveObject(string path, object source)
        {
            try
            {
                var serializer = new XmlSerializer(source.GetType());
                var writer = new StreamWriter(path);
                serializer.Serialize(writer, source);
                writer.Close();
            }
            catch (Exception e)
            {
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
                var serializer = new XmlSerializer(typeof(TObject));
                var reader = new StreamReader(path);
                var obj = (TObject)serializer.Deserialize(reader);
                reader.Close();
                return obj;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return default(TObject);
            }
        }

    }
}

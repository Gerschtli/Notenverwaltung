using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Notenverwaltung
{
    /// <summary>
    /// Repräsentiert ein Musikstück
    /// </summary>
    public class Song
    {

        #region Instanzvariablen

        private string _SongFolder;
        public string SongFolder
        {
            get
            {
                return _SongFolder;
            }
            set
            {
                _SongFolder = value;
                MetaInfo.SongFolder = _SongFolder;
            }
        }

        public string Name;

        public string Composer;

        public string Arranger;

        public Meta MetaInfo;

        public Instrumentation ExInstrumentation = new Instrumentation();

        #endregion

        #region Konstruktor

        /// <summary>
        /// Initialisiert die Instanzvariablen
        /// </summary>
        /// <param name="songFolder">Name des Ordners</param>
        public Song(string songFolder)
        {
            WorkList.CheckFileSystem(songFolder);

            if (IsValidFolder(songFolder) && File.Exists(Config.StoragePath + songFolder + @"\Meta.xml"))
            {
                _SongFolder = songFolder;

                string[] result = songFolder.Split('\\').Last().Split('#');

                switch (result.Length)
                {
                    case 3:
                        Arranger = result[2];
                        goto case 2;
                    case 2:
                        Composer = result[1];
                        goto case 1;
                    case 1:
                        Name = result[0];
                        break;
                }

                MetaInfo = Meta.Load(songFolder);


                string[] files = Directory.GetFiles(Config.StoragePath + songFolder, "*.pdf");

                Array.ForEach<string>(files,
                    (filename) =>
                    {
                        filename = filename.Replace(Config.StoragePath, "");

                        if (Instrument.IsValidFilename(filename))
                            ExInstrumentation.Instruments.Add(Instrument.GetInstrument(filename));
                    });
            }
            else
            {
                // todo: was passiert, wenn Ordner kein Liedordner nicht vorhanden?
            }
        }

        #endregion

        #region Öffentliche Methoden

        /// <summary>
        /// Gibt alle vorhandenen Musikstücke zurück (filtert Ordner mit invaliden Namen heraus)
        /// </summary>
        public static List<string> LoadAll()
        {
            WorkList.CheckFileSystem();

            List<string> songList = new List<string>();

            string[] files = Directory.GetFiles(Config.StoragePath, "Meta.xml", SearchOption.AllDirectories);

            Array.ForEach<string>(files,
                (file) =>
                {
                    file = file.Replace(Config.StoragePath, "").Replace(@"\Meta.xml", "");

                    if (IsValidFolder(file))
                        songList.Add(file);
                });

            return songList;
        }

        /// <summary>
        /// Überprüft, ob der Name ein valider Ordnername ist.
        /// </summary>
        public static bool IsValidFolder(string songFolder)
        {
            songFolder = songFolder.Split('\\').Last();

            if (songFolder == "")
                return false;

            string[] result = songFolder.Split('#');

            if (result.Length <= 3)
                return true;

            return false;
        }

        #endregion

    }
}

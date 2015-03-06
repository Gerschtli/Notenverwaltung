using System;
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
                MetaInfo.SetSongFolder(_SongFolder);
            }
        }

        public string Name;

        public string Composer;

        public string Arranger;

        public Meta MetaInfo;

        public Instrumentation ExInstruments = new Instrumentation();

        #endregion

        #region Konstruktor

        /// <summary>
        /// Initialisiert die Instanzvariablen
        /// </summary>
        /// <param name="songFolder">Name des Ordners</param>
        public Song(string songFolder)
        {
            if (IsValidFolder(songFolder) && Directory.Exists(Config.StoragePath + @"\Musikstücke\" + songFolder))
            {
                _SongFolder = songFolder;

                string[] result = songFolder.Split('#');

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

                // -------
                // todo: Diese Daten bei Bedarf initialisieren oder direkt im Konstruktor?
                MetaInfo = Meta.Load(songFolder);

                try
                {
                    string[] files = Directory.GetFiles(Config.StoragePath + @"\Musikstücke\" + songFolder, "*.pdf");

                    int count = (Config.StoragePath + @"\Musikstücke\" + songFolder).Split('\\').Length;

                    Array.ForEach<string>(files,
                        (filename) =>
                        {
                            filename = filename.Split('\\')[count];

                            if (Instrument.IsValidFilename(filename))
                                ExInstruments.Instruments.Add(Instrument.GetInstrument(filename));
                        });
                }
                catch (Exception e)
                {
                    // todo: Wie sollen die Exceptions behandelt werden?
                    throw e;
                }
                // -------
            }
            else
            {
                // todo: was passiert, wenn Ordner nicht vorhanden?
            }
        }

        #endregion

        #region Öffentliche Methoden

        /// <summary>
        /// Gibt alle vorhandenen Musikstücke zurück (filtert Ordner mit invaliden Namen heraus)
        /// </summary>
        public static List<Song> LoadAll() // als Song-Instanzen zurückgeben oder nur SongFolder Namen?
        {
            string[] songFolders = Directory.GetDirectories(Config.StoragePath + @"\Musikstücke\");

            List<Song> songList = new List<Song>();

            int count = (Config.StoragePath + @"\Musikstücke").Split('\\').Length;

            Array.ForEach<string>(songFolders,
                (song) =>
                {
                    song = song.Split('\\')[count];

                    if (IsValidFolder(song))
                        songList.Add(new Song(song));
                });

            return songList;
        }

        /// <summary>
        /// Überprüft, ob der Name ein valider Ordnername ist.
        /// </summary>
        public static bool IsValidFolder(string songFolder)
        {
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

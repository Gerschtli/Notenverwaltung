using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// Repräsentiert ein Musikstück
    /// </summary>
    public class Song
    {

        #region Instanzvariablen

        private Config config;

        private string songFolder;
        public string SongFolder
        {
            get
            {
                return songFolder;
            }
            set
            {
                songFolder = value;
                MetaInfo.SongFolder = songFolder;
            }
        }

        public string Name { get; set; }

        public string Composer { get; set; }

        public string Arranger { get; set; }

        public Meta MetaInfo { get; set; }

        public Instrumentation ExInstrumentation { get; set; }

        #endregion

        #region Konstruktor

        /// <summary>
        /// Initialisiert die Instanzvariablen
        /// </summary>
        /// <param name="songFolder">Name des Ordners</param>
        public Song(string songFolder)
        {
            config = Config.GetInstance();

            if (File.Exists(Path.Combine(config.StoragePath, String.Format(config.MetaPath, songFolder))))
            {
                this.songFolder = songFolder;

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

                MetaInfo = Factory.GetMeta(songFolder);


                string[] files = Directory.GetFiles(Path.Combine(config.StoragePath, songFolder), "*.pdf");
                Instrument inst = null;

                ExInstrumentation = new Instrumentation();

                Array.ForEach<string>(files,
                    (filename) =>
                    {
                        filename = filename.Substring(config.StoragePath.Length).Trim('\\');

                        inst = Instrument.GetInstrument(filename);
                        if (inst != null)
                            ExInstrumentation.Instruments.Add(Instrument.GetInstrument(filename));
                    });
            }
            else
            {
                // todo: was passiert, wenn Ordner kein Liedordner ist?
            }
        }

        #endregion

        #region Öffentliche Methoden

        /// <summary>
        /// Gibt alle vorhandenen Musikstücke zurück (filtert Ordner mit invaliden Namen heraus)
        /// </summary>
        public static List<string> LoadAll()
        {
            List<string> songList = new List<string>();
            Config config = Config.GetInstance();

            string[] files = Directory.GetFiles(config.StoragePath, config.MetaPath.Substring(4), SearchOption.AllDirectories);

            Array.ForEach<string>(files,
                (file) =>
                {
                    file = file.Substring(config.StoragePath.Length, file.Length - config.StoragePath.Length - config.MetaPath.Substring(4).Length).Trim('\\');

                    songList.Add(file);
                });

            return songList;
        }

        /// <summary>
        /// Prüft, ob die benötigte Stimme vorhanden ist. Wenn nicht, wird die Ausweichstimme oder NULL zurückgegeben.
        /// </summary>
        /// <param name="inst">Benötigte Stimme</param>
        /// <returns>Zu verwendende Stimme oder NULL</returns>
        public Instrument GetInstrument(Instrument inst)
        {
            if (ExInstrumentation.Instruments.Contains(inst))
                return inst;

            if (MetaInfo.FallbackInstrumentation.ContainsKey(inst))
                return MetaInfo.FallbackInstrumentation[inst];

            return null;
        }

        #endregion

    }
}

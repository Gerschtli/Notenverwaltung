using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// Repräsentiert ein Musikstück
    /// </summary>
    public class Song : INotifyPropertyChanged
    {

        #region Instanzvariablen

        private Config config;

        private string songFolder;
        public string SongFolder
        {
            get { return songFolder; }
            set
            {
                if (songFolder != value)
                {
                    songFolder = value;
                    MetaInfo.SongFolder = songFolder;
                    NotifyPropertyChanged("SongFolder");
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string composer;
        public string Composer
        {
            get { return composer; }
            set
            {
                if (composer != value)
                {
                    composer = value;
                    NotifyPropertyChanged("Composer");
                }
            }
        }

        private string arranger;
        public string Arranger
        {
            get { return arranger; }
            set
            {
                if (arranger != value)
                {
                    arranger = value;
                    NotifyPropertyChanged("Arranger");
                }
            }
        }

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

        /// <summary>
        /// Verschiebt den Liedordner
        /// </summary>
        /// <param name="newSongFolder">Neuer Pfad</param>
        /// <returns>0, wenn erfolgreich; 1, wenn Name doppelt; 2, sonst</returns>
        public int MoveFolder(string newSongFolder)
        {
            try
            {
                Directory.Move(
                    Path.Combine(config.StoragePath, SongFolder),
                    Path.Combine(config.StoragePath, newSongFolder)
                );
            }
            catch (IOException e)
            {
                return 1;
            }
            catch
            {
                return 2;
            }

            SongFolder = newSongFolder;

            string[] result = newSongFolder.Split('\\').Last().Split('#');

            Arranger = Composer = Name = null;

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

            return 0;
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }
}

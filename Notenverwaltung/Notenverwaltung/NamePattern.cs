using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Notenverwaltung
{
    /// <summary>
    /// Statische Klasse zum Verwalten aller NamePatterns.
    /// </summary>
    public class NamePattern : INotifyPropertyChanged 
    {

        #region Klassenvariablen

        private static readonly string _Path = @"\Einstellungen\NamePattern.xml";

        private static ObservableCollection<string> _SongPatterns;
        public static ObservableCollection<string> SongPatterns
        {
            get
            {
                if (_SongPatterns == null)
                    Initialize();

                return _SongPatterns;
            }
        }

        private static ObservableCollection<string> _InstrumentPatterns;
        public static ObservableCollection<string> InstrumentPatterns
        {
            get
            {
                if (_InstrumentPatterns == null)
                    Initialize();

                return _InstrumentPatterns;
            }
        }

        #endregion

        /// <summary>
        /// Initialisiert die Klassenvariablen.
        /// </summary>
        private static void Initialize()
        {
            NamePatternSerializable obj = XmlHandler.GetObject<NamePatternSerializable>(Config.StoragePath + _Path);

            _SongPatterns = obj.SongPatterns;
            _SongPatterns.CollectionChanged += Patterns_CollectionChanged;

            _InstrumentPatterns = obj.InstrumentPatterns;
            _InstrumentPatterns.CollectionChanged += Patterns_CollectionChanged;
        }

        #region Eventhandler

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Eventhandler, wenn die Listen verändert werden.
        /// </summary>
        private static void Patterns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var pattern = new NamePatternSerializable()
            {
                SongPatterns = _SongPatterns,
                InstrumentPatterns = _InstrumentPatterns
            };

            XmlHandler.SaveObject(Config.StoragePath + _Path, pattern);
        }

        #endregion

        #region Normalisierung

        /// <summary>
        /// Normalisiert den eingegebenen Namen mit den bekannten NamePatterns.
        /// </summary>
        public static string NormalizeSong(string source)
        {
            return Replace(SongPatterns, source, "${Name}#${Composer}#${Arranger}", '#');
        }

        /// <summary>
        /// Normalisiert den eingegebenen Namen mit den bekannten NamePatterns.
        /// </summary>
        public static string NormalizeInstrument(string source)
        {
            return Replace(InstrumentPatterns, source, "${Name}#${Tune}#${Num}", '#');
        }

        /// <summary>
        /// Enthält die Logik für die Normalisierung
        /// </summary>
        /// <param name="list">Liste mit NamePatterns</param>
        /// <param name="source">Eingabe-String</param>
        /// <param name="replace">Regex-Replace-String</param>
        /// <param name="split">Trennzeichen</param>
        /// <returns>Normalisierter String</returns>
        private static string Replace(ObservableCollection<string> list, string source, string replace, char split)
        {
            foreach (string pattern in list)
            {
                try
                {
                    if (Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
                    {
                        return Regex.Replace(source, pattern, replace,
                            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace).Trim(split);
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        #endregion

    }

    /// <summary>
    /// Instanzierbares Abbild der statischen Klasse. Wird für die Speicherung benötigt.
    /// </summary>
    public class NamePatternSerializable
    {
        public ObservableCollection<string> SongPatterns;

        public ObservableCollection<string> InstrumentPatterns;
    }
}

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
    public static class NamePattern
    {

        #region Klassenvariablen

        private static readonly string _Path = @"NamePattern.xml";

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
        /// Überprüft, ob der eingegebene Name bereits normalisiert ist.
        /// </summary>
        /// <param name="source">Ordnername ohne Pfad</param>
        public static bool IsNormalizedSong(string source)
        {
            string pattern = @"^ (?<Name> [^\#]+) \# (?<Composer> [^\#]*) \# (?<Arranger> [^\#]*) $";

            return Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Normalisiert den eingegebenen Namen mit den bekannten NamePatterns.
        /// </summary>
        /// <param name="source">Ordnername ohne Pfad</param>
        public static string NormalizeSong(string source)
        {
            return IsNormalizedSong(source) ? null : Replace(SongPatterns, source, "${Name}#${Composer}#${Arranger}");
        }

        /// <summary>
        /// Überprüft, ob der eingegebene Name (ohne Dateiendung!) bereits normalisiert ist.
        /// </summary>
        /// <param name="source">Dokumentname ohne Pfad und Dateiendung</param>
        public static bool IsNormalizedInstrument(string source)
        {
            string pattern = @"^ (?<Name> [a-z-\s]+) (\# (?<Tune> [a-z]+) (\# (?<Num> \d+))? )? $";

            return Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Normalisiert den eingegebenen Namen (ohne Dateiendung!) mit den bekannten NamePatterns.
        /// </summary>
        /// <param name="source">Dokumentname ohne Pfad und Dateiendung</param>
        public static string NormalizeInstrument(string source)
        {
            return IsNormalizedSong(source) ? null : Replace(InstrumentPatterns, source, "${Name}#${Tune}#${Num}");
        }

        /// <summary>
        /// Enthält die Logik für die Normalisierung
        /// </summary>
        /// <param name="list">Liste mit NamePatterns</param>
        /// <param name="source">Eingabe-String</param>
        /// <param name="replace">Regex-Replace-String</param>
        /// <param name="trim">Trennzeichen</param>
        /// <returns>Normalisierter String</returns>
        private static string Replace(ObservableCollection<string> list, string source, string replace, char trim = '\0')
        {
            foreach (string pattern in list)
            {
                try
                {
                    if (Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
                    {
                        return Regex.Replace(source, pattern, replace,
                            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace).TrimEnd(trim);
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

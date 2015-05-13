using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Notenverwaltung
{
    /// <summary>
    /// Statische Klasse zum Verwalten aller NamePatterns.
    /// </summary>
    public class NamePattern
    {
        private static readonly RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

        public ObservableCollection<string> SongPatterns { get; set; }

        public ObservableCollection<string> InstrumentPatterns { get; set; }

        /// <summary>
        /// Setzt den Eventhandler.
        /// </summary>
        public NamePattern()
        {
            if (SongPatterns == null)
                SongPatterns = new ObservableCollection<string>();
            if (InstrumentPatterns == null)
                InstrumentPatterns = new ObservableCollection<string>();

            SongPatterns.CollectionChanged += Patterns_CollectionChanged;
            InstrumentPatterns.CollectionChanged += Patterns_CollectionChanged;
        }

        /// <summary>
        /// Eventhandler, wenn die Listen verändert werden.
        /// </summary>
        private void Patterns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Save.NamePattern(this);
        }

        #region Normalisierung

        /// <summary>
        /// Überprüft, ob der eingegebene Name bereits normalisiert ist.
        /// </summary>
        /// <param name="source">Ordnername ohne Pfad</param>
        public bool IsNormalizedSong(string source)
        {
            string pattern = @"^ (?<Name> [^\#]+) \# (?<Composer> [^\#]*) \# (?<Arranger> [^\#]*) $";

            return Regex.IsMatch(source, pattern, regexOptions);
        }

        /// <summary>
        /// Normalisiert den eingegebenen Namen mit den bekannten NamePatterns.
        /// </summary>
        /// <param name="source">Ordnername ohne Pfad</param>
        public string NormalizeSong(string source)
        {
            return IsNormalizedSong(source) ? source : Replace(SongPatterns, source, "${Name}#${Composer}#${Arranger}");
        }

        /// <summary>
        /// Überprüft, ob der eingegebene Name (ohne Dateiendung!) bereits normalisiert ist.
        /// </summary>
        /// <param name="source">Dokumentname ohne Pfad und Dateiendung</param>
        public bool IsNormalizedInstrument(string source)
        {
            string pattern = @"^ (?<Name> [a-z-\s]+) (\# (?<Tune> [a-z]+) (\# (?<Num> \d+))? )? $";

            return Regex.IsMatch(source, pattern, regexOptions);
        }

        /// <summary>
        /// Normalisiert den eingegebenen Namen (ohne Dateiendung!) mit den bekannten NamePatterns.
        /// </summary>
        /// <param name="source">Dokumentname ohne Pfad und Dateiendung</param>
        public string NormalizeInstrument(string source)
        {
            return IsNormalizedInstrument(source) ? source : Replace(InstrumentPatterns, source, "${Name}#${Tune}#${Num}");
        }

        /// <summary>
        /// Enthält die Logik für die Normalisierung
        /// </summary>
        /// <param name="list">Liste mit NamePatterns</param>
        /// <param name="source">Eingabe-String</param>
        /// <param name="replace">Regex-Replace-String</param>
        /// <param name="trim">Trennzeichen</param>
        /// <returns>Normalisierter String</returns>
        private string Replace(IEnumerable<string> list, string source, string replace, char trim = '\0')
        {
            foreach (string pattern in list)
            {
                try
                {
                    if (Regex.IsMatch(source, pattern, regexOptions))
                        return Regex.Replace(source, pattern, replace, regexOptions).TrimEnd(trim);
                }
                catch
                {
                    continue;
                }
            }

            return "";
        }

        #endregion
    }
}

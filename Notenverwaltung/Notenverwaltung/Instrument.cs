using System;
using System.Windows;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt ein Instrument dar.
    /// </summary>
    public class Instrument
    {
        public string Name;

        public int Num;

        public string Tune;

        /// <summary>
        /// ToString zu Testzwecken.
        /// </summary>
        public override string ToString()
        {
            if (Tune == null)
                return Name;
            if (Num == 0)
                return String.Format("{0} in {1}", Name, Tune);
            else
                return String.Format("{2}. {0} in {1}", Name, Tune, Num);
        }

        /// <summary>
        /// Erzeugen einer Instanz zu vorliegendem Dateinamen (z.B. "Schlagzeug.pdf")
        /// </summary>
        public static Instrument GetInstrument(string filename) // todo: Implementierung von anderen NamePatterns
        {
            try
            {
                filename = filename.TrimEnd(".pdf".ToCharArray());

                string[] result = filename.Split('#');

                Instrument inst = new Instrument();

                switch (result.Length)
                {
                    case 3:
                        inst.Num = Convert.ToInt32(result[2]);
                        goto case 2;
                    case 2:
                        inst.Tune = result[1];
                        goto case 1;
                    case 1:
                        inst.Name = result[0];
                        return inst;
                    default:
                        throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Dateiname ist ungültig.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return default(Instrument);
            }
        }
    }
}

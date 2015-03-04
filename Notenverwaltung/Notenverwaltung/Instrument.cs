using System;
using System.Windows;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt ein Instrument (Stimme) dar.
    /// </summary>
    public class Instrument : IEquatable <Instrument>
    {
        public string Name;

        public int Num;

        public string Tune;


        #region "überschriebene Methode"
        /// <summary>
        /// Gleichheitsprüfung über Objekte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
           if (obj == null)
                {
                    return false;
                }
                    
            Instrument i = obj as Instrument ;
            if ((System.Object)i == null)
            {
                return false;
            }

            if (i.Name == this.Name && (i.Num == this.Num | i.Num == 0 | this.Num == 0) && (i.Tune == this.Tune | i.Tune == "" | this.Tune == ""))
                {
                    return true ;
                }
            else
                return false ;
        }

        /// <summary>
        /// Gleichheitsprüfung mit typisiertem Objekt
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool Equals(Instrument i)
        {
            if (i == null)
            {
                return false;
            }

            if (i.Name == this.Name && (i.Num == this.Num | i.Num == 0 | this.Num == 0) && (i.Tune == this.Tune | i.Tune == "" | this.Tune == ""))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gleichheitsprüfung ==-Operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Instrument a, Instrument b)
        {
            if (System.Object.ReferenceEquals(a, b)) // Wenn beide Objekte null oder die gleiche Instanz sind -> true
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null)) // Wenn ein Objekt null ist, aber nicht beide -> false.
            {
                return false;
            }

            if (a.Name == b.Name && (a.Num == b.Num | a.Num == 0 | b.Num == 0) && (a.Tune == b.Tune | a.Tune == "" | b.Tune == ""))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gleichheitsprüfung !=-Operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Instrument a, Instrument b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Ausgabe der Stimmenbezeichnung
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

        #endregion

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

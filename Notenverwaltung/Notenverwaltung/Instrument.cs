using System;
using System.Linq;

namespace Notenverwaltung
{
    /// <summary>
    /// Stellt ein Instrument (Stimme) dar.
    /// </summary>
    public class Instrument : IEquatable<Instrument>
    {
        public string Name;

        public int Num;

        public string Tune;

        #region Überschriebene Methoden

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

            Instrument i = obj as Instrument;
            if ((System.Object)i == null)
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
        /// Überschreiben der GetHashCode Methode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (Num != 0)
                return String.Format("{0}#{1}#{2}", Name, Tune, Num).GetHashCode();
            else if (Tune != null)
                return String.Format("{0}#{1}", Name, Tune).GetHashCode();
            else
                return Name.GetHashCode();
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

        #region Öffentliche Methoden

        /// <summary>
        /// Erzeugen einer Instanz zu vorliegendem Dateinamen (z.B. "Schlagzeug.pdf")
        /// </summary>
        public static Instrument GetInstrument(string filename)
        {
            if (!IsValidFilename(filename))
                return null;

            filename = filename.Split('\\').Last().Substring(0, filename.Length - 4);

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
                    break;
            }

            return inst;
        }

        /// <summary>
        /// Überprüft, ob der Name ein valider Dateiname ist.
        /// </summary>
        public static bool IsValidFilename(string filename)
        {
            if (filename == "" || !filename.EndsWith(".pdf"))
                return false;

            filename = filename.Split('\\').Last().Substring(0, filename.Length - 4);

            string[] result = filename.Split('#');

            if (result.Length == 3)
            {
                try
                {
                    Convert.ToInt32(result[2]);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            if (result.Length <= 3)
                return true;

            return false;
        }

        #endregion

    }
}

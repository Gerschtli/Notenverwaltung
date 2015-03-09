using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notenverwaltung
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ---------- Test von Objektcode: ----------
            // *****Stimme - Equals*****
            //Instrumentation besetzung = new Instrumentation { };
            //besetzung.Instruments.Add(new Instrument() { Name = "Trompete", Num = 1, Tune = "Bb" });
            //besetzung.Instruments.Add(new Instrument() { Name = "Trompete", Num = 2, Tune = "Bb" });
            //besetzung.Instruments.Add(new Instrument() { Name = "Horn", Tune = "F" });
            //besetzung.Instruments.Add(new Instrument() { Name = "Posaune", Tune = "C" });
            //besetzung.Instruments.Add(new Instrument() { Name = "Tuba", Tune = "C" });
            //Instrument x = new Instrument { Name = "Horn", Num = 1, Tune = "F" };
            //Console.WriteLine("'" + x.ToString() + "' existiert: " + besetzung.Instruments.Contains(x));

            // *****Besetzung - Differenz*****
            //Instrumentation besetzung2 = new Instrumentation { };
            //besetzung2.Instruments.Add(new Instrument() { Name = "Trompete", Num = 1, Tune = "Bb" });
            ////besetzung2.Instruments.Add(new Instrument() { Name = "Trompete", Num = 2, Tune = "Bb" });
            //besetzung2.Instruments.Add(new Instrument() { Name = "Horn", Tune = "F" });
            //besetzung2.Instruments.Add(new Instrument() { Name = "Posaune", Tune = "C" });
            //besetzung2.Instruments.Add(new Instrument() { Name = "Tuba", Tune = "C" });
            //besetzung2.Instruments.Add(new Instrument() { Name = "Susaphon"});

            //List<Instrument> x = besetzung.GetMissingInstruments(besetzung2);
            //List<Instrument> y = besetzung.GetNeedlessInstruments(besetzung2);

            // ***** Folder - mission numbers  *****
            //Folder f = new Folder();
            //f.Order.Add(1, "x1");
            //f.Order.Add(12, "x12");
            //f.Order.Add(13, "x13");
            //f.Order.Add(4, "x4");
            //f.Order.Add(5, "x5");
            //List<int> l =  f.getMissingNumbers(); 

            // ***** Song *****
            //List<Song> list = Song.LoadAll();
            //foreach (Song item in list)
            //{
            //    Console.WriteLine(item.Name);
            //    foreach (Instrument inst in item.ExInstruments.Instruments)
            //    {
            //        Console.WriteLine("\t" + inst);
            //    }
            //}

            // ***** NamePatterns *****
            //Console.WriteLine(NamePattern.NormalizeSong("Name&Komponist&Arrangeur"));
            //Console.WriteLine(NamePattern.NormalizeInstrument("Name&Tune&Num"));
            //Console.WriteLine(NamePattern.NormalizeInstrument("Name&Tune&5"));
            //Console.WriteLine(NamePattern.NormalizeSong("Name&Komponist"));
            //Console.WriteLine(NamePattern.NormalizeInstrument("Name"));

            // ***** Watcher *****
            //Watcher w = new Watcher(@"c:\temp\_x");
        }
    }
}

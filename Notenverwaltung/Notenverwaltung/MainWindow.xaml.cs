using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Notenverwaltung
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher watcher;

        private WorkList workList;

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
            //besetzung2.Instruments.Add(new Instrument() { Name = "Susaphon" });

            //List<Instrument> x = besetzung.GetMissingInstruments(besetzung2);
            //List<Instrument> y = besetzung.GetNeedlessInstruments(besetzung2);

            // ***** Folder - mission numbers  *****
            //Folder f = new Folder();
            //f.Order.Add(1, "x1");
            //f.Order.Add(12, "x12");
            //f.Order.Add(13, "x13");
            //f.Order.Add(4, "x4");
            //f.Order.Add(5, "x5");
            //List<int> l = f.GetMissingNumbers(); 

            // ***** Song *****
            //List<string> list = Song.LoadAll();
            //Song song;
            //foreach (string item in list)
            //{
            //    song = new Song(item);
            //    Console.WriteLine(song.Name);
            //    foreach (Instrument inst in song.ExInstrumentation.Instruments)
            //    {
            //        Console.WriteLine("\t" + inst);
            //    }
            //}

            // ***** Watcher *****
            new FileSystemChecker().CheckStructure();
            new NameNormalizer().CheckSystem();
            watcher = Factory.GetWatcher();

            // ***** WorkList *****
            //foreach (var item in WorkList.GetInstance().LoTasks)
            //{
            //    Console.WriteLine(item.Type + ": " + item.Path);
            //}

            workList = WorkList.GetInstance();

            lvTodos.ItemsSource = workList.LoTasks;

            lbCategory.ItemsSource = Factory.GetCategories();

            var songInfo = new Song("Lied#Komponist#Arrangeur");

            gSongDetails.DataContext = songInfo;
        }

        /// <summary>
        /// Zeigt die aktuelle Worklist in der Konsole.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in workList.LoTasks)
            {
                Console.WriteLine(item.Type + ": " + item.Path);
            }
        }

        /// <summary>
        /// Fügt einen Eintrag in die Liste aller Katgorien hinzu.
        /// </summary>
        private void bAddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (tbNewCategory.Text != "")
            {
                List<string> allCategories = Factory.GetCategories();

                if (!allCategories.Exists(name => tbNewCategory.Text == name))
                {
                    allCategories.Add(tbNewCategory.Text);
                    Save.Categories(allCategories);
                    lbCategory.ItemsSource = allCategories;
                    tbNewCategory.Text = "";
                }
            }
        }
    }
}

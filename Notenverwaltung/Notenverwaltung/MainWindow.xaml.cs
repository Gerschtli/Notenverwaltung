﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private Song visibleSong;

        /// <summary>
        /// Initialisiert den Watcher und lädt das UI.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            new FileSystemChecker().CheckStructure();
            new NameNormalizer().CheckSystem();
            watcher = Factory.GetWatcher();

            workList = WorkList.GetInstance();

            // TabControl Aufgaben
            lvTodos.ItemsSource = workList.LoTasks;

            // TabControl Lieder + Liedinfo laden
            List<string> allSongs = Song.LoadAll();

            lbAllSongs.ItemsSource = allSongs;
            LoadSongInfo(allSongs[0]);

            //SoftwareTests();
        }

        #region Hilfsfunktionen

        /// <summary>
        /// Lädt die Informationen eines Liedes.
        /// </summary>
        /// <param name="songFolder">Liedordner</param>
        private void LoadSongInfo(string songFolder)
        {
            visibleSong = new Song(songFolder);
            gSongDetails.DataContext = visibleSong;

            List<CheckBoxListItem> list = new List<CheckBoxListItem>();

            foreach (string category in Factory.GetCategories())
                list.Add(new CheckBoxListItem(category, visibleSong.MetaInfo.Category.Contains(category)));

            lbCategory.ItemsSource = list;
        }

        /// <summary>
        /// Hier werden Software Tests gespeichert.
        /// </summary>
        private void SoftwareTests()
        {
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

            // ***** WorkList *****
            //foreach (var item in WorkList.GetInstance().LoTasks)
            //{
            //    Console.WriteLine(item.Type + ": " + item.Path);
            //}
        }

        #endregion

        #region Eventhandler

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
                    allCategories.Sort();
                    Save.Categories(allCategories);
                    (lbCategory.ItemsSource as List<CheckBoxListItem>).Add(new CheckBoxListItem(tbNewCategory.Text, false));
                    tbNewCategory.Text = "";
                }
            }
        }

        /// <summary>
        /// Song Infos speichern.
        /// </summary>
        private void bSongInfoSave_Click(object sender, RoutedEventArgs e)
        {
            lSongInfoErrorDuplicate.Visibility = Visibility.Hidden;
            lSongInfoErrorInput.Visibility = Visibility.Hidden;
            lSongInfoErrorUnknown.Visibility = Visibility.Hidden;

            // Änderung im Namen?
            string songName = tbSongName.Text,
                   composer = tbComposer.Text,
                   arranger = tbArranger.Text,
                   folderName = String.Format("{0}#{1}#{2}", songName, composer, arranger).Trim('#'),
                   oldFolderName = visibleSong.SongFolder.Split('\\').Last();

            if (songName.Contains('#') || composer.Contains('#') || arranger.Contains('#'))
            {
                lSongInfoErrorInput.Visibility = Visibility.Visible;
                return;
            }

            if (folderName != oldFolderName)
            {
                int result = visibleSong.MoveFolder(visibleSong.SongFolder.Substring(0, visibleSong.SongFolder.Length - oldFolderName.Length) + folderName);

                if (result == 0)
                {
                    visibleSong.Name = songName;
                    visibleSong.Composer = composer;
                    visibleSong.Arranger = arranger;
                }
                else if (result == 1)
                {
                    lSongInfoErrorDuplicate.Visibility = Visibility.Visible;
                    return;
                }
                else if (result == 2)
                {
                    lSongInfoErrorUnknown.Visibility = Visibility.Visible;
                    return;
                }
            }

            // Änderung Kategorien?
            List<CheckBoxListItem> checkBoxList = lbCategory.ItemsSource as List<CheckBoxListItem>;
            List<string> categories = new List<string>();

            foreach (CheckBoxListItem item in checkBoxList)
            {
                if (item.Checked)
                    categories.Add(item.Text);
            }

            visibleSong.MetaInfo.Category = categories;
            Save.Meta(visibleSong.MetaInfo);
        }

        /// <summary>
        /// Lied wird in der TabControl ausgewählt.
        /// </summary>
        private void lbAllSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSongInfo((sender as ListBox).SelectedItem as string);
        }

        #endregion

    }
}

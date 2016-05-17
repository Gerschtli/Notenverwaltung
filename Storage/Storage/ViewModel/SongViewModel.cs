using System.Collections.Generic;
using System.Windows.Input;
using Storage.Helper;
using Storage.Model;
using Storage.Service;
using Storage.View;
using Song = Storage.Model.Song;

namespace Storage.ViewModel
{
    public class SongViewModel : BaseViewModel, ISongViewModel
    {
        #region Fields

        private readonly ISong songService;

        #endregion

        #region Constructor

        public SongViewModel(ISong songService)
        {
            this.songService = songService;
        }

        #endregion

        #region Interface Properties

        public Song Song { get; set; }

        public DirectoryStatus Type
        {
            get { return DirectoryStatus.SONG; }
        }

        public ViewType ViewType
        {
            get { return ViewType.SONG; }
        }

        public string DisplayText
        {
            get { return Name; }
        }

        public string Path
        {
            get { return Song.Path; }
            set
            {
                if (value != Song.Path) {
                    Song.Path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return Song.Name; }
            set
            {
                if (value != Song.Name) {
                    Song.Name = value;
                    RaisePropertyChanged("Name");
                    RaisePropertyChanged("DisplayText");
                }
            }
        }

        public string Arranger
        {
            get { return Song.Arranger; }
            set
            {
                if (value != Song.Arranger) {
                    Song.Arranger = value;
                    RaisePropertyChanged("Arranger");
                }
            }
        }

        public string Composer
        {
            get { return Song.Composer; }
            set
            {
                if (value != Song.Composer) {
                    Song.Composer = value;
                    RaisePropertyChanged("Composer");
                }
            }
        }

        public ISet<Category> Categories
        {
            get { return Song.Categories; }
            set
            {
                if (value != Song.Categories) {
                    Song.Categories = value;
                    RaisePropertyChanged("Categories");
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SaveDetails
        {
            get { return new RelayCommand(SaveDetailsExecute); }
        }

        private void SaveDetailsExecute()
        {
            songService.UpdateDetails(Song);
        }

        #endregion
    }
}

using System.Windows.Input;
using Storage.Helper;
using Storage.Model;
using Storage.Util;
using Storage.View;

namespace Storage.ViewModel
{
    public class DirectoryViewModel : BaseViewModel, IDirectoryViewModel
    {
        #region Fields

        private readonly IFormatter formatter;

        #endregion

        #region Constructor

        public DirectoryViewModel(IFormatter formatter)
        {
            this.formatter = formatter;
        }

        #endregion

        #region Interface Properties

        public Directory Directory { get; set; }

        public DirectoryStatus Type
        {
            get { return DirectoryStatus.NO_SONG; }
        }

        public ViewType ViewType
        {
            get { return ViewType.DIRECTORY; }
        }

        public string DisplayText
        {
            get { return formatter.StripDataPath(Path); }
        }

        public string Path
        {
            get { return Directory.Path; }
            set
            {
                if (value != Directory.Path) {
                    Directory.Path = value;
                    RaisePropertyChanged("Path");
                    RaisePropertyChanged("DisplayText");
                }
            }
        }

        public bool IsSong { get; set; }

        #endregion

        #region Commands

        public ICommand Save
        {
            get { return new RelayCommand(SaveExecute, SaveCanExecute); }
        }

        private bool SaveCanExecute()
        {
            return IsSong;
        }

        private void SaveExecute()
        {
            // Save Status in song.xml
            // Remove Item from Directories
            // Create SongViewModel in Directories
            // Load SongView
        }

        #endregion
    }
}

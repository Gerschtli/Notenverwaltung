using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Autofac.Features.Indexed;
using Storage.Helper;
using Storage.Model;
using Storage.Util.Interface;
using Storage.View;

namespace Storage.ViewModel
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        #region Fields

        private readonly ObservableCollection<IDirectoryListItem> directories =
            new ObservableCollection<IDirectoryListItem>();

        private readonly IIndex<ViewType, IUserControl> userControlIndex;

        private IUserControl currentPage;
        private ICollectionView directoriesSongs;
        private ICollectionView directoriesTasks;
        private IDirectoryListItem selectedDirectory;

        #endregion

        #region Events

        public event TriggerHandler ApplicationShutDown;

        #endregion

        #region Constructor

        public MainViewModel(IIndex<ViewType, IUserControl> userControlIndex)
        {
            this.userControlIndex = userControlIndex;
        }

        #endregion

        #region Commands

        public ICommand WindowClosing
        {
            get { return new RelayCommand(WindowClosingExecute); }
        }

        #endregion

        #region Properties

        public ObservableCollection<IDirectoryListItem> Directories
        {
            get { return directories; }
        }

        public ICollectionView DirectoriesSongs
        {
            get
            {
                if (directoriesSongs == null) {
                    directoriesSongs = CreateCollectionView();
                    directoriesSongs.Filter = Filter(d => d.Type == DirectoryStatus.SONG);
                }
                return directoriesSongs;
            }
        }

        public ICollectionView DirectoriesTasks
        {
            get
            {
                if (directoriesTasks == null) {
                    directoriesTasks = CreateCollectionView();
                    directoriesTasks.Filter = Filter(d => d.Type != DirectoryStatus.SONG);
                    directoriesTasks.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                }
                return directoriesTasks;
            }
        }

        public IDirectoryListItem SelectedDirectory
        {
            get { return selectedDirectory; }
            set
            {
                selectedDirectory = null;
                RaisePropertyChanged("SelectedDirectory");
                selectedDirectory = value;
                RaisePropertyChanged("SelectedDirectory");
            }
        }

        public IUserControl CurrentPage
        {
            get { return currentPage ?? (currentPage = userControlIndex[ViewType.DEFAULT]); }
            set
            {
                currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }

        public ICommand SelectDirectory
        {
            get { return new RelayCommand<IDirectoryListItem>(SelectDirectoryExecute); }
        }

        #endregion

        #region Private Methods

        private ICollectionView CreateCollectionView()
        {
            return new CollectionViewSource {Source = Directories}.View;
        }

        private Predicate<object> Filter(Predicate<IDirectoryListItem> predicate)
        {
            return o =>
            {
                var directoryViewModel = o as IDirectoryListItem;
                return directoryViewModel != null && predicate(directoryViewModel);
            };
        }

        #endregion

        private void SelectDirectoryExecute(IDirectoryListItem directory)
        {
            CurrentPage = userControlIndex[directory.ViewType];
            CurrentPage.DataContext = directory;
        }

        private void WindowClosingExecute()
        {
            if (ApplicationShutDown != null) {
                ApplicationShutDown();
            }
        }
    }
}

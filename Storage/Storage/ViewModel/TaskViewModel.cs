using Storage.Model;
using Storage.Util;
using Storage.View;

namespace Storage.ViewModel
{
    public class TaskViewModel : BaseViewModel, ITaskViewModel
    {
        #region Fields

        private readonly IFormatter formatter;

        #endregion

        #region Constructor

        public TaskViewModel(IFormatter formatter)
        {
            this.formatter = formatter;
        }

        #endregion

        #region Interface Properties

        public Task Task { get; set; }

        public DirectoryStatus Type
        {
            get { return DirectoryStatus.UNKNOWN; }
        }

        public ViewType ViewType
        {
            get { return ViewType.TASK; }
        }

        public string DisplayText
        {
            get { return formatter.StripDataPath(Path); }
        }

        public string Path
        {
            get { return Task.Path; }
            set
            {
                if (value != Task.Path) {
                    Task.Path = value;
                    RaisePropertyChanged("Path");
                    RaisePropertyChanged("DisplayText");
                }
            }
        }

        #endregion
    }
}

using System.Collections.ObjectModel;
using Storage.ViewModel;

namespace Storage.Util.Interface
{
    public interface IDirectoryCollectionProperty
    {
        ObservableCollection<IDirectoryListItem> Directories { get; }
    }
}

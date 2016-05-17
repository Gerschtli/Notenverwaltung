using Storage.Model;

namespace Storage.ViewModel
{
    public interface ISongViewModel : IDirectoryListItem
    {
        Song Song { get; set; }
    }
}

using Storage.Model;

namespace Storage.ViewModel
{
    public interface IDirectoryViewModel : IDirectoryListItem
    {
        Directory Directory { get; set; }
    }
}

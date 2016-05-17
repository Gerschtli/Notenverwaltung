using Storage.Model;
using Storage.View;

namespace Storage.ViewModel
{
    public interface IDirectoryListItem
    {
        DirectoryStatus Type { get; }
        ViewType ViewType { get; }
        string DisplayText { get; }
        string Path { get; set; }
    }
}

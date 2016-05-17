using Storage.Model;

namespace Storage.ViewModel
{
    public interface ITaskViewModel : IDirectoryListItem
    {
        Task Task { get; set; }
    }
}

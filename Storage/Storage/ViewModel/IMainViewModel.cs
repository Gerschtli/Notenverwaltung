using Storage.Util.Interface;

namespace Storage.ViewModel
{
    public interface IMainViewModel : IDirectoryCollectionProperty
    {
        event TriggerHandler ApplicationShutDown;
    }
}

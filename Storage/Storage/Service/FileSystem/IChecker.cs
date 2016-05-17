namespace Storage.Service.FileSystem
{
    public interface IChecker
    {
        event TriggerHandler Finished;
        void Start(string path = "");
    }
}

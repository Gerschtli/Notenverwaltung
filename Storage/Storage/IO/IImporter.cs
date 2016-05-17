namespace Storage.IO
{
    public interface IImporter
    {
        T Read<T>(string path);
    }
}

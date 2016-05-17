namespace Storage.IO
{
    public interface IExporter
    {
        void Write<T>(string path, T data);
    }
}

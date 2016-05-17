using Storage.Model;
using DirectoryModel = Storage.Model.Directory;

namespace Storage.Service
{
    public interface IDirectories
    {
        void AddDirectory(DirectoryModel directory);
        void AddSong(Model.Song song);
        void AddTask(Task task);
        void UpdatePath(string oldPath, string newPath);
        void DeleteByPath(string path);
    }
}

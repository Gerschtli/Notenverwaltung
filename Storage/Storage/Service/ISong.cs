using SongModel = Storage.Model.Song;

namespace Storage.Service
{
    public interface ISong
    {
        void UpdateDetails(SongModel song);
    }
}

using System;

namespace Storage.Service.FileSystem
{
    public interface IWatcher : IDisposable
    {
        void Start();
    }
}

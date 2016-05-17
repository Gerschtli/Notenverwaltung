using System;

namespace Storage.Util
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }
}

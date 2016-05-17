using ThreadingDispatcher = System.Windows.Threading.Dispatcher;

namespace Storage.Util.Interface
{
    public interface IDispatcherProperty
    {
        ThreadingDispatcher Dispatcher { get; }
    }
}

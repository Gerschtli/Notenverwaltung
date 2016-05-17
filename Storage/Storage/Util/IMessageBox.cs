using System.Windows;

namespace Storage.Util
{
    public interface IMessageBox
    {
        MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon);
    }
}

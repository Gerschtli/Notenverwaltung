using System.Windows;
using WindowsMessageBox = System.Windows.MessageBox;

namespace Storage.Util
{
    public class MessageBox : IMessageBox
    {
        #region Methods

        public MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return WindowsMessageBox.Show(text, caption, buttons, icon);
        }

        #endregion
    }
}

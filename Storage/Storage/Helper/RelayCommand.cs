using System;
using System.Windows.Input;

namespace Storage.Helper
{
    public class RelayCommand : ICommand
    {
        #region Constructors

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Fields

        private readonly Func<bool> canExecute;
        private readonly Action execute;

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null) {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecute != null) {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
        }

        #endregion
    }

    public class RelayCommand<T> : ICommand
    {
        #region Constructors

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute == null) {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Fields

        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null) {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecute != null) {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            execute((T) parameter);
        }

        #endregion
    }
}

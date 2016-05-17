using System;
using System.Windows.Threading;
using Storage.Util.Interface;

namespace Storage.Util
{
    public class Dispatcher : IDispatcher
    {
        #region Fields

        private readonly IDispatcherProperty dispatcherContainer;

        #endregion

        #region Constructor

        public Dispatcher(IDispatcherProperty dispatcherContainer)
        {
            this.dispatcherContainer = dispatcherContainer;
        }

        #endregion

        #region Methods

        public void Invoke(Action action)
        {
            dispatcherContainer.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }

        #endregion
    }
}

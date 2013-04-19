using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace libWyvernzora.Patterns.MVVM
{
    /// <summary>
    /// Base class for all ViewModels.
    /// Includes certain utility methods.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// Event Args for State Change request.
        /// </summary>
        public class ChangeStateEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the name of the requested state.
            /// </summary>
            public String State { get; set; }
            /// <summary>
            /// Gets a value indicating whether to use transitions.
            /// </summary>
            public Boolean Transition { get; set; }
        }

        protected Dispatcher dispatcher;

        protected ViewModelBase(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            errors = new Dictionary<string, string>();
        }

        #region UI State Events

        /// <summary>
        /// Fires when ViewModel requests state change from View.
        /// </summary>
        public event EventHandler<ChangeStateEventArgs> ChangeState;

        /// <summary>
        /// Requests View to change its state.
        /// </summary>
        /// <param name="newState">Name of the requested state.</param>
        /// <param name="transition"></param>
        protected void OnChangeState(String newState, Boolean transition = true)
        {
            var handler = ChangeState;
            if (handler != null)
                handler(this, new ChangeStateEventArgs { State = newState, Transition = transition });
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged(String name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDataErrorInfo 

        protected Dictionary<String, String> errors; 

        public string Error { get; protected set; }

        public string this[string columnName]
        {
            get
            {
                return errors.ContainsKey(columnName) ? errors[columnName] : String.Empty;
            }
        }

        #endregion


    }
}

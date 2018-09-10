using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
namespace XDemo.Core.Infrastructure.Input
{
    /// <summary>
    /// BETA!
    /// </summary>
    public class TaskCommand : ICommand
    {
        readonly Func<object, bool> _canExecute;
        readonly Task _execute;
        volatile bool _inProgress;
        private event EventHandler _canExecuteChangedHandler;
        private readonly SynchronizationContext _synchronizationContext;

        public TaskCommand(Task execute, Func<object, bool> canExecute)
        {
            _synchronizationContext = SynchronizationContext.Current;
            // main ctor
            // dont allow null
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            // allow null
            _canExecute = canExecute;
        }

        //public TaskCommand(Action<object> execute, Func<object, bool> canExecute = null)
        //{
        //    // main ctor
        //    // dont allow null
        //    _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        //    // allow null
        //    _canExecute = canExecute;
        //}

        //public TaskCommand(Action execute, Func<bool> canExecute = null) : this(obj => execute(), obj => canExecute())
        //{
        //}

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                _canExecuteChangedHandler = _canExecuteChangedHandler ?? new EventHandler(value);
                lock (_canExecuteChangedHandler)
                {
                    _canExecuteChangedHandler += value;
                }
            }
            remove
            {
                if (_canExecuteChangedHandler != null)
                    lock (_canExecuteChangedHandler)
                    {
                        _canExecuteChangedHandler -= value;
                    }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_inProgress)
                return false;
            if (_canExecute != null)
                return _canExecute(parameter);
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            try
            {
                _inProgress = true;
                ChangeCanExecute();
                //_execute(parameter);
            }
            finally
            {
                _inProgress = false;
                ChangeCanExecute();
            }
        }
        #region private methods

        void ChangeCanExecute()
        {
            _canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}

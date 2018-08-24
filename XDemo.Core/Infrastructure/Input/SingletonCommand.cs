using System;
using System.Windows.Input;
namespace XDemo.Core.Infrastructure.Input
{
    public class SingletonCommand:ICommand
    {
        readonly Func<object, bool> _canExecute;
        readonly Action<object> _execute;
        volatile bool _inProgress;
        private event EventHandler _canExecuteChangedHandler;

        public SingletonCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            // main ctor
            // dont allow null
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            // allow null
            _canExecute = canExecute;
        }

        public SingletonCommand(Action execute, Func<bool> canExecute = null) : this(obj => execute(), obj => canExecute())
        {
        }

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
                _execute(parameter);
            }
            finally
            {
                _inProgress = false;
                ChangeCanExecute();
            }
        }

        void ChangeCanExecute()
        {
            _canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
        }
    }

    public class SingletonCommand<T> : SingletonCommand
    {
        public SingletonCommand(Action<T> execute, Func<T, bool> canExecute = null) : base(obj => execute((T)obj), obj => canExecute((T)obj))
        {
        }
    }
}

using System;
using System.Windows.Input;

namespace SprintPlanner.FrameworkWPF
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public DelegateCommand(Action<T> execute)
            : this(execute, _ => true)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Raise(this);
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute)
            : base(execute != null ? _ => execute() : (Action<object>)null)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : base(execute != null ? _ => execute() : (Action<object>)null,
                    canExecute != null ? _ => canExecute() : (Predicate<object>)null)
        {
        }
    }
}

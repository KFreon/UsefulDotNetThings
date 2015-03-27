using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Creates a command in an easy manner. Not my code.
    /// </summary>
    public class CommandHandler : ICommand
    {
        internal Action<Object> actionWArgs { get; set; }
        internal Action actionWOArgs { get; set; }
        internal bool _canExecute { get; set; }

        public CommandHandler(bool canExecute)
        {
            _canExecute = canExecute;

        }

        public CommandHandler(Action action, bool canExecute) : this(canExecute)
        {
            actionWOArgs = action;
        }

        public CommandHandler(Action<Object> action, bool canExecute) : this(canExecute)
        {
            actionWArgs = action;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public virtual void Execute(object parameter)
        {
            // KFreon: Run designated action
            if (actionWOArgs != null)
                ((Action)actionWOArgs)();
            else if (actionWArgs != null)
                ((Action<object>)actionWArgs)(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}

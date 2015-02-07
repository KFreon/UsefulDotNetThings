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
        internal object _action { get; set; }
        internal bool _canExecute { get; set; }

        public CommandHandler()
        {

        }

        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public virtual void Execute(object parameter)
        {
            ((Action)_action)();
        }

        public event EventHandler CanExecuteChanged;
    }


    /// <summary>
    /// Creates Commands that take arguments in an easy way. This is mine, but clearly based on stuff that isn't mine.
    /// </summary>
    public class CommandHandlerWArgs : CommandHandler
    {
        public CommandHandlerWArgs(Action<object> action, bool canExecute) : base()
        {
            _canExecute = canExecute;
            _action = action;
        }

        public override void Execute(object parameter)
        {
            ((Action<object>)_action)(parameter);
        }
    }
}

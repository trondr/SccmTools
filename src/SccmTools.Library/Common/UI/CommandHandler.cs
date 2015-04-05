using System;
using System.Windows.Input;

namespace SccmTools.Library.Common.UI
{
    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add {  }
            remove {  }
        }

        public void Execute(object parameter)
        {
            _action();
        }
        
    }
}

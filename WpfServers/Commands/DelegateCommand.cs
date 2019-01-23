using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfServers.Commands
{
    public class DelegateCommand : ICommand
    {
        public Action<object> ExecuteCommand = null;

        public Func<object , bool> CanExecuteCommand = null;

        public event EventHandler CanExecuteChanged;

        #region
        public DelegateCommand() { }
        public DelegateCommand(Action<object> executeCommand)
        {
            this.ExecuteCommand = executeCommand;
        }
        public DelegateCommand(Action<object> executeCommand,Func<object,bool> canExecuteCommand)
        {
            this.ExecuteCommand = executeCommand;
            this.CanExecuteCommand = canExecuteCommand;
        }
        #endregion

        public bool CanExecute(object parameter)
        {
            if(CanExecuteCommand != null)
            {
                return this.CanExecuteCommand(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if(this.ExecuteCommand != null)
            {
                this.ExecuteCommand(parameter);
            }
        }

        public void RaiseCanExcuteChanged()
        {
            if(CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}

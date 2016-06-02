using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APCUPS
{
    class OpenWindowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            ((MainWindow)parameter).Show();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace APCUPS
{
    public class HideUPSWindow : CommandBase<HideUPSWindow>
    {
            public override void Execute(object parameter)
            {
                GetTaskbarWindow(parameter).Hide();
                CommandManager.InvalidateRequerySuggested();
            }


            public override bool CanExecute(object parameter)
            {
                Window win = GetTaskbarWindow(parameter);
                return win != null && win.IsVisible;
            }
    }
}

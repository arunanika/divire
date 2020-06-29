//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace divire.ViewModels
{
    /// <summary>
    /// Base class of viewmodel
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise event when a property is changed.
        /// </summary>
        /// <param name="name">Property name</param>
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            if (null == PropertyChanged) return;

            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Delegate Command
        /// </summary>
        public class DelegateCommand : ICommand
        {
            Action<object> execute;
            Predicate<object> canExecute;

            public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public DelegateCommand(Action execute, Func<bool> canExecute)
            {
                this.execute = (p) => { execute(); };
                this.canExecute = (p) => { return canExecute(); };
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                execute?.Invoke(parameter);
            }

            public bool CanExecute(object parameter)
            {
                bool bRet = false;

                if (null != canExecute)
                {
                    bRet = canExecute(parameter);
                }

                return bRet;
            }
        }
    }
}

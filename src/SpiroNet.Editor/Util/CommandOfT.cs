/*
SpiroNet.Editor
Copyright (C) 2019 Wiesław Šoltés

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 3
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA
02110-1301, USA.

*/
using System;
using System.Windows.Input;

namespace SpiroNet.Editor
{
    public class Command<T> : ICommand where T : class
    {
        public event EventHandler CanExecuteChanged;

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter as T);
        }

        public void Execute(object parameter)
        {
            if (_execute == null)
                return;
            _execute(parameter as T);
        }

        public static ICommand Create(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new Command<T>(execute, canExecute);
        }
    }
}

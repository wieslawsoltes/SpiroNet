/*
SpiroNet.Editor
Copyright (C) 2015 Wiesław Šoltés

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
using System.Windows.Input;

namespace SpiroNet.Editor
{
    public class EditorCommands
    {
        public ICommand InvalidateCommand { get; set; }
        
        public ICommand NewCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand ExportCommand { get; set; }

        public ICommand ExitCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand IsStrokedCommand { get; set; }

        public ICommand IsFilledCommand { get; set; }

        public ICommand IsClosedCommand { get; set; }

        public ICommand IsTaggedCommand { get; set; }

        public ICommand PointTypeCommand { get; set; }

        public ICommand ExecuteScriptCommand { get; set; }
    }
}

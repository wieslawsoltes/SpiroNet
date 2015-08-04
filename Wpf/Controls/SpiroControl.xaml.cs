/*
SpiroNet.Wpf
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpiroNet.Wpf
{
    public partial class SpiroControl : UserControl
    {
        private SpiroContext _context;

        public SpiroControl()
        {
            InitializeComponent();

            canvas.PreviewMouseLeftButtonDown += Canvas_PreviewMouseLeftButtonDown;
            canvas.PreviewMouseLeftButtonUp += Canvas_PreviewMouseLeftButtonUp;
            canvas.PreviewMouseRightButtonDown += Canvas_PreviewMouseRightButtonDown;
            canvas.PreviewMouseMove += Canvas_PreviewMouseMove;

            InitializeContext();

            DataContext = _context;

            Loaded += SpiroControl_Loaded;
        }

        private void InitializeContext()
        {
            _context = new SpiroContext()
            {
                Width = 600,
                Height = 600,
                IsClosed = false,
                IsTagged = false,
                PointType = SpiroPointType.G4,
                Shapes = new ObservableCollection<PathShape>(),
                Data = new Dictionary<PathShape, string>()
            };

            _context.Invalidate = canvas.InvalidateVisual;

            _context.NewCommand = Command.Create(_context.New);
            _context.OpenCommand = Command.Create(Open);
            _context.SaveAsCommand = Command.Create(SaveAs);
            _context.ExportAsSvgCommand = Command.Create(ExportAsSvg);
            _context.ExitCommand = Command.Create(Exit);
            _context.IsClosedCommand = Command.Create(_context.ToggleIsClosed);
            _context.IsTaggedCommand = Command.Create(_context.ToggleIsTagged);
            _context.PointTypeCommand = Command<string>.Create(_context.TogglePointType);
            _context.ExecuteScriptCommand = Command<string>.Create(_context.ExecuteScript);
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);
            _context.LeftDown(point.X, point.Y);
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(canvas);
            _context.LeftUp(point.X, point.Y);
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);
            _context.RightDown(point.X, point.Y);
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);
            _context.Move(point.X, point.Y);
        }

        private void SpiroControl_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Focus();
        }

        private void Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Spiro Files (*.spiro)|*.spiro|All Files (*.*)|*.*";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                _context.Open(dlg.FileName);
            }
        }

        private void SaveAs()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Spiro Files (*.spiro)|*.spiro|All Files (*.*)|*.*";
            dlg.FileName = "drawing.spiro";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                _context.SaveAs(dlg.FileName);
            }
        }

        private void ExportAsSvg()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Svg Files (*.svg)|*.svg|All Files (*.*)|*.*";
            dlg.FileName = "drawing.svg";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                _context.ExportAsSvg(dlg.FileName);
            }
        }

        private void Exit()
        {
            Application.Current.Windows[0].Close();
        }
    }
}

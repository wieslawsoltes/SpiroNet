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
using SpiroNet.Editor;

namespace SpiroNet.Wpf
{
    public partial class SpiroControl : UserControl
    {
        private SpiroEditor _editor;

        public SpiroControl()
        {
            InitializeComponent();

            canvas.PreviewMouseDown += Canvas_PreviewMouseDown;
            canvas.PreviewMouseLeftButtonDown += Canvas_PreviewMouseLeftButtonDown;
            canvas.PreviewMouseLeftButtonUp += Canvas_PreviewMouseLeftButtonUp;
            canvas.PreviewMouseRightButtonDown += Canvas_PreviewMouseRightButtonDown;
            canvas.PreviewMouseMove += Canvas_PreviewMouseMove;

            _editor = new SpiroEditor()
            {
                State = new SpirtoEditorState(),
                Commands = new SpirtoEditorCommands(),
                Invalidate = canvas.InvalidateVisual,
                Drawing = new PathDrawing()
                {
                    Width = 600,
                    Height = 600,
                    Shapes = new ObservableCollection<PathShape>()
                },
                Data = new Dictionary<PathShape, string>(),
                Knots = new Dictionary<PathShape, IList<SpiroKnot>>()
            };

            _editor.Commands.InvalidateCommand = Command.Create(_editor.Invalidate);
            _editor.Commands.NewCommand = Command.Create(_editor.New);
            _editor.Commands.OpenCommand = Command.Create(Open);
            _editor.Commands.SaveAsCommand = Command.Create(SaveAs);
            _editor.Commands.ExportCommand = Command.Create(Export);
            _editor.Commands.ExitCommand = Command.Create(Exit);
            _editor.Commands.IsStrokedCommand = Command.Create(_editor.ToggleIsStroked);
            _editor.Commands.IsFilledCommand = Command.Create(_editor.ToggleIsFilled);
            _editor.Commands.IsClosedCommand = Command.Create(_editor.ToggleIsClosed);
            _editor.Commands.IsTaggedCommand = Command.Create(_editor.ToggleIsTagged);
            _editor.Commands.PointTypeCommand = Command<string>.Create(_editor.TogglePointType);
            _editor.Commands.ExecuteScriptCommand = Command<string>.Create(ExecuteScript);

            DataContext = _editor;

            Loaded += SpiroControl_Loaded;
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);

            try
            {
                if (e.ChangedButton == MouseButton.Middle)
                {
                    _editor.MiddleDown(point.X, point.Y);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }
        
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);

            try
            {
                _editor.LeftDown(point.X, point.Y);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(canvas);

            try
            {
                _editor.LeftUp(point.X, point.Y);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);

            try
            {
                _editor.RightDown(point.X, point.Y);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);

            try
            {
                _editor.Move(point.X, point.Y);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void SpiroControl_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Focus();
        }

        private void Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Spiro Files (*.spiro)|*.spiro|Plate Files (*.plate)|*.plate|All Files (*.*)|*.*";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            _editor.OpenDrawing(dlg.FileName);
                            break;
                        case 2:
                            _editor.OpenPlate(dlg.FileName);
                            break;
                        default:
                            _editor.OpenDrawing(dlg.FileName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void SaveAs()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Spiro Files (*.spiro)|*.spiro|Plate Files (*.plate)|*.plate|All Files (*.*)|*.*";
            dlg.FileName = "drawing.spiro";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            _editor.SaveAsDrawing(dlg.FileName);
                            break;
                        case 2:
                            _editor.SaveAsPlate(dlg.FileName);
                            break;
                        default:
                            _editor.SaveAsDrawing(dlg.FileName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void Export()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Svg Files (*.svg)|*.svg|Ps Files (*.ps)|*.ps|All Files (*.*)|*.*";
            dlg.FileName = "drawing.svg";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    switch (dlg.FilterIndex)
                    {
                        case 1:
                            _editor.ExportAsSvg(dlg.FileName);
                            break;
                        case 2:
                            _editor.ExportAsPs(dlg.FileName);
                            break;
                        default:
                            _editor.ExportAsSvg(dlg.FileName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void ExecuteScript(string script)
        {
            try
            {
                _editor.ExecuteScript(script);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void Exit()
        {
            Application.Current.Windows[0].Close();
        }
    }
}

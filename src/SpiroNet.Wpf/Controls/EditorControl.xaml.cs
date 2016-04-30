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
using SpiroNet.Editor;
using SpiroNet.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpiroNet.Wpf
{
    public partial class EditorControl : UserControl
    {
        private SpiroEditor _editor;

        public EditorControl()
        {
            InitializeComponent();

            InitializeEditor();
            InitializeCanvas();
            InitializeSnapMode();

            DataContext = _editor;

            Loaded += SpiroControl_Loaded;
        }

        private void InitializeEditor()
        {
            _editor = new SpiroEditor()
            {
                State = new EditorState(),
                Measure = new EditorMeasure(),
                Commands = new EditorCommands(),
                Invalidate = () => canvas.InvalidateVisual(),
                Capture = () => canvas.CaptureMouse(),
                Release = () => canvas.ReleaseMouseCapture(),
                Drawing = SpiroDrawing.Create(600, 600),
                Data = new Dictionary<SpiroShape, string>(),
                Knots = new Dictionary<SpiroShape, IList<SpiroKnot>>()
            };

            _editor.Commands.InvalidateCommand = Command.Create(_editor.Invalidate);
            _editor.Commands.NewCommand = Command.Create(New);
            _editor.Commands.OpenCommand = Command.Create(Open);
            _editor.Commands.SaveAsCommand = Command.Create(SaveAs);
            _editor.Commands.ExportCommand = Command.Create(Export);
            _editor.Commands.ExitCommand = Command.Create(Exit);
            _editor.Commands.DeleteCommand = Command.Create(_editor.Delete);
            _editor.Commands.IsStrokedCommand = Command.Create(_editor.ToggleIsStroked);
            _editor.Commands.IsFilledCommand = Command.Create(_editor.ToggleIsFilled);
            _editor.Commands.IsClosedCommand = Command.Create(_editor.ToggleIsClosed);
            _editor.Commands.IsTaggedCommand = Command.Create(_editor.ToggleIsTagged);
            _editor.Commands.PointTypeCommand = Command<string>.Create(_editor.TogglePointType);
            _editor.Commands.ExecuteScriptCommand = Command<string>.Create(_editor.ExecuteScript);
        }

        private void InitializeCanvas()
        {
            canvas.PreviewMouseDown += Canvas_PreviewMouseDown;
            canvas.PreviewMouseLeftButtonDown += Canvas_PreviewMouseLeftButtonDown;
            canvas.PreviewMouseLeftButtonUp += Canvas_PreviewMouseLeftButtonUp;
            canvas.PreviewMouseRightButtonDown += Canvas_PreviewMouseRightButtonDown;
            canvas.PreviewMouseMove += Canvas_PreviewMouseMove;
        }

        private void InitializeSnapMode()
        {
            snapModePoint.Click += (sender, e) => UpdateSnapMode();
            snapModeMiddle.Click += (sender, e) => UpdateSnapMode();
            snapModeNearest.Click += (sender, e) => UpdateSnapMode();
            snapModeIntersection.Click += (sender, e) => UpdateSnapMode();
            snapModeHorizontal.Click += (sender, e) => UpdateSnapMode();
            snapModeVertical.Click += (sender, e) => UpdateSnapMode();

            snapModePoint.IsChecked = _editor.State.SnapMode.HasFlag(GuideSnapMode.Point);
            snapModeMiddle.IsChecked = _editor.State.SnapMode.HasFlag(GuideSnapMode.Middle);
            snapModeNearest.IsChecked = _editor.State.SnapMode.HasFlag(GuideSnapMode.Nearest);
            snapModeIntersection.IsChecked = _editor.State.SnapMode.HasFlag(GuideSnapMode.Intersection);
            snapModeHorizontal.IsChecked = _editor.State.SnapMode.HasFlag(GuideSnapMode.Horizontal);
            snapModeVertical.IsChecked = _editor.State.SnapMode.HasFlag(GuideSnapMode.Vertical);
        }

        private void UpdateSnapMode()
        {
            if (snapModePoint.IsChecked == true)
                _editor.State.SnapMode |= GuideSnapMode.Point;
            else
                _editor.State.SnapMode &= ~GuideSnapMode.Point;

            if (snapModeMiddle.IsChecked == true)
                _editor.State.SnapMode |= GuideSnapMode.Middle;
            else
                _editor.State.SnapMode &= ~GuideSnapMode.Middle;

            if (snapModeNearest.IsChecked == true)
                _editor.State.SnapMode |= GuideSnapMode.Nearest;
            else
                _editor.State.SnapMode &= ~GuideSnapMode.Nearest;

            if (snapModeIntersection.IsChecked == true)
                _editor.State.SnapMode |= GuideSnapMode.Intersection;
            else
                _editor.State.SnapMode &= ~GuideSnapMode.Intersection;

            if (snapModeHorizontal.IsChecked == true)
                _editor.State.SnapMode |= GuideSnapMode.Horizontal;
            else
                _editor.State.SnapMode &= ~GuideSnapMode.Horizontal;

            if (snapModeVertical.IsChecked == true)
                _editor.State.SnapMode |= GuideSnapMode.Vertical;
            else
                _editor.State.SnapMode &= ~GuideSnapMode.Vertical;
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
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
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
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
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
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
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
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
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
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void SpiroControl_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Focus();
        }

        private void New()
        {
            var drawing = SpiroDrawing.Create(600, 600);

            _editor.LoadDrawing(drawing);
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
                    var path = dlg.FileName;

                    switch (dlg.FilterIndex)
                    {
                        default:
                        case 1:
                            OpenDrawing(path);
                            break;
                        case 2:
                            OpenPlate(path);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
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
                    var path = dlg.FileName;

                    switch (dlg.FilterIndex)
                    {
                        default:
                        case 1:
                            SaveAsDrawing(path);
                            break;
                        case 2:
                            SaveAsPlate(path);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
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
                    var path = dlg.FileName;

                    switch (dlg.FilterIndex)
                    {
                        default:
                        case 1:
                            ExportAsSvg(path);
                            break;
                        case 2:
                            ExportAsPs(path);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private void OpenDrawing(string path)
        {
            try
            {
                using (var f = System.IO.File.OpenText(path))
                {
                    string json = f.ReadToEnd();
                    var drawing = JsonSerializer.Deserialize<SpiroDrawing>(json);
                    if (drawing != null)
                    {
                        _editor.LoadDrawing(drawing);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void OpenPlate(string path)
        {
            try
            {
                using (var f = System.IO.File.OpenText(path))
                {
                    string plate = f.ReadToEnd();
                    var drawing = _editor.FromPlateString(plate);
                    if (drawing != null)
                    {
                        _editor.LoadDrawing(drawing);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void SaveAsDrawing(string path)
        {
            try
            {
                using (var f = System.IO.File.CreateText(path))
                {
                    var json = JsonSerializer.Serialize(_editor.Drawing);
                    f.Write(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void SaveAsPlate(string path)
        {
            try
            {
                using (var f = System.IO.File.CreateText(path))
                {
                    string plate = _editor.ToPlateString();
                    f.Write(plate);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void ExportAsSvg(string path)
        {
            try
            {
                using (var f = System.IO.File.CreateText(path))
                {
                    string svg = _editor.ToSvgString();
                    f.Write(svg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void ExportAsPs(string path)
        {
            try
            {
                using (var f = System.IO.File.CreateText(path))
                {
                    string ps = _editor.ToPsString();
                    f.Write(ps);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void Exit()
        {
            Application.Current.Windows[0].Close();
        }
    }
}

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
using SpiroNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpiroNet.Wpf
{
    public partial class EditorView : UserControl
    {
        private EditorViewModel _vm;

        public EditorView()
        {
            InitializeComponent();

            InitializeEditor();
            InitializeCanvas();
            InitializeSnapMode();

            DataContext = _vm;

            Loaded += SpiroControl_Loaded;
        }

        private void InitializeEditor()
        {
            _vm = new EditorViewModel();

            _vm.Editor = new SpiroEditor()
            {
                State = new EditorState(),
                Measure = new EditorMeasure(),
                Invalidate = () => canvas.InvalidateVisual(),
                Capture = () => canvas.CaptureMouse(),
                Release = () => canvas.ReleaseMouseCapture(),
                Drawing = SpiroDrawing.Create(600, 600),
                Data = new Dictionary<SpiroShape, string>(),
                Knots = new Dictionary<SpiroShape, IList<SpiroKnot>>()
            };

            _vm.InvalidateCommand = Command.Create(_vm.Editor.Invalidate);
            _vm.NewCommand = Command.Create(New);
            _vm.OpenCommand = Command.Create(Open);
            _vm.SaveAsCommand = Command.Create(SaveAs);
            _vm.ExportCommand = Command.Create(Export);
            _vm.ExitCommand = Command.Create(Exit);
            _vm.DeleteCommand = Command.Create(_vm.Editor.Delete);
            _vm.IsStrokedCommand = Command.Create(_vm.Editor.ToggleIsStroked);
            _vm.IsFilledCommand = Command.Create(_vm.Editor.ToggleIsFilled);
            _vm.IsClosedCommand = Command.Create(_vm.Editor.ToggleIsClosed);
            _vm.IsTaggedCommand = Command.Create(_vm.Editor.ToggleIsTagged);
            _vm.PointTypeCommand = Command<string>.Create(_vm.Editor.TogglePointType);
            _vm.ExecuteScriptCommand = Command<string>.Create(_vm.Editor.ExecuteScript);
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

            snapModePoint.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Point);
            snapModeMiddle.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Middle);
            snapModeNearest.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Nearest);
            snapModeIntersection.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Intersection);
            snapModeHorizontal.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Horizontal);
            snapModeVertical.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Vertical);
        }

        private void UpdateSnapMode()
        {
            if (snapModePoint.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Point;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Point;

            if (snapModeMiddle.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Middle;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Middle;

            if (snapModeNearest.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Nearest;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Nearest;

            if (snapModeIntersection.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Intersection;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Intersection;

            if (snapModeHorizontal.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Horizontal;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Horizontal;

            if (snapModeVertical.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Vertical;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Vertical;
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            var point = e.GetPosition(canvas);

            try
            {
                if (e.ChangedButton == MouseButton.Middle)
                {
                    _vm.Editor.MiddleDown(point.X, point.Y);
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
                _vm.Editor.LeftDown(point.X, point.Y);
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
                _vm.Editor.LeftUp(point.X, point.Y);
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
                _vm.Editor.RightDown(point.X, point.Y);
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
                _vm.Editor.Move(point.X, point.Y);
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

            _vm.Editor.LoadDrawing(drawing);
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
                            _vm.OpenDrawing(path);
                            break;
                        case 2:
                            _vm.OpenPlate(path);
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
                            _vm.SaveAsDrawing(path);
                            break;
                        case 2:
                            _vm.SaveAsPlate(path);
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
                            _vm.ExportAsSvg(path);
                            break;
                        case 2:
                            _vm.ExportAsPs(path);
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

        private void Exit()
        {
            Application.Current.Windows[0].Close();
        }
    }
}

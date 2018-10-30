/*
SpiroNet.Avalonia
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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using SpiroNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpiroNet.Editor.Avalonia.Views
{
    public class EditorView : UserControl
    {
        private EditorViewModel _vm;
        private Canvas _canvas;
        private CheckBox _snapModePoint;
        private CheckBox _snapModeMiddle;
        private CheckBox _snapModeNearest;
        private CheckBox _snapModeIntersection;
        private CheckBox _snapModeHorizontal;
        private CheckBox _snapModeVertical;

        public EditorView()
        {
            this.InitializeComponent();

            FindControls();

            InitializeEditor();
            InitializeCanvas();
            InitializeSnapMode();

            DataContext = _vm;

            AttachedToVisualTree += EditorView_AttachedToVisualTree;
        }

        private void FindControls()
        {
            _canvas = this.FindControl<Canvas>("canvas");
            _snapModePoint = this.FindControl<CheckBox>("snapModePoint");
            _snapModeMiddle = this.FindControl<CheckBox>("snapModeMiddle");
            _snapModeNearest = this.FindControl<CheckBox>("snapModeNearest");
            _snapModeIntersection = this.FindControl<CheckBox>("snapModeIntersection");
            _snapModeHorizontal = this.FindControl<CheckBox>("snapModeHorizontal");
            _snapModeVertical = this.FindControl<CheckBox>("snapModeVertical");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InitializeEditor()
        {
            _vm = new EditorViewModel();

            _vm.Editor = new SpiroEditor()
            {
                State = new EditorState(),
                Measure = new EditorMeasure(),
                Invalidate = () => _canvas.InvalidateVisual(),
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
            _vm.ToolCommand = Command<string>.Create(_vm.Editor.ToggleTool);
            _vm.PointTypeCommand = Command<string>.Create(_vm.Editor.TogglePointType);
            _vm.ExecuteScriptCommand = Command<string>.Create(_vm.Editor.ExecuteScript);
        }

        private void InitializeCanvas()
        {
            _canvas.PointerPressed += Canvas_PointerPressed;
            _canvas.PointerReleased += Canvas_PointerReleased;
            _canvas.PointerMoved += Canvas_PointerMoved;
        }

        private void InitializeSnapMode()
        {
            _snapModePoint.Click += (sender, e) => UpdateSnapMode();
            _snapModeMiddle.Click += (sender, e) => UpdateSnapMode();
            _snapModeNearest.Click += (sender, e) => UpdateSnapMode();
            _snapModeIntersection.Click += (sender, e) => UpdateSnapMode();
            _snapModeHorizontal.Click += (sender, e) => UpdateSnapMode();
            _snapModeVertical.Click += (sender, e) => UpdateSnapMode();

            _snapModePoint.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Point);
            _snapModeMiddle.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Middle);
            _snapModeNearest.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Nearest);
            _snapModeIntersection.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Intersection);
            _snapModeHorizontal.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Horizontal);
            _snapModeVertical.IsChecked = _vm.Editor.State.SnapMode.HasFlag(GuideSnapMode.Vertical);
        }

        private void UpdateSnapMode()
        {
            if (_snapModePoint.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Point;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Point;

            if (_snapModeMiddle.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Middle;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Middle;

            if (_snapModeNearest.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Nearest;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Nearest;

            if (_snapModeIntersection.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Intersection;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Intersection;

            if (_snapModeHorizontal.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Horizontal;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Horizontal;

            if (_snapModeVertical.IsChecked == true)
                _vm.Editor.State.SnapMode |= GuideSnapMode.Vertical;
            else
                _vm.Editor.State.SnapMode &= ~GuideSnapMode.Vertical;
        }

        private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            switch (e.MouseButton)
            {
                case MouseButton.Left:
                    {
                        _canvas.Focus();
                        var point = e.GetPosition(_canvas);

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
                    break;
                case MouseButton.Right:
                    {
                        _canvas.Focus();
                        var point = e.GetPosition(_canvas);

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
                    break;
                case MouseButton.Middle:
                    {
                        _canvas.Focus();
                        var point = e.GetPosition(_canvas);

                        try
                        {
                            _vm.Editor.MiddleDown(point.X, point.Y);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                    break;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            switch (e.MouseButton)
            {
                case MouseButton.Left:
                    {
                        var point = e.GetPosition(_canvas);

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
                    break;
                case MouseButton.Right:
                    break;
                case MouseButton.Middle:
                    break;
            }
        }

        private void Canvas_PointerMoved(object sender, PointerEventArgs e)
        {
            _canvas.Focus();
            var point = e.GetPosition(_canvas);

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

        private void EditorView_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _canvas.Focus();
        }

        private void New()
        {
            var drawing = SpiroDrawing.Create(600, 600);

            _vm.Editor.LoadDrawing(drawing);
        }

        private async void Open()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Spiro", Extensions = { "spiro" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Plate", Extensions = { "plate" } });
            var result = await dlg.ShowAsync(Application.Current.Windows.FirstOrDefault());
            if (result != null)
            {
                try
                {
                    string path = result.FirstOrDefault();
                    string extension = System.IO.Path.GetExtension(path);
                    if (string.Compare(extension, ".spiro", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _vm.OpenDrawing(path);
                    }
                    else if (string.Compare(extension, ".plate", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _vm.OpenPlate(path);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private async void SaveAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Spiro", Extensions = { "spiro" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Plate", Extensions = { "plate" } });
            dlg.InitialFileName = "drawing.spiro";
            dlg.DefaultExtension = "spiro";
            var result = await dlg.ShowAsync(Application.Current.Windows.FirstOrDefault());
            if (result != null)
            {
                try
                {
                    string path = result;
                    string extension = System.IO.Path.GetExtension(path);
                    if (string.Compare(extension, ".spiro", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _vm.SaveAsDrawing(path);
                    }
                    else if (string.Compare(extension, ".plate", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _vm.SaveAsPlate(path);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private async void Export()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Ps", Extensions = { "ps" } });
            dlg.InitialFileName = "drawing.svg";
            dlg.DefaultExtension = "svg";
            var result = await dlg.ShowAsync(Application.Current.Windows.FirstOrDefault());
            if (result != null)
            {
                try
                {
                    string path = result;
                    string extension = System.IO.Path.GetExtension(path);
                    if (string.Compare(extension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _vm.ExportAsSvg(path);
                    }
                    else if (string.Compare(extension, ".ps", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        _vm.ExportAsPs(path);
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
            Application.Current.Exit();
        }
    }
}

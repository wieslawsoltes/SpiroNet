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
using SpiroNet.Editor;
using SpiroNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpiroNet.Avalonia.Views
{
    public class EditorView : UserControl
    {
        private EditorViewModel _vm;
        private Canvas canvas;
        private CheckBox snapModePoint;
        private CheckBox snapModeMiddle;
        private CheckBox snapModeNearest;
        private CheckBox snapModeIntersection;
        private CheckBox snapModeHorizontal;
        private CheckBox snapModeVertical;

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
            canvas = this.FindControl<Canvas>("canvas");
            snapModePoint = this.FindControl<CheckBox>("snapModePoint");
            snapModeMiddle = this.FindControl<CheckBox>("snapModeMiddle");
            snapModeNearest = this.FindControl<CheckBox>("snapModeNearest");
            snapModeIntersection = this.FindControl<CheckBox>("snapModeIntersection");
            snapModeHorizontal = this.FindControl<CheckBox>("snapModeHorizontal");
            snapModeVertical = this.FindControl<CheckBox>("snapModeVertical");
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
                Invalidate = () => canvas.InvalidateVisual(),
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
            canvas.PointerPressed += Canvas_PointerPressed;
            canvas.PointerReleased += Canvas_PointerReleased;
            canvas.PointerMoved += Canvas_PointerMoved;
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

        private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            switch (e.MouseButton)
            {
                case MouseButton.Left:
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
                    break;
                case MouseButton.Right:
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
                    break;
                case MouseButton.Middle:
                    {
                        canvas.Focus();
                        var point = e.GetPosition(canvas);

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
                    break;
                case MouseButton.Right:
                    break;
                case MouseButton.Middle:
                    break;
            }
        }

        private void Canvas_PointerMoved(object sender, PointerEventArgs e)
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

        private void EditorView_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            canvas.Focus();
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
            var result = await dlg.ShowAsync(Window.OpenWindows.FirstOrDefault());
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
            var result = await dlg.ShowAsync(Window.OpenWindows.FirstOrDefault());
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
            var result = await dlg.ShowAsync(Window.OpenWindows.FirstOrDefault());
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

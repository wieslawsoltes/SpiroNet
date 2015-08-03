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
using SpiroNet;

namespace SpiroNet.Wpf
{
    public partial class SpiroControl : UserControl
    {
        private PathShape _shape = null;

        public SpiroControl()
        {
            InitializeComponent();
            
            InitializeMenu();
            InitializeOptions();
            InitializeCanvas();
            InitializeShortcuts();

            Loaded += (sender, e) => canvas.Focus();
        }

        private void InitializeMenu()
        {
            fileNew.Click += fileNew_Click;
            fileOpen.Click += fileOpen_Click;
            fileSaveAs.Click += fileSaveAs_Click;
            fileExportAsSvg.Click += fileExportAsSvg_Click;
            fileExit.Click += fileExit_Click;
        }

        private void Open(string path)
        {
            using (var f = System.IO.File.OpenText(path))
            {
                var json = f.ReadToEnd();
                var drawing = JsonSerializer.Deserialize<PathDrawing>(json);
                Load(drawing);
            }
        }

        private void Load(PathDrawing drawing)
        {
            canvas.Width = drawing.Width;
            canvas.Height = drawing.Height;
            canvas.Shapes = drawing.Shapes;
            canvas.Data = new Dictionary<PathShape, string>();

            foreach (var shape in canvas.Shapes)
            {
                UpdateData(shape);
            }

            canvas.InvalidateVisual();
            BindingOperations.GetBindingExpressionBase(shapesListBox, ItemsControl.ItemsSourceProperty).UpdateTarget();
            BindingOperations.GetBindingExpressionBase(dataTextBox, TextBox.TextProperty).UpdateTarget();
        }

        private void SaveAs(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var drawing = new PathDrawing()
                {
                    Width = canvas.Width,
                    Height = canvas.Height,
                    Shapes = canvas.Shapes
                };
                var json = JsonSerializer.Serialize(drawing);
                f.Write(json);
            }
        }
        
        private void ExportAsSvg(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var sb = new StringBuilder();
                var suffix = Environment.NewLine + "           ";

                sb.AppendLine(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\"  width=\"{1}\" height=\"{0}\">", canvas.Width, canvas.Height));
                foreach (var shape in canvas.Shapes)
                {
                    sb.AppendLine("  <path stroke=\"rgba(0,0,0,255)\"");
                    sb.AppendLine("        stroke-width=\"2\"");
                    sb.AppendLine(shape.IsClosed ? "        fill=\"rgba(128,128,128,128)\"" : "        fill=\"none\"");
                    sb.AppendLine(string.Format("        d=\"{0}\"/>", canvas.Data[shape].Replace(Environment.NewLine, suffix)));
                }
                sb.AppendLine("</svg>");

                f.Write(sb);
            }
        }

        private void fileNew_Click(object sender, RoutedEventArgs e)
        {
            canvas.Shapes = new ObservableCollection<PathShape>();
            canvas.Data = new Dictionary<PathShape, string>();
            canvas.InvalidateVisual();
            BindingOperations.GetBindingExpressionBase(shapesListBox, ItemsControl.ItemsSourceProperty).UpdateTarget();
            BindingOperations.GetBindingExpressionBase(dataTextBox, TextBox.TextProperty).UpdateTarget();
        }
        
        private void fileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "SPIRO Files (*.spiro)|*.spiro|All Files (*.*)|*.*";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                Open(dlg.FileName);
            }
        }
        
        private void fileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "SPIRO Files (*.spiro)|*.spiro|All Files (*.*)|*.*";
            dlg.FileName = "drawing.spiro";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                SaveAs(dlg.FileName);
            }
        }
        
        private void fileExportAsSvg_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "SVG Files (*.svg)|*.svg|All Files (*.*)|*.*";
            dlg.FileName = "drawing.svg";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                ExportAsSvg(dlg.FileName);
            }
        }
 
        private void fileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Windows[0].Close();
        }

        private void InitializeOptions()
        {
            isClosedCheckBox.Click += isClosedCheckBox_Click;
            isTaggedCheckBox.Click += isTaggedCheckBox_Click;
            cornerPointRadioButton.Click += cornerPointRadioButton_Click;
            g4PointRadioButton.Click += g4PointRadioButton_Click;
            g2PointRadioButton.Click += g2PointRadioButton_Click;
            leftPointRadioButton.Click += leftPointRadioButton_Click;
            rightPointRadioButton.Click += rightPointRadioButton_Click;
            endPointRadioButton.Click += endPointRadioButton_Click;
            openContourPointRadioButton.Click += openContourPointRadioButton_Click;
            endOpenContourPointRadioButton.Click += endOpenContourPointRadioButton_Click;
        }
 
        private void isClosedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (_shape != null)
            {
                _shape.IsClosed = isClosedCheckBox.IsChecked == true;
                UpdateData(_shape);
                canvas.InvalidateVisual();
            }
        }

        private void isTaggedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (_shape != null)
            {
                _shape.IsTagged = isTaggedCheckBox.IsChecked == true;
                UpdateData(_shape);
                canvas.InvalidateVisual();
            }
        }
        
        private void cornerPointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.Corner);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }

        private void g4PointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.G4);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }
        
        private void g2PointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.G2);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }

        private void leftPointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.Left);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }

        private void rightPointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.Right);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }

        private void endPointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.End);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }

        private void openContourPointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.OpenContour);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }
 
        private void endOpenContourPointRadioButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousPointType(SpiroPointType.EndOpenContour);
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }
        
        private void InitializeCanvas()
        {
            canvas.Shapes = new ObservableCollection<PathShape>();
            canvas.Data = new Dictionary<PathShape, string>();
            canvas.PreviewMouseLeftButtonDown += Canvas_PreviewMouseLeftButtonDown;
            canvas.PreviewMouseRightButtonDown += Canvas_PreviewMouseRightButtonDown;
            canvas.PreviewMouseMove += Canvas_PreviewMouseMove;
        }

        private SpiroPointType GetSpiroPointType()
        {
            if (cornerPointRadioButton.IsChecked == true)
                return SpiroPointType.Corner;
            else if (g4PointRadioButton.IsChecked == true)
                return SpiroPointType.G4;
            else if (g2PointRadioButton.IsChecked == true)
                return SpiroPointType.G2;
            else if (leftPointRadioButton.IsChecked == true)
                return SpiroPointType.Left;
            else if (rightPointRadioButton.IsChecked == true)
                return SpiroPointType.Right;
            else if (endPointRadioButton.IsChecked == true)
                return SpiroPointType.End;
            else if (openContourPointRadioButton.IsChecked == true)
                return SpiroPointType.OpenContour;
            else if (endOpenContourPointRadioButton.IsChecked == true)
                return SpiroPointType.EndOpenContour;

            return SpiroPointType.G4;
        }

        private void NewShape()
        {
            _shape = new PathShape();
            _shape.IsClosed = isClosedCheckBox.IsChecked == true;
            _shape.IsTagged = isTaggedCheckBox.IsChecked == true;
            _shape.Points = new ObservableCollection<SpiroControlPoint>();
            canvas.Shapes.Add(_shape);
        }

        private void NewPoint(Point p)
        {
            var point = new SpiroControlPoint();
            point.X = p.X;
            point.Y = p.Y;
            point.Type = GetSpiroPointType();
            _shape.Points.Add(point);
        }

        private void SetLastPointPosition(Point p)
        {
            if (_shape == null || _shape.Points.Count < 1)
                return;
            
            var point = new SpiroControlPoint();
            point.X = p.X;
            point.Y = p.Y;
            point.Type = GetSpiroPointType();
            _shape.Points[_shape.Points.Count - 1] = point;
        }
        
        private void SetPreviousPointType(SpiroPointType type)
        {
            if (_shape == null || _shape.Points.Count < 2)
                return;
            
            var old = _shape.Points[_shape.Points.Count - 2];
            var point = new SpiroControlPoint();
            point.X = old.X;
            point.Y = old.Y;
            point.Type = type;
            _shape.Points[_shape.Points.Count - 2] = point;
        }

        private void UpdateData(PathShape shape)
        {
            try
            {
                if (canvas.Data.ContainsKey(shape))
                {
                    string data;
                    if (shape.TryGetData(out data))
                    {
                        canvas.Data[shape] = data;
                    }
                    else
                    {
                        canvas.Data[shape] = null;
                    }
                }
                else
                {
                    string data;
                    if (shape.TryGetData(out data))
                    {
                        canvas.Data.Add(shape, data);
                    }
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
            if (_shape == null)
                NewShape();

            NewPoint(e.GetPosition(canvas));
            UpdateData(_shape);
            canvas.InvalidateVisual();
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            if (_shape != null)
            {
                UpdateData(_shape);
                canvas.InvalidateVisual();
                _shape = null;
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            canvas.Focus();
            if (_shape != null && _shape.Points.Count > 1)
            {
                SetLastPointPosition(e.GetPosition(canvas));
                UpdateData(_shape);
                canvas.InvalidateVisual();
            }
        }
        
        private void InitializeShortcuts()
        {
            PreviewKeyDown += SpiroControl_PreviewKeyDown;;
        }

        private void SpiroControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!canvas.IsFocused)
                return;
            
            switch (e.Key) 
            {
                case Key.S:
                    {
                        isClosedCheckBox.IsChecked = !(isClosedCheckBox.IsChecked == true);
                        if (_shape != null)
                        {
                            _shape.IsClosed = isClosedCheckBox.IsChecked == true;
                            UpdateData(_shape);
                            canvas.InvalidateVisual();
                        }
                    }
                    break;
                case Key.T:
                    {
                        isTaggedCheckBox.IsChecked = !(isTaggedCheckBox.IsChecked == true);
                        if (_shape != null)
                        {
                            _shape.IsTagged = isTaggedCheckBox.IsChecked == true;
                            UpdateData(_shape);
                            canvas.InvalidateVisual();
                        }
                    }
                    break;
                case Key.V:
                    {
                        cornerPointRadioButton.IsChecked = true;
                        SetPreviousPointType(SpiroPointType.Corner);
                        UpdateData(_shape);
                        canvas.InvalidateVisual();
                    }
                    break;
                case Key.O:
                    {
                        g4PointRadioButton.IsChecked = true;
                        SetPreviousPointType(SpiroPointType.G4);
                        UpdateData(_shape);
                        canvas.InvalidateVisual();
                    }
                    break;
                case Key.C:
                    {
                        g2PointRadioButton.IsChecked = true;
                        SetPreviousPointType(SpiroPointType.G2);
                        UpdateData(_shape);
                        canvas.InvalidateVisual();
                    }
                    break;
                case Key.OemOpenBrackets:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            openContourPointRadioButton.IsChecked = true;
                            SetPreviousPointType(SpiroPointType.OpenContour);
                        }
                        else
                        {
                            leftPointRadioButton.IsChecked = true;
                            SetPreviousPointType(SpiroPointType.Left);
                        }

                        UpdateData(_shape);
                        canvas.InvalidateVisual();
                    }
                    break;
                case Key.OemCloseBrackets:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            endOpenContourPointRadioButton.IsChecked = true;
                            SetPreviousPointType(SpiroPointType.EndOpenContour);
                        }
                        else
                        {
                            rightPointRadioButton.IsChecked = true;
                            SetPreviousPointType(SpiroPointType.Right);
                        }
 
                        UpdateData(_shape);
                        canvas.InvalidateVisual();
                    }
                    break;
                case Key.Z:
                    {
                        endPointRadioButton.IsChecked = true;
                        SetPreviousPointType(SpiroPointType.End);
                        UpdateData(_shape);
                        canvas.InvalidateVisual();
                    }
                    break;
            }
        }
    }
}

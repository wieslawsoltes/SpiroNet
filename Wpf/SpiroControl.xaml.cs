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
        private SpiroShape _shape = null;

        public SpiroControl()
        {
            InitializeComponent();
            InitializeCanvas();
        }

        private void InitializeCanvas()
        {
            canvas.Shapes = new ObservableCollection<SpiroShape>();
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
            _shape = new SpiroShape();
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

        private void UpdateLastPoint(Point p)
        {
            var point = new SpiroControlPoint();
            point.X = p.X;
            point.Y = p.Y;
            point.Type = GetSpiroPointType();
            _shape.Points[_shape.Points.Count - 1] = point;
        }

        private static bool ConvertPointsToPath(SpiroShape shape)
        {
            var points = shape.Points.ToArray();
            var bc = new PathBezierContext();

            try
            {
                if (shape.IsTagged)
                {
                    var success = Spiro.TaggedSpiroCPsToBezier(points, bc);
                    if (success)
                        shape.Source = bc.ToString();
                    else
                        shape.Source = string.Empty;

                    return success;
                }
                else
                {
                    var success = Spiro.SpiroCPsToBezier(points, points.Length, shape.IsClosed, bc);
                    if (success)
                        shape.Source = bc.ToString();
                    else
                        shape.Source = string.Empty;

                    return success;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }

            return false;
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_shape == null)
                NewShape();

            NewPoint(e.GetPosition(canvas));
            ConvertPointsToPath(_shape);
            canvas.InvalidateVisual();
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_shape != null)
            {
                ConvertPointsToPath(_shape);
                canvas.InvalidateVisual();
                _shape = null;
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_shape != null && _shape.Points.Count > 1)
            {
                UpdateLastPoint(e.GetPosition(canvas));
                ConvertPointsToPath(_shape);
                canvas.InvalidateVisual();
            }
        }
    }
}

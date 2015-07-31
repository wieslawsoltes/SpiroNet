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
                _shape = null;
            }
        }
    }
}

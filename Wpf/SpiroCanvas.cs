using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SpiroNet.Wpf
{
    public class SpiroCanvas : Canvas
    {
        public IList<SpiroShape> Shapes { get; set; }

        private Brush _geometryBrush;
        private Brush _geometryPenBrush;
        private Pen _geometryPen;
        private Brush _pointBrush;

        public SpiroCanvas()
        {
            Initialize();
        }

        private void Initialize()
        {
            _geometryBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
            _geometryBrush.Freeze();
            _geometryPenBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            _geometryPenBrush.Freeze();
            _geometryPen = new Pen(_geometryPenBrush, 2.0);
            _geometryPen.Freeze();
            _pointBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            _pointBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            DrawShapes(dc);
        }

        private void DrawShapes(DrawingContext dc)
        {
            if (Shapes == null)
                return;

            foreach (var shape in Shapes)
            {
                DrawShape(dc, shape);
            }
        }

        private void DrawShape(DrawingContext dc, SpiroShape shape)
        {
            if (shape == null)
                return;

            if (!string.IsNullOrEmpty(shape.Source))
            {
                var geometry = Geometry.Parse(shape.Source);
                dc.DrawGeometry(shape.IsClosed ? _geometryBrush : null, _geometryPen, geometry);
            }

            if (shape.Points == null)
                return;

            foreach (var point in shape.Points)
            {
                dc.DrawEllipse(_pointBrush, null, new Point(point.X, point.Y), 4.0, 4.0);
            }
        }
    }
}

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SpiroNet.Editor;

namespace SpiroNet.Wpf
{
    public class SpiroCanvas : Canvas
    {
        private static Geometry LeftKnot = Geometry.Parse("M0,-4 A 4,4 0 0 0 0,4");
        private static Geometry RightKnot = Geometry.Parse("M0,-4 A 4,4 0 0 1 0,4");
        private static Geometry EndKnot = Geometry.Parse("M-3.5,-3.5 L3.5,3.5 M-3.5,3.5 L3.5,-3.5");
        private static Geometry EndOpenContourKnot = Geometry.Parse("M-3.5,-3.5 L0,0 -3.5,3.5");
        private static Geometry OpenContourKnot = Geometry.Parse("M3.5,-3.5 L0,0 3.5,3.5");

        private Brush _geometryBrush;
        private Brush _geometryPenBrush;
        private Pen _geometryPen;
        private Brush _hitGeometryBrush;
        private Brush _hitGeometryPenBrush;
        private Pen _hitGeometryPen;
        private Brush _pointBrush;
        private Pen _pointPen;
        private Brush _hitPointBrush;
        private Pen _hitPointPen;

        public SpiroEditor Editor
        {
            get { return (SpiroEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register(
                "Editor",
                typeof(SpiroEditor),
                typeof(SpiroCanvas),
                new PropertyMetadata(null));

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

            _hitGeometryBrush = new SolidColorBrush(Color.FromArgb(128, 128, 0, 0));
            _hitGeometryBrush.Freeze();
            _hitGeometryPenBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            _hitGeometryPenBrush.Freeze();
            _hitGeometryPen = new Pen(_hitGeometryPenBrush, 2.0);
            _hitGeometryPen.Freeze();

            _pointBrush = new SolidColorBrush(Color.FromArgb(192, 0, 0, 255));
            _pointBrush.Freeze();
            _pointPen = new Pen(_pointBrush, 2.0);
            _pointPen.Freeze();

            _hitPointBrush = new SolidColorBrush(Color.FromArgb(192, 255, 0, 0));
            _hitPointBrush.Freeze();
            _hitPointPen = new Pen(_hitPointBrush, 2.0);
            _hitPointPen.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            DrawShapes(dc);
        }

        private void DrawShapes(DrawingContext dc)
        {
            if (Editor == null || Editor.Drawing == null || Editor.Drawing.Shapes == null)
                return;

            foreach (var shape in Editor.Drawing.Shapes)
            {
                DrawShape(dc, shape);

                if (Editor.State.DisplayKnots)
                {
                    DrawKnots(dc, shape);
                }
            }
        }

        private void DrawShape(DrawingContext dc, PathShape shape)
        {
            if (shape == null || Editor == null || Editor.Data == null)
                return;

            var hitShape = Editor.State.HitShape;
            var hitShapePointIndex = Editor.State.HitShapePointIndex;

            string data;
            var result = Editor.Data.TryGetValue(shape, out data);
            if (result && !string.IsNullOrEmpty(data))
            {
                var geometry = Geometry.Parse(data);
                if (shape == hitShape && hitShapePointIndex == -1)
                {
                    dc.DrawGeometry(
                        shape.IsFilled ? _hitGeometryBrush : null,
                        shape.IsStroked ? _hitGeometryPen : null,
                        geometry);
                }
                else
                {
                    dc.DrawGeometry(
                        shape.IsFilled ? _geometryBrush : null,
                        shape.IsStroked ? _geometryPen : null,
                        geometry);
                }
            }
        }

        private void DrawKnots(DrawingContext dc, PathShape shape)
        {
            if (shape == null || Editor == null)
                return;

            var hitShape = Editor.State.HitShape;
            var hitShapePointIndex = Editor.State.HitShapePointIndex;

            IList<SpiroKnot> knots;
            Editor.Knots.TryGetValue(shape, out knots);
            if (knots != null)
            {
                for (int i = 0; i < knots.Count; i++)
                {
                    var knot = knots[i];
                    var brush = shape == hitShape && i == hitShapePointIndex ? _hitPointBrush : _pointBrush;
                    var pen = shape == hitShape && i == hitShapePointIndex ? _hitPointPen : _pointPen;
                    DrawKnot(dc, brush, pen, knot);
                }
            }
            else
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    var point = shape.Points[i];
                    var brush = shape == hitShape && i == hitShapePointIndex ? _hitPointBrush : _pointBrush;
                    var pen = shape == hitShape && i == hitShapePointIndex ? _hitPointPen : _pointPen;
                    DrawPoint(dc, brush, pen, point);
                }
            }
        }

        private void DrawKnot(DrawingContext dc, Brush brush, Pen pen, SpiroKnot knot)
        {
            switch (knot.Type)
            {
                case SpiroPointType.Corner:
                    dc.DrawRectangle(brush, null, new Rect(knot.X - 3.5, knot.Y - 3.5, 7, 7));
                    break;
                case SpiroPointType.G4:
                    dc.DrawEllipse(brush, null, new Point(knot.X, knot.Y), 3.5, 3.5);
                    break;
                case SpiroPointType.G2:
                    dc.PushTransform(new RotateTransform(knot.Theta, knot.X, knot.Y));
                    dc.DrawRectangle(brush, null, new Rect(knot.X - 1.5, knot.Y - 3.5, 3, 7));
                    dc.Pop();
                    break;
                case SpiroPointType.Left:
                    dc.PushTransform(new RotateTransform(knot.Theta, knot.X, knot.Y));
                    dc.PushTransform(new TranslateTransform(knot.X, knot.Y));
                    dc.DrawGeometry(brush, null, LeftKnot);
                    dc.Pop();
                    dc.Pop();
                    break;
                case SpiroPointType.Right:
                    dc.PushTransform(new RotateTransform(knot.Theta, knot.X, knot.Y));
                    dc.PushTransform(new TranslateTransform(knot.X, knot.Y));
                    dc.DrawGeometry(brush, null, RightKnot);
                    dc.Pop();
                    dc.Pop();
                    break;
                case SpiroPointType.End:
                    dc.PushTransform(new RotateTransform(knot.Theta, knot.X, knot.Y));
                    dc.PushTransform(new TranslateTransform(knot.X, knot.Y));
                    dc.DrawGeometry(null, pen, EndKnot);
                    dc.Pop();
                    dc.Pop();
                    break;
                case SpiroPointType.OpenContour:
                    dc.PushTransform(new RotateTransform(knot.Theta, knot.X, knot.Y));
                    dc.PushTransform(new TranslateTransform(knot.X, knot.Y));
                    dc.DrawGeometry(null, pen, OpenContourKnot);
                    dc.Pop();
                    dc.Pop();
                    break;
                case SpiroPointType.EndOpenContour:
                    dc.PushTransform(new RotateTransform(knot.Theta, knot.X, knot.Y));
                    dc.PushTransform(new TranslateTransform(knot.X, knot.Y));
                    dc.DrawGeometry(null, pen, EndOpenContourKnot);
                    dc.Pop();
                    dc.Pop();
                    break;
            }
        }

        private void DrawPoint(DrawingContext dc, Brush brush, Pen pen, SpiroControlPoint point)
        {
            switch (point.Type)
            {
                case SpiroPointType.Corner:
                    dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                    break;
                case SpiroPointType.G4:
                case SpiroPointType.G2:
                case SpiroPointType.Left:
                case SpiroPointType.Right:
                case SpiroPointType.End:
                case SpiroPointType.OpenContour:
                case SpiroPointType.EndOpenContour:
                    dc.PushTransform(new TranslateTransform(point.X, point.Y));
                    dc.DrawGeometry(null, pen, EndKnot);
                    dc.Pop();
                    break;
            }
        }
    }
}

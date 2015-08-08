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

        private IDictionary<BasicStyle, BasicStyleCache> _cache;

        private BasicStyle _geometryStyle;
        private BasicStyle _hitGeometryStyle;
        private BasicStyle _pointStyle;
        private BasicStyle _hitPointStyle;

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
            _cache = new Dictionary<BasicStyle, BasicStyleCache>();

            _geometryStyle = new BasicStyle(
                new Argb(255, 0, 0, 0),
                new Argb(128, 128, 128, 128),
                2.0);

            _hitGeometryStyle = new BasicStyle(
                new Argb(255, 255, 0, 0),
                new Argb(128, 128, 0, 0),
                2.0);

            _pointStyle = new BasicStyle(
                new Argb(192, 0, 0, 255),
                new Argb(192, 0, 0, 255),
                2.0);

            _hitPointStyle = new BasicStyle(
                new Argb(192, 255, 0, 0),
                new Argb(192, 255, 0, 0),
                2.0);
        }

        private BasicStyleCache FromCache(BasicStyle style)
        {
            BasicStyleCache value;
            if (_cache.TryGetValue(style, out value))
            {
                return value;
            }
            value = new BasicStyleCache(style);
            _cache.Add(style, value);
            return value;
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
                    var cache = FromCache(_hitGeometryStyle);
                    dc.DrawGeometry(
                        shape.IsFilled ? cache.FillBrush : null,
                        shape.IsStroked ? cache.StrokePen : null,
                        geometry);
                }
                else
                {
                    var cache = FromCache(_geometryStyle);
                    dc.DrawGeometry(
                        shape.IsFilled ? cache.FillBrush : null,
                        shape.IsStroked ? cache.StrokePen : null,
                        geometry);
                }
            }
        }

        private void DrawKnots(DrawingContext dc, PathShape shape)
        {
            if (shape == null || Editor == null)
                return;

            var pointCache = FromCache(_pointStyle);
            var hitPointCache = FromCache(_hitPointStyle);

            var hitShape = Editor.State.HitShape;
            var hitShapePointIndex = Editor.State.HitShapePointIndex;

            IList<SpiroKnot> knots;
            Editor.Knots.TryGetValue(shape, out knots);
            if (knots != null)
            {
                for (int i = 0; i < knots.Count; i++)
                {
                    var knot = knots[i];
                    var brush = shape == hitShape && i == hitShapePointIndex ? hitPointCache.FillBrush : pointCache.FillBrush;
                    var pen = shape == hitShape && i == hitShapePointIndex ? hitPointCache.StrokePen : pointCache.StrokePen;
                    DrawKnot(dc, brush, pen, knot);
                }
            }
            else
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    var point = shape.Points[i];
                    var brush = shape == hitShape && i == hitShapePointIndex ? hitPointCache.FillBrush : pointCache.FillBrush;
                    var pen = shape == hitShape && i == hitShapePointIndex ? hitPointCache.StrokePen : pointCache.StrokePen;
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

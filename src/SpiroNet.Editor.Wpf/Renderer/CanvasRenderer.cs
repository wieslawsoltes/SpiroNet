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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SpiroNet.Editor.Wpf.Renderer
{
    public class CanvasRenderer : Canvas
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

        private BasicStyle _guideStyle;
        private BasicStyle _snapGuideStyle;
        private BasicStyle _snapPointStyle;
        private BasicStyle _newLineStyle;
        private BasicStyle _lineStyle;

        public SpiroEditor SpiroEditor
        {
            get { return (SpiroEditor)GetValue(SpiroEditorProperty); }
            set { SetValue(SpiroEditorProperty, value); }
        }

        public static readonly DependencyProperty SpiroEditorProperty = DependencyProperty.Register("SpiroEditor", typeof(SpiroEditor), typeof(CanvasRenderer), new PropertyMetadata(null));

        public CanvasRenderer()
        {
            InitializeStyles();
        }

        private void InitializeStyles()
        {
            _cache = new Dictionary<BasicStyle, BasicStyleCache>();

            // Spiro styles.
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

            // Guide styles.
            _guideStyle = new BasicStyle(
                new Argb(255, 0, 255, 255),
                new Argb(255, 0, 255, 255),
                1.0);
            _snapGuideStyle = new BasicStyle(
                new Argb(255, 255, 255, 0),
                new Argb(255, 255, 255, 0),
                1.0);
            _snapPointStyle = new BasicStyle(
                new Argb(255, 255, 255, 0),
                new Argb(255, 255, 255, 0),
                1.0);
            _newLineStyle = new BasicStyle(
                new Argb(255, 255, 255, 0),
                new Argb(255, 255, 255, 0),
                1.0);
            _lineStyle = new BasicStyle(
                new Argb(255, 0, 255, 255),
                new Argb(255, 0, 255, 255),
                1.0);
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

            if (SpiroEditor != null && SpiroEditor.Drawing != null)
            {
                if (SpiroEditor.Drawing.Guides != null && SpiroEditor.State.DisplayGuides)
                {
                    DrawGuides(dc);
                }

                var state = SpiroEditor.State;
                if (SpiroEditor.State.DisplayGuides && (state.Tool == EditorTool.Guide || state.Tool == EditorTool.Spiro))
                {
                    if ((state.Tool == EditorTool.Spiro && state.EnableSnap)
                        || (state.Tool == EditorTool.Guide && state.IsCaptured)
                        || (state.Tool == EditorTool.Guide && state.HaveSnapPoint))
                    {
                        DrawHorizontalGuide(dc,
                            FromCache(state.HaveSnapPoint ? _snapGuideStyle : _guideStyle),
                            state.GuidePosition,
                            SpiroEditor.Drawing.Width);

                        DrawVerticalGuide(dc,
                            FromCache(state.HaveSnapPoint ? _snapGuideStyle : _guideStyle),
                            state.GuidePosition,
                            SpiroEditor.Drawing.Height);
                    }

                    if (state.Tool == EditorTool.Guide && state.HaveSnapPoint)
                    {
                        DrawGuidePoint(
                            dc,
                            FromCache(_snapPointStyle),
                            SpiroEditor.State.SnapPoint,
                            SpiroEditor.State.SnapPointRadius);
                    }

                    if (state.Tool == EditorTool.Guide && state.IsCaptured)
                    {
                        DrawGuideLine(
                            dc,
                            FromCache(_newLineStyle),
                            SpiroEditor.Measure.Point0,
                            SpiroEditor.Measure.Point1);
                    }
                }

                if (SpiroEditor.Drawing.Shapes != null)
                {
                    DrawSpiroShapes(dc);
                }
            }
        }

        private void DrawSpiroShapes(DrawingContext dc)
        {
            var state = SpiroEditor.State;

            foreach (var shape in SpiroEditor.Drawing.Shapes)
            {
                if (!state.HitSetShapes.Contains(shape))
                {
                    DrawSpiroShape(dc, shape, false);

                    if (SpiroEditor.State.DisplayKnots)
                    {
                        DrawSpiroKnots(dc, shape, false, -1);
                    }
                }
            }

            for (int i = 0; i < state.HitListShapes.Count; i++)
            {
                var shape = state.HitListShapes[i];
                var index = state.HitListPoints[i];
                bool isSelected = index == -1;

                DrawSpiroShape(dc, shape, isSelected);

                if (SpiroEditor.State.DisplayKnots)
                {
                    DrawSpiroKnots(dc, shape, true, index);
                }
            }
        }

        private void DrawSpiroShape(DrawingContext dc, SpiroShape shape, bool isSelected)
        {
            string data;
            var result = SpiroEditor.Data.TryGetValue(shape, out data);
            if (result && !string.IsNullOrEmpty(data))
            {
                var geometry = Geometry.Parse(data);
                if (isSelected)
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

        private void DrawSpiroKnots(DrawingContext dc, SpiroShape shape, bool shapeIsSelected, int index)
        {
            var pointCache = FromCache(_pointStyle);
            var hitPointCache = FromCache(_hitPointStyle);

            IList<SpiroKnot> knots;
            SpiroEditor.Knots.TryGetValue(shape, out knots);
            if (knots != null)
            {
                for (int i = 0; i < knots.Count; i++)
                {
                    var knot = knots[i];
                    var brush = shapeIsSelected && i == index ? hitPointCache.FillBrush : pointCache.FillBrush;
                    var pen = shapeIsSelected && i == index ? hitPointCache.StrokePen : pointCache.StrokePen;
                    DrawSpiroKnot(dc, brush, pen, knot);
                }
            }
            else
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    var point = shape.Points[i];
                    var brush = shapeIsSelected && i == index ? hitPointCache.FillBrush : pointCache.FillBrush;
                    var pen = shapeIsSelected && i == index ? hitPointCache.StrokePen : pointCache.StrokePen;
                    DrawSpiroPoint(dc, brush, pen, point);
                }
            }
        }

        private void DrawSpiroKnot(DrawingContext dc, Brush brush, Pen pen, SpiroKnot knot)
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

        private void DrawSpiroPoint(DrawingContext dc, Brush brush, Pen pen, SpiroControlPoint point)
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

        private void DrawGuides(DrawingContext dc)
        {
            foreach (var guide in SpiroEditor.Drawing.Guides)
            {
                DrawGuideLine(dc, FromCache(_lineStyle), guide.Point0, guide.Point1);
            }
        }

        private void DrawGuidePoint(DrawingContext dc, BasicStyleCache cache, GuidePoint point, double radius)
        {
            dc.DrawEllipse(cache.FillBrush, null, new Point(point.X, point.Y), radius, radius);
        }

        private void DrawGuideLine(DrawingContext dc, BasicStyleCache cache, GuidePoint point0, GuidePoint point1)
        {
            var gs = new GuidelineSet(
                new double[] { point0.X + cache.HalfThickness, point1.X + cache.HalfThickness },
                new double[] { point0.Y + cache.HalfThickness, point1.Y + cache.HalfThickness });
            gs.Freeze();
            dc.PushGuidelineSet(gs);
            dc.DrawLine(cache.StrokePen, new Point(point0.X, point0.Y), new Point(point1.X, point1.Y));
            dc.Pop();
        }

        private void DrawHorizontalGuide(DrawingContext dc, BasicStyleCache cache, GuidePoint point, double width)
        {
            var point0 = new GuidePoint(0, point.Y);
            var point1 = new GuidePoint(width, point.Y);
            DrawGuideLine(dc, cache, point0, point1);
        }

        private void DrawVerticalGuide(DrawingContext dc, BasicStyleCache cache, GuidePoint point, double height)
        {
            var point0 = new GuidePoint(point.X, 0);
            var point1 = new GuidePoint(point.X, height);
            DrawGuideLine(dc, cache, point0, point1);
        }
    }
}

/*
SpiroNet.Avalonia
Copyright (C) 2019 Wiesław Šoltés

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
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace SpiroNet.Editor.Avalonia.Renderer;

public class CanvasRenderer : Canvas
{
    private static Geometry LeftKnot = StreamGeometry.Parse("M0,-4 A 4,4 0 0 0 0,4");
    private static Geometry RightKnot = StreamGeometry.Parse("M0,-4 A 4,4 0 0 1 0,4");
    private static Geometry EndKnot = StreamGeometry.Parse("M-3.5,-3.5 L3.5,3.5 M-3.5,3.5 L3.5,-3.5");
    private static Geometry EndOpenContourKnot = StreamGeometry.Parse("M-3.5,-3.5 L0,0 -3.5,3.5");
    private static Geometry OpenContourKnot = StreamGeometry.Parse("M3.5,-3.5 L0,0 3.5,3.5");

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
        get { return GetValue(SpiroEditorProperty); }
        set { SetValue(SpiroEditorProperty, value); }
    }

    public static readonly StyledProperty<SpiroEditor> SpiroEditorProperty =
        AvaloniaProperty.Register<CanvasRenderer, SpiroEditor>(nameof(SpiroEditor));

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

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (SpiroEditor != null && SpiroEditor.Drawing != null)
        {
            if (SpiroEditor.Drawing.Guides != null && SpiroEditor.State.DisplayGuides)
            {
                DrawGuides(context);
            }

            var state = SpiroEditor.State;
            if (SpiroEditor.State.DisplayGuides && (state.Tool == EditorTool.Guide || state.Tool == EditorTool.Spiro))
            {
                if ((state.Tool == EditorTool.Spiro && state.EnableSnap)
                    || (state.Tool == EditorTool.Guide && state.IsCaptured)
                    || (state.Tool == EditorTool.Guide && state.HaveSnapPoint))
                {
                    DrawHorizontalGuide(context,
                        FromCache(state.HaveSnapPoint ? _snapGuideStyle : _guideStyle),
                        state.GuidePosition,
                        SpiroEditor.Drawing.Width);

                    DrawVerticalGuide(context,
                        FromCache(state.HaveSnapPoint ? _snapGuideStyle : _guideStyle),
                        state.GuidePosition,
                        SpiroEditor.Drawing.Height);
                }

                if (state.Tool == EditorTool.Guide && state.HaveSnapPoint)
                {
                    DrawGuidePoint(
                        context,
                        FromCache(_snapPointStyle),
                        SpiroEditor.State.SnapPoint,
                        SpiroEditor.State.SnapPointRadius);
                }

                if (state.Tool == EditorTool.Guide && state.IsCaptured)
                {
                    DrawGuideLine(
                        context,
                        FromCache(_newLineStyle),
                        SpiroEditor.Measure.Point0,
                        SpiroEditor.Measure.Point1);
                }
            }

            if (SpiroEditor.Drawing.Shapes != null)
            {
                DrawSpiroShapes(context);
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
            var geometry = StreamGeometry.Parse(data);
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

    private void DrawSpiroKnot(DrawingContext dc, IBrush brush, Pen pen, SpiroKnot knot)
    {
        switch (knot.Type)
        {
            case SpiroPointType.Corner:
                dc.FillRectangle(brush, new Rect(knot.X - 3.5, knot.Y - 3.5, 7, 7));
                break;
            case SpiroPointType.G4:
                FillEllipse(dc, brush, new GuidePoint(knot.X, knot.Y), 3.5, 3.5);
                break;
            case SpiroPointType.G2:
            {
                var rm = Rotation(ToRadians(knot.Theta), new Vector(knot.X, knot.Y));
                var rt = dc.PushPreTransform(rm);
                dc.FillRectangle(brush, new Rect(knot.X - 1.5, knot.Y - 3.5, 3, 7));
                rt.Dispose();
            }
                break;
            case SpiroPointType.Left:
            {
                var rm = Rotation(ToRadians(knot.Theta), new Vector(knot.X, knot.Y));
                var tm = Translate(knot.X, knot.Y);
                var rt = dc.PushPreTransform(rm);
                var tt = dc.PushPreTransform(tm);
                dc.DrawGeometry(brush, null, LeftKnot);
                tt.Dispose();
                rt.Dispose();
            }
                break;
            case SpiroPointType.Right:
            {
                var rm = Rotation(ToRadians(knot.Theta), new Vector(knot.X, knot.Y));
                var tm = Translate(knot.X, knot.Y);
                var rt = dc.PushPreTransform(rm);
                var tt = dc.PushPreTransform(tm);
                dc.DrawGeometry(brush, null, RightKnot);
                tt.Dispose();
                rt.Dispose();
            }
                break;
            case SpiroPointType.End:
            {
                var rm = Rotation(ToRadians(knot.Theta), new Vector(knot.X, knot.Y));
                var tm = Translate(knot.X, knot.Y);
                var rt = dc.PushPreTransform(rm);
                var tt = dc.PushPreTransform(tm);
                dc.DrawGeometry(null, pen, EndKnot);
                tt.Dispose();
                rt.Dispose();
            }
                break;
            case SpiroPointType.OpenContour:
            {
                var rm = Rotation(ToRadians(knot.Theta), new Vector(knot.X, knot.Y));
                var tm = Translate(knot.X, knot.Y);
                var rt = dc.PushPreTransform(rm);
                var tt = dc.PushPreTransform(tm);
                dc.DrawGeometry(null, pen, OpenContourKnot);
                tt.Dispose();
                rt.Dispose();
            }
                break;
            case SpiroPointType.EndOpenContour:
            {
                var rm = Rotation(ToRadians(knot.Theta), new Vector(knot.X, knot.Y));
                var tm = Translate(knot.X, knot.Y);
                var rt = dc.PushPreTransform(rm);
                var tt = dc.PushPreTransform(tm);
                dc.DrawGeometry(null, pen, EndOpenContourKnot);
                tt.Dispose();
                rt.Dispose();
            }
                break;
        }
    }

    private void DrawSpiroPoint(DrawingContext dc, IBrush brush, Pen pen, SpiroControlPoint point)
    {
        switch (point.Type)
        {
            case SpiroPointType.Corner:
                dc.FillRectangle(brush, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                break;
            case SpiroPointType.G4:
            case SpiroPointType.G2:
            case SpiroPointType.Left:
            case SpiroPointType.Right:
            case SpiroPointType.End:
            case SpiroPointType.OpenContour:
            case SpiroPointType.EndOpenContour:
                var tm = Translate(point.X, point.Y);
                var tt = dc.PushPreTransform(tm);
                dc.DrawGeometry(null, pen, EndKnot);
                tt.Dispose();
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
        FillEllipse(dc, cache.FillBrush, point, radius, radius);
    }

    private void DrawGuideLine(DrawingContext dc, BasicStyleCache cache, GuidePoint point0, GuidePoint point1)
    {
        dc.DrawLine(cache.StrokePen, new Point(point0.X, point0.Y), new Point(point1.X, point1.Y));
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

    private static void FillEllipse(DrawingContext dc, IBrush brush, GuidePoint point, double radiusX, double radiusY)
    {
        var g = new EllipseGeometry(new Rect(point.X - radiusX, point.Y - radiusY, 2.0 * radiusX, 2.0 * radiusY));
        dc.DrawGeometry(brush, null, g);
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static Matrix Translate(double offsetX, double offsetY)
    {
        return new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
    }

    private static Matrix Rotation(double radians)
    {
        double cos = Math.Cos(radians);
        double sin = Math.Sin(radians);
        return new Matrix(cos, sin, -sin, cos, 0, 0);
    }

    private static Matrix Rotation(double angle, Vector center)
    {
        return Translate(-center.X, -center.Y) * Rotation(angle) * Translate(center.X, center.Y);
    }
}
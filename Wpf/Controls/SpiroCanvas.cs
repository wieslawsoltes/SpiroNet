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
        private Brush _geometryBrush;
        private Brush _geometryPenBrush;
        private Pen _geometryPen;
        private Brush _hitGeometryBrush;
        private Brush _hitGeometryPenBrush;
        private Pen _hitGeometryPen;
        private Brush _pointBrush;
        private Brush _hitPointBrush;

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

            _hitPointBrush = new SolidColorBrush(Color.FromArgb(192, 255, 0, 0));
            _hitPointBrush.Freeze();
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
            }
        }

        private void DrawShape(DrawingContext dc, PathShape shape)
        {
            if (shape == null || Editor == null || Editor.Data == null)
                return;

            var hitShape = Editor.State.HitShape;
            var hitShapePointIndex = Editor.State.HitShapePointIndex;

            string data;
            if (Editor.Data.TryGetValue(shape, out data) && !string.IsNullOrEmpty(data))
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

            if (shape.Points != null)
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    var point = shape.Points[i];
                    var brush = shape == hitShape && i == hitShapePointIndex ? _hitPointBrush : _pointBrush;

                    switch (point.Type)
                    {
                        case SpiroPointType.Corner:
                            dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                            break;
                        case SpiroPointType.G4:
                            dc.DrawEllipse(brush, null, new Point(point.X, point.Y), 3.5, 3.5);
                            break;
                        case SpiroPointType.G2:
                            dc.DrawEllipse(brush, null, new Point(point.X, point.Y), 3.5, 3.5);
                            break;
                        case SpiroPointType.Left:
                            dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                            break;
                        case SpiroPointType.Right:
                            dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                            break;
                        case SpiroPointType.End:
                            dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                            break;
                        case SpiroPointType.OpenContour:
                            dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                            break;
                        case SpiroPointType.EndOpenContour:
                            dc.DrawRectangle(brush, null, new Rect(point.X - 3.5, point.Y - 3.5, 7, 7));
                            break;
                    }
                }
            }
        }
    }
}

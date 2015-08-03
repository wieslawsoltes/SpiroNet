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

namespace SpiroNet.Wpf
{
    public class SpiroCanvas : Canvas
    {
        private Brush _geometryBrush;
        private Brush _geometryPenBrush;
        private Pen _geometryPen;
        private Brush _pointBrush;
        private Brush _hitPointBrush;

        public SpiroContext Context
        {
            get { return (SpiroContext)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register(
                "Context", 
                typeof(SpiroContext), 
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
            _pointBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            _pointBrush.Freeze();
            _hitPointBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0));
            _hitPointBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            DrawShapes(dc);
        }

        private void DrawShapes(DrawingContext dc)
        {
            if (Context == null || Context.Shapes == null)
                return;

            foreach (var shape in Context.Shapes)
            {
                DrawShape(dc, shape);
            }
        }

        private void DrawShape(DrawingContext dc, PathShape shape)
        {
            if (shape == null || Context == null || Context.Data == null)
                return;

            string data;
            if (Context.Data.TryGetValue(shape, out data) && !string.IsNullOrEmpty(data))
            {
                var geometry = Geometry.Parse(data);
                dc.DrawGeometry(shape.IsClosed ? _geometryBrush : null, _geometryPen, geometry);
            }

            if (shape.Points != null)
            {
                var hitShape = Context.HitShape;
                var hitShapePointIndex = Context.HitShapePointIndex;

                for (int i = 0; i < shape.Points.Count; i++)
                {
                    var point = shape.Points[i];
                    if (shape == hitShape && i == hitShapePointIndex)
                        dc.DrawEllipse(_hitPointBrush, null, new Point(point.X, point.Y), 4.0, 4.0);
                    else
                        dc.DrawEllipse(_pointBrush, null, new Point(point.X, point.Y), 4.0, 4.0);
                }
            }
        }
    }
}

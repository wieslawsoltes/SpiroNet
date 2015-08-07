/*
SpiroNet.Editor
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

namespace SpiroNet.Editor
{
    public class SpiroEditor : ObservableObject
    {
        private SpirtoEditorState _state = null;
        private SpirtoEditorCommands _commands = null;
        private Action _invalidate = null;
        private PathDrawing _drawing = null;
        private IDictionary<PathShape, string> _data = null;
        private IDictionary<PathShape, IList<SpiroKnot>> _knots = null;
        private double _startX;
        private double _startY;

        public SpirtoEditorState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        public SpirtoEditorCommands Commands
        {
            get { return _commands; }
            set { Update(ref _commands, value); }
        }

        public Action Invalidate
        {
            get { return _invalidate; }
            set { Update(ref _invalidate, value); }
        }

        public PathDrawing Drawing
        {
            get { return _drawing; }
            set { Update(ref _drawing, value); }
        }

        public IDictionary<PathShape, string> Data
        {
            get { return _data; }
            set { Update(ref _data, value); }
        }

        public IDictionary<PathShape, IList<SpiroKnot>> Knots
        {
            get { return _knots; }
            set { Update(ref _knots, value); }
        }

        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }

        public void ToggleIsStroked()
        {
            _state.IsStroked = !_state.IsStroked;
            
            if (_state.Shape != null)
            {
                _state.Shape.IsStroked = _state.IsStroked;
                _invalidate();
            }

            if (_state.HitShape != null && _state.HitShapePointIndex == -1)
            {
                _state.HitShape.IsStroked = _state.IsStroked;
                _invalidate();
            }
        }

        public void ToggleIsFilled()
        {
            _state.IsFilled = !_state.IsFilled;
            
            if (_state.Shape != null)
            {
                _state.Shape.IsFilled = _state.IsFilled;
                _invalidate();
            }
            
            if (_state.HitShape != null && _state.HitShapePointIndex == -1)
            {
                _state.HitShape.IsFilled = _state.IsFilled;
                _invalidate();
            }
        }

        public void ToggleIsClosed()
        {
            _state.IsClosed = !_state.IsClosed;
            
            if (_state.Shape != null)
            {
                _state.Shape.IsClosed = _state.IsClosed;
                UpdateData(_state.Shape);
                _invalidate();
            }
            
            if (_state.HitShape != null && _state.HitShapePointIndex == -1)
            {
                _state.HitShape.IsClosed = _state.IsClosed;
                UpdateData(_state.HitShape);
                _invalidate();
            }
        }

        public void ToggleIsTagged()
        {
            _state.IsTagged = !_state.IsTagged;
            
            if (_state.Shape != null)
            {
                _state.Shape.IsTagged = _state.IsTagged;
                UpdateData(_state.Shape);
                _invalidate();
            }

            if (_state.HitShape != null && _state.HitShapePointIndex == -1)
            {
                _state.HitShape.IsTagged = _state.IsTagged;
                UpdateData(_state.HitShape);
                _invalidate();
            }
        }

        public void TogglePointType(string value)
        {
            var type = (SpiroPointType)Enum.Parse(typeof(SpiroPointType), value);
            _state.PointType = type;

            // Change last point type.
            if (_state.Shape != null && _state.Shape.Points.Count >= 1)
            {
                SetPointType(_state.Shape, _state.Shape.Points.Count - 1, type);
                UpdateData(_state.Shape);
            }
            
            // Change selected point type.
            if (_state.HitShape != null && _state.HitShapePointIndex != -1)
            {
                SetPointType(_state.HitShape, _state.HitShapePointIndex, type);
                UpdateData(_state.HitShape);
            }

            _invalidate();
        }

        private void NewShape()
        {
            _state.Shape = new PathShape()
            {
                IsStroked = _state.IsStroked,
                IsFilled = _state.IsFilled,
                IsClosed = _state.IsClosed,
                IsTagged = _state.IsTagged,
                Points = new ObservableCollection<SpiroControlPoint>()
            };
            _drawing.Shapes.Add(_state.Shape);
        }

        private void NewPoint(PathShape shape, double x, double y)
        {
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = _state.PointType;
            shape.Points.Add(point);
        }

        private void NewPointAt(PathShape shape, double x, double y, int index)
        {
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = _state.PointType;
            shape.Points.Insert(index, point);
        }

        private void SetPointPositionDelta(PathShape shape, int index, double dx, double dy)
        {
            var old = shape.Points[index];
            double x = old.X + dx;
            double y = old.Y + dy;
            double sx = _state.EnableSnap ? Snap(x, _state.SnapX) : x;
            double sy = _state.EnableSnap ? Snap(y, _state.SnapY) : y;
            var point = new SpiroControlPoint();
            point.X = sx;
            point.Y = sy;
            point.Type = old.Type;
            shape.Points[index] = point;
        }
        
        private void SetPointPosition(PathShape shape, int index, double x, double y)
        {
            var old = shape.Points[index];
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = old.Type;
            shape.Points[index] = point;
        }
        
        private void SetPointType(PathShape shape, int index, SpiroPointType type)
        {
            var old = shape.Points[index];
            var point = new SpiroControlPoint();
            point.X = old.X;
            point.Y = old.Y;
            point.Type = type;
            shape.Points[index] = point;
        }

        private static bool TryGetData(PathShape shape, IBezierContext bc)
        {
            if (shape == null || bc == null)
                return false;

            var points = shape.Points.ToArray();
            if (shape.IsTagged)
                return Spiro.TaggedSpiroCPsToBezier0(points, bc);
            else
                return Spiro.SpiroCPsToBezier0(points, points.Length, shape.IsClosed, bc);
        }

        private void UpdateData(PathShape shape)
        {
            if (shape == null)
                return;

            var bc = new PathBezierContext();
            var result = TryGetData(shape, bc);
            if (_data.ContainsKey(shape))
            {
                _data[shape] = result ? bc.ToString() : null;
                _knots[shape] = result ? bc.GetKnots() : null;
            }
            else
            {
                _data.Add(shape, result ? bc.ToString() : null);
                _knots.Add(shape, result ? bc.GetKnots() : null);
            }
        }

        private static double DistanceSquared(double x0, double y0, double x1, double y1)
        {
            double dx = x0 - x1;
            double dy = y0 - y1;
            return dx * dx + dy * dy;
        }

        private static bool HitTestForPoint(IList<PathShape> shapes, double x, double y, double tresholdSquared, out PathShape hitShape, out int hitShapePointIndex)
        {
            foreach (var shape in shapes)
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    var point = shape.Points[i];
                    var distance = DistanceSquared(x, y, point.X, point.Y);
                    if (distance < tresholdSquared)
                    {
                        hitShape = shape;
                        hitShapePointIndex = i;
                        return true;
                    }
                }
            }

            hitShape = null;
            hitShapePointIndex = -1;
            return false;
        }

        private static bool HitTestForShape(IList<PathShape> shapes, double x, double y, double treshold, out PathShape hitShape, out int hitShapePointIndex)
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                var bc = new HitTestBezierContext(x, y);
                var shape = shapes[i];
                int knontIndex;

                try
                {
                    if (!TryGetData(shape, bc))
                        continue;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                    continue;
                }

                if (bc.Report(out knontIndex) < treshold)
                {
                    hitShape = shape;
                    hitShapePointIndex = knontIndex;
                    return true;
                }
            }

            hitShape = null;
            hitShapePointIndex = -1;
            return false;
        }

        public void MiddleDown(double x, double y)
        {
            double sx = _state.EnableSnap ? Snap(x, _state.SnapX) : x;
            double sy = _state.EnableSnap ? Snap(y, _state.SnapY) : y;

            if (_state.Shape == null)
            {
                PathShape hitShape;
                int hitShapePointIndex;

                // Hit test for shape and nearest point.
                if (HitTestForShape(_drawing.Shapes, x, y, _state.HitTreshold, out hitShape, out hitShapePointIndex))
                {
                    // Insert new point.
                    PathShape shape = hitShape;
                    int index = hitShapePointIndex + 1;
                    NewPointAt(shape, sx, sy, index);

                    // Deselect shape.
                    _state.HitShape = null;
                    // Deselect point.
                    _state.HitShapePointIndex = -1;
                    _state.Mode = SpirtoEditorMode.Create;

                    UpdateData(shape);
                    _invalidate();
                    return;
                }

                if (_state.HitShape != null)
                {
                    // Deselect shape.
                    _state.HitShape = null;
                    // Deselect point.
                    _state.HitShapePointIndex = -1;
                    // Begin edit.
                    _state.Mode = SpirtoEditorMode.Create;
                    _invalidate();
                    return;
                }
            }
        }

        public void LeftDown(double x, double y)
        {
            double sx = _state.EnableSnap ? Snap(x, _state.SnapX) : x;
            double sy = _state.EnableSnap ? Snap(y, _state.SnapY) : y;
            _startX = sx;
            _startY = sy;

            if (_state.Shape == null)
            {
                PathShape hitShape;
                int hitShapePointIndex;

                // Hit test for point.
                var result = HitTestForPoint(_drawing.Shapes, x, y, _state.HitTresholdSquared, out hitShape, out hitShapePointIndex);
                if (result)
                {
                    // Select shape.
                    _state.HitShape = hitShape;
                    // Select point.
                    _state.HitShapePointIndex = hitShapePointIndex;
                    // Begin point move.
                    _state.Mode = SpirtoEditorMode.Move;
                    
                    // Update point type.
                    if (_state.HitShapePointIndex != -1)
                    {
                        _state.PointType = _state.HitShape.Points[_state.HitShapePointIndex].Type; 
                    }
                    
                    _invalidate();
                    return;
                }
                else
                {
                    // Hit test for shape and nearest point.
                    if (HitTestForShape(_drawing.Shapes, x, y, _state.HitTreshold, out hitShape, out hitShapePointIndex))
                    {
                        // Select shape.
                        _state.HitShape = hitShape;
                        // Deselect point.
                        _state.HitShapePointIndex = -1;
                        // Begin shape move.
                        _state.Mode = SpirtoEditorMode.Move;
                        
                        // Update shape info.
                        if (_state.HitShape != null)
                        {
                            var shape = _state.HitShape;
                            _state.IsStroked = shape.IsStroked;
                            _state.IsFilled = shape.IsFilled;
                            _state.IsClosed = shape.IsClosed;
                            _state.IsTagged = shape.IsTagged;
                            UpdateData(shape);
                        }

                        _invalidate();
                        return;
                    }

                    if (_state.HitShape != null)
                    {
                        // Deselect shape.
                        _state.HitShape = null;
                        // Deselect point.
                        _state.HitShapePointIndex = -1;
                        // Begin edit.
                        _state.Mode = SpirtoEditorMode.Create;
                        _invalidate();
                        return;
                    }
                }
            }

            if (_state.Mode == SpirtoEditorMode.Create)
            {
                // Add new shape.
                if (_state.Shape == null)
                    NewShape();

                // Add new point.
                NewPoint(_state.Shape, sx, sy);

                UpdateData(_state.Shape);
                _invalidate();
            }
        }

        public void LeftUp(double x, double y)
        {
            double sx = _state.EnableSnap ? Snap(x, _state.SnapX) : x;
            double sy = _state.EnableSnap ? Snap(y, _state.SnapY) : y;
            
            if (_state.Mode == SpirtoEditorMode.Move)
            {
                // Finish move.
                _state.Mode = SpirtoEditorMode.Selected;
            }
        }

        public void RightDown(double x, double y)
        {
            double sx = _state.EnableSnap ? Snap(x, _state.SnapX) : x;
            double sy = _state.EnableSnap ? Snap(y, _state.SnapY) : y;
            
            if (_state.Shape != null)
            {
                // Finish create.
                UpdateData(_state.Shape);
                _invalidate();
                _state.Shape = null;
            }
            else
            {
                if (_state.HitShape != null)
                {
                    // Deselect point.
                    _state.HitShape = null;
                    _state.HitShapePointIndex = -1;
                    // Begin create.
                    _state.Mode = SpirtoEditorMode.Create;
                    _invalidate();
                }

                PathShape hitShape;
                int hitShapePointIndex;
                var result = HitTestForPoint(_drawing.Shapes, x, y, _state.HitTresholdSquared, out hitShape, out hitShapePointIndex);
                if (result)
                {
                    // Delete point.
                    hitShape.Points.RemoveAt(hitShapePointIndex);

                    if (hitShape.Points.Count == 0)
                    {
                        // Delete shape.
                        _drawing.Shapes.Remove(hitShape);
                        _data.Remove(hitShape);
                        _knots.Remove(hitShape);
                        _invalidate();
                    }
                    else
                    {
                        UpdateData(hitShape);
                        _invalidate();
                    }

                    return;
                }
                else
                {
                    if (HitTestForShape(_drawing.Shapes, x, y, _state.HitTreshold, out hitShape, out hitShapePointIndex))
                    {
                        // Delete shape.
                        _drawing.Shapes.Remove(hitShape);
                        _data.Remove(hitShape);
                        _knots.Remove(hitShape);
                        _invalidate();
                        return;
                    }
                }
            }
        }

        public void Move(double x, double y)
        {
            double sx = _state.EnableSnap ? Snap(x, _state.SnapX) : x;
            double sy = _state.EnableSnap ? Snap(y, _state.SnapY) : y;
            
            if (_state.Shape != null)
            {
                if (_state.Shape.Points.Count > 1)
                {
                    SetPointPosition(_state.Shape, _state.Shape.Points.Count - 1, sx, sy);
                    UpdateData(_state.Shape);
                    _invalidate();
                }
            }
            else
            {
                if (_state.Mode == SpirtoEditorMode.Move)
                {
                    // Move selected shape.
                    if (_state.HitShape != null && _state.HitShapePointIndex == -1)
                    {
                        // Calculate move offset.
                        double dx = sx - _startX;
                        double dy = sy - _startY;
                        // Update start position.
                        _startX = sx;
                        _startY = sy;
                        // Move all shape points.
                        for (int i = 0; i < _state.HitShape.Points.Count; i++)
                            SetPointPositionDelta(_state.HitShape, i, dx, dy);
                        
                        UpdateData(_state.HitShape);
                        _invalidate();
                    }
                    
                    // Move selected point.
                    if (_state.HitShapePointIndex != -1)
                    {
                        SetPointPosition(_state.HitShape, _state.HitShapePointIndex, sx, sy);
                        
                        UpdateData(_state.HitShape);
                        _invalidate();
                    }
                }
                else if (_state.Mode == SpirtoEditorMode.Create)
                {
                    PathShape hitShape;
                    int hitShapePointIndex;
                    var result = HitTestForPoint(_drawing.Shapes, x, y, _state.HitTresholdSquared, out hitShape, out hitShapePointIndex);
                    if (result)
                    {
                        // Hover shape.
                        _state.HitShape = hitShape;
                        // Hover point.
                        _state.HitShapePointIndex = hitShapePointIndex;
                        //if (_state.HitShapePointIndex != -1)
                        //    _state.PointType = _state.HitShape.Points[_state.HitShapePointIndex].Type;
                        _invalidate();
                    }
                    else
                    {
                        if (HitTestForShape(_drawing.Shapes, x, y, _state.HitTreshold, out hitShape, out hitShapePointIndex))
                        {
                            // Hover shape.
                            _state.HitShape = hitShape;
                            // Dehover point.
                            _state.HitShapePointIndex = -1;
                        }
                        else
                        {
                            // Dehover shape.
                            _state.HitShape = null;
                            // Dehover point.
                            _state.HitShapePointIndex = -1;
                        }
                        _invalidate();
                    }
                }
            }
        }

        public void New()
        {
            var drawing = new PathDrawing()
            {
                Width = _drawing.Width,
                Height = _drawing.Height,
                Shapes = new ObservableCollection<PathShape>()
            };

            OpenDrawing(drawing);
        }

        public void OpenDrawing(string path)
        {
            using (var f = System.IO.File.OpenText(path))
            {
                var json = f.ReadToEnd();
                var drawing = JsonSerializer.Deserialize<PathDrawing>(json);
                OpenDrawing(drawing);
            }
        }

        public void OpenDrawing(PathDrawing drawing)
        {
            Drawing = drawing;
            Data = new Dictionary<PathShape, string>();
            Knots = new Dictionary<PathShape, IList<SpiroKnot>>();

            foreach (var shape in _drawing.Shapes)
            {
                UpdateData(shape);
            }

            _invalidate();
        }

        public void OpenPlate(string path)
        {
            using (var f = System.IO.File.OpenText(path))
            {
                var plate = f.ReadToEnd();
                var shapes = Plate.ToShapes(plate);
                if (shapes != null)
                {
                    var drawing = new PathDrawing()
                    {
                        Width = _drawing.Width,
                        Height = _drawing.Height,
                        Shapes = shapes
                    };
                    
                    OpenDrawing(drawing);
                }
            }
        }

        public void SaveAsDrawing(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var json = JsonSerializer.Serialize(_drawing);
                f.Write(json);
            }
        }

        public void SaveAsPlate(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var plate = Plate.FromShapes(_drawing.Shapes);
                f.Write(plate);
            }
        }

        public void ExportAsSvg(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var sb = new StringBuilder();
                var suffix = Environment.NewLine + "           ";

                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
                sb.AppendLine(
                    string.Format(
                        "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"{0}\" height=\"{1}\">",
                        _drawing.Width,
                        _drawing.Height));

                foreach (var shape in _drawing.Shapes)
                {
                    sb.AppendLine(
                        string.Format(
                            "  <path style=\"{0};{1}\"",
                            shape.IsStroked ? "stroke:#000000;stroke-opacity:1;stroke-width:2" : "stroke:none",
                            shape.IsFilled ? "fill:#808080;fill-opacity:0.5" : "fill:none"));
                    sb.AppendLine(
                        string.Format(
                            "        d=\"{0}\"/>",
                            Data[shape].Replace(Environment.NewLine, suffix)));
                }

                sb.AppendLine("</svg>");

                f.Write(sb);
            }
        }

        public void ExportAsPs(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var sb = new StringBuilder();

                sb.Append(PsBezierContext.PsProlog);
                sb.Append(string.Format(PsBezierContext.PsSize, _drawing.Width, _drawing.Height));

                foreach (var shape in _drawing.Shapes)
                {
                    var bc = new PsBezierContext();

                    try
                    {
                        if (TryGetData(shape, bc))
                        {
                            sb.Append(bc);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);
                        Debug.Print(ex.StackTrace);
                    }
                }

                sb.Append(PsBezierContext.PsPostlog);

                f.Write(sb);
            }
        }

        public void ExecuteScript(string script)
        {
            var shapes = Plate.ToShapes(script);
            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    _drawing.Shapes.Add(shape);
                    UpdateData(shape);
                }

                _invalidate();
            }
        }
    }
}

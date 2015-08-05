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
using System.Windows.Input;

namespace SpiroNet.Editor
{
    public enum Mode { None, Create, Move, Selected }

    public class SpiroEditor : ObservableObject
    {
        private Mode _mode = Mode.Create;
        private PathShape _shape = null;
        private double _hitTreshold = 7;
        private double _hitTresholdSquared = 49;
        private PathShape _hitShape = null;
        private int _hitShapePointIndex = -1;
        private double _width = 600;
        private double _height = 600;
        private bool _isStroked = true;
        private bool _isFilled = false;
        private bool _isClosed = false;
        private bool _isTagged = false;
        private SpiroPointType _pointType = SpiroPointType.G4;
        private IList<PathShape> _shapes = null;
        private IDictionary<PathShape, string> _data = null;

        public Mode Mode
        {
            get { return _mode; }
            set { Update(ref _mode, value); }
        }

        public PathShape Shape
        {
            get { return _shape; }
            set { Update(ref _shape, value); }
        }

        public double HitTreshold
        {
            get { return _hitTreshold; }
            set { Update(ref _hitTreshold, value); }
        }

        public double HitTresholdSquared
        {
            get { return _hitTresholdSquared; }
            set { Update(ref _hitTresholdSquared, value); }
        }

        public PathShape HitShape
        {
            get { return _hitShape; }
            set { Update(ref _hitShape, value); }
        }

        public int HitShapePointIndex
        {
            get { return _hitShapePointIndex; }
            set { Update(ref _hitShapePointIndex, value); }
        }

        public double Width
        {
            get { return _width; }
            set { Update(ref _width, value); }
        }

        public double Height
        {
            get { return _height; }
            set { Update(ref _height, value); }
        }

        public bool IsStroked
        {
            get { return _isStroked; }
            set { Update(ref _isStroked, value); }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
        }

        public bool IsClosed
        {
            get { return _isClosed; }
            set { Update(ref _isClosed, value); }
        }

        public bool IsTagged
        {
            get { return _isTagged; }
            set { Update(ref _isTagged, value); }
        }

        public SpiroPointType PointType
        {
            get { return _pointType; }
            set { Update(ref _pointType, value); }
        }

        public IList<PathShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        public IDictionary<PathShape, string> Data
        {
            get { return _data; }
            set { Update(ref _data, value); }
        }

        public ICommand NewCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand ExportCommand { get; set; }

        public ICommand ExitCommand { get; set; }

        public ICommand IsStrokedCommand { get; set; }

        public ICommand IsFilledCommand { get; set; }

        public ICommand IsClosedCommand { get; set; }

        public ICommand IsTaggedCommand { get; set; }

        public ICommand PointTypeCommand { get; set; }

        public ICommand ExecuteScriptCommand { get; set; }

        public Action Invalidate { get; set; }

        public void ToggleIsStroked()
        {
            IsStroked = !IsStroked;
            if (_shape != null)
            {
                _shape.IsStroked = IsStroked;
                Invalidate();
            }
        }

        public void ToggleIsFilled()
        {
            IsFilled = !IsFilled;
            if (_shape != null)
            {
                _shape.IsFilled = IsFilled;
                Invalidate();
            }
        }

        public void ToggleIsClosed()
        {
            IsClosed = !IsClosed;
            if (_shape != null)
            {
                _shape.IsClosed = IsClosed;
                UpdateData(_shape);
                Invalidate();
            }
        }

        public void ToggleIsTagged()
        {
            IsTagged = !IsTagged;
            if (_shape != null)
            {
                _shape.IsTagged = IsTagged;
                UpdateData(_shape);
                Invalidate();
            }
        }

        public void TogglePointType(string value)
        {
            var type = (SpiroPointType)Enum.Parse(typeof(SpiroPointType), value);
            PointType = type;
            SetLastPointType(type);
            UpdateData(_shape);
            Invalidate();
        }

        private void NewShape()
        {
            _shape = new PathShape();
            _shape.IsStroked = IsStroked;
            _shape.IsFilled = IsFilled;
            _shape.IsClosed = IsClosed;
            _shape.IsTagged = IsTagged;
            _shape.Points = new ObservableCollection<SpiroControlPoint>();
            Shapes.Add(_shape);
        }

        private void NewPoint(PathShape shape, double x, double y)
        {
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = PointType;
            shape.Points.Add(point);
        }

        private void NewPointAt(PathShape shape, double x, double y, int index)
        {
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = PointType;
            shape.Points.Insert(index, point);
        }

        private void SetPointPosition(PathShape shape, int index, double x, double y)
        {
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = shape.Points[index].Type;
            shape.Points[index] = point;
        }

        private void SetLastPointType(SpiroPointType type)
        {
            if (_shape == null || _shape.Points.Count < 1)
                return;

            var old = _shape.Points[_shape.Points.Count - 1];
            var point = new SpiroControlPoint();
            point.X = old.X;
            point.Y = old.Y;
            point.Type = type;
            _shape.Points[_shape.Points.Count - 1] = point;
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
                _data[shape] = result ? bc.ToString() : null;
            else
                _data.Add(shape, result ? bc.ToString() : null);
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

        public void LeftDown(double x, double y)
        {
            if (_shape == null)
            {
                PathShape hitShape;
                int hitShapePointIndex;

                // Hit test for point.
                var result = HitTestForPoint(_shapes, x, y, _hitTresholdSquared, out hitShape, out hitShapePointIndex);
                if (result)
                {
                    // Select point.
                    _hitShape = hitShape;
                    _hitShapePointIndex = hitShapePointIndex;
                    // Begin move.
                    _mode = Mode.Move;
                    Invalidate();
                    return;
                }
                else
                {
                    // Hit test for shape and nearest point.
                    if (HitTestForShape(_shapes, x, y, _hitTreshold, out hitShape, out hitShapePointIndex))
                    {
                        // Insert new point.
                        PathShape shape = hitShape;
                        int index = hitShapePointIndex + 1;
                        NewPointAt(shape, x, y, index);

                        // Deselect shape.
                        _hitShape = null;
                        // Deselect point.
                        _hitShapePointIndex = -1;
                        _mode = Mode.Create;

                        UpdateData(shape);
                        Invalidate();
                        return;
                    }

                    if (_hitShape != null)
                    {
                        // Deselect shape.
                        _hitShape = null;
                        // Deselect point.
                        _hitShapePointIndex = -1;
                        // Begin edit.
                        _mode = Mode.Create;
                        Invalidate();
                        return;
                    }
                }
            }

            if (_mode == Mode.Create)
            {
                // Add new shape.
                if (_shape == null)
                    NewShape();

                // Add new point.
                NewPoint(_shape, x, y);

                UpdateData(_shape);
                Invalidate();
            }
        }

        public void LeftUp(double x, double y)
        {
            if (_mode == Mode.Move)
            {
                // Finish move.
                _mode = Mode.Selected;
            }
        }

        public void RightDown(double x, double y)
        {
            if (_shape != null)
            {
                // Finish create.
                UpdateData(_shape);
                Invalidate();
                _shape = null;
            }
            else
            {
                if (_hitShape != null)
                {
                    // Deselect point.
                    _hitShape = null;
                    _hitShapePointIndex = -1;
                    // Begin create.
                    _mode = Mode.Create;
                    Invalidate();
                }

                PathShape hitShape;
                int hitShapePointIndex;
                var result = HitTestForPoint(_shapes, x, y, _hitTresholdSquared, out hitShape, out hitShapePointIndex);
                if (result)
                {
                    // Delete point.
                    hitShape.Points.RemoveAt(hitShapePointIndex);

                    if (hitShape.Points.Count == 0)
                    {
                        // Delete shape.
                        _shapes.Remove(hitShape);
                        _data.Remove(hitShape);
                        Invalidate();
                    }
                    else
                    {
                        UpdateData(hitShape);
                        Invalidate();
                    }

                    return;
                }
                else
                {
                    if (HitTestForShape(_shapes, x, y, _hitTreshold, out hitShape, out hitShapePointIndex))
                    {
                        // Delete shape.
                        _shapes.Remove(hitShape);
                        _data.Remove(hitShape);
                        Invalidate();
                        return;
                    }
                }
            }
        }

        public void Move(double x, double y)
        {
            if (_shape != null)
            {
                if (_shape.Points.Count > 1)
                {
                    SetPointPosition(_shape, _shape.Points.Count - 1, x, y);
                    UpdateData(_shape);
                    Invalidate();
                }
            }
            else
            {
                if (_mode == Mode.Move)
                {
                    SetPointPosition(_hitShape, _hitShapePointIndex, x, y);
                    UpdateData(_hitShape);
                    Invalidate();
                }
                else if (_mode == Mode.Create)
                {
                    PathShape hitShape;
                    int hitShapePointIndex;
                    var result = HitTestForPoint(_shapes, x, y, _hitTresholdSquared, out hitShape, out hitShapePointIndex);
                    if (result)
                    {
                        // Hover shape.
                        _hitShape = hitShape;
                        // Hover point.
                        _hitShapePointIndex = hitShapePointIndex;
                        Invalidate();
                    }
                    else
                    {
                        if (HitTestForShape(_shapes, x, y, _hitTreshold, out hitShape, out hitShapePointIndex))
                        {
                            // Hover shape.
                            _hitShape = hitShape;
                            // Dehover point.
                            _hitShapePointIndex = -1;
                        }
                        else
                        {
                            // Dehover shape.
                            _hitShape = null;
                            // Dehover point.
                            _hitShapePointIndex = -1;
                        }
                        Invalidate();
                    }
                }
            }
        }

        public void New()
        {
            Shapes = new ObservableCollection<PathShape>();
            Data = new Dictionary<PathShape, string>();
            Invalidate();
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
            Width = drawing.Width;
            Height = drawing.Height;
            Shapes = drawing.Shapes;
            Data = new Dictionary<PathShape, string>();

            foreach (var shape in Shapes)
            {
                UpdateData(shape);
            }

            Invalidate();
        }

        public void OpenPlate(string path)
        {
            using (var f = System.IO.File.OpenText(path))
            {
                var plate = f.ReadToEnd();
                var shapes = Plate.ToShapes(plate);
                if (shapes != null)
                {
                    Shapes = shapes;
                    Data = new Dictionary<PathShape, string>();

                    foreach (var shape in Shapes)
                    {
                        UpdateData(shape);
                    }

                    Invalidate();
                }
            }
        }

        public void SaveAsDrawing(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var drawing = new PathDrawing()
                {
                    Width = Width,
                    Height = Height,
                    Shapes = Shapes
                };
                var json = JsonSerializer.Serialize(drawing);
                f.Write(json);
            }
        }

        public void SaveAsPlate(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var plate = Plate.FromShapes(Shapes);
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
                        Width,
                        Height));

                foreach (var shape in Shapes)
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

        public void ExecuteScript(string script)
        {
            var shapes = Plate.ToShapes(script);
            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    Shapes.Add(shape);
                    UpdateData(shape);
                }

                Invalidate();
            }
        }
    }
}

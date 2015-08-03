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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace SpiroNet.Wpf
{
    public class SpiroContext : ObservableObject
    {
        private PathShape _shape = null;
        private double _width;
        private double _height;
        private bool _isClosed;
        private bool _isTagged;
        private SpiroPointType _pointType;
        private IList<PathShape> _shapes;
        private IDictionary<PathShape, string> _data;

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

        public ICommand ExportAsSvgCommand { get; set; }

        public ICommand ExitCommand { get; set; }

        public ICommand IsClosedCommand { get; set; }

        public ICommand IsTaggedCommand { get; set; }

        public ICommand PointTypeCommand { get; set; }

        public Action Invalidate { get; set; }

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
            _shape.IsClosed = IsClosed;
            _shape.IsTagged = IsTagged;
            _shape.Points = new ObservableCollection<SpiroControlPoint>();
            Shapes.Add(_shape);
        }

        private void NewPoint(double x, double y)
        {
            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = PointType;
            _shape.Points.Add(point);
        }

        private void SetLastPointPosition(double x, double y)
        {
            if (_shape == null || _shape.Points.Count < 1)
                return;

            var point = new SpiroControlPoint();
            point.X = x;
            point.Y = y;
            point.Type = PointType;
            _shape.Points[_shape.Points.Count - 1] = point;
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

        private void UpdateData(PathShape shape)
        {
            if (shape == null)
                return;

            try
            {
                if (Data.ContainsKey(shape))
                {
                    string data;
                    if (shape.TryGetData(out data))
                    {
                        Data[shape] = data;
                    }
                    else
                    {
                        Data[shape] = null;
                    }
                }
                else
                {
                    string data;
                    if (shape.TryGetData(out data))
                    {
                        Data.Add(shape, data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        public void Left(double x, double y)
        {
            if (_shape == null)
                NewShape();

            NewPoint(x, y);
            UpdateData(_shape);
            Invalidate();
        }

        public void Right(double x, double y)
        {
            if (_shape != null)
            {
                UpdateData(_shape);
                Invalidate();
                _shape = null;
            }
        }

        public void Move(double x, double y)
        {
            if (_shape != null && _shape.Points.Count > 1)
            {
                SetLastPointPosition(x, y);
                UpdateData(_shape);
                Invalidate();
            }
        }

        public void New()
        {
            Shapes = new ObservableCollection<PathShape>();
            Data = new Dictionary<PathShape, string>();
            Invalidate();
        }

        public void Open(string path)
        {
            using (var f = System.IO.File.OpenText(path))
            {
                var json = f.ReadToEnd();
                var drawing = JsonSerializer.Deserialize<PathDrawing>(json);
                Open(drawing);
            }
        }

        public void Open(PathDrawing drawing)
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

        public void SaveAs(string path)
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

        public void ExportAsSvg(string path)
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var sb = new StringBuilder();
                var suffix = Environment.NewLine + "           ";

                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
                sb.AppendLine(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"{1}\" height=\"{0}\">", Width, Height));

                foreach (var shape in Shapes)
                {
                    sb.AppendLine(string.Format("  <path {0}",
                        shape.IsClosed ?
                        "style=\"fill-rule:nonzero;stroke:#000000;stroke-opacity:1;stroke-width:2;fill:#808080;fill-opacity:0.5\"" :
                        "style=\"fill-rule:nonzero;stroke:#000000;stroke-opacity:1;stroke-width:2;fill:none\""));
                    sb.AppendLine(string.Format("        d=\"{0}\"/>", Data[shape].Replace(Environment.NewLine, suffix)));
                }

                sb.AppendLine("</svg>");

                f.Write(sb);
            }
        }
    }
}

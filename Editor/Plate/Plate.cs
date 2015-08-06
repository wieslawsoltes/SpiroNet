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
using System.Globalization;
using System.Linq;
using System.Text;

namespace SpiroNet.Editor
{
    public static class Plate
    {
        private static string Format(double value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("en-GB"));
        }

        public static string FromShapes(IList<PathShape> shapes)
        {
            var sb = new StringBuilder();

            sb.AppendLine("(plate");

            foreach (var shape in shapes)
            {
                if (shape.Points.Count == 0)
                    continue;

                int n = shape.Points.Count;
                for (int j = 0; j < n; j++)
                {
                    var point = shape.Points[j];

                    if (!shape.IsClosed && j == 0)
                    {
                        sb.AppendLine(string.Format("  ({{ {0} {1})", Format(point.X), Format(point.Y)));
                    }
                    else if (!shape.IsClosed && j == n - 1)
                    {
                        sb.AppendLine(string.Format("  (}} {0} {1})", Format(point.X), Format(point.Y)));
                    }
                    else
                    {
                        switch (point.Type)
                        {
                            case SpiroPointType.Corner:
                                sb.AppendLine(string.Format("  (v {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                            case SpiroPointType.G4:
                                sb.AppendLine(string.Format("  (o {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                            case SpiroPointType.G2:
                                sb.AppendLine(string.Format("  (c {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                            case SpiroPointType.Left:
                                sb.AppendLine(string.Format("  ([ {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                            case SpiroPointType.Right:
                                sb.AppendLine(string.Format("  (] {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                            case SpiroPointType.End:
                                sb.AppendLine("  (z)");
                                break;
                            case SpiroPointType.OpenContour:
                                sb.AppendLine(string.Format("  ({{ {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                            case SpiroPointType.EndOpenContour:
                                sb.AppendLine(string.Format("  (}} {0} {1})", Format(point.X), Format(point.Y)));
                                break;
                        }
                    }
                }

                if (shape.IsClosed && !shape.IsTagged)
                    sb.AppendLine("  (z)");
            }

            sb.AppendLine(")");

            return sb.ToString();
        }

        private static PathShape NewShape()
        {
            return new PathShape()
            {
                IsStroked = true,
                IsFilled = false,
                IsClosed = false,
                IsTagged = true,
                Points = new ObservableCollection<SpiroControlPoint>()
            };
        }

        private static SpiroControlPoint NewPoint(SpiroPointType type, string x, string y)
        {
            var point = new SpiroControlPoint();
            point.X = double.Parse(x, CultureInfo.GetCultureInfo("en-GB").NumberFormat);
            point.Y = double.Parse(y, CultureInfo.GetCultureInfo("en-GB").NumberFormat);
            point.Type = type;
            return point;
        }

        public static IList<PathShape> ToShapes(string plate)
        {
            if (string.IsNullOrEmpty(plate))
                return null;

            var shapes = new ObservableCollection<PathShape>();
            var newLine = Environment.NewLine.ToCharArray();
            var separator = new char[] { ' ', '\t' };
            var trim = new char[] { '(', ')' };
            var options = StringSplitOptions.RemoveEmptyEntries;
            var lines = plate.Split(newLine, options).Select(x => x.Trim().Trim(trim).Split(separator, options));

            PathShape shape = null;

            foreach (var line in lines)
            {
                if (line.Length == 0 || line[0] == "plate")
                    continue;

                switch (line[0][0])
                {
                    case 'v':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.Corner, line[1], line[2]));
                            else
                                throw new FormatException();
                        }
                        break;
                    case 'o':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.G4, line[1], line[2]));
                            else
                                throw new FormatException();
                        }
                        break;
                    case 'c':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.G2, line[1], line[2]));
                            else
                                throw new FormatException();
                        }
                        break;
                    case '[':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.Left, line[1], line[2]));
                            else
                                throw new FormatException();
                        }
                        break;
                    case ']':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.Right, line[1], line[2]));
                            else
                                throw new FormatException();
                        }
                        break;
                    case 'z':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 1)
                                shape.Points.Add(NewPoint(SpiroPointType.End, "0", "0"));
                            else if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.End, line[1], line[2]));
                            else
                                throw new FormatException();

                            shapes.Add(shape);
                            shape = null;
                        }
                        break;
                    case '{':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.OpenContour, line[1], line[2]));
                            else
                                throw new FormatException();
                        }
                        break;
                    case '}':
                        {
                            if (shape == null)
                                shape = NewShape();

                            if (line.Length == 3)
                                shape.Points.Add(NewPoint(SpiroPointType.EndOpenContour, line[1], line[2]));
                            else
                                throw new FormatException();

                            shapes.Add(shape);
                            shape = null;
                        }
                        break;
                    default:
                        throw new FormatException();
                }
            }

            if (shape != null)
            {
                shape.IsTagged = false;
                shapes.Add(shape);
                shape = null;
            }

            return shapes;
        }
    }
}

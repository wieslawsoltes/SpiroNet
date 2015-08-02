/*
SpiroNet - bezier context implementation for Path Markup Syntax
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
using System.Globalization;
using System.Text;

namespace SpiroNet
{
    public class PathBezierContext : IBezierContext
    {
        private bool _needToClose = false;
        private StringBuilder _sb = new StringBuilder();

        private static string Format(double value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("en-GB"));
        }

        public string GetData()
        {
            if (_needToClose)
            {
                var close = string.Format("Z");
                _sb.Append(close);
                _needToClose = false;
            }
            return _sb.ToString();
        }

        public override string ToString()
        {
            return GetData();
        }

        public void MoveTo(double x, double y, bool isOpen)
        {
            if (_needToClose)
            {
                var close = string.Format("Z");
                _sb.AppendLine(close);
            }

            var move = string.Format("M {0},{1}", Format(x), Format(y));
             _sb.AppendLine(move);
            _needToClose = !isOpen;
        }
        
        public void LineTo(double x, double y)
        {
            var line = string.Format("L {0},{1}", Format(x), Format(y));
             _sb.AppendLine(line);
        }
        
        public void QuadTo(double x1, double y1, double x2, double y2)
        {
            var quad = string.Format("Q {0},{1} {2},{3}", Format(x1), Format(y1), Format(x2), Format(y2));
             _sb.AppendLine(quad);
        }
        
        public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            var curve = string.Format("C {0},{1} {2},{3} {4},{5}", Format(x1), Format(y1), Format(x2), Format(y2), Format(x3), Format(y3));
             _sb.AppendLine(curve);
        }
        
        public void MarkKnot(int knot_idx) { }
    }
}

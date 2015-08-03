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
    /// <summary>
    /// Bezier context implementation that handles the creation of Path data representation of bézier splines.
    /// </summary>
    /// <remarks>
    /// Internally class used StringBuilder object to append generated Path data.
    /// </remarks>
    public class PathBezierContext : IBezierContext
    {
        private bool _needToClose = false;
        private StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// Format double value using en-GB culture info.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Format(double value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("en-GB"));
        }

        /// <summary>
        /// Get output Path data string format.
        /// </summary>
        /// <returns>The Path data string format.</returns>
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

        /// <summary>
        /// Get output Path data string format.
        /// </summary>
        /// <returns>The Path data string format.</returns>
        public override string ToString()
        {
            return GetData();
        }

        /// <summary>
        /// Start a contour.
        /// </summary>
        /// <param name="x">The X coordinate of the new start point.</param>
        /// <param name="y">The Y coordinate of the new start point.</param>
        /// <param name="isOpen">An boolean flag indicating wheter spline is open (True) or closed (False).</param>
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

        /// <summary>
        /// Move from the last point to the next one on a straight line.
        /// </summary>
        /// <param name="x">The X coordinate of the new end point.</param>
        /// <param name="y">The Y coordinate of the new end point.</param>
        public void LineTo(double x, double y)
        {
            var line = string.Format("L {0},{1}", Format(x), Format(y));
             _sb.AppendLine(line);
        }

        /// <summary>
        /// Move from the last point to the next along a quadratic bezier spline.
        /// </summary>
        /// <param name="x1">The X coordinate of quadratic bezier bezier control point.</param>
        /// <param name="y1">The Y coordinate of quadratic bezier bezier control point.</param>
        /// <param name="x2">The X coordinate of the new end point.</param>
        /// <param name="y2">The Y coordinate of the new end point.</param>
        public void QuadTo(double x1, double y1, double x2, double y2)
        {
            var quad = string.Format("Q {0},{1} {2},{3}", Format(x1), Format(y1), Format(x2), Format(y2));
             _sb.AppendLine(quad);
        }

        /// <summary>
        /// Move from the last point to the next along a cubic bezier spline.
        /// </summary>
        /// <param name="x1">The X coordinate of first cubic bezier spline off-curve control point.</param>
        /// <param name="y1">The Y coordinate of first cubic bezier spline off-curve control point.</param>
        /// <param name="x2">The X coordinate of second cubic bezier spline off-curve control point.</param>
        /// <param name="y2">The Y coordinate of second cubic bezier spline off-curve control point.</param>
        /// <param name="x3">The X coordinate of the new end point.</param>
        /// <param name="y3">The Y coordinate of the new end point.</param>
        public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            var curve = string.Format("C {0},{1} {2},{3} {4},{5}", Format(x1), Format(y1), Format(x2), Format(y2), Format(x3), Format(y3));
             _sb.AppendLine(curve);
        }

        /// <summary>
        /// Mark current control point knot. Currenlty not implemented, may be usefull for marking generated curves to original spiro code points.
        /// </summary>
        /// <param name="knotIndex">The current spiros control point knot index.</param>
        public void MarkKnot(int knotIndex)
        {
        }
    }
}

/*
libspiro - conversion between spiro control points and bezier's
Copyright (C) 2007 Raph Levien
              2015 converted to C# by Wiesław Šoltés

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

namespace SpiroNet
{
    /// <summary>
    /// Abstract type that handles the creation of particular representation of bézier splines.
    /// 
    /// Spiro will convert a set of spiro control points into a set of bézier curves. 
    /// 
    /// As it does so it will call the appropriate routine in your bézier context with this information 
    /// – this should allow you to create your own internal representation of those curves.
    /// </summary>
    public interface IBezierContext
    {
        /// <summary>
        /// Called by spiro to start a contour.
        /// </summary>
        /// <param name="x">The X coordinate of the new start point.</param>
        /// <param name="y">The Y coordinate of the new start point.</param>
        /// <param name="isOpen">An boolean flag indicating wheter spline is open (True) or closed (False).</param>
        void MoveTo(double x, double y, bool isOpen);

        /// <summary>
        /// Called by spiro to move from the last point to the next one on a straight line.
        /// </summary>
        /// <param name="x">The X coordinate of the new end point.</param>
        /// <param name="y">The Y coordinate of the new end point.</param>
        void LineTo(double x, double y);

        /// <summary>
        /// Called by spiro to move from the last point to the next along a quadratic bezier spline
        /// (x1,y1) is the quadratic bezier control point and (x2,y2) will be the new end point.
        /// </summary>
        /// <param name="x1">The X coordinate of quadratic bezier bezier control point.</param>
        /// <param name="y1">The Y coordinate of quadratic bezier bezier control point.</param>
        /// <param name="x2">The X coordinate of the new end point.</param>
        /// <param name="y2">The Y coordinate of the new end point.</param>
        void QuadTo(double x1, double y1, double x2, double y2);

        /// <summary>
        /// Called by spiro to move from the last point to the next along a cubic bezier spline
        /// (x1,y1) and (x2,y2) are the two off-curve control point and (x3,y3) will be the new end point.
        /// </summary>
        /// <param name="x1">The X coordinate of first cubic bezier spline off-curve control point.</param>
        /// <param name="y1">The Y coordinate of first cubic bezier spline off-curve control point.</param>
        /// <param name="x2">The X coordinate of second cubic bezier spline off-curve control point.</param>
        /// <param name="y2">The Y coordinate of second cubic bezier spline off-curve control point.</param>
        /// <param name="x3">The X coordinate of the new end point.</param>
        /// <param name="y3">The Y coordinate of the new end point.</param>
        void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3);

        /// <summary>
        /// Called by spiro to mark current control point knot.
        /// </summary>
        /// <param name="index">The spiros control point knot index.</param>
        /// <param name="theta">The spiros control point knot theta angle.</param>
        /// <param name="x">The spiros control point X location.</param>
        /// <param name="y">The spiros control point Y location.</param>
        /// <param name="type">The spiros control point type.</param>
        void MarkKnot(int index, double theta, double x, double y, SpiroPointType type);
    }
}

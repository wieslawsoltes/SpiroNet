/*
libspiro - conversion between spiro control points and bezier's
Copyright (C) 2007 Raph Levien
              2019 converted to C# by Wiesław Šoltés

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

namespace SpiroNet
{
    /// <summary>
    /// Possible values of the spiro control point Type property.
    /// </summary>
    public enum SpiroPointType
    {
        /// <summary>
        /// A corner point.
        /// Where the slopes and curvatures of the incoming and outgoing splines are unconstrained.
        /// </summary>
        Corner = 'v',

        /// <summary>
        /// A G4 curve point.
        /// Continuous up to the fourth derivative.
        /// </summary>
        G4 = 'o',

        /// <summary>
        /// A G2 curve point.
        /// Continuous up to the second derivative.
        /// </summary>
        G2 = 'c',

        /// <summary>
        /// A left constraint point.
        /// Used to connect a curved line to a straight one.
        /// </summary>
        Left = '[',

        /// <summary>
        /// A right constraint point.
        /// Used to connect a straight line to a curved one.
        /// If you have a contour which is drawn clockwise, and you have a straight segment at the top, then the left point of that straight segment should be a left constraint, and the right point should be a right constraint.
        /// </summary>
        Right = ']',

        /// <summary>
        /// End point.
        /// For a closed contour add an extra cp with a ty set to 'end'.
        /// </summary>
        End = 'z',

        /// <summary>
        /// Open contour.
        /// For an open contour the first cp must have a ty set to 'open contour'.
        /// </summary>
        OpenContour = '{',

        /// <summary>
        /// End open contour.
        /// For an open contour the last cp must have a ty set to 'end open contour'.
        /// </summary>
        EndOpenContour = '}'
    }
}

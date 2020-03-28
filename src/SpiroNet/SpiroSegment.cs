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
    /// The run_spiro() uses array of information given in the spiro control point structure 
    /// and creates an array in this structure format to use by spiro_to_bpath for building bezier curves.
    /// </summary>
    public struct SpiroSegment
    {
        /// <summary>
        /// Spiro code point segment_chord startX.
        /// </summary>
        public double X;

        /// <summary>
        /// Spiro code point segment_chord startY.
        /// </summary>
        public double Y;

        /// <summary>
        /// Spiro code point Type.
        /// </summary>
        public SpiroPointType Type;

        /// <summary>
        /// Bend theta between this vector and next vector.
        /// </summary>
        public double bend_th;

        /// <summary>
        /// A double's array of size 4.
        /// </summary>
        public double[] ks;

        /// <summary>
        /// The segment_chord distance from xy to next spiro code point.
        /// </summary>
        public double seg_ch;

        /// <summary>
        /// The segment_theta angle for this spiro code point.
        /// </summary>
        public double seg_th;

        /// <summary>
        /// Unused.
        /// </summary>
        public double l;

        /// <summary>
        /// Returns string rendering of object.
        /// </summary>
        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + Type.ToString() + " (" +
                seg_th.ToString() + ")";
        }
    }
}

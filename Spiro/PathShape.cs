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
using System.Collections.Generic;
using System.Linq;

namespace SpiroNet
{
    /// <summary>
    /// The spiro shape used to generate Path data.
    /// </summary>
    public class PathShape
    {
        /// <summary>
        /// Spiro control points array.
        /// </summary>
        public IList<SpiroControlPoint> Points { get; set; }

        /// <summary>
        /// Is closed spiro shape.
        /// Whether points describe a closed (True) or open (False) contour.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Is tagged spiro shape.
        /// This requires that spiro control points be tagged according to convention. A closed curve will have an extra control point attached to the end of it with a type of 'End'.
        /// The location of this last point is irrelevant.
        /// In an open contour the point types of the first and last control points are going to be ignored.
        /// </summary>
        public bool IsTagged { get; set; }

        /// <summary>
        /// The generated Path data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Generate Path shape data using path bezier context implementation.
        /// </summary>
        /// <returns>True when Data was generated successfully.</returns>
        public bool UpdateData()
        {
            var points = this.Points.ToArray();
            var bc = new PathBezierContext();

            if (this.IsTagged)
            {
                var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
                if (success)
                    this.Data = bc.ToString();
                else
                    this.Data = string.Empty;

                return success;
            }
            else
            {
                var success = Spiro.SpiroCPsToBezier0(points, points.Length, this.IsClosed, bc);
                if (success)
                    this.Data = bc.ToString();
                else
                    this.Data = string.Empty;

                return success;
            }
        }
    }
}

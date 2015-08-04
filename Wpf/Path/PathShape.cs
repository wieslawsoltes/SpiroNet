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
using System.Linq;

namespace SpiroNet.Wpf
{
    /// <summary>
    /// The spiro shape used to generate Path data.
    /// </summary>
    public class PathShape : ObservableObject
    {
        private bool _isStroked;
        private bool _isFilled;
        private bool _isClosed;
        private bool _isTagged;
        private IList<SpiroControlPoint> _points;

        /// <summary>
        /// Is stroked path shape.
        /// </summary>
        public bool IsStroked
        {
            get { return _isStroked; }
            set { Update(ref _isStroked, value); }
        }

        /// <summary>
        /// Is filled path shape.
        /// </summary>
        public bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
        }

        /// <summary>
        /// Is closed spiro shape.
        /// Whether points describe a closed (True) or open (False) contour.
        /// </summary>
        public bool IsClosed
        {
            get { return _isClosed; }
            set { Update(ref _isClosed, value); }
        }

        /// <summary>
        /// Is tagged spiro shape.
        /// This requires that spiro control points be tagged according to convention. A closed curve will have an extra control point attached to the end of it with a type of 'End'.
        /// The location of this last point is irrelevant.
        /// In an open contour the point types of the first and last control points are going to be ignored.
        /// </summary>
        public bool IsTagged
        {
            get { return _isTagged; }
            set { Update(ref _isTagged, value); }
        }

        /// <summary>
        /// Spiro control points array.
        /// </summary>
        public IList<SpiroControlPoint> Points
        {
            get { return _points; }
            set { Update(ref _points, value); }
        }

        /// <summary>
        /// Try to generate Path shape data from Points using path bezier context implementation.
        /// </summary>
        /// <returns>True when output data was generated successfully.</returns>
        public bool TryGetData(out string data)
        {
            var points = this.Points.ToArray();
            var bc = new PathBezierContext();

            if (this.IsTagged)
            {
                var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
                if (success)
                    data = bc.ToString();
                else
                    data = null;

                return success;
            }
            else
            {
                var success = Spiro.SpiroCPsToBezier0(points, points.Length, this.IsClosed, bc);
                if (success)
                    data = bc.ToString();
                else
                    data = null;

                return success;
            }
        }
    }
}

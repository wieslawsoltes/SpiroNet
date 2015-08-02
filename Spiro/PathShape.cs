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
    public class PathShape
    {
        public IList<SpiroControlPoint> Points { get; set; }
        public bool IsClosed { get; set; }
        public bool IsTagged { get; set; }
        public string Source { get; set; }

        public bool UpdateSource()
        {
            var points = this.Points.ToArray();
            var bc = new PathBezierContext();

            if (this.IsTagged)
            {
                var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
                if (success)
                    this.Source = bc.ToString();
                else
                    this.Source = string.Empty;

                return success;
            }
            else
            {
                var success = Spiro.SpiroCPsToBezier0(points, points.Length, this.IsClosed, bc);
                if (success)
                    this.Source = bc.ToString();
                else
                    this.Source = string.Empty;

                return success;
            }
        }
    }
}

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
    /// The spiro drawing using path shapes geometries. 
    /// </summary>
    public class PathDrawing : ObservableObject
    {
        private double _width;
        private double _height;
        private IList<PathShape> _shapes;

        /// <summary>
        /// Width of the drawing.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { Update(ref _width, value); }
        }

        /// <summary>
        /// Height of the drawing.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { Update(ref _height, value); }
        }

        /// <summary>
        /// Path shapes array.
        /// </summary>
        public IList<PathShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }
    }
}

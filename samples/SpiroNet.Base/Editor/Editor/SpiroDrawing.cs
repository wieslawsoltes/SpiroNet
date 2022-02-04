/*
SpiroNet.Editor
Copyright (C) 2019 Wiesław Šoltés

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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpiroNet.Editor;

/// <summary>
/// The spiro drawing using path shapes geometries. 
/// </summary>
public class SpiroDrawing : ObservableObject
{
    private double _width;
    private double _height;
    private IList<SpiroShape> _shapes;
    private IList<GuideLine> _guides;

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
    public IList<SpiroShape> Shapes
    {
        get { return _shapes; }
        set { Update(ref _shapes, value); }
    }

    /// <summary>
    /// Path guides array.
    /// </summary>
    public IList<GuideLine> Guides
    {
        get { return _guides; }
        set { Update(ref _guides, value); }
    }

    /// <summary>
    /// Creates a new <see cref="SpiroDrawing"/> instance.
    /// </summary>
    /// <param name="width">The drawing width</param>
    /// <param name="height"></param>
    /// <returns>The new instance of the <see cref="SpiroDrawing"/>.</returns>
    public static SpiroDrawing Create(double width, double height)
    {
        return new SpiroDrawing()
        {
            Width = width,
            Height = height,
            Shapes = new ObservableCollection<SpiroShape>(),
            Guides = new ObservableCollection<GuideLine>(),
        };
    }
}
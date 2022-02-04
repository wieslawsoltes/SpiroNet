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

namespace SpiroNet.Editor;

/// <summary>
/// Spiro control point knot. 
/// </summary>
public struct SpiroKnot
{
    /// <summary>
    /// The spiros control point knot index.
    /// </summary>
    public int Index;

    /// <summary>
    /// The spiros control point knot theta angle.
    /// </summary>
    public double Theta;

    /// <summary>
    /// The spiros control point X location.
    /// </summary>
    public double X;

    /// <summary>
    /// The spiros control point Y location.
    /// </summary>
    public double Y;

    /// <summary>
    /// The spiros control point type.
    /// </summary>
    public SpiroPointType Type;
}
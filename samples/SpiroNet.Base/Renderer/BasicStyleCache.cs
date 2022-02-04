/*
SpiroNet.Avalonia
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
using Avalonia.Media;

namespace SpiroNet.Editor.Avalonia.Renderer;

public class BasicStyleCache
{
    public IBrush FillBrush { get; private set; }
    public IBrush StrokeBrush { get; private set; }
    public Pen StrokePen { get; private set; }
    public double Thickness { get; private set; }
    public double HalfThickness { get; private set; }

    public static IBrush ToBrush(Argb color)
    {
        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
    }

    public BasicStyleCache(BasicStyle style)
    {
        if (style == null)
            return;

        Thickness = style.Thickness;
        HalfThickness = Thickness / 2.0;

        if (style.Fill != null)
        {
            FillBrush = ToBrush(style.Fill);
        }

        if (style.Stroke != null)
        {
            StrokeBrush = ToBrush(style.Stroke);

            StrokePen = new Pen(StrokeBrush, Thickness);
        }
    }
}
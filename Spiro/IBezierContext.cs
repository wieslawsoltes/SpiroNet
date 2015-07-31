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
using System;

namespace SpiroNet
{
    public interface IBezierContext
    {
        void MoveTo(double x, double y, bool isOpen);
        void LineTo(double x, double y);
        void QuadTo(double x1, double y1, double x2, double y2);
        void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3);
        void MarkKnot(int knotIndex);
    }
}

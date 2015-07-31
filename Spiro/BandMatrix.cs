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
    internal struct BandMatrix
    {
        public double[] a;  // new double[11];
        public double[] al; // new double[5];

        private void CopyFrom(ref BandMatrix from)
        {
            Array.Copy(from.a, 0, a, 0, 11);
            Array.Copy(from.al, 0, al, 0, 5);
        }

        public static void Copy(BandMatrix[] src, int srcIndex, BandMatrix[] dst, int dstIndex, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                dst[i + dstIndex].CopyFrom(ref src[i + srcIndex]);
            }
        }
    }
}

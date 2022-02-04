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

namespace SpiroNet;

/// <summary>
/// The band matrix.
/// </summary>
internal struct BandMatrix
{
    /// <summary>
    /// The band-diagonal matrix.
    /// A double's array of size 11.
    /// </summary>
    public double[] a;

    /// <summary>
    /// Lower part of band-diagonal decomposition.
    /// A double's array of size 5.
    /// </summary>
    public double[] al;

    /// <summary>
    /// Copy band matrix from source band matrix to current instance of band matrix.
    /// </summary>
    /// <param name="from">The source band matrix.</param>
    private void CopyFrom(ref BandMatrix from)
    {
        Array.Copy(from.a, 0, a, 0, 11);
        Array.Copy(from.al, 0, al, 0, 5);
    }

    /// <summary>
    /// The source band matrix.
    /// </summary>
    /// <param name="src">The source band matrix.</param>
    /// <param name="srcIndex">The source band matrix start element index.</param>
    /// <param name="dst">The destination band matrix.</param>
    /// <param name="dstIndex">The destination band matrix start element index.</param>
    /// <param name="length">Number of elements to copy from source band matrix.</param>
    public static void Copy(BandMatrix[] src, int srcIndex, BandMatrix[] dst, int dstIndex, int length)
    {
        for (int i = 0; i < length; ++i)
        {
            dst[i + dstIndex].CopyFrom(ref src[i + srcIndex]);
        }
    }
}
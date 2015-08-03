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
//#define CHECK_INPUT_FINITENESS
using System;

namespace SpiroNet
{
    /// <summary>
    /// C# implementation of third-order polynomial spirals.
    /// Internal implementation of spiro using ORDER equal to 12.
    /// </summary>
    internal static class SpiroImpl
    {
        /// <summary>
        /// Compute hypotenuse. The function returns what would be the square root of the sum of the squares of x and y (as per the Pythagorean theorem), but without incurring in undue overflow or underflow of intermediate values.
        /// </summary>
        /// <param name="x">The X floating point value corresponding to the legs of a right-angled triangle for which the hypotenuse is computed.</param>
        /// <param name="y">The Y floating point value corresponding to the legs of a right-angled triangle for which the hypotenuse is computed.</param>
        /// <returns>Returns the hypotenuse of a right-angled triangle whose legs are x and y.</returns>
        public static double hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Returns whether x is a finite value.
        /// A finite value is any floating-point value that is neither infinite nor NaN (Not-A-Number).
        /// 
        /// IsFinite() equivalent:
        /// http://stackoverflow.com/questions/10030070/isfinite-equivalent
        /// References:
        /// http://pubs.opengroup.org/onlinepubs/009604499/functions/isfinite.html
        /// http://msdn.microsoft.com/en-us/library/system.double.isinfinity.aspx
        /// http://msdn.microsoft.com/en-us/library/system.double.isnan.aspx
        /// </summary>
        /// <param name="x">A floating-point value.</param>
        /// <returns>A non-zero value (true) if x is finite; and zero (false) otherwise.</returns>
        public static int IsFinite(double x)
        {
            return !double.IsInfinity(x) && !double.IsNaN(x) ? 1 : 0;
        }

        public const int N = 4;

        // Integrate polynomial spiral curve over range -.5 .. .5.
        public static void integrate_spiro(double[] ks, double[] xy, int n)
        {
            double th1 = ks[0];
            double th2 = 0.5 * ks[1];
            double th3 = (1.0 / 6) * ks[2];
            double th4 = (1.0 / 24) * ks[3];
            double x, y;
            double ds = 1.0 / n;
            double ds2 = ds * ds;
            double ds3 = ds2 * ds;
            double k0 = ks[0] * ds;
            double k1 = ks[1] * ds;
            double k2 = ks[2] * ds;
            double k3 = ks[3] * ds;
            int i;
            double s = 0.5 * ds - 0.5;

            x = 0;
            y = 0;

            for (i = 0; i < n; i++)
            {
                double u, v;
                double km0, km1, km2, km3;

                if (n == 1)
                {
                    km0 = k0;
                    km1 = k1 * ds;
                    km2 = k2 * ds2;
                }
                else
                {
                    km0 = (((1.0 / 6) * k3 * s + 0.5 * k2) * s + k1) * s + k0;
                    km1 = ((0.5 * k3 * s + k2) * s + k1) * ds;
                    km2 = (k3 * s + k2) * ds2;
                }

                km3 = k3 * ds3;

                {
                    double t1_1 = km0;
                    double t1_2 = 0.5 * km1;
                    double t1_3 = (1.0 / 6) * km2;
                    double t1_4 = (1.0 / 24) * km3;
                    double t2_2 = t1_1 * t1_1;
                    double t2_3 = 2 * (t1_1 * t1_2);
                    double t2_4 = 2 * (t1_1 * t1_3) + t1_2 * t1_2;
                    double t2_5 = 2 * (t1_1 * t1_4 + t1_2 * t1_3);
                    double t2_6 = 2 * (t1_2 * t1_4) + t1_3 * t1_3;
                    double t2_7 = 2 * (t1_3 * t1_4);
                    double t2_8 = t1_4 * t1_4;
                    double t3_4 = t2_2 * t1_2 + t2_3 * t1_1;
                    double t3_6 = t2_2 * t1_4 + t2_3 * t1_3 + t2_4 * t1_2 + t2_5 * t1_1;
                    double t3_8 = t2_4 * t1_4 + t2_5 * t1_3 + t2_6 * t1_2 + t2_7 * t1_1;
                    double t3_10 = t2_6 * t1_4 + t2_7 * t1_3 + t2_8 * t1_2;
                    double t4_4 = t2_2 * t2_2;
                    double t4_5 = 2 * (t2_2 * t2_3);
                    double t4_6 = 2 * (t2_2 * t2_4) + t2_3 * t2_3;
                    double t4_7 = 2 * (t2_2 * t2_5 + t2_3 * t2_4);
                    double t4_8 = 2 * (t2_2 * t2_6 + t2_3 * t2_5) + t2_4 * t2_4;
                    double t4_9 = 2 * (t2_2 * t2_7 + t2_3 * t2_6 + t2_4 * t2_5);
                    double t4_10 = 2 * (t2_2 * t2_8 + t2_3 * t2_7 + t2_4 * t2_6) + t2_5 * t2_5;
                    double t5_6 = t4_4 * t1_2 + t4_5 * t1_1;
                    double t5_8 = t4_4 * t1_4 + t4_5 * t1_3 + t4_6 * t1_2 + t4_7 * t1_1;
                    double t5_10 = t4_6 * t1_4 + t4_7 * t1_3 + t4_8 * t1_2 + t4_9 * t1_1;
                    double t6_6 = t4_4 * t2_2;
                    double t6_7 = t4_4 * t2_3 + t4_5 * t2_2;
                    double t6_8 = t4_4 * t2_4 + t4_5 * t2_3 + t4_6 * t2_2;
                    double t6_9 = t4_4 * t2_5 + t4_5 * t2_4 + t4_6 * t2_3 + t4_7 * t2_2;
                    double t6_10 = t4_4 * t2_6 + t4_5 * t2_5 + t4_6 * t2_4 + t4_7 * t2_3 + t4_8 * t2_2;
                    double t7_8 = t6_6 * t1_2 + t6_7 * t1_1;
                    double t7_10 = t6_6 * t1_4 + t6_7 * t1_3 + t6_8 * t1_2 + t6_9 * t1_1;
                    double t8_8 = t6_6 * t2_2;
                    double t8_9 = t6_6 * t2_3 + t6_7 * t2_2;
                    double t8_10 = t6_6 * t2_4 + t6_7 * t2_3 + t6_8 * t2_2;
                    double t9_10 = t8_8 * t1_2 + t8_9 * t1_1;
                    double t10_10 = t8_8 * t2_2;
                    u = 1;
                    v = 0;
                    v += (1.0 / 12) * t1_2 + (1.0 / 80) * t1_4;
                    u -= (1.0 / 24) * t2_2 + (1.0 / 160) * t2_4 + (1.0 / 896) * t2_6 + (1.0 / 4608) * t2_8;
                    v -= (1.0 / 480) * t3_4 + (1.0 / 2688) * t3_6 + (1.0 / 13824) * t3_8 + (1.0 / 67584) * t3_10;
                    u += (1.0 / 1920) * t4_4 + (1.0 / 10752) * t4_6 + (1.0 / 55296) * t4_8 + (1.0 / 270336) * t4_10;
                    v += (1.0 / 53760) * t5_6 + (1.0 / 276480) * t5_8 + (1.0 / 1.35168e+06) * t5_10;
                    u -= (1.0 / 322560) * t6_6 + (1.0 / 1.65888e+06) * t6_8 + (1.0 / 8.11008e+06) * t6_10;
                    v -= (1.0 / 1.16122e+07) * t7_8 + (1.0 / 5.67706e+07) * t7_10;
                    u += (1.0 / 9.28973e+07) * t8_8 + (1.0 / 4.54164e+08) * t8_10;
                    v += (1.0 / 4.08748e+09) * t9_10;
                    u -= (1.0 / 4.08748e+10) * t10_10;
                }

                if (n == 1)
                {
                    x = u;
                    y = v;
                }
                else
                {
                    double th = (((th4 * s + th3) * s + th2) * s + th1) * s;
                    double cth = Math.Cos(th);
                    double sth = Math.Sin(th);
                    x += cth * u - sth * v;
                    y += cth * v + sth * u;
                    s += ds;
                }
            }

            xy[0] = x * ds;
            xy[1] = y * ds;
        }

        public static double compute_ends(double[] ks, double[][] ends, double seg_ch)
        {
            double[] xy = new double[2];
            double ch, th;
            double l, l2, l3;
            double th_even, th_odd;
            double k0_even, k0_odd;
            double k1_even, k1_odd;
            double k2_even, k2_odd;

            integrate_spiro(ks, xy, N);
            ch = hypot(xy[0], xy[1]);
            th = Math.Atan2(xy[1], xy[0]);
            l = ch / seg_ch;

            th_even = 0.5 * ks[0] + (1.0 / 48) * ks[2];
            th_odd = 0.125 * ks[1] + (1.0 / 384) * ks[3] - th;
            ends[0][0] = th_even - th_odd;
            ends[1][0] = th_even + th_odd;
            k0_even = l * (ks[0] + 0.125 * ks[2]);
            k0_odd = l * (0.5 * ks[1] + (1.0 / 48) * ks[3]);
            ends[0][1] = k0_even - k0_odd;
            ends[1][1] = k0_even + k0_odd;
            l2 = l * l;
            k1_even = l2 * (ks[1] + 0.125 * ks[3]);
            k1_odd = l2 * 0.5 * ks[2];
            ends[0][2] = k1_even - k1_odd;
            ends[1][2] = k1_even + k1_odd;
            l3 = l2 * l;
            k2_even = l3 * ks[2];
            k2_odd = l3 * 0.5 * ks[3];
            ends[0][3] = k2_even - k2_odd;
            ends[1][3] = k2_even + k2_odd;

            return l;
        }

        public static void compute_pderivs(ref SpiroSegment s, double[][] ends, double[][][] derivs, int jinc)
        {
            double recip_d = 2e6;
            double delta = 1.0 / recip_d;
            double[] try_ks = new double[4];
            double[][] try_ends = { new double[4], new double[4] };
            int i, j, k;

            compute_ends(s.ks, ends, s.seg_ch);
            for (i = 0; i < jinc; i++)
            {
                for (j = 0; j < 4; j++)
                    try_ks[j] = s.ks[j];

                try_ks[i] += delta;
                compute_ends(try_ks, try_ends, s.seg_ch);

                for (k = 0; k < 2; k++)
                    for (j = 0; j < 4; j++)
                        derivs[j][k][i] = recip_d * (try_ends[k][j] - ends[k][j]);
            }
        }

        public static double mod_2pi(double th)
        {
            double u = th / (2 * Math.PI);
            return 2 * Math.PI * (u - Math.Floor(u + 0.5));
        }

        public static SpiroSegment[] setup_path(SpiroControlPoint[] src, int n)
        {
            int i, ilast, n_seg;
            double dx, dy;
            SpiroSegment[] r;

#if CHECK_INPUT_FINITENESS
            // Verify that input values are within realistic limits
            for (i = 0; i < n; i++)
            {
                if (IsFinite(src[i].X) == 0 || IsFinite(src[i].Y) == 0)
                {
                    return null;
                }
            }
#endif
            n_seg = src[0].Type == SpiroPointType.OpenContour ? n - 1 : n;

            r = new SpiroSegment[n_seg + 1];
            for (int j = 0; j < n_seg + 1; j++)
                r[j].ks = new double[4];

            if (r == null)
                return null;

            for (i = 0; i < n_seg; i++)
            {
                r[i].X = src[i].X;
                r[i].Y = src[i].Y;
                r[i].Type = src[i].Type;
                r[i].ks[0] = 0.0;
                r[i].ks[1] = 0.0;
                r[i].ks[2] = 0.0;
                r[i].ks[3] = 0.0;
            }
            r[n_seg].X = src[n_seg % n].X;
            r[n_seg].Y = src[n_seg % n].Y;
            r[n_seg].Type = src[n_seg % n].Type;

            for (i = 0; i < n_seg; i++)
            {
                dx = r[i + 1].X - r[i].X;
                dy = r[i + 1].Y - r[i].Y;
#if !CHECK_INPUT_FINITENESS
                r[i].seg_ch = hypot(dx, dy);
#else
                if (IsFinite(dx) == 0 || IsFinite(dy) == 0 || IsFinite((r[i].seg_ch = hypot(dx, dy))) == 0)
                {
                    return null;
                }
#endif
                r[i].seg_th = Math.Atan2(dy, dx);
            }

            ilast = n_seg - 1;

            for (i = 0; i < n_seg; i++)
            {
                if (r[i].Type == SpiroPointType.OpenContour || r[i].Type == SpiroPointType.EndOpenContour || r[i].Type == SpiroPointType.Corner)
                    r[i].bend_th = 0.0;
                else
                    r[i].bend_th = mod_2pi(r[i].seg_th - r[ilast].seg_th);

                ilast = i;
            }

            return r;
        }

        public static void bandec11(BandMatrix[] m, int[] perm, int n)
        {
            int i, j, k, l, pivot;
            double pivot_val, pivot_scale, tmp, x;

            // pack top triangle to the left.
            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < i + 6; j++)
                    m[i].a[j] = m[i].a[j + 5 - i];

                for (; j < 11; j++)
                    m[i].a[j] = 0.0;
            }

            l = 5;

            for (k = 0; k < n; k++)
            {
                pivot = k;
                pivot_val = m[k].a[0];

                l = l < n ? l + 1 : n;

                for (j = k + 1; j < l; j++)
                    if (Math.Abs(m[j].a[0]) > Math.Abs(pivot_val))
                    {
                        pivot_val = m[j].a[0];
                        pivot = j;
                    }

                perm[k] = pivot;

                if (pivot != k)
                {
                    for (j = 0; j < 11; j++)
                    {
                        tmp = m[k].a[j];
                        m[k].a[j] = m[pivot].a[j];
                        m[pivot].a[j] = tmp;
                    }
                }

                if (Math.Abs(pivot_val) < 1e-12)
                    pivot_val = 1e-12;

                pivot_scale = 1.0 / pivot_val;

                for (i = k + 1; i < l; i++)
                {
                    x = m[i].a[0] * pivot_scale;
                    m[k].al[i - k - 1] = x;

                    for (j = 1; j < 11; j++)
                        m[i].a[j - 1] = m[i].a[j] - x * m[k].a[j];

                    m[i].a[10] = 0.0;
                }
            }
        }

        public static void banbks11(BandMatrix[] m, int[] perm, double[] v, int n)
        {
            int i, k, l;
            double tmp, x;

            // forward substitution
            l = 5;

            for (k = 0; k < n; k++)
            {
                i = perm[k];

                if (i != k)
                {
                    tmp = v[k];
                    v[k] = v[i];
                    v[i] = tmp;
                }

                if (l < n)
                    l++;

                for (i = k + 1; i < l; i++)
                    v[i] -= m[k].al[i - k - 1] * v[k];
            }

            // back substitution
            l = 1;

            for (i = n - 1; i >= 0; i--)
            {
                x = v[i];

                for (k = 1; k < l; k++)
                    x -= m[i].a[k] * v[k + i];

                v[i] = x / m[i].a[0];

                if (l < 11)
                    l++;
            }
        }

        public static int compute_jinc(SpiroPointType ty0, SpiroPointType ty1)
        {
            if (ty0 == SpiroPointType.G4 || ty1 == SpiroPointType.G4 || ty0 == SpiroPointType.Right || ty1 == SpiroPointType.Left)
                return 4;
            else if (ty0 == SpiroPointType.G2 && ty1 == SpiroPointType.G2)
                return 2;
            else if (((ty0 == SpiroPointType.OpenContour || ty0 == SpiroPointType.Corner || ty0 == SpiroPointType.Left) && ty1 == SpiroPointType.G2) || (ty0 == SpiroPointType.G2 && (ty1 == SpiroPointType.EndOpenContour || ty1 == SpiroPointType.Corner || ty1 == SpiroPointType.Right)))
                return 1;
            else
                return 0;
        }

        public static int count_vec(SpiroSegment[] s, int nseg)
        {
            int i, n;

            n = 0;

            for (i = 0; i < nseg; i++)
                n += compute_jinc(s[i].Type, s[i + 1].Type);

            return n;
        }

        public static void add_mat_line(BandMatrix[] m, double[] v, double[] derivs, double x, double y, int j, int jj, int jinc, int nmat)
        {
            int joff, k;

            if (jj >= 0)
            {
                joff = (j + 5 - jj + nmat) % nmat;

                if (nmat < 6)
                {
                    joff = j + 5 - jj;
                }
                else if (nmat == 6)
                {
                    joff = 2 + (j + 3 - jj + nmat) % nmat;
                }

                v[jj] += x;

                for (k = 0; k < jinc; k++)
                    m[jj].a[joff + k] += y * derivs[k];
            }
        }

        public static double spiro_iter(SpiroSegment[] s, BandMatrix[] m, int[] perm, double[] v, int n, int nmat)
        {
            bool cyclic;
            int i, j, jthl, jthr, jk0l, jk0r, jk1l, jk1r, jk2l, jk2r, jinc, jj, k, n_invert;
            SpiroPointType ty0, ty1;
            double dk, norm, th;
            double[][] ends = { new double[4], new double[4] };
            double[][][] derivs =
            {
                new double[2][] { new double[4], new double[4] },
                new double[2][] { new double[4], new double[4] },
                new double[2][] { new double[4], new double[4] },
                new double[2][] { new double[4], new double[4] },
            };

            cyclic = s[0].Type != SpiroPointType.OpenContour && s[0].Type != SpiroPointType.Corner;

            for (i = 0; i < nmat; i++)
            {
                v[i] = 0.0;

                for (j = 0; j < 11; j++)
                    m[i].a[j] = 0.0;

                for (j = 0; j < 5; j++)
                    m[i].al[j] = 0.0;
            }

            j = 0;

            if (s[0].Type == SpiroPointType.G4)
                jj = nmat - 2;
            else if (s[0].Type == SpiroPointType.G2)
                jj = nmat - 1;
            else
                jj = 0;

            for (i = 0; i < n; i++)
            {
                ty0 = s[i].Type;
                ty1 = s[i + 1].Type;
                jinc = compute_jinc(ty0, ty1);
                th = s[i].bend_th;
                jthl = jk0l = jk1l = jk2l = -1;
                jthr = jk0r = jk1r = jk2r = -1;

                compute_pderivs(ref s[i], ends, derivs, jinc);

                // constraints crossing left
                if (ty0 == SpiroPointType.G4 || ty0 == SpiroPointType.G2 || ty0 == SpiroPointType.Left || ty0 == SpiroPointType.Right)
                {
                    jthl = jj++;
                    jj %= nmat;
                    jk0l = jj++;

                    if (ty0 == SpiroPointType.G4)
                    {
                        jj %= nmat;
                        jk1l = jj++;
                        jk2l = jj++;
                    }
                }

                // constraints on left
                if ((ty0 == SpiroPointType.Left || ty0 == SpiroPointType.Corner || ty0 == SpiroPointType.OpenContour || ty0 == SpiroPointType.G2) && jinc == 4)
                {
                    if (ty0 != SpiroPointType.G2)
                        jk1l = jj++;

                    jk2l = jj++;
                }

                // constraints on right
                if ((ty1 == SpiroPointType.Right || ty1 == SpiroPointType.Corner || ty1 == SpiroPointType.EndOpenContour || ty1 == SpiroPointType.G2) && jinc == 4)
                {
                    if (ty1 != SpiroPointType.G2)
                        jk1r = jj++;

                    jk2r = jj++;
                }

                // constraints crossing right
                if (ty1 == SpiroPointType.G4 || ty1 == SpiroPointType.G2 || ty1 == SpiroPointType.Left || ty1 == SpiroPointType.Right)
                {
                    jthr = jj;
                    jk0r = (jj + 1) % nmat;

                    if (ty1 == SpiroPointType.G4)
                    {
                        jk1r = (jj + 2) % nmat;
                        jk2r = (jj + 3) % nmat;
                    }
                }

                add_mat_line(m, v, derivs[0][0], th - ends[0][0], 1, j, jthl, jinc, nmat);
                add_mat_line(m, v, derivs[1][0], ends[0][1], -1, j, jk0l, jinc, nmat);
                add_mat_line(m, v, derivs[2][0], ends[0][2], -1, j, jk1l, jinc, nmat);
                add_mat_line(m, v, derivs[3][0], ends[0][3], -1, j, jk2l, jinc, nmat);
                add_mat_line(m, v, derivs[0][1], -ends[1][0], 1, j, jthr, jinc, nmat);
                add_mat_line(m, v, derivs[1][1], -ends[1][1], 1, j, jk0r, jinc, nmat);
                add_mat_line(m, v, derivs[2][1], -ends[1][2], 1, j, jk1r, jinc, nmat);
                add_mat_line(m, v, derivs[3][1], -ends[1][3], 1, j, jk2r, jinc, nmat);

                if (jthl >= 0)
                    v[jthl] = mod_2pi(v[jthl]);

                if (jthr >= 0)
                    v[jthr] = mod_2pi(v[jthr]);

                j += jinc;
            }

            if (cyclic)
            {
                BandMatrix.Copy(m, 0, m, nmat, nmat);
                BandMatrix.Copy(m, 0, m, 2 * nmat, nmat);
                Array.Copy(v, 0, v, nmat, nmat);
                Array.Copy(v, 0, v, 2 * nmat, nmat);
                n_invert = 3 * nmat;
                j = nmat;
            }
            else
            {
                n_invert = nmat;
                j = 0;
            }

            bandec11(m, perm, n_invert);
            banbks11(m, perm, v, n_invert);
            norm = 0.0;

            for (i = 0; i < n; i++)
            {
                jinc = compute_jinc(s[i].Type, s[i + 1].Type);

                for (k = 0; k < jinc; k++)
                {
                    dk = v[j++];

                    s[i].ks[k] += dk;
                    norm += dk * dk;
                }

                s[i].ks[0] = 2.0 * mod_2pi(s[i].ks[0] / 2.0);
            }

            return norm;
        }

        public static bool check_finiteness(SpiroSegment[] segs, int num_segs)
        {
            // Check if all values are "finite", return true, else return fail=false
            int i, j;

            for (i = 0; i < num_segs; ++i)
                for (j = 0; j < 4; ++j)
                    if (IsFinite(segs[i].ks[j]) == 0)
                        return false;

            return true;
        }

        public static int solve_spiro(SpiroSegment[] s, int nseg)
        {
            int i, converged, nmat, n_alloc;
            BandMatrix[] m;
            double[] v;
            int[] perm;
            double norm;

            nmat = count_vec(s, nseg);
            n_alloc = nmat;

            if (nmat == 0)
                return 1; // just means no convergence problems
            if (s[0].Type != SpiroPointType.OpenContour && s[0].Type != SpiroPointType.Corner)
                n_alloc *= 3;
            if (n_alloc < 5)
                n_alloc = 5;

            m = new BandMatrix[n_alloc];
            for (int n = 0; n < n_alloc; n++)
            {
                m[n].a = new double[11];
                m[n].al = new double[5];
            }

            v = new double[n_alloc];
            perm = new int[n_alloc];

            i = converged = 0; // not solved (yet)
            if (m != null && v != null && perm != null)
            {
                while (i++ < 60)
                {
                    norm = spiro_iter(s, m, perm, v, nseg, nmat);
                    if (check_finiteness(s, nseg) == false)
                        break;

                    if (norm < 1e-12)
                    {
                        converged = 1;
                        break;
                    }
                }
            }

            return converged;
        }

        public static void spiro_seg_to_bpath(double[] ks, double x0, double y0, double x1, double y1, IBezierContext bc, int depth)
        {
            double bend, seg_ch, seg_th, ch, th, scale, rot;
            double th_even, th_odd, ul, vl, ur, vr;
            double thsub, xmid, ymid, cth, sth;
            double[] ksub = new double[4]; double[] xysub = new double[2]; double[] xy = new double[2];

            bend = Math.Abs(ks[0]) + Math.Abs(0.5 * ks[1]) + Math.Abs(0.125 * ks[2]) + Math.Abs((1.0 / 48) * ks[3]);

            if (bend <= 1e-8)
            {
                bc.LineTo(x1, y1);
            }
            else
            {
                seg_ch = hypot(x1 - x0, y1 - y0);
                seg_th = Math.Atan2(y1 - y0, x1 - x0);

                integrate_spiro(ks, xy, N);
                ch = hypot(xy[0], xy[1]);
                th = Math.Atan2(xy[1], xy[0]);
                scale = seg_ch / ch;
                rot = seg_th - th;

                if (depth > 5 || bend < 1.0)
                {
                    th_even = (1.0 / 384) * ks[3] + (1.0 / 8) * ks[1] + rot;
                    th_odd = (1.0 / 48) * ks[2] + 0.5 * ks[0];
                    ul = (scale * (1.0 / 3)) * Math.Cos(th_even - th_odd);
                    vl = (scale * (1.0 / 3)) * Math.Sin(th_even - th_odd);
                    ur = (scale * (1.0 / 3)) * Math.Cos(th_even + th_odd);
                    vr = (scale * (1.0 / 3)) * Math.Sin(th_even + th_odd);
                    bc.CurveTo(x0 + ul, y0 + vl, x1 - ur, y1 - vr, x1, y1);
                }
                else
                {
                    // subdivide
                    ksub[0] = .5 * ks[0] - .125 * ks[1] + (1.0 / 64) * ks[2] - (1.0 / 768) * ks[3];
                    ksub[1] = .25 * ks[1] - (1.0 / 16) * ks[2] + (1.0 / 128) * ks[3];
                    ksub[2] = .125 * ks[2] - (1.0 / 32) * ks[3];
                    ksub[3] = (1.0 / 16) * ks[3];
                    thsub = rot - .25 * ks[0] + (1.0 / 32) * ks[1] - (1.0 / 384) * ks[2] + (1.0 / 6144) * ks[3];
                    cth = .5 * scale * Math.Cos(thsub);
                    sth = .5 * scale * Math.Sin(thsub);
                    integrate_spiro(ksub, xysub, N);
                    xmid = x0 + cth * xysub[0] - sth * xysub[1];
                    ymid = y0 + cth * xysub[1] + sth * xysub[0];
                    spiro_seg_to_bpath(ksub, x0, y0, xmid, ymid, bc, depth + 1);
                    ksub[0] += .25 * ks[1] + (1.0 / 384) * ks[3];
                    ksub[1] += .125 * ks[2];
                    ksub[2] += (1.0 / 16) * ks[3];
                    spiro_seg_to_bpath(ksub, xmid, ymid, x1, y1, bc, depth + 1);
                }
            }
        }

        public static SpiroSegment[] run_spiro(SpiroControlPoint[] src, int n)
        {
            int converged, nseg;
            SpiroSegment[] s;

            if (src == null || n <= 0)
                return null;

            s = setup_path(src, n);

            if (s != null)
            {
                nseg = src[0].Type == SpiroPointType.OpenContour ? n - 1 : n;
                converged = 1; // this value is for when nseg == 1; else actual value determined below
                if (nseg > 1)
                    converged = solve_spiro(s, nseg);

                if (converged != 0)
                    return s;
            }

            return null;
        }

        public static void spiro_to_bpath(SpiroSegment[] s, int n, IBezierContext bc)
        {
            int i, nsegs;
            double x0, y0, x1, y1;

            if (s == null || n <= 0 || bc == null)
                return;

            nsegs = s[n - 1].Type == SpiroPointType.EndOpenContour ? n - 1 : n;

            for (i = 0; i < nsegs; i++)
            {
                x0 = s[i].X; x1 = s[i + 1].X;
                y0 = s[i].Y; y1 = s[i + 1].Y;

                if (i == 0)
                    bc.MoveTo(x0, y0, s[0].Type == SpiroPointType.OpenContour ? true : false);

                bc.MarkKnot(i);
                spiro_seg_to_bpath(s[i].ks, x0, y0, x1, y1, bc, 0);
            }
        }

        public static double get_knot_th(SpiroSegment[] s, int i)
        {
            double[][] ends = { new double[4], new double[4] };

            if (i == 0)
            {
                compute_ends(s[i].ks, ends, s[i].seg_ch);
                return s[i].seg_th - ends[0][0];
            }
            else
            {
                compute_ends(s[i - 1].ks, ends, s[i - 1].seg_ch);
                return s[i - 1].seg_th + ends[1][0];
            }
        }
    }
}

/*
ppedit - A pattern plate editor for Spiro splines.
Copyright (C) 2007 Raph Levien
              2019 converted to C# by Wiesław Šoltés

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
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

namespace SpiroNet.Editor;

public class HitTestBezierContext : IBezierContext
{
    private struct HitTestState
    {
        public double x;
        public double y;
        public double x0;
        public double y0;
        public int knot_idx;
        public int knot_idx_min;
        public double r_min;
    }

    private HitTestState _state;

    private static double cube(double x) { return x * x * x; }

    private static double my_cbrt(double x)
    {
        if (x >= 0)
            return Math.Pow(x, 1.0 / 3.0);
        else
            return -Math.Pow(-x, 1.0 / 3.0);
    }

    private static double hypot(double x, double y)
    {
        return Math.Sqrt(x * x + y * y);
    }

    private static int solve_cubic(double c0, double c1, double c2, double c3, double[/*3*/] root)
    {
        // Give real roots to eqn c0 + c1 * x + c2 * x^2 + c3 * x^3 == 0.
        // Return value is number of roots found.

        double p, q, r, a, b, Q, x0;

        p = c2 / c3;
        q = c1 / c3;
        r = c0 / c3;
        a = (3 * q - p * p) / 3;
        b = (2 * cube(p) - 9 * p * q + 27 * r) / 27;
        Q = b * b / 4 + cube(a) / 27;
        x0 = p / 3;

        if (Q > 0)
        {
            double sQ = Math.Sqrt(Q);
            double t1 = my_cbrt(-b / 2 + sQ) + my_cbrt(-b / 2 - sQ);
            root[0] = t1 - x0;
            return 1;
        }
        else if (Q == 0)
        {
            double t1 = my_cbrt(b / 2);
            double x1 = t1 - x0;
            root[0] = x1;
            root[1] = x1;
            root[2] = -2 * t1 - x0;
            return 3;
        }
        else
        {
            double sQ = Math.Sqrt(-Q);
            double rho = hypot(-b / 2, sQ);
            double th = Math.Atan2(sQ, -b / 2);
            double cbrho = my_cbrt(rho);
            double c = Math.Cos(th / 3);
            double s = Math.Sin(th / 3);
            double sqr3 = Math.Sqrt(3);
            root[0] = 2 * cbrho * c - x0;
            root[1] = -cbrho * (c + sqr3 * s) - x0;
            root[2] = -cbrho * (c - sqr3 * s) - x0;
            return 3;
        }
    }

    private static double dist_to_quadratic(double x, double y, double x0, double y0, double x1, double y1, double x2, double y2)
    {
        double u0, u1, t0, t1, t2, c0, c1, c2, c3;
        double[] roots = new double[3];
        int n_roots;
        double[] ts = new double[4];
        int n_ts;
        int i;
        double minerr = 0;

        u0 = x1 - x0;
        u1 = x0 - 2 * x1 + x2;
        t0 = x0 - x;
        t1 = 2 * u0;
        t2 = u1;
        c0 = t0 * u0;
        c1 = t1 * u0 + t0 * u1;
        c2 = t2 * u0 + t1 * u1;
        c3 = t2 * u1;

        u0 = y1 - y0;
        u1 = y0 - 2 * y1 + y2;
        t0 = y0 - y;
        t1 = 2 * u0;
        t2 = u1;
        c0 += t0 * u0;
        c1 += t1 * u0 + t0 * u1;
        c2 += t2 * u0 + t1 * u1;
        c3 += t2 * u1;

        n_roots = solve_cubic(c0, c1, c2, c3, roots);
        n_ts = 0;

        for (i = 0; i < n_roots; i++)
        {
            double t = roots[i];
            if (t > 0 && t < 1)
                ts[n_ts++] = t;
        }

        if (n_ts < n_roots)
        {
            ts[n_ts++] = 0;
            ts[n_ts++] = 1;
        }

        for (i = 0; i < n_ts; i++)
        {
            double t = ts[i];
            double xa = x0 * (1 - t) * (1 - t) + 2 * x1 * (1 - t) * t + x2 * t * t;
            double ya = y0 * (1 - t) * (1 - t) + 2 * y1 * (1 - t) * t + y2 * t * t;
            double err = hypot(xa - x, ya - y);
            if (i == 0 || err < minerr)
            {
                minerr = err;
            }
        }

        return minerr;
    }

    public double Report(out int knotIndex)
    {
        double r_min = _state.r_min;
        knotIndex = _state.knot_idx_min;
        return r_min;
    }

    public HitTestBezierContext(double x, double y)
    {
        _state = new HitTestState();
        _state.x = x;
        _state.y = y;
        _state.knot_idx_min = -1;
        _state.r_min = 1e12;
    }

    public void MoveTo(double x, double y, bool isOpen)
    {
        _state.x0 = x;
        _state.y0 = y;
    }

    public void LineTo(double x, double y)
    {
        double x0 = _state.x0;
        double y0 = _state.y0;
        double dx = x - x0;
        double dy = y - y0;
        double dotp = (_state.x - x0) * dx + (_state.y - y0) * dy;
        double lin_dotp = dx * dx + dy * dy;
        double r_min, r;

        r = hypot(_state.x - x0, _state.y - y0);
        r_min = r;
        r = hypot(_state.x - x, _state.y - y);
        if (r < r_min)
            r_min = r;

        if (dotp >= 0 && dotp <= lin_dotp)
        {
            double norm = (_state.x - x0) * dy - (_state.y - y0) * dx;
            r = Math.Abs(norm / Math.Sqrt(lin_dotp));
            if (r < r_min)
                r_min = r;
        }

        if (r_min < _state.r_min)
        {
            _state.r_min = r_min;
            _state.knot_idx_min = _state.knot_idx;
        }

        _state.x0 = x;
        _state.y0 = y;
    }

    public void QuadTo(double x1, double y1, double x2, double y2)
    {
        double r = dist_to_quadratic(_state.x, _state.y, _state.x0, _state.y0, x1, y1, x2, y2);

        if (r < _state.r_min)
        {
            _state.r_min = r;
            _state.knot_idx_min = _state.knot_idx;
        }

        _state.x0 = x2;
        _state.y0 = y2;
    }

    public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        double x0 = _state.x0;
        double y0 = _state.y0;
        const int n_subdiv = 32;
        int i;
        double xq2, yq2;

        // TODO: Subdivide to quadratics rather than lines.
        for (i = 0; i < n_subdiv; i++)
        {
            double t = (1.0 / n_subdiv) * (i + 1);
            double mt = 1 - t;
            xq2 = x0 * mt * mt * mt + 3 * x1 * mt * t * t + 3 * x2 * mt * mt * t + x3 * t * t * t;
            yq2 = y0 * mt * mt * mt + 3 * y1 * mt * t * t + 3 * y2 * mt * mt * t + y3 * t * t * t;
            LineTo(xq2, yq2);
        }
    }

    public void MarkKnot(int index, double theta, double x, double y, SpiroPointType type)
    {
        _state.knot_idx = index;
    }
}
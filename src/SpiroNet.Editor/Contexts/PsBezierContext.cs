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
using System.Globalization;
using System.Text;

namespace SpiroNet.Editor
{
    public class PsBezierContext : IBezierContext
    {
        private static CultureInfo ToStringCulture = new CultureInfo("en-GB");

        private struct PsState
        {
            public bool isOpen;
            public double x;
            public double y;
            public StringBuilder sb;
        }

        public const string PsProlog =
            "%!PS\n" +
            "/m { moveto } bind def\n" +
            "/l { lineto } bind def\n" +
            "/c { curveto } bind def\n" +
            "/z { closepath } bind def\n";

        public const string PsSize =
            "<</PageSize [{0} {1}]>>setpagedevice\n" + // {0} = width, {1} = height
            "1 -1 scale\n" +
            "0 -{1} translate\n"; // {0} = height

        public const string PsPostlog =
            "stroke\n" +
            "showpage\n";

        private PsState _state;

        private static string Format(double value)
        {
            return value.ToString(ToStringCulture);
        }

        public string GetData()
        {
            if (!_state.isOpen)
                _state.sb.Append("z\n");
            return _state.sb.ToString();
        }

        public override string ToString()
        {
            return GetData();
        }

        public PsBezierContext()
        {
            _state = new PsState();
            _state.isOpen = true;
            _state.sb = new StringBuilder();
        }

        public void MoveTo(double x, double y, bool isOpen)
        {
            if (!_state.isOpen)
                _state.sb.Append("z\n");
            _state.sb.Append(string.Format("{0} {1} m\n", Format(x), Format(y)));
            _state.isOpen = isOpen;
            _state.x = x;
            _state.y = y;
        }

        public void LineTo(double x, double y)
        {
            _state.sb.Append(string.Format("{0} {1} l\n", Format(x), Format(y)));
            _state.x = x;
            _state.y = y;
        }

        public void QuadTo(double xm, double ym, double x3, double y3)
        {
            double x0, y0;
            double x1, y1;
            double x2, y2;
            x0 = _state.x;
            y0 = _state.y;
            x1 = xm + (1.0 / 3) * (x0 - xm);
            y1 = ym + (1.0 / 3) * (y0 - ym);
            x2 = xm + (1.0 / 3) * (x3 - xm);
            y2 = ym + (1.0 / 3) * (y3 - ym);
            _state.sb.Append(string.Format("{0} {1} {2} {3} {4} {5} c\n", Format(x1), Format(y1), Format(x2), Format(y2), Format(x3), Format(y3)));
            _state.x = x3;
            _state.y = y3;
        }

        public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            _state.sb.Append(string.Format("{0} {1} {2} {3} {4} {5} c\n", Format(x1), Format(y1), Format(x2), Format(y2), Format(x3), Format(y3)));
            _state.x = x3;
            _state.y = y3;
        }

        public void MarkKnot(int index, double theta, double x, double y, SpiroPointType type)
        {
        }
    }
}

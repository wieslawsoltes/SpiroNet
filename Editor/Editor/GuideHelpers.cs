/*
SpiroNet.Editor
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

namespace SpiroNet.Editor
{
    public static class GuideHelpers
    {
        public static double LineSegmentAngle(GuidePoint point0, GuidePoint point1)
        {
            double angle = Math.Atan2(point0.Y - point1.Y, point0.X - point1.X);
            double result = angle * 180.0 / Math.PI;
            if (result < 0.0)
                result += 360.0;
            return result;
        }

        public static double LineSegmentsAngle(GuidePoint point0, GuidePoint point1, GuidePoint point2, GuidePoint point3)
        {
            double angle1 = Math.Atan2(point0.Y - point1.Y, point0.X - point1.X);
            double angle2 = Math.Atan2(point2.Y - point3.Y, point2.X - point3.X);
            double result = (angle2 - angle1) * 180.0 / Math.PI;
            if (result < 0.0)
                result += 360.0;
            return result;
        }

        public static double Distance(GuidePoint point0, GuidePoint point1)
        {
            double dx = point0.X - point1.X;
            double dy = point0.Y - point1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static GuidePoint Middle(GuidePoint point0, GuidePoint point1)
        {
            return new GuidePoint((point0.X + point1.X) / 2.0, (point0.Y + point1.Y) / 2.0);
        }

        public static GuidePoint NearestPointOnLine(GuidePoint a, GuidePoint b, GuidePoint p)
        {
            double ax = p.X - a.X;
            double ay = p.Y - a.Y;
            double bx = b.X - a.X;
            double by = b.Y - a.Y;
            double t = (ax * bx + ay * by) / (bx * bx + by * by);
            if (t < 0.0)
            {
                return new GuidePoint(a.X, a.Y);
            }
            else if (t > 1.0)
            {
                return new GuidePoint(b.X, b.Y);
            }
            return new GuidePoint(bx * t + a.X, by * t + a.Y);
        }

        public static bool PointIsOnLineSegment(GuidePoint point0, GuidePoint point1, GuidePoint point)
        {
            var minX = Math.Min(point0.X, point1.X);
            var maxX = Math.Max(point0.X, point1.X);
            var minY = Math.Min(point0.Y, point1.Y);
            var maxY = Math.Max(point0.Y, point1.Y);
            return minX <= point.X && point.X <= maxX && minY <= point.Y && point.Y <= maxY;
        }

        public static bool LineLineIntersection(GuidePoint point0, GuidePoint point1, GuidePoint point2, GuidePoint point3, out GuidePoint intersection)
        {
            double A1 = point1.Y - point0.Y;
            double B1 = point0.X - point1.X;
            double C1 = A1 * point0.X + B1 * point0.Y;

            double A2 = point3.Y - point2.Y;
            double B2 = point2.X - point3.X;
            double C2 = A2 * point2.X + B2 * point2.Y;

            double det = A1 * B2 - A2 * B1;
            if (det != 0.0)
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;
                var point = new GuidePoint(x, y);

                if (PointIsOnLineSegment(point0, point1, point) && PointIsOnLineSegment(point2, point3, point))
                {
                    intersection = point;
                    return true;
                }
            }

            intersection = default(GuidePoint);
            return false;
        }
    }
}

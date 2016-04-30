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

namespace SpiroNet.Editor
{
    public class GuideLine
    {
        public GuidePoint Point0 { get; private set; }
        public GuidePoint Point1 { get; private set; }

        public GuideLine(GuidePoint point0, GuidePoint point1)
        {
            Point0 = point0;
            Point1 = point1;
        }
    }
}

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

namespace SpiroNet.Editor
{
    public class EditorMeasure : ObservableObject
    {
        private GuidePoint _point0;
        private GuidePoint _point1;
        private double _distance;
        private double _angle;
        private GuideSnapMode _snapResult;

        public GuidePoint Point0
        {
            get { return _point0; }
            set { Update(ref _point0, value); }
        }

        public GuidePoint Point1
        {
            get { return _point1; }
            set { Update(ref _point1, value); }
        }

        public double Distance
        {
            get { return _distance; }
            set { Update(ref _distance, value); }
        }

        public double Angle
        {
            get { return _angle; }
            set { Update(ref _angle, value); }
        }

        public GuideSnapMode SnapResult
        {
            get { return _snapResult; }
            set { Update(ref _snapResult, value); }
        }
    }
}

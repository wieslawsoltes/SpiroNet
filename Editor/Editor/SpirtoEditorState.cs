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
using System.Collections.Generic;
using System.Linq;

namespace SpiroNet.Editor
{
    public class SpirtoEditorState : ObservableObject
    {
        private SpirtoEditorMode _mode;
        private PathShape _shape;
        private double _snapX;
        private double _snapY;
        private bool _enableSnap;
        private double _hitTreshold;
        private double _hitTresholdSquared;
        private PathShape _hitShape;
        private int _hitShapePointIndex;
        private bool _isStroked;
        private bool _isFilled;
        private bool _isClosed;
        private bool _isTagged;
        private SpiroPointType _pointType;
        private bool _displayKnots;

        public SpirtoEditorState()
        {
            _mode = SpirtoEditorMode.Create;
            _shape = null;
            _snapX = 15.0;
            _snapY = 15.0;
            _enableSnap = true;
            _hitTreshold = 7;
            _hitTresholdSquared = 49;
            _hitShape = null;
            _hitShapePointIndex = -1;
            _isStroked = true;
            _isFilled = false;
            _isClosed = true;
            _isTagged = false;
            _pointType = SpiroPointType.G4;
            _displayKnots = true;
        }
        
        public SpirtoEditorMode Mode
        {
            get { return _mode; }
            set { Update(ref _mode, value); }
        }

        public PathShape Shape
        {
            get { return _shape; }
            set { Update(ref _shape, value); }
        }

        public double SnapX
        {
            get { return _snapX; }
            set { Update(ref _snapX, value); }
        }
        
        public double SnapY
        {
            get { return _snapY; }
            set { Update(ref _snapY, value); }
        }
        
        public bool EnableSnap
        {
            get { return _enableSnap; }
            set { Update(ref _enableSnap, value); }
        }

        public double HitTreshold
        {
            get { return _hitTreshold; }
            set { Update(ref _hitTreshold, value); }
        }

        public double HitTresholdSquared
        {
            get { return _hitTresholdSquared; }
            set { Update(ref _hitTresholdSquared, value); }
        }

        public PathShape HitShape
        {
            get { return _hitShape; }
            set { Update(ref _hitShape, value); }
        }

        public int HitShapePointIndex
        {
            get { return _hitShapePointIndex; }
            set { Update(ref _hitShapePointIndex, value); }
        }

        public bool IsStroked
        {
            get { return _isStroked; }
            set { Update(ref _isStroked, value); }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
        }

        public bool IsClosed
        {
            get { return _isClosed; }
            set { Update(ref _isClosed, value); }
        }

        public bool IsTagged
        {
            get { return _isTagged; }
            set { Update(ref _isTagged, value); }
        }
        
        public bool DisplayKnots
        {
            get { return _displayKnots; }
            set { Update(ref _displayKnots, value); }
        }

        public SpiroPointType PointType
        {
            get { return _pointType; }
            set { Update(ref _pointType, value); }
        }
    }
}

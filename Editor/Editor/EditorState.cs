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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpiroNet.Editor
{
    public class EditorState : ObservableObject
    {
        private EditorTool _tool;
        private EditorMode _mode;

        private bool _displayKnots;
        private bool _displayGuides;

        private double _snapX;
        private double _snapY;
        private bool _enableSnap;

        private SpiroShape _shape;
        private double _hitTreshold;
        private double _hitTresholdSquared;
        private ICollection<SpiroShape> _hitSetShapes;
        private ICollection<int> _hitSetPoints;
        private IList<SpiroShape> _hitListShapes;
        private IList<int> _hitListPoints;
        private bool _isStroked;
        private bool _isFilled;
        private bool _isClosed;
        private bool _isTagged;
        private SpiroPointType _pointType;

        private GuidePoint _guidePosition;
        private bool _isCaptured;
        private double _snapTreshold;
        private GuideSnapMode _snapMode;
        private double _snapPointRadius;
        private GuidePoint _snapPoint;
        private bool _haveSnapPoint;

        public EditorState()
        {
            _tool = EditorTool.Spiro;
            _mode = EditorMode.Create;

            _displayKnots = true;
            _displayGuides = true;

            _snapX = 15.0;
            _snapY = 15.0;
            _enableSnap = false;

            _shape = null;
            _hitTreshold = 7;
            _hitTresholdSquared = 49;
            _hitSetShapes = new HashSet<SpiroShape>();
            _hitSetPoints = new HashSet<int>();
            _hitListShapes = new ObservableCollection<SpiroShape>();
            _hitListPoints = new ObservableCollection<int>();
            _isStroked = true;
            _isFilled = false;
            _isClosed = true;
            _isTagged = false;
            _pointType = SpiroPointType.G4;

            _isCaptured = false;
            _snapTreshold = 10.0;
            _snapMode = GuideSnapMode.Point | GuideSnapMode.Middle | GuideSnapMode.Intersection | GuideSnapMode.Horizontal | GuideSnapMode.Vertical;
            _snapPointRadius = 3.5;
            _snapPoint = default(GuidePoint);
            _haveSnapPoint = false;
        }

        public EditorTool Tool
        {
            get { return _tool; }
            set { Update(ref _tool, value); }
        }

        public EditorMode Mode
        {
            get { return _mode; }
            set { Update(ref _mode, value); }
        }

        public bool DisplayKnots
        {
            get { return _displayKnots; }
            set { Update(ref _displayKnots, value); }
        }

        public bool DisplayGuides
        {
            get { return _displayGuides; }
            set { Update(ref _displayGuides, value); }
        }

        public SpiroShape Shape
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

        public ICollection<SpiroShape> HitSetShapes
        {
            get { return _hitSetShapes; }
            set { Update(ref _hitSetShapes, value); }
        }

        public ICollection<int> HitSetPoints
        {
            get { return _hitSetPoints; }
            set { Update(ref _hitSetPoints, value); }
        }

        public IList<SpiroShape> HitListShapes
        {
            get { return _hitListShapes; }
            set { Update(ref _hitListShapes, value); }
        }

        public IList<int> HitListPoints
        {
            get { return _hitListPoints; }
            set { Update(ref _hitListPoints, value); }
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

        public SpiroPointType PointType
        {
            get { return _pointType; }
            set { Update(ref _pointType, value); }
        }

        public GuidePoint GuidePosition
        {
            get { return _guidePosition; }
            set { Update(ref _guidePosition, value); }
        }

        public bool IsCaptured
        {
            get { return _isCaptured; }
            set { Update(ref _isCaptured, value); }
        }

        public double SnapTreshold
        {
            get { return _snapTreshold; }
            set { Update(ref _snapTreshold, value); }
        }

        public GuideSnapMode SnapMode
        {
            get { return _snapMode; }
            set { Update(ref _snapMode, value); }
        }

        public double SnapPointRadius
        {
            get { return _snapPointRadius; }
            set { Update(ref _snapPointRadius, value); }
        }

        public GuidePoint SnapPoint
        {
            get { return _snapPoint; }
            set { Update(ref _snapPoint, value); }
        }

        public bool HaveSnapPoint
        {
            get { return _haveSnapPoint; }
            set { Update(ref _haveSnapPoint, value); }
        }
    }
}

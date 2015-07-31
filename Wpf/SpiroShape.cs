using System;
using System.Collections.Generic;
using SpiroNet;

namespace SpiroNet.Wpf
{
    public class SpiroShape
    {
        public IList<SpiroControlPoint> Points { get; set; }
        public bool IsClosed { get; set; }
        public bool IsTagged { get; set; }
        public string Source { get; set; }
    }
}

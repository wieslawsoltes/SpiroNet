using System;
using SpiroNet;

namespace Examples
{
    class Program1
    {
        public static void Main(string[] args)
        {
            var points = new SpiroControlPoint[4];
            points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
            points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
            points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
            points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;

            var bc = new PathBezierContext();
            var success = Spiro.SpiroCPsToBezier(points, 4, true, bc);

            Console.WriteLine(bc);
            Console.WriteLine("Success: {0} ", success);
            Console.ReadKey(true);
        }
    }
}

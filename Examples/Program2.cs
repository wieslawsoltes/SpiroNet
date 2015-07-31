using System;
using SpiroNet;

namespace Examples
{
    class Program2
    {
        public static void Main(string[] args)
        {
            var points = new SpiroControlPoint[5];
            points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
            points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
            points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
            points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
            points[4].X = 0; points[4].Y = 0; points[4].Type = SpiroPointType.End;

            var bc = new PathBezierContext();
            var success = Spiro.TaggedSpiroCPsToBezier(points, bc);

            Console.WriteLine(bc);
            Console.WriteLine("Success: {0} ", success);
            Console.ReadKey(true);
        }
    }
}

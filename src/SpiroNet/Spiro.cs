/*
libspiro - conversion between spiro control points and bezier's
Copyright (C) 2007 Raph Levien
              2019 converted to C# by Wiesław Šoltés

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

namespace SpiroNet
{
    /// <summary>
    /// C# implementation of third-order polynomial spirals.
    /// Interface routines for Raph's spiro package.
    /// </summary>
    public static class Spiro
    {
        /// <summary>
        /// Convert a set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// Open contours do not need to start with '{', nor to end with '}'.
        /// 
        /// Close contours do not need to end with 'z'.
        /// 
        /// This function is kept for backwards compatibility for older programs. 
        /// Please use the function that return success/failure replies when done.	
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="n">The number of elements in the spiros array.</param>
        /// <param name="isClosed">Whether this describes a closed (True) or open (False) contour.</param>
        /// <param name="bc">A bézier results output context.</param>
        public static void SpiroCPsToBezier(SpiroControlPoint[] spiros, int n, bool isClosed, IBezierContext bc)
        {
            SpiroCPsToBezier0(spiros, n, isClosed, bc);
        }

        /// <summary>
        /// Convert a tagged set of spiro control points into a set of bézier curves.
        ///
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        ///
        /// The spiros array should indicate it's own end.
        ///
        /// Open contours must have the ty field of the first cp set to '{' and have the ty field of the last cp set to '}'.
        ///
        /// Closed contours must have an extra cp at the end whose ty is 'z' the x and y values of this extra cp are ignored.
        ///
        /// This function is kept for backwards compatibility for older programs. 
        /// Please use the functions that return success/failure replies when done.
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="bc">A bézier results output context.</param>
        public static void TaggedSpiroCPsToBezier(SpiroControlPoint[] spiros, IBezierContext bc)
        {
            TaggedSpiroCPsToBezier0(spiros, bc);
        }

        /// <summary>
        /// Convert a set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// Open contours do not need to start with '{', nor to end with '}'.
        /// 
        /// Close contours do not need to end with 'z'.
        /// 
        /// This function is kept for backwards compatibility for older programs. 
        /// Please use the function that return success/failure replies when done.	
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="n">The number of elements in the spiros array.</param>
        /// <param name="isClosed">Whether this describes a closed (True) or open (False) contour.</param>
        /// <param name="bc">A bézier results output context.</param>
        /// <returns>SpiroSegment array or null on failure.</returns>
        /// <example>
        /// var points = new SpiroControlPoint[4];
        /// points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
        /// points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
        /// points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
        /// points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
        /// var bc = new PathBezierContext();
        /// var success = Spiro.SpiroCPsToBezier0(points, 4, true, bc);
        /// Console.WriteLine(bc);
        /// Console.WriteLine("Success: {0} ", success);
        /// </example>
        public static SpiroSegment[]? SpiroCPsToSegments(SpiroControlPoint[] spiros, int n, bool isClosed)
        {
            if (n <= 0)
                return null;
            if (isClosed)
                return SpiroImpl.run_spiro(spiros, n);
            else
            {
                SpiroPointType oldty_start = spiros[0].Type;
                SpiroPointType oldty_end = spiros[n - 1].Type;
                spiros[0].Type = SpiroPointType.OpenContour;
                spiros[n - 1].Type = SpiroPointType.EndOpenContour;
                SpiroSegment[]? s;
                s = SpiroImpl.run_spiro(spiros, n);
                spiros[n - 1].Type = oldty_end;
                spiros[0].Type = oldty_start;
                return s;
            }
        }

        /// <summary>
        /// Convert a set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// Open contours do not need to start with '{', nor to end with '}'.
        /// 
        /// Close contours do not need to end with 'z'.
        /// 
        /// This function is kept for backwards compatibility for older programs. 
        /// Please use the function that return success/failure replies when done.	
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="n">The number of elements in the spiros array.</param>
        /// <param name="isClosed">Whether this describes a closed (True) or open (False) contour.</param>
        /// <param name="bc">A bézier results output context.</param>
        /// <returns>An boolean success flag. True = completed task and have valid bézier results, or False = unable to complete task, bézier results are invalid.</returns>
        /// <example>
        /// var points = new SpiroControlPoint[4];
        /// points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
        /// points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
        /// points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
        /// points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
        /// var bc = new PathBezierContext();
        /// var success = Spiro.SpiroCPsToBezier0(points, 4, true, bc);
        /// Console.WriteLine(bc);
        /// Console.WriteLine("Success: {0} ", success);
        /// </example>
        public static bool SpiroCPsToBezier0(SpiroControlPoint[] spiros, int n, bool isClosed, IBezierContext bc)
        {
            SpiroSegment[]? s = SpiroCPsToSegments(spiros, n, isClosed);
            if (s != null)
            {
                SpiroImpl.spiro_to_bpath(s, n, bc);
                return true; // success
            }

            return false; // spiro did not converge or encountered non-finite values
        }

        /// <summary>
        /// Convert a tagged set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// The spiros array should indicate it's own end.
        /// 
        /// Open contours must have the ty field of the first cp set to '{' and have the ty field of the last cp set to '}'.
        /// 
        /// Closed contours must have an extra cp at the end whose ty is 'z' the x and y values of this extra cp are ignored.
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="bc">A bézier results output context.</param>
        /// <returns>SpiroSegment array or null on failure.</returns>
        /// <example>
        /// var points = new SpiroControlPoint[5];
        /// points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
        /// points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
        /// points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
        /// points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
        /// points[4].X = 0; points[4].Y = 0; points[4].Type = SpiroPointType.End;
        /// var bc = new PathBezierContext();
        /// var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
        /// Console.WriteLine(bc);
        /// Console.WriteLine("Success: {0} ", success);
        /// 
        /// var points = new SpiroControlPoint[5];
        /// points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
        /// points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
        /// points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
        /// points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
        /// points[4].X = 0; points[4].Y = 0; points[4].Type = SpiroPointType.End;
        /// var bc = new PathBezierContext();
        /// var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
        /// Console.WriteLine(bc);
        /// Console.WriteLine("Success: {0} ", success);
        /// </example>
        public static SpiroSegment[]? TaggedSpiroCPsToSegments(SpiroControlPoint[] spiros)
        {
            if(spiros.Length == 0) return null;
            int n = 0;
            while (true)
            {
                if (spiros[n].Type == SpiroPointType.End || spiros[n].Type == SpiroPointType.EndOpenContour)
                    break;

                // invalid input
                if (n >= spiros.Length)
                    return null;

                ++n;
            }

            if (spiros[n].Type == SpiroPointType.EndOpenContour)
                ++n;

            if (n <= 0)
                return null; // invalid input

            return SpiroImpl.run_spiro(spiros, n);
        }

        /// <summary>
        /// Convert a tagged set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// The spiros array should indicate it's own end.
        /// 
        /// Open contours must have the ty field of the first cp set to '{' and have the ty field of the last cp set to '}'.
        /// 
        /// Closed contours must have an extra cp at the end whose ty is 'z' the x and y values of this extra cp are ignored.
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="bc">A bézier results output context.</param>
        /// <returns>An boolean success flag.True = completed task and have valid bézier results, or False = unable to complete task, bézier results are invalid.</returns>
        /// <example>
        /// var points = new SpiroControlPoint[5];
        /// points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
        /// points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
        /// points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
        /// points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
        /// points[4].X = 0; points[4].Y = 0; points[4].Type = SpiroPointType.End;
        /// var bc = new PathBezierContext();
        /// var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
        /// Console.WriteLine(bc);
        /// Console.WriteLine("Success: {0} ", success);
        /// 
        /// var points = new SpiroControlPoint[5];
        /// points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
        /// points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
        /// points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
        /// points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
        /// points[4].X = 0; points[4].Y = 0; points[4].Type = SpiroPointType.End;
        /// var bc = new PathBezierContext();
        /// var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);
        /// Console.WriteLine(bc);
        /// Console.WriteLine("Success: {0} ", success);
        /// </example>
        public static bool TaggedSpiroCPsToBezier0(SpiroControlPoint[] spiros, IBezierContext bc)
        {
            var s = TaggedSpiroCPsToSegments(spiros);
            if (s != null)
            {
                SpiroImpl.spiro_to_bpath(s, s.Length-1, bc);
                return true; // success
            }

            return false; // spiro did not converge or encountered non-finite values
        }

        /// <summary>
        /// Convert a set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// Open contours do not need to start with '{', nor to end with '}'.
        /// 
        /// Close contours do not need to end with 'z'.
        /// 
        /// If you can't use SpiroCPsToBezier0() this function is enhanced version of the original function, 
        /// where spiro success/failure replies are passd back through done output parameter. 
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="n">The number of elements in the spiros array.</param>
        /// <param name="isClosed">Whether this describes a closed (True) or open (False) contour.</param>
        /// <param name="bc">A bézier results output context.</param>
        /// <param name="done">An boolean success flag. True = completed task and have valid bézier results, or False = unable to complete task, bézier results are invalid.</param>
        public static void SpiroCPsToBezier1(SpiroControlPoint[] spiros, int n, bool isClosed, IBezierContext bc, out bool done)
        {
            done = SpiroCPsToBezier0(spiros, n, isClosed, bc);
        }

        /// <summary>
        /// Convert a tagged set of spiro control points into a set of bézier curves.
        /// 
        /// As it does so it will call the appropriate routine in your bézier context with this information 
        /// – this should allow you to create your own internal representation of those curves.
        /// 
        /// The spiros array should indicate it's own end.
        /// 
        /// Open contours must have the ty field of the first cp set to '{' and have the ty field of the last cp set to '}'.
        /// 
        /// Closed contours must have an extra cp at the end whose ty is 'z' the x and y values of this extra cp are ignored.
        /// 
        /// If you can't use TaggedSpiroCPsToBezier0() this function is enhanced version of the original function, 
        /// where spiro success/failure replies are passd back through done output parameter. 
        /// </summary>
        /// <param name="spiros">An array of input spiros.</param>
        /// <param name="bc">A bézier results output context.</param>
        /// <param name="done">An boolean success flag.True = completed task and have valid bézier results, or False = unable to complete task, bézier results are invalid.</param>
        public static void TaggedSpiroCPsToBezier1(SpiroControlPoint[] spiros, IBezierContext bc, out bool done)
        {
            done = TaggedSpiroCPsToBezier0(spiros, bc);
        }
    }
}

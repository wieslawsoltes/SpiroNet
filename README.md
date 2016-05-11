# SpiroNet

[![Build status](https://ci.appveyor.com/api/projects/status/vq16ocwrjj95ak6t?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/spironet)
[![Gitter](https://badges.gitter.im/wieslawsoltes/SpiroNet.svg)](https://gitter.im/wieslawsoltes/SpiroNet?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

The .NET C# port of [libspiro](https://github.com/fontforge/libspiro) - conversion between spiro control points and bezier's

## Introduction

For libspiro introduction please see [libspiro project page](https://github.com/fontforge/libspiro). There is also GUI version using libspiro written in C#/WPF for Windows.

## Usage

Provided examples create  geometric paths as output using [Path Markup Syntax](https://msdn.microsoft.com/en-us/library/cc189041(v=vs.95).aspx) for WPF/Silverlight and [Path Data](http://www.w3.org/TR/SVG/paths.html#PathData) for SVG.

```C#
var points = new SpiroControlPoint[4];
points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;

var bc = new PathBezierContext();
var success = Spiro.SpiroCPsToBezier0(points, 4, true, bc);

Console.WriteLine(bc);
Console.WriteLine("Success: {0} ", success);
```

```C#
var points = new SpiroControlPoint[5];
points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.G4;
points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.G4;
points[4].X = 0; points[4].Y = 0; points[4].Type = SpiroPointType.End;

var bc = new PathBezierContext();
var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);

Console.WriteLine(bc);
Console.WriteLine("Success: {0} ", success);
```

```C#
var points = new SpiroControlPoint[4];
points[0].X = -100; points[0].Y = 0; points[0].Type = SpiroPointType.OpenContour;
points[1].X = 0; points[1].Y = 100; points[1].Type = SpiroPointType.G4;
points[2].X = 100; points[2].Y = 0; points[2].Type = SpiroPointType.G4;
points[3].X = 0; points[3].Y = -100; points[3].Type = SpiroPointType.EndOpenContour;

var bc = new PathBezierContext();
var success = Spiro.TaggedSpiroCPsToBezier0(points, bc);

Console.WriteLine(bc);
Console.WriteLine("Success: {0} ", success);
```

# SpiroNet

[![Gitter](https://badges.gitter.im/wieslawsoltes/SpiroNet.svg)](https://gitter.im/wieslawsoltes/SpiroNet?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

[![Build status](https://dev.azure.com/wieslawsoltes/SpiroNet/_apis/build/status/SpiroNet)](https://dev.azure.com/wieslawsoltes/SpiroNet/_build/latest?definitionId=-1)

[![NuGet](https://img.shields.io/nuget/v/SpiroNet.svg)](https://www.nuget.org/packages/SpiroNet)
[![MyGet](https://img.shields.io/myget/spironet-nightly/vpre/SpiroNet.svg?label=myget)](https://www.myget.org/gallery/spironet-nightly) 

The .NET C# port of [libspiro](https://github.com/fontforge/libspiro) - conversion between spiro control points and bezier's

## Introduction

For libspiro introduction please see [libspiro project page](https://github.com/fontforge/libspiro) and [drawing with spiro](http://designwithfontforge.com/en-US/Drawing_With_Spiro.html). There is also GUI version using libspiro written in C#/WPF for Windows.

## NuGet

SpiroNet is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/SpiroNet/) or by using nightly build feed:
* Add `https://www.myget.org/F/spironet-nightly/api/v2` to your package sources
* Update your package using `SpiroNet` feed

You can install the package like this:

`Install-Package SpiroNet -Pre`

### Available Packages

* [SpiroNet](https://www.nuget.org/packages/SpiroNet) - Core libspiro library.
* [SpiroNet.Editor](https://www.nuget.org/packages/SpiroNet.Editor) - Core spiro shape editor.
* [SpiroNet.Json](https://www.nuget.org/packages/SpiroNet.Json) - Json support for spiro shape editor.
* [SpiroNet.ViewModels](https://www.nuget.org/packages/SpiroNet.ViewModels) - View models for spiro shape editor controls.
* [SpiroNet.Editor.Avalonia](https://www.nuget.org/packages/SpiroNet.Editor.Avalonia) - Avalonia editor view for spiro shape editor.

### Package Dependencies

* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
* [Avalonia](https://www.nuget.org/packages/Avalonia/)

Dependencies are required only for specific editor packages.

### Package Sources

* https://api.nuget.org/v3/index.json

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/SpiroNet)

## Usage

Provided examples create geometric paths as output using [Path Markup Syntax](https://msdn.microsoft.com/en-us/library/cc189041(v=vs.95).aspx) for WPF/Silverlight and [Path Data](http://www.w3.org/TR/SVG/paths.html#PathData) for SVG.

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

## License

SpiroNet is licensed under the [GPL-3.0 license](COPYING).

Original license and patent grant is included in [README by Raph Levien](README-RaphLevien) for ppedit.

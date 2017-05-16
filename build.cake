///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Polly&version=5.1.0"
#addin "nuget:?package=NuGet.Core&version=2.14.0"

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console&version=2.2.0"

///////////////////////////////////////////////////////////////////////////////
// USINGS
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Polly;
using NuGet;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var platform = Argument("platform", "Any CPU");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION
///////////////////////////////////////////////////////////////////////////////

var MainRepo = "wieslawsoltes/SpiroNet";
var MasterBranch = "master";
var AssemblyInfoPath = File("./src/Shared/SharedAssemblyInfo.cs");
var ReleasePlatform = "Any CPU";
var ReleaseConfiguration = "Release";
var MSBuildSolution = "./SpiroNet.sln";

///////////////////////////////////////////////////////////////////////////////
// PARAMETERS
///////////////////////////////////////////////////////////////////////////////

var isPlatformAnyCPU = StringComparer.OrdinalIgnoreCase.Equals(platform, "Any CPU");
var isPlatformX86 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x86");
var isPlatformX64 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x64");
var isLocalBuild = BuildSystem.IsLocalBuild;
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();
var isRunningOnAppVeyor = BuildSystem.AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
var isMainRepo = StringComparer.OrdinalIgnoreCase.Equals(MainRepo, BuildSystem.AppVeyor.Environment.Repository.Name);
var isMasterBranch = StringComparer.OrdinalIgnoreCase.Equals(MasterBranch, BuildSystem.AppVeyor.Environment.Repository.Branch);
var isTagged = BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag 
               && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name);
var isReleasable = StringComparer.OrdinalIgnoreCase.Equals(ReleasePlatform, platform) 
                   && StringComparer.OrdinalIgnoreCase.Equals(ReleaseConfiguration, configuration);
var isMyGetRelease = !isTagged && isReleasable;
var isNuGetRelease = isTagged && isReleasable;

///////////////////////////////////////////////////////////////////////////////
// VERSION
///////////////////////////////////////////////////////////////////////////////

var version = ParseAssemblyInfo(AssemblyInfoPath).AssemblyVersion;

if (isRunningOnAppVeyor)
{
    if (isTagged)
    {
        // Use Tag Name as version
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    }
    else
    {
        // Use AssemblyVersion with Build as version
        version += "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + "-alpha";
    }
}

///////////////////////////////////////////////////////////////////////////////
// DIRECTORIES
///////////////////////////////////////////////////////////////////////////////

var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");
var nugetRoot = artifactsDir.Combine("nuget");
var zipRoot = artifactsDir.Combine("zip");
var dirSuffix = isPlatformAnyCPU ? configuration : platform + "/" + configuration;
var buildDirs = 
    GetDirectories("./src/**/bin/" + dirSuffix) + 
    GetDirectories("./src/**/obj/" + dirSuffix) + 
    GetDirectories("./samples/**/bin/" + dirSuffix) + 
    GetDirectories("./samples/**/obj/" + dirSuffix) +
    GetDirectories("./tests/**/bin/" + dirSuffix) + 
    GetDirectories("./tests/**/obj/" + dirSuffix);

var fileZipSuffix = isPlatformAnyCPU ? configuration + "-" + version + ".zip" : platform + "-" + configuration + "-" + version + ".zip";

var zipSourceAvaloniaDirs = (DirectoryPath)Directory("./samples/SpiroNet.Avalonia/bin/" + dirSuffix);
var zipSourceWpfDirs = (DirectoryPath)Directory("./samples/SpiroNet.Wpf/bin/" + dirSuffix);

var zipTargetAvaloniaDirs = zipRoot.CombineWithFilePath("SpiroNet.Avalonia-" + fileZipSuffix);
var zipTargetWpfDirs = zipRoot.CombineWithFilePath("SpiroNet.Wpf-" + fileZipSuffix);

///////////////////////////////////////////////////////////////////////////////
// NUGET NUSPECS
///////////////////////////////////////////////////////////////////////////////

// Key: Package Id
// Value is Tuple where Item1: Package Version, Item2: The *.csproj/*.props file path.
var packageVersions = new Dictionary<string, IList<Tuple<string,string>>>();

System.IO.Directory.EnumerateFiles(((DirectoryPath)Directory("./src")).FullPath, "*.csproj", SearchOption.AllDirectories)
    .ToList()
    .ForEach(fileName => {
    var xdoc = XDocument.Load(fileName);
    foreach (var reference in xdoc.Descendants().Where(x => x.Name.LocalName == "PackageReference"))
    {
        var name = reference.Attribute("Include").Value;
        var versionAttribute = reference.Attribute("Version");
        var packageVersion = versionAttribute != null 
            ? versionAttribute.Value 
            : reference.Elements().First(x=>x.Name.LocalName == "Version").Value;
        IList<Tuple<string, string>> versions;
        packageVersions.TryGetValue(name, out versions);
        if (versions == null)
        {
            versions = new List<Tuple<string, string>>();
            packageVersions[name] = versions;
        }
        versions.Add(Tuple.Create(packageVersion, fileName));
    }
});

Information("Checking installed NuGet package dependencies versions:");

packageVersions.ToList().ForEach(package =>
{
    var packageVersion = package.Value.First().Item1;
    bool isValidVersion = package.Value.All(x => x.Item1 == packageVersion);
    if (!isValidVersion)
    {
        Information("Error: package {0} has multiple versions installed:", package.Key);
        foreach (var v in package.Value)
        {
            Information("{0}, file: {1}", v.Item1, v.Item2);
        }
        throw new Exception("Detected multiple NuGet package version installed for different projects.");
    }
});

Information("Setting NuGet package dependencies versions:");

var NewtonsoftJsonVersion = packageVersions["Newtonsoft.Json"].FirstOrDefault().Item1;
var AvaloniaVersion = packageVersions["Avalonia"].FirstOrDefault().Item1;

Information("Package: NewtonsoftJsonVersion, version: {0}", NewtonsoftJsonVersion);
Information("Package: Avalonia, version: {0}", AvaloniaVersion);

var nuspecNuGetSpiroNet = new NuGetPackSettings()
{
    Id = "SpiroNet",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("https://opensource.org/licenses/GPL-3.0"),
    ProjectUrl = new Uri("https://github.com/wieslawsoltes/SpiroNet/"),
    RequireLicenseAcceptance = false,
    Symbols = false,
    NoPackageAnalysis = true,
    Description = "The .NET C# port of libspiro - conversion between spiro control points and bezier's.",
    Copyright = "Copyright 2017",
    Tags = new [] { "Spiro", "LibSpiro", "SpiroNet", "Graphics", "Bezier", "Spline", "Splines", "Curve", "Path", "Geometry", "Editor", "Design" },
    Files = new []
    {
        // netstandard1.3
        new NuSpecContent { Source = "src/SpiroNet/bin/" + dirSuffix + "/netstandard1.3/" + "SpiroNet.dll", Target = "lib/netstandard1.3" },
        new NuSpecContent { Source = "src/SpiroNet/bin/" + dirSuffix + "/netstandard1.3/" + "SpiroNet.xml", Target = "lib/netstandard1.3" },
        // net45
        new NuSpecContent { Source = "src/SpiroNet/bin/" + dirSuffix + "/net45/" + "SpiroNet.dll", Target = "lib/net45" },
        new NuSpecContent { Source = "src/SpiroNet/bin/" + dirSuffix + "/net45/" + "SpiroNet.xml", Target = "lib/net45" }
    },
    BasePath = Directory("./"),
    OutputDirectory = nugetRoot
};

var nuspecNuGetSpiroNetEditor = new NuGetPackSettings()
{
    Id = "SpiroNet.Editor",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("https://opensource.org/licenses/GPL-3.0"),
    ProjectUrl = new Uri("https://github.com/wieslawsoltes/SpiroNet/"),
    RequireLicenseAcceptance = false,
    Symbols = false,
    NoPackageAnalysis = true,
    Description = "The .NET C# port of libspiro - conversion between spiro control points and bezier's.",
    Copyright = "Copyright 2017",
    Tags = new [] { "Spiro", "LibSpiro", "SpiroNet", "Graphics", "Bezier", "Spline", "Splines", "Curve", "Path", "Geometry", "Editor", "Design" },
    Dependencies = new []
    {
        new NuSpecDependency { Id = "SpiroNet", Version = version }
    },
    Files = new []
    {
        // netstandard1.3
        new NuSpecContent { Source = "src/SpiroNet.Editor/bin/" + dirSuffix + "/netstandard1.3/" + "SpiroNet.Editor.dll", Target = "lib/netstandard1.3" },
        // net45
        new NuSpecContent { Source = "src/SpiroNet.Editor/bin/" + dirSuffix + "/net45/" + "SpiroNet.Editor.dll", Target = "lib/net45" }
    },
    BasePath = Directory("./"),
    OutputDirectory = nugetRoot
};

var nuspecNuGetSpiroNetJson = new NuGetPackSettings()
{
    Id = "SpiroNet.Json",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("https://opensource.org/licenses/GPL-3.0"),
    ProjectUrl = new Uri("https://github.com/wieslawsoltes/SpiroNet/"),
    RequireLicenseAcceptance = false,
    Symbols = false,
    NoPackageAnalysis = true,
    Description = "The .NET C# port of libspiro - conversion between spiro control points and bezier's.",
    Copyright = "Copyright 2017",
    Tags = new [] { "Spiro", "LibSpiro", "SpiroNet", "Graphics", "Bezier", "Spline", "Splines", "Curve", "Path", "Geometry", "Editor", "Design" },
    Dependencies = new []
    {
        new NuSpecDependency { Id = "SpiroNet", Version = version },
        new NuSpecDependency { Id = "Newtonsoft.Json", Version = NewtonsoftJsonVersion }
    },
    Files = new []
    {
        // netstandard1.3
        new NuSpecContent { Source = "src/SpiroNet.Json/bin/" + dirSuffix + "/netstandard1.3/" + "SpiroNet.Json.dll", Target = "lib/netstandard1.3" },
        // net45
        new NuSpecContent { Source = "src/SpiroNet.Json/bin/" + dirSuffix + "/net45/" + "SpiroNet.Json.dll", Target = "lib/net45" }
    },
    BasePath = Directory("./"),
    OutputDirectory = nugetRoot
};

var nuspecNuGetSpiroNetViewModels = new NuGetPackSettings()
{
    Id = "SpiroNet.ViewModels",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("https://opensource.org/licenses/GPL-3.0"),
    ProjectUrl = new Uri("https://github.com/wieslawsoltes/SpiroNet/"),
    RequireLicenseAcceptance = false,
    Symbols = false,
    NoPackageAnalysis = true,
    Description = "The .NET C# port of libspiro - conversion between spiro control points and bezier's.",
    Copyright = "Copyright 2017",
    Tags = new [] { "Spiro", "LibSpiro", "SpiroNet", "Graphics", "Bezier", "Spline", "Splines", "Curve", "Path", "Geometry", "Editor", "Design" },
    Dependencies = new []
    {
        new NuSpecDependency { Id = "SpiroNet", Version = version },
        new NuSpecDependency { Id = "SpiroNet.Editor", Version = version },
        new NuSpecDependency { Id = "SpiroNet.Json", Version = version }
    },
    Files = new []
    {
        // netstandard1.3
        new NuSpecContent { Source = "src/SpiroNet.ViewModels/bin/" + dirSuffix + "/netstandard1.3/" + "SpiroNet.ViewModels.dll", Target = "lib/netstandard1.3" },
        // net45
        new NuSpecContent { Source = "src/SpiroNet.ViewModels/bin/" + dirSuffix + "/net45/" + "SpiroNet.ViewModels.dll", Target = "lib/net45" }
    },
    BasePath = Directory("./"),
    OutputDirectory = nugetRoot
};

var nuspecNuGetSpiroNetEditorWpf = new NuGetPackSettings()
{
    Id = "SpiroNet.Editor.Wpf",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("https://opensource.org/licenses/GPL-3.0"),
    ProjectUrl = new Uri("https://github.com/wieslawsoltes/SpiroNet/"),
    RequireLicenseAcceptance = false,
    Symbols = false,
    NoPackageAnalysis = true,
    Description = "The .NET C# port of libspiro - conversion between spiro control points and bezier's.",
    Copyright = "Copyright 2017",
    Tags = new [] { "Spiro", "LibSpiro", "SpiroNet", "Graphics", "Bezier", "Spline", "Splines", "Curve", "Path", "Geometry", "Editor", "Design", "WPF" },
    Dependencies = new []
    {
        new NuSpecDependency { Id = "SpiroNet.ViewModels", Version = version }
    },
    Files = new []
    {
        new NuSpecContent { Source = "src/SpiroNet.Editor.Wpf/bin/" + dirSuffix + "/SpiroNet.Editor.Wpf.dll", Target = "lib/net45" }
    },
    BasePath = Directory("./"),
    OutputDirectory = nugetRoot
};

var nuspecNuGetSpiroNetEditorAvalonia = new NuGetPackSettings()
{
    Id = "SpiroNet.Editor.Avalonia",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("https://opensource.org/licenses/GPL-3.0"),
    ProjectUrl = new Uri("https://github.com/wieslawsoltes/SpiroNet/"),
    RequireLicenseAcceptance = false,
    Symbols = false,
    NoPackageAnalysis = true,
    Description = "The .NET C# port of libspiro - conversion between spiro control points and bezier's.",
    Copyright = "Copyright 2017",
    Tags = new [] { "Spiro", "LibSpiro", "SpiroNet", "Graphics", "Bezier", "Spline", "Splines", "Curve", "Path", "Geometry", "Editor", "Design", "Avalonia" },
    Dependencies = new []
    {
        new NuSpecDependency { Id = "SpiroNet.ViewModels", Version = version },
        new NuSpecDependency { Id = "Avalonia", Version = AvaloniaVersion }
    },
    Files = new []
    {
        // netstandard1.3
        new NuSpecContent { Source = "src/SpiroNet.Editor.Avalonia/bin/" + dirSuffix + "/netstandard1.3/" + "SpiroNet.Editor.Avalonia.dll", Target = "lib/netstandard1.3" },
        // net45
        new NuSpecContent { Source = "src/SpiroNet.Editor.Avalonia/bin/" + dirSuffix + "/net45/" + "SpiroNet.Editor.Avalonia.dll", Target = "lib/net45" }
    },
    BasePath = Directory("./"),
    OutputDirectory = nugetRoot
};

var nuspecNuGetSettings = new List<NuGetPackSettings>();

nuspecNuGetSettings.Add(nuspecNuGetSpiroNet);
nuspecNuGetSettings.Add(nuspecNuGetSpiroNetEditor);
nuspecNuGetSettings.Add(nuspecNuGetSpiroNetJson);
nuspecNuGetSettings.Add(nuspecNuGetSpiroNetViewModels);
nuspecNuGetSettings.Add(nuspecNuGetSpiroNetEditorWpf);
nuspecNuGetSettings.Add(nuspecNuGetSpiroNetEditorAvalonia);

var nugetPackages = nuspecNuGetSettings.Select(nuspec => {
    return nuspec.OutputDirectory.CombineWithFilePath(string.Concat(nuspec.Id, ".", nuspec.Version, ".nupkg"));
}).ToArray();

///////////////////////////////////////////////////////////////////////////////
// INFORMATION
///////////////////////////////////////////////////////////////////////////////

Information("Building version {0} of SpiroNet ({1}, {2}, {3}) using version {4} of Cake.", 
    version,
    platform,
    configuration,
    target,
    typeof(ICakeContext).Assembly.GetName().Version.ToString());

if (isRunningOnAppVeyor)
{
    Information("Repository Name: " + BuildSystem.AppVeyor.Environment.Repository.Name);
    Information("Repository Branch: " + BuildSystem.AppVeyor.Environment.Repository.Branch);
}

Information("Target: " + target);
Information("Platform: " + platform);
Information("Configuration: " + configuration);
Information("IsLocalBuild: " + isLocalBuild);
Information("IsRunningOnUnix: " + isRunningOnUnix);
Information("IsRunningOnWindows: " + isRunningOnWindows);
Information("IsRunningOnAppVeyor: " + isRunningOnAppVeyor);
Information("IsPullRequest: " + isPullRequest);
Information("IsMainRepo: " + isMainRepo);
Information("IsMasterBranch: " + isMasterBranch);
Information("IsTagged: " + isTagged);
Information("IsReleasable: " + isReleasable);
Information("IsMyGetRelease: " + isMyGetRelease);
Information("IsNuGetRelease: " + isNuGetRelease);

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
    CleanDirectory(artifactsDir);
    CleanDirectory(testResultsDir);
    CleanDirectory(nugetRoot);
    CleanDirectory(zipRoot);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var maxRetryCount = 5;
    var toolTimeout = 1d;
    Policy
        .Handle<Exception>()
        .Retry(maxRetryCount, (exception, retryCount, context) => {
            if (retryCount == maxRetryCount)
            {
                throw exception;
            }
            else
            {
                Verbose("{0}", exception);
                toolTimeout+=0.5;
            }})
        .Execute(()=> {
            NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
                ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
            });
        });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild(MSBuildSolution, settings => {
        settings.SetConfiguration(configuration);
        settings.WithProperty("Platform", "\"" + platform + "\"");
        settings.SetVerbosity(Verbosity.Minimal);
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    string pattern = "./tests/**/bin/" + dirSuffix + "/*.UnitTests.dll";
    string toolPath = (isPlatformAnyCPU || isPlatformX86) ? 
        "./tools/xunit.runner.console/tools/xunit.console.x86.exe" :
        "./tools/xunit.runner.console/tools/xunit.console.exe";

    XUnit2(pattern, new XUnit2Settings { 
        ToolPath = toolPath,
        OutputDirectory = testResultsDir,
        XmlReportV1 = true,
        NoAppDomain = true
    });
});

Task("Zip-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    Zip(zipSourceAvaloniaDirs, 
        zipTargetAvaloniaDirs, 
        GetFiles(zipSourceAvaloniaDirs.FullPath + "/*.dll") + 
        GetFiles(zipSourceAvaloniaDirs.FullPath + "/*.exe") + 
        GetFiles(zipSourceAvaloniaDirs.FullPath + "/*.config") + 
        GetFiles(zipSourceAvaloniaDirs.FullPath + "/*.so") + 
        GetFiles(zipSourceAvaloniaDirs.FullPath + "/*.dylib"));
    
    if (isRunningOnWindows)
    {
        Zip(zipSourceWpfDirs, 
            zipTargetWpfDirs, 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.dll") + 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.config") + 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.exe"));
    }
});

Task("Create-NuGet-Packages")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    foreach(var nuspec in nuspecNuGetSettings)
    {
        NuGetPack(nuspec);
    }
});

Task("Publish-MyGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMainRepo)
    .WithCriteria(() => isMasterBranch)
    .WithCriteria(() => isMyGetRelease)
    .Does(() =>
{
    var apiKey = EnvironmentVariable("MYGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) 
    {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    var apiUrl = EnvironmentVariable("MYGET_API_URL");
    if(string.IsNullOrEmpty(apiUrl)) 
    {
        throw new InvalidOperationException("Could not resolve MyGet API url.");
    }

    foreach(var nupkg in nugetPackages)
    {
        NuGetPush(nupkg, new NuGetPushSettings {
            Source = apiUrl,
            ApiKey = apiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-MyGet Task failed, but continuing with next Task...");
});

Task("Publish-NuGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMainRepo)
    .WithCriteria(() => isMasterBranch)
    .WithCriteria(() => isNuGetRelease)
    .Does(() =>
{
    var apiKey = EnvironmentVariable("NUGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) 
    {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    var apiUrl = EnvironmentVariable("NUGET_API_URL");
    if(string.IsNullOrEmpty(apiUrl)) 
    {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    foreach(var nupkg in nugetPackages)
    {
        NuGetPush(nupkg, new NuGetPushSettings {
            ApiKey = apiKey,
            Source = apiUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Package")
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Create-NuGet-Packages");

Task("Default")
  .IsDependentOn("Package");

Task("AppVeyor")
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Publish-MyGet")
  .IsDependentOn("Publish-NuGet");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);

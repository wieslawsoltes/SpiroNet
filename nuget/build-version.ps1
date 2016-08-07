$ErrorActionPreference = "Stop"

. ".\include.ps1"

foreach($pkg in $Packages) 
{
    rm -Force -Recurse .\$pkg -ErrorAction SilentlyContinue
}

rm -Force -Recurse *.nupkg -ErrorAction SilentlyContinue

Copy-Item template SpiroNet -Recurse

sv sn "SpiroNet\lib\portable-windows8+net45"
mkdir $sn -ErrorAction SilentlyContinue
Copy-Item ..\src\SpiroNet\bin\Release\SpiroNet.dll $sn

sv sne "SpiroNet.Editor\lib\portable-windows8+net45"
mkdir $sne -ErrorAction SilentlyContinue
Copy-Item ..\src\SpiroNet.Editor\bin\Release\SpiroNet.Editor.dll $sne

sv snj "SpiroNet.Json\lib\portable-windows8+net45"
mkdir $snj -ErrorAction SilentlyContinue
Copy-Item ..\src\SpiroNet.Json\bin\Release\SpiroNet.Json.dll $snj

sv snvm "SpiroNet.ViewModels\lib\net45"
mkdir $snvm -ErrorAction SilentlyContinue
Copy-Item ..\src\SpiroNet.ViewModels\bin\Release\SpiroNet.ViewModels.dll $snvm

sv snea "SpiroNet.Editor.Avalonia\lib\net45"
mkdir $snea -ErrorAction SilentlyContinue
Copy-Item ..\src\SpiroNet.Editor.Avalonia\bin\Release\SpiroNet.Editor.Avalonia.dll $snea

sv snew "SpiroNet.Editor.Wpf\lib\net45"
mkdir $snew -ErrorAction SilentlyContinue
Copy-Item ..\src\SpiroNet.Editor.Wpf\bin\Release\SpiroNet.Editor.Wpf.dll $snew

foreach($pkg in $Packages)
{
    (gc SpiroNet\$pkg.nuspec).replace('#VERSION#', $args[0]) | sc $pkg\$pkg.nuspec
}

foreach($pkg in $Packages)
{
    nuget.exe pack $pkg\$pkg.nuspec
}

foreach($pkg in $Packages)
{
    rm -Force -Recurse .\$pkg
}
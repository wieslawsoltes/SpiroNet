@echo off
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319
csc.exe /r:"..\bin\Release\SpiroNet.dll" Program1.cs
csc.exe /r:"..\bin\Release\SpiroNet.dll" Program2.cs
csc.exe /r:"..\bin\Release\SpiroNet.dll" Program3.cs
copy "..\bin\Release\SpiroNet.dll" SpiroNet.dll
pause
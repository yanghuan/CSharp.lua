dotnet publish ../../ --configuration Release

set lib=../../CSharp.lua.Launcher/bin/Release/netcoreapp2.0/publish/
set dir=../../CSharp.lua.Launcher/bin/Debug/netcoreapp2.0/
set obj="../../CSharp.lua/obj"
if exist %obj% rd /s /q %obj%
dotnet "%dir%CSharp.lua.Launcher.dll" -s ../../CSharp.lua/ -d selfout -l %lib%Microsoft.CodeAnalysis.CSharp.dll;%lib%Microsoft.CodeAnalysis.dll -metadata

pause
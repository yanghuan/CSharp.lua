set dir=../../CSharp.lua.Launcher/bin/Debug/netcoreapp2.0/
set lib=lib/
set obj="../../CSharp.lua/obj"
if exist %obj% rd /s /q %obj%
dotnet "%dir%CSharp.lua.Launcher.dll" -s ../../CSharp.lua/ -d selfout -l %lib%Microsoft.CodeAnalysis.CSharp.dll;%lib%Microsoft.CodeAnalysis.dll
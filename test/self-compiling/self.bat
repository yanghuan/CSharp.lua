dotnet publish ../../ --configuration Release --output bin/Release/PublishOutput
set dir=../../CSharp.lua.Launcher/bin/Release/PublishOutput/
set obj="../../CSharp.lua/obj"
if exist %obj% rd /s /q %obj%
dotnet "%dir%CSharp.lua.Launcher.dll" -s ../../CSharp.lua/ -d selfout -l %dir%Microsoft.CodeAnalysis.CSharp.dll;%dir%Microsoft.CodeAnalysis.dll -metadata
pause
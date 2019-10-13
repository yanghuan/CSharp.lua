set dir=bin/Release/PublishOutput/
dotnet publish ../../ --configuration Release --output %dir%
set obj="../../CSharp.lua/obj"
if exist %obj% rd /s /q %obj%
dotnet "%dir%CSharp.lua.Launcher.dll" -s ../../CSharp.lua/ -d selfout -l %dir%Microsoft.CodeAnalysis.CSharp.dll;%dir%Microsoft.CodeAnalysis.dll;%dir%Cake.Common.dll;%dir%Cake.Core.dll;%dir%Cake.Incubator.dll;%dir%NuGet.Common.dll;%dir%NuGet.Configuration.dll;%dir%NuGet.Frameworks.dll;%dir%NuGet.Packaging.dll;%dir%NuGet.Versioning.dll;%dir%Newtonsoft.Json.dll -metadata
rd /s /q bin
pause
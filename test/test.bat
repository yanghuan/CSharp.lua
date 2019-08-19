dotnet publish ../ --configuration Release --output bin/Release/PublishOutput
set dir=../CSharp.lua.Launcher/bin/Release/PublishOutput/

dotnet "%dir%CSharp.lua.Launcher.dll" -s fibonacci/src -d fibonacci/out
cd TestCases 
cmd
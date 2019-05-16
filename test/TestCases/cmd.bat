set dir=../../CSharp.lua.Launcher/bin/Debug/netcoreapp2.0/
dotnet "%dir%CSharp.lua.Launcher.dll" -l "Bridge/Bridge.dll" -m "Bridge/Bridge.xml" -s src -d out -c -a "TestCase" -metadata
if not %errorlevel%==0 (
    echo please see log, has some error.
    goto:Fail 
)
"../__bin/lua5.3/lua" launcher.lua

:Fail
pause

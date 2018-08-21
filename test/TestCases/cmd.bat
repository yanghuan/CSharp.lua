set dir=../../CSharp.lua.Launcher/bin/Debug/netcoreapp2.0/
dotnet "%dir%CSharp.lua.Launcher.dll" -s src -d out -c -a "TestCase"
if not %errorlevel%==0 (
    echo please see log, has some error.
    goto:Fail 
)
"../__bin/lua5.1/lua" launcher.lua

:Fail
pause

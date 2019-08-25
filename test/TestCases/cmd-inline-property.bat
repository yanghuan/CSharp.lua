set dir=../../CSharp.lua.Launcher/bin/Debug/netcoreapp2.0/
set version=Lua5.3
set lua=../__bin/%version%/lua

dotnet "%dir%CSharp.lua.Launcher.dll" -inline-property  -l "Bridge/Bridge.dll" -m "Bridge/Bridge.xml" -s src -d out -a "TestCase" -metadata
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" launcher.lua
if not %errorlevel%==0 (
  goto:Fail 
)

echo **********************************************
echo ***********  test with jit         ***********
echo **********************************************

set version=LuaJIT-2.0.2
set lua=../__bin/%version%/lua

dotnet "%dir%CSharp.lua.Launcher.dll" -inline-property  -l "Bridge/Bridge.dll" -m "Bridge/Bridge.xml" -s src -d out -a "TestCase" -metadata -c
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" launcher.lua
if not %errorlevel%==0 (
  goto:Fail 
)

:Fail
if not %errorlevel%==0 (
 exit -1
)


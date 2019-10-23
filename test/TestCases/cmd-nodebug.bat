set dir=../../CSharp.lua.Launcher/bin/Debug/netcoreapp3.0/
set version=Lua5.3
set lua=../__bin/%version%/lua

dotnet "%dir%CSharp.lua.Launcher.dll" -l "Bridge/Bridge.dll" -m "Bridge/Bridge.xml" -s src -d out -a "TestCase" -metadata -p
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" launcher.lua nodebug
if not %errorlevel%==0 (
  goto:Fail 
)

echo **********************************************
echo ***********  test with jit         ***********
echo **********************************************

set version=LuaJIT-2.0.2

dotnet "%dir%CSharp.lua.Launcher.dll" -l "Bridge/Bridge.dll" -m "Bridge/Bridge.xml" -s src -d out -a "TestCase" -metadata -c -p
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" launcher.lua nodebug
if not %errorlevel%==0 (
  goto:Fail 
)

rem set version=LuaJIT-2.1.0-beta3
set version=Lua5.3
set lua=../../__bin/%version%/lua
"%lua%" run.lua

if not %errorlevel%==0 (
  echo please see log, has some error.
  goto :Fail 
)

:Fail
pause

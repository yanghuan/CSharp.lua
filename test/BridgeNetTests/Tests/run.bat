set version=Lua5.3
set lua=../../__bin/%version%/lua
call compile-all
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" run.lua
if not %errorlevel%==0 (
  goto:Fail 
)

echo test with jit
set version=LuaJIT-2.0.2
set lua=../../__bin/%version%/lua
call compile-all
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" run.lua
if not %errorlevel%==0 (
  goto:Fail 
)

:Fail


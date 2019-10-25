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

echo **********************************************
echo ***********  test with jit         ***********
echo **********************************************

set version=LuaJIT-2.0.2
call compile-all
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" run.lua
if not %errorlevel%==0 (
  goto:Fail 
)

echo **********************************************
echo ********  test no debug object  *********
echo **********************************************
set version=Lua5.3
set extra=-p
call compile-all
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" run.lua nodebug
if not %errorlevel%==0 (
  goto:Fail 
)

echo **********************************************
echo **  test with jit and  no debug object      **
echo **********************************************
set version=LuaJIT-2.0.2
call compile-all
if not %errorlevel%==0 (
  goto:Fail 
)
"%lua%" run.lua nodebug
if not %errorlevel%==0 (
  goto:Fail 
)

:Fail
if not %errorlevel%==0 (
  pause
  exit -1
)
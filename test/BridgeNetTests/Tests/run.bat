set lua=../../__bin/lua5.3/lua
"%lua%" run.lua

if not %errorlevel%==0 (
  echo please see log, has some error.
  goto :Fail 
)

:Fail
pause

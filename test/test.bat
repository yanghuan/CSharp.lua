cd TestCases
call test
if not %errorlevel%==0 (
  goto:Fail
)
cd ..

cd BridgeNetTests/Tests
call test
if not %errorlevel%==0 (
  goto:Fail 
)
cd ../..

:Fail
if not %errorlevel%==0 (
  pause
  exit -1
)




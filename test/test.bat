cd TestCases 
call cmd
if not %errorlevel%==0 (
  goto:Fail 
)
cd ..


cd BridgeNetTests/Batch1 
dotnet build --configuration Debug
if not %errorlevel%==0 (
  goto:Fail 
)
cd ../..


cd BridgeNetTests/Tests 
dotnet build --configuration Debug
if not %errorlevel%==0 (
  goto:Fail 
)
cd ../..

:Fail
if not %errorlevel%==0 (
  exit -1
)
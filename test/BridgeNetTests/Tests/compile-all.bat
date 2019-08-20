cd ../BridgeAttributes/ 
echo compile BridgeAttributes 
call compile
if not %errorlevel%==0 (
  goto:Fail 
)
cd ..

cd BridgeTestNUnit/ 
echo compile BridgeTestNUnit 
call compile
if not %errorlevel%==0 (
  goto:Fail 
)
cd ..


cd ClientTestHelper/ 
echo compile ClientTestHelper 
call compile
if not %errorlevel%==0 (
  goto:Fail 
)
cd ..


cd Batch1/ 
echo compile Batch1 
call compile
if not %errorlevel%==0 (
  goto:Fail 
)
cd ..


cd Tests/ 
echo compile Tests
call compile
if not %errorlevel%==0 (
  goto:Fail 
)

:Fail



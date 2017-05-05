set version=1.0.0.0
set dir=CSharp.lua
set file=%dir%.%version%.zip
if exist "%file%" del "%file%"
md %dir%
xcopy "..\CSharp.lua\bin\Release"  %dir%  /s /e /y 
set _7z="D:\Program Files\7-Zip\7z"
%_7z% a %file% %dir%
rd /s /q %dir%
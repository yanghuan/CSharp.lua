set version=1.1.0
set binDir=CSharp.lua
set coreDir=CoreSystem.lua
set _7z="D:\Program Files\7-Zip\7z"
set file=%binDir%-%version%.zip
if exist "%file%" del "%file%"
md %binDir%
xcopy "..\CSharp.lua\bin\Release\PublishOutput" %binDir%  /s /e /y 
md %coreDir%
xcopy "..\CSharp.lua\CoreSystem.Lua" %coreDir%  /s /e /y 
%_7z% a %file% %binDir% %coreDir% README.md
rd /s /q %binDir%
rd /s /q %coreDir%
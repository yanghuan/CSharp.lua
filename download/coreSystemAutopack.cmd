set dir=CoreSystem.lua
set file=%dir%.zip
if exist "%file%" del "%file%"
md %dir%
xcopy "..\CSharp.lua\CoreSystem.Lua"  %dir%  /s /e /y 
set _7z="D:\Program Files\7-Zip\7z"
%_7z% a %file% %dir%
rd /s /q %dir%
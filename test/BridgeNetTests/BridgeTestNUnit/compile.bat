call ..\Tests\compile-config.bat
set src=.\src
set out=.\out
set l=..\BridgeAttributes%bin%BridgeAttributes.dll!

%compile% -s %src% -d %out% -l %l%

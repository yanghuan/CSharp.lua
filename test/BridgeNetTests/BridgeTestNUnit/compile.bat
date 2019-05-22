call ..\compile-config.bat
set src=.\src
set out=.\out
set bin=\bin\Debug\netcoreapp2.1\
set lib=..\BridgeAttributes%bin%BridgeAttributes.dll!

%compile% -s %src% -d %out% -l %lib%

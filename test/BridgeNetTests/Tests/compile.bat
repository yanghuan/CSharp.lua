call compile-config.bat
set src=.\src
set out=.\out
set bin=\bin\Debug\netcoreapp2.1\
set l=..\BridgeTestNUnit%bin%BridgeTestNUnit.dll!
%compile% -s %src% -d %out% -l %l%

call run.bat
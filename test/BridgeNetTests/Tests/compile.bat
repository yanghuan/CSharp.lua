call compile-config.bat
set src=.\src
set out=.\out
set l=..\BridgeTestNUnit%bin%BridgeTestNUnit.dll!;..\Batch1%bin%Batch1.dll!
%compile% -s %src% -d %out% -l %l%
call ..\Tests\compile-config.bat
set src=.\src
set out=.\out
set l=..\ClientTestHelper%bin%ClientTestHelper.dll!;..\BridgeTestNUnit%bin%BridgeTestNUnit.dll!;..\BridgeAttributes%bin%BridgeAttributes.dll!
%compile% -s %src% -d %out% -l %l% -a

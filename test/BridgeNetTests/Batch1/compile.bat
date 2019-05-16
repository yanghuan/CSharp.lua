call ..\compile-config.bat
set src=.\src
set out=.\out
set bin=\bin\Debug\netcoreapp2.1\
set lib=..\ClientTestHelper%bin%ClientTestHelper.dll!;..\BridgeTestNUnit%bin%Bridge.Test.NUnit.dll!;..\BridgeAttributes%bin%Bridge.Attributes.dll!

%compile% -s %src% -d %out% -l %lib% -a

set bin=\bin\Debug\netcoreapp3.0\
set CSharpLua=..\..\..\CSharp.lua.Launcher\bin\Debug\netcoreapp3.0\CSharp.lua.Launcher.dll
if not "%lua%"=="%lua:jit=%" (
  set jit=-c -csc /define:__JIT__
) else (
  set jit=
)
set compile=dotnet %CSharpLua% -metadata -module %jit% %extra%
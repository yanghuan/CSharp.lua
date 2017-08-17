set dir=../../CSharp.lua/bin/Debug/netcoreapp2.0/
set lib=lib/
dotnet "%dir%CSharp.lua.dll" -s ../../CSharp.lua/ -d selfout -l %lib%Microsoft.CodeAnalysis.CSharp.dll;%lib%Microsoft.CodeAnalysis.dll
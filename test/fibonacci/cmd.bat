set dir=../../CSharp.lua/bin/Debug/netcoreapp2.0/
dotnet "%dir%CSharp.lua.dll" -s src -d out
"../__bin/lua5.1/lua" launcher.lua
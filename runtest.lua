package.path = package.path .. ";./CSharp.lua/CoreSystem.Lua/?.lua"
require ("All")()
require("out.manifest")("out")
Test.Program.Main()
os.exit()

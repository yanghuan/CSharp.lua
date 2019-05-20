require("strict")

package.path = package.path .. ";../../../CSharp.lua/Coresystem.lua/?.lua"
require("All")()          -- coresystem.lua/All.lua

package.path = package.path .. ";../?.lua"

local modules = {
  "BridgeAttributes",
  "BridgeTestNUnit",
  "ClientTestHelper",
  "Batch1",
  "Tests"
}

for i = 1, #modules do
  local name = modules[i]
  require(name .. "/out/manifest")(name .. "/out")
end

local main = System.Reflection.Assembly.GetEntryAssembly().getEntryPoint()
main:Invoke()

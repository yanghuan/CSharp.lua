local traceback
if arg[1] == "nodebug" then
  traceback = debug.traceback 
  debug = nil
else
  require("strict")
end

package.path = package.path .. ";../../../CSharp.lua/Coresystem.lua/?.lua"
package.path = package.path .. ";../?.lua"

package.path = package.path .. ";CSharp.lua/Coresystem.lua/?.lua"
package.path = package.path .. ";test/BridgeNetTests/?.lua;"
package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"

local now = 0
local timeoutQueue

local conf = {
  traceback = traceback,
  setTimeout = function (f, delay)
    if not timeoutQueue then
      timeoutQueue = System.TimeoutQueue()
    end
    return timeoutQueue:Add(now, delay, f)
  end,
  clearTimeout = function (t)
    timeoutQueue:Erase(t)
  end
}
require("All")("", conf)          -- coresystem.lua/All.lua

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

collectgarbage("collect")
print(collectgarbage("count"))

local main = System.Reflection.Assembly.GetEntryAssembly():getEntryPoint()
main:Invoke()

if timeoutQueue then
  while true do
    local nextExpiration = timeoutQueue:getNextExpiration()
    if nextExpiration ~= timeoutQueue.MaxExpiration then
      now = nextExpiration
      timeoutQueue:RunLoop(now)
    else
      break
    end
  end
end

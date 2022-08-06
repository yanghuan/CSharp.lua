require("run")

local src = "../../CSharp.lua"

local function removeObj()
  local obj =  src .. "/obj/"
  local cmd = ('if exist "%s" rd /s /q "%s"'):format(obj, obj)
  execute(cmd)
end

run({
 depth = 2,
 input = src,
 output = "out",
 libs = ("%sMicrosoft.CodeAnalysis.CSharp.dll;%sMicrosoft.CodeAnalysis.dll"):format(publishOutputDir, publishOutputDir),
 metadata = true,
}, true, removeObj)


local System = System

local Assembly = {}



System.define("System.Assembly")

local entryAssembly = Assembly()
entryAssembly.FullName = ""

function Assembly.GetEntryAssembly()
    return entryAssembly
end

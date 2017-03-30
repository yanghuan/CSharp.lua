local System = System
local Type = System.Type

local Assembly = {}

function Assembly.getEntryPoint(this)
    local main = System.entryPoint
    if main ~= nil then
        
    end
end

function Assembly.GetName(this)
    return this.FullName;
end

local assembly

local function getAssembly()
    return assembly
end

Assembly.GetAssembly = getAssembly
Assembly.GetCallingAssembly = getAssembly
Assembly.GetEntryAssembly = getAssembly
Assembly.GetExecutingAssembly = getAssembly
Assembly.GetTypeFrom = Type.GetTypeFrom

System.define("System.Reflection.Assembly", Assembly)

assembly = Assembly()
assembly.FullName = "CSharp.lua, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
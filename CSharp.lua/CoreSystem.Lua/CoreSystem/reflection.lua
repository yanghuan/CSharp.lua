local System = System

local assert = assert

local Assembly = {}

function Assembly.getEntryPoint(this)
   return System.entryPoint
end

function Assembly.getIsDynamic(this)
    return false
end

function Assembly.getIsFullyTrusted(this)
    return true
end

function Assembly.getLocation(this)
    return ""
end

function Assembly.getReflectionOnly(this)
    return false
end

local entryAssembly

function Assembly.GetAssembly(type)
    return entryAssembly
end

function  Assembly.GetCallingAssembly()
    return entryAssembly
end

function Assembly.GetEntryAssembly()
    return entryAssembly
end

function Assembly.GetExecutingAssembly()
    return entryAssembly
end

function Assembly.CreateInstance(typeName, ignoreCase)
    assert(not ignoreCase)

end

function Assembly.GetName(this)
    return this.FullName;
end


Assembly.FullName = "CSharp.lua, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
System.define("System.Assembly")

entryAssembly = Assembly()


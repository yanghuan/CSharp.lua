local System = System
local Type = System.Type
local getClass = System.getClass
local typeof = System.typeof
local toLuaTable = System.toLuaTable

local setmetatable = setmetatable
local assert = assert
local unpack = table.unpack
local ipairs = ipairs

local Assembly = {}

local function getName(this)
    return this.name
end

Assembly.GetName = getName
Assembly.getFullName = getName

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
assembly.name = "CSharp.lua, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

local MemberInfo = {}
MemberInfo.getName = getName

function MemberInfo.getMemberType(this) 
    return this.memberType 
end

function MemberInfo.getDeclaringType(this)
    return typeof(this.c)
end

System.define("System.Reflection.MemberInfo", MemberInfo)

local MethodInfo = { memberType = 8 }

function Assembly.getEntryPoint(this)
    local entryPoint = System.entryPoint
    if entryPoint ~= nil then
        local _, _, t, name = entryPoint:find("(.*)%.(.*)")
        local cls = getClass(t)
        local f = cls[name]
        return setmetatable({ c = cls, name = name, f = f }, MethodInfo)
    end
end

function Type.GetMethod(this, name)
    if name == nil then
        throw(ArgumentNullException("name"))
    end
    local cls = this.c
    local f = cls[name]
    if type(f) == "function" then
        return setmetatable({ c = cls, name = name, f = f }, MemberInfo)
    end
end

function MethodInfo.Invoke(this, obj, parameters)
    local f = this.f
    if obj == nil then
        if parameters ~= nil then
            local t = toLuaTable(parameters)
            return f(unpack(t, 1, #parameters))
        else
            return f()
        end
    else
        if parameters ~= nil then
            local t = toLuaTable(parameters)
            return f(obj, unpack(t, 1, #parameters))
        else
            return f(obj)
        end
    end
end

local function isDefined(cls, name, attributeCls)
    local attributes = cls.__attributes__
    if attributes ~= nil then
        local f = cls[name]
        local attrTable = attributes[f]
        if attrTable ~= nil then
            for _, v in ipairs(attrTable) do
                if v == attributeCls then
                    return true
                end
            end
        end
    end
    return false
end

function MethodInfo.IsDefined(this, attributeType, inherit)
    if not inherit then
        return isDefined(this.c, this.name, attributeType.c)
    else
        local cls, name, attributeCls = this.c, this.name, attributeType.c
        repeat 
            if isDefined(cls, name, attributeCls) then
                return true
            end
            cls = cls.__base__
        until cls == nil  
        return false
    end
end

function MethodInfo.__eq(left, right)
    if left == right then
        return true
    else
        return left.c == right.c and left.f == right.f
    end
end

MethodInfo.__inherits__ = { MemberInfo }
System.define("System.Reflection.MethodInfo", MethodInfo)


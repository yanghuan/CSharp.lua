--[[
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
--]]

local System = System
local throw = System.throw
local Type = System.Type
local getClass = System.getClass
local typeof = System.typeof
local toLuaTable = System.toLuaTable
local ArgumentException = System.ArgumentException
local ArgumentNullException = System.ArgumentNullException

local setmetatable = setmetatable
local assert = assert
local unpack = table.unpack
local pairs = pairs
local ipairs = ipairs
local tinsert = table.insert

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

function Assembly.GetExportedTypes(this)
  if this.exportedTypes then
    return this.exportedTypes
  end
  local t = {}
  for _, cls in ipairs(System.classes) do
    local type_ = type(cls)
    if type_  == "table" then
      tinsert(t, typeof(cls))
    else
      assert(type_ == "function");
    end
  end
  local array = System.arrayFromTable(t, Type)
  this.exportedTypes = array
  return array
end

System.define("System.Reflection.Assembly", Assembly)

assembly = Assembly()
assembly.name = System.config.assemblyName or "CSharp.lua, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

local MemberInfo = {}
MemberInfo.getName = getName
MemberInfo.__inherits__ = { System.Object }

function MemberInfo.getMemberType(this) 
  return this.memberType 
end

function MemberInfo.getDeclaringType(this)
  return typeof(this.c)
end

local function isDefined(cls, name, attributeCls)
  local attributes = cls.__attributes__
  if attributes ~= nil then
    local attrTable = attributes[name]
    if attrTable ~= nil then
      for _, v in ipairs(attrTable) do
        if System.is(v, attributeCls) then
          return true
        end
      end
    end
  end
  return false
end

function MemberInfo.IsDefined(this, attributeType, inherit)
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

System.define("System.Reflection.MemberInfo", MemberInfo)

local MethodInfo = { memberType = 8 }

local function checkObj(obj, cls)
  if not System.is(obj, cls) then
    throw(ArgumentException("Object does not match target type.", "obj"), 1)
  end
end

function MethodInfo.Invoke(this, obj, parameters)
  local f = this.f
  if obj ~= nil then
    checkObj(obj, this.c)
    if parameters ~= nil then
      local t = toLuaTable(parameters)
      return f(obj, unpack(t, 1, #parameters))
    else
      return f(obj)
    end
  else
    if parameters ~= nil then
      local t = toLuaTable(parameters)
      return f(unpack(t, 1, #parameters))
    else
      return f()
    end
  end
end

local function eq(left, right)
  return left.c == right.c and left.name == right.name
end

MethodInfo.__eq = eq
MethodInfo.__inherits__ = { MemberInfo }

System.define("System.Reflection.MethodInfo", MethodInfo)

local function buildMethodInfo(cls, name, f) 
   return setmetatable({ c = cls, name = name, f = f }, MethodInfo)
end

function Assembly.getEntryPoint(this)
  local entryPoint = System.config.entryPoint
  if entryPoint ~= nil then
    local _, _, t, name = entryPoint:find("(.*)%.(.*)")
    local cls = getClass(t)
    local f = cls[name]
    return buildMethodInfo(cls, name, f)
  end
end

function Type.GetMethod(this, name)
  if name == nil then
    throw(ArgumentNullException("name"))
  end
  local cls = this.c
  local f = cls[name]
  if type(f) == "function" then
    return buildMethodInfo(cls, name, f)
  end
end

function Type.GetMethods(this)
  local t = {}
  local cls = this.c
  repeat
    for k, v in pairs(cls) do
      if type(v) == "function" then
        local methodInfo = buildMethodInfo(cls, k, v)
        tinsert(t, methodInfo)
      end
    end
    cls = cls.__base__
  until cls == nil 
  return System.arrayFromTable(t, MethodInfo)  
end

local FieldInfo = { memberType = 4 }

local function buildFieldInfo(cls, name) 
  return setmetatable({ c = cls, name = name }, FieldInfo)
end


function FieldInfo.GetValue(this, obj)
  if obj ~= nil then
    checkObj(obj, this.c)
    return obj[this.name]
  else
    return this.c[this.name]
  end
end

function FieldInfo.SetValue(this, obj, value)
  if obj ~= nil then
     checkObj(obj, this.c)
     obj[this.name] = value
  else
     this.c[this.name] = value 
  end
end

FieldInfo.__eq = eq
FieldInfo.__inherits__ = { MemberInfo }

System.define("System.Reflection.FieldInfo", FieldInfo)

function Type.GetField(this, name)
  if name == nil then
    throw(ArgumentNullException("name"))
  end
  local cls = this.c
  if type(cls[name]) ~= "function" then
    return setmetatable({ c = cls, name = name }, FieldInfo)
  end
end

local PropertyInfo = { memberType = 16 }

function PropertyInfo.GetValue(this, obj)
  if obj ~= nil then
    checkObj(obj, this.c)
    if this.isField then
      return obj[this.nam]
    else
      local f = obj["get" .. this.name]
      if f == nil then
        throw(ArgumentException("Property get method not found."))
      end
      return f(obj)
    end
  else
    if this.isField then
      return this.c[this.nam]
    else
      local f = this.c["get" .. this.name]
      if f == nil then
        throw(ArgumentException("Property get method not found."))
      end
      return f()
    end
  end
end

function PropertyInfo.SetValue(this, obj, value)
  if obj ~= nil then
    checkObj(obj, this.c)
    if this.isField then
      obj[this.name] = value
    else
      local f = obj["set" .. this.name]
      if f == nil then
        throw(ArgumentException("Property set method not found."))
      end
      f(obj, value)
    end
  else
    if this.isField then
      this.c[this.name] = value 
    else
      local f = this.c["get" .. this.name]
      if f == nil then
        throw(ArgumentException("Property set method not found."))
      end
      f(value)
    end
  end
end

PropertyInfo.__eq = eq
PropertyInfo.__inherits__ = { MemberInfo }

System.define("System.Reflection.PropertyInfo", PropertyInfo)

function Type.GetMembers(this)
  local t = {}
  local names = {};
  local cls = this.c
  repeat
    for k, v in pairs(cls) do
      if type(v) == "function" then
        local methodInfo = buildMethodInfo(cls, k, v)
        tinsert(t, methodInfo)
        names[k] = true;
      else
        local fieldInfo = buildFieldInfo(cls, k)
        tinsert(t, fieldInfo)
        names[k] = true;
      end
    end
    local attributes = cls.__attributes__;
    if attributes then
      for k , v in pairs(attributes) do
        if not names[k] then
            local fieldInfo = buildFieldInfo(cls, k)
            tinsert(t, fieldInfo)
            names[k] = true;
        end
      end
    end
    cls = cls.__base__
  until cls == nil 
  return System.arrayFromTable(t, MemberInfo)  
end

function Type.GetProperty(this, name)
  if name == nil then
    throw(ArgumentNullException("name"))
  end
  local cls = this.c
  if cls["get" .. name] or cls["set" .. name] then
    return setmetatable({ c = cls, name = name }, PropertyInfo)
  else
    return setmetatable({ c = cls, name = name, isField = true }, PropertyInfo)
  end
end

function Type.IsDefined(this, attributeType, inherit)
  if not inherit then
    return isDefined(this.c, "class", attributeType.c)
  else
    local cls, attributeCls = this.c, attributeType.c
    repeat 
      if isDefined(cls, "class", attributeCls) then
        return true
      end
      cls = cls.__base__
    until cls == nil  
    return false
  end
end
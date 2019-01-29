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
local Object = System.Object
local String = System.String
local Boolean = System.Boolean
local Delegate = System.Delegate
local getClass = System.getClass

local InvalidCastException = System.InvalidCastException
local ArgumentNullException = System.ArgumentNullException
local TypeLoadException = System.TypeLoadException
local NullReferenceException = System.NullReferenceException

local Char = System.Char
local SByte = System.SByte
local Byte = System.Byte
local Int16 = System.Int16
local UInt16 = System.UInt16
local Int32 = System.Int32
local UInt32 = System.UInt32
local Int64 = System.Int64
local UInt64 = System.UInt64
local Single = System.Single
local Double = System.Double
local Int = System.Int
local Number = System.Number
local ValueType = System.ValueType

local type = type
local getmetatable = getmetatable
local ipairs = ipairs
local select = select
local unpack = table.unpack
local floor = math.floor

local Type = {}

local NumberType = {
  __index = Type,
  __eq = function (a, b)
    local c1, c2 = a.c, b.c
    if c1 == c2 then
      return true
    end
    if c1 == Number or c2 == Number then
      return true
    end
    return false
  end
}

local function newNumberType(c)
  return setmetatable({ c = c }, NumberType)
end

local types = {
  [Char] = newNumberType(Char),
  [SByte] = newNumberType(SByte),
  [Byte] = newNumberType(Byte),
  [Int16] = newNumberType(Int16),
  [UInt16] = newNumberType(UInt16),
  [Int32] = newNumberType(Int32),
  [UInt32] = newNumberType(UInt32),
  [Int64] = newNumberType(Int64),
  [UInt64] = newNumberType(UInt64),
  [Single] = newNumberType(Single),
  [Double] = newNumberType(Double),
  [Int] = newNumberType(Int),
  [Number] = newNumberType(Number),
}

local createType = System.config.customTypeOf or function(cls) return setmetatable({ c = cls }, Type) end

local function typeof(cls)
  assert(cls)
  local type = types[cls]
  if type == nil then
    type = createType(cls)
    types[cls] = type
  end
  return type
end

local function getType(obj)
  return typeof(getmetatable(obj))
end

System.Object.GetType = getType
System.typeof = typeof

local function isGenericName(name)
  return name:byte(#name) == 93
end

function Type.getIsGenericType(this)
  return isGenericName(this.c.__name__)
end

function Type.getIsEnum(this)
  return this.c.class == "E"
end

function Type.getName(this)
  local name = this.name
  if name == nil then
    local clsName = this.c.__name__
    local pattern = isGenericName(clsName) and "^.*()%.(.*)%[.+%]$" or "^.*()%.(.*)$"
    name = clsName:gsub(pattern, "%2")
    this.name = name
  end
  return name
end

function Type.getFullName(this)
  return this.c.__name__
end

function Type.getNamespace(this)
  local namespace = this.namespace
  if namespace == nil then
    local clsName = this.c.__name__
    local pattern = isGenericName(clsName) and "^(.*)()%..*%[.+%]$" or "^(.*)()%..*$"
    namespace = clsName:gsub(pattern, "%1")
    this.namespace = namespace
  end
  return namespace
end

local function getBaseType(this)
  local baseType = this.baseType
  if baseType == nil then
    local baseCls = getmetatable(this.c)
    if baseCls ~= nil then
      baseType = typeof(baseCls)
      this.baseType = baseType
    end
  end 
  return baseType
end

Type.getBaseType = getBaseType

local function isSubclassOf(this, c)
  local p = this
  if p == c then
    return false
  end
  while p ~= nil do
    if p == c then
      return true
    end
    p = getBaseType(p)
  end
  return false
end

Type.IsSubclassOf = isSubclassOf

local function getIsInterface(this)
  return this.c.class == "I"
end

Type.getIsInterface = getIsInterface

local function getIsValueType(this)
  return this.c.class == "S"
end

Type.getIsValueType = getIsValueType

local function getInterfaces(this)
  local interfaces = this.interfaces
  if interfaces == nil then
    interfaces = {}
    local p = this.c
    repeat
      local interfacesCls = p.interface
      if interfacesCls ~= nil then
        for _, i in ipairs(interfacesCls) do
          interfaces[#interfaces + 1] = typeof(i)
        end
      end
      p = getmetatable(p)
    until p == nil
    this.interfaces = interfaces
  end
  return interfaces
end

function Type.getInterfaces(this)
  local interfaces = getInterfaces(this)
  local array = {}
  for _, i in ipairs(interfaces) do
    array[#array + 1] = i
  end    
  return System.arrayFromTable(array, Type)
end

local function implementInterface(this, ifaceType)
  local t = this
  while t ~= nil do
    local interfaces = getInterfaces(this)
    if interfaces ~= nil then
      for _, i in ipairs(interfaces) do
        if i == ifaceType or implementInterface(i, ifaceType) then
          return true
        end
      end
    end
    t = getBaseType(t)
  end
  return false
end

local function isAssignableFrom(this, c)
  if c == nil then 
    return false 
  end
  if this == c then 
    return true 
  end
  if getIsInterface(this) then
    return implementInterface(c, this)
  else 
    return isSubclassOf(c, this)
  end
end 

Type.IsAssignableFrom = isAssignableFrom

function Type.IsInstanceOfType(this, obj)
  if obj == nil then
    return false 
  end
  return isAssignableFrom(this, obj:GetType())
end

function Type.ToString(this)
  return this.c.__name__
end

function Type.GetTypeFrom(typeName, throwOnError, ignoreCase)
  if typeName == nil then
    throw(ArgumentNullException("typeName"))
  end
  if #typeName == 0 then
    if throwOnError then
      throw(TypeLoadException("Arg_TypeLoadNullStr"))
    end
    return nil
  end
  assert(not ignoreCase, "ignoreCase is not support")
  local cls = getClass(typeName)
  if cls ~= nil then
    return typeof(cls)
  end
  if throwOnError then
    throw(TypeLoadException(typeName .. ": failed to load."))
  end
  return nil
end

Type.Equals = System.equals
System.define("System.Type", Type)

local function isInterfaceOf(t, ifaceType)
  repeat
    local interfaces = t.interface
    if interfaces then
      for _, i in ipairs(interfaces) do
        if i == ifaceType or isInterfaceOf(i, ifaceType) then
          return true
        end
      end 
    end
    t = getmetatable(t)
  until t == nil
  return false
end

local isUserdataTypeOf = System.config.isUserdataTypeOf
local numbers = {
  [Char] = { 0, 65535 },
  [SByte] = { -128, 127 },
  [Byte] = { 0, 255 },
  [Int16] = { -32768, 32767 },
  [UInt16] = { 0, 65535 },
  [Int32] = { -2147483648, 2147483647 },
  [UInt32] = { 0, 4294967295 },
  [Int64] = { -9223372036854775808, 9223372036854775807 },
  [UInt64] = { 0, 18446744073709551615 },
  [Single] = { -3.40282347E+38, 3.40282347E+38, 1 },
  [Double] = { nil, nil, 2 }
}
numbers[Int] = numbers[Int32]

function isTypeOf(obj, cls)    
  if cls == Object then return true end
  local typename = type(obj)
  if typename == "number" then
    local info = numbers[cls]
    if info ~= nil then
      local min, max, sign = info[1], info[2], info[3]
      if sign == nil then
        if obj < min or obj > max then
          return false
        end
        if floor(obj) ~= obj then
          return false
        end
      elseif sign == 1 then
        if obj < min or obj > max then
          return false
        end
      end
      return true
    elseif cls.class == "I" then
      return isInterfaceOf(Number, cls)
    elseif cls == ValueType  then
      return true
    end
    return false
  elseif typename == "string" then
    if cls == String then
      return true
    end
    if cls.class == "I" then
      return isInterfaceOf(String, cls)
    end
    return false
  elseif typename == "table" then   
    local t = getmetatable(obj)
    if t == cls then
      return true
    end
    if cls.class == "I" then
      return isInterfaceOf(t, cls)
    else
      local base = getmetatable(t)
      while base ~= nil do
        if base == cls then
          return true
        end
        base = getmetatable(base)
      end
      return false
    end
  elseif typename == "boolean" then
    if cls == Boolean or cls == ValueType then
      return true
    end
    if cls.class == "I" then
      return isInterfaceOf(Boolean, cls)
    end
    return false
  elseif typename == "function" then 
    return cls == Delegate
  elseif typename == "userdata" then
    if isUserdataTypeOf then
      return isUserdataTypeOf(obj, cls)
    end
    return true
  else
    assert(false)
  end
end

function System.is(obj, cls)
  if obj ~= nil then
    return isTypeOf(obj, cls)
  end
  return cls == nil
end

function System.as(obj, cls)
  if obj ~= nil and isTypeOf(obj, cls) then
    return obj
  end
  return nil
end

function System.cast(cls, obj)
  if obj == nil then
    if cls.class ~= "S" then
      return nil
    end
    throw(NullReferenceException(), 1)
  else 
    if isTypeOf(obj, cls) then
      return obj
    end
    throw(InvalidCastException(), 1)
  end
end

function System.CreateInstance(type, ...)
  if type == nil then
    throw(ArgumentNullException("type"))
  end
  if getmetatable(type) ~= Type then   -- is T
    return type()
  end
  local len = select("#", ...)
  if len == 1 then
    local args = ...
    if System.isArrayLike(args) then
      local t = {}
      for k, v in System.ipairs(args) do
        t[k] = v
      end
      return type.c(unpack(t, 1, #args))
    end
  end
  return type.c(...)
end

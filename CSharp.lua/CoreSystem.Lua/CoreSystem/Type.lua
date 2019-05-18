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
local try = System.try
local new = System.new
local arrayFromTable = System.arrayFromTable

local InvalidCastException = System.InvalidCastException
local ArgumentNullException = System.ArgumentNullException
local MissingMethodException = System.MissingMethodException
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

local assert = assert
local type = type
local setmetatable = setmetatable
local getmetatable = getmetatable
local select = select
local unpack = table.unpack
local floor = math.floor

local Type, typeof

local function isGenericName(name)
  return name:byte(#name) == 93
end

local function getBaseType(this)
  local baseType = this.baseType
  if baseType == nil then
    local baseCls = getmetatable(this[1])
    if baseCls ~= nil then
      baseType = typeof(baseCls)
      this.baseType = baseType
    end
  end
  return baseType
end

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

local function getIsInterface(this)
  return this[1].class == "I"
end

local function getIsValueType(this)
  return this[1].class == "S"
end

local function fillInterfaces(t, cls, set)
  local base = getmetatable(cls)
  if base then
    fillInterfaces(t, base, set)
  end
  local interface = cls.interface
  if interface then
    for i = 1, #interface do
      local it = interface[i]
      if not set[it] then
        t[#t + 1] = typeof(it)
        set[it] = true
      end
      fillInterfaces(t, it, set)
    end
  end
end

local function getInterfaces(this)
  local t = this.interfaces
  if t == nil then
    t = arrayFromTable({}, Type, true)
    fillInterfaces(t, this[1], {})
    this.interfaces = t
  end
  return t
end

local function implementInterface(this, ifaceType)
  local t = this
  while t ~= nil do
    local interfaces = getInterfaces(this)
    if interfaces ~= nil then
      for i = 1, #interfaces do
        local it = interfaces[i]
        if it == ifaceType or implementInterface(it, ifaceType) then
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

Type = System.define("System.Type", {
  Equals = System.equals,
  getIsGenericType = function (this)
    return isGenericName(this[1].__name__)
  end,
  getContainsGenericParameters = function (this)
    return isGenericName(this[1].__name__)
  end,
  MakeGenericType = function (this, ...)
    local args = { ... }
    for i = 1, #args do
      args[i] = args[i][1]
    end
    return typeof(this[1](unpack(args)))
  end,
  getIsEnum = function (this)
    return this[1].class == "E"
  end,
  getIsClass = function (this)
    return this[1].class == "C"
  end,
  getIsValueType = function (this)
    return this[1].class == "S" 
  end,
  getName = function (this)
    local name = this.name
    if name == nil then
      local clsName = this[1].__name__
      local pattern = isGenericName(clsName) and "^.*()%.(.*)%[.+%]$" or "^.*()%.(.*)$"
      name = clsName:gsub(pattern, "%2")
      this.name = name
    end
    return name
  end,
  getFullName = function (this)
    return this[1].__name__
  end,
  getNamespace = function (this)
    local namespace = this.namespace
    if namespace == nil then
      local clsName = this[1].__name__
      local pattern = isGenericName(clsName) and "^(.*)()%..*%[.+%]$" or "^(.*)()%..*$"
      namespace = clsName:gsub(pattern, "%1")
      this.namespace = namespace
    end
    return namespace
  end,
  getBaseType = getBaseType,
  IsSubclassOf = isSubclassOf,
  getIsInterface = getIsInterface,
  getIsValueType = getIsValueType,
  GetInterfaces = getInterfaces,
  IsAssignableFrom = isAssignableFrom,
  IsInstanceOfType = function (this, obj)
    if obj == nil then
      return false 
    end
    return isAssignableFrom(this, obj:GetType())
  end,
  ToString = function (this)
    return this[1].__name__
  end,
  GetTypeFrom = function (typeName, throwOnError, ignoreCase)
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
})

local NumberType = {
  __index = Type,
  __eq = function (a, b)
    local c1, c2 = a[1], b[1]
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
  return setmetatable({ c }, NumberType)
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

local customTypeOf = System.config.customTypeOf

function typeof(cls)
  assert(cls)
  local type = types[cls]
  if type == nil then
    if customTypeOf then
      type = customTypeOf(cls)
    else
      type = setmetatable({ cls }, Type)
    end
    types[cls] = type
  end
  return type
end

local function getType(obj)
  return typeof(getmetatable(obj))
end

System.typeof = typeof
System.Object.GetType = getType

local function addCheckInterface(set, cls)
  local interface = cls.interface
  if interface then
    for i = 1, #interface do
      local it = interface[i]
      set[it] = true
      addCheckInterface(set, it)
    end
  end
end

local function getCheckSet(cls)
  local set = {}
  local p = cls
  repeat
    set[p] = true
    addCheckInterface(set, p)
    p = getmetatable(p)
  until not p
  return set
end

local customTypeCheck = System.config.customTypeCheck

local checks = setmetatable({}, {
  __index = function (checks, cls)
    if customTypeCheck then
      local add, f = customTypeCheck(cls)
      if add then
        checks[cls] = f
      end
      return f
    end

    local set = getCheckSet(cls)
    local function check(obj, T)
      return set[T]
    end
    checks[cls] = check
    return check
  end
})

checks[Number] = function (obj, T)
  local set = getCheckSet(Number)
  local numbers = {
    [Char] = function (obj) return type(obj) == "number" and obj >= 0 and obj <= 65535 and floor(obj) == obj end,
    [SByte] = function (obj) return type(obj) == "number" and obj >= -128 and obj <= 127 and floor(obj) == obj end,
    [Byte] = function (obj) return type(obj) == "number" and obj >= 0 and obj <= 255 and floor(obj) == obj end,
    [Int16] = function (obj) return type(obj) == "number" and obj >= -32768 and obj <= 32767 and floor(obj) == obj end,
    [UInt16] = function (obj) return type(obj) == "number" and obj >= 0 and obj <= 32767 and floor(obj) == obj end,
    [Int32] = function (obj) return type(obj) == "number" and obj >= -2147483648 and obj <= 2147483647 and floor(obj) == obj end,
    [UInt32] = function (obj) return type(obj) == "number" and obj >= 0 and obj <= 4294967295 and floor(obj) == obj end,
    [Int64] = function (obj) return type(obj) == "number" and obj >= -9223372036854775808 and obj <= 9223372036854775807 and floor(obj) == obj end,
    [UInt64] = function (obj) return type(obj) == "number" and obj >= 0 and obj <= 18446744073709551615 and floor(obj) == obj end,
    [Single] = function (obj) return type(obj) == "number" and obj >= -3.40282347E+38 and obj <= 3.40282347E+38 end,
    [Double] = function (obj) return type(obj) == "number" end
  }
  local function check(obj, T)
    local number = numbers[T]
    if number then
      return number(obj)
    end
    return set[T]
  end
  checks[Number] = check
  return check(obj, T)
end

local function is(obj, T)
  return checks[getmetatable(obj)](obj, T)
end

System.is = is

function System.as(obj, cls)
  if obj ~= nil and is(obj, cls) then
    return obj
  end
end

local function cast(cls, obj, nullable)
  if obj ~= nil then
    if is(obj, cls) then
      return obj
    end
    throw(InvalidCastException(), 1)
  else
    if cls.class ~= "S" or nullable then
      return
    end
    throw(NullReferenceException(), 1)
  end
end

System.cast = cast

function System.castWithNullable(cls, obj)
  if System.isNullable(cls) then
    return cast(cls.__genericT__, obj, true)
  end
  return cast(cls, obj)
end

local function tryMatchParameters(parameter, argument)
  if type(argument) == "table" then
      return typeof(parameter) == typeof(argument)
  end
  -- If userdata or nil is handled then the parameter type can be ignored
  if type(argument) == "userdata" then 
    return true 
  end
  return type(argument) == type(parameter)
end

local function tryCallConstructor(cls, ...)
  if not cls.__ctor__ then
    throw(MissingMethodException("No matching constructor was found"))
  end
  if type(cls.__ctor__) == "table" then
    if cls.__metadata__ and cls.__metadata__.methods then
      local methods = cls.__metadata__.methods
      local args = { ... }
      for i = 1, #methods do
        if methods[i][1] == ".ctor" then 
          local index = 4
          local matched = true
          -- Use pairs as arguments can be nil
          for _,arg in pairs(args) do
            if not tryMatchParameters(methods[i][index]) then
              matched = false
            end
            index = index + 1
          end
          if matched then
            return new(cls, i, ...)
          end
        end
      end
      throw(MissingMethodException("No matching constructor was found"))
    else
      -- For backward compability we use the first constructor if no metadata is present, this can later be changed to throwing the exception below
      return new(cls, 1, ...)
      -- throw(MissingMethodException("CSharp.lua can't find a constructor out of multiple constructors in class ".. typeof(cls):getName() .." without defined metadata, use @CSharpLua.Metadata at constructors"))
    end
  else
    return cls(...)
  end
end

function System.CreateInstance(type, ...)
  if type == nil then
    throw(ArgumentNullException("type"))
  end
  if getmetatable(type) ~= Type then   -- is T
    return type()
  end
  return tryCallConstructor(type[1], ...)
end

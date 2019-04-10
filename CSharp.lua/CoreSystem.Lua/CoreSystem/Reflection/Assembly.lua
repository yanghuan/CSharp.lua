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
local define = System.define
local throw = System.throw
local div = System.div
local Type = System.Type
local typeof = System.typeof
local getClass = System.getClass
local is = System.is
local band = System.band
local arrayFromTable = System.arrayFromTable
local toLuaTable = System.toLuaTable

local Exception = System.Exception
local NotSupportedException = System.NotSupportedException
local ArgumentException = System.ArgumentException
local ArgumentNullException = System.ArgumentNullException

local assert = assert
local pairs = pairs
local getmetatable = getmetatable
local setmetatable = setmetatable
local type = type
local unpack = table.unpack

local TargetException = define("System.Reflection.TargetException", {
  __tostring = Exception.ToString,
  __inherits__ = { Exception }
})

local TargetParameterCountException = define("System.Reflection.TargetParameterCountException", {
  __tostring = Exception.ToString,
  __inherits__ = { Exception },
  __ctor__ = function(this, message, innerException) 
    Exception.__ctor__(this, message or "Parameter count mismatch.", innerException)
  end,
})

local AmbiguousMatchException = define("System.Reflection.AmbiguousMatchException", {
  __tostring = Exception.ToString,
  __inherits__ = { Exception },
  __ctor__ = function(this, message, innerException) 
    Exception.__ctor__(this, message or "Ambiguous match found.", innerException)
  end,
})

local function checkMatadata(metadata)
  if not metadata then
    throw(NotSupportedException("not found metadata for this"), 1)
  end
  return metadata
end

local function eq(left, right)
  return left[1] == right[1] and left.name == right.name
end

local function getName(this)
  return this.name
end

local MemberInfo = define("System.Reflection.MemberInfo", {
  getName = getName,
  EqualsObj = function (this, obj)
    if getmetatable(this) ~= getmetatable(obj) then
      return false
    end
    return eq(this, obj)
  end,
  getMemberType = function (this)
    return this.memberType
  end,
  getDeclaringType = function (this)
    return typeof(this[1])
  end,
  getIsStatic = function (this)
    return band(checkMatadata(this.metadata)[2], 0x8) == 1
  end,
  getIsPublic = function (this)
    return band(checkMatadata(this.metadata)[2], 0x7) == 6
  end,
  getIsPrivate = function (this)
    return band(checkMatadata(this.metadata)[2], 0x7) == 1
  end
})

local function getFieldOrPropertyType(this)
  return typeof(checkMatadata(this.metadata)[3])
end

local function checkObj(obj, cls)
  if not is(obj, cls) then
    throw(ArgumentException("Object does not match target type.", "obj"), 1)
  end
end

local function checkTarget(this, obj, metadata)
  if band(metadata[2], 0x8) == 0 then
    if obj == nil then
      throw(TargetException())
    end
    checkObj(obj, this[1])
  else
    return true
  end
end

local function checkValue(value, valueClass)
  if value == nil then
    if valueClass.class == "S" then
      value = valueClass:default()
    end
  else
    checkObj(value, valueClass)
  end
  return value
end

local function getOrSetField(this, obj, isSet, value)
  local metadata = this.metadata
  if metadata then
    if checkTarget(this, obj, metadata) then
      obj = this[1]
    end
    local name = metadata[4]
    if type(name) ~= "string" then
      name = this.name
    end
    if isSet then
      obj[name] = checkValue(value, metadata[3])
    else
      return obj[name]
    end
  else
    if obj ~= nil then
      checkObj(obj, this[1])
    else
      obj = this[1]
    end
    if isSet then
      obj[this.name] = value
    else
      return obj[this.name]
    end
  end
end

local function isMetadataDefined(metadata, index, attributeType)
  attributeType = attributeType[1]
  for i = index, #metadata do
    if is(metadata[i], attributeType) then
      return true
    end
  end
  return false
end

local function fillMetadataCustomAttributes(t, metadata, index, attributeType)
  local count = #t + 1
  if attributeType then
    attributeType = attributeType[1]
    for i = index, #metadata do
      if is(metadata[i], attributeType) then
        t[count] = metadata[i]
        count = count + 1
      end
    end
  else
    for i = index, #metadata do
      t[count] = metadata[i]
      count = count + 1
    end
  end
end

local FieldInfo = define("System.Reflection.FieldInfo", {
  __eq = eq,
  __inherits__ = { MemberInfo },
  memberType = 4,
  getFieldType = getFieldOrPropertyType,
  GetValue = getOrSetField,
  SetValue = function (this, obj, value)
    getOrSetField(this, obj, true, value)
  end,
  IsDefined = function (this, attributeType)
    if attributeType == nil then throw(ArgumentNullException()) end
    local metadata = this.metadata
    if metadata then
      return isMetadataDefined(metadata, 4, attributeType)
    end
    return false
  end,
  GetCustomAttributes = function (this, attributeType, inherit)
    if type(attributeType) == "boolean" then
      attributeType, inherit = nil, attributeType
    else
      if attributeType == nil then throw(ArgumentNullException()) end
    end
    local t = {}
    local metadata = this.metadata
    if metadata then
      local index = 4
      if type(metadata[index]) == "string" then
        index = 5
      end
      return fillMetadataCustomAttributes(t, metadata, index, attributeType)
    end
    return arrayFromTable(t, System.Attribute) 
  end
})

local function getOrSetProperty(this, obj, isSet, value)
  local metadata = this.metadata
  if metadata then
    local isStatic
    if checkTarget(this, obj, metadata) then
      obj = this[1]
      isStatic = true
    end
    if isSet then
      value = checkValue(value, metadata[3])
    end
    local kind = band(metadata[2], 0x300)
    if kind == 0 then
      local name = metadata[4]
      if type(name) ~= "string" then
        name = this.name
      end
      if isSet then
        obj[name] = value
      else
        return obj[name]
      end
    else
      local index
      if kind == 0x100 then
        index = isSet and 5 or 4      
      elseif kind == 0x200 then
        if isSet then
          throw(ArgumentException("Property Set method was not found."))
        end
        index = 4
      else
        if not isSet then
          throw(ArgumentException("Property Get method was not found."))
        end  
        index = 4
      end
      local fn = metadata[index]
      if type(fn) == "table" then
        fn = fn[1]
      end
      if isSet then
        if isStatic then
          fn(value)
        else
          fn(obj, value)
        end  
      else
        return fn(obj)
      end
    end
  else
    local isStatic
    if obj ~= nil then
      checkObj(obj, this[1])
    else
      obj = this[1]
      isStatic = true
    end
    if this.isField then
      if isSet then
        obj[this.name] = value
      else
        return obj[this.name]
      end
    else
      if isSet then
        local fn = obj["set" .. this.name]
        if fn == nil then
          throw(ArgumentException("Property Set method not found."))
        end
        if isStatic then
          fn(value)
        else
          fn(obj, value)
        end
      else
        local fn = obj["get" .. this.name]
        if fn == nil then
          throw(ArgumentException("Property Get method not found."))
        end
        return fn(obj)
      end
    end
  end
end

local function getPropertyAttributesIndex(metadata)
  local kind = band(metadata[2], 0x300)
  local index
  if kind == 0 then
    index = 4
  elseif kind == 0x100 then
    index = 6
  else
    index = 5
  end
  return index
end

local PropertyInfo = define("System.Reflection.PropertyInfo", {
  __eq = eq,
  __inherits__ = { MemberInfo },
  memberType = 16,
  getPropertyType = getFieldOrPropertyType,
  GetValue = getOrSetProperty,
  SetValue = function (this, obj, value)
    getOrSetProperty(this, obj, true, value)
  end,
  IsDefined = function (this, attributeType)
    if attributeType == nil then throw(ArgumentNullException()) end
    local metadata = this.metadata
    if metadata then
      local index = getPropertyAttributesIndex(metadata)
      return isMetadataDefined(metadata, index, attributeType)
    end
    return false
  end,
  GetCustomAttributes = function (this, attributeType, inherit)
    if type(attributeType) == "boolean" then
      attributeType, inherit = nil, attributeType
    else
      if attributeType == nil then throw(ArgumentNullException()) end
    end
    local t = {}
    local metadata = this.metadata
    if metadata then
      local index = getPropertyAttributesIndex(metadata)
      return fillMetadataCustomAttributes(t, metadata, index, attributeType)
    end
    return arrayFromTable(t, System.Attribute) 
  end
})

local function getMethodAttributesIndex(metadata)
  local flags = metadata[2]
  local index
  local typeParametersCount = band(flags, 0xC00)
  if typeParametersCount == 0 then
    local parameterCount = band(flags, 0x300)
    if band(flags, 0x80) == 0 then
      index = 4 + parameterCount
    else
      index = 5 + parameterCount
    end
  else 
    index = 5
  end
  return index
end

local MethodInfo = define("System.Reflection.MethodInfo", {
  __eq = eq,
  __inherits__ = { MemberInfo },
  memberType = 8,
  getReturnType = function (this)
    local metadata = checkMatadata(this.metadata)
    local flags = metadata[2]
    if band(flags, 0x80) == 0 then
      return Type.Void
    end
    if band(flags, 0xC00) > 0 then
      assert(false, "not implement for generic method")
    end
    local parameterCount = band(flags, 0x300)
    return typeof(metadata[4 + parameterCount])
  end,
  Invoke = function (this, obj, parameters)
    local metadata = this.metadata
    if metadata then
      local isStatic
      if checkTarget(this, obj, metadata) then
        isStatic = true
      end
      local t
      local parameterCount = band(metadata[2], 0x300)
      if parameterCount == 0 then
        if parameters ~= nil and #parameters > 0 then
          throw(TargetParameterCountException())
        end
      else
        if parameters == nil and #parameters ~= parameterCount then
          throw(TargetParameterCountException())
        end
        for i = 4, 3 + parameterCount do
          local j = #t
          t[j + 1] = checkValue(parameters:get(j), metadata[i])
        end
      end
      local f = metadata[3]
      if isStatic then
        if t then
          return f(unpack(t, 1, parameterCount))
        else
          return f()
        end
      else
        if t then
          return f(obj, unpack(t, 1, parameterCount))
        else
          return f(obj)
        end
      end
    else
      local f = assert(this.f)
      if obj ~= nil then
        checkObj(obj, this[1])
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
  end,
  IsDefined = function (this, attributeType, inherit)
    if attributeType == nil then throw(ArgumentNullException()) end
    local metadata = this.metadata
    if metadata then
      local index = getMethodAttributesIndex(metadata)
      return isMetadataDefined(metadata, index, attributeType)
    end
    return false
  end,
  GetCustomAttributes = function (this, attributeType, inherit)
    if type(attributeType) == "boolean" then
      attributeType, inherit = nil, attributeType
    else
      if attributeType == nil then throw(ArgumentNullException()) end
    end
    local t = {}
    local metadata = this.metadata
    if metadata then
      local index = getMethodAttributesIndex(metadata)
      return fillMetadataCustomAttributes(t, metadata, index, attributeType)
    end
    return arrayFromTable(t, System.Attribute)
  end
})

local function buildFieldInfo(cls, name, metadata)
  return setmetatable({ c = cls, name = name, metadata = metadata }, FieldInfo)
end

local function buildPropertyInfo(cls, name, metadata, isField)
  return setmetatable({ c = cls, name = name, metadata = metadata, isField = isField }, PropertyInfo)
end

local function buildMethodInfo(cls, name, metadata, f)
  return setmetatable({ c = cls, name = name, metadata = metadata, f = f }, MethodInfo)
end

-- https://en.cppreference.com/w/cpp/algorithm/lower_bound
local function lowerBound(t, first, last, value, comp)
  local count = last - first
  local it, step
  while count > 0 do
    it = first
    step = div(count, 2)
    it = it + step
    if comp(t[it], value) then
      it = it + 1
      first = it
      count = count - (step + 1)
    else
      count = step
    end
  end
  return first
end

local function metadataItemCompByName(item, name)
  return item[1] < name
end

local function binarySearchByName(metadata, name)
  local last = #metadata + 1
  local index = lowerBound(metadata, 1, last, name, metadataItemCompByName)
  if index ~= last then
    return metadata[index], index
  end
  return nil
end

function Type.GetField(this, name)
  if name == nil then throw(ArgumentNullException()) end
  local cls = this[1]
  local metadata = cls.__metadata__
  if metadata then
    local fields = metadata.fields
    if fields then
      local field = binarySearchByName(fields, name)
      if field then
        return buildFieldInfo(cls, name, field)
      end
      return nil
    end
  end
  if type(cls[name]) ~= "function" then
    return buildFieldInfo(cls, name)
  end
end

function Type.GetFields(this)
  local t = {}
  local cls = this[1]
  local count = 1
  repeat
    local metadata = cls.__metadata__
    if metadata then
      local fields = metadata.fields
      if fields then
        for i = 1, #fields do
          local field = fields[i]
          t[count] = buildFieldInfo(cls, field[1], field)
          count = count + 1
        end
      else
        metadata = nil
      end
    end
    if not metadata then
      for k, v in pairs(cls) do
        if type(v) ~= "function" then
          t[count] = buildFieldInfo(cls, k)
          count = count + 1
        end
      end
    end
    cls = getmetatable(cls)
  until cls == nil 
  return arrayFromTable(t, FieldInfo)
end

function Type.GetProperty(this, name)
  if name == nil then throw(ArgumentNullException()) end
  local cls = this[1]
  local metadata = cls.__metadata__
  if metadata then
    local properties = metadata.properties
    if properties then
      local property = binarySearchByName(properties, name)
      if property then
        return buildPropertyInfo(cls, name, property)
      end
      return nil
    end
  end
  if cls["get" .. name] or cls["set" .. name] then
    return buildPropertyInfo(cls, name)
  else
    return buildPropertyInfo(cls, name, nil, true)
  end
end

function Type.GetProperties(this)
  local t = {}
  local cls = this[1]
  local count = 1
  repeat
    local metadata = cls.__metadata__
    if metadata then
      local properties = metadata.properties
      if properties then
        for i = 1, #properties do
          local property = properties[i]
          t[count] = buildPropertyInfo(cls, property[1], property)
          count = count + 1
        end
      end
    end
    cls = getmetatable(cls)
  until cls == nil 
  return arrayFromTable(t, PropertyInfo)
end

function Type.GetMethod(this, name)
  if name == nil then throw(ArgumentNullException()) end
  local cls = this[1]
  local metadata = cls.__metadata__
  if metadata then
    local methods = metadata.methods
    if methods then
      local item, index = binarySearchByName(methods, name)
      if item then
        local next = methods[index + 1]
        if next and next[1] == name then
          throw(AmbiguousMatchException())
        end
        return buildMethodInfo(cls, name, item)
      end
      return nil
    end
  end
  local f = cls[name]
  if type(f) == "function" then
    return buildMethodInfo(cls, name, nil, f)
  end
end

function Type.GetMethods(this)
  local t = {}
  local cls = this[1]
  local count = 1
  repeat
    local metadata = cls.__metadata__
    if metadata then
      local methods = metadata.methods
      if methods then
        for i = 1, #methods do
          local method = methods[i]
          t[count] = buildMethodInfo(cls, method[1], method)
          count = count + 1
        end
      else
        metadata = nil
      end
    end
    if not metadata then
      for k, v in pairs(cls) do
        if type(v) == "function" then
          t[count] = buildMethodInfo(cls, k, nil, v)
          count = count + 1
        end
      end
    end
    cls = getmetatable(cls)
  until cls == nil 
  return arrayFromTable(t, MethodInfo)
end

function Type.IsDefined(this, attributeType, inherit)
  if attributeType == nil then throw(ArgumentNullException()) end
  if not inherit then
    local metadata = this[1].__metadata__
    if metadata then
      local class  = metadata.class
      if class then
        return isMetadataDefined(class, 2, attributeType)
      end
    end
    return false
  else
    local cls = this[1]
    repeat
      local metadata = cls.__metadata__
      if metadata then
        local class  = metadata.class
        if class then
          if isMetadataDefined(class, 2, attributeType) then
            return true
          end
        end
      end
      cls = getmetatable(cls)
    until cls == nil
    return false
  end
end

function Type.GetCustomAttributes(this, attributeType, inherit)
  if type(attributeType) == "boolean" then
    attributeType, inherit = nil, attributeType
  else
    if attributeType == nil then throw(ArgumentNullException()) end
  end
  local t = {}
  if not inherit then
    local metadata = this[1].__metadata__
    if metadata then
      local class  = metadata.class
      if class then
        fillMetadataCustomAttributes(t, class, 2, attributeType)
      end
    end
  else
    local cls = this[1]
    repeat
      local metadata = cls.__metadata__
      if metadata then
        local class  = metadata.class
        if class then
          fillMetadataCustomAttributes(t, class, 2, attributeType)
        end
      end
      cls = getmetatable(cls)
    until cls == nil
  end
  return arrayFromTable(t, System.Attribute)
end

local assembly
local function getAssembly()
  return assembly
end

local function newMemberInfo(cls, name, metadata, T)
  return setmetatable({ c = cls, name = name, metadata = metadata }, T)
end

local Assembly = define("System.Reflection.Assembly", {
  GetName = getName,
  getFullName = getName,
  GetAssembly = getAssembly,
  GetCallingAssembly = getAssembly,
  GetEntryAssembly = getAssembly,
  GetExecutingAssembly = getAssembly,
  GetTypeFrom = Type.GetTypeFrom,
  getEntryPoint = function ()
    local entryPoint = System.entryPoint
    if entryPoint ~= nil then
      local _, _, t, name = entryPoint:find("(.*)%.(.*)")
      local cls = getClass(t)
      local f = assert(cls[name])
      return buildMethodInfo(cls, name, nil, f)
    end
  end,
  GetExportedTypes = function (this)
    if this.exportedTypes then
      return this.exportedTypes
    end
    local classes = System.classes
    local t = {}
    local count = 1
    for i = 1, #classes do
      t[count] = typeof(classes[i])
      count = count + 1
    end
    local array = arrayFromTable(t, Type, true)
    this.exportedTypes = array
    return array
  end
})

assembly = Assembly()
assembly.name = System.config.assemblyName or "CSharp.lua, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

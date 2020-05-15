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
local null = System.null
local falseFn = System.falseFn
local each = System.each
local Array = System.Array
local checkIndexAndCount = System.checkIndexAndCount
local throwFailedVersion = System.throwFailedVersion
local ArgumentNullException = System.ArgumentNullException
local ArgumentException = System.ArgumentException
local KeyNotFoundException = System.KeyNotFoundException
local EqualityComparer = System.EqualityComparer
local NotSupportedException = System.NotSupportedException

local assert = assert
local pairs = pairs
local next = next
local select = select
local getmetatable = getmetatable
local setmetatable = setmetatable
local tconcat = table.concat
local type = type

local counts = setmetatable({}, { __mode = "k" })
System.counts = counts

local function buildFromDictionary(this, dictionary, comparer)
  if comparer ~= nil then throw(NotSupportedException()) end
  if dictionary == nil then throw(ArgumentNullException("dictionary")) end
  local count = 0
  for k, v in pairs(dictionary) do
    this[k] = v
    count = count + 1
  end
  counts[this] = { count, 0 }
end

local function getCount(this)
  local t = counts[this]
  if t then
    return t[1]
  end
  return 0
end

local function pairsFn(t, i)
  local count =  counts[t]
  if count then
    if count[2] ~= count[3] then
      throwFailedVersion()
    end
  end
  local k, v = next(t, i)
  if v == null then
    return k
  end
  return k, v
end

function System.pairs(t)
  local count = counts[t]
  if count then
    count[3] = count[2]
  end
  return pairsFn, t
end

local KeyValuePairFn
local KeyValuePair = {
  __ctor__ = function (this, ...)
    if select("#", ...) == 0 then
      this.Key, this.Value = this.__genericTKey__:default(), this.__genericTValue__:default()
    else
      this.Key, this.Value = ...
    end
  end,
  Create = function (key, value, TKey, TValue)
    return setmetatable({ Key = key, Value = value }, KeyValuePairFn(TKey, TValue))
  end,
  Deconstruct = function (this)
    return this.Key, this.Value
  end,
  ToString = function (this)
    local t = { "[" }
    local count = 2
    local k, v = this.Key, this.Value
    if k ~= nil then
      t[count] = k:ToString()
      count = count + 1
    end
    t[count] = ", "
    count = count + 1
    if v ~= nil then
      t[count] = v:ToString()
      count = count + 1
    end
    t[count] = "]"
    return tconcat(t)
  end
}

KeyValuePairFn = System.defStc("System.Collections.Generic.KeyValuePair", function(TKey, TValue)
  local cls = {
    __genericTKey__ = TKey,
    __genericTValue__ = TValue,
  }
  return cls
end, KeyValuePair)
System.KeyValuePair = KeyValuePairFn

local function isKeyValuePair(t)
  return getmetatable(getmetatable(t)) == KeyValuePair
end

local DictionaryEnumerator = define("System.Collections.Generic.DictionaryEnumerator", {
  getCurrent = System.getCurrent, 
  Dispose = System.emptyFn,
  MoveNext = function (this)
    local t, kind = this.dict, this.kind
    local count = counts[t]
    if this.version ~= (count and count[2] or 0) then
      throwFailedVersion()
    end
    local k, v = next(t, this.index)
    if k ~= nil then
      if kind then
        kind.Key = k
        if v == null then v = nil end
        kind.Value = v
      elseif kind == false then
        if v == null then v = nil end
        this.current = v
      else
        this.current = k
      end
      this.index = k
      return true
    else
      if kind then
        kind.Key, kind.Value = kind.__genericTKey__:default(), kind.__genericTValue__:default()
      elseif kind == false then
        this.current = t.__genericTValue__:default()
      else
        this.current = t.__genericTKey__:default()
      end
      return false
    end
  end
})

local function dictionaryEnumerator(t, kind)
  local current
  if not kind then
    local TKey, TValue = t.__genericTKey__, t.__genericTValue__
    kind = setmetatable({ Key = TKey:default(), Value = TValue:default() }, KeyValuePairFn(TKey, TValue))
    current = kind
  elseif kind == 1 then
    local TKey = t.__genericTKey__
    current = TKey:default()
    kind = nil
  else
    local TValue = t.__genericTValue__
    current = TValue:default()
    kind = false
  end
  local count = counts[t]
  local en = {
    dict = t,
    version = count and count[2] or 0,
    kind = kind,
    current = current
  }
  return setmetatable(en, DictionaryEnumerator)
end

local DictionaryCollection = define("System.Collections.Generic.DictionaryCollection", function (T)
    return {
      base = { System.ICollection_1(T), System.IReadOnlyCollection_1(T), System.ICollection },
      __genericT__ = T
    }
  end, {
  __ctor__ = function (this, dict, kind, T)
    this.dict = dict
    this.kind = kind
  end,
  getCount = function (this)
    return getCount(this.dict)
  end,
  GetEnumerator = function (this)
    return dictionaryEnumerator(this.dict, this.kind)
  end
})

local function add(this, key, value)
  if key == nil then throw(ArgumentNullException("key")) end
  if this[key] ~= nil then throw(ArgumentException("key already exists")) end
  this[key] = value == nil and null or value
  local t = counts[this]
  if t then
    t[1] = t[1] + 1
    t[2] = t[2] + 1
  else
    counts[this] = { 1, 1 }
  end
end

local function remove(this, key)
  if key == nil then throw(ArgumentNullException("key")) end
  if this[key] ~= nil then
    this[key] = nil
    local t = counts[this]
    t[1] = t[1] - 1
    t[2] = t[2] + 1
    return true
  end
  return false
end

local Dictionary = {
  getIsFixedSize = falseFn,
  getIsReadOnly = falseFn,
  __ctor__ = function (this, ...) 
    local n = select("#", ...)
    if n == 0 then
    elseif n == 1 then
      local comparer = ...
      if comparer == nil or type(comparer) == "number" then  
      else
        local getHashCode = comparer.getHashCode
        if getHashCode == nil then
          buildFromDictionary(this, comparer)
        else
          throw(NotSupportedException())
        end
      end
    else
      local dictionary, comparer = ...
      if type(dictionary) == "number" then 
        if comparer ~= nil then throw(NotSupportedException()) end
      else
        buildFromDictionary(this, dictionary, comparer)
      end
    end
  end,
  AddKeyValue = add,
  Add = function (this, ...)
    local k, v
    if select("#", ...) == 1 then
      local pair = ... 
      k, v = pair.Key, pair.Value
    else
      k, v = ...
    end
    add(this, k ,v)
  end,
  Clear = function (this)
    for k, v in pairs(this) do
      this[k] = nil
    end
    counts[this] = nil
  end,
  ContainsKey = function (this, key)
    if key == nil then throw(ArgumentNullException("key")) end
    return this[key] ~= nil 
  end,
  ContainsValue = function (this, value)
    if value == nil then
      for _, v in pairs(this) do
        if v == null then
          return true
        end
      end
    else
      local comparer = EqualityComparer(this.__genericTValue__).getDefault()
      local equals = comparer.EqualsOf
        for _, v in pairs(this) do
          if v ~= null then
            if equals(comparer, value, v ) then
              return true
            end
          end
      end
    end
    return false
  end,
  Contains = function (this, pair)
    local key = pair.Key
    if key == nil then throw(ArgumentNullException("key")) end
    local value = this[key]
    if value ~= nil then
      if value == null then value = nil end
      local comparer = EqualityComparer(this.__genericTValue__).getDefault()
      if comparer:EqualsOf(value, pair.Value) then
        return true
      end
    end
    return false
  end,
  CopyTo = function (this, array, index)
    local count = getCount(this)
    checkIndexAndCount(array, index, count)
    if count > 0 then
      local KeyValuePair = KeyValuePairFn(this.__genericTKey__, this.__genericTValue__)
      index = index + 1
      for k, v in pairs(this) do
        if v == null then v = nil end
        array[index] = setmetatable({ Key = k, Value = v }, KeyValuePair)
        index = index + 1
      end
    end
  end,
  RemoveKey = remove,
  Remove = function (this, key)
    if isKeyValuePair(key) then
      local k, v = key.Key, key.Value
      local value = this[k]
      if value ~= nil then
        if value == null then value = nil end
        local comparer = EqualityComparer(this.__genericTValue__).getDefault()
        if comparer:EqualsOf(value, v) then
          remove(this, k)
          return true
        end
      end
      return false
    end
    return remove(this, key)
  end,
  TryGetValue = function (this, key)
    if key == nil then throw(ArgumentNullException("key")) end
    local value = this[key]
    if value == nil then
      return false, this.__genericTValue__:default()
    end
    if value == null then return true end
    return true, value
  end,
  getComparer = function (this)
    return EqualityComparer(this.__genericTKey__).getDefault()
  end,
  getCount = getCount,
  get = function (this, key)
    if key == nil then throw(ArgumentNullException("key")) end
    local value = this[key]
    if value == nil then throw(KeyNotFoundException()) end
    if value ~= null then
      return value
    end
    return nil
  end,
  set = function (this, key, value)
    if key == nil then throw(ArgumentNullException("key")) end
    local t = counts[this]
    if t then
      if this[key] == nil then
        t[1] = t[1] + 1
      end
      t[2] = t[2] + 1
    else
      counts[this] = { 1, 1 }
    end
    this[key] = value == nil and null or value
  end,
  GetEnumerator = dictionaryEnumerator,
  getKeys = function (this)
    return DictionaryCollection(this.__genericTKey__)(this, 1)
  end,
  getValues = function (this)
    return DictionaryCollection(this.__genericTValue__)(this, 2)
  end
}

local ValueKeyDictionary = (function ()
  local function buildFromDictionary(this, dictionary, comparer)
    if comparer ~= nil then throw(NotSupportedException()) end
    if dictionary == nil then throw(ArgumentNullException("dictionary")) end
    local count = 1
    for _, pair in each(dictionary) do
      local k, v = pair.Key, pair.Value
      this[count] = { k:__clone__(), v }
      count = count + 1
    end
  end 
  
  local function add(this, key, value)
    local len = #this
    for i = 1, len do
      local k = this[i][1]
      if k:EqualsObj(key) then
        throw(ArgumentException("key already exists"))
      end
    end
    this[len + 1] = { key, value }
    this.version = this.version + 1
  end
  
  return {
    version = 0,
    getIsFixedSize = falseFn,
    getIsReadOnly = falseFn,
    __ctor__ = function (this, ...)
      local n = select("#", ...)
      if n == 0 then
      elseif n == 1 then
        local comparer = ...
        if comparer == nil or type(comparer) == "number" then  
        else
          local getHashCode = comparer.getHashCode
          if getHashCode == nil then
            buildFromDictionary(this, comparer)
          else
            throw(NotSupportedException())
          end
        end
      else
        local dictionary, comparer = ...
        if type(dictionary) == "number" then 
          if comparer ~= nil then throw(NotSupportedException()) end
        else
          buildFromDictionary(this, dictionary, comparer)
        end
      end
    end,
    AddKeyValue = add,
    Add = function (this, ...)
      local k, v
      if select("#", ...) == 1 then
        local pair = ... 
        k, v = pair.Key, pair.Value
      else
        k, v = ...
      end
      add(this, k ,v)
    end,
    Clear = function (this)
      for i = 1, #this do
        this[i] = nil
      end
      this.version = this.version + 1
    end,
    ContainsKey = function (this, key)
      for i = 1, #this do
        local k = this[i][1]
        if k:EqualsObj(key) then
          return true
        end
      end
      return false
    end,
    ContainsValue = function (this, value)
      local comparer = EqualityComparer(this.__genericTValue__).getDefault()
      local equals = comparer.EqualsOf
      for i = 1, #this do
        local v = this[i][2]
        if equals(comparer, value, v ) then
          return true
        end
      end
      return false
    end,
    Contains = function (this, pair)
      for i = 1, #this do
        local t = this[i]
        if t[1]:EqualsObj(pair.Key) then
          local comparer = EqualityComparer(this.__genericTValue__).getDefault()
          if comparer:EqualsOf(t[2], pair.Value) then
            return true
          end 
        end
      end
      return false
    end,
    CopyTo = function (this, array, index)
      local count = #this
      checkIndexAndCount(array, index, count)
      if count > 0 then
        local KeyValuePair = KeyValuePairFn(this.__genericTKey__, this.__genericTValue__)
        index = index + 1
        for i = 1, count do
          local t = this[i]
          array[index] = setmetatable({ Key = t.Key:__clone__(), Value = t.Value }, KeyValuePair)
          index = index + 1
        end
      end
    end,
  }
end)()

function System.dictionaryFromTable(t, TKey, TValue)
  return setmetatable(t, Dictionary(TKey, TValue))
end

function System.isDictLike(t)
  return type(t) == "table" and t.GetEnumerator == dictionaryEnumerator
end

local DictionaryFn = define("System.Collections.Generic.Dictionary", function(TKey, TValue) 
  return { 
    base = { System.IDictionary_2(TKey, TValue), System.IDictionary, System.IReadOnlyDictionary_2(TKey, TValue) },
    __genericT__ = KeyValuePairFn(TKey, TValue),
    __genericTKey__ = TKey,
    __genericTValue__ = TValue,
    __len = getCount
  }
end, Dictionary)

System.Dictionary = DictionaryFn

local Object = System.Object
System.Hashtable = DictionaryFn(Object, Object)

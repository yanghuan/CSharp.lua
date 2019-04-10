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
local throwFailedVersion = System.throwFailedVersion
local ArgumentNullException = System.ArgumentNullException
local ArgumentException = System.ArgumentException
local KeyNotFoundException = System.KeyNotFoundException
local EqualityComparer_1 = System.EqualityComparer_1

local assert = assert
local pairs = pairs
local next = next
local select = select
local setmetatable = setmetatable
local tconcat = table.concat

local counts = setmetatable({}, { __mode = "k" })
System.counts = counts

local function buildFromCapacity(this, capacity, comparer)
  if comparer ~= nil then
    assert(false, "no support")
  end
end

local function buildFromDictionary(this, dictionary, comparer)
  buildFromCapacity(this, 0, comparer)
  if dictionary == nil then
    throw(ArgumentNullException("dictionary"))
  end
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

KeyValuePairFn = System.defStc("System.KeyValuePair", function(TKey, TValue)
  local cls = {
    __genericTKey__ = TKey,
    __genericTValue__ = TValue,
  }
  return cls
end, KeyValuePair)

local DictionaryEnumerator = { 
  __index = false,
  getCurrent = System.getCurrent, 
  Dispose = System.emptyFn,
  MoveNext = function (this)
    local t = this.dict
    local count = counts[t]
    if this.version ~= (count and count[2] or 0) then
      throwFailedVersion()
    end
    local k, v = next(t, this.index)
    if k ~= nil then
      if not this.kind then
        local pair = this.pair
        pair.Key = k
        if v == null then
          v = nil
        end
        pair.Value = v
        this.current = pair
      elseif this.kind == 1 then
        this.current = k
      else
        if v == null then
          v = nil
        end
        this.current = v
      end
      this.index = k
      return true
    end
    this.current = nil
    return false
  end
}
DictionaryEnumerator.__index = DictionaryEnumerator

local function dictionaryEnumerator(t, kind)
  local count = counts[t]
  local en = {
    dict = t,
    version = count and count[2] or 0,
    kind = kind,
    pair = not kind and setmetatable({ Key = false, Value = false }, KeyValuePairFn(t.__genericTKey__, t.__genericTValue__)) or nil
  }
  return setmetatable(en, DictionaryEnumerator)
end

local DictionaryCollection = define("System.DictionaryCollection", {
  __ctor__ = function (this, dict, kind, T)
    this.dict = dict
    this.kind = kind
    this.__genericT__ = T
  end,
  getCount = function (this)
    return getCount(this.dict)
  end,
  GetEnumerator = function (this)
    return dictionaryEnumerator(this.dict, this.kind)
  end
})

local Dictionary = {
  __ctor__ = function (this, ...) 
    local len = select("#", ...)
    if len == 0 then
      buildFromCapacity(this, 0)
    elseif len == 1 then
      local comparer = ...
      if comparer == nil or type(comparer) == "number" then  
        buildFromCapacity(this, comparer)
      else
        local getHashCode = comparer.getHashCode
        if getHashCode == nil then
          buildFromDictionary(this, comparer)
        else
          buildFromCapacity(this, 0, comparer)
        end
      end
    else 
        local dictionary, comparer = ...
        if type(dictionary) == "number" then 
          buildFromCapacity(this, dictionary, comparer)
        else
          buildFromDictionary(this, dictionary, comparer)
        end
    end
  end,
  Add = function (this, key, value)
    if key == nil then throw(ArgumentNullException("key")) end
    if this[key] then throw(ArgumentException("key already exists")) end
    this[key] = value == nil and null or value
    local t = counts[this]
    if t then
      t[1] = t[1] + 1
      t[2] = t[2] + 1
    else
      counts[this] = { 1, 1 }
    end
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
      local equals = EqualityComparer_1(this.__genericTValue__).getDefault().Equals
        for _, v in pairs(this) do
          if v ~= null then
            if equals(value, v ) then
              return true
            end
          end
      end
    end
    return false
  end,
  Remove = function (this, key)
    if key == nil then throw(ArgumentNullException("key")) end
    if this[key] then
      this[key] = nil
      local t = counts[this]
      t[1] = t[1] - 1
      t[2] = t[2] + 1
      return true
    end
    return false
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
    return EqualityComparer_1(this.__genericTKey__).getDefault()
  end,
  getCount = getCount,
  get = function (this, key)
    if key == nil then throw(ArgumentNullException("key")) end
    local value = this[key]
    if value == nil then throw(KeyNotFoundException()) end
    if value ~= null then
      return value
    end
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
    return DictionaryCollection(this, 1, this.__genericTKey__)
  end,
  getValues = function (this)
    return DictionaryCollection(this, 2, this.__genericTValue__)
  end
}

function System.dictionaryFromTable(t, TKey, TValue)
  return setmetatable(t, Dictionary(TKey, TValue))
end

define("System.Dictionary", function(TKey, TValue) 
  return { 
    __inherits__ = { System.IDictionary_2(TKey, TValue), System.IDictionary }, 
    __genericTKey__ = TKey,
    __genericTValue__ = TValue,
    __len = getCount
  }
end, Dictionary)

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
local Collection = System.Collection
local wrap = Collection.wrap
local unWrap = Collection.unWrap
local changeVersion = Collection.changeVersion
local addCount = Collection.addCount
local clearCount = Collection.clearCount
local dictionaryEnumerator = Collection.dictionaryEnumerator
local ArgumentNullException = System.ArgumentNullException
local ArgumentException = System.ArgumentException
local KeyNotFoundException = System.KeyNotFoundException
local EqualityComparer_1 = System.EqualityComparer_1

local Dictionary = {}

local function buildFromCapacity(this, capacity, comparer)
  if comparer ~= nil then
    assert(false)
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
  addCount(this, count)
end

function Dictionary.__ctor__(this, ...) 
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
end 

local function checkKey(key)
  if key == nil then
    throw(ArgumentNullException("key"))
  end
end

function Dictionary.Add(this, key, value)
  checkKey(key)
  if this[key] then
    throw(ArgumentException("key already exists"))
  end
  this[key] = wrap(value)
  addCount(this, 1)
  changeVersion(this)
end

function Dictionary.Clear(this)
  for k, v in pairs(this) do
    this[k] = nil
  end
  clearCount(this)
  changeVersion(this)
end

function Dictionary.ContainsKey(this, key)
  checkKey(key)
  return this[key] ~= nil 
end

function Dictionary.ContainsValue(this, value)
  if value == nil then
    for _, v in pairs(this) do
      if unWrap(v) == nil then
        return true
      end
    end    
  else    
    local equals = EqualityComparer_1(this.__genericTValue__).getDefault().Equals
      for _, v in pairs(this) do
        if equals(nil, value, unWrap(v)) then
          return true
        end
    end
  end
  return false
end

function Dictionary.Remove(this, key)
  checkKey(key)
  if this[key] then
    this[key] = nil
    addCount(this, -1)
    changeVersion(this)
    return true
  end
  return false
end

local function getValueDefault(this)
  return this.__genericTValue__.__default__()
end

function Dictionary.TryGetValue(this, key)
  checkKey(key)
  local value = this[key]
  if value == nil then
    return false, getValueDefault(this)
  end
  return true, unWrap(value)
end

function Dictionary.getComparer(this)
  return EqualityComparer_1(this.__genericTKey__).getDefault()
end

Dictionary.getCount = Collection.getCount

function Dictionary.get(this, key)
  checkKey(key)
  local value = this[key]
  if value == nil then
    throw(KeyNotFoundException())
  end
  return unWrap(value) 
end

function Dictionary.set(this, key, value)
  checkKey(key)
  if this[key] == nil then
    addCount(this, 1)
  end
  this[key] = wrap(value)
  changeVersion(this)
end

function Dictionary.GetEnumerator(this)
  return dictionaryEnumerator(this, 0)
end 

local DictionaryCollection = {}

function DictionaryCollection.__ctor__(this, dict, isKey, T)
  this.dict = dict
  this.isKey = isKey
  this.__genericT__ = T
end

function DictionaryCollection.getCount(this)
  return this.dict:getCount()
end

function DictionaryCollection.GetEnumerator(this)
  return dictionaryEnumerator(this.dict, this.isKey and 1 or 2)
end

System.define("System.DictionaryCollection", DictionaryCollection)

function Dictionary.getKeys(this)
  return DictionaryCollection(this, true, this.__genericTKey__)
end

function Dictionary.getValues(this)
  return DictionaryCollection(this, false, this.__genericTValue__)
end

System.define("System.Dictionary", function(TKey, TValue) 
  local cls = { 
    __inherits__ = { System.IDictionary_2(TKey, TValue), System.IDictionary }, 
    __genericTKey__ = TKey,
    __genericTValue__ = TValue,
    __len = Dictionary.getCount
  }
  return cls
end, Dictionary)
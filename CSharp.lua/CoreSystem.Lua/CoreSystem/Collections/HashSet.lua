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
local each = Collection.each
local ArgumentNullException = System.ArgumentNullException

local HashSet = {}

local function build(this, collection, comparer)
  if comparer ~= nil then
    assert(false)
  end
  if collection == nil then
    throw(ArgumentNullException("collection"))
  end
  this:UnionWith(collection)
end

function HashSet.__ctor__(this, ...)
  local len = select("#", ...)
  if len == 0 then
  elseif len == 1 then
    local collection = ...
    if collection == nil then return end
    if collection.getEnumerator ~= nil then
      build(this, collection, nil)
    else
      assert(true)
    end
  else 
    build(this, ...)
  end
end 

function HashSet.Clear(this)
  for k, v in pairs(this) do
    this[k] = nil
  end
  clearCount(this)
  changeVersion(this)
end

function HashSet.Contains(this, item)
  item = wrap(item)
  return this[item] ~= nil
end

function HashSet.Remove(this, item)
  item = wrap(item)
  if this[item] then
    this[item] = nil
    addCount(this, -1)
    changeVersion(this)
    return true
  end
  return false
end

HashSet.getCount = Collection.getCount

function HashSet.getIsReadOnly(this)
  return false
end

function HashSet.GetEnumerator(this)
  return Collection.dictionaryEnumerator(this, 1)
end 

function HashSet.Add(this, v)
  v = wrap(v)
  if this[v] == nil then
    this[v] = true
    addCount(this, 1)
    changeVersion(this)
    return true
  end
  return false
end

function HashSet.UnionWith(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  local count = 0
  for _, v in each(collection) do
    v = wrap(v)
    if this[v] == nil then
      this[v] = true
      count = count + 1
    end
  end
  if count > 0 then
    addCount(this, count)
    changeVersion(v)
  end
end  

function HashSet.IntersectWith(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  local set = {}
  for _, v in each(other) do
    v = wrap(v)
    if this[v] ~= nil then
      set[v] = true
    end
  end
  local count = 0
  for v, _ in pairs(this) do
    if set[v] == nil then
      this[v] = nil
      count = count + 1
    end
  end
  if count > 0 then
    addCount(this, -count)
    changeVersion(this)
  end
end

function HashSet.ExceptWith(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  if other == this then
    this:clear()
    return
  end
  local count = 0
  for _, v in each(other) do
    v = wrap(v)
    if this[v] ~= nil then
      this[v] = nil
      count = count + 1
    end
  end
  if count > 0 then
    addCount(this, -count)
    changeVersion(this)
  end
end

function HashSet.SymmetricExceptWith(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  if other == this then
    this:clear()
    return
  end
  local set = {}
  local count = 0
  local changed = false
  for _, v in each(other) do
    v = wrap(v)
    if this[v] == nil then
      this[v] = true
      count = count + 1
      changed = true
      set[v] = true
    elseif set[v] == nil then 
      this[v] = nil
      count = count - 1
      changed = true
    end
  end
  if changed then
    addCount(this, count)
    changeVersion(this)
  end
end

local function checkUniqueAndUnfoundElements(this, other, returnIfUnfound)
  if #this == 0 then
    local numElementsInOther = 0
    for _, item in each(other) do
      numElementsInOther = numElementsInOther + 1
      break;
    end
    return 0, numElementsInOther
  end
  local set, uniqueCount, unfoundCount = {}, 0, 0
  for _, item in each(other) do
    item = wrap(item)
      if this[item] ~= nil then
        if set[item] == nil then
          set[item] = true
          uniqueCount = uniqueCount + 1
        end
      else
      unfoundCount = unfoundCount + 1
      if returnIfUnfound then
        break
      end
    end
  end
  return uniqueCount, unfoundCount
end

function HashSet.IsSubsetOf(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  local count = #this
  if count == 0 then
    return true
  end
  local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, false)
  return uniqueCount == count and unfoundCount >= 0
end

function HashSet.IsProperSubsetOf(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, false)
  return uniqueCount == #this and unfoundCount > 0
end

function HashSet.IsSupersetOf(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  for _, element in each(other) do
    element = wrap(element)
    if this[element] == nil then
      return false
    end
  end
  return true
end

function HashSet.IsProperSupersetOf(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  local count = #this
  if count == 0 then
    return false
  end
  local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, true)
  return uniqueCount < count and unfoundCount == 0
end

function HashSet.Overlaps(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  if #this == 0 then
    return false
  end
  for _, element in each(other) do
    element = wrap(element)
    if this[element] ~= nil then
      return true
    end
  end
  return false
end

function HashSet.SetEquals(this, other)
  if other == nil then
    throw(ArgumentNullException("other"))
  end
  local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, true)
  return uniqueCount == #this and unfoundCount == 0
end

function HashSet.RemoveWhere(this, match)
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  local numRemoved = 0
  for v, _ in pairs(this) do
    if match(unWrap(v)) then
      this[v] = nil
      numRemoved = numRemoved + 1
    end
  end
  if numRemoved > 0 then
    addCount(this, -numRemoved)
    changeVersion(this)
  end
  return numRemoved
end

HashSet.TrimExcess = System.emptyFn

System.define("System.HashSet", function(T) 
  local cls = { 
    __inherits__ = { System.ICollection_1(T), System.ISet_1(T) }, 
    __genericT__ = T,
    __len = HashSet.getCount
  }
  return cls
end, HashSet)
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
local each = System.each
local wrap = System.wrap
local unWrap = System.unWrap
local ArgumentNullException = System.ArgumentNullException

local select = select
local setmetatable = setmetatable

local function checkUniqueAndUnfoundElements(this, other, returnIfUnfound)
  if this:getCount() == 0 then
    local numElementsInOther = 0
    for _, item in each(other) do
      numElementsInOther = numElementsInOther + 1
      break
    end
    return 0, numElementsInOther
  end
  local set, uniqueCount, unfoundCount = this:newSet(), 0, 0
  for _, item in each(other) do
    if this:Contains(item) then
      if not set:Contains(item) then
        set:Add(item)
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

local HashSetEnumerator = System.define("System.Collections.Generic.HashSetEnumerator", function (T)
  return {
    __genericT__ = T,
    base = { System.IEnumerator_1(T) }
  }
end, {
  getCurrent = System.getCurrent,
  Dispose = System.emptyFn,
  MoveNext = function (this)
    if this.en:MoveNext() then
      local pair = this.en.current
      this.current = unWrap(pair[1])
      return true
    end
    this.current = this.__genericT__:default()
    return false
  end
}, 1)

local HashSet = {
  __ctor__ = function (this, ...)
    local n = select("#", ...)
    local collection, comparer
    if n == 1 then
      local c = ...
      if type(c) == "table" then
        if c.GetEnumerator then
          collection = c
        end
          comparer = c
      end
    elseif n == 2 then
      collection, comparer = ...
    end
    this.dict = System.Dictionary(this.__genericT__, System.Boolean)(comparer)
    if collection then
      this:UnionWith(collection)
    end
  end,
  newSet = function (this)
    return System.HashSet(this.__genericT__)(this.dict.comparer)
  end,
  Clear = function (this)
    this.dict:Clear()
  end,
  getCount = function (this)
    return this.dict:getCount()
  end,
  getIsReadOnly = System.falseFn,
  Contains = function (this, v)
    v = wrap(v)
    return this.dict:ContainsKey(v)
  end,
  Remove = function (this, v)
    v = wrap(v)
    return this.dict:RemoveKey(v)
  end,
  GetEnumerator = function (this)
    return setmetatable({ en = this.dict:GetEnumerator() }, HashSetEnumerator(this.__genericT__))
  end,
  Add = function (this, v)
    v = wrap(v)
    return this.dict:TryAdd(v, true)
  end,
  UnionWith = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    for _, v in each(other) do
      this:Add(v)
    end
  end,
  IntersectWith = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    if this == other or this:getCount() == 0 then
      return
    end
    local set = this:newSet()
    for _, v in each(other) do
      if this:Contains(v) then
        set:Add(v)
      end
    end
    this.dict = set.dict
  end,
  ExceptWith = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    if other == this then
      this:Clear()
      return
    end
    for _, v in each(other) do
      this:Remove(v)
    end
  end,
  SymmetricExceptWith = function (this, other)
    if other == nil then throw(ArgumentNullException("other")) end
    if this:getCount() == 0 then
      this:UnionWith(other)
      return
    end
    if other == this then
      this:Clear()
      return
    end
    for _, v in each(other) do
      if this:Contains(v) then
        this:Remove(v)
      else
        this:Add(v)
      end
    end
  end,
  IsSubsetOf = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    local count = this:getCount()
    if count == 0 then
      return true
    end
    local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, false)
    return uniqueCount == count and unfoundCount >= 0
  end,
  IsProperSubsetOf = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, false)
    return uniqueCount == #this and unfoundCount > 0
  end,
  IsSupersetOf = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    for _, element in each(other) do
      if not this:Contains(element) then
        return false
      end
    end
    return true
  end,
  IsProperSupersetOf = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    local count = this:getCount()
    if count == 0 then
      return false
    end
    local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, true)
    return uniqueCount < count and unfoundCount == 0
  end,
  Overlaps = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    if this:getCount() == 0 then
      return false
    end
    for _, element in each(other) do
      if this:Contains(element) then
        return true
      end
    end
    return false
  end,
  SetEquals = function (this, other)
    if other == nil then
      throw(ArgumentNullException("other"))
    end
    local uniqueCount, unfoundCount = checkUniqueAndUnfoundElements(this, other, true)
    return uniqueCount == #this and unfoundCount == 0
  end,
  RemoveWhere = function (this, match)
    if match == nil then
      throw(ArgumentNullException("match"))
    end
    return this.dict:removeWhere(function (k, v)
      return match(unWrap(k))
    end)
  end,
  TrimExcess = System.emptyFn
}

function System.hashSetFromTable(t, T)
  return setmetatable(t, HashSet(T))
end

System.HashSet = System.define("System.Collections.Generic.HashSet", function(T)
  return {
    base = { System.ICollection_1(T), System.ISet_1(T) },
    __genericT__ = T,
    __genericTKey__ = T,
  }
end, HashSet, 1)

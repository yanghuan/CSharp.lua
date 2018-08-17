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
local Collection = System.Collection
local buildArray = Collection.buildArray
local getArray = Collection.getArray
local setArray = Collection.setArray
local checkIndex = Collection.checkIndex 
local arrayEnumerator = Collection.arrayEnumerator

local assert = assert
local select = select

local Array = {}
local emptys = {}

function Array.__ctor__(this, ...)
  local len = select("#", ...)
  buildArray(this, len, ...)
end

function Array.new(cls, len)
  local this = setmetatable({}, cls)
  buildArray(this, len)
  return this
end

Array.set = setArray
Array.get = getArray
Array.GetEnumerator = arrayEnumerator

function Array.getLength(this)
  return #this
end

function Array.getCount(this)
  return #this
end

function Array.getRank(this)
  return 1
end

function Array.Empty(T)
  local t = emptys[T]
  if t == nil then
    t = Array(T)()
    emptys[T] = t
  end
  return t
end

Array.Exists = Collection.existsOfArray
Array.Find = Collection.findOfArray

local findAll = Collection.findAllOfArray
function Array.FindAll(t, match)
  return findAll(t, match):toArray()
end

Array.FindIndex = Collection.findIndexOfArray
Array.FindLast = Collection.findLastOfArray
Array.FindLastIndex = Collection.findLastIndexOfArray
Array.IndexOf = Collection.indexOfArray
Array.LastIndexOf = Collection.lastIndexOfArray
Array.Reverse = Collection.reverseArray
Array.Sort = Collection.sortArray
Array.TrueForAll = Collection.trueForAllOfArray
Array.Copy = Collection.copyArray

function Array.CreateInstance(elementType, length)
  return Array(elementType.c):new(length)
end

function Array.GetValue(this, index)
  return this:get(index)
end

function Array.SetValue(this, value, index)
  this:set(index, value)
end

System.define("System.Array", function(T) 
  local cls = { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T
  }
  return cls
end, Array)

function System.arrayFromTable(t, T)
  assert(T)
  return setmetatable(t, Array(T))
end

function System.arrayFromList(t)
  return setmetatable(t, Array(t.__genericT__))
end

local MultiArray = {}

function MultiArray.__ctor__(this, rank, ...)
  this.__rank__ = rank
  local length = 1
  for _, i in ipairs(rank) do
    length = length * i
  end
  buildArray(this, length, ...)
end

local function getIndex(this, ...)
  local rank = this.__rank__
  local id = 0
  local len = #rank
  for i = 1, len do
    id = id * rank[i] + select(i, ...)
  end
  return id, len
end

function MultiArray.set(this, ...)
  local index, len = getIndex(this, ...)
  setArray(this, index, select(len + 1, ...))
end

function MultiArray.get(this, ...)
  local index = getIndex(this, ...)
  return getArray(this, index)
end

function MultiArray.getLength(this, dimension)
  if dimension == nil then
    return #this
  end
  local rank = this.__rank__
  checkIndex(rank, dimension)
  return this.__rank__[dimension + 1]
end

function MultiArray.getRank(this)
  return #this.__rank__
end

MultiArray.GetEnumerator = arrayEnumerator

System.define("System.MultiArray", function(T) 
  local cls = { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T
  }
  return cls
end, MultiArray)

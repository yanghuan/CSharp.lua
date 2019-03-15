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
local Collection = System.Collection
local buildArray = Collection.buildArray
local getArray = Collection.getArray
local setArray = Collection.setArray
local arrayEnumerator = Collection.arrayEnumerator
local findAll = Collection.findAllOfArray

local IndexOutOfRangeException = System.IndexOutOfRangeException

local assert = assert
local select = select
local setmetatable = setmetatable
local type = type

local Array = {}
local emptys = {}

Array.new = buildArray
Array.set = setArray
Array.get = getArray
Array.GetEnumerator = arrayEnumerator

function Array.getLength(this)
  return #this
end

function Array.GetLength(this, dimension)
  if dimension ~= 0 then
    throw(IndexOutOfRangeException())
  end
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

function Array.FindAll(t, match)
  return setmetatable(findAll(t, match), Array(t.__genericT__))
end

Array.FindIndex = Collection.findIndexOfArray
Array.FindLast = Collection.findLastOfArray
Array.FindLastIndex = Collection.findLastIndexOfArray
Array.IndexOf = Collection.indexOfArray
Array.LastIndexOf = Collection.lastIndexOfArray
Array.Resize = Collection.resizeArray
Array.Reverse = Collection.reverseArray
Array.Sort = Collection.sortArray
Array.TrueForAll = Collection.trueForAllOfArray
Array.Copy = Collection.copyArray
Array.ForEach = Collection.forEachArray
Array.GetValue = getArray

function Array.CreateInstance(elementType, length)
  return buildArray(Array(elementType.c), length)
end

function Array.SetValue(this, value, index)
  setArray(this, index, value)
end

define("System.Array", function(T) 
  local cls = { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T
  }
  return cls
end, Array)

function Array.__call(T, ...)
  return buildArray(T, select("#", ...), { ... })
end

local function unset()
  throw(System.NotSupportedException("This array is readOnly"))
end

function System.arrayFromTable(t, T, readOnly)
  assert(T)
  local array = setmetatable(t, Array(T))
  if readOnly then
    array.set = unset
  end
  return array
end

function System.arrayFromList(t)
  return setmetatable(t, Array(t.__genericT__))
end

local MultiArray = {}

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

function MultiArray.getLength(this)
	return #this
end

function MultiArray.GetLength(this, dimension)
  local rank = this.__rank__
  if dimension < 0 or dimension >= #rank then
    throw(IndexOutOfRangeException())
  end
  return rank[dimension + 1]
end

function MultiArray.getRank(this)
  return #this.__rank__
end

MultiArray.GetEnumerator = arrayEnumerator

function System.multiArrayFromTable(t, T)
  assert(T)
  return setmetatable(t, MultiArray(T))
end

define("System.MultiArray", function(T) 
  local cls = { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T
  }
  return cls
end, MultiArray)

function MultiArray.__call(T, rank, t)
  local len = 1
  for i = 1, #rank do
    len = len * rank[i]
  end
  t = buildArray(T, len, t)
  t.__rank__ = rank
  return t
end

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
local Collection = System.Collection
local arrayEnumerator = Collection.arrayEnumerator
local findAll = Collection.findAllOfArray

local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local IndexOutOfRangeException = System.IndexOutOfRangeException

local assert = assert
local select = select
local setmetatable = setmetatable
local type = type
local tremove = table.remove

local function getLength(this)
  return #this
end

local emptys = {}

local function get(t, index)
  if index < 0 or index >= #t then
    throw(ArgumentOutOfRangeException("index"))
  end
  local v = t[index + 1]
  if v == null then 
    return nil 
  end
  return v
end

local function set(t, index, v)
  if index < 0 or index >= #t then
    throw(ArgumentOutOfRangeException("index"))
  end
  t[index + 1] = v == nil and null or v
  t.version = t.version + 1
end

local function push(t, v)
  t[#t + 1] = v == nil and null or v
  t.version = t.version + 1
end

local function clear(t)
  local size = #t
  if size > 0 then
    for i = 1, size do
      t[i] = nil
    end
    t.version = t.version + 1
  end
end

local function removeAt(t, index)
  if index < 0 or index >= #t then
    throw(ArgumentOutOfRangeException("index"))
  end
  tremove(t, index + 1)
  t.version = t.version + 1
end

local function buildArray(T, len, t)
  if t == nil then 
    t = {}
    if len > 0 then
      local default = T.__genericT__:default()
      if default == nil then
        default = null
      end
      for i = 1, len do
        t[i] = default
      end
    end
  else
    if len > 0 then
      local default = T.__genericT__:default()
      if default == nil then
        for i = 1, len do
          if t[i] == nil then
            t[i] = null
          end
        end
      end
    end
  end
  return setmetatable(t, T)
end

local Array
Array = { 
  version = 0,
  new = buildArray,
  set = set,
  get = get,
  push = push,
  clear = clear,
  removeAt = removeAt,
  GetEnumerator = arrayEnumerator,
  getLength = getLength,
  getCount = getLength,
  Exists = Collection.existsOfArray,
  Find = Collection.findOfArray,
  FindIndex = Collection.findIndexOfArray,
  FindLast = Collection.findLastOfArray,
  FindLastIndex = Collection.findLastIndexOfArray,
  IndexOf = Collection.indexOfArray,
  LastIndexOf = Collection.lastIndexOfArray,
  Resize = Collection.resizeArray,
  Reverse = Collection.reverseArray,
  Sort = Collection.sortArray,
  TrueForAll = Collection.trueForAllOfArray,
  Copy = Collection.copyArray,
  ForEach = Collection.forEachArray,
  GetValue = get,
  GetLength = function (this, dimension)
    if dimension ~= 0 then
      throw(IndexOutOfRangeException())
    end
    return #this
  end,
  getRank = function (this)
    return 1
  end,
  Empty = function (T)
    local t = emptys[T]
    if t == nil then
      t = Array(T)()
      emptys[T] = t
    end
    return t
  end,
  FindAll = function (t, match)
    return setmetatable(findAll(t, match), Array(t.__genericT__))
  end,
  CreateInstance = function (elementType, length)
    return buildArray(Array(elementType.c), length)
  end,
  SetValue = function (this, value, index)
    set(this, index, value)
  end,
}

define("System.Array", function(T) 
  return { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T
  }
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

local function getIndex(this, ...)
  local rank = this.__rank__
  local id = 0
  local len = #rank
  for i = 1, len do
    id = id * rank[i] + select(i, ...)
  end
  return id, len
end

local MultiArray = { 
  version = 0,
  getLength = getLength,
  GetEnumerator = arrayEnumerator,
  set = function (this, ...)
    local index, len = getIndex(this, ...)
    set(this, index, select(len + 1, ...))
  end,
  get = function (this, ...)
    local index = getIndex(this, ...)
    return get(this, index)
  end,
  getRank = function (this)
    return #this.__rank__
  end,
  GetLength = function (this, dimension)
    local rank = this.__rank__
    if dimension < 0 or dimension >= #rank then
      throw(IndexOutOfRangeException())
    end
    return rank[dimension + 1]
  end,
}

function System.multiArrayFromTable(t, T)
  assert(T)
  return setmetatable(t, MultiArray(T))
end

define("System.MultiArray", function(T) 
  return { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T
  }
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

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
local foreach = System.foreach
local Collection = System.Collection
local unWrap = Collection.unWrap
local checkIndexAndCount = Collection.checkIndexAndCount
local copyArray = Collection.copyArray
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException

local select = select

local List = {}

function List.__ctor__(this, ...)
  local len = select("#", ...)
  if len == 0 then return end
  local collection = ...
  if type(collection) == "number" then 
    return 
  end
  this:AddRange(collection)
end

function List.getCapacity(this)
  return #this
end

function List.getCount(this)
  return #this
end

function List.getIsFixedSize(this)
  return false
end

function List.getIsReadOnly(this)
  return false
end

List.get = Collection.getArray
List.set = Collection.setArray
List.Add = Collection.pushArray

function List.AddRange(this, collection)
  this:InsertRange(#this, collection)
end

List.BinarySearch = Collection.binarySearchArray
List.Clear = Collection.removeArrayAll
List.Contains = Collection.contains

function List.CopyTo(this, ...)
  local len = select("#", ...)
  if len == 1 then
    local array = ...
    copyArray(this, array, #this)
  else
    copyArray(this, ...)
  end
end 

List.Exists = Collection.existsOfArray
List.Find = Collection.findOfArray
List.FindAll = Collection.findAllOfArray
List.FindIndex = Collection.findIndexOfArray
List.FindLast = Collection.findLastOfArray
List.FindLastIndex = Collection.findLastIndexOfArray
List.ForEach = Collection.forEachArray
List.GetEnumerator = Collection.arrayEnumerator

function List.GetRange(this, index, count)
  checkIndexAndCount(this, index, count)
  local list = System.List(this.__genericT__)()
  copyArray(this, index, list, 0, count, true)
  return list
end

local indexOf = Collection.indexOfArray
local removeAt = Collection.removeAtArray
local removeArray = Collection.removeArray

List.IndexOf = indexOf
List.Insert = Collection.insertArray
List.InsertRange = Collection.insertRangeArray
List.LastIndexOf = Collection.lastIndexOfArray

function List.Remove(this, item)
  local index = indexOf(this, item)
  if index >= 0 then
    removeAt(this, index)
    return true
  end
  return false
end

function List.RemoveAll(this, match)
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  local size = #this
  local freeIndex = 1
  while freeIndex <= size and (not match(unWrap(this[freeIndex]))) do freeIndex = freeIndex + 1 end
  if freeIndex > size then return 0 end

  local current = freeIndex + 1
  while current <= size do 
    while current <= size and match(unWrap(this[current])) do current = current + 1 end
    if current <= size then
      this[freeIndex] = this[current]
      freeIndex = freeIndex + 1
      current = current + 1
    end
  end
  freeIndex = freeIndex -1
  local count = size - freeIndex
  removeArray(this, freeIndex, count)
  return count
end

List.RemoveAt = removeAt
List.RemoveRange = removeArray
List.Reverse = Collection.reverseArray
List.Sort = Collection.sortArray
List.TrimExcess = System.emptyFn
List.ToArray = Collection.toArray
List.TrueForAll = Collection.trueForAllOfArray

System.define("System.List", function(T) 
  local cls = { 
    __inherits__ = { System.IList_1(T), System.IList }, 
    __genericT__ = T,
  }
  return cls
end, List)
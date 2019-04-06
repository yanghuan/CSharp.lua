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
local Array = System.Array
local get = Array.get
local removeAt = Array.removeAt
local Collection = System.Collection
local insertRangeArray = Collection.insertRangeArray

local select = select

local Stack = {}

function Stack.__ctor__(this, ...)
  local len = select("#", ...)
  if len == 0 then return end
  local collection = ...
  if type(collection) == "number" then return end
  insertRangeArray(this, 0, collection)
end

Stack.getCount = Array.getLength
Stack.Clear = Array.clear
Stack.Push = Array.push
Stack.Contains = Collection.contains

local function peek(t)
  local n = #t
  if n == 0 then
    throw(InvalidOperationException())
  end
  return get(t, n - 1)
end

Stack.Peek = peek

function Stack.Pop(this)
  local v = peek(this)
  removeAt(this, #this - 1)
  return v
end

function System.stackFromTable(t, T)
  assert(T)
  return setmetatable(t, Stack(T))
end

System.define("System.Stack", function(T) 
  local cls = {
    __inherits__ = { System.IEnumerable_1(T), System.ICollection },
    __genericT__ = T,
  }
  return cls
end, Stack)
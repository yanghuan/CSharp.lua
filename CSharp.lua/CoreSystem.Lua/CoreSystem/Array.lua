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
local div = System.div
local null = System.null
local each = System.each
local falseFn = System.falseFn
local lengthFn = System.lengthFn

local InvalidOperationException = System.InvalidOperationException
local NullReferenceException = System.NullReferenceException
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local IndexOutOfRangeException = System.IndexOutOfRangeException
local EqualityComparer_1 = System.EqualityComparer_1
local Comparer_1 = System.Comparer_1

local assert = assert
local select = select
local setmetatable = setmetatable
local type = type
local table = table
local tinsert = table.insert
local tremove = table.remove
local tmove = table.move
local tsort = table.sort

local null = {}
local arrayEnumerator
local arrayFromTable

local function throwFailedVersion()
  throw(InvalidOperationException("Collection was modified; enumeration operation may not execute."))
end

local function checkIndex(t, index) 
  if index < 0 or index >= #t then
    throw(ArgumentOutOfRangeException("index"))
  end
end

local function checkIndexAndCount(t, index, count)
  if count < 0 or index > #t - count then
    throw(ArgumentOutOfRangeException("index or count"))
  end
end

local function wrap(v)
  if v == nil then 
    return null 
  end
  return v
end

local function unWrap(v)
  if v == null then 
    return nil 
  end
  return v
end

local function ipairs(t)
  local version = t.version
  return function (t, i)
    if version ~= t.version then
      throwFailedVersion()
    end
    local v = t[i]
    if v ~= nil then
      if v == null then
        v = nil
      end
      return i + 1, v
    end
  end, t, 1
end

local function eachFn(en)
  if en:MoveNext() then
    return true, en:getCurrent()
  end
  return nil
end

local function each(t)
  if t == nil then throw(NullReferenceException(), 1) end
  local getEnumerator = t.GetEnumerator
  if getEnumerator == arrayEnumerator then
    return ipairs(t)
  end
  local en = getEnumerator(t)
  return eachFn, en
end

function System.isArrayLike(t)
  return t.GetEnumerator == arrayEnumerator
end

function System.isEnumerableLike(t)
  return type(t) == "table" and t.GetEnumerator ~= nil
end

function System.toLuaTable(array)
  local t = {}
  for i = 1, #array do
    local item = array[i]
    if item ~= null then
      t[i] = item
    end
  end   
  return t
end

System.null = null
System.Void = null
System.each = each
System.ipairs = ipairs
System.throwFailedVersion = throwFailedVersion

System.wrap = wrap
System.unWrap = unWrap
System.checkIndex = checkIndex
System.checkIndexAndCount = checkIndexAndCount

local Array
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

local function addRange(t, collection)
  if collection == nil then throw(ArgumentNullException("collection")) end
  local count = #t + 1
  for _, v in each(collection) do
    t[count] = v == nil and null or v
    count = count + 1
  end
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

local function copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length, reliable)
  if not reliable then
    if sourceArray == nil then throw(ArgumentNullException("sourceArray")) end
    if destinationArray == nil then throw(ArgumentNullException("destinationArray")) end
    checkIndexAndCount(sourceArray, sourceIndex, length)
    checkIndexAndCount(destinationArray, destinationIndex, length)
  end
  tmove(sourceArray, sourceIndex + 1, sourceIndex + length, destinationIndex + 1, destinationArray)
end

local function removeRange(t, index, count)
  if count < 0 or index > #t - count then
    throw(ArgumentOutOfRangeException("index or count"))
  end
  if count > 0 then
    local size = #t - count
    if index < size then
      copy(t, index + count, t, index, size - index)
    end
    for i = size + 1, size + count do
      t[i] = nil
    end
    t.version = t.version + 1
  end
end

local function findAll(t, match)
  if t == nil then throw(ArgumentNullException("array")) end
  if match == nil then throw(ArgumentNullException("match")) end
  local list = {}
  local count = 1
  for i = 1, #t do
    local item = t[i]
    if (item == null and match(nil)) or match(item) then
      list[count] = item
      count = count + 1
    end
  end
  return list
end

local function getComp(t, comparer)
  local compare
  if comparer == nil then
    compare = Comparer_1(t.__genericT__).getDefault().Compare 
  elseif comparer.Compare then    
    compare = comparer.Compare
  else
    compare = comparer
  end
  return function(x, y) 
    if x == null then x = nil end
    if y == null then y = nil end
    return compare(x, y) < 0
  end
end

local function sort(t, comparer)
  if #t > 1 then
    tsort(t, getComp(t, comparer))
    t.version = t.version + 1
  end
end

local ArrayEnumerator = {
  __index = false,
  getCurrent = System.getCurrent, 
  Dispose = System.emptyFn,
  Reset = function (this)
    this.index = 1
    this.current = nil
  end,
  MoveNext = function (this)
    local t = this.list
    if this.version ~= t.version then
      throwFailedVersion()
    end
    local index = this.index
    local v = t[index]
    if v ~= nil then
      if v == null then
        this.current = nil
      else
        this.current = v
      end
      this.index = index + 1
      return true
    end
    this.current = nil
    return false
  end,
}
ArrayEnumerator.__index = ArrayEnumerator

arrayEnumerator = function (t)
  return setmetatable({ list = t, index = 1, version = t.version }, ArrayEnumerator)
end

Array = {
  version = 0,
  new = buildArray,
  set = set,
  get = get,
  ctorList = function (t, ...)
    local len = select("#", ...)
    if len == 0 then return end
    local collection = ...
    if type(collection) == "number" then return end
    addRange(t, collection)
  end,
  add = function (t, v)
    t[#t + 1] = v == nil and null or v
    t.version = t.version + 1
  end,
  addRange = addRange,
  clear = function (t)
    local size = #t
    if size > 0 then
      for i = 1, size do
        t[i] = nil
      end
      t.version = t.version + 1
    end
  end,
  contains = function (t, v)
    local equals = EqualityComparer_1(t.__genericT__).getDefault().Equals
    for i = 1, #t do 
      local item = t[i]
      if item == null then
        item = nil
      end
      if equals(item, v) then
        return true
      end
    end
    return false
  end,
  findAll = function (t, match)
    return setmetatable(findAll(t, match), System.List(t.__genericT__))
  end,
  first = function (t)
    if #t == 0 then throw(InvalidOperationException()) end
    local v = t[1]
    if v ~= null then
      return v
    end
  end,
  insert = function (t, index, v)
    if index < 0 or index > #t then
      throw(ArgumentOutOfRangeException("index"))
    end
    tinsert(t, index + 1, v == nil and null or v)
    t.version = t.version + 1
  end,
  insertRange = function (t, index, collection) 
    if collection == nil then throw(ArgumentNullException("collection")) end
    if index < 0 or index > #t then
      throw(ArgumentOutOfRangeException("index"))
    end
    for _, v in each(collection) do
      index = index + 1
      tinsert(t, index, v == nil and null or v)
    end
    t.version = t.version + 1
  end,
  last = function (t)
    local n = #t
    if n == 0 then throw(InvalidOperationException()) end
    local v = t[n]
    if v ~= null then
      return v
    end
  end,
  popFirst = function (t)
    if #t == 0 then throw(InvalidOperationException()) end
    local v = t[1]
    tremove(t, 1)
    t.version = t.version + 1
    if v ~= null then
      return v
    end
  end,
  popLast = function (t)
    local n = #t
    if n == 0 then throw(InvalidOperationException()) end
    local v = t[n]
    t[n] = nil
    if v ~= null then
      return v
    end
  end,
  removeRange = removeRange,
  remove = function (t, v)
    local equals = EqualityComparer_1(t.__genericT__).getDefault().Equals
    for i = 1, #t do
      local item = t[i]
      if item == null then
        item = nil
      end
      if equals(item, v) then
        tremove(t, i)
        t.version = t.version + 1
        return true
      end
    end
    return false
  end,
  removeAll = function (t, match)
    if match == nil then throw(ArgumentNullException("match")) end
    local size = #t
    local freeIndex = 1
    while freeIndex <= size do
      local item = t[freeIndex]
      if item == null then
        item = nil
      end
      if match(item) then
        break
      end
      freeIndex = freeIndex + 1 
    end
    if freeIndex > size then return 0 end
  
    local current = freeIndex + 1
    while current <= size do 
      while current <= size do
        local item = t[current]
        if item == null then
          item = nil
        end
        if not match(item) then
          break
        end
        current = current + 1 
      end
      if current <= size then
        t[freeIndex] = t[current]
        freeIndex = freeIndex + 1
        current = current + 1
      end
    end
    freeIndex = freeIndex -1
    local count = size - freeIndex
    removeRange(t, freeIndex, count)
    return count
  end,
  removeAt = function (t, index)
    if index < 0 or index >= #t then
      throw(ArgumentOutOfRangeException("index"))
    end
    tremove(t, index + 1)
    t.version = t.version + 1
  end,
  getRange = function (t, index, count)
    if count < 0 or index > #t - count then
      throw(ArgumentOutOfRangeException("index or count"))
    end
    local list = {}
    copy(t, index, list, 0, count, true)
    return setmetatable(list, System.List(t.__genericT__))
  end,
  getCount = lengthFn,
  getSyncRoot = System.identityFn,
  getLongLength = lengthFn,
  getLength = lengthFn,
  getIsSynchronized = falseFn,
  getIsReadOnly = falseFn,
  getIsFixedSize = System.trueFn,
  getRank = System.oneFn,
  binarySearchArray = function (t, ...)
    local v, index, count, comparer
    local len = select("#", ...)
    if len == 1 then
      v = ...
      index = 0
      count = #t
    elseif len == 2 then
      v, index = ...
      count = #t - index
    elseif len == 3 then
      index, count, v = ...
    else
      index, count, v, comparer = ...
    end
    checkIndexAndCount(t, index, count)
    local compare
    if comparer == nil then
      compare = Comparer_1(t.__genericT__).getDefault().Compare 
    else    
      compare = comparer.compare
    end
    local lo = index
    local hi = index + count - 1
    while lo <= hi do
      local i = lo + div(hi - lo, 2)
      local item = t[i + 1]
      if item == null then
        item = nil
      end
      local order = compare(item, v);
      if order == 0 then return i end
      if order < 0 then
        lo = i + 1
      else
        hi = i - 1
      end
    end
    return -1
  end,
  Copy = function (t, ...)
    local len = select("#", ...)     
    if len == 2 then
      local array, length = ...
      copy(t, 0, array, 0, length)
    else 
      copy(t, ...)
    end
  end,
  CreateInstance = function (elementType, length)
    return buildArray(Array(elementType[1]), length)
  end,
  Empty = function (T)
    local t = emptys[T]
    if t == nil then
      t = Array(T)()
      emptys[T] = t
    end
    return t
  end,
  Exists = function (t, match)
    if t == nil then throw(ArgumentNullException("array")) end
    if match == nil then throw(ArgumentNullException("match")) end
    for i = 1, #t do
      local item = t[i]
      if item == null then
        item = nil
      end
      if match(item) then
        return true
      end
    end
    return false
  end,
  Find = function (t, match)
    if t == nil then throw(ArgumentNullException("array")) end
    if match == nil then throw(ArgumentNullException("match")) end
    for i = 1, #t do
      local item = t[i]
      if item == null then
        item = nil
      end
      if match(item) then
        return item
      end
    end
    return t.__genericT__:default()
  end,
  FindAll = function (t, match)
    return setmetatable(findAll(t, match), Array(t.__genericT__))
  end,
  FindIndex = function (t, ...)
    if t == nil then throw(ArgumentNullException("array")) end
    local startIndex, count, match
    local len = select("#", ...)
    if len == 1 then
      startIndex = 0
      count = #t
      match = ...
    elseif len == 2 then
      startIndex, match  = ...
      count = #t - startIndex
      checkIndexAndCount(t, startIndex, count)
    else
      startIndex, count, match = ...
      checkIndexAndCount(t, startIndex, count)
    end
    if match == nil then throw(ArgumentNullException("match")) end
    local endIndex = startIndex + count
    for i = startIndex + 1, endIndex  do
      local item = t[i]
      if item == null then
        item = nil
      end
      if match(item) then
        return i - 1
      end
    end
    return -1
  end,
  FindLast = function (t, match)
    if t == nil then throw(ArgumentNullException("array")) end
    if match == nil then throw(ArgumentNullException("match")) end
    for i = #t, 1, -1 do
      local item = t[i]
      if item == null then
        item = nil
      end
      if match(item) then
        return item
      end
    end
    return t.__genericT__:default()
  end,
  FindLastIndex = function (t, ...)
    if t == nil then throw(ArgumentNullException("array")) end
    local startIndex, count, match
    local len = select("#", ...)
    if len == 1 then
      startIndex = 0
      count = #t
      match = ...
    elseif len == 2 then
      startIndex, match  = ...
      count = #t - startIndex
    else
      startIndex, count, match = ...
    end
    if match == nil then throw(ArgumentNullException("match")) end
    if count < 0 or startIndex - count + 1 < 0 then
      throw(ArgumentOutOfRangeException("count"))
    end
    local endIndex = startIndex - count
    checkIndex(endIndex)
    for i = startIndex + 1, endIndex, -1 do
      local item = t[i]
      if item == null then
        item = nil
      end
      if match(item) then
        return i - 1
      end
    end
    return -1
  end,
  ForEach = function (t, action)
    if action == nil then throw(ArgumentNullException("action")) end
    local version = t.version
    for i = 1, #t do
      if version ~= t.version then
        throwFailedVersion()
      end
      local item = t[i]
      if item == null then
        item = nil
      end
      action(item)
    end
  end,
  IndexOf = function (t, ...)
    if t == nil then throw(ArgumentNullException("array")) end
    local v, index, count
    local len = select("#", ...)
    if len == 1 then
      v = ...
      index = 0
      count = #t
    elseif len == 2 then
      v, index = ...
      count = #t - index
      checkIndexAndCount(t, index, count)
    else
      v, index, count = ...
      checkIndexAndCount(t, index, count)
    end
    local equals = EqualityComparer_1(t.__genericT__).getDefault().Equals
    for i = index + 1, index + count do
      local item = t[i]
      if item == null then
        item = nil
      end
      if equals(item, v) then
        return i - 1
      end
    end
    return -1
  end,
  LastIndexOf = function (t, ...)
    if t == nil then throw(ArgumentNullException("array")) end
    local v, index, count
    local len = select("#", ...)
    if len == 1 then
      v = ...
      count = #t
      index = count - 1
    elseif len == 2 then
      v, index = ...
      count = #t == 0 and 0 or (index + 1)
    else
      v, index, count = ...
    end
    if count < 0 or index - count + 1 < 0 then
      throw(ArgumentOutOfRangeException("count"))
    end
    checkIndex(t, index - count)
    local equals = EqualityComparer_1(t.__genericT__).getDefault().Equals
    for i = index + 1, index - count, -1 do
      local item = t[i]
      if item == null then
        item = nil
      end
      if equals(item, v) then
        return i - 1
      end
    end
    return -1
  end,
  Resize = function (t, newSize, T)
    if newSize < 0 then throw(ArgumentOutOfRangeException("newSize")) end
    if t == nil then
      return buildArray(Array(T), newSize)
    end
    local arr = t
    if #arr ~= newSize then
      arr = setmetatable({}, Array(T))
      tmove(t, 1, #t, 1, arr)
    end
    return arr
  end,
  Reverse = function (t, index, count)
    if not index then
      index = 0
      count = #t
    else
      if count < 0 or index > #t - count then
        throw(ArgumentOutOfRangeException("index or count"))
      end
    end
    local i, j = index + 1, index + count
    while i <= j do
      t[i], t[j] = t[j], t[i]
      i = i + 1
      j = j - 1
    end
    t.version = t.version + 1
  end,
  Sort = function (t, ...)
    if t == nil then throw(ArgumentNullException("array")) end
    local len = select("#", ...)
    if len == 0 then
      sort(t)
    elseif len == 1 then
      local comparer = ...
      sort(t, comparer)
    else
      local index, count, comparer = ...
      if count > 1 then
        local comp = getComp(t, comparer)
        if index == 0 and count == #t then
          tsort(t, comp)
        else
          checkIndexAndCount(t, index, count)
          local arr = {}
          tmove(t, index + 1, index + count, 1, arr)
          tsort(arr, comp)
          tmove(arr, 1, count, index + 1, t)
        end
        t.version = t.version + 1
      end
    end
  end,
  toArray = function (t)
    local array = {}    
    if t.GetEnumerator == arrayEnumerator then
      tmove(t, 1, #t, 1, array)
    else
      local count = 1
      for _, v in each(t) do
        array[count] = v == nil and null or v
        count = count + 1
      end
    end
    return arrayFromTable(array, t.__genericT__)
  end,
  TrueForAll = function (t, match)
    if t == nil then throw(ArgumentNullException("array")) end
    if match == nil then throw(ArgumentNullException("match")) end
    for i = 1, #t do
      local item = t[i]
      if item == null then
        item = nil
      end
      if not match(item) then
        return false
      end
    end
    return true
  end,
  CopyTo = function (this, array, index)
    if array == nil then throw(ArgumentNullException("array")) end
    copy(this, 0, array, index or 0, #this)
  end,
  GetEnumerator = arrayEnumerator,
  GetLength = function (this, dimension)
    if dimension ~= 0 then
      throw(IndexOutOfRangeException())
    end
    return #this
  end,
  GetValue = get,
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

function System.arrayFromList(t)
  return setmetatable(t, Array(t.__genericT__))
end

arrayFromTable = function (t, T, readOnly)
  assert(T)
  local array = setmetatable(t, Array(T))
  if readOnly then
    array.set = unset
  end
  return array
end

System.arrayFromTable = arrayFromTable

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
  end
}

function System.multiArrayFromTable(t, T)
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

getmetatable(MultiArray).__index = Array

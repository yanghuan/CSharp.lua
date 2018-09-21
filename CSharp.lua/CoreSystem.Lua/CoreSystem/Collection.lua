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
local div = System.div
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local InvalidOperationException = System.InvalidOperationException
local ArgumentNullException = System.ArgumentNullException
local NullReferenceException = System.NullReferenceException
local EqualityComparer_1 = System.EqualityComparer_1
local Comparer_1 = System.Comparer_1

local table = table
local tinsert = table.insert
local tremove = table.remove
local tsort = table.sort
local tmove = table.move
local tconcat = table.concat
local pack = table.pack
local unpack = table.unpack
local setmetatable = setmetatable
local select = select
local type = type
local assert = assert
local coroutine = coroutine
local ccreate = coroutine.create
local cresume = coroutine.resume
local cyield = coroutine.yield
local ipairs = ipairs

local Collection = {}
local null = {}

local versons = setmetatable({}, { __mode = "k" })

local function getVersion(t)
  return versons[t] or 0
end

local function changeVersion(t)
  local verson = getVersion(t)
  versons[t] = verson + 1
end

local function checkVersion(t, verson)
  if verson ~= getVersion(t) then
    throw(InvalidOperationException("has change when iterator"))
  end 
end

Collection.getVersion = getVersion
Collection.changeVersion = changeVersion
Collection.checkVersion = checkVersion

local counts = setmetatable({}, { __mode = "k" })

local function getCount(t)
  return counts[t] or 0
end

local function addCount(t, inc)
  if inc ~= 0 then
    local v = (counts[t] or 0) + inc
    assert(v >= 0)
    counts[t] = v 
  end
end

local function clearCount(t)
  counts[t] = nil
end

Collection.getCount = getCount
Collection.addCount = addCount
Collection.clearCount = clearCount

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

Collection.checkIndex = checkIndex
Collection.checkIndexAndCount = checkIndexAndCount

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

Collection.wrap = wrap
Collection.unWrap = unWrap

function Collection.getArray(t, index)
  checkIndex(t, index)
  local item = t[index + 1] 
  return unWrap(item)
end

function Collection.setArray(t, index, v)
  checkIndex(t, index)
  t[index + 1] = wrap(v)
  changeVersion(t)
end

function Collection.pushArray(t, v)
  tinsert(t, wrap(v))
  changeVersion(t)
end

function Collection.buildArray(t, size, ...)
  local len = select("#", ...)
  for i = 1, len do
    local v = select(i, ...)   
    tinsert(t, wrap(v))
  end
  if len < size then
    local default = t.__genericT__:__default__()
    if default == nil then
      default = null
    end
    for i = len + 1, size do
      tinsert(t, default)
    end
  end
end

local function checkInsertIndex(t, index)   
  if index < 0 or index > #t then
    throw(ArgumentOutOfRangeException("index"))
  end
end

function Collection.insertArray(t, index, v)
  checkInsertIndex(t, index)
  tinsert(t, index + 1, wrap(v))
  changeVersion(t)
end

function Collection.removeArrayAll(t)
  local size = #t
  if size > 0 then
    for i = 1, size do
      t[i] = nil
    end
    changeVersion(t)
  end
end

local function copyArray(sourceArray, sourceIndex, destinationArray, destinationIndex, length, reliable)
  if not reliable then
    if sourceArray == nil then throw(ArgumentNullException("sourceArray")) end
    if destinationArray == nil then throw(ArgumentNullException("destinationArray")) end
    checkIndexAndCount(sourceArray, sourceIndex, length)
    checkIndexAndCount(destinationArray, destinationIndex, length)
  end
  tmove(sourceArray, sourceIndex + 1, sourceIndex + length, destinationIndex + 1, destinationArray)
end 

function Collection.copyArray(t, ...)
  local len = select("#", ...)     
  if len == 2 then
    local array, length = ...
    copyArray(t, 0, array, 0, length)
  else 
    copyArray(t, ...)
  end
end

function Collection.removeArray(t, index, count)
  checkIndexAndCount(t, index, count)
  if count > 0 then
    local size = #t - count
    if index < size then
      copyArray(t, index + count, t, index, size - index)
    end
    for i = size + 1, size + count do
      t[i] = nil
    end
    changeVersion(t)
  end
end

function Collection.removeAtArray(t, index)
  checkIndex(t, index)
  tremove(t, index + 1)
  changeVersion(t)
end

local function binarySearchArray(t, index, count, v, comparer)
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
    local order = compare(unWrap(t[i + 1]), v);
    if order == 0 then return i end
    if order < 0 then
      lo = i + 1
    else
      hi = i - 1
    end
  end
  return -1
end 

function Collection.binarySearchArray(t, ...)
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
  return binarySearchArray(t, index, count, v, comparer)
end

local function findIndexOfArray(t, ...)
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
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  checkIndexAndCount(t, startIndex, count)
  local endIndex = startIndex + count
  for i = startIndex + 1, endIndex  do
    local item = unWrap(t[i])
    if match(item) then
      return i - 1
    end
  end
  return -1
end

Collection.findIndexOfArray = findIndexOfArray

function Collection.existsOfArray(t, match)
  return findIndexOfArray(t, match) ~= -1
end

function Collection.findOfArray(t, match)
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  for _, i in ipairs(t) do
    local item = unWrap(i)
    if match(item) then
      return item
    end
  end
  return t.__genericT__:__default__()
end

function Collection.findAllOfArray(t, match)
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  local list = System.List(t.__genericT__)()
  for _, i in ipairs(t) do
    local item = unWrap(i)
    if match(item) then
      list:Add(item)
    end
  end
  return list
end

function Collection.findLastOfArray(t, match)
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  for i = #t, 1, -1 do
    local item = unWrap(t[i])
    if match(item) then
      return item
    end
  end
  return t.__genericT__:__default__()
end

function Collection.findLastIndexOfArray(t, ...)
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
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  if count < 0 or startIndex - count + 1 < 0 then
    throw(ArgumentOutOfRangeException("count"))
  end
  local endIndex = startIndex - count
  checkIndex(endIndex)
  for i = startIndex + 1, endIndex, -1 do
    local item = unWrap(t[i])
    if match(item) then
      return i - 1
    end
  end
  return -1
end

local function indexOfArray(t, v, index, count)
  checkIndexAndCount(t, index, count)
  local equals = EqualityComparer_1(t.__genericT__).getDefault().Equals
  for i = index + 1, index + count do 
    if equals(unWrap(t[i]), v) then
      return i - 1
    end
  end
  return -1
end

function Collection.indexOfArray(t, ...)
  local v, index, count
  local len = select("#", ...)
  if len == 1 then
    v = ...
    index = 0
    count = #t
  elseif len == 2 then
    v, index = ...
    count = #t - index
  else
    v, index, count = ...
  end
  return indexOfArray(t, v, index, count)
end

function Collection.contains(t, item)
  return indexOfArray(t, item, 0, #t) ~= -1
end

local function lastIndexOfArray(t, v, index, count)
  if count < 0 or index - count + 1 < 0 then
    throw(ArgumentOutOfRangeException("count"))
  end
  checkIndex(t, index - count)
  local equals = EqualityComparer_1(t.__genericT__).getDefault().Equals
  for i = index + 1, index - count, -1 do 
    if equals(unWrap(t[i]), v) then
      return i - 1
    end
  end
  return -1
end

function Collection.lastIndexOfArray(t, ...)
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
  return lastIndexOfArray(t, v, index, count)
end

function Collection.reverseArray(t, index, count)
  if not index then
    index = 0
    count = #t
  end 
  checkIndexAndCount(t, index, count)
  local i, j = index + 1, index + count
  while i <= j do
    t[i], t[j] = t[j], t[i]
    i = i + 1
    j = j - 1
  end
  changeVersion(t)
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
    return compare(unWrap(x), unWrap(y)) < 0
  end
end

local function sort(t, comparer)
  if #t > 1 then
    tsort(t, getComp(t, comparer))
    changeVersion(t)
  end
end

Collection.sort = sort

local function sortArray(t, index, count, comparer)
  if count > 1 then
    local comp = getComp(t, comparer)
    if index == 0 and count == #t then
      tsort(t, comp)
    else
      checkIndexAndCount(t, index, count)
      local arr = {}
      for i = index + 1, index + count do
        tinsert(arr, t[i])
      end
      tsort(arr, comp)
      for i = index + 1, index + count do
        t[i] = arr[i - index]
      end
    end
    changeVersion(t)
  end
end

function Collection.sortArray(t, ...)
  local len = select("#", ...)
  if len == 0 then
    sort(t)
  elseif len == 1 then
    local comparer = ...
    sort(t, comparer)
  else
    index, count, comparer = ...
    sortArray(t, index, count, comparer)
  end
end

function Collection.trueForAllOfArray(t, match)
  if match == nil then
    throw(ArgumentNullException("match"))
  end
  for _, i in ipairs(t) do
    if not match(unWrap(i)) then
      return false
    end
  end
  return true
end

local ipairsFn = ipairs(null)
local function ipairsArray(t)
  local version = getVersion(t)
  return function(t, inx) 
    checkVersion(t, version)
    local k, v = ipairsFn(t, inx)
    return k, unWrap(v)
  end, t, 0
end

local pairsFn = next
local function pairsDict(t)
  local version = getVersion(t)
  return function(t, inx) 
    checkVersion(t, version)
    local k, v = pairsFn(t, inx)
    return k, unWrap(v)
  end, t, nil
end

function Collection.forEachArray(t, action)
  if action == null then
    throw(ArgumentNullException("action"))
  end
  local verson = getVersion(t)
  for _, i in ipairs(t) do
    checkVersion(t, verson)
    action(unWrap(i))
  end
end

local ArrayEnumerator = {}
ArrayEnumerator.__index = ArrayEnumerator

function ArrayEnumerator.MoveNext(this)
  local t = this.list
  checkVersion(t, this.verson)
  local index = this.index
  if index < #t then
    local i, v = ipairsFn(t, index)
    this.current = unWrap(v)
    this.index = i
    return true
  end
  this.current = nil
  return false
end

function ArrayEnumerator.getCurrent(this)
  return this.current
end

function ArrayEnumerator.Reset(this)
  this.index = 0
  this.current = nil
end

local function arrayEnumerator(t)
  local en = {
    list = t,
    index = 0,
    verson = getVersion(t),
  }
  setmetatable(en, ArrayEnumerator)
  return en
end

Collection.arrayEnumerator = arrayEnumerator

local function isArrayLike(t)
  return t.GetEnumerator == arrayEnumerator
end

local function isEnumerableLike(t)
  return type(t) == "table" and t.GetEnumerator ~= nil
end

local function eachFn(en)
  if en:MoveNext() then
    return true, en:getCurrent()
  end
  return nil
end

local function each(t)
  if t == nil then
    throw(NullReferenceException(), 1)
  end
  local getEnumerator = assert(t.GetEnumerator, t.__name__)
  if getEnumerator == arrayEnumerator then
    return ipairsArray(t)
  end
  local en = getEnumerator(t)
  return eachFn, en
end

function Collection.insertRangeArray(t, index, collection) 
  if collection == nil then
    throw(ArgumentNullException("collection"))
  end
  checkInsertIndex(t, index)
  for _, v in each(collection) do
    index = index + 1
    tinsert(t, index, wrap(v))
  end
  changeVersion(t)
end

function Collection.toArray(t)
  local array = {}    
  if isArrayLike(t) then
    for _, v in ipairs(t) do
      tinsert(array, v)
    end   
  else
    for _, v in each(t) do
      tinsert(array, wrap(v))
    end
  end
  return System.arrayFromTable(array, t.__genericT__)
end

local function toLuaTable(array)
  local t = {}
  for i, v in ipairs(array) do
    t[i] = unWrap(v)
  end   
  return t
end

local KeyValuePair = {}

function KeyValuePair.__ctor__(this, key, value)
  this.Key, this.Value = key, value
end

function KeyValuePair.__clone__(this)
  return setmetatable({ Key = this.Key, Value = this.Value }, KeyValuePair)
end

function KeyValuePair.__default__(T)
  local TKey, TValue = T.__genericTKey__, T.__genericTValue__
  return setmetatable({ Key = TKey:__default__(), Value = TValue:__default__() }, KeyValuePair)
end

function KeyValuePair.Deconstruct(this)
  return this.Key, this.Value
end

function KeyValuePair.ToString(this)
  local t = {}
  tinsert(t, "[")
  local k, v = this.Key, this.Value
  if k ~= nil then
    tinsert(t, k:ToString())
  end
  tinsert(t, ", ")
  if v ~= nil then
    tinsert(t, v:ToString())
  end
  tinsert(t, "]")
  return tconcat(t)
end

System.defStc("System.KeyValuePair", function (TKey, TValue) 
  return {
    __genericTKey__ = TKey,
    __genericTValue__ = TValue,
  }
end, KeyValuePair)

local DictionaryEnumerator = {}
DictionaryEnumerator.__index = DictionaryEnumerator

function DictionaryEnumerator.MoveNext(this)
  local t = this.dict
  checkVersion(t, this.version)
  local k, v = pairsFn(t, this.index)
  if k ~= nil then
    if this.kind == 0 then
      local pair = this.pair
      pair.Key = k
      pair.Value = unWrap(v)
      this.current = pair
    elseif this.kind == 1 then
      this.current = unWrap(k)
    else
      this.current = unWrap(v)
    end
    this.index = k
    return true
  end
  this.current = nil
  return false
end

function DictionaryEnumerator.getCurrent(this)
  return this.current
end

function Collection.dictionaryEnumerator(t, kind)
  local en = {
    dict = t,
    version = getVersion(t),
    kind = kind,
    pair = kind == 0 and setmetatable({ 
      Key = false, 
      Value = false,
      __genericTKey__ = t.__genericTKey__,
      __genericTValue__ = t.__genericTValue__ }, KeyValuePair) or nil
  }
  setmetatable(en, DictionaryEnumerator)
  return en
end

local LinkedListEnumerator = {}
LinkedListEnumerator.__index = LinkedListEnumerator

function LinkedListEnumerator.MoveNext(this)
  local list = this.list
  local node = this.node
  checkVersion(list, this.version)
  if node == nil then
    return false
  end
  this.current = node.Value
  node = node.next
  if node == list.head then
    node = nil
  end
  this.node = node
  return true 
end

function LinkedListEnumerator.getCurrent(this)
  return this.current
end

function Collection.linkedListEnumerator(t)
  local en = {
    list = t,
    version = getVersion(t),
    node = t.head
  }
  setmetatable(en, LinkedListEnumerator)
  return en
end

local yieldCoroutinePool = {}
local yieldCoroutineExit = {}

local function yieldCoroutineCreate(f)
  local co = tremove(yieldCoroutinePool)
  if co == nil then
    co = ccreate(function (...)
      f(...)
      while true do
        f = nil
        tinsert(yieldCoroutinePool, co)
        f = cyield(yieldCoroutineExit)
        f(cyield())
      end
    end)
  else
    cresume(co, f)
  end
  return co
end

local YieldEnumerator = {}
YieldEnumerator.__inherits__ = { System.IEnumerator }

function YieldEnumerator.MoveNext(this)
  local co = this.co
  if co == "exit" then
    return false
  end

  local ok, v
  if co == nil then
    co = yieldCoroutineCreate(this.f)
    this.co = co
    local args = this.args
    ok, v = cresume(co, unpack(args, 1, args.n))
    this.args = nil
  else
    ok, v = cresume(co)
  end

  if ok then
    if v == yieldCoroutineExit then
      this.co = "exit"
      this.current = nil
      return false
    else
      this.current = v
      return true
    end
  else
    throw(v)
  end
end

function YieldEnumerator.getCurrent(this)
  return this.current
end

System.define("System.YieldEnumerator", YieldEnumerator)

local function yieldIEnumerator(f, T, ...)
  return setmetatable({ f = f, __genericT__ = T, args = pack(...) }, YieldEnumerator)
end

local YieldEnumerable = {}
YieldEnumerable.__inherits__ = { System.IEnumerable }

function YieldEnumerable.GetEnumerator(this)
  return setmetatable({ f = this.f, __genericT__ = this.__genericT__, args = this.args }, YieldEnumerator)
end

System.define("System.YieldEnumerable", YieldEnumerable)

local function yieldIEnumerable(f, T, ...)
  return setmetatable({ f = f, __genericT__ = T, args = pack(...) }, YieldEnumerable)
end

System.toLuaTable = toLuaTable
System.Collection = Collection
System.each = each
System.ipairs = ipairsArray
System.pairs = pairsDict
System.isArrayLike = isArrayLike
System.isEnumerableLike = isEnumerableLike
System.yieldIEnumerator = yieldIEnumerator
System.yieldIEnumerable = yieldIEnumerable
System.yieldReturn = cyield
--[[
Copyright YANG Huan (sy.yanghuan@gmail.com).

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
local div = System.div
local throw = System.throw
local each = System.each
local er = System.er 
local Array = System.Array
local Comparer_1 = System.Comparer_1

local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local InvalidOperationException = System.InvalidOperationException

local select = select
local type = type

local function down(t, k, n, c)
  local j
  while true do
    j = k * 2
    if j <= n and j > 0 then
      if j < n and c(t[j], t[j + 1]) > 0 then
        j = j + 1
      end
      if c(t[k], t[j]) <= 0 then
        break
      end
      t[j], t[k] = t[k], t[j]
      k = j
    else
      break
    end
  end
end

local function up(t, k, n, c)
  while k > 1 do
    local j = div(k, 2)
    if c(t[j], t[k]) <= 0 then
      break
    end
    t[j], t[k] = t[k], t[j]
    k = j
  end
end

local function heapify(t)
  local c = t.c
  local n = #t
  for i = div(n, 2), 1, -1 do
    down(t, i, n, c)  
  end
end

local function __ctor__(t, ...)
  local items, comparer
  local n = select("#", ...)
  if n == 0 then
  else
    items, comparer = ...
    if items == nil then throw(ArgumentNullException("items")) end
  end
  if comparer == nil then comparer = Comparer_1(t.__genericTPriority__).getDefault() end
  local compare = comparer.Compare
  t.comparer, t.c = comparer, function (a, b) return compare(comparer, a[2], b[2]) end
  if type(items) == "table" then
    local i = 1
    for _, tuple in each(items) do
      local element, priority = tuple[1], tuple[2]
      t[i] = { element, priority }
      i = i + 1
    end
    if i > 1 then
      heapify(t)
    end
  end
end

local function dequeue(t, n)
  local v = t[1][1]
  t[1] = t[n]
  t[n] = nil
  down(t, 1, n - 1, t.c)
  return v
end

local function enqueue(t, element, priority)
  local n = #t + 1
  t[n] = { element, priority }
  up(t, n, n, t.c)
end

local function enqueueDequeue(t, element, priority)
  local n = #t
  if n ~= 0 then
    local c = t.c
    local item = { element, priority }
    local root = t[1]
    if c(item, root) > 0 then
      t[1] = item
      down(t, 1, n, c)
      return root[1]
    end
  end
  return element
end

local function enqueueRange(t, ...)
  local len = select("#", ...)
  local n = #t
  if n == 0 then
    local i = 1
    if len == 1 then
      local items = ...
      for _, tuple in each(items) do
        local element, priority = tuple[1], tuple[2]
        t[i] = { element, priority }
        i = i + 1
      end
    else
      local elements, priority = ...
      for _, element in each(elements) do
        t[i] = { element, priority }
        i = i + 1
      end
    end
    if i > 1 then
      heapify(t)
    end
  else
    local c = t.c
    if len == 1 then
      local items = ...
      for _, tuple in each(items) do
        local element, priority = tuple[1], tuple[2]
        n = n + 1
        t[n] = { element, priority }
        up(t, n, n, c)
      end
    else
      local elements, priority = ...
      for _, element in each(elements) do
        n = n + 1
        t[n] = { element, priority }
        up(t, n, n, c)
      end
    end
  end
end

local function ensureCapacity(t, capacity)
  if capacity < 0 then
    throw(ArgumentOutOfRangeException("capacity", er.ArgumentOutOfRange_NeedNonNegNum()))
  end
  local n = #t
  local newcapacity = 2 * n
  newcapacity = math.max(newcapacity, n + 4)
  if newcapacity < 4 then newcapacity = 4 end
  return newcapacity
end

local function peek(t)
  local v = t[1]
  if v == nil then throw(InvalidOperationException(er.InvalidOperation_EmptyQueue())) end
  return v[1]
end

local function tryDequeue(t)
  local n = #t
  if n == 0 then 
    local v = dequeue(t, n)
    return true, v[1], v[2]
  end
  return false, t.__genericTElement__:default(), t.__genericTPriority__:default()
end

local function tryPeek(t)
  local v = t[1] 
  if v then
    return true, v[1], v[2]
  end
  return false, t.__genericTElement__:default(), t.__genericTPriority__:default()
end

local PriorityQueue = {
  __ctor__ = __ctor__,
  getCount = System.lengthFn,
  getComparer = Array.getOrderComparer,
  Clear = Array.clear,
  Dequeue = function (t)
    local n = #t
    if n == 0 then throw(InvalidOperationException(er.InvalidOperation_EmptyQueue())) end
    return dequeue(t, n)
  end,
  Enqueue = enqueue,
  EnqueueDequeue = enqueueDequeue,
  EnqueueRange = enqueueRange,
  EnsureCapacity = ensureCapacity,
  Peek = peek,
  TrimExcess = System.emptyFn,
  TryDequeue = tryDequeue,
  TryPeek = tryPeek,
}

local PriorityQueueFn = System.define("System.Collections.Generic.PriorityQueue", function (TElement, TPriority)
  return { 
    __genericTElement__ = TElement,
    __genericTPriority__ = TPriority,
  }
end, PriorityQueue, 2)

System.PriorityQueue = PriorityQueueFn

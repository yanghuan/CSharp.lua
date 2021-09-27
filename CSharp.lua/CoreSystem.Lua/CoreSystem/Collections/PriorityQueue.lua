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

local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local InvalidOperationException = System.InvalidOperationException

local Comparer_1 = System.Comparer_1

local select = select
local type = type

local function fixDown(t, k, n, comparer, c)
  if not comparer then
    comparer = t.comparer
    c = comparer.Compare
  end
  local j
  while true do
    j = k * 2
    if j <= n and j > 0 then
      if j < n and c(comparer, t[j][2], t[j + 1][2]) > 0 then
        j = j + 1
      end
      if c(comparer, t[k][2], t[j][2]) <= 0 then
        break
      end
      t[j], t[k] = t[k], t[j]
      k = j
    else
      break
    end
  end
end

local function fixUp(t, k, n, comparer, c)
  if not comparer then
    comparer = t.comparer
    c = comparer.Compare
  end
  while k > 1 do
    local j = div(k, 2)
    if c(comparer, t[j][2], t[k][2]) <= 0 then
      break
    end
    t[j], t[k] = t[k], t[j]
    k = j
  end
end

local function heapify(t)
  local comparer = t.comparer
  local c = comparer.Compare
  local n = #t
  for i = div(n, 2), 1, -1 do
    fixDown(t, i, n, comparer, c)  
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
  t.comparer = comparer
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
  fixDown(t, 1, n -1)
  return v
end

local function enqueue(t, element, priority)
  local n = #t + 1
  t[n] = { element, priority }
  fixUp(t, n, n)
end

local function enqueueDequeue(t, element, priority)
  local n = #t
  if n ~= 0 then
    local comparer = t.comparer
    local c = comparer.Compare
    local root = t[1]
    if c(comparer, priority, root[2]) > 0 then
      t[1] = { element, priority }
      fixDown(t, 1, n, comparer, c)
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
    local comparer = t.comparer
    local c = comparer.Compare
    if len == 1 then
      local items = ...
      for _, tuple in each(items) do
        local element, priority = tuple[1], tuple[2]
        n = n + 1
        t[n] = { element, priority }
        fixUp(t, n, n, comparer, c)
      end
    else
      local elements, priority = ...
      for _, element in each(elements) do
        n = n + 1
        t[n] = { element, priority }
        fixUp(t, n, n, comparer, c)
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

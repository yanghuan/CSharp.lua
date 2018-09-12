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
local ArgumentNullException = System.ArgumentNullException

local setmetatable = setmetatable
local getmetatable = getmetatable
local insert = table.insert
local ipairs = ipairs
local assert = assert

local Delegate = {}
debug.setmetatable(System.emptyFn, Delegate)

local multicast = setmetatable({}, Delegate)
multicast.__index = multicast

local memberMethod = setmetatable({}, Delegate)
memberMethod.__index = memberMethod

function multicast.__call(t, ...)
  local result
  for _, f in ipairs(t) do
    result = f(...)
  end
  return result
end

function memberMethod.__call(t, ...)
  return t.method(t.target, ...)
end

local function appendFn(t, f)
  if getmetatable(f) == multicast then
    for _, i in ipairs(f) do
      insert(t, i)
    end
  else
    insert(t, f)
  end
end

local function combineImpl(fn1, fn2)    
  local t = {}
  setmetatable(t, multicast)
  appendFn(t, fn1)
  appendFn(t, fn2)
  return t
end

local function combine(fn1, fn2)
  if fn1 ~= nil then
    if fn2 ~= nil then 
      return combineImpl(fn1, fn2) 
    end
    return fn1 
  end
  if fn2 ~= nil then return fn2 end
  return nil
end

Delegate.Combine = combine

local function bind(target, method)
  if target == nil then
    throw(ArgumentNullException())
  end
  assert(method)

  local delegate = target[method]
  if delegate ~= nil then
    return delegate
  end

  local t = {
    target = target,
    method = method,
  }
  setmetatable(t, memberMethod)
  target[method] = t
  return t
end

Delegate.bind = bind
System.bind = bind

local function equalsSingle(fn1, fn2)
  if getmetatable(fn1) == memberMethod then
    if getmetatable(fn2) == memberMethod then
      return fn1.target == fn2.target and fn1.method == fn2.method
    end
    return false 
  end
  if getmetatable(fn2) == memberMethod then return false end
  return fn1 == fn2
end

local function equalsMulticast(fn1, fn2, start, count)
  for i = 1, count do
    if not equalsSingle(fn1[start + i], fn2[i]) then
      return false
    end
  end
  return true
end

local function delete(fn, count, deleteIndex, deleteCount)
  local t = {}
  setmetatable(t, multicast)
  for i = 1, deleteIndex - 1 do
    insert(t, fn[i])
  end
  for i = deleteIndex + deleteCount, count do
    insert(t, fn[i])
  end
  return t
end

local function removeImpl(fn1, fn2) 
  if getmetatable(fn2) ~= multicast then
    if getmetatable(fn1) ~= multicast then
      if equalsSingle(fn1, fn2) then
          return nil
      end
    else
      local count = #fn1
      for i = count, 1, -1 do
        if equalsSingle(fn1[i], fn2) then
          if count == 2 then
            return fn1[3 - i]
          else
            return delete(fn1, count, i, 1)
          end
        end
      end
    end
  elseif getmetatable(fn1) == multicast then
      local count1, count2 = #fn1, # fn2
      local diff = count1 - count2
      for i = diff + 1, 1, -1 do
        if equalsMulticast(fn1, fn2, i - 1, count2) then
          if diff == 0 then 
            return nil
          elseif diff == 1 then 
            return fn1[i ~= 1 and 1 or count1] 
          else
            return delete(fn1, count1, i, count2)
          end
        end
      end
  end
  return fn1
end

local function remove(fn1, fn2)
  if fn1 ~= nil then
    if fn2 ~= nil then
      return removeImpl(fn1, fn2)
    end
    return fn1
  end
  return nil
end

Delegate.Remove = remove

function Delegate.RemoveAll(source, value)
  local newDelegate
  repeat
    newDelegate = source
    source = remove(source, value)
  until newDelegate == source
  return newDelegate
end

function Delegate.DynamicInvoke(this, ...)
  return this(...)
end

local function equals(fn1, fn2)
  if getmetatable(fn1) == multicast then
    if getmetatable(fn2) == multicast then
      local len1, len2 = #fn1, #fn2
      if len1 ~= len2 then
        return false         
      end
      for i = 1, len1 do
        if not equalsSingle(fn1[i], fn2[2]) then
          return false
        end
      end
      return true
    end
    return false
  end
  if getmetatable(fn2) == multicast then return false end
  return equalsSingle(fn1, fn2)
end

Delegate.__add = combine
multicast.__add = combine
memberMethod.__add = combine

Delegate.__sub = remove
multicast.__sub = remove
memberMethod.__sub = remove

multicast.__eq = equals
memberMethod.__eq = equals
 
 local metatableOfNil = debug.getmetatable(nil)
 metatableOfNil.__add = function (a, b)
  if a == nil then
    if b == nil or type(b) == "number" then
      return nil
    end
    return b
  end
  return nil
 end

function Delegate.EqualsObj(this, obj)
  local typename = type(obj)
  if typename == "function" then
    return equals(this, obj)
  end
  if typename == "table" then
    local metatable = getmetatable(obj)
    if metatable == multicast or metatable == memberMethod then
      return equals(this, obj)
    end
  end
  return false
end

function Delegate.GetType(this)
  return System.typeof(Delegate)
end

System.define("System.Delegate", Delegate)
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
local ArgumentException = System.ArgumentException

local EqualityComparer = {}

function EqualityComparer.__ctor__(this)
  local T = this.__genericT__
  local equals = T.Equals or System.equals
  local getHashCode = T.GetHashCode or System.identityFn

  this.Equals = function(x, y)
    if x ~= nil then
      if y ~= nil then return equals(x, y) end
      return false
    end                 
    if y ~= nil then return false end
    return true
  end

  this.GetHashCode = function(x)
    if x == nil then return 0 end
    return getHashCode(x)
  end
end

System.define("System.EqualityComparer_1", function(T)
  local cls = {
    __inherits__ = { System.IEqualityComparer_1(T) }, 
    __genericT__ = T,
  }
  local defaultComparer
  function cls.getDefault()
    local comparer = defaultComparer 
    if comparer == nil then
      comparer = System.EqualityComparer_1(T)()
      defaultComparer = comparer
    end
    return comparer
  end
  return cls
end, EqualityComparer)

local function compare(a, b)
  if a == b then return 0 end
  if a == nil then return -1 end
  if b == nil then return 1 end
  local ia = a.CompareToObj
  if ia ~= nil then
    return ia(a, b)
  end
  local ib = b.CompareToObj
  if ib ~= nil then
    return -ib(b, a)
  end
  throw(ArgumentException("Argument_ImplementIComparable"))
end

local Comparer = {}
Comparer.Compare = compare

local defaultComparerOfComparer

function Comparer.getDefault()
  local comparer = defaultComparerOfComparer
  if comparer == nil then
    comparer = Comparer()
    defaultComparerOfComparer = comparer;
  end
  return comparer
end

function Comparer.__ctor__(this)
  local T = this.__genericT__
  if T then
    local compareTo = T.CompareTo
    if compareTo ~= nil then
      this.Compare = function(x, y)
        if x ~= nil then
          if y ~= nil then 
            return compareTo(x, y) 
          end
          return 1
        end                 
        if y ~= nil then return -1 end
        return 0
      end
    end
  end
end

System.define("System.Comparer", Comparer)

System.define("System.Comparer_1", function(T)
  local cls = {
    __inherits__ = { System.IComparer_1(T) }, 
    __genericT__ = T,
  }
  local defaultComparer
  function cls.getDefault()
    local comparer = defaultComparer 
    if comparer == nil then
      comparer = System.Comparer_1(T)()
      defaultComparer = comparer
    end
    return comparer
  end
  return cls
end, Comparer)
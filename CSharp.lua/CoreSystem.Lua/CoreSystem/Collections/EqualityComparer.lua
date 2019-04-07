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

local EqualityComparer = {
  __ctor__ = function (this)
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
}

local EqualityComparer_1
EqualityComparer_1 = define("System.EqualityComparer_1", function(T)
  local defaultComparer
  return {
    __genericT__ = T,
    __inherits__ = { System.IEqualityComparer_1(T) }, 
    getDefault = function ()
      local comparer = defaultComparer 
      if comparer == nil then
        comparer = EqualityComparer_1(T)()
        defaultComparer = comparer
      end
      return comparer
    end
  }
end, EqualityComparer)

local defaultComparerOfComparer

local Comparer
Comparer = define("System.Comparer", {
  __ctor__ = function (this)
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
  end,
  Compare = System.compareObj,
  getDefault = function ()
    local comparer = defaultComparerOfComparer
    if comparer == nil then
      comparer = Comparer()
      defaultComparerOfComparer = comparer;
    end
    return comparer
  end
})

local Comparer_1
Comparer_1 = define("System.Comparer_1", function(T)
  local defaultComparer
  local function getDefault()
    local comparer = defaultComparer 
    if comparer == nil then
      comparer = Comparer_1(T)()
      defaultComparer = comparer
    end
    return comparer
  end
  return {
    __genericT__ = T,
    __inherits__ = { System.IComparer_1(T) }, 
    getDefault = getDefault,
    getDefaultInvariant = getDefault,
  }
end, Comparer)

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
local defInf = System.defInf
local emptyFn = System.emptyFn
local IEnumerable = System.IEnumerable
local IEnumerator = System.IEnumerator

defInf("System.IFormattable")
defInf("System.IComparable")
defInf("System.IFormatProvider")
defInf("System.ICloneable")
defInf("System.IConvertible")

defInf("System.IComparable_1", emptyFn)
defInf("System.IEquatable_1", emptyFn)

defInf("System.IPromise")
defInf("System.IDisposable")

local ICollection = defInf("System.ICollection", {
  __inherits__ = { IEnumerable }
})

defInf("System.IList", {
  __inherits__ = { ICollection }
})

defInf("System.IDictionary", {
  __inherits__ = { ICollection }
})

defInf("System.IEnumerator_1", function(T) 
  return {
    __inherits__ = { IEnumerator }
  }
end)

local IEnumerable_1 = defInf("System.IEnumerable_1", function(T) 
  return {
    __inherits__ = { IEnumerable }
  }
end)

local ICollection_1 = defInf("System.ICollection_1", function(T) 
  return { 
    __inherits__ = { IEnumerable_1(T) } 
  }
end)

defInf('System.IDictionary_2', function(TKey, TValue) 
  return {
    __inherits__ = IEnumerable
  }
end)

defInf("System.IList_1", function(T) 
  return {
    __inherits__ = { ICollection_1(T) }
  }
end)

defInf("System.ISet_1", function(T) 
  return {
    __inherits__ = { ICollection_1(T) }
  }
end)

defInf("System.IComparer_1", emptyFn)
defInf("System.IEqualityComparer")
defInf("System.IEqualityComparer_1", emptyFn)

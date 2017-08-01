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
local emptyFn = System.emptyFn

System.defInf("System.IFormattable")
System.defInf("System.IComparable")
System.defInf("System.IFormatProvider")
System.defInf("System.ICloneable")

System.defInf("System.IComparable_1", emptyFn)
System.defInf("System.IEquatable_1", emptyFn)

System.defInf("System.IPromise")
System.defInf("System.IDisposable")

local IEnumerable = System.defInf("System.IEnumerable")
local IEnumerator = System.defInf("System.IEnumerator")

local ICollection = System.defInf("System.ICollection", {
  __inherits__ = { IEnumerable }
})

System.defInf("System.IList", {
  __inherits__ = { ICollection }
})

System.defInf("System.IDictionary", {
  __inherits__ = { ICollection }
})

System.defInf("System.IEnumerator_1", function(T) 
  return {
    __inherits__ = { IEnumerator }
  }
end)

local IEnumerable_1 = System.defInf("System.IEnumerable_1", function(T) 
  return {
    __inherits__ = { IEnumerable }
  }
end)

local ICollection_1 = System.defInf("System.ICollection_1", function(T) 
  return { 
    __inherits__ = { IEnumerable_1(T) } 
  }
end)

System.defInf('System.IDictionary_2', function(TKey, TValue) 
  return {
    __inherits__ = IEnumerable
  }
end)

System.defInf("System.IList_1", function(T) 
  return {
    __inherits__ = { ICollection_1(T) }
  }
end)

System.defInf("System.ISet_1", function(T) 
  return {
    __inherits__ = { ICollection_1(T) }
  }
end)

System.defInf("System.IComparer_1", emptyFn)
System.defInf("System.IEqualityComparer")
System.defInf("System.IEqualityComparer_1", emptyFn)
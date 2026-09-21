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
local Span = System.Span
local Array = System.Array

System.MemoryExtensions = {
  AsSpan = function (array) 
    if type(array) == "string" then
      return array
    end
    local SpanT = Span(array.__genericT__)
    return SpanT(array)
  end,
  AsBoundedSpan = function (array, start, length) 
    if type(array) == "string" then
      return array:Slice(start, length)
    end
    local SpanT = Span(array.__genericT__)
    return SpanT(array, start, length)
  end,
  Contains = function (span, value)
    if type(span) == "string" then
      return string.find(span, string.char(value), 1, true) ~= nil
    end
    if span._str then
      return string.find(span._str, string.char(value), span._min + 1, true) ~= nil
    end
    return Array.Contains(span._array, value)
  end,
  SequenceEqual = function (first, second)
    local len = #first
    if len ~= #second then
      return false
    end
    if type(first) == "string" and type(second) == "string" then
      return first == second
    end
    for i = 0, len - 1 do
      if first:get(i) ~= second:get(i) then
        return false
      end
    end
    return true
  end
}

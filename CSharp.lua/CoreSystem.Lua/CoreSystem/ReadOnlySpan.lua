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
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local IndexOutOfRangeException = System.IndexOutOfRangeException

local ReadOnlySpan = {
  __ctor__ = function (this, input, ...)
    if type(input) == "table" then
      local argsLen = select("#", ...)
      local maxLength = input:getLength()
      local start, length
      if argsLen == 2 then
        start, length = ...
        if start >= maxLength then
          throw(ArgumentOutOfRangeException("start"))
        end
        if start + length > maxLength then
          throw(ArgumentOutOfRangeException("length"))
        end 
      else
        start, length = 0, maxLength
      end
      this._array = input
      this._min = start
      this._max = start + length - 1
    else
      this._array = System.Array(this.__genericT__)(1)
      this._array:set(0, input)
      this._min = 0
      this._max = 0
    end
  end,
  get = function (this, index)
    local i = this._min + index
    if i > this._max then
      throw(IndexOutOfRangeException("index"))
    end
    return this._array:get(i)
  end,
  getIsEmpty = function (this)
    return this:getLength() <= 0
  end,
  getLength = function (this)
    return this._max - this._min + 1
  end,
  Slice = function (this, start, ...)
    local newMin = this._min + start
    if newMin > this._max then
      throw(ArgumentOutOfRangeException("start"))
    end

    local newMax
    local argsLen = select("#", ...)
    if argsLen == 1 then
      length = ...
      newMax = newMin + length - 1
      if newMax > this._max then
        throw(ArgumentOutOfRangeException("length"))
      end
    else
      newMax = this._max
    end
    local ctor = System.ReadOnlySpan(this.__genericT__)
    return ctor(this._array, newMin, newMax - newMin + 1)
  end,
  ctorArray = function (array)
    local ctor = System.ReadOnlySpan(array.__genericT__)
    return ctor(array)    
  end
}

local ReadOnlySpanFn = System.defStc("System.ReadOnlySpan", function (T)
  return {
    __genericT__ = T
  }
end, ReadOnlySpan, 1)

System.ReadOnlySpan = ReadOnlySpanFn

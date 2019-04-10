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
local clear = System.Array.clear
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException

local table = table
local tconcat = table.concat
local schar = string.char
local ssub = string.sub

local function build(this, value, startIndex, length)
  value = value:Substring(startIndex, length)
  local len = #value
  if len > 0 then
    this[#this + 1] = value
    this.Length = len
  end
end

System.define("System.StringBuilder", { 
  Length = 0,
  ToString = tconcat,
  __tostring = tconcat,
  __ctor__ = function (this, ...)
    local len = select("#", ...)
    if len == 0 then
    elseif len == 1 or len == 2 then
      local value = ...
      if type(value) == "string" then
        build(this, value, 0, #value)
      else
        build(this, "", 0, 0)
      end
    else 
      local value, startIndex, length = ...
      build(this, value, startIndex, length)
    end
  end,
  getLength = function (this)
    return this.Length
  end,
  setLength = function (this, value) 
    if value < 0 then throw(ArgumentOutOfRangeException("value")) end
    if value == 0 then
      this:Clear()
      return
    end
    local delta = value - this.Length
    if delta > 0 then
      this:AppendCharRepeat(0, delta)
    else
      local length, remain = #this, value
      for i = 1, length do
        local s = this[i]
        local len = #s
        if len >= remain then
          if len ~= remain then
            s = ssub(s, 0, remain)
            this[i] = s
          end
          for j = i + 1, length do
            this[j] = nil
          end
          break
        end
        remain = remain - len
      end
      this.Length = this.Length + delta
    end  
  end,
  Append = function (this, ...)
    local len = select("#", "...")
    if len == 1 then
      local value = ...
      if value ~= nil then
        value = value:ToString()
        this[#this + 1] = value
        this.Length =  this.Length + #value
      end
    else
      local value, startIndex, length = ...
      if value == nil then
        throw(ArgumentNullException("value"))
      end
      value = value:Substring(startIndex, length)
      this[#this + 1] = value
      this.Length =  this.Length + #value
    end
    return this
  end,
  AppendChar = function (this, v) 
    v = schar(v)
    this[#this + 1] = v
    this.Length = this.Length + 1
    return this
  end,
  AppendCharRepeat = function (this, v, repeatCount)
    if repeatCount < 0 then throw(ArgumentOutOfRangeException("repeatCount")) end
    if repeatCount == 0 then return this end
    v = schar(v)
    local count = #this + 1
    for i = 1, repeatCount do
      this[count] = v
      count = count + 1
    end
    this.Length = this.Length + repeatCount
    return this
  end,
  AppendFormat = function (this, format, ...)
    local value = format:Format(...)
    this[#this + 1] = value
    this.Length = this.Length + #value
    return this
  end,
  AppendLine = function (this, value)
    local count = 1
    local len = #this + 1
    if value ~= nil then
      this[len] = value
      len = len + 1
      count = count + #value
    end
    this[len] = "\n"
    this.Length = this.Length + count
    return this
  end,
  Clear = function (this)
    clear(this)
    this.length = 0
    return this
  end
})

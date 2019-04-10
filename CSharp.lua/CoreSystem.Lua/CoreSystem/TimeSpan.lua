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
local div = System.div
local trunc = System.trunc
local ArgumentException = System.ArgumentException
local OverflowException = System.OverflowException
local ArgumentNullException = System.ArgumentNullException
local FormatException = System.FormatException

local assert = assert
local getmetatable = getmetatable
local select = select
local sformat = string.format
local sfind = string.find
local tostring = tostring
local tonumber = tonumber
local floor = math.floor
local log10 = math.log10

local TimeSpan
local zero

local function compare(t1, t2)
  if t1.ticks > t2.ticks then return 1 end
  if t1.ticks < t2.ticks then return -1 end
  return 0
end

local function add(this, ts) 
  return TimeSpan(this.ticks + ts.ticks)
end

local function subtract(this, ts) 
  return TimeSpan(this.ticks - ts.ticks)
end

local function interval(value, scale)
  if value ~= value then 
    throw(ArgumentException("Arg_CannotBeNaN"))
  end
  local tmp = value * scale
  local millis = tmp + (value >=0 and 0.5 or -0.5)
  if millis > 922337203685477 or millis < -922337203685477 then
    throw(OverflowException("Overflow_TimeSpanTooLong"))
  end
  return TimeSpan(trunc(millis) * 1e4)
end

local function parse(s)
  if s == nil then
    return nil, 1
  end
  local v = tonumber(s)
  if v ~= nil then
    if v ~= floor(v) then
      return nil, 2
    end
    return TimeSpan.FromDays(v)
  end
  local i, j, day,  hour, minute, second, milliseconds
  i, j, day, hour, minute = sfind(s, "^%s*(%d+)%.(%d+):(%d+)")
  if i == nil then
    i, j, hour, minute = sfind(s, "^%s*(%d+):(%d+)")
    if i == nil then
      return nil, 2
    else
      hour, minute = tonumber(hour), tonumber(minute)
    end
    day = 0
  else
    day, hour, minute = tonumber(day), tonumber(hour), tonumber(minute)
  end
  if j < #s then
    local next = j + 1
    i, j, second = sfind(s, "^:(%d+)", next)
    if i == nil then
      if sfind(s, "^%s*$", next) == nil then
        return nil, 2
      else
        second = 0
        milliseconds = 0
      end
    else
      second = tonumber(second)
      next = j + 1
      i, j, milliseconds = sfind(s, "^%.(%d+)%s*$", next)
      if i == nil then
        if sfind(s, "^%s*$", next) == nil then
          return nil, 2
        else
          milliseconds = 0
        end
      else
        milliseconds = tonumber(milliseconds)
        local n = floor(log10(milliseconds) + 1)
        if n > 3 then
          if n > 7 then
            return nil, 2
          end
          milliseconds = milliseconds / (10 ^ (n - 3))
        end
      end
    end
  else
    second = 0
    milliseconds = 0
  end
  return TimeSpan(day, hour, minute, second, milliseconds)
end

TimeSpan = System.defStc("System.TimeSpan", {
  Zero = false,
  MaxValue = false,
  MinValue = false,
  __ctor__ = function (this, ...)
    local ticks
    local length = select("#", ...)
    if length == 1 then
      ticks = ...
    elseif length == 3 then
      local hours, minutes, seconds = ...
      ticks = (((hours * 60 + minutes) * 60) + seconds) * 1e7
    elseif length == 4 then
      local days, hours, minutes, seconds = ...
      ticks = ((((days * 24 + hours) * 60 + minutes) * 60) + seconds) * 1e7
    elseif length == 5 then
      local days, hours, minutes, seconds, milliseconds = ...
      ticks = (((((days * 24 + hours) * 60 + minutes) * 60) + seconds) * 1e3 + milliseconds) * 1e4
    else 
      assert(ticks)
    end
    this.ticks = ticks
  end,
  Compare = compare,
  CompareTo = compare,
  CompareToObj = function (this, t)
    if t == nil then return 1 end
    if getmetatable(t) ~= TimeSpan then
      throw(ArgumentException("Arg_MustBeTimeSpan"))
    end
    compare(this, t)
  end,
  Equals = function (t1, t2)
    return t1.ticks == t2.ticks
  end,
  EqualsObj = function(this, t)
    if getmetatable(t) == TimeSpan then
      return this.ticks == t.ticks
    end
    return false
  end,
  GetHashCode = function (this)
    return this.ticks
  end,
  getTicks = function (this) 
    return this.ticks
  end,
  getDays = function (this) 
    return div(this.ticks, 864e9)
  end,
  getHours = function(this) 
    return div(this.ticks, 36e9) % 24
  end,
  getMinutes = function (this) 
    return div(this.ticks, 6e8) % 60
  end,
  getSeconds = function (this) 
    return div(this.ticks, 1e7) % 60
  end,
  getMilliseconds = function (this) 
    return div(this.ticks, 1e4) % 1000
  end,
  getTotalDays = function (this) 
    return this.ticks / 864e9
  end,
  getTotalHours = function (this) 
    return this.ticks / 36e9
  end,
  getTotalMilliseconds = function (this) 
    return this.ticks / 1e4
  end,
  getTotalMinutes = function (this) 
    return this.ticks / 6e8
  end,
  getTotalSeconds = function (this) 
    return this.ticks / 1e7
  end,
  Add = function (this, ts) 
    return TimeSpan(this.ticks + ts.ticks)
  end,
  Subtract = function (this, ts) 
    return TimeSpan(this.ticks - ts.ticks)
  end,
  Duration = function (this) 
    local ticks = this.ticks
    if ticks == -9223372036854775808 then
      throw(OverflowException("Overflow_Duration"))
    end
    return TimeSpan(ticks >= 0 and ticks or - ticks)
  end,
  Negate = function (this) 
    local ticks = this.ticks
    if ticks == -9223372036854775808 then
      throw(OverflowException("Overflow_NegateTwosCompNum"))
    end
    return TimeSpan(-ticks)
  end,
  ToString = function (this) 
    local day, milliseconds = this:getDays(), this.ticks % 1e7
    local daysStr = day == 0 and "" or (day .. ".")
    local millisecondsStr = milliseconds == 0 and "" or "." .. milliseconds
    return sformat("%s%02d:%02d:%02d%s", daysStr, this:getHours(), this:getMinutes(), this:getSeconds(), millisecondsStr)
  end,
  TimeSpan = function (s)
    local v, err = parse(s)
    if v then
      return v
    end
    if err == 1 then
      throw(ArgumentNullException())
    else
      throw(FormatException())
    end
  end,
  TryParse = function (s)
    local v = parse(s)
    if v then
      return true, v
    end
    return false, zero
  end,
  __add = add,
  __sub = subtract,
  __eq = function (t1, t2)
    return t1.ticks == t2.ticks
  end,
  __lt = function (t1, t2)
    return t1.ticks < t2.ticks
  end,
  __le = function (t1, t2)
    return t1.ticks <= t2.ticks
  end,
  FromDays = function (value) 
    return interval(value, 864e5)
  end,
  FromHours = function (value) 
    return interval(value, 36e5)
  end,
  FromMilliseconds = function (value) 
    return interval(value, 1)
  end,
  FromMinutes = function (value) 
    return interval(value, 6e4)
  end,
  FromSeconds = function (value) 
    return interval(value, 1000)
  end,
  FromTicks = function (value) 
    return TimeSpan(value)
  end,
  __inherits__ = function (_, T)
    return { System.IComparable, System.IComparable_1(T), System.IEquatable_1(T) }
  end,
  default = function ()
    return zero
  end
})

zero = TimeSpan(0)
TimeSpan.Zero = zero
TimeSpan.MaxValue = TimeSpan(9223372036854775807)
TimeSpan.MinValue = TimeSpan(-9223372036854775808)
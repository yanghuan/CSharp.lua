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

local TimeSpan = System.TimeSpan
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local ArgumentException = System.ArgumentException

local getmetatable = getmetatable
local select = select
local format = string.format
local os = os
local ostime = os.time
local osdifftime = os.difftime
local osdate = os.date

--http://referencesource.microsoft.com/#mscorlib/system/datetime.cs
local DateTime = {}

local function compare(t1, t2)
  if t1.ticks > t2.ticks then return 1 end
  if t1.ticks < t2.ticks then return -1 end
  return 0
end

DateTime.Compare = compare
DateTime.CompareTo = compare

function DateTime.CompareToObj(this, t)
  if t == null then return 1 end
  if getmetatable(t) ~= DateTime then
    throw(ArgumentException("Arg_MustBeDateTime"))
  end
  return compare(this, t)
end

function DateTime.Equals(t1, t2)
  return t1.ticks == t2.ticks
end

function DateTime.EqualsObj(this, t)
  if getmetatable(t) == DateTime then
    return this.ticks == t.ticks
  end
  return false
end

function DateTime.GetHashCode(this)
  return this.ticks
end

local daysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 }
local daysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 }

local function isLeapYear(year) 
  if year < 1 or year > 9999 then 
    throw(ArgumentOutOfRangeException("year", "ArgumentOutOfRange_Year"))
  end
  return year % 4 == 0 and (year % 100 ~= 0 or year % 400 == 0)
end

DateTime.IsLeapYear = isLeapYear

local function dateToTicks(year, month, day) 
  if year >= 1 and year <= 9999 and month >= 1 and month <= 12 then
    local days = isLeapYear(year) and daysToMonth366 or daysToMonth365
    if day >= 1 and day <= days[month + 1] - days[month] then
      local y = year - 1
      local n = y * 365 + div(y, 4) - div(y, 100) + div(y, 400) + days[month] + day - 1
      return n * 864e9
    end
  end
end

local function timeToTicks(hour, minute, second)
  if hour >= 0 and hour < 24 and minute >= 0 and minute < 60 and second >=0 and second < 60 then 
      return (((hour * 60 + minute) * 60) + second) * 1e7
  end
  throw(ArgumentOutOfRangeException("ArgumentOutOfRange_BadHourMinuteSecond"))
end

local function checkTicks(ticks)
  if ticks < 0 or ticks > 3155378975999999999 then
    throw(ArgumentOutOfRangeException("ticks", "ArgumentOutOfRange_DateTimeBadTicks"))
  end
end

local function checkKind(kind) 
  if kind and (kind < 0 or kind > 2) then
    throw(ArgumentOutOfRangeException("kind"))
  end
end

function DateTime.__ctor__(this, ...)
  local len = select("#", ...)
  if len == 1 or len == 2 then
    local ticks, kind = ...
    checkTicks(ticks)
    checkKind(kind)
    this.ticks = ticks
    this.kind = kind
  elseif len == 3 then
    this.ticks = dateToTicks(...)
  elseif len == 6 or len == 7 then
    local year, month, day, hour, minute, second, kind = ...
    checkKind(kind)
    this.ticks = dateToTicks(year, month, day) + timeToTicks(hour, minute, second)
    this.kind = kind
  elseif len == 8 then
    local year, month, day, hour, minute, second, millisecond, kind = ...
    checkKind(kind)
    this.ticks = dateToTicks(year, month, day) + timeToTicks(hour, minute, second) + millisecond * 1e4
    this.kind = kind
  else
    assert(false)
  end
end

local function addTicks(this, value)
  return DateTime(this.ticks + value, this.kind)
end

local function add(this, value, scale)
  local millis = trunc(value * scale + (value >= 0 and 0.5 or -0.5))
  return addTicks(this, millis * 10000)
end

DateTime.AddTicks = addTicks

function DateTime.Add(this, ts)
  return addTicks(this, ts.ticks)
end

function DateTime.AddDays(this, days)
  return add(this, days, 86400000)
end

function DateTime.AddHours(this, hours)
  return add(this, hours, 3600000)
end

function DateTime.AddMinutes(this, minutes) 
  return add(this, minutes, 60000);
end

function DateTime.AddSeconds(this, seconds)
  return add(this, seconds, 1000)
end

function DateTime.AddMilliseconds(this, milliseconds)
  return add(this, milliseconds, 1)
end

local function getDataPart(ticks, part)
  local n = div(ticks, 864e9)
  local y400 = div(n, 146097)
  n = n - y400 * 146097
  local y100 = div(n, 36524)
  if y100 == 4 then y100 = 3 end
  n = n - y100 * 36524
  local y4 = div(n, 1461)
  n = n - y4 * 1461;
  local y1 = div(n, 365)
  if y1 == 4 then y1 = 3 end
  if part == 0 then
    return y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1
  end
  n = n - y1 * 365
  if part == 1 then return n + 1 end
  local leapYear = y1 == 3 and (y4 ~= 24 or y100 == 3)
  local days = leapYear and daysToMonth366 or daysToMonth365
  local m = div(n, 32) + 1
  while n >= days[m + 1] do m = m + 1 end
  if part == 2 then return m end
  return n - days[m] + 1
end

local function getDatePart(ticks)
  local year, month, day
  local n = div(ticks, 864e9)
  local y400 = div(n, 146097)
  n = n - y400 * 146097
  local y100 = div(n, 36524)
  if y100 == 4 then y100 = 3 end
  n = n - y100 * 36524
  local y4 = div(n, 1461)
  n = n - y4 * 1461;
  local y1 = div(n, 365)
  if y1 == 4 then y1 = 3 end
  year = y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1
  n = n - y1 * 365
  local leapYear = y1 == 3 and (y4 ~= 24 or y100 == 3)
  local days = leapYear and daysToMonth366 or daysToMonth365
  local m = div(n, 32) + 1
  while n >= days[m + 1] do m = m + 1 end
  month = m
  day = n - days[m] + 1
  return year, month, day
end

local function daysInMonth(year, month)
  if month < 1 or month > 12 then
      throw(ArgumentOutOfRangeException("month"))
  end
  local days = isLeapYear(year) and daysToMonth366 or daysToMonth365
  return days[month + 1] - days[month]
end

DateTime.DaysInMonth = daysInMonth

local function addMonths(this, months)
  if months < -120000 or months > 12000 then
      throw(ArgumentOutOfRangeException("months"))
  end
  local ticks = this.ticks
  local y, m, d
  y, m, d = getDatePart(ticks)
  local i = m - 1 + months
  if i >= 0 then
    m = i % 12 + 1
    y = y + div(i, 12)
  else
    m = 12 + (i + 1) % 12;
    y = y + div(i - 11, 12)
  end
  if y < 1 or y > 9999 then
    throw(ArgumentOutOfRangeException("months")) 
  end
  local days = daysInMonth(y, m)
  if d > days then d = days end
  return DateTime(dateToTicks(y, m, d) + ticks % 864e9, this.kind)
end

DateTime.AddMonths = addMonths

function DateTime.AddYears(this, years)
  if years < - 10000 or years > 10000 then
    throw(ArgumentOutOfRangeException("years")) 
  end
  return addMonths(this, years * 12)
end

function DateTime.SpecifyKind(this, kind)
  return DateTime(this.ticks, kind)
end

function DateTime.Subtract(this, v) 
  if getmetatable(v) == DateTime then
    return TimeSpan(this.ticks - v.ticks)
  end
  return DateTime(this.ticks - v.ticks, this.kind) 
end

function DateTime.getDay(this)
  return getDataPart(this.ticks, 3)
end

function DateTime.getDate(this)
  local ticks = this.ticks
  return DateTime(ticks - ticks % 864e9)
end

function DateTime.getDayOfWeek(this)
  return (div(this.ticks, 864e9) + 1) % 7
end

function DateTime.getDayOfYear(this)
  return getDataPart(this.ticks, 1)
end

function DateTime.getKind(this)
  return this.kind or 0
end

DateTime.getHour = TimeSpan.getHours
DateTime.getMinute = TimeSpan.getMinutes
DateTime.getSecond = TimeSpan.getSeconds
DateTime.getMillisecond = TimeSpan.getMilliseconds

function DateTime.getMonth(this)
  return getDataPart(this.ticks, 2)
end

function DateTime.getYear(this)
  return getDataPart(this.ticks, 0)
end

function DateTime.getTimeOfDay(this)
  return TimeSpan(this.ticks % 864e9)
end

function DateTime.getTicks(this)
  return this.ticks
end

local function getTimeZone()
  local now = ostime()
  return osdifftime(now, ostime(osdate("!*t", now)))
end

local timeZoneTicks = getTimeZone() * 1e7
DateTime.BaseUtcOffset = TimeSpan(timeZoneTicks)

local time = System.config.time or ostime
System.time = time
System.currentTimeMillis = function () return trunc(time() * 1000) end

function DateTime.getUtcNow()
  local seconds = time()
  local ticks = seconds * 1e7 + 621355968000000000
  return DateTime(ticks, 1)
end

function DateTime.getNow()
  local seconds = time()
  local ticks = seconds * 1e7 + timeZoneTicks + 621355968000000000
  return DateTime(ticks, 2)
end

function DateTime.getToday()
  return DateTime.getNow():getDate()
end

function DateTime.ToLocalTime(this)
  if this.kind == 2 then 
    return this
  end
  local ticks = this.ticks + timeZoneTicks
  return DateTime(ticks, 2)
end

function DateTime.ToUniversalTime(this)
  if this.kind == 1 then
    return this
  end
  local ticks = this.ticks - timeZoneTicks
  return DateTime(ticks, 1)
end

function DateTime.ToString(this)
  return format("%d/%d/%d %02d:%02d:%02d.%03d", 
    this:getYear(), this:getMonth(), this:getDay(), 
    this:getHour(), this:getMinute(), this:getSecond(), this:getMillisecond())
end

DateTime.__add = DateTime.Add
DateTime.__sub = DateTime.Subtract

function DateTime.__eq(t1, t2)
  return t1.ticks == t2.ticks
end

function DateTime.__lt(t1, t2)
  return t1.ticks < t2.ticks
end

function DateTime.__le(t1, t2)
  return t1.ticks <= t2.ticks
end

function DateTime.__inherits__()
  return { System.IComparable, System.IComparable_1(DateTime), System.IEquatable_1(DateTime) }
end

System.defStc("System.DateTime", DateTime)

local minValue = DateTime(0)
DateTime.MinValue = minValue
DateTime.MaxValue = DateTime(3155378975999999999)

function DateTime.__default__()
  return minValue
end  

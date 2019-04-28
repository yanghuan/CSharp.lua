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
local emptyFn = System.emptyFn
local ArgumentException = System.ArgumentException
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local FormatException = System.FormatException
local IndexOutOfRangeException = System.IndexOutOfRangeException

local string = string
local schar = string.char
local srep = string.rep
local slower = string.lower
local supper = string.upper
local sbyte = string.byte
local ssub = string.sub
local sfind = string.find
local smatch = string.match
local sgsub = string.gsub

local table = table
local tconcat = table.concat
local unpack = table.unpack
local setmetatable = setmetatable
local select = select
local type = type
local tonumber = tonumber

local String = string
String.traceback = emptyFn  -- make throw(str) not fail
String.getLength = System.lengthFn

local function check(s, startIndex, count)
  local len = #s
  startIndex = startIndex or 0
  if startIndex < 0 or startIndex > len then
    throw(ArgumentOutOfRangeException("startIndex"))
  end
  count = count or len - startIndex
  if count < 0 or count > len - startIndex then
    throw(ArgumentOutOfRangeException("count"))
  end
  return startIndex, count, len
end

local function ctor(_, ...)
  local len = select("#", ...)
  if len == 2 then
    local c, count = ...
    if count <= 0 then
      throw(ArgumentOutOfRangeException("count"))
    end
    return srep(schar(c), count)
  end
  local value, startIndex, length = ...
  startIndex, length = check(value, startIndex, length)
  return schar(unpack(value, startIndex + 1, startIndex + length))
end

local function compare(strA, strB, ignoreCaseOrType, cultureInfo)
  if strA == nil then
    return(strB == nil) and 0 or -1
  end

  if strB == nil then
    return 1
  end

  if ignoreCaseOrType ~= nil then
    if type(ignoreCaseOrType) == "number" then
      -- StringComparison
      if ignoreCaseOrType % 2 ~= 0 then
        strA = slower(strA)
        strB = slower(strB)
      end
    else
      -- ignoreCase
      if ignoreCaseOrType then
        strA = slower(strA)
        strB = slower(strB)
      end

      if cultureInfo then
        -- CultureInfo
        throw(System.NotSupportedException("cultureInfo is not support"))
      end
    end
  end
  if strA > strB then return 1 end
  if strA < strB then return -1 end
  return 0
end

String.Compare = compare

function String.CompareTo(this, v)
  return compare(this, v)
end

function String.CompareToObj(this, v)
  if v == nil then return 1 end
  if type(v) ~= "string" then
    throw(ArgumentException("Arg_MustBeString"))
  end
  return compare(this, v)
end

function String.Equals(this, v, comparisonType)
  return compare(this, v, comparisonType) == 0
end

function String.EqualsObj(this, v)
  if type(v) == "string" then
    return this == v
  end
  return false
end

function String.GetType(this)
  return System.typeof(String)
end

String.ToString = System.identityFn

function String.get(this, index)
  if index < 0 or index >= #this then
    throw(IndexOutOfRangeException())
  end
  return sbyte(this, index + 1)
end

function String.Concat(...)
  local t = {}
  local count = 1
  local len = select("#", ...)
  if len == 1 then
    local v = ...
    if System.isEnumerableLike(v) then
      for _, v in System.each(v) do
        t[count] = v:ToString()
        count = count + 1
      end
    else 
      return v:ToString()
    end
  else
    for i = 1, len do
      local v = select(i, ...)
      t[count] = v:ToString()
      count = count + 1
    end
  end
  return tconcat(t)
end

function String.JoinEnumerable(separator, values)
  if values == nil then throw(ArgumentNullException("values")) end
  if type(separator) == "number" then
    separator = schar(separator)
  end
  local t = {}
  local len = 1
  for _, v in System.each(values) do
    if v ~= nil then
      t[len] = v:ToString()
      len = len + 1
    end
  end
  return tconcat(t, separator)
end

function String.JoinParams(separator, ...)
  if type(separator) == "number" then
    separator = schar(separator)
  end
  local t = {}
  local len = 1
  local n = select("#", ...)
  if n == 1 then
    local values = ...
    if System.isArrayLike(values) then
      for i = 0, #values - 1 do
        local v = values:get(i)
        if v ~= nil then
          t[len] = v:ToString()
          len = len + 1
        end
      end
      return tconcat(t, separator) 
    end
  end
  for i = 1, n do
    local v = select(i, ...)
    if v ~= nil then
      t[len] = v:ToString()
      len = len + 1
    end
  end
  return tconcat(t, separator) 
end

function String.Join(separator, value, startIndex, count)
  local t = {}
  local len = 1
  if startIndex then  
    check(value, startIndex, count)
    for i = startIndex + 1, startIndex + count do
      local v = value:get(i)
      if v ~= nil then
        t[len] = v
        len = len + 1
      end
    end
  else
    for _, v in System.each(value) do
      if v ~= nil then
        t[len] = v
        len = len + 1
      end
    end
  end
  return tconcat(t, separator)
end

local function escape(s)
  return sgsub(s, "([%%%^%.])", "%%%1")
end

local function checkIndexOf(str, value, startIndex, count, comparisonType)
  if value == nil then
    throw(ArgumentNullException("value"))
  end
  startIndex, count = check(str, startIndex, count)
  str = ssub(str, startIndex + 1, startIndex + count)
  if comparisonType and comparisonType % 2 ~= 0 then
    str = slower(str)
    value = slower(value)
  end
  return str, escape(value), startIndex
end

function String.LastIndexOf(str, value, startIndex, count, comparisonType)
  if type(value) == "number" then
    value = schar(value)
  end
  str, value, startIndex = checkIndexOf(str, value, startIndex, count, comparisonType)
  local index = smatch(str, ".*()" .. value)
  if index then
    return index - 1 + startIndex
  end
  return -1
end

local function indexOfAny(str, chars, startIndex, count)
  if chars == nil then
    throw(ArgumentNullException("chars"))
  end
  startIndex, count = check(str, startIndex, count)
  str = ssub(str, startIndex + 1, startIndex + count)
  return str, "[" .. escape(schar(unpack(chars))) .. "]", startIndex
end

function String.LastIndexOfAny(str, chars, startIndex, count)
  str, chars, startIndex = indexOfAny(str, chars, startIndex, count)
  local index = smatch(str, "^.*()" .. chars)
  if index then
    return index - 1 + startIndex
  end
  return -1
end

function String.IsNullOrWhiteSpace(value)
  return value == nil or sfind(value, "^%s*$") ~= nil
end

function String.IsNullOrEmpty(value)
  return value == nil or #value == 0
end

local function simpleFormat(format, args, len, getFn)
  return (sgsub(format, "{(%d)}", function(n)
    n = tonumber(n)
    if n >= len then
      throw(FormatException())
    end
    local v = getFn(args, n)
    if v ~= nil then
      return v:ToString()
    end
    return ""
  end))
end

local function formatGetFromArray(t, n)
  return t:get(n)
end

local function formatGetFromTable(t, n)
  return t[n + 1]
end

function String.Format(format, ...)
  local len = select("#", ...)
  if len == 1 then
    local args = ...
    if System.isArrayLike(args) then
      return simpleFormat(format, args, #args, formatGetFromArray)
    end 
  end
  return simpleFormat(format, { ... }, len, formatGetFromTable)
end

function String.StartsWith(this, prefix)
  return ssub(this, 1, #prefix) == prefix
end

function String.EndsWith(this, suffix)
  return suffix == "" or ssub(this, -#suffix) == suffix
end

function String.Contains(this, value)
  if value == nil then
    throw(ArgumentNullException("value"))
  end
  return sfind(this, value) ~= nil
end

function String.IndexOfAny(str, chars, startIndex, count)
  str, chars, startIndex = indexOfAny(str, chars, startIndex, count)
  local index = sfind(str, chars)
  if index then
    return index - 1 + startIndex
  end
  return -1
end

function String.IndexOf(str, value, startIndex, count, comparisonType)
  if type(value) == "number" then
    value = schar(value)
  end
  str, value, startIndex = checkIndexOf(str, value, startIndex, count, comparisonType)
  local index = sfind(str, value)
  if index then
    return index - 1 + startIndex
  end
  return -1
end

function String.ToCharArray(str, startIndex, count)
  startIndex, count = check(str, startIndex, count)
  local t = {}
  local len = 1
  for i = startIndex + 1, startIndex + count do
    t[len] = sbyte(str, i)
    len = len + 1
  end
  return System.arrayFromTable(t, System.Char)
end

function String.Replace(this, a, b)
  if type(a) == "number" then
    a = schar(a)
    b = schar(b)
  end
  a = escape(a)
  return sgsub(this, a, b)
end

function String.Insert(this, startIndex, value) 
  if value == nil then
    throw(ArgumentNullException("value"))
  end
  startIndex = check(this, startIndex)
  return ssub(this, 1, startIndex) .. value .. ssub(this, startIndex + 1)
end

function String.Remove(this, startIndex, count) 
  startIndex, count = check(this, startIndex, count)
  return ssub(this, 1, startIndex) .. ssub(this, startIndex + 1 + count)
end

function String.Substring(this, startIndex, count)
  startIndex, count = check(this, startIndex, count)
  return ssub(this, startIndex + 1, startIndex + count)
end

local function findAny(s, strings, startIndex)
  local findBegin, findEnd, findStr
  for i = 1, #strings do
    local str = strings[i]
    local pattern = escape(str)
    local posBegin, posEnd = sfind(s, pattern, startIndex)
    if posBegin then
      if not findBegin or posBegin < findBegin then
        findBegin, findEnd, findStr = posBegin, posEnd, str
      end
    end
  end
  return findBegin, findEnd, findStr
end

String.FindAny = findAny

function String.Split(this, strings, count, options) 
  local t = {}
  local find = sfind
  if type(strings) == "table" then
    if #strings == 0 then
      return t
    end  

    if type(strings[1]) == "string" then
      find = findAny
    else
      strings = schar(unpack(strings))
      strings = escape(strings)
      strings = "[" .. strings .. "]"
    end
  elseif type(strings) == "string" then       
    strings = escape(strings)         
  else
    strings = schar(strings)
    strings = escape(strings)
  end

  local len = 1
  local startIndex = 1
  while true do
    local posBegin, posEnd = find(this, strings, startIndex)
    posBegin = posBegin or 0
    local subStr = ssub(this, startIndex, posBegin -1)
    if options ~= 1 or #subStr > 0 then
      t[len] = subStr
      len = len + 1
      if count then
        count = count -1
        if count == 0 then
          break
        end
      end  
    end
    if posBegin == 0 then
      break
    end 
    startIndex = posEnd + 1
  end   
  return System.arrayFromTable(t, String) 
end

String.ToLower = slower
String.ToUpper = supper

function String.TrimEnd(this, chars)
  if chars then
    chars = schar(unpack(chars))
    chars = escape(chars)
    chars = "(.-)[" .. chars .. "]*$"
  else 
    chars = "(.-)%s*$"
  end
  return (sgsub(this, chars, "%1"))
end

function String.TrimStart(this, chars) 
  if chars then
    chars = schar(unpack(chars))
    chars = escape(chars)
    chars = "^[" .. chars .. "]*(.-)"
  else 
    chars = "^%s*(.-)"
  end
  return (sgsub(this, chars, "%1"))
end

function String.Trim(this, chars) 
  if chars then
    chars = schar(unpack(chars))
    chars = escape(chars)
    chars = "^[" .. chars .. "]*(.-)[" .. chars .. "]*$"
  else 
    chars = "^%s*(.-)%s*$"
  end
  return (sgsub(this, chars, "%1"))
end

function String.PadLeft(this, totalWidth, paddingChar) 
  local len = #this;
  if len >= totalWidth then
    return this
  else
    paddingChar = paddingChar or 0x20;
    return srep(schar(paddingChar), totalWidth - len) .. this
  end
end

function String.PadRight(this, totalWidth, paddingChar) 
  local len = #this;
  if len >= totalWidth then
    return this
  else
    paddingChar = paddingChar or 0x20;
    return this .. srep(schar(paddingChar), totalWidth - len)
  end
end

local CharEnumerator = { getCurrent = System.getCurrent, Dispose = emptyFn  }
CharEnumerator.__index = CharEnumerator

function CharEnumerator.MoveNext(this)
  local index, s = this.index, this.s
  if index <= #s then
    this.current = sbyte(s, index)
    this.index = index + 1
    return true
  end
  return false
end

function String.GetEnumerator(this)
  return setmetatable({ s = this, index = 1 }, CharEnumerator)
end

function String.__inherits__()
  return { System.IEnumerable_1(System.Char), System.IEnumerable, System.IComparable, System.IComparable_1(String), System.IConvertible, System.IEquatable_1(String) }
end

System.define("System.String", String)
setmetatable(String, { __index = System.Object, __call = ctor })

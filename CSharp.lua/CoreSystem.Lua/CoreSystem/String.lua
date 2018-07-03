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
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local FormatException = System.FormatException
local IndexOutOfRangeException = System.IndexOutOfRangeException

local unpack = table.unpack
local string = string
local schar = string.char
local table = table
local tinsert = table.insert
local tconcat = table.concat
local setmetatable = setmetatable
local select = select
local type = type
local tonumber = tonumber

local String = string
String.traceback = System.emptyFn  -- make throw(str) not fail

function String.getLength(this)
  return #this
end

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
    return schar(c):rep(count)
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
        strA = strA:lower()
        strB = strB:lower()
      end
    else
      -- ignoreCase
      if ignoreCaseOrType then
        strA = strA:lower()
        strB = strB:lower()
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

String.ToString = tostring

function String.get(this, index)
  if index < 0 or index >= #this then
      throw(IndexOutOfRangeException())
  end
  return this:byte(index + 1)
end

function String.Concat(...)
  local t = {}
  local len = select("#", ...)
  if len == 1 then
    local v = ...
    if System.isEnumerableLike(v) then
      for _, v in System.each(array) do
        tinsert(t, v:ToString())
      end
    else 
      return v:ToString()
    end
  else
    for i = 1, len do
      local v = select(i, ...)
    tinsert(t, v:ToString())
    end
  end
  return tconcat(t)
end

function String.Join(separator, value, startIndex, count)
  local t = {}
  if startIndex then  
    check(value, startIndex, count)
    for i = startIndex + 1, startIndex + count do
      local v = value:get(i)
      if v ~= nil then
        tinsert(t, v)
      end
    end
  else
      for _, v in System.each(value) do
        if v ~= nil then
          tinsert(t, v)
        end      
      end
  end
  return tconcat(t, separator)
end

local function checkIndexOf(str, value, startIndex, count, comparisonType)
  if value == nil then
    throw(ArgumentNullException("value"))
  end
  startIndex, count = check(str, startIndex, count)
  str = str:sub(startIndex + 1, startIndex + count)
  if comparisonType and comparisonType % 2 ~= 0 then
    str = str:lower()
    value = value:lower()
  end
  return str, value, startIndex
end

function String.LastIndexOf(str, value, startIndex, count, comparisonType)
  if type(value) == "number" then
    value = schar(value)
  end
  str, value, startIndex = checkIndexOf(str, value, startIndex, count, comparisonType)
  local index = str:match(".*()" .. value)
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
  str = str:sub(startIndex + 1, startIndex + count)
  return str, "[" .. schar(unpack(chars)) .. "]", startIndex
end

function String.LastIndexOfAny(str, chars, startIndex, count)
  str, chars, startIndex = indexOfAny(str, chars, startIndex, count)
  local index = str:match("^.*()" .. chars)
  if index then
    return index - 1 + startIndex
  end
  return -1
end

function String.IsNullOrWhiteSpace(value)
  return value == nil or value:find("^%s*$") ~= nil
end

function String.IsNullOrEmpty(value)
  return value == nil or #value == 0
end

local function simpleFormat(format, args, len, getFn)
  return (format:gsub("{(%d)}", function(n)
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

function String.Format(format, ...)
  local len = select("#", ...)
  if len == 1 then
    local args = ...
    if System.isArrayLike(args) then
      return simpleFormat(format, args, #args, function (t, n)
        return t:get(n)
      end)
    end 
  end
  return simpleFormat(format, { ... }, len, function (t, n)
    return t[n + 1]
  end)
end

function String.StartsWith(this, prefix)
  return this:sub(1, #prefix) == prefix
end

function String.EndsWith(this, suffix)
  return suffix == "" or this:sub(-#suffix) == suffix
end

function String.Contains(this, value)
  if value == nil then
    throw(ArgumentNullException("value"))
  end
  return this:find(value) ~= nil
end

function String.IndexOfAny(str, chars, startIndex, count)
  str, chars, startIndex = indexOfAny(str, chars, startIndex, count)
  local index = str:find(chars)
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
  local index = str:find(value)
  if index then
    return index - 1 + startIndex
  end
  return -1
end

function String.ToCharArray(str, startIndex, count)
  startIndex, count = check(str, startIndex, count)
  local t = { }
  for i = startIndex + 1, startIndex + count do
    tinsert(t, str:byte(i))
  end
  return System.arrayFromTable(t, System.Char)
end

local function escape(s)
  return s:gsub("([%%%^%.])", "%%%1")
end

function String.Replace(this, a, b)
  if type(a) == "number" then
    a = schar(a)
    b = schar(b)
  end
  a = escape(a)
  return this:gsub(a, b)
end

function String.Insert(this, startIndex, value) 
  if value == nil then
    throw(ArgumentNullException("value"))
  end
  startIndex = check(this, startIndex)
  return this:sub(1, startIndex) .. value .. this:sub(startIndex + 1)
end

function String.Remove(this, startIndex, count) 
  startIndex, count = stringCheck(this, startIndex, count)
  return this:sub(1, startIndex) .. this:sub(startIndex + 1 + count)
end

function String.Substring(this, startIndex, count)
  startIndex, count = check(str, startIndex, count)
  return this:sub(startIndex + 1, startIndex + count)
end

local function findAny(s, strings, startIndex)
  local findBegin, findEnd, findStr
  for _, str in ipairs(strings) do
    local pattern = escape(str)
    local posBegin, posEnd = string.find(s, pattern, startIndex)
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
  local find = string.find
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

  local startIndex = 1
  while true do
    local posBegin, posEnd = find(this, strings, startIndex)
    posBegin = posBegin or 0
    local subStr = this:sub(startIndex, posBegin -1)
    if options ~= 1 or #subStr > 0 then
      tinsert(t, subStr)
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

String.ToLower = string.lower
String.ToUpper = string.upper

function String.TrimEnd(this, chars)
  if chars then
    chars = schar(unpack(chars))
    chars = escape(chars)
    chars = "(.-)[" .. chars .. "]*$"
  else 
    chars = "(.-)%s*$"
  end
  return (this:gsub(chars, "%1"))
end

function String.TrimStart(this, chars) 
  if chars then
    chars = schar(unpack(chars))
    chars = escape(chars)
    chars = "^[" .. chars .. "]*(.-)"
  else 
    chars = "^%s*(.-)"
  end
  return (this:gsub(chars, "%1"))
end

function String.Trim(this, chars) 
  if chars then
    chars = schar(unpack(chars))
    chars = escape(chars)
    chars = "^[" .. chars .. "]*(.-)[" .. chars .. "]*$"
  else 
    chars = "^%s*(.-)%s*$"
  end
  return (this:gsub(chars, "%1"))
end

function String.__inherits__()
  return { System.IComparable, System.IEnumerable, System.IComparable_1(String), System.IEnumerable_1(String), System.IEquatable_1(String) }
end

System.define("System.String", String)
setmetatable(String, { __index = System.Object, __call = ctor })
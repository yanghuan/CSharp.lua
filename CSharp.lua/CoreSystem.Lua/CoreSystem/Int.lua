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
local FormatException = System.FormatException
local OverflowException = System.OverflowException

local type = type
local tonumber = tonumber
local floor = math.floor

local Int = {}

local function compare(this, v)
  if this < v then return -1 end
  if this > v then return 1 end
  return 0
end

Int.GetHashCode = System.identityFn
Int.CompareTo = compare

function Int.CompareToObj(this, v)
  if v == null then return 1 end
  if type(v) ~= "number" then
    throw(ArgumentException("Arg_MustBeInt"))
  end
  return compare(this, v)
end

function Int.Equals(this, v)
  return this == v
end

function Int.EqualsObj(this, v)
  if type(v) ~= "number" then
    return false
  end
  return this == v
end

local function parse(s, min, max)
  if s == nil then
    return nil, 1        
  end
  local v = tonumber(s)
  if v == nil or v ~= floor(v) then
    return nil, 2
  end
  if v < min or v > max then
    return nil, 3
  end
  return v
end

local function tryParse(s, min, max)
  local v = parse(s, min, max)
  if v then
    return true, v
  end
  return false, 0
end

local function parseWithException(s, min, max)
  local v, err = parse(s, min, max)
  if v then
    return v    
  end
  if err == 1 then
    throw(ArgumentNullException())
  elseif err == 2 then
    throw(FormatException())
  else
    throw(OverflowException())
  end
end

function Int.Parse(s)
  return parseWithException(s, -2147483648, 2147483647)
end

function Int.TryParse(s)
  return tryParse(s, -2147483648, 2147483647)
end

function Int.ParseByte(s)
  return parseWithException(s, 0, 255)
end

function Int.TryParseByte(s)
  return tryParse(s, 0, 255)
end

function Int.ParseInt16(s)
  return parseWithException(s, -32768, 32767)
end

function Int.TryParseInt16(s)
  return tryParse(s, -32768, 32767)
end

function Int.ParseInt64(s)
  return parseWithException(s, -9223372036854775808, 9223372036854775807)
end

function Int.TryParseInt64(s)
  return tryParse(s, -9223372036854775808, 9223372036854775807)
end

function Int.ParseSByte(s)
  return parseWithException(s, -128, 127)
end

function Int.TryParseSByte(s)
  return tryParse(s, -128, 127)
end

function Int.ParseUInt16(s)
  return parseWithException(s, 0, 65535)
end

function Int.TryParseUInt16(s)
  return tryParse(s, 0, 65535)
end

function Int.ParseUInt32(s)
  return parseWithException(s, 0, 4294967295)
end

function Int.TryParseUInt32(s)
  return tryParse(s, 0, 4294967295)
end

function Int.ParseUInt64(s)
  return parseWithException(s, 0, 18446744073709551615)
end

function Int.TryParseUInt64(s)
  return tryParse(s, 0, 18446744073709551615)
end

function Int.__default__()
  return 0
end 

function Int.__inherits__()
  return { System.IComparable, System.IFormattable, System.IComparable_1(Int), System.IEquatable_1(Int) }
end

System.defStc("System.Int", Int)
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

local Double = {}
debug.setmetatable(0, Double)

local nan = 0 / 0
local posInf = 1 / 0
local negInf = - 1 / 0
local nanHashCode = {}

Double.NaN = nan
Double.NegativeInfinity = negInf
Double.PositiveInfinity = posInf

--http://lua-users.org/wiki/InfAndNanComparisons
local function isNaN(v)
  return v ~= v
end

Double.IsNaN = isNaN

local function compare(this, v)
  if this < v then return -1 end
  if this > v then return 1 end
  if this == v then return 0 end
  if isNaN(this) then
    return isNaN(v) and 0 or -1
  else 
    return 1
  end
end

Double.CompareTo = compare

function Double.CompareToObj(this, v)
  if v == null then return 1 end
  if type(v) ~= "number" then
    throw(ArgumentException("Arg_MustBeNumber"))
  end
  return compare(this, v)
end

local function equals(this, v)
  if this == v then return true end
  return isNaN(this) and isNaN(v)
end

Double.Equals = equals

function Double.EqualsObj(this, v)
  if type(v) ~= "number" then
    return false
  end
  return equals(this, v)
end

function Double.GetHashCode(this)
  return isNaN(this) and nanHashCode or this
end

Double.ToString = tostring

local function parse(s)
  if s == nil then
    return nil, 1
  end
  local v = tonumber(s)
  if v == nil then
    return nil, 2
  end
  return v
end

local function tryParse(s)
  local v = parse(s)
  if v then
    return true, v
  end
  return false, 0
end

local function parseWithException(s)
  local v, err = parse(s)
  if v then
    return v    
  end
  if err == 1 then
    throw(ArgumentNullException())
  else
    throw(FormatException())
  end
end

Double.Parse = parseWithException
Double.TryParse = tryParse

function Double.IsNegativeInfinity(v)
  return v == negInf
end

function Double.IsPositiveInfinity(v)
  return v == posInf
end

function Double.IsInfinity(v)
  return v == posInf or v == negInf    
end 

function Double.ParseSingle(s)
  local v = parseWithException(s)
  if v < -3.40282347E+38 or v > 3.40282347E+38 then
    throw(OverflowException())
  end
  return v
end

function Double.TryParseSingle(s)
  local v = parse(s)
  if v and v >= -3.40282347E+38 and v < 3.40282347E+38 then
    return true, v
  end
  return false, 0
end

function Double.__default__()
  return 0
end

function Double.__inherits__()
  return { System.IComparable, System.IFormattable, System.IComparable_1(Double), System.IEquatable_1(Double) }
end

System.defStc("System.Double", Double)
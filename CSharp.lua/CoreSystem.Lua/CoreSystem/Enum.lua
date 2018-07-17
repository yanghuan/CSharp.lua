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
local Int = System.Int
local Double = System.Double
local band = System.band
local ArgumentNullException = System.ArgumentNullException
local ArgumentException = System.ArgumentException

local pairs = pairs
local tostring = tostring
local tinsert = table.insert

local Enum = {}

Enum.CompareToObj = Int.CompareToObj
Enum.EqualsObj = Int.EqualsObj
Enum.__default__ = Int.__default__

local function toString(this, cls)
  for k, v in pairs(cls) do
    if v == this then
      return k
    end
  end
  return tostring(this)
end

Enum.ToString = toString
Double.ToEnumString = toString

local function hasFlag(this, flag)
  return band(this, flag) ~= 0
end

Enum.HasFlag = hasFlag
Double.HasFlag = hasFlag

function Enum.GetName(enumType, value)
  if enumType == nil then throw(ArgumentNullException("enumType")) end
  if value == nil then throw(ArgumentNullException("value")) end
  if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
  for k, v in pairs(enumType.c) do
    if v == value then
      return k
    end
  end
  throw(ArgumentException())
end

function Enum.GetNames(enumType)
  if enumType == nil then throw(ArgumentNullException("enumType")) end
  if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
  local t = {}
  for k, v in pairs(enumType.c) do
    tinsert(t, k)
  end
  return System.arrayFromTable(t, System.String)
end

function Enum.GetValues(enumType)
  if enumType == nil then throw(ArgumentNullException("enumType")) end
  if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
  local t = {}
  for k, v in pairs(enumType.c) do
    tinsert(t, v)
  end
  return System.arrayFromTable(t, Int)
end

local function tryParseEnum(enumType, value, ignoreCase)
  if enumType == nil then throw(ArgumentNullException("enumType")) end
  if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
  if value == nil then
    return
  end
  value = value:Trim()
  if #value == 0 then
    return
  end
  if ignoreCase then
    value = value:lower()
  end
  for k, v in pairs(enumType.c) do
    if ignoreCase then
      k = k:lower()
    end
    if k == value then
      return v
    end
  end
end

function Enum.Parse(enumType, value, ignoreCase)
  local result = tryParseEnum(enumType, value, ignoreCase)
  if result == nil then
    throw(ArgumentException("parse enum fail: ".. value))
  end
  return result
end

function Enum.TryParse(TEnum, value, ignoreCase)
  local result = tryParseEnum(System.typeof(TEnum), value, ignoreCase)
  if result == nil then
    return false, 0
  end
  return true, result
end

System.define("System.Enum", Enum)
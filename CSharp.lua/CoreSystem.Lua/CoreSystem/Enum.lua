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
local Number = System.Number
local band = System.band
local ArgumentNullException = System.ArgumentNullException
local ArgumentException = System.ArgumentException

local pairs = pairs
local tostring = tostring

local function toString(this, cls)
  if cls then
    for k, v in pairs(cls) do
      if v == this then
        return k
      end
    end
  end
  return tostring(this)
end

local function hasFlag(this, flag)
  return band(this, flag) ~= 0
end

Number.ToEnumString = toString
Number.HasFlag = hasFlag

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
  for k, v in pairs(enumType[1]) do
    if ignoreCase then
      k = k:lower()
    end
    if k == value then
      return v
    end
  end
end

System.define("System.Enum", {
  CompareToObj = Int.CompareToObj,
  EqualsObj = Int.EqualsObj,
  default = Int.default,
  ToString = toString,
  ToEnumString = toString,
  HasFlag = hasFlag,
  GetName = function (enumType, value)
    if enumType == nil then throw(ArgumentNullException("enumType")) end
    if value == nil then throw(ArgumentNullException("value")) end
    if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
    for k, v in pairs(enumType[1]) do
      if v == value then
        return k
      end
    end
    throw(ArgumentException())
  end,
  GetNames = function (enumType)
    if enumType == nil then throw(ArgumentNullException("enumType")) end
    if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
    local t = {}
    local count = 1
    for k, v in pairs(enumType[1]) do
      t[count] = k
      count = count + 1
    end
    return System.arrayFromTable(t, System.String)
  end,
  GetValues = function (enumType)
    if enumType == nil then throw(ArgumentNullException("enumType")) end
    if not enumType:getIsEnum() then throw(ArgumentException("Arg_MustBeEnum")) end
    local t = {}
    local count = 1
    for k, v in pairs(enumType[1]) do
      t[count] = v
      count = count + 1
    end
    return System.arrayFromTable(t, Int)
  end,
  Parse = function (enumType, value, ignoreCase)
    local result = tryParseEnum(enumType, value, ignoreCase)
    if result == nil then
      throw(ArgumentException("parse enum fail: ".. value))
    end
    return result
  end,
  TryParse = function (TEnum, value, ignoreCase)
    local result = tryParseEnum(System.typeof(TEnum), value, ignoreCase)
    if result == nil then
      return false, 0
    end
    return true, result
  end
})

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

local type = type

local Boolean = {}
debug.setmetatable(false, Boolean)

local function compare(this, v)
  if this == v then
    return 0
  elseif this == false then
    return -1     
  end
  return 1
end

Boolean.GetHashCode = System.identityFn
Boolean.CompareTo = compare

function Boolean.CompareToObj(this, v)
  if v == null then return 1 end
  if type(v) ~= "boolean" then
    throw(ArgumentException("Arg_MustBeBoolean"))
  end
  return compare(this, v)
end

function Boolean.Equals(this, v)
  return this == v
end

function Boolean.EqualsObj(this, v)
  if type(v) ~= "boolean" then
    return false
  end
  return this == v
end

function Boolean.__concat(a, b)
  if type(a) == "boolean" then
    return tostring(a) .. b
  else 
    return a .. tostring(b)
  end
end

Boolean.ToString = tostring

local function parse(s)
  if s == nil then
    return nil, 1
  end
  s = s:lower()
  if s == "true" then
    return true
  elseif s == "false" then
    return false
  end
  return nil, 2
end

function Boolean.Parse(s)
  local v, err = parse(s)
  if v == nil then
    if err == 1 then
      throw(ArgumentNullException()) 
    else
      throw(FormatException())
    end
  end
  return v
end

function Boolean.TryParse(s)
  local v = parse(s)
  if v ~= nil then
    return true, v
  end
  return false, false
end

function Boolean.__default__()
  return false
end

function Boolean.__inherits__()
  return { System.IComparable, System.IComparable_1(Boolean), System.IEquatable_1(Boolean) }
end

System.defStc("System.Boolean", Boolean)
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

local setmetatable = setmetatable
local getmetatable = getmetatable
local type = type
local pairs  = pairs
local assert = assert
local table = table
local tremove = table.remove
local tconcat = table.concat
local floor = math.floor
local ceil = math.ceil
local error = error
local select = select
local xpcall = xpcall
local rawget = rawget
local rawset = rawset
local rawequal = rawequal
local tostring = tostring
local string = string
local sfind = string.find
local ssub = string.sub
local global = _G
local prevSystem = rawget(global, "System")

local emptyFn = function() end
local nilFn = function() return nil end
local falseFn = function() return false end
local trueFn = function() return true end
local identityFn = function(x) return x end
local lengthFn = function (t) return #t end
local zeroFn = function() return 0 end
local oneFn = function() return 1 end
local equals = function(x, y) return x == y end
local getCurrent = function(t) return t.current end
local modules = {}
local usings = {}
local classes = {}
local metadatas
local System, Object, ValueType

local function new(cls, ...)
  local this = setmetatable({}, cls)
  return this, cls.__ctor__(this, ...)
end

local function throw(e, lv)
  if e == nil then e = System.NullReferenceException() end
  e:traceback(lv)
  error(e)
end

local function xpcallErr(e)
  if e == nil then
    e = System.Exception("script error")
    e:traceback()
  elseif type(e) == "string" then
    if sfind(e, "attempt to index") then
      e = System.NullReferenceException(e)
    elseif sfind(e, "attempt to divide by zero") then  
      e = System.DivideByZeroException(e)
    else
      e = System.Exception(e)
    end
    e:traceback()
  end
  return e
end

local function try(try, catch, finally)
  local ok, status, result = xpcall(try, xpcallErr)
  if not ok then
    if catch then
      if finally then
        ok, status, result = xpcall(catch, xpcallErr, status)
      else
        ok, status, result = true, catch(status)
      end
      if ok then
        if status == 1 then
          ok = false
          status = result
        end
      end
    end
  end
  if finally then
    finally()
  end
  if not ok then
    error(status)
  end
  return status, result
end

local function set(className, cls)
  local scope = global
  local starIndex = 1
  while true do
    local pos = sfind(className, "[%.+]", starIndex) or 0
    local name = ssub(className, starIndex, pos -1)
    if pos ~= 0 then
      local t = rawget(scope, name)
      if t == nil then
        if cls then
          t = {}
          rawset(scope, name, t)
        else
          return nil
        end
      end
      scope = t
      starIndex = pos + 1
    else
      if cls then
        assert(rawget(scope, name) == nil, className)
        rawset(scope, name, cls)
        return cls
      else
        return rawget(scope, name)
      end
    end
  end
end

local function multiKey(t, ...)
  local n, i, k = select("#", ...), 1
  while true do
    k = assert(select(i, ...))
    if i == n then
      break
    end
    local tk = t[k]
    if tk == nil then
      tk = {}
      t[k] = tk
    end
    t = tk
    i = i + 1
  end
  return t, k
end

local function genericName(name, ...)
  if name:byte(-2) == 95 then
    name = ssub(name, 1, -3)
  end
  local n = select("#", ...)
  local t = { name, "`", n, "[" }
  local count = 5
  local hascomma
  for i = 1, n do
    local cls = select(i, ...)
    if hascomma then
      t[count] = ","
      count = count + 1
    else
      hascomma = true
    end
    t[count] = cls.__name__
    count = count + 1
  end
  t[count] = "]"
  return tconcat(t)
end

local enumMetatable = { class = "E", default = zeroFn, __index = false, interface = false }
enumMetatable.__index = enumMetatable

local interfaceMetatable = { class = "I", default = nilFn, __index = false }
interfaceMetatable.__index = interfaceMetatable

local ctorMetatable = { __call = function (ctor, ...) return ctor[1](...) end }

local function applyExtends(cls)
  local extends = cls.__inherits__
  if extends then
    if type(extends) == "function" then
      extends = extends(global, cls)
    end
    cls.__inherits__ = nil
  end
  return extends
end

local function applyMetadata(cls)
  local metadata = cls.__metadata__
  if metadata then
    if metadatas then
      metadatas[#metadatas + 1] = function (global)
        cls.__metadata__ = metadata(global)
      end
    else
      cls.__metadata__ = metadata(global)
    end
  end
end

local function setBase(cls, kind)
  cls.__index = cls 
  cls.__call = new

  local ctor = cls.__ctor__
  if ctor and type(ctor) == "table" then
    setmetatable(ctor, ctorMetatable)
  end
  local extends = applyExtends(cls)
  applyMetadata(cls)

  if kind == "S" then
    if extends then
      cls.interface = extends
    end
    setmetatable(cls, ValueType)
  else
    if extends then      
      local base = extends[1]
      if not base then error(cls.__name__ .. "'s base is nil") end
      if base.class == "I" then
        cls.interface = extends
        setmetatable(cls, Object)
      else
        setmetatable(cls, base)
        if #extends > 1 then
          tremove(extends, 1)
          cls.interface = extends
        end
      end
    else
      setmetatable(cls, Object)
    end
  end
end

local function staticCtorSetBase(cls)
  setmetatable(cls, nil)
  local t = cls[cls]
  for k, v in pairs(t) do
    cls[k] = v
  end
  cls[cls] = nil
  local kind = cls.class
  cls.class = nil
  setBase(cls, kind)
  cls:static()
  cls.static = nil
end

local staticCtorMetatable = {
  __index = function(cls, key)
    staticCtorSetBase(cls)
    return cls[key]
  end,
  __newindex = function(cls, key, value)
    staticCtorSetBase(cls)
    cls[key] = value
  end,
  __call = function(cls, ...)
    staticCtorSetBase(cls)
    return new(cls, ...)
  end
}

local function setHasStaticCtor(cls, kind)
  local name = cls.__name__
  cls.__name__ = nil
  local t = {}
  for k, v in pairs(cls) do
    t[k] = v
    cls[k] = nil
  end  
  cls[cls] = t
  cls.__name__ = name
  cls.class = kind
  cls.__call = new
  cls.__index = cls
  setmetatable(cls, staticCtorMetatable)
end

local function defCore(name, kind, cls, generic)
  cls = cls or {}
  cls.__name__ = name
  if not generic then
    set(name, cls)
  end
  if kind == "C" or kind == "S" then
    if cls.static == nil then
      setBase(cls, kind)
    else
      setHasStaticCtor(cls, kind)
    end
  elseif kind == "I" then
    local extends = applyExtends(cls)
    if extends then 
      cls.interface = extends 
    end
    applyMetadata(cls)
    setmetatable(cls, interfaceMetatable)
  elseif kind == "E" then
    applyMetadata(cls)
    setmetatable(cls, enumMetatable)
  else
    assert(false, kind)
  end
  return cls
end

local function def(name, kind, cls, generic)
  if type(cls) == "function" then
    local mt = {}
    local fn = function(_, ...)
      local gt, gk = multiKey(mt, ...)
      local t = gt[gk]
      if t == nil then
        t = defCore(genericName(name, ...), kind, cls(...) or {}, true)
        if generic then
          setmetatable(t, generic)
        end
        gt[gk] = t
      end
      return t
    end

    local base = kind ~= "S" and Object or ValueType
    local caller = setmetatable({ __call = fn, __index = base }, base)
    if generic then
      generic.__index = generic
      generic.__call = new
    end
    return set(name, setmetatable(generic or {}, caller))
  else
    return defCore(name, kind, cls, generic)
  end
end

local function defCls(name, cls, generic)
  return def(name, "C", cls, generic) 
end

local function defInf(name, cls)
  return def(name, "I", cls)
end

local function defStc(name, cls, generic)
  return def(name, "S", cls, generic)
end

local function defEnum(name, cls)
  return def(name, "E", cls)
end

local function defArray(name, cls, Array, MultiArray)
  Array.__index = Array
  MultiArray.__index =  MultiArray
  setmetatable(MultiArray, Array)

  local mt = {}
  local function create(Array, T)
    local ArrayT = mt[T]
    if ArrayT == nil then
      ArrayT = defCore(T.__name__ .. "[]", "C", cls(T), true)
      setmetatable(ArrayT, Array)
      mt[T] = ArrayT
    end
    return ArrayT
  end

  local mtMulti = {}
  local function createMulti(MultiArray, T, dimension)
    local gt, gk = multiKey(mtMulti, T, dimension)
    local ArrayT = gt[gk]
    if ArrayT == nil then
      local name = T.__name__ .. "[" .. (","):rep(dimension - 1) .. "]"
      ArrayT = defCore(name, "C", cls(T), true)
      setmetatable(ArrayT, MultiArray)
      gt[gk] = ArrayT
    end
    return ArrayT
  end

  return set(name, setmetatable(Array, {
    __index = Object,
    __call = function (Array, T, dimension)
      if not dimension then
        return create(Array, T)
      else
        return createMulti(MultiArray, T, dimension)
      end
    end
  }))
end

local function trunc(num) 
  return num > 0 and floor(num) or ceil(num)
end

local function when(f, ...)
  local ok, r = pcall(f, ...)
  return ok and r
end

System = {
  emptyFn = emptyFn,
  falseFn = falseFn,
  trueFn = trueFn,
  identityFn = identityFn,
  lengthFn = lengthFn,
  zeroFn = zeroFn,
  oneFn = oneFn,
  equals = equals,
  getCurrent = getCurrent,
  try = try,
  when = when,
  throw = throw,
  getClass = set,
  multiKey = multiKey,
  define = defCls,
  defInf = defInf,
  defStc = defStc,
  defEnum = defEnum,
  defArray = defArray,
  enumMetatable = enumMetatable,
  trunc = trunc,
  global = global,
  classes = classes
}
if prevSystem then
  setmetatable(System, { __index == prevSystem })
end
global.System = System

local _, _, version = sfind(_VERSION, "^Lua (.*)$")
version = tonumber(version)
System.luaVersion = version

if version < 5.3 then
  local bnot, band, bor, xor, sl, sr
  local bit = rawget(global, "bit")
  if not bit then
    local ok, b = pcall(require, "bit")
    if ok then
      bit = b
    end
  end
  if bit then
    bnot, band, bor, xor, sl, sr = bit.bnot, bit.band, bit.bor, bit.bxor, bit.lshift, bit.rshift
  else
    local function disable()
      throw(System.NotSupportedException("bit operation is not enabled."))
    end
    bnot, band, bor, xor, sl, sr  = disable, disable, disable, disable, disable, disable
  end

  System.bnot = bnot
  System.band = band
  System.bor = bor
  System.xor = xor
  System.sl = sl
  System.sr = sr
  
  function System.bnotOfNull(x)
    if x == nil then
      return nil
    end
    return bnot(x)
  end

  function System.bandOfNull(x, y)
    if x == nil or y == nil then
      return nil
    end
    return band(x, y)
  end

  function System.borOfNull(x, y)
    if x == nil or y == nil then
      return nil
    end
    return bor(x, y)
  end

  function System.xorOfNull(x, y)
    if x == nil or y == nil then
      return nil
    end
    return xor(x, y)
  end

  function System.slOfNull(x, y)
    if x == nil or y == nil then
      return nil
    end
    return sl(x, y)
  end

  function System.srOfNull(x, y)
    if x == nil or y == nil then
      return nil
    end
    return sr(x, y)
  end

  function System.div(x, y) 
    if y == 0 then throw(System.DivideByZeroException(), 1) end
    return trunc(x / y)
  end

  function System.divOfNull(x, y)
    if x == nil or y == nil then
      return nil
    end
    if y == 0 then throw(System.DivideByZeroException(), 1) end
    return trunc(x / y)
  end

  function System.mod(x, y) 
    if y == 0 then throw(System.DivideByZeroException(), 1) end
    local v = x % y
    if v ~= 0 and x * y < 0 then
      return v - y
    end
    return v
  end

  function System.toUInt(v, max, mask, checked)
    if v >= 0 and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    return band(v, mask)
  end

  function System.ToUInt(v, max, mask, checked)
    v = trunc(v)
    if v >= 0 and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    if v < -2147483648 or v > 2147483647 then
      return 0
    end
    return band(v, mask)
  end

  local function toInt(v, mask, umask)
    v = band(v, mask)
    local uv = band(v, umask)
    if uv ~= v then
      v = xor(uv - 1, umask)
      if uv ~= 0 then
        v = -v
      end
    end
    return v
  end

  function System.toInt(v, min, max, mask, umask, checked)
    if v >= min and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    return toInt(v, mask, umask)
  end

  function System.ToInt(v, min, max, mask, umask, checked)
    v = trunc(v)
    if v >= min and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    if v < -2147483648 or v > 2147483647 then
      return 0
    end
    return toInt(v, mask, umask)
  end

  local function toUInt32(v)
    if v <= -2251799813685248 or v >= 2251799813685248 then  -- 2 ^ 51, Lua BitOp used 51 and 52
      throw(System.InvalidCastException()) 
    end
    v = band(v, 0xffffffff)
    local uv = band(v, 0x7fffffff)
    if uv ~= v then
      return uv + 0x80000000
    end
    return v
  end

  function System.toUInt32(v, checked)
    if v >= 0 and v <= 4294967295 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    return toUInt32(v)
  end

  function System.ToUInt32(v, checked)
    v = trunc(v)
    if v >= 0 and v <= 4294967295 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    return toUInt32(v)
  end

  function System.toInt32(v, checked)
    if v >= -2147483648 and v <= 2147483647 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    if v <= -2251799813685248 or v >= 2251799813685248 then  -- 2 ^ 51, Lua BitOp used 51 and 52
      throw(System.InvalidCastException()) 
    end
    return band(v, 0xffffffff)
  end

  function System.toInt64(v, checked) 
    if v >= -9223372036854775808 and v <= 9223372036854775807 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    throw(System.InvalidCastException()) -- 2 ^ 51, Lua BitOp used 51 and 52
  end

  function System.toUInt64(v, checked)
    if v >= 0 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    if v >= -2147483648 then
      return band(v, 0x7fffffff) + 0xffffffff80000000
    end
    throw(System.InvalidCastException()) 
  end

  function System.ToUInt64(v, checked)
    v = trunc(v)
    if v >= 0 and v <= 18446744073709551615 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    if v >= -2147483648 and v <= 2147483647 then
      v = band(v, 0xffffffff)
      local uv = band(v, 0x7fffffff)
      if uv ~= v then
        return uv + 0xffffffff80000000
      end
      return v
    end
    throw(System.InvalidCastException()) 
  end

  if table.pack == nil then
    table.pack = function(...)
      return { n = select("#", ...), ... }
    end
  end

  if table.unpack == nil then
    table.unpack = assert(unpack)
  end

  if table.move == nil then
    table.move = function(a1, f, e, t, a2)
      if a2 == nil then a2 = a1 end
      if t > f then
        t = e - f + t
        while e >= f do
          a2[t] = a1[e]
          t = t - 1
          e = e - 1
        end
      else
        while f <= e do
          a2[t] = a1[f]
          t = t + 1
          f = f + 1
        end
      end
    end
  end
else
  load[[
  local System = System
  local throw = System.throw
  local trunc = System.trunc
  
  function System.bnot(x) return ~x end 
  function System.band(x, y) return x & y end
  function System.bor(x, y) return x | y end
  function System.xor(x, y) return x ~ y end
  function System.sl(x, y) return x << y end
  function System.sr(x, y) return x >> y end
  function System.div(x, y) if x ~ y < 0 then return -(-x // y) end return x // y end
  
  function System.mod(x, y)
    local v = x % y
    if v ~= 0 and 1.0 * x * y < 0 then
      return v - y
    end
    return v
  end
  
  local function toUInt (v, max, mask, checked)  
    if v >= 0 and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 2) 
    end
    return v & mask
  end
  System.toUInt = toUInt

  function System.ToUInt(v, max, mask, checked)
    v = trunc(v)
    if v >= 0 and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 2) 
    end
    if v < -2147483648 or v > 2147483647 then
      return 0
    end
    return v & mask
  end
  
  local function toSingedInt(v, mask, umask)
    v = v & mask
    local uv = v & umask
    if uv ~= v then
      v = (uv - 1) ~ umask
      if uv ~= 0 then
        v = -v
      end
    end
    return v
  end
  
  local function toInt(v, min, max, mask, umask, checked)
    if v >= min and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 2) 
    end
    return toSingedInt(v, mask, umask)
  end
  System.toInt = toInt
  
  function System.ToInt(v, min, max, mask, umask, checked)
    v = trunc(v)
    if v >= min and v <= max then
      return v
    end
    if checked then
      throw(System.OverflowException(), 2) 
    end
    if v < -2147483648 or v > 2147483647 then
      return 0
    end
    return toSingedInt(v, mask, umask)
  end

  function System.toUInt32(v, checked)
    return toUInt(v, 4294967295, 0xffffffff, checked)
  end
  
  function System.ToUInt32(v, checked)
    v = trunc(v)
    if v >= 0 and v <= 4294967295 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    return v & 0xffffffff
  end
  
  function System.toInt32(v, checked)
    return toInt(v, -2147483648, 2147483647, 0xffffffff, 0x7fffffff, checked)
  end

  function System.toInt64(v, checked)
    return toInt(v, -9223372036854775808, 9223372036854775807, 0xffffffffffffffff, 0x7fffffffffffffff, checked)
  end

  function System.toUInt64(v, checked)
    if v >= 0 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    return (v & 0x7fffffffffffffff) + 0x8000000000000000
  end

  function System.ToUInt64(v, checked)
    v = trunc(v)
    if v >= 0 and v <= 18446744073709551615 then
      return v
    end
    if checked then
      throw(System.OverflowException(), 1) 
    end
    v = v & 0xffffffffffffffff
    local uv = v & 0x7fffffffffffffff
    if uv ~= v then
      return uv + 0x8000000000000000
    end
    return v
  end

  ]]()
end

local toUInt = System.toUInt
local toInt = System.toInt
local ToUInt = System.ToUInt
local ToInt = System.ToInt

function System.toByte(v, checked)
  return toUInt(v, 255, 0xff, checked)
end

function System.toSByte(v, checked)
  return toInt(v, -128, 127, 0xff, 0x7f, checked)
end

function System.toInt16(v, checked)
  return toInt(v, -32768, 32767, 0xffff, 0x7fff, checked)
end

function System.toUInt16(v, checked)
  return toUInt(v, 65535, 0xffff, checked)
end

function System.ToByte(v, checked)
  return ToUInt(v, 255, 0xff, checked)
end

function System.ToSByte(v, checked)
  return ToInt(v, -128, 127, 0xff, 0x7f, checked)
end

function System.ToInt16(v, checked)
  return ToInt(v, -32768, 32767, 0xffff, 0x7fff, checked)
end

function System.ToUInt16(v, checked)
  return ToUInt(v, 65535, 0xffff, checked)
end

function System.ToInt32(v, checked)
  v = trunc(v)
  if v >= -2147483648 and v <= 2147483647 then
    return v
  end
  if checked then
    throw(System.OverflowException(), 1) 
  end
  return -2147483648
end

function System.ToInt64(v, checked)
  v = trunc(v)
  if v >= -9223372036854775808 and v <= 9223372036854775807 then
    return v
  end
  if checked then
    throw(System.OverflowException(), 1) 
  end
  return -9223372036854775808
end

function System.ToSingle(v, checked)
  if v >= -3.40282347E+38 and v <= 3.40282347E+38 then
    return v
  end
  if checked then
    throw(System.OverflowException(), 1) 
  end
  if v > 0 then
    return 1 / 0 
  else
    return -1 / 0
  end
end

function System.orOfNull(x, y)
  if x == nil or y == nil then
    return nil
  end
  return x or y
end

function System.andOfNull(x, y)
  if x == nil or y == nil then
    return nil
  end
  return x and y
end

function System.xorOfBoolNull(x, y)
  if x == nil or y == nil then
    return nil
  end
  return x ~= y
end

function System.using(t, f)
  local dispose = t and t.Dispose
  if dispose ~= nil then
    local ok, status, ret = xpcall(f, xpcallErr, t)   
    dispose(t)
    if not ok then
      error(status)
    end
    return status, ret
  else
    return f(t)    
  end
end

function System.usingX(f, ...)
  local ok, status, ret = xpcall(f, xpcallErr, ...)
  for i = 1, select("#", ...) do
    local t = select(i, ...)
    if t ~= nil then
      local dispose = t.Dispose
      if dispose ~= nil then
        dispose(t)
      end
    end
  end
  if not ok then
    error(status)
  end
  return status, ret
end

function System.apply(t, f)
  f(t)
  return t
end

function System.default(T)
  return T:default()
end

function System.property(name)
  local function get(this)
    return this[name]
  end
  local function set(this, v)
    this[name] = v
  end
  return get, set
end

function System.event(name)
  local function add(this, v)
    this[name] = this[name] + v
  end
  local function remove(this, v)
    this[name] = this[name] - v
  end
  return add, remove
end

function System.new(cls, index, ...)
  local this = setmetatable({}, cls)
  return this, cls.__ctor__[index](this, ...)
end

function System.base(this)
  return getmetatable(getmetatable(this))
end

local function equalsObj(x, y)
  if x == y then
    return true
  end
  if x == nil or y == nil then
    return false
  end
  local ix = x.EqualsObj
  if ix ~= nil then
    return ix(x, y)
  end
  local iy = y.EqualsObj
  if iy ~= nil then
    return iy(y, x)
  end
  return false
end

local function compareObj(a, b)
  if a == b then return 0 end
  if a == nil then return -1 end
  if b == nil then return 1 end
  local ia = a.CompareToObj
  if ia ~= nil then
    return ia(a, b)
  end
  local ib = b.CompareToObj
  if ib ~= nil then
    return -ib(b, a)
  end
  throw(System.ArgumentException("Argument_ImplementIComparable"))
end

System.equalsObj = equalsObj
System.compareObj = compareObj

Object = defCls("System.Object", {
  __call = new,
  __ctor__ = emptyFn,
  default = nilFn,
  class = "C",
  EqualsObj = equals,
  ReferenceEquals = rawequal,
  GetHashCode = identityFn,
  EqualsStatic = equalsObj,
  GetType = false,
  ToString = function(this) return this.__name__ end
})
setmetatable(Object, { __call = new })

ValueType = {
  class = "S",
  default = function(T) 
    return T()
  end,
  __clone__ = function(this)
    if type(this) == "table" then
      local cls = getmetatable(this)
      local t = {}
      for k, v in pairs(this) do
        if type(v) == "table" and v.class == "S" then
          t[k] = v:__clone__()
        else
          t[k] = v
        end
      end
      return setmetatable(t, cls)
    end
    return this
  end,
  __copy__ = function (this, obj)
    for k, v in pairs(obj) do
      if type(v) == "table" and v.class == "S" then
        this[k] = v:__clone__()
      else
        this[k] = v
      end
    end
    for k, v in pairs(this) do
      if v ~= nil and rawget(obj, k) == nil then
        this[k] = nil
      end
    end
  end,
  EqualsObj = function (this, obj)
    if getmetatable(this) ~= getmetatable(obj) then return false end
    for k, v in pairs(this) do
      if not equalsObj(v, obj[k]) then
        return false
      end
    end
    return true
  end,
  GetHashCode = function (this)
    throw(System.NotSupportedException(this.__name__ .. " User-defined struct not support GetHashCode"), 1)
  end
}

defCls("System.ValueType", ValueType)

local AnonymousType = defCls("System.AnonymousType", {})

function System.anonymousType(t)
  return setmetatable(t, AnonymousType)
end

local pack, unpack = table.pack, table.unpack

local function tupleDeconstruct(t) 
  return unpack(t, 1, t.n)
end

local function tupleEquals(t, other)
  for i = 1, t.n do
    if not equalsObj(t[i], other[i]) then
      return false
    end
  end
  return true
end

local function tupleEqualsObj(t, obj)
  if getmetatable(obj) ~= getmetatable(t) or t.n ~= obj.n then
    return false
  end
  return tupleEquals(t, obj)
end

local function tupleCompareTo(t, other)
  for i = 1, t.n do
    local v = compareObj(t[i], other[i])
    if v ~= 0 then
      return v
    end
  end
  return 0
end

local function tupleCompareToObj(t, obj)
  if obj == nil then return 1 end
  if getmetatable(obj) ~= getmetatable(t) or t.n ~= obj.n then
    throw(System.ArgumentException())
  end
  return tupleCompareTo(t, obj)
end

local function tupleToString(t)
  local a = { "(" }
  local count = 2
  for i = 1, t.n do
    if i ~= 1 then
      a[count] = ", "
      count = count + 1
    end
    local v = t[i]
    if v ~= nil then
      a[count] = v:ToString()
      count = count + 1
    end
  end
  a[count] = ")"
  return tconcat(a)
end

local function tupleLength(t)
  return t.n
end

local function tupleGet(t, index)
  if index < 0 or index >= t.n then
    throw(System.IndexOutOfRangeException())
  end
  return t[index + 1]
end

local function tupleGetRest(t)
  return t[8]
end

local Tuple = { 
  Deconstruct = tupleDeconstruct,
  ToString = tupleToString,
  EqualsObj = tupleEqualsObj,
  CompareToObj = tupleCompareToObj,
  getLength = tupleLength,
  get = tupleGet,
  getRest = tupleGetRest
}

defCls("System.Tuple", Tuple)

function System.tuple(...)
  return setmetatable(pack(...), Tuple)
end

local ValueTuple = defStc("System.ValueTuple", {
  Deconstruct = tupleDeconstruct,
  ToString = tupleToString,
  __eq = tupleEquals,
  Equals = tupleEquals,
  EqualsObj = tupleEqualsObj,
  CompareTo = tupleCompareTo,
  CompareToObj = tupleCompareToObj,
  getLength = tupleLength,
  get = tupleGet,
  default = function()
    throw(System.NotSupportedException("not support default(T) when T is ValueTuple"))
  end
})

function System.valueTuple(...)
  return setmetatable(pack(...), ValueTuple)
end

defCls("System.Attribute", {})

local Nullable = { 
  default = nilFn,
  Value = function (this)
    if this == nil then
      throw(System.InvalidOperationException("Nullable object must have a value."))
    end
    return this
  end,
  EqualsObj = equalsObj,
  GetHashCode = function (this)
    if this == nil then
      return 0
    end
    return this:GetHashCode()
  end,
  clone = function (t)
    return t and t:__clone__()
  end
}

defStc("System.Nullable", function (T)
  return { 
    __genericT__ = T 
  }
end, Nullable)

function System.isNullable(T)
  return getmetatable(T) == Nullable
end

debug.setmetatable(nil, {
  __concat = function(a, b)
    if a == nil then
      if b == nil then
        return ""
      else
        return b
      end
    else
      return a
    end
  end,
  __add = function (a, b)
    if a == nil then
      if b == nil or type(b) == "number" then
        return nil
      end
      return b
    end
    return nil
  end,
  __sub = nilFn,
  __mul = nilFn,
  __div = nilFn,
  __mod = nilFn,
  __unm = nilFn,
  __lt = falseFn,
  __le = falseFn,

  -- lua 5.3
  __idiv = nilFn,
  __band = nilFn,
  __bor = nilFn,
  __bxor = nilFn,
  __bnot = nilFn,
  __shl = nilFn,
  __shr = nilFn,
})

function System.toString(t)
  return t ~= nil and t:ToString() or ""
end

local function pointerAddress(p)
  local address = p[3]
  if address == nil then
    address = ssub(tostring(p), 7)
    p[3] = address
  end
  return address + p[2]
end

local Pointer
local function newPointer(t, i)
  return setmetatable({ t, i }, Pointer)
end

Pointer = {
  __index = false,
  get = function(this)
    local t, i = this[1], this[2]
    return t[i]
  end,
  set = function(this, value)
    local t, i = this[1], this[2]
    t[i] = value
  end,
  __add = function(this, count)
    return newPointer(this[1], this[2] + count)
  end,
  __sub = function(this, count)
    return newPointer(this[1], this[2] - count)
  end,
  __lt = function(t1, t2)
    return pointerAddress(t1) < pointerAddress(t2)
  end,
  __le = function(t1, t2)
    return pointerAddress(t1) <= pointerAddress(t2)
  end
}
Pointer.__index = Pointer

function System.stackalloc(t)
  return newPointer(t, 1)
end

function System.import(f)
  usings[#usings + 1] = f
end

local namespace

local function defIn(kind, name, f)
  local namespaceName, isClass = namespace[1], namespace[2]
  if #namespaceName > 0 then
    name = namespaceName .. (isClass and "+" or ".") .. name
  end
  assert(modules[name] == nil, name)
  namespace[1], namespace[2] = name, kind == "C" or kind == "S"
  local t = f(namespace)
  namespace[1], namespace[2] = namespaceName, isClass
  modules[isClass and name:gsub("+", ".") or name] = function()
    return def(name, kind, t)
  end
end

namespace = {
  "",
  false,
  class = function(name, f) defIn("C", name, f) end,
  struct = function(name, f) defIn("S", name, f) end,
  interface = function(name, f) defIn("I", name, f) end,
  enum = function(name, f) defIn("E", name, f) end,
  namespace = function(name, f)
    local namespaceName = namespace[1]
    name = namespaceName .. "." .. name
    namespace[1] = name
    f(namespace)
    namespace[1] = namespaceName
  end
}

function System.namespace(name, f)
  namespace[1] = name
  f(namespace)
  namespace[1], namespace[2] = "", false
end

function System.init(namelist, conf)
  metadatas = {}

  local count = #classes + 1
  for i = 1, #namelist do
    local name = namelist[i]
    local cls = assert(modules[name], name)()
    classes[count] = cls
    count = count + 1
  end
  for i = 1, #usings do
    usings[i](global)
  end
  for i = 1, #metadatas do
    metadatas[i](global)
  end
  if conf ~= nil then
    local main = conf.Main
    if main then
      assert(not System.entryPoint)
      System.entryPoint = main
    end
  end

  modules = {}
  usings = {}
  metadatas = nil
end

return function (config)
  System.config = config or {}
end

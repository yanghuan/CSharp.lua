local System = System
local throw = System.throw

local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local ArgumentNullException = System.ArgumentNullException
local NotSupportedException = System.NotSupportedException

local type = type
local getmetatable = getmetatable
local setmetatable = setmetatable

local counts = {
  [System.Byte] = 32,
  [System.Int16] = 16,
  [System.Int32] = 8,
  [System.Single] = 8,
  [System.Double] = 4,
}

local function getCount(T)
    local count = counts[T]
    if not count then
      throw(NotSupportedException("Specified type is not supported"))
    end
    return count
end

local function default(T)
  return T(0)
end

local Vector = {
  __ctor__ = function (this, values, index)
    local count = this:Count()
    if index == nil then
      if type(values) == "number" then
        for i = 1, count do
          this[i] = values
        end
        return
      else
        index = 0
      end
    end
    if values == nil then
      throw(ArgumentNullException())
    end
    if (index < 0) or (#values - index < count) then
      throw(ArgumentOutOfRangeException("IndexMustBeLessOrEqual"))
    end
    for i = 1, count do
      this[i] = values[index + i]
    end
  end,
  get = function (this, index)
    local count = this:Count()
    if (index < 0) or (index >= count) then
      throw(ArgumentOutOfRangeException("index"))
    end
    return this[index + 1]
  end,
  Count = function (T)
    return getCount(T.__genericT__)
  end,
  One = function (T)
    return T(1)
  end,
  IsSupported = function (T)
    return counts[T.__genericT__] ~= nil
  end,
  Zero = default,
  default = default,
  ToString = function (this)
    return '<' .. table.concat(this, ', ') .. '>'
  end,
  __eq = function (a, b)
    local n = #a
    if n ~= #b then
      return false
    end
    for i = 1, n do
      if a[i] ~= b[i] then
        return false
      end
    end
    return true
  end,
  __sub = function (a, b)
    local t, n = {}, #a
    for i = 1, n do
      t[i] = a[i] - b[i]
    end
    return setmetatable(t, getmetatable(a))
  end,
  __mul = function (a, b)
    if type(a) == "number" then
      a, b = b, a
    end
    local t, n = {}, #a
    if type(b) == "number" then
      for i = 1, n do
        t[i] = a[i] * b
      end
    else
      for i = 1, n do
        t[i] = a[i] * b[i]
      end
    end
    return setmetatable(t, getmetatable(a))
  end,
  __div = function (a, b)
    local t, n = {}, #a
    if type(b) == "number" then
      for i = 1, n do
        t[i] = a[i] / b
      end
    else
      for i = 1, n do
        t[i] = a[i] / b[i]
      end
    end
    return setmetatable(t, getmetatable(a))
  end,
}

System.defStc("System.Numerics.Vector", function(T)
  return {
    __genericT__ = T,
    base = { System.IEquatable_1(T) },
    __eq = Vector.__eq,
    __sub = Vector.__sub,
    __mul = Vector.__mul,
    __div = Vector.__div,
  }
end, Vector, 1)

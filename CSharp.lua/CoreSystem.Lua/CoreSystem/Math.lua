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
local trunc = System.trunc

local math = math
local floor = math.floor
local ceil = math.ceil
local min = math.min
local max = math.max
local abs = math.abs
local log = math.log
local sqrt = math.sqrt
local ln2 = log(2)

local function acosh(a)
  return log(a + sqrt(a ^ 2 - 1))
end

local function asinh(a)
  return log(a + sqrt(a ^ 2 + 1))
end

local function atanh(a)
  return 0.5 * log((1 + a) / (1 - a))
end

local function cbrt(a)
  if a >= 0 then
    return a ^ (1 / 3)
  else
    return -abs(a) ^ (1 / 3) 
  end
end

local function copySign(a, b)
  if b >= 0 then
    return a >= 0 and a or -a
  else
    return a >= 0 and -a or a
  end
end

local function fusedMultiplyAdd(a, b, c)
  return a * b + c
end

local function ilogB(a)
  return a == 0 and -2147483648 or floor(log(abs(a)) / ln2)
end

local function log2(a)
  return log(a) / ln2
end

local function maxMagnitude(a, b)
  local x = abs(a)
  local y = abs(b)
  if x > y then
    return a
  elseif x < y then
    return b
  else
    return a > b and a or b
  end
end

local function minMagnitude(a, b)
  local x = abs(a)
  local y = abs(b)
  if x < y then
    return a
  elseif x > y then
    return b
  else
    return a < b and a or b
  end
end

local function reciprocalEstimate(a)
  return 1 / a
end

local function reciprocalSqrtEstimate(a)
  return sqrt(1 / a)
end

local function scaleB(a, b)
  return a * 2 ^ b
end

local function sinCos(a)
  return System.ValueTuple(math.sin(a), math.cos(a))
end

local function bigMul(a, b)
  return a * b
end

local function divRem(a, b)
  local remainder = a % b
  return (a - remainder) / b, remainder
end

local function round(value, digits, mode)
  local mult = 10 ^ (digits or 0)
  local i = value * mult
  if mode == 1 then
    value = trunc(i + (value >= 0 and 0.5 or -0.5))
  elseif mode == 2 then
    value = i >= 0 and floor(i) or ceil(i)
  elseif mode == 3 then
    value = floor(i)
  elseif mode == 4 then
    value = ceil(i)
  else
    value = trunc(i)
    if value ~= i then
      local dif = i - value
      if i >= 0 then
        if dif > 0.5 or (dif == 0.5 and value % 2 ~= 0) then
          value = value + 1  
        end
      else
        if dif < -0.5 or (dif == -0.5 and value % 2 ~= 0) then
          value = value - 1  
        end
      end
    end
  end
  return value / mult
end

local function sign(v)
  return v == 0 and 0 or (v > 0 and 1 or -1) 
end

local function IEEERemainder(x, y)
  if x ~= x then
    return x
  end
  if y ~= y then
    return y
  end
  local regularMod = System.mod(x, y)
  if regularMod ~= regularMod then
    return regularMod
  end
  if regularMod == 0 and x < 0 then
    return -0.0
  end
  local alternativeResult = regularMod - abs(y) * sign(x)
  local i, j = abs(alternativeResult), abs(regularMod)
  if i == j then
    local divisionResult = x / y
    local roundedResult = round(divisionResult)
    if abs(roundedResult) > abs(divisionResult) then
      return alternativeResult
    else
      return regularMod
    end
  end
  if i < j then
    return alternativeResult
  else
    return regularMod
  end
end

local function clamp(a, b, c)
  return min(max(a, b), c)
end

local function truncate(d)
  return trunc(d) * 1.0
end

local log10 = math.log10
if not log10 then
  log10 = function (x) return log(x, 10) end
  math.log10 = log10
end

local exp = math.exp
local cosh = math.cosh or function (x) return (exp(x) + exp(-x)) / 2.0 end
local pow = math.pow or function (x, y) return x ^ y end
local sinh = math.sinh or function (x) return (exp(x) - exp(-x)) / 2.0 end
local tanh = math.tanh or function (x) return sinh(x) / cosh(x) end

local Math = math
Math.Abs = abs
Math.Acos = math.acos
Math.Acosh = acosh
Math.Asin = math.asin
Math.Asinh = asinh
Math.Atan = math.atan
Math.Atanh = atanh
Math.Atan2 = math.atan2 or math.atan
Math.BigMul = bigMul
Math.Cbrt = cbrt
Math.Ceiling = ceil
Math.Clamp = clamp
Math.CopySign = copySign
Math.Cos = math.cos
Math.Cosh = cosh
Math.DivRem = divRem
Math.Exp = exp
Math.Floor = floor
Math.FusedMultiplyAdd = fusedMultiplyAdd
Math.IEEERemainder = IEEERemainder
Math.ILogB = ilogB
Math.Log = log
Math.Log10 = log10
Math.Log2 = log2
Math.Max = max
Math.MaxMagnitude = maxMagnitude
Math.Min = min
Math.MinMagnitude = minMagnitude
Math.Pow = pow
Math.ReciprocalEstimate = reciprocalEstimate
Math.ReciprocalSqrtEstimate = reciprocalSqrtEstimate
Math.Round = round
Math.ScaleB = scaleB
Math.Sign = sign
Math.Sin = math.sin
Math.SinCos = sinCos
Math.Sinh = sinh
Math.Sqrt = sqrt
Math.Tan = math.tan
Math.Tanh = tanh
Math.Truncate = truncate

System.define("System.Math", Math)
System.define("System.MathF", Math)

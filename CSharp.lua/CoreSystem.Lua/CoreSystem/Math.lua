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

local math = math
local exp = math.exp
local cosh = math.cosh or function(x) return (exp(x) + exp(-x)) / 2.0 end
local pow = math.pow or function(x, y) return x ^ y end
local sinh = math.sinh or function(x) return (exp(x) - exp(-x)) / 2.0 end
local tanh = math.tanh or function(x) return sinh(x) / cosh(x) end

local Math = math
Math.Abs = math.abs
Math.Acos = math.acos
Math.Asin = math.asin
Math.Atan = math.atan
Math.Atan2 = math.atan2 or math.atan
Math.Ceiling = math.ceil
Math.Cos = math.cos
Math.Cosh = cosh
Math.Exp = exp
Math.Floor = math.floor
Math.Log = math.log
Math.Max = math.max
Math.Min = math.min
Math.Pow = pow
Math.Sin = math.sin
Math.Sinh = sinh
Math.Sqrt = math.sqrt
Math.Tan = math.tan
Math.Tanh = tanh 

function Math.BigMul(a, b) 
  return a * b 
end

function Math.DivRem(a, b) 
  local remainder = a % b;
  return (a - remainder) / b, remainder
end

local function round(n, d, rounding)
  local m = math.pow(10, d or 0);
  n = n * m
  local sign = n > 0 and 1 or -1
  if n % 1 == 0.5 * sign then 
    local f = math.floor(n)
    return (f + (rounding == 1 and (sign > 0) or (f % 2 * sign))) / m
  end
  return math.floor(n + 0.5) / m
end

function Math.IEEERemainder(x, y)
  return x - (y * round(x / y))
end

Math.Round = round

Math.Sign = function(v) 
  return v == 0 and 0 or (v > 0 and 1 or -1) 
end

Math.Truncate = System.trunc
System.define("System.Math", Math)
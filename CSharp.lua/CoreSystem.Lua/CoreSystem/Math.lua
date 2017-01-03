local System = System

local math = math
local Math = math

Math.Abs = math.abs
Math.Acos = math.acos
Math.Asin = math.asin
Math.Atan = math.atan
Math.Atan2 = math.atan2
Math.Ceiling = math.ceil
Math.Cos = math.cos
Math.Cosh = math.cosh
Math.Exp = math.exp
Math.Floor = math.floor
Math.Log = math.log
Math.Max = math.max
Math.Min = math.min
Math.Pow = math.pow
Math.Sin = math.sin
Math.Sinh = math.sinh
Math.Sqrt = math.sqrt
Math.Tan = math.tan
Math.Tanh = math.tanh

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









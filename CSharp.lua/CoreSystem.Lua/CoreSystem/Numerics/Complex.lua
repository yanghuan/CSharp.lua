local System = System

local atan2 = math.atan2
local cos = math.cos
local sin = math.sin
local sinh = math.sinh
local cosh = math.cosh
local abs = math.abs
local sqrt = math.sqrt
local log = math.log
local atan2 = math.atan2
local exp = math.exp
local pow = math.pow

local bitXor = bitXor
local bitNot = bitNot

local IComparable = System.IComparable
local IComparable_1 = System.IComparable_1
local IEquatable_1 = System.IEquatable_1
local IFormattable = System.IFormattable

local new = function (cls, ...)
    local this = setmetatable({}, cls)
    return this, cls.__ctor__(this, ...)
end

local Complex = {}

Complex.__ctor__ = function (this, real, imaginary)
    this.m_real = real
    this.m_imaginary = imaginary
    local mt = getmetatable(this)
    mt.__unm = Complex.op_UnaryNegation
    setmetatable(this, mt)
end

Complex.__inherits__ = function (_, T)
    return { IComparable, IComparable_1(T), IEquatable_1(T), IFormattable }
end

Complex.Zero = new(Complex, 0.0, 0.0)
Complex.One = new(Complex, 1.0, 0.0)
Complex.ImaginaryOne = new(Complex, 0.0, 1.0)

Complex.getReal = function (this)
    return this.m_real
end

Complex.getImaginary = function (this)
    return this.m_imaginary
end

Complex.getMagnitude = function (this)
    return Complex.Abs(this)
end

Complex.getPhase = function (this)
    return atan2(this.m_imaginary, this.m_real)
end

Complex.FromPolarCoordinates = function (magnitude, phase)
    return new(Complex, (magnitude * cos(phase)), (magnitude * sin(phase)))
end

Complex.Negate = function (value)
    return Complex.op_UnaryNegation(value)
end

Complex.Add = function (left, right)
    return Complex.op_Addition(left, right)
end

Complex.Subtract = function (left, right)
    return Complex.op_Subtraction(left, right)
end

Complex.Multiply = function (left, right)
    return Complex.op_Multiply(left, right)
end

Complex.Divide = function (dividend, divisor)
    return Complex.op_Division(dividend, divisor)
end

Complex.op_UnaryNegation = function (value)
    return new(Complex, (- value.m_real), (- value.m_imaginary))
end

Complex.op_Addition = function (left, right)
    local rs = new(Complex, (left.m_real + right.m_real), (left.m_imaginary + right.m_imaginary))
    return rs
end

Complex.op_Subtraction = function (left, right)
    return new(Complex, (left.m_real - right.m_real), (left.m_imaginary - right.m_imaginary))
end

Complex.op_Multiply = function (left, right)
    -- Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
    local result_Realpart = (left.m_real * right.m_real) - (left.m_imaginary * right.m_imaginary)
    local result_Imaginarypart = (left.m_imaginary * right.m_real) + (left.m_real * right.m_imaginary)
    return new(Complex, result_Realpart, result_Imaginarypart)
end

Complex.op_Division = function (left, right)
    -- Division : Smith's formula.
    local a = left.m_real
    local b = left.m_imaginary
    local c = right.m_real
    local d = right.m_imaginary

    if abs(d) < abs(c) then
        local doc = d / c
        return new(Complex, (a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc))
    else
        local cod = c / d
        return new(Complex, (b + a * cod) / (d + c * cod), (- a + b * cod) / (d + c * cod))
    end
end

Complex.Abs = function (value)
    if System.Double.IsInfinity(value.m_real) or System.Double.IsInfinity(value.m_imaginary) then
        return System.Double.PositiveInfinity
    end

    -- |value| == sqrt(a^2 + b^2)
    -- sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2)
    -- Using the above we can factor out the square of the larger component to dodge overflow.


    local c = abs(value.m_real)
    local d = abs(value.m_imaginary)

    if c > d then
        local r = d / c
        return c * sqrt(1.0 + r * r)
    elseif d == 0.0 then
        return c
      -- c is either 0.0 or NaN
    else
        local r = c / d
        return d * sqrt(1.0 + r * r)
    end
end

Complex.Conjugate = function (value)
    -- Conjugate of a Complex number: the conjugate of x+i*y is x-i*y 

    return new(Complex, value.m_real, (- value.m_imaginary))
end

Complex.Reciprocal = function (value)
    -- Reciprocal of a Complex number : the reciprocal of x+i*y is 1/(x+i*y)
    if (value.m_real == 0) and (value.m_imaginary == 0) then
        return Complex.Zero
    end

    return Complex.op_Division(Complex.One, value)
end

Complex.op_Equality = function (left, right)
    return ((left.m_real == right.m_real) and (left.m_imaginary == right.m_imaginary))
end

Complex.op_Inequality = function (left, right)
    return ((left.m_real ~= right.m_real) or (left.m_imaginary ~= right.m_imaginary))
end

Complex.Equals = function (this, obj)
    if not (System.is(obj, Complex)) then
        return false
    end
    return op_Equality(this, (System.cast(class, Complex)))
end

Complex.op_Implicit = function (value)
    return new(Complex, value, 0.0)
end

Complex.op_Explicit1 = function (value)
    return new(Complex, value, 0.0)
end

Complex.ToString = function (this)
    local sb = System.StringBuilder()
    sb:Append("(")
    sb:Append(this.m_real:ToString())
    sb:Append(", ")
    sb:Append(this.m_imaginary:ToString())
    sb:Append(")")
    return sb:ToString()
end

Complex.GetHashCode = function (this)
    local n1 = 99999997
    local hash_real, hash_imaginary, final_hashcode
    if bixXor and bitNot then
        hash_real = bitXor(this.m_real:GetHashCode(), n1)
        hash_imaginary = this.m_imaginary:GetHashCode()
        final_hashcode = hash_real * bitNot(hash_imaginary)
    else
        hash_real = this.m_real:GetHashCode()^^y
        hash_imaginary = this.m_imaginary:GetHashCode()
        final_hashcode = hash_real ~ hash_imaginary
    end
    return (final_hashcode)
end

Complex.Sin = function (value)
    local a = value.m_real
    local b = value.m_imaginary
    return new(Complex, sin(a) * cosh(b), cos(a) * sinh(b))
end

Complex.Sinh = function (value)
    local a = value.m_real
    local b = value.m_imaginary
    return new(Complex, sinh(a) * cos(b), cosh(a) * sin(b))
end

Complex.Asin = function (value)
    return Complex.op_Multiply((Complex.op_UnaryNegation(Complex.ImaginaryOne)), Complex.Log(Complex.op_Addition(Complex.op_Multiply(Complex.ImaginaryOne, value), Complex.Sqrt(Complex.op_Subtraction(Complex.One, Complex.op_Multiply(value, value))))))
end

Complex.Cos = function (value)
    local a = value.m_real
    local b = value.m_imaginary
    return new(Complex, cos(a) * cosh(b), - (sin(a) * sinh(b)))
end

Complex.Cosh = function (value)
    local a = value.m_real
    local b = value.m_imaginary
    return new(Complex, cosh(a) * cos(b), sinh(a) * sin(b))
end

Complex.Acos = function (value)
    return Complex.op_Multiply((Complex.op_UnaryNegation(Complex.ImaginaryOne)), Complex.Log(Complex.op_Addition(value, Complex.op_Multiply(Complex.ImaginaryOne, Complex.Sqrt(Complex.op_Subtraction(Complex.One, (Complex.op_Multiply(value, value))))))))
end

Complex.Tan = function (value)
    return Complex.op_Division(Complex.Sin(value:__clone__()), Complex.Cos(value:__clone__()))
end

Complex.Tanh = function (value)
    return Complex.op_Division(Complex.Sinh(value:__clone__()), Complex.Cosh(value:__clone__()))
end

Complex.Atan = function (value)
    local Two = new(Complex, 2.0, 0.0)
    return Complex.op_Multiply((Complex.op_Division(Complex.ImaginaryOne, Two)), (Complex.op_Subtraction(Complex.Log(Complex.op_Subtraction(Complex.One, Complex.op_Multiply(Complex.ImaginaryOne, value))), Complex.Log(Complex.op_Addition(Complex.One, Complex.op_Multiply(Complex.ImaginaryOne, value))))))
end

Complex.Log = function (value, baseValue)
    if baseValue == nil then
        return new(Complex, (log(Complex.Abs(value:__clone__()))), (atan2(value.m_imaginary, value.m_real)))
    else
        return Complex.op_Division(Complex.Log(value:__clone__()), Complex.Log(Complex.op_Implicit(baseValue)))
    end
end

Complex.Log10 = function (value)
    local temp_log = Complex.Log(value:__clone__())
    return Complex.Scale(temp_log:__clone__(), 0.43429448190325 --[[(Double)LOG_10_INV]])
end

Complex.Exp = function (value)
    local temp_factor = exp(value.m_real)
    local result_re = temp_factor * cos(value.m_imaginary)
    local result_im = temp_factor * sin(value.m_imaginary)
    return new(Complex, result_re, result_im)
end

Complex.Sqrt = function (value)
    return Complex.FromPolarCoordinates(sqrt(Complex.getMagnitude(value)), Complex.getPhase(value) / 2.0)
end

Complex.Pow = function (value, power)

    -- complex raised to a real power
    if power.m_real == nil then
        power = new(Complex, power, 0)
    end

    if Complex.op_Equality(power, Complex.Zero) then
        return Complex.One
    end

    if Complex.op_Equality(value, Complex.Zero) then
        return Complex.Zero
    end

    local a = value.m_real
    local b = value.m_imaginary
    local c = power.m_real
    local d = power.m_imaginary

    local rho = Complex.Abs(value:__clone__())
    local theta = atan2(b, a)
    local newRho = c * theta + d * log(rho)

    local t = pow(rho, c) * pow(2.71828182845905 --[[Math.E]], - d * theta)

    return new(Complex, t * cos(newRho), t * sin(newRho))
end

Complex.Scale = function (value, factor)
    local result_re = factor * value.m_real
    local result_im = factor * value.m_imaginary
    return new(Complex, result_re, result_im)
end

System.defStc("System.Numerics.Complex", Complex)
local System = System
local bitLShift = bitLShift
local bitNot = bitNot

local HashCodeHelper = {}

function HashCodeHelper.CombineHashCodes(h1, h2)
    if bitLShift and bitNot then
        return (bitLShift(h1, 5) + h1) * bitNot(h2)
    else
        return (((h1 << 5) + h1) ~ h2)
    end
end

System.define("System.Numerics.HashCodeHelper", HashCodeHelper)
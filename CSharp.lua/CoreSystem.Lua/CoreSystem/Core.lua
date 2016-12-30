local bit = require("bit")
--local ok, socket = pcall(require, "socket") 
--local time = ok and socket.gettime or os.time
local time = os.time

local setmetatable = setmetatable
local getmetatable = getmetatable
local type = type
local ipairs = ipairs
local assert = assert
local table = table
local tinsert = table.insert
local tremove = table.remove
local tconcat = table.concat
local rawget = rawget
local floor = math.floor
local ceil = math.ceil
local error = error
local select = select
local pcall = pcall

local emptyFn = function() end
local identityFn = function(x) return x end
local equals = function(x, y) return x == y end
local genericCache = {}
local class = {}
local modules = {}
local usings = {}
local id = 0
local Object = {}

local function new(cls, ...)
    local this = setmetatable({}, cls)
    cls.__ctor__(this, ...)
    return this
end

local function throw(e, lv)
    e:traceback(lv)
    error(e)
end

local rethrow = {}

local function try(try, catch, finally)
    local ok, result = pcall(try)
    if not ok then
        if catch then
            if type(result) == "string" then
                result = System.Exception(result)
            end
            local fine, value
            if finally then
                fine, value = pcall(catch, result)
            else
                fine, value = true, catch(result)
            end
            if fine then
                if value ~= rethrow then
                    ok = true
                    result = value
                end
            else
                result = value
            end
        end
    end
    if finally then
        finally()
    end
    if not ok then
        throw(result)
    end
    return result
end

local function set(className, cls)
    local scope = _G
    local starInx = 1
    while true do
        local pos = className:find("%.", starInx) or 0
        local name = className:sub(starInx, pos -1)
        if pos ~= 0 then
            local t = scope[name]
            if t == nil then
                t = {}
                scope[name] = t
            end
            scope = t
        else
            assert(scope[name] == nil, className)
            scope[name] = cls
            break
        end
        starInx = pos + 1
    end
    return cls
end

local function getId()
    id = id + 1
    return id
end

local function defaultValOfZero()
    return 0
end

local idCreator = {}

local function genericId(id, ...) 
    idCreator[1] = id
    local len = select("#", ...)
    for i = 1, len do
        local cls = select(i, ...)
        idCreator[i + 1] = cls.__id__
    end
    return tconcat(idCreator, ".", 1, len + 1)
end

local nameCreator = {}

local function genericName(name, ...)
    nameCreator[1] = name
    nameCreator[2] = "["
    local comma, offset
    offset = 2
    for i = 1, select("#", ...) do
        local cls = select(i, ...)
        if comma then
            nameCreator[offset + 1] = ","
            nameCreator[offset + 2] = cls.__name__
            offset = offset + 2
        else
            nameCreator[offset + 1] = cls.__name__
            offset = offset + 1
            comma = true
        end
    end
    offset = offset + 1
    nameCreator[offset] = "]"
    return tconcat(nameCreator, nil, 1, offset)
end

local enumMetatable = { __kind__ = "E", __default__ = defaultValOfZero, __index = false }
enumMetatable.__index = enumMetatable

local interfaceMetatable = { __kind__ = "I", __default__ = emptyFn, __index = false }
interfaceMetatable.__index = interfaceMetatable

local function def(name, kind, cls, generic)
    if type(cls) == "function" then
        if generic then
            generic.__index = generic
            generic.__call = new
        end
        local id = getId()
        local fn = function(...)
            local trueId = genericId(id, ...)
            local t = genericCache[trueId]
            if t == nil then
                local obj = cls(...) or {}
                t = def(nil, kind, obj, genericName(name, ...))
                if generic then
                    setmetatable(t, generic)
                end
                genericCache[trueId] = t
            end
            return t
        end
        return set(name, setmetatable(generic or {}, { __call = function(_, ...) return fn(...) end, __index = Object }))
    end
    cls = cls or {}
    if name ~= nil then
        set(name, cls)
        cls.__name__ = name
    else
        cls.__name__ = generic
    end
    cls.__id__ = getId()
    if kind == "C" or kind == "S" then
        cls.__index = cls 
        cls.__call = new
        local extends = cls.__inherits__
        if extends then
            if type(extends) == "function" then
                extends = extends()
            end
            local base = extends[1]
            if base.__kind__ == "C" then
                cls.__base__ = base
                setmetatable(cls, base)
                tremove(extends, 1)
                if #extends > 0 then
                    cls.__interfaces__ = extends
                end
                if cls.__ctor__ == emptyFn then
                    local baseCtor = base.__ctor__
                    if type(baseCtor) == "table" then
                        cls.__ctor__ = baseCtor[1]
                    end
                end 
            else
                setmetatable(cls, Object)
                cls.__interfaces__ = extends
            end
            cls.__inherits__ = nil
        elseif cls ~= Object then
             setmetatable(cls, Object)
        end   
        if kind == "S" then
            cls.__kind__ = kind
        end 
        tinsert(class, cls)
    elseif kind == "I" then
        local extends = cls.__inherits__
        if extends then
            cls.__interfaces__ = extends
            cls.__inherits__ = nil
        end
        setmetatable(cls, interfaceMetatable)
    elseif kind == "E" then
        setmetatable(cls, enumMetatable)
    else
        assert(false, kind)
    end
    return cls
end

local function defCls(name, cls, genericSuper)
    return def(name, "C", cls, genericSuper) 
end

local function defInf(name, cls)
    return def(name, "I", cls)
end

local function defStc(name, cls, genericSuper)
    return def(name, "S", cls, genericSuper)
end

System = {
    null = null,
    emptyFn = emptyFn,
    identityFn = identityFn,
    equals = equals,
    try = try,
    throw = throw,
    rethrow = rethrow,
    define = defCls,
    defInf = defInf,
    defStc = defStc,
}

local System = System

System.bnot = bit.bnot
System.band = bit.band
System.bor = bit.bor
System.xor = bit.bxor
System.sl = bit.lshift
System.sr = bit.rshift
System.srr = bit.arshift

local function trunc(num) 
    return num > 0 and floor(num) or ceil(num)
end

System.trunc = trunc

function System.div(x, y) 
    if y == 0 then
        throw(System.DivideByZeroException())
    end
    return trunc(x / y);
end

function System.mod(x, y) 
    if y == 0 then
        throw(System.DivideByZeroException())
    end
    return x % y;
end

function System.strconcat(t)    
    if t == nil then return "" end
    return t:ToString()
end

System.time = time
    
function System.getTimeZone()
    local now = os.time()
    return os.difftime(now, os.time(os.date("!*t", now)))
end

function System.using(t, f, ...)
    local dispose = t.Dispose
    local ret
    if dispose == nil or dispose == emptyFn then
        ret = f(t, ...)
    else 
        local ok, err = pcall(f, t, ...)
        dispose(t)
        if not ok then
            throw(err)
        else 
            ret = err
        end
    end
    return ret
end

function System.create(t, f)
    f(t)
    return t
end

function System.CreateInstance(type, ...)
    return type.c(...)
end

function System.property(t, name, v)
    t[name] = v
    local function get(this)
        return this[name]
    end
    local function set(this, v)
        this[name] = v
    end;
    t["get" .. name] = get
    t["set" .. name] = set
    return get, set
end

function System.event(t, name, v)
    t[name] = v
    local function add(this, v)
        this[name] = System.combine(this[name], v)
    end
    local function remove(this, v)
        this[name] = System.remove(this[name], v)
    end
    t["add" .. name] = add
    t["remove" .. name] = remove
    return add, remove
end

function System.usingDeclare(f)
    tinsert(usings, f)
end

function System.init(namelist)
    for _, name in ipairs(namelist) do
       assert(modules[name], name)()
    end
    for _, f in ipairs(usings) do
        f()
    end
    for _, cls in ipairs(class) do
        local staticInit, staticCtor =  cls.__staticInit__, cls.__staticCtor__
        if staticInit then
            staticInit(cls)
            cls.__staticInit__ = nil
        end
        if staticCtor then
            staticCtor(cls)
            cls.__staticCtor__ = nil
        end
    end
    modules = {}
    class = {}
    usings = {}
end

local namespace = {}
local curName

local function namespaceDef(kind, name, f)
    if #curName > 0 then
        name = curName .. "." .. name
    end
    assert(modules[name] == nil, name)
    modules[name] = function()
       local t = f()
       def(name, kind, t)
    end
end

function namespace.class(name, f)
    namespaceDef("C", name, f) 
end

function namespace.struct(name, f)
    namespaceDef("S", name, f) 
end

function namespace.interface(name, f)
    namespaceDef("I", name, f) 
end

function namespace.enum(name, f)
    namespaceDef("E", name, f)
end

function System.namespace(name, f)
    curName = name
    f(namespace)
    curName = nil
end

local function multiNew(cls, inx, ...) 
    local this = setmetatable({}, cls)
    cls.__ctor__[inx](this, ...)
    return this
end

Object.__call = new
Object.__default__ = emptyFn
Object.__ctor__ = emptyFn
Object.__kind__ = "C"
Object.new = multiNew
Object.EqualsObj = equals
Object.ReferenceEquals = equals
Object.GetHashCode = identityFn

function Object.EqualsStatic(x, y)
    if x == y then
        return true
    end
    if x == nil or y == nil then
        return false
    end
    return x:EqualsObj(y)
end

function Object.ToString(this)
    return this.__name__
end

defCls("System.Object", Object)


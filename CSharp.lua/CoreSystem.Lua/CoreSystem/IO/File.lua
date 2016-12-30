local System = System
local throw = System.throw
local each = System.each

local open = io.open
local tinsert = table.insert

local IOException = System.define("System.IOException", {
    __tostring = System.Exception.ToString,
    __inherits__ = { System.Exception },
    __ctor__ = function(this, message, innerException) 
        System.Exception.__ctor__(this, message, innerException)
    end,
})

local File = {}

local function openFile(path, mode)
    local f, err = open(path, mode)
    if f == nil then
        throw(IOException(err))
    end
    return f
end

local function readAll(path, mode)
    local f = openFile(path, mode)
    local bytes = f:read("*all")
    f:close()
    return bytes
end

function File.ReadAllBytes(path)
    return readAll(path, "rb")
end

function File.ReadAllText(path)
   return readAll(path, "r")
end

function File.ReadAllLines(path)
    local f = openFile(path, mode)
    local t = {}
    while true do
        local line = f:read()
        if line == nil then
            break
        end
        tinsert(t, line)
    end
    f:close()
    return System.arrayFromTable(t, System.String)
end

local function writeAll(path, contents, mode)
    local f = openFile(path, mode)
    f:write(contents)
    f:close()
    return bytes
end

function File.WriteWriteAllBytes(path, contents)
    writeAll(path, contents, "wb")
end

function File.WriteAllText(path, contents)
    writeAll(path, contents, "w")
end

function File.WriteAllLines(path, contents)
    local f = openFile(path, mode)
    for _, line in each(contents) do
        if line == nil then
            f:write("\n")
        else
            f:write(line, "\n")
        end
    end
    f:close()
end

System.define("System.File", File)
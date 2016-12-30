local System = System

local io = io
local stdin = io.stdin
local stdout = io.stdout
local read = stdin.read
local write = stdout.write
local select = select
local string = string
local byte = string.byte
local char = string.char

local Console = {}

function Console.Read()
    local ch = read(stdin, 1)
    return byte(ch)
end

function Console.ReadLine()
   return read(stdin)
end

function Console.Write(v, ...)
    if select("#", ...) ~= 0 then
        v = v:Format(...)
    else
        v = v:ToString()      
    end
    write(stdout, v)     
end

function Console.WriteChar(v)
    write(stdout, char(v))     
end

function Console.WriteLine(v, ...)
    if select("#", ...) ~= 0 then
        v = v:Format(...)
    else
        v = v:ToString()      
    end
    write(stdout, v, "\n")     
end

function Console.WriteLineChar(v)
    write(stdout, char(v), "\n")     
end

System.define("System.Console", Console)
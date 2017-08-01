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
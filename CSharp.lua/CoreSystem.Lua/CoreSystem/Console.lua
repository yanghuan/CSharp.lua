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
local Format = string.Format

local function getWriteValue(v, ...)
  if select("#", ...) ~= 0 then
    v = Format(v, ...)
  elseif v ~= nil then
    v = v:ToString()      
  else
    v = ""
  end
  return v
end

System.define("System.Console", {
  Read = function ()
    local ch = read(stdin, 1)
    return byte(ch)
  end,
  ReadLine = function ()
    return read(stdin)
  end,
  Write = function (v, ...)
    write(stdout, getWriteValue(v, ...))     
  end,
  WriteChar = function (v)
    write(stdout, char(v))     
  end,
  WriteLine = function (v, ...)
    write(stdout, getWriteValue(v, ...), "\n")     
  end,
  WriteLineChar = function (v)
    write(stdout, char(v), "\n")     
  end
})

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

local randomseed = math.randomseed
local random = math.random

local Random = {}

function Random.__ctor__(this, Seed)
  if Seed then
    randomseed(Seed)
  end
end

function Random:Next0()
  return random(0, 2147483647)
end

function Random:Next1(maxValue)
  return random(1, maxValue) - 1
end

function Random:Next2(minValue, maxValue)
  return random(minValue, maxValue)
end

function Random:NextDouble()
  return random()
end
function Random:Sample()
  return random()
end

System.define("System.Random", Random)

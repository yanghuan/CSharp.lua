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
local Span = System.Span

System.MemoryExtensions = {
  AsSpan = function (array) 
    local SpanT = Span(array.__genericT__)
    return SpanT(array)
  end,
  AsBoundedSpan = function (array, start, length) 
    local SpanT = Span(array.__genericT__)
    return SpanT(array, start, length)
  end
}

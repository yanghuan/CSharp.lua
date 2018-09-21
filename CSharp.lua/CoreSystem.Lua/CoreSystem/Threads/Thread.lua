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
local throw = System.throw
local trunc = System.trunc
local Exception = System.Exception
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local NotSupportedException = System.NotSupportedException

local type = type
local setmetatable = setmetatable
local coroutine = coroutine
local ccreate = coroutine.create
local cresume = coroutine.resume
local cstatus = coroutine.status
local cyield = coroutine.yield

local ThreadStateException = System.define("System.ThreadStateException", {
  __tostring = Exception.ToString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException)
     Exception.__ctor__(this, message or "Thread is running or terminated; it cannot restart.", innerException)
  end
})

local Thread = {}
local nextThreadId = 1
local threadYield = {}

local function getThreadId()
  local id = nextThreadId
  nextThreadId = nextThreadId + 1
  return id
end

local mainThread = setmetatable({
  id = getThreadId(),
}, Thread)
local currentThread = mainThread

function Thread.getCurrentThread()
  return currentThread
end

function Thread.__ctor__(this, start)
  if start == nil then throw(ArgumentNullException("start")) end
  this.start = start
end

function Thread.getIsAlive(this)
  local co = this.co
  return co and cstatus(co) ~= "dead"
end

function Thread.ManagedThreadId(this)
  local id = this.id
  if not id then
    id = getThreadId()
    this.id = id
  end
  return id
end

function Thread.Sleep(timeout)
  if type(timeout) == "table" then
    timeout = trunc(timeout:getTotalMilliseconds())
    if timeout < -1 or timeout > 2147483647 then
      throw(ArgumentOutOfRangeException("timeout"))
    end
  end
  if currentThread == mainThread then
    throw(NotSupportedException("mainThread not support"))
  end
  cyield(timeout)
end

function Thread.Yield()
  if currentThread == mainThread then
    return false
  end
  cyield(threadYield)
  return true
end

Thread.IsBackground = false
Thread.IsThreadPoolThread = false
Thread.Priority = 2
Thread.ApartmentState = 2

local function run(t, obj)
  post(function ()
    currentThread = t
    local ok, v = cresume(t.co, obj)
    currentThread = mainThread
    if ok then
      if v == threadYield then
        run(t)  
      elseif v ~= nil then
        if v ~= -1 then
          
        end
      else   
        t.co = false
      end
    else
      print("Warning: Thread.Start" , e)
    end  
  end)
end
 
function Thread.start(this, parameter)
  if this.co ~= nil then throw(ThreadStateException()) end
  local co = ccreate(this.start)
  this.co = co
  this.start = nil
  run(this, parameter)
end

System.define("System.Thread", Thread)
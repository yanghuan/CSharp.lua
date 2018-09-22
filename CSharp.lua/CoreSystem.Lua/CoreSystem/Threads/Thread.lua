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
local post = System.post
local addTimer = System.addTimer
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

local nextThreadId = 1
local threadYield = {}
local currentThread

local function getThreadId()
  local id = nextThreadId
  nextThreadId = nextThreadId + 1
  return id
end

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
          addTimer(function () 
            run(t)
          end, v)
        end
      else   
        t.co = false
      end
    else
      print("Warning: Thread.Start" , e)
    end  
  end)
end

local Thread =  System.define("System.Thread", {
  IsBackground = false,
  IsThreadPoolThread = false,
  Priority = 2,
  ApartmentState = 2,
  getCurrentThread = function ()
    return currentThread
  end,
  __ctor__ = function (this, start)
	  if start == nil then throw(ArgumentNullException("start")) end
    this.start = start
  end,
  getIsAlive = function (this)
    local co = this.co
    return co and cstatus(co) ~= "dead"
  end,
  ManagedThreadId = function (this)
	  local id = this.id
    if not id then
      id = getThreadId()
      this.id = id
    end
    return id
  end,
  Sleep = function (timeout)
    if currentThread == mainThread then
      throw(NotSupportedException("mainThread not support"))
    end
    if type(timeout) == "table" then
      timeout = trunc(timeout:getTotalMilliseconds())
      if timeout < -1 or timeout > 2147483647 then
        throw(ArgumentOutOfRangeException("timeout"))
      end
    end
    cyield(timeout)
  end,
  Yield = function ()
    if currentThread == mainThread then
      return false
    end
    cyield(threadYield)
    return true
  end,
  Start = function (this, parameter)
    if this.co ~= nil then throw(ThreadStateException()) end
    local co = ccreate(this.start)
    this.co = co
    this.start = nil
    run(this, parameter)
  end
})

local mainThread = setmetatable({ id = getThreadId() }, Thread)
currentThread = mainThread

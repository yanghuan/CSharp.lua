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
local config = System.config
local post = config.post or function (action) action() end
local cancelPost = config.cancelPost or System.empty

System.post = post
System.cancelPost = cancelPost

-- https://github.com/facebook/folly/blob/master/folly/TimeoutQueue.cpp
local Linq = System.Linq.Enumerable
local LinkedListEvent =  System.LinkedList(System.Object) 
local TimeoutQueue = System.define("System.TimeoutQueue", (function ()
  local getNextId, Insert, Add, AddRepeating, AddRepeating1, getNextExpiration, Erase, RunOnce, 
  RunLoop, getCount, Contains, RunInternal, __ctor__
  __ctor__ = function (this)
    this.ids_ = {}
    this.events_ = LinkedListEvent()
  end
  getNextId = function (this)
    local default = this.nextId_
    this.nextId_ = default + 1
    return default
  end
  Insert = function (this, e)
    this.ids_[e.Id] = e
    local next = Linq.FirstOrDefault(this.events_, function (i)
      return i.Expiration > e.Expiration
    end)
    if next ~= nil then
      e.LinkNode = this.events_:AddBefore(next.LinkNode, e)
    else
      e.LinkNode = this.events_:AddLast(e)
    end
  end
  Add = function (this, now, delay, callback)
    return AddRepeating1(this, now, delay, 0, callback)
  end
  AddRepeating = function (this, now, interval, callback)
    return AddRepeating1(this, now, interval, interval, callback)
  end
  AddRepeating1 = function (this, now, delay, interval, callback)
    local id = getNextId(this)
    Insert(this,{
      Id = id,
      Expiration = now + delay,
      RepeatInterval = interval,
      Callback = callback
    })
    return id
  end
  getNextExpiration = function (this)
    return this.events_.Count > 0 and this.events_:getFirst().Value.Expiration or 9223372036854775807 --[[Int64.MaxValue]]
  end
  Erase = function (this, id)
    local e = this.ids_[id]
    if e then
      this.ids_[id] = nil
      this.events_:Remove(e.LinkNode)
      return true
    end
    return false
  end
  RunOnce = function (this, now)
    return RunInternal(this, now, true)
  end
  RunLoop = function (this, now)
    return RunInternal(this, now, false)
  end
  getCount = function (this)
    return this.events_.Count
  end
  Contains = function (this, id)
    return this.ids_[id] ~= nil
  end
  RunInternal = function (this, now, onceOnly)
    local nextExp
    repeat
      local expired = Linq.ToList(Linq.TakeWhile(this.events_, function (i)
        return i.Expiration <= now
      end))
      for _, e in System.each(expired) do
        Erase(this, e.Id)
        if e.RepeatInterval > 0 then
          e.Expiration = e.Expiration + e.RepeatInterval
          Insert(this, e)
        end
      end
      for _, e in System.each(expired) do
        e.Callback(e.Id, now)
      end
      nextExp = getNextExpiration(this)
    until not (not onceOnly and nextExp <= now)
    return nextExp
  end
  return {
    nextId_ = 1,
    Add = Add,
    AddRepeating = AddRepeating,
    AddRepeating1 = AddRepeating1,
    getNextExpiration = getNextExpiration,
    Erase = Erase,
    RunOnce = RunOnce,
    RunLoop = RunLoop,
    getCount = getCount,
    Contains = Contains,
    __ctor__ = __ctor__
  }
end)())

local Timer = {}





local config = System.config
local setTimeout = config.setTimeout
local clearTimeout = config.clearTimeout

System.define("System.Timer", Timer)
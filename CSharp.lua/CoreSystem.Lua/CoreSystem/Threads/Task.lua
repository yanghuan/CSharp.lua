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
local define = System.define
local defStc = System.defStc
local throw = System.throw
local try = System.try
local trunc = System.trunc
local Void = System.Void
local post = System.post
local addTimer = System.addTimer
local removeTimer = System.removeTimer
local waitTask = System.Thread.waitTask
local arrayFromTable = System.arrayFromTable
local Exception = System.Exception
local NotImplementedException = System.NotImplementedException
local ArgumentException = System.ArgumentException
local ArgumentNullException = System.ArgumentNullException
local ArgumentOutOfRangeException = System.ArgumentOutOfRangeException
local InvalidOperationException = System.InvalidOperationException
local AggregateException = System.AggregateException
local ObjectDisposedException = System.ObjectDisposedException

local type = type
local table = table
local select = select

local tremove = table.remove
local setmetatable = setmetatable
local getmetatable = getmetatable
local assert = assert
local coroutine = coroutine
local ccreate = coroutine.create
local cresume = coroutine.resume
local cyield = coroutine.yield

local TaskCanceledException = define("System.TaskCanceledException", {
  __tostring = Exception.ToString,
  __inherits__ = { Exception },
  __ctor__ = function (this, task)
    this.task = task  
    Exception.__ctor__(this, "A task was canceled.")
  end,
  getTask = function(this) 
    return this.task
  end
})

local TaskStatusCreated = 0
local TaskStatusWaitingForActivation = 1
local TaskStatusWaitingToRun = 2
local TaskStatusRunning = 3
local TaskStatusWaitingForChildrenToComplete = 4
local TaskStatusRanToCompletion = 5
local TaskStatusCanceled = 6
local TaskStatusFaulted = 7

System.defEnum("System.TaskStatus", {
  Created = TaskStatusCreated,
  WaitingForActivation = TaskStatusWaitingForActivation,
  WaitingToRun = TaskStatusWaitingToRun,
  Running = TaskStatusRunning,
  WaitingForChildrenToComplete = TaskStatusWaitingForChildrenToComplete,
  RanToCompletion = TaskStatusRanToCompletion,
  Canceled = TaskStatusCanceled,
  Faulted = TaskStatusFaulted,
})

local UnobservedTaskExceptionEventArgs = define("System.UnobservedTaskExceptionEventArgs", {
  __ctor__ = function (this, exception)
    this.exception = exception
  end,
  SetObserved = function (this)
    this.observed = true
  end,
  getObserved = function (this)
    if this.observed then
      return true
    end
    return false
  end,
  getException = function (this)
    return this.exception
  end
})

local unobservedTaskException
local function publishUnobservedTaskException(sender, ueea)
  local handler = unobservedTaskException
  if handler then
    handler(sender, ueea)
  end
end

local TaskScheduler = define("System.TaskScheduler", {
  addUnobservedTaskException = function (value)
    unobservedTaskException = unobservedTaskException + value
  end,
  removeUnobservedTaskException = function (value)
    unobservedTaskException = unobservedTaskException - value
  end
})

local TaskExceptionHolder = {
  __index = false,
  __gc = function (this)
    if not this.isHandled then
      local e = this.exception
      if e then
        local ueea = UnobservedTaskExceptionEventArgs(e)
        publishUnobservedTaskException(this.task, ueea)
        if not ueea.observed then
          print("Warning: TaskExceptionHolder" , e)
        end
      end
    end
  end
}
TaskExceptionHolder.__index = TaskExceptionHolder

local function newTaskExceptionHolder(task, exception) 
  return setmetatable({ task = task, exception = exception }, TaskExceptionHolder)
end

local function getException(task, await)
  local holder = task.data
  if not holder.isHandled then
    holder.isHandled = true
  end
  local e = holder.exception
  if await then
    return e
  end
  return AggregateException(e)
end

local Task
local nextTaskId = 1
local currentTask
local completedTask

local function getNewId()
  local id = nextTaskId
  nextTaskId = nextTaskId + 1
  return id
end

local function getId(this)
  local id = this.id
  if id == nil then
    id = getNewId()
    this.id = id
  end
  return id 
end

local function isCompleted(this)
  local status = this.status
  return status == TaskStatusRanToCompletion or status == TaskStatusFaulted or status == TaskStatusCanceled
end

local function newTask(status, data)
  return setmetatable({ status = status, data = data }, Task)
end

local function fromResult(result)
  return newTask(TaskStatusRanToCompletion, result)
end

local function fromCanceled(cancellationToken)
  if cancellationToken and cancellationToken:getIsCancellationRequested() then 
    throw(ArgumentOutOfRangeException("cancellationToken"))
  end
  return newTask(TaskStatusCanceled, cancellationToken)
end

local function fromException(exception)
  local data = newTaskExceptionHolder(false, exception)
  local t = newTask(TaskStatusFaulted, data) 
  data.task = t
  return t
end

local function getCompletedTask()
  local t = completedTask
  if t == nil then
    t = fromResult()
    completedTask = t
  end
  return t
end

local function trySetComplete(this, status, data)
  if isCompleted(this) then
    return false
  end

  this.status = status
  this.data = data

  local continueActions = this.continueActions
  if continueActions then
    for i = 1, #continueActions do
      continueActions[i](this)
    end
    this.continueActions = nil
  end
  return true
end

local function trySetResult(this, result)
  return trySetComplete(this, TaskStatusRanToCompletion, result)
end

local function trySetException(this, exception)
  if this.data == Void then
    throw(exception)
  end
  return trySetComplete(this, TaskStatusFaulted, newTaskExceptionHolder(this, exception))
end

local function trySetCanceled(this, cancellationToken)
  return trySetComplete(this, TaskStatusCanceled, cancellationToken)
end

local function newWaitingTask(isVoid)
  return newTask(TaskStatusWaitingForActivation, isVoid and Void)
end

local function getContinueActions(task) 
  local continueActions = task.continueActions
  if continueActions == nil then
    continueActions = {}
    task.continueActions = continueActions
  end
  return continueActions
end

local function addContinueAction(task, f)
  local continueActions = getContinueActions(task)
  continueActions[#continueActions + 1] = assert(f)
end

local function checkTasks(...)
  local tasks
  local n = select("#", ...)
  if n == 1 then
    local args = ...
    if args == nil then throw(ArgumentNullException("tasks")) end
    if System.isArrayLike(args) then
      tasks = args
    elseif System.isEnumerableLike(args) then
      tasks = System.Array.toArray(args)
    end
  end
  if not tasks then
    tasks = System.Array(Task)(...)
  end
  for i = 1, #tasks do
    if tasks[i] == System.null then
      throw(ArgumentNullException())
    end
  end
  return tasks
end

local function getDelay(delay)
  if type(delay) == "table" then
    delay = trunc(delay:getTotalMilliseconds())
    if delay < -1 or delay > 2147483647 then
      throw(ArgumentOutOfRangeException("delay"))
    end
  elseif delay < -1 then
    throw(ArgumentOutOfRangeException("millisecondsDelay"))  
  end
  return delay
end

local waitToken = {}
local function getResult(this, await)
  local status = this.status
  if status == TaskStatusRanToCompletion then
    return this.data
  elseif status == TaskStatusFaulted then
    throw(getException(this, await))
  elseif status == TaskStatusCanceled then
    local e = TaskCanceledException(this)
    if not await then e = AggregateException(e) end
    throw(e)
  end
  return waitToken
end

local function awaitTask(t, task)
  assert(t.co)
  addContinueAction(task, function (task)
    local status = task.status
    local ok, v
    if status == TaskStatusRanToCompletion then
      ok, v = true, task.data
    elseif status == TaskStatusFaulted then
      ok, v = false, getException(task, true)
    elseif status == TaskStatusCanceled then
      ok, v = false, TaskCanceledException(task)
    else
      assert(false)
    end
    ok, v = cresume(t.co, ok, v)
    if not ok then
      assert(trySetException(t, v))
    end
  end)
  local ok, v = cyield()
  if ok then
    return v
  else
    throw(v)
  end
end

local factory = {
  StartNew = function (f, state)
    local t = newWaitingTask()
    post(function ()
      try(function ()
        assert(trySetResult(t, f(state)))
      end, function (e)
        assert(trySetException(t, e))
      end)
    end)
    return t
  end
}

Task = define("System.Threading.Tasks.Task", {
  Dispose = System.emptyFn,
  __ctor__ = function (this, action, state)
    if action == nil then throw(ArgumentNullException("action")) end
    this.status = TaskStatusCreated
    this.data = function ()
      return action(state)
    end
  end,
  getId = getId,
  getCurrentId = function ()
    local t = currentTask
    if t then
      return getId(t)
    end
  end,
  getFactory = function ()
    return factory
  end,
  getStatus = function (this)
    return this.status
  end,
  getException = function (this)
    if this.status == TaskStatusFaulted then
      return getException(this)
    end
    return nil
  end,
  getResult = function (this)
    local result = getResult(this)
    if result == waitToken then
      waitTask(getContinueActions(this))
      result = getResult(this)
      assert(result ~= waitToken)
    end
    return result
  end,
  getIsCompleted = isCompleted,
  getIsCanceled = function (this)
    return this.status == TaskStatusCanceled
  end,
  getIsFaulted = function (this)
    return this.status == TaskStatusFaulted
  end,
  FromResult = fromResult,
  FromCanceled = fromCanceled,
  FromException = fromException,
  getCompletedTask = getCompletedTask,
  Delay = function (delay, cancellationToken)
    delay = getDelay(delay)

    if cancellationToken and cancellationToken:getIsCancellationRequested() then
      return fromCanceled(cancellationToken)
    elseif delay == 0 then
      return getCompletedTask()
    end

    local t = newWaitingTask()
    local timerId, registration  

    if cancellationToken and cancellationToken:getCanBeCanceled() then
      registration = cancellationToken:Register(function ()
        local success = trySetCanceled(t, cancellationToken)
        if success and timerId then
          removeTimer(timerId)
        end
      end)
    end

    if delay ~= -1 then
      timerId = addTimer(function ()
        local success = trySetResult(t)
        if success and registration then
          registration:Dispose()
        end
      end, delay)
    end

    return t
  end,
  Run = function (f, cancellationToken)
    local t = Task(f) 
    t:Start()
    return t
  end,
  WhenAll = function (...)
    local tasks = checkTasks(...)
    local count = #tasks
    if count == 0 then
      return getCompletedTask()
    end
    local result, exceptions, cancelled = {}, {}
    local t = newWaitingTask()
    local function f(task)
      local status = task.status
      if status == TaskStatusRanToCompletion then
        result[#result + 1] = task.data
      elseif status == TaskStatusFaulted then
        local exception = getException(task, true)
        exceptions[#exceptions + 1] = exception
      elseif status == TaskStatusCanceled then
        cancelled = true
      end
      count = count - 1
      if count == 0 then
        if #exceptions > 0 then
          trySetException(t, arrayFromTable(exceptions, Exception))
        elseif cancelled then
          trySetCanceled(t)
        else
          trySetResult(arrayFromTable(result, getmetatable(result[1])))
        end
      end
    end
    for i = 1, count do
      local task = tasks[i]
      if isCompleted(task) then
        post(function ()
          f(task)
        end)
      else
        addContinueAction(task, f)
      end
    end
    return t
  end,
  WhenAny = function (...)
    local tasks = checkTasks(...)
    local count = #tasks
    if count == 0 then
      throw(ArgumentException())
    end
    local t = newWaitingTask()
    local function f(task)
      local status = task.status
      if status == TaskStatusRanToCompletion then
        trySetResult(t, task)
      elseif status == TaskStatusFaulted then
        trySetException(t, getException(task))
      elseif status == TaskStatusCanceled then
        trySetCanceled(t)
      end
    end
    for i = 1, count do
      local task = tasks[i]
      if isCompleted(task) then
        post(function ()
          f(task)
        end)
      else
        addContinueAction(task, f)
      end
    end
    return t
  end,
  ContinueWith = function (this, continuationAction)
    if continuationAction == nil then throw(ArgumentNullException("continuationAction")) end
    local t = newWaitingTask()
    local function f(task)
      try(function ()
        t.status = TaskStatusRunning
        assert(trySetResult(t, continuationAction(task)))
      end, function (e)
        assert(trySetException(t, e))
      end)
    end
    if isCompleted(this) then
      post(function ()
        f(this)
      end)
    else
      addContinueAction(this, f)
    end
    return t
  end,
  Start = function (this)
    if this.status ~= TaskStatusCreated then throw(InvalidOperationException("Task was already started.")) end
    this.status = TaskStatusWaitingToRun
    post(function ()
      try(function ()
        this.status = TaskStatusRunning
        assert(trySetResult(this, this.data()))
      end, function (e)
        assert(trySetException(this, e))
      end)
    end)
  end,
  Wait = function (this)
    waitTask(getContinueActions(this))
  end,
  await = function (this, task)
    local result = getResult(task, true)
    if result ~= waitToken then
      return result
    end
    return awaitTask(this, task)
  end
})

local TaskT_TransitionToFinal_AlreadyCompleted = "An attempt was made to transition a task to a final state when it had already completed."
local TaskCompletionSource = define("System.Threading.Tasks.TaskCompletionSource", {
  __ctor__ = function (this)
    this.task = newWaitingTask()
  end,
  getTask = function (this)
    return this.task
  end,
  SetCanceled = function (this)
    if not trySetCanceled(this.task) then
      throw(InvalidOperationException(TaskT_TransitionToFinal_AlreadyCompleted))
    end
  end,
  SetException = function (this, exception)
    if exception == nil then throw(ArgumentNullException("exception")) end
    if not trySetException(this.task, exception) then
      throw(InvalidOperationException(TaskT_TransitionToFinal_AlreadyCompleted))
    end
  end,
  SetResult = function (this, result)
    if not trySetResult(this.task, result) then
      throw(InvalidOperationException(TaskT_TransitionToFinal_AlreadyCompleted))
    end
  end,
  TrySetCanceled = trySetCanceled,
  TrySetException = trySetException,
  TrySetResult = trySetResult
})

local CancellationTokenRegistration = defStc("System.Threading.CancellationTokenRegistration", (function ()
  local function unregister(this)
    local token = this.token
    if token then
      local f = this.f
      if f then
        this.f = nil
        return token.source:unRegister(f)
      end
    end
    return false
  end
  return {
    __inherits__ =  function(_, T)
      return { System.IDisposable, System.IEquatable_1(T) }
    end,
    __ctor__ = function (this, token, f)
      if not token then
        return
      end
      this.token = token
      this.f = f
    end,
    getToken = function (this)
      return this.token
    end,
    Equals = System.equals,
    Unregister = unregister,
    Dispose = unregister
  }
end)())

local canceledSource

local OperationCanceledException = define("System.OperationCanceledException", {
  __tostring = Exception.ToString,
  __inherits__ = { System.SystemException },
  __ctor__ = function (this, message, innerException, token)
    Exception.__ctor__(this, message or "The operation was canceled.", innerException)
    this.tokne = token
  end,
  getCancellationToken = function (this)
    return this.token
  end
})

local CancellationToken 
CancellationToken = defStc("System.Threading.CancellationToken", {
  __ctor__ = function (this, canceled)
    if canceled == nil then
      return
    end
    if canceled == true then
      this.source = canceledSource
    elseif canceled then
      this.source = canceled
    end
  end,
  getCanBeCanceled = function (this)
    return this.source ~= nil
  end,
  getIsCancellationRequested = function (this)
    local source = this.source
    if source then
      return source:getIsCancellationRequested()
    end
    return false
  end,
  getNone = function ()
    return CancellationToken()
  end,
  Equals = System.equals,
  Register = function (this, callback, state)
    local source = this.source
    if source then
      if not source:getIsCancellationRequested() then
        local function f()
          callback(state)
        end
        this.source:register(f)
        return CancellationTokenRegistration(this, f)
      end
      callback(state)
    end
    return CancellationTokenRegistration()
  end,
  ThrowIfCancellationRequested = function (this)
    if this:getIsCancellationRequested() then
      throw(OperationCanceledException())
    end
  end
})

local CancellationTokenSource 
CancellationTokenSource = define("System.Threading.CancellationTokenSource", (function ()
  local function clean(this)
    local timerId = this.timerId
    if timerId then
      removeTimer(timerId)
    end
    local links = this.links
    if links then
      for i = 1, #links do
        links[i]:Dispose()
      end
    end
  end
  return  {
    state = 0,
    __inherits__ = { System.IDisposable },
    __ctor__  = function (this, delay)
      if delay then
        delay = getDelay(delay)
        if delay == 0 then
          this.state = 1
        else
          this.timerId = addTimer(function ()
            this.Cancel()
          end, delay)
        end
      end
    end,
    Cancel = function (this, throwOnFirstException)
      if this.disposed then throw(ObjectDisposedException()) end
      if this.state == 1  then
        return
      end
      clean(this)
      this.state = 1
      local actions = this.continueActions
      if actions then
        local t = {}
        for i = 1, #actions do
          try(function ()
            actions[i]()          
          end, function (e)
            if throwOnFirstException then
              throw(e)
            end
            t[#t + 1] = e
          end)
        end
        if #t > 0 then
          throw(AggregateException(arrayFromTable(t, Exception)))
        end
      end
    end,
    CancelAfter = function (this, delay)
      if this.disposed then throw(ObjectDisposedException()) end
      delay = getDelay(delay)
      if this.state == 1  then
        return
      end
      local timerId = this.timerId
      if timerId then
        removeTimer(timerId)
      end
      this.timerId = addTimer(function ()
        this:Cancel()
      end, delay)
    end,
    Dispose = function (this)
      if this.disposed then
        return
      end
      clean(this)
      this.disposed = true
    end,
    getIsCancellationRequested = function (this)
      return this.state == 1
    end,
    getToken = function (this)
      local t = this.token
      if not t then
        t = CancellationToken(this)
        this.token = t
      end
      return t
    end,
    register = addContinueAction,
    unRegister = function (this, f)
      local actions = this.continueActions
      if actions then
        for i = 1, #actions do
          if actions[i] == f then
            tremove(actions, i)
            return true
          end
        end
      end
      return false
    end,
    CreateLinkedTokenSource = function (...)
      local cts, links, count = CancellationTokenSource(), {}, 1
      cts.links = links
      local n = select("#", ...)
      if n == 1 then
        local args = ...
        if System.isArrayLike(args) then
          for i = 1, #args do
            links[count] = args[i]:Register(cts.Cancel, cts)
            count = count + 1 
          end
          return cts
        end
      end
      for i = 1, n do
        local token = select(i, ...)
        links[count] = token:Register(cts.Cancel, cts)
        count = count + 1 
      end
      return cts
    end
  }
end)())

canceledSource = setmetatable({ state = 1 }, CancellationTokenSource)

System.Task = Task
System.TaskCompletionSource = TaskCompletionSource
System.CancellationTokenRegistration = CancellationTokenRegistration
System.CancellationToken = CancellationToken
System.CancellationTokenSource = CancellationTokenSource

local taskCoroutinePool = {}
local function taskCoroutineCreate(t, f)
  local co = tremove(taskCoroutinePool)
  if co == nil then
    co = ccreate(function (...)
      local r = f(t, ...)
      assert(trySetResult(t, r))
      while true do
        t = nil
        f = nil
        taskCoroutinePool[#taskCoroutinePool + 1] = co
        t, f = cyield()
        r = f(t, cyield())
        assert(trySetResult(t, r))
      end
    end)
    t.co = co
  else
    t.co = co
    cresume(co, t, f)
  end
  return co
end

function System.async(f, void, ...)
  local t = newWaitingTask(void)
  local co = taskCoroutineCreate(t, f)
  local ok, v = cresume(co, ...)
  if not ok then
    assert(trySetException(t, v))
  end
  return t
end

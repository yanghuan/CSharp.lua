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
local Object = System.Object

local traceback = debug.traceback
local tconcat = table.concat

local function getMessage(this)
  return this.message or ("Exception of type '%s' was thrown."):format(this.__name__)
end

local function toString(this)
  local t = { this.__name__ }
  local count = 2
  local message, innerException, stackTrace = getMessage(this), this.innerException, this.errorStack
  t[count] = ": "
  t[count + 1] = message
  count = count + 2
  if innerException then
    t[count] = "---> "
    t[count + 1] = innerException:ToString()
    count = count + 2
  end
  if stackTrace then
    t[count] = stackTrace
  end
  return tconcat(t)
end

local function ctorOfException(this, message, innerException)
  this.message = message
  this.innerException = innerException
end

local Exception = define("System.Exception", {
  __tostring = toString,
  __ctor__ = ctorOfException,
  ToString = toString,
  getMessage = getMessage,
  
  getInnerException = function(this) 
    return this.innerException
  end,

  getStackTrace = function(this) 
    return this.errorStack
  end,

  getData = function (this)
    local data = this.data
    if not data then
      data = System.Dictionary(Object, Object)()
      this.data = data
    end
    return data
  end,

  traceback = function(this, lv)
    this.errorStack = traceback("", lv and lv + 3 or 3)
  end
})

local ArgumentException = define("System.ArgumentException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, paramName, innerException) 
    ctorOfException(this, message or "Value does not fall within the expected range.", innerException)
    this.paramName = paramName
  end,

  getParamName = function(this) 
    return this.paramName
  end
})

define("System.ArgumentNullException", {
  __tostring = toString,
  __inherits__ = { ArgumentException },

  __ctor__ = function(this, paramName, message, innerException) 
    if not message then
      message = "Value cannot be null."
      if paramName then 
        message = message .. "\nParameter name = " .. paramName
      end
    end
    ArgumentException.__ctor__(this, message, paramName, innerException)
  end
})

define("System.ArgumentOutOfRangeException", {
  __tostring = toString,
  __inherits__ = { ArgumentException },

  __ctor__ = function(this, paramName, message, innerException, actualValue) 
    if not message then
      message = "Value is out of range."
      if paramName then
        message = message .. "\nParameter name = " .. paramName
      end
    end
    ArgumentException.__ctor__(this, message, paramName, innerException)
    this.actualValue = actualValue
  end,

  getActualValue = function(this) 
    return this.actualValue
  end
})

define("System.IndexOutOfRangeException", {
   __tostring = toString,
   __inherits__ = { Exception },

   __ctor__ = function (this, message, innerException)
    ctorOfException(this, message or "Index was outside the bounds of the array.", innerException)
  end
})

define("System.CultureNotFoundException", {
  __tostring = toString,
  __inherits__ = { ArgumentException },

  __ctor__ = function(this, paramName, invalidCultureName, message, innerException, invalidCultureId) 
    if not message then 
      message = "Culture is not supported."
      if paramName then
        message = message .. "\nParameter name = " .. paramName
      end
      if invalidCultureName then
        message = message .. "\n" .. invalidCultureName .. " is an invalid culture identifier."
      end
    end
    ArgumentException.__ctor__(this, message, paramName, innerException)
    this.invalidCultureName = invalidCultureName
    this.invalidCultureId = invalidCultureId
  end,

  getInvalidCultureName = function(this)
    return this.invalidCultureName
  end,

  getInvalidCultureId = function(this) 
    return this.invalidCultureId
  end
})

define("System.KeyNotFoundException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Key not found.", innerException)
  end
})

local ArithmeticException = define("System.ArithmeticException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Overflow or underflow in the arithmetic operation.", innerException)
  end
})

define("System.DivideByZeroException", {
  __tostring = toString,
  __inherits__ = { ArithmeticException },

  __ctor__ = function(this, message, innerException) 
    ArithmeticException.__ctor__(this, message or "Division by 0.", innerException)
  end
})

define("System.OverflowException", {
  __tostring = toString,
  __inherits__ = { ArithmeticException },

  __ctor__ = function(this, message, innerException) 
    ArithmeticException.__ctor__(this, message or "Arithmetic operation resulted in an overflow.", innerException)
  end
})

define("System.FormatException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Invalid format.", innerException)
  end
})

define("System.InvalidCastException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "The cast is not valid.", innerException)
  end
})

define("System.InvalidOperationException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Operation is not valid due to the current state of the object.", innerException)
  end
})

define("System.NotImplementedException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "The method or operation is not implemented.", innerException)
  end
})

define("System.NotSupportedException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Specified method is not supported.", innerException)
  end
})

define("System.NullReferenceException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Object reference not set to an instance of an object.", innerException)
  end
})

define("System.RankException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Attempted to operate on an array with the incorrect number of dimensions.", innerException)
  end
})

define("System.TypeLoadException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Failed when load type.", innerException)
  end
})

define("System.MissingMethodException", {
  __tostring = toString,
  __inherits__ = { Exception },

  __ctor__ = function(this, message, innerException) 
    ctorOfException(this, message or "Specified method could not be found.", innerException)
  end
})

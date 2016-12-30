local System = System
local throw = System.throw
local Collection = System.Collection
local removeAtArray = Collection.removeAtArray
local getArray = Collection.getArray
local insertRangeArray = Collection.insertRangeArray
local InvalidOperationException = System.InvalidOperationException

local Queue = {}

function Queue.__ctor__(this, ...)
    local len = select("#", ...)
    if len == 0 then return end
    local collection = ...
    if type(collection) == "number" then return end
    insertRangeArray(this, 0, collection)
end

function Queue.getCount(this)
    return #this
end

Queue.Clear = Collection.removeArrayAll
Queue.Enqueue = Collection.pushArray
Queue.GetEnumerator = Collection.arrayEnumerator
Queue.Contains = Collection.contains
Queue.ToArray = Collection.toArray
Queue.TrimExcess = System.emptyFn

local function peek(t)
    if #t == 0 then
        throw(InvalidOperationException())
    end
    return getArray(t, 0)
end

Queue.Peek = peek

function Queue.Dequeue(t)
    local v = peek(t)
    removeAtArray(t, 0)
    return v
end

System.define("System.Queue", function(T) 
    local cls = {
        __inherits__ = { System.IEnumerable_1(T), System.ICollection },
        __genericT__ = T,
    }
    return cls
end, Queue)

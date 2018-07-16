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
local Collection = System.Collection
local changeVersion = Collection.changeVersion
local each = Collection.each
local ArgumentNullException = System.ArgumentNullException
local InvalidOperationException = System.InvalidOperationException
local EqualityComparer_1 = System.EqualityComparer_1

local setmetatable = setmetatable
local getmetatable = getmetatable

local LinkedListNode = {}

local function newLinkedListNode(list, value)
  return setmetatable({ List = list, Value = value }, LinkedListNode)
end

function LinkedListNode.getNext(this)
  local next = this.next
  if next == nil or next == this.List.head then
    return nil
  end
  return next
end

function LinkedListNode.getPrevious(this)
  local prev = this.prev
  if prev == nil or prev == this.List.head then
    return nil
  end
  return prev
end

System.define("System.LinkedListNode", LinkedListNode)

local LinkedList = {}

function LinkedList.__ctor__(this, ...)
  local len = select("#", ...)
  if len == 0 then
    this.Count = 0
  else
    local collection = ...
    if collection == nil then
      throw(ArgumentNullException("collection"))
    end
    this.Count = 0
    for _, item in each(collection) do
      this:addLast(item)
    end
  end
end

function LinkedList.getCount(this)
  return this.Count
end

function LinkedList.getFirst(this)    
  return this.head
end

function LinkedList.getLast(this)
  local head = this.head
  return head ~= nil and head.prev or nil
end

local function vaildateNode(this, node)
  if node == nil then
    throw(ArgumentNullException("node"))
  end
  if node.List ~= this then
    throw(InvalidOperationException("ExternalLinkedListNode"))
  end
end

local function insertNodeBefore(this, node, newNode)
  newNode.next = node
  newNode.prev = node.prev
  node.prev.next = newNode
  node.prev = newNode
  this.Count = this.Count + 1
  changeVersion(this)
end

local function insertNodeToEmptyList(this, newNode)
  newNode.next = newNode
  newNode.prev = newNode
  this.head = newNode
  this.Count = this.Count + 1
  changeVersion(this)
end

function LinkedList.AddAfter(this, node, newNode)    
  vaildateNode(this, node)
  if getmetatable(newNode) == LinkedListNode then
    vaildateNode(this, newNode)
    insertNodeBefore(this, node.next, newNode)
    newNode.List = this
  else
    local result = newLinkedListNode(node.List, newNode)
    insertNodeBefore(this, node.next, result)
    return result
  end
end

function LinkedList.AddBefore(this, node, newNode)
  vaildateNode(this, node)
  if getmetatable(newNode) == LinkedListNode then
    vaildateNode(this, newNode)
    insertNodeBefore(this, node, newNode)
    newNode.List = this
    if node == this.head then
      this.head = newNode
    end
  else
    local result = newLinkedListNode(node.List, newNode)
    insertNodeBefore(this, node, result)
    if node == this.head then
      this.head = result
    end
    return result
  end
end

function LinkedList.AddFirst(this, node)
  if getmetatable(node) == LinkedListNode then
    vaildateNode(this, node)
    if this.head == nil then
      insertNodeToEmptyList(this, node)
    else
      insertNodeBefore(this, this.head, node)
        this.head = node
      end
      node.List = this
  else
    local result = newLinkedListNode(this, node)
    if this.head == nil then
      insertNodeToEmptyList(this, result)
    else
      insertNodeBefore(this, this.head, result)
      this.head = result
    end
    return result
  end
end

function LinkedList.AddLast(this, node)
  if getmetatable(node) == LinkedListNode then
    vaildateNode(this, node)
    if this.head == nil then
      insertNodeToEmptyList(this, node)
    else
      insertNodeBefore(this, this.head, node)
    end
    node.List = this
  else
    local result = newLinkedListNode(this, node)
    if this.head == nil then
      insertNodeToEmptyList(this, result)
    else
      insertNodeBefore(this, this.head, result)
    end
    return result
  end
end

local function invalidate(this)
  this.List = nil
  this.next = nil
  this.prev = nil
end

function LinkedList.Clear(this)
  local current = this.head
  while current ~= nil do
    local temp = current
    current = current.next
    invalidate(temp)
  end
  this.head = nil
  this.Count = 0
  changeVersion(this)
end

function LinkedList.Contains(this, value)
  return this:Find(value) ~= nil
end

function LinkedList.Find(this, value)     
  local head = this.head
  local node = head
  local equals = EqualityComparer_1(this.__genericT__).getDefault().Equals
  if node ~= nil then
    if value ~= nil then
      repeat
        if equals(node.Value, value) then
          return node
        end
        node = node.next
      until node == head
    else
      repeat 
        if node.Value == nil then
          return node
        end
        node = node.next
      until node == head
    end
  end
  return nil
end

function LinkedList.FindLast(this, value)
  local head = this.head
  if head == nil then return nil end
  local last = head.prev
  local node = last
  local equals = EqualityComparer_1(this.__genericT__).getDefault().Equals
  if node ~= nil then
    if value ~= nil then
      repeat
        if equals(node.Value, value) then
          return node
        end
        node = node.prev
      until node == head
    else
      repeat 
        if node.Value == nil then
          return node
        end
        node = node.prev
       until node == head
    end
  end
  return nil
end

local function remvoeNode(this, node)
  if node.next == node then
    this.head = nil
  else
    node.next.prev = node.prev
    node.prev.next = node.next
    if this.head == node then
      this.head = node.next
    end
  end
  invalidate(node)
  this.Count = this.Count - 1
  changeVersion(this)
end

function LinkedList.Remove(this, node)
  if getmetatable(node) == LinkedListNode then
    vaildateNode(this, node)
    remvoeNode(this, node)
  else
    node = this:Find(node)
    if node ~= nil then
      remvoeNode(this, node)
    end
    return false
  end
end

function LinkedList.RemoveFirst(this)
  local head = this.head
  if head == nil then
    throw(InvalidOperationException("LinkedListEmpty"))
  end
  remvoeNode(this, head)
end

function LinkedList.RemoveLast(this)
  local head = this.head
  if head == nil then
    throw(InvalidOperationException("LinkedListEmpty"))
  end
  remvoeNode(this, head.prev)
end

LinkedList.GetEnumerator = Collection.linkedListEnumerator

System.define("System.LinkedList", function(T) 
  local cls = { 
  __inherits__ = { System.ICollection_1(T), System.ICollection }, 
  __genericT__ = T,
  __len = LinkedList.getCount
  }
  return cls
end, LinkedList)
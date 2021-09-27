require("run")

local function src(module)
  return ("../%s/src"):format(module)
end

local function out(module)
  return ("../%s/out"):format(module)
end

local function lib(module)
  return ("../%s/bin/Debug/net6.0/%s.dll!"):format(module, module)
end

local function libs(...)
  local t = {}  
  for i = 1, select("#", ...) do
    t[i] = lib(select(i, ...)) 
  end
  return table.concat(t, ';')
end

run {
 {
   depth = 3,
   input = src("BridgeAttributes"),
   output = out("BridgeAttributes"),
   metadata = true,
   module = true
 },
 {
   depth = 3,
   input = src("BridgeTestNUnit"),
   output = out("BridgeTestNUnit"),
   libs = lib("BridgeAttributes"),
   metadata = true,
   module = true
 },
 {
   depth = 3,
   input = src("ClientTestHelper"),
   output = out("ClientTestHelper"),
   libs = libs("BridgeAttributes", "BridgeTestNUnit"),
   metadata = true,
   module = true
 },
 {
   depth = 3,
   input = src("Batch1"),
   output = out("Batch1"),
   libs = libs("BridgeAttributes", "BridgeTestNUnit", "ClientTestHelper"),
   attr = true,
   metadata = true,
   module = true
 },
 {
   depth = 3,
   input = "src",
   output = "out",
   libs = libs("BridgeTestNUnit", "Batch1"),
   metadata = true,
   module = true
 },
}
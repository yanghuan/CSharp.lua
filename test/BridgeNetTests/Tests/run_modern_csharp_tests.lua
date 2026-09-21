package.path = package.path .. ";../../../CSharp.lua/Coresystem.lua/?.lua"
package.path = package.path .. ";../?.lua"
package.path = package.path .. ";CSharp.lua/Coresystem.lua/?.lua"
package.path = package.path .. ";test/BridgeNetTests/?.lua;"
package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"

local conf = {
  traceback = debug.traceback,
}
require("All")("", conf)

local modules = {
  "BridgeAttributes",
  "BridgeTestNUnit",
  "ClientTestHelper",
  "Batch1",
}

for i = 1, #modules do
  local name = modules[i]
  require(name .. "/out/manifest")(name .. "/out")
end

local function runTest(className, methodName, fn)
  local status, err = pcall(fn)
  if status then
    print(string.format("  [PASS] %s.%s", className, methodName))
    return true
  else
    print(string.format("  [FAIL] %s.%s: %s", className, methodName, tostring(err)))
    return false
  end
end

local testClasses = {
  { name = "RawStringLiteralTests", cls = Bridge.ClientTest.CSharp11.RawStringLiteralTests, methods = {
    "TestSingleLineRawString",
    "TestMultiLineRawString",
    "TestInterpolatedRawString",
    "TestUtf8StringLiteral",
  }},
  { name = "ListPatternTests", cls = Bridge.ClientTest.CSharp11.ListPatternTests, methods = {
    "TestExactMatch",
    "TestSlicePattern",
    "TestDiscardsAndVariables",
    "TestSwitchExpression",
  }},
  { name = "CollectionExpressionTests", cls = Bridge.ClientTest.CSharp12.CollectionExpressionTests, methods = {
    "TestArrayCreation",
    "TestListCreation",
    "TestSpanCreation",
    "TestSpreadElement",
  }},
  { name = "PrimaryConstructorTests", cls = Bridge.ClientTest.CSharp12.PrimaryConstructorTests, methods = {
    "TestClassPrimaryConstructor",
    "TestInheritanceWithPrimaryConstructor",
    "TestStructPrimaryConstructor",
  }},
  { name = "FieldKeywordTests", cls = Bridge.ClientTest.CSharp14.FieldKeywordTests, methods = {
    "TestDefaultValue",
    "TestSetterTransform",
    "TestSetterValidation",
  }},
}

local total = 0
local passed = 0

print("=== Running Modern C# Feature Tests in Lua ===")
for _, testGroup in ipairs(testClasses) do
  print("\n" .. testGroup.name .. ":")
  local inst = testGroup.cls()
  for _, mName in ipairs(testGroup.methods) do
    total = total + 1
    local fn = testGroup.cls[mName] or inst[mName]
    if runTest(testGroup.name, mName, function() fn(inst) end) then
      passed = passed + 1
    end
  end
end

print(string.format("\n=== Test Results: %d / %d Passed ===", passed, total))
assert(passed == total, "Some tests failed!")

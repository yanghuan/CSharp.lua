local function execute(cmd)
  print("execute", cmd)
  local code = os.execute(cmd)
  assert(code == 0)
end

local luaVersions = {
  "Lua5.3",
  "LuaJIT-2.0.2",
  --"MoonJIT-2.2.0"
}

local function compile(arg)
  local CSharpLua = ("../"):rep(arg.depth) .. "CSharp.lua.Launcher/bin/Debug/net5.0/CSharp.lua.Launcher.dll"
  local cmd = ("dotnet %s -s %s -d %s"):format(CSharpLua, arg.input, arg.output)
  if arg.libs then
    cmd = cmd .. " -l " .. arg.libs
  end
  if arg.metaFiles then
    cmd = cmd .. " -m " .. arg.metaFiles
  end
  if arg.attr then
    if arg.attr == true then
      cmd = cmd .. " -a"
    else
      cmd = cmd .. " -a " .. arg.attr
    end
  end
  if arg.metadata then
    cmd = cmd .. " -metadata"
  end
  if arg.classic then
    cmd = cmd .. " -c"
  end
  if arg.inlineProperty then
    cmd = cmd .. " -inline-property"
  end
  if arg.nodebug then
    cmd = cmd .. " -p"
  end
  if arg.module then
    cmd = cmd .. " -module"
  end
  if arg.extra then
    cmd = cmd .. " " .. arg.extra
  end
  execute(cmd)
end

local function launcher(arg, luaVersion)
  local luaPath = ("..\\"):rep(arg.depth - 1) .. "__bin\\"
  cmd = ("\"%s%s\\lua.exe\" %s"):format(luaPath, luaVersion, "launcher.lua")
  if arg.nodebug then
    cmd = cmd .. " nodebug"
  end
  execute(cmd)
end

local function runAll(t, args)
  for _, v in ipairs(luaVersions) do
    print("--------------------", v, "--------------------")
    for _, arg in ipairs(args) do
      if v:find("JIT") then
        t.classic = true
        t.extra = "-csc /define:__JIT__;DEBUG"
      else
        t.classic = false
        t.extra = "-csc /define:DEBUG"
      end
      arg.__index = arg
      setmetatable(t, arg)
      compile(t, v)
    end
    launcher(t, v)
  end
end


function run(args)
  execute("dotnet build --configuration Debug")
  if args[1] == nil then
    args = { args }
  end
  runAll({}, args)
  runAll({ inlineProperty = true }, args)
  runAll({ nodebug = true }, args)
  runAll({ inlineProperty = true, nodebug = true }, args)
end
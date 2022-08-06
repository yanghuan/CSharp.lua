package.path = package.path .. ";../../CSharp.lua/Coresystem.lua/?.lua"


local codeAnalysis = {
  CSharp = {}
}

local codeAnalysisTypes = { "ISymbol", "INamedTypeSymbol", "INamespaceSymbol" }

for _, name in ipairs(codeAnalysisTypes) do
  codeAnalysis[name] = "Microsoft.CodeAnalysis." .. name
end

Microsoft = {
  CodeAnalysis = codeAnalysis
}

require("All")()          -- coresystem.lua/All.lua
require("out.manifest")("out")
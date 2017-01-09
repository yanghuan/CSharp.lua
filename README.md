# CSharp.lua
The C# to Lua compiler, it will replace [Bridge.lua](https://github.com/yanghuan/bridge.lua) **in the future**.

## Introduction

CSharp.lua is a C# to Lua compiler. Write C# then run on lua VM.
* Build on [Microsoft Roslyn](https://github.com/dotnet/roslyn). Support for C# 6.0.

* Highly readable code generation. C# AST ---> Lua AST ---> Lua Code.

* Allowing almost all of the C# language features.

* Provides [CoreSystem.lua](https://github.com/yanghuan/CSharp.lua/tree/master/CSharp.lua/CoreSystem.Lua/CoreSystem) library, can run away of CLR.

* Self-Compiling, run "./test/self.bat".

## *License*
[Apache 2.0 license](https://raw.githubusercontent.com/yanghuan/CSharp.lua/master/LICENSE).

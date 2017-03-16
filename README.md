# CSharp.lua
The C# to Lua compiler.

## Introduction

CSharp.lua is a C# to Lua compiler. Write C# then run on lua VM.
* Build on [Microsoft Roslyn](https://github.com/dotnet/roslyn). Support for C# 6.0.

* Highly readable code generation. C# AST ---> Lua AST ---> Lua Code.

* Allowing almost all of the C# language features.

* Provides [CoreSystem.lua](https://github.com/yanghuan/CSharp.lua/tree/master/CSharp.lua/CoreSystem.Lua/CoreSystem) library, can run away of CLR.

* Self-Compiling, run "./test/self-compiling/self.bat".

## How to Use 
### Command Line Parameters
```cmd
D:\>CSharp.Lua.exe -h
Usage: CSharp.lua [-s srcfolder] [-d dstfolder]
Arguments 
-s              : source directory, all *.cs files whill be compiled
-d              : destination  directory, will put the out lua files

Options
-h              : show the help message    
-l              : libraries referenced, use ';' to separate      
-m              : meta files, like System.xml, use ';' to separate     
-def            : defines name as a conditional symbol, use ';' to separate

-c              : support classic lua version(5.1), default support 5.3 
-i              : indent number, default is 4
-sem            : append semicolon when statement over
-a              : attributes need to export, use ';' to separate, if ""-a"" only, all attributes whill be exported
```
## *License*
[Apache 2.0 license](https://raw.githubusercontent.com/yanghuan/CSharp.lua/master/LICENSE).

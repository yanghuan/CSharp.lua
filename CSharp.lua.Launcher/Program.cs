/*
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
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua {
  class Program {
    private const string kHelpCmdString = @"Usage: CSharp.lua [-s srcfolder] [-d dstfolder]
Arguments
-s              : source directory, all *.cs files whill be compiled
-d              : destination  directory, will put the out lua files

Options
-h              : show the help message and exit
-l              : libraries referenced, use ';' to separate
                  if the librarie is a module, whitch is compield by CSharp.lua with -module arguemnt, the last character needs to be '!' in order to mark  

-m              : meta files, like System.xml, use ';' to separate
-csc            : csc.exe command argumnets, use ' ' or '\t' to separate

-c              : support classic lua version(5.1), default support 5.3
-a              : attributes need to export, use ';' to separate, if ""-a"" only, all attributes whill be exported
-e              : enums need to export, use ';' to separate, if ""-a"" only, all enums whill be exported
-p              : do not use debug.setmetatable, in some Addon/Plugin environment debug object cannot be used
-metadata       : export all metadata, use @CSharpLua.Metadata annotations for precise control
-module         : the currently compiled assembly needs to be referenced, it's useful for multiple module compiled
-inline-property: inline some single-line properties
";
    public static void Main(string[] args) {
      if (args.Length > 0) {
        try {
          var cmds = Utility.GetCommondLines(args);
          if (cmds.ContainsKey("-h")) {
            ShowHelpInfo();
            return;
          }

          var sw = new Stopwatch();
          sw.Start();

          string folder = cmds.GetArgument("-s");
          string output = cmds.GetArgument("-d");
          string lib = cmds.GetArgument("-l", true);
          string meta = cmds.GetArgument("-m", true);
          bool isClassic = cmds.ContainsKey("-c");
          string atts = cmds.GetArgument("-a", true);
          if (atts == null && cmds.ContainsKey("-a")) {
            atts = string.Empty;
          }
          string enums = cmds.GetArgument("-e", true);
          if (enums == null && cmds.ContainsKey("-e")) {
            enums = string.Empty;
          }
          string csc = GetCSCArgument(args);
          bool isExportMetadata = cmds.ContainsKey("-metadata");
          bool isModule = cmds.ContainsKey("-module");
          bool isInlineSimpleProperty = cmds.ContainsKey("-inline-property");
          bool isPreventDebugObject = cmds.ContainsKey("-p");
          Compiler c = new Compiler(folder, output, lib, meta, csc, isClassic, atts, enums) {
            IsExportMetadata = isExportMetadata,
            IsModule = isModule,
            IsInlineSimpleProperty = isInlineSimpleProperty,
            IsPreventDebugObject = isPreventDebugObject,
          };
          c.Compile();
          Console.WriteLine($"Compiled Success, cost {sw.Elapsed.TotalSeconds}s");
        } catch (CmdArgumentException e) {
          Console.Error.WriteLine(e.Message);
          ShowHelpInfo();
          Environment.ExitCode = -1;
        } catch (CompilationErrorException e) {
          Console.Error.WriteLine(e.Message);
          Environment.ExitCode = -1;
        } catch (Exception e) {
          Console.Error.WriteLine(e.ToString());
          Environment.ExitCode = -1;
        }
      } else {
        ShowHelpInfo();
        Environment.ExitCode = -1;
      }
    }

    private static void ShowHelpInfo() {
      Console.Error.WriteLine(kHelpCmdString);
    }

    private static HashSet<string> argumnets_; 

    private static bool IsArgumentKey(string key) {
      if (argumnets_ == null) {
        argumnets_ = new HashSet<string>();
        string[] lines = kHelpCmdString.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines) {
          if (line.StartsWith('-')) {
            char[] chars = line.TakeWhile(i => !char.IsWhiteSpace(i)).ToArray();
            argumnets_.Add(new string(chars));
          }
        }
      }
      return argumnets_.Contains(key);
    }

    private static string GetCSCArgument(string[] args) {
      int index = args.IndexOf("-csc");
      if (index != -1) {
        var remains = args.Skip(index + 1);
        int end = remains.IndexOf(IsArgumentKey);
        if (end != -1) {
          remains = remains.Take(end);
        }
        return string.Join(" ", remains);
      }
      return null;
    }
  }
}

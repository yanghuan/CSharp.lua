using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua {
    class Program {
        private const string HelpCmdString = @"Usage: CSharp.lua [-s srcfolder] [-d dstfolder]
Arguments 
-s              : source directory, all *.cs files whill be compiled
-d              : destination  directory, will put the out lua files

Options
-l              : libraries referenced, use ';' to separate      
-m              : meta files, like System.xml, use ';' to separate     
-h              : show the help message    
-def            : defines name as a conditional symbol, use ';' to separate
";
        static void Main(string[] args) {
            if(args.Length > 0) {
                try {
                    var cmds = Utility.GetCommondLines(args);
                    if(cmds.ContainsKey("-h")) {
                        ShowHelpInfo();
                        return;
                    }

                    string folder = cmds.GetArgument("-s");
                    string output = cmds.GetArgument("-d");
                    string lib = cmds.GetArgument("-l", true);
                    string meta = cmds.GetArgument("-m", true);
                    string defines = cmds.GetArgument("-def", true);
                    Worker w = new Worker(folder, output, lib, meta, defines);
                    w.Do();
                    Console.WriteLine("all operator success");
                }
                catch(CmdArgumentException e) {
                    Console.Error.WriteLine(e.Message);
                    ShowHelpInfo();
                    Environment.ExitCode = -1;
                }
                catch(CompilationErrorException e) {
                    Console.Error.WriteLine(e.Message);
                    Environment.ExitCode = -1;
                }
                catch(Exception e) {
                    Console.Error.WriteLine(e.ToString());
                    Environment.ExitCode = -1;
                }
            }
            else {
                ShowHelpInfo();
                Environment.ExitCode = -1;
            }
        }

        private static void ShowHelpInfo() {
            Console.Error.WriteLine(HelpCmdString);
        }
    }
}

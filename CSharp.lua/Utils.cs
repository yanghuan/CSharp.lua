using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLua {
    public sealed class CmdArgumentException : Exception {
        public CmdArgumentException(string message) : base(message) {
        }
    }

    public sealed class CompilationErrorException : Exception {
        public CompilationErrorException(string message) : base(message) {
        }
    }

    public static class Utility {
        public static Dictionary<string, string[]> GetCommondLines(string[] args) {
            Dictionary<string, string[]> cmds = new Dictionary<string, string[]>();

            string key = "";
            List<string> values = new List<string>();

            foreach(string arg in args) {
                string i = arg.Trim();
                if(i.StartsWith("-")) {
                    if(!string.IsNullOrEmpty(key)) {
                        cmds.Add(key, values.ToArray());
                        key = "";
                        values.Clear();
                    }
                    key = i;
                }
                else {
                    values.Add(i);
                }
            }

            if(!string.IsNullOrEmpty(key)) {
                cmds.Add(key, values.ToArray());
            }
            return cmds;
        }

        public static T GetOrDefault<K, T>(this IDictionary<K, T> dict, K key, T t = default(T)) {
            T v;
            if(dict.TryGetValue(key, out v)) {
                return v;
            }
            return t;
        }

        public static string GetArgument(this Dictionary<string, string[]> args, string name, bool isOption = false) {
            string[] values = args.GetOrDefault(name);
            if(values == null || values.Length == 0) {
                if(isOption) {
                    return null;
                }
                throw new CmdArgumentException(name + " is not found");
            }
            return values[0];
        }

        public static string GetCurrentDirectory(string path) {
            const string CurrentDirectorySign1 = "~/";
            const string CurrentDirectorySign2 = "~\\";

            if(path.StartsWith(CurrentDirectorySign1)) {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(CurrentDirectorySign1.Length));
            }
            else if(path.StartsWith(CurrentDirectorySign2)) {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Substring(CurrentDirectorySign2.Length));
            }

            return Path.Combine(Environment.CurrentDirectory, path);
        }

        public static string Move(string directory, string lib) {
            string path = Path.Combine(directory, Path.GetFileName(lib));
            File.Copy(lib, path, true);
            return path;
        }

        public static string[] SplitPaths(string path) {
            HashSet<string> list = new HashSet<string>();
            if(!string.IsNullOrEmpty(path)) {
                string[] paths = path.Split(';');
                foreach(string file in paths) {
                    list.Add(Utility.GetCurrentDirectory(path));
                }
            }
            return list.ToArray();
        }
    }
}

using System;
using Bridge;

namespace CoreLib.TestScript
{
    public class Globals
    {
        [ScriptAlias("setTimeout")]
        public static int SetTimeout(Action action, int milliseconds)
        {
            return 0;
        }
    }
}

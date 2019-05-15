using System;

namespace Bridge.ClientTest.Batch4
{
    public static class TestHelper
    {
        public static void Safe(Action a)
        {
            try
            {
                a();
            }
            catch { }
        }
    }
}

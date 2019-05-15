using System.Collections.Generic;
using System.Linq;
using Bridge.Test.NUnit;
using System;
using Bridge.ClientTest.Collections.Generic.Base;

#if false
namespace Bridge.ClientTest.Collections.Generic
{
    public enum EnumerableType
    {
        HashSet,
        SortedSet,
        List,
        Queue,
        Lazy,
    }

    /// <summary>
    /// Contains tests that ensure the correctness of the SortedSet class.
    /// </summary>
    public abstract partial class SortedSet_Generic_Tests<T>: TestBase<T>
    {
#region ISet<T> Helper Methods

        protected override ISet<T> GenericISetFactory()
        {
            return new SortedSet<T>();
        }

#endregion


    }
}
#endif

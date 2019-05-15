// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.ClientTest.Collections.Generic.Base;
using Bridge.Test.NUnit;

#if false
namespace Bridge.ClientTest.Collections.Generic
{
    /// <summary>
    /// Contains tests that ensure the correctness of the Dictionary class.
    /// </summary>
    public abstract class SortedList_Generic_Tests<TKey, TValue> : TestBase
    {
        protected abstract KeyValuePair<TKey, TValue> CreateT(int seed);
        protected abstract TKey CreateTKey(int seed);
        protected abstract TValue CreateTValue(int seed);

        protected TKey GetNewKey(IDictionary<TKey, TValue> dictionary)
        {
            int seed = 840;
            TKey missingKey = CreateTKey(seed++);
            while (dictionary.ContainsKey(missingKey) || missingKey.Equals(default(TKey)))
            {
                missingKey = CreateTKey(seed++);
            }
            return missingKey;
        }

        public virtual IComparer<TKey> GetKeyIComparer()
        {
            return Comparer<TKey>.Default;
        }

#region IDictionary<TKey, TValue> Helper Methods
        protected virtual IDictionary<TKey, TValue> GenericIDictionaryFactory(int count)
        {
            IDictionary<TKey, TValue> collection = GenericIDictionaryFactory();
            AddToCollection(collection, count);
            return collection;
        }

        protected void AddToCollection(ICollection<KeyValuePair<TKey, TValue>> collection, int numberOfItemsToAdd)
        {
            int seed = 12353;
            IDictionary<TKey, TValue> casted = (IDictionary<TKey, TValue>)collection;
            int initialCount = casted.Count;
            while ((casted.Count - initialCount) < numberOfItemsToAdd)
            {
                KeyValuePair<TKey, TValue> toAdd = CreateT(seed++);
                while (casted.ContainsKey(toAdd.Key))
                {
                    toAdd = CreateT(seed++);
                }
                collection.Add(toAdd);
            }
        }

        protected IDictionary<TKey, TValue> GenericIDictionaryFactory()
        {
            return new SortedList<TKey, TValue>();
        }

        protected Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

#endregion
    }
}

#endif

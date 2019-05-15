// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Bridge.ClientTest.Collections.Generic.Base;
using Bridge.Test.NUnit;

#if false
namespace Bridge.ClientTest.Collections.Generic
{
    [Category(Constants.MODULE_ACTIVATOR)]
    [TestFixture(TestNameFormat = "SortedList_IDictionary_NonGeneric_Tests - {0}")]
    public class SortedList_IDictionary_NonGeneric_Tests: TestBase
    {
#region IDictionary Helper Methods

        protected object GetNewKey(IDictionary dictionary)
        {
            int seed = 840;
            object missingKey = CreateTKey(seed++);
            while (dictionary.Contains(missingKey) || missingKey.Equals(null))
            {
                missingKey = CreateTKey(seed++);
            }
            return missingKey;
        }


        protected IDictionary NonGenericIDictionaryFactory()
        {
            return new SortedList<string, string>();
        }

        protected object CreateTKey(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        protected object CreateTValue(int seed) => CreateTKey(seed);

        protected Type ICollection_NonGeneric_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

#endregion

#region IDictionary tests

        [Test]
        public void IDictionary_NonGeneric_ItemSet_NullValueWhenDefaultValueIsNonNull()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, int>();
                Assert.Throws<ArgumentNullException>(() => dictionary[GetNewKey(dictionary)] = null);
            }
        }

        [Test]
        public void IDictionary_NonGeneric_ItemSet_KeyOfWrongType()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, string>();
                Assert.Throws<ArgumentNullException>(() => dictionary[23] = CreateTValue(12345));
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void IDictionary_NonGeneric_ItemSet_ValueOfWrongType()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, string>();
                object missingKey = GetNewKey(dictionary);
                Assert.Throws<ArgumentException>(() => dictionary[missingKey] = 324);
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void IDictionary_NonGeneric_Add_KeyOfWrongType()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, string>();
                object missingKey = 23;
                Assert.Throws<ArgumentException>(() => dictionary.Add(missingKey, CreateTValue(12345)));
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void IDictionary_NonGeneric_Add_ValueOfWrongType()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, string>();
                object missingKey = GetNewKey(dictionary);
                Assert.Throws<ArgumentException>(() => dictionary.Add(missingKey, 324));
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void IDictionary_NonGeneric_Add_NullValueWhenDefaultTValueIsNonNull()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, int>();
                object missingKey = GetNewKey(dictionary);
                Assert.Throws<ArgumentNullException>(() => dictionary.Add(missingKey, null));
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void IDictionary_NonGeneric_Contains_KeyOfWrongType()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary dictionary = new SortedList<string, int>();
                Assert.False(dictionary.Contains(1));
            }
        }
#endregion
    }
}

#endif

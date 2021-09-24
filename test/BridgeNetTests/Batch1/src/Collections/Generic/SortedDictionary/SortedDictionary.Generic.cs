// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Test.NUnit;

#if true
namespace Bridge.ClientTest.Collections.Generic
{
    [Category(Constants.MODULE_ACTIVATOR)]
    [TestFixture(TestNameFormat = "SortedDictionaryGenericTestsStringString - {0}")]
    public class SortedDictionaryGenericTestsStringString : SortedDictionary_Generic_Tests<string, string>
    {
#region Constructor_IComparer

        [Test]
        public void SortedDictionary_Generic_Constructor_IComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IComparer<string> comparer = GetKeyIComparer();
                IDictionary<string, string> source = GenericIDictionaryFactory(count);
                SortedDictionary<string, string> copied = new SortedDictionary<string, string>(source, comparer);
                Assert.AreDeepEqual(source.ToArray(), copied.ToArray());
                Assert.True(Object.ReferenceEquals(comparer, copied.Comparer));
            }
        }

#endregion

#region Constructor_IDictionary

        [Test]
        public void SortedDictionary_Generic_Constructor_IDictionary()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary<string, string> source = GenericIDictionaryFactory(count);
                IDictionary<string, string> copied = new SortedDictionary<string, string>(source);
                Assert.AreDeepEqual(source.ToArray(), copied.ToArray());
            }

        }

        [Test]
        public void SortedDictionary_Generic_Constructor_NullIDictionary_ThrowsArgumentNullException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                Assert.Throws<ArgumentNullException>(() => new SortedDictionary<string, string>((IDictionary<string, string>)null));
            }

        }

#endregion

#region Constructor_IDictionary_IComparer

        [Test]
        public void SortedDictionary_Generic_Constructor_IDictionary_IComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IComparer<string> comparer = GetKeyIComparer();
                IDictionary<string, string> source = GenericIDictionaryFactory(count);
                SortedDictionary<string, string> copied = new SortedDictionary<string, string>(source, comparer);
                Assert.AreDeepEqual(source.ToArray(), copied.ToArray());
                Assert.True(Object.ReferenceEquals(comparer, copied.Comparer));
            }


        }

#endregion

#region Constructor_int

        [Test]
        public void SortedDictionary_Generic_Constructor_int()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();
                Assert.AreEqual(0, dictionary.Count);
            }
        }

#endregion

#region Constructor_int_IComparer

        [Test]
        public void SortedDictionary_Generic_Constructor_int_IComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IComparer<string> comparer = GetKeyIComparer();
                SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>(comparer);
                Assert.AreEqual(0, dictionary.Count);
                Assert.True(Object.ReferenceEquals(comparer, dictionary.Comparer));
            }

        }

#endregion

#region Capacity

        [Test]
        public void SortedDictionary_Generic_Capacity_setRoundTrips()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                Assert.AreEqual(count, dictionary.Count);
            }


        }

        [Test]
        public void SortedDictionary_Generic_Capacity_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                int capacityBefore = dictionary.Count;
                Assert.AreEqual(capacityBefore, dictionary.Count);
            }


        }

        [Test]
        public void SortedDictionary_Generic_Capacity_LessThanCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory();
                for (int i = 0; i < count; i++)
                {
                    AddToCollection(dictionary, 1);
                    Assert.Throws<KeyNotFoundException>(() => dictionary["abc"].ToString());
                }
            }

        }

        [Test]
        public void SortedDictionary_Generic_Capacity_GrowsDuringAdds()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory();
                for (int i = 0; i < count; i++)
                {
                    AddToCollection(dictionary, 1);
                }
            }
        }

        [Test]
        public void SortedDictionary_Generic_Capacity_ClearDoesntTrim()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory();
                for (int i = 0; i < count; i++)
                {
                    AddToCollection(dictionary, 1);
                }
                dictionary.Clear();
            }
        }

        [Test]
        public void SortedDictionary_Generic_Capacity_ClearTrimsToInitialCapacity()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();
                AddToCollection(dictionary, count);
                dictionary.Clear();
            }
        }

#endregion

#region ContainsValue

        [Test]
        public void SortedDictionary_Generic_ContainsValue_NotPresent()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                int seed = 4315;
                string notPresent = CreateTValue(seed++);
                while (dictionary.Values.Contains(notPresent))
                {
                    notPresent = CreateTValue(seed++);
                }
                Assert.False(dictionary.ContainsValue(notPresent));
            }
        }

        [Test]
        public void SortedDictionary_Generic_ContainsValue_Present()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                int seed = 4315;
                KeyValuePair<string, string> notPresent = CreateT(seed++);
                while (dictionary.Contains(notPresent))
                {
                    notPresent = CreateT(seed++);
                }
                dictionary.Add(notPresent.Key, notPresent.Value);
                Assert.True(dictionary.ContainsValue(notPresent.Value));

            }
        }

        [Test]
        public void SortedDictionary_Generic_ContainsValue_DefaultValueNotPresent()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                Assert.False(dictionary.ContainsValue(default(string)));
            }
        }

        [Test]
        public void SortedDictionary_Generic_ContainsValue_DefaultValuePresent()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                int seed = 4315;
                string notPresent = CreateTKey(seed++);
                while (dictionary.ContainsKey(notPresent))
                {
                    notPresent = CreateTKey(seed++);
                }
                dictionary.Add(notPresent, default(string));
                Assert.True(dictionary.ContainsValue(default(string)));
            }
        }

#endregion

#region IndexOfKey

        [Test]
        public void SortedDictionary_Generic_IndexOfKey_EachKey()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                // Assumes no duplicate elements contained in the dictionary returned by GenericIListFactory
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                var keys = dictionary.Keys;
                Enumerable.Range(0, count).ForEach(index =>
                {
                    Assert.AreEqual(keys.ElementAt(index), dictionary.ElementAt(index).Key);
                });
            }
        }

#endregion

#region IndexOfValue

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_DefaultValueNotContainedInList()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                string value = default(string);
                while (dictionary.ContainsValue(value))
                {
                    string key = dictionary.First(i => i.Value == value).Key;
                    dictionary.Remove(key);
                }
                Assert.False(dictionary.Values.Any(i => i == value));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_DefaultValueContainedInList()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                string key = GetNewKey(dictionary);
                string value = default(string);
                while (dictionary.ContainsValue(value))
                {
                    dictionary.Remove(dictionary.First(i => i.Value == value).Key);
                }

                List<string> keys = dictionary.Keys.ToList();
                keys.Add(key);
                keys.Sort();
                int expectedIndex = keys.IndexOf(key);
                dictionary.Add(key, value);
                Assert.AreEqual(expectedIndex, dictionary.Values.ToList().IndexOf(value));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_ValueInCollectionMultipleTimes()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                int seed = 53214;
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                string key1 = CreateTKey(seed++);
                string key2 = CreateTKey(seed++);
                string value = CreateTValue(seed++);
                while (dictionary.ContainsKey(key1))
                {
                    key1 = CreateTKey(seed++);
                }
                while (key1.Equals(key2) || dictionary.ContainsKey(key2))
                {
                    key2 = CreateTKey(seed++);
                }
                while (dictionary.ContainsValue(value))
                {
                    var key = dictionary.First(i => i.Value == value).Key;
                    dictionary.Remove(key);
                }

                List<string> keys = dictionary.Keys.ToList();
                keys.Add(key1);
                keys.Add(key2);
                keys.Sort();
                int expectedIndex = Math.Min(keys.IndexOf(key1), keys.IndexOf(key2));

                dictionary.Add(key1, value);
                dictionary.Add(key2, value);
                Assert.AreEqual(expectedIndex, dictionary.Values.ToList().IndexOf(value));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_EachValue()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                // Assumes no duplicate elements contained in the dictionary returned by GenericIListFactory
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                var keys = dictionary.Keys;
                Enumerable.Range(0, count).ForEach(index =>
                {
                    var key = keys.ElementAt(index);
                    Assert.AreEqual(key, dictionary.ElementAt(index).Key);
                });
            }
        }

#endregion

#region RemoveAt

        private void RemoveAt(SortedDictionary<string, string> dictionary, KeyValuePair<string, string> element)
        {
            dictionary.Remove(element.Key);
        }

        [Test]
        public void SortedDictionary_Generic_RemoveAt_NonDefaultValueContainedInCollection()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                int seed = count * 251;
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                KeyValuePair<string, string> pair = CreateT(seed++);
                if (!dictionary.ContainsKey(pair.Key))
                {
                    dictionary.Add(pair.Key, pair.Value);
                    count++;
                }
                RemoveAt(dictionary, pair);
                Assert.AreEqual(count - 1, dictionary.Count);
            }
        }

        [Test]
        public void SortedDictionary_Generic_RemoveAt_EveryValue()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                dictionary.ToList().ForEach(value =>
                {
                    RemoveAt(dictionary, value);
                });
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void SortedDictionary_Generic_RemoveAt_OutOfRangeValues()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(count);
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove((-1).ToString()));
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove(int.MinValue.ToString()));
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove(count.ToString()));
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove((count + 1).ToString()));

            }
        }

#endregion

#region TrimExcess

        [Test]
        public void SortedDictionary_Generic_TrimExcess_AfterClearingAndAddingSomeElementsBack()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int dictionaryLength = (int)testCase[0];

                if (dictionaryLength > 0)
                {
                    SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(dictionaryLength);
                    dictionary.Clear();
                    Assert.AreEqual(0, dictionary.Count);

                    AddToCollection(dictionary, dictionaryLength / 10);
                    Assert.AreEqual(dictionaryLength / 10, dictionary.Count);
                }
            }


        }

        [Test]
        public void SortedDictionary_Generic_TrimExcess_AfterClearingAndAddingAllElementsBack()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int dictionaryLength = (int)testCase[0];
                if (dictionaryLength > 0)
                {
                    SortedDictionary<string, string> dictionary = (SortedDictionary<string, string>)GenericIDictionaryFactory(dictionaryLength);
                    dictionary.Clear();
                    Assert.AreEqual(0, dictionary.Count);

                    AddToCollection(dictionary, dictionaryLength);
                    Assert.AreEqual(dictionaryLength, dictionary.Count);
                }
            }
        }

#endregion

#region IReadOnlyDictionary<string, string>.Keys

        [Test]
        public void IReadOnlyDictionary_Generic_Keys_ContainsAllCorrectKeys()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary<string, string> dictionary = GenericIDictionaryFactory(count);
                IEnumerable<string> expected = dictionary.Select((pair) => pair.Key);
                IEnumerable<string> keys = ((IReadOnlyDictionary<string, string>)dictionary).Keys;
                Assert.True(expected.SequenceEqual(keys));
            }


        }

        [Test]
        public void IReadOnlyDictionary_Generic_Values_ContainsAllCorrectValues()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                IDictionary<string, string> dictionary = GenericIDictionaryFactory(count);
                IEnumerable<string> expected = dictionary.Select((pair) => pair.Value);
                IEnumerable<string> values = ((IReadOnlyDictionary<string, string>)dictionary).Values;
                Assert.True(expected.SequenceEqual(values));
            }
        }

#endregion

        protected override KeyValuePair<string, string> CreateT(int seed)
        {
            return new KeyValuePair<string, string>(CreateTKey(seed), CreateTKey(seed + 500));
        }

        protected override string CreateTKey(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes1 = new byte[stringLength];
            rand.NextBytes(bytes1);
            return Convert.ToBase64String(bytes1);
        }

        protected override string CreateTValue(int seed) => CreateTKey(seed);
    }

    [Category(Constants.MODULE_ACTIVATOR)]
    [TestFixture(TestNameFormat = "SortedDictionaryGenericTestsIntInt - {0}")]
    public class SortedDictionaryGenericTestsIntInt : SortedDictionary_Generic_Tests<int, int>
    {
#region Constructor_IComparer

        [Test]
        public void SortedDictionary_Generic_Constructor_IComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IComparer<int> comparer = GetKeyIComparer();
                IDictionary<int, int> source = GenericIDictionaryFactory(count);
                SortedDictionary<int, int> copied = new SortedDictionary<int, int>(source, comparer);
                Assert.AreDeepEqual(source.ToArray(), copied.ToArray());
                Assert.True(Object.ReferenceEquals(comparer, copied.Comparer));
            }
        }

#endregion

#region Constructor_IDictionary

        [Test]
        public void SortedDictionary_Generic_Constructor_IDictionary()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary<int, int> source = GenericIDictionaryFactory(count);
                IDictionary<int, int> copied = new SortedDictionary<int, int>(source);
                Assert.AreDeepEqual(source.ToArray(), copied.ToArray());
            }

        }

        [Test]
        public void SortedDictionary_Generic_Constructor_NullIDictionary_ThrowsArgumentNullException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                Assert.Throws<ArgumentNullException>(() => new SortedDictionary<int, int>((IDictionary<int, int>)null));
            }

        }

#endregion

#region Constructor_IDictionary_IComparer

        [Test]
        public void SortedDictionary_Generic_Constructor_IDictionary_IComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IComparer<int> comparer = GetKeyIComparer();
                IDictionary<int, int> source = GenericIDictionaryFactory(count);
                SortedDictionary<int, int> copied = new SortedDictionary<int, int>(source, comparer);
                Assert.AreDeepEqual(source.ToArray(), copied.ToArray());
                Assert.True(Object.ReferenceEquals(comparer, copied.Comparer));
            }


        }

#endregion

#region Constructor_int

        [Test]
        public void SortedDictionary_Generic_Constructor_int()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = new SortedDictionary<int, int>();
                Assert.AreEqual(0, dictionary.Count);
            }
        }

        [Test]
        public void SortedDictionary_Generic_Constructor_NegativeCapacity_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                IDictionary<int, int> dictionary = null;
                Assert.Throws<ArgumentNullException>(() => new SortedDictionary<int, int>(dictionary));
            }
        }

#endregion

#region Constructor_int_IComparer

        [Test]
        public void SortedDictionary_Generic_Constructor_int_IComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IComparer<int> comparer = GetKeyIComparer();
                SortedDictionary<int, int> dictionary = new SortedDictionary<int, int>(comparer);
                Assert.AreEqual(0, dictionary.Count);
                Assert.True(Object.ReferenceEquals(comparer, dictionary.Comparer));
            }

        }

#endregion

#region Capacity

        [Test]
        public void SortedDictionary_Generic_Capacity_setRoundTrips()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                Assert.AreEqual(count, dictionary.Count);
            }


        }

        [Test]
        public void SortedDictionary_Generic_Capacity_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int capacityBefore = dictionary.Count;
                Assert.AreEqual(capacityBefore, dictionary.Count);

            }


        }

        [Test]
        public void SortedDictionary_Generic_Capacity_LessThanCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory();
                for (int i = 0; i < count; i++)
                {
                    AddToCollection(dictionary, 1);
                }
            }

        }

        [Test]
        public void SortedDictionary_Generic_Capacity_GrowsDuringAdds()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory();
                for (int i = 0; i < count; i++)
                {
                    AddToCollection(dictionary, 1);
                }

            }


        }

        [Test]
        public void SortedDictionary_Generic_Capacity_ClearDoesntTrim()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory();
                for (int i = 0; i < count; i++)
                {
                    AddToCollection(dictionary, 1);
                }
                dictionary.Clear();
            }
        }

        [Test]
        public void SortedDictionary_Generic_Capacity_ClearTrimsToInitialCapacity()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = new SortedDictionary<int, int>();
                AddToCollection(dictionary, count);
                dictionary.Clear();
            }
        }

#endregion

#region ContainsValue

        [Test]
        public void SortedDictionary_Generic_ContainsValue_NotPresent()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int seed = 4315;
                int notPresent = CreateTValue(seed++);
                while (dictionary.Values.Contains(notPresent))
                {
                    notPresent = CreateTValue(seed++);
                }
                Assert.False(dictionary.ContainsValue(notPresent));
            }
        }

        [Test]
        public void SortedDictionary_Generic_ContainsValue_Present()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int seed = 4315;
                KeyValuePair<int, int> notPresent = CreateT(seed++);
                while (dictionary.Contains(notPresent))
                {
                    notPresent = CreateT(seed++);
                }
                dictionary.Add(notPresent.Key, notPresent.Value);
                Assert.True(dictionary.ContainsValue(notPresent.Value));

            }
        }

        [Test]
        public void SortedDictionary_Generic_ContainsValue_DefaultValueNotPresent()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                Assert.False(dictionary.ContainsValue(default(int)));
            }
        }

        [Test]
        public void SortedDictionary_Generic_ContainsValue_DefaultValuePresent()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int seed = 4315;
                int notPresent = CreateTKey(seed++);
                while (dictionary.ContainsKey(notPresent))
                {
                    notPresent = CreateTKey(seed++);
                }
                dictionary.Add(notPresent, default(int));
                Assert.True(dictionary.ContainsValue(default(int)));
            }
        }

#endregion

#region IndexOfKey

        [Test]
        public void SortedDictionary_Generic_IndexOf_DefaultKeyNotContainedInSortedDictionary()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int key = default(int);
                if (dictionary.ContainsKey(key))
                {
                    dictionary.Remove(key);
                }
                Assert.AreEqual(-1, dictionary.Keys.ToList().IndexOf(key));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfKey_EachKey()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                // Assumes no duplicate elements contained in the dictionary returned by GenericIListFactory
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                var keys = dictionary.Keys;
                Enumerable.Range(0, count).ForEach(index =>
                {
                    var key = keys.ElementAt(index);
                    Assert.AreEqual(key, dictionary.ElementAt(index).Key);
                });
            }
        }

#endregion

#region IndexOfValue

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_DefaultValueNotContainedInList()
        {
            var data = ValidCollectionSizes();
            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int value = default(int);
                while (dictionary.ContainsValue(value))
                {
                    dictionary.Remove(dictionary.First(i => i.Value == value).Key);
                }
                Assert.AreEqual(-1, dictionary.Values.ToList().IndexOf(value));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_DefaultValueContainedInList()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int key = GetNewKey(dictionary);
                int value = default(int);
                while (dictionary.ContainsValue(value))
                {
                    dictionary.Remove(dictionary.First(i => i.Value == value).Key);
                }

                List<int> keys = dictionary.Keys.ToList();
                keys.Add(key);
                keys.Sort();
                int expectedIndex = keys.IndexOf(key);
                dictionary.Add(key, value);
                Assert.AreEqual(expectedIndex, dictionary.Values.ToList().IndexOf(value));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_ValueInCollectionMultipleTimes()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                int seed = 53214;
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                int key1 = CreateTKey(seed++);
                int key2 = CreateTKey(seed++);
                int value = CreateTValue(seed++);
                while (dictionary.ContainsKey(key1))
                {
                    key1 = CreateTKey(seed++);
                }
                while (key1.Equals(key2) || dictionary.ContainsKey(key2))
                {
                    key2 = CreateTKey(seed++);
                }
                while (dictionary.ContainsValue(value))
                {
                    dictionary.Remove(dictionary.First(i => i.Value == value).Key);
                }

                List<int> keys = dictionary.Keys.ToList();
                keys.Add(key1);
                keys.Add(key2);
                keys.Sort();
                int expectedIndex = Math.Min(keys.IndexOf(key1), keys.IndexOf(key2));

                dictionary.Add(key1, value);
                dictionary.Add(key2, value);
                Assert.AreEqual(expectedIndex, dictionary.Values.ToList().IndexOf(value));
            }
        }

        [Test]
        public void SortedDictionary_Generic_IndexOfValue_EachValue()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                // Assumes no duplicate elements contained in the dictionary returned by GenericIListFactory
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                var keys = dictionary.Keys;
                Enumerable.Range(0, count).ForEach(index =>
                {
                    var key = keys.ElementAt(index);
                    Assert.AreEqual(key, dictionary.ElementAt(index).Key);
                });
            }
        }

#endregion

#region RemoveAt

        private void RemoveAt(SortedDictionary<int, int> dictionary, KeyValuePair<int, int> element)
        {
            dictionary.Remove(element.Key);
        }

        [Test]
        public void SortedDictionary_Generic_RemoveAt_NonDefaultValueContainedInCollection()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                int seed = count * 251;
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                KeyValuePair<int, int> pair = CreateT(seed++);
                if (!dictionary.ContainsKey(pair.Key))
                {
                    dictionary.Add(pair.Key, pair.Value);
                    count++;
                }
                RemoveAt(dictionary, pair);
                Assert.AreEqual(count - 1, dictionary.Count);
            }
        }

        [Test]
        public void SortedDictionary_Generic_RemoveAt_EveryValue()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                dictionary.ToList().ForEach(value =>
                {
                    RemoveAt(dictionary, value);
                });
                Assert.True(dictionary.Count == 0);
            }
        }

        [Test]
        public void SortedDictionary_Generic_RemoveAt_OutOfRangeValues()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];
                SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(count);
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove(-1));
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove(int.MinValue));
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove(count));
                Assert.Throws<KeyNotFoundException>(() => dictionary.Remove(count + 1));

            }
        }

#endregion

#region TrimExcess

        [Test]
        public void SortedDictionary_Generic_TrimExcess_AfterClearingAndAddingSomeElementsBack()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int dictionaryLength = (int)testCase[0];

                if (dictionaryLength > 0)
                {
                    SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(dictionaryLength);
                    dictionary.Clear();
                    Assert.AreEqual(0, dictionary.Count);

                    AddToCollection(dictionary, dictionaryLength / 10);
                    Assert.AreEqual(dictionaryLength / 10, dictionary.Count);
                }
            }


        }

        [Test]
        public void SortedDictionary_Generic_TrimExcess_AfterClearingAndAddingAllElementsBack()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int dictionaryLength = (int)testCase[0];

                if (dictionaryLength > 0)
                {
                    SortedDictionary<int, int> dictionary = (SortedDictionary<int, int>)GenericIDictionaryFactory(dictionaryLength);
                    dictionary.Clear();
                    Assert.AreEqual(0, dictionary.Count);

                    AddToCollection(dictionary, dictionaryLength);
                    Assert.AreEqual(dictionaryLength, dictionary.Count);
                }
            }
        }

#endregion

#region IReadOnlyDictionary<int, int>.Keys

        [Test]
        public void IReadOnlyDictionary_Generic_Keys_ContainsAllCorrectKeys()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];

                IDictionary<int, int> dictionary = GenericIDictionaryFactory(count);
                IEnumerable<int> expected = dictionary.Select((pair) => pair.Key);
                IEnumerable<int> keys = ((IReadOnlyDictionary<int, int>)dictionary).Keys;
                Assert.True(expected.SequenceEqual(keys));
            }


        }

        [Test]
        public void IReadOnlyDictionary_Generic_Values_ContainsAllCorrectValues()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int count = (int)testCase[0];


                IDictionary<int, int> dictionary = GenericIDictionaryFactory(count);
                IEnumerable<int> expected = dictionary.Select((pair) => pair.Value);
                IEnumerable<int> values = ((IReadOnlyDictionary<int, int>)dictionary).Values;
                Assert.True(expected.SequenceEqual(values));
            }
        }

#endregion

        protected override KeyValuePair<int, int> CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new KeyValuePair<int, int>(rand.Next(), rand.Next());
        }

        protected override int CreateTKey(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override int CreateTValue(int seed) => CreateTKey(seed);
    }
}

#endif

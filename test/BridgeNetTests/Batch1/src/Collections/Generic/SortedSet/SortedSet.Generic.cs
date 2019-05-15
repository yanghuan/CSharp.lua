using Bridge.ClientTest.Collections.Generic.Base;
using Bridge.Test.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

#if false
namespace Bridge.ClientTest.Collections.Generic
{
    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_string - {0}")]
    public class SortedSet_Generic_Tests_string : SortedSet_Generic_Tests<string>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<string> set = new SortedSet<string>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<string> comparer = GetIComparer();
            SortedSet<string> set = new SortedSet<string>(comparer);
            Assert.AreEqual(comparer ?? Comparer<string>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<string> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<string> set = new SortedSet<string>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<string>((IEnumerable<string>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<string>((IEnumerable<string>)null, Comparer<string>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<string> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<string> set = new SortedSet<string>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<string> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<string> set = new SortedSet<string>(enumerable, GetIComparer() ?? Comparer<string>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<string> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<string> set = new SortedSet<string>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<string> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(string), set.Min);
                    Assert.AreEqual(default(string), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                    string firstElement = set.ElementAt(0);
                    string lastElement = set.ElementAt(setLength - 1);
                    SortedSet<string> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<string> comparer = GetIComparer() ?? Comparer<string>.Default;
                    SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                    string firstElement = set.ElementAt(1);
                    string lastElement = set.ElementAt(setLength - 2);

                    List<string> expected = new List<string>(setLength - 2);
                    foreach (string value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<string> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<string> comparer = GetIComparer() ?? Comparer<string>.Default;
                    SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                    string firstElement = set.ElementAt(0);
                    string lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                    IComparer<string> comparer = GetIComparer() ?? Comparer<string>.Default;
                    string firstElement = set.ElementAt(0);
                    string middleElement = set.ElementAt(setLength / 2);
                    string lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<string> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                string firstElement = set.ElementAt(0);
                string secondElement = set.ElementAt(1);
                string nextToLastElement = set.ElementAt(setLength - 2);
                string lastElement = set.ElementAt(setLength - 1);

                string[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<string> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(string), view.Min);
                Assert.AreEqual(default(string), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<string> set = (SortedSet<string>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                List<string> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (string value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                List<string> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (string value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                List<string> expected = set.ToList();
                expected.Sort(GetIComparer());
                string[] actual = new string[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                List<string> expected = set.ToList();
                expected.Sort(GetIComparer());
                string[] actual = new string[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<string> set = (SortedSet<string>)GenericISetFactory(setLength);
                string[] actual = new string[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<string> objects = new List<string>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<string>>()
            {
                new SortedSet<string> { objects[0], objects[1], objects[2] },
                new SortedSet<string> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<string>>()
            {
                new SortedSet<string> { objects[0], objects[1], objects[2] },
                new SortedSet<string> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<string>>(SortedSet<string>.CreateSetComparer())
            {
                new SortedSet<string> { objects[0], objects[1], objects[2] },
                new SortedSet<string> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<string>>(SortedSet<string>.CreateSetComparer())
            {
                new SortedSet<string> { objects[3], objects[4], objects[5] },
                new SortedSet<string> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_int - {0}")]
    public class SortedSet_Generic_Tests_int : SortedSet_Generic_Tests<int>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<int> set = new SortedSet<int>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<int> comparer = GetIComparer();
            SortedSet<int> set = new SortedSet<int>(comparer);
            Assert.AreEqual(comparer ?? Comparer<int>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<int> set = new SortedSet<int>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null, Comparer<int>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer() ?? Comparer<int>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<int> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(int), set.Min);
                    Assert.AreEqual(default(int), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(1);
                    int lastElement = set.ElementAt(setLength - 2);

                    List<int> expected = new List<int>(setLength - 2);
                    foreach (int value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    int firstElement = set.ElementAt(0);
                    int middleElement = set.ElementAt(setLength / 2);
                    int lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<int> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                int firstElement = set.ElementAt(0);
                int secondElement = set.ElementAt(1);
                int nextToLastElement = set.ElementAt(setLength - 2);
                int lastElement = set.ElementAt(setLength - 1);

                int[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<int> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(int), view.Min);
                Assert.AreEqual(default(int), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<int> set = (SortedSet<int>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (int value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (int value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int[] actual = new int[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<int> objects = new List<int>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[3], objects[4], objects[5] },
                new SortedSet<int> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected bool DefaultValueAllowed => true;

        [Test]
        public void SortedSet_Generic_GetViewBetween_MinMax()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 1, 3, 5, 7, 9 }, 5, 5);
            SortedSet<int> view = set.GetViewBetween(4, 8);

            Assert.True(set.Contains(1));
            Assert.True(set.Contains(3));
            Assert.True(set.Contains(5));
            Assert.True(set.Contains(7));
            Assert.True(set.Contains(9));

            Assert.False(view.Contains(1));
            Assert.False(view.Contains(3));
            Assert.True(view.Contains(5));
            Assert.True(view.Contains(7));
            Assert.False(view.Contains(9));

            Assert.AreEqual(1, set.Min);
            Assert.AreEqual(9, set.Max);

            Assert.AreEqual(5, view.Min);
            Assert.AreEqual(7, view.Max);
        }

        [Test]
        public void SortedSet_Generic_IntersectWith_SupersetEnumerableWithDups()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 1, 3, 5, 7, 9 }, 5, 5);
            set.IntersectWith(new[] { 5, 7, 3, 7, 11, 7, 5, 2 });

            Assert.AreDeepEqual(new[] { 3, 5, 7 }, set.ToArray());
        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MinMax_Exhaustive()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 7, 11, 3, 1, 5, 9, 13 }, 7, 7);
            for (int i = 0; i < 14; i++)
            {
                for (int j = i; j < 14; j ++)
                {
                    SortedSet<int> view = set.GetViewBetween(i, j);

                    if (j < i || (j == i && i % 2 == 0) )
                    {
                        Assert.AreEqual(default(int), view.Min);
                        Assert.AreEqual(default(int), view.Max);
                    }
                    else
                    {
                        Assert.AreEqual(i + ((i+1) % 2), view.Min);
                        Assert.AreEqual(j - ((j+1) % 2), view.Max);
                    }
                }
            }
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_int_With_NullComparer - {0}")]
    public class SortedSet_Generic_Tests_int_With_NullComparer : SortedSet_Generic_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_AbsOfInt());
        }

#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<int> set = new SortedSet<int>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<int> comparer = GetIComparer();
            SortedSet<int> set = new SortedSet<int>(comparer);
            Assert.AreEqual(comparer ?? Comparer<int>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<int> set = new SortedSet<int>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null, Comparer<int>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer() ?? Comparer<int>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<int> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(int), set.Min);
                    Assert.AreEqual(default(int), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(1);
                    int lastElement = set.ElementAt(setLength - 2);

                    List<int> expected = new List<int>(setLength - 2);
                    foreach (int value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    int firstElement = set.ElementAt(0);
                    int middleElement = set.ElementAt(setLength / 2);
                    int lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<int> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                int firstElement = set.ElementAt(0);
                int secondElement = set.ElementAt(1);
                int nextToLastElement = set.ElementAt(setLength - 2);
                int lastElement = set.ElementAt(setLength - 1);

                int[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<int> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(int), view.Min);
                Assert.AreEqual(default(int), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<int> set = (SortedSet<int>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (int value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (int value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int[] actual = new int[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<int> objects = new List<int>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[3], objects[4], objects[5] },
                new SortedSet<int> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override IComparer<int> GetIComparer() => null;
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_EquatableBackwardsOrder - {0}")]
    public class SortedSet_Generic_Tests_EquatableBackwardsOrder : SortedSet_Generic_Tests<EquatableBackwardsOrder>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<EquatableBackwardsOrder> set = new SortedSet<EquatableBackwardsOrder>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<EquatableBackwardsOrder> comparer = GetIComparer();
            SortedSet<EquatableBackwardsOrder> set = new SortedSet<EquatableBackwardsOrder>(comparer);
            Assert.AreEqual(comparer ?? Comparer<EquatableBackwardsOrder>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<EquatableBackwardsOrder> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<EquatableBackwardsOrder> set = new SortedSet<EquatableBackwardsOrder>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<EquatableBackwardsOrder>((IEnumerable<EquatableBackwardsOrder>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<EquatableBackwardsOrder>((IEnumerable<EquatableBackwardsOrder>)null, Comparer<EquatableBackwardsOrder>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<EquatableBackwardsOrder> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<EquatableBackwardsOrder> set = new SortedSet<EquatableBackwardsOrder>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<EquatableBackwardsOrder> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<EquatableBackwardsOrder> set = new SortedSet<EquatableBackwardsOrder>(enumerable, GetIComparer() ?? Comparer<EquatableBackwardsOrder>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<EquatableBackwardsOrder> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<EquatableBackwardsOrder> set = new SortedSet<EquatableBackwardsOrder>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<EquatableBackwardsOrder> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(EquatableBackwardsOrder), set.Min);
                    Assert.AreEqual(default(EquatableBackwardsOrder), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                    EquatableBackwardsOrder firstElement = set.ElementAt(0);
                    EquatableBackwardsOrder lastElement = set.ElementAt(setLength - 1);
                    SortedSet<EquatableBackwardsOrder> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<EquatableBackwardsOrder> comparer = GetIComparer() ?? Comparer<EquatableBackwardsOrder>.Default;
                    SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                    EquatableBackwardsOrder firstElement = set.ElementAt(1);
                    EquatableBackwardsOrder lastElement = set.ElementAt(setLength - 2);

                    List<EquatableBackwardsOrder> expected = new List<EquatableBackwardsOrder>(setLength - 2);
                    foreach (EquatableBackwardsOrder value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<EquatableBackwardsOrder> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<EquatableBackwardsOrder> comparer = GetIComparer() ?? Comparer<EquatableBackwardsOrder>.Default;
                    SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                    EquatableBackwardsOrder firstElement = set.ElementAt(0);
                    EquatableBackwardsOrder lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                    IComparer<EquatableBackwardsOrder> comparer = GetIComparer() ?? Comparer<EquatableBackwardsOrder>.Default;
                    EquatableBackwardsOrder firstElement = set.ElementAt(0);
                    EquatableBackwardsOrder middleElement = set.ElementAt(setLength / 2);
                    EquatableBackwardsOrder lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<EquatableBackwardsOrder> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                EquatableBackwardsOrder firstElement = set.ElementAt(0);
                EquatableBackwardsOrder secondElement = set.ElementAt(1);
                EquatableBackwardsOrder nextToLastElement = set.ElementAt(setLength - 2);
                EquatableBackwardsOrder lastElement = set.ElementAt(setLength - 1);

                EquatableBackwardsOrder[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<EquatableBackwardsOrder> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(EquatableBackwardsOrder), view.Min);
                Assert.AreEqual(default(EquatableBackwardsOrder), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                List<EquatableBackwardsOrder> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (EquatableBackwardsOrder value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                List<EquatableBackwardsOrder> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (EquatableBackwardsOrder value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                List<EquatableBackwardsOrder> expected = set.ToList();
                expected.Sort(GetIComparer());
                EquatableBackwardsOrder[] actual = new EquatableBackwardsOrder[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                List<EquatableBackwardsOrder> expected = set.ToList();
                expected.Sort(GetIComparer());
                EquatableBackwardsOrder[] actual = new EquatableBackwardsOrder[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<EquatableBackwardsOrder> set = (SortedSet<EquatableBackwardsOrder>)GenericISetFactory(setLength);
                EquatableBackwardsOrder[] actual = new EquatableBackwardsOrder[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<EquatableBackwardsOrder> objects = new List<EquatableBackwardsOrder>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<EquatableBackwardsOrder>>()
            {
                new SortedSet<EquatableBackwardsOrder> { objects[0], objects[1], objects[2] },
                new SortedSet<EquatableBackwardsOrder> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<EquatableBackwardsOrder>>()
            {
                new SortedSet<EquatableBackwardsOrder> { objects[0], objects[1], objects[2] },
                new SortedSet<EquatableBackwardsOrder> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<EquatableBackwardsOrder>>(SortedSet<EquatableBackwardsOrder>.CreateSetComparer())
            {
                new SortedSet<EquatableBackwardsOrder> { objects[0], objects[1], objects[2] },
                new SortedSet<EquatableBackwardsOrder> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<EquatableBackwardsOrder>>(SortedSet<EquatableBackwardsOrder>.CreateSetComparer())
            {
                new SortedSet<EquatableBackwardsOrder> { objects[3], objects[4], objects[5] },
                new SortedSet<EquatableBackwardsOrder> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override EquatableBackwardsOrder CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new EquatableBackwardsOrder(rand.Next());
        }

        protected override ISet<EquatableBackwardsOrder> GenericISetFactory()
        {
            return new SortedSet<EquatableBackwardsOrder>();
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_int_With_Comparer_SameAsDefaultComparer - {0}")]
    public class SortedSet_Generic_Tests_int_With_Comparer_SameAsDefaultComparer : SortedSet_Generic_Tests<int>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<int> set = new SortedSet<int>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<int> comparer = GetIComparer();
            SortedSet<int> set = new SortedSet<int>(comparer);
            Assert.AreEqual(comparer ?? Comparer<int>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<int> set = new SortedSet<int>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null, Comparer<int>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer() ?? Comparer<int>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<int> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(int), set.Min);
                    Assert.AreEqual(default(int), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(1);
                    int lastElement = set.ElementAt(setLength - 2);

                    List<int> expected = new List<int>(setLength - 2);
                    foreach (int value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    int firstElement = set.ElementAt(0);
                    int middleElement = set.ElementAt(setLength / 2);
                    int lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<int> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                int firstElement = set.ElementAt(0);
                int secondElement = set.ElementAt(1);
                int nextToLastElement = set.ElementAt(setLength - 2);
                int lastElement = set.ElementAt(setLength - 1);

                int[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<int> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(int), view.Min);
                Assert.AreEqual(default(int), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<int> set = (SortedSet<int>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (int value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (int value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int[] actual = new int[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<int> objects = new List<int>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[3], objects[4], objects[5] },
                new SortedSet<int> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_SameAsDefaultComparer();
        }

        protected override IComparer<int> GetIComparer()
        {
            return new Comparer_SameAsDefaultComparer();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_SameAsDefaultComparer());
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_int_With_Comparer_HashCodeAlwaysReturnsZero - {0}")]
    public class SortedSet_Generic_Tests_int_With_Comparer_HashCodeAlwaysReturnsZero : SortedSet_Generic_Tests<int>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<int> set = new SortedSet<int>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<int> comparer = GetIComparer();
            SortedSet<int> set = new SortedSet<int>(comparer);
            Assert.AreEqual(comparer ?? Comparer<int>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<int> set = new SortedSet<int>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null, Comparer<int>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer() ?? Comparer<int>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<int> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(int), set.Min);
                    Assert.AreEqual(default(int), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(1);
                    int lastElement = set.ElementAt(setLength - 2);

                    List<int> expected = new List<int>(setLength - 2);
                    foreach (int value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    int firstElement = set.ElementAt(0);
                    int middleElement = set.ElementAt(setLength / 2);
                    int lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<int> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                int firstElement = set.ElementAt(0);
                int secondElement = set.ElementAt(1);
                int nextToLastElement = set.ElementAt(setLength - 2);
                int lastElement = set.ElementAt(setLength - 1);

                int[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<int> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(int), view.Min);
                Assert.AreEqual(default(int), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<int> set = (SortedSet<int>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (int value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (int value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int[] actual = new int[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<int> objects = new List<int>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[3], objects[4], objects[5] },
                new SortedSet<int> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_HashCodeAlwaysReturnsZero();
        }

        protected override IComparer<int> GetIComparer()
        {
            return new Comparer_HashCodeAlwaysReturnsZero();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_HashCodeAlwaysReturnsZero());
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_int_With_Comparer_ModOfInt - {0}")]
    public class SortedSet_Generic_Tests_int_With_Comparer_ModOfInt : SortedSet_Generic_Tests<int>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<int> set = new SortedSet<int>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<int> comparer = GetIComparer();
            SortedSet<int> set = new SortedSet<int>(comparer);
            Assert.AreEqual(comparer ?? Comparer<int>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<int> set = new SortedSet<int>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null, Comparer<int>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer() ?? Comparer<int>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<int> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(int), set.Min);
                    Assert.AreEqual(default(int), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(1);
                    int lastElement = set.ElementAt(setLength - 2);

                    List<int> expected = new List<int>(setLength - 2);
                    foreach (int value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    int firstElement = set.ElementAt(0);
                    int middleElement = set.ElementAt(setLength / 2);
                    int lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<int> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                int firstElement = set.ElementAt(0);
                int secondElement = set.ElementAt(1);
                int nextToLastElement = set.ElementAt(setLength - 2);
                int lastElement = set.ElementAt(setLength - 1);

                int[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<int> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(int), view.Min);
                Assert.AreEqual(default(int), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<int> set = (SortedSet<int>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (int value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (int value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int[] actual = new int[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<int> objects = new List<int>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[3], objects[4], objects[5] },
                new SortedSet<int> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_ModOfInt(15000);
        }

        protected override IComparer<int> GetIComparer()
        {
            return new Comparer_ModOfInt(15000);
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_ModOfInt(15000));
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "SortedSet_Generic_Tests_int_With_Comparer_AbsOfInt - {0}")]
    public class SortedSet_Generic_Tests_int_With_Comparer_AbsOfInt : SortedSet_Generic_Tests<int>
    {
#region Constructors

        [Test]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<int> set = new SortedSet<int>();
            Assert.True(set.Count == 0);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            IComparer<int> comparer = GetIComparer();
            SortedSet<int> set = new SortedSet<int>(comparer);
            Assert.AreEqual(comparer ?? Comparer<int>.Default, set.Comparer);
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
                SortedSet<int> set = new SortedSet<int>(enumerable);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<int>((IEnumerable<int>)null, Comparer<int>.Default));
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer());
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netfx()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, GetIComparer() ?? Comparer<int>.Default);
                Assert.True(set.SetEquals(enumerable));
            }
        }

        [Test]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp()
        {
            var data = EnumerableTestData();

            foreach (var testCase in data)
            {
                EnumerableType enumerableType = (EnumerableType)testCase[0];
                int setLength = (int)testCase[1];
                int enumerableLength = (int)testCase[2];
                int numberOfMatchingElements = (int)testCase[3];
                int numberOfDuplicateElements = (int)testCase[4];

                IEnumerable<int> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
                SortedSet<int> set = new SortedSet<int>(enumerable, comparer: null);
                Assert.True(set.SetEquals(enumerable));
            }
        }

#endregion

#region Max and Min

        [Test]
        public void SortedSet_Generic_MaxAndMin()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                if (setLength > 0)
                {
                    List<int> expected = set.ToList();
                    expected.Sort(GetIComparer());
                    Assert.AreEqual(expected[0], set.Min);
                    Assert.AreEqual(expected[setLength - 1], set.Max);
                }
                else
                {
                    Assert.AreEqual(default(int), set.Min);
                    Assert.AreEqual(default(int), set.Max);
                }
            }
        }

#endregion

#region GetViewBetween

        [Test]
        public void SortedSet_Generic_GetViewBetween_EntireSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength > 0)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(setLength, view.Count);
                    Assert.True(set.SetEquals(view));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(1);
                    int lastElement = set.ElementAt(setLength - 2);

                    List<int> expected = new List<int>(setLength - 2);
                    foreach (int value in set)
                    {
                        if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        {
                            expected.Add(value);
                        }
                    }

                    SortedSet<int> view = set.GetViewBetween(firstElement, lastElement);
                    Assert.AreEqual(expected.Count, view.Count);
                    Assert.True(view.SetEquals(expected));
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 2)
                {
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    int firstElement = set.ElementAt(0);
                    int lastElement = set.ElementAt(setLength - 1);
                    if (comparer.Compare(firstElement, lastElement) < 0)
                    {
                        Assert.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength >= 3)
                {
                    SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                    IComparer<int> comparer = GetIComparer() ?? Comparer<int>.Default;
                    int firstElement = set.ElementAt(0);
                    int middleElement = set.ElementAt(setLength / 2);
                    int lastElement = set.ElementAt(setLength - 1);
                    if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                    {
                        SortedSet<int> view = set.GetViewBetween(firstElement, middleElement);
                        Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                    }
                }
            }


        }

        [Test]
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                if (setLength < 4) continue;

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                Assert.AreEqual(setLength, set.Count);

                int firstElement = set.ElementAt(0);
                int secondElement = set.ElementAt(1);
                int nextToLastElement = set.ElementAt(setLength - 2);
                int lastElement = set.ElementAt(setLength - 1);

                int[] items = set.ToArray();
                for (int i = 1; i < setLength - 1; i++)
                {
                    set.Remove(items[i]);
                }
                Assert.AreEqual(2, set.Count);

                SortedSet<int> view = set.GetViewBetween(secondElement, nextToLastElement);
                Assert.AreEqual(0, view.Count);

                Assert.AreEqual(default(int), view.Min);
                Assert.AreEqual(default(int), view.Max);
            }

        }

#endregion

#region RemoveWhere

        [Test]
        public void SortedSet_Generic_RemoveWhere_AllElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return true; });
                Assert.AreEqual(setLength, removedCount);
            }
        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NoElements()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int removedCount = set.RemoveWhere((value) => { return false; });
                Assert.AreEqual(0, removedCount);
                Assert.AreEqual(setLength, set.Count);
            }


        }

        [Test]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<int> set = (SortedSet<int>)GenericISetFactory();
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

#endregion

#region Enumeration and Ordering

        [Test]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int expectedIndex = 0;
                foreach (int value in set)
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                expected.Reverse();
                int expectedIndex = 0;
                foreach (int value in set.Reverse())
                {
                    Assert.AreEqual(expected[expectedIndex++], value);
                }
            }


        }

        [Test]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                {
                    sortedSet.Add(i);
                }
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.AreEqual(46, mySubSet.Count); //"not all elements were encountered"

            IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
        }

#endregion

#region CopyTo

        [Test]
        public void SortedSet_Generic_CopyTo_WithoutIndex()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_WithValidFullCount()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                List<int> expected = set.ToList();
                expected.Sort(GetIComparer());
                int[] actual = new int[setLength];
                set.CopyTo(actual, 0, setLength);
                Assert.AreDeepEqual(expected.ToArray(), actual);
            }


        }

        [Test]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var data = ValidCollectionSizes();

            foreach (var testCase in data)
            {
                int setLength = (int)testCase[0];

                SortedSet<int> set = (SortedSet<int>)GenericISetFactory(setLength);
                int[] actual = new int[setLength];
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
            }


        }

#endregion

#region CreateSetComparer

        [Test]
        public void SetComparer_SetEqualsTests()
        {
            List<int> objects = new List<int>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new HashSet<SortedSet<int>>()
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[0], objects[1], objects[2] },
                new SortedSet<int> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer())
            {
                new SortedSet<int> { objects[3], objects[4], objects[5] },
                new SortedSet<int> { objects[0], objects[1], objects[2] }
            };

            Assert.False(noComparerSet.SetEquals(set));
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
#endregion

        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_AbsOfInt();
        }

        protected override IComparer<int> GetIComparer()
        {
            return new Comparer_AbsOfInt();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_AbsOfInt());
        }
    }
}
#endif

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bridge.Test.NUnit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.ClientTest.Collections.Generic
{
    /// <summary>
    /// Helper class that verifies some properties of the linked list.
    /// </summary>
    internal class LinkedList_T_Tests<T>
    {
        private readonly LinkedList<T> _collection;
        private readonly T[] _expectedItems;

        /// <summary>
        /// Tests the linked list based on the expected items and the given linked list.
        /// </summary>
        internal LinkedList_T_Tests(LinkedList<T> collection, T[] expectedItems)
        {
            _collection = collection;
            _expectedItems = expectedItems;
        }

        /// <summary>
        /// Tests the initial items in the list.
        /// </summary>
        [Test]
        public void InitialItems_Tests()
        {
            VerifyState(_collection, _expectedItems);
            VerifyGenericEnumerator(_collection, _expectedItems);
            VerifyEnumerator(_collection, _expectedItems);
        }

        /// <summary>
        /// Verifies that the tail/head properties are valid and
        /// can iterate through the list (backwards and forwards) to
        /// verify the contents of the list.
        /// </summary>
        private static void VerifyState(LinkedList<T> linkedList, T[] expectedItems)
        {
            T[] tempArray;
            int index;
            LinkedListNode<T> currentNode, previousNode, nextNode;

            //[] Verify Count
            Assert.AreEqual(expectedItems.Length, linkedList.Count); //"Err_0821279 List.Count"

            //[] Verify Head/Tail
            if (expectedItems.Length == 0)
            {
                Assert.Null(linkedList.First); //"Err_48928ahid Expected Head to be null\n"
                Assert.Null(linkedList.Last); //"Err_56418ahjidi Expected Tail to be null\n"
            }
            else if (expectedItems.Length == 1)
            {
                VerifyLinkedListNode(linkedList.First, expectedItems[0], linkedList, null, null);
                VerifyLinkedListNode(linkedList.Last, expectedItems[0], linkedList, null, null);
            }
            else
            {
                VerifyLinkedListNode(linkedList.First, expectedItems[0], linkedList, true, false);
                VerifyLinkedListNode(linkedList.Last, expectedItems[expectedItems.Length - 1], linkedList, false, true);
            }

            //[] Moving forward through he collection starting at head
            currentNode = linkedList.First;
            previousNode = null;
            index = 0;

            while (currentNode != null)
            {
                nextNode = currentNode.Next;

                VerifyLinkedListNode(currentNode, expectedItems[index], linkedList, previousNode, nextNode);

                previousNode = currentNode;
                currentNode = currentNode.Next;

                ++index;
            }

            //[] Moving backward through he collection starting at Tail
            currentNode = linkedList.Last;
            nextNode = null;
            index = 0;

            while (currentNode != null)
            {
                previousNode = currentNode.Previous;
                VerifyLinkedListNode(currentNode, expectedItems[expectedItems.Length - 1 - index], linkedList, previousNode, nextNode);

                nextNode = currentNode;
                currentNode = currentNode.Previous;

                ++index;
            }

            //[] Verify Contains
            for (int i = 0; i < expectedItems.Length; i++)
            {
                Assert.True(linkedList.Contains(expectedItems[i]),
                    "Err_9872haid Expected Contains with item=" + expectedItems[i] + " to return true");
            }

            //[] Verify CopyTo
            tempArray = new T[expectedItems.Length];
            linkedList.CopyTo(tempArray, 0);

            for (int i = 0; i < expectedItems.Length; i++)
            {
                Assert.AreEqual(expectedItems[i], tempArray[i]); //"Err_0310auazp After CopyTo index=" + i.ToString()
            }

            //[] Verify Enumerator()
            index = 0;
            foreach (T item in linkedList)
            {
                Assert.AreEqual(expectedItems[index], item); //"Err_0310auazp Enumerator index=" + index.ToString()
                ++index;
            }
        }

        /// <summary>
        /// Verifies that the contents of a linkedlistnode are correct.
        /// </summary>
        private static void VerifyLinkedListNode(LinkedListNode<T> node, T expectedValue, LinkedList<T> expectedList,
            LinkedListNode<T> expectedPrevious, LinkedListNode<T> expectedNext)
        {
            Assert.AreEqual(expectedValue, node.Value); //"Err_548ajoid Node Value"
            Assert.AreEqual(expectedList.ToArray(), node.List.ToArray()); //"Err_0821279 Node List"

            if (expectedPrevious != null && node.Previous != null)
            {
                Assert.AreEqual(expectedPrevious.Value, node.Previous.Value); //"Err_8548ajhiod Previous Node"
            }
            else
            {
                Assert.Null(expectedPrevious);
                Assert.Null(node.Previous);
            }

            if (expectedNext != null && node.Next != null)
            {
                Assert.AreEqual(expectedNext.Value, node.Next.Value); //"Err_8548ajhiod Previous Node"
            }
            else
            {
                Assert.Null(expectedNext);
                Assert.Null(node.Next);
            }
        }

        /// <summary>
        /// verifies that the contents of a linkedlist node are correct.
        /// </summary>
        private static void VerifyLinkedListNode(LinkedListNode<T> node, T expectedValue, LinkedList<T> expectedList,
            bool expectedPreviousNull, bool expectedNextNull)
        {
            Assert.AreEqual(expectedValue, node.Value); //"Err_548ajoid Expected Node Value"
            Assert.AreDeepEqual(expectedList.ToArray(), node.List.ToArray()); //"Err_0821279 Expected Node List"

            if (expectedPreviousNull)
            {
                Assert.Null(node.Previous); //"Expected node.Previous to be null."
            }
            else
            {
                Assert.NotNull(node.Previous); //"Expected node.Previous not to be null"
            }

            if (expectedNextNull)
            {
                Assert.Null(node.Next); //"Expected node.Next to be null."
            }
            else
            {
                Assert.NotNull(node.Next); //"Expected node.Next not to be null"
            }
        }

        /// <summary>
        /// Verifies that the generic enumerator retrieves the correct items.
        /// </summary>
        private void VerifyGenericEnumerator(ICollection<T> collection, T[] expectedItems)
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            int iterations = 0;
            int expectedCount = expectedItems.Length;

            //[] Verify non deterministic behavior of current every time it is called before a call to MoveNext() has been made
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    T tempCurrent = enumerator.Current;
                }
                catch (Exception) { }
            }

            // There is a sequential order to the collection, so we're testing for that.
            while ((iterations < expectedCount) && enumerator.MoveNext())
            {
                T currentItem = enumerator.Current;
                T tempItem;

                //[] Verify we have not gotten more items then we expected
                Assert.True(iterations < expectedCount,
                    "Err_9844awpa More items have been returned from the enumerator(" +
                    iterations + " items) than are in the expectedElements(" +
                    expectedCount + " items)");

                //[] Verify Current returned the correct value
                Assert.AreEqual(currentItem, expectedItems[iterations]); //"Err_1432pauy Current returned unexpected value at index: " + iterations

                //[] Verify Current always returns the same value every time it is called
                for (int i = 0; i < 3; i++)
                {
                    tempItem = enumerator.Current;
                    Assert.AreEqual(currentItem, tempItem); //"Err_8776phaw Current is returning inconsistent results"
                }

                iterations++;
            }

            Assert.AreEqual(expectedCount, iterations); //"Err_658805eauz Number of items to iterate through"

            for (int i = 0; i < 3; i++)
            {
                Assert.False(enumerator.MoveNext()); //"Err_2929ahiea Expected MoveNext to return false after" + iterations + " iterations"
            }

            //[] Verify non deterministic behavior of current every time it is called after the enumerator is positioned after the last item
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    T tempCurrent = enumerator.Current;
                }
                catch (Exception) { }
            }

            enumerator.Dispose();
        }

        /// <summary>
        /// Verifies that the non-generic enumerator retrieves the correct items.
        /// </summary>
        private void VerifyEnumerator(ICollection<T> collection, T[] expectedItems)
        {
            IEnumerator enumerator = collection.GetEnumerator();
            int iterations = 0;
            int expectedCount = expectedItems.Length;

            //[] Verify non deterministic behavior of current every time it is called before a call to MoveNext() has been made
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    object tempCurrent = enumerator.Current;
                }
                catch (Exception) { }
            }

            // There is no sequential order to the collection, so we're testing that all the items
            // in the readonlydictionary exist in the array.
            bool[] itemsVisited = new bool[expectedCount];
            bool itemFound;
            while ((iterations < expectedCount) && enumerator.MoveNext())
            {
                object currentItem = enumerator.Current;
                object tempItem;

                //[] Verify we have not gotten more items then we expected
                Assert.True(iterations < expectedCount,
                    "Err_9844awpa More items have been returned from the enumerator(" + iterations + " items) then are in the expectedElements(" + expectedCount + " items)");

                //[] Verify Current returned the correct value
                itemFound = false;

                for (int i = 0; i < itemsVisited.Length; ++i)
                {
                    if (!itemsVisited[i] && expectedItems[i].Equals(currentItem))
                    {
                        itemsVisited[i] = true;
                        itemFound = true;
                        break;
                    }
                }
                Assert.True(itemFound, "Err_1432pauy Current returned unexpected value=" + currentItem);

                //[] Verify Current always returns the same value every time it is called
                for (int i = 0; i < 3; i++)
                {
                    tempItem = enumerator.Current;
                    Assert.AreEqual(currentItem, tempItem); //"Err_8776phaw Current is returning inconsistent results Current."
                }

                iterations++;
            }

            for (int i = 0; i < expectedCount; ++i)
            {
                Assert.True(itemsVisited[i], "Err_052848ahiedoi Expected Current to return true for item: " + expectedItems[i] + "index: " + i);
            }

            Assert.AreEqual(expectedCount, iterations); //"Err_658805eauz Number of items to iterate through"

            for (int i = 0; i < 3; i++)
            {
                Assert.False(enumerator.MoveNext()); //"Err_2929ahiea Expected MoveNext to return false after" + iterations + " iterations"
            }

            //[] Verify non deterministic behavior of current every time it is called after the enumerator is positioned after the last item
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    object tempCurrent = enumerator.Current;
                }
                catch (Exception) { }
            }
        }
    }

    /// <summary>
    /// Class to test reference type.
    /// </summary>
    internal class LinkedList_Person
    {
        internal readonly string Name;

        internal readonly int Age;

        internal LinkedList_Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override bool Equals(object obj)
        {
            LinkedList_Person given = obj as LinkedList_Person;
            if (given == null)
            {
                return false;
            }

            bool isNameEqual = Name.Equals(given.Name);
            bool isAgeEqual = Age.Equals(given.Age);
            return isNameEqual && isAgeEqual;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "LinkedList_Generic_Tests_string - {0}")]
    public class LinkedList_Generic_Tests_string : LinkedList_Generic_Tests<string>
    {
        #region Constructor_IEnumerable

        [Test]
        public void LinkedList_Generic_Constructor_IEnumerable()
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
                LinkedList<string> queue = new LinkedList<string>(enumerable);
                Assert.AreDeepEqual(enumerable.ToArray(), queue.ToArray());
            }


        }

        [Test]
        public void LinkedList_Generic_Constructor_IEnumerable_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkedList<string>(null));
        }

        #endregion

        [Test]
        public void AddAfter_LLNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 8293;
            string[] tempItems, headItems, headItemsReverse, tailItems;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddAfter(linkedList.First, default(string));
            InitialItems_Tests(linkedList, new string[] { headItems[0], default(string) });

            //[] Node is the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Node is after the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 2, headItems.Length - 2);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First.Next, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 1, headItems.Length - 2);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 1, headItems.Length - 3);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Call AddAfter several times remove some of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[headItems.Length - 6];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            string[] tempItems2 = new string[tempItems.Length + tailItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(tailItems, 0, tempItems2, tempItems.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddAfter several times remove all of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddAfter several times then call Clear
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddAfter_LLNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 8293;
            string[] items;

            //[] Verify Null node
            linkedList = new LinkedList<string>();
            Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(null, CreateT(seed++))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"

            InitialItems_Tests(linkedList, new string[0]);
            //[] Verify Node that is a new Node
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(new LinkedListNode<string>(CreateT(seed++)), CreateT(seed++))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(tempLinkedList.Last, CreateT(seed++))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddAfter_LLNode_LLNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int seed = 8293;
            int arraySize = 16;
            string[] tempItems, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];
            tailItemsReverse = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddAfter(linkedList.First, new LinkedListNode<string>(default(string)));
            InitialItems_Tests(linkedList, new string[] { headItems[0], default(string) });

            //[] Node is the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Node is after the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 2, headItems.Length - 2);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First.Next, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 1, headItems.Length - 2);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 1, headItems.Length - 3);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous.Previous, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);


            //[] Call AddAfter several times remove some of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(headItems[i]));
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[headItems.Length - 6];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(tailItems[i]));
            }

            string[] tempItems2 = new string[tempItems.Length + tailItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(tailItems, 0, tempItems2, tempItems.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddAfter several times remove all of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(headItems[i]));
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddAfter several times then call Clear
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(headItems[i]));
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                if (0 == (i & 1))
                {
                    linkedList.AddBefore(linkedList.First, headItems[i]);
                    linkedList.AddAfter(linkedList.Last, tailItems[i]);
                }
                else
                {
                    linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(headItems[i]));
                    linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(tailItems[i]));
                }
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddAfter_LLNode_LLNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 8293;
            string[] items;

            //[] Verify Null node
            linkedList = new LinkedList<string>();
            Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(null, new LinkedListNode<string>(CreateT(seed++)))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new string[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(new LinkedListNode<string>(CreateT(seed++)), new LinkedListNode<string>(CreateT(seed++)))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(tempLinkedList.Last, new LinkedListNode<string>(CreateT(seed++)))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            // negative tests on NewNode

            //[] Verify Null newNode
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(linkedList.First, (LinkedListNode<string>)null)); //"Err_0808ajeoia Expected null newNode to throws ArgumentNullException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in this collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(linkedList.First, linkedList.Last)); //"Err_58808adjioe Verify newNode that already exists in this collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(linkedList.First, tempLinkedList.Last)); //"Err_54808ajied newNode that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddBefore_LLNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 8293;
            string[] tempItems, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];
            tailItemsReverse = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddBefore(linkedList.First, default(string));
            InitialItems_Tests(linkedList, new string[] { default(string), headItems[0] });

            //[] Node is the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Node is the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 1, tempItems, 0, headItems.Length - 1);
            tempItems[tempItems.Length - 1] = headItems[0];

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is after the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First.Next, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 0, headItems.Length - 2);
            tempItems[tempItems.Length - 2] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 3);
            tempItems[tempItems.Length - 3] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Call AddBefore several times remove some of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();

            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, tailItems[i]);
            }

            string[] tempItems2 = new string[tailItemsReverse.Length + tempItems.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddBefore several times remove all of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Call AddBefore several times then call Clear
            linkedList.Clear();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddBefore_LLNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 8293;
            string[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddBefore(null, CreateT(seed++))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new string[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(new LinkedListNode<string>(CreateT(seed++)), CreateT(seed++))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(tempLinkedList.Last, CreateT(seed++))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddBefore_LLNode_LLNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 8293;
            string[] tempItems, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];
            tailItemsReverse = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(default(string)));
            InitialItems_Tests(linkedList, new string[] { default(string), headItems[0] });

            //[] Node is the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Node is the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 1, tempItems, 0, headItems.Length - 1);
            tempItems[tempItems.Length - 1] = headItems[0];

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is after the Head
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First.Next, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 0, headItems.Length - 2);
            tempItems[tempItems.Length - 2] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new string[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 3);
            tempItems[tempItems.Length - 3] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous.Previous, new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Call AddBefore several times remove some of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(headItems[i]));
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(tailItems[i]));
            }

            string[] tempItems2 = new string[tailItemsReverse.Length + tempItems.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddBefore several times remove all of the items
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(headItems[i]));
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Call AddBefore several times then call Clear
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(headItems[i]));
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<string>(headItems[i]));
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<string>(tailItems[i]));
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddBefore_LLNode_LLNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 8293;
            string[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddBefore(null, new LinkedListNode<string>(CreateT(seed++)))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new string[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(new LinkedListNode<string>(CreateT(seed++)), new LinkedListNode<string>(CreateT(seed++)))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(tempLinkedList.Last, new LinkedListNode<string>(CreateT(seed++)))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            // Tests for the NewNode

            //[] Verify Null newNode
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<ArgumentNullException>(() => linkedList.AddBefore(linkedList.First, (LinkedListNode<string>)null)); //"Err_0808ajeoia Expected null newNode to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in this collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(linkedList.First, linkedList.Last)); //"Err_58808adjioe Verify newNode that already exists in this collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(linkedList.First, tempLinkedList.Last)); //"Err_54808ajied newNode that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddFirst_T_Tests()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int seed = 21543;
            int arraySize = 16;
            string[] tempItems, tempItems2, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];
            tailItemsReverse = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(default(string));
            InitialItems_Tests(linkedList, new string[] { default(string) });

            //[] Call AddHead(string) several times
            linkedList = new LinkedList<string>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Call AddHead(string) several times remove some of the items
            linkedList = new LinkedList<string>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            // expected items are headItems in reverse order, or a subset of them.
            tempItems = new string[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);
            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(tailItems[i]);
            }

            tempItems2 = new string[tempItems.Length + tailItemsReverse.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);
            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddHead(string) several times remove all of the items
            linkedList = new LinkedList<string>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Call AddHead(string) several times then call Clear
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            linkedList.Clear();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddHead and AddTail calls
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
                linkedList.AddLast(tailItems[i]);
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddFirst_LinkedListNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] tempItems, tempItems2, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];
            tailItemsReverse = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(new LinkedListNode<string>(default(string)));
            InitialItems_Tests(linkedList, new string[] { default(string) });

            //[] Call AddHead(LinkedListNode<string>) several times
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Call AddHead(LinkedListNode<string>) several times remove some of the items
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(headItems[i]));
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);
            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(tailItems[i]));
            }

            tempItems2 = new string[tailItemsReverse.Length + tempItems.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);
            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddHead(LinkedListNode<string>) several times remove all of the items
            linkedList = new LinkedList<string>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(headItems[i]));
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddHead and AddTail calls
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(headItems[i]));
                linkedList.AddLast(new LinkedListNode<string>(tailItems[i]));
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddFirst_LinkedListNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 21543;
            string[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddFirst((LinkedListNode<string>)null)); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new string[0]);

            //[] Verify Node that already exists in this collection that is the Head
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddFirst(linkedList.First)); //"Err_0568ajods Expected Node that already exists in this collection that is the Head throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in this collection that is the Tail
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddFirst(linkedList.Last)); //"Err_98809ahied Expected Node that already exists in this collection that is the Tail throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddFirst(tempLinkedList.Last)); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddLast_T_Tests()
        {
            int arraySize = 16;
            int seed = 21543;
            string[] tempItems, headItems, tailItems;

            headItems = new string[16];
            tailItems = new string[16];
            for (int i = 0; i < 16; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Verify value is default(string)
            LinkedList<string> linkedList = new LinkedList<string>();
            linkedList.AddLast(default(string));
            InitialItems_Tests(linkedList, new string[] { default(string) });

            //[] Call AddTail(string) several times
            linkedList.Clear();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddTail(string) several times remove some of the items
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            linkedList.Remove(tailItems[2]);
            linkedList.Remove(tailItems[tailItems.Length - 3]);
            linkedList.Remove(tailItems[1]);
            linkedList.Remove(tailItems[tailItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[tailItems.Length - 6];
            Array.Copy(tailItems, 3, tempItems, 0, tailItems.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            //[] adding some more items to the tail of the linked list.
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            string[] tempItems2 = new string[tempItems.Length + headItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(headItems, 0, tempItems2, tempItems.Length, headItems.Length);
            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddTail(string) several times then call Clear
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            linkedList.Clear();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Mix AddTail and AddTail calls
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
                linkedList.AddLast(tailItems[i]);
            }

            tempItems = new string[headItems.Length];
            // adding the headItems in reverse order.
            for (int i = 0; i < headItems.Length; i++)
            {
                int index = (headItems.Length - 1) - i;
                tempItems[i] = headItems[index];
            }

            tempItems2 = new string[tempItems.Length + tailItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(tailItems, 0, tempItems2, tempItems.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems2);
        }

        [Test]
        public void AddLast_LinkedListNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] tempItems, headItems, headItemsReverse, tailItems;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            headItemsReverse = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                string head = CreateT(seed++);
                string tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
            }

            //[] Verify value is default(string)
            linkedList = new LinkedList<string>();
            linkedList.AddLast(new LinkedListNode<string>(default(string)));
            InitialItems_Tests(linkedList, new string[] { default(string) });

            //[] Call AddTail(LinkedListNode<string>) several times
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<string>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddTail(LinkedListNode<string>) several times remove some of the items
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<string>(tailItems[i]));
            }

            linkedList.Remove(tailItems[2]);
            linkedList.Remove(tailItems[tailItems.Length - 3]);
            linkedList.Remove(tailItems[1]);
            linkedList.Remove(tailItems[tailItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();

            //With the above remove we should have removed the first and last 3 items
            tempItems = new string[tailItems.Length - 6];
            Array.Copy(tailItems, 3, tempItems, 0, tailItems.Length - 6);
            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<string>(headItems[i]));
            }

            string[] tempItems2 = new string[tempItems.Length + headItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(headItems, 0, tempItems2, tempItems.Length, headItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddTail(string) several times then call Clear
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<string>(tailItems[i]));
            }

            linkedList.Clear();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<string>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Mix AddTail and AddTail calls
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<string>(headItems[i]));
                linkedList.AddLast(new LinkedListNode<string>(tailItems[i]));
            }

            tempItems = new string[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddLast_LinkedListNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 21543;
            string[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddLast((LinkedListNode<string>)null)); //"Err_858ahia Expected null node to throws ArgumentNullException"
            InitialItems_Tests(linkedList, new string[0]);

            //[] Verify Node that already exists in this collection that is the Head
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddLast(linkedList.First)); //"Err_0568ajods Expected Node that already exists in this collection that is the Head throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in this collection that is the Tail
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddLast(linkedList.Last)); //"Err_98809ahied Expected Node that already exists in this collection that is the Tail throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddLast(tempLinkedList.Last)); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public static void CtorTest()
        {
            LinkedList_T_Tests<string> helper = new LinkedList_T_Tests<string>(new LinkedList<string>(), new string[0]);
            helper.InitialItems_Tests();
            LinkedList_T_Tests<LinkedList_Person> helper2 = new LinkedList_T_Tests<LinkedList_Person>(new LinkedList<LinkedList_Person>(), new LinkedList_Person[0]);
            helper2.InitialItems_Tests();
        }

        [Test]
        public static void Ctor_IEnumerableTest()
        {
            int arraySize = 16;
            int[] intArray = new int[arraySize];
            LinkedList_Person[] personArray = new LinkedList_Person[arraySize];

            int printableCharIndex = 65; // ascii "A"
            for (int i = 0; i < arraySize; ++i)
            {
                intArray[i] = i;
                char asChar = (char)(printableCharIndex + i);
                personArray[i] = new LinkedList_Person(asChar.ToString(), i);
            }

            // Testing with value types
            LinkedList<int> intList = new LinkedList<int>(Clone(intArray));
            LinkedList_T_Tests<int> helper = new LinkedList_T_Tests<int>(intList, intArray);
            helper.InitialItems_Tests();

            intArray = new int[0];
            intList = new LinkedList<int>(Clone(intArray));
            helper = new LinkedList_T_Tests<int>(intList, intArray);
            helper.InitialItems_Tests();

            intArray = new int[] { -3 };
            intList = new LinkedList<int>(Clone(intArray));
            helper = new LinkedList_T_Tests<int>(intList, intArray);
            helper.InitialItems_Tests();

            // Testing with an reference type
            LinkedList<LinkedList_Person> personList = new LinkedList<LinkedList_Person>(Clone(personArray));
            LinkedList_T_Tests<LinkedList_Person> helper2 = new LinkedList_T_Tests<LinkedList_Person>(personList, personArray);
            helper2.InitialItems_Tests();

            personArray = new LinkedList_Person[0];
            personList = new LinkedList<LinkedList_Person>(Clone(personArray));
            helper2 = new LinkedList_T_Tests<LinkedList_Person>(personList, personArray);
            helper2.InitialItems_Tests();

            personArray = new LinkedList_Person[] { new LinkedList_Person("Jack", 18) };
            personList = new LinkedList<LinkedList_Person>(Clone(personArray));
            helper2 = new LinkedList_T_Tests<LinkedList_Person>(personList, personArray);
            helper2.InitialItems_Tests();
        }

        [Test]
        public static void Ctor_IEnumerableTest_Negative()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkedList<string>(null)); //"Err_982092 Expected ArgumentNullException to be thrown with null collection"
        }

        private static IEnumerable<T2> Clone<T2>(T2[] source)
        {
            if (source == null)
            {
                return null;
            }
            return (T2[])source.Clone();
        }

        [Test]
        public void Find_T()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] headItems, tailItems;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call Find an empty collection
            linkedList = new LinkedList<string>();
            Assert.Null(linkedList.Find(headItems[0])); //"Err_2899hjaied Expected Find to return false with a non null item on an empty collection"
            Assert.Null(linkedList.Find(default(string))); //"Err_5808ajiea Expected Find to return false with a null item on an empty collection"

            //[] Call Find on a collection with one item in it
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            Assert.Null(linkedList.Find(headItems[1])); //"Err_2899hjaied Expected Find to return false with a non null item on an empty collection size=1"
            Assert.Null(linkedList.Find(default(string))); //"Err_5808ajiea Expected Find to return false with a null item on an empty collection size=1"
            VerifyFind(linkedList, new string[] { headItems[0] });

            //[] Call Find on a collection with two items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            Assert.Null(linkedList.Find(headItems[2])); //"Err_5680ajoed Expected Find to return false with an non null item not in the collection size=2"
            Assert.Null(linkedList.Find(default(string))); //"Err_858196aieh Expected Find to return false with an null item not in the collection size=2"
            VerifyFind(linkedList, new string[] { headItems[0], headItems[1] });

            //[] Call Find on a collection with three items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            Assert.Null(linkedList.Find(headItems[3])); //"Err_50878adie Expected Find to return false with an non null item not in the collection size=3"
            Assert.Null(linkedList.Find(default(string))); //"Err_3969887wiqpi Expected Find to return false with an null item not in the collection size=3"
            VerifyFind(linkedList, new string[] { headItems[0], headItems[1], headItems[2] });

            //[] Call Find on a collection with multiple items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_5808ajdoi Expected Find to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.Find(default(string))); //"Err_5588aied Expected Find to return false with an null item not in the collection size=16"
            VerifyFind(linkedList, headItems);

            //[] Call Find on a collection with duplicate items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_8548ajia Expected Find to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.Find(default(string))); //"Err_3108qoa Expected Find to return false with an null item not in the collection size=16"
            string[] tempItems = new string[headItems.Length + headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Copy(headItems, 0, tempItems, headItems.Length, headItems.Length);
            VerifyFindDuplicates(linkedList, tempItems);


            //[] Call Find with default(string) at the beginning
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddFirst(default(string));

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_8548ajia Expected Find to return false with an non null item not in the collection default(string) at the beginning"

            tempItems = new string[headItems.Length + 1];
            tempItems[0] = default(string);
            Array.Copy(headItems, 0, tempItems, 1, headItems.Length);

            VerifyFind(linkedList, tempItems);

            //[] Call Find with default(string) in the middle
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(string));
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            Assert.Null(linkedList.Find(CreateT(seed++))); //"Err_78585ajhed Expected Find to return false with an non null item not in the collection default(string) in the middle"

            // prepending tempitems2 to tailitems into tempitems
            tempItems = new string[tailItems.Length + 1];
            tempItems[0] = default(string);
            Array.Copy(tailItems, 0, tempItems, 1, tailItems.Length);

            string[] tempItems2 = new string[headItems.Length + tempItems.Length];
            Array.Copy(headItems, 0, tempItems2, 0, headItems.Length);
            Array.Copy(tempItems, 0, tempItems2, headItems.Length, tempItems.Length);

            VerifyFind(linkedList, tempItems2);

            //[] Call Find on a collection with duplicate items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(string));

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_208089ajdi Expected Find to return false with an non null item not in the collection default(string) at the end"
            tempItems = new string[headItems.Length + 1];
            tempItems[headItems.Length] = default(string);
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            VerifyFind(linkedList, tempItems);
        }

        [Test]
        public void FindLast_T()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int seed = 21543;
            int arraySize = 16;
            string[] headItems, tailItems, prependDefaultHeadItems, prependDefaultTailItems;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }
            prependDefaultHeadItems = new string[headItems.Length + 1];
            prependDefaultHeadItems[0] = default(string);
            Array.Copy(headItems, 0, prependDefaultHeadItems, 1, headItems.Length);

            prependDefaultTailItems = new string[tailItems.Length + 1];
            prependDefaultTailItems[0] = default(string);
            Array.Copy(tailItems, 0, prependDefaultTailItems, 1, tailItems.Length);

            //[] Call FindLast an empty collection
            linkedList = new LinkedList<string>();
            Assert.Null(linkedList.FindLast(headItems[0])); //"Err_2899hjaied Expected FindLast to return false with a non null item on an empty collection"
            Assert.Null(linkedList.FindLast(default(string))); //"Err_5808ajiea Expected FindLast to return false with a null item on an empty collection"

            //[] Call FindLast on a collection with one item in it
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            Assert.Null(linkedList.FindLast(headItems[1])); //"Err_2899hjaied Expected FindLast to return false with a non null item on an empty collection size=1"
            Assert.Null(linkedList.FindLast(default(string))); //"Err_5808ajiea Expected FindLast to return false with a null item on an empty collection size=1"
            VerifyFindLast(linkedList, new string[] { headItems[0] });

            //[] Call FindLast on a collection with two items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            Assert.Null(linkedList.FindLast(headItems[2])); //"Err_5680ajoed Expected FindLast to return false with an non null item not in the collection size=2"
            Assert.Null(linkedList.FindLast(default(string))); //"Err_858196aieh Expected FindLast to return false with an null item not in the collection size=2"
            VerifyFindLast(linkedList, new string[] { headItems[0], headItems[1] });

            //[] Call FindLast on a collection with three items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            Assert.Null(linkedList.FindLast(headItems[3])); //"Err_50878adie Expected FindLast to return false with an non null item not in the collection size=3"
            Assert.Null(linkedList.FindLast(default(string))); //"Err_3969887wiqpi Expected FindLast to return false with an null item not in the collection size=3"
            VerifyFindLast(linkedList, new string[] { headItems[0], headItems[1], headItems[2] });

            //[] Call FindLast on a collection with multiple items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_5808ajdoi Expected FindLast to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.FindLast(default(string))); //"Err_5588aied Expected FindLast to return false with an null item not in the collection size=16"
            VerifyFindLast(linkedList, headItems);

            //[] Call FindLast on a collection with duplicate items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_8548ajia Expected FindLast to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.FindLast(default(string))); //"Err_3108qoa Expected FindLast to return false with an null item not in the collection size=16"
            string[] tempItems = new string[headItems.Length + headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Copy(headItems, 0, tempItems, headItems.Length, headItems.Length);
            VerifyFindLastDuplicates(linkedList, tempItems);

            //[] Call FindLast with default(string) at the beginning
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddFirst(default(string));

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_8548ajia Expected FindLast to return false with an non null item not in the collection default(string) at the beginning"
            VerifyFindLast(linkedList, prependDefaultHeadItems);

            //[] Call FindLast with default(string) in the middle
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(string));
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            Assert.Null(linkedList.FindLast(CreateT(seed++))); //"Err_78585ajhed Expected FindLast to return false with an non null item not in the collection default(string) in the middle"

            tempItems = new string[headItems.Length + prependDefaultTailItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Copy(prependDefaultTailItems, 0, tempItems, headItems.Length, prependDefaultTailItems.Length);

            VerifyFindLast(linkedList, tempItems);

            //[] Call FindLast on a collection with duplicate items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(string));

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_208089ajdi Expected FindLast to return false with an non null item not in the collection default(string) at the end"
            string[] temp = new string[headItems.Length + 1];
            temp[headItems.Length] = default(string);
            Array.Copy(headItems, temp, headItems.Length);
            VerifyFindLast(linkedList, temp);
        }

        [Test]
        public void Verify()
        {
            LinkedListNode<string> node;
            int seed = 21543;
            string value;

            //[] Verify passing default(string) into the constructor
            node = new LinkedListNode<string>(default(string));
            VerifyLinkedListNode(node, default(string), null, null, null);

            //[] Verify passing something other then default(string) into the constructor
            value = CreateT(seed++);
            node = new LinkedListNode<string>(value);
            VerifyLinkedListNode(node, value, null, null, null);

            //[] Verify passing something other then default(string) into the constructor and set the value to something other then default(string)
            value = CreateT(seed++);
            node = new LinkedListNode<string>(value);
            value = CreateT(seed++);
            node.Value = value;

            VerifyLinkedListNode(node, value, null, null, null);

            //[] Verify passing something other then default(string) into the constructor and set the value to default(string)
            value = CreateT(seed++);
            node = new LinkedListNode<string>(value);
            node.Value = default(string);

            VerifyLinkedListNode(node, default(string), null, null, null);

            //[] Verify passing default(string) into the constructor and set the value to something other then default(string)
            node = new LinkedListNode<string>(default(string));
            value = CreateT(seed++);
            node.Value = value;

            VerifyLinkedListNode(node, value, null, null, null);

            //[] Verify passing default(string) into the constructor and set the value to default(string)
            node = new LinkedListNode<string>(default(string));
            value = CreateT(seed++);
            node.Value = default(string);

            VerifyLinkedListNode(node, default(string), null, null, null);
        }


        [Test]
        public void RemoveFirst_Tests()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] headItems, tailItems;
            LinkedListNode<string> tempNode1, tempNode2, tempNode3;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call RemoveHead on a collection with one item in it
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            tempNode1 = linkedList.First;

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new string[0]);
            VerifyRemovedNode(tempNode1, headItems[0]);

            //[] Call RemoveHead on a collection with two items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.Last;

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new string[] { headItems[1] });

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new string[0]);

            VerifyRemovedNode(linkedList, tempNode1, headItems[0]);
            VerifyRemovedNode(linkedList, tempNode2, headItems[1]);

            //[] Call RemoveHead on a collection with three items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.First.Next;
            tempNode3 = linkedList.Last;

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new string[] { headItems[1], headItems[2] });

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new string[] { headItems[2] });

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new string[0]);

            VerifyRemovedNode(tempNode1, headItems[0]);
            VerifyRemovedNode(tempNode2, headItems[1]);
            VerifyRemovedNode(tempNode3, headItems[2]);

            //[] Call RemoveHead on a collection with 16 items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
                int startIndex = i + 1;
                int length = arraySize - i - 1;
                string[] expectedItems = new string[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Mix RemoveHead and RemoveTail call
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                if ((i & 1) == 0)
                {
                    linkedList.RemoveFirst();
                }
                else
                {
                    linkedList.RemoveLast();
                }

                int startIndex = (i / 2) + 1;
                int length = arraySize - i - 1;
                string[] expectedItems = new string[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }
        }

        [Test]
        public void RemoveFirst_Tests_Negative()
        {
            //[] Call RemoveHead an empty collection
            LinkedList<string> linkedList = new LinkedList<string>();
            Assert.Throws<InvalidOperationException>(() => linkedList.RemoveFirst()); //"Expected invalidoperation exception removing from empty list."
            InitialItems_Tests(linkedList, new string[0]);
        }

        [Test]
        public void RemoveLast_Tests()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] headItems, tailItems;
            LinkedListNode<string> tempNode1, tempNode2, tempNode3;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call RemoveHead on a collection with one item in it
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            tempNode1 = linkedList.Last;

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new string[0]);
            VerifyRemovedNode(tempNode1, headItems[0]);

            //[] Call RemoveHead on a collection with two items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.Last;
            tempNode2 = linkedList.First;

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new string[] { headItems[0] });

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new string[0]);

            VerifyRemovedNode(linkedList, tempNode1, headItems[1]);
            VerifyRemovedNode(linkedList, tempNode2, headItems[0]);

            //[] Call RemoveHead on a collection with three items in it
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.Last;
            tempNode2 = linkedList.Last.Previous;
            tempNode3 = linkedList.First;

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new string[] { headItems[0], headItems[1] });

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new string[] { headItems[0] });

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new string[0]);
            VerifyRemovedNode(tempNode1, headItems[2]);
            VerifyRemovedNode(tempNode2, headItems[1]);
            VerifyRemovedNode(tempNode3, headItems[0]);

            //[] Call RemoveHead on a collection with 16 items in it
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveLast();
                int length = arraySize - i - 1;
                string[] expectedItems = new string[length];
                Array.Copy(headItems, 0, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Mix RemoveHead and RemoveTail call
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                if ((i & 1) == 0)
                {
                    linkedList.RemoveFirst();
                }
                else
                {
                    linkedList.RemoveLast();
                }
                int startIndex = (i / 2) + 1;
                int length = arraySize - i - 1;
                string[] expectedItems = new string[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }
        }

        [Test]
        public void RemoveLast_Tests_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            Assert.Throws<InvalidOperationException>(() => linkedList.RemoveLast()); //"Expected invalidoperation exception removing from empty list."
            InitialItems_Tests(linkedList, new string[0]);
        }

        [Test]
        public void Remove_LLNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] headItems, tailItems, tempItems;
            LinkedListNode<string> tempNode1, tempNode2, tempNode3;

            headItems = new string[arraySize];
            tailItems = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call Remove with an item that exists in the collection size=1
            linkedList = new LinkedList<string>();
            linkedList.AddLast(headItems[0]);
            tempNode1 = linkedList.First;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved

            VerifyRemovedNode(linkedList, new string[0], tempNode1, headItems[0]);
            InitialItems_Tests(linkedList, new string[0]);

            //[] Call Remove with the Head collection size=2
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.First;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new string[] { headItems[1] });
            VerifyRemovedNode(tempNode1, headItems[0]);

            //[] Call Remove with the Tail collection size=2
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.Last;

            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new string[] { headItems[0] });
            VerifyRemovedNode(tempNode1, headItems[1]);

            //[] Call Remove all the items collection size=2
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.Last;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new string[0]);
            VerifyRemovedNode(linkedList, new string[0], tempNode1, headItems[0]);
            VerifyRemovedNode(linkedList, new string[0], tempNode2, headItems[1]);

            //[] Call Remove with the Head collection size=3
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            InitialItems_Tests(linkedList, new string[] { headItems[1], headItems[2] });
            VerifyRemovedNode(linkedList, new string[] { headItems[1], headItems[2] }, tempNode1, headItems[0]);

            //[] Call Remove with the middle item collection size=3
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First.Next;

            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            InitialItems_Tests(linkedList, new string[] { headItems[0], headItems[2] });
            VerifyRemovedNode(linkedList, new string[] { headItems[0], headItems[2] }, tempNode1, headItems[1]);

            //[] Call Remove with the Tail collection size=3
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.Last;

            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved
            InitialItems_Tests(linkedList, new string[] { headItems[0], headItems[1] });
            VerifyRemovedNode(linkedList, new string[] { headItems[0], headItems[1] }, tempNode1, headItems[2]);

            //[] Call Remove all the items collection size=3
            linkedList = new LinkedList<string>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.First.Next;
            tempNode3 = linkedList.Last;

            linkedList.Remove(linkedList.First.Next.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new string[0]);
            VerifyRemovedNode(tempNode1, headItems[0]);
            VerifyRemovedNode(tempNode2, headItems[1]);
            VerifyRemovedNode(tempNode3, headItems[2]);

            //[] Call Remove all the items starting with the first collection size=16
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
                int startIndex = i + 1;
                int length = arraySize - i - 1;
                string[] expectedItems = new string[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Call Remove all the items starting with the last collection size=16
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = arraySize - 1; 0 <= i; --i)
            {
                linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved
                string[] expectedItems = new string[i];
                Array.Copy(headItems, 0, expectedItems, 0, i);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Remove some items in the middle
            linkedList = new LinkedList<string>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            linkedList.Remove(linkedList.First.Next.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous.Previous); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous);
            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            //With the above remove we should have removed the first and last 3 items
            string[] headItemsReverse = new string[arraySize];
            Array.Copy(headItems, headItemsReverse, headItems.Length);
            Array.Reverse(headItemsReverse);

            tempItems = new string[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            //[] Remove an item with a value of default(string)
            linkedList = new LinkedList<string>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            linkedList.AddLast(default(string));

            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, headItems);
        }

        [Test]
        public void Remove_Duplicates_LLNode()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            int arraySize = 16;
            int seed = 21543;
            string[] items;
            LinkedListNode<string>[] nodes = new LinkedListNode<string>[arraySize * 2];
            LinkedListNode<string> currentNode;
            int index;

            items = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                items[i] = CreateT(seed++);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(items[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(items[i]);
            }

            currentNode = linkedList.First;
            index = 0;

            while (currentNode != null)
            {
                nodes[index] = currentNode;
                currentNode = currentNode.Next;
                ++index;
            }

            linkedList.Remove(linkedList.First.Next.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous.Previous); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous);
            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            //[] Verify that the duplicates were removed from the beginning of the collection
            currentNode = linkedList.First;

            //Verify the duplicates that should have been removed
            for (int i = 3; i < nodes.Length - 3; ++i)
            {
                Assert.NotNull(currentNode); //"Err_48588ahid CurrentNode is null index=" + i
                Assert.AreEqual(currentNode.Value, nodes[i].Value); //"Err_5488ahid CurrentNode is not the expected node index=" + i
                Assert.AreEqual(items[i % items.Length], currentNode.Value); //"Err_16588ajide CurrentNode value index=" + i

                currentNode = currentNode.Next;
            }

            Assert.Null(currentNode); //"Err_30878ajid Expected CurrentNode to be null after moving through entire list"
        }

        [Test]
        public void Remove_LLNode_Negative()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            LinkedList<string> tempLinkedList = new LinkedList<string>();
            int seed = 21543;
            string[] items;

            //[] Verify Null node
            linkedList = new LinkedList<string>();
            Assert.Throws<ArgumentNullException>(() => linkedList.Remove((LinkedListNode<string>)null)); //"Err_858ahia Expected null node to throws ArgumentNullException\n"

            InitialItems_Tests(linkedList, new string[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.Remove(new LinkedListNode<string>(CreateT(seed++)))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<string>();
            items = new string[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.Remove(tempLinkedList.Last)); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
#if false
            return Convert.ToBase64String(bytes);
#endif
            return rand.Next().ToString();
        }

        protected override ISet<string> GenericISetFactory()
        {
            throw new NotImplementedException();
        }
    }

    [Category(Constants.MODULE_ICOLLECTION)]
    [TestFixture(TestNameFormat = "LinkedList_Generic_Tests_int - {0}")]
    public class LinkedList_Generic_Tests_int : LinkedList_Generic_Tests<int>
    {
#region Constructor_IEnumerable

        [Test]
        public void LinkedList_Generic_Constructor_IEnumerable()
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
                LinkedList<int> queue = new LinkedList<int>(enumerable);
                Assert.AreDeepEqual(enumerable.ToArray(), queue.ToArray());
            }


        }

        [Test]
        public void LinkedList_Generic_Constructor_IEnumerable_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkedList<int>(null));
        }

#endregion

        [Test]
        public void AddAfter_LLNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 8293;
            int[] tempItems, headItems, headItemsReverse, tailItems;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddAfter(linkedList.First, default(int));
            InitialItems_Tests(linkedList, new int[] { headItems[0], default(int) });

            //[] Node is the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Node is after the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 2, headItems.Length - 2);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First.Next, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 1, headItems.Length - 2);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 1, headItems.Length - 3);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Call AddAfter several times remove some of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[headItems.Length - 6];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            int[] tempItems2 = new int[tempItems.Length + tailItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(tailItems, 0, tempItems2, tempItems.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddAfter several times remove all of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddAfter several times then call Clear
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, headItems[i]);
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddAfter_LLNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 8293;
            int[] items;

            //[] Verify Null node
            linkedList = new LinkedList<int>();
            Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(null, CreateT(seed++))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"

            InitialItems_Tests(linkedList, new int[0]);
            //[] Verify Node that is a new Node
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(new LinkedListNode<int>(CreateT(seed++)), CreateT(seed++))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(tempLinkedList.Last, CreateT(seed++))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddAfter_LLNode_LLNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int seed = 8293;
            int arraySize = 16;
            int[] tempItems, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            tailItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddAfter(linkedList.First, new LinkedListNode<int>(default(int)));
            InitialItems_Tests(linkedList, new int[] { headItems[0], default(int) });

            //[] Node is the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Node is after the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 2, headItems.Length - 2);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.First.Next, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 1, headItems.Length - 2);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 1, headItems.Length - 3);
            tempItems[0] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last.Previous.Previous, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);


            //[] Call AddAfter several times remove some of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(headItems[i]));
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[headItems.Length - 6];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(tailItems[i]));
            }

            int[] tempItems2 = new int[tempItems.Length + tailItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(tailItems, 0, tempItems2, tempItems.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddAfter several times remove all of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(headItems[i]));
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddAfter several times then call Clear
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(headItems[i]));
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                if (0 == (i & 1))
                {
                    linkedList.AddBefore(linkedList.First, headItems[i]);
                    linkedList.AddAfter(linkedList.Last, tailItems[i]);
                }
                else
                {
                    linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(headItems[i]));
                    linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(tailItems[i]));
                }
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddAfter_LLNode_LLNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 8293;
            int[] items;

            //[] Verify Null node
            linkedList = new LinkedList<int>();
            Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(null, new LinkedListNode<int>(CreateT(seed++)))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new int[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(new LinkedListNode<int>(CreateT(seed++)), new LinkedListNode<int>(CreateT(seed++)))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(tempLinkedList.Last, new LinkedListNode<int>(CreateT(seed++)))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            // negative tests on NewNode

            //[] Verify Null newNode
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<ArgumentNullException>(() => linkedList.AddAfter(linkedList.First, null)); //"Err_0808ajeoia Expected null newNode to throws ArgumentNullException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in this collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(linkedList.First, linkedList.Last)); //"Err_58808adjioe Verify newNode that already exists in this collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddAfter(linkedList.First, tempLinkedList.Last)); //"Err_54808ajied newNode that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddBefore_LLNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 8293;
            int[] tempItems, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            tailItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddBefore(linkedList.First, default(int));
            InitialItems_Tests(linkedList, new int[] { default(int), headItems[0] });

            //[] Node is the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Node is the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 1, tempItems, 0, headItems.Length - 1);
            tempItems[tempItems.Length - 1] = headItems[0];
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is after the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First.Next, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 0, headItems.Length - 2);
            tempItems[tempItems.Length - 2] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 3);
            tempItems[tempItems.Length - 3] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous.Previous, headItems[i]);
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Call AddBefore several times remove some of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();

            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, tailItems[i]);
            }

            int[] tempItems2 = new int[tailItemsReverse.Length + tempItems.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddBefore several times remove all of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Call AddBefore several times then call Clear
            linkedList.Clear();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, headItems[i]);
                linkedList.AddAfter(linkedList.Last, tailItems[i]);
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddBefore_LLNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 8293;
            int[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddBefore(null, CreateT(seed++))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new int[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(new LinkedListNode<int>(CreateT(seed++)), CreateT(seed++))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(tempLinkedList.Last, CreateT(seed++))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddBefore_LLNode_LLNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 8293;
            int[] tempItems, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            tailItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(default(int)));
            InitialItems_Tests(linkedList, new int[] { default(int), headItems[0] });

            //[] Node is the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Node is the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 1, tempItems, 0, headItems.Length - 1);
            tempItems[tempItems.Length - 1] = headItems[0];
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is after the Head
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Reverse(tempItems, 1, headItems.Length - 1);

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First.Next, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is before the Tail
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 2, tempItems, 0, headItems.Length - 2);
            tempItems[tempItems.Length - 2] = headItems[0];
            tempItems[tempItems.Length - 1] = headItems[1];

            for (int i = 2; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Node is somewhere in the middle
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);

            tempItems = new int[headItems.Length];
            Array.Copy(headItems, 3, tempItems, 0, headItems.Length - 3);
            tempItems[tempItems.Length - 3] = headItems[0];
            tempItems[tempItems.Length - 2] = headItems[1];
            tempItems[tempItems.Length - 1] = headItems[2];

            for (int i = 3; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.Last.Previous.Previous, new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, tempItems);

            //[] Call AddBefore several times remove some of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(headItems[i]));
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(tailItems[i]));
            }

            int[] tempItems2 = new int[tailItemsReverse.Length + tempItems.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddBefore several times remove all of the items
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(headItems[i]));
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Call AddBefore several times then call Clear
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(headItems[i]));
            }

            linkedList.Clear();

            linkedList.AddFirst(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddBefore and AddAfter calls
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            linkedList.AddLast(tailItems[0]);
            for (int i = 1; i < arraySize; ++i)
            {
                linkedList.AddBefore(linkedList.First, new LinkedListNode<int>(headItems[i]));
                linkedList.AddAfter(linkedList.Last, new LinkedListNode<int>(tailItems[i]));
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddBefore_LLNode_LLNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 8293;
            int[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddBefore(null, new LinkedListNode<int>(CreateT(seed++)))); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new int[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(new LinkedListNode<int>(CreateT(seed++)), new LinkedListNode<int>(CreateT(seed++)))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(tempLinkedList.Last, new LinkedListNode<int>(CreateT(seed++)))); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            // Tests for the NewNode

            //[] Verify Null newNode
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<ArgumentNullException>(() => linkedList.AddBefore(linkedList.First, null)); //"Err_0808ajeoia Expected null newNode to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in this collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(linkedList.First, linkedList.Last)); //"Err_58808adjioe Verify newNode that already exists in this collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify newNode that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddBefore(linkedList.First, tempLinkedList.Last)); //"Err_54808ajied newNode that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddFirst_T_Tests()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int seed = 21543;
            int arraySize = 16;
            int[] tempItems, tempItems2, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            tailItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(default(int));
            InitialItems_Tests(linkedList, new int[] { default(int) });

            //[] Call AddHead(int) several times
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Call AddHead(int) several times remove some of the items
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            // expected items are headItems in reverse order, or a subset of them.
            tempItems = new int[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);
            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(tailItems[i]);
            }

            tempItems2 = new int[tempItems.Length + tailItemsReverse.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);
            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddHead(int) several times remove all of the items
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Call AddHead(int) several times then call Clear
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            linkedList.Clear();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddHead and AddTail calls
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
                linkedList.AddLast(tailItems[i]);
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);

            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddFirst_LinkedListNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] tempItems, tempItems2, headItems, headItemsReverse, tailItems, tailItemsReverse;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            tailItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
                tailItemsReverse[index] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(new LinkedListNode<int>(default(int)));
            InitialItems_Tests(linkedList, new int[] { default(int) });

            //[] Call AddHead(LinkedListNode<int>) several times
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItemsReverse);

            //[] Call AddHead(LinkedListNode<int>) several times remove some of the items
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(headItems[i]));
            }

            linkedList.Remove(headItems[2]);
            linkedList.Remove(headItems[headItems.Length - 3]);
            linkedList.Remove(headItems[1]);
            linkedList.Remove(headItems[headItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);
            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(tailItems[i]));
            }

            tempItems2 = new int[tailItemsReverse.Length + tempItems.Length];
            Array.Copy(tailItemsReverse, 0, tempItems2, 0, tailItemsReverse.Length);
            Array.Copy(tempItems, 0, tempItems2, tailItemsReverse.Length, tempItems.Length);
            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddHead(LinkedListNode<int>) several times remove all of the items
            linkedList = new LinkedList<int>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(headItems[i]));
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItemsReverse);

            //[] Mix AddHead and AddTail calls
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(headItems[i]));
                linkedList.AddLast(new LinkedListNode<int>(tailItems[i]));
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddFirst_LinkedListNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 21543;
            int[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddFirst(null)); //"Err_858ahia Expected null node to throws ArgumentNullException\n"
            InitialItems_Tests(linkedList, new int[0]);

            //[] Verify Node that already exists in this collection that is the Head
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddFirst(linkedList.First)); //"Err_0568ajods Expected Node that already exists in this collection that is the Head throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in this collection that is the Tail
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddFirst(linkedList.Last)); //"Err_98809ahied Expected Node that already exists in this collection that is the Tail throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddFirst(tempLinkedList.Last)); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public void AddLast_T_Tests()
        {
            int arraySize = 16;
            int seed = 21543;
            int[] tempItems, headItems, tailItems;

            headItems = new int[16];
            tailItems = new int[16];
            for (int i = 0; i < 16; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Verify value is default(int)
            LinkedList<int> linkedList = new LinkedList<int>();
            linkedList.AddLast(default(int));
            InitialItems_Tests(linkedList, new int[] { default(int) });

            //[] Call AddTail(int) several times
            linkedList.Clear();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddTail(int) several times remove some of the items
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            linkedList.Remove(tailItems[2]);
            linkedList.Remove(tailItems[tailItems.Length - 3]);
            linkedList.Remove(tailItems[1]);
            linkedList.Remove(tailItems[tailItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();
            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[tailItems.Length - 6];
            Array.Copy(tailItems, 3, tempItems, 0, tailItems.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            //[] adding some more items to the tail of the linked list.
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            int[] tempItems2 = new int[tempItems.Length + headItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(headItems, 0, tempItems2, tempItems.Length, headItems.Length);
            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddTail(int) several times then call Clear
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            linkedList.Clear();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Mix AddTail and AddTail calls
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
                linkedList.AddLast(tailItems[i]);
            }

            tempItems = new int[headItems.Length];
            // adding the headItems in reverse order.
            for (int i = 0; i < headItems.Length; i++)
            {
                int index = (headItems.Length - 1) - i;
                tempItems[i] = headItems[index];
            }

            tempItems2 = new int[tempItems.Length + tailItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(tailItems, 0, tempItems2, tempItems.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems2);
        }

        [Test]
        public void AddLast_LinkedListNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] tempItems, headItems, headItemsReverse, tailItems;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            headItemsReverse = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                int index = (arraySize - 1) - i;
                int head = CreateT(seed++);
                int tail = CreateT(seed++);
                headItems[i] = head;
                headItemsReverse[index] = head;
                tailItems[i] = tail;
            }

            //[] Verify value is default(int)
            linkedList = new LinkedList<int>();
            linkedList.AddLast(new LinkedListNode<int>(default(int)));
            InitialItems_Tests(linkedList, new int[] { default(int) });

            //[] Call AddTail(LinkedListNode<int>) several times
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<int>(tailItems[i]));
            }

            InitialItems_Tests(linkedList, tailItems);

            //[] Call AddTail(LinkedListNode<int>) several times remove some of the items
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<int>(tailItems[i]));
            }

            linkedList.Remove(tailItems[2]);
            linkedList.Remove(tailItems[tailItems.Length - 3]);
            linkedList.Remove(tailItems[1]);
            linkedList.Remove(tailItems[tailItems.Length - 2]);
            linkedList.RemoveFirst();
            linkedList.RemoveLast();

            //With the above remove we should have removed the first and last 3 items
            tempItems = new int[tailItems.Length - 6];
            Array.Copy(tailItems, 3, tempItems, 0, tailItems.Length - 6);
            InitialItems_Tests(linkedList, tempItems);

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<int>(headItems[i]));
            }

            int[] tempItems2 = new int[tempItems.Length + headItems.Length];
            Array.Copy(tempItems, 0, tempItems2, 0, tempItems.Length);
            Array.Copy(headItems, 0, tempItems2, tempItems.Length, headItems.Length);

            InitialItems_Tests(linkedList, tempItems2);

            //[] Call AddTail(int) several times then call Clear
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<int>(tailItems[i]));
            }

            linkedList.Clear();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(new LinkedListNode<int>(headItems[i]));
            }

            InitialItems_Tests(linkedList, headItems);

            //[] Mix AddTail and AddTail calls
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(new LinkedListNode<int>(headItems[i]));
                linkedList.AddLast(new LinkedListNode<int>(tailItems[i]));
            }

            tempItems = new int[headItemsReverse.Length + tailItems.Length];
            Array.Copy(headItemsReverse, 0, tempItems, 0, headItemsReverse.Length);
            Array.Copy(tailItems, 0, tempItems, headItemsReverse.Length, tailItems.Length);
            InitialItems_Tests(linkedList, tempItems);
        }

        [Test]
        public void AddLast_LinkedListNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 21543;
            int[] items;

            //[] Verify Null node
            Assert.Throws<ArgumentNullException>(() => linkedList.AddLast(null)); //"Err_858ahia Expected null node to throws ArgumentNullException"
            InitialItems_Tests(linkedList, new int[0]);

            //[] Verify Node that already exists in this collection that is the Head
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddLast(linkedList.First)); //"Err_0568ajods Expected Node that already exists in this collection that is the Head throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in this collection that is the Tail
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);
            Assert.Throws<InvalidOperationException>(() => linkedList.AddLast(linkedList.Last)); //"Err_98809ahied Expected Node that already exists in this collection that is the Tail throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.AddLast(tempLinkedList.Last)); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"
            InitialItems_Tests(linkedList, items);
        }

        [Test]
        public static void CtorTest()
        {
            LinkedList_T_Tests<string> helper = new LinkedList_T_Tests<string>(new LinkedList<string>(), new string[0]);
            helper.InitialItems_Tests();
            LinkedList_T_Tests<LinkedList_Person> helper2 = new LinkedList_T_Tests<LinkedList_Person>(new LinkedList<LinkedList_Person>(), new LinkedList_Person[0]);
            helper2.InitialItems_Tests();
        }

        [Test]
        public static void Ctor_IEnumerableTest()
        {
            int arraySize = 16;
            int[] intArray = new int[arraySize];
            LinkedList_Person[] personArray = new LinkedList_Person[arraySize];

            int printableCharIndex = 65; // ascii "A"
            for (int i = 0; i < arraySize; ++i)
            {
                intArray[i] = i;
                char asChar = (char)(printableCharIndex + i);
                personArray[i] = new LinkedList_Person(asChar.ToString(), i);
            }

            // Testing with value types
            LinkedList<int> intList = new LinkedList<int>(Clone(intArray));
            LinkedList_T_Tests<int> helper = new LinkedList_T_Tests<int>(intList, intArray);
            helper.InitialItems_Tests();

            intArray = new int[0];
            intList = new LinkedList<int>(Clone(intArray));
            helper = new LinkedList_T_Tests<int>(intList, intArray);
            helper.InitialItems_Tests();

            intArray = new int[] { -3 };
            intList = new LinkedList<int>(Clone(intArray));
            helper = new LinkedList_T_Tests<int>(intList, intArray);
            helper.InitialItems_Tests();

            // Testing with an reference type
            LinkedList<LinkedList_Person> personList = new LinkedList<LinkedList_Person>(Clone(personArray));
            LinkedList_T_Tests<LinkedList_Person> helper2 = new LinkedList_T_Tests<LinkedList_Person>(personList, personArray);
            helper2.InitialItems_Tests();

            personArray = new LinkedList_Person[0];
            personList = new LinkedList<LinkedList_Person>(Clone(personArray));
            helper2 = new LinkedList_T_Tests<LinkedList_Person>(personList, personArray);
            helper2.InitialItems_Tests();

            personArray = new LinkedList_Person[] { new LinkedList_Person("Jack", 18) };
            personList = new LinkedList<LinkedList_Person>(Clone(personArray));
            helper2 = new LinkedList_T_Tests<LinkedList_Person>(personList, personArray);
            helper2.InitialItems_Tests();
        }

        [Test]
        public static void Ctor_IEnumerableTest_Negative()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkedList<string>(null)); //"Err_982092 Expected ArgumentNullException to be thrown with null collection"
        }

        private static IEnumerable<T2> Clone<T2>(T2[] source)
        {
            if (source == null)
            {
                return null;
            }
            return (T2[])source.Clone();
        }

        [Test]
        public void Find_T()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] headItems, tailItems;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call Find an empty collection
            linkedList = new LinkedList<int>();
            Assert.Null(linkedList.Find(headItems[0])); //"Err_2899hjaied Expected Find to return false with a non null item on an empty collection"
            Assert.Null(linkedList.Find(default(int))); //"Err_5808ajiea Expected Find to return false with a null item on an empty collection"

            //[] Call Find on a collection with one item in it
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            Assert.Null(linkedList.Find(headItems[1])); //"Err_2899hjaied Expected Find to return false with a non null item on an empty collection size=1"
            Assert.Null(linkedList.Find(default(int))); //"Err_5808ajiea Expected Find to return false with a null item on an empty collection size=1"
            VerifyFind(linkedList, new int[] { headItems[0] });

            //[] Call Find on a collection with two items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            Assert.Null(linkedList.Find(headItems[2])); //"Err_5680ajoed Expected Find to return false with an non null item not in the collection size=2"
            Assert.Null(linkedList.Find(default(int))); //"Err_858196aieh Expected Find to return false with an null item not in the collection size=2"
            VerifyFind(linkedList, new int[] { headItems[0], headItems[1] });

            //[] Call Find on a collection with three items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            Assert.Null(linkedList.Find(headItems[3])); //"Err_50878adie Expected Find to return false with an non null item not in the collection size=3"
            Assert.Null(linkedList.Find(default(int))); //"Err_3969887wiqpi Expected Find to return false with an null item not in the collection size=3"
            VerifyFind(linkedList, new int[] { headItems[0], headItems[1], headItems[2] });

            //[] Call Find on a collection with multiple items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_5808ajdoi Expected Find to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.Find(default(int))); //"Err_5588aied Expected Find to return false with an null item not in the collection size=16"
            VerifyFind(linkedList, headItems);

            //[] Call Find on a collection with duplicate items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_8548ajia Expected Find to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.Find(default(int))); //"Err_3108qoa Expected Find to return false with an null item not in the collection size=16"
            int[] tempItems = new int[headItems.Length + headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Copy(headItems, 0, tempItems, headItems.Length, headItems.Length);
            VerifyFindDuplicates(linkedList, tempItems);


            //[] Call Find with default(int) at the beginning
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddFirst(default(int));

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_8548ajia Expected Find to return false with an non null item not in the collection default(int) at the beginning"

            tempItems = new int[headItems.Length + 1];
            tempItems[0] = default(int);
            Array.Copy(headItems, 0, tempItems, 1, headItems.Length);

            VerifyFind(linkedList, tempItems);

            //[] Call Find with default(int) in the middle
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(int));
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            Assert.Null(linkedList.Find(CreateT(seed++))); //"Err_78585ajhed Expected Find to return false with an non null item not in the collection default(int) in the middle"

            // prepending tempitems2 to tailitems into tempitems
            tempItems = new int[tailItems.Length + 1];
            tempItems[0] = default(int);
            Array.Copy(tailItems, 0, tempItems, 1, tailItems.Length);

            int[] tempItems2 = new int[headItems.Length + tempItems.Length];
            Array.Copy(headItems, 0, tempItems2, 0, headItems.Length);
            Array.Copy(tempItems, 0, tempItems2, headItems.Length, tempItems.Length);

            VerifyFind(linkedList, tempItems2);

            //[] Call Find on a collection with duplicate items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(int));

            Assert.Null(linkedList.Find(tailItems[0])); //"Err_208089ajdi Expected Find to return false with an non null item not in the collection default(int) at the end"
            tempItems = new int[headItems.Length + 1];
            tempItems[headItems.Length] = default(int);
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            VerifyFind(linkedList, tempItems);
        }

        [Test]
        public void FindLast_T()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int seed = 21543;
            int arraySize = 16;
            int[] headItems, tailItems, prependDefaultHeadItems, prependDefaultTailItems;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }
            prependDefaultHeadItems = new int[headItems.Length + 1];
            prependDefaultHeadItems[0] = default(int);
            Array.Copy(headItems, 0, prependDefaultHeadItems, 1, headItems.Length);

            prependDefaultTailItems = new int[tailItems.Length + 1];
            prependDefaultTailItems[0] = default(int);
            Array.Copy(tailItems, 0, prependDefaultTailItems, 1, tailItems.Length);

            //[] Call FindLast an empty collection
            linkedList = new LinkedList<int>();
            Assert.Null(linkedList.FindLast(headItems[0])); //"Err_2899hjaied Expected FindLast to return false with a non null item on an empty collection"
            Assert.Null(linkedList.FindLast(default(int))); //"Err_5808ajiea Expected FindLast to return false with a null item on an empty collection"

            //[] Call FindLast on a collection with one item in it
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            Assert.Null(linkedList.FindLast(headItems[1])); //"Err_2899hjaied Expected FindLast to return false with a non null item on an empty collection size=1"
            Assert.Null(linkedList.FindLast(default(int))); //"Err_5808ajiea Expected FindLast to return false with a null item on an empty collection size=1"
            VerifyFindLast(linkedList, new int[] { headItems[0] });

            //[] Call FindLast on a collection with two items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            Assert.Null(linkedList.FindLast(headItems[2])); //"Err_5680ajoed Expected FindLast to return false with an non null item not in the collection size=2"
            Assert.Null(linkedList.FindLast(default(int))); //"Err_858196aieh Expected FindLast to return false with an null item not in the collection size=2"
            VerifyFindLast(linkedList, new int[] { headItems[0], headItems[1] });

            //[] Call FindLast on a collection with three items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            Assert.Null(linkedList.FindLast(headItems[3])); //"Err_50878adie Expected FindLast to return false with an non null item not in the collection size=3"
            Assert.Null(linkedList.FindLast(default(int))); //"Err_3969887wiqpi Expected FindLast to return false with an null item not in the collection size=3"
            VerifyFindLast(linkedList, new int[] { headItems[0], headItems[1], headItems[2] });

            //[] Call FindLast on a collection with multiple items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_5808ajdoi Expected FindLast to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.FindLast(default(int))); //"Err_5588aied Expected FindLast to return false with an null item not in the collection size=16"
            VerifyFindLast(linkedList, headItems);

            //[] Call FindLast on a collection with duplicate items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_8548ajia Expected FindLast to return false with an non null item not in the collection size=16"
            Assert.Null(linkedList.FindLast(default(int))); //"Err_3108qoa Expected FindLast to return false with an null item not in the collection size=16"
            int[] tempItems = new int[headItems.Length + headItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Copy(headItems, 0, tempItems, headItems.Length, headItems.Length);
            VerifyFindLastDuplicates(linkedList, tempItems);

            //[] Call FindLast with default(int) at the beginning
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddFirst(default(int));

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_8548ajia Expected FindLast to return false with an non null item not in the collection default(int) at the beginning"
            VerifyFindLast(linkedList, prependDefaultHeadItems);

            //[] Call FindLast with default(int) in the middle
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(int));
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(tailItems[i]);
            }

            Assert.Null(linkedList.FindLast(CreateT(seed++))); //"Err_78585ajhed Expected FindLast to return false with an non null item not in the collection default(int) in the middle"

            tempItems = new int[headItems.Length + prependDefaultTailItems.Length];
            Array.Copy(headItems, 0, tempItems, 0, headItems.Length);
            Array.Copy(prependDefaultTailItems, 0, tempItems, headItems.Length, prependDefaultTailItems.Length);

            VerifyFindLast(linkedList, tempItems);

            //[] Call FindLast on a collection with duplicate items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < headItems.Length; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }
            linkedList.AddLast(default(int));

            Assert.Null(linkedList.FindLast(tailItems[0])); //"Err_208089ajdi Expected FindLast to return false with an non null item not in the collection default(int) at the end"
            int[] temp = new int[headItems.Length + 1];
            temp[headItems.Length] = default(int);
            Array.Copy(headItems, temp, headItems.Length);
            VerifyFindLast(linkedList, temp);
        }

        [Test]
        public void Verify()
        {
            LinkedListNode<int> node;
            int seed = 21543;
            int value;

            //[] Verify passing default(int) into the constructor
            node = new LinkedListNode<int>(default(int));
            VerifyLinkedListNode(node, default(int), null, null, null);

            //[] Verify passing something other then default(int) into the constructor
            value = CreateT(seed++);
            node = new LinkedListNode<int>(value);
            VerifyLinkedListNode(node, value, null, null, null);

            //[] Verify passing something other then default(int) into the constructor and set the value to something other then default(int)
            value = CreateT(seed++);
            node = new LinkedListNode<int>(value);
            value = CreateT(seed++);
            node.Value = value;

            VerifyLinkedListNode(node, value, null, null, null);

            //[] Verify passing something other then default(int) into the constructor and set the value to default(int)
            value = CreateT(seed++);
            node = new LinkedListNode<int>(value);
            node.Value = default(int);

            VerifyLinkedListNode(node, default(int), null, null, null);

            //[] Verify passing default(int) into the constructor and set the value to something other then default(int)
            node = new LinkedListNode<int>(default(int));
            value = CreateT(seed++);
            node.Value = value;

            VerifyLinkedListNode(node, value, null, null, null);

            //[] Verify passing default(int) into the constructor and set the value to default(int)
            node = new LinkedListNode<int>(default(int));
            value = CreateT(seed++);
            node.Value = default(int);

            VerifyLinkedListNode(node, default(int), null, null, null);
        }


        [Test]
        public void RemoveFirst_Tests()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] headItems, tailItems;
            LinkedListNode<int> tempNode1, tempNode2, tempNode3;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call RemoveHead on a collection with one item in it
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            tempNode1 = linkedList.First;

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new int[0]);
            VerifyRemovedNode(tempNode1, headItems[0]);

            //[] Call RemoveHead on a collection with two items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.Last;

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new int[] { headItems[1] });

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new int[0]);

            VerifyRemovedNode(linkedList, tempNode1, headItems[0]);
            VerifyRemovedNode(linkedList, tempNode2, headItems[1]);

            //[] Call RemoveHead on a collection with three items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.First.Next;
            tempNode3 = linkedList.Last;

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new int[] { headItems[1], headItems[2] });

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new int[] { headItems[2] });

            linkedList.RemoveFirst();
            InitialItems_Tests(linkedList, new int[0]);

            VerifyRemovedNode(tempNode1, headItems[0]);
            VerifyRemovedNode(tempNode2, headItems[1]);
            VerifyRemovedNode(tempNode3, headItems[2]);

            //[] Call RemoveHead on a collection with 16 items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveFirst();
                int startIndex = i + 1;
                int length = arraySize - i - 1;
                int[] expectedItems = new int[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Mix RemoveHead and RemoveTail call
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                if ((i & 1) == 0)
                {
                    linkedList.RemoveFirst();
                }
                else
                {
                    linkedList.RemoveLast();
                }

                int startIndex = (i / 2) + 1;
                int length = arraySize - i - 1;
                int[] expectedItems = new int[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }
        }

        [Test]
        public void RemoveFirst_Tests_Negative()
        {
            //[] Call RemoveHead an empty collection
            LinkedList<int> linkedList = new LinkedList<int>();
            Assert.Throws<InvalidOperationException>(() => linkedList.RemoveFirst()); //"Expected invalidoperation exception removing from empty list."
            InitialItems_Tests(linkedList, new int[0]);
        }

        [Test]
        public void RemoveLast_Tests()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] headItems, tailItems;
            LinkedListNode<int> tempNode1, tempNode2, tempNode3;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call RemoveHead on a collection with one item in it
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            tempNode1 = linkedList.Last;

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new int[0]);
            VerifyRemovedNode(tempNode1, headItems[0]);

            //[] Call RemoveHead on a collection with two items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.Last;
            tempNode2 = linkedList.First;

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new int[] { headItems[0] });

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new int[0]);

            VerifyRemovedNode(linkedList, tempNode1, headItems[1]);
            VerifyRemovedNode(linkedList, tempNode2, headItems[0]);

            //[] Call RemoveHead on a collection with three items in it
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.Last;
            tempNode2 = linkedList.Last.Previous;
            tempNode3 = linkedList.First;

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new int[] { headItems[0], headItems[1] });

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new int[] { headItems[0] });

            linkedList.RemoveLast();
            InitialItems_Tests(linkedList, new int[0]);
            VerifyRemovedNode(tempNode1, headItems[2]);
            VerifyRemovedNode(tempNode2, headItems[1]);
            VerifyRemovedNode(tempNode3, headItems[0]);

            //[] Call RemoveHead on a collection with 16 items in it
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.RemoveLast();
                int length = arraySize - i - 1;
                int[] expectedItems = new int[length];
                Array.Copy(headItems, 0, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Mix RemoveHead and RemoveTail call
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                if ((i & 1) == 0)
                {
                    linkedList.RemoveFirst();
                }
                else
                {
                    linkedList.RemoveLast();
                }
                int startIndex = (i / 2) + 1;
                int length = arraySize - i - 1;
                int[] expectedItems = new int[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }
        }

        [Test]
        public void RemoveLast_Tests_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            Assert.Throws<InvalidOperationException>(() => linkedList.RemoveLast()); //"Expected invalidoperation exception removing from empty list."
            InitialItems_Tests(linkedList, new int[0]);
        }

        [Test]
        public void Remove_LLNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] headItems, tailItems, tempItems;
            LinkedListNode<int> tempNode1, tempNode2, tempNode3;

            headItems = new int[arraySize];
            tailItems = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                headItems[i] = CreateT(seed++);
                tailItems[i] = CreateT(seed++);
            }

            //[] Call Remove with an item that exists in the collection size=1
            linkedList = new LinkedList<int>();
            linkedList.AddLast(headItems[0]);
            tempNode1 = linkedList.First;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved

            VerifyRemovedNode(linkedList, new int[0], tempNode1, headItems[0]);
            InitialItems_Tests(linkedList, new int[0]);

            //[] Call Remove with the Head collection size=2
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.First;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new int[] { headItems[1] });
            VerifyRemovedNode(tempNode1, headItems[0]);

            //[] Call Remove with the Tail collection size=2
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.Last;

            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new int[] { headItems[0] });
            VerifyRemovedNode(tempNode1, headItems[1]);

            //[] Call Remove all the items collection size=2
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.Last;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new int[0]);
            VerifyRemovedNode(linkedList, new int[0], tempNode1, headItems[0]);
            VerifyRemovedNode(linkedList, new int[0], tempNode2, headItems[1]);

            //[] Call Remove with the Head collection size=3
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First;

            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            InitialItems_Tests(linkedList, new int[] { headItems[1], headItems[2] });
            VerifyRemovedNode(linkedList, new int[] { headItems[1], headItems[2] }, tempNode1, headItems[0]);

            //[] Call Remove with the middle item collection size=3
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First.Next;

            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            InitialItems_Tests(linkedList, new int[] { headItems[0], headItems[2] });
            VerifyRemovedNode(linkedList, new int[] { headItems[0], headItems[2] }, tempNode1, headItems[1]);

            //[] Call Remove with the Tail collection size=3
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.Last;

            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved
            InitialItems_Tests(linkedList, new int[] { headItems[0], headItems[1] });
            VerifyRemovedNode(linkedList, new int[] { headItems[0], headItems[1] }, tempNode1, headItems[2]);

            //[] Call Remove all the items collection size=3
            linkedList = new LinkedList<int>();
            linkedList.AddFirst(headItems[0]);
            linkedList.AddLast(headItems[1]);
            linkedList.AddLast(headItems[2]);
            tempNode1 = linkedList.First;
            tempNode2 = linkedList.First.Next;
            tempNode3 = linkedList.Last;

            linkedList.Remove(linkedList.First.Next.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, new int[0]);
            VerifyRemovedNode(tempNode1, headItems[0]);
            VerifyRemovedNode(tempNode2, headItems[1]);
            VerifyRemovedNode(tempNode3, headItems[2]);

            //[] Call Remove all the items starting with the first collection size=16
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
                int startIndex = i + 1;
                int length = arraySize - i - 1;
                int[] expectedItems = new int[length];
                Array.Copy(headItems, startIndex, expectedItems, 0, length);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Call Remove all the items starting with the last collection size=16
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            for (int i = arraySize - 1; 0 <= i; --i)
            {
                linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved
                int[] expectedItems = new int[i];
                Array.Copy(headItems, 0, expectedItems, 0, i);
                InitialItems_Tests(linkedList, expectedItems);
            }

            //[] Remove some items in the middle
            linkedList = new LinkedList<int>();
            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddFirst(headItems[i]);
            }

            linkedList.Remove(linkedList.First.Next.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous.Previous); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous);
            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            //With the above remove we should have removed the first and last 3 items
            int[] headItemsReverse = new int[arraySize];
            Array.Copy(headItems, headItemsReverse, headItems.Length);
            Array.Reverse(headItemsReverse);

            tempItems = new int[headItemsReverse.Length - 6];
            Array.Copy(headItemsReverse, 3, tempItems, 0, headItemsReverse.Length - 6);

            InitialItems_Tests(linkedList, tempItems);

            //[] Remove an item with a value of default(int)
            linkedList = new LinkedList<int>();

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(headItems[i]);
            }

            linkedList.AddLast(default(int));

            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            InitialItems_Tests(linkedList, headItems);
        }

        [Test]
        public void Remove_Duplicates_LLNode()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            int arraySize = 16;
            int seed = 21543;
            int[] items;
            LinkedListNode<int>[] nodes = new LinkedListNode<int>[arraySize * 2];
            LinkedListNode<int> currentNode;
            int index;

            items = new int[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                items[i] = CreateT(seed++);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(items[i]);
            }

            for (int i = 0; i < arraySize; ++i)
            {
                linkedList.AddLast(items[i]);
            }

            currentNode = linkedList.First;
            index = 0;

            while (currentNode != null)
            {
                nodes[index] = currentNode;
                currentNode = currentNode.Next;
                ++index;
            }

            linkedList.Remove(linkedList.First.Next.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous.Previous); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.First.Next); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last.Previous);
            linkedList.Remove(linkedList.First); //Remove when  VS Whidbey: 234648 is resolved
            linkedList.Remove(linkedList.Last); //Remove when  VS Whidbey: 234648 is resolved

            //[] Verify that the duplicates were removed from the beginning of the collection
            currentNode = linkedList.First;

            //Verify the duplicates that should have been removed
            for (int i = 3; i < nodes.Length - 3; ++i)
            {
                Assert.NotNull(currentNode); //"Err_48588ahid CurrentNode is null index=" + i
                Assert.AreEqual(currentNode.Value, nodes[i].Value); //"Err_5488ahid CurrentNode is not the expected node index=" + i
                Assert.AreEqual(items[i % items.Length], currentNode.Value); //"Err_16588ajide CurrentNode value index=" + i

                currentNode = currentNode.Next;
            }

            Assert.Null(currentNode); //"Err_30878ajid Expected CurrentNode to be null after moving through entire list"
        }

        [Test]
        public void Remove_LLNode_Negative()
        {
            LinkedList<int> linkedList = new LinkedList<int>();
            LinkedList<int> tempLinkedList = new LinkedList<int>();
            int seed = 21543;
            int[] items;

            //[] Verify Null node
            linkedList = new LinkedList<int>();
            Assert.Throws<ArgumentNullException>(() => linkedList.Remove(null)); //"Err_858ahia Expected null node to throws ArgumentNullException\n"

            InitialItems_Tests(linkedList, new int[0]);

            //[] Verify Node that is a new Node
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++) };
            linkedList.AddLast(items[0]);
            Assert.Throws<InvalidOperationException>(() => linkedList.Remove(new LinkedListNode<int>(CreateT(seed++)))); //"Err_0568ajods Expected Node that is a new Node throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);

            //[] Verify Node that already exists in another collection
            linkedList = new LinkedList<int>();
            items = new int[] { CreateT(seed++), CreateT(seed++) };
            linkedList.AddLast(items[0]);
            linkedList.AddLast(items[1]);

            tempLinkedList.Clear();
            tempLinkedList.AddLast(CreateT(seed++));
            tempLinkedList.AddLast(CreateT(seed++));
            Assert.Throws<InvalidOperationException>(() => linkedList.Remove(tempLinkedList.Last)); //"Err_98809ahied Node that already exists in another collection throws InvalidOperationException\n"

            InitialItems_Tests(linkedList, items);
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            throw new NotImplementedException();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Bridge.ClientTest.Collections.Generic.Base;
using Bridge.Test.NUnit;

namespace Bridge.ClientTest.Collections.Generic
{
    /// <summary>
    /// Contains tests that ensure the correctness of the LinkedList class.
    /// </summary>
    public abstract partial class LinkedList_Generic_Tests<T> : TestBase<T>
    {
        #region ICollection<T> Helper Methods

        protected ICollection<T> GenericICollectionFactory()
        {
            return GenericLinkedListFactory();
        }

        protected Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

        #endregion

        #region LinkedList<T> Helper Methods

        protected virtual LinkedList<T> GenericLinkedListFactory()
        {
            return new LinkedList<T>();
        }

        /// <summary>
        /// Tests the items in the list to make sure they are the same.
        /// </summary>
        protected void InitialItems_Tests(LinkedList<T> collection, T[] expectedItems)
        {
            VerifyState(collection, expectedItems);
            VerifyGenericEnumerator(collection, expectedItems);
            VerifyEnumerator(collection, expectedItems);
        }

        /// <summary>
        /// Verifies that the tail/head properties are valid and
        /// can iterate through the list (backwards and forwards) to
        /// verify the contents of the list.
        /// </summary>
        internal static void VerifyState(LinkedList<T> linkedList, T[] expectedItems)
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
                Assert.True(linkedList.Contains(expectedItems[i]), "Err_9872haid Expected Contains with item=" + expectedItems[i] + " to return true");
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
        internal static void VerifyLinkedListNode(LinkedListNode<T> node, T expectedValue, LinkedList<T> expectedList,
            LinkedListNode<T> expectedPrevious, LinkedListNode<T> expectedNext)
        {
            Assert.AreEqual(expectedValue, node.Value); //"Err_548ajoid Node Value"
            Assert.AreEqual(expectedList, node.List); //"Err_0821279 Node List"

            Assert.AreEqual(expectedPrevious, node.Previous); //"Err_8548ajhiod Previous Node"
            Assert.AreEqual(expectedNext, node.Next); //"Err_4688anmjod Next Node"
        }

        /// <summary>
        /// verifies that the contents of a linkedlist node are correct.
        /// </summary>
        internal static void VerifyLinkedListNode(LinkedListNode<T> node, T expectedValue, LinkedList<T> expectedList,
            bool expectedPreviousNull, bool expectedNextNull)
        {
            Assert.AreEqual(expectedValue, node.Value); //"Err_548ajoid Expected Node Value"
            Assert.AreEqual(expectedList, node.List); //"Err_0821279 Expected Node List"

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
        protected void VerifyGenericEnumerator(ICollection<T> collection, T[] expectedItems)
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
                    "Err_9844awpa More items have been returned from the enumerator(" + iterations + " items) than are in the expectedElements(" + expectedCount + " items)");

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
        protected void VerifyEnumerator(ICollection<T> collection, T[] expectedItems)
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
                    if (itemsVisited[i])
                    {
                        continue;
                    }
                    if ((expectedItems[i] == null && currentItem == null)
                        || (expectedItems[i] != null && expectedItems[i].Equals(currentItem)))
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
                    Assert.AreEqual(currentItem, tempItem, "Current is returning inconsistent results Current."); //"Err_8776phaw Current is returning inconsistent results Current."
                }

                iterations++;
            }

            for (int i = 0; i < expectedCount; ++i)
            {
                Assert.True(itemsVisited[i], "Err_052848ahiedoi Expected Current to return true for item: " + expectedItems[i] + "index: " + i);
            }

            Assert.AreEqual(expectedCount, iterations, "Number of items to iterate through"); //"Err_658805eauz Number of items to iterate through"

            for (int i = 0; i < 3; i++)
            {
                Assert.False(enumerator.MoveNext(), "Expected MoveNext to return false"); //"Err_2929ahiea Expected MoveNext to return false after" + iterations + " iterations"
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

        protected void VerifyContains(LinkedList<T> linkedList, T[] expectedItems)
        {
            //[] Verify Contains
            for (int i = 0; i < expectedItems.Length; i++)
            {
                Assert.True(linkedList.Contains(expectedItems[i]),
                    "Err_9872haid Expected Contains with item=" + expectedItems[i] + " to return true");
            }
        }

        protected void VerifyFindLastDuplicates(LinkedList<T> linkedList, T[] expectedItems)
        {
            LinkedListNode<T> previousNode, currentNode = null, nextNode;
            LinkedListNode<T>[] nodes = new LinkedListNode<T>[expectedItems.Length];
            int index = 0;

            currentNode = linkedList.First;

            while (currentNode != null)
            {
                nodes[index] = currentNode;
                currentNode = currentNode.Next;
                ++index;
            }

            for (int i = 0; i < expectedItems.Length; ++i)
            {
                currentNode = linkedList.FindLast(expectedItems[i]);

                index = Array.LastIndexOf(expectedItems, expectedItems[i]);
                previousNode = 0 < index ? nodes[index - 1] : null;
                nextNode = nodes.Length - 1 > index ? nodes[index + 1] : null;

                Assert.AreEqual(nodes[index], currentNode); //"Node returned from FindLast index=" + i.ToString()

                VerifyLinkedListNode(currentNode, expectedItems[i], linkedList, previousNode, nextNode);
            }
        }

        protected void VerifyFindDuplicates(LinkedList<T> linkedList, T[] expectedItems)
        {
            LinkedListNode<T> previousNode, currentNode = null, nextNode;
            LinkedListNode<T>[] nodes = new LinkedListNode<T>[expectedItems.Length];
            int index = 0;

            currentNode = linkedList.First;

            while (currentNode != null)
            {
                nodes[index] = currentNode;
                currentNode = currentNode.Next;
                ++index;
            }

            for (int i = 0; i < expectedItems.Length; ++i)
            {
                currentNode = linkedList.Find(expectedItems[i]);

                index = Array.IndexOf(expectedItems, expectedItems[i]);
                previousNode = 0 < index ? nodes[index - 1] : null;
                nextNode = nodes.Length - 1 > index ? nodes[index + 1] : null;

                Assert.AreEqual(nodes[index], currentNode); //"Node returned from Find index=" + i.ToString()

                VerifyLinkedListNode(currentNode, expectedItems[i], linkedList, previousNode, nextNode);
            }
        }

        protected void VerifyFindLast(LinkedList<T> linkedList, T[] expectedItems)
        {
            LinkedListNode<T> previousNode, currentNode, nextNode;

            currentNode = null;
            for (int i = 0; i < expectedItems.Length; ++i)
            {
                previousNode = currentNode;
                currentNode = linkedList.FindLast(expectedItems[i]);
                nextNode = currentNode.Next;
                VerifyLinkedListNode(currentNode, expectedItems[i], linkedList, previousNode, nextNode);
            }

            currentNode = null;
            for (int i = expectedItems.Length - 1; 0 <= i; --i)
            {
                nextNode = currentNode;
                currentNode = linkedList.FindLast(expectedItems[i]);
                previousNode = currentNode.Previous;
                VerifyLinkedListNode(currentNode, expectedItems[i], linkedList, previousNode, nextNode);
            }
        }

        protected void VerifyFind(LinkedList<T> linkedList, T[] expectedItems)
        {
            LinkedListNode<T> previousNode, currentNode, nextNode;

            currentNode = null;
            for (int i = 0; i < expectedItems.Length; ++i)
            {
                previousNode = currentNode;
                currentNode = linkedList.Find(expectedItems[i]);
                nextNode = currentNode.Next;
                VerifyLinkedListNode(currentNode, expectedItems[i], linkedList, previousNode, nextNode);
            }

            currentNode = null;
            for (int i = expectedItems.Length - 1; 0 <= i; --i)
            {
                nextNode = currentNode;
                currentNode = linkedList.Find(expectedItems[i]);
                previousNode = currentNode.Previous;
                VerifyLinkedListNode(currentNode, expectedItems[i], linkedList, previousNode, nextNode);
            }
        }

        protected void VerifyRemovedNode(LinkedListNode<T> node, T expectedValue)
        {
            LinkedList<T> tempLinkedList = new LinkedList<T>();
            LinkedListNode<T> headNode, tailNode;

            tempLinkedList.AddLast(default(T));
            tempLinkedList.AddLast(default(T));
            headNode = tempLinkedList.First;
            tailNode = tempLinkedList.Last;

            Assert.Null(node.List); //"Err_298298anied Node.LinkedList returned non null"
            Assert.Null(node.Previous); //"Err_298298anied Node.Previous returned non null"
            Assert.Null(node.Next); //"Err_298298anied Node.Next returned non null"
            Assert.AreEqual(expectedValue, node.Value); //"Err_969518aheoia Node.Value"

            tempLinkedList.AddAfter(tempLinkedList.First, node);

            Assert.AreEqual(tempLinkedList, node.List); //"Err_7894ahioed Node.LinkedList"
            Assert.AreEqual(headNode, node.Previous); //"Err_14520aheoak Node.Previous"
            Assert.AreEqual(tailNode, node.Next); //"Err_42358aujea Node.Next"
            Assert.AreEqual(expectedValue, node.Value); //"Err_64888joqaxz Node.Value"

            InitialItems_Tests(tempLinkedList, new T[] { default(T), expectedValue, default(T) });
        }

        protected void VerifyRemovedNode(LinkedList<T> linkedList, LinkedListNode<T> node, T expectedValue)
        {
            LinkedListNode<T> tailNode = linkedList.Last;

            Assert.Null(node.List); //"Err_564898ajid Node.LinkedList returned non null"
            Assert.Null(node.Previous); //"Err_30808wia Node.Previous returned non null"
            Assert.Null(node.Next); //"Err_78280aoiea Node.Next returned non null"
            Assert.AreEqual(expectedValue, node.Value); //"Err_98234aued Node.Value"

            linkedList.AddLast(node);
            Assert.AreEqual(linkedList, node.List); //"Err_038369aihead Node.LinkedList"
            Assert.AreEqual(tailNode, node.Previous); //"Err_789108aiea Node.Previous"
            Assert.Null(node.Next); //"Err_37896riad Node.Next returned non null"

            linkedList.RemoveLast();
        }

        protected void VerifyRemovedNode(LinkedList<T> linkedList, T[] linkedListValues, LinkedListNode<T> node, T expectedValue)
        {
            LinkedListNode<T> tailNode = linkedList.Last;

            Assert.Null(node.List); //"Err_564898ajid Node.LinkedList returned non null"
            Assert.Null(node.Previous); //"Err_30808wia Node.Previous returned non null"
            Assert.Null(node.Next); //"Err_78280aoiea Node.Next returned non null"
            Assert.AreEqual(expectedValue, node.Value); //"Err_98234aued Node.Value"

            linkedList.AddLast(node);
            Assert.AreEqual(linkedList, node.List); //"Err_038369aihead Node.LinkedList"
            Assert.AreEqual(tailNode, node.Previous); //"Err_789108aiea Node.Previous"
            Assert.Null(node.Next); //"Err_37896riad Node.Next returned non null"
            Assert.AreEqual(expectedValue, node.Value); //"Err_823902jaied Node.Value"

            T[] expected = new T[linkedListValues.Length + 1];
            Array.Copy(linkedListValues, 0, expected, 0, linkedListValues.Length);
            expected[linkedListValues.Length] = expectedValue;

            InitialItems_Tests(linkedList, expected);
            linkedList.RemoveLast();
        }

        #endregion
    }
}

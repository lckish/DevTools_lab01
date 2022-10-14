using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace PowerCollections.Tests
{
    [TestClass]
    public class StackTests
    {        
        //Constructor test
        [TestMethod]
        public void TestConstructorStack()
        {
            Stack<int> stack = new Stack<int>(10);
            Assert.AreEqual(0, stack.Count);
            Assert.AreEqual(10, stack.Capacity);
        }

        //Enumerator Test
        [TestMethod]
        public void Enumerator_ReturnInvertedStack()
        {
            Stack<int> stack = new Stack<int>(3);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            int[] Arr = new int[] { 3, 2, 1 };
            int[] testArr = new int[3];
            int i = 0;
            foreach (int item in stack)
                testArr[i++] = item;
            CollectionAssert.AreEqual(Arr, testArr);
        }

        //Push tests
        [TestMethod]
        public void TestPush_IfStackNotIsFull()
        {
            Stack<string> stack = new Stack<string>(10);
            stack.Push("1");
            stack.Push("2");
            stack.Push("3");
            Assert.AreEqual(3, stack.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestPushWithException_IfStackFull()
        {
            Stack<string> stack = new Stack<string>(1);
            stack.Push("1");
            stack.Push("2");
        }

        //Pop tests
        [TestMethod]
        public void TestPop_IfStackNotIsEmpty()
        {
            Stack<string> stack = new Stack<string>(10);
            string a = "1";
            string b = "2";
            stack.Push(a);
            stack.Push(b);
            string pop1 = stack.Pop();
            string pop2 = stack.Pop();
            Assert.AreEqual(pop1, b);
            Assert.AreEqual(pop2, a);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestPopWithException_StackIsEmpty()
        {
            Stack<int> stack = new Stack<int>(1);
            stack.Pop();
        }

        //Top Tests
        [TestMethod]
        public void TestTop_IfStackNotIsEmpty()
        {
            Stack<string> stack = new Stack<string>(10);
            stack.Push("1");
            stack.Push("2");
            stack.Push("3");
            string t1 = stack.Top();
            string t2 = stack.Top();
            Assert.AreEqual("3", t1);
            Assert.AreEqual("3", t2);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestTopWithException_StackIsEmpty()
        {
            Stack<int> stack = new Stack<int>(1);
            stack.Top();
        }

        //IsEmpty Tests
        [TestMethod]
        public void TestIsEmpty_TrueWhenCreateNewStack()
        {
            Stack<string> stack = new Stack<string>(10);
            Assert.IsTrue(stack.IsEmpty);
        }

        [TestMethod]
        public void TestIsEmpty_FalseAfterPush()
        {
            Stack<int> stack = new Stack<int>(10);
            stack.Push(1);
            Assert.IsFalse(stack.IsEmpty);
        }

        [TestMethod]
        public void TestIsEmpty_TrueAfterPop()
        {
            Stack<int> stack = new Stack<int>(3);
            stack.Push(1);
            stack.Pop();
            Assert.IsTrue(stack.IsEmpty);
        }


        //IsFull tests
        [TestMethod]
        public void TestIsFull_TrueAfterFilligStack()
        {
            Stack<string> stack = new Stack<string>(1);
            stack.Push("r");
            Assert.IsTrue(stack.IsFull);
        }

        [TestMethod]
        public void TestIsFull_TrueWhenCreateNewStack()
        {
            Stack<int> stack = new Stack<int>(10);
            Assert.IsFalse(stack.IsFull);
        }

        [TestMethod]
        public void TestIsFull_FalseAfterPop()
        {
            Stack<int> stack = new Stack<int>(2);
            stack.Push(2);
            stack.Push(1);
            stack.Pop();
            Assert.IsFalse(stack.IsFull);
        }

        //Exception
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void TestException_WhenCreateNewStackWithNegativeValue()
        {
            Stack<int> stack = new Stack<int>(-2);
        }
    }
}
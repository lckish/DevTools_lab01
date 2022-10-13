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
        [TestMethod]
        public void Enumerator()
        {
            int[] arr = new int[] { 5, 2, 7 };
            Stack<int> stack = new Stack<int>(10);
            foreach (int i in arr)
                stack.Push(i);
            var revArr = arr.Reverse().ToArray();
            Assert.AreEqual(5, revArr[2]);
            Assert.AreEqual(7, stack.Pop());
        }

        [TestMethod]
        public void TestPush()
        {
            Stack<string> stack = new Stack<string>(10);
            stack.Push("1");
            stack.Push("2");
            stack.Push("3");
            Assert.AreEqual(3, stack.Count);
        }

        [TestMethod]
        public void TestPop()
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
        public void TestConstructorStack()
        {
            Stack<int> stack = new Stack<int>(10);
            Assert.AreEqual(0, stack.Count);
            Assert.AreEqual(10, stack.Capacity);
        }

        [TestMethod]
        public void TestTop()
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
        public void TestIsEmpty()
        {
            Stack<string> stack = new Stack<string>(10);
            Assert.IsTrue(stack.IsEmpty);
        }

        [TestMethod]
        public void TestIsFull()
        {
            Stack<string> stack = new Stack<string>(10);
            for (int i = 0; i < stack.Capacity; i++)
                stack.Push($"push {i++}");
            Assert.IsTrue(stack.IsFull);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestPushExceptionIndexOutOfRange()
        {
            Stack<int> stack = new Stack<int>(0);
            stack.Push(1);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestTopExceptionIndexOutOfRange()
        {
            Stack<int> stack = new Stack<int>(1);
            stack.Top();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestPopExceptionIndexOutOfRange()
        {
            Stack<int> stack = new Stack<int>(1);
            stack.Pop();
        }
    }
}
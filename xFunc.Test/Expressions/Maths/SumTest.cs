﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;

namespace xFunc.Test.Expressions.Maths
{

    [TestClass]
    public class SumTest
    {

        [TestMethod]
        public void CalculateTest1()
        {
            var sum = new Sum(new Variable("i"), new Number(20));

            Assert.AreEqual(210.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest2()
        {
            var sum = new Sum(new Variable("i"), new Number(4), new Number(20));

            Assert.AreEqual(204.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest3()
        {
            var sum = new Sum(new Variable("i"), new Number(4), new Number(20), new Number(2));

            Assert.AreEqual(108.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest4()
        {
            var sum = new Sum(new Variable("k"), new Number(4), new Number(20), new Number(2), new Variable("k"));

            Assert.AreEqual(108.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest5()
        {
            var sum = new Sum(new Pow(new Variable("a"), new Variable("i")), new Number(4));

            Assert.AreEqual(30.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

        [TestMethod]
        public void CalculateTest6()
        {
            var sum = new Sum(new Pow(new Variable("a"), new Variable("i")), new Number(2), new Number(5));

            Assert.AreEqual(60.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

        [TestMethod]
        public void CalculateTest7()
        {
            var sum = new Sum(new Pow(new Variable("a"), new Variable("i")), new Number(4), new Number(8), new Number(2));

            Assert.AreEqual(336.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

        [TestMethod]
        public void CalculateTest8()
        {
            var sum = new Sum(new Pow(new Variable("a"), new Variable("k")), new Number(4), new Number(8), new Number(2), new Variable("k"));

            Assert.AreEqual(336.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

    }

}

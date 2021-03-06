﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;

namespace xFunc.Test.Expressions.Maths
{

    [TestClass]
    public class ProductTest
    {

        [TestMethod]
        public void CalculateTest1()
        {
            var sum = new Product(new Variable("i"), new Number(8));

            Assert.AreEqual(40320.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest2()
        {
            var sum = new Product(new Variable("i"), new Number(4), new Number(8));

            Assert.AreEqual(6720.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest3()
        {
            var sum = new Product(new Variable("i"), new Number(4), new Number(8), new Number(2));

            Assert.AreEqual(192.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest4()
        {
            var sum = new Product(new Variable("k"), new Number(4), new Number(8), new Number(2), new Variable("k"));

            Assert.AreEqual(192.0, sum.Calculate());
        }

        [TestMethod]
        public void CalculateTest5()
        {
            var sum = new Product(new Pow(new Variable("a"), new Variable("i")), new Number(4));

            Assert.AreEqual(1024.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

        [TestMethod]
        public void CalculateTest6()
        {
            var sum = new Product(new Pow(new Variable("a"), new Variable("i")), new Number(2), new Number(5));

            Assert.AreEqual(16384.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

        [TestMethod]
        public void CalculateTest7()
        {
            var sum = new Product(new Pow(new Variable("a"), new Variable("i")), new Number(4), new Number(8), new Number(2));

            Assert.AreEqual(262144.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

        [TestMethod]
        public void CalculateTest8()
        {
            var sum = new Product(new Pow(new Variable("a"), new Variable("k")), new Number(4), new Number(8), new Number(2), new Variable("k"));

            Assert.AreEqual(262144.0, sum.Calculate(new ParameterCollection() { new Parameter("a", 2) }));
        }

    }

}

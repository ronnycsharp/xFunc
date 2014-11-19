﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using xFunc.Maths.Expressions;

namespace xFunc.Test.Expressions.Maths
{

    [TestClass]
    public class PowTest
    {

        [TestMethod]
        public void CalculateTest()
        {
            IExpression exp = new Pow(new Number(2), new Number(10));

            Assert.AreEqual(1024.0, exp.Calculate());
        }

        [TestMethod]
        public void NegativeCalculateTest()
        {
            IExpression exp = new Pow(new Number(-8), new Number(1 / 3.0));

            Assert.AreEqual(-2.0, exp.Calculate());
        }

        [TestMethod]
        public void NegativeNumberCalculateTest()
        {
            var exp = new Pow(new Number(-25), new Number(1 / 2.0));

            Assert.AreEqual(double.NaN, exp.Calculate());
        }

    }

}

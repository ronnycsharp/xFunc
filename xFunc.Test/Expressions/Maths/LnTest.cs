﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using xFunc.Maths.Expressions;

namespace xFunc.Test.Expressions.Maths
{

    [TestClass]
    public class LnTest
    {

        [TestMethod]
        public void CalculateTest()
        {
            IExpression exp = new Ln(new Number(2));

            Assert.AreEqual(Math.Log(2), exp.Calculate());
        }

        [TestMethod]
        public void DerivativeTest1()
        {
            IExpression exp = new Ln(new Mul(new Number(2), new Variable("x")));
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("(2 * 1) / (2 * x)", deriv.ToString());
        }

        [TestMethod]
        public void DerivativeTest2()
        {
            // ln(2x)
            Number num = new Number(2);
            Variable x = new Variable("x");
            Mul mul = new Mul(num, x);

            IExpression exp = new Ln(mul);
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("(2 * 1) / (2 * x)", deriv.ToString());

            num.Value = 5;
            Assert.AreEqual("ln(5 * x)", exp.ToString());
            Assert.AreEqual("(2 * 1) / (2 * x)", deriv.ToString());
        }

        [TestMethod]
        public void PartialDerivativeTest1()
        {
            // ln(xy)
            IExpression exp = new Ln(new Mul(new Variable("x"), new Variable("y")));
            IExpression deriv = exp.Differentiate();
            Assert.AreEqual("(1 * y) / (x * y)", deriv.ToString());
        }

        [TestMethod]
        public void PartialDerivativeTest2()
        {
            // ln(xy)
            IExpression exp = new Ln(new Mul(new Variable("x"), new Variable("y")));
            IExpression deriv = exp.Differentiate(new Variable("y"));
            Assert.AreEqual("(x * 1) / (x * y)", deriv.ToString());
        }

        [TestMethod]
        public void PartialDerivativeTest3()
        {
            IExpression exp = new Ln(new Variable("y"));
            IExpression deriv = exp.Differentiate();
            Assert.AreEqual("0", deriv.ToString());
        }

    }

}

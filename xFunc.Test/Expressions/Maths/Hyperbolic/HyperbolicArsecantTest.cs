﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Hyperbolic;

namespace xFunc.Test.Expressions.Maths.Hyperbolic
{

    [TestClass]
    public class HyperbolicArsecantTest
    {

        [TestMethod]
        public void CalculateTest()
        {
            var exp = new Arsech(new Number(1));

            Assert.AreEqual(MathExtentions.Asech(1), exp.Calculate());
        }

        [TestMethod]
        public void DerivativeTest()
        {
            IExpression exp = new Arsech(new Mul(new Number(2), new Variable("x")));
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("-((2 * 1) / ((2 * x) * sqrt(1 - ((2 * x) ^ 2))))", deriv.ToString());
        }

    }

}

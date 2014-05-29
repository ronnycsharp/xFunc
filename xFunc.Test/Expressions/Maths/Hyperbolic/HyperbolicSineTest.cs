﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Hyperbolic;

namespace xFunc.Test.Expressions.Maths.Hyperbolic
{

    [TestClass]
    public class HyperbolicSineTest
    {
        
        [TestMethod]
        public void CalculateTest()
        {
            var exp = new Sinh(new Number(1));

            Assert.AreEqual(Math.Sinh(1), exp.Calculate());
        }

        [TestMethod]
        public void DerivativeTest()
        {
            IExpression exp = new Sinh(new Mul(new Number(2), new Variable("x")));
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("(2 * 1) * cosh(2 * x)", deriv.ToString());
        }

    }

}

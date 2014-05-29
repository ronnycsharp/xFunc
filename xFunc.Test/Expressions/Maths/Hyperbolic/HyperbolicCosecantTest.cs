﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Hyperbolic;

namespace xFunc.Test.Expressions.Maths.Hyperbolic
{

    [TestClass]
    public class HyperbolicCosecantTest
    {

        [TestMethod]
        public void CalculateTest()
        {
            var exp = new Csch(new Number(1));

            Assert.AreEqual(MathExtentions.Csch(1), exp.Calculate());
        }

        [TestMethod]
        public void DerivativeTest()
        {
            IExpression exp = new Csch(new Mul(new Number(2), new Variable("x")));
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("-((2 * 1) * (coth(2 * x) * csch(2 * x)))", deriv.ToString());
        }

    }

}

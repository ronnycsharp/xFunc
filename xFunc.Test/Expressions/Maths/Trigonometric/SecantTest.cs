﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Trigonometric;

namespace xFunc.Test.Expressions.Maths.Trigonometric
{

    [TestClass]
    public class SecantTest
    {
        
        [TestMethod]
        public void CalculateDegreeTest()
        {
            IExpression exp = new Sec(new Number(1));

            Assert.AreEqual(MathExtentions.Sec(Math.PI / 180), exp.Calculate(AngleMeasurement.Degree));
        }

        [TestMethod]
        public void CalculateRadianTest()
        {
            IExpression exp = new Sec(new Number(1));

            Assert.AreEqual(MathExtentions.Sec(1), exp.Calculate(AngleMeasurement.Radian));
        }

        [TestMethod]
        public void CalculateGradianTest()
        {
            IExpression exp = new Sec(new Number(1));

            Assert.AreEqual(MathExtentions.Sec(Math.PI / 200), exp.Calculate(AngleMeasurement.Gradian));
        }

        [TestMethod]
        public void DerivativeTest1()
        {
            IExpression exp = new Sec(new Mul(new Number(2), new Variable("x")));
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("(2 * 1) * (tan(2 * x) * sec(2 * x))", deriv.ToString());
        }

        [TestMethod]
        public void DerivativeTest2()
        {
            // sec(2x)
            Number num = new Number(2);
            Variable x = new Variable("x");
            Mul mul = new Mul(num, x);

            IExpression exp = new Sec(mul);
            IExpression deriv = exp.Differentiate();

            Assert.AreEqual("(2 * 1) * (tan(2 * x) * sec(2 * x))", deriv.ToString());

            num.Value = 4;
            Assert.AreEqual("sec(4 * x)", exp.ToString());
            Assert.AreEqual("(2 * 1) * (tan(2 * x) * sec(2 * x))", deriv.ToString());
        }

    }
}

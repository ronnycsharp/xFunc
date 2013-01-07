﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using xFunc.Library.Maths;
using xFunc.Library.Maths.Expressions;

namespace xFunc.Test.Expressions.Maths
{

    [TestClass]
    public class ArccscMathExpressionTest
    {

        private MathParser parser;

        [TestInitialize]
        public void TestInit()
        {
            parser = new MathParser();
        }

        [TestMethod]
        public void CalculateRadianTest()
        {
            parser.AngleMeasurement = AngleMeasurement.Radian;
            IMathExpression exp = parser.Parse("arccsc(1)");

            Assert.AreEqual(MathExtentions.Acsc(1), exp.Calculate(null));
        }

        [TestMethod]
        public void CalculateDegreeTest()
        {
            parser.AngleMeasurement = AngleMeasurement.Degree;
            IMathExpression exp = parser.Parse("arccsc(1)");

            Assert.AreEqual(MathExtentions.Acsc(1) / Math.PI * 180, exp.Calculate(null));
        }

        [TestMethod]
        public void CalculateGradianTest()
        {
            parser.AngleMeasurement = AngleMeasurement.Gradian;
            IMathExpression exp = parser.Parse("arccsc(1)");

            Assert.AreEqual(MathExtentions.Acsc(1) / Math.PI * 200, exp.Calculate(null));
        }

        [TestMethod]
        public void DerivativeTest()
        {
            IMathExpression exp = parser.Parse("deriv(arccsc(2x), x)").Derivative();

            Assert.AreEqual("-(2 / (abs((2 * x)) * sqrt(((2 * x) ^ 2) - 1)))", exp.ToString());
        }

    }

}

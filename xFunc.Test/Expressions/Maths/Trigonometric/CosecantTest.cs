﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Trigonometric;

namespace xFunc.Test.Expressions.Maths.Trigonometric
{

    [TestClass]
    public class CosecantTest
    {

        private MathParser parser;

        [TestInitialize]
        public void TestInit()
        {
            parser = new MathParser();
        }

        [TestMethod]
        public void CalculateDegreeTest()
        {
            parser.AngleMeasurement = AngleMeasurement.Degree;
            IMathExpression exp = parser.Parse("csc(1)");

            Assert.AreEqual(MathExtentions.Csc(Math.PI / 180), exp.Calculate(null));
        }

        [TestMethod]
        public void CalculateRadianTest()
        {
            parser.AngleMeasurement = AngleMeasurement.Radian;
            IMathExpression exp = parser.Parse("csc(1)");

            Assert.AreEqual(MathExtentions.Csc(1), exp.Calculate(null));
        }

        [TestMethod]
        public void CalculateGradianTest()
        {
            parser.AngleMeasurement = AngleMeasurement.Gradian;
            IMathExpression exp = parser.Parse("csc(1)");

            Assert.AreEqual(MathExtentions.Csc(Math.PI / 200), exp.Calculate(null));
        }

        [TestMethod]
        public void DerivativeTest()
        {
            IMathExpression exp = parser.Parse("deriv(csc(2x), x)").Differentiation();

            Assert.AreEqual("-2 * (cot(2 * x) * csc(2 * x))", exp.ToString());
        }

    }

}
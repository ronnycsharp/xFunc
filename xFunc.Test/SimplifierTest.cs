﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xFunc.Maths;
using xFunc.Maths.Expressions;

namespace xFunc.Test
{

    [TestClass]
    public class SimplifierTest
    {

        ISimplifier simplifier;

        public SimplifierTest()
        {
            simplifier = new MathSimplifier();
        }

        private void SimpleTest(IMathExpression exp, IMathExpression expected)
        {
            var simple = simplifier.Simplify(exp);

            Assert.AreEqual(expected, simple);
        }

        [TestMethod]
        public void DoubleUnary()
        {
            var un = new UnaryMinus(new UnaryMinus(new Variable("x")));
            var expected = new Variable("x");

            SimpleTest(un, expected);
        }

        [TestMethod]
        public void UnaryNumber()
        {
            var un = new UnaryMinus(new Number(1));
            var expected = new Number(-1);

            SimpleTest(un, expected);
        }

        #region Add

        [TestMethod]
        public void AddFirstZero()
        {
            var add = new Add(new Number(0), new Variable("x"));
            var expected = new Variable("x");

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddSecondZero()
        {
            var add = new Add(new Variable("x"), new Number(0));
            var expected = new Variable("x");

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddTwoNumbers()
        {
            var add = new Add(new Number(3), new Number(2));
            var expected = new Number(5);

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddFirstUnaryMinus()
        {
            var add = new Add(new UnaryMinus(new Variable("x")), new Number(2));
            var expected = new Sub(new Number(2), new Variable("x"));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddSecondUnaryMinus()
        {
            var add = new Add(new Number(2), new UnaryMinus(new Variable("x")));
            var expected = new Sub(new Number(2), new Variable("x"));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiffNumAdd_NumAddVar_()
        {
            // 2 + (2 + x)
            var add = new Add(new Number(2), new Add(new Number(2), new Variable("x")));
            var expected = new Add(new Variable("x"), new Number(4));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiffNumAdd_VarAddNum_()
        {
            // 2 + (x + 2)
            var add = new Add(new Number(2), new Add(new Variable("x"), new Number(2)));
            var expected = new Add(new Variable("x"), new Number(4));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiff_NumAddVar_AddNum()
        {
            // (2 + x) + 2
            var add = new Add(new Add(new Number(2), new Variable("x")), new Number(2));
            var expected = new Add(new Variable("x"), new Number(4));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiff_VarAddNum_AddNum()
        {
            // (x + 2) + 2
            var add = new Add(new Add(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Add(new Variable("x"), new Number(4));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiffNum_NumSubVar_()
        {
            // 2 + (2 - x)
            var add = new Add(new Number(2), new Sub(new Number(2), new Variable("x")));
            var expected = new Sub(new Number(4), new Variable("x"));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiffNum_VarSubNum_()
        {
            // 2 + (x - 2)
            var add = new Add(new Number(2), new Sub(new Variable("x"), new Number(2)));
            var expected = new Variable("x");

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiff_NumSubVar_AddNum()
        {
            // (2 - x) + 2
            var add = new Add(new Sub(new Number(2), new Variable("x")), new Number(2));
            var expected = new Sub(new Number(4), new Variable("x"));

            SimpleTest(add, expected);
        }

        [TestMethod]
        public void AddDiff_VarSubNum_AddNum()
        {
            // (x - 2) + 2
            var add = new Add(new Sub(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Variable("x");

            SimpleTest(add, expected);
        }

        #endregion

        #region Sub

        [TestMethod]
        public void SubFirstZero()
        {
            var sub = new Sub(new Number(0), new Variable("x"));
            var expected = new UnaryMinus(new Variable("x"));

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubSecondZero()
        {
            var sub = new Sub(new Variable("x"), new Number(0));
            var expected = new Variable("x");

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubTwoNumbers()
        {
            var sub = new Sub(new Number(3), new Number(2));
            var expected = new Number(1);

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubSecondUnaryMinus()
        {
            var sub = new Sub(new Number(2), new UnaryMinus(new Variable("x")));
            var expected = new Add(new Number(2), new Variable("x"));

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiff_NumAddVar_SubNum()
        {
            // (2 + x) - 2
            var sub = new Sub(new Add(new Number(2), new Variable("x")), new Number(2));
            var expected = new Variable("x");

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiff_VarAddNum_SubNum()
        {
            // (x + 2) - 2
            var sub = new Sub(new Add(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Variable("x");

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiffNumSub_NumAddVar_()
        {
            // 2 - (2 + x)
            var sub = new Sub(new Number(2), new Add(new Number(2), new Variable("x")));
            var expected = new UnaryMinus(new Variable("x"));

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiffNumSub_VarAddNum_()
        {
            // 2 - (x + 2)
            var sub = new Sub(new Number(2), new Add(new Variable("x"), new Number(2)));
            var expected = new UnaryMinus(new Variable("x"));

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiff_NumSubVar_SubNum()
        {
            var sub = new Sub(new Sub(new Number(2), new Variable("x")), new Number(2));
            var expected = new UnaryMinus(new Variable("x"));

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiff_VarSubNum_SubNum()
        {
            var sub = new Sub(new Sub(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Sub(new Variable("x"), new Number(4));

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiffNumSub_NumSubVar_()
        {
            var sub = new Sub(new Number(2), new Sub(new Number(2), new Variable("x")));
            var expected = new Variable("x");

            SimpleTest(sub, expected);
        }

        [TestMethod]
        public void SubDiffNumSub_VarSubNum_()
        {
            var sub = new Sub(new Number(2), new Sub(new Variable("x"), new Number(2)));
            var expected = new Sub(new Number(4), new Variable("x"));

            SimpleTest(sub, expected);
        }

        #endregion

        #region Mul

        [TestMethod]
        public void MulByZero()
        {
            var mul = new Mul(new Variable("x"), new Number(0));
            var expected = new Number(0);

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulFirstOne()
        {
            var mul = new Mul(new Number(1), new Variable("x"));
            var expected = new Variable("x");

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulSecondOne()
        {
            var mul = new Mul(new Variable("x"), new Number(1));
            var expected = new Variable("x");

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulTwoNumbers()
        {
            var mul = new Mul(new Number(2), new Number(3));
            var expected = new Number(6);

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiffNumMul_NumMulVar_()
        {
            var mul = new Mul(new Number(2), new Mul(new Number(2), new Variable("x")));
            var expected = new Mul(new Number(4), new Variable("x"));

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiffNumMul_VarMulNum_()
        {
            var mul = new Mul(new Number(2), new Mul(new Variable("x"), new Number(2)));
            var expected = new Mul(new Number(4), new Variable("x"));

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiff_NumMulVar_MulNum()
        {
            var mul = new Mul(new Mul(new Number(2), new Variable("x")), new Number(2));
            var expected = new Mul(new Number(4), new Variable("x"));

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiff_VarMulNum_MulNum()
        {
            var mul = new Mul(new Mul(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Mul(new Number(4), new Variable("x"));

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiffNumMul_NumDivVar_()
        {
            // 2 * (2 / x)
            var mul = new Mul(new Number(2), new Div(new Number(2), new Variable("x")));
            var expected = new Div(new Number(4), new Variable("x"));

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiffNumMul_VarDivNum_()
        {
            // 2 * (x / 2)
            var mul = new Mul(new Number(2), new Div(new Variable("x"), new Number(2)));
            var expected = new Variable("x");

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiffl_NumDivVar_MulNum()
        {
            // (2 / x) * 2
            var mul = new Mul(new Div(new Number(2), new Variable("x")), new Number(2));
            var expected = new Div(new Number(4), new Variable("x"));

            SimpleTest(mul, expected);
        }

        [TestMethod]
        public void MulDiff_VarDivNum_MulNum()
        {
            // (x / 2) * 2
            var mul = new Mul(new Div(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Variable("x");

            SimpleTest(mul, expected);
        }

        #endregion

        #region Div

        [TestMethod]
        public void DivZero()
        {
            var div = new Div(new Number(0), new Variable("x"));
            var expected = new Number(0);

            SimpleTest(div, expected);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void DivByZero()
        {
            var div = new Div(new Variable("x"), new Number(0));

            SimpleTest(div, null);
        }

        [TestMethod]
        public void DivByOne()
        {
            var div = new Div(new Variable("x"), new Number(1));
            var expected = new Variable("x");

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivTwoNumbers()
        {
            var div = new Div(new Number(8), new Number(2));
            var expected = new Number(4);

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiff_NumMulVar_DivNum()
        {
            // (2 * x) / 4
            var div = new Div(new Mul(new Number(2), new Variable("x")), new Number(4));
            var expected = new Div(new Variable("x"), new Number(2));

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiff_VarMulNum_DivNum()
        {
            // (x * 2) / 4
            var div = new Div(new Mul(new Variable("x"), new Number(2)), new Number(4));
            var expected = new Div(new Variable("x"), new Number(2));

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiffNumDiv_NumMulVar_()
        {
            // 2 / (2 * x)
            var div = new Div(new Number(2), new Mul(new Number(2), new Variable("x")));
            var expected = new Div(new Number(1), new Variable("x"));

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiffNumDiv_VarMulNum_()
        {
            // 2 / (2 * x)
            var div = new Div(new Number(2), new Mul(new Variable("x"), new Number(2)));
            var expected = new Div(new Number(1), new Variable("x"));

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiff_NumDivVar_DivNum()
        {
            // (2 / x) / 2
            var div = new Div(new Div(new Number(2), new Variable("x")), new Number(2));
            var expected = new Div(new Number(1), new Variable("x"));

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiff_VarDivNum_DivNum()
        {
            // (x / 2) / 2
            var div = new Div(new Div(new Variable("x"), new Number(2)), new Number(2));
            var expected = new Div(new Variable("x"), new Number(4));

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiffNumDiv_NumDivVar_()
        {
            // 2 / (2 / x)
            var div = new Div(new Number(2), new Div(new Number(2), new Variable("x")));
            var expected = new Variable("x");

            SimpleTest(div, expected);
        }

        [TestMethod]
        public void DivDiffNumDiv_VarDivNum_()
        {
            // 2 / (x / 2)
            var div = new Div(new Number(2), new Div(new Variable("x"), new Number(2)));
            var expected = new Div(new Number(4), new Variable("x"));

            SimpleTest(div, expected);
        }

        #endregion

        [TestMethod]
        public void PowerZero()
        {
            var pow = new Pow(new Variable("x"), new Number(0));
            var expected = new Number(1);

            SimpleTest(pow, expected);
        }

        [TestMethod]
        public void PowerOne()
        {
            var pow = new Pow(new Variable("x"), new Number(1));
            var expected = new Variable("x");

            SimpleTest(pow, expected);
        }

        [TestMethod]
        public void RootOne()
        {
            var root = new Root(new Variable("x"), new Number(1));
            var expected = new Variable("x");

            SimpleTest(root, expected);
        }

        [TestMethod]
        public void Log()
        {
            var log = new Log(new Variable("x"), new Variable("x"));
            var expected = new Number(1);

            SimpleTest(log, expected);
        }

        [TestMethod]
        public void Ln()
        {
            var ln = new Ln(new Variable("e"));
            var expected = new Number(1);

            SimpleTest(ln, expected);
        }

        [TestMethod]
        public void Lg()
        {
            var log = new Lg(new Number(10));
            var expected = new Number(1);

            SimpleTest(log, expected);
        }

    }

}
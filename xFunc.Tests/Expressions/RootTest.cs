﻿// Copyright 2012-2018 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
using System;
using System.Numerics;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.LogicalAndBitwise;
using Xunit;

namespace xFunc.Tests.Expressions
{

    public class RootTest
    {

        [Fact]
        public void CalculateRootTest1()
        {
            var exp = new Root(new Number(8), new Number(3));

            Assert.Equal(Math.Pow(8, 1.0 / 3.0), exp.Execute());
        }

        [Fact]
        public void CalculateRootTest2()
        {
            var exp = new Root(new Number(-8), new Number(3));
            Assert.Equal (Complex.Pow (new Complex (-8, 0), 1.0 / 3.0), exp.Execute ());
        }

        [Fact]
        public void CalculateComplexRootTest ()
        {
            var exp = new Processor ().Parse ("root(cos(2+3i), 3)");

            var c = new Complex (2, 3);
            var r = Complex.Cos (c);
            var p = Complex.Pow (r, 1.0 / 3.0);

            Assert.Equal (p, exp.Execute ());
        }

        [Fact]
        public void ComplexDegreeTest () {
            var exp = new Processor ().Parse ("root(cos(2+3i), 5+6i)");

            var c = new Complex (2, 3);
            var r = Complex.Cos (c);
            var d = new Complex (5, 6);

            var p = Complex.Pow (r, 1.0 / d);
            Assert.Equal (p, exp.Execute ());
        }

        [Fact]
        public void NegativeNumberExecuteTest()
        {
            var exp = new Root(new Number(-25), new Number(2));

            Assert.Equal(new Complex(0, 5), exp.Execute());
        }

        [Fact]
        public void NegativeNumberExecuteTest2()
        {
            var exp = new Root(new Number(-25), new Number(-2));

            Assert.Equal(new Complex(0, -0.2), exp.Execute());
        }

        [Fact]
        public void ExecuteExceptionTest()
        {
            var exp = new Root(new Bool(false), new Bool(false));

            Assert.Throws<ResultIsNotSupportedException>(() => exp.Execute());
        }

        [Fact]
        public void CloneTest()
        {
            var exp = new Root(Variable.X, new Number(0));
            var clone = exp.Clone();

            Assert.Equal(exp, clone);
        }

    }

}

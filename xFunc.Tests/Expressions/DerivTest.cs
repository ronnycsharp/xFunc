﻿// Copyright 2012-2017 Dmitry Kischenko
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
using Moq;
using System;
using xFunc.Maths;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Trigonometric;
using Xunit;

namespace xFunc.Tests.Expressionss
{

    public class DerivTest
    {

        [Fact]
        public void ExecutePointTest()
        {
            var differentiator = new Mock<IDifferentiator>();
            differentiator.Setup(d => d.Analyze(It.IsAny<Derivative>()))
                          .Returns<Derivative>(exp => exp.Expression);

            var deriv = new Derivative(new Variable("x"), new Variable("x"), new Number(2));
            deriv.Differentiator = differentiator.Object;

            Assert.Equal(2.0, deriv.Execute());
        }

        [Fact]
        public void ExecuteNullDerivTest()
        {
            var exp = new Derivative(new Variable("x"))
            {
                Differentiator = null,
                Simplifier = null
            };

            Assert.Throws<ArgumentNullException>(() => exp.Execute());
        }

        [Fact]
        public void ExecuteNullSimpTest()
        {
            var differentiator = new Mock<IDifferentiator>();
            differentiator.Setup(d => d.Analyze(It.IsAny<Derivative>()))
                          .Returns<Derivative>(e => e.Expression);

            var exp = new Derivative(new Variable("x"))
            {
                Differentiator = differentiator.Object,
                Simplifier = null
            };

            var result = exp.Execute();

            Assert.Equal(result, result);
        }

        [Fact]
        public void CloneTest()
        {
            var exp = new Derivative(new Sin(new Variable("x")), new Variable("x"), new Number(1));
            var clone = exp.Clone();

            Assert.Equal(exp, clone);
        }

    }

}

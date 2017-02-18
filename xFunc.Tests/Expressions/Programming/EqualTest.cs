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
using System;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;
using xFunc.Maths.Expressions.LogicalAndBitwise;
using xFunc.Maths.Expressions.Programming;
using Xunit;

namespace xFunc.Tests.Expressionss.Programming
{

    public class EqualTest
    {

        [Fact]
        public void NumberEqualTest()
        {
            var equal = new Equal(new Number(10), new Number(10));
            var result = (bool)equal.Execute();

            Assert.Equal(true, result);
        }

        [Fact]
        public void NumberVarEqualTest()
        {
            var parameters = new ParameterCollection()
            {
                new Parameter("x", 10),
                new Parameter("y", 10)
            };
            var equal = new Equal(new Variable("x"), new Variable("y"));
            var result = (bool)equal.Execute(parameters);

            Assert.Equal(true, result);
        }

        [Fact]
        public void NumberAndBoolEqualTest()
        {
            Assert.Throws<ParameterTypeMismatchException>(() => new Equal(new Number(10), new Bool(false)));
        }

        [Fact]
        public void NumberAndBoolVarEqualTest()
        {
            var parameters = new ParameterCollection()
            {
                new Parameter("x", 10),
                new Parameter("y", false)
            };
            var equal = new Equal(new Variable("x"), new Variable("y"));

            Assert.Throws<NotSupportedException>(() => (bool)equal.Execute(parameters));
        }

        [Fact]
        public void BoolTrueEqualTest()
        {
            var equal = new Equal(new Bool(true), new Bool(true));
            var result = (bool)equal.Execute();

            Assert.Equal(true, result);
        }

        [Fact]
        public void BoolTrueVarEqualTest()
        {
            var parameters = new ParameterCollection()
            {
                new Parameter("x", true),
                new Parameter("y", true)
            };
            var equal = new Equal(new Variable("x"), new Variable("y"));
            var result = (bool)equal.Execute(parameters);

            Assert.Equal(true, result);
        }

        [Fact]
        public void BoolTrueAndFalseEqualTest()
        {
            var equal = new Equal(new Bool(true), new Bool(false));
            var result = (bool)equal.Execute();

            Assert.Equal(false, result);
        }

        [Fact]
        public void BoolTrueAndFalseVarEqualTest()
        {
            var parameters = new ParameterCollection()
            {
                new Parameter("x", true),
                new Parameter("y", false)
            };
            var equal = new Equal(new Variable("x"), new Variable("y"));
            var result = (bool)equal.Execute(parameters);

            Assert.Equal(false, result);
        }

        [Fact]
        public void BoolFalseEqualTest()
        {
            var equal = new Equal(new Bool(false), new Bool(false));
            var result = (bool)equal.Execute();

            Assert.Equal(true, result);
        }

        [Fact]
        public void BoolFalseVarEqualTest()
        {
            var parameters = new ParameterCollection()
            {
                new Parameter("x", false),
                new Parameter("y", false)
            };
            var equal = new Equal(new Variable("x"), new Variable("y"));
            var result = (bool)equal.Execute(parameters);

            Assert.Equal(true, result);
        }

        [Fact]
        public void CloneTest()
        {
            var exp = new Equal(new Number(2), new Number(3));
            var clone = exp.Clone();

            Assert.Equal(exp, clone);
        }

    }

}

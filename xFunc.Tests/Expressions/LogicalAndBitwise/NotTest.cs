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
using xFunc.Maths.Expressions.LogicalAndBitwise;
using Xunit;

namespace xFunc.Tests.Expressionss.LogicalAndBitwise
{

    public class NotTest
    {

        [Fact]
        public void ExecuteTest1()
        {
            var exp = new Not(new Number(2));

            Assert.Equal(-3.0, exp.Execute());
        }

        [Fact]
        public void ExecuteTest2()
        {
            var exp = new Not(new Number(2.5));

            Assert.Equal(-4.0, exp.Execute());
        }

        [Fact]
        public void ExecuteTest3()
        {
            var exp = new Not(new Bool(true));

            Assert.Equal(false, exp.Execute());
        }

        [Fact]
        public void CloneTest()
        {
            var exp = new Not(new Bool(false));
            var clone = exp.Clone();

            Assert.Equal(exp, clone);
        }

    }

}

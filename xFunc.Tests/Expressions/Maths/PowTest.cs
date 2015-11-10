﻿// Copyright 2012-2015 Dmitry Kischenko
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
using Xunit;

namespace xFunc.Tests.Expressions.Maths
{
    
    public class PowTest
    {

        [Fact]
        public void CalculateTest()
        {
            IExpression exp = new Pow(new Number(2), new Number(10));

            Assert.Equal(1024.0, exp.Calculate());
        }

        [Fact]
        public void NegativeCalculateTest()
        {
            IExpression exp = new Pow(new Number(-8), new Number(1 / 3.0));

            Assert.Equal(-2.0, exp.Calculate());
        }

        [Fact]
        public void NegativeNumberCalculateTest()
        {
            var exp = new Pow(new Number(-25), new Number(1 / 2.0));

            Assert.Equal(double.NaN, exp.Calculate());
        }

    }

}
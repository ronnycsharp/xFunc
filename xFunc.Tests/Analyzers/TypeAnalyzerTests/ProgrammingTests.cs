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
using xFunc.Maths.Analyzers.TypeAnalyzers;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.ComplexNumbers;
using xFunc.Maths.Expressions.LogicalAndBitwise;
using xFunc.Maths.Expressions.Programming;
using Xunit;

namespace xFunc.Tests.Analyzers.TypeAnalyzerTests
{

    public class ProgrammingTests : TypeAnalyzerBaseTests
    {

        [Fact]
        public void TestAddAssignUndefined()
        {
            var exp = new AddAssign(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestAddAssignNumber()
        {
            var exp = new AddAssign(new Variable("x"), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestAddAssignException()
        {
            var exp = new AddAssign(new Variable("x"), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestConditionalAndUndefined()
        {
            var exp = new Maths.Expressions.Programming.And(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestConditionalAndBool()
        {
            var exp = new Maths.Expressions.Programming.And(new Bool(false), new Bool(true));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestConditionalAndBoolNumberExpection()
        {
            var exp = new Maths.Expressions.Programming.And(new Bool(false), new Number(1));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestConditionalAndNumberBoolExpection()
        {
            var exp = new Maths.Expressions.Programming.And(new Number(1), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestConditionalAndException()
        {
            var exp = new Maths.Expressions.Programming.And(new ComplexNumber(2, 3), new Bool(false));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestDecNumber()
        {
            var exp = new Dec(new Number(3));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestDivAssignUndefined()
        {
            var exp = new DivAssign(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestDivAssignNumber()
        {
            var exp = new DivAssign(new Variable("x"), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestDivAssignException()
        {
            var exp = new DivAssign(new Variable("x"), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestEqualUndefined()
        {
            var exp = new Equal(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestEqualNumber()
        {
            var exp = new Equal(new Number(20), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestEqualNumberBoolException()
        {
            var exp = new Equal(new Number(20), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestEqualBoolNumberException()
        {
            var exp = new Equal(new Bool(true), new Number(20));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestEqualBoolean()
        {
            var exp = new Equal(new Bool(false), new Bool(true));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestEqualBoolComplexException()
        {
            var exp = new Equal(new Bool(false), new ComplexNumber(1, 2));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestEqualComplexBoolException()
        {
            var exp = new Equal(new ComplexNumber(1, 2), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestEqualException()
        {
            var exp = new Equal(new ComplexNumber(2, 3), new Bool(false));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestNotEqualUndefined()
        {
            var exp = new NotEqual(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestNotEqualNumber()
        {
            var exp = new NotEqual(new Number(20), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestNotEqualNumberBoolException()
        {
            var exp = new NotEqual(new Number(20), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestNotEqualBoolNumberException()
        {
            var exp = new NotEqual(new Bool(true), new Number(20));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestNotEqualBoolean()
        {
            var exp = new NotEqual(new Bool(false), new Bool(true));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestNotEqualBoolComplexException()
        {
            var exp = new NotEqual(new Bool(false), new ComplexNumber(1, 1));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestNotEqualComplexBoolException()
        {
            var exp = new NotEqual(new ComplexNumber(1, 1), new Bool(false));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestNotEqualException()
        {
            var exp = new NotEqual(new ComplexNumber(2, 3), new Bool(false));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestForUndefined()
        {
            var exp = new For(null, null, new Variable("x"), null);

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestForNumber()
        {
            var exp = new For(null, null, new Bool(false), null);

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestForException()
        {
            var exp = new For(null, null, new ComplexNumber(2, 3), null);

            TestException(exp);
        }

        [Fact]
        public void TestGreaterOrEqualUndefined()
        {
            var exp = new GreaterOrEqual(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestGreaterOrEqualNumber()
        {
            var exp = new GreaterOrEqual(new Number(10), new Number(10));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestGreaterOrEqualNumberBoolException()
        {
            var exp = new GreaterOrEqual(new Number(10), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestGreaterOrEqualBoolNumberException()
        {
            var exp = new GreaterOrEqual(new Bool(true), new Number(10));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestGreaterOrEqualException()
        {
            var exp = new GreaterOrEqual(new Bool(true), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestGreaterThanUndefined()
        {
            var exp = new GreaterThan(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestGreaterThanNumber()
        {
            var exp = new GreaterThan(new Number(10), new Number(10));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestGreaterThanNumberBoolException()
        {
            var exp = new GreaterThan(new Number(10), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestGreaterThanBoolNumberException()
        {
            var exp = new GreaterThan(new Bool(true), new Number(10));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestGreaterThanException()
        {
            var exp = new GreaterThan(new Bool(true), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestIfUndefined()
        {
            var exp = new If(new Variable("x"), new Number(10));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestIfBool()
        {
            var exp = new If(new Bool(false), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestIfElseBool()
        {
            var exp = new If(new Bool(false), new Number(10), new Number(2));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestIfException()
        {
            var exp = new If(new ComplexNumber(2, 4), new Number(10));

            TestException(exp);
        }

        [Fact]
        public void TestIncNumber()
        {
            var exp = new Inc(new Number(3));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestLessOrEqualUndefined()
        {
            var exp = new LessOrEqual(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestLessOrEqualNumber()
        {
            var exp = new LessOrEqual(new Number(10), new Number(10));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestLessOrEqualNumberBoolException()
        {
            var exp = new LessOrEqual(new Number(10), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestLessOrEqualBoolNumberException()
        {
            var exp = new LessOrEqual(new Bool(true), new Number(10));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestLessOrEqualException()
        {
            var exp = new LessOrEqual(new Bool(true), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestLessThanUndefined()
        {
            var exp = new LessThan(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestLessThanNumber()
        {
            var exp = new LessThan(new Number(10), new Number(10));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestLessThanNumberBoolException()
        {
            var exp = new LessThan(new Number(10), new Bool(true));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestLessThanBoolNumberException()
        {
            var exp = new LessThan(new Bool(true), new Number(10));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestLessThanException()
        {
            var exp = new LessThan(new Bool(true), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestMulAssignUndefined()
        {
            var exp = new MulAssign(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestMulAssignNumber()
        {
            var exp = new MulAssign(new Variable("x"), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestMulAssignException()
        {
            var exp = new MulAssign(new Variable("x"), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestConditionalOrUndefined()
        {
            var exp = new Maths.Expressions.Programming.Or(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestConditionalOrBool()
        {
            var exp = new Maths.Expressions.Programming.Or(new Bool(false), new Bool(true));

            Test(exp, ResultType.Boolean);
        }

        [Fact]
        public void TestConditionalOrBoolNumberException()
        {
            var exp = new Maths.Expressions.Programming.Or(new Bool(false), new Number(2));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestConditionalOrException()
        {
            var exp = new Maths.Expressions.Programming.Or(new ComplexNumber(2, 3), new Bool(false));

            TestBinaryException(exp);
        }

        [Fact]
        public void TestSubAssignUndefined()
        {
            var exp = new SubAssign(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestSubAssignNumber()
        {
            var exp = new SubAssign(new Variable("x"), new Number(10));

            Test(exp, ResultType.Number);
        }

        [Fact]
        public void TestSubAssignException()
        {
            var exp = new SubAssign(new Variable("x"), new Bool(false));

            TestException(exp);
        }

        [Fact]
        public void TestWhileUndefined()
        {
            var exp = new While(new Variable("x"), new Variable("x"));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestWhileNumber()
        {
            var exp = new While(new Variable("x"), new Bool(false));

            Test(exp, ResultType.Undefined);
        }

        [Fact]
        public void TestWhileException()
        {
            var exp = new While(new Variable("x"), new Number(1));

            TestException(exp);
        }

    }

}

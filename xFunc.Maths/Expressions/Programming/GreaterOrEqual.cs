﻿// Copyright 2012-2014 Dmitry Kischenko
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

namespace xFunc.Maths.Expressions.Programming
{

    public class GreaterOrEqual : BinaryExpression
    {

        internal GreaterOrEqual() { }

        public GreaterOrEqual(IExpression left, IExpression right)
            : base(left, right) { }

        public override string ToString()
        {
            if (parent is BinaryExpression)
                return ToString("({0} >= {1})");

            return ToString("{0} >= {1}");
        }

        public override object Calculate(ExpressionParameters parameters)
        {
            var leftValue = (double)left.Calculate(parameters);
            var rightValue = (double)right.Calculate(parameters);

            return (leftValue >= rightValue).AsNumber();
        }

        public override IExpression Clone()
        {
            return new GreaterOrEqual(left.Clone(), right.Clone());
        }

        public override IExpression Differentiate(Variable variable)
        {
            throw new NotSupportedException();
        }

    }

}
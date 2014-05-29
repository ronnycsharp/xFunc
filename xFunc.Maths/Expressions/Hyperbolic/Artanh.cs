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

namespace xFunc.Maths.Expressions.Hyperbolic
{

    /// <summary>
    /// Represents the Artanh function.
    /// </summary>
    [ReverseFunction(typeof(Tanh))]
    public class Artanh : HyperbolicExpression
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Artanh"/> class.
        /// </summary>
        internal Artanh() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Artanh"/> class.
        /// </summary>
        /// <param name="firstMathExpression">The argument of function.</param>
        public Artanh(IExpression firstMathExpression)
            : base(firstMathExpression)
        {

        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode(4073);
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString()
        {
            return ToString("artanh({0})");
        }

        /// <summary>
        /// Calculates this mathemarical expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Calculate(ExpressionParameters parameters)
        {
            return MathExtentions.Atanh((double)argument.Calculate(parameters));
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone()
        {
            return new Artanh(argument.Clone());
        }

        /// <summary>
        /// Calculates a derivative of the expression.
        /// </summary>
        /// <param name="variable">The variable of differentiation.</param>
        /// <returns>
        /// Returns a derivative of the expression of several variables.
        /// </returns>
        /// <seealso cref="Variable" />
        protected override IExpression _Differentiation(Variable variable)
        {
            var sqr = new Pow(argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), sqr);
            var div = new Div(argument.Clone().Differentiate(variable), sub);

            return div;
        }

    }

}

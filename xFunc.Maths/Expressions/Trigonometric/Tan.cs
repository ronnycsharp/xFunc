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

namespace xFunc.Maths.Expressions.Trigonometric
{

    /// <summary>
    /// Represents the Tangent function.
    /// </summary>
    [ReverseFunction(typeof(Arctan))]
    public class Tan : TrigonometricExpression
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Tan"/> class.
        /// </summary>
        internal Tan() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tan"/> class.
        /// </summary>
        /// <param name="firstMathExpression">The argument of function.</param>
        public Tan(IExpression firstMathExpression) : base(firstMathExpression) { }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode(3089);
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString()
        {
            return ToString("tan({0})");
        }

        /// <summary>
        /// Calculates this mathemarical expression (using degree).
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        protected override double CalculateDergee(ExpressionParameters parameters)
        {
            var radian = (double)argument.Calculate(parameters) * Math.PI / 180;

            return Math.Tan(radian);
        }

        /// <summary>
        /// Calculates this mathemarical expression (using radian).
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        protected override double CalculateRadian(ExpressionParameters parameters)
        {
            return Math.Tan((double)argument.Calculate(parameters));
        }

        /// <summary>
        /// Calculates this mathemarical expression (using gradian).
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        protected override double CalculateGradian(ExpressionParameters parameters)
        {
            var radian = (double)argument.Calculate(parameters) * Math.PI / 200;

            return Math.Tan(radian);
        }
			
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone()
        {
            return new Tan(argument.Clone());
        }

    }

}

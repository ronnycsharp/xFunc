﻿// Copyright 2012-2016 Dmitry Kischenko
// Ronny Weidemann - June 2014, Added Vector-Calculation
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
using System.Linq;
using xFunc.Maths.Expressions.Matrices;

namespace xFunc.Maths.Expressions
{
    /// <summary>
    /// Represents the Absolute operation.
    /// </summary>
    public class Abs : UnaryExpression
    {
        internal Abs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Abs"/> class.
        /// </summary>
        /// <param name="expression">The argument of function.</param>
        /// <seealso cref="IExpression"/>
        public Abs(IExpression expression) : base(expression) { }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return base.GetHashCode(6329);
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString() {
            return ToString("abs({0})");
        }

		/// <summary>
		/// Executes this Absolute expression.
		/// </summary>
		/// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
		/// <returns>
		/// A result of the execution.
		/// </returns>
		/// <seealso cref="ExpressionParameters" />
		public override object Execute (ExpressionParameters parameters) {
			if (m_argument is Vector) {
				return this.AbsVector ((Vector)m_argument, parameters);
			} else {
				var result = m_argument.Execute (parameters);
				if (result is Complex)
					return Complex.Abs ((Complex)result);

				return Math.Abs ((double)result);
			}
		}

        double AbsVector (Vector vec, ExpressionParameters parameters) {
            return Math.Sqrt(
                vec.Arguments.Sum ( a => Math.Pow ( 
                   ( double ) a.Execute ( parameters ), 2 ) ) );
        }

        /// <summary>
        /// Clones this instance of the <see cref="xFunc.Maths.Expressions.Abs"/> class.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone() {
            return new Abs(m_argument.Clone());
        }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        /// <value>
        /// The type of the argument.
        /// </value>
        public override ExpressionResultType ArgumentType {
            get {
                return ExpressionResultType.Number | ExpressionResultType.ComplexNumber;
            }
        }
    }
}
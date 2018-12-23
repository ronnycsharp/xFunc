﻿// Copyright 2014-2016 Ronny Weidemann
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
using xFunc.Maths.Analyzers;

namespace xFunc.Maths.Expressions {
    /// <summary>
    /// Represents the "round" function.
    /// </summary>
    public class RoundUnary : UnaryExpression {
        internal RoundUnary() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundUnary"/> class.
        /// </summary>
        /// <param name="argument">The expression that represents a double-precision floating-point number to be rounded.</param>
        public RoundUnary(IExpression argument) :
            base ( argument ) { }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return base.GetHashCode(5680);
        }


		/*
        public override string ToString() {
            return ToString("roundunary({0})");
        } */

        /// <summary>
        /// Calculates this mathemarical expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Execute(ExpressionParameters parameters) {
			return Math.Round ((double)this.Argument.Execute(parameters));
        }

        /// <summary>
        /// Clones this instance of the <see cref="IExpression" />.
        /// </summary>
        /// <returns>
        /// Returns the new instance of <see cref="IExpression" /> that is a clone of this instance.
        /// </returns>
        public override IExpression Clone() {
            return new RoundUnary(this.Argument.Clone ( ));
        }

		public override TResult Analyze<TResult> (IAnalyzer<TResult> analyzer) {
			return analyzer.Analyze (this);
		}
	}
}

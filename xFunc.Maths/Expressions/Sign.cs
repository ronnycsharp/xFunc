// Copyright 2013-2018 Dmitry Kischenko & Ronny Weidemann
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
using System.Diagnostics.CodeAnalysis;
using xFunc.Maths.Analyzers;

namespace xFunc.Maths.Expressions {

    /// <summary>
    /// Represents the sign operation.
    /// </summary>
    public class Sign : UnaryExpression {

        [ExcludeFromCodeCoverage]
        internal Sign() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sign"/> class.
        /// </summary>
        /// <param name="argument">The argument of sign.</param>
        public Sign(IExpression argument) : base(argument) { }

        /// <summary>
        /// Executes this expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the execution.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Execute(ExpressionParameters parameters) {
            return (double)Math.Sign(
                (double)this.Argument.Execute(parameters));
        }

        public override TResult Analyze<TResult>(IAnalyzer<TResult> analyzer) {
            return analyzer.Analyze(this);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone() {
            return new Sign(m_argument.Clone());
        }

    }

}

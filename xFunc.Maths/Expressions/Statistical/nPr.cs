// Copyright 2012-2017 Dmitry Kischenko
// Copyright 2013-2018 Ronny Weidemann
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

namespace xFunc.Maths.Expressions.Statistical {
    /// <summary>
    /// Represent the nPr function.
    /// </summary>
    /// <seealso cref="xFunc.Maths.Expressions.BinaryExpression" />
    public class nPr : BinaryExpression {

        [ExcludeFromCodeCoverage]
        internal nPr () { }

        /// <summary>
        /// Initializes a new instance of the <see cref="nCr"/> class.
        /// </summary>
        public nPr (IExpression n, IExpression r) : base (n, r) { }

        /// <summary>
        /// Executes this expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the execution.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Execute (ExpressionParameters parameters) {
            var n = (int)(double) this.Left.Execute (parameters);
            var r = (int)(double) this.Right.Execute (parameters);
            return (double)PermutationsAndCombinations.nPr (n, r);
        }

        /// <summary>
        /// Analyzes the current expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>
        /// The analysis result.
        /// </returns>
        public override TResult Analyze<TResult>(IAnalyzer<TResult> analyzer) {
            return analyzer.Analyze(this);
        }

        /// <summary>
        /// Clones this instance of the <see cref="IExpression" />.
        /// </summary>
        /// <returns>
        /// Returns the new instance of <see cref="IExpression" /> that is a clone of this instance.
        /// </returns>
        public override IExpression Clone() {
            return new nPr (this.Left.Clone (), this.Right.Clone ());
        }
    }
}

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
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Expressions.Programming;

namespace xFunc.Maths.Expressions {
    /// <summary>
    /// Represents the "solve" function.
    /// </summary>
    public class Solve : DifferentParametersExpression {

        [ExcludeFromCodeCoverage]
        internal Solve() : base(null, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Solve"/> class.
        /// </summary>
        /// <param name="argument">The expression which should be solved.</param>
        /// <param name="variable"></param>
        public Solve(IExpression argument, IExpression variable) : this(new[] { argument, variable }, 2) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Solve"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        /// <exception cref="System.ArgumentNullException">args</exception>
        /// <exception cref="System.ArgumentException">The length of <paramref name="args"/> is not equal to <paramref name="countOfParams"/>.</exception>
        public Solve(IExpression[] args, int countOfParams)
            : base(args, countOfParams) {

            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.Length != countOfParams)
                throw new ArgumentException();
        }

        /// <summary>
        /// Executes this expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the execution.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Execute(ExpressionParameters parameters) {
            if (!(this.Argument is Equal))
                throw new InvalidOperationException();

            var equal = (Equal)this.Argument;
            var solver = new EquationSolver(
                equal, this.Variable.ToString(), parameters.AngleMeasurement);

            return solver
                .Solve(-50, 50)
                .Select(p => p.X)
                .ToArray();
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
            return new Solve(CloneArguments(), ParametersCount);
        }

        /// <summary>
        /// Gets the minimum count of parameters.
        /// </summary>
        /// <value>
        /// The minimum count of parameters.
        /// </value>
        public override int MinParameters => 2;

        /// <summary>
        /// Gets the maximum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The maximum count of parameters.
        /// </value>
        public override int MaxParameters => 4;

        /// <summary>
        /// The expression that represents a double-precision floating-point number to be rounded.
        /// </summary>
        /// <value>
        /// The expression that represents a double-precision floating-point number to be rounded.
        /// </value>
        public IExpression Argument => m_arguments[0];

        /// <summary>
        /// 
        /// </summary>
        public IExpression Variable => m_arguments[1];

    }
}
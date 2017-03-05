// Copyright 2012-2017 Dmitry Kischenko
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
using System.Linq;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Expressions.Collections;
using xFunc.Maths.Expressions.Matrices;

namespace xFunc.Maths.Expressions.Statistical
{

    /// <summary>
    /// Represents the "sum" function.
    /// </summary>
    public class Sum : DifferentParametersExpression
    {

        [ExcludeFromCodeCoverage]
        internal Sum() : base(null, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sum"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="args"/> is not equal to <paramref name="countOfParams"/> or last parameter is not variable.</exception>
        public Sum(IExpression[] args, int countOfParams)
            : base(args, countOfParams)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (args.Length != countOfParams)
                throw new ArgumentException();
            if (countOfParams == 5 && !(args[4] is Variable))
                throw new ArgumentException();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Sum"/> class.
		/// </summary>
		/// <param name="body">The function that is executed on each iteration.</param>
		/// <param name="from">The initial value (including).</param>
		/// <param name="to">The final value (including).</param>
		/// <param name="inc">The increment.</param>
		/// <param name="variable">The increment variable.</param>
		public Sum (IExpression body, IExpression from, IExpression to, IExpression inc, Variable variable)
			: base (new [] { body, from, to, inc, variable }, 5)
		{
		}

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return base.GetHashCode(6089, 9949);
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
			return this.Calculate (parameters); // this.m_arguments.Sum(exp => (double)exp.Execute(parameters));
        }

		// added calculate-method from v3.0
		// it's needed for bug-fix the newer execute-method

		private object Calculate (ExpressionParameters parameters) {
			var body = this.Body;
			var from = (double)(this.From?.Execute (parameters) ?? 1.0);
			var to = (double)this.To.Execute (parameters);
			var inc = (double)(this.Increment.Execute (parameters) ?? 1.0);

			var localParams = new ParameterCollection (parameters.Variables.Collection);

			var variable = Variable != null ? this.Variable.Name : GetVarName (localParams);
			localParams.Add (variable, from);

			var param = new ExpressionParameters (
				parameters.AngleMeasurement, 
				localParams, 
				parameters.Functions);

			double S = 0;
			for (; from <= to; from += inc) {
				localParams [variable] = from;
				S += (double)body.Execute (param);
			}
			return S;
		}

		private static string GetVarName (ParameterCollection parameters) {
			const string variable = "n";
			if (!parameters.ContainsKey (variable))
				return variable;

			for (int i = 1; ; i++) {
				var localVar = variable + i;
				if (!parameters.ContainsKey (localVar))
					return localVar;
			}
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
        public override IExpression Clone()
        {
            return new Sum(CloneArguments(), ParametersCount);
        }

        /// <summary>
        /// Gets the minimum count of parameters.
        /// </summary>
        /// <value>
        /// The minimum count of parameters.
        /// </value>
        public override int MinParameters => 1;

        /// <summary>
        /// Gets the maximum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The maximum count of parameters.
        /// </value>
        public override int MaxParameters => -1;



		/// <summary>
		/// Gets the function that is executed on each iteration.
		/// </summary>
		/// <value>
		/// The function that is executed on each iteration.
		/// </value>
		public IExpression Body {
			get {
				return m_arguments [0];
			}
		}

		/// <summary>
		/// Gets ghe initial value (including).
		/// </summary>
		/// <value>
		/// The initial value (including).
		/// </value>
		public IExpression From {
			get {
				return ParametersCount >= 3 ? m_arguments [1] : null;
			}
		}

		/// <summary>
		/// Gets the final value (including).
		/// </summary>
		/// <value>
		/// The final value (including).
		/// </value>
		public IExpression To {
			get {
				return ParametersCount == 2 ? m_arguments [1] : m_arguments [2];
			}
		}

		/// <summary>
		/// Gets the increment.
		/// </summary>
		/// <value>
		/// The increment.
		/// </value>
		public IExpression Increment {
			get {
				return ParametersCount >= 4 ? m_arguments [3] : null;
			}
		}

		/// <summary>
		/// Gets the increment variable.
		/// </summary>
		/// <value>
		/// The increment variable.
		/// </value>
		public Variable Variable {
			get {
				return ParametersCount == 5 ? (Variable)m_arguments [4] : null;
			}
		}


        /// <summary>
        /// Gets the arguments types.
        /// </summary>
        /// <value>
        /// The arguments types.
        /// </value>
        public override ExpressionResultType[] ArgumentsTypes
        {
            get
            {
                var result = new ExpressionResultType[ParametersCount];
                if (ParametersCount > 0)
                {
                    result[0] = ExpressionResultType.Number | ExpressionResultType.Vector;
                    for (var i = 1; i < result.Length; i++)
                        result[i] = ExpressionResultType.Number;
                }

                return result;
            }
        }

    }

}

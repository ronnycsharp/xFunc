// Copyright 2012-2017 Ronny Weidemann
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
using xFunc.Maths.Resources;

namespace xFunc.Maths.Expressions {
	/// <summary>
	/// Represents the Nth-Deriv function.
	/// </summary>
	public class NDerivative : DifferentParametersExpression {

		[ExcludeFromCodeCoverage]
		internal NDerivative () : base (null, -1) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="NDerivative" /> class.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="countOfParams">The count of parameters.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="args"/> is null.</exception>
		public NDerivative (IExpression [] args, int countOfParams)
			: base (args, countOfParams) {
			if (args == null)
				throw new ArgumentNullException (nameof (args));
			if (args.Length != countOfParams)
				throw new ArgumentException ();
			/*
			if (countOfParams == 2 && ( !(args[1] is Number) || !(args [2] is Variable)))
				throw new ArgumentException (Resource.InvalidExpression);
				*/
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NDerivative" /> class.
		/// </summary>
		public NDerivative (IExpression expression, IExpression number) 
			: base (new [] { expression, number }, 2) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="NDerivative" /> class.
		/// </summary>
		public NDerivative (IExpression expression, IExpression number, Variable variable) 
			: base (new [] { expression, number, variable }, 3) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="NDerivative" /> class.
		/// </summary>
		public NDerivative (IExpression expression, IExpression number, Variable variable, Number point) 
			: base (new [] { expression, number, variable, point }, 4) { }

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode () {
			return base.GetHashCode (591, 1253);
		}

		/// <summary>
		/// Executes this expression.
		/// </summary>
		/// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
		/// <returns>
		/// Returns the n-th derivation of the expression
		/// </returns>
		/// <seealso cref="ExpressionParameters" />
		public override object Execute (ExpressionParameters parameters) {
			if (Differentiator == null)
				throw new ArgumentNullException (nameof (Differentiator));

			var variable = this.Variable;

			this.Differentiator.Variable = variable;
			this.Differentiator.Parameters = parameters;

			var exp = this.Expression;

			// iterate number of derivations
			for (var n = 0; n < (int)((Number)this.Number).Value; n++) {
				exp = exp.Analyze (this.Differentiator);
			}

			var point = this.DerivativePoint;
			if (variable != null && point != null) {
				if (parameters == null)
					parameters = new ExpressionParameters ();

				parameters.Variables [variable.Name] = point.Value;
				return exp.Execute (parameters);
			}

			return exp.Execute (parameters);

			/*
			return exp.Analyze (this.Simplifier);
			*/
		}

		/// <summary>
		/// Analyzes the current expression.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="analyzer">The analyzer.</param>
		/// <returns>
		/// The analysis result.
		/// </returns>
		public override TResult Analyze<TResult> (IAnalyzer<TResult> analyzer) {
			return analyzer.Analyze (this);
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
		public override IExpression Clone () {
			return new NDerivative (CloneArguments (), ParametersCount);
		}

		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		/// <value>
		/// The expression.
		/// </value>
		public IExpression Expression {
			get {
				return m_arguments [0];
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				m_arguments [0] = value;
				m_arguments [0].Parent = this;
			}
		}

		/// <summary>
		/// Gets or sets the number of derivation
		/// </summary>
		/// <value>
		/// The variable.
		/// </value>
		public IExpression Number {
			get { return m_arguments [1]; }
			set {
				m_arguments [1] = value;
			}
		}

		/// <summary>
		/// Gets or sets the variable.
		/// </summary>
		/// <value>
		/// The variable.
		/// </value>
		public Variable Variable => ParametersCount >= 3 ? (Variable)m_arguments [2] : new Variable ("x");

		/// <summary>
		/// Gets the derivative point.
		/// </summary>
		/// <value>
		/// The derivative point.
		/// </value>
		public Number DerivativePoint => ParametersCount >= 4 ? (Number)m_arguments [3] : null;

		/// <summary>
		/// Gets or sets the arguments.
		/// </summary>
		/// <value>
		/// The arguments.
		/// </value>
		public override IExpression [] Arguments {
			get {
				return base.Arguments;
			}
			set {
				/*
				if (value != null &&
					((value.Length >= 2 && !(value [1] is Number)) ||
				     (value.Length >= 3 && !(value [2] is Variable)) ||
					 (value.Length >= 4 && !(value [3] is Number))))
					throw new ArgumentException (Resource.InvalidExpression);
				*/

				base.Arguments = value;
			}
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
		public override int MaxParameters => 4;

		/// <summary>
		/// Gets or sets the simplifier.
		/// </summary>
		/// <value>
		/// The simplifier.
		/// </value>
		public ISimplifier Simplifier { get; set; }

		/// <summary>
		/// Gets or sets the differentiator.
		/// </summary>
		/// <value>
		/// The differentiator.
		/// </value>
		public IDifferentiator Differentiator { get; set; }

	}

}


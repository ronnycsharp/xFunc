// 2014-2016 Ronny Weidemann

using System;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Expressions.Collections;

namespace xFunc.Maths.Expressions {
	public class DefiniteIntegral : DifferentParametersExpression {
		internal DefiniteIntegral ()
            : base (null, -1) {
		}

		public DefiniteIntegral (IExpression[] args, int countOfParams)
            : base (args, countOfParams) {
			if (args == null)
				throw new ArgumentNullException (nameof(args));

			if (args.Length != countOfParams)
				throw new ArgumentException ();
		}

		public DefiniteIntegral (
			IExpression expression, 
			Variable variable, 
			IExpression left, 
			IExpression right)
            : base (new[] { expression, variable, left, right }, 4) {
		}

		/// <summary>
		/// Converts this expression to the equivalent string.
		/// </summary>
		/// <returns>The string that represents this expression.</returns>
		public override string ToString () {
			return String.Format (
				"integral({0}, {1}, {2}, {3})", 
				this.Body, this.Variable, this.Left, this.Right);
		}

		/// <summary>
		/// Calculates this mathemarical expression.
		/// </summary>
		/// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
		/// <returns>
		/// A result of the calculation.
		/// </returns>
		/// <seealso cref="ExpressionParameters" />
		public override object Execute (ExpressionParameters parameters) {
			return Simpson (parameters);
		}

		/// <summary>
		/// calculates a rectangle part of the expression
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		double Rectangle (ExpressionParameters parameters) {
			if (parameters == null)
				parameters = new ExpressionParameters ();
				
			var result = 0.0M;
			var left = (decimal)(double)this.Left.Execute (parameters);
			var right = (decimal)(double)this.Right.Execute (parameters);

			if (right <= left)
				throw new InvalidOperationException ("Invalid Intergral-Bounds");

			var n = 1000;    // number of rectangles
			var dx = (right - left) / (decimal)n;

			var param = default(Parameter);
			foreach (var p in parameters.Variables.Collection) {
				if (p.Key == this.Variable.Name) {
					param = p;
					break;
				}
			}
			if (param == null) {
				param = new Parameter (
					this.Variable.Name, 0, ParameterType.Normal);

				parameters.Variables.Add (param);
			}
			for (var x = left; x <= right; x += dx) {
				param.Value = (double)x;
				var z = (double)this.Body.Execute (parameters);
				result += (dx * (decimal)z);
			}
			return (double)result;
		}

		/// <summary>
		/// calculates a part of the integral-result with the simpson-algorithm
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		double Simpson (ExpressionParameters parameters) {
			if (parameters == null)
				parameters = new ExpressionParameters ();

			var a = (double)this.Left.Execute (parameters);
			var b = (double)this.Right.Execute (parameters);

			if (b <= a)
				throw new InvalidOperationException ("Invalid Intergral-Bounds");

			var n = 10000;    // number of rectangles
			var h = (b - a) / (double)n;

			var param = default(Parameter);
			foreach (var p in parameters.Variables.Collection) {
				if (p.Key == this.Variable.Name) {
					param = p;
					break;
				}
			}
			if (param == null) {
				param = new Parameter (
					this.Variable.Name, 0, ParameterType.Normal);

				parameters.Variables.Add (param);
			}

			var sum = 0.0;
			for (var i = 1; i <= n - 3; i = i + 2) {
				param.Value = a + i * h;
				sum += (double)this.Body.Execute (parameters);
			}

			param.Value = a + (n - 1) * h;
			sum += (double)this.Body.Execute (parameters);
			sum *= 4;

			var sum2 = 0.0;
			for (var i = 2; i <= n - 4; i += 2) {
				param.Value = a + i * h;
				sum2 += (double)this.Body.Execute (parameters);
			}

			param.Value = a + (n - 2) * h;
			sum2 += (double)this.Body.Execute (parameters);
			sum2 *= 2;

			sum += sum2;
			param.Value = a;
			sum += (double)this.Body.Execute (parameters);

			param.Value = b;
			sum += (double)this.Body.Execute (parameters);
            
			return Math.Round ( h / 3 * sum, 5 );
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
		public override IExpression Clone () {
			return new DefiniteIntegral (
				CloneArguments (), 4);
		}

		public override TResult Analyze<TResult> (IAnalyzer<TResult> analyzer) {
			return analyzer.Analyze (this);
		}

		#region Properties

		/// <summary>
		/// Gets or sets the expression to integrate.
		/// </summary>
		/// <value>
		/// The expression.
		/// </value>
		public IExpression Body {
			get {
				return m_arguments [0];
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof(Body));

				m_arguments [0] = value;
				m_arguments [0].Parent = this;
			}
		}

		/// <summary>
		/// Gets or sets the integration-variable.
		/// </summary>
		/// <value>
		/// The variable.
		/// </value>
		public Variable Variable {
			get {
				return (Variable) m_arguments [1];
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof(Variable));

				m_arguments [1] = value;
				m_arguments [1].Parent = this;
			}
		}

		public IExpression Left {
			get {
				return m_arguments [2];
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof(Left));

				m_arguments [2] = value;
				m_arguments [2].Parent = this;
			}
		}

		public IExpression Right {
			get {
				return m_arguments [3];
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof(Right));

				m_arguments [3] = value;
				m_arguments [3].Parent = this;
			}
		}

		/// <summary>
		/// Gets the minimum count of parameters.
		/// </summary>
		/// <value>
		/// The minimum count of parameters.
		/// </value>
		public override int MinParameters {
			get {
				return 4;
			}
		}

		/// <summary>
		/// Gets the maximum count of parameters. -1 - Infinity.
		/// </summary>
		/// <value>
		/// The maximum count of parameters.
		/// </value>
		public override int MaxParameters {
			get {
				return 4;
			}
		}

		#endregion
	}
}

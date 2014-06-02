// 2014 Ronny Weidemann

using System;
using xFunc.Maths.Resources;
using xFunc.Maths.Expressions.Collections;

namespace xFunc.Maths.Expressions {
    public class DefiniteIntegral : DifferentParametersExpression {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefiniteIntegral"/> class.
        /// </summary>
        internal DefiniteIntegral ()
            : base(null, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefiniteIntegral"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        public DefiniteIntegral(IExpression[] args, int countOfParams)
            : base(args, countOfParams) {
            if (args == null)
                throw new ArgumentNullException("args");

            if (args.Length != countOfParams)
                throw new ArgumentException();
        }

        public DefiniteIntegral (
            IExpression expression, 
            Variable variable, 
            IExpression left, 
            IExpression right )
            : base(new[] { expression, variable, left, right }, 4) { }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString() {
            return String.Format(
                "integral({0}, {1}, {2}, {3})", Body, Variable, Left, Right );
        }

        /// <summary>
        /// Calculates this mathemarical expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Calculate(ExpressionParameters parameters) {
            return Simpson ( parameters );
        }

        double Rectangle ( ExpressionParameters parameters ) {
            if (parameters == null)
                parameters = new ExpressionParameters();

            parameters.AngleMeasurement = AngleMeasurement.Radian;

            var result  = 0.0M;
            var left    = ( decimal ) ( double ) this.Left.Calculate(parameters);
            var right   = ( decimal ) ( double ) this.Right.Calculate(parameters);

            if ( right <= left )
                throw new InvalidOperationException ( "Invalid Intergral-Bounds" );

            var n       = 1000;    // number of rectangles
            var dx      = (right - left) / (decimal)n;

            var param = default(Parameter);
            foreach (var p in parameters.Parameters.Collection) {
                if ( p.Key == this.Variable.Name ) {
                    param = p;
                    break;
                }
            }
            if (param == null) {
                param = new Parameter(
                    this.Variable.Name, 0, ParameterType.Normal);

                parameters.Parameters.Add(param);
            }
            for ( var x = left; x <= right; x+=dx ) {
                param.Value = (double)x;
                var z = (double)this.Body.Calculate(parameters);
                result += (dx * (decimal)z);
            }
            return (double)result;
        }

        double Simpson (Func<double, double> f, double a, double b, int n) {
            var h = (b - a) / n;

            var sum = 0.0;
            for (var i = 1; i <= n - 3; i = i + 2)
                sum += f(a + i * h);

            sum += f(a + (n - 1) * h);
            sum = 4 * sum;

            var sum2 = 0.0;
            for (var i = 2; i <= n - 4; i += 2)
                sum2 += f(a + i * h);

            sum2 += f(a + (n - 2) * h);
            sum2 *= 2;
            sum += sum2 + f(a) + f(b);

            return h / 3 * sum;
        }

        double Quadratur (ExpressionParameters parameters) {
            if (parameters == null)
                parameters = new ExpressionParameters();

            parameters.AngleMeasurement = AngleMeasurement.Radian;

            var result = 0.0M;
            var left = (decimal)(double)this.Left.Calculate(parameters);
            var right = (decimal)(double)this.Right.Calculate(parameters);

            if (right <= left)
                throw new InvalidOperationException("Invalid Intergral-Bounds");

            var n = 1000;    // number of rectangles
            var dx = (right - left) / (decimal)n;

            var param = default(Parameter);
            foreach (var p in parameters.Parameters.Collection) {
                if (p.Key == this.Variable.Name) {
                    param = p;
                    break;
                }
            }
            if (param == null) {
                param = new Parameter(
                    this.Variable.Name, 0, ParameterType.Normal);

                parameters.Parameters.Add(param);
            }
            for (var x = left; x <= right; x += dx) {
                param.Value = (double)x;
                var z = (double)this.Body.Calculate(parameters);
                result += (dx * (decimal)z);
            }
            return (double)result;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone ()
        {
            return new DefiniteIntegral(
                CloneArguments(), countOfParams);
        }

        /// <summary>
        /// Calculates a derivative of the expression.
        /// </summary>
        /// <returns>
        /// Returns a derivative of the expression.
        /// </returns>
        public override IExpression Differentiate () {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates a derivative of the expression.
        /// </summary>
        /// <param name="variable">The variable of differentiation.</param>
        /// <returns>
        /// Returns a derivative of the expression of several variables.
        /// </returns>
        /// <seealso cref="Variable" />
        public override IExpression Differentiate (Variable variable) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the expression to integrate.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public IExpression Body
        {
            get
            {
                return arguments[0];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                arguments[0] = value;
                arguments[0].Parent = this;
            }
        }

        /// <summary>
        /// Gets or sets the integration-variable.
        /// </summary>
        /// <value>
        /// The variable.
        /// </value>
        public Variable Variable
        {
            get {
                return (Variable)arguments[1];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                arguments[1] = value;
                arguments[1].Parent = this;
            }
        }

        public IExpression Left {
            get {
                return arguments[2];
            }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                arguments[2] = value;
                arguments[2].Parent = this;
            }
        }

        public IExpression Right {
            get {
                return arguments[3];
            }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                arguments[3] = value;
                arguments[3].Parent = this;
            }
        }

        /// <summary>
        /// Gets the minimum count of parameters.
        /// </summary>
        /// <value>
        /// The minimum count of parameters.
        /// </value>
        public override int MinCountOfParams
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets the maximum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The maximum count of parameters.
        /// </value>
        public override int MaxCountOfParams
        {
            get
            {
                return 4;
            }
        }

    }

}

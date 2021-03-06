﻿// Copyright 2014 - Ronny Weidemann

using System;
namespace xFunc.Maths.Expressions {
    public class Fraction : DifferentParametersExpression {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fraction"/> class.
        /// </summary>
		internal Fraction ()
            : base(null, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fraction"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentException"></exception>
		public Fraction (IExpression[] args, int countOfParams)
            : base(args, countOfParams)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (args.Length < 2 && args.Length != countOfParams)
                throw new ArgumentException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fraction"/> class.
        /// </summary>
		public Fraction(IExpression wholePart, IExpression numeratorPart, IExpression denominatorPart )
			: base(new[] { wholePart, numeratorPart, denominatorPart }, 3 ) {

        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
			return base.GetHashCode ();	// TODO Add Hash - Values
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString()
        {
            return base.ToString("fract");
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
			var whole = (double)this.Arguments [0].Calculate (parameters);			// Whole-Number-Part
			var numerator = (double)this.Arguments [1].Calculate (parameters);		// Numerator-Part
			var denominator = (double)this.Arguments [2].Calculate (parameters);	// Denominator-Part
			return whole + numerator / denominator;
        }

        /// <summary>
        /// Clones this instance of the <see cref="Fraction"/>.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="Fraction"/> that is a clone of this instance.</returns>
        public override IExpression Clone()
        {
            return new Fraction(CloneArguments(), arguments.Length);
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
                return 3;
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
                return 3;
            }
        }

    }

}

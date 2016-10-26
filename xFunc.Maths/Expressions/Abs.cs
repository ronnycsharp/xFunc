<<<<<<< HEAD
﻿// Copyright 2012-2016 Dmitry Kischenko
=======
﻿// Copyright 2012-2014 Dmitry Kischenko
// Ronny Weidemann - June 2014, Added Vector-Calculation
>>>>>>> 0dedb0a5e5bcf6c2a4787468fc545c5e1a5c3488
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
<<<<<<< HEAD
using System.Numerics;
=======
using System.Linq;
using xFunc.Maths.Expressions.Matrices;
>>>>>>> 0dedb0a5e5bcf6c2a4787468fc545c5e1a5c3488

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
        public override int GetHashCode()
        {
            return base.GetHashCode(6329);
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString()
        {
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
<<<<<<< HEAD
        public override object Execute(ExpressionParameters parameters)
        {
            var result = m_argument.Execute(parameters);

            if (result is Complex)
                return Complex.Abs((Complex)result);

            return Math.Abs((double)result);
=======
        public override object Calculate(ExpressionParameters parameters) {
            if (argument is Vector) {
                return Absolute(
                    (Vector)argument, parameters);
            } else {
                var arg = argument.Calculate(parameters);
                if (arg is Double)
                    return Math.Abs((double)arg);
                else if (arg is Vector) {
                    return Absolute(
                        (Vector)arg, parameters);
                }
            }
            throw new NotSupportedException();
        }

        static double Absolute (Vector vec, ExpressionParameters parameters) {
            return Math.Sqrt(
                vec.Arguments.Sum ( a => Math.Pow ( 
                    ( double ) a.Calculate ( parameters ), 2 ) ) );
>>>>>>> 0dedb0a5e5bcf6c2a4787468fc545c5e1a5c3488
        }

        /// <summary>
        /// Clones this instance of the <see cref="xFunc.Maths.Expressions.Abs"/> class.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
<<<<<<< HEAD
        public override IExpression Clone()
        {
            return new Abs(m_argument.Clone());
        }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        /// <value>
        /// The type of the argument.
        /// </value>
        public override ExpressionResultType ArgumentType
        {
            get
            {
                return ExpressionResultType.Number | ExpressionResultType.ComplexNumber;
            }
=======
        public override IExpression Clone() {
            return new Abs(argument.Clone());
>>>>>>> 0dedb0a5e5bcf6c2a4787468fc545c5e1a5c3488
        }
    }
}
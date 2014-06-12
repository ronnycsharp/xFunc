﻿// Copyright 2012-2014 Dmitry Kischenko
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
using xFunc.Maths.Expressions.Matrices;

namespace xFunc.Maths.Expressions
{

    /// <summary>
    /// Represents an Addition operation.
    /// </summary>
    public class Add : BinaryExpression
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Add"/> class.
        /// </summary>
        internal Add() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Add"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <seealso cref="IExpression"/>
        public Add(IExpression left, IExpression right) : base(left, right) { }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode(6203, 6883);
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString()
        {
            if (m_parent is BinaryExpression)
            {
                return ToString("({0} + {1})");
            }

            return ToString("{0} + {1}");
        }

        /// <summary>
        /// Calculates this expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Calculate(ExpressionParameters parameters)
        {
            if (ResultIsMatrix)
            {
                if (m_left is Vector && m_right is Vector)
                    return MatrixExtentions.Add((Vector)m_left, (Vector)m_right, parameters);
                if (m_left is Matrix && m_right is Matrix)
                    return MatrixExtentions.Add((Matrix)m_left, (Matrix)m_right, parameters);
                if ((m_left is Vector && m_right is Matrix) || (m_right is Vector && m_left is Matrix))
                    throw new NotSupportedException();

                if (!(m_left is Vector || m_left is Matrix))
                {
                    var l = m_left.Calculate(parameters);

                    if (l is Vector)
                        return MatrixExtentions.Add((Vector)l, (Vector)m_right, parameters);
                    if (l is Matrix)
                        return MatrixExtentions.Add((Matrix)l, (Matrix)m_right, parameters);

                    throw new NotSupportedException();
                }

                if (!(m_right is Vector || m_right is Matrix))
                {
                    var r = m_right.Calculate(parameters);

                    if (r is Vector)
                        return MatrixExtentions.Add((Vector)m_left, (Vector)r, parameters);
                    if (r is Matrix)
                        return MatrixExtentions.Add((Matrix)m_left, (Matrix)r, parameters);

                    throw new NotSupportedException();
                }

                throw new NotSupportedException();
            }

            return (double)m_left.Calculate(parameters) + (double)m_right.Calculate(parameters);
        }

        /// <summary>
        /// Calculates a derivative of the expression.
        /// </summary>
        /// <param name="variable">The variable of differentiation.</param>
        /// <returns>
        /// Returns a derivative of the expression of several variables.
        /// </returns>
        /// <seealso cref="Variable" />
        public override IExpression Differentiate(Variable variable)
        {
            var first = Parser.HasVar(left, variable);
            var second = Parser.HasVar(right, variable);

            if (first && second)
                return new Add(left.Clone().Differentiate(variable), right.Differentiate(variable).Clone());
            if (first)
                return left.Clone().Differentiate(variable);
            if (second)
                return right.Differentiate(variable).Clone();

            return new Number(0);
        }

        /// <summary>
        /// Clones this instance of the <see cref="Add"/>.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone()
        {
            return new Add(m_left.Clone(), m_right.Clone());
        }

        /// <summary>
        /// Gets a value indicating whether result is a matrix.
        /// </summary>
        /// <value>
        ///   <c>true</c> if result is a matrix; otherwise, <c>false</c>.
        /// </value>
        public override bool ResultIsMatrix
        {
            get
            {
                return m_left.ResultIsMatrix && m_right.ResultIsMatrix;
            }
        }

    }

}

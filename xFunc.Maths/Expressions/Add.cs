﻿// Copyright 2012-2017 Dmitry Kischenko
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
using System.Numerics;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Expressions.Matrices;

namespace xFunc.Maths.Expressions
{

    /// <summary>
    /// Represents an Addition operation.
    /// </summary>
    public class Add : BinaryExpression
    {

        [ExcludeFromCodeCoverage]
        internal Add() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Add"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <seealso cref="IExpression"/>
        public Add(IExpression left, IExpression right) : base(left, right) { }

        /// <summary>
        /// Gets the result type.
        /// </summary>
        /// <returns>
        /// The result type of current expression.
        /// </returns>
        protected override ExpressionResultType GetResultType()
        {
            if ((m_left.ResultType.HasFlagNI(ExpressionResultType.ComplexNumber) && m_left.ResultType != ExpressionResultType.All) ||
                (m_right.ResultType.HasFlagNI(ExpressionResultType.ComplexNumber) && m_right.ResultType != ExpressionResultType.All))
                return ExpressionResultType.ComplexNumber;

            if (m_left.ResultType == ExpressionResultType.Number || m_right.ResultType == ExpressionResultType.Number)
                return ExpressionResultType.Number;

            if (m_left.ResultType == ExpressionResultType.Matrix || m_right.ResultType == ExpressionResultType.Matrix)
                return ExpressionResultType.Matrix;

            if (m_left.ResultType == ExpressionResultType.Vector || m_right.ResultType == ExpressionResultType.Vector)
                return ExpressionResultType.Vector;

            if (m_left.ResultType.HasFlagNI(ExpressionResultType.Number) || m_right.ResultType.HasFlagNI(ExpressionResultType.Number))
                return ExpressionResultType.Number;

            return ExpressionResultType.Number | ExpressionResultType.ComplexNumber | ExpressionResultType.Vector | ExpressionResultType.Matrix;
        }

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
        /// Executes this expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the execution.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public override object Execute(ExpressionParameters parameters)
        {
            var resultType = this.ResultType;

            var leftResult = m_left.Execute(parameters);
            var rightResult = m_right.Execute(parameters);

            if (resultType == ExpressionResultType.ComplexNumber)
            {
                var leftComplex = leftResult as Complex? ?? (double)leftResult;
                var rightComplex = rightResult as Complex? ?? (double)rightResult;

                return Complex.Add(leftComplex, rightComplex);
            }

            if (resultType == ExpressionResultType.Matrix)
                return ((Matrix)leftResult).Add((Matrix)rightResult, parameters);

            if (resultType == ExpressionResultType.Vector)
                return ((Vector)leftResult).Add((Vector)rightResult, parameters);

            return (double)leftResult + (double)rightResult;
        }

        /// <summary>
        /// Analyzes the current expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>
        /// The analysis result.
        /// </returns>
        public override TResult Analyze<TResult>(IAnalyzer<TResult> analyzer)
        {
            return analyzer.Analyze(this);
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
        /// Gets the type of the left parameter.
        /// </summary>
        /// <value>
        /// The type of the left parameter.
        /// </value>
        public override ExpressionResultType LeftType
        {
            get
            {
                if (m_right != null)
                {
                    if (m_right.ResultType.HasFlagNI(ExpressionResultType.ComplexNumber) || m_right.ResultType.HasFlagNI(ExpressionResultType.Number))
                        return ExpressionResultType.Number | ExpressionResultType.ComplexNumber;

                    if (m_right.ResultType == ExpressionResultType.Matrix)
                        return ExpressionResultType.Matrix;

                    if (m_right.ResultType == ExpressionResultType.Vector)
                        return ExpressionResultType.Vector;
                }

                return ExpressionResultType.Number | ExpressionResultType.ComplexNumber | ExpressionResultType.Vector | ExpressionResultType.Matrix;
            }
        }

        /// <summary>
        /// Gets the type of the right parameter.
        /// </summary>
        /// <value>
        /// The type of the right parameter.
        /// </value>
        public override ExpressionResultType RightType
        {
            get
            {
                if (m_left != null)
                {
                    if (m_left.ResultType.HasFlagNI(ExpressionResultType.ComplexNumber) || m_left.ResultType.HasFlagNI(ExpressionResultType.Number))
                        return ExpressionResultType.Number | ExpressionResultType.ComplexNumber;

                    if (m_left.ResultType == ExpressionResultType.Matrix)
                        return ExpressionResultType.Matrix;

                    if (m_left.ResultType == ExpressionResultType.Vector)
                        return ExpressionResultType.Vector;
                }

                return ExpressionResultType.Number | ExpressionResultType.ComplexNumber | ExpressionResultType.Vector | ExpressionResultType.Matrix;
            }
        }

    }

}

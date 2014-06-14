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
#if NET35_OR_GREATER || PORTABLE
using System.Linq;
#endif
using System.Text;

namespace xFunc.Maths.Expressions
{

    /// <summary>
    /// The base class for expressions with different number of parameters.
    /// </summary>
    public abstract class DifferentParametersExpression : IExpression
    {

        /// <summary>
        /// The parent expression of this expression.
        /// </summary>
        protected IExpression m_parent;
        /// <summary>
        /// The arguments.
        /// </summary>
        protected IExpression[] m_arguments;
        /// <summary>
        /// The count of parameters.
        /// </summary>
        protected int m_countOfParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferentParametersExpression"/> class.
        /// </summary>
        /// <param name="countOfParams">The count of parameters.</param>
        protected DifferentParametersExpression(int countOfParams)
            : this(null, countOfParams)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferentParametersExpression" /> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        protected DifferentParametersExpression(IExpression[] arguments, int countOfParams)
        {
            this.Arguments = arguments;
            this.m_countOfParams = countOfParams;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        protected int GetHashCode(int first, int second)
        {
            return m_arguments.Aggregate(first, (current, item) => current * second + item.GetHashCode());
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        protected string ToString(string function)
        {
            var sb = new StringBuilder();

            sb.Append(function).Append('(');
            foreach (var item in m_arguments)
                sb.Append(item).Append(", ");
            sb.Remove(sb.Length - 2, 2).Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Calculates this mathemarical expression. Don't use this method if your expression has variables or user-functions.
        /// </summary>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        public virtual object Calculate()
        {
            return Calculate(null);
        }

        /// <summary>
        /// Calculates this mathemarical expression.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the calculation.
        /// </returns>
        /// <seealso cref="ExpressionParameters" />
        public abstract object Calculate(ExpressionParameters parameters);

        /// <summary>
        /// Clones this instance of the <see cref="IExpression" />.
        /// </summary>
        /// <returns>
        /// Returns the new instance of <see cref="IExpression" /> that is a clone of this instance.
        /// </returns>
        public abstract IExpression Clone();

        /// <summary>
        /// Closes the arguments.
        /// </summary>
        /// <returns>The new array of <see cref="IExpression"/>.</returns>
        protected IExpression[] CloneArguments()
        {
            var args = new IExpression[m_arguments.Length];
            for (int i = 0; i < m_arguments.Length; i++)
                args[i] = m_arguments[i].Clone();

            return args;
        }

        /// <summary>
        /// Get or Set the parent expression.
        /// </summary>
        public IExpression Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public virtual IExpression[] Arguments
        {
            get
            {
                return m_arguments;
            }
            set
            {
                m_arguments = value;
                if (m_arguments != null)
                {
                    foreach (var item in m_arguments)
                    {
                        if (item != null)
                            item.Parent = this;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum count of parameters.
        /// </summary>
        /// <value>
        /// The minimum count of parameters.
        /// </value>
        public abstract int MinCountOfParams { get; }

        /// <summary>
        /// Gets the maximum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The maximum count of parameters.
        /// </value>
        public abstract int MaxCountOfParams { get; }

        /// <summary>
        /// Gets or Sets the count of parameters.
        /// </summary>
        /// <value>
        /// The count of parameters.
        /// </value>
        public int CountOfParams
        {
            get
            {
                return m_countOfParams;
            }
            set
            {
                m_countOfParams = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether result is a matrix. Default implementation returns <c>false</c>. Override it if this expression returns a matrix.
        /// </summary>
        /// <value>
        ///   <c>true</c> if result is a matrix; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ResultIsMatrix
        {
            get
            {
                return false;
            }
        }

    }

}

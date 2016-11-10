﻿// Copyright 2012-2016 Dmitry Kischenko
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
using System.Text;
using xFunc.Maths.Expressions.Collections;

namespace xFunc.Maths.Expressions
{

    /// <summary>
    /// Represents user-defined functions.
    /// </summary>
    public class UserFunction : DifferentParametersExpression
    {

        private string function;

        internal UserFunction()
            : this(null, null, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFunction"/> class.
        /// </summary>
        /// <param name="function">The name of function.</param>
        internal UserFunction(string function)
            : this(function, null, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFunction"/> class.
        /// </summary>
        /// <param name="function">The name of function.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        public UserFunction(string function, int countOfParams) : this(function, null, countOfParams) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFunction"/> class.
        /// </summary>
        /// <param name="function">The name of function.</param>
        /// <param name="args">Arguments.</param>
        /// <param name="countOfParams">The count of parameters.</param>
        public UserFunction(string function, IExpression[] args, int countOfParams)
            : base(args, countOfParams)
        {
            this.function = function;
            this.m_arguments = args;
            this.countOfParams = countOfParams;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var exp = obj as UserFunction;
            if (exp != null && this.function == exp.function && this.countOfParams == exp.countOfParams)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 1721;

            hash = (hash * 5701) + function.GetHashCode();
            hash = (hash * 5701) + countOfParams.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Converts this expression to the equivalent string.
        /// </summary>
        /// <returns>The string that represents this expression.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(function).Append('(');
            if (m_arguments != null && m_arguments.Length > 0)
            {
                foreach (var item in m_arguments)
                    sb.Append(item).Append(", ");
                sb.Remove(sb.Length - 2, 2);
            }
            else if (countOfParams > 0)
            {
                for (int i = 1; i <= countOfParams; i++)
                    sb.AppendFormat("x{0}, ", i);
                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Executes the user function.
        /// </summary>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// A result of the execution.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="parameters"/> is null.</exception>
        /// <seealso cref="ExpressionParameters" />
        public override object Execute(ExpressionParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var func = parameters.Functions.GetKeyByKey(this);

            var newParameters = new ParameterCollection(parameters.Variables.Collection);
            for (int i = 0; i < m_arguments.Length; i++)
            {
                var arg = func.Arguments[i] as Variable;
                newParameters[arg.Name] = (double)this.m_arguments[i].Execute(parameters);
            }

            var expParam = new ExpressionParameters(parameters.AngleMeasurement, newParameters, parameters.Functions);
            return parameters.Functions[this].Execute(expParam);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone()
        {
            return new UserFunction(function, m_arguments, countOfParams);
        }

        /// <summary>
        /// Gets the name of function.
        /// </summary>
        /// <value>The name of function.</value>
        public string Function
        {
            get
            {
                return function;
            }
        }

        /// <summary>
        /// Gets the minimum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The minimum count of parameters.
        /// </value>
        public override int MinParameters
        {
            get
            {
                return countOfParams;
            }
        }

        /// <summary>
        /// Gets the maximum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The maximum count of parameters.
        /// </value>
        public override int MaxParameters
        {
            get
            {
                return countOfParams;
            }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public override ExpressionResultType ResultType
        {
            get
            {
                return ExpressionResultType.All;
            }
        }

    }

}

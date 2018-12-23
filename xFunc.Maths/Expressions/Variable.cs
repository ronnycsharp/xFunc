// Copyright 2012-2018 Dmitry Kischenko
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
using xFunc.Maths.Analyzers;
using xFunc.Maths.Analyzers.Formatters;

namespace xFunc.Maths.Expressions
{

    /// <summary>
    /// Represents variables in expressions.
    /// </summary>
    public class Variable : IExpression
    {

        /// <summary>
        /// The 'x' variable.
        /// </summary>
        public static readonly Variable X = new Variable("x");

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">A name of variable.</param>
        public Variable(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Defines an implicit conversion of a Variable object to a string object.
        /// </summary>
        /// <param name="variable">The value to convert.</param>
        /// <returns>An object that contains the converted value.</returns>
        public static implicit operator string(Variable variable)
        {
            return variable?.Name;
        }

        /// <summary>
        /// Defines an implicit conversion of a string object to a Variable object.
        /// </summary>
        /// <param name="variable">The value to convert.</param>
        /// <returns>An object that contains the converted value.</returns>
        public static implicit operator Variable(string variable)
        {
            return new Variable(variable);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Variable @var && @var.Name == name;
        }

        /// <summary>
        /// Returns a hash function for this type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Variable"/>.</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode() ^ 9239;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatter formatter)
        {
            return this.Analyze(formatter);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ToString(new CommonFormatter());
        }

        /// <summary>
        /// Do not use this method. It always throws an exception.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Always.</exception>
        public object Execute()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets value of this variable from <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">Collection of variables.</param>
        /// <returns>A value of this variable.</returns>
        public object Execute(ExpressionParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return parameters.Variables[name];
        }

        /// <summary>
        /// Analyzes the current expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>
        /// The analysis result.
        /// </returns>
        public TResult Analyze<TResult>(IAnalyzer<TResult> analyzer)
        {
            return analyzer.Analyze(this);
        }

        /// <summary>
        /// Clones this instance of the <see cref="Variable"/>.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="Variable"/> that is a clone of this instance.</returns>
        public IExpression Clone()
        {
            return new Variable(name);
        }

		// The Name-Property must be writeable, because we need backward-compatibility

        /// <summary>
        /// A name of this variable.
        /// </summary>
		public string Name { 
			get { return name; } 
			set { name = value; } 
		}

        /// <summary>
        /// Get or Set the parent expression.
        /// </summary>
        public IExpression Parent { get; set; }

        /// <summary>
        /// Gets the minimum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The minimum count of parameters.
        /// </value>
        public int MinParameters => 0;

        /// <summary>
        /// Gets the maximum count of parameters. -1 - Infinity.
        /// </summary>
        /// <value>
        /// The maximum count of parameters.
        /// </value>
        public int MaxParameters => -1;

        /// <summary>
        /// Gets the count of parameters.
        /// </summary>
        /// <value>
        /// The count of parameters.
        /// </value>
        public int ParametersCount => 0;

    }

}

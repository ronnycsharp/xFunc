// Copyright 2012-2018 Dmitry Kischenko & Ronny Weidemann
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
using System.Collections.Generic;
using System.Linq;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;
using xFunc.Maths.Tokenization.Tokens;

namespace xFunc.Maths
{

    /// <summary>
    /// The helper class with additional methods.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Checks that <paramref name="expression"/> has  the <paramref name="arg"/> variable.
        /// </summary>
        /// <param name="expression">The expression that is checked.</param>
        /// <param name="arg">The variable that can be contained in the expression.</param>
        /// <returns>true if <paramref name="expression"/> has <paramref name="arg"/>; otherwise, false.</returns>
        public static bool HasVariable(IExpression expression, Variable arg)
        {
            var bin = expression as BinaryExpression;
            if (bin != null)
                return HasVariable(bin.Left, arg) || HasVariable(bin.Right, arg);

            var un = expression as UnaryExpression;
            if (un != null)
                return HasVariable(un.Argument, arg);

            var paramExp = expression as DifferentParametersExpression;
            if (paramExp != null)
                return paramExp.Arguments.Any(e => HasVariable(e, arg));

            return expression is Variable && expression.Equals(arg);
        }

        /// <summary>
        /// Gets parameters of expression.
        /// </summary>
        /// <param name="tokens">The list of tokens.</param>
        /// <returns>A collection of parameters.</returns>
        public static ParameterCollection GetParameters(IEnumerable<IToken> tokens)
        {
            var c = new SortedSet<Parameter>();

            foreach (var token in tokens)
                if (token is VariableToken @var)
                    c.Add(new Parameter(@var.Variable, false));

            return new ParameterCollection(c, false);
        }

        /// <summary>
        /// Converts the logic expression to collection.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The collection of expression parts.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> variable is null.</exception>
        public static IEnumerable<IExpression> ConvertExpressionToCollection(IExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var collection = new List<IExpression>();
            ConvertToColletion(expression, collection);

            return collection;
        }

        private static void ConvertToColletion(IExpression expression, List<IExpression> collection)
        {
            if (expression is UnaryExpression)
            {
                var un = expression as UnaryExpression;
                ConvertToColletion(un.Argument, collection);
            }
            else if (expression is BinaryExpression)
            {
                var bin = expression as BinaryExpression;
                ConvertToColletion(bin.Left, collection);
                ConvertToColletion(bin.Right, collection);
            }
            else if (expression is Variable)
            {
                return;
            }

            collection.Add(expression);
        }

        /// <summary>
        /// Gets all variables.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The list of variables.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="expression"/> is null.</exception>
        public static IEnumerable<Variable> GetAllVariables(IExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var collection = new HashSet<Variable>();
            GetAllVariables(expression, collection);

            return collection;
        }

        private static void GetAllVariables(IExpression expression, HashSet<Variable> collection)
        {
            if (expression is UnaryExpression un)
            {
                GetAllVariables(un.Argument, collection);
            }
            else if (expression is BinaryExpression bin)
            {
                GetAllVariables(bin.Left, collection);
                GetAllVariables(bin.Right, collection);
            }
            else if (expression is DifferentParametersExpression diff)
            {
                foreach (var exp in diff.Arguments)
                    GetAllVariables(exp, collection);
            }
            else if (expression is Variable)
            {
                collection.Add((Variable)expression);
            }
        }

        public static bool IsChildOf<T>(IExpression exp) where T : IExpression {
            if ( exp.Parent is T ) {
                return true;
            } else {
                return exp.Parent != null 
                    && IsChildOf<T>(exp.Parent);
            }
        }

		public static double DegToRad (double angle) {
			return Math.PI * angle / 180.0;
		}

		public static double RadToDeg (double angle) {
			return angle * (180.0 / Math.PI);
		}

        public static bool AlmostEqual(double a, double b, double epsilon) {
            return Math.Abs(a - b) < epsilon;
        }

        public static float Distance(float value1, float value2) {
            return Math.Abs(value1 - value2);
        }

        public static double Distance(double value1, double value2) {
            return Math.Abs(value1 - value2);
        }
    }
}

// Copyright 2013-2018 Ronny Weidemann
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
using xFunc.Maths.Analyzers;

namespace xFunc.Maths.Expressions {
    public class Condition : DifferentParametersExpression {
        public Condition() : base(-1) {}

        public Condition(IExpression exp, string condition) 
            : base ( new IExpression [] { exp, ParseLogicalCondition(condition) }, 2 ) {

        }

        public Condition(IExpression[] arguments, int countOfParams)
            : base(arguments, countOfParams) {

            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Length != countOfParams)
                throw new ArgumentException();
        }

        #region Properties

        public IExpression Expression => this.Arguments[0];

        public IExpression ConditionLogic => this.Arguments[1];

        public override int MinParameters => 2;

        public override int MaxParameters => -1;

        /*
        public override ExpressionResultType[] ArgumentsTypes {
            get {
                var result = new ExpressionResultType[ParametersCount];
                if (ParametersCount > 0) {
                    result[0] = ExpressionResultType.Number;
                    for ( var i = 1; i < result.Length; i++)
                        result[i] = ExpressionResultType.Boolean;
                }
                return result;
            }
        }*/

        #endregion

        public override IExpression Clone() {
            return new Condition(CloneArguments(), ParametersCount);
        }

        public bool IsFulfilled(ExpressionParameters parameters) {
            for (var i = 1; i < this.Arguments.Length; i++) {
                var result = this.Arguments[i].Execute(parameters);
                if ( result is Boolean b && b ) {
                    continue;
                } else {
                    return false;
                }
            }
            return true;
        }

        public override object Execute(ExpressionParameters parameters) {
            if (!IsFulfilled(parameters))
                return null;

            return this.Expression.Execute(parameters);
        }

        private static IExpression ParseLogicalCondition(string expression) {
            var proc = new Processor();
            var conditions = new List<String>();
            if (!expression.Contains("&")) {
                var lastIndex = 0;
                var lastVariable = 'z';
                var first = true;
                for (var i = 0; i < expression.Length; i++) {
                    var c = expression[i];
                    if (c == '<' || c == '≤' || c == '>' || c == '≥') {
                        if (!first) {
                            conditions.Add(expression.Substring(lastIndex, i));
                            lastIndex = i;
                        }
                        first = false;
                    } else if (
                        c == 'x' || c == 'y' || c == 'z'
                            || c == 'd' || c == 'a' || c == 'b') {
                        lastVariable = c;
                    }

                    if (i == expression.Length - 1) {
                        var condition = expression.Substring(lastIndex, i - lastIndex + 1);
                        conditions.Add((lastIndex > 0 ? lastVariable.ToString() : String.Empty) + condition);
                    }
                }

                expression = String.Empty;
                var and = false;
                foreach (var c in conditions) {
                    if (and) {
                        expression += "&&";
                    }
                    expression += c;
                    and = true;
                }
            }

            expression = expression
                //.Replace("=", "==")
                .Replace("≤", "<=")
                .Replace("≥", ">=");

            return proc.Parse(expression);
        }

        public override TResult Analyze<TResult>(IAnalyzer<TResult> analyzer) {
            return analyzer.Analyze(this);
        }
    }
}

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
using System.Linq;
using xFunc.Maths.Analyzers;
using xFunc.Maths.Analyzers.Formatters;

namespace xFunc.Maths.Expressions {
    public class MultiCondition : DifferentParametersExpression {

        public MultiCondition() : base(-1) { }

        public MultiCondition(params Condition[] conditions) 
            : base (conditions, conditions.Length){
        }

        public MultiCondition(IExpression[] arguments, int countOfParams)
            : base(arguments, countOfParams) {

            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Length != countOfParams)
                throw new ArgumentException();
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override int MinParameters => 1;

        /// <summary>
        /// 
        /// </summary>
        public override int MaxParameters => -1;

        /*
        public override ExpressionResultType[] ArgumentsTypes {
            get {
                var result = new ExpressionResultType[ParametersCount];
                if (ParametersCount > 0) {
                    return Enumerable.Repeat<ExpressionResultType>(ExpressionResultType.All, ParametersCount).ToArray();
                }
                return result;
            }
        }*/

        #endregion

        public override TResult Analyze<TResult>(IAnalyzer<TResult> analyzer) {
            return analyzer.Analyze(this);
        }

        public override IExpression Clone() {
            return new MultiCondition(CloneArguments(), ParametersCount);
        }

        public override object Execute(ExpressionParameters parameters) {
            foreach (var condition in this.Arguments.OfType<Condition>()) {
                if (condition.IsFulfilled(parameters)) {
                    return condition.Execute(parameters);
                }
            }
            return Double.NaN;
        }

        /*
        public override string ToString() {
            return this.ToString(new CommonFormatter());
        }*/
    }
}

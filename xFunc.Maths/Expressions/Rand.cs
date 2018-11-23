// Copyright 2012-2017 Dmitry Kischenko
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
using xFunc.Maths.Analyzers;
using xFunc.Maths.Analyzers.Formatters;

namespace xFunc.Maths.Expressions {
    /// <summary>
    /// Represents the Rand function.
    /// </summary>
    public class Rand : DifferentParametersExpression {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rand"/> class.
        /// </summary>
        public Rand() : base(0) { }

        public override int MinParameters => 0;

        public override int MaxParameters => 0;

        #region Properties

        #endregion

        public override TResult Analyze<TResult> (IAnalyzer<TResult> analyzer) {
            return analyzer.Analyze (this);
        }

        /// <summary>
        /// Clones this instance of the <see cref="Rand"/>.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="IExpression"/> that is a clone of this instance.</returns>
        public override IExpression Clone() {
            return new Rand ();
        }

        public override object Execute () {
            return new Random ().NextDouble ();
        }

        public override object Execute (ExpressionParameters parameters) {
            return Execute ();
        }
    }
}
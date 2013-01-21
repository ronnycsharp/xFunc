﻿// Copyright 2012 Dmitry Kischenko
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
using System.Text;

namespace xFunc.Logics.Expressions
{

    public class TruthTableExpression : UnaryLogicExpression
    {

        public TruthTableExpression() : base(null) { }

        public TruthTableExpression(ILogicExpression expression) : base(expression) { }

        public override bool Calculate(LogicParameterCollection parameters)
        {
            throw new NotSupportedException();
        }

    }

}
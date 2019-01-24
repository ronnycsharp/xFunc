// Copyright 2012-2017 Dmitry Kischenko
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
using xFunc.Maths.Expressions.Hyperbolic;
using xFunc.Maths.Expressions.Statistical;
using xFunc.Maths.Expressions.Trigonometric;

namespace xFunc.Maths.Analyzers
{
    /// <summary>
    /// The differentiator of expressions.
    /// </summary>
    /// <seealso cref="xFunc.Maths.Analyzers.Analyzer{TResult}" />
    /// <seealso cref="xFunc.Maths.Analyzers.IDifferentiator" />
    public class Differentiator : Analyzer<IExpression>, IDifferentiator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Differentiator"/> class.
        /// </summary>
        public Differentiator() : this(new ExpressionParameters(), new Variable("x")) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Differentiator"/> class.
        /// </summary>
        /// <param name="variable">The variable.</param>
        public Differentiator(Variable variable) : this(new ExpressionParameters(), variable) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Differentiator"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="variable">The variable.</param>
        public Differentiator(ExpressionParameters parameters, Variable variable)
        {
            Parameters = parameters;
            Variable = variable;
        }

        #region Properties

        public DerivationStep RootStep { get; set; }

        public DerivationStep FindStep(IExpression child)
        {
            if (parents.ContainsKey(child))
            {
                return parents[child];
            }
            return null;
        }

        public void SetParent(IExpression child, DerivationStep parentStep)
        {
            if (child == null || parentStep == null)
                throw new ArgumentNullException();

            if (parents.ContainsKey(child) && parents[child] != parentStep)
                throw new Exception("Key already exists");

            parents[child] = parentStep;
        }

        private Dictionary<IExpression, DerivationStep> parents
            = new Dictionary<IExpression, DerivationStep>(new KeyComparer());

        class KeyComparer : IEqualityComparer<IExpression>
        {
            public bool Equals(IExpression x, IExpression y)
            {
                return x == y;
            }

            public int GetHashCode(IExpression obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion

        #region Standard

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Abs exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var div = new Div(exp.Argument.Clone(), exp.Clone());
            var mul = new Mul(exp.Argument.Clone().Analyze(this), div);

            return mul;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Add exp) {
            var currentStep = FindCurrentStep(exp);
            currentStep.Rule = DerivationRule.Sum;

            // check whether the given expression is a constant 
            if (IsConstant(exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number(0);
                return currentStep.Derivative;
            }

            var first   = Helpers.HasVariable(exp.Left, Variable);
            var second  = Helpers.HasVariable(exp.Right, Variable);

            if (first && second) {
                var left = exp.Left.Clone();
                var right = exp.Right.Clone();

                var leftDeriv   = left.AsDerivative(this);
                var rightDeriv  = right.AsDerivative(this);

                currentStep.Intermediate    = new Add(leftDeriv, rightDeriv);
                currentStep.Derivative      = new Add(
                                                currentStep.AddStep(left), 
                                                currentStep.AddStep(right));
                                                
                return currentStep.SimplifiedDerivative;
            }

            if (first) {
                return currentStep.AddStep(exp.Left.Clone());
            }

            return currentStep.AddStep( exp.Right.Clone());
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Derivative exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var diff = exp.Expression.Analyze(this);
            if (exp.Parent is Derivative)
                diff = diff.Analyze(this);

            return diff;
        }

        public override IExpression Analyze(NDerivative exp)
        {
            return new NDerivative(
                exp.Expression,
                new Number(
                    ((Number)exp.Number).Value + 1),
                exp.Variable);
        }

        public override IExpression Analyze(DefiniteIntegral exp)
        {
            return new Number(0);
        }

        public override IExpression Analyze(Sign exp)
        {
            return new Number(0);
        }

        public override IExpression Analyze(RoundUnary exp)
        {
            return new Number(0);
        }

        public override IExpression Analyze(Sum exp)
        {
            return new Sum(
                exp.Body.Analyze(this),
                exp.From,
                exp.To,
                exp.Increment,
                exp.Variable);
        }

        public override IExpression Analyze(MultiCondition exp)
        {
            var conditions = new List<Condition>();
            foreach (var condition in exp.Arguments.OfType<Condition>())
            {
                conditions.Add(
                    new Condition(
                        new[]{
                            condition.Expression.Analyze(this),
                            condition.ConditionLogic
                        }, 2));
            }
            return new MultiCondition(conditions.ToArray());
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Div exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var first = Helpers.HasVariable(exp.Left, Variable);
            var second = Helpers.HasVariable(exp.Right, Variable);

            if (first && second)
            {
                var mul1 = new Mul(exp.Left.Clone().Analyze(this), exp.Right.Clone());
                var mul2 = new Mul(exp.Left.Clone(), exp.Right.Clone().Analyze(this));
                var sub = new Sub(mul1, mul2);
                var inv = new Pow(exp.Right.Clone(), new Number(2));
                var division = new Div(sub, inv);

                return division;
            }

            if (second)
            {
                var mul2 = new Mul(exp.Left.Clone(), exp.Right.Clone().Analyze(this));
                var unMinus = new UnaryMinus(mul2);
                var inv = new Pow(exp.Right.Clone(), new Number(2));
                var division = new Div(unMinus, inv);

                return division;
            }

            //if (first)
            return new Div(exp.Left.Clone().Analyze(this), exp.Right.Clone());
        }


        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Exp exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            return new Mul(exp.Argument.Clone().Analyze(this), exp.Clone());
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Lb exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var ln = new Ln(new Number(2));
            var mul = new Mul(exp.Argument.Clone(), ln);
            var div = new Div(exp.Argument.Clone().Analyze(this), mul);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Lg exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var ln = new Ln(new Number(10));
            var mul1 = new Mul(exp.Argument.Clone(), ln);
            var div = new Div(exp.Argument.Clone().Analyze(this), mul1);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Ln exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            return new Div(exp.Argument.Clone().Analyze(this), exp.Argument.Clone());
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Log exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            if (Helpers.HasVariable(exp.Left, Variable))
            {
                var ln1 = new Ln(exp.Right.Clone());
                var ln2 = new Ln(exp.Left.Clone());
                var div = new Div(ln1, ln2);

                return Analyze(div);
            }

            // if (Helpers.HasVar(exp.Right, variable))
            var ln = new Ln(exp.Left.Clone());
            var mul = new Mul(exp.Right.Clone(), ln);
            var div2 = new Div(exp.Right.Clone().Analyze(this), mul);

            return div2;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Mul exp) {
            var currentStep = FindCurrentStep(exp);
            currentStep.Rule = DerivationRule.Product;

            // check whether the given expression is a constant 
            if (IsConstant(exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number(0);
                return currentStep.Derivative;
            }

            var first   = Helpers.HasVariable(exp.Left, Variable);
            var second  = Helpers.HasVariable(exp.Right, Variable);

            if (first && second) {
                var mul1 = new Mul(exp.Left.Clone().AsDerivative(this), exp.Right.Clone());
                var mul2 = new Mul(exp.Left.Clone(), exp.Right.Clone().AsDerivative(this));
                var add = new Add(mul1, mul2);

                currentStep.Intermediate = add;
                currentStep.Derivative = add.AsAnalyzedExpression(this);

                return currentStep.Derivative;
            }

            if (first) {
                currentStep.Rule = DerivationRule.Factor;
                currentStep.Intermediate = new Mul(exp.Left.Clone().AsDerivative(this), exp.Right.Clone());
                currentStep.Derivative = new Mul(exp.Left.Clone().Analyze(this), exp.Right.Clone());
                return currentStep.Derivative;
            }

            // if (second)
            currentStep.Rule = DerivationRule.Factor;
            currentStep.Intermediate = new Mul(exp.Left.Clone(), exp.Right.Clone().AsDerivative(this));
            currentStep.Derivative = new Mul(exp.Left.Clone(), currentStep.AddStep(exp.Right.Clone()));
            return currentStep.Derivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Number exp)
        {
            return new Number(0);
        }

        DerivationStep FindCurrentStep(IExpression exp)
        {
            if (this.RootStep == null)
            {
                var step = new DerivationStep(this);
                step.Expression = exp;
                RootStep = step;
                return step;
            }
            else
            {
                var step = FindStep(exp);
                if (step == null)
                    throw new InvalidOperationException("something went wrong");

                return step;
            }
        }

        bool IsConstant(IExpression expression)
        {
            return !Helpers.HasVariable(expression, Variable);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Pow exp) {
            var currentStep = FindCurrentStep(exp);
            currentStep.Rule = DerivationRule.Power;

            // check whether the given expression is a constant 
            if (IsConstant(exp)) {
                currentStep.Rule        = DerivationRule.Constant;
                currentStep.Derivative  = new Number(0);
                return currentStep.Derivative;
            }

            if (Helpers.HasVariable(exp.Left, Variable)) {
                var sub     = new Sub(exp.Right.Clone(), new Number(1));
                var inv     = new Pow(exp.Left.Clone(), sub);
                var mul1    = new Mul(exp.Right.Clone(), inv);
                var mul2    = new Mul(exp.Left.Clone().AsDerivative(this), mul1);

                currentStep.Rule            = DerivationRule.Chain;
                currentStep.Intermediate    = mul2;    // Intermediate Step

                sub     = new Sub(exp.Right.Clone(), new Number(1));
                inv     = new Pow(exp.Left.Clone(), sub);
                mul1    = new Mul(exp.Right.Clone(), inv);
                mul2    = new Mul(currentStep.AddStep(exp.Left.Clone()), mul1);

                currentStep.Derivative = mul2;
                return currentStep.Derivative;
            }

            // if (Helpers.HasVar(exp.Right, variable))
            var ln      = new Ln(exp.Left.Clone());
            var mul3    = new Mul(ln, exp.Clone());
            var mul4    = new Mul(mul3, currentStep.AddStep(exp.Right.Clone()));

            currentStep.Derivative = mul4;
            return currentStep.Derivative;
        }

        Simplifier simplifier = new Simplifier();

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Root exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var div = new Div(new Number(1), exp.Right.Clone());
            var pow = new Pow(exp.Left.Clone(), div);

            return Analyze(pow);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Simplify exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            return exp.Argument.Analyze(this);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sqrt exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var mul = new Mul(new Number(2), exp.Clone());
            var div = new Div(exp.Argument.Clone().Analyze(this), mul);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sub exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var first = Helpers.HasVariable(exp.Left, Variable);
            var second = Helpers.HasVariable(exp.Right, Variable);

            if (first && second)
                return new Sub(exp.Left.Clone().Analyze(this), exp.Right.Clone().Analyze(this));
            if (first)
                return exp.Left.Clone().Analyze(this);

            // if (second)
            return new UnaryMinus(exp.Right.Clone().Analyze(this));
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(UnaryMinus exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            return new UnaryMinus(exp.Argument.Clone().Analyze(this));
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(UserFunction exp)
        {
            if (Parameters == null)
                throw new ArgumentNullException(nameof(Parameters));

            return Parameters.Functions[exp].Analyze(this);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Variable exp) {
            var currentStep = FindCurrentStep(exp);
            currentStep.Rule = DerivationRule.Variable;
            if (exp.Equals(Variable)) {
                currentStep.Derivative = new Number(1);
                return currentStep.Derivative;
            }

            //currentStep.Rule = DerivationRule.Constant; // TODO ???
            currentStep.Derivative = exp.Clone();
            return currentStep.Derivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(nPr exp)
        {
            return new Number(0);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(nCr exp)
        {
            return new Number(0);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Rand exp)
        {
            return new Number(0);
        }

        #endregion Standard

        #region Trigonometric

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arccos exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var pow = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), pow);
            var sqrt = new Sqrt(sub);
            var division = new Div(exp.Argument.Clone().Analyze(this), sqrt);
            var unMinus = new UnaryMinus(division);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arccot exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var involution = new Pow(exp.Argument.Clone(), new Number(2));
            var add = new Add(new Number(1), involution);
            var div = new Div(exp.Argument.Clone().Analyze(this), add);
            var unMinus = new UnaryMinus(div);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arccsc exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var abs = new Abs(exp.Argument.Clone());
            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(sqr, new Number(1));
            var sqrt = new Sqrt(sub);
            var mul = new Mul(abs, sqrt);
            var div = new Div(exp.Argument.Clone().Analyze(this), mul);
            var unary = new UnaryMinus(div);

            return unary;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcsec exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var abs = new Abs(exp.Argument.Clone());
            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(sqr, new Number(1));
            var sqrt = new Sqrt(sub);
            var mul = new Mul(abs, sqrt);
            var div = new Div(exp.Argument.Clone().Analyze(this), mul);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcsin exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var involution = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), involution);
            var sqrt = new Sqrt(sub);
            var division = new Div(exp.Argument.Clone().Analyze(this), sqrt);

            return division;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arctan exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var involution = new Pow(exp.Argument.Clone(), new Number(2));
            var add = new Add(new Number(1), involution);
            var div = new Div(exp.Argument.Clone().Analyze(this), add);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Cos exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sine = new Sin(exp.Argument.Clone());
            var multiplication = new Mul(sine, exp.Argument.Clone().Analyze(this));
            var unMinus = new UnaryMinus(multiplication);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Cot exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sine = new Sin(exp.Argument.Clone());
            var involution = new Pow(sine, new Number(2));
            var division = new Div(exp.Argument.Clone().Analyze(this), involution);
            var unMinus = new UnaryMinus(division);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Csc exp)
        {
            var unary = new UnaryMinus(exp.Argument.Clone().Analyze(this));
            var cot = new Cot(exp.Argument.Clone());
            var csc = new Csc(exp.Argument.Clone());
            var mul1 = new Mul(cot, csc);
            var mul2 = new Mul(unary, mul1);

            return mul2;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sec exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var tan = new Tan(exp.Argument.Clone());
            var sec = new Sec(exp.Argument.Clone());
            var mul1 = new Mul(tan, sec);
            var mul2 = new Mul(exp.Argument.Clone().Analyze(this), mul1);

            return mul2;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sin exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var cos = new Cos(exp.Argument.Clone());
            var mul = new Mul(cos, exp.Argument.Clone().Analyze(this));

            return mul;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Tan exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var cos = new Cos(exp.Argument.Clone());
            var inv = new Pow(cos, new Number(2));
            var div = new Div(exp.Argument.Clone().Analyze(this), inv);

            return div;
        }

        #endregion

        #region Hyperbolic

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcosh exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(sqr, new Number(1));
            var sqrt = new Sqrt(sub);
            var div = new Div(exp.Argument.Clone().Analyze(this), sqrt);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcoth exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), sqr);
            var div = new Div(exp.Argument.Clone().Analyze(this), sub);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcsch exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var inv = new Pow(exp.Argument.Clone(), new Number(2));
            var add = new Add(new Number(1), inv);
            var sqrt = new Sqrt(add);
            var abs = new Abs(exp.Argument.Clone());
            var mul = new Mul(abs, sqrt);
            var div = new Div(exp.Argument.Clone().Analyze(this), mul);
            var unMinus = new UnaryMinus(div);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arsech exp)
        {
            var inv = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), inv);
            var sqrt = new Sqrt(sub);
            var mul = new Mul(exp.Argument.Clone(), sqrt);
            var div = new Div(exp.Argument.Clone().Analyze(this), mul);
            var unMinus = new UnaryMinus(div);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arsinh exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var add = new Add(sqr, new Number(1));
            var sqrt = new Sqrt(add);
            var div = new Div(exp.Argument.Clone().Analyze(this), sqrt);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Artanh exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), sqr);
            var div = new Div(exp.Argument.Clone().Analyze(this), sub);

            return div;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Cosh exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sinh = new Sinh(exp.Argument.Clone());
            var mul = new Mul(exp.Argument.Clone().Analyze(this), sinh);

            return mul;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Coth exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var sinh = new Sinh(exp.Argument.Clone());
            var inv = new Pow(sinh, new Number(2));
            var div = new Div(exp.Argument.Clone().Analyze(this), inv);
            var unMinus = new UnaryMinus(div);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Csch exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var coth = new Coth(exp.Argument.Clone());
            var mul1 = new Mul(coth, exp.Clone());
            var mul2 = new Mul(exp.Argument.Clone().Analyze(this), mul1);
            var unMinus = new UnaryMinus(mul2);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sech exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var tanh = new Tanh(exp.Argument.Clone());
            var mul1 = new Mul(tanh, exp.Clone());
            var mul2 = new Mul(exp.Argument.Clone().Analyze(this), mul1);
            var unMinus = new UnaryMinus(mul2);

            return unMinus;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sinh exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var cosh = new Cosh(exp.Argument.Clone());
            var mul = new Mul(exp.Argument.Clone().Analyze(this), cosh);

            return mul;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Tanh exp)
        {
            if (!Helpers.HasVariable(exp, Variable))
                return new Number(0);

            var cosh = new Cosh(exp.Argument.Clone());
            var inv = new Pow(cosh, new Number(2));
            var div = new Div(exp.Argument.Clone().Analyze(this), inv);

            return div;
        }

        #endregion Hyperbolic

        /// <summary>
        /// Gets or sets the variable.
        /// </summary>
        /// <value>
        /// The variable.
        /// </value>
        public Variable Variable { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public ExpressionParameters Parameters { get; set; }

    }

    public static class MathExtensions {
        public static Derivative AsDerivative(this IExpression expression, Differentiator differentiator) {
            return new Derivative(differentiator, simplifier, expression);
        }

        public static Derivative AsDerivative(this IExpression expression, Differentiator differentiator, Variable variable) {
            return new Derivative(differentiator, simplifier, expression, variable);
        }

        public static IExpression AsAnalyzedExpression(this IExpression expression, Differentiator differentiator) {
            return AsAnalyzedExpression(expression, differentiator, differentiator.Variable);
        }

        public static IExpression AsAnalyzedExpression(this IExpression expression, Differentiator differentiator, Variable variable) {
            if (expression is Derivative) {
                // TODO Update Derivation Tree
                return expression.Clone().Analyze(differentiator);
            }

            if (expression is UnaryExpression unary){
                var u = (UnaryExpression)unary.Clone();
                u.Argument = u.Argument.AsAnalyzedExpression(differentiator, variable);
                return u;
            } else if (expression is BinaryExpression binary) {
                var b = (BinaryExpression)binary.Clone();
                b.Left = b.Left.AsAnalyzedExpression(differentiator, variable);
                b.Right = b.Right.AsAnalyzedExpression(differentiator, variable);
                return b;
            } else if (expression is DifferentParametersExpression different) {
                var d = (DifferentParametersExpression)different.Clone();
                for (int i = 0; i < d.Arguments.Length; i++) {
                    d.Arguments[i] = d.Arguments[i].AsAnalyzedExpression(differentiator, variable);
                }
                return d;
            }
            return expression;
        }

        private static Simplifier simplifier = new Simplifier();
    }
}


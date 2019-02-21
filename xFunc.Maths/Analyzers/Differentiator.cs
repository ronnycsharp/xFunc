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

        public void SetParent(IExpression child, DerivationStep parentStep) {
            if (child == null || parentStep == null)
                throw new ArgumentNullException();

            if (parents.ContainsKey(child) && parents[child] != parentStep)
                throw new Exception("Key already exists");

            parents[child] = parentStep;
        }

        private Dictionary<IExpression, DerivationStep> parents
            = new Dictionary<IExpression, DerivationStep>(new KeyComparer());

        class KeyComparer : IEqualityComparer<IExpression> {
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
        public override IExpression Analyze(Abs exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var div = new Div(exp.Argument.Clone(), exp.Clone());
            var mul = new Mul(exp.Argument.Clone().AsDerivative(
                    currentStep.Differentiator, 
                    currentStep.Differentiator.Variable),
                div);

            currentStep.Intermediate = mul;

            div = new Div (exp.Argument.Clone (), exp.Clone ());
            mul = new Mul (
                currentStep.AddStep(exp.Argument.Clone ()),
                div);

            currentStep.Derivative = mul;
            return currentStep.SimplifiedDerivative;
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

                currentStep.Intermediate = new Add(leftDeriv, rightDeriv);
                currentStep.Derivative   = new Add(
                    currentStep.AddStep(left), 
                    currentStep.AddStep(right));
                                                
                return currentStep.SimplifiedDerivative;
            }

            if (first) {
                currentStep.Derivative = currentStep.AddStep (exp.Left.Clone ());
                return currentStep.SimplifiedDerivative;
            }

            currentStep.Derivative = currentStep.AddStep (exp.Right.Clone ());
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Derivative exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            currentStep.Intermediate = new NDerivative (
                exp.Expression, 
                new Number(2),  // degree
                new Variable(exp.Variable.Name));

            var derivative = currentStep.AddStep (exp.Expression);
            derivative = currentStep.AddStep (derivative);

            currentStep.Derivative = derivative;
            return currentStep.SimplifiedDerivative;
        }

        public override IExpression Analyze(NDerivative exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            currentStep.Derivative = new NDerivative (
                                            exp.Expression,
                                            new Number (((Number)exp.Number).Value + 1),
                                            exp.Variable);

            return currentStep.Derivative;
        }

        public override IExpression Analyze(DefiniteIntegral exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Constant;
            currentStep.Derivative = new Number (0);
            return currentStep.Derivative;
        }

        public override IExpression Analyze(Sign exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Constant;
            currentStep.Derivative = new Number (0);
            return currentStep.Derivative;
        }

        public override IExpression Analyze(RoundUnary exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Constant;
            currentStep.Derivative = new Number (0);
            return currentStep.Derivative;
        }

        public override IExpression Analyze(Sum exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Sum;

            // check whether the given expression is a constant 
            if (IsConstant (exp.Body)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var body        = exp.Body.Clone ();
            var bodyDeriv   = body.AsDerivative (this);

            currentStep.Intermediate = new Sum (
                bodyDeriv,
                exp.From.Clone(),
                exp.To.Clone(),
                exp.Increment.Clone(),
                exp.Variable.Clone() as Variable);

            currentStep.Derivative = new Sum (
                currentStep.AddStep(exp.Body.Clone()),
                exp.From.Clone(),
                exp.To.Clone(),
                exp.Increment.Clone(),
                exp.Variable.Clone() as Variable);

            return currentStep.SimplifiedDerivative;
        }

        public override IExpression Analyze(MultiCondition exp) {
            var currentStep = FindCurrentStep (exp);

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var conditions = new List<Condition> ();
            foreach (var condition in exp.Arguments.OfType<Condition> ()) {
                conditions.Add (
                    new Condition (
                        new []{
                            condition.Expression.Clone().AsDerivative( 
                                currentStep.Differentiator, 
                                currentStep.Differentiator.Variable),
                            condition.ConditionLogic.Clone()
                        }, 2));
            }

            currentStep.Intermediate = new MultiCondition (conditions.ToArray ());

            conditions = new List<Condition>();
            foreach (var condition in exp.Arguments.OfType<Condition>()) {
                conditions.Add(
                    new Condition(
                        new[]{
                            currentStep.AddStep(condition.Expression.Clone()),
                            condition.ConditionLogic.Clone()
                        }, 2));
            }

            currentStep.Derivative = new MultiCondition (conditions.ToArray ());
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Div exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Quotient;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule        = DerivationRule.Constant;
                currentStep.Derivative  = new Number (0);
                return currentStep.Derivative;
            }

            var first   = Helpers.HasVariable(exp.Left, Variable);
            var second  = Helpers.HasVariable(exp.Right, Variable);

            if (first && second) {
                var mul1        = new Mul(exp.Left.Clone().AsDerivative(this), exp.Right.Clone());
                var mul2        = new Mul(exp.Left.Clone(), exp.Right.Clone().AsDerivative(this));
                var sub         = new Sub(mul1, mul2);
                var inv         = new Pow(exp.Right.Clone(), new Number(2));
                var division    = new Div(sub, inv);

                currentStep.Intermediate = division;

                mul1        = new Mul (currentStep.AddStep(exp.Left.Clone ()), exp.Right.Clone ());
                mul2        = new Mul (exp.Left.Clone (), currentStep.AddStep( exp.Right.Clone ()));
                sub         = new Sub (mul1, mul2);
                inv         = new Pow (exp.Right.Clone (), new Number (2));
                division    = new Div (sub, inv);

                currentStep.Derivative = division;
                return currentStep.SimplifiedDerivative;
            }

            if (second) {
                var mul2        = new Mul(exp.Left.Clone(), exp.Right.Clone().AsDerivative(this));
                var unMinus     = new UnaryMinus(mul2);
                var inv         = new Pow(exp.Right.Clone(), new Number(2));
                var division    = new Div(unMinus, inv);

                currentStep.Intermediate = division;

                mul2        = new Mul (exp.Left.Clone (), currentStep.AddStep( exp.Right.Clone ()));
                unMinus     = new UnaryMinus (mul2);
                inv         = new Pow (exp.Right.Clone (), new Number (2));
                division    = new Div (unMinus, inv);

                currentStep.Derivative = division;
                return currentStep.SimplifiedDerivative;
            }

            currentStep.Intermediate    = new Div (exp.Left.Clone ().AsDerivative (this), exp.Right.Clone ());
            currentStep.Derivative      = new Div (currentStep.AddStep(exp.Left.Clone ()), exp.Right.Clone ());

            //if (first)
            return currentStep.SimplifiedDerivative;
        }


        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Exp exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule        = DerivationRule.Constant;
                currentStep.Derivative  = new Number (0);
                return currentStep.Derivative;
            }

            currentStep.Derivative = new Mul (currentStep.AddStep(exp.Argument.Clone ()), exp.Clone ());
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Lb exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var ln  = new Ln(new Number(2));
            var mul = new Mul(exp.Argument.Clone(), ln);
            var div = new Div(exp.Argument.Clone().AsDerivative(this), mul);

            currentStep.Intermediate = div;

            ln  = new Ln (new Number (2));
            mul = new Mul (exp.Argument.Clone (), ln);
            div = new Div (currentStep.AddStep(exp.Argument.Clone ()), mul);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Lg exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var ln      = new Ln(new Number(10));
            var mul1    = new Mul(exp.Argument.Clone(), ln);
            var div     = new Div(exp.Argument.Clone().AsDerivative(this), mul1);

            currentStep.Intermediate = div;

            ln      = new Ln (new Number (10));
            mul1    = new Mul (exp.Argument.Clone (), ln);
            div     = new Div (currentStep.AddStep(exp.Argument.Clone ()), mul1);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Ln exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            currentStep.Intermediate    = new Div (exp.Argument.Clone ().AsDerivative (this), exp.Argument.Clone ());
            currentStep.Derivative      = new Div (currentStep.AddStep(exp.Argument.Clone ()), exp.Argument.Clone ());
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Log exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            if (Helpers.HasVariable(exp.Left, Variable)) {
                var ln1 = new Ln(exp.Right.Clone());
                var ln2 = new Ln(exp.Left.Clone());
                var div = new Div(ln1, ln2);

                return Analyze(div);
            }

            // if (Helpers.HasVar(exp.Right, variable))
            var ln      = new Ln (exp.Left.Clone ());
            var mul     = new Mul (exp.Right.Clone (), ln);
            var div2    = new Div (exp.Right.Clone ().AsDerivative(this), mul);

            currentStep.Intermediate = div2;

            ln      = new Ln(exp.Left.Clone());
            mul     = new Mul(exp.Right.Clone(), ln);
            div2    = new Div(currentStep.AddStep(exp.Right.Clone()), mul);

            currentStep.Derivative = div2;
            return currentStep.SimplifiedDerivative;
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

                mul1 = new Mul (currentStep.AddStep(exp.Left.Clone ()), exp.Right.Clone ());
                mul2 = new Mul (exp.Left.Clone (), currentStep.AddStep(exp.Right.Clone ()));
                add = new Add (mul1, mul2);

                currentStep.Derivative  = add;
                currentStep.Rule        = DerivationRule.Product;

                return currentStep.SimplifiedDerivative;
            }

            if (first) {
                currentStep.Rule = DerivationRule.Factor;
                currentStep.Intermediate = new Mul(exp.Left.Clone().AsDerivative(this), exp.Right.Clone());
                currentStep.Derivative = new Mul(currentStep.AddStep(exp.Left.Clone()), exp.Right.Clone());
                return currentStep.SimplifiedDerivative;
            }

            // if (second)
            currentStep.Rule            = DerivationRule.Factor;
            currentStep.Intermediate    = new Mul(exp.Left.Clone(), exp.Right.Clone().AsDerivative(this));
            currentStep.Derivative      = new Mul(exp.Left.Clone(), currentStep.AddStep(exp.Right.Clone()));
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Number exp) {
            var step = FindCurrentStep (exp);
            step.Rule = DerivationRule.Constant;
            step.Derivative = new Number (0);
            return step.Derivative;
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
                if ( exp.Left is Variable /* without constant multiplier */ ) {
                    // Power-Rule like x^2 or x^3

                    var sub     = new Sub (exp.Right.Clone (), new Number (1));
                    var inv     = new Pow (exp.Left.Clone (), sub);
                    var mul1    = new Mul (exp.Right.Clone (), inv);

                    currentStep.Rule            = DerivationRule.Power;
                    currentStep.Intermediate    = mul1;    // Intermediate Step

                    sub     = new Sub (exp.Right.Clone (), new Number (1));
                    inv     = new Pow (exp.Left.Clone (), sub);
                    mul1    = new Mul (exp.Right.Clone (), inv);

                    currentStep.Derivative = mul1;
                    return currentStep.SimplifiedDerivative;
                } else {
                    // Chain-Rule like 2x^2

                    var sub = new Sub (exp.Right.Clone (), new Number (1));
                    var inv = new Pow (exp.Left.Clone (), sub);
                    var mul1 = new Mul (exp.Right.Clone (), inv);
                    var mul2 = new Mul (exp.Left.Clone ().AsDerivative (this, this.Variable), mul1);

                    currentStep.Rule = DerivationRule.Chain;
                    currentStep.Intermediate = mul2;    // Intermediate Step

                    sub = new Sub (exp.Right.Clone (), new Number (1));
                    inv = new Pow (exp.Left.Clone (), sub);
                    mul1 = new Mul (exp.Right.Clone (), inv);
                    mul2 = new Mul (currentStep.AddStep (exp.Left.Clone ()), mul1);

                    currentStep.Derivative = mul2;
                    return currentStep.SimplifiedDerivative;
                }
            }

            // if (Helpers.HasVar(exp.Right, variable))
            var ln      = new Ln(exp.Left.Clone());
            var mul3    = new Mul(ln, exp.Clone());
            var mul4    = new Mul(mul3, currentStep.AddStep(exp.Right.Clone()));

            currentStep.Derivative = mul4;
            return currentStep.SimplifiedDerivative;
        }

        Simplifier simplifier = new Simplifier();

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Root exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var div = new Div(new Number(1), exp.Right.Clone());
            var pow = new Pow(exp.Left.Clone(), div);


            currentStep.Derivative = currentStep.AddStep (pow);
            return currentStep.SimplifiedDerivative;
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
        public override IExpression Analyze(Sqrt exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Power;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var mul = new Mul(new Number(2), exp.Clone());
            var div = new Div(exp.Argument.Clone().AsDerivative(this), mul);

            currentStep.Intermediate = div;

            mul = new Mul (new Number (2), exp.Clone ());
            div = new Div (currentStep.AddStep (exp.Argument.Clone ()), mul);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sub exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Difference;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var first   = Helpers.HasVariable(exp.Left, Variable);
            var second  = Helpers.HasVariable(exp.Right, Variable);

            if (first && second) {
                currentStep.Intermediate = new Sub (
                    exp.Left.Clone ().AsDerivative (this),
                    exp.Right.Clone ().AsDerivative (this));

                currentStep.Derivative = new Sub (
                    currentStep.AddStep(exp.Left.Clone ()),
                    currentStep.AddStep(exp.Right.Clone ()));

                return currentStep.SimplifiedDerivative;
            }

            if (first) {
                currentStep.Intermediate = exp.Left.Clone ().AsDerivative (this);
                currentStep.Derivative = currentStep.AddStep(exp.Left.Clone ());
                return currentStep.SimplifiedDerivative;
            }

            // if (second)
            currentStep.Intermediate    = new UnaryMinus(exp.Right.Clone ().AsDerivative (this));
            currentStep.Derivative      = new UnaryMinus (currentStep.AddStep (exp.Right.Clone ()));

            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(UnaryMinus exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            currentStep.Intermediate = new UnaryMinus (exp.Argument.Clone ().AsDerivative(this));

            var arg         = exp.Argument.Clone ();
            var derivedArg  = currentStep.AddStep (arg);
            var derivative = new UnaryMinus (derivedArg);

            currentStep.Derivative = derivative;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(UserFunction exp) {
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
            if (exp.Equals(Variable)) {
                currentStep.Rule = DerivationRule.Variable;
                currentStep.Derivative = new Number(1);
                return currentStep.Derivative;
            }

            // check if the given variable is the root expression,
            // if so, then we must use it as a constant
            if ( currentStep.Parent == null ) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            // Variable is a multiplicative constant
            currentStep.Derivative = exp.Clone();
            return Simplify (currentStep.Derivative);
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(nPr exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Constant;
            currentStep.Derivative = new Number (0);
            return currentStep.Derivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(nCr exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Constant;
            currentStep.Derivative = new Number (0);
            return currentStep.Derivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Rand exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Constant;
            currentStep.Derivative = new Number (0);
            return currentStep.Derivative;
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
        public override IExpression Analyze(Arccos exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var pow         = new Pow(exp.Argument.Clone(), new Number(2));
            var sub         = new Sub(new Number(1), pow);
            var sqrt        = new Sqrt(sub);
            var division    = new Div(exp.Argument.Clone().AsDerivative(this), sqrt);
            var unMinus     = new UnaryMinus(division);

            currentStep.Intermediate = unMinus;

            pow         = new Pow (exp.Argument.Clone (), new Number (2));
            sub         = new Sub (new Number (1), pow);
            sqrt        = new Sqrt (sub);
            division    = new Div (currentStep.AddStep(exp.Argument.Clone ()), sqrt);
            unMinus     = new UnaryMinus (division);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arccot exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var involution  = new Pow(exp.Argument.Clone(), new Number(2));
            var add         = new Add(new Number(1), involution);
            var div         = new Div(exp.Argument.Clone().AsDerivative(this), add);
            var unMinus     = new UnaryMinus(div);

            currentStep.Intermediate = unMinus;

            involution  = new Pow (exp.Argument.Clone (), new Number (2));
            add         = new Add (new Number (1), involution);
            div         = new Div (currentStep.AddStep(exp.Argument.Clone ()), add);
            unMinus     = new UnaryMinus (div);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arccsc exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var abs = new Abs(exp.Argument.Clone());
            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(sqr, new Number(1));
            var sqrt = new Sqrt(sub);
            var mul = new Mul(abs, sqrt);
            var div = new Div(exp.Argument.Clone().AsDerivative(this), mul);
            var unary = new UnaryMinus(div);

            currentStep.Intermediate = unary;

            abs     = new Abs (exp.Argument.Clone ());
            sqr     = new Pow (exp.Argument.Clone (), new Number (2));
            sub     = new Sub (sqr, new Number (1));
            sqrt    = new Sqrt (sub);
            mul     = new Mul (abs, sqrt);
            div     = new Div (currentStep.AddStep(exp.Argument.Clone ()), mul);
            unary   = new UnaryMinus (div);

            currentStep.Derivative = unary;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcsec exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var abs     = new Abs(exp.Argument.Clone());
            var sqr     = new Pow(exp.Argument.Clone(), new Number(2));
            var sub     = new Sub(sqr, new Number(1));
            var sqrt    = new Sqrt(sub);
            var mul     = new Mul(abs, sqrt);
            var div     = new Div(exp.Argument.Clone().AsDerivative(this), mul);

            currentStep.Intermediate = div;

            abs     = new Abs (exp.Argument.Clone ());
            sqr     = new Pow (exp.Argument.Clone (), new Number (2));
            sub     = new Sub (sqr, new Number (1));
            sqrt    = new Sqrt (sub);
            mul     = new Mul (abs, sqrt);
            div     = new Div (currentStep.AddStep(exp.Argument.Clone ()), mul);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcsin exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var involution  = new Pow(exp.Argument.Clone(), new Number(2));
            var sub         = new Sub(new Number(1), involution);
            var sqrt        = new Sqrt(sub);
            var division    = new Div(exp.Argument.Clone().AsDerivative(this), sqrt);

            currentStep.Intermediate = division;

            involution  = new Pow (exp.Argument.Clone (), new Number (2));
            sub         = new Sub (new Number (1), involution);
            sqrt        = new Sqrt (sub);
            division    = new Div (currentStep.AddStep(exp.Argument.Clone ()), sqrt);

            currentStep.Derivative = division;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arctan exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var involution  = new Pow(exp.Argument.Clone(), new Number(2));
            var add         = new Add(new Number(1), involution);
            var div         = new Div(exp.Argument.Clone().AsDerivative(this), add);

            currentStep.Intermediate = div;

            involution  = new Pow (exp.Argument.Clone (), new Number (2));
            add         = new Add (new Number (1), involution);
            div         = new Div (currentStep.AddStep(exp.Argument.Clone ()), add);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Cos exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sine            = new Sin (exp.Argument.Clone ());
            var multiplication  = new Mul (sine, exp.Argument.Clone ().AsDerivative (this));
            var unMinus         = new UnaryMinus (multiplication);

            currentStep.Intermediate = unMinus;

            sine            = new Sin(exp.Argument.Clone());
            multiplication  = new Mul (sine, currentStep.AddStep (exp.Argument.Clone ()));
            unMinus         = new UnaryMinus(multiplication);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Cot exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sine        = new Sin(exp.Argument.Clone());
            var involution  = new Pow(sine, new Number(2));
            var division    = new Div(exp.Argument.Clone().AsDerivative(this), involution);
            var unMinus     = new UnaryMinus(division);

            currentStep.Intermediate = unMinus;

            sine            = new Sin (exp.Argument.Clone ());
            involution      = new Pow (sine, new Number (2));
            division        = new Div (currentStep.AddStep(exp.Argument.Clone ()), involution);
            unMinus         = new UnaryMinus (division);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Csc exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var unary   = new UnaryMinus (exp.Argument.Clone ().AsDerivative(this));
            var cot     = new Cot (exp.Argument.Clone ());
            var csc     = new Csc (exp.Argument.Clone ());
            var mul1    = new Mul (cot, csc);
            var mul2    = new Mul (unary, mul1);

            currentStep.Intermediate = mul2;

            unary   = new UnaryMinus(currentStep.AddStep(exp.Argument.Clone()));
            cot     = new Cot(exp.Argument.Clone());
            csc     = new Csc(exp.Argument.Clone());
            mul1    = new Mul(cot, csc);
            mul2    = new Mul(unary, mul1);

            currentStep.Derivative = mul2;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sec exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule        = DerivationRule.Constant;
                currentStep.Derivative  = new Number (0);
                return currentStep.Derivative;
            }

            var tan = new Tan (exp.Argument.Clone ());
            var sec = new Sec (exp.Argument.Clone ());
            var mul1 = new Mul (tan, sec);
            var mul2 = new Mul (exp.Argument.Clone ().AsDerivative(this), mul1);

            currentStep.Intermediate = mul2;

            tan     = new Tan(exp.Argument.Clone());
            sec     = new Sec(exp.Argument.Clone());
            mul1    = new Mul(tan, sec);
            mul2    = new Mul(currentStep.AddStep(exp.Argument.Clone()), mul1);

            currentStep.Derivative = mul2;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sin exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var cos = new Cos (exp.Argument.Clone ());
            var mul = new Mul (cos, exp.Argument.Clone ().AsDerivative(
                currentStep.Differentiator, 
                currentStep.Differentiator.Variable));

            currentStep.Intermediate = mul;

            cos = new Cos(exp.Argument.Clone());
            mul = new Mul(cos, currentStep.AddStep( exp.Argument.Clone()));

            currentStep.Derivative = mul;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Tan exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var cos = new Cos (exp.Argument.Clone ());
            var inv = new Pow (cos, new Number (2));
            var div = new Div (exp.Argument.Clone ().AsDerivative(this), inv);

            currentStep.Intermediate = div;

            cos = new Cos(exp.Argument.Clone());
            inv = new Pow(cos, new Number(2));
            div = new Div(currentStep.AddStep(exp.Argument.Clone()), inv);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
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
        public override IExpression Analyze(Arcosh exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sqr     = new Pow (exp.Argument.Clone (), new Number (2));
            var sub     = new Sub (sqr, new Number (1));
            var sqrt    = new Sqrt (sub);
            var div     = new Div (exp.Argument.Clone ().AsDerivative (this), sqrt);

            currentStep.Intermediate = div;

            sqr     = new Pow(exp.Argument.Clone(), new Number(2));
            sub     = new Sub(sqr, new Number(1));
            sqrt    = new Sqrt(sub);
            div     = new Div(currentStep.AddStep(exp.Argument.Clone()), sqrt);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcoth exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), sqr);
            var div = new Div(exp.Argument.Clone().AsDerivative(this), sub);

            currentStep.Intermediate = div;

            sqr = new Pow (exp.Argument.Clone (), new Number (2));
            sub = new Sub (new Number (1), sqr);
            div = new Div (currentStep.AddStep(exp.Argument.Clone ()), sub);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arcsch exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var inv     = new Pow (exp.Argument.Clone (), new Number (2));
            var add     = new Add (new Number (1), inv);
            var sqrt    = new Sqrt (add);
            var abs     = new Abs (exp.Argument.Clone ());
            var mul     = new Mul (abs, sqrt);
            var div     = new Div (exp.Argument.Clone ().AsDerivative (this), mul);
            var unMinus = new UnaryMinus (div);

            currentStep.Intermediate = unMinus;

            inv     = new Pow(exp.Argument.Clone(), new Number(2));
            add     = new Add(new Number(1), inv);
            sqrt    = new Sqrt(add);
            abs     = new Abs(exp.Argument.Clone());
            mul     = new Mul(abs, sqrt);
            div     = new Div(currentStep.AddStep(exp.Argument.Clone()), mul);
            unMinus = new UnaryMinus(div);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arsech exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            var inv     = new Pow (exp.Argument.Clone (), new Number (2));
            var sub     = new Sub (new Number (1), inv);
            var sqrt    = new Sqrt (sub);
            var mul     = new Mul (exp.Argument.Clone (), sqrt);
            var div     = new Div (currentStep.AddStep(exp.Argument.Clone ()), mul);
            var unMinus = new UnaryMinus (div);

            currentStep.Derivative = unMinus;

            inv     = new Pow(exp.Argument.Clone(), new Number(2));
            sub     = new Sub(new Number(1), inv);
            sqrt    = new Sqrt(sub);
            mul     = new Mul(exp.Argument.Clone(), sqrt);
            div     = new Div(exp.Argument.Clone().AsDerivative(this), mul);
            unMinus = new UnaryMinus(div);

            currentStep.Intermediate = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Arsinh exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sqr     = new Pow(exp.Argument.Clone(), new Number(2));
            var add     = new Add(sqr, new Number(1));
            var sqrt    = new Sqrt(add);
            var div     = new Div(exp.Argument.Clone().AsDerivative(this), sqrt);

            currentStep.Intermediate = div;

            sqr     = new Pow (exp.Argument.Clone (), new Number (2));
            add     = new Add (sqr, new Number (1));
            sqrt    = new Sqrt (add);
            div     = new Div (currentStep.AddStep(exp.Argument.Clone ()), sqrt);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Artanh exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sqr = new Pow(exp.Argument.Clone(), new Number(2));
            var sub = new Sub(new Number(1), sqr);
            var div = new Div(exp.Argument.Clone().AsDerivative(this), sub);

            currentStep.Intermediate = div;

            sqr = new Pow (exp.Argument.Clone (), new Number (2));
            sub = new Sub (new Number (1), sqr);
            div = new Div (currentStep.AddStep(exp.Argument.Clone ()), sub);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Cosh exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sinh    = new Sinh(exp.Argument.Clone());
            var mul     = new Mul(exp.Argument.Clone().AsDerivative(this), sinh);

            currentStep.Intermediate = mul;

            sinh    = new Sinh (exp.Argument.Clone ());
            mul     = new Mul (currentStep.AddStep(exp.Argument.Clone ()), sinh);

            currentStep.Derivative = mul;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Coth exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var sinh    = new Sinh(exp.Argument.Clone());
            var inv     = new Pow(sinh, new Number(2));
            var div     = new Div(exp.Argument.Clone().AsDerivative(this), inv);
            var unMinus = new UnaryMinus(div);

            currentStep.Intermediate = unMinus;

            sinh        = new Sinh (exp.Argument.Clone ());
            inv         = new Pow (sinh, new Number (2));
            div         = new Div (currentStep.AddStep(exp.Argument.Clone ()), inv);
            unMinus     = new UnaryMinus (div);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Csch exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var coth    = new Coth(exp.Argument.Clone());
            var mul1    = new Mul(coth, exp.Clone());
            var mul2    = new Mul(exp.Argument.Clone().AsDerivative(this), mul1);
            var unMinus = new UnaryMinus(mul2);

            currentStep.Intermediate = unMinus;

            coth    = new Coth (exp.Argument.Clone ());
            mul1    = new Mul (coth, exp.Clone ());
            mul2    = new Mul (currentStep.AddStep(exp.Argument.Clone ()), mul1);
            unMinus = new UnaryMinus (mul2);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sech exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var tanh    = new Tanh(exp.Argument.Clone());
            var mul1    = new Mul(tanh, exp.Clone());
            var mul2    = new Mul(exp.Argument.Clone().AsDerivative(this), mul1);
            var unMinus = new UnaryMinus(mul2);

            currentStep.Intermediate = unMinus;

            tanh    = new Tanh (exp.Argument.Clone ());
            mul1    = new Mul (tanh, exp.Clone ());
            mul2    = new Mul (currentStep.AddStep(exp.Argument.Clone ()), mul1);
            unMinus = new UnaryMinus (mul2);

            currentStep.Derivative = unMinus;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Sinh exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule        = DerivationRule.Constant;
                currentStep.Derivative  = new Number (0);

                return currentStep.Derivative;
            }

            var cosh    = new Cosh(exp.Argument.Clone());
            var mul     = new Mul(exp.Argument.Clone().AsDerivative(this), cosh);

            currentStep.Intermediate = mul;

            cosh    = new Cosh (exp.Argument.Clone ());
            mul     = new Mul (currentStep.AddStep(exp.Argument.Clone ()), cosh);

            currentStep.Derivative = mul;
            return currentStep.SimplifiedDerivative;
        }

        /// <summary>
        /// Analyzes the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>
        /// The result of analysis.
        /// </returns>
        public override IExpression Analyze(Tanh exp) {
            var currentStep = FindCurrentStep (exp);
            currentStep.Rule = DerivationRule.Other;

            // check whether the given expression is a constant 
            if (IsConstant (exp)) {
                currentStep.Rule = DerivationRule.Constant;
                currentStep.Derivative = new Number (0);
                return currentStep.Derivative;
            }

            var cosh    = new Cosh (exp.Argument.Clone ());
            var inv     = new Pow (cosh, new Number (2));
            var div     = new Div (exp.Argument.Clone ().AsDerivative(this), inv);

            currentStep.Intermediate = div;

            cosh        = new Cosh(exp.Argument.Clone());
            inv         = new Pow(cosh, new Number(2));
            div         = new Div (currentStep.AddStep (exp.Argument.Clone ()), inv);

            currentStep.Derivative = div;
            return currentStep.SimplifiedDerivative;
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


        IExpression Simplify(IExpression exp) {
            return exp; // exp.Clone ().Analyze (simplifier);
        }
    }

    public static class MathExtensions {
        public static Derivative AsDerivative(this IExpression expression, Differentiator differentiator) {
            var variable    = new Variable (differentiator.Variable.Name);
            var derivative = new Derivative (differentiator, simplifier, expression, variable);
            variable.Parent = derivative;
            return derivative;
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


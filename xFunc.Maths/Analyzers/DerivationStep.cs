// Copyright 2013-2019 Ronny Weidemann

using System.Collections.Generic;
using xFunc.Maths.Expressions;

namespace xFunc.Maths.Analyzers {
    public class DerivationStep {
        public DerivationStep (Differentiator differentiator, Simplifier simplifier = null) {
            this.Differentiator = differentiator;
            this.Simplifier     = simplifier ?? new Simplifier ();
            this.Substeps       = new List<DerivationStep> ();
        }

        #region Properties

        public string Name { get; set; }

        public Differentiator Differentiator { get; set; }

        public Simplifier Simplifier { get; set; }

        public DerivationStep Parent { get; set; }

        public DerivationRule Rule { get; set; } = DerivationRule.Other;

        public IExpression Expression { get; set; }

        public IExpression Derivative { get; set; }

        public IExpression Simplified { get; private set; }

        /// <summary>
        /// Gets or sets the intermediate step
        /// </summary>
        /// <value>The expression of the intermediate step.</value>
        public IExpression Intermediate {
            get { return intermediate; }
            set {
                if (intermediate != value) {
                    intermediate = value;
                    this.Derivative = intermediate?.AsAnalyzedExpression (this.Differentiator);
                    this.Simplified = this.Derivative?.Analyze (this.Simplifier);
                }
            }
        }

        public List<DerivationStep> Substeps { get; private set; }

        #endregion

        public IExpression SetDerivative (IExpression derivative, DerivationRule rule = DerivationRule.Other) {
            this.Derivative = derivative;
            this.Rule = rule;
            return derivative;
        }

        public IExpression AddStep (
            IExpression expression,
            IExpression derivative,
            DerivationRule rule = DerivationRule.Other)
        {
            this.Substeps.Add (
                new DerivationStep (Differentiator) {
                    Parent = this,
                    Expression = expression,
                    Derivative = derivative,
                    Rule = rule
                });
            return derivative;
        }

        public IExpression AddStep (IExpression expression, DerivationRule rule = DerivationRule.Other) {
            var derivative = expression.Analyze (this.Differentiator);
            return AddStep (expression, derivative, rule);
        }

        public override string ToString () {
            return Expression.ToString () + "->" + Derivative.ToString ();
        }

        private IExpression intermediate;
    }

    public enum DerivationRule {
        Constant,   // Ableitung einer Konstante -> 0
        Variable,   // Ableitung einer Variable	 -> 1
        Sum,        // Summenregel
        Difference, // Differenzregel
        Product,    // Produktregel
        Factor,     // Faktorregel
        Quotient,   // Quotientenregel
        Chain,      // Kettenregel
        Power,      // Potenzregel
        Reciprocal, // Reziprogenregel
        Other,
    }
}
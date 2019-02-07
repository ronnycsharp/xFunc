// Copyright 2013-2019 Ronny Weidemann

using System.Collections.Generic;
using System.ComponentModel;
using xFunc.Maths.Expressions;

namespace xFunc.Maths.Analyzers {
    public class DerivationStep : INotifyPropertyChanged {
        public DerivationStep (Differentiator differentiator, Simplifier simplifier = null) {
            this.Differentiator = differentiator;
            this.Simplifier     = simplifier ?? new Simplifier ();
            this.Substeps       = new List<DerivationStep> ();
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title {
            get {
                // TODO Language-Support

                switch(this.Rule) {
                case DerivationRule.Constant: {
                        return "Ableitung einer Konstante";
                    }
                case DerivationRule.Chain: {
                        return "Kettenregel anwenden";
                    }
                case DerivationRule.Variable: {
                        return "Ableitung einer Variable";
                    }
                case DerivationRule.Factor: {
                        return "Faktorregel anwenden";
                    }
                case DerivationRule.Power: {
                        return "Potenzregel anwenden";
                    }
                case DerivationRule.Product: {
                        return "Produktregel anwenden";
                    }
                case DerivationRule.Quotient: {
                        return "Quotientenregel anwenden";
                    }
                case DerivationRule.Reciprocal: {
                        return "Reziprogenregel anwenden";
                    }
                case DerivationRule.Sum: {
                        return "Summenregel anwenden";
                    }
                case DerivationRule.Difference: {
                        return "Differenzregel anwenden";
                    }
                }
                return "Nebenrechnung";
            }
        }

        /// <summary>
        /// Gets or sets the differentiator.
        /// </summary>
        /// <value>The differentiator.</value>
        public Differentiator Differentiator { get; set; }

        /// <summary>
        /// Gets or sets the simplifier.
        /// </summary>
        /// <value>The simplifier.</value>
        public Simplifier Simplifier { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public DerivationStep Parent { get; set; }

        /// <summary>
        /// Gets or sets the derivation rule.
        /// </summary>
        /// <value>The rule.</value>
        public DerivationRule Rule { get; set; } = DerivationRule.Other;

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        /// <summary>
        public IExpression Expression {
            get { return expression; }
            set {
                if (expression != value) {
                    expression = value;
                    this.PropertyChanged?.Invoke (
                        this, new PropertyChangedEventArgs (
                            nameof (Expression)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the derivative.
        /// </summary>
        /// <value>The derivative.</value>
        public IExpression Derivative { 
            get { return derivative; }
            set {
                if ( derivative != value ) {
                    derivative = value;
                    this.PropertyChanged?.Invoke (
                        this, new PropertyChangedEventArgs (
                            nameof (Derivative)));

                    // update the simplified derivative
                    this.SimplifiedDerivative = derivative?.Clone().Analyze (this.Simplifier);
                    this.PropertyChanged?.Invoke (
                        this, new PropertyChangedEventArgs (
                            nameof (SimplifiedDerivative)));
                }
            }
        }

        /// <summary>
        /// Gets the simplified derivative.
        /// </summary>
        /// <value>The simplified derivative.</value>
        public IExpression SimplifiedDerivative { get; private set; }

        /// <summary>
        /// Gets or sets the intermediate step
        /// </summary>
        /// <value>The intermediate step contains derivatives whose result has not been yet calculated.</value>
        public IExpression Intermediate { 
            get { return intermediate; } 
            set { 
                if ( intermediate != value ) {
                    intermediate = value;
                    this.PropertyChanged?.Invoke (
                        this, new PropertyChangedEventArgs (
                            nameof (Intermediate)));

                    // update the simplified intermediate step
                    this.SimplifiedIntermediate = intermediate?.Clone().Analyze (this.Simplifier);
                        this.PropertyChanged?.Invoke (
                            this, new PropertyChangedEventArgs (
                                nameof (SimplifiedIntermediate)));
                }
            }
        }

        /// <summary>
        /// Gets the simplified intermediate step.
        /// </summary>
        /// <value>The simplified intermediate.</value>
        public IExpression SimplifiedIntermediate { get; private set; }

        /// <summary>
        /// Returns true, if the derivation has a simplified value.
        /// </summary>
        /// <value><c>true</c> if has simplified derivation; otherwise, <c>false</c>.</value>
        public bool HasSimplifiedDerivation {
            get {
                return this.Derivative != null 
                    && !this.Derivative.Equals (this.SimplifiedDerivative);
            }
        }

        /// <summary>
        /// Returns true, if the intermediate step has a simplified value.
        /// </summary>
        /// <value><c>true</c> if has simplified intermediate; otherwise, <c>false</c>.</value>
        public bool HasSimplifiedIntermediate {
            get {
                return this.Intermediate != null
                    && !this.Intermediate.Equals (this.SimplifiedIntermediate);
            }
        }

        /// <summary>
        /// Gets the sub derivation steps.
        /// </summary>
        /// <value>The substeps.</value>
        public List<DerivationStep> Substeps { get; private set; }

        #endregion

        /// <summary>
        /// Adds a sub derivation step to substeps-list
        /// </summary>
        /// <returns>Returns the derivative of the given expression</returns>
        /// <param name="expression">Expression.</param>
        /// <param name="rule">Rule.</param>
        public IExpression AddStep (IExpression expression, DerivationRule rule = DerivationRule.Other) {
            var substep = new DerivationStep(this.Differentiator) { 
                Parent = this,
                Expression = expression,
                Rule = rule,
            };

            this.Differentiator.SetParent(expression, substep);

            // derive expression
            substep.Derivative = expression.Analyze (this.Differentiator);
            this.Substeps.Add(substep);
            return substep.SimplifiedDerivative;
        }

        public override string ToString () {
            return Expression.ToString () + "->" + Derivative.ToString ();
        }

        private IExpression intermediate;
        private IExpression derivative;
        private IExpression expression;
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
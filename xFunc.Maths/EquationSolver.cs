// Copyright 2013-2018 Ronny Weidemann

using System;
using System.Linq;
using System.Collections.Generic;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;
using xFunc.Maths.Expressions.Programming;

namespace xFunc.Maths {
    public class EquationSolver {
        public EquationSolver(
            string equation, 
            string variable = "x",  
            AngleMeasurement measurement = AngleMeasurement.Radian) {

            if (String.IsNullOrWhiteSpace(equation))
                throw new ArgumentException("Invalid equation");

            if (String.IsNullOrWhiteSpace(variable))
                throw new ArgumentException("Invalid variable");

            this.Measurement    = measurement;
            this.Equation       = equation;
          
            var processor       = new Processor();
            var splitted        = equation.Split('=');

            if (splitted.Length == 0)
                throw new ArgumentException("The equation must contain an equal sign.");

            var left            = splitted[0];
            var right           = splitted[1];

            this.LeftExpression     = processor.Parse(left);
            this.RightExpression    = processor.Parse(right);

            var parameters  = new ExpressionParameters(measurement);
            var paramX      = new Parameter(variable, 0);
            var paramT      = new Parameter("t", 0);

            parameters.Variables.Add(paramX);
            parameters.Variables.Add(paramT);

            this.Time       = paramT;
            this.Variable   = paramX;
            this.Parameters = parameters;
        }

        public EquationSolver(
            IExpression expression,
            string variable = "x",
            AngleMeasurement measurement = AngleMeasurement.Radian) {

            if (expression==null)
                throw new ArgumentNullException(nameof(expression));

            if (!(expression is Equal eq))
                throw new ArgumentException("Invalid type of expression");

            if (String.IsNullOrWhiteSpace(variable))
                throw new ArgumentException("Invalid variable");

            this.Measurement    = measurement;
            this.Equation       = expression.ToString();

            this.LeftExpression     = eq.Left;
            this.RightExpression    = eq.Right;

            var parameters  = new ExpressionParameters(measurement);
            var paramX      = new Parameter(variable, 0);
            var paramT      = new Parameter("t", 0);

            parameters.Variables.Add(paramX);
            parameters.Variables.Add(paramT);

            this.Time       = paramT;
            this.Variable   = paramX;
            this.Parameters = parameters;
        }

        #region Properties

        public string Equation { get; private set; }

        public Parameter Variable { get; private set; }

        public Parameter Time { get; private set; }

        public ExpressionParameters Parameters { get; private set; }

        public AngleMeasurement Measurement { get; private set; }

        public IExpression LeftExpression { get; private set; }

        public IExpression RightExpression { get; private set; }

        #endregion

        public Point[] Solve(double from, double to, int decimalPlaces = 5) {
            return SolveWithNewtonRaphson(from, to, decimalPlaces).ToArray();
        }

        public Point[] SolveWithNewtonRaphson(
            double from, double to, int decimalPlaces = 5) {

            // transform equation to the left side, because the newton-raphson-method
            // is searching for zero-points.

            var equation    = "(" + this.LeftExpression.ToString() + ")-(" + this.RightExpression.ToString() + ")";

            var rootFinder  = new RootFinder(equation, this.Variable.Key, this.Measurement);
            var values      = rootFinder.FindRootsWithNewtonRaphson(from, to, decimalPlaces, 120);

            // the newton-method sometimes returns invalid values,
            // especially if the equation doesn't have any result
            // and therefore we have to validate these results

            var results = new List<Point>();
            foreach (var x in values) {
                this.Variable.Value = x;

                var leftResult      = double.NaN;
                var rightResult     = double.NaN;

                var objLeftResult   = this.LeftExpression.Execute (this.Parameters);
                var objRightResult  = this.RightExpression.Execute (this.Parameters);

                if (objLeftResult is Double)
                    leftResult = (double)objLeftResult;
                else if (objLeftResult is System.Numerics.Complex cmp) {
                    leftResult = double.NaN; //cmp.Real;
                }

                if (objRightResult is Double)
                    rightResult = (double)objRightResult;
                else if (objRightResult is System.Numerics.Complex cmp) {
                    rightResult = double.NaN; // cmp.Real;
                }

                if (double.IsNaN (leftResult) || double.IsNaN (rightResult))
                    continue;

                // to be a valid result, 
                // the computed left part of the equation 
                // must be equal the computed right part

                if (Helpers.AlmostEqual(leftResult, rightResult, 1 / Math.Pow(10, decimalPlaces-1))) {
                    results.Add(new Point(x, rightResult));
                }
            }
            return results.ToArray();
        }

        private struct Vector3 {
            public Vector3(double x, double y, double z) {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public double X;
            public double Y;
            public double Z;

            public double Distance {
                get {
                    return Math.Abs(this.Y - this.Z);
                }
            }
        }

        public Point[] SolveWithSquareScanApproximation(
            double from = -10, double to = 10, double delta = 0.00001,
            int decimalPlaces = 4) {

            /*
            * 1. Alle Quadranten in Quadraten durchlaufen, wobei die Quadrate mit jeder Rekursion
            *    kleiner werden.
            * 2. Im ersten Schritt haben die Quadrate eine Länge von eins, daraus resultierend ergibt
            *    sich bei X/Y von -5 bis +5  10*10=100
            * 3. Wird in dem Quadrat ein Ergebnis des Linken und Rechten-Gleichungsteils gefunden,
            *    so wird als Ergebnis true zurückgeliefert, sonst false.
            * 
            * Bsp. x²=0, hat einen Schnittpunkt bei X=0
            *    I.   Erstes Quadrat von x -5 bis -4 und y -5 bis -4
            *    II.  Das Ergebnis ist false, somit werden keine kleineren Quadrate gebildet und das nächste Quadrat getestet
            *    III. Zweites Quadrat von x -4 bis -3 und y -5 bis -4 testen
            * 
            * -> Quadrat testen: 
            *    a. Quadrate mit jeweils  1/10 Länge abtasten (Abtastrate X/Y-Achse)
            *    b. fLinks(Xn) > Quadrat.Bottom && fLinks(Xn) <= Quadrat.Y
            *          && fRechts(Xn) > Quadrat.Bottom && fRechts(Xn) <= Quadrat.Y    
            */

            var intersections = Search(from, -5, to, 5, 1);

            Vector3 Compute(double x) {
                var leftResult = (double)this.LeftExpression.Execute(this.Parameters);
                var rightResult = (double)this.RightExpression.Execute(this.Parameters);
                return new Vector3(x, leftResult, rightResult);
            }

            Vector3[] Search(
                double fromX, 
                double fromY, 
                double toExclusiveX, 
                double toExclusiveY, 
                double stepSize) {
                var points = new List<Vector3>();
                for (var y = fromY; y <= toExclusiveY; y+=stepSize) {
                    for (var x = fromX; x <= toExclusiveX; x+=stepSize) {
                        var innerStepSize = stepSize / 10.0;
                        if (ContainsPoint(x, y, x + stepSize, y + stepSize, innerStepSize)) {
                            if ( innerStepSize <= 0.0000001) {
                                points.Add(Compute(x));
                            } else {
                                points.AddRange(
                                    Search(x, y, x + stepSize, y + stepSize, innerStepSize));
                            }
                        }
                    }
                }
                return points.ToArray();
            }

            // Returns true, if at least one point is inside the given rectangle
            bool ContainsPoint(
                double left, 
                double top, 
                double exclusiveRight, 
                double exclusiveBottom, 
                double stepSize) {

                var containsLeft    = false;
                var containsRight   = false;

                for (var x = left; x < exclusiveRight; x += stepSize) {
                    this.Variable.Value = x;

                    var leftResult  = (double)this.LeftExpression.Execute(this.Parameters);
                    var rightResult = (double)this.RightExpression.Execute(this.Parameters);

                    if ( ( leftResult >= top && leftResult <= exclusiveBottom )
                            || Helpers.AlmostEqual(leftResult, exclusiveBottom, 0.000000001)
                            || Helpers.AlmostEqual(leftResult, top, 0.000000001)) {
                        containsLeft = true;
                    }
                    if ((rightResult >= top && rightResult <= exclusiveBottom ) 
                            || Helpers.AlmostEqual(rightResult, exclusiveBottom, 0.000000001) 
                            || Helpers.AlmostEqual(rightResult, top, 0.000000001)) {
                        containsRight = true;

                    }
                    if (containsLeft && containsRight) {
                        return true;
                    }
                }
                return false;
            }

            var results = new List<Point>();
            foreach ( var i in intersections.GroupBy(t => Math.Round(t.X, 2))) {
                var item = i.OrderBy(t => t.Distance).First();
                results.Add(new Point(item.X, item.Y));
            }
            return results.ToArray();
        }

        public Point[] SolveWithChangedSignApproximation(
            double from         = -10, 
            double to           = 10, 
            double delta        = 0.00001,
            int decimalPlaces   = 4) {

            var results     = new List<Point>();
            var lastSign    = -2;
            var step        = .1;

            var first = true;
            for (var x = from; x <= to; x += step) {
                this.Variable.Value = x;

                var y1 = (double)this.LeftExpression.Execute(this.Parameters);
                var y2 = (double)this.RightExpression.Execute(this.Parameters);

                var sign = Math.Sign(y1 - y2);

                if (first) {
                    lastSign = sign;
                    first = false;
                }

                if (lastSign != sign) {
                    if (step <= delta) {
                        results.Add(
                            new Point(
                                Math.Round(x, decimalPlaces), 
                                Math.Round(y2, decimalPlaces)));
                        lastSign = sign;
                        step = .1;
                        continue;
                    }
                    x -= step;
                    step /= 2f;
                }
            }
            return results
                .Distinct()
                .OrderBy(d => d.X )
                .ToArray();

        }
    }
}
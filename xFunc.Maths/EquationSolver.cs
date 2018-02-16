// Copyright 2013-2018 Ronny Weidemann

using System;
using System.Linq;
using System.Collections.Generic;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;
using xFunc.Maths.Expressions.Programming;

namespace xFunc.Maths {
    internal class EquationSolver {
        public EquationSolver(
            string equation, 
            string variable = "x",  
            AngleMeasurement measurement = AngleMeasurement.Radian) {

            this.Measurement    = measurement;
            this.Equation       = equation;
          
            var processor       = new Processor();
            var splitted        = equation.Split('=');
            var left            = splitted[0];
            var right           = splitted[1];

            this.LeftExpression     = processor.Parse(left);
            this.RightExpression    = processor.Parse(right);

            var parameters = new ExpressionParameters(measurement) { };
            var paramX = new Parameter(variable, 0);

            parameters.Variables.Add(paramX);

            this.Variable = paramX;
            this.Parameters = parameters;
        }

        public EquationSolver(
            IExpression equation,
            string variable = "x",
            AngleMeasurement measurement = AngleMeasurement.Radian) {

            if (!(equation is Equal eq))
                throw new ArgumentException();

            this.Measurement = measurement;
            this.Equation = equation.ToString();

            this.LeftExpression = eq.Left;
            this.RightExpression = eq.Right;

            var parameters = new ExpressionParameters(measurement);
            var paramX = new Parameter(variable, 0);

            parameters.Variables.Add(paramX);

            this.Variable = paramX;
            this.Parameters = parameters;
        }

        #region Properties

        public string Equation { get; private set; }

        public Parameter Variable { get; private set; }

        public ExpressionParameters Parameters { get; private set; }

        public AngleMeasurement Measurement { get; private set; }

        public IExpression LeftExpression { get; private set; }

        public IExpression RightExpression { get; private set; }

        #endregion

        public double[] Solve(
            double from = -10,
            double to = 10,
            double delta = 0.00001,
            int decimalPlaces = 4) {
            return this.Intersect(
                    from, to, delta, decimalPlaces)
                .Select(i => i.X)
                .ToArray();
        }

        public Point[] Intersect(
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

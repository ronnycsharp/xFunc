// Copyright 2013-2018 Ronny Weidemann

using System;
using System.Linq;
using System.Collections.Generic;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;
using System.Numerics;

namespace xFunc.Maths {
    public class RootFinder {
        public RootFinder(string function, string variable = "x", AngleMeasurement measurement = AngleMeasurement.Radian) {
            processor = new Processor();
            expression = processor.Parse(function);
            derivation = processor.Differentiate(expression, new Variable(variable));
            parameters = new ExpressionParameters(measurement);
            paramX = new Parameter(variable, 0);
            parameters.Variables.Add(paramX);
        }

        public double[] FindRootsWithNewtonRaphson(
            double from = -10, 
            double to   = +10, 
            int decimalPlaces = 5,
            int iterations = 10) {

            var results = new List<Double>();
            for (var x0 = from; x0 <= to; x0 += 0.1) {
                var result = NewtonRaphson(
                    expression, derivation, parameters, paramX, x0, iterations);

                var roundedResult = Math.Round(result, decimalPlaces);
                if (!Double.IsNaN(result)
                        && roundedResult >= from
                        && roundedResult <= to
                        && !results.Contains(roundedResult)) {
                    results.Add(roundedResult);
                }
            }
            return results
                .Distinct()
                .OrderBy(r => r)
                .ToArray();
        }

        private double NewtonRaphson(
            IExpression expression,
            IExpression derivative,
            ExpressionParameters parameters,
            Parameter paramX,
            double x0,
            int iterations) {

            for (var i = 0; i < iterations; i++) {
                paramX.Value = x0;

                var expResult = expression.Execute(parameters);
                var derResult = derivative.Execute(parameters);

                var dExpResult = Double.NaN;
                var dDerResult = Double.NaN;

                if (expResult is Complex cmp)
                    dExpResult = cmp.Real;
                else if (expResult is Double d)
                    dExpResult = d;

                if (derResult is Complex cmp2)
                    dDerResult = cmp2.Real;
                else if (derResult is Double d)
                    dDerResult = d;

                x0 = x0 - (dExpResult / dDerResult);
            }

            return x0;
        }

        public double[] FindRootsWithNumericalApproximation(
            double from     = -10, 
            double to       = +10, 
            double delta    = 0.00001 ) {

            var lastSign    = -2;
            var step        = 0.1;

            var first   = true;
            var results = new List<double>();

            for (var x = from; x <= to; x += step) {
                paramX.Value = x;

                var result = (double)expression.Execute(parameters);
                var sign = Math.Sign(result);

                if (first) {
                    lastSign = sign;
                    first = false;
                }

                if (lastSign != sign) {
                    if (step < delta) {
                        results.Add(x);
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
                .OrderBy(d => d)
                .ToArray();
        }

        private Parameter               paramX;
        private ExpressionParameters    parameters;
        private Processor               processor;
        private IExpression             expression;
        private IExpression             derivation;
    }
}

﻿// Copyright 2012-2016 Dmitry Kischenko
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
#if !PORTABLE
using System.Threading.Tasks;
#endif
using xFunc.Maths.Resources;

namespace xFunc.Maths.Expressions.Matrices
{

    /// <summary>
    /// Provides extention methods for matrices and vectors.
    /// </summary>
    public static class MatrixExtentions
    {

        /// <summary>
        /// Calculates the absolute value (norm) of vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>Return the absolute value of vector.</returns>
        public static double Abs(this Vector vector, ExpressionParameters parameters)
        {
            return Math.Sqrt(vector.Arguments.Sum(arg => Math.Pow((double)arg.Execute(parameters), 2)));
        }

        /// <summary>
        /// Adds the <paramref name="right"/> vector to the <paramref name="left"/> vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>The sum of matrices.</returns>
        public static Vector Add(this Vector left, Vector right)
        {
            return Add(left, right, null);
        }

        /// <summary>
        /// Adds the <paramref name="right"/> vector to the <paramref name="left"/> vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The sum of matrices.</returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Vector Add(this Vector left, Vector right, ExpressionParameters parameters)
        {
            if (left.ParametersCount != right.ParametersCount)
                throw new ArgumentException(Resource.MatrixArgException);

            var exps = new IExpression[left.ParametersCount];
#if !PORTABLE
            Parallel.For(0, left.ParametersCount,
                i => exps[i] = new Number((double)left.Arguments[i].Execute(parameters) + (double)right.Arguments[i].Execute(parameters))
            );
#else
            for (int i = 0; i < left.ParametersCount; i++)
                exps[i] = new Number((double)left.Arguments[i].Execute(parameters) + (double)right.Arguments[i].Execute(parameters));
#endif

            return new Vector(exps);
        }

        /// <summary>
        /// Subtracts the <paramref name="right"/> vector from the <paramref name="left"/> vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>The difference of matrices.</returns>
        public static Vector Sub(this Vector left, Vector right)
        {
            return Sub(left, right, null);
        }

        /// <summary>
        /// Subtracts the <paramref name="right"/> vector from the <paramref name="left"/> vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The difference of matrices.</returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Vector Sub(this Vector left, Vector right, ExpressionParameters parameters)
        {
            if (left.ParametersCount != right.ParametersCount)
                throw new ArgumentException(Resource.MatrixArgException);

            var exps = new IExpression[left.ParametersCount];
#if !PORTABLE
            Parallel.For(0, left.ParametersCount,
                i => exps[i] = new Number((double)left.Arguments[i].Execute(parameters) - (double)right.Arguments[i].Execute(parameters))
            );
#else
            for (int i = 0; i < left.ParametersCount; i++)
                exps[i] = new Number((double)left.Arguments[i].Execute(parameters) - (double)right.Arguments[i].Execute(parameters));
#endif

            return new Vector(exps);
        }

        /// <summary>
        /// Multiplies <paramref name="vector"/> by <paramref name="number"/>.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="number">The number.</param>
        /// <returns>The product of matrices.</returns>
        public static Vector Mul(this Vector vector, IExpression number)
        {
            return Mul(vector, number, null);
        }

        /// <summary>
        /// Multiplies <paramref name="vector"/> by <paramref name="number"/>.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="number">The number.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The product of matrices.</returns>
        public static Vector Mul(this Vector vector, IExpression number, ExpressionParameters parameters)
        {
            var n = (double)number.Execute(parameters);
#if !PORTABLE
            var numbers = (from num in vector.Arguments.AsParallel().AsOrdered()
                           select new Number((double)num.Execute(parameters) * n))
                          .ToArray();
#else
            var numbers = (from num in vector.Arguments
                           select new Number((double)num.Execute(parameters) * n))
                          .ToArray();
#endif

            return new Vector(numbers);
        }

        /// <summary>
        /// Adds the <paramref name="right"/> matrix to the <paramref name="left"/> matrix.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <returns>The sum of matrices.</returns>
        public static Matrix Add(this Matrix left, Matrix right)
        {
            return Add(left, right, null);
        }

        /// <summary>
        /// Adds the <paramref name="right"/> matrix to the <paramref name="left"/> matrix.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The sum of matrices.</returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Matrix Add(this Matrix left, Matrix right, ExpressionParameters parameters)
        {
            if (left.ParametersCount != right.ParametersCount || left.SizeOfVectors != right.SizeOfVectors)
                throw new ArgumentException(Resource.MatrixArgException);

            var vectors = new Vector[left.ParametersCount];
#if !PORTABLE
            Parallel.For(0, left.ParametersCount, i =>
            {
                var exps = new IExpression[left.SizeOfVectors];

                for (int j = 0; j < left.SizeOfVectors; j++)
                    exps[j] = new Number((double)left[i][j].Execute(parameters) + (double)right[i][j].Execute(parameters));

                vectors[i] = new Vector(exps);
            });
#else
            for (int i = 0; i < left.ParametersCount; i++)
            {
                var exps = new IExpression[left.SizeOfVectors];

                for (int j = 0; j < left.SizeOfVectors; j++)
                    exps[j] = new Number((double)left[i][j].Execute(parameters) + (double)right[i][j].Execute(parameters));

                vectors[i] = new Vector(exps);
            }
#endif

            return new Matrix(vectors);
        }

        /// <summary>
        /// Subtracts the <paramref name="right"/> matrix from the <paramref name="left"/> matrix.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <returns>The difference of matrices.</returns>
        public static Matrix Sub(this Matrix left, Matrix right)
        {
            return Sub(left, right, null);
        }

        /// <summary>
        /// Subtracts the <paramref name="right"/> matrix from the <paramref name="left"/> matrix.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The difference of matrices.</returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Matrix Sub(this Matrix left, Matrix right, ExpressionParameters parameters)
        {
            if (left.ParametersCount != right.ParametersCount || left.SizeOfVectors != right.SizeOfVectors)
                throw new ArgumentException(Resource.MatrixArgException);

            var vectors = new Vector[left.ParametersCount];
#if !PORTABLE
            Parallel.For(0, left.ParametersCount, i =>
            {
                var exps = new IExpression[left.SizeOfVectors];

                for (int j = 0; j < left.SizeOfVectors; j++)
                    exps[j] = new Number((double)left[i][j].Execute(parameters) - (double)right[i][j].Execute(parameters));

                vectors[i] = new Vector(exps);
            });
#else
            for (int i = 0; i < left.ParametersCount; i++)
            {
                var exps = new IExpression[left.SizeOfVectors];

                for (int j = 0; j < left.SizeOfVectors; j++)
                    exps[j] = new Number((double)left[i][j].Execute(parameters) - (double)right[i][j].Execute(parameters));

                vectors[i] = new Vector(exps);
            }
#endif

            return new Matrix(vectors);
        }

        /// <summary>
        /// Multiplies <paramref name="matrix"/> by <paramref name="number"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="number">The number.</param>
        /// <returns>The product of matrix and number.</returns>
        public static Matrix Mul(this Matrix matrix, IExpression number)
        {
            return Mul(matrix, number, null);
        }

        /// <summary>
        /// Multiplies <paramref name="matrix"/> by <paramref name="number"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="number">The number.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The product of matrix and number.</returns>
        public static Matrix Mul(this Matrix matrix, IExpression number, ExpressionParameters parameters)
        {
            var n = (double)number.Execute(parameters);
#if !PORTABLE
            var result = from v in matrix.Arguments.AsParallel().AsOrdered()
                         select new Vector(
                             (from num in ((Vector)v).Arguments
                              select new Number((double)num.Execute(parameters) * n))
                             .ToArray()
                         );
#else
            var result = from v in matrix.Arguments
                         select new Vector(
                             (from num in ((Vector)v).Arguments
                              select new Number((double)num.Execute(parameters) * n))
                             .ToArray()
                         );
#endif

            return new Matrix(result.ToArray());
        }

        /// <summary>
        /// Multiplies the <paramref name="left" /> matrix by the <paramref name="right" /> matrix.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <returns>
        /// The product of matrices.
        /// </returns>
        public static Matrix Mul(this Matrix left, Matrix right)
        {
            return Mul(left, right, null);
        }

        /// <summary>
        /// Multiplies the <paramref name="left" /> matrix by the <paramref name="right" /> matrix.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// The product of matrices.
        /// </returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Matrix Mul(this Matrix left, Matrix right, ExpressionParameters parameters)
        {
            if (left.SizeOfVectors != right.ParametersCount)
                throw new ArgumentException(Resource.MatrixArgException);

            var result = new Matrix(left.ParametersCount, right.SizeOfVectors);
#if !PORTABLE
            Parallel.For(0, right.SizeOfVectors, i =>
            {
                for (int j = 0; j < left.ParametersCount; j++)
                {
                    double el = 0;
                    for (int k = 0; k < left.SizeOfVectors; k++)
                        el += (double)left[j][k].Execute(parameters) * (double)right[k][i].Execute(parameters);
                    result[j][i] = new Number(el);
                }
            });
#else
            for (int i = 0; i < right.SizeOfVectors; i++)
            {
                for (int j = 0; j < left.ParametersCount; j++)
                {
                    double el = 0;
                    for (int k = 0; k < left.SizeOfVectors; k++)
                        el += (double)left[j][k].Execute(parameters) * (double)right[k][i].Execute(parameters);
                    result[j][i] = new Number(el);
                }
            }
#endif

            return result;
        }

        /// <summary>
        /// Multiplies the <paramref name="left" /> vector by the <paramref name="right" /> matrix.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right matrix.</param>
        /// <returns>
        /// The product of matrices.
        /// </returns>
        public static Matrix Mul(this Vector left, Matrix right)
        {
            return Mul(left, right, null);
        }

        /// <summary>
        /// Multiplies the <paramref name="left" /> vector by the <paramref name="right" /> matrix.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// The product of matrices.
        /// </returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Matrix Mul(this Vector left, Matrix right, ExpressionParameters parameters)
        {
            var matrix = new Matrix(new[] { left });

            return matrix.Mul(right, parameters);
        }

        /// <summary>
        /// Multiplies the <paramref name="left" /> matrix by the <paramref name="right" /> vector.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>
        /// The product of matrices.
        /// </returns>
        public static Matrix Mul(this Matrix left, Vector right)
        {
            return Mul(left, right, null);
        }

        /// <summary>
        /// Multiplies the <paramref name="left" /> matrix by the <paramref name="right" /> vector.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>
        /// The product of matrices.
        /// </returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static Matrix Mul(this Matrix left, Vector right, ExpressionParameters parameters)
        {
            var matrix = new Matrix(new[] { right });

            return left.Mul(matrix, parameters);
        }

        /// <summary>
        /// Transposes the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The transposed matrix.</returns>
        public static Matrix Transpose(this Vector vector)
        {
            var vectors = new Vector[vector.ParametersCount];
#if !PORTABLE
            Parallel.For(0, vectors.Length, i => vectors[i] = new Vector(new[] { vector[i] }));
#else
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = new Vector(new[] { vector[i] });
#endif

            return new Matrix(vectors);
        }

        /// <summary>
        /// Transposes the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The transposed matrix.</returns>
        public static IExpression Transpose(this Matrix matrix)
        {
            var result = new Matrix(matrix.SizeOfVectors, matrix.ParametersCount);

#if !PORTABLE
            Parallel.For(0, matrix.ParametersCount, i =>
            {
                for (int j = 0; j < matrix.SizeOfVectors; j++)
                    result[j][i] = matrix[i][j];
            });
#else
            for (int i = 0; i < matrix.ParametersCount; i++)
                for (int j = 0; j < matrix.SizeOfVectors; j++)
                    result[j][i] = matrix[i][j];
#endif

            return result;
        }

        /// <summary>
        /// Calculates a determinant of specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The determinant of matrix.</returns>
        public static double Determinant(this Matrix matrix)
        {
            return Determinant(matrix, null);
        }

        /// <summary>
        /// Calculates a determinant of specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>The determinant of matrix.</returns>
        /// <exception cref="System.ArgumentException">The size of matrices is invalid.</exception>
        public static double Determinant(this Matrix matrix, ExpressionParameters parameters)
        {
            if (!matrix.IsSquare)
                throw new ArgumentException(Resource.MatrixArgException);

            var array = matrix.ToCalculatedArray(parameters);

            return Determinant_(array);
        }

        private static double Determinant_(double[][] matrix)
        {
            if (matrix.Length == 1)
                return matrix[0][0];
            if (matrix.Length == 2)
                return matrix[0][0] * matrix[1][1] - matrix[1][0] * matrix[0][1];

            int[] permutation;
            int toggle;
            var lu = LUPDecomposition_(matrix, out permutation, out toggle);

            if (lu == null)
                throw new MatrixIsInvalidException();

            double result = toggle;
            for (var i = 0; i < lu.Length; i++)
                result *= lu[i][i];

            return result;
        }

        /// <summary>
        /// Decomposes a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <param name="permutation">An array of permutations.</param>
        /// <param name="toggle">Used for calculating a determinant.</param>
        /// <returns>Combined Lower and Upper matrices.</returns>
        public static Matrix LUPDecomposition(this Matrix matrix, ExpressionParameters parameters, out int[] permutation, out int toggle)
        {
            if (!matrix.IsSquare)
                throw new ArgumentException(Resource.MatrixArgException);

            var result = LUPDecomposition_(matrix.ToCalculatedArray(parameters), out permutation, out toggle);
            var m = new Matrix(result.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    m[i][j] = new Number(result[i][j]);

            return m;
        }

        private static double[][] LUPDecomposition_(double[][] matrix, out int[] permutation, out int toggle)
        {
            int size = matrix.Length;

            permutation = new int[size];
            for (var i = 0; i < size; i++)
                permutation[i] = i;

            toggle = 1;

            for (var j = 0; j < size - 1; j++)
            {
                double colMax = Math.Abs(matrix[j][j]);
                int pRow = j;

                for (var i = j + 1; i < size; i++)
                {
                    if (matrix[i][j] > colMax)
                    {
                        colMax = matrix[i][j];
                        pRow = i;
                    }
                }

                if (pRow != j)
                {
                    double[] rowPtr = matrix[pRow];
                    matrix[pRow] = matrix[j];
                    matrix[j] = rowPtr;
                    int tmp = permutation[pRow];
                    permutation[pRow] = permutation[j];
                    permutation[j] = tmp;
                    toggle = -toggle;
                }

                if (matrix[j][j].Equals(0) || Math.Abs(matrix[j][j]) < 1E-14)
                    throw new MatrixIsInvalidException();

                for (var i = j + 1; i < size; i++)
                {
                    matrix[i][j] /= matrix[j][j];
                    for (var k = j + 1; k < size; k++)
                        matrix[i][k] -= matrix[i][j] * matrix[j][k];
                }
            }

            return matrix;
        }

        private static double[] HelperSolve(double[][] lu, double[] b)
        {
            int n = lu.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= lu[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= lu[n - 1][n - 1];

            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= lu[i][j] * x[j];
                x[i] = sum / lu[i][i];
            }

            return x;
        }

        /// <summary>
        /// Inverts a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="parameters">An object that contains all parameters and functions for expressions.</param>
        /// <returns>An inverse matrix.</returns>
        public static Matrix Inverse(this Matrix matrix, ExpressionParameters parameters)
        {
            if (!matrix.IsSquare)
                throw new ArgumentException(Resource.MatrixArgException);

            var size = matrix.ParametersCount;
            var result = Inverse_(matrix.ToCalculatedArray(parameters));
            var m = new Matrix(size, size);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    m[i][j] = new Number(result[i][j]);

            return m;
        }

        private static double[][] Inverse_(double[][] matrix)
        {
            int n = matrix.Length;
            var result = new double[n][];

            for (var i = 0; i < n; i++)
            {
                result[i] = new double[n];
                for (var j = 0; j < n; j++)
                    result[i][j] = matrix[i][j];
            }

            int[] permutation;
            int toggle;
            var lu = LUPDecomposition_(matrix, out permutation, out toggle);

            if (lu == null)
                throw new MatrixIsInvalidException();

            var b = new double[n];
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (i == permutation[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }
                var x = HelperSolve(lu, b);
                for (var j = 0; j < n; j++)
                    result[j][i] = x[j];
            }

            return result;
        }

    }

}

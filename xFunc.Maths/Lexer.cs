﻿// Copyright 2012-2018 Dmitry Kischenko & Ronny Weidemann
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
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using xFunc.Maths.Resources;
using xFunc.Maths.Tokens;

namespace xFunc.Maths
{
    /// <summary>
    /// The lexer for mathematical expressions.
    /// </summary>
    public class Lexer : ILexer
    {
        private Regex regexSymbols;
        private Regex regexOperations;
        private Regex regexFunctions;
        private Regex regexConst;

        private Regex regexNumberHex;
        private Regex regexNumberBin;
        private Regex regexNumberOct;
        private Regex regexNumber;

        private Regex regexComplexNumber;

        private Regex regexSkip;

        private Regex regexAllWhitespaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lexer"/> class.
        /// </summary>
        public Lexer()
        {
            InitRegex();
        }

        private void InitRegex()
        {
			var options = /*RegexOptions.Compiled |*/ RegexOptions.IgnoreCase;

            regexSymbols = new Regex(@"\G(\(|\)|{|}|,)", options);
            regexOperations = new Regex(@"\G([^a-zα-ω0-9(){}µ,°\s]+|nand|nor|and|or|xor|not|eq|impl|mod)", options);
            regexFunctions = new Regex(@"\G([a-zα-ωµ][0-9a-zα-ω]*)(\(|{)?", options);
			regexConst = new Regex (@"\G(true|false)", options);

            regexNumberHex = new Regex(@"\G[+-]?0x[0-9a-f]+", options);
            regexNumberBin = new Regex(@"\G[+-]?0b[01]+", options);
            regexNumberOct = new Regex(@"\G[+-]?0[0-7]+", options);
            regexNumber = new Regex(@"\G[+-]?\d*\.?\d+([e][-+]?\d+)?", options);

            regexComplexNumber = new Regex(@"\G([+-]?\s*\d*\.?\d+)\s*([∠+-]+\s*\s*\d*\.?\d+)°", options);

            regexSkip = new Regex(@"\G\s+", options);
            regexAllWhitespaces = new Regex(@"\s+", options);
        }

        /// <summary>
        /// Creates the symbol token from matched string.
        /// </summary>
        /// <param name="match">The matched string.</param>
        /// <param name="tokens">The list of tokens.</param>
        /// <exception cref="LexerException">
        /// The specified symbol is not supported.
        /// </exception>
        private void CreateSymbol(string match, IList<IToken> tokens)
        {
            if (match == "(")
            {
                tokens.Add(new SymbolToken(Symbols.OpenBracket));
            }
            else if (match == ")")
            {
                var lastToken = tokens.LastOrDefault() as SymbolToken;
                if (lastToken != null && lastToken.Symbol == Symbols.Comma)
                    throw new LexerException(Resource.NotEnoughParams);

                tokens.Add(new SymbolToken(Symbols.CloseBracket));
            }
            else if (match == "{")
            {
                if (!(tokens.LastOrDefault() is FunctionToken))
                    tokens.Add(new FunctionToken(Functions.Vector));

                tokens.Add(new SymbolToken(Symbols.OpenBrace));
            }
            else if (match == "}")
            {
                tokens.Add(new SymbolToken(Symbols.CloseBrace));
            }
            else if (match == ",")
            {
                tokens.Add(new SymbolToken(Symbols.Comma));
            }
            else
            {
                throw new LexerException(string.Format(Resource.NotSupportedSymbol, match));
            }
        }

        /// <summary>
        /// Creates the operation token from matched string.
        /// </summary>
        /// <param name="match">The matched string.</param>
        /// <param name="tokens">The list of tokens.</param>
        /// <exception cref="LexerException">
        /// The specified operation is not supported.
        /// </exception>
        private void CreateOperations(string match, IList<IToken> tokens)
        {
            if (match == "+=")
            {
                tokens.Add(new OperationToken(Operations.AddAssign));
            }
            else if (match == "-=" || match == "−=")
            {
                tokens.Add(new OperationToken(Operations.SubAssign));
            }
            else if (match == "*=" || match == "×=")
            {
                tokens.Add(new OperationToken(Operations.MulAssign));
            }
            else if (match == "*" || match == "×")
            {
                tokens.Add(new OperationToken(Operations.Multiplication));
            }
            else if (match == "/")
            {
                tokens.Add(new OperationToken(Operations.Division));
            }
            else if (match == "^")
            {
                tokens.Add(new OperationToken(Operations.Exponentiation));
            }
            else if (match == "!")
            {
                var lastToken = tokens.LastOrDefault();
                if (lastToken != null)
                {
                    var symbol = lastToken as SymbolToken;
                    if ((symbol != null && symbol.Symbol == Symbols.CloseBracket) || lastToken is NumberToken || lastToken is VariableToken)
                    {
                        tokens.Add(new OperationToken(Operations.Factorial));
                        return;
                    }
                }

                throw new LexerException(string.Format(Resource.NotSupportedSymbol, match));
            }
            else if (match == "%" || match == "mod")
            {
                tokens.Add(new OperationToken(Operations.Modulo));
            }
            else if (match == "&&")
            {
                tokens.Add(new OperationToken(Operations.ConditionalAnd));
            }
            else if (match == "||")
            {
                tokens.Add(new OperationToken(Operations.ConditionalOr));
            }
            else if (match == "==" || match == "≡" /* workarround to prevent a single equal sign */)
            {
                tokens.Add(new OperationToken(Operations.Equal));
            }
            else if (match == "!=" || match == "≠")
            {
                tokens.Add(new OperationToken(Operations.NotEqual));
            }
            else if (match == "<=" || match == "≤")
            {
                tokens.Add(new OperationToken(Operations.LessOrEqual));
            }
            else if (match == "<")
            {
                tokens.Add(new OperationToken(Operations.LessThan));
            }
            else if (match == ">=" || match == "≥")
            {
                tokens.Add(new OperationToken(Operations.GreaterOrEqual));
            }
            else if (match == ">")
            {
                tokens.Add(new OperationToken(Operations.GreaterThan));
            }
            else if (match == "++")
            {
                tokens.Add(new OperationToken(Operations.Increment));
            }
            else if (match == "--" || match == "−−")
            {
                tokens.Add(new OperationToken(Operations.Decrement));
            }
            else if (match == "+")
            {
                var lastToken = tokens.LastOrDefault();
                if (lastToken == null)
                {
                    return;
                }
                else
                {
                    var symbolToken = lastToken as SymbolToken;
                    if (symbolToken != null && (symbolToken.Symbol == Symbols.OpenBracket ||
                                                symbolToken.Symbol == Symbols.OpenBrace))
                        return;
                }

                tokens.Add(new OperationToken(Operations.Addition));
            }
            else if (match == "-" || match == "−")
            {
                var lastToken = tokens.LastOrDefault();
                if (lastToken == null)
                {
                    tokens.Add(new OperationToken(Operations.UnaryMinus));
                }
                else
                {
                    var symbolToken = lastToken as SymbolToken;
                    if (symbolToken != null && (symbolToken.Symbol == Symbols.OpenBracket ||
                                                symbolToken.Symbol == Symbols.OpenBrace ||
                                                symbolToken.Symbol == Symbols.Comma))
                    {
                        tokens.Add(new OperationToken(Operations.UnaryMinus));
                    }
                    else
                    {
                        var operationToken = lastToken as OperationToken;
                        if (operationToken != null && (operationToken.Operation == Operations.Exponentiation ||
                                                       operationToken.Operation == Operations.Multiplication ||
                                                       operationToken.Operation == Operations.Division ||
                                                       operationToken.Operation == Operations.Assign ||
                                                       operationToken.Operation == Operations.AddAssign ||
                                                       operationToken.Operation == Operations.SubAssign ||
                                                       operationToken.Operation == Operations.MulAssign ||
                                                       operationToken.Operation == Operations.DivAssign))
                        {
                            tokens.Add(new OperationToken(Operations.UnaryMinus));
                        }
                        else
                        {
                            tokens.Add(new OperationToken(Operations.Subtraction));
                        }
                    }
                }
            }
            else if (match == "/=")
            {
                tokens.Add(new OperationToken(Operations.DivAssign));
            }
            else if (match == ":=")
            {
                tokens.Add(new OperationToken(Operations.Assign));
            }
            else if (match == "not" || match == "~")
            {
                var lastToken = tokens.LastOrDefault();
                if (lastToken != null)
                {
                    var symbol = lastToken as SymbolToken;
                    if ((symbol != null && symbol.Symbol == Symbols.CloseBracket) || lastToken is NumberToken || lastToken is VariableToken)
                        throw new LexerException(string.Format(Resource.NotSupportedSymbol, match));
                }

                tokens.Add(new OperationToken(Operations.Not));
            }
            else if (match == "and" || match == "&")
            {
                tokens.Add(new OperationToken(Operations.And));
            }
            else if (match == "or" || match == "|")
            {
                tokens.Add(new OperationToken(Operations.Or));
            }
            else if (match == "xor" || match == "⊕" || match == "⊻")
            {
                tokens.Add(new OperationToken(Operations.XOr));
            }
            else if (match == "impl" || match == "->" || match == "−>" || match == "=>")
            {
                tokens.Add(new OperationToken(Operations.Implication));
            }
            else if (match == "eq" || match == "<->" || match == "<−>" || match == "<=>")
            {
                tokens.Add(new OperationToken(Operations.Equality));
            }
            else if (match == "nor" || match == "⊽")
            {
                tokens.Add(new OperationToken(Operations.NOr));
            }
            else if (match == "nand" || match == "⊼")
            {
                tokens.Add(new OperationToken(Operations.NAnd));
            }
            else
            {
                throw new LexerException(string.Format(Resource.NotSupportedSymbol, match));
            }
        }

        /// <summary>
        /// Creates the function token from matched string.
        /// </summary>
        /// <param name="match">The matched string.</param>
        /// <param name="tokens">The list of tokens.</param>
        private void CreateFunction(string match, IList<IToken> tokens)
        {
            var lowerMatch = match.ToLower();
            if (lowerMatch == "add")
            {
                tokens.Add(new FunctionToken(Functions.Add));
            }
            else if (lowerMatch == "sub")
            {
                tokens.Add(new FunctionToken(Functions.Sub));
            }
            else if (lowerMatch == "mul")
            {
                tokens.Add(new FunctionToken(Functions.Mul));
            }
            else if (lowerMatch == "div")
            {
                tokens.Add(new FunctionToken(Functions.Div));
            }
            else if (lowerMatch == "pow")
            {
                tokens.Add(new FunctionToken(Functions.Pow));
            }
            else if (lowerMatch == "abs")
            {
                tokens.Add(new FunctionToken(Functions.Absolute));
            }
            else if (lowerMatch == "sin")
            {
                tokens.Add(new FunctionToken(Functions.Sine));
            }
            else if (lowerMatch == "cos")
            {
                tokens.Add(new FunctionToken(Functions.Cosine));
            }
            else if (lowerMatch == "tg" || lowerMatch == "tan")
            {
                tokens.Add(new FunctionToken(Functions.Tangent));
            }
            else if (lowerMatch == "ctg" || lowerMatch == "cot")
            {
                tokens.Add(new FunctionToken(Functions.Cotangent));
            }
            else if (lowerMatch == "sec")
            {
                tokens.Add(new FunctionToken(Functions.Secant));
            }
            else if (lowerMatch == "csc" || lowerMatch == "cosec")
            {
                tokens.Add(new FunctionToken(Functions.Cosecant));
            }
            else if (lowerMatch == "arcsin")
            {
                tokens.Add(new FunctionToken(Functions.Arcsine));
            }
            else if (lowerMatch == "arccos")
            {
                tokens.Add(new FunctionToken(Functions.Arccosine));
            }
            else if (lowerMatch == "arctg" || lowerMatch == "arctan")
            {
                tokens.Add(new FunctionToken(Functions.Arctangent));
            }
            else if (lowerMatch == "arcctg" || lowerMatch == "arccot")
            {
                tokens.Add(new FunctionToken(Functions.Arccotangent));
            }
            else if (lowerMatch == "arcsec")
            {
                tokens.Add(new FunctionToken(Functions.Arcsecant));
            }
            else if (lowerMatch == "arccsc" || lowerMatch == "arccosec")
            {
                tokens.Add(new FunctionToken(Functions.Arccosecant));
            }
            else if (lowerMatch == "sqrt")
            {
                tokens.Add(new FunctionToken(Functions.Sqrt));
            }
            else if (lowerMatch == "root")
            {
                tokens.Add(new FunctionToken(Functions.Root));
            }
            else if (lowerMatch == "ln")
            {
                tokens.Add(new FunctionToken(Functions.Ln));
            }
            else if (lowerMatch == "lg")
            {
                tokens.Add(new FunctionToken(Functions.Lg));
            }
            else if (lowerMatch == "lb" || lowerMatch == "log2")
            {
                tokens.Add(new FunctionToken(Functions.Lb));
            }
            else if (lowerMatch == "log")
            {
                tokens.Add(new FunctionToken(Functions.Log));
            }
            else if (lowerMatch == "sh" || lowerMatch == "sinh")
            {
                tokens.Add(new FunctionToken(Functions.Sineh));
            }
            else if (lowerMatch == "ch" || lowerMatch == "cosh")
            {
                tokens.Add(new FunctionToken(Functions.Cosineh));
            }
            else if (lowerMatch == "th" || lowerMatch == "tanh")
            {
                tokens.Add(new FunctionToken(Functions.Tangenth));
            }
            else if (lowerMatch == "cth" || lowerMatch == "coth")
            {
                tokens.Add(new FunctionToken(Functions.Cotangenth));
            }
            else if (lowerMatch == "sech")
            {
                tokens.Add(new FunctionToken(Functions.Secanth));
            }
            else if (lowerMatch == "csch")
            {
                tokens.Add(new FunctionToken(Functions.Cosecanth));
            }
            else if (lowerMatch == "arsh" || lowerMatch == "arsinh")
            {
                tokens.Add(new FunctionToken(Functions.Arsineh));
            }
            else if (lowerMatch == "arch" || lowerMatch == "arcosh")
            {
                tokens.Add(new FunctionToken(Functions.Arcosineh));
            }
            else if (lowerMatch == "arth" || lowerMatch == "artanh")
            {
                tokens.Add(new FunctionToken(Functions.Artangenth));
            }
            else if (lowerMatch == "arcth" || lowerMatch == "arcoth")
            {
                tokens.Add(new FunctionToken(Functions.Arcotangenth));
            }
            else if (lowerMatch == "arsch" || lowerMatch == "arsech")
            {
                tokens.Add(new FunctionToken(Functions.Arsecanth));
            }
            else if (lowerMatch == "arcsch")
            {
                tokens.Add(new FunctionToken(Functions.Arcosecanth));
            }
            else if (lowerMatch == "exp")
            {
                tokens.Add(new FunctionToken(Functions.Exp));
            }
            else if (lowerMatch == "gcd" || lowerMatch == "gcf" || lowerMatch == "hcf")
            {
                tokens.Add(new FunctionToken(Functions.GCD));
            }
            else if (lowerMatch == "lcm" || lowerMatch == "scm")
            {
                tokens.Add(new FunctionToken(Functions.LCM));
            }
            else if (lowerMatch == "fact")
            {
                tokens.Add(new FunctionToken(Functions.Factorial));
            }
            else if (lowerMatch == "sum")
            {
                tokens.Add(new FunctionToken(Functions.Sum));
            }
            else if (lowerMatch == "product")
            {
                tokens.Add(new FunctionToken(Functions.Product));
            }
            else if (lowerMatch == "round")
            {
                tokens.Add(new FunctionToken(Functions.Round));
            }
            else if (lowerMatch == "floor")
            {
                tokens.Add(new FunctionToken(Functions.Floor));
            }
            else if (lowerMatch == "ceil")
            {
                tokens.Add(new FunctionToken(Functions.Ceil));
            }
            else if (lowerMatch == "if")
            {
                tokens.Add(new FunctionToken(Functions.If));
            }
            else if (lowerMatch == "for")
            {
                tokens.Add(new FunctionToken(Functions.For));
            }
            else if (lowerMatch == "while")
            {
                tokens.Add(new FunctionToken(Functions.While));
            }
            else if (lowerMatch == "del" || lowerMatch == "nabla")
            {
                tokens.Add(new FunctionToken(Functions.Del));
            }
            else if (lowerMatch == "deriv")
            {
                tokens.Add(new FunctionToken(Functions.Derivative));
            }
            else if (lowerMatch == "simplify")
            {
                tokens.Add(new FunctionToken(Functions.Simplify));
            }
            else if (lowerMatch == "def")
            {
                tokens.Add(new FunctionToken(Functions.Define));
            }
            else if (lowerMatch == "undef")
            {
                tokens.Add(new FunctionToken(Functions.Undefine));
            }
            else if (lowerMatch == "transpose")
            {
                tokens.Add(new FunctionToken(Functions.Transpose));
            }
            else if (lowerMatch == "det" || lowerMatch == "determinant")
            {
                tokens.Add(new FunctionToken(Functions.Determinant));
            }
            else if (lowerMatch == "inverse")
            {
                tokens.Add(new FunctionToken(Functions.Inverse));
            }
            else if (lowerMatch == "vector")
            {
                tokens.Add(new FunctionToken(Functions.Vector));
            }
            else if (lowerMatch == "matrix")
            {
                tokens.Add(new FunctionToken(Functions.Matrix));
            }
            else if (lowerMatch == "re" || lowerMatch == "real")
            {
                tokens.Add(new FunctionToken(Functions.Re));
            }
            else if (lowerMatch == "im" || lowerMatch == "imaginary")
            {
                tokens.Add(new FunctionToken(Functions.Im));
            }
            else if (lowerMatch == "phase")
            {
                tokens.Add(new FunctionToken(Functions.Phase));
            }
            else if (lowerMatch == "conjugate")
            {
                tokens.Add(new FunctionToken(Functions.Conjugate));
            }
            else if (lowerMatch == "reciprocal")
            {
                tokens.Add(new FunctionToken(Functions.Reciprocal));
            }
            else if (lowerMatch == "min")
            {
                tokens.Add(new FunctionToken(Functions.Min));
            }
            else if (lowerMatch == "max")
            {
                tokens.Add(new FunctionToken(Functions.Max));
            }
            else if (lowerMatch == "avg")
            {
                tokens.Add(new FunctionToken(Functions.Avg));
            }
            else if (lowerMatch == "count")
            {
                tokens.Add(new FunctionToken(Functions.Count));
            }
            else if (lowerMatch == "var")
            {
                tokens.Add(new FunctionToken(Functions.Var));
            }
            else if (lowerMatch == "varp")
            {
                tokens.Add(new FunctionToken(Functions.Varp));
            }
            else if (lowerMatch == "stdev")
            {
                tokens.Add(new FunctionToken(Functions.Stdev));
            }
            else if (lowerMatch == "stdevp") 
                {
                tokens.Add(new FunctionToken(Functions.Stdevp));
            } 
            else if (match == "integral") 
            {
				tokens.Add (new FunctionToken (Functions.DefiniteIntegral));
			} 
            else if (match == "roundunary") 
            {
				tokens.Add (new FunctionToken (Functions.RoundUnary));
			} else if (match == "nderivative") 
            {
				tokens.Add (new FunctionToken (Functions.NDerivative));
			} else if (match == "fract") 
            {
				tokens.Add (new FunctionToken (Functions.Fract));
            } 
            else if (match == "condition") 
            {
                tokens.Add(new FunctionToken(Functions.Condition));
            } 
            else if (match == "multicondition") 
            {
                tokens.Add(new FunctionToken(Functions.MultiCondition));
            } 
            else if (match == "solve") {
                tokens.Add(new FunctionToken(Functions.Solve));
            } else if (match == "sign") {
                tokens.Add(new FunctionToken(Functions.Sign));

            } else if (match == "rand") {
                tokens.Add (new FunctionToken (Functions.Rand));
            } else if (match == "nPr") {
                tokens.Add (new FunctionToken (Functions.NPr));
            } else if (match == "nCr") {
                tokens.Add (new FunctionToken (Functions.NCr));
            } else {
                tokens.Add(new UserFunctionToken(match));
            }
        }

        /// <summary>
        /// Creates the constant token from matched string.
        /// </summary>
        /// <param name="match">The matched string.</param>
        /// <param name="tokens">The list of tokens.</param>
        /// <exception cref="LexerException">
        /// The specified constant is not supported.
        /// </exception>
        private void CreateConst(string match, IList<IToken> tokens)
        {
            if (match == "true")
            {
                tokens.Add(new BooleanToken(true));
            }
            else if (match == "false")
            {
                tokens.Add(new BooleanToken(false));
            }
            else
            {
                throw new LexerException(string.Format(Resource.NotSupportedSymbol, match));
            }
        }

        /// <summary>
        /// Determines whether brackets in the specified string is balanced.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is balanced; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBalanced(string str)
        {
            int brackets = 0;
            int braces = 0;

            foreach (var item in str)
            {
                if (item == '(') brackets++;
                else if (item == ')') brackets--;
                else if (item == '{') braces++;
                else if (item == '}') braces--;

                if (brackets < 0 || braces < 0)
                    return false;
            }

            return brackets == 0 && braces == 0;
        }

        /// <summary>
        /// Converts the string into a sequence of tokens.
        /// </summary>
        /// <param name="function">The string that contains the functions and operators.</param>
        /// <returns>The sequence of tokens.</returns>
        /// <seealso cref="IToken"/>
        /// <exception cref="ArgumentNullException">Throws when the <paramref name="function"/> parameter is null or empty.</exception>
        /// <exception cref="LexerException">Throws when <paramref name="function"/> has the not supported symbol.</exception>
        public IEnumerable<IToken> Tokenize(string function)
        {
            if (string.IsNullOrWhiteSpace(function))
                throw new ArgumentNullException(nameof(function), Resource.NotSpecifiedFunction);
            if (!IsBalanced(function))
                throw new LexerException(Resource.NotBalanced);

            function = function.Replace("\n", "").Replace("\r", "");
            var tokens = new List<IToken>();
            for (var i = 0; i < function.Length;) {
                // whitespaces
                var match = regexSkip.Match(function, i);
                if (match.Success)
                {
                    i += match.Length;
                    continue;
                }

                // symbols
                match = regexSymbols.Match(function, i);
                if (match.Success)
                {
                    CreateSymbol(match.Value, tokens);

                    i += match.Length;
                    continue;
                }

                // complex numbers
                match = regexComplexNumber.Match(function, i);
                if (match.Success)
                {
                    double magnitude;
                    double phase;

                    if (!double.TryParse(regexAllWhitespaces.Replace(match.Groups[1].Value, string.Empty), NumberStyles.Number, CultureInfo.InvariantCulture, out magnitude))
                        magnitude = 0.0;
                    if (!double.TryParse(regexAllWhitespaces.Replace(match.Groups[2].Value, string.Empty).Replace("∠", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out phase))
                        phase = 1.0;

                    tokens.Add(new ComplexNumberToken(Complex.FromPolarCoordinates(magnitude, phase * Math.PI / 180)));

                    i += match.Length;
                    continue;
                }

                // operations
                match = regexOperations.Match(function, i);
                if (match.Success)
                {
                    CreateOperations(match.Value, tokens);

                    i += match.Length;
                    continue;
                }

                // hex numbers
                match = regexNumberHex.Match(function, i);
                if (match.Success)
                {
                    tokens.Add(new NumberToken(Convert.ToInt64(match.Value, 16)));

                    i += match.Length;
                    continue;
                }

                // bin numbers
                match = regexNumberBin.Match(function, i);
                if (match.Success)
                {
                    tokens.Add(new NumberToken(Convert.ToInt64(match.Value.Replace("0b", ""), 2)));

                    i += match.Length;
                    continue;
                }

                // oct numbers
                match = regexNumberOct.Match(function, i);
                if (match.Success)
                {
                    tokens.Add(new NumberToken(Convert.ToInt64(match.Value, 8)));

                    i += match.Length;
                    continue;
                }

                // numbers
                match = regexNumber.Match(function, i);
                if (match.Success)
                {
                    tokens.Add(new NumberToken(double.Parse(match.Value, CultureInfo.InvariantCulture)));

                    i += match.Length;
                    continue;
                }

                // consts
                match = regexConst.Match(function, i);
                if (match.Success)
                {
                    CreateConst(match.Value, tokens);

                    i += match.Length;
                    continue;
                }

                // functions
                match = regexFunctions.Match(function, i);
                if (match.Success)
                {
                    if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                    {
                        var funcName = match.Groups[1].Value;
                        CreateFunction(funcName, tokens);

                        i += funcName.Length;
                    }
                    else
                    {
                        var lastToken = tokens.LastOrDefault() as NumberToken;
                        if (lastToken != null)
                            tokens.Add(new OperationToken(Operations.Multiplication));

                        if (match.Value == "i")
                            tokens.Add(new ComplexNumberToken(Complex.ImaginaryOne));
                        else if (match.Value == "pi")
                            tokens.Add(new VariableToken("π"));
                        else
                            tokens.Add(new VariableToken(match.Value));

                        i += match.Length;
                    }

                    continue;
                }

                throw new LexerException(string.Format(Resource.NotSupportedSymbol, function));
            }

            return CountParams(tokens);
        }

        private int _CountParams(List<IToken> tokens, int index)
        {
            var func = (FunctionToken)tokens[index];

            int countOfParams = 0;
            int brackets = 1;
            bool hasBraces = false;
            bool oneParam = true;
            int i = index + 2;
            for (; i < tokens.Count;)
            {
                var token = tokens[i];
                if (token is SymbolToken)
                {
                    var symbol = token as SymbolToken;
                    if (symbol.Symbol == Symbols.OpenBrace)
                        hasBraces = true;

                    if (symbol.Symbol == Symbols.CloseBracket || symbol.Symbol == Symbols.CloseBrace)
                    {
                        brackets--;

                        if (brackets == 0)
                            break;
                    }
                    else if (symbol.Symbol == Symbols.OpenBracket || symbol.Symbol == Symbols.OpenBrace)
                    {
                        brackets++;

                        if (oneParam)
                        {
                            countOfParams++;
                            oneParam = false;
                        }
                    }
                    else if (symbol.Symbol == Symbols.Comma)
                    {
                        oneParam = true;
                    }

                    i++;
                }
                else if (token is FunctionToken)
                {
                    if (oneParam)
                    {
                        countOfParams++;
                        oneParam = false;
                    }

                    var f = (FunctionToken)token;
                    if (f.Function == Functions.Matrix || f.Function == Functions.Vector)
                        hasBraces = true;

                    i = _CountParams(tokens, i) + 1;
                }
                else
                {
                    if (oneParam)
                    {
                        countOfParams++;
                        oneParam = false;
                    }

                    i++;
                }
            }

            if (func.Function == Functions.Vector && hasBraces)
                tokens[index] = new FunctionToken(Functions.Matrix, countOfParams);
            else
                func.CountOfParams = countOfParams;

            return i;
        }

        /// <summary>
        /// Calculates the number of parametes of functions.
        /// </summary>
        /// <param name="tokens">The list of tokens.</param>
        /// <returns>The list of tokens.</returns>
        private IEnumerable<IToken> CountParams(List<IToken> tokens)
        {
            for (int i = 0; i < tokens.Count;)
            {
                if (tokens[i] is FunctionToken)
                    i = _CountParams(tokens, i) + 1;
                else
                    i++;
            }

            return tokens;
        }

    }

}

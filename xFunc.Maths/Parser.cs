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
using System.Collections.Generic;
using xFunc.Maths.Expressions;
using xFunc.Maths.Resources;
using xFunc.Maths.Tokens;
using xFunc.Maths.Expressions.LogicalAndBitwise;
using System.Linq;
using xFunc.Maths.Expressions.ComplexNumbers;

namespace xFunc.Maths
{

    /// <summary>
    /// The parser for mathematical expressions.
    /// </summary>
    public class Parser : IParser
    {
        
        private IExpressionFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class with default implementations of <see cref="IExpressionFactory"/>.
        /// </summary>
        public Parser()
            : this(new ExpressionFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public Parser(IExpressionFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Checks the <paramref name="expression"/> parameter has <paramref name="arg"/>.
        /// </summary>
        /// <param name="expression">A expression that is checked.</param>
        /// <param name="arg">A variable that can be contained in the expression.</param>
		/// <remarks>
		/// backward-compatibility
		/// </remarks>
        /// <returns>true if <paramref name="expression"/> has <paramref name="arg"/>; otherwise, false.</returns>
        public static bool HasVar(IExpression expression, Variable arg) {
            if (expression is BinaryExpression) {
                var bin = expression as BinaryExpression;
                if (HasVar(bin.Left, arg))
                    return true;

                return HasVar(bin.Right, arg);
            }
            if (expression is UnaryExpression) {
                var un = expression as UnaryExpression;
                return HasVar(un.Argument, arg);
            }
            if (expression is DifferentParametersExpression) {
                var paramExp = expression as DifferentParametersExpression;
                return paramExp.Arguments.Any(e => HasVar(e, arg));
            }
            return expression is Variable && expression.Equals(arg);
        }

		/// <summary>
		/// Parse the specified string-expression.
		/// </summary>
		/// <remarks>
		/// backward-compatibility
		/// </remarks>
		/// <param name="expression">Expression.</param>
		public IExpression Parse (string expression) {
			return this.Parse (new Lexer ().Tokenize (expression));
		}

        /// <summary>
        /// Parses the specified function.
        /// </summary>
        /// <param name="tokens">The list of tokens.</param>
        /// <returns>The parsed expression.</returns>
        public IExpression Parse(IEnumerable<IToken> tokens){
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            var tokenList = tokens.ToList();
            if (!tokenList.Any())
                throw new ArgumentException(nameof(tokens));

            var rpn = ConvertToReversePolishNotation(tokenList);
            var expressions = ConvertTokensToExpressions(rpn);

            var stack = new Stack<IExpression>();
            foreach (var expression in expressions)
            {
                if (expression is Number || expression is ComplexNumber || expression is Bool || expression is Variable)
                {
                    stack.Push(expression);
                }
                else if (expression is BinaryExpression)
                {
                    var binExp = expression as BinaryExpression;
                    binExp.Right = stack.Pop();
                    binExp.Left = stack.Pop();

                    stack.Push(binExp);
                }
                else if (expression is UnaryExpression)
                {
                    var unaryMathExp = expression as UnaryExpression;
                    unaryMathExp.Argument = stack.Pop();

                    stack.Push(unaryMathExp);
                }
                else if (expression is DifferentParametersExpression)
                {
                    var func = expression as DifferentParametersExpression;

                    var arg = new IExpression[func.ParametersCount];
                    for (var i = func.ParametersCount - 1; i >= 0; i--)
                        arg[i] = stack.Pop();

                    func.Arguments = arg;

                    stack.Push(func);
                }
                else if (expression is Define)
                {
                    if (stack.Count < 2)
                        throw new ParserException(Resource.InvalidNumberOfVariables);

                    var assign = expression as Define;
                    assign.Value = stack.Pop();
                    assign.Key = stack.Pop();

                    stack.Push(assign);
                }
                else if (expression is Undefine)
                {
                    var undef = expression as Undefine;
                    undef.Key = stack.Pop();

                    stack.Push(undef);
                }
                else
                {
                    throw new ParserException(Resource.UnexpectedError);
                }
            }

            if (stack.Count > 1)
                throw new ParserException(Resource.ErrorWhileParsingTree);

            return stack.Pop();
        }

        private IEnumerable<IExpression> ConvertTokensToExpressions(IEnumerable<IToken> tokens)
        {
            var preOutput = new List<IExpression>();

            foreach (var token in tokens)
            {
                var exp = factory.Create(token);
                if (exp == null)
                    throw new ParserException(Resource.ErrorWhileParsingTree);

                if (token is FunctionToken)
                {
                    var t = token as FunctionToken;

                    if (t.CountOfParams < exp.MinParameters)
                        throw new ParserException(Resource.LessParams);
                    if (exp.MaxParameters != -1 && t.CountOfParams > exp.MaxParameters)
                        throw new ParserException(Resource.MoreParams);
                }

                preOutput.Add(exp);
            }

            return preOutput;
        }

        private IEnumerable<IToken> ConvertToReversePolishNotation(IEnumerable<IToken> tokens)
        {
            var output = new List<IToken>();
            var stack = new Stack<IToken>();

            var openBracketToken = new SymbolToken(Symbols.OpenBracket);
            var openBraceToken = new SymbolToken(Symbols.OpenBrace);
            foreach (var token in tokens)
            {
                IToken stackToken;
                if (token is SymbolToken)
                {
                    var t = token as SymbolToken;
                    switch (t.Symbol)
                    {
                        case Symbols.OpenBracket:
                        case Symbols.OpenBrace:
                            stack.Push(token);
                            break;
                        case Symbols.CloseBracket:
                        case Symbols.CloseBrace:
                            stackToken = stack.Pop();
                            while (!stackToken.Equals(openBracketToken) && !stackToken.Equals(openBraceToken))
                            {
                                output.Add(stackToken);
                                stackToken = stack.Pop();
                            }
                            break;
                        case Symbols.Comma:
                            stackToken = stack.Pop();

                            while (!stackToken.Equals(openBracketToken) && !stackToken.Equals(openBraceToken))
                            {
                                output.Add(stackToken);
                                stackToken = stack.Pop();
                            }

                            stack.Push(stackToken);
                            break;
                    }
                }
                else if (token is NumberToken || token is VariableToken)
                {
                    output.Add(token);
                }
                else
                {
                    while (stack.Count != 0 && (stackToken = stack.Peek()).Priority >= token.Priority)
                    {
                        if (stackToken.Equals(openBracketToken) || stackToken.Equals(openBraceToken))
                            break;
                        output.Add(stack.Pop());
                    }

                    stack.Push(token);
                }
            }

            if (stack.Count != 0)
                output.AddRange(stack);

            return output;
        }
        
        /// <summary>
        /// Gets or sets the expression factory.
        /// </summary>
        /// <value>
        /// The expression factory.
        /// </value>
        public IExpressionFactory ExpressionFactory
        {
            get
            {
                return factory;
            }
            set
            {
                factory = value;
            }
        }
        
    }

}

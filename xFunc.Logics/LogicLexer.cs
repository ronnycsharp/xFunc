﻿// Copyright 2012-2014 Dmitry Kischenko
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
#if NET35_OR_GREATER || PORTABLE
using System.Linq;
#endif
using xFunc.Logics.Resources;

namespace xFunc.Logics
{

    /// <summary>
    /// The lexer for logical expressions.
    /// </summary>
    public class LogicLexer : ILexer
    {

#if NET35_OR_GREATER || PORTABLE
        private readonly HashSet<string> supportedOp;
#elif NET20_OR_GREATER
        private readonly List<string> supportedOp;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicLexer"/> class.
        /// </summary>
        public LogicLexer()
        {
#if NET35_OR_GREATER || PORTABLE
            supportedOp = new HashSet<string>
            {
                "not", "and", "or", "xor", "nand", "nor", "impl", "eq"
            };
#elif NET20_OR_GREATER
            supportedOp = new List<string>
            {
                "not", "and", "or", "xor", "nand", "nor", "impl", "eq"
            };
#endif
        }

        private bool IsBalanced(string str)
        {
            int brackets = 0;

            foreach (var item in str)
            {
                if (item == '(') brackets++;
                else if (item == ')') brackets--;

                if (brackets < 0)
                    return false;
            }

            return brackets == 0;
        }

        /// <summary>
        /// Converts the string into a sequence of tokens.
        /// </summary>
        /// <param name="function">The string that contains the functions and operators.</param>
        /// <returns>The sequence of tokens.</returns>
        public IEnumerable<LogicToken> Tokenize(string function)
        {
#if NET40_OR_GREATER || PORTABLE
            if (string.IsNullOrWhiteSpace(function))
#elif NET20_OR_GREATER
            if (StringExtention.IsNullOrWhiteSpace(function))
#endif
                throw new ArgumentNullException("function", Resource.NotSpecifiedFunction);

            function = function.ToLower().Replace(" ", "");
            if (!IsBalanced(function))
                throw new LogicLexerException(Resource.NotBalanced);
            var tokens = new List<LogicToken>();

            for (int i = 0; i < function.Length; )
            {
                char letter = function[i];
                var token = new LogicToken();
                if (letter == '(')
                {
                    token.Type = LogicTokenType.OpenBracket;
                }
                else if (letter == ')')
                {
                    token.Type = LogicTokenType.CloseBracket;
                }
                else if (letter == '!')
                {
                    token.Type = LogicTokenType.Not;
                }
                else if (letter == '&')
                {
                    token.Type = LogicTokenType.And;
                }
                else if (letter == '|')
                {
                    token.Type = LogicTokenType.Or;
                }
                else if ((letter == '-' || letter == '=') && (i + 1 < function.Length && function[i + 1] == '>'))
                {
                    token.Type = LogicTokenType.Implication;
                    tokens.Add(token);
                    i += 2;

                    continue;
                }
                else if (letter == '<' && (i + 1 <= function.Length && (function[i + 1] == '=' || function[i + 1] == '-')) && (i + 2 <= function.Length && function[i + 2] == '>'))
                {
                    token.Type = LogicTokenType.Equality;
                    tokens.Add(token);
                    i += 3;

                    continue;
                }
                else if (letter == '^')
                {
                    token.Type = LogicTokenType.XOr;
                }
                else if (letter == ':' && i + 1 < function.Length && function[i + 1] == '=')
                {
                    token.Type = LogicTokenType.Assign;
                    tokens.Add(token);
                    i += 2;

                    continue;
                }
                else if (char.IsLetter(letter))
                {
                    var sub = function.Substring(i);
                    if (sub.StartsWith("true"))
                    {
                        token.Type = LogicTokenType.True;
                        tokens.Add(token);
                        i += 4;

                        continue;
                    }
                    if (sub.StartsWith("false"))
                    {
                        token.Type = LogicTokenType.False;
                        tokens.Add(token);
                        i += 5;

                        continue;
                    }
                    if (sub.StartsWith("not("))
                    {
                        token.Type = LogicTokenType.Not;
                        tokens.Add(token);
                        i += 3;

                        continue;
                    }
                    if (sub.StartsWith("or"))
                    {
                        token.Type = LogicTokenType.Or;
                        tokens.Add(token);
                        i += 2;

                        continue;
                    }
                    if (sub.StartsWith("and"))
                    {
                        token.Type = LogicTokenType.And;
                        tokens.Add(token);
                        i += 3;

                        continue;
                    }
                    if (sub.StartsWith("impl"))
                    {
                        token.Type = LogicTokenType.Implication;
                        tokens.Add(token);
                        i += 4;

                        continue;
                    }
                    if (sub.StartsWith("eq"))
                    {
                        token.Type = LogicTokenType.Equality;
                        tokens.Add(token);
                        i += 2;

                        continue;
                    }
                    if (sub.StartsWith("nor"))
                    {
                        token.Type = LogicTokenType.NOr;
                        tokens.Add(token);
                        i += 3;

                        continue;
                    }
                    if (sub.StartsWith("nand"))
                    {
                        token.Type = LogicTokenType.NAnd;
                        tokens.Add(token);
                        i += 4;

                        continue;
                    }
                    if (sub.StartsWith("xor"))
                    {
                        token.Type = LogicTokenType.XOr;
                        tokens.Add(token);
                        i += 3;

                        continue;
                    }
                    if (sub.StartsWith("table("))
                    {
                        token.Type = LogicTokenType.TruthTable;
                        tokens.Add(token);
                        i += 5;

                        continue;
                    }
                    if (sub.StartsWith("undef("))
                    {
                        token.Type = LogicTokenType.Undefine;
                        tokens.Add(token);
                        i += 5;

                        continue;
                    }

                    if (letter == 't')
                    {
                        token.Type = LogicTokenType.True;
                        tokens.Add(token);
                        i++;

                        continue;
                    }
                    if (letter == 'f')
                    {
                        token.Type = LogicTokenType.False;
                        tokens.Add(token);
                        i++;

                        continue;
                    }

                    token.Type = LogicTokenType.Variable;
                    int length = 1;

                    for (int j = i + 1; j < function.Length && char.IsLetter(function[j]) && !supportedOp.Any(s => function.Substring(j).StartsWith(s)); j++, length++) ;
                    token.Variable = function.Substring(i, length);
                    tokens.Add(token);

                    i += length;

                    continue;
                }
                else
                {
                    throw new LogicLexerException(string.Format(Resource.NotSupportedSymbol, letter));
                }

                tokens.Add(token);
                i++;
            }

            return tokens;
        }

    }

}

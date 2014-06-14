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

namespace xFunc.Maths.Expressions.Programming
{

    /// <summary>
    /// A static class for bool extentions.
    /// </summary>
    public static class Bool
    {

        /// <summary>
        /// The true constant.
        /// </summary>
        public const int True = 1;
        /// <summary>
        /// The false constant.
        /// </summary>
        public const int False = 0;

        /// <summary>
        /// Convert an object (a number) to bool.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns a bool.</returns>
        public static bool AsBool(this object value)
        {
            if (value is double)
            {
                var num = (double)value;

                return num != False;
            }
            if (value is int)
            {
                var num = (int)value;

                return num != False;
            }

            return false;
        }

        /// <summary>
        /// Convert a bool to number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns a number.</returns>
        public static int AsNumber(this bool value)
        {
            return value ? 1 : 0;
        }

    }

}

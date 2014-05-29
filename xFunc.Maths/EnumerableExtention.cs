﻿#if NET20 || NET30

using System;
using System.Collections.Generic;
using xFunc.Maths.Resources;

namespace System.Runtime.CompilerServices
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class ExtensionAttribute : Attribute { }

}

namespace xFunc.Maths
{

    internal static class EnumerableExtention
    {

        public static TSource Aggregate<TSource>(this IEnumerable<TSource> value, Func<TSource, TSource, TSource> func)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (func == null)
                throw new ArgumentNullException("func");

            using (var enumerator = value.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("No elements in source list");

                TSource folded = enumerator.Current;
                while (enumerator.MoveNext())
                    folded = func(folded, enumerator.Current);
                return folded;
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> value, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (func == null)
                throw new ArgumentNullException("func");

            TAccumulate folded = seed;

            foreach (var item in value)
                folded = func(folded, item);

            return folded;
        }

        public static bool Any<T>(this IEnumerable<T> value, Func<T, bool> predicate)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (T item in value)
                if (predicate(item))
                    return true;

            return false;
        }

        public static int Count<T>(this IEnumerable<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var collection = value as ICollection<T>;
            if (collection != null)
                return collection.Count;

            int count = 0;
            foreach (var item in value)
                count++;

            return count;
        }

        public static T First<T>(this IEnumerable<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var list = value as IList<T>;
            if (list != null)
            {
                if (list.Count > 0)
                    return list[0];

                throw new InvalidOperationException(Resource.InvalidInFirst);
            }

            using (var e = value.GetEnumerator())
                if (e.MoveNext())
                    return e.Current;

            throw new InvalidOperationException(Resource.InvalidInFirst);
        }

        public static T First<T>(this IEnumerable<T> value, Func<T, bool> predicate)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (T item in value)
                if (predicate(item))
                    return item;

            throw new InvalidOperationException(Resource.InvalidInFirst);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> value) where T : class
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var list = value as IList<T>;
            if (list != null)
            {
                if (list.Count > 0)
                    return list[0];

                return null;
            }

            using (var e = value.GetEnumerator())
                if (e.MoveNext())
                    return e.Current;

            return null;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> value, Func<T, bool> predicate) where T : class
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (T item in value)
                if (predicate(item))
                    return item;

            return null;
        }

        public static T LastOrDefault<T>(this IEnumerable<T> value) where T : class
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var list = value as IList<T>;
            if (list != null)
                return list.Count > 0 ? list[list.Count - 1] : null;

            using (var e = value.GetEnumerator())
            {
                if (!e.MoveNext())
                    return null;

                var last = e.Current;
                while (e.MoveNext())
                    last = e.Current;

                return last;
            }
        }

        public static IEnumerable<V> Select<T, V>(this IEnumerable<T> value, Func<T, V> func)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (func == null)
                throw new ArgumentNullException("func");

            foreach (var item in value)
                yield return func(item);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> value, IEnumerable<TSource> obj)
        {
            if (value == null) 
                throw new ArgumentNullException("value");
            if (obj == null) 
                throw new ArgumentNullException("obj");

            using (IEnumerator<TSource> first = value.GetEnumerator(), second = obj.GetEnumerator())
            {
                do
                {
                    if (!first.MoveNext())
                        return !second.MoveNext();

                    if (!second.MoveNext())
                        return false;
                } 
                while (first.Current.Equals(second.Current));
            }

            return false;
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> value, Func<T, bool> predicate)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (var item in value)
                if (predicate(item))
                    yield return item;
        }

        public static T[] ToArray<T>(this IEnumerable<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new List<T>(value).ToArray();
        }

    }

}

#endif
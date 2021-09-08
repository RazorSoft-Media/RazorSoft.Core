// Copyright © 2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace RazorSoft.Core.Linq {

    /// <summary>
    /// 
    /// </summary>
    public static class LinqExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="toAppend"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> toAppend) {
            var s = source.ToList();
            s.AddRange(toAppend);

            return s;
        }
        /// <summary>
        /// Executes a standard action on each item within the collection.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="source">source entity collection</param>
        /// <param name="action">action</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (var e in source) {
                action(e);
            }
        }
        /// <summary>
        /// ???
        /// NOTE: how is this different from System.Linq ???
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static IEnumerable<T> Take<T>(this IEnumerator<T> enumerator, int take) {
            var count = 0;
            var loop = true;

            while (loop && count < take) {
                loop = enumerator.MoveNext();

                yield return enumerator.Current;

                ++count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TOut> ToList<TOut>(this IEnumerable source) {
            return source
                .OfType<TOut>()
                .ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TOut[] ToArray<TOut>(this IEnumerable source) {
            return source
                .OfType<TOut>()
                .ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static List<TOut> ToList<TOut>(this IEnumerable source, Func<object, TOut> select) {
            return source
                .Select(select)
                .ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TOut SingleOrDefault<TOut>(this IEnumerable source) {
            var array = source?.ToArray<TOut>();

            if(array == null) {
                throw new ArgumentNullException("source cannot be null");
            }

            if(array.Length == 0) {
                return default;
            }

            if(array.Length > 1) {
                throw new InvalidOperationException($"source has too many elements; expects exactly 1 [{array?.Length}]");
            }

            return array[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="items"></param>
        public static void AddRange(this IList source, IEnumerable items) {
            foreach (var i in items) {
                source.Add(i);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> items) {
            items.ForEach((i) => source.Add(i));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> Select<TIn, TOut>(this IEnumerable source, Func<TIn, TOut> select) {
            IEnumerator iterator = source.GetEnumerator();

            while (iterator.MoveNext()) {
                if (iterator.Current is TIn item) {
                    yield return select(item);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> Select<TOut>(this IEnumerable source, Func<object, TOut> select) {
            IEnumerator iterator = source.GetEnumerator();

            while (iterator.MoveNext()) {
                yield return select(iterator.Current);
            }
        }
    }
}

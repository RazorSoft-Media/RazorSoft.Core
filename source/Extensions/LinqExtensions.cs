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
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="source">source entity collection</param>
        /// <param name="action">action</param>
        public static void ForEach<TEntity>(this IEnumerable<TEntity> source, Action<TEntity> action) {
            foreach(var e in source) {
                action(e);
            }
        }
        /// <summary>
        /// ???
        /// NOTE: how is this different from System.Linq ???
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="enumerator"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> Take<TValue>(this IEnumerator<TValue> enumerator, int take) {
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
        /// <typeparam name="TItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        public static void AddRange(this IList source, IEnumerable items) {
            foreach(var i in items) {
                source.Add(i);
            }
        }
    }
}

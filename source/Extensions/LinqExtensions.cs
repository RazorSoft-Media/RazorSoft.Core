//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
using System.Collections.Generic;


namespace RazorSoft.Core.Extensions {

    public static class LinqExtensions {
		
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
    }
}

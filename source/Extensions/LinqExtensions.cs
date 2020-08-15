//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.Collections.Generic;


namespace RazorSoft.Core.Extensions {

    public static class LinqExtensions {
		
        public static void ForEach<TEntity>(this IEnumerable<TEntity> source, Action<TEntity> action) {
            foreach(var e in source) {
                action(e);
            }
        }
    }
}

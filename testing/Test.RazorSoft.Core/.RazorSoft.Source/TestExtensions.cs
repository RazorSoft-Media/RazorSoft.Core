// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq;
using System.Collections.Generic;
//
using RazorSoft.Core.Linq;


namespace UnitTest.RazorSoft.Core.Collections {

    /// <summary>
    /// 
    /// </summary>
    public static class TestExtensions {

        public static TValue[][] ToRows<TValue>(this IEnumerable<TValue> source, int width) {
            var rowCount = source.Count() / width;
            var rows = new List<TValue[]>();
            var iterator = source.GetEnumerator();
            var count = 0;

            while (count < rowCount) {
                rows.Add(iterator.Take(width).ToArray());

                ++count;
            }

            return rows.ToArray();
        }

        public static TValue[][] ToColumns<TValue>(this IEnumerable<TValue> source, int height) {
            //  configure
            var array = source.ToArray();
            var colCount = array.Length / height;
            //  counters
            var arrIndex = 0;
            var colIndex = 0;
            int rowIndex = 0;
            //  control
            var loop = ((arrIndex + (rowIndex * colCount)) < array.Length);
            //  storage
            var columns = new List<TValue[]>();
            var colArray = default(TValue[]);

            while (loop) {
                //  clear column array
                colArray = Enumerable.Repeat(default(TValue), height).ToArray();

                while (rowIndex < height) {
                    colArray[rowIndex] = array[arrIndex + (rowIndex * colCount)];

                    ++rowIndex;
                }

                columns.Add(colArray);

                rowIndex = 0;

                if (++colIndex < colCount) {
                    ++arrIndex;
                }
                else {
                    loop = false;
                }
            }

            return columns.ToArray();
        }
    }
}

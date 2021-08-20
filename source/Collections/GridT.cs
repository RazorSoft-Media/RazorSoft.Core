// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//
using RazorSoft.Core.Linq;


namespace RazorSoft.Core.Collections {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool HasNext<TValue>(out TValue value);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCell"></typeparam>
    public abstract class Grid<TCell> {
        #region		fields
        private readonly Memory<TCell> memory;
        #endregion	fields


        #region		properties

        /// <summary>
        /// Get grid's 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public TCell this[(int Y, int X) coordinate] => memory.Span[Id(coordinate)];
        /// <summary>
        /// Column count
        /// </summary>
        public int ColCount { get; }
        /// <summary>
        /// Row count
        /// </summary>
        public int RowCount { get; }
        /// <summary>
        /// Grid capacity
        /// </summary>
        public int Capacity => ColCount * RowCount;
        /// <summary>
        /// Grid length - number of elements written to the grid collection
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// Gets the grid's column enumerable
        /// </summary>
        public IEnumerable<TCell[]> Columns => ColumnEnumerable();
        /// <summary>
        /// Gets the grid's row enumerable
        /// </summary>
        public IEnumerable<TCell[]> Rows => RowEnumerable();
        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// Private ctor to initialize memory
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        private Grid(int columns, int rows) {
            memory = (new TCell[(ColCount = columns) * (RowCount = rows)])
                .AsMemory();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="range"></param>
        protected Grid(int columns, int rows, IEnumerable<TCell> range) : this(columns, rows) {
            var array = range.ToArray();

            if (array.Length != Capacity) {
                throw new InvalidOperationException($"range length must be equal to grid capacity");
            }

            var idx = 0;

            while (idx < Capacity) {
                memory.Span[idx] = array[idx];

                if (++idx <= Capacity) {
                    ++Length;
                }
            }

            OnGridGenerated();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="hasNext"></param>
        protected Grid(int columns, int rows, HasNext<TCell> hasNext) : this(columns, rows) {
            var idx = 0;

            while (idx < Capacity) {
                if (hasNext(out TCell value)) {
                    memory.Span[idx] = value;
                    ++Length;
                }

                ++idx;
            }

            OnGridGenerated();
        }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public int Id((int Y, int X) coordinate) {
            if (coordinate.Y < RowCount && coordinate.X < ColCount) {
                var idx = coordinate.Y + (coordinate.Y * (ColCount - 1)) + coordinate.X;

                return idx;
            }

            throw new InvalidOperationException($"coordinate [{coordinate}] outside grid bounds");
        }
        /// <summary>
        /// Sorted cell index
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns>(int Index, TCell Value) array</returns>
        public (int Index, TCell Value)[] Index<TReturn>(Func<TCell, TReturn> orderBy) {
            var array = new GridIterator(memory);

            IEnumerable<(int Index, TCell Value)> enumerate() {
                var idx = 0;
                while (array.MoveNext()) {
                    yield return (Index: idx, Value: array.Current);
                    ++idx;
                }
            }

            //var result = enumerate()
            //    .OrderBy(idx => sortBy(idx.Value))
            //    .ToArray();

            return enumerate()
                .OrderBy(idx => orderBy(idx.Value))
                .ToArray();
        }
        /// <summary>
        /// Unsorted cell index
        /// </summary>
        /// <returns>(int Index, TCell Value) array</returns>
        public (int Index, TCell Value)[] Index() {
            var array = new GridIterator(memory);

            IEnumerable<(int Index, TCell Value)> enumerate() {
                var idx = 0;
                while (array.MoveNext()) {
                    yield return (Index: idx, Value: array.Current);
                    ++idx;
                }
            }

            //var result = enumerate()
            //    .ToArray();

            return enumerate()
                .ToArray();
        }
        #endregion	public methods & functions


        #region		non-public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TCell[]> RowEnumerable() {
            //  source
            var iterator = new GridIterator(memory);
            //  counter
            int count = 0;
            //  storage
            var rowArray = default(TCell[]);

            while (count < RowCount) {
                rowArray = iterator.Take(ColCount).ToArray();

                yield return rowArray;

                ++count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TCell[]> ColumnEnumerable() {
            //  counters
            var memIndex = 0;
            var colIndex = 0;
            int rowIndex = 0;
            //  control
            var loop = ((memIndex + (rowIndex * ColCount)) < memory.Length);
            //  storage
            var colArray = default(TCell[]);

            while (loop) {
                //  clear column array
                colArray = Enumerable.Repeat(default(TCell), RowCount).ToArray();

                while (rowIndex < RowCount) {
                    colArray[rowIndex] = memory.Span[memIndex + (rowIndex * ColCount)];

                    ++rowIndex;
                }

                yield return colArray;

                rowIndex = 0;

                if (++colIndex < ColCount) {
                    ++memIndex;
                }
                else { 
                    loop = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnGridGenerated() { }
        #endregion	non-public methods & functions


        #region		private classes
        private class GridIterator : IEnumerator<TCell> {
            #region		fields
            private readonly ReadOnlyMemory<TCell> memory;

            #endregion	fields


            #region		properties
            public TCell Current => memory.Span[Index];

            object IEnumerator.Current => Current;

            public int Index { get; private set; } = -1;
            #endregion	properties


            #region		constructors & destructors
            internal GridIterator(Memory<TCell> gridMemory) {
                memory = gridMemory;
            }
            #endregion	constructors & destructors


            #region		public methods & functions
            public bool MoveNext() {
                return ++Index < memory.Length;
            }

            public void Reset() {
                Index = -1;
            }

            public void Dispose() {
                //  anything to do here???
            }

            #endregion	public methods & functions


            #region		non-public methods & functions

            #endregion	non-public methods & functions
        }
        #endregion	private classes
    }
}

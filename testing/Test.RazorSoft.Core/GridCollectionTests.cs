// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RazorSoft.Core.Collections;
//  Linked sources
using RazorSoft.Media.Drawing;


namespace UnitTest.RazorSoft.Core.Collections {

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GridCollectionTests {
        private const string POINTSARRAYFILE = @".\..\..\..\..\..\data\.RazorSoft.Source\PointsArray.cs";

        #region		configuration
        private TestContext testContext;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext {
            get { return testContext; }
            set { testContext = value; }
        }

        [ClassInitialize]
        public static void InitializeTestHarness(TestContext context) {
            WritePointArraySource(TestHexGrid.LoadHexCenterPoints());
        }

        [TestInitialize]
        public void InitializeTest() {

        }

        [TestCleanup]
        public void CleanupTest() {

        }
        #endregion	configuration


        [TestMethod]
        public void DefaultGridCollection() {
            var expCapacity = TestGridCollection.COLUMNS * TestGridCollection.ROWS;
            var grid = new TestGridCollection(Enumerable.Repeat(0, expCapacity));

            Assert.AreEqual(TestGridCollection.COLUMNS, grid.ColCount);
            Assert.AreEqual(TestGridCollection.ROWS, grid.RowCount);
            Assert.AreEqual(expCapacity, grid.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InitializeWithInvalidRange() {
            var array = GetIntegerArray(28);
            _ = new TestGridCollection(array);
        }

        [TestMethod]
        public void InitializeWithValidRange() {
            var array = GetIntegerArray(25);
            var grid = new TestGridCollection(array);

            var expLength = array.Length;
            int actLength = grid.Length;

            Assert.AreEqual(expLength, actLength);
        }

        [TestMethod]
        public void InitializeWithFunction() {
            var array = GetIntegerArray(25);

            var grid = new TestGridCollection(GenerateNextValue(array));

            var expLength = array.Length;
            int actLength = grid.Length;

            Assert.AreEqual(expLength, actLength);
        }

        [TestMethod]
        public void IterateByRows() {
            var array = new int[] {
                38,     972,    132,    264,    635,
                224,    863,    673,    14,     73,
                9312,   428,    993,    4538,   8245,
                66873,  227,    1034,   385,    818,
                397,    22,     1268,   6382,   669
            };
            var expRows = new int[][] {
                new []{ 38,     972,    132,    264,    635 },
                new []{ 224,    863,    673,    14,     73 },
                new []{ 9312,   428,    993,    4538,   8245 },
                new []{ 66873,  227,    1034,   385,    818 },
                new []{ 397,    22,     1268,   6382,   669 }
            };

            var grid = new TestGridCollection(array);

            //  5 rows 5 columns in width
            Log($"grid ({expRows.Length}x{expRows[0].Length})");

            var r = 0;
            foreach (var actRow in grid.Rows) {
                CollectionAssert.AreEqual(expRows[r], actRow, $"row {r} elements do not match: \n\texp [{string.Join(",\t", expRows[r])}]\n\tact [{string.Join(",\t", actRow)}]");

                ++r;
            }
        }

        [TestMethod]
        public void IterateByColumns() {
            var array = new int[] {
                38,     972,    132,    264,    635,
                224,    863,    673,    14,     73,
                9312,   428,    993,    4538,   8245,
                66873,  227,    1034,   385,    818,
                397,    22,     1268,   6382,   669
            };
            var expCols = new int[][] {
                new []{ 38,     224,    9312,   66873,  397 },
                new []{ 972,    863,    428,    227,    22 },
                new []{ 132,    673,    993,    1034,   1268 },
                new []{ 264,    14,     4538,   385,    6382 },
                new []{ 635,    73,     8245,   818,    669 }
            };

            var grid = new TestGridCollection(array);

            //  5 columns 5 rows in height
            Log($"grid ({expCols[0].Length}x{expCols.Length})");

            var c = 0;
            //var columns = grid.Columns.ToArray();
            foreach (var actCol in grid.Columns) {
                CollectionAssert.AreEqual(expCols[c], actCol, $"column {c} elements do not match: \n\texp [{string.Join(",\t", expCols[c])}]\n\tact [{string.Join(",\t", actCol)}]");
                Log($"[{c}] {{{string.Join("", actCol.Select(v => $"{v,7}"))}}}");

                ++c;
            }

            Assert.AreEqual(expCols.Length, c);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetGridIndexByInvalidCoordinate() {
            var capacity = TestGridCollection.COLUMNS * TestGridCollection.ROWS;
            var grid = new TestGridCollection(Enumerable.Repeat(0, capacity));

            try {
                _ = grid.Id((Y: 6, X: 4));
            }
            catch (Exception ex) {
                Log($"{ex.Message}");

                throw;
            }
        }

        [TestMethod]
        public void GetGridIndexByValidCoordinate() {
            var capacity = TestGridCollection.COLUMNS * TestGridCollection.ROWS;
            var grid = new TestGridCollection(Enumerable.Repeat(0, capacity));

            var coordinate = (Y: 3, X: 2);
            var expIndex = 17;

            var actIndex = grid.Id(coordinate);

            Assert.AreEqual(expIndex, actIndex);

            Log($"index at [{coordinate}] == {actIndex}");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValueByInvalidCoordinate() {
            var capacity = TestGridCollection.COLUMNS * TestGridCollection.ROWS;
            var grid = new TestGridCollection(Enumerable.Repeat(0, capacity));

            try {
                _ = grid[(Y: 6, X: 4)];
            }
            catch (Exception ex) {
                Log($"{ex.Message}");

                throw;
            }
        }

        [TestMethod]
        public void GetValueByValidCoordinate() {
            var array = new int[] {
                38,     972,    132,    264,    635,
                224,    863,    673,    14,     73,
                9312,   428,    993,    4538,   8245,
                66873,  227,    1034,   385,    818,
                397,    22,     1268,   6382,   669
            };
            var coordinate = (Y: 3, X: 2);
            var expValue = array[17];       //  1034

            var grid = new TestGridCollection(array);

            var actValue = grid[coordinate];

            Assert.AreEqual(expValue, actValue);

            Log($"value at [{coordinate}] == {actValue}");
        }

        [TestMethod]
        public void GridIndexBySortFunction() {
            var array = new int[] {
                38,     972,    132,    264,    635,
                224,    863,    673,    14,     73,
                9312,   428,    993,    4538,   8245,
                66873,  227,    1034,   385,    818,
                397,    22,     1268,   6382,   669
            };
            var grid = new TestGridCollection(array);

            var expSorted = array
                .Select((v, i) => (Index: i, Value: v))
                .OrderBy(e => e.Value)
                .ToArray();

            (int Index, int Value)[] actSorted = grid.Index(v => v);

            Assert.AreEqual(expSorted.Length, actSorted.Length);

            var idx = 0;
            while (idx < expSorted.Length) {
                Assert.AreEqual(expSorted[idx].Index, actSorted[idx].Index);
                Assert.AreEqual(expSorted[idx].Value, actSorted[idx].Value);

                var count = $"[{idx}]";
                Log($"{count,-5}{actSorted[idx]}");

                ++idx;
            }
        }

        [TestMethod]
        public void GenerateHexCentersGrid() {
            //  generates test points
            var hexGrid = new TestHexGrid();

            var expPointCount = hexGrid.ExpectedCenters.Length;
            var actCenters = hexGrid.Index()
                .Select(idx => {
                    Log($"{idx.Value}");

                    return idx.Value;
                })
                .ToArray();
            var actPointCount = actCenters.Length;

            Assert.AreEqual(expPointCount, actPointCount);

            CollectionAssert.AreEqual(hexGrid.ExpectedCenters, actCenters);
        }

        [TestMethod]
        public void IterateHexGridByRows() {
            var grid = new TestHexGrid();

            var expRows = PointsArray.Points.ToRows(TestHexGrid.COLUMNS);
            var actRows = grid.Rows.ToArray();

            Assert.AreEqual(expRows.Length, actRows.Length);

            var idx = 0;
            while(idx < expRows.Length) {
                CollectionAssert.AreEqual(expRows[idx], actRows[idx], $"fail at row [{idx}]");

                ++idx;
            }
        }

        [TestMethod]
        public void IterateHexGridByColumns() {
            var grid = new TestHexGrid();

            var expColumns = PointsArray.Points.ToColumns(TestHexGrid.ROWS);
            var actColumns = grid.Columns.ToArray();

            Assert.AreEqual(expColumns.Length, actColumns.Length);

            var idx = 0;
            while (idx < expColumns.Length) {
                CollectionAssert.AreEqual(expColumns[idx], actColumns[idx], $"fail at row [{idx}]");

                ++idx;
            }
        }


        #region 	utility methods
        private static int[] GetIntegerArray(int length) {
            var rnd = new Random();
            var array = new int[length];
            var idx = 0;

            while (idx < length) {
                array[idx] = rnd.Next();

                ++idx;
            }

            return array;
        }

        private HasNext<TValue> GenerateNextValue<TValue>(IEnumerable<TValue> source) {
            var iterator = source.GetEnumerator();

            bool Generator(out TValue value) {
                value = default(TValue);

                if (iterator.MoveNext()) {
                    value = iterator.Current;

                    return true;
                }

                return false;
            }

            return Generator;
        }

        private static (double X, double Y) Pixelate((double X, double Y) p) {
            var pixelated = Vector.Pixelate(p);

            return (X: p.X, Y: p.Y);
        }

        private static void WritePointArraySource(IEnumerable<(double X, double Y)> pointSource, bool overwrite = false) {
            /*
                public static class PointsArray {
                    public static (double X, double Y)[] Points = new (double X, double Y)[] {
                        (23, 20), (56, 39), (89, 20), (122, 39), (155, 20),
                        ...,      ...,      ...,      ...,       ..., 
                        ...,      ...,      ...,      ...,       ..., 
                        ...,      ...,      ...,      ...,       ..., 
                    };
                }
            */
            var FILE = POINTSARRAYFILE;

            var state = "overwrite";
            System.Diagnostics.Debug.WriteLine($"{state, -15} '{FILE}' [{overwrite}]");

            var lines = new string[] {
                "// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.",
                "",
                "",
                "using System;",
                "",
                "",
                "namespace UnitTest.RazorSoft.Core.Collections {",
                    "",
                    "\tpublic static class PointsArray {",
                        "\t\tpublic static (double X, double Y)[] Points = new (double X, double Y)[] {",
                            "\t\t\t{0}",
                        "\t\t};",
                    "\t}",
                "}"
            };

            if (!File.Exists(FILE)) {
                state = "create";
                System.Diagnostics.Debug.WriteLine($"{state, -15} '{FILE}'");

                using (var file = File.Open(FILE, FileMode.Create)) { }
                overwrite = true;
            }

            if (overwrite) {
                var idx = 0;
                var encoded = new List<byte[]>();
                var target = Array.IndexOf(lines, "\t\t\t{0}");

                state = "write data";
                System.Diagnostics.Debug.WriteLine($"{state, -15} '{FILE}'");

                using (var file = File.OpenWrite(FILE)) {
                    var buffer = default(byte[]);

                    while (idx < lines.Length) {
                        var l = $"{lines[idx]}\n";

                        if (idx == target) {
                            var cols = 0;
                            var rows = 0;
                            var values = new List<string>();

                            var iter = pointSource.GetEnumerator();

                            while (rows < 13) {
                                while (cols < 10 && iter.MoveNext()) {
                                    values.Add($"{iter.Current}");

                                    ++cols;
                                }
                                encoded.Add(Encoding.UTF8.GetBytes($"{string.Format(lines[idx], string.Join(", ", values.Select(v => $"{v,10}")))},\n"));

                                values.Clear();
                                cols = 0;

                                ++rows;
                            }

                            ++idx;

                            continue;
                        }

                        encoded.Add(Encoding.UTF8.GetBytes(l));

                        ++idx;
                    }

                    buffer = encoded
                        .SelectMany(b => b)
                        .ToArray();

                    file.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods


        #region     test class
        private class TestGridCollection : Grid<int> {
            #region		fields
            internal const int COLUMNS = 5;
            internal const int ROWS = 5;
            #endregion	fields


            #region		properties

            #endregion	properties


            #region		constructors & destructors
            internal TestGridCollection(IEnumerable<int> source) : base(COLUMNS, ROWS, source) { }

            internal TestGridCollection(HasNext<int> next) : base(COLUMNS, ROWS, next) { }
            #endregion	constructors & destructors


            #region		public methods & functions
            #endregion	public methods & functions


            #region		non-public methods & functions
            #endregion	non-public methods & functions
        }

        private class TestHexGrid : Grid<(double X, double Y)> {
            #region		fields
            private const string CENTERSFILE = @".\.Data\points.txt";

            internal const int COLUMNS = 10;
            internal const int ROWS = 13;

            //  hex size
            //      d = Abs(c.X - v.X)
            private static readonly double SPAN = 22.0;
            //      w = span * 2
            private static readonly double WIDTH = SPAN * 2;
            //      h = sqrt(3) * span
            private static readonly double HEIGHT = Math.Sqrt(3) * SPAN;
            #endregion	fields


            #region		properties
            internal (double X, double Y)[] ExpectedCenters { get; }
            #endregion	properties


            #region		constructors & destructors
            internal TestHexGrid() : base(COLUMNS, ROWS, GenerateCenterPoints(out (double X, double Y)[] loaded)) {
                ExpectedCenters = loaded;
            }
            #endregion	constructors & destructors


            #region		public methods & functions

            #endregion	public methods & functions


            #region		non-public methods & functions
            private static HasNext<(double X, double Y)> GenerateCenterPoints(out (double X, double Y)[] loaded) {
                loaded = LoadHexCenterPoints();

                var iterator = loaded.GetEnumerator();
                var loop = true;

                bool NextCenterPoint(out (double X, double Y) point) {
                    point = default((double X, double Y));

                    loop = iterator.MoveNext();

                    if (loop) {
                        point = ((double X, double Y))iterator.Current;
                    }

                    return loop;
                }

                return NextCenterPoint;
            }

            public static (double X, double Y)[] LoadHexCenterPoints() {
                var FILE = CENTERSFILE;

                var centers = new List<(double X, double Y)>();
                var data = string.Empty;

                using (var f = File.OpenRead(FILE)) {
                    var buffer = new byte[f.Length];
                    f.Read(buffer, 0, buffer.Length);

                    data = Encoding.UTF8.GetString(buffer);
                }

                centers.AddRange(data.Split("|").Select(d => Parse(d)));

                return centers.ToArray();
            }

            private static (double X, double Y) Move((double X, double Y) origin, (double M, double D) vector) {
                return (origin.X + vector.M, origin.Y + vector.D);
            }

            private static (double X, double Y) Parse(string text) {
                //  (45.00000,20.00000)
                //  strip any '(' or ')'
                text = text
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty);
                //  split on ','
                var xy = text.Split(',', StringSplitOptions.RemoveEmptyEntries);

                var x = double.Parse(xy[0]);
                var y = double.Parse(xy[1]);

                return (x, y);
            }
            #endregion	non-public methods & functions
        }
        #endregion  test class
    }
}

// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Data;
using RazorSoft.Core.Linq;
using RazorSoft.Core.Extensions;


namespace Testing.Dexter.Data.Repository {

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class JsonRepositoryTests {
        private const string SOURCE_PATH = @"..\..\..\..\data\";
        private const string DATA_DIRECTORY = ".Data";
        //  NOTE: const for interpolated strings is in preview only
        private static string DATA_PATH = $@".\{DATA_DIRECTORY}";
        private const string ARRAY_FILE = "array.json";
        private const string OBJECT_FILE = "object.json";
        private const int EXP_OBJECT_COUNT = 1;

        private static readonly DirectoryInfo DIRECTORY = new DirectoryInfo(SysEnviron.CurrentDirectory);

        private static int EXP_ARRAY_COUNT;

        private string[] expKeys = new[] { "4BD0394HAVOC", "B4BC8647LSLL" };
        private string[] expNames = new[] { "Houdini", "Huckleberry" };
        private string arrayFile;
        private string objectFile;


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
            if (!Directory.Exists(DATA_PATH)) {
                Assert.IsTrue(Directory.Exists(new DirectoryInfo(DATA_PATH).CreatePath()));
            }

            JsonRepository.RootPath = DATA_DIRECTORY;

            Assert.IsTrue(File.Exists(Path.Combine(SOURCE_PATH, ARRAY_FILE)));
            Assert.IsTrue(File.Exists(Path.Combine(SOURCE_PATH, OBJECT_FILE)));
        }

        [ClassCleanup]
        public static void CleanupTestHarness() {

        }

        [TestInitialize]
        public void InitializeTest() {
            DIRECTORY.SetRoot(DATA_PATH, out DirectoryInfo root);

            Assert.IsTrue(new FileInfo(Path.Combine(SOURCE_PATH, ARRAY_FILE)).Clone(root.FullName, out arrayFile));

            Log($"Cloned {ARRAY_FILE} to ['{arrayFile}']");

            Assert.IsTrue(new FileInfo(Path.Combine(SOURCE_PATH, OBJECT_FILE)).Clone(root.FullName, out objectFile));

            Log($"Cloned {OBJECT_FILE} to ['{objectFile}']");
        }

        [TestCleanup]
        public void CleanupTest() {

        }
        #endregion	configuration


        [TestMethod]
        public void DefaultArrayJsonRepository() {
            JsonRepository repo = new TestRepository(Log, arrayFile);

            Assert.AreEqual(DATA_PATH, JsonRepository.RootPath, "root path");
            Assert.AreEqual($@"...\{DATA_DIRECTORY}\", repo.DataPath, "data path");
            Assert.AreEqual(ARRAY_FILE, repo.DataFile, "data file");

            Log($"Repo Root Path [{repo.DataPath}] {repo.DataFile}");
        }

        [TestMethod]
        public void DefaultObjectJsonRepository() {
            JsonRepository repo = new TestRepository(Log, objectFile);

            Assert.AreEqual(DATA_PATH, JsonRepository.RootPath, "root path");
            Assert.AreEqual($@"...\{DATA_DIRECTORY}\", repo.DataPath, "data path");
            Assert.AreEqual(OBJECT_FILE, repo.DataFile, "data file");

            Log($"Repo Root Path [{repo.DataPath}] {repo.DataFile}");
        }

        [TestMethod]
        public void LoadDataItemArrayRepository() {
            var expCount = EXP_ARRAY_COUNT = expKeys.Length;
            GuidDataClassRepository repo = new(Log, arrayFile);

            Assert.AreEqual(expCount, repo.Collection.Count, "items count");

            foreach (var item in repo.Collection) {
                if (item is GuidDataClass guidData) {
                    Log($"{guidData.Key} -- {guidData.Name}");
                }
            }
        }

        [TestMethod]
        public void LoadDataItemObjectRepository() {
            var expCount = EXP_OBJECT_COUNT;
            DataClassRepository repo = new(Log, objectFile);

            Assert.AreEqual(expCount, repo.Collection.Count, "items count");

            foreach (var item in repo.Collection) {
                if (item is DataClass data) {
                    Log($"{data.Key} -- {data.Name}");
                }
            }
        }

        [TestMethod]
        public void AddItemToArrayRepository() {
            EXP_ARRAY_COUNT = expKeys.Length;
            GuidDataClassRepository repo = new(Log, arrayFile);
            EXP_ARRAY_COUNT = repo.Collection.Count;

            object item = new GuidDataClass() {
                Guid = Guid.NewGuid(),
                Key = "DF47765AAHPI",
                Name = "Jacob",
                Number = 76,
                Boolean = false
            };
            GuidDataClass expData = null;

            if (repo.Add(item) is GuidDataClass data) {
                //  increment expected count
                ++EXP_ARRAY_COUNT;
                expData = data;
            }

            //  collection count should be +1
            Assert.AreEqual(EXP_ARRAY_COUNT, repo.Collection.Count);

            repo.Commit();
            repo.Dispose();

            //  re-initialize repository
            repo = new(Log, arrayFile);
            GuidDataClass actData = repo.All()
                .Select((i) => i as GuidDataClass)
                .Where(i => i.Key == expData.Key)
                .FirstOrDefault();

            Assert.IsNotNull(actData);
        }

        [TestMethod]
        public void AddItemToObjectRepository() {
            DataClassRepository repo = new(Log, objectFile);

            object item = new DataClass() {
                Key = "DF47765AAHPI",
                Name = "Jacob",
                Number = 76,
                Boolean = false
            };
            DataClass expData = null;

            if (repo.Add(item) is DataClass data) {
                expData = data;
            }

            //  collection count should be 1
            Assert.AreEqual(EXP_OBJECT_COUNT, repo.Collection.Count);

            repo.Commit();
            repo.Dispose();

            //  re-initialize repository
            repo = new(Log, objectFile);
            DataClass actData = repo.All()
                .Select((i) => i as DataClass)
                .Where(i => i.Key == expData.Key)
                .FirstOrDefault();

            Assert.IsNotNull(actData);
        }

        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods


        #region     test classes
        /// <summary>
        /// Test ArrayData (R/W) methods
        /// </summary>
        private class GuidDataClassRepository : TestRepository {

            #region		constructors & destructors
            public GuidDataClassRepository(Action<string> logFunc, string dataPath) : base(logFunc, dataPath) {
                Load();
            }
            #endregion	constructors & destructors


            //  customize read
            protected override ICollection OnRead(JsonLoader loader) {
                ArrayList items = new(loader.Read<List<GuidDataClass>>());

                Assert.IsTrue(items.Count == EXP_ARRAY_COUNT);

                Log($"loaded multiple [{nameof(GuidDataClass)}] items {string.Join(", ", items.Select((GuidDataClass i) => i.Key))}");

                return items;
            }

            //  customize write
            protected override void OnWrite(JsonLoader loader) {
                loader.Write(Cache().ToList<GuidDataClass>());

                Log($"committed {Collection.Count} [{nameof(GuidDataClass)}] items to data store");
            }
        }
        /// <summary>
        /// Test ObjectData (R/W) methods
        /// </summary>
        private class DataClassRepository : TestRepository {

            #region		constructors & destructors
            public DataClassRepository(Action<string> logFunc, string dataPath) : base(logFunc, dataPath) {
                Load();
            }
            #endregion	constructors & destructors


            //  customize add - only has a single object ever in the repository
            protected override object Add(IList list, object item) {
                return list[0] = item;
            }

            //  customize read
            protected override ICollection OnRead(JsonLoader loader) {
                //  expects a single object to be loaded so is loaded into internal Cache
                ArrayList items = new() { loader.Read<DataClass>() };

                Assert.IsTrue(items.Count == EXP_OBJECT_COUNT);

                DataClass data = null;

                if (items[0] is DataClass dataClass) {
                    data = dataClass;
                }

                Log($"loaded single [{nameof(DataClass)}] item {data.Key}");

                return items;
            }

            //  customize write
            protected override void OnWrite(JsonLoader loader) {
                var data = Cache().SingleOrDefault<DataClass>();

                loader.Write(data);
            }
        }

        private class TestRepository : JsonRepository {
            #region		fields
            private readonly IList cache = new ArrayList();
            #endregion	fields


            #region		properties
            internal ICollection Collection => cache;

            protected Action<string> Log { get; set; }
            #endregion	properties


            #region		constructors & destructors
            public TestRepository(Action<string> logFunc, string dataPath) : base(dataPath) {
                Log = logFunc;
            }
            #endregion	constructors & destructors


            #region		public methods & functions
            #endregion	public methods & functions


            #region		non-public methods & functions
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <typeparam name="TData"></typeparam>
            /// <returns></returns>
            protected override IList Cache() {
                return cache;
            }
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <typeparam name="TData"></typeparam>
            /// <param name="data"></param>
            protected override void OnDataLoaded(IEnumerable data) {
                cache.Clear();
                cache.AddRange(data);
            }
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            protected override void OnInitialized() {
                // nothing to test here (yet)
            }
            protected override ICollection OnRead(JsonLoader loader) {
                throw new NotImplementedException($"read method is not implemented by {GetType().Name}");
            }
            protected override void OnWrite(JsonLoader loader) {
                throw new NotImplementedException($"write method is not implemented by {GetType().Name}");
            }
            #endregion	non-public methods & functions
        }

        private class DataClass {
            public string Key { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public bool Boolean { get; set; }
        }

        private class GuidDataClass : DataClass {
            public Guid Guid { get; set; }
        }
        #endregion  test class
    }
}

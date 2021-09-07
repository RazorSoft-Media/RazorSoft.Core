// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Collections;
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

        private static readonly DirectoryInfo DIRECTORY = new DirectoryInfo(SysEnviron.CurrentDirectory);

        private string[] expIds = new[] { "4BD0394HAVOC", "B4BC8647LSLL" };
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
            JsonRepository repo = new TestRepository(arrayFile);

            Assert.AreEqual(DATA_PATH, JsonRepository.RootPath, "root path");
            Assert.AreEqual($@"...\{DATA_DIRECTORY}\", repo.DataPath, "data path");
            Assert.AreEqual(ARRAY_FILE, repo.DataFile, "data file");

            Log($"Repo Root Path [{repo.DataPath}] {repo.DataFile}");
        }

        [TestMethod]
        public void DefaultObjectJsonRepository() {
            JsonRepository repo = new TestRepository(objectFile);

            Assert.AreEqual(DATA_PATH, JsonRepository.RootPath, "root path");
            Assert.AreEqual($@"...\{DATA_DIRECTORY}\", repo.DataPath, "data path");
            Assert.AreEqual(OBJECT_FILE, repo.DataFile, "data file");

            Log($"Repo Root Path [{repo.DataPath}] {repo.DataFile}");
        }

        [TestMethod]
        public void LoadDataItemArrayRepository() {
            var expCount = expIds.Length;
            TestGuidDataRepository repo = new(arrayFile);

            Assert.AreEqual(expCount, repo.Collection.Count, "items count");

            foreach (var item in repo.Collection) {
                if (item is GuidDataClass guidData) {
                    Log($"{guidData.Id} -- {guidData.Name}");
                }
            }
        }

        [TestMethod]
        public void LoadDataItemObjectRepository() {
            var expCount = 1;
            TestDataRepository repo = new(objectFile);

            Assert.AreEqual(expCount, repo.Collection.Count, "items count");

            foreach (var item in repo.Collection) {
                if (item is DataClass data) {
                    Log($"{data.Id} -- {data.Name}");
                }
            }
        }

        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods


        #region     test classes
        private class TestDataRepository : TestRepository {

            #region		constructors & destructors
            public TestDataRepository(string dataPath) : base(dataPath) {
                Load<DataClass>();
            }
            #endregion	constructors & destructors

        }

        private class TestGuidDataRepository : TestRepository {

            #region		constructors & destructors
            public TestGuidDataRepository(string dataPath) : base(dataPath) {
                Load<GuidDataClass>();
            }
            #endregion	constructors & destructors

        }

        private class TestRepository : JsonRepository {
            #region		fields
            private readonly IList cache = new ArrayList();
            #endregion	fields


            #region		properties
            internal ICollection Collection => cache;
            #endregion	properties


            #region		constructors & destructors
            public TestRepository(string dataPath) : base(dataPath) {

            }
            #endregion	constructors & destructors


            #region		public methods & functions

            #endregion	public methods & functions


            #region		non-public methods & functions
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <typeparam name="TData"></typeparam>
            /// <param name="data"></param>
            protected override void OnLoad(IEnumerable data) {
                cache.Clear();
                cache.AddRange(data);
            }
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <typeparam name="TData"></typeparam>
            /// <returns></returns>
            protected override IList Cache() {
                return cache;
            }

            #endregion	non-public methods & functions
        }

        private class DataClass {
            public string Id { get; set; }
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

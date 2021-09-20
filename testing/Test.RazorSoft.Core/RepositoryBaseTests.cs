// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Data;
using RazorSoft.Core.Extensions;


namespace Testing.Dexter.Data.Repository {

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RepositoryBaseTests {
        private const string SOURCE_PATH = @"..\..\..\..\data\";
        private const string DATA_DIRECTORY = ".Data";
        //  NOTE: const for interpolated strings is in preview only
        private static string DATA_PATH = $@".\{DATA_DIRECTORY}";
        private const string ORG_FILE = "organizations.json";

        private static readonly DirectoryInfo DIRECTORY = new DirectoryInfo(SysEnviron.CurrentDirectory);

        private static string ORG_FILE_PATH;
        private static List<Organization> ORG_LIST;

        private OrganizationRepository repository = new();

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

            Assert.IsTrue(File.Exists(Path.Combine(SOURCE_PATH, ORG_FILE)));

            DIRECTORY.SetRoot(DATA_PATH, out DirectoryInfo root);

            Assert.IsTrue(new FileInfo(Path.Combine(SOURCE_PATH, ORG_FILE)).Clone(root.FullName, out ORG_FILE_PATH));

            context.WriteLine($"Cloned {ORG_FILE} to ['{ORG_FILE_PATH}']");

            using (var stream = File.OpenRead(ORG_FILE_PATH)) {
                var buffer = new byte[stream.Length];
                stream.Read(buffer);

                Utf8JsonReader reader = new(buffer);

                ORG_LIST = JsonSerializer.Deserialize<List<Organization>>(ref reader);
            }

            Assert.AreEqual(4, ORG_LIST.Count);
        }

        [ClassCleanup]
        public static void CleanupTestHarness() {

        }

        [TestInitialize]
        public void InitializeTest() {
            if (repository.Logger == null) {
                repository.Logger = Log;
            }

            foreach(var o in ORG_LIST) {
                repository.Add(o);
            }
        }

        [TestCleanup]
        public void CleanupTest() {
            repository.Dispose();
        }
        #endregion	configuration


        [TestMethod]
        public void DefaultRepositoryImplementation() {
            var expDataSource = "TestContext";
            var actDataSource = string.Empty;

            using (OrganizationRepository repo = new()) {
                actDataSource = repo.DataSource;
            }

            Assert.AreEqual(expDataSource, actDataSource);

            Log($"Repo.DataSource [{actDataSource}]");
        }

        [TestMethod]
        public void GetAllItems() {
            string[] orgIds = new[] { "ECA51301LSLL", "416EC836HGIU", "F99240BBHVAC", "DF47765AAHPI" };
            List<Organization> orgs = default;

            orgs = repository.All()
                .ToList();

            Assert.AreEqual(orgIds.Length, orgs.Count);
        }

        [TestMethod]
        public void AddNewData() {
            var orgKey = "F96EC80AQJLI";
            repository.Add(new() {
                Key = orgKey,
                Name = "Quacker Jacked Locksmithy, Inc",
                Active = true
            });

            Organization org = repository.Get(o => o.Key == orgKey)
                .FirstOrDefault();

            Assert.IsNotNull(org);

            Log($"added org [{org.Name} - {org.Key}]");
        }


        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods
    }
}

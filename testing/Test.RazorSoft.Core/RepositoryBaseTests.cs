// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Data;
using RazorSoft.Core.Extensions;
using RazorSoft.Core.Messaging;
//
using Testing.Dexter.Services;
using Testing.Dexter.Data.Repositories;


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

        private string orgFile;


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
            CommandRouter.Default.AddRoute<IOrganizationAPI>(new OrganizationService());

            if (!Directory.Exists(DATA_PATH)) {
                Assert.IsTrue(Directory.Exists(new DirectoryInfo(DATA_PATH).CreatePath()));
            }

            JsonRepository.RootPath = DATA_DIRECTORY;

            Assert.IsTrue(File.Exists(Path.Combine(SOURCE_PATH, ORG_FILE)));
        }

        [ClassCleanup]
        public static void CleanupTestHarness() {

        }

        [TestInitialize]
        public void InitializeTest() {
            DIRECTORY.SetRoot(DATA_PATH, out DirectoryInfo root);

            Assert.IsTrue(new FileInfo(Path.Combine(SOURCE_PATH, ORG_FILE)).Clone(root.FullName, out orgFile));

            Log($"Cloned {ORG_FILE} to ['{orgFile}']");
        }

        [TestCleanup]
        public void CleanupTest() {

        }
        #endregion	configuration


        [TestMethod]
        public void DefaultRepositoryImplementation() {
            var dataSource = string.Empty;

            using (OrganizationRepository repo = new()) {
                dataSource = repo.DataSource;
            }

            Log($"Repo.DataSource [{dataSource}]");
        }

        [TestMethod]
        public void GetAllItems() {
            string[] orgIds = new[] { "ECA51301LSLL", "416EC836HGIU", "F99240BBHVAC", "DF47765AAHPI" };
            List<Organization> orgs = default;

            using (OrganizationRepository repo = new()) {
                orgs = repo.All()
                    .ToList();
            }


            Assert.AreEqual(orgIds.Length, orgs.Count);
        }

        [TestMethod]
        public void AddNewData() {
            Organization org = default;

            using (OrganizationRepository repo = new()) {
                org = repo.Add(new() {
                    Name = "Quacker Jacked Locksmithy, Inc",
                    Active = true
                });
            }

            Assert.IsNotNull(org);
            Assert.IsFalse(string.IsNullOrEmpty(org.Key));

            Log($"added org [{org.Name} - {org.Key}]");
        }

        [TestMethod]
        public void PersistDataChanges() {
            Organization expOrg = default;
            Organization actOrg = default;

            using (OrganizationRepository repo = new()) {
                expOrg = repo.Add(new() {
                    Name = "Quacker Jacked Locksmithy, Inc",
                    Active = true
                });

                repo.Commit();
            }

            using (OrganizationRepository repo = new()) {
                actOrg = repo.Get(o => o.Key == expOrg.Key)
                    .FirstOrDefault();
            }

            Assert.IsNotNull(actOrg);

            Log($"found org [{actOrg.Name} - {actOrg.Key}]");
        }

        [TestMethod]
        public void DeleteData() {
            string[] orgIds = new[] { "ECA51301LSLL", "416EC836HGIU", "F99240BBHVAC", "DF47765AAHPI" };
            Organization org = default;
            bool result = false;

            using (OrganizationRepository repo = new()) {
                org = repo.Get(o => o.Key == orgIds[2])
                    .FirstOrDefault();

                if(org is not null) {
                    result = repo.Delete(org);
                }

                repo.Commit();
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateData() {
            string key = "F99240BBHVAC";
            bool expValue = true;

            Organization organization;

            using (OrganizationRepository repo = new()) {
                organization = repo.Get(o => o.Key == key)
                    .FirstOrDefault();

                Log($" {organization.Name} Active [{organization.Active}]");
                organization.Active = expValue;

                Assert.IsTrue(repo.Update(organization), $"organization '{organization.Name}' not found");

                repo.Commit();
            }

            //  reload
            using (OrganizationRepository repo = new()) {
                organization = repo.Get(o => o.Key == key)
                    .FirstOrDefault();
            }

            Assert.AreEqual(expValue, organization.Active);

            Log($" {organization.Name} Active [{organization.Active}]");
        }


        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods
    }
}

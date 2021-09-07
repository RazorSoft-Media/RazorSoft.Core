// Copyright © 2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Extensions;


namespace UnitTest.RazorSoft.Core {
    [TestClass]
    public class ExtensionsTests {
        private const string EXP_DATA_FILE = @"..\..\..\..\data\data.json";

        private static DirectoryInfo currentDirectory = new DirectoryInfo(SysEnviron.CurrentDirectory);
        private static string[] subDirs = { "radium", "test0", "test1", "test2" };

        private TestContext context;
        private string testPath;

        #region test harness configuration
        public TestContext TestContext {
            get => context;
            set => context = value;
        }

        [ClassInitialize]
        public static void InitializeTestHarness(TestContext context) {
            Assert.IsTrue(File.Exists(EXP_DATA_FILE));
        }

        [TestCleanup]
        public void CleanupTest() {
            testPath = currentDirectory.CombinePaths(subDirs[0]).FullName;

            if (Directory.Exists(testPath)) {
                //  recursive delete
                Directory.Delete(testPath, true);
            }

            Assert.IsFalse(Directory.Exists(testPath), $"{testPath} exists");
        }
        #endregion


        [TestMethod]
        public void CombineDirectoryPaths_1() {
            var expFullPath = $@"{currentDirectory.FullName}\radium\test0\test1\test2";
            var actFullPath = currentDirectory.CombinePaths(subDirs[0], subDirs.Skip(1).ToArray());

            Assert.AreEqual(expFullPath, actFullPath.FullName);

            Debug.WriteLine($"Full path: {actFullPath}");
        }

        [TestMethod]
        public void CombineDirectoryPaths_2() {
            var expFullPath = $@"{currentDirectory.FullName}\radium\test0\test1\test2";
            var actFullPath = currentDirectory.CombinePaths(string.Join(@"\", subDirs));

            Assert.AreEqual(expFullPath, actFullPath.FullName);

            Debug.WriteLine($"Full path: {actFullPath}");
        }

        [TestMethod]
        public void CreateDirectoryPath_1() {
            var expFullPath = $@"{currentDirectory.FullName}\radium\test0\test1\test2";
            var actFullPath = new DirectoryInfo(expFullPath).CreatePath();

            Assert.AreEqual(expFullPath, actFullPath);
            Debug.WriteLine($"Full path [{actFullPath}] exists: {Directory.Exists(actFullPath)}");

            testPath = actFullPath;
        }

        [TestMethod]
        public void AbbreviateFilePath() {
            var expFilePath = $@"{currentDirectory.FullName}\radium\test.txt";

            //  test path
            var fullPath = $@"{currentDirectory.FullName}\radium\";
            testPath = new DirectoryInfo(fullPath).CreatePath();
            //  ---------

            var filePath = Path.Combine(testPath, "test.txt");

            FileInfo fInfo;

            using(var f = File.Create(filePath)) {
                fInfo = new FileInfo(filePath);
            }

            var actFilePath = fInfo.FullName;

            Assert.AreEqual(expFilePath, actFilePath);
            Debug.WriteLine($"Full path [{actFilePath}] exists: {File.Exists(actFilePath)}");

            //  abbreviated depth: 1 (DEFAULT)
            var expLvl1AbbrPath = @"...\radium\test.txt";
            var actLvl1AbbrPath = fInfo.AbbreviatePath();

            Assert.AreEqual(expLvl1AbbrPath, actLvl1AbbrPath);
            Debug.WriteLine($"Abbreviated path (Depth=1) [{actLvl1AbbrPath}]");

            //  abbreviated depth: 3
            var expLvl2AbbrPath = @"...\Debug\net5.0\radium\test.txt";
            var actLvl2AbbrPath = fInfo.AbbreviatePath(3);

            Assert.AreEqual(expLvl2AbbrPath, actLvl2AbbrPath);
            Debug.WriteLine($"Abbreviated path (Depth=3) [{actLvl2AbbrPath}]");

            File.Delete(fInfo.FullName);
        }

        [TestMethod]
        public void AbbreviateDirectoryPath() {
            //  trailing '\' would cause an empty entry 
            //    - fixed using StringSplitOptions.RemoveEmptyEntries
            var fullPath = $@"{currentDirectory.FullName}\radium\depth_2\depth_1\";
            DirectoryInfo testDirectory;
            testPath = (testDirectory = new DirectoryInfo(fullPath)).CreatePath();

            //  abbreviated depth: 1 (DEFAULT)
            var expLvl1AbbrPath = @"...\depth_1\";
            var actLvl1AbbrPath = testDirectory.AbbreviatePath();

            Assert.AreEqual(expLvl1AbbrPath, actLvl1AbbrPath);
            Debug.WriteLine($"Abbreviated path (Depth=1) [{actLvl1AbbrPath}]");

            //  abbreviated depth: 3
            var expLvl2AbbrPath = @"...\radium\depth_2\depth_1\";
            var actLvl2AbbrPath = testDirectory.AbbreviatePath(3);

            Assert.AreEqual(expLvl2AbbrPath, actLvl2AbbrPath);
            Debug.WriteLine($"Abbreviated path (Depth=3) [{actLvl2AbbrPath}]");

        }

        [TestMethod]
        public void SetSimpleDirectoryRoot() {
            //  ".\testing\"
            var expPath = new DirectoryInfo(EXP_DATA_FILE).Parent.Parent;

            Assert.AreEqual("testing", expPath.Name);
            Assert.IsTrue(currentDirectory.SetRoot(@"testing", out DirectoryInfo actPath));

            Assert.AreEqual(expPath.FullName, actPath.FullName);

            Log($"rooted path: {actPath}");
        }

        [TestMethod]
        public void SetComplexDirectoryRoot() {
            //  ".\testing\data"
            var expPath = new DirectoryInfo(EXP_DATA_FILE).Parent;

            Assert.AreEqual("testing", expPath.Parent.Name);
            Assert.IsTrue(currentDirectory.SetRoot(@"testing\data", out DirectoryInfo actPath));

            Assert.AreEqual(expPath.FullName, actPath.FullName);

            Log($"rooted path: {actPath}");
        }

        [TestMethod]
        public void ExtendDirectoryToRoot() {
            var expPath = new DirectoryInfo(EXP_DATA_FILE).Parent;

            //  ".\testing\"
            currentDirectory.SetRoot(@"testing", out DirectoryInfo path);
            Log($"original path: {path}");

            //   extend the path to a child directory by prepending new root with '.\'
            //  ".\testing\data"
            Assert.IsTrue(path.SetRoot(@".\data", out DirectoryInfo actPath), $"did not extend root: {expPath} [{(actPath?.FullName ?? "empty")}]");
            Assert.AreEqual(expPath.FullName, actPath.FullName);

            Log($"rooted path: {actPath}");
        }

        [TestMethod]
        public void AbbreviatePathToKnownDirectory() {
            var dataPath = new DirectoryInfo(EXP_DATA_FILE).Parent;
            var expAbbreviatedPath = @"...\testing\data\";

            var actAbbreviatedPath = dataPath.AbbreviatePath("testing");

            Assert.AreEqual(expAbbreviatedPath, actAbbreviatedPath);

            Log($"original [{dataPath.FullName}] abbreviated [{actAbbreviatedPath}]");
        }


        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods
    }
}

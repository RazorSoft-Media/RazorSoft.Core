// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RazorSoft.Core.IO;


namespace Testing.RazorSoft.Core {

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StorageTests {
        private const string ROOT_PATH = @".\.Data";
        private const string FILE_NAME = "datafile.dat";

        private static DirectoryInfo ROOT;



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
            if (!Directory.Exists(ROOT_PATH)) {
                Directory.CreateDirectory(ROOT_PATH);
            }

            Assert.IsTrue(Directory.Exists(ROOT_PATH));

            ROOT = new(ROOT_PATH);
            context.WriteLine($"ROOT [{ROOT.FullName}]");
        }

        [ClassCleanup]
        public static void CleanupTestHarness() {

        }

        [TestInitialize]
        public void InitializeTest() {

        }

        [TestCleanup]
        public void CleanupTest() {
            File.Delete(Path.Combine(ROOT_PATH, FILE_NAME));
        }
        #endregion	configuration


        [TestMethod]
        public void StorageCreateFile() {
            var filePath = Storage.Create(Path.Combine(ROOT_PATH, FILE_NAME));

            Assert.IsTrue(File.Exists(filePath));

            Log($"created file '{filePath}' [Length: {new FileInfo(filePath).Length} bytes]");
        }

        [TestMethod]
        public void StorageIsFile_False_DirExistTrue() {
            bool result = true;

            Assert.IsFalse(result = Storage.IsFile(ROOT_PATH, out bool exists));
            Assert.IsTrue(exists, "directory does not exist");

            Log($"'{ROOT_PATH}' is a {(result ? "file" : "directory")}");
        }

        [TestMethod]
        public void StorageIsFile_True_FileExistFalse() {
            bool result = false;
            string path = string.Empty;

            Assert.IsTrue(result = Storage.IsFile(path = Path.Combine(ROOT_PATH, FILE_NAME), out bool exists));
            Assert.IsFalse(exists, "file exists");

            Log($"'{path}' {(exists ? (result ? "is a file" : "is a directory") : $" {(result ? "file" : "directory")} does not exist")}");
        }

        [TestMethod]
        public void StorageIsFile_True_FileExistTrue() {
            var filePath = Storage.Create(Path.Combine(ROOT_PATH, FILE_NAME));

            bool result = false;

            Assert.IsTrue(result = Storage.IsFile(filePath, out bool exists));
            Assert.IsTrue(exists, "file exists");

            Log($"'{filePath}' {(exists ? (result ? $"is a file  [Length: {new FileInfo(filePath).Length} bytes]" : "is a directory") : "does not exist")}");
        }

        [TestMethod]
        [ExpectedException(typeof(StorageOperationException))]
        public void InstantiateStoreFromFilePath_StorageOperation() {
            string fullPath;
            FileInfo fileInfo = new(fullPath = Path.Combine(ROOT_PATH, FILE_NAME));

            try {
                var storage = Storage.FromPath(fileInfo.FullName);
            }
            catch (Exception ex) {
                Log(ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void InstantiateStoreFromFilePath() {
            string fullPath;
            FileInfo fileInfo = new(fullPath = Path.Combine(ROOT_PATH, FILE_NAME));
            var storage = Storage.FromPath(fileInfo.FullName, true);

            Assert.AreEqual(Path.GetFullPath(fullPath), storage.FullPath);

            Log($"storage ['{storage.FullPath}']");
        }

        [TestMethod]
        public void StorageOpenBinary() {
            string fullPath;
            FileInfo fileInfo = new(fullPath = Path.Combine(ROOT_PATH, FILE_NAME));
            var storage = Storage.FromPath(fileInfo.FullName, true);

            using(var stream = storage.OpenBinary(AccessMode.Read)) {
                Assert.IsInstanceOfType(stream, typeof(BinaryStream), "stream is not BinaryStream");
                Assert.IsTrue(stream.CanRead);
                Assert.IsFalse(stream.CanWrite);
            }
        }

        [TestMethod]
        public void WriteToBinaryStorage() {
            string fullPath;
            FileInfo fileInfo = new(fullPath = Path.Combine(ROOT_PATH, FILE_NAME));
            var storage = Storage.FromPath(fileInfo.FullName, true);

            var expDateTime = DateTime.UtcNow;
            var actDateTime = default(DateTime);

            using (var stream = storage.OpenBinary(AccessMode.Read | AccessMode.Write)) {
                var length = 0;

                using (var writer = stream.Write()) {
                    var buffer = BitConverter.GetBytes(expDateTime.ToFileTimeUtc());
                    length = buffer.Length;

                    writer.Write(buffer);
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = stream.Read()) {
                    var buffer = reader.ReadBytes(length);
                    var ticks = BitConverter.ToInt64(buffer);

                    actDateTime = DateTime.FromFileTimeUtc(ticks);
                }
            }

            Assert.AreEqual(expDateTime, actDateTime);
        }

        [TestMethod]
        public void StorageOpenFile() {
            string fullPath;
            FileInfo fileInfo = new(fullPath = Path.Combine(ROOT_PATH, FILE_NAME));
            var storage = Storage.FromPath(fileInfo.FullName, true);

            using (var stream = storage.OpenFile(AccessMode.Read)) {
                Assert.IsInstanceOfType(stream, typeof(FileStream), "stream is not FileStream");
                Assert.IsTrue(stream.CanRead);
                Assert.IsFalse(stream.CanWrite);
            }
        }

        [TestMethod]
        public void WriteToFileStorage() {
            string fullPath;
            FileInfo fileInfo = new(fullPath = Path.Combine(ROOT_PATH, FILE_NAME));
            var storage = Storage.FromPath(fileInfo.FullName, true);

            var expDateTime = DateTime.UtcNow;
            var actDateTime = default(DateTime);
            var length = 0;

            using (var stream = storage.OpenFile(AccessMode.Write)) {
                var output = BitConverter.GetBytes(expDateTime.ToFileTimeUtc());
                length = output.Length;

                stream.Write(output, 0, output.Length);
            }

            using (var stream = storage.OpenFile(AccessMode.Read)) {
                var input = new byte[length];
                stream.Read(input);
                var ticks = BitConverter.ToInt64(input);

                actDateTime = DateTime.FromFileTimeUtc(ticks);
            }

            Assert.AreEqual(expDateTime, actDateTime);
        }


        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods
    }
}

/* ***********************************************
 *  © 2020 RazorSoft Media, DBA
 *         Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Extensions;
using System;

namespace UnitTest.RazorSoft.Core {
    [TestClass]
    public class ExtensionsTests {
        private static DirectoryInfo currentDirectory = new DirectoryInfo(SysEnviron.CurrentDirectory);
        private static string[] subDirs = { "radium", "test0", "test1", "test2" };

        private string testPath;

        [TestCleanup]
        public void CleanupTest() {
            testPath = currentDirectory.CombinePaths(subDirs[0]).FullName;

            if (Directory.Exists(testPath)) {
                //  recursive delete
                Directory.Delete(testPath, true);
            }

            Assert.IsTrue(!Directory.Exists(testPath), $"{testPath} exists");
         }

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
        public void GenerateRandomString_1() {
            var numeric = new Random().GenerateNumberString(10);
            var letter = new Random().GenerateLetterString(10);
            var upper = new Random().GenerateLetterString(10, LetterCase.Upper);
            var lower = new Random().GenerateLetterString(10, LetterCase.Lower);
            var alpha = new Random().GenerateAlphaNumericString(10, LetterCase.Mixed);

            Debug.WriteLine(numeric + " - Numeric");
            Debug.WriteLine(letter + " - Alpha");
            Debug.WriteLine(upper + " - UpperAlpha");
            Debug.WriteLine(lower + " - LowerAlpha");
            Debug.WriteLine(alpha + " - AlphaNumeric");
        }
    }
}

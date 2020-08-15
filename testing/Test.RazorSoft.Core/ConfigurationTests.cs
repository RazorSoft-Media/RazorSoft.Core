/* ***********************************************
 *  © 2020 RazorSoft Media, DBA
 *         Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Extensions;
using RazorSoft.Core.Configuration;


namespace UnitTest.RazorSoft.Core {

    [TestClass]
    public class ConfigurationTests {
        private static readonly string LOCAL = "test";
        private static readonly string CONFIG = "test";

        private static IConfiguration configuration;
        private static Settings settings;

        [ClassInitialize]
        public static void SetupTests(TestContext context) {
            settings = (Settings)(configuration = new Settings(LOCAL, CONFIG));
        }
        [ClassCleanup]
        public static void CleanupTests() {
            //  recursive delete
            Directory.Delete(configuration.FilePath, true);
        }
        [TestCleanup]
        public void CleanupTest() {
            configuration.Clear();
        }

        [TestMethod]
        public void DefaultSettings() {
            Assert.IsTrue(Directory.Exists(configuration.FilePath), $"[path] {configuration.FilePath} does not exist");
            Assert.IsTrue(File.Exists(Path.Combine(configuration.FilePath, configuration.FileName)), $"[file] {configuration.FileName} does not exist");

            Debug.WriteLine($"Settings Path: {configuration.FilePath}");
            Debug.WriteLine($"Settings File: {configuration.FileName}");
        }

        [TestMethod]
        public void AddSetting() {
            var key = "key0";
            var expValue = 1;

            configuration.Add(key, expValue);

            Assert.IsTrue(configuration.Keys.Any(k => k == key));
            Assert.AreEqual(expValue, configuration[key].DecodeAs<int>());

            configuration.Save();
        }

        [TestMethod]
        public void ChangeSetting() {
            var key = "key0";
            var expValue = DateTime.UtcNow;

            //  initial setting
            configuration.Add(key, 1);
            configuration.Save();

            //  as a matter of completeness, Set will also Add key if missing...
            configuration.Set(key, expValue);

            Assert.AreEqual(expValue, configuration[key].DecodeAs<DateTime>());

            configuration.Save();
        }

        [TestMethod]
        public void GetSetting() {
            var key = "key0";
            var expValue = DateTime.UtcNow;

            configuration.Add(key, expValue);

            var actValue = configuration.Get<DateTime>(key);

            Assert.AreEqual(expValue, actValue);
        }

        private class Settings : Configuration {
            public Settings(string path = "", string file = "") : base(path, file) {
            }
        }
    }
}

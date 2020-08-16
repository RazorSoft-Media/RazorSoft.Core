/* ***********************************************
 *  © 2020 RazorSoft Media, DBA
 *         Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RazorSoft.Core.Logging;
using RazorSoft.Core.Modules;
using RazorSoft.Core.Extensions;

namespace UnitTest.RazorSoft.Core {
    [TestClass]
    public class ScribeTests {
        private static readonly string LOCAL = @"test\logs";
        private static readonly string LOG_PATH = Path.Combine(Environment.CurrentDirectory, LOCAL);

        [ClassInitialize]
        public static void InitializeTests(TestContext context) {
            if (!Directory.Exists(LOG_PATH)) {
                Directory.CreateDirectory(LOG_PATH);
            }
        }

        [ClassCleanup]
        public static void CleanupTests() {
            Directory.Delete(LOG_PATH, true);
        }

        [TestMethod]
        public void CreateLogFile() {
            IFileLogger logger = new FileLogger(LOG_PATH);

            //  file exists
            Assert.IsTrue(Directory.Exists(LOG_PATH));
            Assert.IsTrue(File.Exists(logger.LogPath));
        }

        [TestMethod]
        public void FileLogEntry() {
            var expLogEntry = "This_is_a_test";
            IFileLogger logger = new FileLogger(LOG_PATH);

            logger.Log(expLogEntry);

            //  default file content
            var logText = string.Empty;
            using (var logStream = File.OpenRead(logger.LogPath)) {
                var buffer = new byte[logStream.Length];
                logStream.Read(buffer, 0, buffer.Length);

                logText = logger.Encoder.GetString(buffer);
            }

            var logParts = logText.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var actLogEntry = logParts[logParts.Length - 1];

            Assert.AreEqual(expLogEntry, actLogEntry);
        }

        [TestMethod]
        public void BufferedLogEntry() {
            var expOutput = "This is a test";
            var streamLogger = new StreamLogger();
            var actOutput = string.Empty;

            streamLogger.LogUpdated += (output) => {
                actOutput = output;
            };

            ((ILogger)streamLogger).Log(expOutput);

            Assert.AreEqual($"{expOutput}{Environment.NewLine}", actOutput);
        }

        private delegate void OnLogUpdated(string logText);

        private class FileLogger : Scribe, IFileLogger {

            public string LogPath { get; }

            internal FileLogger(string path) {
                LogPath = Path.Combine(path, $"{DateTime.UtcNow.AsFileName("log")}");
                Open();
            }

            public void Log(string entry) {
                Write(entry);
            }

            public virtual void LogAssert(Func<bool> assert, Func<bool, string> entryResult) {
                Log(entryResult(assert()));
            }

            protected override byte[] Encode(string entry) {
                if (string.IsNullOrEmpty(entry)) {
                    return Encoder.GetBytes(Environment.NewLine);
                }

                return Encoder.GetBytes(DateTime.UtcNow.AsLogEntry($"{entry}{Environment.NewLine}"));
            }
            protected override Stream RequestStream() {
                var stream = File.OpenWrite(LogPath);
                stream.Seek(0, SeekOrigin.End);

                return stream;
            }

            private void Open() {
                using (var stream = File.Open(LogPath, FileMode.OpenOrCreate, FileAccess.Write)) {
                    var buffer = Encoding.UTF8.GetBytes($"{DateTime.UtcNow.AsLogHeader()}{Environment.NewLine}");
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
        private class StreamLogger : Scribe, ILogger {
            private StringBuilder stringBuilder = new StringBuilder();

            internal event OnLogUpdated LogUpdated;

            public void Log(string entry) {
                Write(entry);
            }

            public virtual void LogAssert(Func<bool> assert, Func<bool, string> entryResult) {
                Log(entryResult(assert()));
            }

            public override void Write(string entry) {
                NotifyWrite(Encode(entry));
            }

            protected override void NotifyWrite(byte[] buffer) {
                stringBuilder.Append(Encoder.GetChars(buffer));
                stringBuilder.AppendLine();

                LogUpdated?.Invoke(stringBuilder.ToString());
            }
        }
    }

}

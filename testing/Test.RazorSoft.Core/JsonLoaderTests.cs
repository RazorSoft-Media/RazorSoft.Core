// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SysEnviron = System.Environment;
//
using RazorSoft.Core.Data;
using RazorSoft.Core.Extensions;


namespace Testing.Dexter.Data.Repository {

    [TestClass]
    public class JsonLoaderTests {
        //  .\testing\data
        private const string SOURCE_PATH = @"..\..\..\..\data\";
        private const string SOURCE_FILE = "data.json";
        private const string DATA_PATH = @".\.Data";
        private const string ARRAY_FILE = "array.json";
        private const string OBJECT_FILE = "object.json";
        private const string ID = "B4BC8647LSLL";

        private static readonly DirectoryInfo DIRECTORY = new DirectoryInfo(SysEnviron.CurrentDirectory);

        private DataClass data;
        private string dataFile;


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

            Assert.IsTrue(File.Exists(Path.Combine(SOURCE_PATH, SOURCE_FILE)));
        }

        [TestInitialize]
        public void InitializeTest() {
            DIRECTORY.SetRoot(@".\.Data", out DirectoryInfo root);

            Assert.IsTrue(new FileInfo(Path.Combine(SOURCE_PATH, SOURCE_FILE)).Clone(root.FullName, out dataFile));

            Log($"Cloned {SOURCE_FILE} to ['{dataFile}']");
        }

        [TestCleanup]
        public void CleanupTest() {

        }
        #endregion	configuration


        [TestMethod]
        public void ReadData() {
            using (JsonLoader loader = new(dataFile)) {
                data = loader.Read<DataClass>();
            }

            Assert.IsNotNull(data);
            Assert.AreEqual(ID, data.Id);

            Log($"[{data.Id}] {data.Name} - {data.Number} {data.Boolean}");
        }

        [TestMethod]
        public void WriteData() {
            DataClass expData = new() {
                Id = "2E5C864HAVOC",
                Name = "Spencer",
                Number = 13,
                Boolean = false
            };

            using (JsonLoader loader = new(dataFile)) {
                loader.Write(expData);

                var fileInfo = new FileInfo(dataFile);
                Log($"{fileInfo.Name} [{fileInfo.Size()}]");
            }

            DataClass actData = null;
            using (JsonLoader loader = new(dataFile)) {
                actData = loader.Read<DataClass>();
            }

            Assert.IsNotNull(actData);
            Assert.AreEqual(expData.Id, actData.Id);

            Log($"[{actData.Id}] {actData.Name} - {actData.Number} {actData.Boolean}");
        }

        [TestMethod]
        public void WriteWithGuidConverter() {
            JsonSerializerOptions jsonOptions = new() {
                WriteIndented = true,
                Converters = { new GuidConverter(Log) }
            };

            Guid expGuid = Guid.NewGuid();

            //  populate 'data' object
            ReadData();

            //  populate test object
            GuidDataClass expData = new() {
                Id = data.Id,
                Name = data.Name,
                Number = data.Number,
                Boolean = data.Boolean,
                Guid = expGuid
            };

            using (JsonLoader loader = new(dataFile) { JsonOptions = jsonOptions }) {
                loader.Write(expData);

                var fileInfo = new FileInfo(dataFile);
                Log($"{fileInfo.Name} [{fileInfo.Size()}]");
            }

            GuidDataClass actData = null;
            using (JsonLoader loader = new(dataFile) { JsonOptions = jsonOptions }) {
                actData = loader.Read<GuidDataClass>();
            }

            Assert.IsNotNull(actData);
            Assert.AreEqual(expData.Guid, actData.Guid);

            Log($"[{actData.Id}] {actData.Name} - {actData.Number} {actData.Boolean}");
        }

        [TestMethod]
        public void LoaderTokenTypeIndicator() {
            /*
             *  Problem occurs when root element type is unknown - array or object.
             *  When root starts with '[' it is an array.
             *  When root starts with '{' it is a (dictionary) object
             * ***/
            DIRECTORY.SetRoot(@".\.Data", out DirectoryInfo root);
            new FileInfo(Path.Combine(SOURCE_PATH, OBJECT_FILE)).Clone(root.FullName, out dataFile);

            Log($"Cloned {OBJECT_FILE} to ['{dataFile}']");

            var expType = JsonLoader.TokenType.Object;
            var actType = JsonLoader.TokenType.Undef;

            using(JsonLoader loader = new(dataFile)) {
                actType = loader.DataType;
            }

            Assert.AreEqual(expType, actType);

            new FileInfo(Path.Combine(SOURCE_PATH, ARRAY_FILE)).Clone(root.FullName, out dataFile);

            Log($"Cloned {ARRAY_FILE} to ['{dataFile}']");

            expType = JsonLoader.TokenType.Array;
            actType = JsonLoader.TokenType.Undef;

            using (JsonLoader loader = new(dataFile)) {
                actType = loader.DataType;
            }

            Assert.AreEqual(expType, actType);
        }

        [TestMethod]
        public void StringEncoding() {
            var encoder = Encoding.UTF8;

            var expString = "25 25 65 35";
            Log($"input:  {expString}");

            var bytes = encoder.GetBytes(expString);
            var sBytes = string.Join("", bytes);
            Log($"bytes [{bytes.Length}]: {sBytes}");

            var actString = encoder.GetString(bytes);
            Log($"output: {actString}");

            Assert.AreEqual(expString, actString);
            Log("");

            expString = "engineer";
            Log($"input:  {expString}");
            bytes = encoder.GetBytes(expString);
            sBytes = string.Join("", bytes);
            Log($"bytes [{bytes.Length}]: {sBytes}");
            actString = encoder.GetString(bytes);
            Log($"output: {actString}");

            Assert.AreEqual(expString, actString);
        }


        #region 	utility methods
        private void Log(string entry) {
            TestContext.WriteLine($"[{TestContext.TestName}]:\t{entry}");
        }
        #endregion	utility methods


        #region     test classes
        private class DataClass {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public bool Boolean { get; set; }
        }

        private class GuidDataClass : DataClass {
            public Guid Guid { get; set; }
        }

        private class GuidConverter : JsonConverter<Guid> {
            private Action<string> log;

            internal GuidConverter(Action<string> logFunc) {
                log = logFunc;
            }

            public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                var input = reader.GetString();
                log($"read in -   {input}");

                var output = Guid.Parse(input);
                log($"read out -  {output}");

                return output;
            }

            public override void Write(Utf8JsonWriter writer, Guid guid, JsonSerializerOptions options) {
                var input = guid;
                log($"write in -  {input}");

                //  sidebar - this converter demonstrates Guid.Parse(...) does not require '-'
                var output = input.ToString()
                    .Replace("-", string.Empty)
                    .ToUpper();
                log($"write out - {output}");

                writer.WriteStringValue(output);
            }
        }
        #endregion  test classes
    }
}

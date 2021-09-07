// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
//
using RazorSoft.Core.Extensions;


namespace RazorSoft.Core.Data {

    /// <summary>
    /// 
    /// </summary>
    public abstract class JsonRepository : IObjectContext {
        #region		fields
        private const string ROOT_PATH = "ROOTPATH";

        private static readonly DirectoryInfo CURRENT_DIRECTORY = new DirectoryInfo(Environment.CurrentDirectory);
        private static readonly Dictionary<string, string> SETTINGS = new() {
            { ROOT_PATH, Path.Combine(@".\", CURRENT_DIRECTORY.Name) }
        };

        private readonly FileInfo file;
        #endregion	fields


        #region		properties
        private JsonSerializerOptions JsonOptions { get; init; } = new JsonSerializerOptions() {
            WriteIndented = true,
            Converters = {
                new GuidConverter()
            }
        };

        /// <summary>
        /// Access collection of all Json converters
        /// </summary>
        protected IList<JsonConverter> Converters => JsonOptions.Converters;

        /// <summary>
        /// 
        /// </summary>
        public static string RootPath {
            get => SETTINGS[ROOT_PATH];
            set => SETTINGS[ROOT_PATH] = Path.Combine(@".\", value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataPath { get; }
        /// <summary>
        /// 
        /// </summary>
        public string DataFile { get; }
        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// Instantiate base repository
        /// </summary>
        /// <param name="dataPath">expects relative to root '\.Data\'</param>
        /// <param name="converters"></param>
        protected JsonRepository(string dataPath, IEnumerable<JsonConverter> converters = null) {
            if (!Directory.Exists(RootPath)) {
                Directory.CreateDirectory(RootPath);
            }

            file = new(Path.Combine(RootPath, dataPath));
            //  check first if directory exists
            var parentDir = file.Directory;
            if (!parentDir.Exists) {
                parentDir.CreatePath();
            }

            DataPath = parentDir.AbbreviatePath(parentDir.Name);
            //  check if data file exists
            if (!file.Exists) {
                using (_ = File.Create(file.FullName)) ;
            }

            DataFile = file.Name;

            if (converters != null) {
                foreach (var c in converters) {
                    JsonOptions.Converters.Add(c);
                }
            }
        }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public IEnumerable<TData> All<TData>() {
            var iterator = Cache().GetEnumerator();

            while (iterator.MoveNext()) {
                if (iterator.Current is TData data) {
                    yield return data;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public TData Add<TData>(TData item) {
            Cache().Add(item);

            return item;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove<TData>(TData item) {
            var idx = Cache().IndexOf(item);
            bool has;

            if (has = idx >= 0) {
                Cache().RemoveAt(idx);
                has &= Cache().IndexOf(item) == -1;
            }

            return has;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose() {
            Cache().Clear();
        }

        #endregion	public methods & functions


        #region		non-public methods & functions
        /// <summary>
        /// Retrieves the internal records cache
        /// </summary>
        protected abstract IList Cache();
        /// <summary>
        /// Loads data from JSON file
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        protected void Load<TData>() where TData : class, new() {
            List<TData> data = new() { };

            using (var loader = new JsonLoader(file.FullName) { JsonOptions = JsonOptions }) {
                switch (loader.DataType) {
                    case JsonLoader.TokenType.Undef:
                        break;

                    case JsonLoader.TokenType.Array:
                        data.AddRange(loader.Read<List<TData>>());

                        break;
                    case JsonLoader.TokenType.Object:
                        data.Add(loader.Read<TData>());

                        break;
                }
            }

            OnLoad(data);
        }
        /// <summary>
        /// Occures when Json data has been loaded
        /// </summary>
        protected abstract void OnLoad(IEnumerable data);
        /// <summary>
        /// 
        /// </summary>
        protected void Commit<TData>() {
            using (var loader = new JsonLoader(file.FullName) { JsonOptions = JsonOptions }) {
                var data = Cache().Cast<TData>();

                switch (loader.DataType) {
                    case JsonLoader.TokenType.Undef:
                        //  new file won't have any bytes to figure this out
                        if (data.Count() > 1) {
                            loader.Write(data.ToList());
                        }
                        else {
                            loader.Write(data.Any() ? data.First() : default);
                        }

                        break;
                    case JsonLoader.TokenType.Array:
                        loader.Write(data.ToList());

                        break;
                    case JsonLoader.TokenType.Object:
                        loader.Write(data.Any() ? data.First() : default);

                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected void Update<TData>(TData data, Func<TData, bool> selector) {
            //var idx = repo
            //    .Where((d, i) => selector((TData)d))
            //    .Select((d, i) => d == default ? -1 : i)
            //    .First();

            //repo[idx] = data;
        }
        #endregion	non-public methods & functions


        #region     private classes
        private class GuidConverter : JsonConverter<Guid> {
            public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                var gString = reader.GetString();

                if (string.IsNullOrEmpty(gString)) {
                    return Guid.NewGuid();
                }

                return Guid.Parse(gString);
            }

            public override void Write(Utf8JsonWriter writer, Guid guid, JsonSerializerOptions options) {
                if (guid == Guid.Empty) {
                    guid = Guid.NewGuid();
                }

                var gString = guid.ToString()
                    .Replace("-", string.Empty)
                    .ToUpper();

                writer.WriteStringValue(gString);
            }
        }
        #endregion  private classes

    }
}

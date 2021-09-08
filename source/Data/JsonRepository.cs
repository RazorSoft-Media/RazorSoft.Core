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

            OnInitialized();
        }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable All() {
            var iterator = Cache().GetEnumerator();

            while (iterator.MoveNext()) {
                yield return iterator.Current;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public object Add(object item) {
            return Add(Cache(), item);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Commit() {
            Save();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(object item) {
            var idx = Cache().IndexOf(item);
            bool has;

            if (has = (idx >= 0)) {
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
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual object Add(IList list, object item) {
            list.Add(item);

            return item;
        }
        /// <summary>
        /// Retrieves the internal records cache
        /// </summary>
        protected abstract IList Cache();
        /// <summary>
        /// Occurs when the JsonRepository object is initialized
        /// </summary>
        protected abstract void OnInitialized();
        /// <summary>
        /// Occures when Json data has been loaded
        /// </summary>
        protected abstract void OnDataLoaded(IEnumerable data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        protected abstract void OnWrite(JsonLoader loader);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        protected abstract ICollection OnRead(JsonLoader loader);
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
        /// <summary>
        /// Loads data from JSON file
        /// </summary>
        protected void Load() {
            ArrayList data = new() { };

            using (var loader = new JsonLoader(file.FullName) { JsonOptions = JsonOptions }) {
                data.AddRange(OnRead(loader));
            }

            OnDataLoaded(data);
        }
        /// <summary>
        /// 
        /// </summary>
        protected void Save() {
            using (var loader = new JsonLoader(file.FullName) { JsonOptions = JsonOptions }) {
                OnWrite(loader);
            }
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

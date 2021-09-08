// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Text.Json;


namespace RazorSoft.Core.Data {

    /// <summary>
    /// 
    /// </summary>
    public sealed class JsonLoader : ISelect, IDisposable {
        #region		fields
        private delegate void Serializer(Utf8JsonWriter writer, object? dataObject, Type type, JsonSerializerOptions options = null);
        private delegate object Deserializer(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options = null);

        private const byte ARRAY = (byte)'[';
        private const byte OBJECT = (byte)'{';

        private readonly FileInfo dataFile;
        private readonly byte[] buffer;
        #endregion	fields


        #region		properties
        private static Serializer Serialize => JsonSerializer.Serialize;
        private static Deserializer Deserialize => JsonSerializer.Deserialize;

        /// <summary>
        /// Get or set a static DataRoot
        /// CAUTION: should be used when a specific root directory or path is expected not to change
        /// </summary>
        public static string DataRoot { get; set; } = string.Empty;
        /// <summary>
        /// JsonSerializerOptions
        /// </summary>
        public JsonSerializerOptions JsonOptions { get; init; } = null;
        /// <summary>
        /// 
        /// </summary>
        public TokenType DataType { get; private set; } = TokenType.Undef;
        /// <summary>
        /// Indicates if the Loader object is in Debug mode
        /// </summary>
        public bool Debug { get; init; } = false;
        /// <summary>
        /// If in Debug mode, get the JsonDocument object populated with the json data
        /// </summary>
        public object Json { get; private set; }
        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// Constructs a new JsonLoader object
        /// </summary>
        /// <param name="dataPath"></param>
        public JsonLoader(string dataPath) {
            dataPath = Path.Combine(DataRoot, dataPath);

            if (!File.Exists(dataPath)) {
                throw new FileNotFoundException($"could not find part of '{dataPath}'");
            }

            dataFile = new FileInfo(dataPath);
            buffer = LoadFile();
        }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// Read json source
        /// </summary>
        /// <typeparam name="TData">TYPE to which data is cast</typeparam>
        /// <returns>TDATA</returns>
        public TData Read<TData>() where TData : class, new() {
            TData data = default;

            if (buffer.Length == 0) {
                data = new TData();
            }
            else {
                var reader = new Utf8JsonReader(buffer);

                data = (TData)Deserialize(ref reader, typeof(TData), JsonOptions);

                if (Debug) {
                    reader = new Utf8JsonReader(buffer);

                    Json = Deserialize(ref reader, typeof(object), JsonOptions);
                }
            }

            return data;
        }
        /// <summary>
        /// Write data to json
        /// </summary>
        /// <typeparam name="TData">TYPE from which data is extracted</typeparam>
        /// <param name="data">data object to be written</param>
        public void Write<TData>(TData data) {
            using (var stream = File.OpenWrite(dataFile.FullName)) {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = true })) {
                    stream.SetLength(0);
                    Serialize(writer, data, typeof(TData), JsonOptions);
                }
            }
        }
        /// <summary>
        /// Dispose Loader object
        /// </summary>
        public void Dispose() {
            Array.Clear(buffer, 0, buffer.Length);
        }
        #endregion	public methods & functions


        #region		non-public methods & functions
        private byte[] LoadFile() {
            var buffer = default(byte[]);

            using (var stream = File.OpenRead(dataFile.FullName)) {
                buffer = new byte[stream.Length];
                stream.Read(buffer);
            }

            if (buffer[0] == ARRAY) {
                DataType = TokenType.Array;
            }
            else if (buffer[0] == OBJECT) {
                DataType = TokenType.Object;
            }
            else {
                throw new ArgumentException($"unknown object type [{(char)buffer[0]}] reading json");
            }

            return buffer;
        }
        #endregion	non-public methods & functions


        #region     nested classes
        /// <summary>
        /// Json object type
        /// </summary>
        public enum TokenType {
            /// <summary>
            /// Undefined type indicator
            /// </summary>
            Undef = -1,
            /// <summary>
            /// Array type indicator
            /// </summary>
            Array = 0,
            /// <summary>
            /// Object type indicator
            /// </summary>
            Object = 1
        }
        #endregion  nested classes
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISelect { }
}

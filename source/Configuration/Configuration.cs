// Copyright © 2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;


namespace RazorSoft.Core.Configuration {

    /// <summary>
    /// Core configuration abstraction
    /// </summary>
    public abstract class Configuration : IConfiguration {
        private static readonly SettingsContainer SETTINGS = new SettingsContainer();

        private static readonly string FILE_EXT = "config";
        private static readonly string FILE_NAME = "settings";
        private static readonly string ROOT = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);


        private string fullFileName => Path.Combine(FilePath, FileName);


        /// <summary>
        /// Get the key's value in the default format (byte[])
        /// </summary>
        /// <param name="key">specified key name</param>
        /// <returns>byte[]</returns>
        public byte[] this[string key] => SETTINGS[key];
        /// <summary>
        /// Returns a read-only collection of configuration keys
        /// </summary>
        public IReadOnlyCollection<string> Keys => SETTINGS.Keys;
        /// <summary>
        /// Assumes Environment.CurrentDirectory
        /// Get the default configuration file extension
        /// </summary>
        public string FileExt => $"{FILE_EXT}";
        /// <summary>
        /// Get the configuration file name
        /// </summary>
        public string FileName { get; } = $"{FILE_NAME}.{FILE_EXT}";
        /// <summary>
        /// Get the configuration file path
        /// </summary>
        public string FilePath { get; } = ROOT;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="path">optional: path if different than default</param>
        /// <param name="file">optional: file name if different than default</param>
        protected Configuration(string path = "", string file = "") {
            if (!string.IsNullOrEmpty(file)) {
                FileName = $"{file}.{FILE_EXT}";
            }
            if (!string.IsNullOrEmpty(path)) {
                FilePath = $"{Path.Combine(ROOT, path)}";

                if (!Directory.Exists(FilePath)) {
                    Directory.CreateDirectory(FilePath);
                }
            }
            if (!File.Exists(fullFileName)) {
                Save();
            } else {
                Load();
            }
        }


        /// <summary>
        /// Add a new configuration key by name with the specified value
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">value assigned to the key</param>
        void IConfiguration.Add<TValue>(string name, TValue value) {
            SETTINGS.Add(name, value);
        }
        /// <summary>
        /// Clears the entire configuration context of all settings
        /// </summary>
        public void Clear() {
            SETTINGS.Clear();
        }
        /// <summary>
        /// Gets the value assigned to the named key
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <returns>TValue</returns>
        TValue IConfiguration.Get<TValue>(string name) {
            return SETTINGS.GetValue<TValue>(name);
        }
        /// <summary>
        /// Sets the value assigned to the named key
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">value to be assigned</param>
        void IConfiguration.Set<TValue>(string name, TValue value) {
            SETTINGS.Set(name, value);
        }
        /// <summary>
        /// Load configuration file
        /// </summary>
        public void Load() {
            var items = ReadJson(fullFileName);

            if (items == null) {
                return;
            }

            foreach (var item in items) {
                var setting = Setting.FromBytes(item.Key, StringToByteArray(item.Value));
                SETTINGS.Add(setting);
            }
        }
        /// <summary>
        /// Save the configuration file
        /// </summary>
        /// <returns></returns>
        public bool Save() {
            using (var stream = File.OpenWrite(fullFileName)) {
                //  truncate file
                stream.SetLength(0);

                var options = new JsonWriterOptions {
                    Indented = true
                };

                using (var writer = new Utf8JsonWriter(stream, options)) {
                    writer.WriteStartObject();

                    foreach (var item in SETTINGS.Values) {
                        var value = BitConverter.ToString(item.value)
                            .Replace("-", string.Empty);

                        writer.WriteString(item.name, value);
                    }

                    writer.WriteEndObject();
                }
            }

            return File.Exists(FileName);
        }

        private static Dictionary<string, string> ReadJson(string fileName) {
            //  from: https://stackoverflow.com/a/55429664/210709
            var items = new Dictionary<string, string>();
            var jsonUtf8 = default(byte[]);

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                jsonUtf8 = new byte[stream.Length];
                stream.Read(jsonUtf8, 0, jsonUtf8.Length);
            }

            var reader = new Utf8JsonReader(jsonUtf8);

            string name = string.Empty;
            string value = string.Empty;

            while (reader.Read()) {
                switch (reader.TokenType) {
                    case JsonTokenType.PropertyName:
                        ReadStringToken(reader, out name);

                        break;
                    case JsonTokenType.String:
                        ReadStringToken(reader, out value);

                        break;
                }

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)) {
                    items.Add(name, value);

                    name = string.Empty;
                    value = string.Empty;
                }
            }

            return items;
        }
        private static void ReadStringToken(Utf8JsonReader reader, out string text) {
            text = reader.GetString();
        }
        private static byte[] StringToByteArray(string strBytes) {
            int length = strBytes.Length;
            byte[] bytes = new byte[length / 2];

            for (int i = 0; i < length; i += 2) {
                var hex = strBytes.Substring(i, 2);
                var index = i / 2;

                bytes[index] = Convert.ToByte(hex, 16);
            }

            return bytes;
        }
    }
}

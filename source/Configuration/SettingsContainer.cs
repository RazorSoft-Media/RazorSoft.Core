// Copyright © 2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System.Linq;
using System.Collections.Generic;
//
using RazorSoft.Core.Extensions;


namespace RazorSoft.Core.Configuration {

    /// <summary>
    /// Settings container
    /// </summary>
    public class SettingsContainer {

        private readonly Dictionary<string, Setting> settings;


        /// <summary>
        /// Get number of settings in container
        /// </summary>
        public int Count => settings.Count;
        /// <summary>
        /// Get read-only collection of key names
        /// </summary>
        public IReadOnlyCollection<string> Keys => settings.Keys;


        /// <summary>
        /// Internal
        /// Get read-only collection of key/value tuples with values serialized
        /// </summary>
        internal IReadOnlyCollection<(string name, byte[] value)> Values => settings.Values
            .Select(v => (v.Name, v.Value))
            .ToArray();
        /// <summary>
        /// Internal
        /// Get specified key's serialized value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal byte[] this[string name] => settings[name].Value;


        /// <summary>
        /// ctor
        /// </summary>
        public SettingsContainer() {
            settings = new Dictionary<string, Setting>();
        }


        /// <summary>
        /// Add name/value pair to the current settings container
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">key value</param>
        /// <returns>ISetting</returns>
        public ISetting Add<TValue>(string name, TValue value) {
            if (settings.ContainsKey(name)) {
                return settings[name];
            }

            var setting = Setting.Create(name, value);

            settings.Add(setting.Name, setting);

            return setting;
        }
        /// <summary>
        /// Clear settings container
        /// </summary>
        public void Clear() {
            settings.Clear();
        }
        /// <summary>
        /// Get setting value by key name
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <returns>TValue: key value</returns>
        public TValue GetValue<TValue>(string name) {
            return settings[name].Value.DecodeAs<TValue>();
        }
        /// <summary>
        /// Set setting value by key name
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">key value</param>
        /// <returns>ISetting</returns>
        public ISetting Set<TValue>(string name, TValue value) {
            if(!settings.TryGetValue(name, out Setting setting)) {
                return Add(name, value);
            }

            setting.SetValue(value);

            return setting;
        }
        /// <summary>
        /// Add a setting to the current settings container
        /// </summary>
        /// <param name="setting">setting</param>
        /// <returns>ISetting</returns>
        internal ISetting Add(Setting setting) {
            settings.Add(setting.Name, setting);

            return setting;
        }
    }
}

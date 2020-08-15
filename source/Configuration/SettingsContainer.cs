//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System.Linq;
using System.Collections.Generic;
//
using RazorSoft.Core.Extensions;


namespace RazorSoft.Core.Configuration {
    public class SettingsContainer {

        private readonly Dictionary<string, Setting> settings;

        public int Count => settings.Count;
        public IReadOnlyCollection<string> Keys => settings.Keys;

        internal IReadOnlyCollection<(string name, byte[] value)> Values => settings.Values
            .Select(v => (v.Name, v.Value))
            .ToArray();

        // TODO: error checking (???)
        internal byte[] this[string name] => settings[name].Value;

        public SettingsContainer() {
            settings = new Dictionary<string, Setting>();
        }

        public ISetting Add<TValue>(string name, TValue value) {
            if (settings.ContainsKey(name)) {
                return settings[name];
            }

            var setting = Setting.Create(name, value);

            settings.Add(setting.Name, setting);

            return setting;
        }

        public void Clear() {
            settings.Clear();
        }

        public TValue GetValue<TValue>(string name) {
            return settings[name].Value.DecodeAs<TValue>();
        }

        public ISetting Set<TValue>(string name, TValue value) {
            if(!settings.TryGetValue(name, out Setting setting)) {
                return Add(name, value);
            }

            setting.SetValue(value);

            return setting;
        }

        internal ISetting Add(Setting setting) {
            settings.Add(setting.Name, setting);

            return setting;
        }
    }
}

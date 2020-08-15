//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using RazorSoft.Core.Extensions;
using System;
using System.Linq;
using System.Text;


namespace RazorSoft.Core.Configuration {
    internal class Setting : ISetting {

        public string Name { get; }
        public byte[] Value { get; private set; }

        private Setting(string name, byte[] value) {
            Name = name;
            Value = value;
        }

        public static implicit operator Setting((string Name, int Value) setting) {
            return new Setting(setting.Name, setting.Value.Encode());
        }

        public static implicit operator Setting((string Name, string Value) setting) {
            return new Setting(setting.Name, setting.Value.Encode());
        }

        public static implicit operator Setting((string Name, DateTime Value) setting) {
            return new Setting(setting.Name, setting.Value.Encode());
        }

        public static Setting Create<TValue>(string name, TValue value) {
            return new Setting(name, value.Encode());
        }

        public static Setting FromBytes(string name, byte[] value) {
            return new Setting(name, value);
        }

        internal void SetValue<TValue>(TValue value) {
            Value = value.Encode();
        }
    }
}

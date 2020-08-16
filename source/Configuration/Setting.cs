//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
//
using RazorSoft.Core.Extensions;


namespace RazorSoft.Core.Configuration {
    
    /// <summary>
    /// Setting class
    /// </summary>
    internal class Setting : ISetting {


        /// <summary>
        /// Get the current setting name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Get the current setting value
        /// </summary>
        public byte[] Value { get; private set; }


        /// <summary>
        /// private ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private Setting(string name, byte[] value) {
            Name = name;
            Value = value;
        }


        /// <summary>
        /// Implicit cast from tuple to setting
        /// </summary>
        /// <param name="setting"></param>
        public static implicit operator Setting((string Name, int Value) setting) {
            return new Setting(setting.Name, setting.Value.Encode());
        }
        /// <summary>
        /// Implicit cast from tuple to setting
        /// </summary>
        /// <param name="setting"></param>
        public static implicit operator Setting((string Name, string Value) setting) {
            return new Setting(setting.Name, setting.Value.Encode());
        }
        /// <summary>
        /// Implicit cast from tuple to setting
        /// </summary>
        /// <param name="setting"></param>
        public static implicit operator Setting((string Name, DateTime Value) setting) {
            return new Setting(setting.Name, setting.Value.Encode());
        }
        /// <summary>
        /// Create setting from specified name and value
        /// </summary>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">key value</param>
        /// <returns>Setting</returns>
        public static Setting Create<TValue>(string name, TValue value) {
            return new Setting(name, value.Encode());
        }
        /// <summary>
        /// Create setting from specified name and value
        /// </summary>
        /// <param name="name">key name</param>
        /// <param name="value">key value</param>
        /// <returns>Setting</returns>
        public static Setting FromBytes(string name, byte[] value) {
            return new Setting(name, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        internal void SetValue<TValue>(TValue value) {
            Value = value.Encode();
        }
    }
}

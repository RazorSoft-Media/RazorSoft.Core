//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
using System.Collections.Generic;


namespace RazorSoft.Core.Configuration { 

    /// <summary>
    /// Configuration interface for publicly exposed functions and properties
    /// </summary>
    public interface IConfiguration {

        /// <summary>
        /// Get the key's value in the default format (byte[])
        /// </summary>
        /// <param name="key">specified key name</param>
        /// <returns>byte[]</returns>
        byte[] this[string key] { get; }
        /// <summary>
        /// Returns a read-only collection of configuration keys
        /// </summary>
        IReadOnlyCollection<string> Keys { get; }
        /// <summary>
        /// Get the default configuration file extension
        /// </summary>
        string FileExt { get; }
        /// <summary>
        /// Get the configuration file name
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Get the configuration file path
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Add a new configuration key by name with the specified value
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">value assigned to the key</param>
        void Add<TValue>(string name, TValue value);
        /// <summary>
        /// Clears the entire configuration context of all settings
        /// </summary>
        void Clear();
        /// <summary>
        /// Gets the value assigned to the named key
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <returns>TValue</returns>
        TValue Get<TValue>(string name);
        /// <summary>
        /// Sets the value assigned to the named key
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="name">key name</param>
        /// <param name="value">value to be assigned</param>
        void Set<TValue>(string name, TValue value);
        /// <summary>
        /// Load configuration file
        /// </summary>
        void Load();
        /// <summary>
        /// Save the configuration file
        /// </summary>
        /// <returns></returns>
        bool Save();
    }
}

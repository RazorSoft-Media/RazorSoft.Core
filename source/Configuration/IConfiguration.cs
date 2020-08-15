//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.Collections.Generic;


namespace RazorSoft.Core.Configuration { 
    public interface IConfiguration {

        byte[] this[string key] { get; }
        IReadOnlyCollection<string> Keys { get; }
        string FileExt { get; }
        string FileName { get; }
        string FilePath { get; }

        void Add<TValue>(string name, TValue value);
        void Clear();
        TValue Get<TValue>(string name);
        void Load();
        bool Save();
        void Set<TValue>(string name, TValue value);
    }
}

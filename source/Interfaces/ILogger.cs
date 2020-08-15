//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.Text;


namespace RazorSoft.Core.Logging {

    public interface ILogger {

        Encoding Encoder { get; }

        void Log(string entry);
        void LogAssert(Func<bool> assert, Func<bool, string> entryResult);
    }
}

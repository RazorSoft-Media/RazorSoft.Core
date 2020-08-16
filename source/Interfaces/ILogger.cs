//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
using System.Text;


namespace RazorSoft.Core.Logging {

    /// <summary>
    /// Logging interface
    /// </summary>
    public interface ILogger {


        /// <summary>
        /// Get logger encoding
        /// </summary>
        Encoding Encoder { get; }


        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="entry">entry</param>
        void Log(string entry);
        /// <summary>
        /// Assert boolean function and log result
        /// </summary>
        /// <param name="assert">boolean function</param>
        /// <param name="entryResult">log response function</param>
        void LogAssert(Func<bool> assert, Func<bool, string> entryResult);
    }
}

//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


namespace RazorSoft.Core.Logging {

    /// <summary>
    /// File logging interface
    /// </summary>
    public interface IFileLogger : ILogger {

        /// <summary>
        /// Get file log path
        /// </summary>
        string LogPath { get; }
    }
}

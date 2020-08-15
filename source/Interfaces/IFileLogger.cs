//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.Text;


namespace RazorSoft.Core.Logging {

    public interface IFileLogger : ILogger {

        string LogPath { get; }
    }
}

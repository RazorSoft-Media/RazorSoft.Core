//	* ********************************************************************
//	*  � 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;


namespace RazorSoft.Core {

    public interface ITransaction {
		Type Type { get; }
    }
}

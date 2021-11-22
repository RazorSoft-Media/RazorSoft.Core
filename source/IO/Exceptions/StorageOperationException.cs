// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;


namespace RazorSoft.Core.IO {

    /// <summary>
    /// 
    /// </summary>
    public class StorageOperationException : Exception {
        #region		fields

        #endregion	fields


        #region		properties

        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// 
        /// </summary>
        public StorageOperationException(string filePath) : base($"Storage file '{filePath}' not found or could not be created") {

        }
        #endregion	constructors & destructors


        #region		public methods & functions

        #endregion	public methods & functions


        #region		non-public methods & functions

        #endregion	non-public methods & functions
    }
}

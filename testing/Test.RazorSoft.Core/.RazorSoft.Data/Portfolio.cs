// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Testing.Dexter.Data {

    /// <summary>
    /// 
    /// </summary>
    public class Portfolio {
        #region		fields

        #endregion	fields


        #region		properties
        /// <summary>
        /// Get or set the Organization Id
        /// </summary>
        [JsonPropertyName("OrgId")]
        public string Organization { get; set; }
        /// <summary>
        /// Get or set the Portfolio Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Get or set the Portfolio Name
        /// </summary>
        [JsonPropertyName("Set")]
        public string Name { get; set; }
        /// <summary>
        /// Get or set the Portfolio Origin - Local | System
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// Get the list of available templates
        /// </summary>
        public List<string> Templates { get; set; }
        #endregion	properties


        #region		constructors & destructors

        #endregion	constructors & destructors


        #region		public methods & functions
        #endregion	public methods & functions


        #region		non-public methods & functions

        #endregion	non-public methods & functions
    }


}

// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Testing.Dexter.Data {

    /// <summary>
    /// 
    /// </summary>
    public class Organization {
        #region		fields

        #endregion	fields


        #region		properties
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        [JsonPropertyName("Index")]
        public List<string> PortfolioIndex { get; set; } = new ();
        [JsonIgnore]
        public ICollection<Portfolio> Catalog { get; set; } = new List<Portfolio>();
        #endregion	properties


        #region		constructors & destructors

        #endregion	constructors & destructors


        #region		public methods & functions
        public override bool Equals(object obj) {
            if (obj is not Organization other) {
                return false;
            }

            return other.Id == Id;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
        #endregion	public methods & functions


        #region		non-public methods & functions

        #endregion	non-public methods & functions
    }
}

// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Collections.Generic;
//
using RazorSoft.Core.Data;


namespace Testing.Data.Repositories {

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationRepository : RepositoryBase<Organization> {
        #region		fields

        #endregion	fields


        #region		properties
        internal Action<string> Logger { get; set; }
        #endregion	properties


        #region		constructors & destructors

        public OrganizationRepository() : base(new OrganizationContext()) {
        }
        #endregion	constructors & destructors


        #region		public methods & functions

        #endregion	public methods & functions


        #region		non-public methods & functions

        #endregion	non-public methods & functions


        #region     private class
        private class OrganizationContext : IObjectContext<Organization> {
            private readonly Dictionary<string, Organization> keyedCache = new();

            private OnValidateAdd<Organization> onValidate = (o) => true;

            public OnValidateAdd<Organization> ValidateAdd {
                get => onValidate;
                set => onValidate = value;
            }

            public string DataSource => "TestContext";

            public Organization Add(Organization item) {
                if(!keyedCache.TryGetValue(item.Key, out Organization organization)) {
                    keyedCache.Add(item.Key, organization = item);
                }

                return organization;
            }

            public IEnumerable<Organization> All() {
                return keyedCache.Values;
            }

            public void Commit() {
                //  hmmm ... we could just mock this, but implementation will be different for any data context
            }

            public bool Remove(Organization item) {
                return keyedCache.Remove(item.Key);
            }

            public bool Update(Organization item) {
                keyedCache[item.Key] = item;

                return true;
            }

            public bool Update(IEnumerable<Organization> itemList) {
                foreach(var item in itemList) {
                    Update(item);
                }

                return true;
            }

            public void Dispose() {
                keyedCache.Clear();
            }
        }
        #endregion  private class
    }
}

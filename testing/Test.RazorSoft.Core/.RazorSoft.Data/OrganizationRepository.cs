// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
//
using RazorSoft.Core.Data;
using RazorSoft.Core.Linq;
using RazorSoft.Core.Messaging;
using RazorSoft.Core.Extensions;
//
using Testing.Dexter.Services;


namespace Testing.Dexter.Data.Repositories {

    /// <summary>
    /// 
    /// </summary>
    public sealed class OrganizationRepository : RepositoryBase<Organization> {
        #region		fields 
        private const string DATA_FILE = "organizations.json";

        private static readonly Command<IOrganizationAPI> API = CommandRouter.Default.GetRoute<IOrganizationAPI>();
        #endregion	fields


        #region		properties

        #endregion	properties


        #region		constructors & destructors
        public OrganizationRepository() : base(new OrganizationContext()) {

        }

        #endregion	constructors & destructors


        #region		public methods & functions

        #endregion	public methods & functions


        #region		non-public methods & functions
        protected override bool OnAdd(Organization organization) {
            if (string.IsNullOrEmpty(organization.Key)) {
                CreateId(organization);
            }

            if (!CreateFolder(organization)) {
                //  log org folder creation exception
            }

            return Duplicate(organization);
        }

        private bool Duplicate(Organization organiation) {
            return !All().Any(o => organiation.Key == o.Key);
        }


        private static void CreateId(Organization organization) {
            API.Execute((api) => api.CreateId(organization));
        }

        private static bool CreateFolder(Organization organization) {
            var exists = false;

            API.Execute((api) => api.CreateFolder(organization, out exists));

            return exists;
        }
        #endregion	non-public methods & functions


        #region     private classes
        private class OrganizationContext : JsonRepository<Organization> {

            #region		properties

            #endregion	properties


            #region		constructors & destructors
            public OrganizationContext() : base(DATA_FILE) {
                Load();
            }
            #endregion	constructors & destructors


            #region		non-public methods & functions
            protected override void OnInitialized() {
                //  prevent loading when base JsonRepository is intialized
            }

            protected override ICollection OnRead(JsonLoader loader) {
                return loader.Read<List<Organization>>();
            }

            protected override void OnWrite(JsonLoader loader) {
                loader.Write(Cache().ToList<Organization>());
            }
            #endregion	non-public methods & functions
        }
        #endregion  private classes

    }
}

// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
//
using RazorSoft.Core.Data;
using RazorSoft.Core.Linq;
//
using Testing.Dexter.Data;
using Testing.Dexter.Data.Repositories;


namespace Testing.Dexter.Services {

    public sealed class OrganizationService : IOrganizationAPI {
        #region     fields
        private const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NUMBERS = "0123456789";
        private const string SYMBOLS = "&@";

        //private static readonly Settings SETTINGS = Settings.Default;
        private static readonly Dictionary<string, Organization> CACHE = new();
        #endregion  fields


        #region		properties
        public string Name => IOrganizationAPI.NAME;

        #endregion	properties


        #region		constructors & destructors
        public OrganizationService() {

        }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <param name="exists">TRUE if created; otherwise FALSE</param>
        /// <returns>string: organization folder path</returns>
        public string CreateFolder(Organization organization, out bool exists) {
            if (string.IsNullOrEmpty(organization.Id)) {
                CreateId(organization);
            }

            var directory = @".\.Data";

            if (Directory.Exists(directory)) {
                directory = Path.Combine(directory, organization.Id);

                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
            }

            exists = Directory.Exists(directory);

            return directory;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <returns>string: organization folder path</returns>
        public string CreateFolder(string organizationId) {
            if (string.IsNullOrEmpty(organizationId)) {
                return default;
            }

            var directory = @".\.Data";

            if (Directory.Exists(directory)) {
                directory = Path.Combine(directory, organizationId);

                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
            }

            return directory;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <returns>string: organization ID</returns>
        public string CreateId(Organization organization) {
            var oid = new string(Guid.NewGuid()
                .ToString()
                .TakeLast(12)
                .ToArray())
                .ToUpper();
            StringBuilder abr = new();

            foreach (var n in organization.Name.Split(" ", StringSplitOptions.RemoveEmptyEntries)) {
                var i = 0;
                var l = false;

                if (n.IndexOfAny(SYMBOLS.ToCharArray()) != -1) {
                    continue;
                }

                while (i < n.Length && !(l = LETTERS.Contains(n[i]) || NUMBERS.Contains(n[i]))) {
                    ++i;
                }

                if (l) {
                    abr.Append(n[0]);
                }
                else {
                    abr.Append(LETTERS[new Random().Next(LETTERS.Length - 1)]);
                }

            }

            while (abr.Length < 4) {
                abr.Append(LETTERS[new Random().Next(LETTERS.Length - 1)]);
            }

            var ot = oid
                .Take(oid.Length - abr.Length)
                .Append(abr.ToString())
                .ToArray();

            return organization.Id = new string(ot).ToUpper();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <returns>TRUE if exists; otherwise FALSE</returns>
        public bool FolderExists(Organization organization) {
            var directory = Path.Combine(@".\.Data", organization.Id);

            return Directory.Exists(directory);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <param name="exists">TRUE if exists: otherwise FALSE</param>
        /// <returns>string: organization folder path</returns>
        public string GetFolder(Organization organization, out bool exists) {
            return (exists = FolderExists(organization)) ?
                Path.Combine(@".\.Data", organization.Id) :
                string.Empty;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organizationId">target organization by ID</param>
        /// <param name="exists">TRUE if exists: otherwise FALSE</param>
        /// <returns>string: organization folder path</returns>
        public string GetFolder(string organizationId, out bool exists) {
            var organization = Get(organizationId);

            return (exists = FolderExists(organization)) ?
                Path.Combine(@".\.Data", organization.Id) :
                string.Empty;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>TRUE if success; otherwise FALSE</returns>
        public bool Commit() {
            var result = false;
            var orgList = CACHE.Values.ToList();

            using (IRepository<Organization> repo = new OrganizationRepository()) {
                result = repo.Update(orgList);
            }

            foreach(var o in orgList) {
                CACHE.Remove(o.Id);
            }

            return result;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <returns>Organization</returns>
        public Organization Get(string organizationId) {
            if (!CACHE.TryGetValue(organizationId, out Organization organization)) {
                using (IRepository<Organization> repo = new OrganizationRepository()) {
                    organization = repo.All()
                        .Where(o => string.Equals(organizationId, o.Id))
                        .FirstOrDefault();
                }

                CACHE.Add(organization.Id, organization);
            }

            return organization;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <returns>TRUE if success; otherwise FALSE</returns>
        public bool Save(Organization organization) {
            if (CACHE.ContainsKey(organization.Id)) {
                CACHE[organization.Id] = organization;
            }
            else {
                CACHE.Add(organization.Id, organization);
            }

            return CACHE.ContainsKey(organization.Id);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="organization">target organization</param>
        /// <returns>delegate: PortfolioIdentifier</returns>
        public PortfolioIdentifier CreatePortfolio(Organization organization) {
            var handle = DateTime.UtcNow.ToBinary();
            var identifier = BitConverter.ToString(BitConverter.GetBytes(handle))
                .Replace("-", string.Empty);

            organization.PortfolioIndex.Add(identifier);

            return () => (organizationId: organization.Id, portfolioId: identifier);
        }

        #endregion	public methods & functions


        #region		non-public methods & functions

        #endregion	non-public methods & functions
    }
}

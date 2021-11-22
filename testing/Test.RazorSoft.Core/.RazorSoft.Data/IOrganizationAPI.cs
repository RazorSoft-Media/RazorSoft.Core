// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;


namespace Testing.Data.Services {

    public delegate (string organizationId, string portfolioId) PortfolioIdentifier();
    public delegate (PortfolioIdentifier identifier, string name) PortfolioInfo();

    /// <summary>
    /// Routed Organization Service API
    /// </summary>
    public interface IOrganizationAPI {
        public static readonly string NAME = "Organizations";

        /// <summary>
        /// Creates organization data directory
        /// </summary>
        string CreateFolder(Organization organization, out bool exists);
        /// <summary>
        /// Creates organization data directory
        /// </summary>
        string CreateFolder(string organizationId);
        /// <summary>
        /// Creates organization ID
        /// </summary>
        string CreateId(Organization organization);
        /// <summary>
        /// Determines if organization data directory exists
        /// </summary>
        bool FolderExists(Organization organization);
        /// <summary>
        /// Gets organization data directory if it exists
        /// </summary>
        string GetFolder(Organization organization, out bool exists);
        /// <summary>
        /// Gets organization data directory if it exists
        /// </summary>
        string GetFolder(string organizationId, out bool exists);
        /// <summary>
        /// Creates an organization portfolio placeholder
        /// </summary>
        PortfolioIdentifier CreatePortfolio(Organization organization);
    }
}
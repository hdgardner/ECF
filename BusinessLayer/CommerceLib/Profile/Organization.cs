using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Profile.Search;
using Mediachase.MetaDataPlus;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Contains information about the Organization.
    /// </summary>
    public partial class Organization : Principal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Organization"/> class.
        /// </summary>
        public Organization() : base(ProfileContext.Current.OrganizationMetaClass)
        {
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public override void SetParent(object parent)
        {
            Principal principal = (Principal)parent;
            //this.PrincipalId = principal.PrincipalId;
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (this.ObjectState == MetaObjectState.Added)
            {
                this.Type = "ORG";
            }

            //if (ApplicationId == Guid.Empty)
            //    ApplicationId = ProfileConfiguration.ApplicationId;

            //if (PrincipalId == Guid.Empty)
            //    PrincipalId = ProfileContext.Current.UserId;

            base.AcceptChanges();
        }

        /// <summary>
        /// Loads the by principal id.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <returns></returns>
        public static Organization LoadByPrincipalId(Guid principalId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_PrincipalId", ProfileContext.Current.OrganizationMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", ProfileConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PrincipalId", principalId, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            MetaStorageCollectionBase<Organization> orgs = LoadSearchResults(searchGuid);
            if (orgs.Count > 0)
                return orgs[0];

            // Load results and return them back
            return null;
        }

        /// <summary>
        /// Loads the by id.
        /// </summary>
        /// <param name="organizationId">The organization id.</param>
        /// <returns></returns>
        public static Organization LoadById(int organizationId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_OrganizationId", ProfileContext.Current.OrganizationMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", ProfileConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("OrganizationId", organizationId, DataParameterType.Int));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            MetaStorageCollectionBase<Organization> orgs = LoadSearchResults(searchGuid);
            if (orgs.Count > 0)
                return orgs[0];

            // Load results and return them back
            return null;
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<Organization> Search(ProfileSearch search, out int totalRecords)
        {
            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            totalRecords = search.Search(searchGuid);

            // Load results and return them back
            return LoadSearchResults(searchGuid);
        }

        #region Private Data Help Methods
        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="SearchGuid">The search GUID.</param>
        /// <returns></returns>
        private static MetaStorageCollectionBase<Organization> LoadSearchResults(Guid SearchGuid)
        {
            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}", ProfileContext.Current.OrganizationMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SearchSetId", SearchGuid, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataResult result = DataService.LoadDataSet(cmd);

            MetaStorageCollectionBase<Organization> orgs = new MetaStorageCollectionBase<Organization>();

            PopulateCollection<Organization>(ProfileContext.Current.OrganizationClassInfo, orgs, result.DataSet);
            return orgs;
        }
        #endregion

    }
}

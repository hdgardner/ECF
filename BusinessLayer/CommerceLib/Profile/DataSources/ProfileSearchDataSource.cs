using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile.Search;
using System.Web.Security;
using Mediachase.Data.Provider;
using System.Data;

namespace Mediachase.Commerce.Profile.DataSources
{
    /// <summary>
    /// Allows specifying source of data
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// The current membership provider will be used as a source.
        /// </summary>
        MembershipProvider,
        /// <summary>
        /// The profile accounts table will be used as a source.
        /// </summary>
        ProfileSystem,

		/// <summary>
		/// The profile accounts table with customer address table will be used as a source.
		/// </summary>
		ExtendedProfileSystem
    }

    /// <summary>
    /// Implements operations for and represents the profile search data source. (Inherits <see cref="DataSourceControl"/>.)
    /// </summary>
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[ParseChildren(true)]
	public class ProfileSearchDataSource : DataSourceControl
	{
		#region IDataSource Members

		/// <summary>
		/// Gets or sets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		[Category("Data")]
		[Browsable(true)]
		[Bindable(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ProfileSearchParameters Parameters
		{
			get
			{
				return ((ProfileSearchDataSourceView)this.GetView(String.Empty)).Parameters;
			}
		}

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        [Category("Data")]
        [Browsable(true)]
        [Bindable(false)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public SourceType SourceType
        {
            get
            {
                return ((ProfileSearchDataSourceView)this.GetView(String.Empty)).SourceType;
            }
            set
            {
                ((ProfileSearchDataSourceView)this.GetView(String.Empty)).SourceType = value;
            }
        }

		/// <summary>
		/// Gets or sets the options.
		/// </summary>
		/// <value>The options.</value>
		[Category("Data")]
		[Browsable(true)]
		[Bindable(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ProfileSearchOptions Options
		{
			get
			{
				return ((ProfileSearchDataSourceView)this.GetView(String.Empty)).Options;
			}
		}

		private ProfileSearchDataSourceView view = null;
		/// <summary>
		/// Gets the named data source view associated with the data source control.
		/// </summary>
		/// <param name="viewName">The name of the view to retrieve.</param>
		/// <returns>
		/// Returns the named <see cref="T:System.Web.UI.DataSourceView"/> associated with the <see cref="T:System.Web.UI.IDataSource"/>.
		/// </returns>
		protected override DataSourceView GetView(string viewName)
		{
			if (null == view)
			{
				view = new ProfileSearchDataSourceView(this, String.Empty);
			}
			return view;
		}

		/// <summary>
		/// Gets a collection of names representing the list of view objects associated with the <see cref="T:System.Web.UI.IDataSource"/> interface.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.ICollection"/> that contains the names of the views associated with the <see cref="T:System.Web.UI.IDataSource"/>.
		/// </returns>
		protected override ICollection GetViewNames()
		{
			ArrayList al = new ArrayList(1);
			al.Add(ProfileSearchDataSourceView.DefaultViewName);
			return al as ICollection;
		}

		#endregion

		/// <summary>
		/// Implements operations for and represents the profile search data source view. (Inherits <see cref="DataSourceView"/>.)
		/// </summary>
		public class ProfileSearchDataSourceView : DataSourceView
		{
            /// <summary>
            /// Represents the default view name.
            /// </summary>
			public static string DefaultViewName = "AccountsView";

			/// <summary>
			/// Initializes a new instance of the <see cref="ProfileSearchDataSourceView"/> class.
			/// </summary>
			/// <param name="owner">The owner.</param>
			/// <param name="name">The name.</param>
			public ProfileSearchDataSourceView(IDataSource owner, string name)
				: base(owner, DefaultViewName)
			{
			}

			ProfileSearchParameters _Parameters = new ProfileSearchParameters();

			/// <summary>
			/// Gets or sets the parameters.
			/// </summary>
			/// <value>The parameters.</value>
			public ProfileSearchParameters Parameters
			{
				get { return _Parameters; }
				set { _Parameters = value; }
			}

			ProfileSearchOptions _Options = new ProfileSearchOptions();
			/// <summary>
			/// Gets or sets the options.
			/// </summary>
			/// <value>The options.</value>
			public ProfileSearchOptions Options
			{
				get { return _Options; }
				set { _Options = value; }
			}

            SourceType _SourceType = SourceType.ProfileSystem;

            /// <summary>
            /// Gets or sets the type of the source.
            /// </summary>
            /// <value>The type of the source.</value>
            public SourceType SourceType
            {
                get { return _SourceType; }
                set { _SourceType = value; }
            }

			/// <summary>
			/// Gets a list of data from the underlying data storage.
			/// </summary>
			/// <param name="arguments">A <see cref="T:System.Web.UI.DataSourceSelectArguments"/> that is used to request operations on the data beyond basic data retrieval.</param>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerable"/> list of data from the underlying data storage.
			/// </returns>
			protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
			{
				Options.RecordsToRetrieve = arguments.MaximumRows;
				Options.StartingRecord = arguments.StartRowIndex;

				if (!String.IsNullOrEmpty(arguments.SortExpression))
					Parameters.OrderByClause = arguments.SortExpression;

				int totalRecords = 0;

                if (SourceType == SourceType.ProfileSystem)
                {
					Account[] accounts = ProfileContext.Current.FindAccounts(Parameters, Options, out totalRecords);
                    if (accounts != null)
                        arguments.TotalRowCount = totalRecords;
                    return accounts;
                }
                else
				if (SourceType == SourceType.ExtendedProfileSystem)
				{
					Account[] accounts = ProfileContext.Current.FindAccounts(Parameters, Options, out totalRecords);
					if (accounts != null)
					{ 
						arguments.TotalRowCount = totalRecords;
						if (accounts.Length > 0)
						{
							DataTable dt = new DataTable();
							dt.Columns.Add(new DataColumn("PrincipalId", typeof(string)));
							dt.Columns.Add(new DataColumn("Name", typeof(string)));
							dt.Columns.Add(new DataColumn("State", typeof(string)));
							dt.Columns.Add(new DataColumn("Description", typeof(string)));
							dt.Columns.Add(new DataColumn("ProviderKey", typeof(string)));
							dt.Columns.Add(new DataColumn("OrganizationId", typeof(int)));
							dt.Columns.Add(new DataColumn("CustomerGroup", typeof(string)));
							dt.Columns.Add(new DataColumn("Created", typeof(DateTime)));
							dt.Columns.Add(new DataColumn("Email", typeof(string)));
							foreach (Account ac in accounts)
							{
								DataRow dr = dt.NewRow();
								dr["PrincipalId"] = ac.PrincipalId.ToString();
								dr["Name"] = ac.Name;
								switch (ac.State)
								{
									case 1:
										dr["State"] = "Pending";
										break;
									case 2:
										dr["State"] = "Active";
										break;
									case 3:
										dr["State"] = "Suspended";
										break;
								}
								dr["Description"] = ac.Description;
								dr["ProviderKey"] = ac.ProviderKey;
								dr["OrganizationId"] = ac.OrganizationId;
								dr["CustomerGroup"] = ac.CustomerGroup;
								dr["Created"] = ac.Created;
								if (ac.Addresses != null && ac.Addresses.Count > 0)
								{
									dr["Email"] = ac.Addresses[0].Email;
								}
								//else if(!String.IsNullOrEmpty(ac.ProviderKey))
								//{
								//    MembershipUser user = Membership.GetUser(new Guid(ac.ProviderKey));
								//    if (user != null)
								//        dr["Email"] = user.Email;
								//}
								dt.Rows.Add(dr);
							}
							dt.AcceptChanges();
							return dt.DefaultView;
						}
					}
					return null;
				}
				else
                {
                    int pageIndex = 0;
                    pageIndex = Options.StartingRecord / Options.RecordsToRetrieve;
                    MembershipUserCollection accounts = Membership.GetAllUsers(pageIndex, Options.RecordsToRetrieve, out totalRecords);

                    if (accounts != null)
                        arguments.TotalRowCount = totalRecords;

                    return accounts;
                }
			}

			/// <summary>
			/// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports retrieving the total number of data rows, instead of the data.
			/// </summary>
			/// <value></value>
			/// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
			public override bool CanRetrieveTotalRowCount
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports paging through the data retrieved by the <see cref="M:System.Web.UI.DataSourceView.ExecuteSelect(System.Web.UI.DataSourceSelectArguments)"/> method.
			/// </summary>
			/// <value></value>
			/// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
			public override bool CanPage
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports the <see cref="M:System.Web.UI.DataSourceView.ExecuteDelete(System.Collections.IDictionary,System.Collections.IDictionary)"/> operation.
			/// </summary>
			/// <value></value>
			/// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
			public override bool CanDelete
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports the <see cref="M:System.Web.UI.DataSourceView.ExecuteInsert(System.Collections.IDictionary)"/> operation.
			/// </summary>
			/// <value></value>
			/// <returns>true if the operation is supported; otherwise, false. The base class implementation returns false.</returns>
			public override bool CanInsert
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Gets a value indicating whether the <see cref="T:System.Web.UI.DataSourceView"/> object associated with the current <see cref="T:System.Web.UI.DataSourceControl"/> object supports the <see cref="M:System.Web.UI.DataSourceView.ExecuteUpdate(System.Collections.IDictionary,System.Collections.IDictionary,System.Collections.IDictionary)"/> operation.
			/// </summary>
			/// <value></value>
			/// <returns>true if the operation is supported; otherwise, false. The default implementation returns false.</returns>
			public override bool CanUpdate
			{
				get
				{
					return false;
				}
			}
		}
	}
}

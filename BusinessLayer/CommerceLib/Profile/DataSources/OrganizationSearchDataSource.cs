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

namespace Mediachase.Commerce.Profile.DataSources
{
    /// <summary>
    /// Implements operations for the organization search data source. (Inherits <see cref="DataSourceControl"/>.)
    /// </summary>
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[ParseChildren(true)]
	public class OrganizationSearchDataSource : DataSourceControl
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
				return ((OrganizationSearchDataSourceView)this.GetView(String.Empty)).Parameters;
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
				return ((OrganizationSearchDataSourceView)this.GetView(String.Empty)).Options;
			}
		}

		private OrganizationSearchDataSourceView view = null;
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
				view = new OrganizationSearchDataSourceView(this, String.Empty);
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
			al.Add(OrganizationSearchDataSourceView.DefaultViewName);
			return al as ICollection;
		}

		#endregion

		/// <summary>
		/// Implements operations for the organization search data source view. (Inherits <see cref="DataSourceView"/>.)
		/// </summary>
		public class OrganizationSearchDataSourceView : DataSourceView
		{
            /// <summary>
            /// Represents the default view name.
            /// </summary>
			public static string DefaultViewName = "OrganizationsView";

			/// <summary>
			/// Initializes a new instance of the <see cref="OrganizationSearchDataSourceView"/> class.
			/// </summary>
			/// <param name="owner">The owner.</param>
			/// <param name="name">The name.</param>
			public OrganizationSearchDataSourceView(IDataSource owner, string name)
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

				Organization[] organizations = ProfileContext.Current.FindOrganizations(Parameters, Options, out totalRecords);

				if (organizations != null)
					arguments.TotalRowCount = totalRecords;

				return organizations;
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

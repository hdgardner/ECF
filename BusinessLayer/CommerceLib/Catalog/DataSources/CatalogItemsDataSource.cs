using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Commerce.Catalog.DataSources
{
    /// <summary>
    /// Represents catalog items data source to data-bound controls.
    /// </summary>
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[ParseChildren(true)]
	public class CatalogItemsDataSource : DataSourceControl
	{
        private ItemSearchParameters _Parameters = new ItemSearchParameters();

		/// <summary>
		/// Gets or sets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		[Category("Data")]
		[Browsable(true)]
		[Bindable(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
        public ItemSearchParameters Parameters
		{
			get
			{
                return _Parameters;
			}
		}

        CatalogNodeResponseGroup _ResponseGroup = new CatalogNodeResponseGroup();

		/// <summary>
		/// Gets or sets the response group.
		/// </summary>
		/// <value>The response group.</value>
		[Category("Data")]
		[Browsable(true)]
		[Bindable(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public CatalogNodeResponseGroup ResponseGroup
		{
			get
			{
				return _ResponseGroup;
			}
		}

		#region DataSource
		/// <summary>
        /// Gets the named data source view associated with the data source control.
        /// </summary>
        /// <param name="viewName">The name of the <see cref="T:System.Web.UI.DataSourceView"/> to retrieve. In data source controls that support only one view, such as <see cref="T:System.Web.UI.WebControls.SqlDataSource"/>, this parameter is ignored.</param>
        /// <returns>
        /// Returns the named <see cref="T:System.Web.UI.DataSourceView"/> associated with the <see cref="T:System.Web.UI.DataSourceControl"/>.
        /// </returns>
		protected override DataSourceView GetView(string viewName)
		{
            CatalogNodesDataSourceView view = new CatalogNodesDataSourceView(this, viewName);
            view.Parameters = this.Parameters;
            view.ResponseGroup = this.ResponseGroup;
            return view;
		}

        /// <summary>
        /// Gets a collection of names, representing the list of <see cref="T:System.Web.UI.DataSourceView"/> objects associated with the <see cref="T:System.Web.UI.DataSourceControl"/> control.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.ICollection"/> that contains the names of the <see cref="T:System.Web.UI.DataSourceView"/> objects associated with the <see cref="T:System.Web.UI.DataSourceControl"/>.
        /// </returns>
		protected override System.Collections.ICollection GetViewNames()
		{
			ArrayList al = new ArrayList(2);
			al.Add(CatalogNodesDataSourceView.DefaultViewName);
            al.Add(CatalogNodesDataSourceView.CatalogNodesTreeViewName);
			return al as ICollection;
		}
		#endregion

		/// <summary>
		/// Represents the catalog nodes data source view.
		/// </summary>
		public class CatalogNodesDataSourceView : DataSourceView
		{
            /// <summary>
            /// Represents the default catalog node view name
            /// </summary>
			public static string DefaultViewName = "CatalogNodesView";
            /// <summary>
            /// Represents the view where first row is [...] allowing to navigate to parent folder
            /// </summary>
            public static string CatalogNodesTreeViewName = "CatalogNodesTreeView";

			/// <summary>
			/// Initializes a new instance of the <see cref="CatalogNodesDataSourceView"/> class.
			/// </summary>
			/// <param name="owner">The owner.</param>
			/// <param name="name">The name.</param>
			public CatalogNodesDataSourceView(IDataSource owner, string name)
				: base(owner, name)
			{
			}

            private ItemSearchParameters _Parameters = new ItemSearchParameters();

			/// <summary>
			/// Gets or sets the _Parameters.
			/// </summary>
			/// <value>The Parameters.</value>
            public ItemSearchParameters Parameters
			{
				get { return _Parameters; }
				set { _Parameters = value; }
			}

			CatalogNodeResponseGroup _ResponseGroup = new CatalogNodeResponseGroup();

			/// <summary>
			/// Gets or sets the response group.
			/// </summary>
			/// <value>The response group.</value>
			public CatalogNodeResponseGroup ResponseGroup
			{
				get { return _ResponseGroup; }
				set { _ResponseGroup = value; }
			}

			/// <summary>
			/// Gets a list of data from the underlying data storage.
			/// </summary>
			protected override System.Collections.IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
			{
				if (String.Compare(base.Name, CatalogNodesTreeViewName, true) == 0 && this.Parameters.ParentNodeId > 0)
				{
					// a minor hack for the case when a record with [...] is added later
					if (arguments.StartRowIndex < arguments.MaximumRows)
					{
						Parameters.RecordsToRetrieve = arguments.MaximumRows - 1;
						Parameters.StartingRecord = arguments.StartRowIndex + 1;
					}
				}
				else
				{
					// normal logic
					Parameters.RecordsToRetrieve = arguments.MaximumRows;
					Parameters.StartingRecord = arguments.StartRowIndex + 1;
				}

				Parameters.ReturnTotalCount = arguments.RetrieveTotalRowCount;

				if (!String.IsNullOrEmpty(arguments.SortExpression))
					Parameters.OrderByClause = arguments.SortExpression;

				DataTable table = CatalogContext.Current.FindCatalogItemsTable(this.Parameters, this.ResponseGroup);

				if (table == null)
					throw new InvalidOperationException("No data loaded from data source.");

				if (this.Parameters.ReturnTotalCount)
				{
					if (table.Rows.Count > 0)
					{
						object obj = table.Rows[0][table.Columns.Count - 1];
						if (obj != null && obj != DBNull.Value && obj.GetType() == typeof(int))
							// get rowcount from the last column
							arguments.TotalRowCount = Int32.Parse(obj.ToString());
						else
							throw new InvalidOperationException("Couldn't retrieve TotalRowCount.");
					}
					else
						arguments.TotalRowCount = 0;
				}

                // need to create a copy so the cached version is not modified
                DataTable newTable = table.Copy();

				// add a column that indicates whether checkbox in a row will be enabled/disabled (they are enabled by default)
				newTable.Columns.Add(new DataColumn("CheckboxEnabled", typeof(Boolean)));

                if (newTable.Rows.Count >= 0)
                {
                    if (String.Compare(base.Name, CatalogNodesTreeViewName, true) == 0 && this.Parameters.ParentNodeId > 0)
                    {
                        if (newTable.Rows.Count == 0)
                            arguments.TotalRowCount = 1;
                        else
                            if (this.Parameters.ReturnTotalCount)
                                arguments.TotalRowCount++;

                        // add additional row at the top. Don't add row if parent node is a Catalog or if it's page > 1
						if (arguments.StartRowIndex < arguments.MaximumRows)
						{
							DataRow row = newTable.NewRow();
							row["ID"] = this.Parameters.ParentNodeId;
							row["Name"] = "[..]";
							row["Type"] = "LevelUp";
							row["CheckboxEnabled"] = false;
							newTable.Rows.InsertAt(row, 0);
						}
                    }
                }

                return newTable.Rows;
			}

			#region Properties
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
			#endregion
		}
	}
}
